using AngleSharp.Text;
using DownloadManager.NativeMethods;
using System.Diagnostics;
using System.Reflection;

namespace DownloadManager
{
    public partial class CurrentDownloads : Form
    {
        internal bool ForceActiveBar { get; set; }
        public static CurrentDownloads _instance;
        public List<DownloadItem> itemList = new List<DownloadItem>();

        public static bool firstShown = true;
        public static bool firstShownDataError = true;
        public static bool triedRefresh = false;
        public int columnsSize = 0;

        public CurrentDownloads()
        {
            _instance = this;
            InitializeComponent();
            this.ForceActiveBar = true;
            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);

            // Resize the window based on the last saved size
            this.Size = Settings.Default.currentDownloadsSize;

            // Double buffer the progressGridView control
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, progressGridView, new object[] { true });

            // Move the window to to correct location while resizing
            CurrentDownloads_Move(new object(), new EventArgs());
        }

        private void CurrentDownloads_Load(object sender, EventArgs e)
        {
            // Get a list of hidden columns
            List<Column> hiddenColumns = new List<Column>();
            foreach (string item in Settings.Default.currentDownloadsHiddenColumns)
            {
                Application.DoEvents();
                int columnType = item.ToInteger(-1);
                if (columnType == -1)
                {
                    Logging.Log("Setting 'currentDownloadsHiddenColumns' contains invalid values! Ignoring invalid values, some unexpected columns may be shown.", Color.Orange);
                }
                hiddenColumns.Add((Column)columnType);
            }

            // get a list of the shown columns
            List<Column> shownColumns = new List<Column>();
            foreach (string item in Settings.Default.currentDownloadsShownColumns)
            {
                Application.DoEvents();
                int columnType = item.ToInteger(-1);
                if (columnType == -1)
                {
                    Logging.Log("Setting 'currentDownloadsShownColumns' contains invalid values! Ignoring invalid values, some unexpected columns may be hidden.", Color.Orange);
                }
                shownColumns.Add((Column)columnType);
            }

            // Show/hide columns
            HideColumns(hiddenColumns, shownColumns);

            // Resize columns based on the new size
            int totalColumnWidth = progressGridView.Columns.Cast<DataGridViewColumn>()
            .Where(c => c.HeaderText != "File Name" && c.Visible)
            .Sum(c => c.Width);

            progressGridView.Columns[0].Width = this.Width - totalColumnWidth - 16;

            this.MinimumSize = new Size(totalColumnWidth + 209, this.MinimumSize.Height);
        }

        public void HideColumns(List<Column> hideColumns, List<Column> showColumns)
        {
            foreach (Column column in hideColumns)
            {
                Application.DoEvents();
                progressGridView.Columns[(int)column].Visible = false;
                Debug.WriteLine($"Hiding column {column}.");
            }

            foreach (Column column in showColumns)
            {
                Application.DoEvents();
                progressGridView.Columns[(int)column].Visible = true;
                if (column != Column.fileName)
                {
                    columnsSize += progressGridView.Columns[(int)column].Width;
                }
                Debug.WriteLine($"Showing column {column}.");
            }

            int totalColumnWidth = progressGridView.Columns.Cast<DataGridViewColumn>()
            .Where(c => c.HeaderText != "File Name" && c.Visible)
            .Sum(c => c.Width);

            this.MinimumSize = new Size(totalColumnWidth + 209, this.MinimumSize.Height);

            if (progressGridView.Columns[0].Visible == true)
            {
                progressGridView.Columns[0].Width = this.Width - totalColumnWidth - 16;
            }
        }

        public enum Column
        {
            fileName = 0,
            progress = 1,
            url = 2,
            received = 3,
            size = 4,
            speed = 5
        }

        public Task HideAfterFirstShow()
        {
            while (firstShown)
            {
                // wait
                Application.DoEvents();
            }

            //this.Hide();

            return Task.FromResult(true);
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == User32.WM_NCACTIVATE)
            {
                if (this.ForceActiveBar && m.WParam == IntPtr.Zero)
                {
                    User32.SendMessageW(this.Handle, User32.WM_NCACTIVATE, new IntPtr(1), IntPtr.Zero);
                }
            }
        }

        public void RefreshList()
        {
            foreach (DownloadItem item in itemList)
            {
                item.DisposeNoRemove();
            }

            itemList.Clear();
            progressGridView.Rows.Clear();

            foreach (DownloadProgress progress in DownloadForm.downloadsList)
            {
                AddItem(progress);
            }
        }

        private void AddItem(DownloadProgress progress)
        {
            DownloadItem item = new DownloadItem();
            itemList.Add(item);
            item.Initialize(progress);
        }

        private void CurrentDownloads_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            DownloadForm._instance.button2.BackColor = Color.Transparent;
            Settings.Default.showDownloadToolWindow = false;
            Settings.Default.Save();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshList();
        }

        private void CurrentDownloads_Move(object sender, EventArgs e)
        {
            if (DownloadForm._instance == null)
            {
                return;
            }

            try
            {
                // Middle of the form
                int middleY = DownloadForm._instance.Height / 2;

                // To set the currentDownloads window y position to the middle of the parent form:
                this.Location = new Point(DownloadForm._instance.Location.X + DownloadForm._instance.Width + 5, DownloadForm._instance.Location.Y + middleY - (this.Height / 2));
            }
            catch { }
        }

        private void CurrentDownloads_Shown(object sender, EventArgs e)
        {
            if (firstShown)
            {
                firstShown = false;
            }
        }

        private void progressGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception.GetType().FullName.ToString() == "System.FormatException")
            {
                // Do nothing (internal winforms fault)
                return;
            }
            else
            {
                Logging.Log($"A data error was thrown in progressGridView. This is a bug, please report at https://downloadmanager.soniczac7.rf.gd/bugReport!\n{e.Exception.Message} ({e.Exception.GetType().FullName})\n{e.Exception.StackTrace}", Color.Red);
                if (firstShownDataError)
                {
                    firstShownDataError = false;
                    MessageBox.Show($"A data error was thrown in progressGridView. This is a bug, please report at https://downloadmanager.soniczac7.rf.gd/bugReport!!\n{e.Exception.Message} ({e.Exception.GetType().FullName})\n{e.Exception.StackTrace}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            progressGridView.Refresh();
        }

        private void CurrentDownloads_Resize(object sender, EventArgs e)
        {
            // Resize columns based on the new size
            int totalColumnWidth = progressGridView.Columns.Cast<DataGridViewColumn>()
            .Where(c => c.HeaderText != "File Name" && c.Visible)
            .Sum(c => c.Width);

            progressGridView.Columns[0].Width = this.Width - totalColumnWidth - 16;

            // Move the window to to correct location while resizing
            CurrentDownloads_Move(sender, e);
        }

        private void CurrentDownloads_ResizeEnd(object sender, EventArgs e)
        {
            // Save the size of the window
            Settings.Default.currentDownloadsSize = this.Size;
            Settings.Default.Save();
        }

        private void showSelectedDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Show selected row associated download window
            if (progressGridView.SelectedRows.Count != 0)
                try
                {
                    itemList[progressGridView.SelectedRows[0].Index].progress.Show();
                    triedRefresh = false;
                }
                catch (ArgumentOutOfRangeException)
                {
                    if (triedRefresh)
                    {
                        // Item is invalid
                        Logging.Log($"Item in position ({progressGridView.SelectedRows[0].Index}) is invalid and will be removed.", Color.Orange);
                        progressGridView.Rows.RemoveAt(progressGridView.SelectedRows[0].Index);
                        return;
                    }

                    // Refresh the list and try again (a item may have finished and made the index out of range)
                    RefreshList();
                    triedRefresh = true;

                    // Try again
                    showSelectedDownloadToolStripMenuItem_Click(sender, e);
                }
            else
                new DarkMessageBox("There are no selected items!", "Download Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning).ShowDialog();
        }

        private void hideSelectedDownloadWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Hide selected row associated download window
            if (progressGridView.SelectedRows.Count != 0)
                try
                {
                    itemList[progressGridView.SelectedRows[0].Index].progress.Hide();
                    triedRefresh = false;
                }
                catch (ArgumentOutOfRangeException)
                {
                    if (triedRefresh)
                    {
                        // Item is invalid
                        Logging.Log($"Item in position ({progressGridView.SelectedRows[0].Index}) is invalid and will be removed.", Color.Orange);
                        progressGridView.Rows.RemoveAt(progressGridView.SelectedRows[0].Index);
                        return;
                    }

                    // Refresh the list and try again (a item may have finished and made the index out of range)
                    RefreshList();
                    triedRefresh = true;

                    // Try again
                    hideSelectedDownloadWindowToolStripMenuItem_Click(sender, e);
                }
            else
                new DarkMessageBox("There are no selected items!", "Download Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning).ShowDialog();
        }

        private void cancelSelectedDownloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Cancel the selected download
            if (progressGridView.SelectedRows.Count != 0)
            {
                try
                {
                    itemList[progressGridView.SelectedRows[0].Index].progress.cancelButton_Click(this, new EventArgs());
                    triedRefresh = false;
                }
                catch (ArgumentOutOfRangeException)
                {
                    if (triedRefresh)
                    {
                        // Item is invalid
                        Logging.Log($"Item in position ({progressGridView.SelectedRows[0].Index}) is invalid and will be removed.", Color.Orange);
                        progressGridView.Rows.RemoveAt(progressGridView.SelectedRows[0].Index);
                        return;
                    }

                    // Refresh the list and try again (a item may have finished and made the index out of range)
                    RefreshList();
                    triedRefresh = true;

                    // Try again
                    cancelSelectedDownloadToolStripMenuItem_Click(sender, e);
                }
            }
            else
                new DarkMessageBox("There are no selected items!", "Download Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning).ShowDialog();
        }

        private void selectColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Open column editor
            ColumnEditor editor = new ColumnEditor(this);
            editor.ShowDialog();
        }
    }
}
