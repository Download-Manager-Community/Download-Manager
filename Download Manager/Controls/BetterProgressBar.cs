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

        /// <summary>
        /// Indicates the state of the progress bar.
        /// </summary>
        [Browsable(true)]
        [Category("Appearance")]
        [DefaultValue(ProgressBarState.Normal)]
        [Description("Indicates the state of the progress bar.\nNormal = Blue\nWarning = Orange\nError = Red")]
        public ProgressBarState State { get; set; } = ProgressBarState.Normal;

        /// <summary>
        /// Normal = Blue, 
        /// Warning = Orange,
        /// Error = Red
        /// </summary>
        public enum ProgressBarState
        {
            Normal,
            Warning,
            Error
        }

        public BetterProgressBar()
        {
            // Set the UserPaint style to true
            SetStyle(ControlStyles.UserPaint, true);
            // Enable double buffering
            DoubleBuffered = true;

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
            if (MarqueeAnim)
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
            if (Style == ProgressBarStyle.Marquee)
            {
                // Calculate the width of the marquee block
                int width = Width / 3;
                // Calculate the x-coordinate of the left edge of the marquee block
                int x = marqueePosition - width;
                // Draw the marquee-style progress bar
                e.Graphics.FillRectangle(Brushes.Blue, x, 0, width, Height);
            }
            else
            {
                // Draw the normal progress bar
                int width = (int)(Width * ((double)Value / Maximum));

                switch (State)
                {
                    case ProgressBarState.Normal:
                        e.Graphics.FillRectangle(Brushes.Blue, 0, 0, width, Height);
                        break;
                    case ProgressBarState.Warning:
                        e.Graphics.FillRectangle(Brushes.Orange, 0, 0, width, Height);
                        break;
                    case ProgressBarState.Error:
                        e.Graphics.FillRectangle(Brushes.Red, 0, 0, width, Height);
                        break;
                    default:
                        throw new NullReferenceException(nameof(State));
                }

                if (this.ShowText)
                {
                    using (StringFormat format = new StringFormat())
                    {
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;

                        // Calculate the percentage of the progress bar
                        int percent = (int)(((double)this.Value / (double)this.Maximum) * 100);

                        // Draw the percentage text
                        e.Graphics.DrawString(percent + "%", this.Font, Brushes.White, this.ClientRectangle, format);
                    }
                }
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            // Start or stop the marquee timer based on whether the control is visible
            if (Visible && Style == ProgressBarStyle.Marquee)
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
            if (Visible && Style == ProgressBarStyle.Marquee)
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
            if (Width != 0)
            {
                // Update the marquee position
                marqueePosition = (marqueePosition + 5) % (Width + Width / 3);

                // Redraw the control
                Invalidate();
            }
        }
    }
}