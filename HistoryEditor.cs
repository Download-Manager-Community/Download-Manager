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

            if (Settings.Default.downloadHistory != null)
            {
                foreach (var item in Settings.Default.downloadHistory)
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
                DarkMessageBox msg = new DarkMessageBox("Cannot remove an item that does not exist!", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                msg.ShowDialog();
            }
            else if (listBox1.SelectedIndex == -1)
            {
                DarkMessageBox msg = new DarkMessageBox("No item selected.", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                msg.ShowDialog();
            }
            else
            {
                Settings.Default.downloadHistory.RemoveAt(listBox1.SelectedIndex);
                Settings.Default.Save();
                DownloadForm._instance.textBox1.Items.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            // Refresh
            listBox1.Items.Clear();
            if (Settings.Default.downloadHistory != null)
            {
                foreach (var item in Settings.Default.downloadHistory)
                {
                    Application.DoEvents();
                    listBox1.Items.Add(item);
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            // Clear
            Settings.Default.downloadHistory.Clear();
            Settings.Default.Save();
            DownloadForm._instance.textBox1.Items.Clear();
            listBox1.Items.Clear();
        }

        private void HistoryEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
