using FreeWheels.PozyxLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary.RegisterHeaders
{
    // Inteded as parent class
    public class RegisterHeaders
    {
        private IConnection Connection; //TODO set to private. Childer shouldnt directly write

        public RegisterHeaders(IConnection pozyxConnection)
        {
            Connection = pozyxConnection;
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

            if (data[0] != 1)
            {
                throw new PozyxFailException(0xB2);
            }
            return true;
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

            if (data[0] != 1)
            {
                throw new PozyxFailException(0xB3);
            }
            return true;
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

        public byte[] ReadRegister(byte registerHeader, int numReturnBytes, byte[] parameters = null, int remoteId = 0)
        {
            byte[] data = new byte[numReturnBytes]; //The returned data
            byte[] request; // The registerheader + parameters

            if (parameters == null)
            {
                request = new byte[] { registerHeader };
            }
            else
            {
                request = new byte[parameters.Length + 1];
                request[0] = registerHeader;
                parameters.CopyTo(request, 1);
            }

            if (remoteId > 0)
            {
                byte[] test = new byte[] { registerHeader };
                bool txd = TXData(0, new byte[] { registerHeader, (byte)numReturnBytes });
                if (!txd)
                {
                    throw new PozyxException(0xB2, "Failed TXData setting on register 0x" + registerHeader.ToString("X2"));
                }
                bool txs = TXSend(remoteId, 0x02);
                if (!txs)
                {
                    throw new PozyxException(0xB3, "Failed TX sending on register 0x" + registerHeader.ToString("X2"));
                }

                Task.Delay(3000).Wait();

                byte[] rxData = RXData();

                if (rxData[0] != 1)
                {
                    throw new Exception("RXData contains an error for register: 0x" + registerHeader.ToString("X2"));
                }

                Array.Copy(rxData, 1, data, 0, numReturnBytes);
            }
            else
            {
                data = Connection.ReadWrite(request, numReturnBytes);
            }

            return request;
        }

        public void WriteRegister(byte registerHeader, byte[] parameters, int remoteId = 0)
        {
            byte[] request = new byte[parameters.Length + 1];
            request[0] = registerHeader;
            parameters.CopyTo(request, 1);

            if (remoteId == 0)
            {
                Connection.Write(request);
            }
            else
            {
                bool txd = TXData(0, request);

                if (!txd)
                {
                    throw new PozyxException(0xB2, "Failed TXData setting on register 0x" + registerHeader.ToString("X2"));
                }

                if (!TXSend(remoteId, 0x04))
                {
                    throw new PozyxException(0xB3, "Failed TX sending on register 0x" + registerHeader.ToString("X2"));
                }
            }
        }
    }
}
