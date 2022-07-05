using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace DownloadManager
{
    public partial class MD5Calculator : Form
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

        public MD5Calculator(string file)
        {
            InitializeComponent();
            textBox1.Text = file;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
            if (File.Exists(file))
            {
                byte[] fileData = File.ReadAllBytes(file);
                byte[] myHash = MD5.Create().ComputeHash(fileData);
                StringBuilder result = new StringBuilder(myHash.Length * 2);

                for (int i = 0; i < myHash.Length; i++)
                {
                    result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                }

                textBox2.Text = result.ToString();
            }
            else
            {
                MessageBox.Show("File not found!", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            // Title-bar Drag
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Close
            this.Close();
            this.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Minimize
            WindowState = FormWindowState.Minimized;
        }
    }
}
