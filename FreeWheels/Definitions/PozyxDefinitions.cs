using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Definitions
{
    class PozyxDefinitions
    {
        //Status registers

        public const int POZYX_WHO_AM_I = 0x00;
        public const int POZYX_FIRMWARE_VER = 0x01;
        public const int POZYX_HARDWARE_VER = 0x02;
        public const int POZYX_ST_RESULT = 0x03;
        public const int POZYX_ERRORCODE = 0x04;
        public const int POZYX_INT_STATUS = 0x05;
        public const int POZYX_CALIB_STATUS = 0x06;

        //Configuration registers

        public const int POZYX_INT_MASK = 0x10;
        public const int POZYX_INT_CONFIG = 0x11;
        public const int POZYX_POS_FILTER = 0x14;
        public const int POZYX_CONFIG_LEDS = 0x15;
        public const int POZYX_POS_ALG = 0x16;
        public const int POZYX_POS_NUM_ANCHORS = 0x17;
        public const int POZYX_POS_INTERVAL = 0x18;
        public const int POZYX_NETWORK_ID = 0x1A;
        public const int POZYX_UWB_CHANNEL = 0x1C;
        public const int POZYX_UWB_RATES = 0x1D;
        public const int POZYX_UWB_PLEN = 0x1E;
        public const int POZYX_UWB_GAIN = 0x1F;
        public const int POZYX_UWB_XTALTRIM = 0x20;
        public const int POZYX_RANGE_PROTOCOL = 0x21;
        public const int POZYX_OPERATION_MODE = 0x22;
        public const int POZYX_SENSORS_MODE = 0x23;
        public const int POZYX_CONFIG_GPIO1 = 0x27;
        public const int POZYX_CONFIG_GPIO2 = 0x28;
        public const int POZYX_CONFIG_GPIO3 = 0x29;
        public const int POZYX_CONFIG_GPIO4 = 0x2A;

        //Positioning data

        public const int POZYX_POS_X = 0x30;
        public const int POZYX_POS_Y = 0x34;
        public const int POZYX_POS_Z = 0x38;
        public const int POZYX_POS_ERR_X = 0x3C;
        public const int POZYX_POS_ERR_Y = 0x3E;
        public const int POZYX_POS_ERR_Z = 0x40;
        public const int POZYX_POS_ERR_YX = 0x42;
        public const int POZYX_POS_ERR_XZ = 0x44;
        public const int POZYX_POS_ERR_YZ = 0x46;

        //Sensor data

        public const int POZYX_MAX_LIN_ACC = 0x4E;
        public const int POZYX_PRESSURE = 0x50;
        public const int POZYX_ACCEL_X = 0x54;
        public const int POZYX_ACCEL_Y = 0x56;
        public const int POZYX_ACCEL_Z = 0x58;
        public const int POZYX_MAGN_X = 0x5A;
        public const int POZYX_MAGN_Y = 0x5C;
        public const int POZYX_MAGN_Z = 0x5E;
        public const int POZYX_GYRO_X = 0x60;
        public const int POZYX_GYRO_Y = 0x62;
        public const int POZYX_GYRO_Z = 0x64;
        public const int POZYX_EUL_HEADING = 0x66;
        public const int POZYX_EUL_ROLL = 0x68;
        public const int POZYX_EUL_PITCH = 0x6A;
        public const int POZYX_QUAT_W = 0x6C;
        public const int POZYX_QUAT_X = 0x6E;
        public const int POZYX_QUAT_Y = 0x70;
        public const int POZYX_QUAT_Z = 0x72;
        public const int POZYX_LIA_X = 0x74;
        public const int POZYX_LIA_Y = 0x76;
        public const int POZYX_LIA_Z = 0x78;
        public const int POZYX_GRAV_X = 0x7A;
        public const int POZYX_GRAV_Y = 0x7C;
        public const int POZYX_GRAV_Z = 0x7E;
        public const int POZYX_TEMPERATURE = 0x80;


        //General data

        public const int POZYX_DEVICE_LIST_SIZE = 0x81;
        public const int POZYX_RX_NETWORK_ID = 0x82;
        public const int POZYX_RX_DATA_LEN = 0x84;
        public const int POZYX_GPIO1 = 0x85;
        public const int POZYX_GPIO2 = 0x86;
        public const int POZYX_GPIO3 = 0x87;
        public const int POZYX_GPIO4 = 0x88;

        //Register Functions

        public const int POZYX_RESET_SYS = 0xB0;
        public const int POZYX_LED_CTRL = 0xB1;
        public const int POZYX_TX_DATA = 0xB2;
        public const int POZYX_TX_SEND = 0xB3;
        public const int POZYX_RX_DATA = 0xB4;
        public const int POZYX_DO_RANGING = 0xB5;
        public const int POZYX_DO_POSITIONING = 0xB6;
        public const int POZYX_POS_SET_ANCHOR_IDS = 0xB7;
        public const int POZYX_POS_GET_ANCHOR_IDS = 0xB8;
        public const int POZYX_FLASH_RESET = 0xB9;
        public const int POZYX_FLASH_SAVE = 0xBA;
        public const int POZYX_FLASH_DETAILS = 0xBB;

        //Device list functions

        public const int POZYX_DEVICES_GETIDS = 0xC0;
        public const int POZYX_DEVICES_DISCOVER = 0xC1;
        public const int POZYX_DEVICES_CALIBRATE = 0xC2;
        public const int POZYX_DEVICES_CLEAR = 0xC3;
        public const int POZYX_DEVICE_ADD = 0xC4;
        public const int POZYX_DEVICE_GETINFO = 0xC5;
        public const int POZYX_DEVICE_GETCOORDS = 0xC6;
        public const int POZYX_DEVICE_GETRANGEINFO = 0xC7;
        public const int POZYX_CIR_DATA = 0xC8;



        //------------------------------//


    }
}
