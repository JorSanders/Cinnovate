using FreeWheels.Classes.PozyxApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace FreeWheels.Classes
{
    public class Pozyx
    {

        public Pozyx()
        {
            Connection.Connect();
        }

        public bool LetsGo()
        {

            DeviceListFunctions.DevicesClear();

            DeviceListFunctions.DeviceAdd(24632, 1, 0, 0, 1800);
            DeviceListFunctions.DeviceAdd(24667, 1, 2500, 0, 1500);
            DeviceListFunctions.DeviceAdd(24617, 1, 0, 8200, 1800);
            DeviceListFunctions.DeviceAdd(24647, 1, 2500, 8200, 2000);

            ConfigurationRegisters.PosAlg(0, 3);

            ConfigurationRegisters.PosInterval(100);

            DispatcherTimer dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 200);

            dispatcherTimer.Start();

            return true;
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            int x = PositioningData.PosX();
            int y = PositioningData.PosY();
            int z = PositioningData.PosZ();

            Debug.Write("x: " + x + "\t y: " + y + "\t z: " + z + "\n");
        }

    }
}
