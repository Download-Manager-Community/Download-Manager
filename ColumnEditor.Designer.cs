namespace DownloadManager
{
    partial class ColumnEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColumnEditor));
            descLabel = new Label();
            showLabel = new Label();
            showList = new ListBox();
            hideList = new ListBox();
            hideLabel = new Label();
            showButton = new Button();
            hideButton = new Button();
            doneButton = new Button();
            resetButton = new Button();
            SuspendLayout();
            // 
            // descLabel
            // 
            descLabel.Location = new Point(12, 9);
            descLabel.Name = "descLabel";
            descLabel.Size = new Size(436, 55);
            descLabel.TabIndex = 0;
            descLabel.Text = "Use this dialogue to select all colunms to show or hide.\r\nTo show columns move them to the 'Show' side.\r\nTo hide columns move them to the 'Hide' side.";
            // 
            // showLabel
            // 
            showLabel.AutoSize = true;
            showLabel.Location = new Point(12, 64);
            showLabel.Name = "showLabel";
            showLabel.Size = new Size(37, 16);
            showLabel.TabIndex = 1;
            showLabel.Text = "Show";
            // 
            // showList
            // 
            showList.FormattingEnabled = true;
            showList.ItemHeight = 16;
            showList.Location = new Point(12, 83);
            showList.Name = "showList";
            showList.Size = new Size(215, 260);
            showList.TabIndex = 2;
            // 
            // hideList
            // 
            hideList.FormattingEnabled = true;
            hideList.ItemHeight = 16;
            hideList.Location = new Point(233, 83);
            hideList.Name = "hideList";
            hideList.Size = new Size(215, 260);
            hideList.TabIndex = 4;
            // 
            // hideLabel
            // 
            hideLabel.AutoSize = true;
            hideLabel.Location = new Point(233, 64);
            hideLabel.Name = "hideLabel";
            hideLabel.Size = new Size(33, 16);
            hideLabel.TabIndex = 3;
            hideLabel.Text = "Hide";
            // 
            // showButton
            // 
            showButton.BackColor = Color.Black;
            showButton.FlatAppearance.MouseDownBackColor = Color.Gray;
            showButton.FlatStyle = FlatStyle.Flat;
            showButton.ForeColor = Color.White;
            showButton.Location = new Point(464, 83);
            showButton.Margin = new Padding(4, 3, 4, 3);
            showButton.Name = "showButton";
            showButton.Size = new Size(88, 29);
            showButton.TabIndex = 5;
            showButton.Text = "<< Show";
            showButton.UseVisualStyleBackColor = false;
            showButton.Click += showButton_Click;
            // 
            // hideButton
            // 
            hideButton.BackColor = Color.Black;
            hideButton.FlatAppearance.MouseDownBackColor = Color.Gray;
            hideButton.FlatStyle = FlatStyle.Flat;
            hideButton.ForeColor = Color.White;
            hideButton.Location = new Point(464, 118);
            hideButton.Margin = new Padding(4, 3, 4, 3);
            hideButton.Name = "hideButton";
            hideButton.Size = new Size(88, 29);
            hideButton.TabIndex = 6;
            hideButton.Text = "Hide >>";
            hideButton.UseVisualStyleBackColor = false;
            hideButton.Click += hideButton_Click;
            // 
            // doneButton
            // 
            doneButton.BackColor = Color.Black;
            doneButton.FlatAppearance.MouseDownBackColor = Color.Gray;
            doneButton.FlatStyle = FlatStyle.Flat;
            doneButton.ForeColor = Color.White;
            doneButton.Location = new Point(464, 314);
            doneButton.Margin = new Padding(4, 3, 4, 3);
            doneButton.Name = "doneButton";
            doneButton.Size = new Size(88, 29);
            doneButton.TabIndex = 7;
            doneButton.Text = "Done";
            doneButton.UseVisualStyleBackColor = false;
            doneButton.Click += doneButton_Click;
            // 
            // resetButton
            // 
            resetButton.BackColor = Color.Black;
            resetButton.FlatAppearance.MouseDownBackColor = Color.Gray;
            resetButton.FlatStyle = FlatStyle.Flat;
            resetButton.ForeColor = Color.White;
            resetButton.Location = new Point(464, 12);
            resetButton.Margin = new Padding(4, 3, 4, 3);
            resetButton.Name = "resetButton";
            resetButton.Size = new Size(88, 29);
            resetButton.TabIndex = 8;
            resetButton.Text = "Reset";
            resetButton.UseVisualStyleBackColor = false;
            resetButton.Click += resetButton_Click;
            // 
            // ColumnEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(565, 361);
            Controls.Add(resetButton);
            Controls.Add(doneButton);
            Controls.Add(hideButton);
            Controls.Add(showButton);
            Controls.Add(hideList);
            Controls.Add(hideLabel);
            Controls.Add(showList);
            Controls.Add(showLabel);
            Controls.Add(descLabel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ColumnEditor";
            Text = "Column Editor";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label descLabel;
        private Label showLabel;
        private ListBox showList;
        private ListBox hideList;
        private Label hideLabel;
        private Button showButton;
        private Button hideButton;
        private Button doneButton;
        private Button resetButton;
    }
}