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
        public int Id { get; private set; }
        public int Flag { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        public Anchor(int id, int flag, int x, int y, int z)
        {
            Id = id;
            Flag = flag;
            X = x;
            Y = y;
            Z = z;

            DeviceListFunctions.DeviceAdd(id, flag, x, y, z);
        }

        public void RefreshInfo()
        {
            int[] info = DeviceListFunctions.DeviceGetInfo(Id);
            this.Id = info[0];
            this.Flag = info[1];
            this.X = info[2];
            this.Y = info[3];
            this.Z = info[4];
        }

    }
}
