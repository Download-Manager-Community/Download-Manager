using DownloadManager.NativeMethods;

namespace DownloadManager
{
    public partial class CurrentDownloads : Form
    {
        public static CurrentDownloads _instance;
        public List<DownloadItem> itemList = new List<DownloadItem>();
        public static int nextY = 12;

        public CurrentDownloads()
        {
            _instance = this;
            InitializeComponent();

            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);
        }

        public void RefreshList()
        {
            foreach (DownloadItem item in itemList)
            {
                item.DisposeNoRemove();
            }

            itemList.Clear();

            nextY = 12;

            foreach (DownloadProgress progress in DownloadForm.downloadsList)
            {
                AddItem(progress);
                nextY = nextY + 117;
            }
        }

        private void AddItem(DownloadProgress progress)
        {
            DownloadItem item = new DownloadItem();
            itemList.Add(item);
            item.Initialize(progress);
        }

        private void CurrentDownloads_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            DownloadForm._instance.button2.BackColor = Color.FromArgb(34, 34, 34);
            Settings.Default.showDownloadToolWindow = false;
            Settings.Default.Save();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshList();
        }
    }
}
