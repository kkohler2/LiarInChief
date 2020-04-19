using LiarInChief.Helpers;
using LiarInChief.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

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
            SetBackgroundImage(false); // Intentionally not waiting for method to finish!
        }

        private async Task SetBackgroundImage(bool forceRefresh)
        {
            string img = await DataService.GetBackgroundImage(forceRefresh);
            Height = img == "trump_truck.png" ? 200 : 600;
            Image = img;
        }

        bool isRefreshing;

        public bool IsRefreshing
        {
            get { return isRefreshing; }
            set
            {
                isRefreshing = value;
                OnPropertyChanged();
            }
        }

        public ICommand RefreshCommand => new Command(async () => await RefreshItemsAsync());

        public ICommand OpenWebCommand { get; }

        public string Summary { get; set; } = "My name is Donald J. Trump. I am a liar, cheater, failed businessman and a corrupt President of the United States. I've been banned from running charities.  I have stiffed countless contractors for work.  I hire foreigners over Americans to work at my properties and I have the distinct honor of being the third president of the United States to be impeached. So much winning!";

        private string _image;
        public string Image {
            get { return _image; }
            set
            {
                _image = value;
                OnPropertyChanged();
            }
        }
        private int _height;
        public int Height
        {
            get { return _height; }
            set
            {
                _height = value;
                OnPropertyChanged();
            }
        }

        async Task RefreshItemsAsync()
        {
            IsRefreshing = true;
            await SetBackgroundImage(true);
            IsRefreshing = false;
        }
    }
}