using FreeWheels.PozyxLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary.RegisterHeaders
{
    public class GeneralData : RegisterHeaders
    {
        public GeneralData(IConnection connection) : base(connection)
        {
        }

        /// <summary>
        ///     Returns the number of devices stored internally
        /// </summary>
        /// <returns></returns>
        public int GetDeviceListSize(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x81, 1, null, remoteId);

            return data[0];
        }

        /// <summary>
        ///     Places the network id of device A in POZYX_RX_NETWORK_ID
        /// </summary>
        /// <returns>Network id of the latest received message</returns>
        public int RxNetworkId(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x82, 2, null, remoteId);

            return BitConverter.ToUInt16(data, 0);
        }

        /// <summary>
        ///     Places the length of the received data in the register
        /// </summary>
        /// <returns>The length of the latest received message</returns>
        public int RxDataLen(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x84, 1, null, remoteId);

            return data[0];
        }

        /// <summary>
        ///     This register can be read from to obtain the current state of the GPIO pin if it is configured as an input. 
        ///     When the pin is configured as an output, the value written will determine the new state of the pin.
        ///     Possible values:
        ///     The digital state of the pin is LOW at 0V.
        ///     The digital state of the pin is HIGH at 3.3V.
        ///     Default value: 0
        /// </summary>
        /// 
        /// <returns>Value of the GPIO pin 1</returns>
        public int Gpio1(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x85, 1, null, remoteId);

            return data[0];
        }

        /// <summary>
        ///     This register can be read from to obtain the current state of the GPIO pin if it is configured as an input. 
        ///     When the pin is configured as an output, the value written will determine the new state of the pin.
        ///     Possible values:
        ///     The digital state of the pin is LOW at 0V.
        ///     The digital state of the pin is HIGH at 3.3V.
        ///     Default value: 0
        /// </summary>
        /// <returns>Value of the GPIO pin 2</returns>
        /// 
        public int Gpio2(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x86, 1, null, remoteId);

            return data[0];
        }

        /// <summary>
        ///     This register can be read from to obtain the current state of the GPIO pin if it is configured as an input. 
        ///     When the pin is configured as an output, the value written will determine the new state of the pin.
        ///     Possible values:
        ///     The digital state of the pin is LOW at 0V.
        ///     The digital state of the pin is HIGH at 3.3V.
        ///     Default value: 0
        /// </summary>
        /// <returns>Value of the GPIO pin 3</returns>
        public int Gpio3(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x87, 1, null, remoteId);

            return data[0];
        }

        /// <summary>
        ///     This register can be read from to obtain the current state of the GPIO pin if it is configured as an input. 
        ///     When the pin is configured as an output, the value written will determine the new state of the pin.
        ///     Possible values:
        ///     The digital state of the pin is LOW at 0V.
        ///     The digital state of the pin is HIGH at 3.3V.
        ///     Default value: 0
        /// </summary>
        /// <returns>Value of the GPIO pin 4</returns>
        public int Gpio4(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x88, 1, null, remoteId);

            return data[0];
        }
    }
}
