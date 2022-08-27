using System.Runtime.InteropServices;

namespace CrashHandler
{
    public partial class CrashMessage : Form
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

        public CrashMessage(string[] ex)
        {
            InitializeComponent();

            // Populate exception
            textBox1.Text = ex[0];

            // Populate message
            textBox2.Text = ex[1];

            // Populate stacktrace
            foreach (var line in ex)
            {
                // If lines are exception or message
                if (line == ex[0] || line == ex[1])
                {
                    // Do nothing
                    continue;
                }
                // Else output to stacktrace
                richTextBox1.Text += line + "\n";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Open bug report page
            try
            {
                // Start browser
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://github.com/Soniczac7/Download-Manager/issues/new?assignees=&labels=bug&template=bug_report.yml") { UseShellExecute = true });
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                {
                    // System cannot find the file specified
                    MessageBox.Show(noBrowser.Message);
                }
                else
                {
                    // Other error
                    MessageBox.Show("Cannot automatically open bug report page in your browser. Please visit:\nhttps://github.com/Soniczac7/Download-Manager/issues/new/choose/", "CrashHandler Error");
                }
            }
            catch (System.Exception other)
            {
                // Unknown error
                MessageBox.Show("Cannot automatically open bug report page in your browser (" + other.Message + "). Please visit:\nhttps://github.com/Soniczac7/Download-Manager/issues/new/choose/", "CrashHandler Error");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Exit
            Application.Exit();
        }
    }
}