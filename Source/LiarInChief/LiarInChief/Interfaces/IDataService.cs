using LiarInChief.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LiarInChief.Interfaces
{
    public interface IDataService
    {
        Podcast GetPodcast();
        Task<List<PodcastEpisode>> GetPodcastEpisodesAsync(string id, bool forceRefresh);
        Task<IEnumerable<Tweet>> GetTweetsAsync(string screenName);
    }
}
