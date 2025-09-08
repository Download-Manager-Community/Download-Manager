namespace YouTubeDownloadAddon
{
    partial class Test
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
            label1 = new Label();
            button1 = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(325, 26);
            label1.Name = "label1";
            label1.Size = new Size(56, 16);
            label1.TabIndex = 0;
            label1.Text = "Cool Test";
            // 
            // button1
            // 
            button1.Location = new Point(282, 45);
            button1.Name = "button1";
            button1.Size = new Size(165, 23);
            button1.TabIndex = 1;
            button1.Text = "YouTube Downloader Test";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // Test
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(button1);
            Controls.Add(label1);
            Name = "Test";
            Text = "Test";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Button button1;
    }
}