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
        private IConnection _Connection; // Pozyx connection object. 

        // Register header objects
        public ConfigurationRegisters ConfigurationRegisters;
        public DeviceListFunctions DeviceListFunctions;
        public GeneralData GeneralData;
        public PositioningData PositioningData;
        public RegisterFunctions RegisterFunctions;
        public SensorData SensorData;
        public StatusRegisters StatusRegisters;

        // The anchor list (positioning beacons)
        public List<Anchor> Anchors;

        public Pozyx()
        {
            Anchors = new List<Anchor>();
        }

        /// <summary>
        ///     Keep trying to connect to I2C. 
        ///     TODO: set timeout
        /// </summary>
        /// <returns></returns>
        public async Task ConnectI2c()
        {
            _Connection = new ConnectionI2c();

            do
            {
                await _Connection.Connect();
            }
            while (!_Connection.connected);

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
        public async Task<bool> DoAnchorDiscovery(int minAnchors = 4)
        {
            /* 
             * TODO: implement this. 
             * Its not implemented because I dont know how to set a maximum number of anchors through the Pozyx api
             * Maybe idleslots, havent tested it.
             * Pozyx can calibrate up to 6 anchors thats why its set to 6
             * */
            int maxAnchors = 6;

            if (!ClearDevices())
            {
                return false;
            }

            // Check the devicelist size
            int deviceListSize = await DiscoverDevices(10, 4, 0);

            if (deviceListSize < minAnchors)
            {
                return false;
            }

            // Get the deviceIds
            int[] deviceIds = DeviceListFunctions.DevicesGetIds(0, deviceListSize);

            // Set the anchor height on 1800 and the flag to 8
            foreach (int deviceId in deviceIds)
            {
                AddAnchor(deviceId, 8, 0, 0, 1800);
            }

            // Calibrate the anchors wait 10 seconds
            await Task.Delay(TimeSpan.FromSeconds(1));
            DeviceListFunctions.CalibrateDevices(1, 10 /*, deviceIds */);
            await Task.Delay(TimeSpan.FromSeconds(10));

            // Refresh the anchor info
            foreach (Anchor anchor in Anchors)
            {
                RefreshAnchorInfo(anchor);
            }

            RegisterFunctions.PosSetAnchorIds(deviceIds);

            // setting numAnchors always results in pozyx raising the error not enough anchors.
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
        /// <returns> number of devices found. -1 means an error or not enough devices found</returns>
        public async Task<int> DiscoverDevices(int discoverAttempts = 10, int minDevices = 4, int deviceType = 0, int idleSlots = 3, int idleSlotDuration = 10)
        {
            bool discoverSuccess;

            int deviceListSize;

            for (int i = 0; i < discoverAttempts; i++)
            {
                discoverSuccess = DeviceListFunctions.DevicesDiscover(deviceType, idleSlots, idleSlotDuration);

                if (!discoverSuccess)
                {
                    return -1;
                }

                await Task.Delay(2000);
                deviceListSize = GeneralData.GetDeviceListSize();
                if (deviceListSize >= minDevices)
                {
                    return deviceListSize;
                }

                // Not enough deviced found wait 1 second and try again
                await Task.Delay(1000);
            }

            // Not enough devices found and out of attempts
            return -1;
        }

        /// <summary>
        ///     Set the configurations how we recommend them
        /// </summary>
        /// <returns></returns>
        public async Task SetRecommendedConfigurations()
        {
            ConfigurationRegisters.PosInterval(0, 0); // We call DoPositioning every time we want an update. So we dont set an update interval
            await Task.Delay(200);
            ConfigurationRegisters.PosAlg(4, 3, 0); // 3d positioning and tracking
            await Task.Delay(200);
            ConfigurationRegisters.PosFilter(0, 0, 0); // Dont filter, so we can see the results most clearly. 
            await Task.Delay(200);
            ConfigurationRegisters.RangeProtocol(0, 0); // Set to its default value
            await Task.Delay(200);
            ConfigurationRegisters.UwbPlen(8, 0); // Set to its default value
            await Task.Delay(200);
            ConfigurationRegisters.UwbRates(0, 2, 0); // Set to its default value
            await Task.Delay(200);
        }

        /// <summary>
        ///     Adds a pozyx anchor to your anchor list.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="flag"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public bool AddAnchor(int id, int flag, int x, int y, int z, int remoteId = 0)
        {
            if (!DeviceListFunctions.DeviceAdd(id, flag, x, y, z, remoteId))
            {
                return false;
            }

            if (remoteId == 0)
            {
                Anchors.Add(new Anchor(id, flag, x, y, z));
            }

            return true;
        }

        /// <summary>
        ///     Update the information about an anchor. Should be called after calibrating, because then the device coordinates will change
        /// </summary>
        /// <param name="anchor"></param>
        public void RefreshAnchorInfo(Anchor anchor)
        {
            int[] info = DeviceListFunctions.DeviceGetInfo(anchor.Id);
            anchor.RefreshInfo(info);
        }

        // Clear the devicelist
        public bool ClearDevices()
        {
            bool success = DeviceListFunctions.DevicesClear();

            if (success)
            {
                this.Anchors = new List<Anchor>();
            }

            return success;
        }

        /// <summary>
        ///     Retrieves the anchors from the Pozyx flash
        /// </summary>
        public void RetrieveAnchors()
        {
            int[] deviceIds = RegisterFunctions.PosGetAnchorIds().ToArray();

            int[] deviceInfo;

            foreach (int deviceId in deviceIds)
            {
                deviceInfo = DeviceListFunctions.DeviceGetInfo(deviceId);

                AddAnchor(deviceInfo[0], deviceInfo[1], deviceInfo[2], deviceInfo[3], deviceInfo[4]);
            }
        }
    }
}
