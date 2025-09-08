using DownloadManager;
using DownloadManager.Components;
using DownloadManager.Components.Addons;
using DownloadManager.Components.Addons.YouTubeDownloader;
using System.Diagnostics;
using static DownloadManager.BetterProgressBar;
using static DownloadManager.Logging;

namespace YouTubeDownloadAddon
{
    public class Addon : IAddon
    {
        public static IAddon _instance = null;
        public string Name => "YouTube Downloader";

        public void Execute()
        {
            _instance = this;
            Test test = new Test();
            test.Show();
        }

        /// <summary>
        /// Interacts with the YouTube Downloader addon to download a video from YouTube.
        /// </summary>
        /// <param name="url">The URL of the video to download.</param>
        /// <param name="convert">If the video should be converted to MP3.</param>
        public static void Download(string url, bool convert, YouTubeOptions window)
        {
            Logging.Log(LogLevel.Info, "Downloading YouTube video from " + url);

            // Check if the addon is installed
            if (!CheckAddon())
            {
                // Display an error and return if the addon is not installed
                DarkMessageBox.Show("The YouTube Downloader addon is missing essential files.\nPlease reinstall this addon.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                window.ToggleControls(true);
                window.progressBar.Value = 100;
                window.progressBar.State = ProgressBarState.Error;
                window.progressBar.Style = ProgressBarStyle.Blocks;
                window.progressBar.CustomText = "Status: Missing essential files.";
                return;
            }

            // --addonMode --url:URL --path:PATH --convert:True/False
            Process process = CreateAddonProcess($"--addonMode --url:{url} --path:{DownloadForm.downloadsFolder} --convert:{convert.ToString()}");
            process.Start();
            process.OutputDataReceived += (sender, e) => LogOutput(e.Data, window);
            process.ErrorDataReceived += (sender, e) => LogOutput(e.Data, window);
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Reset the progress bar and set it to marquee mode
            window.progressBar.Value = 0;
            window.progressBar.State = ProgressBarState.Normal;
            window.progressBar.Style = ProgressBarStyle.Marquee;
        }

        private static Process CreateAddonProcess(string arguments)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = $"{DownloadForm.installationPath}\\Addons\\YouTubeDownload.exe",
                Arguments = arguments,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };
            Process process = new Process
            {
                StartInfo = startInfo
            };
            return process;
        }

        private static void LogOutput(string data, YouTubeOptions window)
        {
            if (!string.IsNullOrEmpty(data))
            {
                // Invoke the UI thread to update the progress bar and log messages
                window.Invoke((MethodInvoker)delegate
                {
                    // Check for specific keywords in the output data and update the progress bar accordingly
                    if (data.ToLower().Contains("downloading"))
                    {
                        // Downloading message
                        Logging.Log(LogLevel.Info, data);
                        window.progressBar.CustomText = "Status: Downloading...";
                    }
                    else if (data.ToLower().Contains("converting"))
                    {
                        // Converting message
                        Logging.Log(LogLevel.Info, data);
                        window.progressBar.CustomText = "Status: Converting...";
                    }
                    else if (data.ToLower().Contains("successfully"))
                    {
                        // Download completed successfully
                        Logging.Log(LogLevel.Info, data);
                        window.progressBar.CustomText = "Status: Download completed successfully.";
                        window.ToggleControls(true);
                        window.progressBar.Value = 100;
                        window.progressBar.State = ProgressBarState.Normal;
                        window.progressBar.Style = ProgressBarStyle.Blocks;
                    }
                    else if (data.ToLower().Contains("error"))
                    {
                        // Error message
                        Logging.Log(LogLevel.Error, data);
                        window.progressBar.CustomText = "Status: Error occurred during download.";
                        window.progressBar.Value = 100;
                        window.progressBar.State = ProgressBarState.Error;
                        window.progressBar.Style = ProgressBarStyle.Blocks;
                        window.ToggleControls(true);
                    }
                    else if (data.ToLower().Contains("fatal"))
                    {
                        // Fatal error
                        Logging.Log(LogLevel.Error, data);
                        window.progressBar.CustomText = "Status: Fatal error occurred during download.";
                        window.progressBar.Value = 100;
                        window.progressBar.State = ProgressBarState.Error;
                        window.progressBar.Style = ProgressBarStyle.Blocks;
                        window.ToggleControls(true);
                    }
                    else // Other messages
                        Logging.Log(LogLevel.Info, data);
                });
            }
        }

        private static bool CheckAddon()
        {
            if (!File.Exists($"{DownloadForm.installationPath}\\Addons\\YouTubeDownload.exe"))
            {
                // The addon is missing files, return false
                return false;
            }
            else
            {
                // The addon is not missing files, return true
                return true;
            }
        }
    }
}
