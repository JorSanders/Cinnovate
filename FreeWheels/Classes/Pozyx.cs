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

        public async Task LetsGo()
        {

            Debug.WriteLine("INFO: Flash Reset" + (RegisterFunctions.FlashReset() ? "SUCCESS" : "FAILURE"));
            await (Task.Delay(5000));

            DeviceListFunctions.DevicesClear();
            Debug.WriteLine("INFO: ClearDevices");
            await (Task.Delay(1000));

            //DeviceListFunctions.DeviceAdd(24632, 1, 0, 0, 1880);
            //DeviceListFunctions.DeviceAdd(24667, 1, 2554, 0, 1600);
            //DeviceListFunctions.DeviceAdd(24617, 1, 0, 8123, 1900);
            //DeviceListFunctions.DeviceAdd(24647, 1, 3105, 8176, 2050);

            DeviceListFunctions.DeviceAdd(24667, 1, 0, 0, 1880);
            Debug.WriteLine("INFO: DeviceAdd - 24667");
            await (Task.Delay(1000));

            DeviceListFunctions.DeviceAdd(24632, 1, 2554, 0, 1600);
            Debug.WriteLine("INFO: DeviceAdd - 24632");
            await (Task.Delay(1000));

            DeviceListFunctions.DeviceAdd(24647, 1, 0, 8123, 1900);
            Debug.WriteLine("INFO: DeviceAdd - 24647");
            await (Task.Delay(1000));

            DeviceListFunctions.DeviceAdd(24617, 1, 3105, 8176, 2050);
            Debug.WriteLine("INFO: DeviceAdd - 24617");
            await (Task.Delay(1000));

            //ConfigurationRegisters.PosAlg(1, 3);
            ConfigurationRegisters.PosAlg(0, 3);
            Debug.WriteLine("INFO: Set Position Algorithm - 0, 3");
            await (Task.Delay(1000));

            int[] posAlg = ConfigurationRegisters.PosAlg();
            Debug.WriteLine("PosAlg:" + posAlg[0] + ", " + posAlg[1]);
            await (Task.Delay(1000));

            ConfigurationRegisters.PosFilter(15, 1);
            Debug.WriteLine("INFO: Set Position Filter - 15, 1");
            await (Task.Delay(1000));

            int[] posFilter = ConfigurationRegisters.PosFilter();
            Debug.WriteLine("PosFilter:" + posFilter[0] + ", " + posFilter[1]);
            await (Task.Delay(1000));

            ConfigurationRegisters.PosInterval(200);
            Debug.WriteLine("INFO: Set Position Interval - 200");
            await (Task.Delay(1000));

            Debug.WriteLine("PosInterval:" + ConfigurationRegisters.PosInterval());
            await (Task.Delay(1000));
        }

    }
}
