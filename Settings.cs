using System.Runtime.InteropServices;

namespace DownloadManager
{
    public partial class Settings : Form
    {
        #region DLL Import
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr one, int two, int three, int four);
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );
        #endregion

        public Settings()
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Close
            this.Hide();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Minimize
            this.WindowState = FormWindowState.Minimized;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            // Titlebar Drag
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Browse default download location
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath + @"\";
            }
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            // Load settings
            if (Settings1.Default.defaultDownload == "" || Settings1.Default.defaultDownload == null)
            {
                Settings1.Default.defaultDownload = DownloadForm.downloadsFolder;
                Settings1.Default.Save();
            }
            textBox1.Text = Settings1.Default.defaultDownload;
            checkBox1.Checked = Settings1.Default.closeOnComplete;
            checkBox2.Checked = Settings1.Default.keepOnTop;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Default download location
            Settings1.Default.defaultDownload = textBox1.Text;
            Settings1.Default.Save();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Close progress window on complete
            Settings1.Default.closeOnComplete = checkBox1.Checked;
            Settings1.Default.Save();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            // Keep progress window on top
            Settings1.Default.keepOnTop = checkBox2.Checked;
            Settings1.Default.Save();
        }
    }
}
