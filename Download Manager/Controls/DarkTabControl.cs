using System.ComponentModel;

namespace DownloadManager.Controls
{
    public class DarkTabControl : TabControl
    {
        public DarkTabControl()
        {
            // This line makes sure that the control will be drawn by the operating system
            // Then we can override the OnDrawItem to customize its appearance
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.DrawMode = TabDrawMode.OwnerDrawFixed;
        }

        private struct TabItemInfo
        {
            public Color BackColor;
            public Rectangle Bounds;
            public Font Font;
            public Color ForeColor;
            public int Index;
            public DrawItemState State;

            public TabItemInfo(DrawItemEventArgs e)
            {
                this.BackColor = e.BackColor;
                this.ForeColor = e.ForeColor;
                this.Bounds = e.Bounds;
                this.Font = e.Font;
                this.Index = e.Index;
                this.State = e.State;
            }
        }

        private Dictionary<int, TabItemInfo> _tabItemStateMap = new Dictionary<int, TabItemInfo>();

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);
            if (!_tabItemStateMap.ContainsKey(e.Index))
            {
                _tabItemStateMap.Add(e.Index, new TabItemInfo(e));
            }
            else
            {
                _tabItemStateMap[e.Index] = new TabItemInfo(e);
            }
        }

        private const int WM_PAINT = 0x000F;
        private const int WM_ERASEBKGND = 0x0014;

        // Cache context to avoid repeatedly re-creating the object.
        // WM_PAINT is called frequently so it's better to declare it as a member.
        private BufferedGraphicsContext _bufferContext = BufferedGraphicsManager.Current;

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_PAINT:
                    {
                        // Let system do its thing first.
                        base.WndProc(ref m);

                        // Custom paint Tab items.
                        if (!DesignMode)
                        {
                            HandlePaint(ref m);
                        }

                        break;
                    }
                case WM_ERASEBKGND:
                    {
                        if (DesignMode)
                        {
                            // Ignore to prevent flickering in DesignMode.
                        }
                        else
                        {
                            base.WndProc(ref m);
                        }
                        break;
                    }
                default:
                    base.WndProc(ref m);
                    break;
            }
        }


        private Color _backColor = Color.FromArgb(31, 31, 31);
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new Color BackColor
        {
            get
            {
                return _backColor;
            }
            set
            {
                _backColor = value;
            }
        }


        private void HandlePaint(ref Message m)
        {
            using (var g = Graphics.FromHwnd(m.HWnd))
            {
                SolidBrush backBrush = new SolidBrush(BackColor);
                Rectangle r = ClientRectangle;
                using (var buffer = _bufferContext.Allocate(g, r))
                {
                    if (Enabled)
                    {
                        buffer.Graphics.FillRectangle(backBrush, r);
                    }
                    else
                    {
                        buffer.Graphics.FillRectangle(backBrush, r);
                    }

                    // Paint items
                    foreach (int index in _tabItemStateMap.Keys)
                    {
                        DrawTabItemInternal(buffer.Graphics, _tabItemStateMap[index]);
                    }

                    buffer.Render();
                }
                backBrush.Dispose();
            }
        }


        private void DrawTabItemInternal(Graphics gr, TabItemInfo tabInfo)
        {
            if ((tabInfo.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                gr.FillRectangle(_tabBackBrush, tabInfo.Bounds);
                gr.DrawString(this.TabPages[tabInfo.Index].Text, tabInfo.Font,
                    _tabForeBrush, tabInfo.Bounds, _tabTextFormat);
            }
            else
            {
                gr.FillRectangle(_tabBackBrush, tabInfo.Bounds);
                gr.DrawString(this.TabPages[tabInfo.Index].Text, tabInfo.Font,
                    _tabForeBrush, tabInfo.Bounds, _tabTextFormat);
            }
        }

        private SolidBrush _backBrush;
        private SolidBrush _tabBackBrush;
        private SolidBrush _tabForeBrush;


        private Color _tabBackColor = Color.FromArgb(31, 31, 31);
        public Color TabBackColor
        {
            get
            {
                return _tabBackColor;
            }
            set
            {
                _tabBackColor = value;
                _tabBackBrush?.Dispose();
                _tabBackBrush = new SolidBrush(_tabBackColor);
            }
        }


        private Color _tabForeColor = Color.FromArgb(241, 241, 241);
        public Color TabForeColor
        {
            get
            {
                return _tabForeColor;
            }
            set
            {
                _tabForeColor = value;
                _tabForeBrush?.Dispose();
                _tabForeBrush = new SolidBrush(_tabForeColor);
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                _backBrush.Dispose();
                _tabBackBrush.Dispose();
                _tabForeBrush.Dispose();

                base.Dispose(disposing);
            }
            catch (ObjectDisposedException) { }
            catch (NullReferenceException) { }
        }

        private StringFormat _tabTextFormat = new StringFormat();


        private void UpdateTextAlign()
        {
            switch (this.TextAlign)
            {
                case ContentAlignment.TopLeft:
                    _tabTextFormat.Alignment = StringAlignment.Near;
                    _tabTextFormat.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopCenter:
                    _tabTextFormat.Alignment = StringAlignment.Center;
                    _tabTextFormat.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.TopRight:
                    _tabTextFormat.Alignment = StringAlignment.Far;
                    _tabTextFormat.LineAlignment = StringAlignment.Near;
                    break;
                case ContentAlignment.MiddleLeft:
                    _tabTextFormat.Alignment = StringAlignment.Near;
                    _tabTextFormat.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleCenter:
                    _tabTextFormat.Alignment = StringAlignment.Center;
                    _tabTextFormat.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.MiddleRight:
                    _tabTextFormat.Alignment = StringAlignment.Far;
                    _tabTextFormat.LineAlignment = StringAlignment.Center;
                    break;
                case ContentAlignment.BottomLeft:
                    _tabTextFormat.Alignment = StringAlignment.Near;
                    _tabTextFormat.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomCenter:
                    _tabTextFormat.Alignment = StringAlignment.Center;
                    _tabTextFormat.LineAlignment = StringAlignment.Far;
                    break;
                case ContentAlignment.BottomRight:
                    _tabTextFormat.Alignment = StringAlignment.Far;
                    _tabTextFormat.LineAlignment = StringAlignment.Far;
                    break;
            }
        }


        private ContentAlignment _textAlign = ContentAlignment.TopLeft;
        public ContentAlignment TextAlign
        {
            get
            {
                return _textAlign;
            }
            set
            {
                if (value != _textAlign)
                {
                    _textAlign = value;
                    UpdateTextAlign();
                }
            }
        }
    }
}