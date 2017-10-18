using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Classes
{
    class Pozyx
    {

        public List<Anchor> Anchors;
        public Tag MyPozyx;

        public Pozyx()
        {
            Init();
        }

        public bool Init()
        {
            if (!PozyxApi.DiscoverDevices())
            {
                return false;
            }

            if (!PozyxApi.StartPositioning())
            {
                return false;
            }

            if (!PozyxApi.CalibrateDevices())
            {
                return false;
            }

            List<byte[]> anchorIds = PozyxApi.GetAnchorIds();

            Anchors = new List<Anchor>();

            foreach (byte[] anchorId in anchorIds)
            {
                Anchors.Add(new Anchor(anchorId));
            }

            foreach (Anchor anchor in Anchors)
            {
                anchor.Position = PozyxApi.GetAnchorPosition(anchor.Id);
            }

            Position tmp = MyPozyx.Position;

            return true;
        }
    }
}
