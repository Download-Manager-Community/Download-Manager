using DownloadManager.NativeMethods;
using System.Runtime.InteropServices;
using YoutubeExplode;
using static DownloadManager.DownloadProgress;

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

            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);

            // Set comboBox1's default value
            comboBox1.SelectedIndex = 0;

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
            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.MarqueeAnim = false;
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            /// Download selected video
            // Show progressabr
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnim = true;

            // Ensure a video is selected
            if (treeView1.SelectedNode.Index == 0)
            {
                DarkMessageBox msg = new DarkMessageBox("Cannot download the playlist from here.\nTo download the playlist click " + '"' + "Download" + '"' + "on the YouTube Downloader window.", "Download Manager - Playlist Downloader Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, true);
                msg.ShowDialog();
                progressBar1.Visible = false;
                return;
            }

            YoutubeDownloadType type;

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    // Audio Only
                    type = YoutubeDownloadType.Audio;
                    break;
                case 1:
                    // Video Only
                    type = YoutubeDownloadType.Video;
                    break;
                case 2:
                    // Audio and Video
                    type = YoutubeDownloadType.AudioVideo;
                    break;
                default:
                    DarkMessageBox msg = new DarkMessageBox("Please select a download type.", "No valid download type", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    msg.ShowDialog();
                    return;
            }

            DownloadProgress download = new DownloadProgress(metadata.Url, Settings.Default.defaultDownload, DownloadType.YoutubeVideo, type, "", 0);
            download.Show();

            progressBar1.Style = ProgressBarStyle.Blocks;
            progressBar1.MarqueeAnim = false;
        }
    }
}
