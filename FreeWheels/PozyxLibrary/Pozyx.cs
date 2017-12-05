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
            }
            while (!_Connection.connected);

            ConfigurationRegisters  = new ConfigurationRegisters(_Connection);
            DeviceListFunctions     = new DeviceListFunctions(_Connection);
            GeneralData             = new GeneralData(_Connection);
            PositioningData         = new PositioningData(_Connection);
            RegisterFunctions       = new RegisterFunctions(_Connection);
            SensorData              = new SensorData(_Connection);
            StatusRegisters         = new StatusRegisters(_Connection);

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
            }

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
            DeviceListFunctions.CalibrateDevices(1, 30, deviceIds);
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
                await Task.Delay(2000);
                deviceListSize = GeneralData.GetDeviceListSize();
                if (deviceListSize >= minDevices)
                {
                    Debug.WriteLine(deviceListSize + " Devices found in " + (i + 1) + " tries");
                    return deviceListSize;
                }

                Debug.WriteLine("Not enough devices found: " + deviceListSize + " required: " + minDevices);
                await Task.Delay(1000);
            }
            return 0;
        }

        public async Task SetConfiguration()
        {
            ConfigurationRegisters.PosInterval(50);
            await Task.Delay(200);
            ConfigurationRegisters.PosAlg(4, 3);
            await Task.Delay(200);
            ConfigurationRegisters.PosFilter(5, 3);
            await Task.Delay(200);
            ConfigurationRegisters.RangeProtocol(1);
            await Task.Delay(200);
            //ConfigurationRegisters.UwbPlen(8);
            //ConfigurationRegisters.UwbRates(0, 2);
            //await Task.Delay(1000);
            int[] posAlg = ConfigurationRegisters.PosAlg();
            Debug.WriteLine("PosAlg: " + posAlg[0] + " + " + posAlg[1]);

            string err = StatusRegisters.ErrorCode();
            if (err != "0x00 - Success")
            {
                Debug.WriteLine("ERROR: " + err);
            }

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

        public async Task ManualAnchorsSetup()
        {
            //DeviceListFunctions.DeviceAdd(0x605B, 1, 0, 0, 500);
            //await Task.Delay(200);
            //DeviceListFunctions.DeviceAdd(0x6038, 1, 7000, 0, 2000);
            //await Task.Delay(200);
            //DeviceListFunctions.DeviceAdd(0x6029, 1, 0, 5100, 500);
            //await Task.Delay(200);
            //DeviceListFunctions.DeviceAdd(0x6047, 1, 6750, 5100, 10);
            //await Task.Delay(200);

            AddAnchor(0x605B, 1, 0, 0, 500);
            await Task.Delay(200);
            AddAnchor(0x6038, 1, 7000, 0, 2000);
            await Task.Delay(200);
            AddAnchor(0x6029, 1, 0, 5100, 500);
            await Task.Delay(200);
            AddAnchor(0x6047, 1, 6750, 5100, 10);
            await Task.Delay(200);

            ConfigurationRegisters.PosInterval(50);
            await Task.Delay(200);
            ConfigurationRegisters.PosAlg(4, 3);
            await Task.Delay(200);
            ConfigurationRegisters.PosFilter(10, 4);
            await Task.Delay(200);
            ConfigurationRegisters.RangeProtocol(1);
            await Task.Delay(200);
            ConfigurationRegisters.UwbPlen(8);
            await Task.Delay(200);
            ConfigurationRegisters.UwbRates(0, 2);
            await Task.Delay(200);

        }



    }
}
