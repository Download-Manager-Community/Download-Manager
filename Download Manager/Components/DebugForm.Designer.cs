
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
            msgBoxPage = new TabPage();
            checkBox2 = new CheckBox();
            label1 = new Label();
            button3 = new Button();
            listBox2 = new ListBox();
            listBox1 = new ListBox();
            crashHandlerPage = new TabPage();
            checkBox1 = new CheckBox();
            button2 = new Button();
            button1 = new Button();
            button4 = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            darkTabControl.SuspendLayout();
            msgBoxPage.SuspendLayout();
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
            darkTabControl.Controls.Add(msgBoxPage);
            darkTabControl.Controls.Add(crashHandlerPage);
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
            // msgBoxPage
            // 
            msgBoxPage.Controls.Add(button4);
            msgBoxPage.Controls.Add(checkBox2);
            msgBoxPage.Controls.Add(label1);
            msgBoxPage.Controls.Add(button3);
            msgBoxPage.Controls.Add(listBox2);
            msgBoxPage.Controls.Add(listBox1);
            msgBoxPage.Location = new Point(4, 25);
            msgBoxPage.Name = "msgBoxPage";
            msgBoxPage.Padding = new Padding(3);
            msgBoxPage.Size = new Size(768, 347);
            msgBoxPage.TabIndex = 1;
            msgBoxPage.Text = "DarkMessageBox";
            msgBoxPage.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Checked = true;
            checkBox2.CheckState = CheckState.Checked;
            checkBox2.Location = new Point(258, 6);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(95, 20);
            checkBox2.TabIndex = 1007;
            checkBox2.Text = "Enable Close";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.CheckedChanged += checkBox2_CheckedChanged;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 122);
            label1.Name = "label1";
            label1.Size = new Size(147, 16);
            label1.TabIndex = 1006;
            label1.Text = "Result: DialogResult.None";
            // 
            // button3
            // 
            button3.Location = new Point(6, 96);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 1005;
            button3.Text = "Test";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // listBox2
            // 
            listBox2.FormattingEnabled = true;
            listBox2.Items.AddRange(new object[] { "None", "Information", "Question", "Warning", "Error" });
            listBox2.Location = new Point(132, 6);
            listBox2.Name = "listBox2";
            listBox2.Size = new Size(120, 84);
            listBox2.TabIndex = 1004;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.Items.AddRange(new object[] { "OK", "OKCancel", "AbortRetryIgnore", "YesNoCancel", "YesNo", "RetryCancel", "CancelTryContinue" });
            listBox1.Location = new Point(6, 6);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(120, 84);
            listBox1.TabIndex = 1003;
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
            // button2
            // 
            button2.Location = new Point(87, 6);
            button2.Name = "button2";
            button2.Size = new Size(94, 23);
            button2.TabIndex = 1;
            button2.Text = "Test Crash Ext";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
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
            // button4
            // 
            button4.Location = new Point(87, 96);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 1008;
            button4.Text = "Close";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
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
            msgBoxPage.ResumeLayout(false);
            msgBoxPage.PerformLayout();
            crashHandlerPage.ResumeLayout(false);
            crashHandlerPage.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox richTextBox1;
        private PictureBox pictureBox1;
        private Controls.DarkTabControl darkTabControl;
        private TabPage crashHandlerPage;
        private TabPage msgBoxPage;
        private Button button2;
        private Button button1;
        private CheckBox checkBox1;
        private ListBox listBox1;
        private ListBox listBox2;
        private Button button3;
        private Label label1;
        private CheckBox checkBox2;
        private Button button4;
    }
}