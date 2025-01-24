namespace DownloadManager
{
    partial class Logging
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Logging));
            richTextBox1 = new RichTextBox();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            saveAsToolStripMenuItem = new ToolStripMenuItem();
            loggingToolStripMenuItem = new ToolStripMenuItem();
            toggleLoggingToolStripMenuItem = new ToolStripMenuItem();
            saveFileDialog = new SaveFileDialog();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.Black;
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.Dock = DockStyle.Fill;
            richTextBox1.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            richTextBox1.ForeColor = Color.White;
            richTextBox1.Location = new Point(0, 24);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.ReadOnly = true;
            richTextBox1.Size = new Size(957, 494);
            richTextBox1.TabIndex = 6;
            richTextBox1.Text = "";
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, loggingToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.RenderMode = ToolStripRenderMode.System;
            menuStrip1.Size = new Size(957, 24);
            menuStrip1.TabIndex = 7;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveAsToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(38, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // saveAsToolStripMenuItem
            // 
            saveAsToolStripMenuItem.BackColor = Color.Black;
            saveAsToolStripMenuItem.ForeColor = Color.White;
            saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            saveAsToolStripMenuItem.Size = new Size(180, 22);
            saveAsToolStripMenuItem.Text = "Save As";
            saveAsToolStripMenuItem.Click += saveAsToolStripMenuItem_Click;
            // 
            // loggingToolStripMenuItem
            // 
            loggingToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toggleLoggingToolStripMenuItem });
            loggingToolStripMenuItem.Name = "loggingToolStripMenuItem";
            loggingToolStripMenuItem.Size = new Size(63, 20);
            loggingToolStripMenuItem.Text = "Logging";
            // 
            // toggleLoggingToolStripMenuItem
            // 
            toggleLoggingToolStripMenuItem.BackColor = Color.Black;
            toggleLoggingToolStripMenuItem.Checked = true;
            toggleLoggingToolStripMenuItem.CheckState = CheckState.Checked;
            toggleLoggingToolStripMenuItem.ForeColor = Color.White;
            toggleLoggingToolStripMenuItem.Name = "toggleLoggingToolStripMenuItem";
            toggleLoggingToolStripMenuItem.Size = new Size(208, 22);
            toggleLoggingToolStripMenuItem.Text = "Toggle Automatic Saving";
            toggleLoggingToolStripMenuItem.Click += toggleLoggingToolStripMenuItem_Click;
            // 
            // saveFileDialog
            // 
            saveFileDialog.DefaultExt = "log";
            saveFileDialog.Filter = "Log files|*.log";
            // 
            // Logging
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Black;
            ClientSize = new Size(957, 518);
            Controls.Add(richTextBox1);
            Controls.Add(menuStrip1);
            ForeColor = Color.White;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(358, 149);
            Name = "Logging";
            Text = "Debug Logging";
            FormClosing += Logging_FormClosing;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private RichTextBox richTextBox1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem loggingToolStripMenuItem;
        private ToolStripMenuItem saveAsToolStripMenuItem;
        private ToolStripMenuItem toggleLoggingToolStripMenuItem;
        private SaveFileDialog saveFileDialog;
    }
}