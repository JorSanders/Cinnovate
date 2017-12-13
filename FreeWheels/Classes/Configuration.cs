using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Classes
{
    public class Configuration
    {
        public int initStatus;

        public class IntMask
        {
            public bool Err { get; set; }
            public bool Pos { get; set; }
            public bool Imu { get; set; }
            public bool RxData { get; set; }
            public bool Func { get; set; }
            public bool Pin { get; set; }
        }

        public class IntConfig
        {
            public int PinNum { get; set; }
            public bool Mode { get; set; }
            public bool Act { get; set; }
            public bool Latch { get; set; }
        }

        public class PosFilter
        {
            public int Strength { get; set; }
            public int Filter { get; set; }
        }

        public class ConfigLeds
        {
            public bool Led1 { get; set; }
            public bool Led2 { get; set; }
            public bool Led3 { get; set; }
            public bool Led4 { get; set; }
            public bool LedRx { get; set; }
            public bool LedTx { get; set; }
        }

        public class PosAlg
        {
            public int Algorithm { get; set; }
            public int Dim { get; set; }
        }

        public class NumAnchors
        {
            public int Num { get; set; }
            public bool Mode { get; set; }
        }

        public int Interval { get; set; }

        public int NetworkId { get; set; }

        public int UwbChannel { get; set; }

        public class UwbRates
        {
            public int Bitrate { get; set; }
            public int Prf { get; set; }
        }

        public int UwbPlen { get; set; }

        public int UwbGain { get; set; }

        public int UwbXtalTrim { get; set; }

        public int RangeProtocol { get; set; }

        public int OperationMode { get; set; }

        public int SensorsMode { get; set; }

        public class ConfigGpio1
        {
            public int Mode { get; set; }
            public int Pull { get; set; }
        }

        public class ConfigGpio2
        {
            public int Mode { get; set; }
            public int Pull { get; set; }
        }

        public class ConfigGpio3
        {
            public int Mode { get; set; }
            public int Pull { get; set; }
        }

        public class ConfigGpio4
        {
            public int Mode { get; set; }
            public int Pull { get; set; }
        }

    }
}
