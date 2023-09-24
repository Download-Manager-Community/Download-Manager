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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CurrentDownloads));
            groupBox1 = new GroupBox();
            progressBar1 = new ProgressBar();
            label8 = new Label();
            label7 = new Label();
            label6 = new Label();
            label5 = new Label();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            panel1 = new Panel();
            panel2 = new Panel();
            splitter1 = new Splitter();
            refreshButton = new Button();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(progressBar1);
            groupBox1.Controls.Add(label8);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 13);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(428, 112);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Visible = false;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(290, 46);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(121, 25);
            progressBar1.TabIndex = 1;
            // 
            // label8
            // 
            label8.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label8.Location = new Point(121, 82);
            label8.Name = "label8";
            label8.Size = new Size(163, 16);
            label8.TabIndex = 7;
            label8.Text = "0%";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(6, 82);
            label7.Name = "label7";
            label7.Size = new Size(114, 16);
            label7.TabIndex = 6;
            label7.Text = "Download Progress:";
            // 
            // label6
            // 
            label6.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label6.Location = new Point(96, 61);
            label6.Name = "label6";
            label6.Size = new Size(188, 16);
            label6.TabIndex = 5;
            label6.Text = "0 Bytes";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 61);
            label5.Name = "label5";
            label5.Size = new Size(91, 16);
            label5.TabIndex = 4;
            label5.Text = "Download Size:";
            // 
            // label4
            // 
            label4.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            label4.Location = new Point(97, 41);
            label4.Name = "label4";
            label4.Size = new Size(187, 16);
            label4.TabIndex = 3;
            label4.Text = "FileURL";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 41);
            label3.Name = "label3";
            label3.Size = new Size(89, 16);
            label3.TabIndex = 2;
            label3.Text = "Download URL:";
            // 
            // label2
            // 
            label2.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label2.Location = new Point(92, 20);
            label2.Name = "label2";
            label2.Size = new Size(192, 16);
            label2.TabIndex = 1;
            label2.Text = "FileName";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
            label1.Location = new Point(6, 20);
            label1.Name = "label1";
            label1.Size = new Size(83, 15);
            label1.TabIndex = 0;
            label1.Text = "Downloading:";
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.Controls.Add(groupBox1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(452, 427);
            panel1.TabIndex = 1;
            // 
            // panel2
            // 
            panel2.Controls.Add(splitter1);
            panel2.Controls.Add(refreshButton);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 430);
            panel2.Name = "panel2";
            panel2.Size = new Size(452, 50);
            panel2.TabIndex = 2;
            // 
            // splitter1
            // 
            splitter1.BackColor = Color.DimGray;
            splitter1.Dock = DockStyle.Top;
            splitter1.Enabled = false;
            splitter1.Location = new Point(0, 0);
            splitter1.MinExtra = 1;
            splitter1.MinSize = 1;
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(452, 1);
            splitter1.TabIndex = 55;
            splitter1.TabStop = false;
            // 
            // refreshButton
            // 
            refreshButton.BackColor = Color.Transparent;
            refreshButton.FlatAppearance.MouseDownBackColor = Color.Gray;
            refreshButton.FlatStyle = FlatStyle.Flat;
            refreshButton.ForeColor = Color.White;
            refreshButton.Location = new Point(185, 13);
            refreshButton.Name = "refreshButton";
            refreshButton.Size = new Size(82, 25);
            refreshButton.TabIndex = 54;
            refreshButton.Text = "Refresh";
            refreshButton.UseVisualStyleBackColor = false;
            refreshButton.Click += refreshButton_Click;
            // 
            // CurrentDownloads
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(452, 480);
            Controls.Add(panel2);
            Controls.Add(panel1);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "CurrentDownloads";
            Text = "Donwload Manager | Current Downloads";
            FormClosing += CurrentDownloads_FormClosing;
            Move += CurrentDownloads_Move;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private ProgressBar progressBar1;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label2;
        private Label label1;
        private Panel panel2;
        private Button refreshButton;
        private Splitter splitter1;
        public Panel panel1;
    }
}