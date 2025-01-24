namespace DownloadManager.Controls
{
    internal class DarkToolStripRenderer : System.Windows.Forms.ToolStripRenderer
    {
        protected override void OnRenderToolStripBackground(System.Windows.Forms.ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBackground(e);
            e.Graphics.Clear(System.Drawing.Color.FromArgb(0, 0, 0));
        }

        protected override void OnRenderItemText(System.Windows.Forms.ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = System.Drawing.Color.White;
            base.OnRenderItemText(e);
        }

        protected override void OnRenderSeparator(System.Windows.Forms.ToolStripSeparatorRenderEventArgs e)
        {
            base.OnRenderSeparator(e);
            e.Graphics.FillRectangle(System.Drawing.Brushes.Gray, e.Item.ContentRectangle);
        }

        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderMenuItemBackground(e);
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(System.Drawing.Brushes.Blue, e.Item.ContentRectangle);
            }
            else if (e.Item.Pressed)
            {
                e.Graphics.FillRectangle(System.Drawing.Brushes.Blue, e.Item.ContentRectangle);
            }
        }

        protected override void OnRenderButtonBackground(System.Windows.Forms.ToolStripItemRenderEventArgs e)
        {
            base.OnRenderButtonBackground(e);
            if (e.Item.Selected)
            {
                e.Graphics.FillRectangle(System.Drawing.Brushes.Blue, e.Item.ContentRectangle);
            }
            else if (e.Item.Pressed)
            {
                e.Graphics.FillRectangle(System.Drawing.Brushes.Blue, e.Item.ContentRectangle);
            }
        }
    }
}
