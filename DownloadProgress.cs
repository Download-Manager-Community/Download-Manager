using DownloadManager.NativeMethods;
using Microsoft.Toolkit.Uwp.Notifications;
using System.Diagnostics;
using System.Media;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using static DownloadManager.BetterProgressBar;
using static DownloadManager.Logging;

namespace DownloadManager
{
    public partial class DownloadProgress : Form
    {
        public static DownloadProgress _instance;
        Thread thread;
        public DownloadType downloadType;
        public YoutubeDownloadType? ytDownloadType;
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

        public enum YoutubeDownloadType
        {
            Audio,
            Video,
            AudioVideo
        };

        public DownloadProgress(string urlArg, string locationArg, DownloadType downloadType, YoutubeDownloadType? ytDownloadType, string hashArg, int hashTypeArg, int downloadAttempts = 0)
        {
            InitializeComponent();
            _instance = this;

            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);

            this.downloadAttempts = downloadAttempts;

            this.downloadType = downloadType;
            this.ytDownloadType = ytDownloadType;

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

        private void progress_Load(object sender, EventArgs e)
        {
            Log("Preparing to start downloading...", Color.White);
            checkBox2.Checked = Settings.Default.closeOnComplete;
            checkBox1.Checked = Settings.Default.keepOnTop;
            progressBar1.Style = ProgressBarStyle.Marquee;
            progressBar1.MarqueeAnim = true;
            if (downloadType == DownloadType.Normal)
            {
                thread = new Thread(new ThreadStart(StartNormalDownload));
                thread.Start();
            }
            else if (downloadType == DownloadType.YoutubeVideo)
            {
                thread = new Thread(new ThreadStart(StartYoutubeVideoDownload));
                thread.Start();
            }
            else if (downloadType == DownloadType.YoutubePlaylist)
            {
                thread = new Thread(new ThreadStart(StartYoutubePlaylistDownload));
                thread.Start();
            }
        }

        private async void StartYoutubePlaylistDownload()
        {
            YoutubeClient client = new YoutubeClient();
            YoutubeExplode.Playlists.Playlist listMetadata = client.Playlists.GetAsync(YoutubeExplode.Playlists.PlaylistId.Parse(url)).Result;

            updateDisplayTimer.Stop();

            this.Invoke(new MethodInvoker(delegate ()
            {
                fileName = listMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_");

                this.Text = $"Downloading {listMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + @"\"}...";
                urlLabel.Text = $"{listMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + @"\"} from youtube.com";
                hashLabel.Text = "Downloading YouTube playlists does not support file verification.";
                bytesLabel.Visible = false;
                label3.Visible = false;
                progressBar1.Style = ProgressBarStyle.Marquee;
                progressBar1.Visible = true;

                if (downloadType == DownloadType.YoutubeVideo || downloadType == DownloadType.YoutubePlaylist)
                {
                    if (DownloadForm.ytDownloading == true)
                    {
                        progressBar1.Style = ProgressBarStyle.Blocks;
                        progressBar1.State = ProgressBarState.Error;
                        progressBar1.ShowText = false;
                        progressBar1.Value = 100;
                        DarkMessageBox msg = new("Another YouTube download is currently in progress.\nPlease wait until the download is complete before attempting to download another.", "YouTube Download Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        msg.ShowDialog();
                        downloading = false;
                        DownloadForm.downloadsAmount -= 1;
                        DownloadForm.downloadsList.Remove(this);
                        this.Close();
                        this.Dispose();
                        return;
                    }
                }

                DownloadForm.ytDownloading = true;
            }));

            if (downloading == false)
            {
                return;
            }

            if (ytDownloadType != null)
            {
                // Create a folder for the playlist
                string playlistFolder = location + listMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + @"\";
                System.IO.Directory.CreateDirectory(playlistFolder);

                if (ytDownloadType == YoutubeDownloadType.Audio)
                {
                    // Audio Only
                    Thread thread = new Thread(async delegate ()
                    {
                        // Check how many videos are in the playlist
                        IAsyncEnumerable<YoutubeExplode.Playlists.PlaylistVideo> videosList = client.Playlists.GetVideosAsync(listMetadata.Id);
                        int videoCount = 0;

                        await foreach (YoutubeExplode.Playlists.PlaylistVideo video in client.Playlists.GetVideosAsync(listMetadata.Id))
                        {
                            videoCount += 1;
                        }

                        // Update progressbar with video count 
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            progressBar1.Maximum = videoCount;
                            progressBar1.Minimum = 0;
                            progressBar1.Value = 0;
                            progressBar1.Style = ProgressBarStyle.Blocks;
                        }));

                        // Download each video
                        await foreach (YoutubeExplode.Playlists.PlaylistVideo video in client.Playlists.GetVideosAsync(listMetadata.Id))
                        {
                            var streamManifest = client.Videos.Streams.GetManifestAsync(video.Id);

                            var streamInfo = streamManifest.Result.GetAudioOnlyStreams().TryGetWithHighestBitrate();

                            await client.Videos.Streams.DownloadAsync(streamInfo, System.IO.Path.GetTempPath() + "temp.webm");

                            try
                            {
                                ConvertAudio(System.IO.Path.GetTempPath() + "temp.webm", playlistFolder + video.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + ".mp3");
                            }
                            catch (Exception ex)
                            {
                                DarkMessageBox msgerr = new DarkMessageBox("Error while converting file format:\n" + ex.Message + Environment.NewLine + ex.StackTrace, "Download Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, false);
                                DialogResult result = msgerr.ShowDialog();

                                if (result == DialogResult.Retry)
                                {
                                    this.Invoke(new MethodInvoker(delegate ()
                                    {
                                        StartYoutubeVideoDownload();
                                    }));

                                    return;
                                }
                                else
                                {
                                    downloading = false;
                                    DownloadForm.downloadsAmount -= 1;
                                    DownloadForm.ytDownloading = false;
                                    DownloadForm.downloadsList.Remove(this);
                                    this.Invoke(new MethodInvoker(delegate ()
                                    {
                                        this.Close();
                                        this.Dispose();
                                    }));
                                    return;
                                }
                            }

                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                progressBar1.Value += 1;
                            }));
                        }

                        pictureBox1.Image = Properties.Resources.fileTransferDone;

                        downloading = false;
                        DownloadForm.downloadsAmount -= 1;
                        DownloadForm.ytDownloading = false;
                        Log("Finished downloading file.", Color.White);

                        if (Settings.Default.notifyDone)
                        {
                            new ToastContentBuilder()
                             .AddText($"The download of {listMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")} is complete.")
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
                            progressBar1.Style = ProgressBarStyle.Blocks;
                            progressBar1.MarqueeAnim = false;
                            if (checkBox2.Checked == true)
                            {

                                this.Close();
                                this.Dispose();
                                return;
                            }
                        }));
                    });
                    thread.Start();
                }
                else if (ytDownloadType == YoutubeDownloadType.Video)
                {
                    // Video Only
                    Thread thread = new Thread(async delegate ()
                    {
                        // Check how many videos are in the playlist
                        IAsyncEnumerable<YoutubeExplode.Playlists.PlaylistVideo> videosList = client.Playlists.GetVideosAsync(listMetadata.Id);
                        int videoCount = 0;

                        await foreach (YoutubeExplode.Playlists.PlaylistVideo video in client.Playlists.GetVideosAsync(listMetadata.Id))
                        {
                            videoCount += 1;
                        }

                        // Update progressbar with video count 
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            progressBar1.Maximum = videoCount;
                            progressBar1.Minimum = 0;
                            progressBar1.Value = 0;
                            progressBar1.Style = ProgressBarStyle.Blocks;
                        }));

                        // Download each video
                        await foreach (YoutubeExplode.Playlists.PlaylistVideo video in client.Playlists.GetVideosAsync(listMetadata.Id))
                        {
                            var streamManifest = client.Videos.Streams.GetManifestAsync(video.Id);

                            var streamInfo = streamManifest.Result.GetVideoOnlyStreams().TryGetWithHighestVideoQuality();

                            await client.Videos.Streams.DownloadAsync(streamInfo, System.IO.Path.GetTempPath() + "temp.webm");

                            try
                            {
                                File.Move(System.IO.Path.GetTempPath() + "temp.webm", playlistFolder + video.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + ".webm");
                            }
                            catch (Exception ex)
                            {
                                DarkMessageBox msg = new DarkMessageBox(ex.Message, "Error writing file", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                                msg.ShowDialog();
                            }

                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                progressBar1.Value += 1;
                            }));
                        }

                        pictureBox1.Image = Properties.Resources.fileTransferDone;

                        downloading = false;
                        DownloadForm.downloadsAmount -= 1;
                        DownloadForm.ytDownloading = false;
                        Log("Finished downloading file.", Color.White);

                        if (Settings.Default.notifyDone)
                        {
                            new ToastContentBuilder()
                             .AddText($"The download of {listMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")} is complete.")
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
                    });
                    thread.Start();
                }
                else if (ytDownloadType == YoutubeDownloadType.AudioVideo)
                {
                    // Audio & Video
                    Thread thread = new Thread(async delegate ()
                    {
                        // Check how many videos are in the playlist
                        IAsyncEnumerable<YoutubeExplode.Playlists.PlaylistVideo> videosList = client.Playlists.GetVideosAsync(listMetadata.Id);
                        int videoCount = 0;

                        await foreach (YoutubeExplode.Playlists.PlaylistVideo video in client.Playlists.GetVideosAsync(listMetadata.Id))
                        {
                            videoCount += 1;
                        }

                        // Update progressbar with video count 
                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            progressBar1.Maximum = videoCount;
                            progressBar1.Minimum = 0;
                            progressBar1.Value = 0;
                            progressBar1.Style = ProgressBarStyle.Blocks;
                        }));

                        // Download each video
                        await foreach (YoutubeExplode.Playlists.PlaylistVideo video in client.Playlists.GetVideosAsync(listMetadata.Id))
                        {
                            var streamManifest = client.Videos.Streams.GetManifestAsync(video.Id);

                            var streamInfo = streamManifest.Result.GetMuxedStreams().TryGetWithHighestVideoQuality();

                            await client.Videos.Streams.DownloadAsync(streamInfo, System.IO.Path.GetTempPath() + "temp.webm");

                            try
                            {
                                File.Move(System.IO.Path.GetTempPath() + "temp.webm", playlistFolder + video.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + ".webm");
                            }
                            catch (Exception ex)
                            {
                                DarkMessageBox msg = new DarkMessageBox(ex.Message, "Error writing file", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                                msg.ShowDialog();
                            }

                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                progressBar1.Value += 1;
                            }));
                        }

                        pictureBox1.Image = Properties.Resources.fileTransferDone;

                        downloading = false;
                        DownloadForm.downloadsAmount -= 1;
                        DownloadForm.ytDownloading = false;
                        Log("Finished downloading file.", Color.White);

                        if (Settings.Default.notifyDone)
                        {
                            new ToastContentBuilder()
                             .AddText($"The download of {listMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")} is complete.")
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
                            progressBar1.MarqueeAnim = false;
                            if (checkBox2.Checked == true)
                            {
                                this.Close();
                                this.Dispose();
                                return;
                            }
                        }));
                    });
                    thread.Start();
                }
                else
                {
                    if (Settings.Default.notifyFail)
                    {
                        new ToastContentBuilder()
                           .AddText("An internal error has occurred preventing you from downloading this file.")
                           .AddText("Please submit a bug report.")
                           .Show();
                    }

                    DarkMessageBox msg = new("An internal error has occurred preventing you from downloading your file.\nPlease submit a bug report.\n\nDetails: ytDownloadType is null when attempting a YouTube Download.", "Internal Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, false);
                    if (msg.ShowDialog() == DialogResult.Retry)
                    {
                        StartYoutubeVideoDownload();
                        return;
                    }
                    else
                    {
                        downloading = false;
                        DownloadForm.downloadsAmount -= 1;
                        DownloadForm.downloadsList.Remove(this);
                        DownloadForm.ytDownloading = false;

                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            this.Close();
                            this.Dispose();
                        }));
                    }
                }
            }
        }

        private async void StartYoutubeVideoDownload()
        {
            if (ytDownloadType != null)
            {
                YoutubeClient client = new YoutubeClient();
                YoutubeExplode.Videos.Video? vidMetadata = null;

                vidMetadata = client.Videos.GetAsync(YoutubeExplode.Videos.VideoId.Parse(url)).Result;

                updateDisplayTimer.Stop();

                this.Invoke(new MethodInvoker(delegate ()
                {
                    bytesLabel.Visible = false;
                    label3.Text = "Downloading YouTube videos does not support progress callbacks.";
                    hashLabel.Text = "Downloading YouTube videos does not support file verification.";
                    pauseButton.Enabled = false;

                    if (downloadType == DownloadType.YoutubeVideo || downloadType == DownloadType.YoutubePlaylist)
                    {
                        if (DownloadForm.ytDownloading == true)
                        {
                            progressBar1.Style = ProgressBarStyle.Blocks;
                            progressBar1.State = ProgressBarState.Error;
                            progressBar1.ShowText = false;
                            progressBar1.Value = 100;
                            DarkMessageBox msg = new("Another YouTube download is currently in progress.\nPlease wait until the download is complete before attempting to download another.", "YouTube Download Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            msg.ShowDialog();
                            downloading = false;
                            DownloadForm.downloadsAmount -= 1;
                            DownloadForm.downloadsList.Remove(this);
                            this.Close();
                            this.Dispose();
                            return;
                        }
                    }

                    DownloadForm.ytDownloading = true;
                }));


                if (downloading == false)
                {
                    return;
                }

                if (ytDownloadType == YoutubeDownloadType.Audio)
                {
                    //
                    // Audio
                    //

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        fileName = $"{vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")}.mp3";

                        this.Text = $"Downloading {vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")}.mp3...";
                        urlLabel.Text = $"{vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")}.mp3 from youtube.com";
                    }));

                    var streamManifest = client.Videos.Streams.GetManifestAsync(vidMetadata.Id);

                    var streamInfo = streamManifest.Result.GetAudioOnlyStreams().GetWithHighestBitrate();

                    await client.Videos.Streams.DownloadAsync(streamInfo, System.IO.Path.GetTempPath() + "temp.webm");

                    string newName = vidMetadata.Title;
                    int charsReplaced = 0;

                    foreach (char badChar in Path.GetInvalidFileNameChars())
                    {
                        if (newName.Contains(badChar))
                        {
                            newName.Replace(badChar, '_');
                            charsReplaced += 1;
                        }
                    }

                    if (charsReplaced >= 1)
                    {
                        DarkMessageBox msgerr = new DarkMessageBox("Video title contains " + charsReplaced + " illegal characters.\nThe characters will be replaced with '_' in the file name.", "YouTube Download Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, true);
                        msgerr.ShowDialog();
                    }

                    try
                    {
                        ConvertAudio(System.IO.Path.GetTempPath() + "temp.webm", location + vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + ".mp3");
                    }
                    catch (Exception ex)
                    {
                        DarkMessageBox msgerr = new DarkMessageBox("Error while converting file format:\n" + ex.Message + Environment.NewLine + ex.StackTrace, "Download Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, false);
                        DialogResult result = msgerr.ShowDialog();

                        if (result == DialogResult.Retry)
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                StartYoutubeVideoDownload();
                            }));

                            return;
                        }
                        else
                        {
                            downloading = false;
                            DownloadForm.downloadsAmount -= 1;
                            DownloadForm.ytDownloading = false;
                            DownloadForm.downloadsList.Remove(this);
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                this.Close();
                                this.Dispose();
                            }));
                            return;
                        }
                    }

                    pictureBox1.Image = Properties.Resources.fileTransferDone;

                    downloading = false;
                    DownloadForm.downloadsAmount -= 1;
                    DownloadForm.ytDownloading = false;
                    Log("Finished downloading file.", Color.White);

                    if (Settings.Default.notifyDone)
                    {
                        new ToastContentBuilder()
                         .AddText($"The download of {vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")}.mp3 is complete.")
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
                else if (ytDownloadType == YoutubeDownloadType.Video)
                {
                    //
                    // Video
                    //
                    var streamManifest = client.Videos.Streams.GetManifestAsync(vidMetadata.Id);

                    var streamInfo = streamManifest.Result.GetVideoStreams().GetWithHighestVideoQuality();

                    await client.Videos.Streams.DownloadAsync(streamInfo, System.IO.Path.GetTempPath() + "temp.webm");

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        fileName = $"{vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")}.webm";

                        this.Text = $"Downloading {vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")}.webm...";
                        urlLabel.Text = $"{vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")}.webm from youtube.com";
                    }));

                    string newName = vidMetadata.Title;
                    int charsReplaced = 0;

                    foreach (char badChar in Path.GetInvalidFileNameChars())
                    {
                        if (newName.Contains(badChar))
                        {
                            newName.Replace(badChar, '_');
                            charsReplaced += 1;
                        }
                    }

                    if (charsReplaced >= 1)
                    {
                        DarkMessageBox msgerr = new DarkMessageBox("Video title contains " + charsReplaced + " illegal characters.\nThe characters will be replaced with '_' in the file name.", "YouTube Download Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, true);
                        msgerr.ShowDialog();
                    }

                    try
                    {
                        File.Move(System.IO.Path.GetTempPath() + "temp.webm", location + vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + ".webm");
                    }
                    catch (Exception ex)
                    {
                        DarkMessageBox msgerr = new DarkMessageBox("Error while writing file:\n" + ex.Message + Environment.NewLine + ex.StackTrace, "Download Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, false);
                        DialogResult result = msgerr.ShowDialog();

                        if (result == DialogResult.Retry)
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                StartYoutubeVideoDownload();
                            }));

                            return;
                        }
                        else
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                downloading = false;
                                DownloadForm.downloadsAmount -= 1;
                                DownloadForm.ytDownloading = false;
                                DownloadForm.downloadsList.Remove(this);
                                this.Close();
                                this.Dispose();
                            }));
                            return;
                        }
                    }

                    pictureBox1.Image = Properties.Resources.fileTransferDone;

                    downloading = false;
                    DownloadForm.downloadsAmount -= 1;
                    DownloadForm.ytDownloading = false;
                    Log("Finished downloading file.", Color.White);

                    if (Settings.Default.notifyDone)
                    {
                        new ToastContentBuilder()
                         .AddText($"The download of {vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")}.webm is complete.")
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
                    //
                    // AudioVideo
                    //
                    var streamManifest = client.Videos.Streams.GetManifestAsync(vidMetadata.Id);

                    var streamInfo = streamManifest.Result.GetMuxedStreams().GetWithHighestVideoQuality();

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        fileName = $"{vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")}.webm";

                        this.Text = $"Downloading {vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")}.webm...";
                        urlLabel.Text = $"{vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")}.webm from youtube.com";
                    }));

                    try
                    {
                        await client.Videos.Streams.DownloadAsync(streamInfo, System.IO.Path.GetTempPath() + "temp.webm");
                    }
                    catch (Exception ex)
                    {
                        DarkMessageBox msg = new($"{ex.Message} ({ex.GetType().FullName})\n{ex.StackTrace}");
                        msg.ShowDialog();

                        if (Settings.Default.notifyFail)
                        {
                            new ToastContentBuilder()
                               .AddText($"The download of {vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")}.webm has failed.")
                               .Show();
                        }

                        downloading = false;
                        DownloadForm.downloadsAmount -= 1;
                        DownloadForm.ytDownloading = false;
                        DownloadForm.downloadsList.Remove(this);

                        this.Invoke(new MethodInvoker(delegate ()
                        {
                            this.Close();
                            this.Dispose();
                        }));
                        return;
                    }

                    string newName = vidMetadata.Title;
                    int charsReplaced = 0;

                    foreach (char badChar in Path.GetInvalidFileNameChars())
                    {
                        if (newName.Contains(badChar))
                        {
                            newName.Replace(badChar, '_');
                            charsReplaced += 1;
                        }
                    }

                    if (charsReplaced >= 1)
                    {
                        DarkMessageBox msgerr = new DarkMessageBox("Video title contains " + charsReplaced + " illegal characters.\nThe characters will be replaced with '_' in the file name.", "YouTube Download Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning, true);
                        msgerr.ShowDialog();
                    }

                    try
                    {
                        File.Move(System.IO.Path.GetTempPath() + "temp.webm", location + vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_") + ".webm");
                    }
                    catch (Exception ex)
                    {
                        DarkMessageBox msgerr = new DarkMessageBox("Error while writing file:\n" + ex.Message + Environment.NewLine + ex.StackTrace, "Download Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, false);
                        DialogResult result = msgerr.ShowDialog();

                        if (result == DialogResult.Retry)
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                StartYoutubeVideoDownload();
                            }));

                            return;
                        }
                        else
                        {
                            this.Invoke(new MethodInvoker(delegate ()
                            {
                                downloading = false;
                                DownloadForm.downloadsAmount -= 1;
                                DownloadForm.ytDownloading = false;
                                DownloadForm.downloadsList.Remove(this);
                                this.Close();
                                this.Dispose();
                            }));

                            return;
                        }
                    }

                    pictureBox1.Image = Properties.Resources.fileTransferDone;

                    downloading = false;
                    DownloadForm.downloadsAmount -= 1;
                    DownloadForm.ytDownloading = false;
                    Log("Finished downloading file.", Color.White);

                    if (Settings.Default.notifyDone)
                    {
                        new ToastContentBuilder()
                         .AddText($"The download of {vidMetadata.Title.Replace(":", "_").Replace("<", "_").Replace(">", "_").Replace('"', '_').Replace("/", "_").Replace(@"\", "_").Replace("|", "_").Replace("?", "_").Replace("*", "_")}.webm is complete.")
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
                if (Settings.Default.notifyFail)
                {
                    new ToastContentBuilder()
                       .AddText("An internal error has occurred preventing you from downloading this file.")
                       .AddText("Please submit a bug report.")
                       .Show();
                }

                DarkMessageBox msg = new("An internal error has occurred preventing you from downloading your file.\nPlease submit a bug report.\n\nDetails: ytDownloadType is null when attempting a YouTube Download.", "Internal Error", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, false);
                if (msg.ShowDialog() == DialogResult.Retry)
                {
                    StartYoutubeVideoDownload();
                    return;
                }
                else
                {
                    downloading = false;
                    DownloadForm.downloadsAmount -= 1;
                    DownloadForm.ytDownloading = false;
                    DownloadForm.downloadsList.Remove(this);

                    this.Invoke(new MethodInvoker(delegate ()
                    {
                        this.Close();
                        this.Dispose();
                    }));
                }
            }
        }

        private void ConvertAudio(string fileName, string destFileName)
        {
            Log("Beginning file conversion...", Color.White);

            ProcessStartInfo startInfo = new();
            startInfo.FileName = "ffmpeg.exe";
            startInfo.Arguments = $"-i \"{fileName}\" -vn -y \"{destFileName}\"";
            startInfo.CreateNoWindow = true;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            Process process = new();
            process.StartInfo = startInfo;
            process.OutputDataReceived += (sender, args) => Log("[ffmpeg.exe] " + args.Data, Color.White);
            process.ErrorDataReceived += (sender, args) => Log("[ffmpeg.exe] " + args.Data, Color.White);
            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                throw new Exception("ffmpeg.exe failed to convert file with exit code " + process.ExitCode + ".");
            }
            else
            {
                Log("Finished file conversion.", Color.White);
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
                                                    DownloadProgress download = new DownloadProgress(url, location, DownloadType.Normal, null, hash, hashType, downloadAttempts + 1);
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
                                                    DownloadProgress download = new DownloadProgress(url, location, DownloadType.Normal, null, hash, hashType, downloadAttempts + 1);
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
                                                    DownloadProgress download = new DownloadProgress(url, location, DownloadType.Normal, null, hash, hashType, downloadAttempts + 1);
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
                                                    DownloadProgress download = new DownloadProgress(url, location, DownloadType.Normal, null, hash, hashType, downloadAttempts + 1);
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
                                                    DownloadProgress download = new DownloadProgress(url, location, DownloadType.Normal, null, hash, hashType, downloadAttempts + 1);
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