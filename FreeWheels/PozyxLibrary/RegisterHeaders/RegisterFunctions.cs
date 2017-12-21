using FreeWheels.PozyxLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary.RegisterHeaders
{
    public class RegisterFunctions
    {

        private IConnection Connection;

        public RegisterFunctions(IConnection pozyxConnection)
        {
            Connection = pozyxConnection;
        }

        /// <summary>
        ///     Calling this function resets the Pozyx device.
        ///     This also clears the device list and returns the settings to their defualt state (including UWB settings)
        /// </summary>
        /// <returns></returns>
        public bool ResetSys()
        {
            byte[] request = { 0xB0 };
            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }
        
        /// <summary>
        ///     Gives control over the 4 onboard pozyx LEDS. 
        /// </summary>
        /// <param name="led_1"></param>
        /// <param name="led_2"></param>
        /// <param name="led_3"></param>
        /// <param name="led_4"></param>
        /// <param name="useled_1"></param>
        /// <param name="useled_2"></param>
        /// <param name="useled_3"></param>
        /// <param name="useled_4"></param>
        /// <returns> True or False </returns>
        public bool LedCTRL(bool led_1, bool led_2, bool led_3, bool led_4, bool useled_1, bool useled_2, bool useled_3, bool useled_4)
        {
            byte parameters = 0x0;
            bool[] leds = { led_1, led_2, led_3, led_4, useled_1, useled_2, useled_3, useled_4 };

            for (int i = 0; i < leds.Count(); i++)
            {
                if (leds[i])
                {
                    parameters = (byte)(0x1 << (byte)i | parameters);
                }
            }

            byte[] request = { 0xB1, parameters };
            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }

        /// <summary>
        ///     Fills the transmit buffer with data bytes.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="dataBytes"></param>
        /// <returns> True or False </returns>
        public bool TXData(int offset, byte[] dataBytes)
        {
            byte[] request = new byte[dataBytes.Length + 2];
            request[0] = 0xB2;
            request[1] = (byte)offset;
            dataBytes.CopyTo(request, 2);
            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }

        /// <summary>
        ///     Initiates the wireless transfer of all the data stored in the transmitter buffer.
        ///     Upon successful reception of the message by the destination node, the destination node will answer with an acknowledgement (ACK) message.
        ///     When a REG_READ or a REG_FUNC message was transmitted, the ACK will contain the requested data and the RX_DATA bit is set POZYX_INT_STATUS and an interrupt is fired if the RX_DATA bit is enabled in POZYX_INT_MASK. 
        ///     The received ACK data can be read from POZYX_RX_DATA. Depending on the UWB settings, it may take a few up to tens of milliseconds before an ACK is to be expected. 
        ///     After sending the data, the transmission buffer is emptied.
        /// </summary>
        /// <param name="networkID"></param>
        /// <param name="option"></param>
        /// <returns> True or False </returns>
        public bool TXSend(int networkID, int option)
        {
            byte[] request = new byte[] { 0xB3, (byte)networkID, (byte)(networkID >> 8), (byte)option };
            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }

        /// <summary>
        ///     Allows you to read from the received data buffer
        ///     This buffer is filled whenever data is wirelessly received by the UWB receiver.
        ///     Upon this event, the RX_DATA bit is set POZYX_INT_STATUS and an interrupt is fired if the RX_DATA bit is enabled in POZYX_INT_MASK. 
        ///     The RX data buffer is cleared after reading it. 
        ///     When a new data message arrives before the RX buffer was read, it will be overwritten and the previously received data will be lost. 
        ///     Note that the receiver must be turned on the receive incoming messages. 
        ///     This is done automatically whenever an acknowledgment is expected (after a POZYX_TX_SEND operation). 
        /// </summary>
        /// <returns> Requested bytes from the receive buffer </returns>
        public byte[] RXData(int offset = 0)
        {
            List<int> rxData = new List<int>();

            byte[] request = { 0xB4, (byte)offset };
            byte[] data = Connection.ReadWrite(request, 100);

            return data;
        }

        /// <summary>
        ///     Initiates a ranging operation.
        ///     When ranging is finished (after about 15ms) the FUNC bit is set POZYX_INT_STATUS
        ///     and an interrupt is fired if the FUNC bit is enabled in POZYX_INT_MASK. 
        ///     The range information can be obtained using POZYX_DEVICE_GETRANGEINFO.
        ///     The device will be added to the device list if it wasn't present before. 
        /// </summary>
        /// <param name="networkID"></param>
        /// <returns> True or False </returns>
        public bool DoRanging(int networkID)
        {
            byte[] request = new byte[3];
            request[0] = 0xB5;
            BitConverter.GetBytes((UInt16)networkID).CopyTo(request, 1);
            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }

        /// <summary>
        /// Initiates a positioning operation.
        /// Calling this function will turn of continuous positioning. 
        /// When positioning is finished (after about 70ms) the POS bit is set POZYX_INT_STATUS and an interrupt is fired if the POS bit is enabled in POZYX_INT_MASK. 
        /// The result is stored in the positioning registers starting from POZYX_POS_X
        /// </summary>
        /// <returns> True or False </returns>
        public bool DoPositioning()
        {
            byte[] request = { 0xB6 };
            byte[] data = Connection.ReadWrite(request, 1);

            if (data.Length > 0 && data[0] == 1)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Sets the anchor network IDs that can be used for the positioning algorithm. 
        ///     The positioning algorithm will select the first NUM (as given in POZYX_POS_NUM_ANCHORS) anchors from this list only when the MODE (as given in POZYX_POS_NUM_ANCHORS) is set to the fixed anchor set. 
        ///     Note that the actual anchor coordinates must be provided by POZYX_ADD_ANCHOR for each anchor id. 
        /// </summary>
        /// <param name="networkIds"></param>
        /// <returns> True or False </returns>
        public bool PosSetAnchorIds(int[] networkIds)
        {
            byte[] request = new byte[networkIds.Length * 2 + 1];
            request[0] = 0xB7;

            for (int i = 0; i < networkIds.Length * 2; i += 2)
            {
                BitConverter.GetBytes((UInt16)networkIds[i / 2]).CopyTo(request, i + 1);
            }

            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }

        /// <summary>
        ///     When the positioning algorithm is set to the automatic anchor selection mode in (POZYX_POS_NUM_ANCHORS), this list will be filled automatically with the anchors chosen by the anchor selection algorithm.
        /// </summary>
        /// <returns> Anchor IDs that are used for the positioning algorithm. </returns>
        public List<int> PosGetAnchorIds()
        {
            byte[] request = { 0xB8 };
            byte[] data = Connection.ReadWrite(request, 33);

            List<int> anchorIds = new List<int>();

            if(data[0] == 0)
            {
                return anchorIds;
            }

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

        /// <summary>
        ///     Clear the saved flash memory content and reset all the registers to their default values. 
        ///     Clearing data from the flash memory may take some time.
        /// </summary>
        /// <returns> True or False </returns>
        public bool FlashReset()
        {
            byte[] request = { 0xB9 };
            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }

        /// <summary>
        ///     Allows you to store the content of writable registers in the non-volatile flash memory. 
        ///     On the next boot of the system the saved content of these registers is automatically loaded. 
        ///     This way, any configuration is not lost after the device is reset or powered down. 
        ///     Note that storing data in the flash memory may take some time. 
        /// </summary>
        /// <param name="dataTypes"></param>
        /// <param name="regData"></param>
        /// <returns> True or False </returns>
        public bool FlashSave(int dataTypes, int[] regData = null)
        {
            regData = regData ?? new int[0];

            if (dataTypes == 1 && regData.Length <= 0)
            {
                return false;
            }

            byte[] request = new byte[dataTypes + 1];
            request[0] = 0xBA;
            request[1] = (byte)dataTypes;

            byte[] data = Connection.ReadWrite(request, 1);

            return data[0] == 1;
        }

        /// <summary>
        ///     Returns detailed information about which data registers are stored in flash memory.
        /// </summary>
        /// <returns> 21 bytes of information </returns>
        public List<int> FlashDetail()
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
