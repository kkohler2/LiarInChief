using LiarInChief.Interfaces;
using LiarInChief.Models;
using LiarInChief.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LiarInChief.Views
{
    public partial class RealTwitterPage : ContentPage, IPageHelpers
    {
        RealTwitterViewModel vm;
        RealTwitterViewModel ViewModel => vm ?? (vm = (RealTwitterViewModel)BindingContext);

        public RealTwitterPage()
        {
            InitializeComponent();

            BindingContext = new RealTwitterViewModel();

            listView.ItemTapped += (sender, args) =>
                listView.SelectedItem = null;

            listView.ItemSelected += (sender, args) =>
            {
                if (listView.SelectedItem == null)
                    return;


                var tweet = listView.SelectedItem as Tweet;


                ViewModel.OpenTweetCommand.Execute(tweet.StatusID);


                listView.SelectedItem = null;
            };
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (DeviceInfo.Platform != DevicePlatform.UWP)
                OnPageVisible();
        }

        public void OnPageVisible()
        {
            if (ViewModel == null || !ViewModel.CanLoadMore || ViewModel.IsBusy || ViewModel.Tweets.Count > 0)
                return;

            ViewModel.LoadCommand.Execute(null);
        }
    }
}
