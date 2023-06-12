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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.progressBar1 = new DownloadManager.BetterProgressBar();
            this.downloadButton = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.Black;
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.treeView1.ForeColor = System.Drawing.Color.White;
            this.treeView1.LineColor = System.Drawing.Color.White;
            this.treeView1.Location = new System.Drawing.Point(12, 12);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(537, 219);
            this.treeView1.TabIndex = 0;
            // 
            // progressBar1
            // 
            this.progressBar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 271);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.ShowText = false;
            this.progressBar1.Size = new System.Drawing.Size(561, 11);
            this.progressBar1.TabIndex = 1;
            this.progressBar1.Style = ProgressBarStyle.Marquee;
            this.progressBar1.MarqueeAnim = true;
            // 
            // downloadButton
            // 
            this.downloadButton.BackColor = System.Drawing.Color.Black;
            this.downloadButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Gray;
            this.downloadButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.downloadButton.ForeColor = System.Drawing.Color.White;
            this.downloadButton.Location = new System.Drawing.Point(461, 237);
            this.downloadButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.downloadButton.Name = "downloadButton";
            this.downloadButton.Size = new System.Drawing.Size(88, 27);
            this.downloadButton.TabIndex = 18;
            this.downloadButton.Text = "Download";
            this.downloadButton.UseVisualStyleBackColor = false;
            this.downloadButton.Click += new System.EventHandler(this.downloadButton_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.BackColor = System.Drawing.Color.Black;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.Enabled = false;
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox1.ForeColor = System.Drawing.Color.White;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Audio",
            "Video",
            "Audio & Video"});
            this.comboBox1.Location = new System.Drawing.Point(342, 240);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(112, 23);
            this.comboBox1.TabIndex = 19;
            // 
            // YouTubePlaylistViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(561, 282);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.downloadButton);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.treeView1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "YouTubePlaylistViewer";
            this.Text = "Playlist Viewer";
            this.Load += new System.EventHandler(this.YouTubePlaylistViewer_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private TreeView treeView1;
        private BetterProgressBar progressBar1;
        private Button downloadButton;
        private ComboBox comboBox1;
    }
}