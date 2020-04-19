using MvvmHelpers;
using System.Collections.Generic;
using System.Linq;

namespace LiarInChief.Models
{
    public class Podcast : ObservableObject
    {
        private string _id;
        public string Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }
        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _art;
        public string Art
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set { SetProperty(ref _description, value); }
        }
        private string _feedUrl;
        public string FeedUrl
        {
            get { return _feedUrl; }
            set { SetProperty(ref _feedUrl, value); }
        }
        private List<Host> _hosts;
        public List<Host> Hosts
        {
            get { return _hosts; }
            set { SetProperty(ref _hosts, value); }
        }

        public string HostsNames 
        {
            get
            {
                if (Hosts.Count == 0)
                    return string.Empty;

                if(Hosts.Count == 1)
                    return $"{Hosts.FirstOrDefault()?.Name ?? string.Empty}";

                return string.Join(", ", Hosts.Select(h => h.Name));
            }
        }

        private string _category;
        public string Category
        {
            get { return _category; }
            set { SetProperty(ref _category, value); }
        }
        private string _websiteUrl;
        public string WebsiteUrl
        {
            get { return _websiteUrl; }
            set { SetProperty(ref _websiteUrl, value); }
        }
        private string _twitterUrl;
        public string TwitterUrl
        {
            get { return _twitterUrl; }
            set { SetProperty(ref _twitterUrl, value); }
        }
        private List<PodcastService> _podcastServices;
        public List<PodcastService> PodcastServices
        {
            get { return _podcastServices; }
            set { SetProperty(ref _podcastServices, value); }
        }
    }
}
