﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LiarInChief.Helpers;
using LiarInChief.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace LiarInChief.Models
{
    public class SocialItem
    {
        public SocialItem()
        {
            OpenUrlCommand = new Command(async () => await OpenSocialUrl());
        }

        public string Icon { get; set; }
        public string Url { get; set; }

        public ICommand OpenUrlCommand { get; }

        async Task OpenSocialUrl()
        {
            if (DeviceInfo.Platform == DevicePlatform.iOS && Url.Contains("twitter"))
            {
                if (await Launcher.CanOpenAsync("twitter://"))
                {
                    await Launcher.OpenAsync("twitter://user?screen_name=shanselman");
                    return;
                }
                else if (await Launcher.CanOpenAsync("tweetbot://"))
                {
                    await Launcher.OpenAsync("tweetbot://shanselman/timeline");
                    return;
                }
            }
            await ViewModelBase.OpenBrowserAsync(Url);
        }
    }
}
