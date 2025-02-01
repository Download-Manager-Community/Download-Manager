namespace DownloadManager.Components
{
    partial class DarkMessageBoxForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DarkMessageBoxForm));
            pictureBox = new PictureBox();
            messageBox = new RichTextBox();
            button3 = new Button();
            button2 = new Button();
            button1 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox).BeginInit();
            SuspendLayout();
            // 
            // pictureBox
            // 
            pictureBox.BackColor = Color.Transparent;
            pictureBox.BackgroundImage = Properties.Resources.info;
            pictureBox.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox.Location = new Point(12, 13);
            pictureBox.Name = "pictureBox";
            pictureBox.Size = new Size(30, 32);
            pictureBox.TabIndex = 0;
            pictureBox.TabStop = false;
            // 
            // messageBox
            // 
            messageBox.BackColor = Color.Black;
            messageBox.BorderStyle = BorderStyle.None;
            messageBox.ForeColor = Color.White;
            messageBox.Location = new Point(48, 13);
            messageBox.Name = "messageBox";
            messageBox.ReadOnly = true;
            messageBox.ScrollBars = RichTextBoxScrollBars.Vertical;
            messageBox.Size = new Size(359, 154);
            messageBox.TabIndex = 999;
            messageBox.Text = "";
            // 
            // button3
            // 
            button3.FlatAppearance.MouseDownBackColor = Color.Gray;
            button3.FlatStyle = FlatStyle.Flat;
            button3.Location = new Point(170, 173);
            button3.Name = "button3";
            button3.Size = new Size(75, 25);
            button3.TabIndex = 2;
            button3.Text = "3";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button2
            // 
            button2.FlatAppearance.MouseDownBackColor = Color.Gray;
            button2.FlatStyle = FlatStyle.Flat;
            button2.Location = new Point(251, 173);
            button2.Name = "button2";
            button2.Size = new Size(75, 25);
            button2.TabIndex = 3;
            button2.Text = "2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // button1
            // 
            button1.FlatAppearance.MouseDownBackColor = Color.Gray;
            button1.FlatStyle = FlatStyle.Flat;
            button1.Location = new Point(332, 173);
            button1.Name = "button1";
            button1.Size = new Size(75, 25);
            button1.TabIndex = 4;
            button1.Text = "1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // DarkMessageBoxForm
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(419, 206);
            Controls.Add(button1);
            Controls.Add(button2);
            Controls.Add(button3);
            Controls.Add(messageBox);
            Controls.Add(pictureBox);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "DarkMessageBoxForm";
            Text = "DarkMessageBox";
            TopMost = true;
            ((System.ComponentModel.ISupportInitialize)pictureBox).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private PictureBox pictureBox;
        private RichTextBox messageBox;
        private Button button3;
        private Button button2;
        private Button button1;
    }
}