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

        public void LetsGo()
        {

            DeviceListFunctions.DevicesClear();

            //DeviceListFunctions.DeviceAdd(24632, 1, 0, 0, 1880);
            //DeviceListFunctions.DeviceAdd(24667, 1, 2554, 0, 1600);
            //DeviceListFunctions.DeviceAdd(24617, 1, 0, 8123, 1900);
            //DeviceListFunctions.DeviceAdd(24647, 1, 3105, 8176, 2050);

            DeviceListFunctions.DeviceAdd(24667, 1, 0, 0, 1880);
            DeviceListFunctions.DeviceAdd(24632, 1, 2554, 0, 1600);
            DeviceListFunctions.DeviceAdd(24647, 1, 0, 8123, 1900);
            DeviceListFunctions.DeviceAdd(24617, 1, 3105, 8176, 2050);

            ConfigurationRegisters.PosAlg(0, 3);

            ConfigurationRegisters.PosInterval(200);
        }

    }
}
