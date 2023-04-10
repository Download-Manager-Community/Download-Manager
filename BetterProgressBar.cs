using System.ComponentModel;
using Timer = System.Windows.Forms.Timer;

namespace DownloadManager
{
    public class BetterProgressBar : ProgressBar
    {
        private Timer marqueeTimer;
        private Timer styleTimer;
        private int marqueePosition;

        /// <summary>
        /// Indicates whether the marquee animation should be playing on the progress bar.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(false)]
        [Description("Indicates whether the marquee animation should be playing on the progress bar.")]
        public bool MarqueeAnim { get; set; } = false;

        /// <summary>
        /// Indicates whether the text should be displayed on the progress bar.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(true)]
        [Description("Indicates whether the text should be displayed on the progress bar.")]
        public bool ShowText { get; set; } = true;

        public BetterProgressBar()
        {
            // Set the UserPaint style to true
            this.SetStyle(ControlStyles.UserPaint, true);
            // Enable double buffering
            this.DoubleBuffered = true;

            // Create a timer to update the marquee position
            marqueeTimer = new Timer();
            marqueeTimer.Interval = 30;
            marqueeTimer.Tick += MarqueeTimer_Tick;
            UpdateMarqueeTimer();

            // Create a timer to update the style
            styleTimer = new Timer();
            styleTimer.Interval = 1;
            styleTimer.Tick += StyleTimer_Tick;
            styleTimer.Start();
        }

        private void StyleTimer_Tick(object? sender, EventArgs e)
        {
            if (this.MarqueeAnim)
            {
                marqueeTimer.Start();
            }
            else
            {
                marqueeTimer.Stop();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (this.Style == ProgressBarStyle.Marquee)
            {
                // Calculate the width of the marquee block
                int width = this.Width / 3;
                // Calculate the x-coordinate of the left edge of the marquee block
                int x = marqueePosition - width;
                // Draw the marquee-style progress bar
                e.Graphics.FillRectangle(Brushes.Blue, x, 0, width, this.Height);
            }
            else
            {
                // Draw the normal progress bar
                int width = (int)(this.Width * ((double)this.Value / this.Maximum));
                e.Graphics.FillRectangle(Brushes.Blue, 0, 0, width, this.Height);
                if (this.ShowText)
                {
                    using (StringFormat format = new StringFormat())
                    {
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;
                        e.Graphics.DrawString(this.Value.ToString() + "%", this.Font, Brushes.White, this.ClientRectangle, format);
                    }
                }
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            // Start or stop the marquee timer based on whether the control is visible
            if (this.Visible && this.Style == ProgressBarStyle.Marquee)
            {
                marqueeTimer.Start();
            }
            else
            {
                marqueeTimer.Stop();
            }
        }

        private void UpdateMarqueeTimer()
        {
            if (this.Visible && this.Style == ProgressBarStyle.Marquee)
            {
                marqueeTimer.Start();
            }
            else
            {
                marqueeTimer.Stop();
            }
        }

        private void MarqueeTimer_Tick(object sender, EventArgs e)
        {
            // Update the marquee position
            marqueePosition = (marqueePosition + 5) % (this.Width + (this.Width / 3));

            // Redraw the control
            this.Invalidate();
        }
    }
}