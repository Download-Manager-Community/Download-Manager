using DownloadManager.Components;
using DownloadManager.NativeMethods;
using System.Security.Cryptography;
using System.Text;

namespace DownloadManager
{
    public partial class HashCalculator : Form
    {
        // Initialise variables
        private HashCalculator _instance;
        private string file;

        public HashCalculator(string file)
        {
            // Initialise form
            _instance = this;
            InitializeComponent();

            // Set window style
            DesktopWindowManager.SetWindowStyle(this.Handle);

            // Set file text box to file path
            fileNameBox.Text = file;

            // Set file path variable
            this.file = file;

            // If the file exists, calculate the hashes
            if (File.Exists(file))
            {
                Thread thread = new Thread(() =>
                {
                    // Wait for 1 second
                    Thread.Sleep(1000);

                    // Show progress bar
                    ShowProgressBar(true);

                    // MD5
                    using (var md5 = MD5.Create())
                        Calculate(md5, md5Box);

                    // SHA-1
                    using (var hash = SHA1.Create())
                        Calculate(hash, sha1Box);

                    // SHA-256
                    using (var hash = SHA256.Create())
                        Calculate(hash, sha256Box);

                    // SHA-384
                    using (var hash = SHA384.Create())
                        Calculate(hash, sha384Box);

                    // SHA-512
                    using (var hash = SHA512.Create())
                        Calculate(hash, sha512Box);

                    // Hide progress bar
                    ShowProgressBar(false);
                });
                thread.Start();
            }
            else
            {
                // If the file does not exist, show an error message and close the form
                DarkMessageBox.Show("File not found!", "Download Manager - Error", MessageBoxButtons.OK, MessageBoxIcon.Error, true);
                this.Close();
            }
        }

        private void Calculate(HashAlgorithm hash, TextBox textBox)
        {
            try
            {
                // Initialise variables
                byte[] hashBytes;

                // Open file stream
                using (var stream = File.OpenRead(file))
                {
                    // Calculate hash and close the stream
                    hashBytes = hash.ComputeHash(stream);
                    stream.Close();
                }

                // Convert hash bytes to string
                StringBuilder result = new StringBuilder(hashBytes.Length * 2);

                // Append each byte to the string
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    result.Append(hashBytes[i].ToString(false ? "X2" : "x2"));
                }

                // Write to the hash text box
                Action action = () => textBox.Text = result.ToString();
                if (this.IsHandleCreated) { this.Invoke(action); }
            }
            catch (Exception ex)
            {
                // If an error occurs, show the error message in the hash text box
                Action action = () => textBox.Text = ex.Message;
                if (this.IsHandleCreated) { this.Invoke(action); }
            }
        }

        internal void ShowProgressBar(bool show)
        {
            // If the form handle is created
            if (this.IsHandleCreated)
            {
                // Invoke the method (on the UI thread)
                Invoke(new MethodInvoker(delegate ()
                {
                    if (show)
                    {
                        // Show progress bar and resize form (larger)
                        progressBar.Visible = true;
                        this.Size = new System.Drawing.Size(664, 465);
                    }
                    else
                    {
                        // Hide progress bar and resize form (smaller)
                        progressBar.Visible = false;
                        this.Size = new System.Drawing.Size(664, 454);
                    }
                }));
            }
        }
    }
}
