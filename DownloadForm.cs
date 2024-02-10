﻿using DownloadManager.NativeMethods;
using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using YoutubeExplode;
using YoutubeExplode.Common;
using static DownloadManager.DownloadProgress;

namespace DownloadManager
{
    public partial class DownloadForm : Form
    {
        public static readonly string installationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
        public static DownloadForm _instance;
        public Logging logging = new Logging();
        ApplicationSettings settings = new ApplicationSettings();
        Server browserIntercept = new Server();
        public static int downloadsAmount = 0;
        public static bool ytDownloading = false;
        public static string downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Replace("Desktop", "Downloads") + "\\";
        public static List<DownloadProgress> downloadsList = new List<DownloadProgress>();
        public static CurrentDownloads currentDownloads = new CurrentDownloads();
        YoutubeExplode.Playlists.Playlist? listMetadata = null;

        public static bool firstShown = true;

        public DownloadForm(bool hasUpgraded)
        {
            _instance = this;

            Logging.Log("Downloads folder: " + downloadsFolder, Color.White);
            if (Settings.Default.downloadHistory == null)
            {
                Logging.Log("Download History is null. Performing first time setup.", Color.Orange);
                Settings.Default.downloadHistory = new System.Collections.Specialized.StringCollection { };
            }

            if (Settings.Default.currentDownloadsHiddenColumns == null || Settings.Default.currentDownloadsShownColumns == null)
            {
                Logging.Log("One or more current downloads column settings are null. Performing first time setuo", Color.Orange);
                SetupColumnPrefs();
            }

            Settings.Default.Save();
            InitializeComponent();

            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);

            comboBox1.SelectedIndex = 0;
            browserIntercept.StartServer();
            if (Settings.Default.downloadHistory != null)
            {
                foreach (var item in Settings.Default.downloadHistory)
                {
                    Application.DoEvents();
                    textBox1.Items.Add(item);
                }
            }
            textBox2.Text = Settings.Default.defaultDownload;

            if (hasUpgraded)
            {
                DarkMessageBox msg = new DarkMessageBox("You have upgraded your version of Download Manager so your settings file has been automatically upgraded.\nIf you have deleted the configuration file or this is the first time startup you can ignore this message.\nIf you have not upgraded Download Manager and you are getting this message, please create a bug report.\nIf you would like to create a bug report, click Yes.\nIf you would like to continue without making a bug report, click No.", "Download Manager - Settings Upgraded", MessageBoxButtons.YesNo, MessageBoxIcon.Information, false);
                if (msg.ShowDialog() == DialogResult.Yes)
                {
                    Process.Start(new ProcessStartInfo("https://github.com/Download-Manager-Community/Download-Manager/issues/new?assignees=&labels=bug&projects=&template=bug_report.yml") { UseShellExecute = true });
                }
            }
        }

        public void SetupColumnPrefs()
        {
            Settings.Default.currentDownloadsHiddenColumns = new System.Collections.Specialized.StringCollection
                {
                    // Default hidden column ids go here
                    "3",
                    "4"
                };
            Settings.Default.currentDownloadsShownColumns = new System.Collections.Specialized.StringCollection
                {
                    // Default shown column ids go here
                    "0",
                    "1",
                    "2",
                    "5"
                };
            Settings.Default.Save();
            Logging.Log("Column Preferences Reset!", Color.Orange);
        }

        private async void DownloadForm_Shown(object sender, EventArgs e)
        {
            if (firstShown)
            {
                if (Settings.Default.trayOnStartup)
                {
                    this.Close();
                    await currentDownloads.HideAfterFirstShow();
                    return;
                }

                firstShown = false;
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            bool ignoreNcActivate = false;

            switch (m.Msg)
            {
                case User32.WM_NCACTIVATE:
                    if (m.WParam == IntPtr.Zero)
                    {
                        if (ignoreNcActivate)
                        {
                            ignoreNcActivate = false;
                        }
                        else
                        {
                            User32.SendMessageW(this.Handle, User32.WM_NCACTIVATE, new IntPtr(1), IntPtr.Zero);
                        }
                    }
                    break;
                case User32.WM_ACTIVATEAPP:
                    if (m.WParam == IntPtr.Zero)
                    {
                        User32.PostMessageW(this.Handle, User32.WM_NCACTIVATE, IntPtr.Zero, IntPtr.Zero);
                        foreach (CurrentDownloads f in this.OwnedForms.OfType<CurrentDownloads>())
                        {
                            f.ForceActiveBar = false;
                            User32.PostMessageW(f.Handle, User32.WM_NCACTIVATE, IntPtr.Zero, IntPtr.Zero);
                        }
                        ignoreNcActivate = true;
                    }
                    else if (m.WParam == new IntPtr(1))
                    {
                        User32.SendMessageW(this.Handle, User32.WM_NCACTIVATE, new IntPtr(1), IntPtr.Zero);
                        foreach (CurrentDownloads f in this.OwnedForms.OfType<CurrentDownloads>())
                        {
                            f.ForceActiveBar = true;
                            User32.SendMessageW(f.Handle, User32.WM_NCACTIVATE, new IntPtr(1), IntPtr.Zero);
                        }
                    }
                    break;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (Settings.Default.showDownloadToolWindow)
            {
                button2.BackColor = Color.FromArgb(32, 32, 32);

                // Middle of the form
                int middleY = this.Height / 2;

                // To set the currentDownloads window y position to the middle of the parent form:
                currentDownloads.Location = new Point(this.Location.X + this.Width + 5, this.Location.Y + middleY - (currentDownloads.Height / 2));

                currentDownloads.Show(this);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox1.Text.Contains(" "))
            {
                DarkMessageBox msg = new DarkMessageBox("Please enter a valid URL.", "Enter a valid URL", MessageBoxButtons.OK, MessageBoxIcon.Error);
                msg.ShowDialog();
                return;
            }

            if (textBox2.Text == "")
            {
                DarkMessageBox msg = new DarkMessageBox("Please enter a valid download location.\nIf this is not filled out with your user download location automatically, go to Settings > Default downloads location to change your default download location.", "Enter a download location", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msg.ShowDialog();
                return;
            }

            if (textBox1.Text.StartsWith("http://youtube.com/") || textBox1.Text.StartsWith("https://youtube.com/") || textBox1.Text.StartsWith("http://www.youtube.com/") || textBox1.Text.StartsWith("https://www.youtube.com/"))
            {
                YouTubeDownloadForm ytDownload = new YouTubeDownloadForm(textBox1.Text, textBox2.Text);
                ytDownload.Show();
                textBox1.Text = "";
                return;
            }

            if (!textBox1.Items.Contains(textBox1.Text))
            {
                textBox1.Items.Add(textBox1.Text);
                Settings.Default.downloadHistory.Add(textBox1.Text);
                Settings.Default.Save();
            }
            DownloadProgress downloadProgress = new DownloadProgress(textBox1.Text, textBox2.Text, DownloadType.Normal, null, textBox3.Text, comboBox1.SelectedIndex);
            downloadProgress.Show();

            downloadsList.Add(downloadProgress);

            currentDownloads.RefreshList();

            textBox1.Text = "";
            textBox3.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog.SelectedPath + @"\";
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            label3.Text = "Downloading " + downloadsAmount + " files...";
            if (downloadsAmount > 0)
            {
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.MarqueeAnim = true;
            }
            else
            {
                progressBar1.Style = ProgressBarStyle.Blocks;
                progressBar1.MarqueeAnim = false;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            // Debug log
            logging.Show();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            // Settings
            settings.Show();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            // Calculate checksum of a file
            DialogResult result = openFileDialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                try
                {
                    HashCalculator hashCalc = new HashCalculator(openFileDialog.FileName);
                    hashCalc.Show();
                }
                catch (Exception ex)
                {
                    DarkMessageBox msg = new DarkMessageBox(ex.Message + Environment.NewLine + ex.StackTrace, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                    msg.ShowDialog();
                }
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // Open Form
            this.Show();
            try
            {
                currentDownloads.Show(this);
            }
            catch { }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Report a bug
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "https://github.com/Soniczac7/Download-Manager/issues/new?assignees=&labels=bug&template=bug_report.yml&title=",
                Arguments = "",
                UseShellExecute = true,
                RedirectStandardOutput = false,
                RedirectStandardError = false,
                RedirectStandardInput = false
            };
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            // Exit
            Environment.Exit(0);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "https://microsoftedge.microsoft.com/addons/detail/download-manager/facopbimneimllhcabghncloejfeficd?hl=en-GB",
                Arguments = "",
                UseShellExecute = true,
                RedirectStandardError = false,
                RedirectStandardInput = false,
                RedirectStandardOutput = false
            };
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
        }

        private void DownloadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            currentDownloads.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Settings.Default.showDownloadToolWindow = !Settings.Default.showDownloadToolWindow;
            Settings.Default.Save();

            if (Settings.Default.showDownloadToolWindow)
            {
                button2.BackColor = Color.FromArgb(32, 32, 32);

                currentDownloads.Show(this);
            }
            else
            {
                button2.BackColor = Color.Transparent;

                currentDownloads.Hide();
            }
        }

        private void DownloadForm_Move(object sender, EventArgs e)
        {
            // Middle of the form
            int middleY = this.Height / 2;

            // To set the currentDownloads window y position to the middle of the parent form:
            currentDownloads.Location = new Point(this.Location.X + this.Width + 5, this.Location.Y + middleY - (currentDownloads.Height / 2));
        }

        private void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.Show();
                try
                {
                    currentDownloads.Show(this);
                }
                catch { }
            }
        }

        private void textBox1_TextUpdate(object sender, EventArgs e)
        {
            // Check if the URL is a YouTube URL
            Regex regex = new Regex(@"^(https?\:\/\/)?((www\.)?youtube\.com|youtu\.be)\/.+$");

            string url = textBox1.Text;
            bool isPlaylist = false;

            videoDownloadOptions.Show();

            if (regex.IsMatch(url))
            {
                if (videoDownloadTypeComboBox.SelectedIndex == -1)
                    button4.Enabled = false;

                if (url.Contains("playlist?"))
                {
                    isPlaylist = true;
                }

                Thread thread = new Thread(delegate ()
                {
                    YoutubeClient client = new YoutubeClient();
                    YoutubeExplode.Videos.Video? vidMetadata = null;

                    Thread.CurrentThread.IsBackground = true;

                    try
                    {
                        if (!isPlaylist)
                        {
                            vidMetadata = client.Videos.GetAsync(YoutubeExplode.Videos.VideoId.Parse(url)).Result;
                        }
                        else
                        {
                            listMetadata = client.Playlists.GetAsync(YoutubeExplode.Playlists.PlaylistId.Parse(url)).Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex.Message, Color.Red);
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            videoErrorBox.Show();
                            videoDownloadOptions.Hide();
                        }));

                        return;
                    }

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        if (vidMetadata != null)
                        {
                            videoDateLabel.Text = "Date: ";
                            playlistViewButton.Visible = false;
                            videoDuration.Visible = true;
                            videoDurationLabel.Visible = true;
                            videoTitle.Text = vidMetadata.Title;
                            videoChannel.Text = vidMetadata.Author.ChannelTitle;
                            videoDate.Text = vidMetadata.UploadDate.LocalDateTime.ToString();
                            videoDuration.Text = vidMetadata.Duration.ToString();

                            // Get Thumbnail
                            Thumbnail thumbnail = vidMetadata.Thumbnails.GetWithHighestResolution();

                            try
                            {
                                videoThumb.CoreWebView2.Navigate(thumbnail.Url);
                            }
                            catch (Exception ex)
                            {
                                this.Focus();

                                Point loc = PointToScreen(videoThumb.Location);
                                videoThumbErrorTip.Show("An error occurred while displaying the thumbnail.\n" + ex.Message, this, loc.X, loc.Y);
                                /*DarkMessageBox msg = new DarkMessageBox("An error occurred while displaying the thumbnail.\n" + ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                                msg.ShowDialog();*/
                            }
                        }
                        else if (listMetadata != null)
                        {
                            videoTitle.Text = listMetadata.Title;
                            if (listMetadata.Author == null)
                            {
                                videoChannel.Text = "Unknown";
                            }
                            else
                            {
                                videoChannel.Text = listMetadata.Author.ChannelTitle;
                            }
                            videoDate.Text = listMetadata.Id;
                            videoDateLabel.Text = "Id: ";
                            videoDuration.Visible = false;
                            videoDurationLabel.Visible = false;
                            playlistViewButton.Visible = true;


                            // Get Thumbnail
                            Thumbnail thumbnail = listMetadata.Thumbnails.GetWithHighestResolution();

                            try
                            {
                                videoThumb.CoreWebView2.Navigate(thumbnail.Url);
                            }
                            catch (Exception ex)
                            {
                                this.Focus();

                                Point loc = PointToScreen(videoThumb.Location);
                                videoThumbErrorTip.Show("An error occurred while displaying the thumbnail.\n" + ex.Message, this, loc.X, loc.Y);
                                /*DarkMessageBox msg = new DarkMessageBox("An error occurred while displaying the thumbnail.\n" + ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                                msg.ShowDialog();*/
                            }
                        }
                        else
                        {
                            Logging.Log("An error occurred while fetching the YouTube metadata.", Color.Red);
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                videoErrorBox.Show();
                                videoDownloadOptions.Hide();
                            }));
                        }
                    }));
                });
                thread.Start();
            }
            else
            {
                videoDownloadOptions.BringToFront();
                videoDownloadOptions.Hide();
                videoErrorBox.Hide();
            }
        }

        private void playlistViewButton_Click(object sender, EventArgs e)
        {
            // Open playlist viewer
            YouTubePlaylistViewer playlistViewer = new YouTubePlaylistViewer(listMetadata);
            playlistViewer.Show();
        }

        private void videoDownloadTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Check if there is an item selected
            if (videoDownloadTypeComboBox.SelectedIndex == -1)
            {
                // There is no item selected so the download button should be disabled
                button4.Enabled = false;
            }
            else
            {
                // There is an item selected so the download button can be enabled
                button4.Enabled = true;
            }
        }
    }
}
