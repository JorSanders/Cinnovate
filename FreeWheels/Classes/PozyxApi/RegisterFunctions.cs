using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Classes.PozyxApi
{
    public static class RegisterFunctions
    {
        
        /*
        public static bool LedCTRL()
        {


        }

        */
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

        public static List<int> RXData()
        {
            List<int> rxData = new List<int>();

            byte[] request = { 0xB4 };
            byte[] data = Connection.ReadWrite(request, 100);

            if (data.Length > 0 && data[0] == 1)
            {
                for (int i = 0; i < data.Length; i++) {
                    rxData.Add(data[i]);
                }
            }

            return rxData;
        }

        public static bool DoRanging(int networkID)
        {
            byte[] request = new byte[3];
            request[0] = 0xB5;
            BitConverter.GetBytes((UInt16)networkID).CopyTo(request, 1);
            byte[] data = Connection.ReadWrite(request, 1);

            
            return data[0] == 1;
        }

        public static bool PosSetAnchorId(int[] networkIds)
        {
            byte[] request = new byte[networkIds.Length * 2 + 1];
            request[0] = 0xB7;

            for (int i = 0; i < networkIds.Length * 2; i+=2)
            {
                BitConverter.GetBytes((UInt16)networkIds[i/2]).CopyTo(request, i + 1);
            }

            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }

        public static bool FlashReset()
        {
            byte[] request = { 0xB9 };
            byte[] data = Connection.ReadWrite(request, 1);
            
            return data[0] == 1;
        }

        public static bool FlashSave(int dataTypes, int[] regData = null)
        {
            regData = regData ?? new int[0];

            if(dataTypes == 1 && regData.Length <= 0)
            {
                return false;
            }

            byte[] request = new byte[dataTypes + 1];
            request[0] = 0xBA;

            
            request[1] = (byte)dataTypes;
            

            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }

        public static List<int> FlashDetails()
        {
            List<int> flashDetail = new List<int>();

            byte[] request = { 0xBB };
            byte[] data = Connection.ReadWrite(request, 22);

            for (int i = 0; i < data.Length; i++)
            {
                flashDetail.Add(data[i]);
            }

            return flashDetail;
        }


    }
}
