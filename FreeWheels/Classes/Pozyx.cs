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
<<<<<<< HEAD

        public static string GetErrorCode()
        {
            byte[] request = { 0x4 };
            byte[] data = Request(request, 1);


            if (data.Length <= 0)
            {
                string errors = "Leeg resultaat";
                return errors;
            }
            if(data.Length > 1)
            {
                string errors = "Meer dan één error gevonden";
                return errors;
            }

            //Stores the result in a variable
            int result = data[0];

            //convert int result to hex
            string hexResult = result.ToString("X2");
            
            //Gets the variable name that is assigned to the result of the error
            string stringValue = Enum.GetName(typeof(PozyxErrorCode), result);

            if (result == 0)
            {
                return stringValue;
            }
            else
            {
                return "Error: " + stringValue + "\nHex code: 0x" + hexResult + "\n";
            }

        }

=======
>>>>>>> c0536193eafb21a3571063b6660bed350403819d
    }
}
