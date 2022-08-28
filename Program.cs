using System.Diagnostics;

namespace DownloadManager
{
    internal static class Program
    {
        private static bool DEBUG = false;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]

        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            if (DEBUG == true)
            {
                MessageBox.Show("Debug mode is on. You can now attach a debugger.\nTo turn it off, go to Program.cs and set DEBUG to false.\nPress OK to continue...", "Download Manager - DEBUG", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            try
            {
                Application.Run(new DownloadForm());
            }
            catch (Exception ex)
            {
                DarkMessageBox msg = new DarkMessageBox("An unhanded exception occurred and Download Manager has to restart.\nPlease file a bug report at: https://github.com/Download-Manager-Community/Download-Manager/issues/new?assignees=&labels=bug&template=bug_report.md&title=\nError details:\n" + ex.Message + Environment.NewLine + ex.StackTrace, "Download Manager - Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error, false);
                msg.ShowDialog();
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}