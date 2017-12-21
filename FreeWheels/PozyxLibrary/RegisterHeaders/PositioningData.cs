using FreeWheels.PozyxLibrary.Classes;
using FreeWheels.PozyxLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary.RegisterHeaders
{
    public class PositioningData : RegisterHeaders
    {
        public PositioningData(IConnection connection) : base(connection)
        {
        }

        /// <summary>
        ///     x-coordinate of the device in mm.
        /// </summary>
        /// <returns></returns>
        public int PosX(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x30, 4, null, remoteId);

            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        ///     y-coordinate of the device in mm.
        /// </summary>
        /// <returns></returns>
        public int PosY(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x34, 4, null, remoteId);

            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        ///     z-coordinate of the device in mm.
        /// </summary>
        /// <returns></returns>
        public int PosZ(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x38, 4, null, remoteId);

            return BitConverter.ToInt32(data, 0);
        }

        public Position Pos(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x30, 12, null, remoteId);
            return new Position(BitConverter.ToInt32(data, 0), BitConverter.ToInt32(data, 4), BitConverter.ToInt32(data, 8));
        }

        /// <summary>
        ///     estimated error covariance of x
        /// </summary>
        /// <returns></returns>
        public int PosErrX(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x3C, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     estimated error covariance of y
        /// </summary>
        /// <returns></returns>
        public int PosErrY(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x3E, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     estimated error covariance of z
        /// </summary>
        /// <returns></returns>
        public int PosErrZ(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x40, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     estimated covariance of xy
        /// </summary>
        /// <returns></returns>
        public int PosErrXY(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x42, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        /// 	estimated covariance of xz
        /// </summary>
        /// <returns></returns>
        public int PosErrXZ(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x44, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        /// 	estimated covariance of YZ
        /// </summary>
        /// <returns></returns>
        public int PosErrYZ(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x46, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }
    }
}
