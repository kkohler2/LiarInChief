using LiarInChief.Models;
using Xamarin.Forms;

namespace LiarInChief.Views.Twitter
{
    public class TweetDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate TweetTemplate { get; set; }
        public DataTemplate TweetWithMediaTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((Tweet)item).HasMedia ? TweetWithMediaTemplate : TweetTemplate;
        }
    }
}
