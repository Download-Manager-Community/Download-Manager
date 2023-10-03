namespace DownloadManager
{
    partial class DownloadProgress
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadProgress));
            updateDisplayTimer = new System.Windows.Forms.Timer(components);
            urlLabel = new Label();
            label3 = new Label();
            checkBox1 = new CheckBox();
            cancelButton = new Button();
            checkBox2 = new CheckBox();
            hashLabel = new Label();
            splitter1 = new Splitter();
            progressBar1 = new BetterProgressBar();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            openFolderButton = new Button();
            openButton = new Button();
            pauseButton = new Button();
            bytesLabel = new Label();
            checkBox3 = new CheckBox();
            progressBar2 = new BetterProgressBar();
            totalProgressBar = new BetterProgressBar();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // updateDisplayTimer
            // 
            updateDisplayTimer.Enabled = true;
            updateDisplayTimer.Tick += updateDisplayTimer_Tick;
            // 
            // urlLabel
            // 
            urlLabel.AutoSize = true;
            urlLabel.BackColor = Color.Black;
            urlLabel.FlatStyle = FlatStyle.Flat;
            urlLabel.ForeColor = Color.White;
            urlLabel.Location = new Point(12, 102);
            urlLabel.Margin = new Padding(4, 0, 4, 0);
            urlLabel.Name = "urlLabel";
            urlLabel.Size = new Size(89, 16);
            urlLabel.TabIndex = 7;
            urlLabel.Text = "file from server";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Black;
            label3.FlatStyle = FlatStyle.Flat;
            label3.ForeColor = Color.White;
            label3.Location = new Point(13, 149);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(41, 16);
            label3.TabIndex = 9;
            label3.Text = "0.00%";
            // 
            // checkBox1
            // 
            checkBox1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            checkBox1.AutoSize = true;
            checkBox1.BackColor = Color.Black;
            checkBox1.Checked = true;
            checkBox1.CheckState = CheckState.Checked;
            checkBox1.FlatStyle = FlatStyle.Flat;
            checkBox1.ForeColor = Color.White;
            checkBox1.Location = new Point(12, 252);
            checkBox1.Margin = new Padding(4, 3, 4, 3);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(60, 20);
            checkBox1.TabIndex = 11;
            checkBox1.Text = "On top";
            checkBox1.UseVisualStyleBackColor = false;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // cancelButton
            // 
            cancelButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            cancelButton.BackColor = Color.Transparent;
            cancelButton.FlatAppearance.MouseDownBackColor = Color.Gray;
            cancelButton.FlatStyle = FlatStyle.Flat;
            cancelButton.ForeColor = Color.White;
            cancelButton.Location = new Point(340, 247);
            cancelButton.Margin = new Padding(4, 3, 4, 3);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(88, 29);
            cancelButton.TabIndex = 12;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = false;
            cancelButton.Click += cancelButton_Click;
            // 
            // checkBox2
            // 
            checkBox2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            checkBox2.AutoSize = true;
            checkBox2.BackColor = Color.Black;
            checkBox2.FlatStyle = FlatStyle.Flat;
            checkBox2.ForeColor = Color.White;
            checkBox2.Location = new Point(13, 214);
            checkBox2.Margin = new Padding(4, 3, 4, 3);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(290, 20);
            checkBox2.TabIndex = 13;
            checkBox2.Text = "Close this window when the download completes";
            checkBox2.UseVisualStyleBackColor = false;
            // 
            // hashLabel
            // 
            hashLabel.AutoSize = true;
            hashLabel.BackColor = Color.Black;
            hashLabel.FlatStyle = FlatStyle.Flat;
            hashLabel.ForeColor = Color.White;
            hashLabel.Location = new Point(13, 165);
            hashLabel.Name = "hashLabel";
            hashLabel.Size = new Size(161, 16);
            hashLabel.TabIndex = 15;
            hashLabel.Text = "No file verification (No hash)";
            // 
            // splitter1
            // 
            splitter1.Dock = DockStyle.Bottom;
            splitter1.Location = new Point(0, 288);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(439, 1);
            splitter1.TabIndex = 16;
            splitter1.TabStop = false;
            // 
            // progressBar1
            // 
            progressBar1.BackColor = Color.FromArgb(20, 20, 20);
            progressBar1.Location = new Point(12, 122);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(207, 25);
            progressBar1.TabIndex = 18;
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.Image = Properties.Resources.fileTransfer;
            pictureBox1.Location = new Point(12, 13);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(285, 65);
            pictureBox1.TabIndex = 19;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 81);
            label1.Name = "label1";
            label1.Size = new Size(82, 16);
            label1.TabIndex = 20;
            label1.Text = "Downloading:";
            // 
            // openFolderButton
            // 
            openFolderButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            openFolderButton.BackColor = Color.Transparent;
            openFolderButton.FlatAppearance.MouseDownBackColor = Color.Gray;
            openFolderButton.FlatStyle = FlatStyle.Flat;
            openFolderButton.ForeColor = Color.White;
            openFolderButton.Location = new Point(244, 247);
            openFolderButton.Margin = new Padding(4, 3, 4, 3);
            openFolderButton.Name = "openFolderButton";
            openFolderButton.Size = new Size(88, 29);
            openFolderButton.TabIndex = 21;
            openFolderButton.Text = "Open Folder";
            openFolderButton.UseVisualStyleBackColor = false;
            openFolderButton.Click += openFolderButton_Click;
            // 
            // openButton
            // 
            openButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            openButton.BackColor = Color.Transparent;
            openButton.Enabled = false;
            openButton.FlatAppearance.MouseDownBackColor = Color.Gray;
            openButton.FlatStyle = FlatStyle.Flat;
            openButton.ForeColor = Color.White;
            openButton.Location = new Point(148, 247);
            openButton.Margin = new Padding(4, 3, 4, 3);
            openButton.Name = "openButton";
            openButton.Size = new Size(88, 29);
            openButton.TabIndex = 22;
            openButton.Text = "Open";
            openButton.UseVisualStyleBackColor = false;
            openButton.Click += openButton_Click;
            // 
            // pauseButton
            // 
            pauseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            pauseButton.BackColor = Color.Transparent;
            pauseButton.FlatAppearance.MouseDownBackColor = Color.Gray;
            pauseButton.FlatStyle = FlatStyle.Flat;
            pauseButton.ForeColor = Color.White;
            pauseButton.Location = new Point(340, 212);
            pauseButton.Margin = new Padding(4, 3, 4, 3);
            pauseButton.Name = "pauseButton";
            pauseButton.Size = new Size(88, 29);
            pauseButton.TabIndex = 23;
            pauseButton.Text = "Pause";
            pauseButton.UseVisualStyleBackColor = false;
            pauseButton.Click += pauseButton_Click;
            // 
            // bytesLabel
            // 
            bytesLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bytesLabel.Location = new Point(220, 149);
            bytesLabel.Name = "bytesLabel";
            bytesLabel.Size = new Size(206, 16);
            bytesLabel.TabIndex = 24;
            bytesLabel.Text = "(0 B / 0 B)";
            bytesLabel.TextAlign = ContentAlignment.MiddleRight;
            // 
            // checkBox3
            // 
            checkBox3.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            checkBox3.AutoSize = true;
            checkBox3.BackColor = Color.Black;
            checkBox3.FlatStyle = FlatStyle.Flat;
            checkBox3.ForeColor = Color.White;
            checkBox3.Location = new Point(13, 188);
            checkBox3.Margin = new Padding(4, 3, 4, 3);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(265, 20);
            checkBox3.TabIndex = 25;
            checkBox3.Text = "Open this file when the download completes";
            checkBox3.UseVisualStyleBackColor = false;
            // 
            // progressBar2
            // 
            progressBar2.BackColor = Color.FromArgb(20, 20, 20);
            progressBar2.Location = new Point(221, 122);
            progressBar2.Name = "progressBar2";
            progressBar2.Size = new Size(207, 25);
            progressBar2.TabIndex = 26;
            // 
            // totalProgressBar
            // 
            totalProgressBar.BackColor = Color.FromArgb(20, 20, 20);
            totalProgressBar.Location = new Point(12, 122);
            totalProgressBar.Name = "totalProgressBar";
            totalProgressBar.Size = new Size(414, 25);
            totalProgressBar.TabIndex = 27;
            totalProgressBar.Visible = false;
            // 
            // DownloadProgress
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(439, 289);
            Controls.Add(progressBar2);
            Controls.Add(checkBox3);
            Controls.Add(bytesLabel);
            Controls.Add(pauseButton);
            Controls.Add(openButton);
            Controls.Add(openFolderButton);
            Controls.Add(label1);
            Controls.Add(pictureBox1);
            Controls.Add(progressBar1);
            Controls.Add(splitter1);
            Controls.Add(hashLabel);
            Controls.Add(checkBox2);
            Controls.Add(cancelButton);
            Controls.Add(checkBox1);
            Controls.Add(label3);
            Controls.Add(urlLabel);
            Controls.Add(totalProgressBar);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "DownloadProgress";
            StartPosition = FormStartPosition.Manual;
            Text = "Downloading file... (0%)";
            TopMost = true;
            FormClosing += DownloadProgress_FormClosing;
            Load += progress_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Timer updateDisplayTimer;
        private System.Windows.Forms.Label urlLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.CheckBox checkBox2;
        private Label hashLabel;
        private Splitter splitter1;
        private Button openFolderButton;
        private PictureBox pictureBox1;
        private Label label1;
        private Button openButton;
        private Button pauseButton;
        private Label bytesLabel;
        public BetterProgressBar progressBar1;
        private CheckBox checkBox3;
        public BetterProgressBar progressBar2;
        public BetterProgressBar totalProgressBar;
    }
}