using System;
using Xamarin.Forms;

namespace LiarInChief.Controls
{
    public class LiarInChiefNavigationPage :NavigationPage
    {
        public LiarInChiefNavigationPage(Page root) : base(root)
        {
            Init();
        }

        public LiarInChiefNavigationPage()
        {
            Init();
        }

        void Init()
        {

            BarBackgroundColor = Color.FromHex("#03A9F4");
            BarTextColor = Color.White;
        }
    }
}

