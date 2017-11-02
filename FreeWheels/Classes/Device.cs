using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Classes
{
    public class Device
    {

        public int NetworkID { get; set; }
        public int Flag { get; set; }

        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public Device(int networkID)
        {
            this.NetworkID = networkID;
        }

        public void GetDeviceInfo()
        {
            DeviceInfo deviceInfo = PozyxApi.DeviceGetInfo(this.NetworkID);
            this.Flag = deviceInfo.Flag;
            this.X = deviceInfo.X;
            this.Y = deviceInfo.Y;
            this.Z = deviceInfo.Z;
        }

    }

    public class DeviceInfo
    {

        public int Flag { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Z { get; set; }

        public DeviceInfo() { }

        public DeviceInfo(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public DeviceInfo(int flag, int x, int y, int z)
        {
            this.Flag = flag;
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

    }

}
