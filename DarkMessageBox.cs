using System.Media;
using System.Runtime.InteropServices;

namespace DownloadManager
{
    public partial class DarkMessageBox : Form
    {
        #region DLL Import
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }

        internal static class NativeMethods
        {
            public const int SC_CLOSE = 0xF060;
            public const int MF_BYCOMMAND = 0;
            public const int MF_ENABLED = 0;
            public const int MF_GRAYED = 1;

            [DllImport("user32.dll")]
            public static extern IntPtr GetSystemMenu(IntPtr hWnd, bool revert);

            [DllImport("user32.dll")]
            public static extern int EnableMenuItem(IntPtr hMenu, int IDEnableItem, int enable);
        }
        #endregion

        MessageBoxButtons buttonType = MessageBoxButtons.OK;

        public DarkMessageBox(string message, string? title = "", MessageBoxButtons? buttons = MessageBoxButtons.OK, MessageBoxIcon? icon = MessageBoxIcon.Information, bool? enableClose = true)
        {
            InitializeComponent();
            richTextBox1.Text = message;

            if (title != null)
            {
                this.Text = title;
            }
            else
            {
                this.Text = "Download Manager";
            }

            if (buttons != null)
            {
                buttonType = (MessageBoxButtons)buttons;

                if (buttons == MessageBoxButtons.OK)
                {
                    button3.Visible = false;
                    button2.Visible = false;
                    button1.Text = "OK";
                    button1.TabIndex = 1;
                }
                else if (buttons == MessageBoxButtons.OKCancel)
                {
                    button3.Visible = false;
                    button2.Text = "OK";
                    button1.Text = "Cancel";
                    button2.TabIndex = 1;
                    button2.TabIndex = 2;
                }
                else if (buttons == MessageBoxButtons.AbortRetryIgnore)
                {
                    button3.Text = "Abort";
                    button2.Text = "Retry";
                    button1.Text = "Ignore";
                    button3.TabIndex = 1;
                    button2.TabIndex = 2;
                    button1.TabIndex = 3;
                }
                else if (buttons == MessageBoxButtons.YesNoCancel)
                {
                    button3.Text = "Yes";
                    button2.Text = "No";
                    button1.Text = "Cancel";
                    button3.TabIndex = 1;
                    button2.TabIndex = 2;
                    button1.TabIndex = 3;
                }
                else if (buttons == MessageBoxButtons.YesNo)
                {
                    button3.Visible = false;
                    button2.Text = "Yes";
                    button1.Text = "No";
                    button2.TabIndex = 1;
                    button1.TabIndex = 2;
                }
                else if (buttons == MessageBoxButtons.RetryCancel)
                {
                    button3.Visible = false;
                    button2.Text = "Retry";
                    button1.Text = "Cancel";
                    button2.TabIndex = 1;
                    button1.TabIndex = 2;
                }
                else if (buttons == MessageBoxButtons.CancelTryContinue)
                {
                    button3.Text = "Cancel";
                    button2.Text = "Retry";
                    button1.Text = "Continue";
                    button3.TabIndex = 1;
                    button2.TabIndex = 2;
                    button1.TabIndex = 3;
                }
                else
                {
                    buttonType = MessageBoxButtons.OK;
                    button3.Visible = false;
                    button2.Visible = false;
                    button1.TabIndex = 1;
                    button1.Text = "OK";
                }
            }
            else
            {
                button3.Visible = false;
                button2.Visible = false;
                button1.Text = "OK";
                button1.TabIndex = 1;
                buttonType = MessageBoxButtons.OK;
            }

            if (icon != null)
            {
                if (icon == MessageBoxIcon.Hand || icon == MessageBoxIcon.Stop || icon == MessageBoxIcon.Error)
                {
                    pictureBox1.BackgroundImage = Properties.Resources.error;
                    if (Settings.Default.soundOnMessage == true)
                        SystemSounds.Hand.Play();
                }
                else if (icon == MessageBoxIcon.Warning)
                {
                    pictureBox1.BackgroundImage = Properties.Resources.warn;
                    if (Settings.Default.soundOnMessage == true)
                        SystemSounds.Exclamation.Play();
                }
                else if (icon == MessageBoxIcon.Asterisk || icon == MessageBoxIcon.Information)
                {
                    pictureBox1.BackgroundImage = Properties.Resources.info;
                    if (Settings.Default.soundOnMessage == true)
                        SystemSounds.Asterisk.Play();
                }
                else if (icon == MessageBoxIcon.Question)
                {
                    pictureBox1.BackgroundImage = Properties.Resources.question;
                    if (Settings.Default.soundOnMessage == true)
                        SystemSounds.Asterisk.Play();
                }
            }
            else
            {
                pictureBox1.BackgroundImage = Properties.Resources.info;
                if (Settings.Default.soundOnMessage == true)
                    SystemSounds.Asterisk.Play();
            }

            if (enableClose != null)
            {
                if (enableClose == true)
                {
                    // Keep close button enabled
                    return;
                }
                else
                {
                    // Disable close button
                    IntPtr hMenu = NativeMethods.GetSystemMenu(this.Handle, false);
                    if (hMenu != IntPtr.Zero)
                    {
                        NativeMethods.EnableMenuItem(hMenu,
                                                     NativeMethods.SC_CLOSE,
                                                     NativeMethods.MF_BYCOMMAND | (false ? NativeMethods.MF_ENABLED : NativeMethods.MF_GRAYED));
                    }
                }
            }
            else
            {
                // Keep close button enabled
                return;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (buttonType == MessageBoxButtons.AbortRetryIgnore)
            {
                this.DialogResult = DialogResult.Abort;
                this.Close();
            }
            else if (buttonType == MessageBoxButtons.YesNoCancel)
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
            else if (buttonType == MessageBoxButtons.CancelTryContinue)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (buttonType == MessageBoxButtons.AbortRetryIgnore)
            {
                this.DialogResult = DialogResult.Retry;
                this.Close();
            }
            else if (buttonType == MessageBoxButtons.CancelTryContinue)
            {
                this.DialogResult = DialogResult.Retry;
                this.Close();
            }
            else if (buttonType == MessageBoxButtons.OKCancel)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else if (buttonType == MessageBoxButtons.RetryCancel)
            {
                this.DialogResult = DialogResult.Retry;
                this.Close();
            }
            else if (buttonType == MessageBoxButtons.YesNo)
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
            else if (buttonType == MessageBoxButtons.YesNoCancel)
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (buttonType == MessageBoxButtons.AbortRetryIgnore)
            {
                this.DialogResult = DialogResult.Ignore;
                this.Close();
            }
            else if (buttonType == MessageBoxButtons.CancelTryContinue)
            {
                this.DialogResult = DialogResult.Continue;
                this.Close();
            }
            else if (buttonType == MessageBoxButtons.OKCancel)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else if (buttonType == MessageBoxButtons.RetryCancel)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else if (buttonType == MessageBoxButtons.YesNo)
            {
                this.DialogResult = DialogResult.No;
                this.Close();
            }
            else if (buttonType == MessageBoxButtons.YesNoCancel)
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else if (buttonType == MessageBoxButtons.OK)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
