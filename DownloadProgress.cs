using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using static DownloadManager.Logging;

namespace DownloadManager
{
    public partial class DownloadProgress : Form
    {
        #region DLL Import
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr one, int two, int three, int four);
        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn
        (
            int nLeftRect,     // x-coordinate of upper-left corner
            int nTopRect,      // y-coordinate of upper-left corner
            int nRightRect,    // x-coordinate of lower-right corner
            int nBottomRect,   // y-coordinate of lower-right corner
            int nWidthEllipse, // width of ellipse
            int nHeightEllipse // height of ellipse
        );
        #endregion

        string url;
        string location;
        string authUser;
        string authPass;
        int authType;
        bool isUrlInvalid = false;
        bool downloading = true;
        bool closeOnComplete = false;
        WebClient client = new WebClient();

        public DownloadProgress(string urlArg, string locationArg, string authUserArg, string authPassArg, int authTypeArg)
        {
            InitializeComponent();
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 10, 10));
            authUser = authUserArg;
            authPass = authPassArg;
            authType = authTypeArg;
            DownloadForm.downloadsAmount += 1;
            textBox1.Text = urlArg;
            location = locationArg + @"\";
            url = urlArg;
        }

        private void progress_Load(object sender, EventArgs e)
        {
            Log("Preparing to start downloading...", Color.Black);
            progressBar1.Style = ProgressBarStyle.Marquee;
            Thread thread = new Thread(() =>
            {
                try
                {
                    client.OpenRead(url);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Download Manager - Error Fetching File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (this.IsHandleCreated == true)
                    {
                        Invoke(new MethodInvoker(delegate ()
                        {
                            DownloadForm.downloadsAmount -= 1;
                            client.CancelAsync();
                            client.Dispose();
                            this.Close();
                            this.Dispose();
                        }));
                    }
                    return;
                }
                Int64 bytes_total = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
                Action action = () => label4.Text = "File Size: " + bytes_total.ToString() + " bytes";
                this.Invoke(action);
                client.DownloadFileCompleted += Client_DownloadFileCompleted;
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                // ---------------------------------------------------------------------------------- START
                Uri uri;

                if (authType == 0)
                {
                    uri = new Uri(url);
                }
                else if (authType == 1)
                {
                    client.Credentials = new NetworkCredential(authUser, authPass);
                    uri = new Uri(url);
                }
                else if (authType == 2)
                {
                    bool ishttp = false;
                    bool ishttps = false;

                    if (url.Contains("http://"))
                    {
                        url = url.Replace("http://", "");
                        ishttp = true;
                    }
                    else if (url.Contains("https://"))
                    {
                        url = url.Replace("https://", "");
                        ishttps = true;
                    }
                    else
                    {
                        MessageBox.Show("Failed to determine whether URL is http:// or https://", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        client.Dispose();
                        this.Close();
                        this.Dispose();
                        return;
                    }

                    if (ishttp == true)
                    {
                        uri = new Uri("http://" + authUser + ":" + authPass + "@" + url);
                    }
                    else if (ishttps == true)
                    {
                        uri = new Uri("https://" + authUser + ":" + authPass + "@" + url);
                    }
                    else
                    {
                        MessageBox.Show("Failed to determine whether URL is http:// or https://", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        client.Dispose();
                        this.Close();
                        this.Dispose();
                        return;
                    }
                }
                else
                {
                    uri = new Uri(url);
                }

                // ---------------------------------------------------------------------------------- END
                string fileName = System.IO.Path.GetFileName(uri.AbsolutePath);
                Action action1 = () => progressBar1.Style = ProgressBarStyle.Blocks;
                this.Invoke(action1);
                try
                {
                    client.DownloadFileAsync(uri, location + fileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Download Manager - Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Invoke(new MethodInvoker(delegate ()
                    {
                        DownloadForm.downloadsAmount -= 1;
                        client.CancelAsync();
                        client.Dispose();
                        this.Close();
                        this.Dispose();
                    }));
                    return;
                }
            });
            thread.Start();
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            //Update progress bar & label
            if (this.IsHandleCreated)
            {
                try
                {
                    Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Minimum = 0;
                        double receive = double.Parse(e.BytesReceived.ToString());
                        double total = double.Parse(e.TotalBytesToReceive.ToString());
                        double percentage = receive / total * 100;
                        label3.Text = "Percentage Complete: " + $"{string.Format("{0:0.##}", percentage)}%";
                        try
                        {
                            progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
                        }
                        catch (Exception ex)
                        {
                            if (isUrlInvalid == false)
                            {
                                isUrlInvalid = true;
                                DownloadForm.downloadsAmount -= 1;
                                MessageBox.Show(ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                client.Dispose();
                                this.Close();
                                this.Dispose();
                                return;
                            }
                            else { }
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Log(ex.Message + Environment.NewLine + ex.StackTrace, Color.Red);
                }
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                Invoke(new MethodInvoker(delegate ()
                {
                    checkBox2.Enabled = false;
                    button2.Enabled = false;
                    downloading = false;
                    DownloadForm.downloadsAmount -= 1;
                    if (closeOnComplete == true)
                    {
                        client.Dispose();
                        this.Close();
                        this.Dispose();
                        return;
                    }
                }));
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            // Title-bar Drag
            ReleaseCapture();
            SendMessage(Handle, 0x112, 0xf012, 0);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            // Minimize
            WindowState = FormWindowState.Minimized;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            // TopMost
            if (checkBox1.Checked == true)
            {
                TopMost = true;
            }
            else
            {
                TopMost = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            // Close on complete
            if (checkBox2.Checked == true)
            {
                closeOnComplete = true;
            }
            else
            {
                closeOnComplete = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Close
            if (!downloading)
            {
                this.Close();
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure you want to cancel the download?", "Jarvis Download Manager", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.OK)
                {
                    client.Dispose();
                    this.Close();
                }
            }
        }
    }
}
