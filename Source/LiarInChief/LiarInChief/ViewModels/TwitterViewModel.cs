﻿using LiarInChief.Models;
using MvvmHelpers;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LiarInChief.ViewModels
{
    public class TwitterViewModel : ViewModelBase
    {
        public ObservableRangeCollection<Tweet> Tweets { get; set; }

        private string _screenName;

        public TwitterViewModel(string screenName)
        {
            _screenName = screenName;
            Title = "Twitter";
            Icon = "slideout.png";
            Tweets = new ObservableRangeCollection<Tweet>();
            OpenTweetCommand = new Command<string>(async (s) => await ExecuteOpenTweetCommand(s));
        }

        public Command<string> OpenTweetCommand { get; }

        Command loadCommand;
        Command refreshCommand;
        public Command RefreshCommand => refreshCommand ??
                  (refreshCommand = new Command(async () =>
                  {
                      await ExecuteLoadCommand(true);
                  }, () =>
                  {
                      return !IsBusy;
                  }));

        public Command LoadCommand => loadCommand ??
                  (loadCommand = new Command(async () =>
                  {
                      await ExecuteLoadCommand(false);
                  }, () =>
                  {
                      return !IsBusy;
                  }));



        public async Task ExecuteLoadCommand(bool forceRefresh)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            try
            {
#if DEBUG
                await Task.Delay(1000);
#endif
                var items = await DataService.GetTweetsAsync(_screenName);
                if (items == null)
                {
                    await DisplayAlert("Error", "Unable to load tweets.", "OK");
                }
                else
                {
                    Tweets.ReplaceRange(items);
                }
            }
            catch
            {
                await DisplayAlert("Error", "Unable to load tweets.", "OK");
            }
            finally
            {
                IsBusy = false;
            }

            LoadCommand.ChangeCanExecute();
        }

        async Task ExecuteOpenTweetCommand(string statusId)
        {
            if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                if (await Launcher.CanOpenAsync("twitter://"))
                {
                    await Launcher.OpenAsync($"twitter://status?id={statusId}");
                    return;
                }
                else if (await Launcher.CanOpenAsync("tweetbot://"))
                {
                    await Launcher.OpenAsync($"tweetbot:///status/{statusId}");
                    return;
                }
            }

            await OpenBrowserAsync($"http://twitter.com/{_screenName}/status/" + statusId);
        }
    }
}