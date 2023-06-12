using DownloadManager.NativeMethods;
using YoutubeExplode;
using YoutubeExplode.Common;
using static DownloadManager.DownloadProgress;

namespace DownloadManager
{
    public partial class YouTubeDownloadForm : Form
    {
        YoutubeClient client = new YoutubeClient();
        YoutubeExplode.Videos.Video? vidMetadata = null;
        YoutubeExplode.Playlists.Playlist? listMetadata = null;
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

        private string url;
        private string location;
        private bool isPlaylist = false;

        public YouTubeDownloadForm(string url, string location)
        {
            InitializeComponent();

            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);

            comboBox1.SelectedIndex = 2;
            webView1.EnsureCoreWebView2Async();

            this.url = url;
            this.location = location;

            label12.Text = location;

            if (url.Contains("playlist?"))
            {
                isPlaylist = true;
            }

            Thread thread = new Thread(delegate ()
            {
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
                    DarkMessageBox msg = new DarkMessageBox(ex.Message, "Error fetching YouTube metadata", MessageBoxButtons.OK, MessageBoxIcon.Error, false);
                    msg.ShowDialog();

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        this.Close();
                        this.Dispose();
                    }));

                    return;
                }

                this.Invoke(new MethodInvoker(delegate ()
                {
                    if (vidMetadata != null)
                    {
                        button4.Visible = false;
                        label2.Text = vidMetadata.Title;
                        label3.Text = vidMetadata.Author.ChannelTitle;
                        label4.Text = vidMetadata.UploadDate.LocalDateTime.ToString();
                        label5.Text = vidMetadata.Duration.ToString();

                        // Get Thumbnail
                        Thumbnail thumbnail = vidMetadata.Thumbnails.GetWithHighestResolution();
                        string url = thumbnail.Url;

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
                            this.Focus();

                            Point loc = PointToScreen(webView1.Location);
                            toolTip1.Show("An error occurred while displaying the thumbnail.\n" + ex.Message, this, loc.X, loc.Y);
                            /*DarkMessageBox msg = new DarkMessageBox("An error occurred while displaying the thumbnail.\n" + ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                            msg.ShowDialog();*/
                        }
                    }
                    else if (listMetadata != null)
                    {
                        label2.Text = listMetadata.Title;
                        label3.Text = listMetadata.Author.ChannelTitle;
                        label4.Text = listMetadata.Id;
                        label8.Text = "Id: ";
                        label5.Visible = false;
                        label9.Visible = false;


                        // Get Thumbnail
                        Thumbnail thumbnail = listMetadata.Thumbnails.GetWithHighestResolution();
                        string url = thumbnail.Url;

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
                    }
                    else
                    {
                        DarkMessageBox msg = new DarkMessageBox("An error occurred while fetching the YouTube metadata.", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                        msg.ShowDialog();
                    }
                }));
            });
            thread.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DownloadType downloadType;

            switch (isPlaylist)
            {
                case true:
                    downloadType = DownloadType.YoutubePlaylist;
                    break;
                case false:
                    downloadType = DownloadType.YoutubeVideo;
                    break;
            }

            YoutubeDownloadType ytDownloadType = (YoutubeDownloadType)Enum.ToObject(typeof(YoutubeDownloadType), comboBox1.SelectedIndex);

            DownloadProgress progress = new DownloadProgress(url, location, downloadType, ytDownloadType, "", 0);
            progress.Show();

            this.Close();
            this.Dispose();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Open playlist viewer
            YouTubePlaylistViewer playlistViewer = new YouTubePlaylistViewer(listMetadata);
            playlistViewer.Show();
        }
    }
}
