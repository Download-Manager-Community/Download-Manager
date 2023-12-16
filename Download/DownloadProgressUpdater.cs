namespace DownloadManager.Download
{
    internal class DownloadProgressUpdater
    {
        // DownloadSegment0
        public long contentLength0 = 0;
        public long read0 = 0;
        public long progress0 = 0;
        public double bytesPerSecond0 = 0;
        public double kilobytesPerSecond0 = 0;
        public double megabytesPerSecond0 = 0;

        // DownloadSegment1
        public long contentLength1 = 0;
        public long read1 = 0;
        public long progress1 = 0;
        public double bytesPerSecond1 = 0;
        public double kilobytesPerSecond1 = 0;
        public double megabytesPerSecond1 = 0;

        // Total
        public long totalLength = 0;
        private long totalRead = 0;
        private int totalProgress = 0;

        // Other
        internal DownloadProgress progressWindow;

        public void Initialize(long totalLength, DownloadProgress window)
        {
            this.totalLength = totalLength;
            this.progressWindow = window;
            UpdateUI();
        }

        public void UpdateUI()
        {
            totalRead = read0 + read1;
            if (progressWindow == null)
            {
                return;
            }

            progressWindow.Invoke(new MethodInvoker(delegate
            {
                double bytesPerSecond = bytesPerSecond0 + bytesPerSecond1;
                double kilobytesPerSecond = kilobytesPerSecond0 + kilobytesPerSecond1;
                double megabytesPerSecond = megabytesPerSecond0 + megabytesPerSecond1;

                progressWindow.bytesLabel.Text = $"({totalRead} B / {totalLength} B)";

                if (megabytesPerSecond > 1)
                {
                    progressWindow.speedLabel.Text = $"{megabytesPerSecond.ToString("0.00")} MB/s";
                }
                else if (kilobytesPerSecond > 1)
                {
                    progressWindow.speedLabel.Text = $"{kilobytesPerSecond.ToString("0.00")} KB/s";
                }
                else
                {
                    progressWindow.speedLabel.Text = $"{bytesPerSecond.ToString("0.00")} B/s";
                }

                totalProgress = ((int)progress0 + (int)progress1) / 2;
                progressWindow.percentageDone = totalProgress;
                progressWindow.Text = $"Downloading {progressWindow.fileName}... ({totalProgress}%)";
                progressWindow.progressLabel.Text = $"{((double)progress0 + (double)progress1) / 2}%";
                progressWindow.totalProgress = totalProgress;
            }));
        }
    }
}
