namespace DownloadManager
{
    partial class HashCalculator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HashCalculator));
            md5Box = new TextBox();
            label2 = new Label();
            fileNameBox = new TextBox();
            label1 = new Label();
            sha1Box = new TextBox();
            label3 = new Label();
            sha256Box = new TextBox();
            label4 = new Label();
            sha512Box = new TextBox();
            label5 = new Label();
            sha384Box = new TextBox();
            label6 = new Label();
            progressBar = new BetterProgressBar();
            SuspendLayout();
            // 
            // md5Box
            // 
            md5Box.BackColor = Color.Black;
            md5Box.BorderStyle = BorderStyle.FixedSingle;
            md5Box.ForeColor = Color.White;
            md5Box.Location = new Point(17, 90);
            md5Box.Margin = new Padding(4, 3, 4, 3);
            md5Box.Name = "md5Box";
            md5Box.ReadOnly = true;
            md5Box.Size = new Size(432, 23);
            md5Box.TabIndex = 12;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(13, 69);
            label2.Margin = new Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new Size(86, 16);
            label2.TabIndex = 11;
            label2.Text = "File MD5 Hash";
            // 
            // fileNameBox
            // 
            fileNameBox.BackColor = Color.Black;
            fileNameBox.BorderStyle = BorderStyle.FixedSingle;
            fileNameBox.ForeColor = Color.White;
            fileNameBox.Location = new Point(17, 29);
            fileNameBox.Margin = new Padding(4, 3, 4, 3);
            fileNameBox.Name = "fileNameBox";
            fileNameBox.ReadOnly = true;
            fileNameBox.Size = new Size(432, 23);
            fileNameBox.TabIndex = 10;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 10);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(62, 16);
            label1.TabIndex = 9;
            label1.Text = "File Name";
            // 
            // sha1Box
            // 
            sha1Box.BackColor = Color.Black;
            sha1Box.BorderStyle = BorderStyle.FixedSingle;
            sha1Box.ForeColor = Color.White;
            sha1Box.Location = new Point(17, 151);
            sha1Box.Margin = new Padding(4, 3, 4, 3);
            sha1Box.Name = "sha1Box";
            sha1Box.ReadOnly = true;
            sha1Box.Size = new Size(432, 23);
            sha1Box.TabIndex = 15;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(13, 131);
            label3.Margin = new Padding(4, 0, 4, 0);
            label3.Name = "label3";
            label3.Size = new Size(93, 16);
            label3.TabIndex = 14;
            label3.Text = "File SHA-1 Hash";
            // 
            // sha256Box
            // 
            sha256Box.BackColor = Color.Black;
            sha256Box.BorderStyle = BorderStyle.FixedSingle;
            sha256Box.ForeColor = Color.White;
            sha256Box.Location = new Point(17, 214);
            sha256Box.Margin = new Padding(4, 3, 4, 3);
            sha256Box.Name = "sha256Box";
            sha256Box.ReadOnly = true;
            sha256Box.Size = new Size(432, 23);
            sha256Box.TabIndex = 17;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(13, 194);
            label4.Margin = new Padding(4, 0, 4, 0);
            label4.Name = "label4";
            label4.Size = new Size(109, 16);
            label4.TabIndex = 16;
            label4.Text = "File SHA-256 Hash";
            // 
            // sha512Box
            // 
            sha512Box.BackColor = Color.Black;
            sha512Box.BorderStyle = BorderStyle.FixedSingle;
            sha512Box.ForeColor = Color.White;
            sha512Box.Location = new Point(17, 337);
            sha512Box.Margin = new Padding(4, 3, 4, 3);
            sha512Box.Name = "sha512Box";
            sha512Box.ReadOnly = true;
            sha512Box.Size = new Size(432, 23);
            sha512Box.TabIndex = 19;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(13, 318);
            label5.Margin = new Padding(4, 0, 4, 0);
            label5.Name = "label5";
            label5.Size = new Size(107, 16);
            label5.TabIndex = 18;
            label5.Text = "File SHA-512 Hash";
            // 
            // sha384Box
            // 
            sha384Box.BackColor = Color.Black;
            sha384Box.BorderStyle = BorderStyle.FixedSingle;
            sha384Box.ForeColor = Color.White;
            sha384Box.Location = new Point(17, 277);
            sha384Box.Margin = new Padding(4, 3, 4, 3);
            sha384Box.Name = "sha384Box";
            sha384Box.ReadOnly = true;
            sha384Box.Size = new Size(432, 23);
            sha384Box.TabIndex = 21;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(13, 257);
            label6.Margin = new Padding(4, 0, 4, 0);
            label6.Name = "label6";
            label6.Size = new Size(109, 16);
            label6.TabIndex = 20;
            label6.Text = "File SHA-384 Hash";
            // 
            // progressBar
            // 
            progressBar.Dock = DockStyle.Bottom;
            progressBar.Location = new Point(0, 375);
            progressBar.MarqueeAnim = true;
            progressBar.Name = "progressBar";
            progressBar.ShowText = false;
            progressBar.Size = new Size(462, 10);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.TabIndex = 22;
            // 
            // HashCalculator
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(462, 385);
            Controls.Add(progressBar);
            Controls.Add(sha384Box);
            Controls.Add(label6);
            Controls.Add(sha512Box);
            Controls.Add(label5);
            Controls.Add(sha256Box);
            Controls.Add(label4);
            Controls.Add(sha1Box);
            Controls.Add(label3);
            Controls.Add(md5Box);
            Controls.Add(label2);
            Controls.Add(fileNameBox);
            Controls.Add(label1);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "HashCalculator";
            Text = "Hash Calculator";
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        public TextBox md5Box;
        private Label label2;
        private TextBox fileNameBox;
        private Label label1;
        public TextBox sha1Box;
        private Label label3;
        public TextBox sha256Box;
        private Label label4;
        public TextBox sha512Box;
        private Label label5;
        public TextBox sha384Box;
        private Label label6;
        private BetterProgressBar progressBar;
    }
}