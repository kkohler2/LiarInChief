using System;
using LiarInChief.Helpers;
using MvvmHelpers;

namespace LiarInChief.Models
{
    public class PodcastEpisode : ObservableObject
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Date { get; set; }

        public string Description { get; set; }

        public string Mp3Url { get; set; }

        private string _artworkUrl;
        public string ArtworkUrl
        {
            get { return _artworkUrl; }
            set { SetProperty(ref _artworkUrl, value); }
        }
        public string Duration { get; set; }

        public string Explicit { get; set; }

        public string EpisodeNumber { get; set; }

        public string EpisodeUrl { get; set; }

        public string PodcastName
        {
            get;set;
        }

        string displayDate;
        public string DisplayDate
        {
            get => DateTimeOffset.TryParse(Date, out var time) ? time.PodcastEpisodeHumanize() : Date;
            set => displayDate = value;
        }
    }
}
