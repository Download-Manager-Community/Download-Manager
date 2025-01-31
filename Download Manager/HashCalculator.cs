using DownloadManager.NativeMethods;
using System.Security.Cryptography;
using System.Text;

namespace DownloadManager
{
    public partial class HashCalculator : Form
    {
        HashCalculator _instance;

        public HashCalculator(string file)
        {
            _instance = this;
            InitializeComponent();

            DesktopWindowManager.SetImmersiveDarkMode(this.Handle, true);
            DesktopWindowManager.EnableMicaIfSupported(this.Handle);
            DesktopWindowManager.ExtendFrameIntoClientArea(this.Handle);

            this.Size = new System.Drawing.Size(664, 465);
            fileNameBox.Text = file;
            if (File.Exists(file))
            {
                Thread thread = new Thread(() =>
                {
                    Thread.Sleep(1000);
                    ShowProgressBar(true);
                    // MD5
                    try
                    {
                        byte[] myHash;
                        using (var md5 = MD5.Create())
                        using (var stream = File.OpenRead(file))
                        {
                            myHash = md5.ComputeHash(stream);
                            stream.Close();
                        }

                        StringBuilder result = new StringBuilder(myHash.Length * 2);

                        for (int i = 0; i < myHash.Length; i++)
                        {
                            result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                        }
                        Action action = () => md5Box.Text = result.ToString();
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }
                    catch (Exception ex)
                    {
                        Action action = () => md5Box.Text = ex.Message;
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }

                    // SHA-1
                    try
                    {
                        byte[] myHash;
                        using (var hash = SHA1.Create())
                        using (var stream = File.OpenRead(file))
                        {
                            myHash = hash.ComputeHash(stream);
                            stream.Close();
                        }

                        StringBuilder result = new StringBuilder(myHash.Length * 2);

                        for (int i = 0; i < myHash.Length; i++)
                        {
                            result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                        }
                        Action action = () => sha1Box.Text = result.ToString();
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }
                    catch (Exception ex)
                    {
                        Action action = () => sha1Box.Text = ex.Message;
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }

                    // SHA-256
                    try
                    {
                        byte[] myHash;
                        using (var hash = SHA256.Create())
                        using (var stream = File.OpenRead(file))
                        {
                            myHash = hash.ComputeHash(stream);
                            stream.Close();
                        }

                        StringBuilder result = new StringBuilder(myHash.Length * 2);

                        for (int i = 0; i < myHash.Length; i++)
                        {
                            result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                        }
                        Action action = () => sha256Box.Text = result.ToString();
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }
                    catch (Exception ex)
                    {
                        Action action = () => sha256Box.Text = ex.Message;
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }

                    // SHA-384
                    try
                    {
                        byte[] myHash;
                        using (var hash = SHA384.Create())
                        using (var stream = File.OpenRead(file))
                        {
                            myHash = hash.ComputeHash(stream);
                            stream.Close();
                        }

                        StringBuilder result = new StringBuilder(myHash.Length * 2);

                        for (int i = 0; i < myHash.Length; i++)
                        {
                            result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                        }
                        Action action = () => sha384Box.Text = result.ToString();
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }
                    catch (Exception ex)
                    {
                        Action action = () => sha384Box.Text = ex.Message;
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }

                    // SHA-512
                    try
                    {
                        byte[] myHash;
                        using (var hash = SHA512.Create())
                        using (var stream = File.OpenRead(file))
                        {
                            myHash = hash.ComputeHash(stream);
                            stream.Close();
                        }

                        StringBuilder result = new StringBuilder(myHash.Length * 2);

                        for (int i = 0; i < myHash.Length; i++)
                        {
                            result.Append(myHash[i].ToString(false ? "X2" : "x2"));
                        }
                        Action action = () => sha512Box.Text = result.ToString();
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }
                    catch (Exception ex)
                    {
                        Action action = () => sha512Box.Text = ex.Message;
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }
                    ShowProgressBar(false);
                });
                thread.Start();
            }
            else
            {
                DarkMessageBox msg = new DarkMessageBox("File not found!", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                msg.ShowDialog();
                this.Close();
            }
        }

        internal void ShowProgressBar(bool show)
        {
            if (this.IsHandleCreated)
            {
                Invoke(new MethodInvoker(delegate ()
                {
                    if (show)
                    {
                        progressBar.Visible = true;
                        this.Size = new System.Drawing.Size(664, 465);
                    }
                    else
                    {
                        progressBar.Visible = false;
                        this.Size = new System.Drawing.Size(664, 454);
                    }
                }));
            }
        }
    }
}
