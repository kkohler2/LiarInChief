using System.Collections.Generic;
using Xamarin.Essentials;

namespace LiarInChief.Models
{
    public class PodcastService
    {
        public string Title { get; set; }
        public string Url { get; set; }
        public List<DevicePlatform> SupportedPlatforms { get; set; }
    }
}
