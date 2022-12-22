using System.Runtime.InteropServices;

namespace DownloadManager
{
    public partial class CurrentDownloads : Form
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

        public static CurrentDownloads _instance;
        List<GroupBox> controlsList = new List<GroupBox>();
        int nextY = 12;

        public CurrentDownloads()
        {
            _instance = this;
            InitializeComponent();
        }

        public void RefreshList()
        {
            foreach (Control control in controlsList)
            {
                this.Controls.Remove(control);
            }

            nextY = 12;

            foreach (DownloadProgress progress in DownloadForm.downloadsList)
            {
                AddItem(progress);
                nextY += 12 + 105;
            }
        }

        private void AddItem(DownloadProgress progress)
        {
            GroupBox groupBox = new GroupBox()
            {
                Size = new Size(428, 105),
                Location = new Point(12, nextY),
                Text = ""
            };

            this.Controls.Add(groupBox);

            controlsList.Add(groupBox);

            // TODO: Add controls to groupbox and make it update
        }

        private void CurrentDownloads_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            DownloadForm._instance.button2.BackColor = Color.FromArgb(34, 34, 34);
            Settings.Default.showDownloadToolWindow = false;
            Settings.Default.Save();
        }
    }
}
