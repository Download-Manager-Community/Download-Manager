namespace DownloadManager.Download
{
    internal class DownloadProgressUpdater
    {
        // DownloadSegment0
        public long contentLength0 = 0;
        public long read0 = 0;
        public long progress0 = 0;

        // DownloadSegment1
        public long contentLength1 = 0;
        public long read1 = 0;
        public long progress1 = 0;

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
                progressWindow.bytesLabel.Text = $"({totalRead} B / {totalLength} B)";
                totalProgress = ((int)progress0 + (int)progress1) / 2;
                progressWindow.percentageDone = totalProgress;
                progressWindow.Text = $"Downloading {progressWindow.fileName}... ({totalProgress}%)";
                progressWindow.progressLabel.Text = $"{((double)progress0 + (double)progress1) / 2}%";
                progressWindow.totalProgress = totalProgress;
            }));
        }
    }
}
