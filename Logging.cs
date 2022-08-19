using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DownloadManager
{
    public partial class Logging : Form
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

        public static Logging _instance;

        public Logging()
        {
            _instance = this;
            InitializeComponent();
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

        private void Logging_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
