using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary.Classes
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
        }

        public void RefreshInfo(int[] info)
        {
            this.Id = info[0];
            this.Flag = info[1];
            this.X = info[2];
            this.Y = info[3];
            this.Z = info[4];
        }

    }
}
