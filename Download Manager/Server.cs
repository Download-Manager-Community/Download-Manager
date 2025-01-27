using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using static DownloadManager.Logging;

namespace DownloadManager
{
    internal class Server
    {
        public Server _instance;
        public Socket httpServer;
        private int serverPort = Settings.Default.serverPort;
        public Thread thread;
        private int failureCount = 0;

        public void StartServer()
        {
            _instance = this;
            Log(LogLevel.Info, "Starting internal server...");
            try
            {
                thread = new Thread(new ThreadStart(ConnectionThreadMethod));
                thread.Start();
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, "Error while starting internal server:" + Environment.NewLine + ex.Message + ex.StackTrace);
            }
        }

        private void ConnectionThreadMethod()
        {
            try
            {
                httpServer = new Socket(SocketType.Stream, ProtocolType.Tcp);
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, serverPort);
                httpServer.Bind(endPoint);
                httpServer.Listen(1);
                ListenForConnections();
            }
            catch (Exception ex)
            {
                Log(LogLevel.Error, "Error while starting internal server:" + Environment.NewLine + ex.Message + ex.StackTrace);
            }
        }

        private void ListenForConnections()
        {
            while (true)
            {
                if (failureCount >= Settings.Default.maxServerFailureCount)
                {
                    Log(LogLevel.Warning, "The internal server has failed to start, some functionality of Download Manager may be limited.");
                    DarkMessageBox msg = new DarkMessageBox("Download Manager encountered an error while attempting to start the internal server multiple times and has stopped attempting to prevent unexpected behaviour.\nSome functionality may not be available such as browser integration.\nPlease check your firewall and ports settings.\nFor more information, check the debug logs.", "Download Manager Internal Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    msg.ShowDialog();
                    break;
                }

                string data = "";
                byte[] bytes = new byte[2048];
                Socket? client = null;

                try
                {
                    Log(LogLevel.Info, "Internal server ready. Listening for connections on port " + serverPort + ".");
                    client = httpServer.Accept();

                }
                catch (Exception ex)
                {
                    Log(LogLevel.Error, "Error while starting internal server:" + Environment.NewLine + ex.Message + ex.StackTrace);
                    failureCount++;
                    continue;
                }

                Log(LogLevel.Debug, "Reading inbound connection data.");

                bool timeOut = false;
                bool nullBytes = false;

                // Read inbound connection data
                while (true)
                {
                    if (client.Poll(1000, SelectMode.SelectWrite) == false)
                    {
                        Log(LogLevel.Warning, "No data is available to be read and will be ignored.");
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
                                Log(LogLevel.Warning, "The connection was cancelled by the browser!");
                            }
                            else
                            {
                                Log(LogLevel.Error, ex.Message + " (" + ex.GetType() + ")" + Environment.NewLine + ex.StackTrace);
                            }
                        }
                        catch (Exception ex)
                        {
                            Log(LogLevel.Error, ex.Message + " (" + ex.GetType() + ")" + Environment.NewLine + ex.StackTrace);
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
                        Log(LogLevel.Warning, "Download Manager timed-out while reading inbound connection data. The connection will be ignored.");
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

                Log(LogLevel.Debug, "Finished reading inbound connection data.");

                if (timeOut)
                {
                    Log(LogLevel.Error, "Failed to read inbound connection data. Receiving data timed-out!");
                    client.Close();
                    bytes = null;
                    continue;
                }

                if (nullBytes)
                {
                    Log(LogLevel.Error, "Failed to read inbound connection data. The number of bytes was null!");
                    client.Close();
                    bytes = null;
                    continue;
                }

                string[] splittedData = data.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedData.Length <= 0)
                {
                    Log(LogLevel.Warning, "Received empty request. Ignoring...");
                    continue;
                }

                // TOOD: Temp debug logging
                Log(LogLevel.Debug, "--- Start Request ---");
                Log(LogLevel.Debug, data);
                Log(LogLevel.Debug, "--- End Request ---");

                string url = splittedData[0].ToString().Replace("%22 HTTP/1.1", "");

                Log(LogLevel.Debug, url);

                if (url.EndsWith(" HTTP/1.1"))
                {
                    url = url.Remove(url.Length - 9, 9);
                }

                Log(LogLevel.Debug, url);

                if (url.StartsWith("GET /?url=%22") || url.StartsWith("GET /?url="))
                {
                    url = url.Replace("GET /?url=%22", "");

                    Regex ytRegex = new Regex(@"^(https?\:\/\/)?((www?\.|m?\.)?youtube\.com|^(https?\:\/\/)?(www?\.)?youtu\.be)\/.+$");

                    if (ytRegex.IsMatch(url))
                    {
                        if (!url.Contains("favicon.ico"))
                        {
                            DownloadForm._instance.Invoke((MethodInvoker)delegate
                            {
                                Log(LogLevel.Info, "Request received for URL: " + url);
                                Log(LogLevel.Warning, "URL is a YouTube link. Which is unsupported by Download Manager!");

                                DarkMessageBox msg = new DarkMessageBox("Download Manager does not support downloading YouTube videos.\nFor more information, check the release notes for version 6.0.0.0 at:\nhttps://github.com/Download-Manager-Community/Download-Manager/releases/tag/6.0.0.0", "Unsupported", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                msg.ShowDialog();
                            });
                        }
                    }
                    else
                    {
                        if (!url.Contains("favicon.ico"))
                        {
                            DownloadForm._instance.Invoke((MethodInvoker)delegate
                            {
                                Log(LogLevel.Warning, "Request received for URL: " + url);
                                if (Settings.Default.downloadHistory.Contains(url) == false)
                                {
                                    DownloadForm._instance.textBox1.Items.Add(url);
                                    Settings.Default.downloadHistory.Add(url);
                                    Settings.Default.Save();
                                }
                                DownloadProgress downloadProgress = new DownloadProgress(url, Settings.Default.defaultDownload, "", 0);
                                downloadProgress.Show();

                                DownloadForm.downloadsList.Add(downloadProgress);
                                DownloadForm.currentDownloads.RefreshList();
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
                        Log(LogLevel.Info, "Request received to show download window.");
                        DownloadForm._instance.Invoke((MethodInvoker)delegate
                        {
                            DownloadForm._instance.Show();
                            DownloadForm._instance.Focus();
                        });
                    }
                    else if (url.Contains("False"))
                    {
                        Log(LogLevel.Info, "Request received to hide download window.");
                        DownloadForm._instance.Invoke((MethodInvoker)delegate
                        {
                            DownloadForm._instance.Hide();
                        });
                    }
                    else
                    {
                        Log(LogLevel.Warning, "Malformed request received to show/hide download window. Ignoring...");
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
