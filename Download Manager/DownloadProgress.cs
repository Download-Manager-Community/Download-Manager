using DownloadManager.Components;
using DownloadManager.Download;
using DownloadManager.NativeMethods;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using System.Media;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using static DownloadManager.BetterProgressBar;
using static DownloadManager.Logging;

namespace DownloadManager
{
    public partial class DownloadProgress : Form
    {
        public static DownloadProgress _instance;
        Thread thread;
        public string url;
        public string location;
        public string fileName;
        public string hash;
        internal string hostName;
        public int hashType = 0;
        bool isUrlInvalid = false;
        public bool downloading = true;
        public bool isPaused = false;
        bool doFileVerify = false;
        FileStream? stream = null;
        SoundPlayer complete = new SoundPlayer(@"C:\WINDOWS\Media\tada.wav");

        public string fileSize = "0";
        public long totalSize = 0;
        public double percentageDone = 0;

        public double receivedBytes = 0;
        public double totalBytes = 0;
        public int totalProgress = 0;

        public bool cancelled = false;
        public bool forceCancel = false;
        internal bool restartNoProgress = false;
        internal int downloadAttempts = 0;
        internal bool doSafeMode = false;
        public CancellationTokenSource cancellationToken = new CancellationTokenSource();
        DownloadProgressUpdater progressUpdater = new DownloadProgressUpdater();

        internal Stream fileStream0;
        internal Stream fileStream1;

        internal double bytesPerSecond = 0;
        internal double kilobytesPerSecond = 0;
        internal double megabytesPerSecond = 0;

        public DownloadProgress(string urlArg, string locationArg, string hashArg, int hashTypeArg, int downloadAttempts = 0, bool doSafeMode = false)
        {
            InitializeComponent();
            _instance = this;

            this.doSafeMode = doSafeMode;

            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);

            this.downloadAttempts = downloadAttempts;

            hashType = hashTypeArg;
            hashType += 1;
            hash = hashArg;
            if (hash != "")
            {
                hashLabel.Text = "File verification on (Using hash: " + hash + ")";
                doFileVerify = true;
            }
            else
            {
                doFileVerify = false;
            }
            DownloadForm.downloadsAmount += 1;
            if (!locationArg.EndsWith("\\"))
            {
                location = locationArg + @"\";
            }
            else
            {
                location = locationArg;
            }
            url = urlArg;
        }

        protected override void WndProc(ref Message m)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_MINIMIZE = 0xF020;

            if (m.Msg == WM_SYSCOMMAND && (int)m.WParam == SC_MINIMIZE)
            {
                this.Hide(); // Hide the form
                return; // Don't continue with the default processing
            }

            base.WndProc(ref m);
        }


        private void progress_Load(object sender, EventArgs e)
        {
            Log(LogLevel.Info, "Preparing to start downloading...");
            checkBox2.Checked = Settings.Default.closeOnComplete;
            checkBox1.Checked = Settings.Default.keepOnTop;
            progressBar1.Visible = false;
            progressBar2.Visible = false;
            totalProgressBar.Visible = true;
            totalProgressBar.Style = ProgressBarStyle.Marquee;
            totalProgressBar.MarqueeAnim = true;
            if (doSafeMode)
            {
                Log(LogLevel.Warning, "Safe mode flag is set!");
                safeModeLabel.Visible = true;

                thread = new Thread(new ThreadStart(StartSafeModeDownload));
                thread.Start();
            }
            else
            {
                // Normal download
                thread = new Thread(new ThreadStart(StartNormalDownload));
                thread.Start();
            }
        }

        private async void StartSafeModeDownload()
        {
            try
            {
                if (File.Exists(location + fileName + ".download0"))
                {
                    File.Delete(location + fileName + ".download0");
                }

                if (File.Exists(location + fileName))
                {
                    File.Delete(location + fileName);
                }

                Uri uri = new Uri(url);

                hostName = uri.Host;
                fileName = HttpUtility.UrlDecode(Path.GetFileName(uri.AbsolutePath));

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false);
                Stream streamResponse = response.GetResponseStream();

                request = (HttpWebRequest)WebRequest.Create(uri);
                streamResponse = (await request.GetResponseAsync().ConfigureAwait(false)).GetResponseStream();

                Stream fileStream0 = File.Create(location + fileName + ".download0");

                // In safe mode so progress reporting is unavailable
                progressUpdater = null;

                this.Invoke(new MethodInvoker(delegate ()
                {
                    bytesLabel.Text = $"(0 B / ? B)";
                    this.Text = $"Downloading {fileName}... [Safe Mode]";
                    urlLabel.Text = $"{fileName} from {hostName}";
                    progressLabel.Text = "Progress report unavailable";
                    progressBar1.Visible = false;
                    progressBar2.Visible = false;
                    totalProgressBar.Visible = true;
                    totalProgressBar.Style = ProgressBarStyle.Marquee;
                    totalProgressBar.MarqueeAnim = true;
                }));

                await SaveFileStreamAsync(streamResponse, fileStream0, null, totalProgressBar);

                File.Move(location + fileName + ".download0", location + fileName);

                Client_DownloadFileCompleted();
            }
            catch (Exception ex)
            {
                if (Settings.Default.notifyFail)
                {
                    new ToastContentBuilder()
                       .AddText($"[Safe Mode]\nThe download of {fileName} has failed.")
                       .Show();
                }

                DarkMessageBox.Show($"{ex.Message} ({ex.GetType()})\n{ex.StackTrace}", "Download Error [Safe Mode]", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
            }
        }

        private async void StartNormalDownload()
        {
            Uri? uri = null;
            try
            {
                uri = new Uri(url);
            }
            catch (Exception ex)
            {
                this.Invoke(new MethodInvoker(delegate ()
                {
                    totalProgressBar.Value = 100;
                    totalProgressBar.State = ProgressBarState.Error;
                    totalProgressBar.ShowText = false;
                    totalProgressBar.Style = ProgressBarStyle.Blocks;
                }));

                downloading = false;
                DownloadForm.downloadsAmount -= 1;
                DownloadForm.downloadsList.Remove(this);

                Logging.Log(LogLevel.Error, $"Failed to parse URI.\n{ex.Message} ({ex.GetType().FullName})\n{ex.StackTrace}");
                DarkMessageBox.Show(ex.Message + $" ({ex.GetType().FullName})\n" + ex.StackTrace, "Failed to parse URI", MessageBoxButtons.OK, MessageBoxIcon.Error);

                this.Invoke(new MethodInvoker(delegate ()
                {
                    this.Close();
                    this.Dispose();
                }));
                return;
            }

            hostName = uri.Host;
            fileName = HttpUtility.UrlDecode(Path.GetFileName(uri.AbsolutePath));

            this.Invoke(new MethodInvoker(delegate ()
            {
                this.Text = $"Downloading {fileName}... (0%)";
                urlLabel.Text = fileName + " from " + hostName;
            }));

            if (!fileName.Contains("."))
            {
                DialogResult result = DarkMessageBox.Show($"The file you are attempting to download ({fileName}) does not have a file extension.\nThis may be because the site uses redirection to download files.\nPress Cancel to cancel the download.\nPress Retry to rename the file.\nPress Continue to continue downloading the file anyway. ", "Download Warning", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Warning, false);

                if (result == DialogResult.Cancel)
                {
                    downloading = false;
                    cancelled = true;
                    DownloadForm.downloadsAmount -= 1;

                    cancellationToken.Cancel();
                    Logging.Log(LogLevel.Warning, "Download of " + fileName + " has been canceled.");

                    if (Settings.Default.notifyFail)
                    {
                        new ToastContentBuilder()
                        .AddText($"The download of {fileName} has been canceled.")
                        .Show();
                    }

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        this.Close();
                        this.Dispose();
                    }));

                    this.Hide();
                    return;
                }
                else if (result == DialogResult.Retry)
                {
                    DialogResult? result1 = null;
                    SaveFileDialog? fileDialog = null;

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        fileDialog = new SaveFileDialog();
                        fileDialog.Filter = "All files|*.*";
                        fileDialog.FileName = fileName;
                        result1 = fileDialog.ShowDialog();
                    }));

                    if (fileDialog == null)
                    {
                        if (Settings.Default.notifyFail)
                        {
                            new ToastContentBuilder()
                               .AddText($"The download of {fileName} has failed.")
                               .Show();
                        }

                        Invoke(new MethodInvoker(delegate
                        {
                            progressBar1.Style = ProgressBarStyle.Blocks;
                            progressBar1.MarqueeAnim = false;
                            progressBar1.Value = 100;
                            progressBar1.State = ProgressBarState.Error;
                        }));

                        DarkMessageBox.Show("Failed to obtain results from save file dialog.\nfileDialog was null!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (stream != null)
                        {
                            stream.Close();
                        }

                        cancellationToken.Cancel();
                        downloading = false;
                        Invoke(new MethodInvoker(delegate ()
                        {
                            DownloadForm.downloadsAmount -= 1;

                            cancellationToken.Cancel();

                            this.Close();
                            this.Dispose();
                        }));
                        return;
                    }

                    if (result1 == null)
                    {
                        if (Settings.Default.notifyFail)
                        {
                            new ToastContentBuilder()
                               .AddText($"The download of {fileName} has failed.")
                               .Show();
                        }

                        Invoke(new MethodInvoker(delegate
                        {
                            progressBar1.Style = ProgressBarStyle.Blocks;
                            progressBar1.MarqueeAnim = false;
                            progressBar1.Value = 100;
                            progressBar1.State = ProgressBarState.Error;
                        }));

                        DarkMessageBox.Show("Failed to obtain results from save file dialog.\nDialogResult was null!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        if (stream != null)
                        {
                            stream.Close();
                        }

                        cancellationToken.Cancel();
                        downloading = false;
                        Invoke(new MethodInvoker(delegate ()
                        {
                            DownloadForm.downloadsAmount -= 1;

                            cancellationToken.Cancel();

                            this.Close();
                            this.Dispose();
                        }));
                        return;
                    }

                    if (result1 == DialogResult.OK)
                    {
                        fileName = Path.GetFileName(fileDialog.FileName);
                    }
                    else
                    {
                        downloading = false;
                        cancelled = true;
                        DownloadForm.downloadsAmount -= 1;

                        cancellationToken.Cancel();
                        Logging.Log(LogLevel.Warning, "Download of " + fileName + " has been canceled.");

                        if (Settings.Default.notifyFail)
                        {
                            new ToastContentBuilder()
                            .AddText($"The download of {fileName} has been canceled.")
                            .Show();
                        }

                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            this.Close();
                            this.Dispose();
                        }));

                        this.Hide();
                        return;
                    }
                }
            }

            this.Invoke(new MethodInvoker(delegate ()
            {
                totalProgressBar.Style = ProgressBarStyle.Blocks;
                totalProgressBar.MarqueeAnim = false;
                totalProgressBar.Visible = false;
                progressBar1.Visible = true;
                progressBar2.Visible = true;
            }));

            Log(LogLevel.Info, "Downloading file " + uri + " to " + location + fileName);
            try
            {
                await DownloadFileAsync(uri, cancellationToken.Token, Client_DownloadProgressChanged);

                if (restartNoProgress)
                {
                    cancellationToken.TryReset();
                    await DownloadFileAsync(uri, cancellationToken.Token);
                }

                Client_DownloadFileCompleted();
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                Log(LogLevel.Info, "Download of " + url + "was canceled.");
            }
            catch (OperationCanceledException ex)
            {
                Log(LogLevel.Info, "Download of " + url + "was canceled.");

                fileStream0.Close();
                fileStream1.Close();
            }
            catch (Exception ex)
            {
                if (stream != null)
                {
                    stream.Close();
                }

                if (this.IsHandleCreated)
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        progressBar1.Visible = false;
                        progressBar2.Visible = false;
                        totalProgressBar.Visible = true;
                        totalProgressBar.Style = ProgressBarStyle.Blocks;
                        totalProgressBar.MarqueeAnim = false;
                        totalProgressBar.Value = 100;
                        totalProgressBar.State = ProgressBarState.Error;
                    }));
                }

                if (Settings.Default.notifyFail)
                {
                    new ToastContentBuilder()
                       .AddText($"The download of {fileName} has failed.")
                       .Show();
                }

                try
                {
                    cancellationToken.Cancel();
                }
                catch { }

                Log(LogLevel.Error, ex.Message);
                DarkMessageBox.Show(ex.Message + $" ({ex.GetType().FullName})\n{ex.StackTrace}", "Download Manager - Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                downloading = false;
                Invoke(new MethodInvoker(delegate ()
                {
                    DownloadForm.downloadsAmount -= 1;

                    cancellationToken.Cancel();

                    this.Close();
                    this.Dispose();
                }));
                return;
            }
        }

        private Task CombineFilesIntoSingleFile(string fileName0, string fileName1, string outputFilePath)
        {
            this.Invoke(new MethodInvoker(delegate ()
            {
                progressBar1.Visible = false;
                progressBar2.Visible = false;
                totalProgressBar.Visible = true;

                totalProgressBar.MarqueeAnim = true;
                totalProgressBar.Style = ProgressBarStyle.Marquee;
            }));

            string[] inputFilePaths = new string[]{
                fileName0,
                fileName1
            };
            Logging.Log(LogLevel.Info, $"Number of files: {inputFilePaths.Length}.");
            using (var outputStream = File.Create(outputFilePath))
            {
                foreach (var inputFilePath in inputFilePaths)
                {
                    using (var inputStream = File.OpenRead(inputFilePath))
                    {
                        // Buffer size can be passed as the second argument.
                        inputStream.CopyTo(outputStream);
                    }
                    Logging.Log(LogLevel.Info, $"The file {inputFilePath} has been processed.");
                }
            }

            return Task.CompletedTask;
        }

        public async Task DownloadFileAsync(Uri uri, CancellationToken cancellationToken = default, Action<long, long, BetterProgressBar>? progressCallback = null, BetterProgressBar? progressBar = null)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));

            if (uri.IsFile)
            {
                await using Stream file = File.OpenRead(uri.LocalPath);
                await using Stream fileStream = File.Create(location + fileName);

                if (progressCallback != null)
                {
                    long length = file.Length;
                    byte[] buffer = new byte[4096];
                    int read;
                    int totalRead = 0;
                    while ((read = await file.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
                        totalRead += read;
                        progressCallback(totalRead, length, progressBar);
                    }
                    Debug.Assert(totalRead == length || length == -1);
                }
                else
                {
                    await file.CopyToAsync(fileStream, cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                if (File.Exists(location + fileName))
                {
                    DialogResult result = DarkMessageBox.Show("The file already exists.\nWould you like to overwrite the file?", "Download Manager - File Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, false);
                    if (result == DialogResult.Yes)
                    {
                        File.Delete(location + fileName);
                    }
                    else
                    {
                        downloading = false;
                        cancelled = true;
                        DownloadForm.downloadsAmount -= 1;

                        Logging.Log(LogLevel.Warning, "Download of " + fileName + " has been canceled.");

                        if (Settings.Default.notifyFail)
                        {
                            new ToastContentBuilder()
                            .AddText($"The download of {fileName} has been canceled.")
                            .Show();
                        }

                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            this.Close();
                            this.Dispose();
                        }));

                        this.Hide();
                        return;
                    }
                }

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync().ConfigureAwait(false);
                Stream streamResponse = response.GetResponseStream();
                long contentLength = response.ContentLength;

                totalSize = contentLength;
                fileSize = contentLength.ToString();

                request = (HttpWebRequest)WebRequest.Create(uri);
                request.AddRange(0, contentLength / 2);
                streamResponse = (await request.GetResponseAsync().ConfigureAwait(false)).GetResponseStream();

                fileStream0 = File.Create(location + fileName + ".download0");

                if (response.StatusCode == HttpStatusCode.RequestedRangeNotSatisfiable)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Visible = false;
                        progressBar2.Visible = false;
                        totalProgressBar.Visible = true;
                    }));

                    // The server does not support range requests or range not satisfiable
                    await SaveFileStreamAsync(streamResponse, fileStream0, progressCallback, totalProgressBar);

                    File.Move(location + fileName + ".download0", location + fileName);
                }
                else if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.PartialContent)
                {
                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Visible = true;
                        progressBar2.Visible = true;
                        totalProgressBar.Visible = false;
                    }));

                    if (contentLength == -1)
                    {
                        // The server failed to report content length so does not support range requests and we cannot report progress
                        // Dispose of multiple progress bars updater
                        progressUpdater = null;

                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            bytesLabel.Text = $"(0 B / ? B)";
                            this.Text = $"Downloading {fileName}...";
                            progressLabel.Text = "Progress report unavailable";
                            progressBar1.Visible = false;
                            progressBar2.Visible = false;
                            totalProgressBar.Visible = true;
                            totalProgressBar.Style = ProgressBarStyle.Marquee;
                            totalProgressBar.MarqueeAnim = true;
                        }));

                        await SaveFileStreamAsync(streamResponse, fileStream0, null, totalProgressBar);

                        File.Move(location + fileName + ".download0", location + fileName);

                        return;
                    }

                    progressUpdater.contentLength0 = contentLength / 2 + 1;
                    progressUpdater.contentLength1 = contentLength / 2 + 1;
                    progressUpdater.Initialize(contentLength, this);

                    // The server supports range requests
                    DownloadSegment segment0 = new DownloadSegment();
                    segment0.DownloadFileSegment(DownloadSegment.DownloadSegmentID.Segment0, this, streamResponse, fileStream0, contentLength / 2 + 1, contentLength, location + fileName + ".download0", progressBar1, cancellationToken, progressUpdater);

                    fileStream1 = File.Create(location + fileName + ".download1");

                    // Download the rest of the file
                    request = (HttpWebRequest)WebRequest.Create(uri);
                    request.AddRange(contentLength / 2 + 1, contentLength);
                    streamResponse = (await request.GetResponseAsync().ConfigureAwait(false)).GetResponseStream();
                    DownloadSegment segment1 = new DownloadSegment();
                    await segment1.DownloadFileSegment(DownloadSegment.DownloadSegmentID.Segment1, this, streamResponse, fileStream1, contentLength / 2 + 1, contentLength, location + fileName + ".download1", progressBar2, cancellationToken, progressUpdater);

                    while (segment0.isDownloading || segment1.isDownloading)
                    {
                        Application.DoEvents();
                        await Task.Delay(100);
                    }

                    this.Invoke(new MethodInvoker(delegate
                    {
                        updateDisplayTimer.Stop();
                        progressBar2.Value = 100;
                        this.Text = $"Downloading {fileName}... (100%)";
                        progressLabel.Text = $"100%";
                        bytesLabel.Text = $"({totalSize} B / {totalSize} B)";
                        speedLabel.Visible = false;
                    }));

                    await CombineFilesIntoSingleFile(location + fileName + ".download0", location + fileName + ".download1", location + fileName);

                    File.Delete(location + fileName + ".download0");
                    File.Delete(location + fileName + ".download1");
                }

                request.Abort();
                response.Close();
            }
        }

        private async Task SaveFileStreamAsync(Stream inputStream, Stream outputStream, Action<long, long, BetterProgressBar>? progressCallback, BetterProgressBar? progressBar = null)
        {
            Stopwatch stopwatch = new Stopwatch();

            if (progressCallback != null)
            {
                byte[] buffer = new byte[8 * 1024];
                int len;
                long totalRead = 0;

                stopwatch.Start();

                while ((len = await inputStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
                {
                    await outputStream.WriteAsync(buffer, 0, len).ConfigureAwait(false);
                    totalRead += len;

                    bytesPerSecond = totalRead / stopwatch.Elapsed.TotalSeconds;
                    kilobytesPerSecond = (totalRead / 1024) / stopwatch.Elapsed.TotalSeconds;
                    megabytesPerSecond = ((totalRead / 1024) / 1024) / stopwatch.Elapsed.TotalSeconds;

                    //Logging.Log($"Download speed: {bytesPerSecond} B/s, {kilobytesPerSecond} KB/s, {megabytesPerSecond} MB/s", Color.Gray);

                    progressCallback?.Invoke(totalRead, totalSize, progressBar);
                }

                if (progressCallback != null && totalSize != -1)
                {
                    Debug.Assert(totalRead == totalSize);
                }

                stopwatch.Stop();

                await outputStream.FlushAsync().ConfigureAwait(false);
                outputStream.Close();
            }
            else
            {
                byte[] buffer = new byte[8 * 1024];
                int len;
                long totalRead = 0;

                stopwatch.Start();

                while ((len = await inputStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
                {
                    await outputStream.WriteAsync(buffer, 0, len).ConfigureAwait(false);
                    totalRead += len;

                    bytesPerSecond = totalRead / stopwatch.Elapsed.TotalSeconds;
                    kilobytesPerSecond = (totalRead / 1024) / stopwatch.Elapsed.TotalSeconds;
                    megabytesPerSecond = ((totalRead / 1024) / 1024) / stopwatch.Elapsed.TotalSeconds;

                    //Logging.Log($"Download speed: {bytesPerSecond} B/s, {kilobytesPerSecond} KB/s, {megabytesPerSecond} MB/s", Color.Gray);

                    receivedBytes = totalRead;
                }

                stopwatch.Stop();

                await outputStream.FlushAsync().ConfigureAwait(false);
                outputStream.Close();
            }
        }

        private void Client_DownloadProgressChanged(long totalRead, long size, BetterProgressBar? progressBar = null)
        {
            if (progressBar == null)
            {
                //Update progress bar & label
                if (this.IsHandleCreated)
                {
                    try
                    {
                        totalSize = size;
                        fileSize = size.ToString();

                        totalProgressBar.Minimum = 0;
                        receivedBytes = totalRead;
                        totalBytes = size;
                        percentageDone = receivedBytes / totalBytes * 100;

                        Invoke(new MethodInvoker(delegate ()
                        {
                            try
                            {
                                totalProgressBar.Value = int.Parse(Math.Truncate(percentageDone).ToString());
                            }
                            catch (Exception ex)
                            {
                                if (isUrlInvalid == false)
                                {
                                    isUrlInvalid = true;
                                    DownloadForm.downloadsAmount -= 1;
                                    Log(LogLevel.Error, ex.Message);
                                    Invoke(new MethodInvoker(delegate
                                    {
                                        totalProgressBar.State = ProgressBarState.Error;
                                    }));
                                    DarkMessageBox.Show(ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                                    downloading = false;

                                    if (Settings.Default.notifyFail)
                                    {
                                        new ToastContentBuilder()
                                                    .AddText($"The download of {fileName} has failed.")
                                                    .Show();
                                    }

                                    cancellationToken.Cancel();

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
                        if (this.IsDisposed == false)
                        {
                            try
                            {
                                Invoke(new MethodInvoker(delegate
                                {
                                    totalProgressBar.Style = ProgressBarStyle.Blocks;
                                    totalProgressBar.MarqueeAnim = false;
                                    totalProgressBar.Value = 100;
                                    totalProgressBar.State = ProgressBarState.Error;
                                }));
                                DownloadForm.downloadsAmount -= 1;
                                downloading = false;

                                if (Settings.Default.notifyFail)
                                {
                                    new ToastContentBuilder()
                                                    .AddText($"The download of {fileName} has failed.")
                                                    .Show();
                                }

                                cancellationToken.Cancel();
                                this.Close();
                                this.Dispose();
                                return;
                            }
                            catch { }
                        }
                        Log(LogLevel.Error, ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                }
            }
            else
            {
                //Update progress bar & label
                if (this.IsHandleCreated)
                {
                    try
                    {
                        totalSize = size;
                        fileSize = size.ToString();

                        progressBar.Minimum = 0;
                        receivedBytes = totalRead;
                        totalBytes = size;
                        percentageDone = receivedBytes / totalBytes * 100;

                        Invoke(new MethodInvoker(delegate ()
                        {
                            try
                            {
                                progressBar.Value = int.Parse(Math.Truncate(percentageDone).ToString());
                            }
                            catch (Exception ex)
                            {
                                if (isUrlInvalid == false)
                                {
                                    isUrlInvalid = true;
                                    DownloadForm.downloadsAmount -= 1;
                                    Log(LogLevel.Error, ex.Message);
                                    Invoke(new MethodInvoker(delegate
                                    {
                                        progressBar.State = ProgressBarState.Error;
                                    }));
                                    DarkMessageBox.Show(ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                                    downloading = false;

                                    if (Settings.Default.notifyFail)
                                    {
                                        new ToastContentBuilder()
                                                    .AddText($"The download of {fileName} has failed.")
                                                    .Show();
                                    }

                                    cancellationToken.Cancel();

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
                        if (this.IsDisposed == false)
                        {
                            try
                            {
                                Invoke(new MethodInvoker(delegate
                                {
                                    progressBar.Style = ProgressBarStyle.Blocks;
                                    progressBar.MarqueeAnim = false;
                                    progressBar.Value = 100;
                                    progressBar.State = ProgressBarState.Error;
                                }));
                                DownloadForm.downloadsAmount -= 1;
                                downloading = false;

                                if (Settings.Default.notifyFail)
                                {
                                    new ToastContentBuilder()
                                                    .AddText($"The download of {fileName} has failed.")
                                                    .Show();
                                }

                                cancellationToken.Cancel();
                                this.Close();
                                this.Dispose();
                                return;
                            }
                            catch { }
                        }
                        Log(LogLevel.Error, ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                }
            }
        }

        private void Client_DownloadFileCompleted()
        {
            if (this.IsHandleCreated)
            {
                DownloadForm.downloadsList.Remove(this);

                pictureBox1.Image = Properties.Resources.fileTransferDone;

                this.Invoke(new MethodInvoker(delegate ()
                {
                    CurrentDownloads._instance.RefreshList();
                }));

                try
                {
                    Invoke(new MethodInvoker(delegate ()
                    {
                        bytesPerSecond = 0;
                        kilobytesPerSecond = 0;
                        megabytesPerSecond = 0;
                        speedLabel.Visible = false;

                        progressBar1.Visible = false;
                        progressBar2.Visible = false;
                        totalProgressBar.Visible = true;
                        totalProgressBar.Style = ProgressBarStyle.Marquee;
                        totalProgressBar.MarqueeAnim = true;
                        if (!isUrlInvalid)
                        {
                            if (doFileVerify)
                            {
                                Thread thread = new Thread(() =>
                                {
                                    //MD5
                                    if (hashType == 1)
                                    {
                                        byte[] myHash;
                                        using (var hash = MD5.Create())
                                        using (var stream = File.OpenRead(location + fileName))
                                        {
                                            myHash = hash.ComputeHash(stream);
                                            stream.Close();
                                        }
                                        StringBuilder result = new StringBuilder(myHash.Length * 2);

                                        for (int i = 0; i < myHash.Length; i++)
                                        {
                                            result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                                        }

                                        if (result.ToString() == hash)
                                        {
                                            Log(LogLevel.Info, "File verification OK.");
                                            downloading = false;
                                            DownloadForm.downloadsAmount -= 1;
                                            Log(LogLevel.Info, "Finished downloading file.");

                                            if (Settings.Default.notifyDoneHashOk)
                                            {
                                                new ToastContentBuilder()
                                                 .AddText($"The download of {fileName} is complete and has been successfully verified against the checksum provided.")
                                                 .Show();
                                            }

                                            if (Settings.Default.soundOnComplete == true)
                                                complete.Play();

                                            if (checkBox3.Checked)
                                            {
                                                // Open downloaded file
                                                ProcessStartInfo startInfo = new ProcessStartInfo();
                                                startInfo.FileName = location + fileName;
                                                startInfo.UseShellExecute = true;
                                                try
                                                {
                                                    Process.Start(startInfo);
                                                }
                                                catch (Exception ex)
                                                {
                                                    DarkMessageBox.Show(ex.Message + $" ({ex.GetType().FullName})" + ex.StackTrace, "Failed to start process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                            }

                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                cancelButton.Text = "Close";
                                                openButton.Enabled = true;
                                                pauseButton.Enabled = false;
                                                progressBar1.Visible = false;
                                                progressBar2.Visible = false;
                                                totalProgressBar.Visible = true;
                                                totalProgressBar.Value = 100;
                                                totalProgressBar.Style = ProgressBarStyle.Blocks;
                                                totalProgressBar.MarqueeAnim = false;
                                                if (checkBox2.Checked == true)
                                                {
                                                    this.Close();
                                                    this.Dispose();
                                                    return;
                                                }
                                                else if (!this.Visible)
                                                {
                                                    this.Close();
                                                    this.Dispose();
                                                    return;
                                                }
                                            }));
                                        }
                                        else
                                        {
                                            Invoke(new MethodInvoker(delegate
                                            {
                                                progressBar1.Visible = false;
                                                progressBar2.Visible = false;
                                                totalProgressBar.Visible = true;
                                                totalProgressBar.Style = ProgressBarStyle.Blocks;
                                                totalProgressBar.MarqueeAnim = false;
                                                totalProgressBar.Value = 100;
                                                totalProgressBar.State = ProgressBarState.Error;
                                            }));
                                            Log(LogLevel.Error, "Failed to verify file. The file will be re-downloaded.");

                                            if (Settings.Default.notifyDoneHashNo)
                                            {
                                                new ToastContentBuilder()
                                                .AddText($"The download of {fileName} is complete but failed to verify against the checksum provided and will be re-downloaded.")
                                                .Show();
                                            }

                                            downloading = false;
                                            cancelled = false;
                                            cancellationToken.Cancel();
                                            DownloadForm.downloadsAmount -= 1;
                                            hashType -= 1;

                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                this.Close();
                                                this.Dispose();

                                                if (downloadAttempts != Settings.Default.autoDownloadAttempts - 1)
                                                {
                                                    DownloadProgress download = new DownloadProgress(url, location, hash, hashType, downloadAttempts + 1);
                                                    download.Show();

                                                    DownloadForm.downloadsList.Add(download);
                                                    DownloadForm.currentDownloads.RefreshList();
                                                }
                                                else
                                                {
                                                    if (Settings.Default.notifyFail)
                                                    {
                                                        new ToastContentBuilder()
                                                        .AddText($"The download of {fileName} failed. The download exceeded the maximum number of attempts.")
                                                        .Show();
                                                    }
                                                }
                                            }));
                                            return;
                                        }
                                    }
                                    // SHA-1
                                    else if (hashType == 2)
                                    {
                                        byte[] myHash;
                                        using (var hash = SHA1.Create())
                                        using (var stream = File.OpenRead(location + fileName))
                                        {
                                            myHash = hash.ComputeHash(stream);
                                            stream.Close();
                                        }
                                        StringBuilder result = new StringBuilder(myHash.Length * 2);

                                        for (int i = 0; i < myHash.Length; i++)
                                        {
                                            result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                                        }

                                        if (result.ToString() == hash)
                                        {
                                            Log(LogLevel.Info, "File verification OK.");
                                            downloading = false;
                                            DownloadForm.downloadsAmount -= 1;

                                            if (Settings.Default.notifyDoneHashOk)
                                            {
                                                new ToastContentBuilder()
                                                   .AddText($"The download of {fileName} is complete and has been successfully verified against the checksum provided.")
                                                   .Show();
                                            }

                                            Log(LogLevel.Info, "Finished downloading file.");
                                            if (Settings.Default.soundOnComplete == true)
                                                complete.Play();

                                            if (checkBox3.Checked)
                                            {
                                                // Open downloaded file
                                                ProcessStartInfo startInfo = new ProcessStartInfo();
                                                startInfo.FileName = location + fileName;
                                                startInfo.UseShellExecute = true;
                                                try
                                                {
                                                    Process.Start(startInfo);
                                                }
                                                catch (Exception ex)
                                                {
                                                    DarkMessageBox.Show(ex.Message + $" ({ex.GetType().FullName})" + ex.StackTrace, "Failed to start process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                            }

                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                cancelButton.Text = "Close";
                                                openButton.Enabled = true;
                                                pauseButton.Enabled = false;
                                                progressBar1.Visible = false;
                                                progressBar2.Visible = false;
                                                totalProgressBar.Visible = true;
                                                totalProgressBar.Value = 100;
                                                totalProgressBar.Style = ProgressBarStyle.Blocks;
                                                totalProgressBar.MarqueeAnim = false;
                                                if (checkBox2.Checked == true)
                                                {
                                                    this.Close();
                                                    this.Dispose();
                                                    return;
                                                }
                                                else if (!this.Visible)
                                                {
                                                    this.Close();
                                                    this.Dispose();
                                                    return;
                                                }
                                            }));
                                        }
                                        else
                                        {
                                            Invoke(new MethodInvoker(delegate
                                            {
                                                progressBar1.Visible = false;
                                                progressBar2.Visible = false;
                                                totalProgressBar.Visible = true;
                                                totalProgressBar.Style = ProgressBarStyle.Blocks;
                                                totalProgressBar.MarqueeAnim = false;
                                                totalProgressBar.Value = 100;
                                                totalProgressBar.State = ProgressBarState.Error;
                                            }));
                                            Log(LogLevel.Error, "Failed to verify file. The file will be re-downloaded.");

                                            if (Settings.Default.notifyDoneHashNo)
                                            {
                                                new ToastContentBuilder()
                                                   .AddText($"The download of {fileName} is complete but failed to verify against the checksum provided and will be re-downloaded.")
                                                   .Show();
                                            }

                                            downloading = false;
                                            cancelled = false;
                                            cancellationToken.Cancel();
                                            DownloadForm.downloadsAmount -= 1;
                                            hashType -= 1;

                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                this.Close();
                                                this.Dispose();

                                                if (downloadAttempts != Settings.Default.autoDownloadAttempts - 1)
                                                {
                                                    DownloadProgress download = new DownloadProgress(url, location, hash, hashType, downloadAttempts + 1);
                                                    download.Show();

                                                    DownloadForm.downloadsList.Add(download);
                                                    DownloadForm.currentDownloads.RefreshList();
                                                }
                                                else
                                                {
                                                    if (Settings.Default.notifyFail)
                                                    {
                                                        new ToastContentBuilder()
                                                        .AddText($"The download of {fileName} failed. The download exceeded the maximum number of attempts.")
                                                        .Show();
                                                    }
                                                }
                                            }));
                                            return;
                                        }
                                    }
                                    // SHA-256
                                    else if (hashType == 3)
                                    {
                                        byte[] myHash;
                                        using (var hash = SHA256.Create())
                                        using (var stream = File.OpenRead(location + fileName))
                                        {
                                            myHash = hash.ComputeHash(stream);
                                            stream.Close();
                                        }
                                        StringBuilder result = new StringBuilder(myHash.Length * 2);

                                        for (int i = 0; i < myHash.Length; i++)
                                        {
                                            result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                                        }

                                        if (result.ToString() == hash)
                                        {
                                            Log(LogLevel.Info, "File verification OK.");
                                            downloading = false;
                                            DownloadForm.downloadsAmount -= 1;

                                            if (Settings.Default.notifyDoneHashOk)
                                            {
                                                new ToastContentBuilder()
                                                .AddText($"The download of {fileName} is complete and has been successfully verified against the checksum provided.")
                                                .Show();
                                            }

                                            Log(LogLevel.Info, "Finished downloading file.");
                                            if (Settings.Default.soundOnComplete == true)
                                                complete.Play();

                                            if (checkBox3.Checked)
                                            {
                                                // Open downloaded file
                                                ProcessStartInfo startInfo = new ProcessStartInfo();
                                                startInfo.FileName = location + fileName;
                                                startInfo.UseShellExecute = true;
                                                try
                                                {
                                                    Process.Start(startInfo);
                                                }
                                                catch (Exception ex)
                                                {
                                                    DarkMessageBox.Show(ex.Message + $" ({ex.GetType().FullName})" + ex.StackTrace, "Failed to start process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                            }

                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                cancelButton.Text = "Close";
                                                openButton.Enabled = true;
                                                pauseButton.Enabled = false;
                                                progressBar1.Visible = false;
                                                progressBar2.Visible = false;
                                                totalProgressBar.Visible = true;
                                                totalProgressBar.Value = 100;
                                                totalProgressBar.Style = ProgressBarStyle.Blocks;
                                                totalProgressBar.MarqueeAnim = false;
                                                if (checkBox2.Checked == true)
                                                {
                                                    this.Close();
                                                    this.Dispose();
                                                    return;
                                                }
                                                else if (!this.Visible)
                                                {
                                                    this.Close();
                                                    this.Dispose();
                                                    return;
                                                }
                                            }));
                                        }
                                        else
                                        {
                                            Invoke(new MethodInvoker(delegate
                                            {
                                                progressBar1.Visible = false;
                                                progressBar2.Visible = false;
                                                totalProgressBar.Visible = true;
                                                totalProgressBar.Style = ProgressBarStyle.Blocks;
                                                totalProgressBar.MarqueeAnim = false;
                                                totalProgressBar.Value = 100;
                                                totalProgressBar.State = ProgressBarState.Error;
                                            }));
                                            Log(LogLevel.Error, "Failed to verify file. The file will be re-downloaded.");

                                            if (Settings.Default.notifyDoneHashNo)
                                            {
                                                new ToastContentBuilder()
                                                   .AddText($"The download of {fileName} is complete but failed to verify against the checksum provided and will be re-downloaded.")
                                                   .Show();
                                            }

                                            downloading = false;
                                            cancelled = false;
                                            cancellationToken.Cancel();
                                            DownloadForm.downloadsAmount -= 1;
                                            hashType -= 1;

                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                this.Close();
                                                this.Dispose();

                                                if (downloadAttempts != Settings.Default.autoDownloadAttempts - 1)
                                                {
                                                    DownloadProgress download = new DownloadProgress(url, location, hash, hashType, downloadAttempts + 1);
                                                    download.Show();

                                                    DownloadForm.downloadsList.Add(download);
                                                    DownloadForm.currentDownloads.RefreshList();
                                                }
                                                else
                                                {
                                                    if (Settings.Default.notifyFail)
                                                    {
                                                        new ToastContentBuilder()
                                                        .AddText($"The download of {fileName} failed. The download exceeded the maximum number of attempts.")
                                                        .Show();
                                                    }
                                                }
                                            }));
                                            return;
                                        }
                                    }
                                    // SHA-384
                                    else if (hashType == 4)
                                    {
                                        byte[] myHash;
                                        using (var hash = SHA384.Create())
                                        using (var stream = File.OpenRead(location + fileName))
                                        {
                                            myHash = hash.ComputeHash(stream);
                                            stream.Close();
                                        }
                                        StringBuilder result = new StringBuilder(myHash.Length * 2);

                                        for (int i = 0; i < myHash.Length; i++)
                                        {
                                            result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                                        }

                                        if (result.ToString() == hash)
                                        {
                                            Log(LogLevel.Info, "File verification OK.");
                                            downloading = false;
                                            DownloadForm.downloadsAmount -= 1;

                                            if (Settings.Default.notifyDoneHashOk)
                                            {
                                                new ToastContentBuilder()
                                                .AddText($"The download of {fileName} is complete and has been successfully verified against the checksum provided.")
                                                .Show();
                                            }

                                            Log(LogLevel.Info, "Finished downloading file.");
                                            if (Settings.Default.soundOnComplete == true)
                                                complete.Play();

                                            if (checkBox3.Checked)
                                            {
                                                // Open downloaded file
                                                ProcessStartInfo startInfo = new ProcessStartInfo();
                                                startInfo.FileName = location + fileName;
                                                startInfo.UseShellExecute = true;
                                                try
                                                {
                                                    Process.Start(startInfo);
                                                }
                                                catch (Exception ex)
                                                {
                                                    DarkMessageBox.Show(ex.Message + $" ({ex.GetType().FullName})" + ex.StackTrace, "Failed to start process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                            }

                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                cancelButton.Text = "Close";
                                                openButton.Enabled = true;
                                                pauseButton.Enabled = false;
                                                progressBar1.Visible = false;
                                                progressBar2.Visible = false;
                                                totalProgressBar.Visible = true;
                                                totalProgressBar.Value = 100;
                                                totalProgressBar.Style = ProgressBarStyle.Blocks;
                                                totalProgressBar.MarqueeAnim = false;
                                                if (checkBox2.Checked == true)
                                                {
                                                    this.Close();
                                                    this.Dispose();
                                                    return;
                                                }
                                                else if (!this.Visible)
                                                {
                                                    this.Close();
                                                    this.Dispose();
                                                    return;
                                                }
                                            }));
                                        }
                                        else
                                        {
                                            Invoke(new MethodInvoker(delegate
                                            {
                                                progressBar1.Visible = false;
                                                progressBar2.Visible = false;
                                                totalProgressBar.Visible = true;
                                                totalProgressBar.Style = ProgressBarStyle.Blocks;
                                                totalProgressBar.MarqueeAnim = false;
                                                totalProgressBar.Value = 100;
                                                totalProgressBar.State = ProgressBarState.Error;
                                            }));
                                            Log(LogLevel.Error, "Failed to verify file. The file will be re-downloaded.");

                                            if (Settings.Default.notifyDoneHashNo)
                                            {
                                                new ToastContentBuilder()
                                                .AddText($"The download of {fileName} is complete but failed to verify against the checksum provided and will be re-downloaded.")
                                                .Show();
                                            }

                                            downloading = false;
                                            cancelled = false;
                                            cancellationToken.Cancel();
                                            DownloadForm.downloadsAmount -= 1;
                                            hashType -= 1;

                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                this.Close();
                                                this.Dispose();

                                                if (downloadAttempts != Settings.Default.autoDownloadAttempts - 1)
                                                {
                                                    DownloadProgress download = new DownloadProgress(url, location, hash, hashType, downloadAttempts + 1);
                                                    download.Show();

                                                    DownloadForm.downloadsList.Add(download);
                                                    DownloadForm.currentDownloads.RefreshList();
                                                }
                                                else
                                                {
                                                    if (Settings.Default.notifyFail)
                                                    {
                                                        new ToastContentBuilder()
                                                        .AddText($"The download of {fileName} failed. The download exceeded the maximum number of attempts.")
                                                        .Show();
                                                    }
                                                }
                                            }));
                                            return;
                                        }
                                    }
                                    // SHA-512
                                    else if (hashType == 5)
                                    {
                                        byte[] myHash;
                                        using (var hash = SHA512.Create())
                                        using (var stream = File.OpenRead(location + fileName))
                                        {
                                            myHash = hash.ComputeHash(stream);
                                            stream.Close();
                                        }
                                        StringBuilder result = new StringBuilder(myHash.Length * 2);

                                        for (int i = 0; i < myHash.Length; i++)
                                        {
                                            result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                                        }

                                        if (result.ToString() == hash)
                                        {
                                            Log(LogLevel.Info, "File verification OK.");
                                            downloading = false;
                                            DownloadForm.downloadsAmount -= 1;

                                            if (Settings.Default.notifyDoneHashOk)
                                            {
                                                new ToastContentBuilder()
                                                .AddText($"The download of {fileName} is complete and has been successfully verified against the checksum provided.")
                                                .Show();
                                            }

                                            Log(LogLevel.Info, "Finished downloading file.");
                                            if (Settings.Default.soundOnComplete == true)
                                                complete.Play();

                                            if (checkBox3.Checked)
                                            {
                                                // Open downloaded file
                                                ProcessStartInfo startInfo = new ProcessStartInfo();
                                                startInfo.FileName = location + fileName;
                                                startInfo.UseShellExecute = true;
                                                try
                                                {
                                                    Process.Start(startInfo);
                                                }
                                                catch (Exception ex)
                                                {
                                                    DarkMessageBox.Show(ex.Message + $" ({ex.GetType().FullName})" + ex.StackTrace, "Failed to start process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                                }
                                            }

                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                cancelButton.Text = "Close";
                                                openButton.Enabled = true;
                                                pauseButton.Enabled = false;
                                                progressBar1.Visible = false;
                                                progressBar2.Visible = false;
                                                totalProgressBar.Visible = true;
                                                totalProgressBar.Value = 100;
                                                totalProgressBar.Style = ProgressBarStyle.Blocks;
                                                totalProgressBar.MarqueeAnim = false;
                                                if (checkBox2.Checked == true)
                                                {
                                                    this.Close();
                                                    this.Dispose();
                                                    return;
                                                }
                                                else if (!this.Visible)
                                                {
                                                    this.Close();
                                                    this.Dispose();
                                                    return;
                                                }
                                            }));
                                        }
                                        else
                                        {
                                            Invoke(new MethodInvoker(delegate
                                            {
                                                progressBar1.Visible = false;
                                                progressBar2.Visible = false;
                                                totalProgressBar.Visible = true;
                                                totalProgressBar.Style = ProgressBarStyle.Blocks;
                                                totalProgressBar.MarqueeAnim = false;
                                                totalProgressBar.Value = 100;
                                                totalProgressBar.State = ProgressBarState.Error;
                                            }));
                                            Log(LogLevel.Error, "Failed to verify file. The file will be re-downloaded.");

                                            if (Settings.Default.notifyDoneHashNo)
                                            {
                                                new ToastContentBuilder()
                                                .AddText($"The download of {fileName} is complete but failed to verify against the checksum provided and will be re-downloaded.")
                                                .Show();
                                            }

                                            downloading = false;
                                            cancelled = false;
                                            cancellationToken.Cancel();
                                            DownloadForm.downloadsAmount -= 1;
                                            hashType -= 1;

                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                this.Close();
                                                this.Dispose();

                                                if (downloadAttempts != Settings.Default.autoDownloadAttempts - 1)
                                                {
                                                    DownloadProgress download = new DownloadProgress(url, location, hash, hashType, downloadAttempts + 1);
                                                    download.Show();

                                                    DownloadForm.downloadsList.Add(download);
                                                    DownloadForm.currentDownloads.RefreshList();
                                                }
                                                else
                                                {
                                                    if (Settings.Default.notifyFail)
                                                    {
                                                        new ToastContentBuilder()
                                                        .AddText($"The download of {fileName} failed. The download exceeded the maximum number of attempts.")
                                                        .Show();
                                                    }
                                                }
                                            }));
                                            return;
                                        }
                                    }
                                    // Invalid
                                    else
                                    {
                                        Invoke(new MethodInvoker(delegate
                                        {
                                            progressBar1.Visible = false;
                                            progressBar2.Visible = false;
                                            totalProgressBar.Visible = true;
                                            totalProgressBar.Style = ProgressBarStyle.Blocks;
                                            totalProgressBar.MarqueeAnim = false;
                                            totalProgressBar.Value = 100;
                                            totalProgressBar.State = ProgressBarState.Error;
                                        }));
                                        Log(LogLevel.Error, "Invalid hash type '" + hashType + "'. The file could not be verified.");

                                        if (Settings.Default.notifyDone)
                                        {
                                            new ToastContentBuilder()
                                                .AddText($"The download of {fileName} is complete but could not be verified against the checksum provided due to an internal error.")
                                                .Show();
                                        }

                                        DarkMessageBox.Show("Invalid hash type '" + hashType + "'. The file could not be verified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                                        Invoke(new MethodInvoker(delegate ()
                                        {
                                            downloading = false;
                                            DownloadForm.downloadsAmount -= 1;

                                            cancellationToken.Cancel();

                                            this.Close();
                                            this.Dispose();
                                        }));
                                        return;
                                    }
                                });
                                thread.Start();
                            }
                            else
                            {
                                totalProgressBar.Value = 100;
                                totalProgressBar.Style = ProgressBarStyle.Blocks;
                                totalProgressBar.MarqueeAnim = false;
                                downloading = false;
                                DownloadForm.downloadsAmount -= 1;
                                Log(LogLevel.Info, "Finished downloading file.");

                                if (Settings.Default.notifyDone)
                                {
                                    new ToastContentBuilder()
                                                .AddText($"The download of {fileName} is complete.")
                                                .Show();
                                }

                                if (Settings.Default.soundOnComplete == true)
                                    complete.Play();

                                if (checkBox3.Checked)
                                {
                                    // Open downloaded file
                                    ProcessStartInfo startInfo = new ProcessStartInfo();
                                    startInfo.FileName = location + fileName;
                                    startInfo.UseShellExecute = true;
                                    try
                                    {
                                        Process.Start(startInfo);
                                    }
                                    catch (Exception ex)
                                    {
                                        DarkMessageBox.Show(ex.Message + $" ({ex.GetType().FullName})" + ex.StackTrace, "Failed to start process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }

                                Invoke(new MethodInvoker(delegate ()
                                {
                                    checkBox2.Enabled = false;
                                    cancelButton.Text = "Close";
                                    openButton.Enabled = true;
                                    pauseButton.Enabled = false;
                                    progressBar1.Visible = false;
                                    progressBar2.Visible = false;
                                    totalProgressBar.Visible = true;
                                    totalProgressBar.Value = 100;
                                    totalProgressBar.Style = ProgressBarStyle.Blocks;
                                    totalProgressBar.MarqueeAnim = false;
                                    if (checkBox2.Checked == true)
                                    {
                                        this.Close();
                                        this.Dispose();
                                        return;
                                    }
                                    else if (!this.Visible)
                                    {
                                        this.Close();
                                        this.Dispose();
                                        return;
                                    }
                                }));
                            }
                        }
                        else
                        {
                            Invoke(new MethodInvoker(delegate
                            {
                                progressBar1.Visible = false;
                                progressBar2.Visible = false;
                                totalProgressBar.Visible = true;
                                totalProgressBar.Style = ProgressBarStyle.Blocks;
                                totalProgressBar.MarqueeAnim = false;
                                totalProgressBar.Value = 100;
                                totalProgressBar.State = ProgressBarState.Error;
                            }));

                            Log(LogLevel.Error, "Download failed.");

                            if (Settings.Default.notifyFail)
                            {
                                new ToastContentBuilder()
                                    .AddText($"The download of {fileName} has failed.")
                                    .Show();
                            }

                            Invoke(new MethodInvoker(delegate ()
                            {
                                downloading = false;
                                DownloadForm.downloadsAmount -= 1;

                                cancellationToken.Cancel();

                                this.Close();
                                this.Dispose();
                            }));
                        }
                    }));
                }
                catch (Exception ex)
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        progressBar1.Visible = false;
                        progressBar2.Visible = false;
                        totalProgressBar.Visible = true;
                        totalProgressBar.Style = ProgressBarStyle.Blocks;
                        totalProgressBar.MarqueeAnim = false;
                        totalProgressBar.Value = 100;
                        totalProgressBar.State = ProgressBarState.Error;
                        downloading = false;
                        DownloadForm.downloadsAmount -= 1;

                        this.Close();
                        this.Dispose();
                        return;
                    }));

                    Log(LogLevel.Error, ex.Message + Environment.NewLine + ex.StackTrace);
                    DarkMessageBox.Show(ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                }
            }
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

        public void cancelButton_Click(object sender, EventArgs e)
        {
            // Close
            if (!downloading)
            {
                this.Close();
                this.Dispose();
            }
            else
            {
                if (!forceCancel)
                {
                    DialogResult result = DarkMessageBox.Show("Are you sure you want to cancel the download?", "Download Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, false);
                    if (result == DialogResult.Yes)
                    {
                        downloading = false;
                        cancelled = true;
                        DownloadForm.downloadsAmount -= 1;
                        DownloadForm.downloadsList.Remove(this);

                        cancellationToken.Cancel();
                        Logging.Log(LogLevel.Warning, "Download of " + fileName + " has been canceled.");

                        try
                        {
                            File.Delete(fileName);
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(LogLevel.Error, ex.Message + Environment.NewLine + ex.StackTrace);
                        }

                        if (Settings.Default.notifyFail)
                        {
                            new ToastContentBuilder()
                            .AddText($"The download of {fileName} has been canceled.")
                            .Show();
                        }

                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            this.Close();
                            this.Dispose();
                        }));

                        this.Hide();
                    }
                }
                else
                {
                    downloading = false;
                    cancelled = true;
                    DownloadForm.downloadsAmount -= 1;
                    DownloadForm.downloadsList.Remove(this);

                    cancellationToken.Cancel();

                    try
                    {
                        if (File.Exists(fileName + ".download0"))
                        {
                            File.Delete(fileName);
                        }

                        if (File.Exists(fileName + ".download0"))
                        {
                            File.Delete(fileName + ".download0");
                        }

                        if (File.Exists(fileName + ".download1"))
                        {
                            File.Delete(fileName + ".download1");
                        }
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(LogLevel.Error, ex.Message + Environment.NewLine + ex.StackTrace);
                    }

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        this.Close();
                        this.Dispose();
                    }));

                    this.Hide();
                }
            }
        }

        private void DownloadProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (downloading == true)
            {
                if (!forceCancel)
                {
                    DialogResult result = DarkMessageBox.Show("Are you sure you want to cancel the download?", "Download Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, false);

                    if (result == DialogResult.Yes)
                    {
                        DownloadForm.downloadsList.Remove(this);
                        DownloadForm.downloadsAmount -= 1;
                        downloading = false;
                        cancelled = true;

                        cancellationToken.Cancel();
                        Logging.Log(LogLevel.Warning, "Download of " + fileName + " has been canceled.");

                        try
                        {
                            File.Delete(fileName);
                        }
                        catch (Exception ex)
                        {
                            Logging.Log(LogLevel.Error, ex.Message + Environment.NewLine + ex.StackTrace);
                        }

                        if (Settings.Default.notifyFail)
                        {
                            new ToastContentBuilder()
                            .AddText($"The download of {fileName} has been canceled.")
                            .Show();
                        }

                        e.Cancel = false;
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    DownloadForm.downloadsList.Remove(this);
                    e.Cancel = false;
                }
            }
            else
            {
                DownloadForm.downloadsList.Remove(this);
                downloading = false;
                cancelled = true;

                try
                {
                    File.Delete(fileName);
                }
                catch (Exception ex)
                {
                    Logging.Log(LogLevel.Error, ex.Message + Environment.NewLine + ex.StackTrace);
                }

                e.Cancel = false;
            }
        }

        async private void pauseButton_Click(object sender, EventArgs e)
        {
            // Check if the file is actually downloading
            if (!downloading || cancelled)
            {
                return;
            }

            // Toggle pause/resume
            isPaused = !isPaused;

            if (isPaused)
            {
                DialogResult result = DarkMessageBox.Show("Are you sure you want to pause this download?\nThis will make the download start again when the download has been resumed.", "Pause Download?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, false);

                if (result == DialogResult.Yes)
                {
                    Log(LogLevel.Info, "Download of " + url + " has been paused.");

                    pauseButton.Text = "Resume";

                    cancellationToken.Cancel();
                    progressBar1.State = ProgressBarState.Warning;
                    progressBar2.State = ProgressBarState.Warning;
                    totalProgressBar.State = ProgressBarState.Warning;
                }
                else
                {
                    isPaused = false;
                    return;
                }
            }
            else
            {
                pauseButton.Text = "Pause";

                Uri uri = new Uri(url);

                cancellationToken = new CancellationTokenSource();

                progressBar1.State = ProgressBarState.Normal;
                progressBar2.State = ProgressBarState.Normal;
                totalProgressBar.State = ProgressBarState.Normal;

                try
                {
                    stream = new FileStream(location + fileName, FileMode.Create);

                    await DownloadFileAsync(uri, cancellationToken.Token, Client_DownloadProgressChanged);

                    Client_DownloadFileCompleted();

                    stream.Close();
                }
                catch (System.Threading.Tasks.TaskCanceledException ex)
                {
                    Log(LogLevel.Info, "Download of " + url + "was cancelled.");
                    stream.Close();
                }
                catch (Exception ex)
                {
                    if (stream != null)
                    {
                        stream.Close();
                    }

                    Invoke(new MethodInvoker(delegate
                    {
                        progressBar1.Visible = false;
                        progressBar2.Visible = false;
                        totalProgressBar.Visible = true;
                        totalProgressBar.Style = ProgressBarStyle.Blocks;
                        totalProgressBar.MarqueeAnim = false;
                        totalProgressBar.Value = 100;
                        totalProgressBar.State = ProgressBarState.Error;
                    }));

                    Log(LogLevel.Error, ex.Message);
                    DarkMessageBox.Show(ex.Message, "Download Manager - Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                    downloading = false;
                    Invoke(new MethodInvoker(delegate ()
                    {
                        DownloadForm.downloadsAmount -= 1;

                        cancellationToken.Cancel();

                        this.Close();
                        this.Dispose();
                    }));
                    return;
                }

            }
        }

        private void openFolderButton_Click(object sender, EventArgs e)
        {
            // Open download folder
            Process.Start("explorer.exe", location);
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            // Open downloaded file
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = location + fileName;
            startInfo.UseShellExecute = true;
            try
            {
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                DarkMessageBox.Show(ex.Message + $" ({ex.GetType().FullName})" + ex.StackTrace, "Failed to start process", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void updateDisplayTimer_Tick(object sender, EventArgs e)
        {
            // Update display
            if (progressUpdater != null)
            {
                progressUpdater.UpdateUI();
            }
            else
            {
                bytesLabel.Text = $"({receivedBytes} B / ? B)";

                if (megabytesPerSecond > 1)
                {
                    speedLabel.Text = $"{megabytesPerSecond.ToString("0.00")} MB/s";
                }
                else if (kilobytesPerSecond > 1)
                {
                    speedLabel.Text = $"{kilobytesPerSecond.ToString("0.00")} KB/s";
                }
                else
                {
                    speedLabel.Text = $"{bytesPerSecond.ToString("0.00")} B/s";
                }
            }
        }
    }
}