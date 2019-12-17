using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using LiarInChief.Models;
using MvvmHelpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LiarInChief.ViewModels
{
    public class PodcastDetailsBaseViewModel : ViewModelBase
    {
        public ICommand SubscribeCommand { get; set; }
        public ICommand LoadEpisodesCommand { get; set; }
        public Podcast Podcast { get; set; }
        public List<PodcastEpisode> AllEpisodes { get; set; }
        public ObservableRangeCollection<PodcastEpisode> Episodes { get; set; }
        private readonly bool _theAsset;

        public PodcastDetailsBaseViewModel(bool theAsset)
        {
            SubscribeCommand = new Command(async () => await ExecuteSubscribeCommand());
            LoadEpisodesCommand = new Command(async () => await ExecuteLoadEpisodesCommand(false));
            Episodes = new ObservableRangeCollection<PodcastEpisode>();
            _theAsset = theAsset;
            Podcast = theAsset ? DataService.GetTheAssetPodcast(false) : DataService.GetTrumpIncPodcast(false);
            AllEpisodes = new List<PodcastEpisode>();
        }

        public async Task ExecuteSubscribeCommand()
        {
            var services = Podcast
                .PodcastServices
                .Where(s => s.SupportedPlatforms.Contains(DeviceInfo.Platform))
                .Select(s => s.Title);

            var result = await CurrentPage.DisplayActionSheet("Subscribe on:", "Cancel", null,services.ToArray());

            var service = Podcast.PodcastServices.Find(s => s.Title == result);
            if (service == null)
                return;

            await OpenBrowserAsync(service.Url);
        }

        public async Task ExecuteLoadEpisodesCommand(bool forceRefresh)
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
#if DEBUG
                await Task.Delay(1000);
#endif
                var episodes = await DataService.GetPodcastEpisodesAsync(Podcast, _theAsset, forceRefresh);

                AllEpisodes.Clear();
                Episodes.Clear();
                CanLoadMore = true;
                AllEpisodes.AddRange(episodes);
                LoadMoreEpisodes();
            }
            catch (System.Exception)
            {
                //stuff
            }
            finally
            {
                IsBusy = false;
            }
        }

        private const int chunk = 50;

        public void LoadMoreEpisodes()
        {
            if (!CanLoadMore)
                return;

            var totalLeft = AllEpisodes.Count - Episodes.Count;
            var toGet = totalLeft > chunk ? chunk : totalLeft;
            Episodes.AddRange(AllEpisodes.GetRange(Episodes.Count, toGet));
            CanLoadMore = Episodes.Count != AllEpisodes.Count;
        }

        Command refreshCommand;
        public Command RefreshCommand => refreshCommand ??
                  (refreshCommand = new Command(async () =>
                  {
                      await ExecuteLoadEpisodesCommand(true);
                  }, () =>
                  {
                      return !IsBusy;
                  }));

    }
}
