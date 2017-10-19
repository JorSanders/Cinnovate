using FreeWheels.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Classes
{
    public class Tag
    {
        public Position Position
        {
            get
            {
                // Do not call Tag.Position.x then Tag.Position.Y because this way you will make two calls for the position
                // You should first do Position myPos = Tag.position then call myPos.x and myPos.y
                return new Position(PozyxApi.PosX(), PozyxApi.PosY(), PozyxApi.PosZ());
            }
        }

    }
}
