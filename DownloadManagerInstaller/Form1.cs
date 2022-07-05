namespace DownloadManagerInstaller
{
    public partial class Form1 : Form
    {
        int installStage = 1;

        public Form1()
        {
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
                installStage += 1;
            }
            else if (installStage == 2)
            {
                tabControl2.Visible = false;
                groupBox1.Visible = true;
                installStage += 1;
            }
        }
    }
}