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
        public int rangeInfo { get; set; }

        public Device(byte[] id)
        {
            this.Id = id;
        }

        //Pozyx.getRangeInfo(Id);

    }
}
