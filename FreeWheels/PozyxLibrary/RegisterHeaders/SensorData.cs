using FreeWheels.PozyxLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary.RegisterHeaders
{
    public class SensorData : RegisterHeaders
    {
        public SensorData(IConnection connection) : base(connection)
        {
        }

        /// <summary>
        ///     This register contains the maximum measured norm of the 3D linear acceleration.
        ///     This value is reset after reading the register.
        ///     The sensor data is represented as an unsigned 16-bit integer.
        ///     1mg = 1 int.
        /// </summary>
        /// <returns>Maximum linear acceleration</returns>
        public int MaxLinAcc(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x4E, 2, null, remoteId);

            return BitConverter.ToUInt16(data, 0);
        }

        /// <summary>
        ///     This register holds the pressure exerted on the pozyx device.
        ///     At sealevel the pressure is The pressure is stored as an unsigned 32-bit integer.
        ///     1mPa = 1 int.
        /// </summary>
        /// <returns>Pressure data</returns>
        public UInt32 Pressure(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x50, 4, null, remoteId);

            return BitConverter.ToUInt32(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated acceleration in the x-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Accelerometer data (in mg)</returns>
        public int AccelX(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x54, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated acceleration in the y-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Accelerometer data (in mg)</returns>
        public int AccelY(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x56, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated acceleration in the z-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Accelerometer data (in mg)</returns>
        public int AccelZ(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x58, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the magnetic field strength along the x-axis of the Pozyx device.
        ///     The sensor data is stored as a signed 16-bit integer.
        ///     1µT = 16 int.
        /// </summary>
        /// <returns>Magnemtometer data</returns>
        public int MagnX(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x5A, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the magnetic field strength along the y-axis of the Pozyx device.
        ///     The sensor data is stored as a signed 16-bit integer.
        ///     1µT = 16 int.
        /// </summary>
        /// <returns>Magnemtometer data</returns>
        public int MagnY(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x5C, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the magnetic field strength along the z-axis of the Pozyx device.
        ///     The sensor data is stored as a signed 16-bit integer.
        ///     1µT = 16 int.
        /// </summary>
        /// <returns>Magnemtometer data</returns>
        public int MagnZ(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x5E, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated angular velocity for the x-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Gyroscope data</returns>
        public int GyroX(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x60, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated angular velocity for the y-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Gyroscope data</returns>
        public int GyroY(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x62, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated angular velocity for the z-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Gyroscope data</returns>
        public int GyroZ(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x64, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the absolute heading or yaw of the device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Euler angles heading (or yaw)</returns>
        public int EulHeading(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x66, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the roll of the device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Euler angles roll</returns>
        public int EulRoll(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x68, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the pitch of the device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Euler angles pitch</returns>
        public int EulPitch(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x6A, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The orientation can be represented using quaternions (w, x, y, z).
        ///     This register contains the weight w and is represented as a signed 16-bit integer.
        ///     1 quaternion(unit less) = 2^14 int = 16384 int.
        /// </summary>
        /// <returns>Weight of quaternion</returns>
        public int QuatW(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x6C, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The orientation can be represented using quaternions (w, x, y, z).
        ///     This register contains the x and is represented as a signed 16-bit integer.
        ///     1 quaternion(unit less) = 2^14 int = 16384 int.
        /// </summary>
        /// <returns>x of quaternion</returns>
        public int QuatX(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x6E, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The orientation can be represented using quaternions (w, x, y, z).
        ///     This register contains the y and is represented as a signed 16-bit integer.
        ///     1 quaternion(unit less) = 2^14 int = 16384 int.
        /// </summary>
        /// <returns>y of quaternion</returns>
        public int QuatY(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x70, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The orientation can be represented using quaternions (w, x, y, z).
        ///     This register contains the z and is represented as a signed 16-bit integer.
        ///     1 quaternion(unit less) = 2^14 int = 16384 int.
        /// </summary>
        /// <returns>z of quaternion</returns>
        public int QuatZ(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x72, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The linear acceleration is the total acceleration minus the gravity.
        ///     The linear acceleration expressed the acceleration due to movement.
        ///     This register holds the linear acceleration along the x-axis of the pozyx device (i.e. body coordinates).
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Linear acceleration in x-direction</returns>
        public int LiaX(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x74, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The linear acceleration is the total acceleration minus the gravity.
        ///     The linear acceleration expressed the acceleration due to movement.
        ///     This register holds the linear acceleration along the y-axis of the pozyx device (i.e. body coordinates).
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Linear acceleration in y-direction</returns>
        public int LiaY(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x76, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The linear acceleration is the total acceleration minus the gravity.
        ///     The linear acceleration expressed the acceleration due to movement.
        ///     This register holds the linear acceleration along the z-axis of the pozyx device (i.e. body coordinates).
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Linear acceleration in z-direction</returns>
        public int LiaZ(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x78, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     Gravity is a force of 1g( = 9.80665 m/s2 ) directed towards the ground.
        ///     This register represents the gravity component in the x-axis and is represented by a singed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>x-component of gravity vector</returns>
        public int GravX(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x7A, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     Gravity is a force of 1g( = 9.80665 m/s2 ) directed towards the ground.
        ///     This register represents the gravity component in the y-axis and is represented by a singed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>y-component of gravity vector</returns>
        public int GravY(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x7C, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     Gravity is a force of 1g( = 9.80665 m/s2 ) directed towards the ground.
        ///     This register represents the gravity component in the z-axis and is represented by a singed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>z-component of gravity vector</returns>
        public int GravZ(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x7E, 2, null, remoteId);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     Read out the internal chip temperature.
        ///     This is loosely related to the ambient room temperature.
        ///     For more accurate ambient temperature measurements, it is recommended to use a separate sensor.
        /// </summary>
        /// <returns>Temperature</returns>
        public int Temperature(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x80, 1, null, remoteId);

            return data[0];
        }
    }
}
