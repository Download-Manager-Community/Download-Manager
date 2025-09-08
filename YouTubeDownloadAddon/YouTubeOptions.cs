using DownloadManager.NativeMethods;
using YouTubeDownloadAddon;

namespace DownloadManager.Components.Addons.YouTubeDownloader
{
    public partial class YouTubeOptions : Form
    {
        public YouTubeOptions()
        {
            InitializeComponent();

            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);
        }

        private void downloadButton_Click(object sender, EventArgs e)
        {
            UpdateStatus("Initializing...");
            ToggleControls(false);
            Addon.Download(urlBox.Text, convertCheckBox.Checked, this);
        }

        /// <summary>
        /// Toggle the enabled status of the controls.
        /// </summary>
        /// <param name="enabled">Enables or disables all controls.</param>
        public void ToggleControls(bool enabled)
        {
            // Toggle controls
            urlBox.Enabled = enabled;
            convertCheckBox.Enabled = enabled;
            downloadButton.Enabled = enabled;

            // Toggle the close button of the window
            NativeMethods.User32.ToggleCloseButton(this.Handle, enabled);
        }

        /// <summary>
        /// Handles the custom painting of the download button when it is disabled.
        /// </summary>
        /// <remarks>This method ensures that the text of the download button is rendered in a visually
        /// distinct style when the button is disabled, using a gray color and centered alignment.
        /// Only required as default disabled text is invisible.</remarks>
        /// <param name="sender">The source of the event, typically the download button.</param>
        /// <param name="e">The <see cref="PaintEventArgs"/> containing data for the paint event.</param>
        private void downloadButton_Paint(object sender, PaintEventArgs e)
        {
            if (!downloadButton.Enabled)
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    downloadButton.Text,
                    downloadButton.Font,
                    downloadButton.ClientRectangle,
                    SystemColors.GrayText, // Use a color that contrasts with the background
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
                );
            }
        }

        /// <summary>
        /// Handles the custom painting of the convert checkbox when it is disabled.
        /// </summary>
        /// <remarks>This method ensures that the text of the convert checkbox is rendered in a visually
        /// distinct style when the button is disabled, using a gray color and centered alignment.
        /// Only required as default disabled text is invisible.</remarks>
        /// <param name="sender">The source of the event, typically the convert checkbox.</param>
        /// <param name="e">The <see cref="PaintEventArgs"/> containing data for the paint event.</param>
        private void convertCheckBox_Paint(object sender, PaintEventArgs e)
        {
            if (!convertCheckBox.Enabled)
            {
                TextRenderer.DrawText(
                    e.Graphics,
                    convertCheckBox.Text,
                    convertCheckBox.Font,
                    convertCheckBox.ClientRectangle,
                    SystemColors.GrayText, // Use a color that contrasts with the background
                    TextFormatFlags.Default | TextFormatFlags.Default
                );
            }
        }

        public void UpdateStatus(string status, bool active = true)
        {
            progressBar.CustomText = $"Status: {status}";

            if (active)
            {
                progressBar.MarqueeAnim = true;
                progressBar.Style = ProgressBarStyle.Marquee;
            }
            else
            {
                progressBar.MarqueeAnim = false;
                progressBar.Style = ProgressBarStyle.Blocks;
            }
        }
    }
}