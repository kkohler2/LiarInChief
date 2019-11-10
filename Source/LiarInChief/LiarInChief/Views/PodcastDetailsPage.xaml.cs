using LiarInChief.Models;
using LiarInChief.ViewModels;
using LiarInChief.Views;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LiarInChief.Views
{
    public partial class PodcastDetailsPage : ContentPage
    {
        PodcastDetailsViewModel VM => (PodcastDetailsViewModel)BindingContext;
        public PodcastDetailsPage()
        {
            BindingContext = new PodcastDetailsViewModel();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if(VM.Episodes.Count == 0)
                VM.LoadEpisodesCommand.Execute(null);
        }

        void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var listView = sender as ListView;
            if (!(listView?.SelectedItem is PodcastEpisode episode))
                return;

            await Navigation.PushModalAsync(new PodcastEpisodePage(episode, VM.Podcast.Title));


            listView.SelectedItem = null;
        }

        void ListView_ItemAppearing(object sender, ItemVisibilityEventArgs e)
        {

            if(e.ItemIndex == 0 && StackLayoutInfo.IsVisible)
            {
                StackLayoutInfo.FadeTo(0).ContinueWith((t) =>
                {
                    StackLayoutInfo.IsVisible = false;
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }

            if (VM.IsBusy || VM.Episodes.Count == 0)
                return;
            //hit bottom!
            if (e.ItemIndex == VM.Episodes.Count - 1)
            {
                VM.LoadMoreEpisodes();
            }
        }

        private void ListView_ItemDisappearing(object sender, ItemVisibilityEventArgs e)
        {
            if (e.ItemIndex != 0 || StackLayoutInfo.IsVisible)
                return;

            StackLayoutInfo.FadeTo(1);
            StackLayoutInfo.IsVisible = true;
        }
    }
}