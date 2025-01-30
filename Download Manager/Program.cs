using System.Diagnostics;
using System.Text;

namespace DownloadManager
{
    internal static class Program
    {
        public const bool DEBUG = true;
        public static bool allowBypassCrashHandler = true;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            // Enable hardware acceleration
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            if (DEBUG == true)
            {
                MessageBox.Show("Debug mode is on. You can now attach a debugger.\nTo turn it off, go to Program.cs and set DEBUG to false.\nPress OK to continue...", "Download Manager - DEBUG", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                try
                {
                    HttpClient client = new HttpClient();
                    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"http://localhost:{Settings.Default.serverPort}?show=True&ref=Instance");
                    client.Send(request);
                    Environment.Exit(45);
                }
                catch (Exception ex)
                {
                    Logging.Log(Logging.LogLevel.Error, $"Open instance of Download Manager is not responding or could not be reached, not exiting. [{ex.Message} ({ex.GetType().FullName})]");
                }
            }

            bool hasUpgraded = false;

            if (!Settings.Default.SettingsAreUpgraded)
            {
                Settings.Default.Upgrade();
                Settings.Default.SettingsAreUpgraded = true;
                Settings.Default.Save();
                hasUpgraded = true;
            }

            try
            {
                Application.Run(new DownloadForm(hasUpgraded));
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached && allowBypassCrashHandler)
                {
                    throw;
                }

                // Define exception information
                string exceptionType = Convert.ToString(ex.GetType());
                string exceptionMessage = ex.Message;
                string[] exceptionStackTraceOld = ex.StackTrace.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                StringBuilder exceptionStackTraceNew = new StringBuilder();

                // Stacktrace
                foreach (string line in exceptionStackTraceOld)
                {
                    // Append new StackTrace line to new StackTrace
                    exceptionStackTraceNew.Append('"' + line + '"' + " ");
                }

                // Create CrashHandler process
                ProcessStartInfo crashInfo = new ProcessStartInfo();
                crashInfo.FileName = "CrashHandler.exe";
                crashInfo.Arguments = '"' + exceptionType + '"' + " " + '"' + exceptionMessage + '"' + " " + exceptionStackTraceNew;
                crashInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
                crashInfo.UseShellExecute = true;
                Process crashProcess = new Process();
                crashProcess.StartInfo = crashInfo;
                try
                {
                    // Start CrashHandler
                    crashProcess.Start();
                }
                catch
                {
                    // If CrashHandler failed to start, show fallback message
                    MessageBox.Show("Download Manager failed to open CrashHandler and has reverted to fallback CrashHandler.\nPlease create a bug report.\n" + ex.Message + Environment.NewLine + ex.StackTrace, "Download Manager Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // If a debugger is attached, break
                Debugger.Break();

                // Kill Download Manager
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}