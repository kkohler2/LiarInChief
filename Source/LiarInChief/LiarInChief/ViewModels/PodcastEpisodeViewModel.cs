using LiarInChief.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LiarInChief.ViewModels
{
    public class PodcastEpisodeViewModel : ViewModelBase
    {
        public PodcastEpisode Episode { get; set; }
        public Command PlayPodcastCommand { get; }

        public PodcastEpisodeViewModel()
        {
            PlayPodcastCommand = new Command(async () => await PlayPodcastAsync());
        }

        public PodcastEpisodeViewModel(PodcastEpisode episode) :
            this()
        {
            Episode = episode;
        }

        async Task PlayPodcastAsync()
        {
            try
            {
                await Launcher.OpenAsync(Episode.Mp3Url);
            }
            catch (Exception /*ex*/)
            {
            }
        }
    }
}
