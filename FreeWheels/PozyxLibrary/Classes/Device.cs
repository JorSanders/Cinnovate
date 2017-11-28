using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary.Classes
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
    }
}
