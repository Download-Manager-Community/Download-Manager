using Google.Apis.Services;
using Google.Apis.YouTube.v3;
using Google.Apis.YouTube.v3.Data;
using System.Net;

namespace DownloadManager.YouTube
{
    internal class RequestHandler
    {
        public string endpointUrl = "https://www.youtube.com/youtubei/v1/player?key=AIzaSyA8eiZmM1FaDVjRy-df2KTyQ_vz_yYM39w"; // POST

        public static async Task<VideoListResponse> Request(string videoId)
        {
            // Initialize your YouTube service (use your own API key)
            var youtubeService = new YouTubeService(new BaseClientService.Initializer
            {
                ApiKey = "AIzaSyBvxKINt-quHeurMzQA4V-MCGRLAbPWODs",
                ApplicationName = "Download Manager"
            });

            // Call the API to get video details
            var videoRequest = youtubeService.Videos.List("snippet,statistics");
            videoRequest.Id = videoId;
            var videoResponse = videoRequest.Execute();

            return videoResponse;
        }

        public static async Task<string> PostRequest(string url, string requestBody)
        {
            var request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = requestBody.Length;
            request.Headers.Add("User-Agent", "com.google.android.youtube/17.36.4 (Linux; U; Android 12; GB) gzip");

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(requestBody);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = (HttpWebResponse)request.GetResponse();

            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }

        /*public static async Task<object> GetVideoDownload(string videoUrl)
        {
            var uri = new Uri(videoUrl);
            var query = HttpUtility.ParseQueryString(uri.Query);
            var id = query.Get("v");
            var dt = new WebClient().DownloadString($"https://www.youtube.com/get_video_info?video_id={id}&el=embedded&ps=default&eurl=&gl=US&hl=en");

            if (dt.Contains("status=fail"))
            {
                var parameters = HttpUtility.ParseQueryString(dt);
                var errorObj = new Dictionary<string, string>
                {
                    {"error", "true"}
                };

                foreach (var key in parameters.AllKeys)
                {
                    errorObj[key] = parameters[key];
                }

                return errorObj;
            }
            else
            {
                var parameters = HttpUtility.ParseQueryString(dt);
                var streams = parameters.Get("url_encoded_fmt_stream_map").Split(',');
                var streamList = new List<Dictionary<string, string>>();

                foreach (var stream in streams)
                {
                    var streamParameters = HttpUtility.ParseQueryString(stream);
                    var streamObj = new Dictionary<string, string>();

                    foreach (var key in streamParameters.AllKeys)
                    {
                        streamObj[key] = streamParameters[key];
                    }

                    streamList.Add(streamObj);
                }

                return streamList;
            }
        }*/
    }
}
