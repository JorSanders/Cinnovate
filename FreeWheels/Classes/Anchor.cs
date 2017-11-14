using FreeWheels.Classes.PozyxApi;
using FreeWheels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Classes
{
    public class Anchor
    {
        public byte[] Id { get; set; }
        public Position Position { get; set; }

        public RangeInfo RangeInfo { get; set; }


        public Anchor(byte[] id)
        {
            this.Id = id;
        }

        public void Ranging()
        {

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
