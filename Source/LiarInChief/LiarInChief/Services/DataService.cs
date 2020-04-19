using LiarInChief.Interfaces;
using LiarInChief.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Essentials;

namespace LiarInChief.Services
{
    public class DataService : IDataService
    {
        private XmlDocument theAssetXmlDocument;
        private XmlDocument trumpIncXmlDocument;
        private readonly HttpClient client;

        public DataService()
        {
            client = new HttpClient();
        }

        public async Task<string> GetBackgroundImage(bool forceRefresh)
        {
            WebClient client = new WebClient();
            string indexFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ImageIndex.txt");
            string fileUrl = "https://raw.githubusercontent.com/kkohler2/LiarInChief/master/Source/LiarInChief/LiarInChief/ImageIndex.txt";
            FileInfo fileInfo = new FileInfo(indexFile);
            if (!fileInfo.Exists || (forceRefresh && (DateTime.Now - fileInfo.LastWriteTime) > new TimeSpan(24, 0, 0)))
            {
                try
                {
                    using (Stream stream = client.OpenRead(fileUrl))
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string data = await reader.ReadToEndAsync();
                            using (StreamWriter writer = new StreamWriter(indexFile))
                            {
                                await writer.WriteAsync(data);
                            }
                        }
                    }
                }
                catch (Exception)
                {

                }
            }
            List<string> files = new List<string>();
            if (!File.Exists(indexFile))
            {
                indexFile = "ImageIndex.txt";
                // Get Assembly
                var assembly = Assembly.GetExecutingAssembly();

                // Get Resources
                var resources = assembly.GetManifestResourceNames();

                // Get File Path from FileName
                string filePath = resources?.Single(resource => resource.EndsWith(indexFile, StringComparison.Ordinal));

                using (var stream = assembly.GetManifestResourceStream(filePath))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string l;
                        while ((l = await reader.ReadLineAsync()) != null)
                        {
                            if (!string.IsNullOrEmpty(l))
                            {
                                files.Add(l);
                            }
                        }
                    }
                }
            }
            else
            { 
                using (StreamReader reader = new StreamReader(indexFile))
                {
                    string l;
                    while ((l = await reader.ReadLineAsync()) != null)
                    {
                        if (!string.IsNullOrEmpty(l))
                        {
                            files.Add(l);
                        }
                    }
                }
            }

            Random random = new Random((int)DateTime.Now.Ticks);
            string imageUrl = files[random.Next(0, files.Count - 1)];
            string format = string.Empty;
            string imageFile = imageUrl;

            if (imageUrl.IndexOf(":") != -1)
            {
                int pos = imageUrl.LastIndexOf("/");
                if (pos != -1)
                {
                    imageFile = imageFile.Substring(pos + 1);
                    pos = imageFile.LastIndexOf("?");
                    if (pos != -1)
                    {
                        int pos2 = imageFile.IndexOf("format=");
                        if (pos2 != -1)
                        {
                            format = imageFile.Substring(pos2 + 7);
                            pos2 = format.IndexOf("&");
                            if (pos2 != -1)
                            {
                                format = format.Substring(0, pos2);
                            }

                        }
                        imageFile = imageFile.Substring(0, pos);
                        if (!string.IsNullOrWhiteSpace(format))
                        {
                            imageFile = $"{imageFile}.{format}";
                        }
                    }
                }
                string image = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), imageFile);
                imageFile = image;
                if (!File.Exists(image))
                {
                    byte[] imageBuffer = null;
                    using (Stream data = client.OpenRead(imageUrl))
                    {
                        using (BinaryReader reader = new BinaryReader(data))
                        {
                            const int bufferSize = 4096;
                            using (var ms = new MemoryStream())
                            {
                                byte[] buffer = new byte[bufferSize];
                                int count;
                                while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                                    ms.Write(buffer, 0, count);
                                imageBuffer = ms.ToArray();
                            }
                        }
                    }
                    using (FileStream imageFileStream = new FileStream(image, FileMode.Create))
                    {
                        using (BinaryWriter writer = new BinaryWriter(imageFileStream))
                        {
                            writer.Write(imageBuffer);
                        }
                    }
                }
            }
            return imageFile;
        }

        private string GetImageFile(XmlDocument xmlDocument)
        {
            string imageUrl = xmlDocument.SelectSingleNode("//rss/channel/image/url")?.InnerText;
            FileInfo imageUrlInfo = new FileInfo(imageUrl);

            string imageFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "theasset." + imageUrlInfo.Extension);
            FileInfo imageFileInfo = new FileInfo(imageFile);
            if (!File.Exists(imageFile))
            {
                WebClient client = new WebClient();
                byte[] imageBuffer = null;
                using (Stream data = client.OpenRead(imageUrl))
                {
                    using (BinaryReader reader = new BinaryReader(data))
                    {
                        const int bufferSize = 4096;
                        using (var ms = new MemoryStream())
                        {
                            byte[] buffer = new byte[bufferSize];
                            int count;
                            while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                                ms.Write(buffer, 0, count);
                            imageBuffer = ms.ToArray();
                        }
                    }
                }
                using (FileStream imageFileStream = new FileStream(imageFileInfo.FullName, FileMode.Create))
                {
                    using (BinaryWriter writer = new BinaryWriter(imageFileStream))
                    {
                        writer.Write(imageBuffer);
                    }
                }
            }
            return new Uri(imageFile).AbsoluteUri.Replace("file://", "");
        }

        public async Task<Podcast> GetTheAssetPodcast(bool forceRefresh)
        {
            XmlDocument xmldoc = await GetTheAssetXmlDocument(forceRefresh);

            var xmlNode = xmldoc.SelectSingleNode("//rss/channel");

            Podcast podcast = new Podcast
            {
                //.Art = GetImageFile(xmldoc),
                Category = GetValues(xmlNode, "itunes:category"),
                Description = GetValue(xmlNode, "itunes:summary"),
                FeedUrl = "https://rss.art19.com/the-asset",
                Hosts = new List<Host>
                {
                    new Host { Name = "Protect the Investigation" },
                    new Host { Name = "Center for American Progress Action Fund" },
                    new Host { Name = "District Productive" }
                },
                // TODO: Verify if this is correct...
                PodcastServices = new List<PodcastService>
                {
                    new PodcastService
                    {
                        Title = "Apple Podcasts",
                        Url = "https://podcasts.apple.com/us/podcast/the-asset/id1461422307",
                        SupportedPlatforms = new List<DevicePlatform>
                        {
                            DevicePlatform.iOS,
                            DevicePlatform.UWP
                        }
                    },
                    //new PodcastService
                    //{
                    //    Title = "Google Podcasts",
                    //    Url = "https://www.google.com/podcasts?feed=aHR0cHM6Ly9oYW5zZWxtaW51dGVzLmNvbS9zdWJzY3JpYmU%3D",
                    //    SupportedPlatforms = new List<DevicePlatform>
                    //    {
                    //        DevicePlatform.Android
                    //    }
                    //}
                },
                Id = "asset",
                Title = xmlNode.SelectSingleNode("title").InnerText,
                TwitterUrl = null,
                WebsiteUrl = "https://theassetpodcast.org/"
            };
            podcast.Art = GetImageFile(xmldoc);
            return podcast;
        }

        public async Task<Podcast> GetTrumpIncPodcast(bool forceRefresh)
        {
            XmlDocument xmldoc = await GetTrumpIncXmlDocument(forceRefresh);

            var xmlNode = xmldoc.SelectSingleNode("//rss/channel");

            Podcast podcast = new Podcast
            {
                Art = GetImageFile(xmldoc),
                Category = GetValues(xmlNode, "itunes:category"),
                Description = GetValue(xmlNode, "itunes:summary"),
                FeedUrl = "https://feeds.feedburner.com/trumpinc",
                Hosts = new List<Host>
                {
                    new Host { Name = "Protect the Investigation" },
                    new Host { Name = "Center for American Progress Action Fund" },
                    new Host { Name = "District Productive" }
                },
                // TODO: Verify if this is correct...
                PodcastServices = new List<PodcastService>
                {
                    new PodcastService
                    {
                        Title = "Apple Podcasts",
                        Url = "https://podcasts.apple.com/us/podcast/trump-inc/id1344894187",
                        SupportedPlatforms = new List<DevicePlatform>
                        {
                            DevicePlatform.iOS,
                            DevicePlatform.UWP
                        }
                    },
                    //new PodcastService
                    //{
                    //    Title = "Google Podcasts",
                    //    Url = "https://www.google.com/podcasts?feed=aHR0cHM6Ly9oYW5zZWxtaW51dGVzLmNvbS9zdWJzY3JpYmU%3D",
                    //    SupportedPlatforms = new List<DevicePlatform>
                    //    {
                    //        DevicePlatform.Android
                    //    }
                    //}
                },
                Id = "asset",
                Title = xmlNode.SelectSingleNode("title").InnerText,
                TwitterUrl = null,
                WebsiteUrl = "https://www.wnycstudios.org/podcasts/trumpinc"
            };
            return podcast;
        }

        public async Task<List<PodcastEpisode>> GetPodcastEpisodesAsync(Podcast podcast, bool theAsset, bool forceRefresh)
        {
            List<PodcastEpisode> podcastEpisodes = new List<PodcastEpisode>();

            XmlDocument xmldoc = await (theAsset ? GetTheAssetXmlDocument(forceRefresh) : GetTrumpIncXmlDocument(forceRefresh));
            XmlNodeList xmlNodeList = xmldoc.SelectNodes("//rss/channel/item");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                PodcastEpisode podcastEpisode = new PodcastEpisode
                {
                    ArtworkUrl = podcast.Art,
                    Date = xmlNode.SelectSingleNode("pubDate")?.InnerText,
                    Description = GetValue(xmlNode, "itunes:summary"),
                    //DisplayDate - This is a property based on Date
                    Duration = GetValue(xmlNode, "itunes:duration"),
                    EpisodeNumber = GetValue(xmlNode, "itunes:episode"),
                    Explicit = GetValue(xmlNode, "itunes:explicit"),
                    //Id = Guid.NewGuid().ToString(),
                    Mp3Url = xmlNode.SelectSingleNode("enclosure")?.Attributes["url"].Value,
                    Title = xmlNode.SelectSingleNode("title")?.InnerText,
                    PodcastName = podcast.Title
                };
                if (theAsset)
                {
                    podcastEpisode.EpisodeUrl = $"https://theassetpodcast.org/episode/{podcastEpisode.Title.ToLower().Replace(' ', '-')}/";
                }
                else
                {
                    podcastEpisode.EpisodeUrl = $"https://www.wnycstudios.org/podcasts/trumpinc/episodes/{podcastEpisode.Title.ToLower().Replace(' ', '-')}/";
                }
                podcastEpisodes.Add(podcastEpisode);
            }
            return podcastEpisodes;
        }

        public async Task<IEnumerable<Tweet>> GetTweetsAsync(string screenName)
        {
            var accessToken = await GetAccessToken(client);

            var requestUserTimeline = new HttpRequestMessage(HttpMethod.Get,
                $"https://api.twitter.com/1.1/statuses/user_timeline.json?count=100&screen_name={screenName}&trim_user=0&exclude_replies=1");

            requestUserTimeline.Headers.Add("Authorization", "Bearer " + accessToken);

            var responseUserTimeLine = await client.SendAsync(requestUserTimeline);
            var json = await responseUserTimeLine.Content.ReadAsStringAsync();

            var tweetsRaw = TweetRaw.FromJson(json);

            return tweetsRaw.Select(t => new Tweet
            {
                StatusID = t?.RetweetedStatus?.User?.ScreenName == screenName ? t.RetweetedStatus.IdStr : t.IdStr,
                ScreenName = t?.RetweetedStatus?.User?.ScreenName ?? t.User.ScreenName,
                Text = t?.Text,
                RetweetCount = t.RetweetCount,
                FavoriteCount = t.FavoriteCount,
                CreatedAt = GetDate(t.CreatedAt, DateTime.MinValue),
                Image = t?.RetweetedStatus != null && t?.RetweetedStatus?.User != null ?
                                     t.RetweetedStatus.User.ProfileImageUrlHttps.ToString() : (t?.User?.ScreenName == screenName ? "liar_in_chief.png" : t?.User?.ProfileImageUrlHttps.ToString()),
                MediaUrl = t?.Entities?.Media?.FirstOrDefault()?.MediaUrlHttps?.AbsoluteUri
            }).ToList();
        }

        public readonly string[] DateFormats = { "ddd MMM dd HH:mm:ss %zzzz yyyy",
                                                         "yyyy-MM-dd\\THH:mm:ss\\Z",
                                                         "yyyy-MM-dd HH:mm:ss",
                                                         "yyyy-MM-dd HH:mm"};

        private DateTime GetDate(string date, DateTime defaultValue)
        {
            return string.IsNullOrWhiteSpace(date) ||
                !DateTime.TryParseExact(date,
                        DateFormats,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var result)
                    ? defaultValue
                    : result;
        }

        private static async Task<string> GetAccessToken(HttpClient client)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/oauth2/token ");
            var customerInfo = Convert.ToBase64String(new UTF8Encoding()
                                      .GetBytes("ZTmEODUCChOhLXO4lnUCEbH2I:Y8z2Wouc5ckFb1a0wjUDT9KAI6DUat5tFNdmIkPLl8T4Nyaa2J"));
            request.Headers.Add("Authorization", "Basic " + customerInfo);
            request.Content = new StringContent("grant_type=client_credentials",
                                                    Encoding.UTF8, "application/x-www-form-urlencoded");

            var response = await client.SendAsync(request);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonValue.Parse(json);

            return result["access_token"];
        }

        private async Task<XmlDocument> GetRSSDocument(bool forceRefresh, string file, string rssFeed)
        {
            string fileName = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), file);
            FileInfo fileInfo = new FileInfo(fileName);
            string rssFeedData = null;

            if (forceRefresh || !fileInfo.Exists || (DateTime.Now - fileInfo.LastWriteTime) > new TimeSpan(24, 0, 0))
            {
                WebClient client = new WebClient();
                rssFeedData = await client.DownloadStringTaskAsync(rssFeed);
                using (StreamWriter writer = new StreamWriter(fileInfo.FullName))
                {
                    await writer.WriteAsync(rssFeedData);
                }
            }
            else
            {
                using (FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read))
                {
                    using (TextReader reader = new StreamReader(fs))
                    {
                        rssFeedData = await reader.ReadToEndAsync();
                    }
                }
            }


            XmlDocument document = new XmlDocument();
            document.LoadXml(rssFeedData);
            return document;
        }

        private async Task<XmlDocument> GetTheAssetXmlDocument(bool forceRefresh)
        {
            if (!forceRefresh && theAssetXmlDocument != null)
            {
                return theAssetXmlDocument;
            }
            theAssetXmlDocument = await GetRSSDocument(forceRefresh, "theasset.xml", "https://rss.art19.com/the-asset");
            return theAssetXmlDocument;
        }

        private async Task<XmlDocument> GetTrumpIncXmlDocument(bool forceRefresh)
        {
            if (!forceRefresh && trumpIncXmlDocument != null)
            {
                return trumpIncXmlDocument;
            }
            trumpIncXmlDocument = await GetRSSDocument(forceRefresh, "trumpinc.xml", "https://feeds.feedburner.com/trumpinc");
            return trumpIncXmlDocument;
        }

        private static string GetValue(XmlNode xmlNode, string name)
        {
            string result = string.Empty;
            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                if (child.Name == name)
                {
                    result = child.InnerText;
                    break;
                }
            }
            return result;
        }

        private static string GetValues(XmlNode xmlNode, string name)
        {
            string result = string.Empty;
            bool first = true;
            foreach (XmlNode child in xmlNode.ChildNodes)
            {
                if (child.Name == name)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        result += ", ";
                    }
                    result += child.Attributes["text"].Value;
                }
            }
            return result;
        }
    }
}
