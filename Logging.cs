using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DownloadManager
{
    public partial class Logging : Form
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

        public static Logging _instance;

        public Logging()
        {
            _instance = this;
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
        }

        public static void Log(string text, Color color)
        {
            if (_instance == null)
            {
                return;
            }
            else if (_instance.IsDisposed == true)
            {
                return;
            }
            else
            {
                if (!_instance.IsHandleCreated)
                {
                    try
                    {
                        _instance.CreateHandle();
                    }
                    catch (Exception err)
                    {
                        MessageBox.Show("A error occurred and logging cannot continue.\nError Details:\n" + err.Message + "\n" + err.StackTrace, "Download Manager - Logging - Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (_instance.richTextBox1.InvokeRequired)
                {
                    Action logAction1 = () => _instance.richTextBox1.SelectionStart = _instance.richTextBox1.TextLength;
                    _instance.richTextBox1.Invoke(logAction1);
                    Action logAction2 = () => _instance.richTextBox1.SelectionLength = 0;
                    _instance.richTextBox1.Invoke(logAction2);
                    Action logAction3 = () => _instance.richTextBox1.SelectionColor = color;
                    _instance.richTextBox1.Invoke(logAction3);
                    Action logAction4 = () => _instance.richTextBox1.AppendText(text + "\n");
                    _instance.richTextBox1.Invoke(logAction4);
                    Action logAction5 = () => _instance.richTextBox1.ScrollToCaret();
                    _instance.richTextBox1.Invoke(logAction5);
                    Action logAction6 = () => _instance.richTextBox1.SelectionColor = _instance.richTextBox1.ForeColor;
                    _instance.richTextBox1.Invoke(logAction6);
                }
                else
                {
                    _instance.richTextBox1.SelectionStart = _instance.richTextBox1.TextLength;
                    _instance.richTextBox1.SelectionLength = 0;
                    _instance.richTextBox1.SelectionColor = color;
                    _instance.richTextBox1.AppendText(text + "\n");
                    _instance.richTextBox1.ScrollToCaret();
                    _instance.richTextBox1.SelectionColor = _instance.richTextBox1.ForeColor;
                }
            }
        }

        public void LogProcessOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Log(e.Data, Color.White);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            // Title-bar Drag
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }
    }
}
