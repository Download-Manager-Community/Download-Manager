using AngleSharp.Text;
using Timer = System.Windows.Forms.Timer;

namespace DownloadManager
{
    public class DownloadItem
    {
        CurrentDownloads downloadsForm = CurrentDownloads._instance;
        Timer timer = new Timer();
        public DownloadProgress progress;
        int index = 0;
        bool isYtDownload = false;
        bool contentLengthIssue = false;

        public void Initialize(DownloadProgress progress)
        {
            this.progress = progress;

            index = downloadsForm.progressGridView.Rows.Add(progress.fileName, progress.percentageDone, progress.url, "? B");

            timer.Tick += UpdateTimer_Tick;
            timer.Interval = 500;
            timer.Start();
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (progress == null || progress.downloading == false || progress.cancelled == true)
            {
                // If the download is finished, dispose the item
                Dispose();
                timer.Stop();
                return;
            }
            else
            {
                // Update the values
                try
                {
                    downloadsForm.progressGridView.Rows[index].Cells[0].Value = progress.fileName;
                    downloadsForm.progressGridView.Rows[index].Cells[3].Value = progress.url;
                    if (progress.totalSize < 1)
                    {
                        downloadsForm.progressGridView.Rows[index].Cells[5].Value = "? B";
                    }
                    else
                    {
                        long totalBytes = progress.totalSize;
                        long totalKilobytes = (progress.totalSize / 1024);
                        long totalMegabytes = ((progress.totalSize / 1024) / 1024);
                        float totalGigabytes = ((((float)progress.totalSize / 1024) / 1024) / 1024);

                        if (totalGigabytes > 1)
                        {
                            downloadsForm.progressGridView.Rows[index].Cells[5].Value = $"{totalGigabytes.ToString("0.00")} GB";
                        }
                        else if (totalMegabytes > 1)
                        {
                            downloadsForm.progressGridView.Rows[index].Cells[5].Value = $"{totalMegabytes.ToString("0.00")} MB";
                        }
                        else if (totalKilobytes > 1)
                        {
                            downloadsForm.progressGridView.Rows[index].Cells[5].Value = $"{totalKilobytes.ToString("0.00")} KB";
                        }
                        else
                        {
                            downloadsForm.progressGridView.Rows[index].Cells[5].Value = $"{totalBytes.ToString("0.00")} B";
                        }

                        long receivedBytes = (long)progress.receivedBytes;
                        long receivedKilobytes = ((long)progress.receivedBytes / 1024);
                        long receivedMegabytes = (((long)progress.receivedBytes / 1024) / 1024);
                        float receivedGigabytes = ((((float)progress.receivedBytes / 1024) / 1024) / 1024);

                        if (receivedGigabytes > 1)
                        {
                            downloadsForm.progressGridView.Rows[index].Cells[4].Value = receivedGigabytes.ToString("0.00") + " GB";
                        }
                        else if (receivedMegabytes > 1)
                        {
                            downloadsForm.progressGridView.Rows[index].Cells[4].Value = $"{receivedMegabytes.ToString("0.00")} MB";
                        }
                        else if (receivedKilobytes > 1)
                        {
                            downloadsForm.progressGridView.Rows[index].Cells[4].Value = $"{receivedKilobytes.ToString("0.00")} KB";
                        }
                        else
                        {
                            downloadsForm.progressGridView.Rows[index].Cells[4].Value = $"{receivedBytes.ToString("0.00")} B";
                        }

                        //Debug.WriteLine($"receivedBytes is {receivedBytes} which is:\n{receivedKilobytes}KB, {receivedMegabytes}MB and {receivedGigabytes}GB\n({progress.receivedBytes})");
                    }

                    // Display Speed
                    double bps = progress.bytesPerSecond;
                    double kbps = progress.kilobytesPerSecond;
                    double mbps = progress.megabytesPerSecond;

                    if (mbps > 1)
                    {
                        downloadsForm.progressGridView.Rows[index].Cells[2].Value = $"{mbps.ToString("0.00")} MB/s";
                    }
                    else if (kbps > 1)
                    {
                        downloadsForm.progressGridView.Rows[index].Cells[2].Value = $"{kbps.ToString("0.00")} KB/s";
                    }
                    else
                    {
                        downloadsForm.progressGridView.Rows[index].Cells[2].Value = $"{bps.ToString("0.00")} B/s";
                    }

                    if (progress.downloadType == DownloadProgress.DownloadType.YoutubePlaylist)
                    {
                        // Update the progressBar
                        downloadsForm.progressGridView.Rows[index].Cells[1].Value = (int)progress.progressLabel.Text.Replace("%", "").ToDouble();
                    }
                    else if (progress.downloadType == DownloadProgress.DownloadType.YoutubeVideo)
                    {
                        downloadsForm.progressGridView.Rows[index].Cells[1].Tag = -1;
                    }
                    else
                    {
                        // Update the progress bar
                        if ((int)progress.percentageDone > 100)
                        {
                            Logging.Log("DownloadItem Progress is greater than 100%! The item has been removed to prevent a crash!", Color.Orange);
                            Dispose();
                        }

                        if (progress.totalSize < 1)
                        {
                            // If the total size is less than 1, then we cannot report progress
                            // Update the progress bar
                            downloadsForm.progressGridView.Rows[index].Cells[1].Tag = -1;
                        }
                        else
                        {
                            // Update the progress bar
                            downloadsForm.progressGridView.Rows[index].Cells[1].Tag = (int)progress.progressLabel.Text.Replace("%", "").ToDouble();
                        }

                        //Logging.Log(((int)progress.progressLabel.Text.Replace("%", "").ToDouble()).ToString(), Color.Gray);
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Refresh the list, an item may have completed making the index out of range
                    downloadsForm.RefreshList();
                }
                catch (Exception ex)
                {
                    Logging.Log($"{ex.Message} ({ex.GetType().FullName})\n{ex.StackTrace}", Color.Red);
                }
            }
        }

        public void Dispose()
        {
            downloadsForm.itemList.Remove(this);
            try
            {
                downloadsForm.progressGridView.Rows.RemoveAt(index);
            }
            catch (Exception ex)
            {
                Logging.Log($"{ex.Message} ({ex.GetType().FullName})\n{ex.StackTrace}", Color.Red);
            }

            timer.Stop();
            timer.Dispose();
        }

        public void DisposeNoRemove()
        {
            timer.Stop();
            timer.Dispose();
        }
    }
}
