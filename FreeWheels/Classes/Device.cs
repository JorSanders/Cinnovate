using FreeWheels.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Classes
{
    public class Device
    {
        public byte[] Id { get; set; }
        public Position Position { get; set; }
        public RangeInfo RangeInfo { get; set; }

        public int test
        {
            get
            {
                int a = 2;
                int b = 3;
                return a +b;
            }

            set { }
        }

        public Device(byte[] id)
        {
            this.Id = id;
        }

        public void Ranging()
        {
            if (PozyxApi.DoRanging(this.Id))
            {
                this.RangeInfo = PozyxApi.GetRangeInfo(Id);
            }
        }

    }

    public class RangeInfo
    {
        int Timestamp { get; set; }
        int LastMeasurement { get; set; }
        int SignalStrength { get; set; }

        public RangeInfo() { }

        public RangeInfo(int timestamp, int lastmeasurement, int signalstrength)
        {
            this.Timestamp = timestamp;
            this.LastMeasurement = lastmeasurement;
            this.SignalStrength = signalstrength;
        }
    }
}
