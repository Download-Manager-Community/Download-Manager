namespace DownloadManager
{
    partial class CurrentDownloads
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CurrentDownloads));
            progressGridView = new DataGridView();
            FileNameColumn = new DataGridViewTextBoxColumn();
            ProgressColumn = new Controls.DataGridViewProgressColumn();
            UrlColumn = new DataGridViewLinkColumn();
            SizeColumn = new DataGridViewTextBoxColumn();
            timer = new System.Windows.Forms.Timer(components);
            menuStrip1 = new MenuStrip();
            columnsToolStripMenuItem = new ToolStripMenuItem();
            selectColumnsToolStripMenuItem = new ToolStripMenuItem();
            actionsToolStripMenuItem = new ToolStripMenuItem();
            showSelectedDownloadToolStripMenuItem = new ToolStripMenuItem();
            hideSelectedDownloadWindowToolStripMenuItem = new ToolStripMenuItem();
            cancelSelectedDownloadToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)progressGridView).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // progressGridView
            // 
            progressGridView.AllowUserToAddRows = false;
            progressGridView.AllowUserToDeleteRows = false;
            progressGridView.AllowUserToResizeColumns = false;
            progressGridView.AllowUserToResizeRows = false;
            progressGridView.BackgroundColor = Color.Black;
            progressGridView.BorderStyle = BorderStyle.None;
            progressGridView.CellBorderStyle = DataGridViewCellBorderStyle.Sunken;
            progressGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            progressGridView.Columns.AddRange(new DataGridViewColumn[] { FileNameColumn, ProgressColumn, UrlColumn, SizeColumn });
            progressGridView.Dock = DockStyle.Fill;
            progressGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            progressGridView.GridColor = Color.Gray;
            progressGridView.Location = new Point(0, 24);
            progressGridView.MultiSelect = false;
            progressGridView.Name = "progressGridView";
            progressGridView.ReadOnly = true;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.Black;
            dataGridViewCellStyle1.Font = new Font("Segoe UI Variable Small", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = Color.White;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
            progressGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            progressGridView.RowHeadersVisible = false;
            progressGridView.RowTemplate.Height = 25;
            progressGridView.ScrollBars = ScrollBars.None;
            progressGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            progressGridView.ShowCellErrors = false;
            progressGridView.ShowCellToolTips = false;
            progressGridView.ShowEditingIcon = false;
            progressGridView.ShowRowErrors = false;
            progressGridView.Size = new Size(479, 456);
            progressGridView.TabIndex = 1;
            progressGridView.CellContentClick += progressGridView_CellContentClick;
            progressGridView.DataError += progressGridView_DataError;
            // 
            // FileNameColumn
            // 
            FileNameColumn.HeaderText = "File Name";
            FileNameColumn.Name = "FileNameColumn";
            FileNameColumn.ReadOnly = true;
            FileNameColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            FileNameColumn.Width = 209;
            // 
            // ProgressColumn
            // 
            ProgressColumn.HeaderText = "Progress";
            ProgressColumn.Name = "ProgressColumn";
            ProgressColumn.ReadOnly = true;
            ProgressColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            // 
            // UrlColumn
            // 
            UrlColumn.HeaderText = "URL";
            UrlColumn.Name = "UrlColumn";
            UrlColumn.ReadOnly = true;
            UrlColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            UrlColumn.Width = 110;
            // 
            // SizeColumn
            // 
            SizeColumn.HeaderText = "Size";
            SizeColumn.Name = "SizeColumn";
            SizeColumn.ReadOnly = true;
            SizeColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            SizeColumn.Width = 60;
            // 
            // timer
            // 
            timer.Enabled = true;
            timer.Interval = 200;
            timer.Tick += timer_Tick;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { columnsToolStripMenuItem, actionsToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.RenderMode = ToolStripRenderMode.System;
            menuStrip1.Size = new Size(479, 24);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip";
            // 
            // columnsToolStripMenuItem
            // 
            columnsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { selectColumnsToolStripMenuItem });
            columnsToolStripMenuItem.Name = "columnsToolStripMenuItem";
            columnsToolStripMenuItem.Size = new Size(67, 20);
            columnsToolStripMenuItem.Text = "Columns";
            // 
            // selectColumnsToolStripMenuItem
            // 
            selectColumnsToolStripMenuItem.Name = "selectColumnsToolStripMenuItem";
            selectColumnsToolStripMenuItem.Size = new Size(197, 22);
            selectColumnsToolStripMenuItem.Text = "Add/Remove Columns";
            selectColumnsToolStripMenuItem.Click += selectColumnsToolStripMenuItem_Click;
            // 
            // actionsToolStripMenuItem
            // 
            actionsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { showSelectedDownloadToolStripMenuItem, hideSelectedDownloadWindowToolStripMenuItem, cancelSelectedDownloadToolStripMenuItem });
            actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            actionsToolStripMenuItem.Size = new Size(59, 20);
            actionsToolStripMenuItem.Text = "Actions";
            // 
            // showSelectedDownloadToolStripMenuItem
            // 
            showSelectedDownloadToolStripMenuItem.Name = "showSelectedDownloadToolStripMenuItem";
            showSelectedDownloadToolStripMenuItem.Size = new Size(260, 22);
            showSelectedDownloadToolStripMenuItem.Text = "Show Selected Download Window";
            showSelectedDownloadToolStripMenuItem.Click += showSelectedDownloadToolStripMenuItem_Click;
            // 
            // hideSelectedDownloadWindowToolStripMenuItem
            // 
            hideSelectedDownloadWindowToolStripMenuItem.Name = "hideSelectedDownloadWindowToolStripMenuItem";
            hideSelectedDownloadWindowToolStripMenuItem.Size = new Size(260, 22);
            hideSelectedDownloadWindowToolStripMenuItem.Text = "Hide Selected Download Window";
            hideSelectedDownloadWindowToolStripMenuItem.Click += hideSelectedDownloadWindowToolStripMenuItem_Click;
            // 
            // cancelSelectedDownloadToolStripMenuItem
            // 
            cancelSelectedDownloadToolStripMenuItem.Name = "cancelSelectedDownloadToolStripMenuItem";
            cancelSelectedDownloadToolStripMenuItem.Size = new Size(260, 22);
            cancelSelectedDownloadToolStripMenuItem.Text = "Cancel Selected Download";
            cancelSelectedDownloadToolStripMenuItem.Click += cancelSelectedDownloadToolStripMenuItem_Click;
            // 
            // CurrentDownloads
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(479, 480);
            Controls.Add(progressGridView);
            Controls.Add(menuStrip1);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(495, 400);
            Name = "CurrentDownloads";
            Text = "Donwload Manager | Current Downloads";
            FormClosing += CurrentDownloads_FormClosing;
            Load += CurrentDownloads_Load;
            Shown += CurrentDownloads_Shown;
            ResizeEnd += CurrentDownloads_ResizeEnd;
            Move += CurrentDownloads_Move;
            Resize += CurrentDownloads_Resize;
            ((System.ComponentModel.ISupportInitialize)progressGridView).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Panel panel2;
        private Button refreshButton;
        private Splitter splitter1;
        public Panel panel1;
        public DataGridView progressGridView;
        private System.Windows.Forms.Timer timer;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem columnsToolStripMenuItem;
        private ToolStripMenuItem selectColumnsToolStripMenuItem;
        private ToolStripMenuItem actionsToolStripMenuItem;
        private ToolStripMenuItem showSelectedDownloadToolStripMenuItem;
        private ToolStripMenuItem hideSelectedDownloadWindowToolStripMenuItem;
        private ToolStripMenuItem cancelSelectedDownloadToolStripMenuItem;
        private DataGridViewTextBoxColumn FileNameColumn;
        private Controls.DataGridViewProgressColumn ProgressColumn;
        private DataGridViewLinkColumn UrlColumn;
        private DataGridViewTextBoxColumn SizeColumn;
    }
}