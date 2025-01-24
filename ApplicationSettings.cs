using DownloadManager.NativeMethods;
using IWshRuntimeLibrary;
using System.Diagnostics;
using System.Reflection;
using System.Security.Principal;
using static DownloadManager.Logging;
using File = System.IO.File;

namespace DownloadManager
{
    public partial class ApplicationSettings : Form
    {
        public static readonly string installationPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\";
        HistoryEditor historyEditor = new HistoryEditor();

        public ApplicationSettings()
        {
            InitializeComponent();

            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);

            // Set download sound checkbox
            if (Settings.Default.soundOnComplete == false)
            {
                checkBox4.Checked = false;
            }

            // Set messagebox sound checkbox
            if (Settings.Default.soundOnMessage == false)
            {
                checkBox5.Checked = false;
            }

            label12.Text = "Version: " + Assembly.GetEntryAssembly().GetName().Version;

            numericUpDown1.Value = Settings.Default.serverPort;
            label15.Visible = false;

            // Start menu shortcut
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk"))
            {
                label8.Text = "Start Menu Shortcut: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk";
                button6.Enabled = false;
                button7.Enabled = true;
            }
            else
            {
                label8.Text = "Start Menu Shortcut: None Set";
                button6.Enabled = true;
                button7.Enabled = false;
            }

            // Desktop shortcut
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk"))
            {
                label9.Text = "Desktop Shortcut: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk";
                button8.Enabled = true;
                button9.Enabled = false;
            }
            else
            {
                label9.Text = "Desktop Shortcut: None Set";
                button8.Enabled = false;
                button9.Enabled = true;
            }

            Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup));

            // Startup shortcut
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + "\\Download Manager.lnk"))
            {
                checkBox3.Checked = true;
            }

            if (IsAdministrator() == false)
            {
                checkBox3.Enabled = false;
            }

            // Logging settings
            checkBox11.Checked = Settings.Default.doAutomaticLogSaving;
            textBox2.Text = Settings.Default.automaticLogSavingLocation;

            if (textBox2.Text == "")
            {
                textBox2.Text = $"{installationPath}Logs\\";
            }

            // Set notification checkboxes
            checkBox7.Checked = Settings.Default.notifyDone;
            checkBox6.Checked = Settings.Default.notifyDoneHashOk;
            checkBox8.Checked = Settings.Default.notifyDoneHashNo;
            checkBox9.Checked = Settings.Default.notifyFail;
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
            if (Settings.Default.defaultDownload == "" || Settings.Default.defaultDownload == null)
            {
                Settings.Default.defaultDownload = DownloadForm.downloadsFolder;
                Settings.Default.Save();
            }
            textBox1.Text = Settings.Default.defaultDownload;
            checkBox1.Checked = Settings.Default.closeOnComplete;
            checkBox2.Checked = Settings.Default.keepOnTop;
            checkBox10.Checked = Settings.Default.trayOnStartup;
            numericUpDown2.Value = Settings.Default.autoDownloadAttempts;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Default download location
            Settings.Default.defaultDownload = textBox1.Text;
            Settings.Default.Save();
            DownloadForm._instance.textBox2.Text = textBox1.Text;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // Close progress window on complete
            Settings.Default.closeOnComplete = checkBox1.Checked;
            Settings.Default.Save();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            // Keep progress window on top
            Settings.Default.keepOnTop = checkBox2.Checked;
            Settings.Default.Save();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Remove Start Menu Shortcut
            try
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk");
            }
            catch (Exception ex)
            {
                DarkMessageBox msg = new DarkMessageBox("An error occurred while saving your settings:\n" + ex.Message, "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                msg.ShowDialog();
            }

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk"))
            {
                label8.Text = "Start Menu Shortcut: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk";
                button6.Enabled = false;
                button7.Enabled = true;
            }
            else
            {
                label8.Text = "Start Menu Shortcut: None Set";
                button6.Enabled = true;
                button7.Enabled = false;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Create Start Menu Shortcut
            try
            {
                string pathToExe = DownloadForm.installationPath + "DownloadManager.exe";
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
            catch (Exception ex)
            {
                DarkMessageBox msg = new DarkMessageBox("An error occurred while saving your settings:\n" + ex.Message, "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                msg.ShowDialog();
            }

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk"))
            {
                label8.Text = "Start Menu Shortcut: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu) + "\\Programs\\Download Manager\\Download Manager.lnk";
                button6.Enabled = false;
                button7.Enabled = true;
            }
            else
            {
                label8.Text = "Start Menu Shortcut: None Set";
                button6.Enabled = true;
                button7.Enabled = false;
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            // Remove Desktop Shortcut
            try
            {
                File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk");
            }
            catch (Exception ex)
            {
                DarkMessageBox msg = new DarkMessageBox("An error occurred while saving your settings:\n" + ex.Message, "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                msg.ShowDialog();
            }

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk"))
            {
                label9.Text = "Desktop Shortcut: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk";
                button8.Enabled = true;
                button9.Enabled = false;
            }
            else
            {
                label9.Text = "Desktop Shortcut: None Set";
                button8.Enabled = false;
                button9.Enabled = true;
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            // Create Desktop Shortcut
            string pathToExe = DownloadForm.installationPath + "DownloadManager.exe";
            string commonDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory);

            if (!Directory.Exists(commonDesktopPath))
                Directory.CreateDirectory(commonDesktopPath);

            string shortcutLocation = Path.Combine(commonDesktopPath, "Download Manager" + ".lnk");
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

            shortcut.Description = "Download Manager helps you download your files faster.";
            shortcut.TargetPath = pathToExe;
            try
            {
                shortcut.Save();
            }
            catch (Exception ex)
            {
                DarkMessageBox msg = new DarkMessageBox("An error occurred while saving your settings:\n" + ex.Message, "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                msg.ShowDialog();
            }

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk"))
            {
                label9.Text = "Desktop Shortcut: " + Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) + "\\Download Manager.lnk";
                button8.Enabled = true;
                button9.Enabled = false;
            }
            else
            {
                label9.Text = "Desktop Shortcut: None Set";
                button8.Enabled = false;
                button9.Enabled = true;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            string commonStartupPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
            try
            {
                if (checkBox3.Checked)
                {
                    string pathToExe = DownloadForm.installationPath + "DownloadManager.exe";

                    if (!Directory.Exists(commonStartupPath))
                        Directory.CreateDirectory(commonStartupPath);

                    string shortcutLocation = Path.Combine(commonStartupPath, "Download Manager" + ".lnk");
                    WshShell shell = new WshShell();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutLocation);

                    shortcut.Description = "Download Manager helps you download your files faster.";
                    shortcut.TargetPath = pathToExe;
                    shortcut.Save();
                }
                else
                {
                    if (File.Exists(commonStartupPath + "\\Download Manager.lnk"))
                    {
                        try
                        {
                            File.Delete(commonStartupPath + "\\Download Manager.lnk");
                        }
                        catch (Exception ex)
                        {
                            DarkMessageBox msg = new DarkMessageBox("An error occurred while saving your settings:\n" + ex.Message, "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                            msg.ShowDialog();
                        }
                    }
                    else
                    {
                        DarkMessageBox msg = new DarkMessageBox("Could not delete startup shortcut. No such file exists.", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                        msg.Show();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR: " + ex.Message + Environment.NewLine + ex.StackTrace);
            }
        }

        public static bool IsAdministrator()
        {
            return (new WindowsPrincipal(WindowsIdentity.GetCurrent())).IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // Check for updates
            Logging.Log(LogLevel.Debug, "Checking for updates...");
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "DownloadManagerInstaller.exe";
            startInfo.Arguments = "--update";
            startInfo.UseShellExecute = true;
            startInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            startInfo.Verb = "runas";
            Process process = new Process();
            process.StartInfo = startInfo;

            Thread thread = new Thread(() =>
            {
                try
                {
                    process.Start();
                    process.WaitForExit();

                    // Check exit code
                    if (process.ExitCode == 0 || process.ExitCode == 1)
                    {
                        Logging.Log(LogLevel.Info, "Checking for updates succeeded with exit code: " + process.ExitCode);
                    }
                    else if (process.ExitCode == 2)
                    {
                        Logging.Log(LogLevel.Error, "Update error: Setup exited before the installation could complete.");
                    }
                    else if (process.ExitCode == 3)
                    {
                        Logging.Log(LogLevel.Error, "Update error: The update XML file is malformed.");
                    }
                    else if (process.ExitCode == 4)
                    {
                        Logging.Log(LogLevel.Error, "Update error: The update failed due to an error during the installation process.");
                    }
                    else if (process.ExitCode == 5)
                    {
                        Logging.Log(LogLevel.Error, "Update error: Setup could not retrieve the existing Download Manager installation. Ensure setup is in the same path as Download Manager and try again.");
                    }
                    else
                    {
                        Logging.Log(LogLevel.Error, "Update error: An unknown error occurred while checking for updates.");
                    }
                }
                catch (Exception ex)
                {
                    DarkMessageBox msg = new DarkMessageBox(ex.Message, "Download Manager - Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                    msg.ShowDialog();
                    Logging.Log(LogLevel.Error, "The setup application encountered a fatal error.");
                }
            });
            thread.Start();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            // Clear download history
            foreach (var item in DownloadForm._instance.textBox1.Items)
            {
                Application.DoEvents();
                DownloadForm._instance.textBox1.Items.Remove(item);
            }
            Settings.Default.downloadHistory = new System.Collections.Specialized.StringCollection();
            Settings.Default.Save();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            // Open history editor
            historyEditor.Show();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                Settings.Default.serverPort = Convert.ToInt32(numericUpDown1.Value);
                Settings.Default.Save();
                label15.Visible = true;
            }
            catch (Exception ex)
            {
                Logging.Log(LogLevel.Error, ex.Message + Environment.NewLine + ex.StackTrace);
                DarkMessageBox msg = new DarkMessageBox(ex.Message + "\nSee the debug log for more information.", "Download Manager Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                msg.ShowDialog();
            }
        }

        private void Settings_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Report a bug
            Process.Start(new ProcessStartInfo("https://github.com/Soniczac7/Download-Manager/issues/new?assignees=&labels=bug&template=bug_report.yml") { UseShellExecute = true });
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Request a feature
            Process.Start(new ProcessStartInfo("https://github.com/Soniczac7/Download-Manager/issues/new?assignees=&labels=feature&template=feature_request.yml") { UseShellExecute = true });
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open contributors webView window
            WebViewWindow webView = new WebViewWindow("https://github.com/Soniczac7/Download-Manager/graphs/contributors", "Download Manager Contributors");
            webView.Show();
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open documentation webView window
            WebViewWindow webView = new WebViewWindow("https://github.com/Download-Manager-Community/Download-Manager/wiki", "Download Manager Documentation");
            webView.Show();
        }

        private void linkLabel5_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Open release notes webView window
            WebViewWindow webView = new WebViewWindow("https://github.com/Download-Manager-Community/Download-Manager/releases", "Download Manager Release Notes");
            webView.Show();
        }

        private void linkLabel6_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Browser extention page
            Process.Start(new ProcessStartInfo("https://microsoftedge.microsoft.com/addons/detail/download-manager/facopbimneimllhcabghncloejfeficd?hl=en-GB") { UseShellExecute = true });
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            // Sound on download complete
            Settings.Default.soundOnComplete = checkBox4.Checked;
            Settings.Default.Save();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            // Sound on messagebox open
            Settings.Default.soundOnMessage = checkBox5.Checked;
            Settings.Default.Save();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            // Notification when download done
            Settings.Default.notifyDone = checkBox7.Checked;
            Settings.Default.Save();
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            // Notification when download done with hash verified
            Settings.Default.notifyDoneHashOk = checkBox6.Checked;
            Settings.Default.Save();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            // Notification when download done hash bad
            Settings.Default.notifyDoneHashNo = checkBox8.Checked;
            Settings.Default.Save();
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            // Notification when download fails
            Settings.Default.notifyFail = checkBox9.Checked;
            Settings.Default.Save();
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            // Automatic download attempts
            Settings.Default.autoDownloadAttempts = Convert.ToInt32(numericUpDown2.Value);
            Settings.Default.Save();
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            // Tray on startup
            Settings.Default.trayOnStartup = checkBox10.Checked;
            Settings.Default.Save();
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            // Automatic log saving
            Settings.Default.doAutomaticLogSaving = checkBox11.Checked;
            Settings.Default.Save();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            // Automatic log saving location
            Settings.Default.automaticLogSavingLocation = textBox2.Text;
            Settings.Default.Save();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            // Browse automatic log saving location
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Text = folderBrowserDialog1.SelectedPath + @"\";
            }
        }
    }
}
