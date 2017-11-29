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

        private IConnection Connection;

        public PositioningData(IConnection pozyxConnection)
        {
            Connection = pozyxConnection;
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
