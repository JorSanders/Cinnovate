using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;
using System.Threading.Tasks;
using System.Collections;
using System.Diagnostics;
using System.Text;
using FreeWheels.Classes;
using FreeWheels.Classes.PozyxApi;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FreeWheels
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Pozyx _Pozyx;

        public MainPage()
        {
            this.InitializeComponent();
            _Pozyx = new Pozyx();

            //Start();
        }

        async void Start()
        {
            _Pozyx.LetsGo();

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);

            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            int x = PositioningData.PosX();
            int y = PositioningData.PosY();
            int z = PositioningData.PosZ();

            this.Output.Text = "x: " + x + "\t y: " + y + "\t z: " + z;

            String timeStamp = DateTime.Now.ToString();
            this.Timestamp.Text = "Timestamp: " + timeStamp;

        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            RegisterFunctions.ResetSys();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            Start();
            this.StartButton.Visibility = Visibility.Collapsed;
        }
    }
}
