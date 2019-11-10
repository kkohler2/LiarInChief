using LiarInChief.Helpers;
using LiarInChief.Models;
using System.Collections.Generic;
using System.Windows.Input;

namespace LiarInChief.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public List<SocialItem> SocialItems { get; }
        public AboutViewModel()
        {
            SocialItems = new List<SocialItem>
            {
                new SocialItem
                {
                    Icon = IconConstants.TwitterCircle,
                    Url = "https://www.twitter.com/realDonaldTrump"
                },
                new SocialItem
                {
                    Icon = IconConstants.TwitterCircle,
                    Url = "https://www.twitter.com/realDonaldTrFan"
                },
                new SocialItem
                {
                    Icon = IconConstants.RssBox,
                    Url = "https://theassetpodcast.org/"
                },
                new SocialItem
                {
                    Icon = IconConstants.RssBox,
                    Url = "https://www.wnycstudios.org/podcasts/trumpinc"
                }
            };
        }

        public ICommand OpenWebCommand { get; }
    }
}