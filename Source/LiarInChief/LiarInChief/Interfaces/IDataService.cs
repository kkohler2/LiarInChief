using LiarInChief.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiarInChief.Interfaces
{
    public interface IDataService
    {
        Podcast GetTheAssetPodcast();
        Podcast GetTrumpIncPodcast();
        Task<List<PodcastEpisode>> GetPodcastEpisodesAsync(Podcast podcast, bool theAsset, bool forceRefresh);
        Task<IEnumerable<Tweet>> GetTweetsAsync(string screenName);
    }
}
