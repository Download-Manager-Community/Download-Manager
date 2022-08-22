using System.Runtime.InteropServices;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Videos.Streams;

namespace DownloadManager
{
    public partial class YouTubeDownloadForm : Form
    {
        #region DLL Import
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }
        #endregion

        YoutubeClient client = new YoutubeClient();
        YoutubeExplode.Videos.Video? metadata = null;
        readonly string[] blankHtml = new string[]
        {
            "<html>",
            "<head>",
            "<style>\nbody\n{\nmargin: 0px;\nbackground-color:rgb(80, 80, 80);\n}\n</style>",
            "</head>",
            "<body>",
            "</body>",
            "</html>"
        };

        public YouTubeDownloadForm()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 2;
            comboBox2.SelectedIndex = 0;
            textBox2.Text = Settings1.Default.defaultDownload;
        }

        private void YouTubeDownloadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox1.Text == " ")
            {
                DarkMessageBox msg = new DarkMessageBox("YouTube URL is empty.\nEnter a valid YouTube URL.", "Download Manager", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                msg.ShowDialog();
                return;
            }

            if (textBox2.Text.EndsWith(@"\") == false)
            {
                textBox2.Text = textBox2.Text + @"\";
            }

            textBox1.Enabled = false;
            textBox2.Enabled = false;
            comboBox1.Enabled = false;
            comboBox2.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;

            progressBar1.Visible = true;

            Thread thread = new Thread(delegate ()
            {
                Thread.CurrentThread.IsBackground = true;

                metadata = null;
                try
                {
                    metadata = client.Videos.GetAsync(YoutubeExplode.Videos.VideoId.Parse(textBox1.Text)).Result;
                }
                catch (Exception ex)
                {
                    DarkMessageBox msg = new DarkMessageBox(ex.Message, "Error fetching YouTube metadata", MessageBoxButtons.OK, MessageBoxIcon.Error, false);
                    msg.ShowDialog();

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Visible = false;
                        textBox1.Enabled = true;
                        textBox2.Enabled = true;
                        comboBox1.Enabled = true;
                        button1.Enabled = true;
                        button2.Enabled = false;
                        button3.Enabled = true;
                        textBox1.Text = "";
                        textBox2.Text = Settings1.Default.defaultDownload;
                        label2.Text = "Video Title";
                        label3.Text = "Channel Name";
                        label4.Text = "Date Posted";
                        label5.Text = "Duration";
                        try
                        {
                            File.WriteAllLines(System.IO.Path.GetTempPath() + "thumbnail.html", blankHtml);

                            webView1.CoreWebView2.Navigate(System.IO.Path.GetTempPath() + "thumbnail.html");
                        }
                        catch (Exception ex)
                        {
                            DarkMessageBox msg = new DarkMessageBox("An error occurred while displaying the thumbnail.\n" + ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                            msg.ShowDialog();
                        }
                    }));

                    return;
                }

                this.Invoke(new MethodInvoker(delegate ()
                {
                    label2.Text = metadata.Title;
                    label3.Text = metadata.Author.ChannelTitle;
                    label4.Text = metadata.UploadDate.LocalDateTime.ToString();
                    label5.Text = metadata.Duration.ToString();

                    // Get Thumbnail
                    Thumbnail thumbnail = metadata.Thumbnails.GetWithHighestResolution();
                    string url = thumbnail.Url;

                    /*string fileName = System.IO.Path.GetTempPath() + "maxresdefault.png";

                    WebClient client = new WebClient();
                    client.DownloadFileAsync(new Uri(url), fileName);*/

                    string[] html = new string[]
                    {
                        "<html>",
                        "<head>",
                        "<style>\nbody\n{\nmargin: 0px;\n}\n</style>",
                        "</head>",
                        "<body>",
                        "<img src='" + thumbnail.Url + "' width='100%' height='100%'>",
                        "</body>",
                        "</html>"
                    };

                    try
                    {
                        File.WriteAllLines(System.IO.Path.GetTempPath() + "thumbnail.html", html);

                        webView1.CoreWebView2.Navigate(System.IO.Path.GetTempPath() + "thumbnail.html");
                    }
                    catch (Exception ex)
                    {
                        DarkMessageBox msg = new DarkMessageBox("An error occurred while displaying the thumbnail.\n" + ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                        msg.ShowDialog();
                    }

                    progressBar1.Visible = false;
                    comboBox1.Enabled = true;
                    comboBox2.Enabled = true;
                    button1.Enabled = true;
                    button2.Enabled = true;
                    button3.Enabled = true;
                    textBox1.Enabled = true;
                    textBox2.Enabled = true;
                }));
            });
            thread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Download Audio/Video/Both

            if (comboBox1.SelectedIndex == 0)
            {
                // Audio
                progressBar1.Visible = true;
                Thread thread = new Thread(async delegate ()
                {
                    var streamManifest = client.Videos.Streams.GetManifestAsync(metadata.Id);

                    var streamInfo = streamManifest.Result.GetAudioOnlyStreams().GetWithHighestBitrate();

                    await client.Videos.Streams.DownloadAsync(streamInfo, System.IO.Path.GetTempPath() + "temp.mp3");

                    string newName = metadata.Title;
                    int charsReplaced = 0;

                    foreach (char badChar in Path.GetInvalidFileNameChars())
                    {
                        if (newName.Contains(badChar))
                        {
                            newName.Replace(badChar, '_');
                            charsReplaced += 1;
                        }
                    }

                    if (charsReplaced >= 1)
                    {
                        DarkMessageBox msgerr = new DarkMessageBox("Video title contains " + charsReplaced + " illegal characters.\nThe characters will be replaced with '_' in the file name.", "YouTube Download Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, true);
                        msgerr.ShowDialog();
                    }

                    try
                    {
                        File.Move(System.IO.Path.GetTempPath() + "temp.mp3", textBox2.Text + metadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + ".mp4");
                    }
                    catch (Exception ex)
                    {
                        DarkMessageBox msgerr = new DarkMessageBox("Error while writing file:\n" + ex.Message + Environment.NewLine + ex.StackTrace, "Download Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, false);
                        DialogResult result = msgerr.ShowDialog();

                        if (result == DialogResult.Retry)
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                button2_Click(sender, e);
                            }));

                            return;
                        }
                        else
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                this.Hide();
                                progressBar1.Visible = false;
                                textBox1.Enabled = true;
                                textBox2.Enabled = true;
                                comboBox1.Enabled = true;
                                comboBox2.Enabled = true;
                                button1.Enabled = true;
                                button2.Enabled = false;
                                button3.Enabled = true;
                                textBox1.Text = "";
                                textBox2.Text = Settings1.Default.defaultDownload;
                                label2.Text = "Video Title";
                                label3.Text = "Channel Name";
                                label4.Text = "Date Posted";
                                label5.Text = "Duration";
                                try
                                {
                                    File.WriteAllLines(System.IO.Path.GetTempPath() + "thumbnail.html", blankHtml);

                                    webView1.CoreWebView2.Navigate(System.IO.Path.GetTempPath() + "thumbnail.html");
                                }
                                catch (Exception ex)
                                {
                                    DarkMessageBox msg = new DarkMessageBox("An error occurred while displaying the thumbnail.\n" + ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                                    msg.ShowDialog();
                                }
                            }));

                            return;
                        }
                    }

                    DarkMessageBox msg = new DarkMessageBox("Finished Downloading:\nTitle: " + metadata.Title + "\nId: " + metadata.Id, "Download Manager", MessageBoxButtons.OK, MessageBoxIcon.Information, true);
                    msg.ShowDialog();

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Visible = false;
                        textBox1.Enabled = true;
                        textBox2.Enabled = true;
                        comboBox1.Enabled = true;
                        comboBox2.Enabled = true;
                        button1.Enabled = true;
                        button2.Enabled = false;
                        button3.Enabled = true;
                        textBox1.Text = "";
                        textBox2.Text = Settings1.Default.defaultDownload;
                        label2.Text = "Video Title";
                        label3.Text = "Channel Name";
                        label4.Text = "Date Posted";
                        label5.Text = "Duration";
                        try
                        {
                            File.WriteAllLines(System.IO.Path.GetTempPath() + "thumbnail.html", blankHtml);

                            webView1.CoreWebView2.Navigate(System.IO.Path.GetTempPath() + "thumbnail.html");
                        }
                        catch (Exception ex)
                        {
                            DarkMessageBox msg = new DarkMessageBox("An error occurred while displaying the thumbnail.\n" + ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                            msg.ShowDialog();
                        }
                    }));
                });
                thread.Start();
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                // Video
                progressBar1.Visible = true;
                Thread thread = new Thread(async delegate ()
                {
                    var streamManifest = client.Videos.Streams.GetManifestAsync(metadata.Id);

                    var streamInfo = streamManifest.Result.GetVideoStreams().GetWithHighestVideoQuality();

                    await client.Videos.Streams.DownloadAsync(streamInfo, System.IO.Path.GetTempPath() + "temp.mp4");

                    string newName = metadata.Title;
                    int charsReplaced = 0;

                    foreach (char badChar in Path.GetInvalidFileNameChars())
                    {
                        if (newName.Contains(badChar))
                        {
                            newName.Replace(badChar, '_');
                            charsReplaced += 1;
                        }
                    }

                    if (charsReplaced >= 1)
                    {
                        DarkMessageBox msgerr = new DarkMessageBox("Video title contains " + charsReplaced + " illegal characters.\nThe characters will be replaced with '_' in the file name.", "YouTube Download Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, true);
                        msgerr.ShowDialog();
                    }

                    try
                    {
                        File.Move(System.IO.Path.GetTempPath() + "temp.mp4", textBox2.Text + metadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + ".mp4");
                    }
                    catch (Exception ex)
                    {
                        DarkMessageBox msgerr = new DarkMessageBox("Error while writing file:\n" + ex.Message + Environment.NewLine + ex.StackTrace, "Download Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, false);
                        DialogResult result = msgerr.ShowDialog();

                        if (result == DialogResult.Retry)
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                button2_Click(sender, e);
                            }));

                            return;
                        }
                        else
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                this.Hide();
                                progressBar1.Visible = false;
                                textBox1.Enabled = true;
                                textBox2.Enabled = true;
                                comboBox1.Enabled = true;
                                comboBox2.Enabled = true;
                                button1.Enabled = true;
                                button2.Enabled = false;
                                button3.Enabled = true;
                                textBox1.Text = "";
                                textBox2.Text = Settings1.Default.defaultDownload;
                                label2.Text = "Video Title";
                                label3.Text = "Channel Name";
                                label4.Text = "Date Posted";
                                label5.Text = "Duration";
                                try
                                {
                                    File.WriteAllLines(System.IO.Path.GetTempPath() + "thumbnail.html", blankHtml);

                                    webView1.CoreWebView2.Navigate(System.IO.Path.GetTempPath() + "thumbnail.html");
                                }
                                catch (Exception ex)
                                {
                                    DarkMessageBox msg = new DarkMessageBox("An error occurred while displaying the thumbnail.\n" + ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                                    msg.ShowDialog();
                                }
                            }));

                            return;
                        }
                    }

                    DarkMessageBox msg = new DarkMessageBox("Finished Downloading:\nTitle: " + metadata.Title + "\nId: " + metadata.Id, "Download Manager", MessageBoxButtons.OK, MessageBoxIcon.Information, true);
                    msg.ShowDialog();

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Visible = false;
                        textBox1.Enabled = true;
                        textBox2.Enabled = true;
                        comboBox1.Enabled = true;
                        comboBox2.Enabled = true;
                        button1.Enabled = true;
                        button2.Enabled = false;
                        button3.Enabled = true;
                        textBox1.Text = "";
                        textBox2.Text = Settings1.Default.defaultDownload;
                        label2.Text = "Video Title";
                        label3.Text = "Channel Name";
                        label4.Text = "Date Posted";
                        label5.Text = "Duration";
                        try
                        {
                            File.WriteAllLines(System.IO.Path.GetTempPath() + "thumbnail.html", blankHtml);

                            webView1.CoreWebView2.Navigate(System.IO.Path.GetTempPath() + "thumbnail.html");
                        }
                        catch (Exception ex)
                        {
                            DarkMessageBox msg = new DarkMessageBox("An error occurred while displaying the thumbnail.\n" + ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                            msg.ShowDialog();
                        }
                    }));
                });
                thread.Start();
            }
            else if (comboBox1.SelectedIndex == 2)
            {
                // Audio & Video
                progressBar1.Visible = true;
                Thread thread = new Thread(async delegate ()
                {
                    var streamManifest = client.Videos.Streams.GetManifestAsync(metadata.Id);

                    var streamInfo = streamManifest.Result.GetMuxedStreams().GetWithHighestVideoQuality();

                    await client.Videos.Streams.DownloadAsync(streamInfo, System.IO.Path.GetTempPath() + "temp.mp4");

                    string newName = metadata.Title;
                    int charsReplaced = 0;

                    foreach (char badChar in Path.GetInvalidFileNameChars())
                    {
                        if (newName.Contains(badChar))
                        {
                            newName.Replace(badChar, '_');
                            charsReplaced += 1;
                        }
                    }

                    if (charsReplaced >= 1)
                    {
                        DarkMessageBox msgerr = new DarkMessageBox("Video title contains " + charsReplaced + " illegal characters.\nThe characters will be replaced with '_' in the file name.", "YouTube Download Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, true);
                        msgerr.ShowDialog();
                    }

                    try
                    {
                        File.Move(System.IO.Path.GetTempPath() + "temp.mp4", textBox2.Text + metadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + ".mp4");
                    }
                    catch (Exception ex)
                    {
                        DarkMessageBox msgerr = new DarkMessageBox("Error while writing file:\n" + ex.Message + Environment.NewLine + ex.StackTrace, "Download Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, false);
                        DialogResult result = msgerr.ShowDialog();

                        if (result == DialogResult.Retry)
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                button2_Click(sender, e);
                            }));

                            return;
                        }
                        else
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                this.Hide();
                                progressBar1.Visible = false;
                                textBox1.Enabled = true;
                                textBox2.Enabled = true;
                                comboBox1.Enabled = true;
                                comboBox2.Enabled = true;
                                button1.Enabled = true;
                                button2.Enabled = false;
                                button3.Enabled = true;
                                textBox1.Text = "";
                                textBox2.Text = Settings1.Default.defaultDownload;
                                label2.Text = "Video Title";
                                label3.Text = "Channel Name";
                                label4.Text = "Date Posted";
                                label5.Text = "Duration";
                                try
                                {
                                    File.WriteAllLines(System.IO.Path.GetTempPath() + "thumbnail.html", blankHtml);

                                    webView1.CoreWebView2.Navigate(System.IO.Path.GetTempPath() + "thumbnail.html");
                                }
                                catch (Exception ex)
                                {
                                    DarkMessageBox msg = new DarkMessageBox("An error occurred while displaying the thumbnail.\n" + ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                                    msg.ShowDialog();
                                }
                            }));

                            return;
                        }
                    }

                    DarkMessageBox msg = new DarkMessageBox("Finished Downloading:\nTitle: " + metadata.Title + "\nId: " + metadata.Id, "Download Manager", MessageBoxButtons.OK, MessageBoxIcon.Information, true);
                    msg.ShowDialog();

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Visible = false;
                        textBox1.Enabled = true;
                        textBox2.Enabled = true;
                        comboBox1.Enabled = true;
                        comboBox2.Enabled = true;
                        button1.Enabled = true;
                        button2.Enabled = false;
                        button3.Enabled = true;
                        textBox1.Text = "";
                        textBox2.Text = Settings1.Default.defaultDownload;
                        label2.Text = "Video Title";
                        label3.Text = "Channel Name";
                        label4.Text = "Date Posted";
                        label5.Text = "Duration";
                        try
                        {
                            File.WriteAllLines(System.IO.Path.GetTempPath() + "thumbnail.html", blankHtml);

                            webView1.CoreWebView2.Navigate(System.IO.Path.GetTempPath() + "thumbnail.html");
                        }
                        catch (Exception ex)
                        {
                            DarkMessageBox msg = new DarkMessageBox("An error occurred while displaying the thumbnail.\n" + ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                            msg.ShowDialog();
                        }
                    }));
                });
                thread.Start();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // Browse folder
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath + @"\";
            }
        }

        private void YouTubeDownloadForm_Load(object sender, EventArgs e)
        {
            try
            {
                File.WriteAllLines(System.IO.Path.GetTempPath() + "thumbnail.html", blankHtml);

                webView1.CoreWebView2.Navigate(System.IO.Path.GetTempPath() + "thumbnail.html");
            }
            catch (Exception ex)
            {
                DarkMessageBox msg = new DarkMessageBox("An error occurred while displaying the thumbnail.\n" + ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                msg.ShowDialog();
            }
        }
    }
}
