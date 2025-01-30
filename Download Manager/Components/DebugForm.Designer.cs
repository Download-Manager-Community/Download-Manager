
namespace DownloadManager.Components
{
    partial class DebugForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebugForm));
            richTextBox1 = new RichTextBox();
            pictureBox1 = new PictureBox();
            darkTabControl = new DownloadManager.Controls.DarkTabControl();
            crashHandlerPage = new TabPage();
            button2 = new Button();
            button1 = new Button();
            tabPage2 = new TabPage();
            checkBox1 = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            darkTabControl.SuspendLayout();
            crashHandlerPage.SuspendLayout();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.Black;
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.ForeColor = Color.White;
            richTextBox1.Location = new Point(48, 12);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.Vertical;
            richTextBox1.Size = new Size(447, 32);
            richTextBox1.TabIndex = 1001;
            richTextBox1.Text = "This form is used for debugging Download Manager, some options here may distrupt current downloads or make Download Manager behave unexpectedly.";
            // 
            // pictureBox1
            // 
            pictureBox1.BackColor = Color.Transparent;
            pictureBox1.BackgroundImage = Properties.Resources.warn;
            pictureBox1.BackgroundImageLayout = ImageLayout.Zoom;
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(30, 32);
            pictureBox1.TabIndex = 1000;
            pictureBox1.TabStop = false;
            // 
            // darkTabControl
            // 
            darkTabControl.Controls.Add(crashHandlerPage);
            darkTabControl.Controls.Add(tabPage2);
            darkTabControl.DrawMode = TabDrawMode.OwnerDrawFixed;
            darkTabControl.Location = new Point(12, 62);
            darkTabControl.Name = "darkTabControl";
            darkTabControl.SelectedIndex = 0;
            darkTabControl.Size = new Size(776, 376);
            darkTabControl.TabBackColor = Color.FromArgb(31, 31, 31);
            darkTabControl.TabForeColor = Color.FromArgb(241, 241, 241);
            darkTabControl.TabIndex = 1002;
            darkTabControl.TextAlign = ContentAlignment.TopLeft;
            // 
            // crashHandlerPage
            // 
            crashHandlerPage.Controls.Add(checkBox1);
            crashHandlerPage.Controls.Add(button2);
            crashHandlerPage.Controls.Add(button1);
            crashHandlerPage.Location = new Point(4, 25);
            crashHandlerPage.Name = "crashHandlerPage";
            crashHandlerPage.Padding = new Padding(3);
            crashHandlerPage.Size = new Size(768, 347);
            crashHandlerPage.TabIndex = 0;
            crashHandlerPage.Text = "CrashHandler";
            crashHandlerPage.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(87, 6);
            button2.Name = "button2";
            button2.Size = new Size(94, 23);
            button2.TabIndex = 1;
            button2.Text = "Test Crash Ext";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(6, 6);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 0;
            button1.Text = "Test Crash";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // tabPage2
            // 
            tabPage2.Location = new Point(4, 25);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(768, 347);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "tabPage2";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Checked = true;
            checkBox1.CheckState = CheckState.Checked;
            checkBox1.Location = new Point(6, 35);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(212, 20);
            checkBox1.TabIndex = 2;
            checkBox1.Text = "Program.allowBypassCrashHandler";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // DebugForm
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(darkTabControl);
            Controls.Add(richTextBox1);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "DebugForm";
            Text = "DebugForm";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            darkTabControl.ResumeLayout(false);
            crashHandlerPage.ResumeLayout(false);
            crashHandlerPage.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richTextBox1;
        private PictureBox pictureBox1;
        private Controls.DarkTabControl darkTabControl;
        private TabPage crashHandlerPage;
        private TabPage tabPage2;
        private Button button2;
        private Button button1;
        private CheckBox checkBox1;
    }
}