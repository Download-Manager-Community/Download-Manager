namespace DownloadManager.Components.Addons.YouTubeDownloader
{
    partial class YouTubeOptions
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YouTubeOptions));
            descLabel = new Label();
            downloadButton = new Button();
            urlBox = new TextBox();
            label1 = new Label();
            convertCheckBox = new CheckBox();
            help1 = new Label();
            toolTip = new ToolTip(components);
            progressBar = new BetterProgressBar();
            SuspendLayout();
            // 
            // descLabel
            // 
            descLabel.AutoSize = true;
            descLabel.Location = new Point(12, 9);
            descLabel.Name = "descLabel";
            descLabel.Size = new Size(421, 48);
            descLabel.TabIndex = 0;
            descLabel.Text = "Please enter your desired options and press \"Download\" when you are ready.\r\nTo download, YouTube videos and playlists must be set to public or unlisted.\r\nPlaylist downloading is not yet implemented.";
            // 
            // downloadButton
            // 
            downloadButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            downloadButton.BackColor = Color.Black;
            downloadButton.FlatAppearance.MouseDownBackColor = Color.Gray;
            downloadButton.FlatStyle = FlatStyle.Flat;
            downloadButton.ForeColor = Color.White;
            downloadButton.Location = new Point(345, 109);
            downloadButton.Margin = new Padding(4, 3, 4, 3);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new Size(88, 29);
            downloadButton.TabIndex = 2;
            downloadButton.Text = "Download";
            downloadButton.UseVisualStyleBackColor = false;
            downloadButton.Click += downloadButton_Click;
            downloadButton.Paint += downloadButton_Paint;
            // 
            // urlBox
            // 
            urlBox.BackColor = Color.Black;
            urlBox.BorderStyle = BorderStyle.FixedSingle;
            urlBox.ForeColor = Color.White;
            urlBox.Location = new Point(64, 72);
            urlBox.Margin = new Padding(4, 3, 4, 3);
            urlBox.Name = "urlBox";
            urlBox.Size = new Size(370, 23);
            urlBox.TabIndex = 16;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(29, 74);
            label1.Name = "label1";
            label1.Size = new Size(28, 16);
            label1.TabIndex = 17;
            label1.Text = "URL";
            // 
            // convertCheckBox
            // 
            convertCheckBox.AutoSize = true;
            convertCheckBox.Location = new Point(12, 101);
            convertCheckBox.Name = "convertCheckBox";
            convertCheckBox.RightToLeft = RightToLeft.Yes;
            convertCheckBox.Size = new Size(69, 20);
            convertCheckBox.TabIndex = 18;
            convertCheckBox.Text = "Convert";
            convertCheckBox.UseVisualStyleBackColor = true;
            convertCheckBox.Paint += convertCheckBox_Paint;
            // 
            // help1
            // 
            help1.AutoSize = true;
            help1.Cursor = Cursors.Help;
            help1.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            help1.ForeColor = Color.Gray;
            help1.Location = new Point(87, 103);
            help1.Name = "help1";
            help1.Size = new Size(12, 15);
            help1.TabIndex = 19;
            help1.Text = "?";
            toolTip.SetToolTip(help1, "Converts the YouTube video to an MP3 automatically.\r\nAll conversion happens locally and no data is sent to third parties.");
            // 
            // toolTip
            // 
            toolTip.BackColor = Color.Black;
            toolTip.ForeColor = Color.White;
            toolTip.IsBalloon = true;
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.ToolTipTitle = "What's This?";
            // 
            // progressBar
            // 
            progressBar.BackColor = Color.FromArgb(20, 20, 20);
            progressBar.CustomText = "Status: Waiting...";
            progressBar.Dock = DockStyle.Bottom;
            progressBar.Location = new Point(0, 147);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(447, 15);
            progressBar.TabIndex = 20;
            progressBar.TextAlign = StringAlignment.Near;
            progressBar.TextIgnoresMarquee = true;
            // 
            // YouTubeOptions
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(447, 162);
            Controls.Add(progressBar);
            Controls.Add(help1);
            Controls.Add(convertCheckBox);
            Controls.Add(label1);
            Controls.Add(urlBox);
            Controls.Add(downloadButton);
            Controls.Add(descLabel);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "YouTubeOptions";
            Text = "YouTube Download";
            TopMost = true;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label descLabel;
        private Button downloadButton;
        private TextBox urlBox;
        private Label label1;
        private CheckBox convertCheckBox;
        private Label help1;
        private ToolTip toolTip;
        public BetterProgressBar progressBar;
    }
}