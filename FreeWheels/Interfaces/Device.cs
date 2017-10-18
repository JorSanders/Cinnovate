using FreeWheels.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Interfaces
{
    public interface Device
    {
        byte[] Id { get; set; }
        Position Position { get; set; }
        int RangeInfo { get; set; }
    }
}
