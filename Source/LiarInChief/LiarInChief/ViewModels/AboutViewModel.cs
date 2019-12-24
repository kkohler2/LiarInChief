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
            string img = DataService.GetBackgroundImage();
            Height = img == "trump_truck.png" ? 200 : 600;
            Image = img;
        }

        public ICommand OpenWebCommand { get; }

        public string Summary { get; set; } = "My name is Donald J. Trump. I am a liar, cheater, failed businessman and a corrupt President of the United States. I've been banned from running charities.  I have stiffed countless contractors for work.  I hire foreigners over Americans to work at my properties and I have the distinct honor of being the third president of the United States to be impeached. So much winning!";
        public string Image { get; set; } = "trump_truck.png";
        public int Height { get; set; }
    }
}