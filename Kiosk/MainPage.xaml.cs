﻿using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Networking.Connectivity;
using System.Net;
using System.Net.Sockets;


namespace Kiosk
{

    public static class Registry
    {
        private static int index = 0;
        public static List<Uri> URIs = new List<Uri>();
        
        static Registry()
        {
            URIs.Add(new Uri("http://www.randomgoat.com/"));
            URIs.Add(new Uri("http://dashboard.rackerroush.com/3rd/"));
            URIs.Add(new Uri("http://dashboard.rackerroush.com/maintenance/"));
            URIs.Add(new Uri("http://fawsdashboards.rackspace.com/ticketstats/index.php"));
            URIs.Add(new Uri("http://fawsdashboards.rackspace.com/ticketstats/index2.php"));
            URIs.Add(new Uri("http://fawsdashboards.rackspace.com/ticketstats/index3.php"));
            URIs.Add(new Uri("http://fawsdashboards.rackspace.com/ticketstats/index4.php"));
        }

        public static Uri Cycle()
        {
            if (index > URIs.Count) {
                index = 0;
            }
            return URIs[index++];
        }
    }

    public sealed partial class MainPage : Page
    {

        public MainPage()
        {

            DispatcherTimer dt = new DispatcherTimer();
            dt.Tick += Dt_Tick;
            dt.Interval = new TimeSpan(0, 0, 10);
            dt.Start();

            this.InitializeComponent();

            Marquee.Navigate(Registry.Cycle());
        }

        private void Dt_Tick(object sender, object e)
        {
            Marquee.Navigate(Registry.Cycle());
            Update();
        }

        private void Marquee_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ApplicationView.GetForCurrentView().TryEnterFullScreenMode())
            {
                Update();
            }

        }

        private void Marquee_LayoutUpdated(object sender, object e)
        {
            Update();
        }

        private void Update()
        {
            try
            {
                Refresh();
            }
            catch (FormatException ex)
            {
                Marquee.NavigateToString(String.Format("<html><body><h2>{0}</h2><hr /><code>{1}</code></body></html>", ex.Message, ex.StackTrace));
            }
        }

        private void Refresh()
        {
            try
            {
                status.Text = string.Format("FAWS-3RD // {0}", GetIpAddress().ToString());
                Marquee.Width = ApplicationView.GetForCurrentView().VisibleBounds.Width;
                Marquee.Height = ApplicationView.GetForCurrentView().VisibleBounds.Height;
            }
            catch (FormatException ex)
            {
                Marquee.NavigateToString(String.Format("<html><body><h2>{0}</h2><hr /><code>{1}</code></body></html>", ex.Message, ex.StackTrace));
            }
        }

        private IPAddress GetIpAddress()
        {
            var hosts = NetworkInformation.GetHostNames();
            foreach (var host in hosts)
            {
                IPAddress addr;
                if (!IPAddress.TryParse(host.DisplayName, out addr)) continue;
                if (addr.AddressFamily != AddressFamily.InterNetwork) continue;
                if (addr.ToString().StartsWith("169.254.")) continue;
                return addr;
            }
            return null;
        }
    }


}
