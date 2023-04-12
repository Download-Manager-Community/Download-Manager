using DownloadManager.NativeMethods;
using System.Diagnostics;
using System.Reflection;
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
        YouTubeDownloadForm ytDownload = new YouTubeDownloadForm();
        public static int downloadsAmount = 0;
        public static string downloadsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).Replace("Desktop", "Downloads") + "\\";
        public static List<DownloadProgress> downloadsList = new List<DownloadProgress>();
        CurrentDownloads currentDownloads = new CurrentDownloads();

        public DownloadForm()
        {
            _instance = this;

            Settings.Default.Upgrade();

            Logging.Log("Downloads folder: " + downloadsFolder, Color.White);
            if (Settings.Default.downloadHistory == null)
            {
                Logging.Log("Download History is null. Performing first time setup.", Color.Orange);
                Settings.Default.downloadHistory = new System.Collections.Specialized.StringCollection { };
            }
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

            if (Settings.Default.showDownloadToolWindow)
            {
                currentDownloads.Show();
                button2.BackColor = Color.FromArgb(64, 64, 64);
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
            else if (textBox2.Text == "")
            {
                DarkMessageBox msg = new DarkMessageBox("Please enter a valid download location.\nIf this is not filled out with your user download location automatically, go to Settings > Default downloads location to change your default download location.", "Enter a download location", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msg.ShowDialog();
                return;
            }
            else if (textBox1.Text.StartsWith("http://youtube.com/") || textBox1.Text.StartsWith("https://youtube.com/"))
            {
                DarkMessageBox msg = new DarkMessageBox("You cannot download YouTube videos or playlists from here.\nUse the YouTube downloader instead.", "Enter a valid URL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                msg.ShowDialog();
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
            // Calculate MD5 of a file
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
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            // Report a bug
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "https://github.com/Soniczac7/Download-Manager/issues/new?assignees=&labels=bug&template=bug_report.md&title=",
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
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            // Open youtube download form
            ytDownload.Show();
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
                button2.BackColor = Color.FromArgb(64, 64, 64);

                currentDownloads.Show();
            }
            else
            {
                button2.BackColor = Color.FromArgb(34, 34, 34);

                currentDownloads.Hide();
            }
        }
    }
}
