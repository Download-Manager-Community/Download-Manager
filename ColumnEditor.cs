using DownloadManager.NativeMethods;
using static DownloadManager.CurrentDownloads;

namespace DownloadManager
{
    public partial class ColumnEditor : Form
    {
        CurrentDownloads downloads;

        public ColumnEditor(CurrentDownloads downloads)
        {
            InitializeComponent();

            this.downloads = downloads;

            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);

            foreach (string item in Settings.Default.currentDownloadsShownColumns)
            {
                Application.DoEvents();
                switch (item)
                {
                    case "0":
                        showList.Items.Add("File Name");
                        break;
                    case "1":
                        showList.Items.Add("Progress");
                        break;
                    case "2":
                        showList.Items.Add("URL");
                        break;
                    case "3":
                        showList.Items.Add("Received");
                        break;
                    case "4":
                        showList.Items.Add("Size");
                        break;
                    default:
                        // Invalid column
                        Logging.Log("Found invalid column while creating list of shown columns!", Color.Red);
                        showList.Items.Add("<unknown>");
                        break;
                }
            }

            foreach (string item in Settings.Default.currentDownloadsHiddenColumns)
            {
                Application.DoEvents();

                if (Settings.Default.currentDownloadsShownColumns.Contains(item))
                {
                    Logging.Log("Duplicate column found in shown columns while making list of hidden columns. The item will be removed.", Color.Orange);
                    Settings.Default.currentDownloadsHiddenColumns.Remove(item);
                    Settings.Default.Save();
                    break;
                }

                switch (item)
                {
                    case "0":
                        hideList.Items.Add("File Name");
                        break;
                    case "1":
                        hideList.Items.Add("Progress");
                        break;
                    case "2":
                        hideList.Items.Add("URL");
                        break;
                    case "3":
                        hideList.Items.Add("Received");
                        break;
                    case "4":
                        hideList.Items.Add("Size");
                        break;
                    default:
                        // Invalid column
                        Logging.Log("Found invalid column while creating list of shown columns!", Color.Red);
                        hideList.Items.Add("<unknown>");
                        break;
                }
            }
        }

        private void doneButton_Click(object sender, EventArgs e)
        {
            // Create list of shown columns
            Settings.Default.currentDownloadsShownColumns.Clear();

            List<Column> shownColumns = new List<Column>();
            foreach (string item in showList.Items)
            {
                switch (item)
                {
                    case "File Name":
                        shownColumns.Add(Column.fileName);
                        Settings.Default.currentDownloadsShownColumns.Add(((int)Column.fileName).ToString());
                        break;
                    case "Progress":
                        shownColumns.Add(Column.progress);
                        Settings.Default.currentDownloadsShownColumns.Add(((int)Column.progress).ToString());
                        break;
                    case "URL":
                        shownColumns.Add(Column.url);
                        Settings.Default.currentDownloadsShownColumns.Add(((int)Column.url).ToString());
                        break;
                    case "Received":
                        shownColumns.Add(Column.received);
                        Settings.Default.currentDownloadsShownColumns.Add(((int)Column.received).ToString());
                        break;
                    case "Size":
                        shownColumns.Add(Column.size);
                        Settings.Default.currentDownloadsShownColumns.Add(((int)Column.size).ToString());
                        break;
                    default:
                        // Invalid column
                        Logging.Log("Found invalid column while creating list of shown columns! The item will be ignored.", Color.Orange);
                        break;
                }
            }

            Settings.Default.currentDownloadsHiddenColumns.Clear();

            List<Column> hiddenColumns = new List<Column>();
            foreach (string item in hideList.Items)
            {
                switch (item)
                {
                    case "File Name":
                        hiddenColumns.Add(Column.fileName);
                        Settings.Default.currentDownloadsHiddenColumns.Add(((int)Column.fileName).ToString());
                        break;
                    case "Progress":
                        hiddenColumns.Add(Column.progress);
                        Settings.Default.currentDownloadsHiddenColumns.Add(((int)Column.progress).ToString());
                        break;
                    case "URL":
                        hiddenColumns.Add(Column.url);
                        Settings.Default.currentDownloadsHiddenColumns.Add(((int)Column.url).ToString());
                        break;
                    case "Received":
                        hiddenColumns.Add(Column.received);
                        Settings.Default.currentDownloadsHiddenColumns.Add(((int)Column.received).ToString());
                        break;
                    case "Size":
                        hiddenColumns.Add(Column.size);
                        Settings.Default.currentDownloadsHiddenColumns.Add(((int)Column.size).ToString());
                        break;
                    default:
                        // Invalid column
                        Logging.Log("Found invalid column while creating list of hidden columns! The item will be ignored.", Color.Orange);
                        break;
                }
            }

            Settings.Default.Save();
            downloads.HideColumns(hiddenColumns, shownColumns);

            this.Close();
        }

        private void showButton_Click(object sender, EventArgs e)
        {
            // Move item to shown list
            if (hideList.SelectedIndex == -1)
            {
                // Do nothing
                return;
            }

            showList.Items.Add(hideList.Items[hideList.SelectedIndex]);
            hideList.Items.RemoveAt(hideList.SelectedIndex);
        }

        private void hideButton_Click(object sender, EventArgs e)
        {
            // Hide item to hide list
            if (showList.SelectedIndex == -1)
            {
                // Do nothing
                return;
            }

            hideList.Items.Add(showList.Items[showList.SelectedIndex]);
            showList.Items.RemoveAt(showList.SelectedIndex);
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            DialogResult result = new DarkMessageBox("Are you sure you want to reset your column preferences?\nThis should only be used if new columns added in an update are not showing or you are experiencing issues with hiding/showing columns.", "Reset Column Preferences?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, false).ShowDialog();
            if (result == DialogResult.Yes)
            {
                DownloadForm._instance.SetupColumnPrefs();

                showList.Items.Clear();
                hideList.Items.Clear();

                foreach (string item in Settings.Default.currentDownloadsShownColumns)
                {
                    Application.DoEvents();
                    switch (item)
                    {
                        case "0":
                            showList.Items.Add("File Name");
                            break;
                        case "1":
                            showList.Items.Add("Progress");
                            break;
                        case "2":
                            showList.Items.Add("URL");
                            break;
                        case "3":
                            showList.Items.Add("Received");
                            break;
                        case "4":
                            showList.Items.Add("Size");
                            break;
                        default:
                            // Invalid column
                            Logging.Log("Found invalid column while creating list of shown columns!", Color.Red);
                            showList.Items.Add("<unknown>");
                            break;
                    }
                }

                foreach (string item in Settings.Default.currentDownloadsHiddenColumns)
                {
                    Application.DoEvents();

                    if (Settings.Default.currentDownloadsShownColumns.Contains(item))
                    {
                        Logging.Log("Duplicate column found in shown columns while making list of hidden columns. The item will be removed.", Color.Orange);
                        Settings.Default.currentDownloadsHiddenColumns.Remove(item);
                        Settings.Default.Save();
                        break;
                    }

                    switch (item)
                    {
                        case "0":
                            hideList.Items.Add("File Name");
                            break;
                        case "1":
                            hideList.Items.Add("Progress");
                            break;
                        case "2":
                            hideList.Items.Add("URL");
                            break;
                        case "3":
                            hideList.Items.Add("Received");
                            break;
                        case "4":
                            hideList.Items.Add("Size");
                            break;
                        default:
                            // Invalid column
                            Logging.Log("Found invalid column while creating list of shown columns!", Color.Red);
                            hideList.Items.Add("<unknown>");
                            break;
                    }
                }

                downloads.HideColumns(new List<Column> { Column.received }, new List<Column> { Column.fileName, Column.progress, Column.size, Column.url });
            }
        }
    }
}