using System.Drawing.Drawing2D;

namespace DownloadManager.Controls
{
    public class DataGridViewProgressColumn : DataGridViewImageColumn
    {
        public DataGridViewProgressColumn()
        {
            CellTemplate = new DataGridViewProgressCell();
        }
    }
}
namespace DownloadManager.Controls
{
    public class DataGridViewProgressCell : DataGridViewImageCell
    {
        internal System.Windows.Forms.Timer marqueeTimer = new System.Windows.Forms.Timer();
        internal Rectangle progressBarBounds = Rectangle.Empty;
        internal int marqueePosition = 0;

        public DataGridViewProgressCell()
        {
            try
            {
                this.Tag = 10;
                // Create a timer to update the marquee position
                marqueeTimer.Interval = 30;
                marqueeTimer.Tick += MarqueeTimer_Tick;
                //marqueeTimer.Start();
            }
            catch (Exception ex)
            {
                if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime)
                {
                    Logging.Log($"[DataViewProgressColumnControl] {ex.Message} ({ex.GetType().FullName})\n{ex.StackTrace}", Color.Red);
                }
                else
                {
                    Console.WriteLine($"[DataViewProgressColumnControl] {ex.Message} ({ex.GetType().FullName})\n{ex.StackTrace}");
                }
            }
        }

        private void MarqueeTimer_Tick(object sender, EventArgs e)
        {
            /*try
            {
                if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime)
                {
                    if (progressBarBounds.Width != 0)
                    {
                        // Update the marquee position
                        marqueePosition = (marqueePosition + 5) % (progressBarBounds.Width + progressBarBounds.Width / 3);

                        // Redraw the control
                        //this.DataGridView.Refresh();
                    }
                }
            }
            catch (Exception ex)
            {
                if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime)
                {
                    Logging.Log($"[DataViewProgressColumnControl] {ex.Message} ({ex.GetType().FullName})\n{ex.StackTrace}", Color.Red);
                }
                else
                {
                    Console.WriteLine($"[DataViewProgressColumnControl] {ex.Message} ({ex.GetType().FullName})\n{ex.StackTrace}");
                }
            }*/
        }

        // Override Paint method
        protected override void Paint(Graphics g, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            try
            {
                try
                {
                    if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime)
                    {
                        if (progressBarBounds.Width != 0)
                        {
                            // Update the marquee position
                            marqueePosition = (marqueePosition + 5) % (progressBarBounds.Width + progressBarBounds.Width / 3);

                            // Redraw the control
                            //this.DataGridView.Refresh();
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime)
                    {
                        Logging.Log($"[DataViewProgressColumnControl] {ex.Message} ({ex.GetType().FullName})\n{ex.StackTrace}", Color.Red);
                    }
                    else
                    {
                        Console.WriteLine($"[DataViewProgressColumnControl] {ex.Message} ({ex.GetType().FullName})\n{ex.StackTrace}");
                    }
                }

                progressBarBounds = cellBounds;

                int progressVal = (int)Tag;
                float percentage = ((float)progressVal / 100.0f); // assuming value is in percentage
                Brush backColorBrush = new SolidBrush(cellStyle.BackColor);
                Brush foreColorBrush = new SolidBrush(cellStyle.ForeColor);

                // base.Paint - Paints the cell background.
                base.Paint(g, clipBounds, cellBounds, rowIndex, cellState, (int)Tag, 0, errorText, cellStyle, advancedBorderStyle, (paintParts & ~DataGridViewPaintParts.ContentForeground));

                if (progressVal >= 0)
                {
                    // Draw the progress bar and the text
                    int width = (int)(cellBounds.Width * ((int)Tag / 100));
                    g.FillRectangle(Brushes.Blue, cellBounds.X, cellBounds.Y, width, cellBounds.Height);

                    // Draw the text
                    using (StringFormat format = new StringFormat())
                    {
                        format.Alignment = StringAlignment.Center;
                        format.LineAlignment = StringAlignment.Center;

                        // Calculate the percentage of the progress bar
                        int percent = (int)(((int)Tag / (double)100) * 100);

                        // Draw the percentage text
                        g.DrawString(percent + "%", cellStyle.Font, Brushes.White, cellBounds, format);
                    }

                    //g.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 255)), cellBounds.X, cellBounds.Y + 2, Convert.ToInt32((percentage * cellBounds.Width - 4)), cellBounds.Height - 4);
                    //g.DrawString(progressVal.ToString() + "%", cellStyle.Font, foreColorBrush, cellBounds.X + (cellBounds.Width / 2) - 5, cellBounds.Y + 2);
                }
                else
                {
                    if (this.DataGridView.CurrentRow == null)
                    {
                        return;
                    }

                    g.SetClip(cellBounds, CombineMode.Replace);

                    // Calculate the width of the marquee block
                    int width = cellBounds.Width / 3;
                    // Calculate the x-coordinate of the left edge of the marquee block
                    int x = cellBounds.X + marqueePosition - width;
                    // Draw the marquee-style progress bar
                    g.FillRectangle(Brushes.Blue, x, cellBounds.Y, width, cellBounds.Height);

                    // draw the text
                    if (this.DataGridView.CurrentRow.Index == rowIndex)
                        g.DrawString("Processing...", cellStyle.Font, new SolidBrush(cellStyle.SelectionForeColor), cellBounds.X + 6, cellBounds.Y + 2);
                    else
                        g.DrawString("Processing...", cellStyle.Font, foreColorBrush, cellBounds.X + 6, cellBounds.Y + 2);

                    g.ResetClip();
                }
            }
            catch (Exception ex)
            {
                if (System.ComponentModel.LicenseManager.UsageMode != System.ComponentModel.LicenseUsageMode.Designtime)
                {
                    Logging.Log($"[DataViewProgressColumnControl] {ex.Message} ({ex.GetType().FullName})\n{ex.StackTrace}", Color.Red);
                }
                else
                {
                    Console.WriteLine($"[DataViewProgressColumnControl] {ex.Message} ({ex.GetType().FullName})\n{ex.StackTrace}");
                }
            }
        }
    }
}