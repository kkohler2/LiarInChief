using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using LiarInChief.Interfaces;
using LiarInChief.Models;
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

        public Podcast GetTheAssetPodcast()
        {
            XmlDocument xmldoc = GetTheAssetXmlDocument();

            var xmlNode = xmldoc.SelectSingleNode("//rss/channel");

            Podcast podcast = new Podcast
            {
                Art = xmldoc.SelectSingleNode("//rss/channel/image/url")?.InnerText,
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
            return podcast;
        }

        public Podcast GetTrumpIncPodcast()
        {
            XmlDocument xmldoc = GetTrumpIncXmlDocument();

            var xmlNode = xmldoc.SelectSingleNode("//rss/channel");

            Podcast podcast = new Podcast
            {
                Art = xmldoc.SelectSingleNode("//rss/channel/image/url")?.InnerText,
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

        public Task<List<PodcastEpisode>> GetPodcastEpisodesAsync(Podcast podcast, bool theAsset, bool forceRefresh)
        {
            List<PodcastEpisode> podcastEpisodes = new List<PodcastEpisode>();

            XmlDocument xmldoc = theAsset ? GetTheAssetXmlDocument() : GetTrumpIncXmlDocument();

            string artworkUrl = xmldoc.SelectSingleNode("//rss/channel/image/url")?.InnerText;

            XmlNodeList xmlNodeList = xmldoc.SelectNodes("//rss/channel/item");
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                PodcastEpisode podcastEpisode = new PodcastEpisode
                {
                    ArtworkUrl = artworkUrl,
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
            return Task.FromResult(podcastEpisodes);
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

        private XmlDocument GetTheAssetXmlDocument()
        {
            if (theAssetXmlDocument != null)
            {
                return theAssetXmlDocument;
            }

            //if (!File.Exists(@"D:\source\TheAssetRSS\TheAssetRSS\TheAsset.xml"))
            //{
            WebClient client = new WebClient();
            string rssFeedData = null;
            using (Stream data = client.OpenRead("https://rss.art19.com/the-asset"))
            {
                using (StreamReader reader = new StreamReader(data))
                {
                    rssFeedData = reader.ReadToEnd();
                }
            }
            //using (StreamWriter writer = new StreamWriter(@"D:\source\TheAssetRSS\TheAssetRSS\TheAsset.xml"))
            //{
            //    writer.Write(s);
            //}
            //}
            //string xml = null;
            //using (FileStream fs = new FileStream(@"D:\source\TheAssetRSS\TheAssetRSS\TheAsset.xml", FileMode.Open, FileAccess.Read))
            //{
            //    using (TextReader reader = new StreamReader(fs))
            //    {
            //        xml = reader.ReadToEnd();
            //    }
            //}
            theAssetXmlDocument = new XmlDocument();
            theAssetXmlDocument.LoadXml(rssFeedData);
            return theAssetXmlDocument;
        }

        private XmlDocument GetTrumpIncXmlDocument()
        {
            if (trumpIncXmlDocument != null)
            {
                return trumpIncXmlDocument;
            }

            //if (!File.Exists(@"D:\source\TheAssetRSS\TheAssetRSS\TheAsset.xml"))
            //{
            WebClient client = new WebClient();
            string rssFeedData = null;
            using (Stream data = client.OpenRead("https://feeds.feedburner.com/trumpinc"))
            {
                using (StreamReader reader = new StreamReader(data))
                {
                    rssFeedData = reader.ReadToEnd();
                }
            }
            //using (StreamWriter writer = new StreamWriter(@"D:\source\TheAssetRSS\TheAssetRSS\TheAsset.xml"))
            //{
            //    writer.Write(s);
            //}
            //}
            //string xml = null;
            //using (FileStream fs = new FileStream(@"D:\source\TheAssetRSS\TheAssetRSS\TheAsset.xml", FileMode.Open, FileAccess.Read))
            //{
            //    using (TextReader reader = new StreamReader(fs))
            //    {
            //        xml = reader.ReadToEnd();
            //    }
            //}
            trumpIncXmlDocument = new XmlDocument();
            trumpIncXmlDocument.LoadXml(rssFeedData);
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
