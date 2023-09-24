using DownloadManager.NativeMethods;

namespace DownloadManager
{
    public partial class CurrentDownloads : Form
    {
        internal bool ForceActiveBar { get; set; }
        public static CurrentDownloads _instance;
        public List<DownloadItem> itemList = new List<DownloadItem>();
        public static int nextY = 12;

        public CurrentDownloads()
        {
            _instance = this;
            InitializeComponent();
            this.ForceActiveBar = true;

            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_NCHITTEST = 0x0084;
            const int HTCAPTION = 2;

            switch (m.Msg)
            {
                case WM_NCHITTEST:
                    base.WndProc(ref m);
                    if (m.Result == (IntPtr)HTCAPTION)
                    {
                        m.Result = (IntPtr)0x00; // Disable drag on titlebar
                        return;
                    }
                    break;
            }

            base.WndProc(ref m);
            if (m.Msg == User32.WM_NCACTIVATE)
            {
                if (this.ForceActiveBar && m.WParam == IntPtr.Zero)
                {
                    User32.SendMessageW(this.Handle, User32.WM_NCACTIVATE, new IntPtr(1), IntPtr.Zero);
                }
            }
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
            DownloadForm._instance.button2.BackColor = Color.Transparent;
            Settings.Default.showDownloadToolWindow = false;
            Settings.Default.Save();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void CurrentDownloads_Move(object sender, EventArgs e)
        {
            try
            {
                // Middle of the form
                int middleY = DownloadForm._instance.Height / 2;

                // To set the currentDownloads window y position to the middle of the parent form:
                this.Location = new Point(DownloadForm._instance.Location.X + DownloadForm._instance.Width + 5, DownloadForm._instance.Location.Y + middleY - (this.Height / 2));
            }
            catch { }
        }
    }
}
