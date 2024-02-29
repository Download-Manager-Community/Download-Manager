namespace DownloadManager
{
    partial class DownloadForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DownloadForm));
            label1 = new Label();
            textBox2 = new TextBox();
            label2 = new Label();
            button3 = new Button();
            button4 = new Button();
            folderBrowserDialog = new FolderBrowserDialog();
            downloadTimer = new System.Windows.Forms.Timer(components);
            label3 = new Label();
            trayIcon = new NotifyIcon(components);
            trayContextMenu = new ContextMenuStrip(components);
            toolStripTextBox1 = new ToolStripTextBox();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            linkLabel1 = new LinkLabel();
            textBox3 = new TextBox();
            label4 = new Label();
            label5 = new Label();
            toolTip = new ToolTip(components);
            openFileDialog = new OpenFileDialog();
            comboBox1 = new ComboBox();
            textBox1 = new ComboBox();
            toolStripButton2 = new Button();
            toolStripButton3 = new Button();
            toolStripButton1 = new Button();
            panel1 = new Panel();
            button2 = new Button();
            progressBar1 = new BetterProgressBar();
            videoDownloadOptions = new GroupBox();
            playlistViewButton = new Button();
            videoThumb = new Microsoft.Web.WebView2.WinForms.WebView2();
            videoDownloadTypeComboBox = new ComboBox();
            videoDurationLabel = new Label();
            videoDateLabel = new Label();
            videoChannelLabel = new Label();
            videoTitleLabel = new Label();
            videoDuration = new Label();
            videoDate = new Label();
            videoChannel = new Label();
            videoTitle = new Label();
            videoErrorBox = new GroupBox();
            videoErrorText = new RichTextBox();
            videoThumbErrorTip = new ToolTip(components);
            trayContextMenu.SuspendLayout();
            panel1.SuspendLayout();
            videoDownloadOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)videoThumb).BeginInit();
            videoErrorBox.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.BackColor = Color.Black;
            label1.ForeColor = Color.White;
            label1.Location = new Point(10, 54);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(86, 16);
            label1.TabIndex = 4;
            label1.Text = "Download URL";
            // 
            // textBox2
            // 
            textBox2.BackColor = Color.Black;
            textBox2.BorderStyle = BorderStyle.FixedSingle;
            textBox2.ForeColor = Color.White;
            textBox2.Location = new Point(14, 134);
            textBox2.Margin = new Padding(4, 3, 4, 3);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(868, 23);
            textBox2.TabIndex = 8;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.BackColor = Color.Black;
            label2.ForeColor = Color.White;
            label2.Location = new Point(10, 114);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(111, 16);
            label2.TabIndex = 7;
            label2.Text = "Download Location";
            // 
            // button3
            // 
            button3.BackColor = Color.Black;
            button3.FlatAppearance.MouseDownBackColor = Color.Gray;
            button3.FlatStyle = FlatStyle.Flat;
            button3.ForeColor = Color.White;
            button3.Location = new Point(892, 132);
            button3.Margin = new Padding(4, 3, 4, 3);
            button3.Name = "button3";
            button3.Size = new Size(29, 29);
            button3.TabIndex = 9;
            button3.Text = "...";
            button3.UseVisualStyleBackColor = false;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.BackColor = Color.Black;
            button4.FlatAppearance.MouseDownBackColor = Color.Gray;
            button4.FlatStyle = FlatStyle.Flat;
            button4.ForeColor = Color.White;
            button4.Location = new Point(832, 386);
            button4.Margin = new Padding(4, 3, 4, 3);
            button4.Name = "button4";
            button4.Size = new Size(88, 29);
            button4.TabIndex = 1;
            button4.Text = "Download";
            button4.UseVisualStyleBackColor = false;
            button4.Click += button4_Click;
            // 
            // downloadTimer
            // 
            downloadTimer.Enabled = true;
            downloadTimer.Interval = 1000;
            downloadTimer.Tick += timer2_Tick;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.Black;
            label3.ForeColor = Color.White;
            label3.Location = new Point(10, 383);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(123, 16);
            label3.TabIndex = 11;
            label3.Text = "Downloading 0 files...";
            // 
            // trayIcon
            // 
            trayIcon.ContextMenuStrip = trayContextMenu;
            trayIcon.Icon = (Icon)resources.GetObject("trayIcon.Icon");
            trayIcon.Text = "Download Manager";
            trayIcon.Visible = true;
            trayIcon.MouseClick += trayIcon_MouseClick;
            // 
            // trayContextMenu
            // 
            trayContextMenu.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            trayContextMenu.Items.AddRange(new ToolStripItem[] { toolStripTextBox1, toolStripSeparator1, toolStripMenuItem1, toolStripMenuItem2, toolStripMenuItem3 });
            trayContextMenu.Name = "contextMenuStrip1";
            trayContextMenu.RenderMode = ToolStripRenderMode.System;
            trayContextMenu.Size = new Size(172, 94);
            // 
            // toolStripTextBox1
            // 
            toolStripTextBox1.BorderStyle = BorderStyle.None;
            toolStripTextBox1.Enabled = false;
            toolStripTextBox1.Name = "toolStripTextBox1";
            toolStripTextBox1.ReadOnly = true;
            toolStripTextBox1.Size = new Size(111, 16);
            toolStripTextBox1.Text = "Download Manager";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(168, 6);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Image = (Image)resources.GetObject("toolStripMenuItem1.Image");
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(171, 22);
            toolStripMenuItem1.Text = "Open";
            toolStripMenuItem1.Click += toolStripMenuItem1_Click;
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Image = (Image)resources.GetObject("toolStripMenuItem2.Image");
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(171, 22);
            toolStripMenuItem2.Text = "Report a bug";
            toolStripMenuItem2.Click += toolStripMenuItem2_Click;
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Image = (Image)resources.GetObject("toolStripMenuItem3.Image");
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(171, 22);
            toolStripMenuItem3.Text = "Exit";
            toolStripMenuItem3.Click += toolStripMenuItem3_Click;
            // 
            // linkLabel1
            // 
            linkLabel1.ActiveLinkColor = Color.Blue;
            linkLabel1.AutoSize = true;
            linkLabel1.BackColor = Color.Black;
            linkLabel1.LinkColor = Color.FromArgb(90, 90, 255);
            linkLabel1.Location = new Point(10, 399);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(138, 16);
            linkLabel1.TabIndex = 13;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "Install Browser Extention";
            linkLabel1.VisitedLinkColor = Color.FromArgb(0, 195, 255);
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // textBox3
            // 
            textBox3.BackColor = Color.Black;
            textBox3.BorderStyle = BorderStyle.FixedSingle;
            textBox3.ForeColor = Color.White;
            textBox3.Location = new Point(16, 201);
            textBox3.Margin = new Padding(4, 3, 4, 3);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(809, 23);
            textBox3.TabIndex = 15;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.Black;
            label4.ForeColor = Color.White;
            label4.Location = new Point(12, 181);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(56, 16);
            label4.TabIndex = 14;
            label4.Text = "File Hash";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.BackColor = Color.Black;
            label5.Cursor = Cursors.Help;
            label5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label5.ForeColor = Color.Gray;
            label5.Location = new Point(66, 181);
            label5.Name = "label5";
            label5.Size = new Size(12, 15);
            label5.TabIndex = 16;
            label5.Text = "?";
            toolTip.SetToolTip(label5, resources.GetString("label5.ToolTip"));
            // 
            // toolTip
            // 
            toolTip.IsBalloon = true;
            toolTip.ToolTipIcon = ToolTipIcon.Info;
            toolTip.ToolTipTitle = "Help";
            // 
            // openFileDialog
            // 
            openFileDialog.DefaultExt = "*.*";
            openFileDialog.Filter = "All files|*.*";
            // 
            // comboBox1
            // 
            comboBox1.BackColor = Color.Black;
            comboBox1.DisplayMember = "0";
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FlatStyle = FlatStyle.Flat;
            comboBox1.ForeColor = Color.White;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "MD5", "SHA-1", "SHA-256", "SHA-384", "SHA-512" });
            comboBox1.Location = new Point(832, 201);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(89, 24);
            comboBox1.TabIndex = 17;
            comboBox1.ValueMember = "0";
            // 
            // textBox1
            // 
            textBox1.BackColor = Color.Black;
            textBox1.FlatStyle = FlatStyle.Flat;
            textBox1.ForeColor = Color.White;
            textBox1.FormattingEnabled = true;
            textBox1.Location = new Point(14, 74);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(907, 24);
            textBox1.TabIndex = 18;
            textBox1.SelectedIndexChanged += textBox1_SelectedIndexChanged;
            textBox1.TextUpdate += textBox1_TextUpdate;
            // 
            // toolStripButton2
            // 
            toolStripButton2.Anchor = AnchorStyles.Right;
            toolStripButton2.BackColor = Color.Transparent;
            toolStripButton2.BackgroundImage = (Image)resources.GetObject("toolStripButton2.BackgroundImage");
            toolStripButton2.BackgroundImageLayout = ImageLayout.Zoom;
            toolStripButton2.FlatAppearance.BorderColor = Color.FromArgb(34, 34, 34);
            toolStripButton2.FlatStyle = FlatStyle.Flat;
            toolStripButton2.ForeColor = Color.White;
            toolStripButton2.Location = new Point(894, 0);
            toolStripButton2.Name = "toolStripButton2";
            toolStripButton2.Size = new Size(39, 42);
            toolStripButton2.TabIndex = 21;
            toolStripButton2.UseVisualStyleBackColor = false;
            toolStripButton2.Click += toolStripButton2_Click;
            // 
            // toolStripButton3
            // 
            toolStripButton3.BackColor = Color.Transparent;
            toolStripButton3.BackgroundImage = (Image)resources.GetObject("toolStripButton3.BackgroundImage");
            toolStripButton3.BackgroundImageLayout = ImageLayout.Zoom;
            toolStripButton3.FlatAppearance.BorderColor = Color.FromArgb(34, 34, 34);
            toolStripButton3.FlatStyle = FlatStyle.Flat;
            toolStripButton3.ForeColor = Color.White;
            toolStripButton3.Location = new Point(39, 0);
            toolStripButton3.Name = "toolStripButton3";
            toolStripButton3.Size = new Size(39, 42);
            toolStripButton3.TabIndex = 22;
            toolStripButton3.UseVisualStyleBackColor = false;
            toolStripButton3.Click += toolStripButton3_Click;
            // 
            // toolStripButton1
            // 
            toolStripButton1.BackColor = Color.Transparent;
            toolStripButton1.BackgroundImage = (Image)resources.GetObject("toolStripButton1.BackgroundImage");
            toolStripButton1.BackgroundImageLayout = ImageLayout.Center;
            toolStripButton1.FlatAppearance.BorderColor = Color.FromArgb(34, 34, 34);
            toolStripButton1.FlatStyle = FlatStyle.Flat;
            toolStripButton1.ForeColor = Color.White;
            toolStripButton1.Location = new Point(0, 0);
            toolStripButton1.Name = "toolStripButton1";
            toolStripButton1.Size = new Size(39, 42);
            toolStripButton1.TabIndex = 23;
            toolStripButton1.UseVisualStyleBackColor = false;
            toolStripButton1.Click += toolStripButton1_Click;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(34, 34, 34);
            panel1.Controls.Add(button2);
            panel1.Controls.Add(toolStripButton1);
            panel1.Controls.Add(toolStripButton3);
            panel1.Controls.Add(toolStripButton2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(933, 42);
            panel1.TabIndex = 24;
            // 
            // button2
            // 
            button2.BackColor = Color.Transparent;
            button2.BackgroundImage = Properties.Resources.currentdownload;
            button2.BackgroundImageLayout = ImageLayout.Center;
            button2.FlatAppearance.BorderColor = Color.FromArgb(34, 34, 34);
            button2.FlatStyle = FlatStyle.Flat;
            button2.ForeColor = Color.White;
            button2.Location = new Point(77, 0);
            button2.Name = "button2";
            button2.Size = new Size(39, 42);
            button2.TabIndex = 25;
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // progressBar1
            // 
            progressBar1.BackColor = Color.FromArgb(20, 20, 20);
            progressBar1.Dock = DockStyle.Bottom;
            progressBar1.Location = new Point(0, 427);
            progressBar1.Name = "progressBar1";
            progressBar1.ShowText = false;
            progressBar1.Size = new Size(933, 11);
            progressBar1.TabIndex = 25;
            // 
            // videoDownloadOptions
            // 
            videoDownloadOptions.Controls.Add(playlistViewButton);
            videoDownloadOptions.Controls.Add(videoThumb);
            videoDownloadOptions.Controls.Add(videoDownloadTypeComboBox);
            videoDownloadOptions.Controls.Add(videoDurationLabel);
            videoDownloadOptions.Controls.Add(videoDateLabel);
            videoDownloadOptions.Controls.Add(videoChannelLabel);
            videoDownloadOptions.Controls.Add(videoTitleLabel);
            videoDownloadOptions.Controls.Add(videoDuration);
            videoDownloadOptions.Controls.Add(videoDate);
            videoDownloadOptions.Controls.Add(videoChannel);
            videoDownloadOptions.Controls.Add(videoTitle);
            videoDownloadOptions.Location = new Point(16, 245);
            videoDownloadOptions.Name = "videoDownloadOptions";
            videoDownloadOptions.Size = new Size(439, 121);
            videoDownloadOptions.TabIndex = 26;
            videoDownloadOptions.TabStop = false;
            videoDownloadOptions.Text = "YouTube Download Options";
            videoDownloadOptions.Visible = false;
            // 
            // playlistViewButton
            // 
            playlistViewButton.BackColor = Color.Transparent;
            playlistViewButton.FlatAppearance.MouseDownBackColor = Color.Gray;
            playlistViewButton.FlatStyle = FlatStyle.Flat;
            playlistViewButton.Location = new Point(327, 88);
            playlistViewButton.Name = "playlistViewButton";
            playlistViewButton.Size = new Size(106, 25);
            playlistViewButton.TabIndex = 38;
            playlistViewButton.Text = "Playlist Viewer";
            playlistViewButton.UseVisualStyleBackColor = false;
            playlistViewButton.Visible = false;
            playlistViewButton.Click += playlistViewButton_Click;
            // 
            // videoThumb
            // 
            videoThumb.AllowExternalDrop = true;
            videoThumb.BackgroundImageLayout = ImageLayout.Zoom;
            videoThumb.CreationProperties = null;
            videoThumb.DefaultBackgroundColor = Color.Black;
            videoThumb.Location = new Point(327, 22);
            videoThumb.Name = "videoThumb";
            videoThumb.Size = new Size(106, 64);
            videoThumb.Source = new Uri("https://raw.githubusercontent.com/Soniczac7/Download-Manager/master/Resources/error.png", UriKind.Absolute);
            videoThumb.TabIndex = 37;
            videoThumb.ZoomFactor = 1D;
            // 
            // videoDownloadTypeComboBox
            // 
            videoDownloadTypeComboBox.BackColor = Color.Black;
            videoDownloadTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            videoDownloadTypeComboBox.FlatStyle = FlatStyle.Flat;
            videoDownloadTypeComboBox.ForeColor = Color.White;
            videoDownloadTypeComboBox.FormattingEnabled = true;
            videoDownloadTypeComboBox.Items.AddRange(new object[] { "Audio", "Video", "Audio & Video" });
            videoDownloadTypeComboBox.Location = new Point(7, 88);
            videoDownloadTypeComboBox.Name = "videoDownloadTypeComboBox";
            videoDownloadTypeComboBox.Size = new Size(112, 24);
            videoDownloadTypeComboBox.TabIndex = 36;
            videoDownloadTypeComboBox.SelectedIndexChanged += videoDownloadTypeComboBox_SelectedIndexChanged;
            // 
            // videoDurationLabel
            // 
            videoDurationLabel.AutoSize = true;
            videoDurationLabel.Location = new Point(18, 69);
            videoDurationLabel.Name = "videoDurationLabel";
            videoDurationLabel.Size = new Size(57, 16);
            videoDurationLabel.TabIndex = 34;
            videoDurationLabel.Text = "Duration:";
            // 
            // videoDateLabel
            // 
            videoDateLabel.AutoSize = true;
            videoDateLabel.Location = new Point(18, 53);
            videoDateLabel.Name = "videoDateLabel";
            videoDateLabel.Size = new Size(36, 16);
            videoDateLabel.TabIndex = 33;
            videoDateLabel.Text = "Date:";
            // 
            // videoChannelLabel
            // 
            videoChannelLabel.AutoSize = true;
            videoChannelLabel.Location = new Point(18, 37);
            videoChannelLabel.Name = "videoChannelLabel";
            videoChannelLabel.Size = new Size(55, 16);
            videoChannelLabel.TabIndex = 32;
            videoChannelLabel.Text = "Channel:";
            // 
            // videoTitleLabel
            // 
            videoTitleLabel.AutoSize = true;
            videoTitleLabel.Location = new Point(18, 21);
            videoTitleLabel.Name = "videoTitleLabel";
            videoTitleLabel.Size = new Size(68, 16);
            videoTitleLabel.TabIndex = 31;
            videoTitleLabel.Text = "Video Title:";
            // 
            // videoDuration
            // 
            videoDuration.AutoEllipsis = true;
            videoDuration.Location = new Point(89, 69);
            videoDuration.Name = "videoDuration";
            videoDuration.Size = new Size(232, 16);
            videoDuration.TabIndex = 30;
            videoDuration.Text = "Duration";
            // 
            // videoDate
            // 
            videoDate.AutoEllipsis = true;
            videoDate.Location = new Point(89, 53);
            videoDate.Name = "videoDate";
            videoDate.Size = new Size(232, 16);
            videoDate.TabIndex = 29;
            videoDate.Text = "Date Posted";
            // 
            // videoChannel
            // 
            videoChannel.AutoEllipsis = true;
            videoChannel.Location = new Point(89, 37);
            videoChannel.Name = "videoChannel";
            videoChannel.Size = new Size(232, 16);
            videoChannel.TabIndex = 28;
            videoChannel.Text = "Channel Name";
            // 
            // videoTitle
            // 
            videoTitle.AutoEllipsis = true;
            videoTitle.Location = new Point(89, 21);
            videoTitle.Name = "videoTitle";
            videoTitle.Size = new Size(232, 16);
            videoTitle.TabIndex = 27;
            videoTitle.Text = "Video Title";
            // 
            // videoErrorBox
            // 
            videoErrorBox.Controls.Add(videoErrorText);
            videoErrorBox.Location = new Point(17, 245);
            videoErrorBox.Name = "videoErrorBox";
            videoErrorBox.Size = new Size(439, 121);
            videoErrorBox.TabIndex = 39;
            videoErrorBox.TabStop = false;
            videoErrorBox.Text = "YouTube Download Options";
            videoErrorBox.Visible = false;
            // 
            // videoErrorText
            // 
            videoErrorText.BorderStyle = BorderStyle.None;
            videoErrorText.Dock = DockStyle.Fill;
            videoErrorText.ForeColor = Color.Red;
            videoErrorText.Location = new Point(3, 19);
            videoErrorText.Name = "videoErrorText";
            videoErrorText.ReadOnly = true;
            videoErrorText.Size = new Size(433, 99);
            videoErrorText.TabIndex = 0;
            videoErrorText.Text = "An error occured while fetching the provided YouTube URL.\nCheck the debug log for more information.";
            // 
            // videoThumbErrorTip
            // 
            videoThumbErrorTip.IsBalloon = true;
            videoThumbErrorTip.ToolTipIcon = ToolTipIcon.Error;
            videoThumbErrorTip.ToolTipTitle = "Failed to fetch video thumbnail";
            // 
            // DownloadForm
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(933, 438);
            Controls.Add(videoDownloadOptions);
            Controls.Add(progressBar1);
            Controls.Add(panel1);
            Controls.Add(textBox1);
            Controls.Add(comboBox1);
            Controls.Add(label5);
            Controls.Add(textBox3);
            Controls.Add(label4);
            Controls.Add(linkLabel1);
            Controls.Add(label3);
            Controls.Add(videoErrorBox);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(textBox2);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "DownloadForm";
            Text = "Download Manager";
            FormClosing += DownloadForm_FormClosing;
            Shown += DownloadForm_Shown;
            Move += DownloadForm_Move;
            trayContextMenu.ResumeLayout(false);
            trayContextMenu.PerformLayout();
            panel1.ResumeLayout(false);
            videoDownloadOptions.ResumeLayout(false);
            videoDownloadOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)videoThumb).EndInit();
            videoErrorBox.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.Timer downloadTimer;
        private System.Windows.Forms.Label label3;
        public TextBox textBox2;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayContextMenu;
        private ToolStripTextBox toolStripTextBox1;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private LinkLabel linkLabel1;
        private TextBox textBox3;
        private Label label4;
        private Label label5;
        private ToolTip toolTip;
        private OpenFileDialog openFileDialog;
        private ComboBox comboBox1;
        public ComboBox textBox1;
        private Button toolStripButton2;
        private Button toolStripButton3;
        private Button toolStripButton1;
        private Panel panel1;
        public Button button2;
        private BetterProgressBar progressBar1;
        private GroupBox videoDownloadOptions;
        private Button playlistViewButton;
        private Microsoft.Web.WebView2.WinForms.WebView2 videoThumb;
        private ComboBox videoDownloadTypeComboBox;
        private Label videoDurationLabel;
        private Label videoDateLabel;
        private Label videoChannelLabel;
        private Label videoTitleLabel;
        private Label videoDuration;
        private Label videoDate;
        private Label videoChannel;
        private Label videoTitle;
        private GroupBox videoErrorBox;
        private RichTextBox videoErrorText;
        private ToolTip videoThumbErrorTip;
    }
}