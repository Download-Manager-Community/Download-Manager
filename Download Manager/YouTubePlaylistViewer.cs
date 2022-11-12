using System.Runtime.InteropServices;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace DownloadManager
{
    public partial class YouTubePlaylistViewer : Form
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
        YoutubeExplode.Playlists.Playlist metadata;
        TreeNode playlist;
        string[] videos = new string[] { };

        public YouTubePlaylistViewer(YoutubeExplode.Playlists.Playlist metadata)
        {
            InitializeComponent();

            // Set comboBox1's default value
            comboBox1.SelectedIndex = 0;

            // Show progressbar
            progressBar1.Visible = true;

            // Ensure metadata is not null
            if (metadata == null)
            {
                throw new ArgumentNullException("metadata");
            }

            this.metadata = metadata;
        }

        async private void YouTubePlaylistViewer_Load(object sender, EventArgs e)
        {
            // Set the title
            this.Text = metadata.Title;

            // Add first node to treeView
            playlist = treeView1.Nodes.Add(metadata.Title);

            // Get video titles and add to the treeView
            await foreach (var video in client.Playlists.GetVideosAsync(metadata.Id))
            {
                playlist.Nodes.Add(video.Title);
                List<string> temp = videos.ToList();
                temp.Add(video.Id);
                videos = temp.ToArray();
                temp = null;
            }

            // Hide progressbar
            progressBar1.Visible = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            /// Download selected video
            // Show progressabr
            progressBar1.Visible = true;

            // Ensure a video is selected
            if (treeView1.SelectedNode.Index == 0)
            {
                DarkMessageBox msg = new DarkMessageBox("Cannot download the playlist from here.\nTo download the playlist click " + '"' + "Download" + '"' + "on the YouTube Downloader window.", "Download Manager - Playlist Downloader Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, true);
                msg.ShowDialog();
                progressBar1.Visible = false;
                return;
            }

            // Find the selected video
            string videoId = videos[treeView1.SelectedNode.Index];

            // Get video's title
            string vidTitle = treeView1.SelectedNode.Text;

            if (comboBox1.SelectedIndex == 0)
            {
                Thread thread = new Thread(async delegate ()
                {
                    /// Audio Only
                    // Get stream
                    var vidManifest = client.Videos.Streams.GetManifestAsync(videoId).Result;

                    // Get stream info
                    var streamInfo = vidManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

                    // Download video
                    await client.Videos.Streams.DownloadAsync(streamInfo, System.IO.Path.GetTempPath() + "temp.mp4");

                    try
                    {
                        File.Move(System.IO.Path.GetTempPath() + "temp.mp4", Settings1.Default.defaultDownload + vidTitle.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + ".mp3");
                    }
                    catch (Exception ex)
                    {
                        DarkMessageBox msgerr = new DarkMessageBox("Error while writing file:\n" + ex.Message + Environment.NewLine + ex.StackTrace, "Download Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, false);
                        DialogResult result = msgerr.ShowDialog();

                        if (result == DialogResult.Retry)
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                button1_Click(sender, e);
                            }));

                            return;
                        }
                    }

                    // Hide progressbar
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Visible = false;
                    }));
                });
                thread.Start();
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                Thread thread = new Thread(async delegate ()
                {
                    /// Video Only
                    // Get stream
                    var vidManifest = client.Videos.Streams.GetManifestAsync(videoId).Result;

                    // Get stream info
                    var streamInfo = vidManifest.GetVideoOnlyStreams().GetWithHighestVideoQuality();

                    // Download video
                    await client.Videos.Streams.DownloadAsync(streamInfo, Settings1.Default.defaultDownload + vidTitle.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + "mp4");

                    // Hide progressbar
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Visible = false;
                    }));
                });
                thread.Start();
            }
            else
            {
                Thread thread = new Thread(async delegate ()
                {
                    /// Audio and Video
                    // Get stream
                    var vidManifest = client.Videos.Streams.GetManifestAsync(videoId).Result;

                    // Get stream info
                    var streamInfo = vidManifest.GetMuxedStreams().GetWithHighestVideoQuality();

                    // Download video
                    await client.Videos.Streams.DownloadAsync(streamInfo, vidTitle.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + "mp4");

                    // Hide progressbar
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Visible = false;
                    }));
                });
                thread.Start();
            }
        }
    }
}
