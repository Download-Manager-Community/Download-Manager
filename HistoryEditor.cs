using System.Runtime.InteropServices;

namespace DownloadManager
{
    public partial class HistoryEditor : Form
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

        public HistoryEditor()
        {
            InitializeComponent();

            if (Settings1.Default.downloadHistory != null)
            {
                foreach (var item in Settings1.Default.downloadHistory)
                {
                    Application.DoEvents();
                    listBox1.Items.Add(item);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            // Remove Item
            if (listBox1.Items.Count < 1)
            {
                MessageBox.Show("Cannot remove an item that does not exist!", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (listBox1.SelectedIndex == -1)
            {
                MessageBox.Show("No item selected.", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Settings1.Default.downloadHistory.RemoveAt(listBox1.SelectedIndex);
                Settings1.Default.Save();
                DownloadForm._instance.textBox1.Items.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Refresh
            listBox1.Items.Clear();
            if (Settings1.Default.downloadHistory != null)
            {
                foreach (var item in Settings1.Default.downloadHistory)
                {
                    Application.DoEvents();
                    listBox1.Items.Add(item);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Clear
            Settings1.Default.downloadHistory.Clear();
            Settings1.Default.Save();
            DownloadForm._instance.textBox1.Items.Clear();
            listBox1.Items.Clear();
        }
    }
}
