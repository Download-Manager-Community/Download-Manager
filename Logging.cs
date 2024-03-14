using DownloadManager.Controls;
using DownloadManager.NativeMethods;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace DownloadManager
{
    public partial class Logging : Form
    {
        public static Logging _instance;
        internal static FileStream fs;

        public Logging()
        {
            fs = new FileStream($"{Settings.Default.automaticLogSavingLocation}{DateTime.Now.ToString("dd-MM-yyyy")}.log", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            fs.Position = fs.Length;

            _instance = this;
            InitializeComponent();

            menuStrip1.Renderer = new DarkToolStripRenderer();
            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);

            toggleLoggingToolStripMenuItem.Checked = Settings.Default.doAutomaticLogSaving;
        }

        public static async void Log(LogLevel level, string text)
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
                        msg.ShowDialog();
                        return;
                    }
                }
                if (_instance.richTextBox1.InvokeRequired)
                {
                    Color color = Color.White;
                    switch (level)
                    {
                        case LogLevel.Debug:
                            color = Color.Gray;
                            break;
                        case LogLevel.Info:
                            color = Color.White;
                            break;
                        case LogLevel.Warning:
                            color = Color.Orange;
                            break;
                        case LogLevel.Error:
                            color = Color.Red;
                            break;
                    }

                    _instance.Invoke(new MethodInvoker(delegate ()
                    {
                        _instance.richTextBox1.SelectionStart = _instance.richTextBox1.TextLength;
                        _instance.richTextBox1.SelectionLength = 0;
                        _instance.richTextBox1.SelectionColor = color;
                        _instance.richTextBox1.AppendText($"[{DateTime.Now}] [{level}] {text}\n");
                        _instance.richTextBox1.ScrollToCaret();
                        _instance.richTextBox1.SelectionColor = _instance.richTextBox1.ForeColor;
                    }));

                    string newText = text;
                    Regex regex = new Regex("(http(s)?\\:\\/\\/)?[-a-zA-Z0-9@:%._\\+~#=]+\\.[a-zA-Z0-9()]+\\b([-a-zA-Z0-9()@:%_\\+.~#?&//=]*)"); // Any URL
                    if (regex.IsMatch(newText))
                    {
                        string url = regex.Match(newText).Value;
                        newText = newText.Replace(url, "[url]");
                    }

                    await fs.WriteAsync(Encoding.UTF8.GetBytes($"[{DateTime.Now}] [{level}] {newText}\n"));
                    await fs.FlushAsync();
                }
                else
                {
                    Color color = Color.White;
                    switch (level)
                    {
                        case LogLevel.Debug:
                            color = Color.Gray;
                            break;
                        case LogLevel.Info:
                            color = Color.White;
                            break;
                        case LogLevel.Warning:
                            color = Color.Orange;
                            break;
                        case LogLevel.Error:
                            color = Color.Red;
                            break;
                    }

                    _instance.richTextBox1.SelectionStart = _instance.richTextBox1.TextLength;
                    _instance.richTextBox1.SelectionLength = 0;
                    _instance.richTextBox1.SelectionColor = color;
                    _instance.richTextBox1.AppendText($"[{DateTime.Now}] [{level}] {text}\n");
                    _instance.richTextBox1.ScrollToCaret();
                    _instance.richTextBox1.SelectionColor = _instance.richTextBox1.ForeColor;

                    string newText = text;
                    Regex regex = new Regex("(http(s)?\\:\\/\\/)?[-a-zA-Z0-9@:%._\\+~#=]+\\.[a-zA-Z0-9()]+\\b([-a-zA-Z0-9()@:%_\\+.~#?&//=]*)"); // Any URL
                    if (regex.IsMatch(newText))
                    {
                        string url = regex.Match(newText).Value;
                        newText = newText.Replace(url, "[url]");
                    }

                    await fs.WriteAsync(Encoding.UTF8.GetBytes($"[{DateTime.Now}] [{level}] {newText}\n"));
                    await fs.FlushAsync();
                }
            }
        }

        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error
        }

        public void LogProcessOutput(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null)
            {
                Log(LogLevel.Info, e.Data);
            }
        }

        private void Logging_FormClosing(object sender, FormClosingEventArgs e)
        {
            fs.Close();
            e.Cancel = true;
            this.Hide();
        }

        private void toggleLoggingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Toggle logging
            if (toggleLoggingToolStripMenuItem.Checked == true)
            {
                toggleLoggingToolStripMenuItem.Checked = false;
                DownloadForm._instance.settings.checkBox11.Checked = false;
            }
            else
            {
                toggleLoggingToolStripMenuItem.Checked = true;
                DownloadForm._instance.settings.checkBox11.Checked = true;
            }
        }

        private async void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(saveFileDialog.FileName))
                {
                    File.Delete(saveFileDialog.FileName);
                }

                FileStream saveFs = new FileStream($"{saveFileDialog.FileName}", FileMode.Create, FileAccess.ReadWrite, FileShare.Read);

                foreach (string line in richTextBox1.Lines)
                {
                    string newLine = line;
                    Regex regex = new Regex("(http(s)?\\:\\/\\/)?[-a-zA-Z0-9@:%._\\+~#=]+\\.[a-zA-Z0-9()]+\\b([-a-zA-Z0-9()@:%_\\+.~#?&//=]*)"); // Any URL
                    if (regex.IsMatch(newLine))
                    {
                        string url = regex.Match(newLine).Value;
                        newLine = newLine.Replace(url, "[url]");
                    }

                    await saveFs.WriteAsync(Encoding.UTF8.GetBytes($"{newLine}\n"));
                }
                await saveFs.FlushAsync();
                saveFs.Close();
            }
        }
    }
}
