using LiarInChief.Interfaces;
using MvvmHelpers;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LiarInChief.ViewModels
{
    public class ViewModelBase : BaseViewModel
    {
        IDataService dataService;
        protected IDataService DataService => dataService ?? (dataService = DependencyService.Get<IDataService>());

        protected Page CurrentPage => Application.Current.MainPage;

        protected Task DisplayAlert(string title, string message, string cancel) =>
            CurrentPage.DisplayAlert(title, message, cancel);

        public static Task OpenBrowserAsync(string url) =>
            Browser.OpenAsync(url, new BrowserLaunchOptions
            {
                LaunchMode = BrowserLaunchMode.SystemPreferred,
                TitleMode = BrowserTitleMode.Show,
                PreferredControlColor = Color.White,
                PreferredToolbarColor = (Color)Application.Current.Resources["PrimaryColor"]
            });
    }
}
