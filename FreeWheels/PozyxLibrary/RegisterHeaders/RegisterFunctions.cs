using FreeWheels.PozyxLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary.RegisterHeaders
{
    public class RegisterFunctions : RegisterHeaders
    {
        public RegisterFunctions(IConnection connection) : base(connection)
        {
        }

        /// <summary>
        ///     Calling this function resets the Pozyx device.
        ///     This also clears the device list and returns the settings to their defualt state (including UWB settings)
        /// </summary>
        /// <returns></returns>
        public bool ResetSys(int remoteId = 0)
        {
            byte[] data = ReadRegister(0xB0, 1, null, remoteId);

            if (data[0] != 1)
            {
                //throw new PozyxFailException(0xB0);
            }
            return true;
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
        public bool LedCTRL(bool led_1, bool led_2, bool led_3, bool led_4, bool useled_1, bool useled_2, bool useled_3, bool useled_4, int remoteId = 0)
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

            byte[] data = ReadRegister(0xB1, 1, new byte[] { parameters }, remoteId);

            if (data[0] != 1)
            {
                throw new PozyxFailException(0xB1);
            }
            return true;
        }


        // See parent, You only should use this if you want to to custom remote requests
        public bool TXData(int offset, byte[] dataBytes)
        {
            return base.TXData(offset, dataBytes);
        }

        // See parent, You only should use this if you want to to custom remote requests
        public bool TXSend(int networkID, int option)
        {
            return base.TXSend(networkID, option);
        }

        // See parent, You only should use this if you want to to custom remote requests
        public byte[] RXData(int offset = 0)
        {
            return base.RXData(offset);
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
        public bool DoRanging(int networkID, int remoteId = 0)
        {
            byte[] parameters = BitConverter.GetBytes((UInt16)networkID);

            byte[] data = ReadRegister(0xB5, 1, parameters, remoteId);

            if (data[0] != 1)
            {
                throw new PozyxFailException(0xB5);
            }
            return true;
        }

        /// <summary>
        /// Initiates a positioning operation.
        /// Calling this function will turn of continuous positioning. 
        /// When positioning is finished (after about 70ms) the POS bit is set POZYX_INT_STATUS and an interrupt is fired if the POS bit is enabled in POZYX_INT_MASK. 
        /// The result is stored in the positioning registers starting from POZYX_POS_X
        /// </summary>
        /// <returns> True or False </returns>
        public bool DoPositioning(int remoteId = 0)
        {
            byte[] data = ReadRegister(0xB6, 1, null, remoteId);

            if (data[0] != 1)
            {
                throw new PozyxFailException(0xB6);
            }
            return true;
        }

        /// <summary>
        ///     Sets the anchor network IDs that can be used for the positioning algorithm. 
        ///     The positioning algorithm will select the first NUM (as given in POZYX_POS_NUM_ANCHORS) anchors from this list only when the MODE (as given in POZYX_POS_NUM_ANCHORS) is set to the fixed anchor set. 
        ///     Note that the actual anchor coordinates must be provided by POZYX_ADD_ANCHOR for each anchor id. 
        /// </summary>
        /// <param name="networkIds"></param>
        /// <returns> True or False </returns>
        public bool PosSetAnchorIds(int[] networkIds, int remoteId = 0)
        {
            byte[] parameters = new byte[networkIds.Length * 2 + 1];

            for (int i = 0; i < networkIds.Length * 2; i += 2)
            {
                BitConverter.GetBytes((UInt16)networkIds[i / 2]).CopyTo(parameters, i);
            }

            byte[] data = ReadRegister(0xB7, 1, parameters, remoteId);

            if (data[0] != 1)
            {
                throw new PozyxFailException(0xB7);
            }
            return true;
        }

        /// <summary>
        ///     When the positioning algorithm is set to the automatic anchor selection mode in (POZYX_POS_NUM_ANCHORS), this list will be filled automatically with the anchors chosen by the anchor selection algorithm.
        /// </summary>
        /// <returns> Anchor IDs that are used for the positioning algorithm. </returns>
        public List<int> PosGetAnchorIds(int remoteId = 0)
        {
            byte[] data = ReadRegister(0xB8, 33, null, remoteId);

            List<int> anchorIds = new List<int>();

            if (data[0] == 0)
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
        public bool FlashReset(int remoteId = 0)
        {
            byte[] data = ReadRegister(0xB9, 1, null, remoteId);

            if (data[0] != 1)
            {
                //throw new PozyxFailException(0xB9);
            }
            return true;
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
        public bool FlashSave(int dataType, int[] registerAddresses = null, int remoteId = 0)
        {
            byte[] parameters;
            if (registerAddresses == null)
            {
                if (dataType == 1)
                {
                    throw new ArgumentNullException();
                }
                parameters = new byte[registerAddresses.Length + 1];
                registerAddresses.CopyTo(parameters, 1);
                parameters[0] = (byte)dataType;
            }
            else
            {
                parameters = new byte[] { (byte)dataType };
            }

            byte[] data = ReadRegister(0xBA, 1, parameters, remoteId);

            if (data[0] != 1)
            {
                throw new PozyxFailException(0xBA);
            }
            return true;
        }

        /// <summary>
        ///     Returns detailed information about which data registers are stored in flash memory.
        /// </summary>
        /// <returns> 21 bytes of information </returns>
        public List<int> FlashDetail(int remoteId = 0)
        {
            byte[] data = ReadRegister(0xBB, 21, null, remoteId);

            List<int> flashDetail = new List<int>();

            for (int i = 0; i < data.Length; i++)
            {
                flashDetail.Add(data[i]);
            }

            return flashDetail;
        }

    }
}
