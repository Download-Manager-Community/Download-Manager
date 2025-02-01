using DownloadManager.NativeMethods;
using System.Media;

namespace DownloadManager.Components
{
    public partial class DarkMessageBoxForm : Form
    {
        // Initialise variables
        MessageBoxButtons buttonType;

        /// <summary>
        /// Constructor for the DarkMessageBoxForm
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="title">The title the message box should have.</param>
        /// <param name="buttons">The buttons contained on the message box.</param>
        /// <param name="icon">The icon that the message box will use.</param>
        /// <param name="enableClose">If the message box window should be allowed to be closed using the close button.</param>
        public DarkMessageBoxForm(string message, string? title = "", MessageBoxButtons? buttons = MessageBoxButtons.OK, MessageBoxIcon? icon = MessageBoxIcon.Information, bool? enableClose = true)
        {
            // Initialise form and set styling
            InitializeComponent();
            DesktopWindowManager.SetWindowStyle(this.Handle);

            // Set the message box text
            messageBox.Text = message;

            // Set the form title
            if (title != null)
                this.Text = title; // Specified title
            else
                this.Text = "Download Manager"; // Default title

            // Set the buttons
            if (buttons != null)
            {
                buttonType = (MessageBoxButtons)buttons; // Set the button type

                if (buttons == MessageBoxButtons.OKCancel) // Set the button type for OKCancel
                {
                    button3.Visible = false;
                    button2.Text = "OK";
                    button1.Text = "Cancel";
                    button2.TabIndex = 1;
                    button2.TabIndex = 2;
                }
                else if (buttons == MessageBoxButtons.AbortRetryIgnore) // Set the button type for AbortRetryIgnore
                {
                    button3.Text = "Abort";
                    button2.Text = "Retry";
                    button1.Text = "Ignore";
                    button3.TabIndex = 1;
                    button2.TabIndex = 2;
                    button1.TabIndex = 3;
                }
                else if (buttons == MessageBoxButtons.YesNoCancel) // Set the button type for YesNoCancel
                {
                    button3.Text = "Yes";
                    button2.Text = "No";
                    button1.Text = "Cancel";
                    button3.TabIndex = 1;
                    button2.TabIndex = 2;
                    button1.TabIndex = 3;
                }
                else if (buttons == MessageBoxButtons.YesNo) // Set the button type for YesNo
                {
                    button3.Visible = false;
                    button2.Text = "Yes";
                    button1.Text = "No";
                    button2.TabIndex = 1;
                    button1.TabIndex = 2;
                }
                else if (buttons == MessageBoxButtons.RetryCancel) // Set the button type for RetryCancel
                {
                    button3.Visible = false;
                    button2.Text = "Retry";
                    button1.Text = "Cancel";
                    button2.TabIndex = 1;
                    button1.TabIndex = 2;
                }
                else if (buttons == MessageBoxButtons.CancelTryContinue) // Set the button type for CancelTryContinue
                {
                    button3.Text = "Cancel";
                    button2.Text = "Retry";
                    button1.Text = "Continue";
                    button3.TabIndex = 1;
                    button2.TabIndex = 2;
                    button1.TabIndex = 3;
                }
                else // Set the button type to OK (default)
                {
                    buttonType = MessageBoxButtons.OK;
                    button3.Visible = false;
                    button2.Visible = false;
                    button1.TabIndex = 1;
                    button1.Text = "OK";
                }
            }
            else // Set the button type to OK (default)
            {
                buttonType = MessageBoxButtons.OK;
                button3.Visible = false;
                button2.Visible = false;
                button1.Text = "OK";
                button1.TabIndex = 1;
            }

            // Set the icon and play associated sound (if enabled)
            if (icon != null)
            {
                // If the icon is one of the error icons, set the icon to the error icon
                if (icon == MessageBoxIcon.Hand || icon == MessageBoxIcon.Stop || icon == MessageBoxIcon.Error)
                {
                    pictureBox.BackgroundImage = Properties.Resources.error; // Set the image to the error icon
                    if (Settings.Default.soundOnMessage == true)
                        SystemSounds.Hand.Play(); // Play the error sound if enabled
                }
                else if (icon == MessageBoxIcon.Warning)
                {
                    pictureBox.BackgroundImage = Properties.Resources.warn; // Set the image to the warning icon
                    if (Settings.Default.soundOnMessage == true)
                        SystemSounds.Exclamation.Play(); // Play the warning sound if enabled
                }
                else if (icon == MessageBoxIcon.Asterisk || icon == MessageBoxIcon.Information)
                {
                    pictureBox.BackgroundImage = Properties.Resources.info; // Set the image to the information icon
                    if (Settings.Default.soundOnMessage == true)
                        SystemSounds.Asterisk.Play(); // Play the information sound if enabled
                }
                else if (icon == MessageBoxIcon.Question)
                {
                    pictureBox.BackgroundImage = Properties.Resources.question; // Set the image to the question icon
                    if (Settings.Default.soundOnMessage == true)
                        SystemSounds.Asterisk.Play(); // Play the information sound if enabled
                }
            }
            else
            {
                // If an icon is not specified, default the icon to the information icon
                pictureBox.BackgroundImage = Properties.Resources.info; // Set the image to the information icon
                if (Settings.Default.soundOnMessage == true)
                    SystemSounds.Asterisk.Play(); // Play the information sound if enabled
            }

            // Change the close button
            if (enableClose != null)
            {
                if (enableClose == true)
                {
                    // Enable the close button
                    NativeMethods.User32.ToggleCloseButton(this.Handle, true);
                }
                else
                {
                    // Disable close button
                    NativeMethods.User32.ToggleCloseButton(this.Handle, false);
                }
            }
            else
            {
                // Enable the close button (default)
                NativeMethods.User32.ToggleCloseButton(this.Handle, true);
            }
        }

        // Button 3 click event, can be used for Abort, Yes, or Cancel
        private void button3_Click(object sender, EventArgs e)
        {
            if (buttonType == MessageBoxButtons.AbortRetryIgnore) // If the type is AbortRetryIgnore
            {
                this.DialogResult = DialogResult.Abort; // Set the dialog result to Abort
                this.Close(); // Close the form
            }
            else if (buttonType == MessageBoxButtons.YesNoCancel) // If the type is YesNoCancel
            {
                this.DialogResult = DialogResult.Yes; // Set the dialog result to Yes
                this.Close(); // Close the form
            }
            else if (buttonType == MessageBoxButtons.CancelTryContinue) // If the type is CancelTryContinue
            {
                this.DialogResult = DialogResult.Cancel; // Set the dialog result to Cancel
                this.Close(); // Close the form
            }
        }

        // Button 2 click event, can be used for Retry, OK, Yes, or No
        private void button2_Click(object sender, EventArgs e)
        {
            if (buttonType == MessageBoxButtons.AbortRetryIgnore) // If the type is AbortRetryIgnore
            {
                this.DialogResult = DialogResult.Retry; // Set the dialog result to Retry
                this.Close(); // Close the form
            }
            else if (buttonType == MessageBoxButtons.YesNoCancel) // If the type is YesNoCancel
            {
                this.DialogResult = DialogResult.No; // Set the dialog result to No
                this.Close(); // Close the form
            }
            else if (buttonType == MessageBoxButtons.CancelTryContinue) // If the type is CancelTryContinue
            {
                this.DialogResult = DialogResult.Retry; // Set the dialog result to Retry
                this.Close(); // Close the form
            }
            else if (buttonType == MessageBoxButtons.YesNo) // If the type is YesNo
            {
                this.DialogResult = DialogResult.Yes; // Set the dialog result to Yes
                this.Close(); // Close the form
            }
            else if (buttonType == MessageBoxButtons.OKCancel) // If the type is OKCancel
            {
                this.DialogResult = DialogResult.OK; // Set the dialog result to OK
                this.Close(); // Close the form
            }
            else if (buttonType == MessageBoxButtons.RetryCancel) // If the type is RetryCancel
            {
                this.DialogResult = DialogResult.Retry; // Set the dialog result to Retry
                this.Close(); // Close the form
            }
        }

        // Button 1 click event, can be used for Ignore, Continue, Cancel, No, Cancel, or OK
        private void button1_Click(object sender, EventArgs e)
        {
            if (buttonType == MessageBoxButtons.AbortRetryIgnore) // If the type is AbortRetryIgnore
            {
                this.DialogResult = DialogResult.Ignore; // Set the dialog result to Ignore
                this.Close(); // Close the form
            }
            else if (buttonType == MessageBoxButtons.YesNoCancel) // If the type is YesNoCancel
            {
                this.DialogResult = DialogResult.Cancel; // Set the dialog result to Cancel
                this.Close(); // Close the form
            }
            else if (buttonType == MessageBoxButtons.CancelTryContinue) // If the type is CancelTryContinue
            {
                this.DialogResult = DialogResult.Continue; // Set the dialog result to Continue
                this.Close(); // Close the form
            }
            else if (buttonType == MessageBoxButtons.YesNo) // If the type is YesNo
            {
                this.DialogResult = DialogResult.No; // Set the dialog result to No
                this.Close(); // Close the form
            }
            else if (buttonType == MessageBoxButtons.OKCancel) // If the type is OKCancel
            {
                this.DialogResult = DialogResult.Cancel; // Set the dialog result to Cancel
                this.Close(); // Close the form
            }
            else if (buttonType == MessageBoxButtons.RetryCancel) // If the type is RetryCancel
            {
                this.DialogResult = DialogResult.Cancel; // Set the dialog result to Cancel
                this.Close(); // Close the form
            }
            else if (buttonType == MessageBoxButtons.OK) // If the type is OK
            {
                this.DialogResult = DialogResult.OK; // Set the dialog result to OK
                this.Close(); // Close the form
            }
        }
    }

    public static class DarkMessageBox
    {
        /// <summary>
        /// Show the custom themed message box
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="title">The title the message box should have.</param>
        /// <param name="buttons">The buttons contained on the message box.</param>
        /// <param name="icon">The icon that the message box will use.</param>
        /// <param name="enableClose">If the message box window should be allowed to be closed using the close button.</param>
        /// <returns>The result of the dialog.</returns>
        public static DialogResult Show(string message, string? title = "", MessageBoxButtons? buttons = MessageBoxButtons.OK, MessageBoxIcon? icon = MessageBoxIcon.Information, bool? enableClose = true)
        {
            // Create a new instance of the form
            DarkMessageBoxForm darkMessageBoxForm = new DarkMessageBoxForm(message, title, buttons, icon, enableClose);
            return darkMessageBoxForm.ShowDialog(); // Show the form as a dialog box and return the result
        }
    }
}
