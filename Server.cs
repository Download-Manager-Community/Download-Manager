using System.Net;
using System.Net.Sockets;
using System.Text;
using static DownloadManager.DownloadProgress;
using static DownloadManager.Logging;

namespace DownloadManager
{
    internal class Server
    {
        public Server _instance;
        public Socket httpServer;
        private int serverPort = Settings.Default.serverPort;
        public Thread thread;

        public void StartServer()
        {
            _instance = this;
            Log("Starting internal server...", Color.White);
            try
            {
                thread = new Thread(new ThreadStart(ConnectionThreadMethod));
                thread.Start();
            }
            catch (Exception ex)
            {
                Log("Error while starting internal server:" + Environment.NewLine + ex.Message + ex.StackTrace, Color.Red);
            }
        }

        private void ConnectionThreadMethod()
        {
            httpServer = new Socket(SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, serverPort);
            httpServer.Bind(endPoint);
            httpServer.Listen(1);
            ListenForConnections();
        }

        private void ListenForConnections()
        {
            while (true)
            {
                string data = "";
                byte[] bytes = new byte[2048];

                Log("Internal server ready. Listening for connections on port " + serverPort + ".", Color.Green);
                Socket client = httpServer.Accept();

                Log("Reading inbound connection data.", Color.Gray);

                bool timeOut = false;
                bool nullBytes = false;

                // Read inbound connection data
                while (true)
                {
                    if (client.Poll(1000, SelectMode.SelectWrite) == false)
                    {
                        Log("No data is available to be read and will be ignored.", Color.Orange);
                        break;
                    }
                    // Use a 5 second timeout
                    int? numBytes = null;

                    Thread thread = new Thread(new ThreadStart(delegate ()
                    {
                        try
                        {
                            numBytes = client.Receive(bytes);
                        }
                        catch (SocketException ex)
                        {
                            if (ex.Message == "An established connection was aborted by the software in your host machine.")
                            {
                                Log("The connection was canceled by the browser!", Color.Orange);
                            }
                            else
                            {
                                Log(ex.Message + " (" + ex.GetType() + ")" + Environment.NewLine + ex.StackTrace, Color.Red);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log(ex.Message + " (" + ex.GetType() + ")" + Environment.NewLine + ex.StackTrace, Color.Red);
                        }
                    }));
                    thread.Start();
                    if (!thread.Join(TimeSpan.FromSeconds(2)))
                    {
                        try
                        {
                            thread.Abort();
                        }
                        catch { }
                        Log("Download Manager timed-out while reading inbound connection data. The connection will be ignored.", Color.Orange);
                        timeOut = true;
                        break;
                    }

                    if (numBytes == null)
                    {
                        nullBytes = true;
                        break;
                    }

                    data += Encoding.ASCII.GetString(bytes, 0, Convert.ToInt32(numBytes));

                    if (data.IndexOf("\r\n") > -1 || data == "")
                    {
                        break;
                    }
                }

                Log("Finished reading inbound connection data.", Color.Gray);

                if (timeOut)
                {
                    Log("Failed to read inbound connection data. Receiving data timed-out!", Color.Red);
                    client.Close();
                    bytes = null;
                    continue;
                }

                if (nullBytes)
                {
                    Log("Failed to read inbound connection data. The number of bytes was null!", Color.Red);
                    client.Close();
                    bytes = null;
                    continue;
                }

                string[] splittedData = data.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedData.Length <= 0)
                {
                    Log("Received empty request. Ignoring...", Color.Orange);
                    continue;
                }
                string url = splittedData[0].ToString().Replace("%22 HTTP/1.1", "");

                if (url.StartsWith("GET /?url=%22"))
                {
                    if (url.EndsWith("?ytdownload=True"))
                    {
                        url = url.Replace("GET /?url=%22", "").Replace("?ytdownload=True", "");

                        if (!url.Contains("favicon.ico"))
                        {
                            DownloadForm._instance.Invoke((MethodInvoker)delegate
                            {
                                Log("Request received for URL: " + url, Color.White);
                                Log("URL is a YouTube link. Opening the YouTube Download Form.", Color.White);

                                // Open the YouTube download window
                                YouTubeDownloadForm youtubeDownloadForm = new YouTubeDownloadForm(url, Settings.Default.defaultDownload);
                                youtubeDownloadForm.Show();

                                /*DownloadProgress downloadProgress = new DownloadProgress(url, Settings.Default.defaultDownload, DownloadType.Normal, null, "", 0);
                                downloadProgress.Show();*/
                                //Log("--- Start Request ---", Color.White);
                                //Log(data, Color.White);
                                //Log("--- End Request ---", Color.White);
                            });
                        }
                    }
                    else
                    {
                        url = url.Replace("GET /?url=%22", "");

                        if (!url.Contains("favicon.ico"))
                        {
                            DownloadForm._instance.Invoke((MethodInvoker)delegate
                            {
                                Log("Request received for URL: " + url, Color.White);
                                if (Settings.Default.downloadHistory.Contains(url) == false)
                                {
                                    DownloadForm._instance.textBox1.Items.Add(url);
                                    Settings.Default.downloadHistory.Add(url);
                                    Settings.Default.Save();
                                }
                                DownloadProgress downloadProgress = new DownloadProgress(url, Settings.Default.defaultDownload, DownloadType.Normal, null, "", 0);
                                downloadProgress.Show();
                                //Log("--- Start Request ---", Color.White);
                                //Log(data, Color.White);
                                //Log("--- End Request ---", Color.White);
                            });
                        }
                    }
                }
                else if (url.StartsWith("GET /?show="))
                {
                    url = url.Replace("GET /?show=", "");

                    if (url.Contains("True"))
                    {
                        Log("Request received to show download window.", Color.White);
                        DownloadForm._instance.Invoke((MethodInvoker)delegate
                        {
                            DownloadForm._instance.Show();
                            DownloadForm._instance.Focus();
                        });
                    }
                    else if (url.Contains("False"))
                    {
                        Log("Request received to hide download window.", Color.White);
                        DownloadForm._instance.Invoke((MethodInvoker)delegate
                        {
                            DownloadForm._instance.Hide();
                        });
                    }
                    else
                    {
                        Log("Malformed request received to show/hide download window. Ignoring...", Color.Orange);
                    }

                    if (url.EndsWith("&ref=Instance"))
                    {
                        DarkMessageBox msg = new DarkMessageBox("Only one instance of Download Manager can run at a time!", "Download Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        msg.Show();
                    }
                }

                string resHeader = "HTTP/1.1 200 OK\nServer: DownloadManager_Internal\nContent-Type: text/html; charset: UTF-8\n\n";
                string resBody = "<!DOCTYPE html><html><head><title>Download Manager</title><style>body{background-color: rgb(18, 22, 58);color: white;font-family: Arial, Helvetica, sans-serif;}h1{font-size: 30px;}</style></head><body><h1>Download Manager</h1><hr><p>Please do not manually send requests to the internal server.</p><p>This may cause Download Manager to behave unexpectedly.</p></body></html>";
                string resStr = resHeader + resBody;
                byte[] resData = Encoding.ASCII.GetBytes(resStr);
                client.SendTo(resData, client.RemoteEndPoint);
                client.Close();
                bytes = null;
            }
        }
    }
}
