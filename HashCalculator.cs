using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace DownloadManager
{
    public partial class HashCalculator : Form
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

        HashCalculator _instance;

        public HashCalculator(string file)
        {
            _instance = this;
            InitializeComponent();
            this.Size = new System.Drawing.Size(478, 400);
            textBox1.Text = file;
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
                        Action action = () => textBox2.Text = result.ToString();
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }
                    catch (Exception ex)
                    {
                        Action action = () => textBox2.Text = ex.Message;
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
                        Action action = () => textBox3.Text = result.ToString();
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }
                    catch (Exception ex)
                    {
                        Action action = () => textBox3.Text = ex.Message;
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
                        Action action = () => textBox4.Text = result.ToString();
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }
                    catch (Exception ex)
                    {
                        Action action = () => textBox4.Text = ex.Message;
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
                        Action action = () => textBox6.Text = result.ToString();
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }
                    catch (Exception ex)
                    {
                        Action action = () => textBox6.Text = ex.Message;
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
                        Action action = () => textBox5.Text = result.ToString();
                        if (this.IsHandleCreated) { _instance.Invoke(action); }
                    }
                    catch (Exception ex)
                    {
                        Action action = () => textBox5.Text = ex.Message;
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
            if (show)
            {
                if (this.IsHandleCreated)
                {
                    Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Visible = true;
                        this.Size = new System.Drawing.Size(478, 400);
                    }));
                }
            }
            else
            {
                if (this.IsHandleCreated)
                {
                    Invoke(new MethodInvoker(delegate ()
                    {
                        progressBar1.Visible = false;
                        this.Size = new System.Drawing.Size(478, 390);
                    }));
                }
            }
        }
    }
}
