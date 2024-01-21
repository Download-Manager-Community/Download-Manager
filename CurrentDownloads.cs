using DownloadManager.NativeMethods;
using System.Reflection;

namespace DownloadManager
{
    public partial class CurrentDownloads : Form
    {
        internal bool ForceActiveBar { get; set; }
        public static CurrentDownloads _instance;
        public List<DownloadItem> itemList = new List<DownloadItem>();
        public static int nextY = 12;

        public static bool firstShown = true;
        public static bool firstShownDataError = true;

        public CurrentDownloads()
        {
            _instance = this;
            InitializeComponent();
            this.ForceActiveBar = true;

            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);

            // Double buffer the progressGridView control
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, progressGridView, new object[] { true });
        }

        public Task HideAfterFirstShow()
        {
            while (firstShown)
            {
                // wait
                Application.DoEvents();
            }

            //this.Hide();

            return Task.FromResult(true);
        }

        protected override void WndProc(ref Message m)
        {
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
            progressGridView.Rows.Clear();

            foreach (DownloadProgress progress in DownloadForm.downloadsList)
            {
                AddItem(progress);
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
            if (DownloadForm._instance == null)
            {
                return;
            }

            try
            {
                // Middle of the form
                int middleY = DownloadForm._instance.Height / 2;

                // To set the currentDownloads window y position to the middle of the parent form:
                this.Location = new Point(DownloadForm._instance.Location.X + DownloadForm._instance.Width + 5, DownloadForm._instance.Location.Y + middleY - (this.Height / 2));
            }
            catch { }
        }

        private void CurrentDownloads_Shown(object sender, EventArgs e)
        {
            if (firstShown)
            {
                firstShown = false;
            }
        }

        private void progressGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.GetType().FullName.ToString() == "System.FormatException")
            {
                // Do nothing (internal winforms fault)
                return;
            }
            else
            {
                Logging.Log($"A data error was thrown in progressGridView. This is a bug, please report at https://downloadmanager.soniczac7.rf.gd/bugReport!\n{e.Exception.Message} ({e.Exception.GetType().FullName})\n{e.Exception.StackTrace}", Color.Red);
                if (firstShownDataError)
                {
                    firstShownDataError = false;
                    MessageBox.Show($"A data error was thrown in progressGridView. This is a bug, please report at https://downloadmanager.soniczac7.rf.gd/bugReport!!\n{e.Exception.Message} ({e.Exception.GetType().FullName})\n{e.Exception.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            progressGridView.Refresh();
        }

        private void CurrentDownloads_Resize(object sender, EventArgs e)
        {
            // Move the window to to correct location while resizing
            CurrentDownloads_Move(sender, e);
        }
    }
}
