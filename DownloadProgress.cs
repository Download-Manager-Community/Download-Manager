using DownloadManager.NativeMethods;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using System.Media;
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
        int hashType = 0;
        bool isUrlInvalid = false;
        public bool downloading = true;
        public bool isPaused = false;
        bool doFileVerify = false;
        FileStream? stream = null;
        SoundPlayer complete = new SoundPlayer(@"C:\WINDOWS\Media\tada.wav");

        public string fileSize = "0";
        public long totalSize = 0;
        public double percentageDone = 0;

        double receivedBytes = 0;
        double totalBytes = 0;

        public bool cancelled = false;
        internal bool restartNoProgress = false;
        internal int downloadAttempts = 0;
        CancellationTokenSource cancellationToken = new CancellationTokenSource();

        public enum DownloadType
        {
            Normal = 0,
            YoutubeVideo = 1,
            YoutubePlaylist = 2
        };

        public DownloadProgress(string urlArg, string locationArg, string hashArg, int hashTypeArg, int downloadAttempts = 0)
        {
            InitializeComponent();
            _instance = this;

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
                //this.Size = new System.Drawing.Size(651, 212);
            }
            else
            {
                doFileVerify = false;
                //this.Size = new System.Drawing.Size(651, 183);
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

        private void progress_Load(object sender, EventArgs e)
        {
            Log("Preparing to start downloading...", Color.White);
            checkBox2.Checked = Settings.Default.closeOnComplete;
            checkBox1.Checked = Settings.Default.keepOnTop;
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnim = true;
            thread = new Thread(async () =>
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
                        progressBar1.Value = 100;
                        progressBar1.State = ProgressBarState.Error;
                        progressBar1.ShowText = false;
                        progressBar1.Style = ProgressBarStyle.Blocks;
                    }));

                    downloading = false;
                    DownloadForm.downloadsAmount -= 1;
                    DownloadForm.downloadsList.Remove(this);

                    Logging.Log($"Failed to parse URI.\n{ex.Message} ({ex.GetType().FullName})\n{ex.StackTrace}", Color.Red);
                    DarkMessageBox msg = new DarkMessageBox(ex.Message + $" ({ex.GetType().FullName})\n" + ex.StackTrace, "Failed to parse URI", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    msg.ShowDialog();
                    msg.Dispose();
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
                    DarkMessageBox msg = new DarkMessageBox($"The file you are attempting to download ({fileName}) does not have a file extension.\nThis may be because the site uses redirection to download files.\nPress Cancel to cancel the download.\nPress Retry to rename the file.\nPress Continue to continue downloading the file anyway. ", "Download Warning", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Warning, false);
                    DialogResult result = msg.ShowDialog();

                    if (result == DialogResult.Cancel)
                    {
                        downloading = false;
                        cancelled = true;
                        DownloadForm.downloadsAmount -= 1;

                        cancellationToken.Cancel();
                        Logging.Log("Download of " + fileName + " has been canceled.", Color.Orange);

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

                            DarkMessageBox msg2 = new DarkMessageBox("Failed to obtain results from save file dialog.\nfileDialog was null!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            msg2.ShowDialog();

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

                            DarkMessageBox msg2 = new DarkMessageBox("Failed to obtain results from save file dialog.\nDialogResult was null!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            msg2.ShowDialog();

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
                            Logging.Log("Download of " + fileName + " has been canceled.", Color.Orange);

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
                    progressBar1.Style = ProgressBarStyle.Blocks;
                    progressBar1.MarqueeAnim = false;
                }));

                Log("Downloading file " + uri + " to " + location + fileName, Color.White);
                try
                {
                    stream = new FileStream(location + fileName, FileMode.Create);

                    await DownloadFileAsync(uri, stream, cancellationToken.Token, Client_DownloadProgressChanged);

                    if (restartNoProgress)
                    {
                        cancellationToken.TryReset();
                        await DownloadFileAsync(uri, stream, cancellationToken.Token);
                    }

                    stream.Close();

                    Client_DownloadFileCompleted();
                }
                catch (System.Threading.Tasks.TaskCanceledException)
                {
                    Log("Download of " + url + "was canceled.", Color.White);
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
                        progressBar1.Style = ProgressBarStyle.Blocks;
                        progressBar1.MarqueeAnim = false;
                        progressBar1.Value = 100;
                        progressBar1.State = ProgressBarState.Error;
                    }));

                    if (Settings.Default.notifyFail)
                    {
                        new ToastContentBuilder()
                           .AddText($"The download of {fileName} has failed.")
                           .Show();
                    }

                    cancellationToken.Cancel();

                    Log(ex.Message, Color.Red);
                    DarkMessageBox msg = new DarkMessageBox(ex.Message, "Download Manager - Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                    msg.ShowDialog();
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
            });
            thread.Start();
        }

        public static async Task DownloadFileAsync(Uri uri, Stream toStream, CancellationToken cancellationToken = default, Action<long, long>? progressCallback = null)
        {
            if (uri == null)
                throw new ArgumentNullException(nameof(uri));
            if (toStream == null)
                throw new ArgumentNullException(nameof(toStream));

            if (uri.IsFile)
            {
                await using Stream file = File.OpenRead(uri.LocalPath);

                if (progressCallback != null)
                {
                    long length = file.Length;
                    byte[] buffer = new byte[4096];
                    int read;
                    int totalRead = 0;
                    while ((read = await file.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        await toStream.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
                        totalRead += read;
                        progressCallback(totalRead, length);
                    }
                    Debug.Assert(totalRead == length || length == -1);
                }
                else
                {
                    await file.CopyToAsync(toStream, cancellationToken).ConfigureAwait(false);
                }
            }
            else
            {
                using HttpClient client = new HttpClient();
                using HttpResponseMessage response = await client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

                if (progressCallback != null)
                {
                    long length = 0;
                    if (_instance.totalSize == 0)
                    {
                        length = response.Content.Headers.ContentLength ?? -1;
                        _instance.totalSize = length;

                        if (length == -1)
                        {
                            _instance.restartNoProgress = true;
                            _instance.cancellationToken.Cancel();
                            Logging.Log("Server failed to provide content length. Progress report will not be available.", Color.Orange);
                            new ToastContentBuilder()
                                                .AddText($"The remote server failed to provide size of {_instance.fileName}. Progress reports will not be available.")
                                                .Show();
                            _instance.progressBar1.Style = ProgressBarStyle.Marquee;
                            _instance.progressBar1.MarqueeAnim = true;
                            return;
                        }
                    }
                    else
                    {
                        length = _instance.totalSize;
                    }
                    await using Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    byte[] buffer = new byte[4096];
                    int read;
                    long totalRead = 0;
                    while ((read = await stream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                    {
                        await toStream.WriteAsync(buffer, 0, read, cancellationToken).ConfigureAwait(false);
                        totalRead += read;
                        progressCallback(totalRead, length);
                    }
                    Debug.Assert(totalRead == length || length == -1);
                }
                else
                {
                    await response.Content.CopyToAsync(toStream).ConfigureAwait(false);
                }
            }
        }

        private void Client_DownloadProgressChanged(long totalRead, long size)
        {
            //Update progress bar & label
            if (this.IsHandleCreated)
            {
                try
                {
                    totalSize = size;
                    fileSize = size.ToString();

                    progressBar1.Minimum = 0;
                    receivedBytes = totalRead;
                    totalBytes = size;
                    percentageDone = receivedBytes / totalBytes * 100;

                    Invoke(new MethodInvoker(delegate ()
                    {
                        try
                        {
                            progressBar1.Value = int.Parse(Math.Truncate(percentageDone).ToString());
                        }
                        catch (Exception ex)
                        {
                            if (isUrlInvalid == false)
                            {
                                isUrlInvalid = true;
                                DownloadForm.downloadsAmount -= 1;
                                Log(ex.Message, Color.Red);
                                Invoke(new MethodInvoker(delegate
                                {
                                    progressBar1.State = ProgressBarState.Error;
                                }));
                                DarkMessageBox msg = new DarkMessageBox(ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                                msg.ShowDialog();
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
                                progressBar1.Style = ProgressBarStyle.Blocks;
                                progressBar1.MarqueeAnim = false;
                                progressBar1.Value = 100;
                                progressBar1.State = ProgressBarState.Error;
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
                    Log(ex.Message + Environment.NewLine + ex.StackTrace, Color.Red);
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
                        progressBar1.Style = ProgressBarStyle.Marquee;
                        progressBar1.MarqueeAnim = true;
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
                                            Log("File verification OK.", Color.White);
                                            downloading = false;
                                            DownloadForm.downloadsAmount -= 1;
                                            Log("Finished downloading file.", Color.White);

                                            if (Settings.Default.notifyDoneHashOk)
                                            {
                                                new ToastContentBuilder()
                                                 .AddText($"The download of {fileName} is complete and has been successfully verified against the checksum provided.")
                                                 .Show();
                                            }

                                            if (Settings.Default.soundOnComplete == true)
                                                complete.Play();
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                cancelButton.Text = "Close";
                                                openButton.Enabled = true;
                                                pauseButton.Enabled = false;
                                                progressBar1.Value = 100;
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                progressBar1.MarqueeAnim = false;
                                                if (checkBox2.Checked == true)
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
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                progressBar1.MarqueeAnim = false;
                                                progressBar1.Value = 100;
                                                progressBar1.State = ProgressBarState.Error;
                                            }));
                                            Log("Failed to verify file. The file will be re-downloaded.", Color.Red);

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
                                            Log("File verification OK.", Color.White);
                                            downloading = false;
                                            DownloadForm.downloadsAmount -= 1;

                                            if (Settings.Default.notifyDoneHashOk)
                                            {
                                                new ToastContentBuilder()
                                                   .AddText($"The download of {fileName} is complete and has been successfully verified against the checksum provided.")
                                                   .Show();
                                            }

                                            Log("Finished downloading file.", Color.White);
                                            if (Settings.Default.soundOnComplete == true)
                                                complete.Play();
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                cancelButton.Text = "Close";
                                                openButton.Enabled = true;
                                                pauseButton.Enabled = false;
                                                progressBar1.Value = 100;
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                progressBar1.MarqueeAnim = false;
                                                if (checkBox2.Checked == true)
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
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                progressBar1.MarqueeAnim = false;
                                                progressBar1.Value = 100;
                                                progressBar1.State = ProgressBarState.Error;
                                            }));
                                            Log("Failed to verify file. The file will be re-downloaded.", Color.Red);

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
                                            Log("File verification OK.", Color.White);
                                            downloading = false;
                                            DownloadForm.downloadsAmount -= 1;

                                            if (Settings.Default.notifyDoneHashOk)
                                            {
                                                new ToastContentBuilder()
                                                .AddText($"The download of {fileName} is complete and has been successfully verified against the checksum provided.")
                                                .Show();
                                            }

                                            Log("Finished downloading file.", Color.White);
                                            if (Settings.Default.soundOnComplete == true)
                                                complete.Play();
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                cancelButton.Text = "Close";
                                                openButton.Enabled = true;
                                                pauseButton.Enabled = false;
                                                progressBar1.Value = 100;
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                progressBar1.MarqueeAnim = false;
                                                if (checkBox2.Checked == true)
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
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                progressBar1.MarqueeAnim = false;
                                                progressBar1.Value = 100;
                                                progressBar1.State = ProgressBarState.Error;
                                            }));
                                            Log("Failed to verify file. The file will be re-downloaded.", Color.Red);

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
                                            Log("File verification OK.", Color.White);
                                            downloading = false;
                                            DownloadForm.downloadsAmount -= 1;

                                            if (Settings.Default.notifyDoneHashOk)
                                            {
                                                new ToastContentBuilder()
                                                .AddText($"The download of {fileName} is complete and has been successfully verified against the checksum provided.")
                                                .Show();
                                            }

                                            Log("Finished downloading file.", Color.White);
                                            if (Settings.Default.soundOnComplete == true)
                                                complete.Play();
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                cancelButton.Text = "Close";
                                                openButton.Enabled = true;
                                                pauseButton.Enabled = false;
                                                progressBar1.Value = 100;
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                progressBar1.MarqueeAnim = false;
                                                if (checkBox2.Checked == true)
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
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                progressBar1.MarqueeAnim = false;
                                                progressBar1.Value = 100;
                                                progressBar1.State = ProgressBarState.Error;
                                            }));
                                            Log("Failed to verify file. The file will be re-downloaded.", Color.Red);

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

                                        //Log("Provided Hash: " + hash + Environment.NewLine + "Generated Hash: " + result.ToString(), Color.White);

                                        if (result.ToString() == hash)
                                        {
                                            Log("File verification OK.", Color.White);
                                            downloading = false;
                                            DownloadForm.downloadsAmount -= 1;

                                            if (Settings.Default.notifyDoneHashOk)
                                            {
                                                new ToastContentBuilder()
                                                .AddText($"The download of {fileName} is complete and has been successfully verified against the checksum provided.")
                                                .Show();
                                            }

                                            Log("Finished downloading file.", Color.White);
                                            if (Settings.Default.soundOnComplete == true)
                                                complete.Play();
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                cancelButton.Text = "Close";
                                                openButton.Enabled = true;
                                                pauseButton.Enabled = false;
                                                progressBar1.Value = 100;
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                progressBar1.MarqueeAnim = false;
                                                if (checkBox2.Checked == true)
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
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                progressBar1.MarqueeAnim = false;
                                                progressBar1.Value = 100;
                                                progressBar1.State = ProgressBarState.Error;
                                            }));
                                            Log("Failed to verify file. The file will be re-downloaded.", Color.Red);

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
                                            progressBar1.Style = ProgressBarStyle.Blocks;
                                            progressBar1.MarqueeAnim = false;
                                            progressBar1.Value = 100;
                                            progressBar1.State = ProgressBarState.Error;
                                        }));
                                        Log("Invalid hash type '" + hashType + "'. The file could not be verified.", Color.Red);

                                        if (Settings.Default.notifyDone)
                                        {
                                            new ToastContentBuilder()
                                                .AddText($"The download of {fileName} is complete but could not be verified against the checksum provided due to an internal error.")
                                                .Show();
                                        }

                                        DarkMessageBox msg = new DarkMessageBox("Invalid hash type '" + hashType + "'. The file could not be verified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
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
                                downloading = false;
                                DownloadForm.downloadsAmount -= 1;
                                Log("Finished downloading file.", Color.White);

                                if (Settings.Default.notifyDone)
                                {
                                    new ToastContentBuilder()
                                                .AddText($"The download of {fileName} is complete.")
                                                .Show();
                                }

                                if (Settings.Default.soundOnComplete == true)
                                    complete.Play();
                                Invoke(new MethodInvoker(delegate ()
                                {
                                    checkBox2.Enabled = false;
                                    cancelButton.Text = "Close";
                                    openButton.Enabled = true;
                                    pauseButton.Enabled = false;
                                    progressBar1.Value = 100;
                                    progressBar1.Style = ProgressBarStyle.Blocks;
                                    progressBar1.MarqueeAnim = false;
                                    if (checkBox2.Checked == true)
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
                                progressBar1.Style = ProgressBarStyle.Blocks;
                                progressBar1.MarqueeAnim = false;
                                progressBar1.Value = 100;
                                progressBar1.State = ProgressBarState.Error;
                            }));

                            Log("Download failed.", Color.Red);

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
                                progressBar1.Value = 0;

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
                        progressBar1.Style = ProgressBarStyle.Blocks;
                        progressBar1.MarqueeAnim = false;
                        progressBar1.Value = 100;
                        progressBar1.State = ProgressBarState.Error;
                        downloading = false;
                        DownloadForm.downloadsAmount -= 1;

                        this.Close();
                        this.Dispose();
                        return;
                    }));

                    Log(ex.Message + Environment.NewLine + ex.StackTrace, Color.Red);
                    DarkMessageBox msg = new DarkMessageBox(ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                    msg.ShowDialog();
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

        private void cancelButton_Click(object sender, EventArgs e)
        {
            // Close
            if (!downloading)
            {
                this.Close();
                this.Dispose();
            }
            else
            {
                DarkMessageBox msg = new DarkMessageBox("Are you sure you want to cancel the download?", "Download Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, false);
                DialogResult result = msg.ShowDialog();
                if (result == DialogResult.Yes)
                {
                    downloading = false;
                    cancelled = true;
                    DownloadForm.downloadsAmount -= 1;
                    DownloadForm.downloadsList.Remove(this);

                    cancellationToken.Cancel();
                    Logging.Log("Download of " + fileName + " has been canceled.", Color.Orange);

                    try
                    {
                        File.Delete(fileName);
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex.Message + Environment.NewLine + ex.StackTrace, Color.Red);
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
        }

        private void DownloadProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (downloading == true)
            {
                DarkMessageBox msg = new DarkMessageBox("Are you sure you want to cancel the download?", "Download Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, false);
                DialogResult result = msg.ShowDialog();
                if (result == DialogResult.Yes)
                {
                    DownloadForm.downloadsList.Remove(this);
                    DownloadForm.downloadsAmount -= 1;
                    downloading = false;
                    cancelled = true;

                    cancellationToken.Cancel();
                    Logging.Log("Download of " + fileName + " has been canceled.", Color.Orange);

                    try
                    {
                        File.Delete(fileName);
                    }
                    catch (Exception ex)
                    {
                        Logging.Log(ex.Message + Environment.NewLine + ex.StackTrace, Color.Red);
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
                DarkMessageBox msg = new DarkMessageBox("Are you sure you want to pause this download?\nThis will make the download start again when the download has been resumed.", "Pause Download?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, false);
                DialogResult result = msg.ShowDialog();

                if (result == DialogResult.Yes)
                {
                    Log("Download of " + url + " has been paused.", Color.White);

                    pauseButton.Text = "Resume";

                    cancellationToken.Cancel();
                    progressBar1.State = ProgressBarState.Warning;
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

                try
                {
                    stream = new FileStream(location + fileName, FileMode.Create);

                    await DownloadFileAsync(uri, stream, cancellationToken.Token, Client_DownloadProgressChanged);

                    Client_DownloadFileCompleted();

                    stream.Close();
                }
                catch (System.Threading.Tasks.TaskCanceledException ex)
                {
                    Log("Download of " + url + "was canceled.", Color.White);
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
                        progressBar1.Style = ProgressBarStyle.Blocks;
                        progressBar1.MarqueeAnim = false;
                        progressBar1.Value = 100;
                        progressBar1.State = ProgressBarState.Error;
                    }));

                    Log(ex.Message, Color.Red);
                    DarkMessageBox msg = new DarkMessageBox(ex.Message, "Download Manager - Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                    msg.ShowDialog();
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
                DarkMessageBox msg = new DarkMessageBox(ex.Message + $" ({ex.GetType().FullName})" + ex.StackTrace, "Failed to start process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                msg.ShowDialog();
                msg.Dispose();
            }
        }

        private void updateDisplayTimer_Tick(object sender, EventArgs e)
        {
            // Update display
            bytesLabel.Text = $"({receivedBytes} B / {totalBytes} B)";
            this.Text = $"Downloading {fileName}... ({string.Format("{0:0.##}", percentageDone)}%)";
            label3.Text = $"{percentageDone}%";
        }
    }
}