using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Classes.PozyxApi
{
    public static class SensorData
    {
        /// <summary>
        ///     This register contains the maximum measured norm of the 3D linear acceleration.
        ///     This value is reset after reading the register.
        ///     The sensor data is represented as an unsigned 16-bit integer.
        ///     1mg = 1 int.
        /// </summary>
        /// <returns>Maximum linear acceleration</returns>
        public static int MaxLinAcc()
        {
            byte[] request = { 0x4E };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToUInt16(data, 0);
        }

        /// <summary>
        ///     This register holds the pressure exerted on the pozyx device.
        ///     At sealevel the pressure is The pressure is stored as an unsigned 32-bit integer.
        ///     1mPa = 1 int.
        /// </summary>
        /// <returns>Pressure data</returns>
        public static UInt32 Pressure()
        {
            byte[] request = { 0x50 };
            byte[] data = Connection.ReadWrite(request, 4);

            return BitConverter.ToUInt32(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated acceleration in the x-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Accelerometer data (in mg)</returns>
        public static int AccelX()
        {
            byte[] request = { 0x54 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated acceleration in the y-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Accelerometer data (in mg)</returns>
        public static int AccelY()
        {
            byte[] request = { 0x56 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated acceleration in the z-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Accelerometer data (in mg)</returns>
        public static int AccelZ()
        {
            byte[] request = { 0x58 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the magnetic field strength along the x-axis of the Pozyx device.
        ///     The sensor data is stored as a signed 16-bit integer.
        ///     1µT = 16 int.
        /// </summary>
        /// <returns>Magnemtometer data</returns>
        public static int MagnX()
        {
            byte[] request = { 0x5A };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the magnetic field strength along the y-axis of the Pozyx device.
        ///     The sensor data is stored as a signed 16-bit integer.
        ///     1µT = 16 int.
        /// </summary>
        /// <returns>Magnemtometer data</returns>
        public static int MagnY()
        {
            byte[] request = { 0x5C };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the magnetic field strength along the z-axis of the Pozyx device.
        ///     The sensor data is stored as a signed 16-bit integer.
        ///     1µT = 16 int.
        /// </summary>
        /// <returns>Magnemtometer data</returns>
        public static int MagnZ()
        {
            byte[] request = { 0x5E };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated angular velocity for the x-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Gyroscope data</returns>
        public static int GyroX()
        {
            byte[] request = { 0x60 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated angular velocity for the y-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Gyroscope data</returns>
        public static int GyroY()
        {
            byte[] request = { 0x62 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the offset compensated angular velocity for the z-axis of the Pozyx device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Gyroscope data</returns>
        public static int GyroZ()
        {
            byte[] request = { 0x64 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the absolute heading or yaw of the device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Euler angles heading (or yaw)</returns>
        public static int EulHeading()
        {
            byte[] request = { 0x66 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the roll of the device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Euler angles roll</returns>
        public static int EulRoll()
        {
            byte[] request = { 0x68 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     This register contains the pitch of the device.
        ///     The sensor data is represented as a signed 16-bit integer.
        ///     1degree = 16 int.
        /// </summary>
        /// <returns>Euler angles pitch</returns>
        public static int EulPitch()
        {
            byte[] request = { 0x6A };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The orientation can be represented using quaternions (w, x, y, z).
        ///     This register contains the weight w and is represented as a signed 16-bit integer.
        ///     1 quaternion(unit less) = 2^14 int = 16384 int.
        /// </summary>
        /// <returns>Weight of quaternion</returns>
        public static int QuatW()
        {
            byte[] request = { 0x6C };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The orientation can be represented using quaternions (w, x, y, z).
        ///     This register contains the x and is represented as a signed 16-bit integer.
        ///     1 quaternion(unit less) = 2^14 int = 16384 int.
        /// </summary>
        /// <returns>x of quaternion</returns>
        public static int QuatX()
        {
            byte[] request = { 0x6E };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The orientation can be represented using quaternions (w, x, y, z).
        ///     This register contains the y and is represented as a signed 16-bit integer.
        ///     1 quaternion(unit less) = 2^14 int = 16384 int.
        /// </summary>
        /// <returns>y of quaternion</returns>
        public static int QuatY()
        {
            byte[] request = { 0x70 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The orientation can be represented using quaternions (w, x, y, z).
        ///     This register contains the z and is represented as a signed 16-bit integer.
        ///     1 quaternion(unit less) = 2^14 int = 16384 int.
        /// </summary>
        /// <returns>z of quaternion</returns>
        public static int QuatZ()
        {
            byte[] request = { 0x72 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The linear acceleration is the total acceleration minus the gravity.
        ///     The linear acceleration expressed the acceleration due to movement.
        ///     This register holds the linear acceleration along the x-axis of the pozyx device (i.e. body coordinates).
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Linear acceleration in x-direction</returns>
        public static int LiaX()
        {
            byte[] request = { 0x74 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The linear acceleration is the total acceleration minus the gravity.
        ///     The linear acceleration expressed the acceleration due to movement.
        ///     This register holds the linear acceleration along the y-axis of the pozyx device (i.e. body coordinates).
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Linear acceleration in y-direction</returns>
        public static int LiaY()
        {
            byte[] request = { 0x76 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     The linear acceleration is the total acceleration minus the gravity.
        ///     The linear acceleration expressed the acceleration due to movement.
        ///     This register holds the linear acceleration along the z-axis of the pozyx device (i.e. body coordinates).
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>Linear acceleration in z-direction</returns>
        public static int LiaZ()
        {
            byte[] request = { 0x78 };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     Gravity is a force of 1g( = 9.80665 m/s2 ) directed towards the ground.
        ///     This register represents the gravity component in the x-axis and is represented by a singed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>x-component of gravity vector</returns>
        public static int GravX()
        {
            byte[] request = { 0x7A };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     Gravity is a force of 1g( = 9.80665 m/s2 ) directed towards the ground.
        ///     This register represents the gravity component in the y-axis and is represented by a singed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>y-component of gravity vector</returns>
        public static int GravY()
        {
            byte[] request = { 0x7C };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     Gravity is a force of 1g( = 9.80665 m/s2 ) directed towards the ground.
        ///     This register represents the gravity component in the z-axis and is represented by a singed 16-bit integer.
        ///     1mg = 16 int.
        /// </summary>
        /// <returns>z-component of gravity vector</returns>
        public static int GravZ()
        {
            byte[] request = { 0x7E };
            byte[] data = Connection.ReadWrite(request, 2);

            return BitConverter.ToInt16(data, 0);
        }

        /// <summary>
        ///     Read out the internal chip temperature.
        ///     This is loosely related to the ambient room temperature.
        ///     For more accurate ambient temperature measurements, it is recommended to use a separate sensor.
        /// </summary>
        /// <returns>Temperature</returns>
        public static int Temperature()
        {
            byte[] request = { 0x80 };
            byte[] data = Connection.ReadWrite(request, 1);

            return data[0];
        }
    }
}
