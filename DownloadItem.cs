using Timer = System.Windows.Forms.Timer;

namespace DownloadManager
{
    public class DownloadItem
    {
        CurrentDownloads downloadsForm = CurrentDownloads._instance;
        Timer timer = new Timer();
        DownloadProgress progress;
        GroupBox groupBox;

        #region Predefined Controls
        ProgressBar progressBar = new ProgressBar()
        {
            Location = new Point(290, 43),
            Size = new Size(121, 23),
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 1,
        };

        Label title1 = new Label()
        {
            Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            Location = new Point(6, 19),
            Text = "Downloading:"
        };
        Label fileNameLabel = new Label()
        {
            Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            Location = new Point(92, 19),
            Text = "fileName"
        };
        Label title2 = new Label()
        {
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            Location = new Point(6, 38),
            Text = "Download URL:"
        };
        Label fileUrlLabel = new Label()
        {
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            Location = new Point(97, 38),
            Text = "fileUrl"
        };
        Label title3 = new Label()
        {
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            Location = new Point(6, 57),
            Text = "Download Size:"
        };
        Label fileSizeLabel = new Label()
        {
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            Location = new Point(96, 57),
            Text = "0 Bytes"
        };
        Label title4 = new Label()
        {
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            Location = new Point(6, 77),
            Text = "Download Progress:"
        };
        Label fileProgressLabel = new Label()
        {
            Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0),
            ForeColor = Color.White,
            Location = new Point(121, 77),
            Text = "0%"
        };
        #endregion

        public void Initialize(DownloadProgress progress)
        {
            this.progress = progress;

            this.groupBox = new GroupBox()
            {
                Size = new Size(428, 105),
                Location = new Point(12, CurrentDownloads.nextY),
                Text = ""
            };

            downloadsForm.Controls.Add(groupBox);

            groupBox.Controls.Add(progressBar);
            groupBox.Controls.Add(title1);
            groupBox.Controls.Add(fileNameLabel);
            groupBox.Controls.Add(title2);
            groupBox.Controls.Add(fileUrlLabel);
            groupBox.Controls.Add(title3);
            groupBox.Controls.Add(fileSizeLabel);
            groupBox.Controls.Add(title4);
            groupBox.Controls.Add(fileProgressLabel);

            timer.Tick += UpdateTimer_Tick;
            timer.Interval = 500;
            timer.Start();
        }

        private void UpdateTimer_Tick(object? sender, EventArgs e)
        {
            if (progress == null || progress.downloading == false)
            {
                // If the download is finished, dispose the item
                Dispose();
            }
            else
            {
                // Update the progress bar
                progressBar.Style = ProgressBarStyle.Blocks;
                progressBar.Value = (int)progress.percentageDone;

                // Update the labels
                fileNameLabel.Text = progress.fileName;
                fileUrlLabel.Text = progress.url;
                fileSizeLabel.Text = progress.fileSize + " Bytes";
                fileProgressLabel.Text = progress.percentageDone.ToString() + "%";
            }
        }

        public void Dispose()
        {
            downloadsForm.itemList.Remove(this);

            timer.Stop();
            timer.Dispose();

            downloadsForm.Controls.Remove(groupBox);
            groupBox.Dispose();
            progressBar.Dispose();
            title1.Dispose();
            fileNameLabel.Dispose();
            title2.Dispose();
            fileUrlLabel.Dispose();
            title3.Dispose();
            fileSizeLabel.Dispose();
            title4.Dispose();
            fileProgressLabel.Dispose();
        }

        public void DisposeNoRemove()
        {
            timer.Stop();
            timer.Dispose();

            downloadsForm.Controls.Remove(groupBox);
            groupBox.Dispose();
            progressBar.Dispose();
            title1.Dispose();
            fileNameLabel.Dispose();
            title2.Dispose();
            fileUrlLabel.Dispose();
            title3.Dispose();
            fileSizeLabel.Dispose();
            title4.Dispose();
            fileProgressLabel.Dispose();
        }
    }
}
