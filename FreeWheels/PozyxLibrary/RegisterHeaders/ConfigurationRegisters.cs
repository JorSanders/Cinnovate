﻿using FreeWheels.PozyxLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.PozyxLibrary.RegisterHeaders
{
    public class ConfigurationRegisters : RegisterHeaders
    {
        public ConfigurationRegisters(IConnection connection) : base(connection)
        {
        }

        /// <summary>
        ///     	Indicates which interrupts are enabled.    
        /// </summary>
        /// <param name="err">Enables interrupts whenever an error occurs.</param>
        /// <param name="pos">Enables interrupts whenever a new positiong update is availabe.</param>
        /// <param name="imu">Enables interrupts whenever a new IMU update is availabe.</param>
        /// <param name="rxData">Enables interrupts whenever data is received through the ultra-wideband network.</param>
        /// <param name="funt">Enables interrupts whenever a register function call has completed.</param>
        /// <param name="pin">Configures the interrupt pin.</param>
        public void IntMask(bool err, bool pos, bool imu, bool rxData, bool funt, int pin, int remoteId = 0)
        {
            byte parameters = 0x0;
            bool[] interupts = { err, pos, imu, rxData, funt };

            for (int i = 0; i < interupts.Count(); i++)
            {
                if (interupts[i])
                {
                    parameters = (byte)(0x1 << i | parameters);
                }
            }
            parameters = (byte)(pin << 7 | parameters);

            WriteRegister(0x10, new byte[] { parameters }, remoteId);
        }

        // See IntMask(bool err, bool pos, bool imu, bool rxData, bool funt, int pin)
        public List<string> IntMask(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x10, 1, null, remoteId);

            List<string> interupts = new List<string>();
            interupts.Add(((0x80 & data[0]) == 1) ? "PIN1" : "PIN0");

            string[] interuptFlags = { "ERR", "POS", "IMU", "RXDATA", "FUNC" };

            for (int i = 0; i < interuptFlags.Length; i++)
            {
                if ((data[0] >> i & 0x1) == 1)
                {
                    interupts.Add(interuptFlags[i]);
                }
            }

            return interupts;
        }

        /// <summary>
        ///     This register configures the external interrupt pin of the Pozyx device. It should be configured in combination with the POZYX_INT_MASK register.
        /// </summary>
        /// <param name="pinNum">
        ///     Selects the pin used by the interrupt. Possible values:
        ///     0 - No pin(default)
        ///     1 - GPIO pin 1 (pin 9 on the pozyx tag)
        ///     2 - GPIO pin 2 (pin 10 on the pozyx tag)
        ///     3 - GPIO pin 3 (pin 11 on the pozyx tag)
        ///     4 - GPIO pin 4 (pin 12 on the pozyx tag)
        ///     5 - GPIO pin 5 (pin 2 on the pozyx tag)
        ///     6 - GPIO pin 6 (pin 3 on the pozyx tag)
        /// </param>
        /// <param name="mode">
        ///     Select the interrupt pin mode. Possible values:
        ///     0 - Push-pull(default): the pin will actively set the interrupt line high or low.The line cannot be shared with multiple devices.
        ///     1 - open drain: this allows the user to share the interrupt line with multiple devices.This mode requires an external pull-up or pull-down resistor.
        /// </param>
        /// <param name="act">
        ///     The voltage level when an interrupt happens. Possible values:
        ///     0 - active low(default): 0V
        ///     1 - active high: 3.3V
        /// </param>
        /// <param name="latch">
        ///     Select if the interrupt pin should latch after an interrupt. Possible values:
        ///     0 - No latch(default): the interrupt is a short pulse of about 6µs
        ///     1 - Latch: after an interrupt, the interrupt pin will stay at the active level until the POZYX_INT_STATE register is read from
        /// </param>
        public void IntConfig(int pinNum, int mode, int act, int latch, int remoteId = 0)
        {
            byte parameters = (byte)pinNum;
            int[] options = { mode, act, latch };

            for (int i = 0; i < options.Count(); i++)
            {
                if (options[i] == 1)
                {
                    parameters = (byte)(0x1 << (byte)i + 3 | parameters);
                }
            }

            WriteRegister(0x11, new byte[] { parameters }, remoteId);
        }

        // See IntConfig(int pinNum, int mode, int act, int latch)
        public int[] IntConfig(int remoteId = 0)
        {
            byte[] request = { 0x11 };
            byte[] data = ReadRegister(0x11, 1, null, remoteId);

            return new int[] { data[0] & 0x7, data[0] & 0x8, data[0] & 0x20, data[0] & 0x40 };
        }

        /// <summary>
        ///     This register selects and configures any additional positioning filters used by the pozyx device.
        /// </summary>
        /// <param name="Strength">
        ///     Indicates the strength of the filter. In general, the position will be delayed by the number of samples equal to the strength.
        ///     Possible values: 0-15 
        /// </param>
        /// <param name="filter">
        /// 	This is the filter type. Possible values:
        /// 	1 : NONE(Default value). No additional filtering is applied.
        /// 	2 : FIR.A low-pass filter is applied which filters out high-frequency jitters.
        /// 	3 : MOVING_AVERAGE.A moving average filter is applied, which smoothens the trajectory.
        /// 	4 : MOVING_MEDIAN.A moving median filter is applied, which filters out outliers.
        /// </param>
        public void PosFilter(int strength, int filter, int remoteId = 0)
        {
            byte parameters = (byte)filter;
            byte strengthByte = (byte)(strength << 4);
            parameters |= strengthByte;

            WriteRegister(0x14, new byte[] { parameters }, remoteId);
        }

        // See PosFilter(int strength, int filter)
        public int[] PosFilter(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x14, 1, null, remoteId);

            return new int[] { data[0] >> 4, data[0] & 0x0F };
        }

        /// <summary>
        ///     This register configures the functionality of the 6 LEDs on the pozyx device. At all times, the user can control LEDs 1 through 4 using POZYX_LED_CTRL.
        /// </summary>
        /// <param name="led1">
        ///     Possible values:
        ///     0 : The LED is not controlled by the Pozyx system.
        ///     1 : The LED is controlled by the Pozyx system.The LED will blink (roughly) every 2s to indicate that the device is working properly.
        /// </param>
        /// <param name="led2">
        /// 	Possible values:
        /// 	0 : The LED is not controlled by the Pozyx system.
        /// 	1 : The LED is controlled by the Pozyx system.The LED will be turned on when the device is performing a register write operation or a register function (i.e., calibrating, positioning, ..). 
        /// </param>
        /// <param name="led3">
        ///     0 : The LED is not controlled by the Pozyx system.
        ///     1 : The LED is controlled by the Pozyx system.The LED will be turned on when the device is performing a register write operation or a register function(i.e., calibrating, positioning, ..). 
        /// </param>
        ///  <param name="led4">
        ///     Possible values:
        /// 	0 : The LED is not controlled by the Pozyx system.
        /// 	1 : The LED is controlled by the Pozyx system.The LED is turned on whenever an error occurs. The error can be read from the POZYX_ERRORCODE register.
        /// </param>
        /// <param name="ledRx">
        /// 	Possible values:
        /// 	0 : The LED will not blink upon reception of an UWB message.
        /// 	1 : The LED will blink upon reception of an UWB message.
        /// </param>
        /// <param name="ledTx">
        ///     Possible values:
        ///     0 : The LED will not blink upon transmission of an UWB message.
        ///     1 : The LED will blink upon transmission of an UWB message.
        /// </param>
        public void ConfigLeds(bool led1, bool led2, bool led3, bool led4, bool ledRx, bool ledTx, int remoteId = 0)
        {
            byte parameters = 0x0;
            bool[] leds = { led1, led2, led3, led4, ledRx, ledTx };

            for (int i = 0; i < leds.Count(); i++)
            {
                if (leds[i])
                {
                    parameters = (byte)(0x1 << (byte)i | parameters);
                }
            }

            WriteRegister(0x15, new byte[] { parameters }, remoteId);
        }

        // See ConfigLeds(bool led1, bool led2, bool led3, bool led4, bool ledRx, bool ledTx)
        public bool[] ConfigLeds(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x15, 1, null, remoteId);

            bool[] leds = new bool[6];

            for (int i = 0; i < leds.Count(); i++)
            {
                leds[i] = (data[0] >> i & 0x1) == 1;
            }

            return leds;
        }


        /// <summary>
        ///     This register selects and configures the positioning algorithm used by the pozyx device.
        /// </summary>
        /// <param name="algorithm">
        ///     Indicates which algorithm to use for positioning
        ///     0: UWB-only (Default value).
        ///     4: Tracking
        /// </param>
        /// <param name="dim">
        ///     This indicates the spatial dimension. Possible values:
        ///     2 : 2D (Default value).
        ///     1 : 2,5D
        ///     3 : 3D
        /// </param>
        public void PosAlg(int algorithm, int dim, int remoteId = 0)
        {
            byte parameters = (byte)algorithm;
            byte dimByte = (byte)(dim << 4);
            parameters |= dimByte;

            WriteRegister(0x16, new byte[] { parameters }, remoteId);
        }

        // See PosAlg(int algorithm, int dim)
        public int[] PosAlg(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x16, 1, null, remoteId);

            return new int[] { data[0] & 0xF, data[0] >> 4 };
        }

        /// <summary>
        ///     Configure the number of anchors and selection procedure
        /// </summary>
        /// <param name="num">
        /// 	Indicate the maximum number of anchors to be used for positioning. Value between 3 and 15.
        /// </param>
        /// <param name="mode">
        ///     a single bit to indicate wether to choose from a fixed set of anchors or perform automatic anchor selection. Possible values:
        ///     0 : indicates fixed anchor set. 
        ///     1 : indicates automatic anchor selection.
        /// </param>
        public void PosNumAnchors(int num, int mode, int remoteId = 0)
        {
            byte parameters = 0x0;
            parameters &= (byte)num;
            byte modeByte = (byte)(mode << 7);
            parameters &= modeByte;

            WriteRegister(0x17, new byte[] { parameters }, remoteId);
        }

        // PosNumAnchors(int num, int mode)
        public int[] PosNumAnchors(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x17, 1, null, remoteId);

            return new int[] { data[0] & 0xF, data[0] >> 7 };
        }

        /// <summary>
        ///     Pozyx can be run in continuous mode to provide continuous positioning. 
        ///     The interval in milliseconds between successive updates can be configured with this register. 
        /// </summary>
        /// <param name="interval">
        ///     The value is capped between 10ms and 60000ms (1 minute). 
        ///     Writing the value 0 to this registers disables the continuous mode.
        /// </param>
        public void PosInterval(int interval, int remoteId = 0)
        {
            byte[] intervalBytes = BitConverter.GetBytes((UInt16)interval);

            WriteRegister(0x18, intervalBytes, remoteId);
        }

        // See void PosInterval(int interval)
        public int PosInterval(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x18, 2, null, remoteId);

            return BitConverter.ToUInt16(data, 0);
        }

        /// <summary>
        ///     set the network id
        /// </summary>
        /// <param name="networkID"></param>
        public void NetworkID(int networkID, int remoteId = 0)
        {
            byte[] parameters = new byte[2];
            BitConverter.GetBytes((UInt16)networkID).CopyTo(parameters, 0);

            WriteRegister(0x1A, parameters, remoteId);
        }

        // See NetworkId(NetworkID(int networkID))
        public int NetworkID(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x1A, 2, null, remoteId);

            return BitConverter.ToInt32(data, 0);
        }

        /// <summary>
        ///     Select the ultra-wideband transmission and reception channel. In general the transmission range increases at lower frequencies, i.e., lower channels. Allow up to 20ms to let the device switch channel. 
        ///     Warning: to enable wireless communication between two devices they must operate on the same channel.
        /// </summary>
        /// <param name="uwbChannel">
        ///     Default value: 5 
        ///     Indicate the UWB channel. Possible values:
        ///     1 : Centre frequency 3494.4MHz, using the band(MHz): 3244.8 – 3744, bandwidth 499.2 MHz 
        ///     2 : Centre frequency 3993.6MHz, using the band(MHz): 3774 – 4243.2, bandwidth 499.2 MHz
        ///     3 : Centre frequency 4492.8MHz, using the band(MHz): 4243.2 – 4742.4 bandwidth 499.2 MHz
        ///     4 : Centre frequency 3993.6MHz, using the band(MHz): 3328 – 4659.2 bandwidth 1331.2 MHz(capped to 900MHz)
        ///     5 : Centre frequency 6489.6MHz, using the band(MHz): 6240 – 6739.2 bandwidth 499.2 MHz
        ///     7 : Centre frequency 6489.6MHz, using the band(MHz): 5980.3 – 6998.9 bandwidth 1081.6 MHz(capped to 900MHz)
        /// </param>
        public void UwbChannel(int uwbChannel, int remoteId = 0)
        {
            WriteRegister(0x1A, new byte[] { (byte)uwbChannel }, remoteId);
        }


        // See UwbChannel(int uwbChannel)
        public int UwbChannel(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x1C, 1, null, remoteId);

            return data[0];
        }


        /// <summary>
        ///     This register describes the UWB bitrate and nominal pulse repition frequency (PRF).
        /// </summary>
        /// <param name="bitrate">
        ///     Indicate the UWB bitrate. Possible values:
        ///     0 : bitrate 110 kbits/s(Default value)
        ///     1 : bitrate 850 kbits/s
        ///     2 : bitrate 6.8 Mbits/s</param>
        /// <param name="prf">
        ///     Indicates the pulse repetition frequency to be used. Possible values
        ///     1 : 16 MHz 
        ///     2 : 64 MHz (default value)
        /// </param>
        public void UwbRates(int bitrate, int prf, int remoteId = 0)
        {
            byte parameters = (byte)bitrate;
            parameters |= (byte)(prf << 6);

            WriteRegister(0x1D, new byte[] { parameters }, remoteId);
        }

        // See UwbRates(int bitrate, int prf)
        public int[] UwbRates(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x1D, 1, null, remoteId);

            return new int[] { data[0] & 0x3F, data[0] >> 6 };
        }

        /// <summary>
        ///     This register describes the preamble length of the UWB wireless packets.
        /// </summary>
        /// <param name="plen">
        ///     Indicate the UWB preamble length. Possible values:
        ///     12 : 4096 symbols.Standard preamble length 4096 symbols
        ///     40 : 2048 symbols.Non-standard preamble length 2048 symbols
        ///     24 : 1536 symbols.Non-standard preamble length 1536 symbols
        ///     8  : 1024 symbols.Standard preamble length 1024 symbols(default value)
        ///     52 : 512 symbols.Non-standard preamble length 512 symbols
        ///     36 : 256 symbols.Non-standard preamble length 256 symbols
        ///     20 : 128 symbols.Non-standard preamble length 128 symbols
        ///     4  : 64 symbols.Standard preamble length 64 symbols
        /// </param>
        public void UwbPlen(int plen, int remoteId = 0)
        {
            WriteRegister(0x1D, new byte[] { (byte)plen }, remoteId);
        }

        // See UwbPlen(int plen)
        public int UwbPlen(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x1E, 1, null, remoteId);

            return data[0];
        }

        /// <summary>
        ///     Configure the power gain for the UWB transmitter
        ///     Warning: when changing channel, bitrate or preamble length, the power is also overwritten to the default value for this UWB configuration.
        ///     Warning: changing this value can make the Pozyx device fall out of regulation.
        /// </summary>
        /// <param name="gain">
        ///     Possible values are between 0 and 67. 1dB = 2 int.
        /// </param>
        public void UwbGain(int gain, int remoteId = 0)
        {
            WriteRegister(0x1F, new byte[] { (byte)gain }, remoteId);
        }

        // See UwbGain(int gain)
        public int UwbGain(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x1F, 1, null, remoteId);

            return data[0];
        }

        /// <summary>
        ///     This register contains the trimming value to fine-tune the operating frequency of the crystal oscillator used by the ultra-wideband front-end. 
        ///     By carefully selecting this value, the operating frequency can be tuned with an error of 1ppm. 
        ///     A smaller error on the operating frequency will increase the sensitivity of the UWB receiver. 
        /// </summary>
        /// <param name="xTalTrim"></param>
        public void XTalTrim(int xTalTrim, int remoteId = 0)
        {
            WriteRegister(0x20, new byte[] { (byte)xTalTrim }, remoteId);
        }

        // See XTalTrim(int xTalTrim)
        public int XTalTrim(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x20, 1, null, remoteId);

            return data[0];
        }

        /// <summary>
        ///     This register determines how the ranging measurements are made. 
        /// </summary>
        /// <param name="rangeProtocol">
        ///     0: PRECISION (Default value)
        ///     1: FAST
        /// </param>
        public void RangeProtocol(int rangeProtocol, int remoteId = 0)
        {
            WriteRegister(0x21, new byte[] { (byte)rangeProtocol }, remoteId);
        }

        // See RangeProtocol(int rangeProtocol)
        public int RangeProtocol(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x21, 1, null, remoteId);

            return data[0];
        }

        /// <summary>
        ///     Configure the mode of operation of the pozyx device
        /// </summary>
        /// <param name="operationMode">
        ///     0 : Tag mode. In tag mode, the device can more around. In this mode the device cannot be used by other devices for positioning.
        ///     1 : Anchor mode.In anchor mode the device is assumed to be immobile. The device can be used by other devices for positioning.
        /// </param>
        public void OperationMode(int operationMode, int remoteId = 0)
        {
            WriteRegister(0x22, new byte[] { (byte)operationMode }, remoteId);
        }

        // See OperationMode(int operationMode)
        public int OperationMode(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x22, 1, null, remoteId);

            return data[0];
        }

        /// <summary>
        ///     Configure the mode of operation of the sensors
        /// </summary>
        /// <param name="sensorMode">
        ///     Non-fusion modes:
        ///         0 : MODE_OFF
        ///         1 : ACCONLY 
        ///         2 : MAGONLY 
        ///         3 : GYROONLY 
        ///         4 : ACCMAGx
        ///         5 : ACCGYRO 
        ///         6 : MAGGYRO 
        ///         7 : AMG
        ///     Fusion modes:
        ///         8 : IMU 
        ///         9 : COMPASS 
        ///         10 : M4G 
        ///         11 : NDOF_FMC_OFF
        ///         12 : NDOF
        /// </param>
        public void SensorsMode(int sensorMode, int remoteId = 0)
        {
            WriteRegister(0x23, new byte[] { (byte)sensorMode }, remoteId);
        }

        // See SensorsMode(int sensorMode)
        public int SensorsMode(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x23, 1, null, remoteId);

            return data[0];
        }

        /// <summary>
        ///     Configure GPIO pin 1.
        /// </summary>
        /// <param name="mode">
        ///     Indicates the input or output mode of the pin
        ///     0 : digital input
        ///     1 : digital output(push-pull)
        ///     2 : digital output(open-drain)
        /// </param>
        /// <param name="pull">
        ///     When selecting input or open-drain output, the pin can be internally connected with a pull-up (to 0V) or pull-down (to 3.3V) resistor. 
        ///     0 : no pull-up or pull-down resistor.
        ///     1 : pull-up resistor. 
        ///     2 : pull-down resistor
        /// </param>
        public void ConfigGpio1(int mode, int pull, int remoteId = 0)
        {
            byte parameters = (byte)mode;
            parameters |= (byte)(pull >> 3);

            WriteRegister(0x27, new byte[] { parameters }, remoteId);
        }

        // See ConfigGpio1(int mode, int pull)
        public int[] ConfigGpio1(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x27, 1, null, remoteId);

            return new int[] { data[0] & 0x7, data[0] >> 3 };
        }

        public void ConfigGpio2(int mode, int pull, int remoteId = 0)
        {
            byte parameters = (byte)mode;
            parameters |= (byte)(pull >> 3);

            WriteRegister(0x28, new byte[] { parameters }, remoteId);
        }

        public int[] ConfigGpio2(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x28, 1, null, remoteId);

            return new int[] { data[0] & 0x7, data[0] >> 3 };
        }

        public void ConfigGpio3(int mode, int pull, int remoteId = 0)
        {
            byte parameters = (byte)mode;
            parameters |= (byte)(pull >> 3);

            WriteRegister(0x29, new byte[] { parameters }, remoteId);
        }

        public int[] ConfigGpio3(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x29, 1, null, remoteId);

            return new int[] { data[0] & 0x7, data[0] >> 3 };
        }

        public void ConfigGpio4(int mode, int pull, int remoteId = 0)
        {
            byte parameters = (byte)mode;
            parameters |= (byte)(pull >> 3);

            WriteRegister(0x2A, new byte[] { parameters }, remoteId);
        }

        public int[] ConfigGpio4(int remoteId = 0)
        {
            byte[] data = ReadRegister(0x2A, 1, null, remoteId);

            return new int[] { data[0] & 0x7, data[0] >> 3 };
        }

    }
}
