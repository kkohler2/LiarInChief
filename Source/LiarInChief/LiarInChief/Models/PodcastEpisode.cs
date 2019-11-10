using System;
using LiarInChief.Helpers;

namespace LiarInChief.Models
{
    public class PodcastEpisode
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Date { get; set; }

        public string Description { get; set; }

        public string Mp3Url { get; set; }

        public string ArtworkUrl { get; set; }

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
