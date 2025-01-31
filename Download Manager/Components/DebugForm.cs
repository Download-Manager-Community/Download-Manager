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

        // Crash test extended
        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                throw new Exception("This is a test inner exception.\nTest text.");
            }
            catch (Exception ex)
            {
                throw new Exception("Manually Initiated Crash\nLine 2.\nLine 3\nLine 4", ex);
            }
        }

        // Allow bypassing crash handler with debugger attached (default: true)
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Program.allowBypassCrashHandler = checkBox1.Checked;
        }
    }
}
