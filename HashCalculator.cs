using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace DownloadManager
{
    public partial class HashCalculator : Form
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

        public HashCalculator(string file)
        {
            InitializeComponent();
            textBox1.Text = file;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
            if (File.Exists(file))
            {
                // MD5
                try
                {
                    byte[] myHash;
                    using (var md5 = MD5.Create())
                    using (var stream = File.OpenRead(file))
                    {
                        myHash = md5.ComputeHash(stream);
                        stream.Close();
                    }

                    StringBuilder result = new StringBuilder(myHash.Length * 2);

                    for (int i = 0; i < myHash.Length; i++)
                    {
                        result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                    }
                    textBox2.Text = result.ToString();
                }
                catch (Exception ex)
                {
                    textBox2.Text = ex.Message;
                }

                // SHA-1
                try
                {
                    byte[] myHash;
                    using (var hash = SHA1.Create())
                    using (var stream = File.OpenRead(file))
                    {
                        myHash = hash.ComputeHash(stream);
                        stream.Close();
                    }

                    StringBuilder result = new StringBuilder(myHash.Length * 2);

                    for (int i = 0; i < myHash.Length; i++)
                    {
                        result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                    }
                    textBox3.Text = result.ToString();
                }
                catch (Exception ex)
                {
                    textBox3.Text = ex.Message;
                }

                // SHA-256
                try
                {
                    byte[] myHash;
                    using (var hash = SHA256.Create())
                    using (var stream = File.OpenRead(file))
                    {
                        myHash = hash.ComputeHash(stream);
                        stream.Close();
                    }

                    StringBuilder result = new StringBuilder(myHash.Length * 2);

                    for (int i = 0; i < myHash.Length; i++)
                    {
                        result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                    }
                    textBox4.Text = result.ToString();
                }
                catch (Exception ex)
                {
                    textBox4.Text = ex.Message;
                }

                // SHA-384
                try
                {
                    byte[] myHash;
                    using (var hash = SHA384.Create())
                    using (var stream = File.OpenRead(file))
                    {
                        myHash = hash.ComputeHash(stream);
                        stream.Close();
                    }

                    StringBuilder result = new StringBuilder(myHash.Length * 2);

                    for (int i = 0; i < myHash.Length; i++)
                    {
                        result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                    }
                    textBox6.Text = result.ToString();
                }
                catch (Exception ex)
                {
                    textBox6.Text = ex.Message;
                }

                // SHA-512
                try
                {
                    byte[] myHash;
                    using (var hash = SHA512.Create())
                    using (var stream = File.OpenRead(file))
                    {
                        myHash = hash.ComputeHash(stream);
                        stream.Close();
                    }

                    StringBuilder result = new StringBuilder(myHash.Length * 2);

                    for (int i = 0; i < myHash.Length; i++)
                    {
                        result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                    }
                    textBox5.Text = result.ToString();
                }
                catch (Exception ex)
                {
                    textBox5.Text = ex.Message;
                }
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
