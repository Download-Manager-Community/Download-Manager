using IWshRuntimeLibrary;
using System.Diagnostics;
using System.IO.Compression;
using File = System.IO.File;

namespace DownloadManagerInstaller
{
    public partial class Form1 : Form
    {
        static public Form1 _instance;
        int installStage = 1;
        string fileName;
        string path;
        string md5Hash = "";
        bool installing = false;

        public Form1()
        {
            _instance = this;
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Browse Install Folder
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (folderBrowserDialog1.SelectedPath.EndsWith("\\"))
                {
                    textBox1.Text = folderBrowserDialog1.SelectedPath;
                }
                else
                {
                    textBox1.Text = folderBrowserDialog1.SelectedPath + "\\";
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Next
            if (installStage == 1)
            {
                tabControl1.Visible = false;
                tabControl2.Visible = true;
                button1.Enabled = false;
                checkBox3.Visible = true;
                checkBox3.Enabled = false;
                DownloadProgress progress = new DownloadProgress("https://raw.githubusercontent.com/Soniczac7/Download-Manager/master/LICENSE.txt", System.IO.Path.GetTempPath());
                progress.ShowDialog();
                installStage += 1;
            }
            else if (installStage == 2)
            {
                tabControl2.Visible = false;
                checkBox3.Visible = false;
                groupBox1.Visible = true;
                button1.Text = "Finish";
                button1.Enabled = false;
                Install();
                installStage += 1;
            }
            else
            {
                if (checkBox4.Checked)
                {
                    ProcessStartInfo info = new ProcessStartInfo();
                    info.FileName = path + "DownloadManager.exe";
                    Process.Start(info);
                }

                Process.GetCurrentProcess().Kill();
            }
        }

        public void LicenseFailed()
        {
            DialogResult result = MessageBox.Show("The MD5 verification of the license failed.\nRetry Download?", "Download Manager - Error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            if (result == DialogResult.Yes)
            {
                DownloadProgress progress = new DownloadProgress("https://raw.githubusercontent.com/Soniczac7/Download-Manager/master/LICENSE.txt", System.IO.Path.GetTempPath());
                progress.ShowDialog();
            }
            else
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        public void Install()
        {
            installing = true;
            path = textBox1.Text;
            Invoke(new MethodInvoker(delegate ()
            {
                progressBar1.Value = 0;
                progressBar1.Style = ProgressBarStyle.Blocks;
            }));

            Thread thread = new Thread(() =>
            {
                try
                {
                    Action increaseProgress20 = () => progressBar1.Value += 20;
                    Action increaseProgress10 = () => progressBar1.Value += 10;

                    // Check if path exists
                    if (!System.IO.Directory.Exists(textBox1.Text))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }
                    else
                    {
                        System.IO.Directory.Delete(path, true);
                        System.IO.Directory.CreateDirectory(path);
                    }
                    _instance.Invoke(increaseProgress20);

                    // Put zip in directory
                    File.WriteAllBytes(path + "install.zip", Properties.Resources.Download_Manager);

                    _instance.Invoke(increaseProgress20);

                    ZipFile.ExtractToDirectory(path + "install.zip", path);

                    _instance.Invoke(increaseProgress20);

                    // Delete zip
                    File.Delete(path + "install.zip");

                    _instance.Invoke(increaseProgress20);

                    // Create desktop shortcut
                    if (checkBox1.Checked)
                    {
                        string pathToExe = path + "DownloadManager.exe";
                        string commonDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);

                        if (!Directory.Exists(commonDesktopPath))
                            Directory.CreateDirectory(commonDesktopPath);

                        string shortcutLocation = Path.Combine(commonDesktopPath, "Download Manager" + ".lnk");
                        WshShell shell = new WshShell();
                        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                        shortcut.Description = "Download Manager helps you download your files faster.";
                        shortcut.TargetPath = pathToExe;
                        shortcut.Save();
                    }

                    _instance.Invoke(increaseProgress10);

                    // Create start menu shortcut
                    if (checkBox2.Checked)
                    {
                        string pathToExe = path + "DownloadManager.exe";
                        string commonStartMenuPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu);
                        string appStartMenuPath = Path.Combine(commonStartMenuPath, "Programs", "Download Manager");

                        if (!Directory.Exists(appStartMenuPath))
                            Directory.CreateDirectory(appStartMenuPath);

                        string shortcutLocation = Path.Combine(appStartMenuPath, "Download Manager" + ".lnk");
                        WshShell shell = new WshShell();
                        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                        shortcut.Description = "Download Manager helps you download your files faster.";
                        shortcut.TargetPath = pathToExe;
                        shortcut.Save();
                    }

                    _instance.Invoke(increaseProgress10);

                    Invoke(new MethodInvoker(delegate ()
                    {
                        label5.Text = "Download Manager has installed successfully.";
                        checkBox4.Visible = true;
                        button1.Enabled = true;
                        installing = false;
                    }));
                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show(ex.Message, "Setup - Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (result == DialogResult.Retry)
                    {
                        Install();
                    }
                    else
                    {
                        Process.GetCurrentProcess().Kill();
                    }
                }
            });
            thread.Start();

        }

        public void LicenseDownloaded(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    richTextBox1.Text = File.ReadAllText(path);
                    checkBox3.Enabled = true;
                }
                catch (Exception ex)
                {
                    DialogResult result = MessageBox.Show("Failed to read license.\n" + ex.Message, "Download Manager Setup - Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (result == DialogResult.Retry)
                    {
                        LicenseDownloaded(path);
                    }
                    else
                    {
                        MessageBox.Show(ex.Message, "Download Manager Setup - Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }
                }

            }
            else
            {
                DialogResult result = MessageBox.Show("License failed to download.", "Download Manager Setup - Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                if (result == DialogResult.Retry)
                {
                    DownloadProgress progress = new DownloadProgress("https://raw.githubusercontent.com/Soniczac7/Download-Manager/master/LICENSE.txt", path);
                    progress.ShowDialog();
                }
                else
                {
                    MessageBox.Show("License failed to download.\nSetup cannot continue.", "Download Manager Setup - Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Application.Exit();
                }
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            // Toggle button1
            if (checkBox3.Checked)
            {
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (installing)
            {
                MessageBox.Show("Download Manager is currently installing on your computer.\nPlease wait until setup has completed to close.", "Download Manager Setup - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure you want to cancel the setup of Download Manager?", "Download Manager Setup", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}