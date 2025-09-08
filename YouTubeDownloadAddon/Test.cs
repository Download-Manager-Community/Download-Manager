using DownloadManager.Components.Addons.YouTubeDownloader;

namespace YouTubeDownloadAddon
{
    public partial class Test : Form
    {
        public Test()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            YouTubeOptions options = new YouTubeOptions();
            options.Show();
        }
    }
}
