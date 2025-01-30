namespace DownloadManager.Components
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();
        }

        // Crash test
        public void button1_Click(object sender, EventArgs e)
        {
            throw new Exception("Manually Initiated Crash");
        }

        // Allow bypassing crash handler with debugger attached (default: true)
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Program.allowBypassCrashHandler = checkBox1.Checked;
        }
    }
}
