namespace DownloadManager
{
    partial class YouTubePlaylistViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(YouTubePlaylistViewer));
            treeView1 = new TreeView();
            progressBar1 = new BetterProgressBar();
            downloadButton = new Button();
            comboBox1 = new ComboBox();
            SuspendLayout();
            // 
            // treeView1
            // 
            treeView1.BackColor = Color.Black;
            treeView1.BorderStyle = BorderStyle.FixedSingle;
            treeView1.ForeColor = Color.White;
            treeView1.LineColor = Color.White;
            treeView1.Location = new Point(12, 12);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(537, 219);
            treeView1.TabIndex = 0;
            // 
            // progressBar1
            // 
            progressBar1.BackColor = Color.FromArgb(20, 20, 20);
            progressBar1.Dock = DockStyle.Bottom;
            progressBar1.Location = new Point(0, 271);
            progressBar1.MarqueeAnim = true;
            progressBar1.Name = "progressBar1";
            progressBar1.ShowText = false;
            progressBar1.Size = new Size(561, 11);
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.TabIndex = 1;
            // 
            // downloadButton
            // 
            downloadButton.BackColor = Color.Black;
            downloadButton.FlatAppearance.MouseDownBackColor = Color.Gray;
            downloadButton.FlatStyle = FlatStyle.Flat;
            downloadButton.ForeColor = Color.White;
            downloadButton.Location = new Point(461, 237);
            downloadButton.Margin = new Padding(4, 3, 4, 3);
            downloadButton.Name = "downloadButton";
            downloadButton.Size = new Size(88, 27);
            downloadButton.TabIndex = 18;
            downloadButton.Text = "Download";
            downloadButton.UseVisualStyleBackColor = false;
            downloadButton.Click += downloadButton_Click;
            // 
            // comboBox1
            // 
            comboBox1.BackColor = Color.Black;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox1.FlatStyle = FlatStyle.Flat;
            comboBox1.ForeColor = Color.White;
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Audio", "Video", "Audio & Video" });
            comboBox1.Location = new Point(342, 240);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(112, 23);
            comboBox1.TabIndex = 19;
            // 
            // YouTubePlaylistViewer
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(561, 282);
            Controls.Add(comboBox1);
            Controls.Add(downloadButton);
            Controls.Add(progressBar1);
            Controls.Add(treeView1);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "YouTubePlaylistViewer";
            Text = "Playlist Viewer";
            Load += YouTubePlaylistViewer_Load;
            ResumeLayout(false);
        }

        #endregion

        private TreeView treeView1;
        private BetterProgressBar progressBar1;
        private Button downloadButton;
        private ComboBox comboBox1;
    }
}