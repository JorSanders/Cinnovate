using FreeWheels.Classes.PozyxApi;
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
            Connection.Connect();
            MyPozyx = new Tag();
        }

        public bool Init()
        {
            if (!PozyxApiBase.DiscoverDevices())
            {
                return false;
            }

            if (!PozyxApiBase.StartPositioning())
            {
                return false;
            }

            if (!PozyxApiBase.CalibrateDevices())
            {
                return false;
            }

            List<byte[]> anchorIds = PozyxApiBase.GetAnchorIds();

            Anchors = new List<Anchor>();

            foreach (byte[] anchorId in anchorIds)
            {
                Anchors.Add(new Anchor(anchorId));
            }

            foreach (Anchor anchor in Anchors)
            {
                anchor.Position = PozyxApiBase.GetAnchorPosition(anchor.Id);
            }

            MyPozyx = new Tag();

            return true;
        }

        public bool Reset()
        {
            Anchors = new List<Anchor>();
            MyPozyx = new Tag();
            return PozyxApiBase.Reset();
        }
    }
}
