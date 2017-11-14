using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Classes.PozyxApi
{
    public static class RegisterFunctions
    {

        /// <summary>
        ///     Calling this function resets the Pozyx device.
        ///     This also clears the device list and returns the settings to their defualt state (including UWB settings)
        /// </summary>
        /// <returns></returns>
        public static bool ResetSys()
        {
            byte[] request = { 0xB0 };
            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }

        public static bool TXData(int offset, byte[] dataBytes)
        {
            byte[] request = new byte[dataBytes.Length + 2];
            request[0] = 0xB2;
            request[1] = (byte)offset;
            dataBytes.CopyTo(request, 2);

            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }

        public static bool TXSend(int networkID, byte[] dataBytes)
        {
            byte[] request = new byte[] { 0xB3, 0x60, 0x5B, 0x02 };

            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }

        public static void RXData(byte[] dataBytes)
        {

        }

        public static bool DoPositioning()
        {
            byte[] request = { 0xB6 };
            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }

        public static List<int> GetAnchorIds()
        {
            byte[] request = { 0xB8 };
            byte[] data = Connection.ReadWrite(request, 33);

            List<int> anchorIds = new List<int>();

            for (int i = 1; i + 1 < data.Length; i += 2)
            {
                if (!(data[i] == 0 && data[i + 1] == 0))
                {
                    int id = BitConverter.ToUInt16(new byte[] { data[i], data[i + 1] }, 0);
                    anchorIds.Add(id);
                }
            }

            return anchorIds;
        }

    }
}
