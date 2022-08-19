﻿using System.ComponentModel;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using static DownloadManager.Logging;

namespace DownloadManager
{
    public partial class DownloadProgress : Form
    {
        #region DLL Import
        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);

        protected override void OnHandleCreated(EventArgs e)
        {
            if (DwmSetWindowAttribute(Handle, 19, new[] { 1 }, 4) != 0)
                DwmSetWindowAttribute(Handle, 20, new[] { 1 }, 4);
        }
        #endregion

        string url;
        string location;
        string fileName;
        string hash;
        int hashType = 0;
        bool isUrlInvalid = false;
        bool downloading = true;
        bool doFileVerify = false;
        WebClient client = new WebClient();

        public DownloadProgress(string urlArg, string locationArg, string hashArg, int hashTypeArg)
        {
            InitializeComponent();
            client.Headers.Add("Cache-Control", "no-cache");
            client.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            hashType = hashTypeArg;
            hashType += 1;
            hash = hashArg;
            if (hash != "")
            {
                textBox2.Text = hash;
                doFileVerify = true;
                this.Size = new System.Drawing.Size(651, 212);
            }
            else
            {
                doFileVerify = false;
                textBox2.Visible = false;
                label5.Visible = false;
                this.Size = new System.Drawing.Size(651, 183);
            }
            DownloadForm.downloadsAmount += 1;
            textBox1.Text = urlArg;
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
            checkBox2.Checked = Settings1.Default.closeOnComplete;
            checkBox1.Checked = Settings1.Default.keepOnTop;
            progressBar1.Style = ProgressBarStyle.Marquee;
            Thread thread = new Thread(() =>
            {
                try
                {
                    client.OpenRead(url);
                }
                catch (Exception ex)
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        progressBar1.Style = ProgressBarStyle.Blocks;
                        progressBar1.Value = 100;
                        ProgressBarColor.SetState(progressBar1, 2);
                    }));

                    MessageBox.Show(ex.Message, "Download Manager - Error Fetching File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    downloading = false;
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

                Uri uri = new Uri(url);

                fileName = System.IO.Path.GetFileName(uri.AbsolutePath);
                Action action1 = () => progressBar1.Style = ProgressBarStyle.Blocks;
                this.Invoke(action1);
                Log("Downloading file " + uri + " to " + location + fileName, Color.White);
                try
                {
                    client.DownloadFileAsync(uri, location + fileName);
                }
                catch (Exception ex)
                {
                    Invoke(new MethodInvoker(delegate
                    {
                        progressBar1.Style = ProgressBarStyle.Blocks;
                        progressBar1.Value = 100;
                        ProgressBarColor.SetState(progressBar1, 2);
                    }));

                    Log(ex.Message, Color.Red);
                    MessageBox.Show(ex.Message, "Download Manager - Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    downloading = false;
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
                                Log(ex.Message, Color.Red);
                                Invoke(new MethodInvoker(delegate
                                {
                                    ProgressBarColor.SetState(progressBar1, 2);
                                }));
                                MessageBox.Show(ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                downloading = false;
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
                    if (this.IsDisposed == false)
                    {
                        try
                        {
                            Invoke(new MethodInvoker(delegate
                            {
                                progressBar1.Style = ProgressBarStyle.Blocks;
                                progressBar1.Value = 100;
                                ProgressBarColor.SetState(progressBar1, 2);
                            }));
                        }
                        catch { }
                    }
                    Log(ex.Message + Environment.NewLine + ex.StackTrace, Color.Red);
                }
            }
        }

        private void Client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (this.IsHandleCreated)
            {
                try
                {
                    Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Style = ProgressBarStyle.Marquee;
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
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                button3.Enabled = false;
                                                progressBar1.Value = 100;
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                if (checkBox2.Checked == true)
                                                {
                                                    client.Dispose();
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
                                                progressBar1.Value = 100;
                                                ProgressBarColor.SetState(progressBar1, 2);
                                            }));
                                            Log("Failed to verify file. The file will be re-downloaded.", Color.Red);
                                            DownloadForm.downloadsAmount -= 1;
                                            hashType -= 1;
                                            DownloadProgress download = new DownloadProgress(url, location, hash, hashType);
                                            download.Show();
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                client.CancelAsync();
                                                client.Dispose();
                                                this.Close();
                                                this.Dispose();
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
                                            Log("Finished downloading file.", Color.White);
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                button3.Enabled = false;
                                                progressBar1.Value = 100;
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                if (checkBox2.Checked == true)
                                                {
                                                    client.Dispose();
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
                                                progressBar1.Value = 100;
                                                ProgressBarColor.SetState(progressBar1, 2);
                                            }));
                                            Log("Failed to verify file. The file will be re-downloaded.", Color.Red);
                                            DownloadForm.downloadsAmount -= 1;
                                            hashType -= 1;
                                            DownloadProgress download = new DownloadProgress(url, location, hash, hashType);
                                            download.Show();
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                client.CancelAsync();
                                                client.Dispose();
                                                this.Close();
                                                this.Dispose();
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
                                            Log("Finished downloading file.", Color.White);
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                button3.Enabled = false;
                                                progressBar1.Value = 100;
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                if (checkBox2.Checked == true)
                                                {
                                                    client.Dispose();
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
                                                progressBar1.Value = 100;
                                                ProgressBarColor.SetState(progressBar1, 2);
                                            }));
                                            Log("Failed to verify file. The file will be re-downloaded.", Color.Red);
                                            DownloadForm.downloadsAmount -= 1;
                                            hashType -= 1;
                                            DownloadProgress download = new DownloadProgress(url, location, hash, hashType);
                                            download.Show();
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                client.CancelAsync();
                                                client.Dispose();
                                                this.Close();
                                                this.Dispose();
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
                                            Log("Finished downloading file.", Color.White);
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                button3.Enabled = false;
                                                progressBar1.Value = 100;
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                if (checkBox2.Checked == true)
                                                {
                                                    client.Dispose();
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
                                                progressBar1.Value = 100;
                                                ProgressBarColor.SetState(progressBar1, 2);
                                            }));
                                            Log("Failed to verify file. The file will be re-downloaded.", Color.Red);
                                            DownloadForm.downloadsAmount -= 1;
                                            hashType -= 1;
                                            DownloadProgress download = new DownloadProgress(url, location, hash, hashType);
                                            download.Show();
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                client.CancelAsync();
                                                client.Dispose();
                                                this.Close();
                                                this.Dispose();
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
                                            Log("Finished downloading file.", Color.White);
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                checkBox2.Enabled = false;
                                                button3.Enabled = false;
                                                progressBar1.Value = 100;
                                                progressBar1.Style = ProgressBarStyle.Blocks;
                                                if (checkBox2.Checked == true)
                                                {
                                                    client.Dispose();
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
                                                progressBar1.Value = 100;
                                                ProgressBarColor.SetState(progressBar1, 2);
                                            }));
                                            Log("Failed to verify file. The file will be re-downloaded.", Color.Red);
                                            DownloadForm.downloadsAmount -= 1;
                                            hashType -= 1;
                                            DownloadProgress download = new DownloadProgress(url, location, hash, hashType);
                                            download.Show();
                                            Invoke(new MethodInvoker(delegate ()
                                            {
                                                client.CancelAsync();
                                                client.Dispose();
                                                this.Close();
                                                this.Dispose();
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
                                            progressBar1.Value = 100;
                                            ProgressBarColor.SetState(progressBar1, 2);
                                        }));
                                        Log("Invalid hash type '" + hashType + "'. The file could not be verified.", Color.Red);
                                        MessageBox.Show("Invalid hash type '" + hashType + "'. The file could not be verified.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                        Invoke(new MethodInvoker(delegate ()
                                        {
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
                            else
                            {
                                downloading = false;
                                DownloadForm.downloadsAmount -= 1;
                                Log("Finished downloading file.", Color.White);
                                Invoke(new MethodInvoker(delegate ()
                                {
                                    checkBox2.Enabled = false;
                                    button3.Enabled = false;
                                    progressBar1.Value = 100;
                                    progressBar1.Style = ProgressBarStyle.Blocks;
                                    if (checkBox2.Checked == true)
                                    {
                                        client.Dispose();
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
                                progressBar1.Value = 100;
                                ProgressBarColor.SetState(progressBar1, 2);
                            }));

                            Log("Download failed.", Color.Red);
                            Invoke(new MethodInvoker(delegate ()
                            {
                                progressBar1.Value = 0;
                                client.CancelAsync();
                                client.Dispose();
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
                        progressBar1.Value = 100;
                        ProgressBarColor.SetState(progressBar1, 2);
                    }));

                    Log(ex.Message + Environment.NewLine + ex.StackTrace, Color.Red);
                    MessageBox.Show(ex.Message, "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void button1_Click(object sender, EventArgs e)
        {
            // Close
            if (!downloading)
            {
                client.Dispose();
                this.Close();
                this.Dispose();
            }
            else
            {
                DialogResult result = MessageBox.Show("Are you sure you want to cancel the download?", "Download Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    downloading = false;
                    DownloadForm.downloadsAmount -= 1;
                    client.CancelAsync();
                    client.Dispose();
                    this.Close();
                    this.Dispose();
                }
            }
        }

        private void DownloadProgress_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (downloading == true)
            {
                DialogResult result = MessageBox.Show("Are you sure you want to cancel the download?", "Download Manager", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    DownloadForm.downloadsAmount -= 1;
                    client.CancelAsync();
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }
            }
            else
            {
                e.Cancel = false;
            }
        }
    }

    public static class ProgressBarColor
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr w, IntPtr l);
        public static void SetState(this ProgressBar p, int state)
        {
            SendMessage(p.Handle, 1040, (IntPtr)state, IntPtr.Zero);
        }
    }
}
