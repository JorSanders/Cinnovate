using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FreeWheels.Enums
{
    //Error codes - https://www.pozyx.io/Documentation/Datasheet/RegisterOverview -

    public enum PozyxErrorCode
    {
        Success = 0x00,
        ErrorI2cWrite = 0x01,
        ErrorI2cCmdfull = 0x02,
        ErrorAnchorAdd = 0x03,
        ErrorCommQueueFull = 0x04,
        ErrorI2cRead = 0x05,
        ErrorUwbConfig = 0x06,
        ErrorOperationQueueFull = 0x07,
        ErrorTdma = 0xA0,
        ErrorStartupBusfault = 0x08,
        ErrorFlashInvalid = 0x09,
        ErrorNotEnoughAnchors = 0X0A,
        ErrorDiscovery = 0X0B,
        ErrorCalibration = 0x0C,
        ErrorFuncParam = 0x0D,
        ErrorAnchorNotFound = 0x0E,
        ErrorFlash = 0x0F,
        ErrorMemory = 0x10,
        ErrorRanging = 0x11,
        ErrorRtimeout1 = 0x12,
        ErrorRtimeout2 = 0x13,
        ErrorTxlate = 0x14,
        ErrorUwbBusy = 0x15,
        ErrorPosalg = 0x16,
        ErrorNoack = 0x17,
        ErrorSniffOverflow = 0xE0,
        ErrorNoPps = 0xF0,
        ErrorNewTask = 0xF1,
        ErrorUnrecdev = 0xFE,
        ErrorGeneral = 0xFF,

    }

}
