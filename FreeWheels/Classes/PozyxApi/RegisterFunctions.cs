using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Classes.PozyxApi
{
    public static class RegisterFunctions
    {
        
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

    }
}
