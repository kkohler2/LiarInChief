using System;
using LiarInChief.Helpers;
using LiarInChief.Models;
using LiarInChief.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LiarInChief.Views
{
    public partial class TheAssetPodcastEpisodePage : ContentPage
    {
        public TheAssetPodcastEpisodePage(PodcastEpisode episode, string title)
        {
            InitializeComponent();
            BindingContext = new PodcastEpisodeViewModel(episode)
            {
                Title = title
            };
        }

        public TheAssetPodcastEpisodePage()
        {
            InitializeComponent();
        }

        private async void ButtonClose_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }
    }
}