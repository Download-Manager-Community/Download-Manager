using DownloadManager.NativeMethods;
using System.Diagnostics;

namespace DownloadManager
{
    public partial class Logging : Form
    {
        public static Logging _instance;

        public Logging()
        {
            _instance = this;
            InitializeComponent();

            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);
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
                        DarkMessageBox msg = new DarkMessageBox("A error occurred and logging cannot continue.\nError Details:\n" + err.Message + "\n" + err.StackTrace, "Download Manager - Logging - Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
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
