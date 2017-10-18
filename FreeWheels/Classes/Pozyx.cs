using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreeWheels.Api;

namespace FreeWheels.Classes
{
    class Pozyx
    {

        public List<Device> Anchors = new List<Device>();
        public Pozyx()
        {
            PozyxApi.Connect();
            Init();
        }

        public bool Init()
        {
            PozyxApi.DiscoverDevices();

            PozyxApi.StartPositioning();

            PozyxApi.CalibrateDevices();

            List<byte[]> anchorIds = PozyxApi.GetAnchorIds();

            foreach (byte[] anchorId in anchorIds)
            {
                Anchors.Add(new Device(anchorId));
            }

            foreach (Device anchor in Anchors)
            {
                anchor.Position = PozyxApi.GetAnchorPosition(anchor.Id);
            }
         
            return true;
        }
    }
}
