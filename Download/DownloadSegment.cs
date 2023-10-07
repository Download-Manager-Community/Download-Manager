namespace DownloadManager.Download
{
    internal class DownloadSegment
    {
        public bool isDownloading = true;
        DownloadProgress progressWindow;
        bool div0 = false;

        public async Task DownloadFileSegment(DownloadSegmentID id, DownloadProgress window, Stream downloadStream, Stream outputStream, long contentLength, long totalLength, string location, BetterProgressBar progressBar, CancellationToken token, DownloadProgressUpdater progressUpdater)
        {
            Action<BetterProgressBar, long, long, long> progressCallback = new Action<BetterProgressBar, long, long, long>(SegmentProgressCallback);
            progressWindow = window;

            byte[] buffer = new byte[8 * 1024];
            int len;
            long totalRead = 0;

            while ((len = await downloadStream.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false)) > 0)
            {
                await outputStream.WriteAsync(buffer, 0, len).ConfigureAwait(false);
                totalRead += len;
                progressCallback.Invoke(progressBar, totalRead, contentLength, totalLength);

                switch (id)
                {
                    case DownloadSegmentID.Segment0:
                        progressUpdater.read0 = totalRead;
                        progressUpdater.progress0 = (int)((double)totalRead / (double)contentLength * (double)100);
                        break;
                    case DownloadSegmentID.Segment1:
                        progressUpdater.read1 = totalRead;
                        progressUpdater.progress1 = (int)((double)totalRead / (double)contentLength * (double)100);
                        break;
                }
            }

            if (progressCallback != null && totalLength != -1)
            {
                //Debug.Assert(totalRead == totalLength);
            }

            await outputStream.FlushAsync().ConfigureAwait(false);
            outputStream.Close();
            isDownloading = false;
        }

        private void SegmentProgressCallback(BetterProgressBar progressBar, long totalRead, long contentLength, long totalLength)
        {
            if (!div0 && contentLength == 0)
            {
                DarkMessageBox errMsg = new DarkMessageBox($"contentLength is 0, this will cause a DivideByZeroException.\nThe file will still download but progress reporting will be unavailable.\nDebug Information:\ntotalRead = {totalRead}\ncontentLength = {contentLength}", "Download Manager - Progress Callback", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                errMsg.ShowDialog();
                div0 = true;
                return;
            }
            else if (div0)
            {
                return;
            }

            double percentageDone = (double)totalRead / (double)contentLength * (double)100; // number (36561) / number (524289) * 100 = 0???????

            /*try
            {
                percentageDone = totalRead / contentLength * 100;
            }
            catch (Exception ex)
            {
                DarkMessageBox errMsg = new DarkMessageBox($"{ex.Message} ({ex.GetType().FullName})\nGot Value: {percentageDone}\nOther Values:\ntotalRead: {totalRead}\ncontentLength: {contentLength}\n{ex.StackTrace}", "Download Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
                errMsg.ShowDialog();
            }*/


            progressWindow.Invoke(new MethodInvoker(delegate
            {
                progressBar.Minimum = 0;
                progressBar.Maximum = 100;
                progressBar.Value = (int)percentageDone;
            }));
        }

        public enum DownloadSegmentID
        {
            Segment0,
            Segment1
        }
    }
}
