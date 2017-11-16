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

        }

        async void Start()
        {
            ConfigurationRegisters.PosAlg(0, 3);
            ConfigurationRegisters.PosInterval(400);

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);

            int[] posAlg = ConfigurationRegisters.PosAlg();
            Debug.WriteLine(posAlg[0] + " " + posAlg[1]);
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
            //this.StartButton.Visibility = Visibility.Collapsed;
        }

        private void ManualButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("Manual\n");
            DeviceListFunctions.DeviceAdd(24632, 1, 0, 0, 1800);
            DeviceListFunctions.DeviceAdd(24667, 1, 2500, 0, 1500);
            DeviceListFunctions.DeviceAdd(24617, 1, 0, 8200, 1800);
            DeviceListFunctions.DeviceAdd(24647, 1, 2500, 8200, 2000);
        }

        private void CalibButton_Click(object sender, RoutedEventArgs e)
        {
            int[] deviceIds = DeviceListFunctions.DevicesGetIds(0, 4);
            Debug.Write("Calib\n");
            DeviceListFunctions.CalibrateDevices(2, 10, deviceIds);
        }

        private void DiscoverButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("Discover\n");
            DeviceListFunctions.DevicesDiscover(0);
        }

        private void SetAnchors_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("Set anchors\n");
            int[] deviceIds = DeviceListFunctions.DevicesGetIds(0, 4);
            RegisterFunctions.PosSetAnchorIds(deviceIds);
        }

        private void AnchorPosButton_Click(object sender, RoutedEventArgs e)
        {
            int[] anchorIds = RegisterFunctions.PosGetAnchorIds().ToArray();
            Debug.Write("Anchor pos\n");
            foreach (int anchorId in anchorIds)
            {
                Debug.Write(anchorId + " \n");
                int[] pos = DeviceListFunctions.DeviceGetCoords(anchorId);
                Debug.Write(pos[0] + " \n");
                Debug.Write(pos[1] + " \n");
                Debug.Write(pos[2] + " \n\n");

            }
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            Debug.Write("Clear\n");
            DeviceListFunctions.DevicesClear();
        }

        private void Aids_Click(object sender, RoutedEventArgs e)
        {
            int[] anchorIds = RegisterFunctions.PosGetAnchorIds().ToArray();
            int[][] anchors = new int[anchorIds.Length][];
            Debug.Write("Anchor pos\n");
            int i = 0;
            foreach (int anchorId in anchorIds)
            {
                Debug.Write(anchorId + " \n");
                int[] pos = DeviceListFunctions.DeviceGetCoords(anchorId);
                Debug.Write(pos[0] + " \n");
                Debug.Write(pos[1] + " \n");
                Debug.Write(pos[2] + " \n\n");

                anchors[i] = new int[4];
                anchors[i][0] = anchorId;
                anchors[i][1] = pos[0];
                anchors[i][2] = pos[1];
                anchors[i][3] = pos[2];
                i++;
            }

            //DeviceListFunctions.DevicesClear();
            int height = 2000;

            for (int j = 0; j < anchors.Length; j++)
            {
                DeviceListFunctions.DeviceAdd(anchors[j][0], 1, anchors[j][1], anchors[j][2], height);
            }

        }
    }
}
