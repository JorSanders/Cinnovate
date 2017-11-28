using FreeWheels.PozyxLibrary.Classes;
using FreeWheels.PozyxLibrary.Interfaces;
using FreeWheels.PozyxLibrary.RegisterHeaders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary
{
    public class Pozyx
    {
        private IConnection _Connection;
        public ConfigurationRegisters ConfigurationRegisters;
        public DeviceListFunctions DeviceListFunctions;
        public GeneralData GeneralData;
        public PositioningData PositioningData;
        public RegisterFunctions RegisterFunctions;
        public SensorData SensorData;
        public StatusRegisters StatusRegisters;

        public List<Anchor> Anchors;

        public Pozyx()
        {
            Anchors = new List<Anchor>();
        }

        public async Task ConnectI2c()
        {
            _Connection = new ConnectionI2c();
            do
            {
                await _Connection.Connect();
            } while (!_Connection.connected);

            ConfigurationRegisters = new ConfigurationRegisters(_Connection);
            DeviceListFunctions = new DeviceListFunctions(_Connection);
            GeneralData = new GeneralData(_Connection);
            PositioningData = new PositioningData(_Connection);
            RegisterFunctions = new RegisterFunctions(_Connection);
            SensorData = new SensorData(_Connection);
            StatusRegisters = new StatusRegisters(_Connection);
        }


        /// <summary>
        ///     Discover up to 6 anchors in range. And calibrate them. Carefull this wipes the current anchorList
        /// </summary>
        /// <param name="minAnchors"></param>
        /// <returns>False if not enough anchors are found. True on succes</returns>
        public async Task<bool> SetAnchors(int minAnchors = 4)
        {
            bool succes;
            int nAnchors = 6; // TODO implement this. Maybe idleslots// Pozyx can calibrate up to 6 anchors

            // Clear the anchors
            Anchors = new List<Anchor>();
            succes = DeviceListFunctions.DevicesClear();
            Debug.WriteLine("Devicelist clear: " + (succes ? "succes" : "failed"));

            // Check the devicelist size
            int deviceListSize = await DiscoverDevices(10, 4, 0);

            if (deviceListSize < minAnchors)
            {
                Debug.WriteLine("Not enough devices discovered");
                return false;
            };

            // Get the deviceIds
            int[] deviceIds = DeviceListFunctions.DevicesGetIds(0, deviceListSize);

            // Set the anchor height on 1800 and the flag to 8
            foreach (int deviceId in deviceIds)
            {
                if(!AddAnchor(deviceId, 8, 0, 0, 1800))
                {
                    return false;
                }
            }

            // Calibrate the anchors
            await Task.Delay(TimeSpan.FromSeconds(1));
            DeviceListFunctions.CalibrateDevices(1, 10, deviceIds);
            Debug.WriteLine("Calibrating... ");
            await Task.Delay(TimeSpan.FromSeconds(4));

            // Refresh the anchor update and print the positions
            foreach (Anchor anchor in Anchors)
            {
                RefreshAnchorInfo(anchor);
                Debug.Write("Id: " + anchor.Id + "\t x:" + anchor.X + "\t y:" + anchor.Y + "\t z:" + anchor.Z + "\n");
            }

            RegisterFunctions.PosSetAnchorIds(deviceIds);
            //ConfigurationRegisters.PosNumAnchors(this.Anchors.Count, 0);

            return true;
        }

        /// <summary>
        ///     Tries to discover enough devices
        /// </summary>
        /// <param name="discoverAttempts">How many times should we try to discover</param>
        /// <param name="minDevices">How many times devices do we minimally need</param>
        /// <param name="deviceType">Pozyx device type</param>
        /// <param name="idleSlots">Pozyx idle slot</param>
        /// <param name="idleSlotDuration">Pozyx idle slot duration</param>
        /// <returns></returns>
        public async Task<int> DiscoverDevices(int discoverAttempts = 10, int minDevices = 4, int deviceType = 0, int idleSlots = 3, int idleSlotDuration = 10)
        {
            bool DiscoverSuccess;

            int deviceListSize;

            for (int i = 0; i < discoverAttempts; i++)
            {
                DiscoverSuccess = DeviceListFunctions.DevicesDiscover(deviceType, idleSlots, idleSlotDuration);
                Debug.WriteLine("Discover devices: " + (DiscoverSuccess ? "Success" : "Failed"));
                await Task.Delay(TimeSpan.FromSeconds(2));
                deviceListSize = GeneralData.GetDeviceListSize();
                if (deviceListSize >= minDevices)
                {
                    Debug.WriteLine(deviceListSize + " Devices found in " + (i + 1) + " tries");
                    return deviceListSize;
                }
            }
            return 0;
        }

        public void SetConfiguration()
        {
            ConfigurationRegisters.PosInterval(400);
            ConfigurationRegisters.PosAlg(0, 3);
            ConfigurationRegisters.PosFilter(15, 1);
        }

        public bool AddAnchor(int id, int flag, int x, int y, int z)
        {
            if (!DeviceListFunctions.DeviceAdd(id, flag, x, y, z))
            {
                return false;
            }

            Anchors.Add(new Anchor(id, flag, x, y, z));

            return true;
        }

        public void RefreshAnchorInfo(Anchor anchor)
        {
            int[] info = DeviceListFunctions.DeviceGetInfo(anchor.Id);
            anchor.RefreshInfo(info);
        }
        public async Task LetsGo()
        {

            Debug.WriteLine("INFO: Flash Reset - " + (RegisterFunctions.FlashReset() ? "SUCCESS" : "FAILURE"));
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

            ConfigurationRegisters.PosFilter(15, 3);
            Debug.WriteLine("INFO: Set Position Filter - 15, 4");
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
