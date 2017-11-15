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
            //Setup();
        }

        public bool LetsGo()
        {

            DeviceListFunctions.DevicesClear();

            DeviceListFunctions.DeviceAdd(24632, 1, 0, 0, 1800);
            DeviceListFunctions.DeviceAdd(24667, 1, 2500, 0, 1500);
            DeviceListFunctions.DeviceAdd(24617, 1, 0, 8200, 1800);
            DeviceListFunctions.DeviceAdd(24647, 1, 2500, 8200, 2000);

            DeviceListFunctions.CalibrateDevices(2, 10 /*, anchorIds */);

            ConfigurationRegisters.PosAlg(0, 1);

            ConfigurationRegisters.PosInterval(400);

            return true;
        }

        public async void Setup()
        {
            await Task.Delay(TimeSpan.FromSeconds(5));

            Debug.Write("Setting pos alg\n");
            ConfigurationRegisters.PosAlg(0, 1);

            Debug.Write("Reset: " + RegisterFunctions.ResetSys());
            await Task.Delay(TimeSpan.FromSeconds(5));
            Debug.Write("\n\n");

            Debug.Write("Discover: " + DeviceListFunctions.DevicesDiscover());
            await Task.Delay(TimeSpan.FromSeconds(1));
            Debug.Write("\n\n");

            int[] deviceIds = DeviceListFunctions.DevicesGetIds(0, 4);
            Debug.Write("DeviceIds: \n");
            foreach (int deviceId in deviceIds)
            {
                Debug.Write(deviceId + " \n");
            }

            RegisterFunctions.DoPositioning();
            bool tmp = RegisterFunctions.PosSetAnchorIds(deviceIds);
            Debug.Write("PosSetAnchorIds: " + tmp + "\n");

            int[] anchorIds = RegisterFunctions.PosGetAnchorIds().ToArray();
            Debug.Write("anchorIds: \n");
            foreach (int anchorId in anchorIds)
            {
                Debug.Write(anchorId + " \n");
            }

            Debug.Write("Calib anchors\n");
            DeviceListFunctions.CalibrateDevices(2, 10 /*, anchorIds */);
            await Task.Delay(TimeSpan.FromSeconds(15));

            Debug.Write("Anchor pos\n");
            foreach (int anchorId in anchorIds)
            {
                Debug.Write(anchorId + " \n");
                int[] pos = DeviceListFunctions.DeviceGetCoords(anchorId);
                Debug.Write(pos[0] + " \n");
                Debug.Write(pos[1] + " \n");
                Debug.Write(pos[2] + " \n\n");

            }

            Debug.Write("Set Interval\n");
            ConfigurationRegisters.PosInterval(500);

            while (true)
            {
                tmp = RegisterFunctions.DoPositioning();
                await Task.Delay(TimeSpan.FromSeconds(1));
                Debug.Write("Positioning: " + tmp + "\n");

                int x = PositioningData.PosX();
                int y = PositioningData.PosY();
                int z = PositioningData.PosZ();
                Debug.Write("x: " + x + "\t y: " + y + "\t z: " + z + "\n");
            }
        }
    }
}
