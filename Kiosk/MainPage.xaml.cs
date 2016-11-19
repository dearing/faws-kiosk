using System;
using System.Collections.Generic;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Networking.Connectivity;
using System.Net;
using System.Net.Sockets;

namespace Kiosk
{
    public sealed partial class MainPage : Page
    {

        private static int Index;
        private static Dictionary<int, string> Registry;

        public MainPage()
        {

            Registry = new Dictionary<int, string>();
            Registry.Add(0, "http://dashboard.rackerroush.com/3rd/");
            Registry.Add(1, "http://dashboard.rackerroush.com/maintenance/");
            Registry.Add(2, "http://fawsdashboards.rackspace.com/ticketstats/index.php");
            Registry.Add(3, "http://fawsdashboards.rackspace.com/ticketstats/index2.php");
            Registry.Add(4, "http://fawsdashboards.rackspace.com/ticketstats/index3.php");
            Registry.Add(5, "http://fawsdashboards.rackspace.com/ticketstats/index4.php");

            DispatcherTimer dt = new DispatcherTimer();
            dt.Tick += Dt_Tick;
            dt.Interval = new TimeSpan(0, 0, 30);
            dt.Start();
            this.InitializeComponent();
        }

        private void Dt_Tick(object sender, object e)
        {

            Index += 1;
            if (Index >= Registry.Count)
            {
                Index = 0;
            }
            Update(Registry[Index]);

        }

        private void Marquee_Loaded(object sender, RoutedEventArgs e)
        {
            if (!ApplicationView.GetForCurrentView().TryEnterFullScreenMode())
            {
                Update(Registry[Index]);
            }

        }

        private void Marquee_LayoutUpdated(object sender, object e)
        {
            Update(Registry[Index]);
        }

        private void Update(string URL)
        {
            try
            {
                Marquee.Navigate(new Uri(URL));
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
