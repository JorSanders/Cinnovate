using FreeWheels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Classes
{
    public class Tag : Device
    {
        public byte[] Id { get; set; }
        public Position Position { get; set; }
        public int RangeInfo { get; set; }

        public Position getPosition()
        {
            return new Position();
        }
    }
}
