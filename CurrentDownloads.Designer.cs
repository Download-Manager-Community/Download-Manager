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
            ((System.ComponentModel.ISupportInitialize)progressGridView).BeginInit();
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
            progressGridView.Location = new Point(0, 0);
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
            progressGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            progressGridView.ShowCellErrors = false;
            progressGridView.ShowCellToolTips = false;
            progressGridView.ShowEditingIcon = false;
            progressGridView.ShowRowErrors = false;
            progressGridView.Size = new Size(479, 480);
            progressGridView.TabIndex = 1;
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
            // CurrentDownloads
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(479, 480);
            Controls.Add(progressGridView);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(480, 400);
            Name = "CurrentDownloads";
            Text = "Donwload Manager | Current Downloads";
            FormClosing += CurrentDownloads_FormClosing;
            Shown += CurrentDownloads_Shown;
            ResizeEnd += CurrentDownloads_ResizeEnd;
            Move += CurrentDownloads_Move;
            Resize += CurrentDownloads_Resize;
            ((System.ComponentModel.ISupportInitialize)progressGridView).EndInit();
            ResumeLayout(false);
        }

        #endregion
        private Panel panel2;
        private Button refreshButton;
        private Splitter splitter1;
        public Panel panel1;
        public DataGridView progressGridView;
        private System.Windows.Forms.Timer timer;
        private DataGridViewTextBoxColumn FileNameColumn;
        private Controls.DataGridViewProgressColumn ProgressColumn;
        private DataGridViewLinkColumn UrlColumn;
        private DataGridViewTextBoxColumn SizeColumn;
    }
}