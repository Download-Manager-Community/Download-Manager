using System.Runtime.InteropServices;

namespace DownloadManager
{
    public partial class WebViewWindow : Form
    {
        #region DLL Import
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }
        #endregion

        string url;

        public WebViewWindow(string url, string windowTitle)
        {
            InitializeComponent();
            this.Text = windowTitle;
            this.url = url;
            textBox1.Text = url;

            // Initialize webView
            webView.NavigationStarting += webView_NavigationStarted;
            webView.NavigationCompleted += webView_NavigationCompleted;
        }

        private void WebViewWindow_Load(object sender, EventArgs e)
        {
            // Navigate to url
            try
            {
                webView.CoreWebView2.Navigate(url);
            }
            catch (Exception ex)
            {
                DarkMessageBox msg = new DarkMessageBox(ex.ToString() + ": " + ex.Message + Environment.NewLine + ex.StackTrace, "WebView Fatal Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                msg.ShowDialog();
                webView.BackgroundImage = Properties.Resources.error;
                webView.BackgroundImageLayout = ImageLayout.Stretch;
            }
        }

        void webView_NavigationStarted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            // Update progress bar
            progressBar1.Style = ProgressBarStyle.Marquee;
        }

        void webView_NavigationCompleted(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            // Update url
            textBox1.Text = webView.CoreWebView2.Source;

            // Update progress bar
            progressBar1.Style = ProgressBarStyle.Blocks;
        }
    }
}
