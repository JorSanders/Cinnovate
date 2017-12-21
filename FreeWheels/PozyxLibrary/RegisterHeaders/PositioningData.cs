using FreeWheels.PozyxLibrary.Classes;
using FreeWheels.PozyxLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary.RegisterHeaders
{
    public class PositioningData
    {

        private RegisterFunctions registerFunctions;
        private IConnection Connection;

        public PositioningData(IConnection pozyxConnection)
        {
            Connection = pozyxConnection;
            registerFunctions = new RegisterFunctions(pozyxConnection);
        }

        /// <summary>
        ///     x-coordinate of the device in mm.
        /// </summary>
        /// <returns></returns>
        public int PosX()
        {
            byte[] request = { 0x30 };
            byte[] data = Connection.ReadWrite(request, 4);

            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        ///     y-coordinate of the device in mm.
        /// </summary>
        /// <returns></returns>
        public int PosY()
        {
            byte[] request = { 0x34 };
            byte[] data = Connection.ReadWrite(request, 4);

            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        ///     z-coordinate of the device in mm.
        /// </summary>
        /// <returns></returns>
        public int PosZ()
        {
            byte[] request = { 0x38 };
            byte[] data = Connection.ReadWrite(request, 4);

            return BitConverter.ToInt32(data, 0);
        }

        async public Task<Position> Pos(int remoteId = 0)
        {
            byte header = 0x30;
            byte numReturnBytes = 12;
            byte[] data;

            if (remoteId > 0)
            {
                // friend get pos
                bool txd = registerFunctions.TXData(0, new byte[] { header, numReturnBytes });
                //if (!txd) { return txd; };
                await Task.Delay(200);
                bool txs = registerFunctions.TXSend(remoteId, 2);
                //if (!txs) { return txd; };
                await Task.Delay(1000);
                data = registerFunctions.RXData();

                //Sorry ill fix later i promise
                data = new byte[] { data[1], data[2], data[3], data[4], data[5], data[6], data[7], data[8], data[9], data[10], data[11], data[12] };
            }
            else
            {
                byte[] request = { header };
                data = Connection.ReadWrite(request, numReturnBytes);
            }

            return new Position(BitConverter.ToInt32(data, 0), BitConverter.ToInt32(data, 4), BitConverter.ToInt32(data, 8));
        }

        /// <summary>
        ///     estimated error covariance of x
        /// </summary>
        /// <returns></returns>
        public int PosErrX()
        {
            byte[] request = { 0x3C };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     estimated error covariance of y
        /// </summary>
        /// <returns></returns>
        public int PosErrY()
        {
            byte[] request = { 0x3E };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     estimated error covariance of z
        /// </summary>
        /// <returns></returns>
        public int PosErrZ()
        {
            byte[] request = { 0x40 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     estimated covariance of xy
        /// </summary>
        /// <returns></returns>
        public int PosErrXY()
        {
            byte[] request = { 0x42 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        /// 	estimated covariance of xz
        /// </summary>
        /// <returns></returns>
        public int PosErrXZ()
        {
            byte[] request = { 0x44 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        /// 	estimated covariance of YZ
        /// </summary>
        /// <returns></returns>
        public int PosErrYZ()
        {
            byte[] request = { 0x46 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }
    }
}
