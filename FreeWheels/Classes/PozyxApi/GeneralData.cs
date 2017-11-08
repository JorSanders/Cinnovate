using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Classes.PozyxApi
{
    class GeneralData
    {
        /// <summary>
        ///     Returns the number of devices stored internally
        /// </summary>
        /// <returns></returns>
        public static int GetDeviceListSize()
        {
            byte[] request = { 0x81 };
            byte[] data = Connection.ReadWrite(request, 1);

            if (data.Length > 0)
            {
                return data[0];
            }

            return 0;
        }

        /// <summary>
        ///     Places the network id of device A in POZYX_RX_NETWORK_ID
        /// </summary>
        /// <returns>Network id of the latest received message</returns>
        public static int RxNetworkId()
        {
            byte[] request = { 0x82 };
            byte[] data = Connection.ReadWrite(request, 2);

            byte[] rxNetworkId = { data[0], data[1] };

            return BitConverter.ToUInt16(rxNetworkId, 0);
        }

        /// <summary>
        ///     Places the length of the received data in the register
        /// </summary>
        /// <returns>The length of the latest received message</returns>
        public static int RxDataLen()
        {
            byte[] request = { 0x84 };
            byte[] data = Connection.ReadWrite(request, 1);

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
        public static int Gpio1()
        {
            byte[] request = { 0x85 };
            byte[] data = Connection.ReadWrite(request, 1);

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
        public static int Gpio2()
        {
            byte[] request = { 0x86 };
            byte[] data = Connection.ReadWrite(request, 1);

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
        public static int Gpio3()
        {
            byte[] request = { 0x87 };
            byte[] data = Connection.ReadWrite(request, 1);

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
        public static int Gpio4()
        {
            byte[] request = { 0x88 };
            byte[] data = Connection.ReadWrite(request, 1);

            return data[0];
        }
    }
}
