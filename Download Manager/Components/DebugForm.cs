namespace DownloadManager.Components
{
    public partial class DebugForm : Form
    {
        private bool allowClose = true;

        public DebugForm()
        {
            InitializeComponent();
        }

        #region Crash Handler
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
        #endregion

        #region DarkMessageBox
        // Test MessageBox
        private void button3_Click(object sender, EventArgs e)
        {
            MessageBoxIcon? icon = null;

            int item = listBox2.SelectedIndex;
            if (item == 0)
                icon = MessageBoxIcon.None;
            else if (item == 1)
                icon = MessageBoxIcon.Information;
            else if (item == 2)
                icon = MessageBoxIcon.Question;
            else if (item == 3)
                icon = MessageBoxIcon.Warning;
            else if (item == 4)
                icon = MessageBoxIcon.Error;

            DialogResult result = DarkMessageBox.Show("This is a test message box.", "Download Manager - Debug", (MessageBoxButtons)listBox1.SelectedIndex, icon, checkBox2.Checked);

            label1.Text = $"Result: DialogResult.{result.ToString()}";
        }

        // Change state of allow close
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            allowClose = checkBox2.Checked;
        }

        // Clear result
        private void button4_Click(object sender, EventArgs e)
        {
            label1.Text = "Result: DialogResult.None";
        }
        #endregion
    }
}
