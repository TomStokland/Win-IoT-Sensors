using System;
using Windows.Devices.I2c;
using Communications;

namespace Sensor
{
    public class VL6180X
    {
        #region enums

        public enum Register
        {   // register                                address     reset value
            IDENTIFICATION__MODEL_ID = 0x000,  //  ID = 0xB4
            IDENTIFICATION__MODEL_REV_MAJOR = 0x001,  //  0x01
            IDENTIFICATION__MODEL_REV_MINOR = 0x002,  //  0x03
            IDENTIFICATION__MODULE_REV_MAJOR = 0x003,  //  0x01
            IDENTIFICATION__MODULE_REV_MINOR = 0x004,  //  0x02 
            IDENTIFICATION__DATE_HI = 0x006,  //  YY/MM
            IDENTIFICATION__DATE_LO = 0x007,  //  ddddd/ppp   p = phase
            IDENTIFICATION__TIME = 0x008, //   16-bit, seconds since midnight
            //                                          0x009

            SYSTEM__MODE_GPIO0 = 0x010,  //  0x60
            SYSTEM__MODE_GPIO1 = 0x011,  //  0x20
            SYSTEM__HISTORY_CTRL = 0x012,  //  0x00
            SYSTEM__INTERRUPT_CONFIG_GPIO = 0x014,  //  0x00
            SYSTEM__INTERRUPT_CLEAR = 0x015,  //  0x00
            SYSTEM__FRESH_OUT_OF_RESET = 0x016,  //  0x01
            SYSTEM__GROUPED_PARAMETER_HOLD = 0x017,  //  0x00

            SYSRANGE__START = 0x018,  //  0x00
            SYSRANGE__THRESH_HIGH = 0x019,  //  0xFF
            SYSRANGE__THRESH_LOW = 0x01A,  //  0x00
            SYSRANGE__INTERMEASUREMENT_PERIOD = 0x01B,  //  0xFF
            SYSRANGE__MAX_CONVERGENCE_TIME = 0x01C,  //  0x31
            SYSRANGE__CROSSTALK_COMPENSATION_RATE = 0x01E,  //  16-bit, 0x0000
            SYSRANGE__CROSSTALK_VALID_HEIGHT = 0x021,  //  0x14
            SYSRANGE__EARLY_CONVERGENCE_ESTIMATE = 0x022,  //  16-bit, 0x0000
            SYSRANGE__PART_TO_PART_RANGE_OFFSET = 0x024,  //  0xYY
            SYSRANGE__RANGE_IGNORE_VALID_HEIGHT = 0x025,  //  0x00
            SYSRANGE__RANGE_IGNORE_THRESHOLD = 0x026,  //  16-bit, 0x0000
            SYSRANGE__MAX_AMBIENT_LEVEL_MULT = 0x02C,  //  0xA0
            SYSRANGE__RANGE_CHECK_ENABLES = 0x02D,  //  0x11
            SYSRANGE__VHV_RECALIBRATE = 0x02E,  //  0x00
            //                                          0x030   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION
            SYSRANGE__VHV_REPEAT_RATE = 0x031,  //  0x00

            SYSALS__START = 0x038,  //  0x00
            SYSALS__THRESH_HIGH = 0x03A,  //  16 bits, 0xFFFF
            //                                          0x03B
            SYSALS__THRESH_LOW = 0x03C,  //  16 bits, 0x0000
            //                                          0x03D
            SYSALS__INTERMEASUREMENT_PERIOD = 0x03E,  //  0xFF
            SYSALS__ANALOGUE_GAIN = 0x03F,  //  0x06
            SYSALS__INTEGRATION_PERIOD = 0x040,  //  0x00
            //                                          0x041 - 0x04C unused
            RESULT__RANGE_STATUS = 0x04D,  //  0x01
            RESULT__ALS_STATUS = 0x04E,  //  0x01
            RESULT__INTERRUPT_STATUS_GPIO = 0x04F,  //  0x00
            RESULT__ALS_VAL = 0x050,  //  16-bit, 0x0000
            //                                          0x051
            RESULT__HISTORY_BUFFER_0 = 0x052,  //  16-bit, 0x0000 
            //                                          0x053
            RESULT__HISTORY_BUFFER_1 = 0x054,  //  16-bit, 0x0000
            //                                          0x055
            RESULT__HISTORY_BUFFER_2 = 0x056,  //  16-bit, 0x0000
            //                                          0x057
            RESULT__HISTORY_BUFFER_3 = 0x058,  //  16-bit, 0x0000
            //                                          0x059
            RESULT__HISTORY_BUFFER_4 = 0x05A,  //  16-bit, 0x0000
            //                                          0x05B
            RESULT__HISTORY_BUFFER_5 = 0x05C,  //  16-bit, 0x0000
            //                                          0x05D
            RESULT__HISTORY_BUFFER_6 = 0x05E,  //  16-bit, 0x0000
            //                                          0x05F
            RESULT__HISTORY_BUFFER_7 = 0x060,  //  16-bit, 0x0000
            //                                          0x061
            RESULT__RANGE_VAL = 0x062,  //  0x00
            //                                          0x063   //  unused
            RESULT__RANGE_RAW = 0x064,  //  0x00
            //                                          0x065   //  unused
            RESULT__RANGE_RETURN_RATE = 0x066,  //  16-bit, 0x0000
            //                                          0x067
            RESULT__RANGE_REFERENCE_RATE = 0x068,  //  16-bit, 0x0000
            //                                          0x069
            //                                          0x06A, 0x06B unused
            RESULT__RANGE_RETURN_SIGNAL_COUNT = 0x06C, //   32-bit, 0X00000000
            //                                          0X06D - 0X06F
            RESULT__RANGE_REFERENCE_SIGNAL_COUNT = 0x070,  //  32-bit, 0X00000000
            //                                          0x071 - 0x073
            RESULT__RANGE_RETURN_AMB_COUNT = 0x074,  //  32-bit, 0x00000000
            //                                          0x075 - 0x077
            RESULT__RANGE_REFERENCE_AMB_COUNT = 0x078,  //  32-bit, 0x00000000
            //                                          0x079 - 0x07B
            RESULT__RANGE_RETURN_CONV_TIME = 0x07C,  //  32-bit, 0x00000000
            //                                          0x07D - 0x07F
            RESULT__RANGE_REFERENCE_CONV_TIME = 0x080,  //  32-bit, 0x00000000
            //                                          0x081 - 0x083

            RANGE_SCALER = 0x096, // 16-bit - see STSW-IMG003 core/inc/vl6180x_def.h
            //                                          0x097
            //                                          0x09F   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0A3   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0B2   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0B7   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0BB   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0CA   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0D9   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0DB   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0DC   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0DD   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0E3   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0E4   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0E5   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0E6   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0E7   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0F5   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x0FF   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x100   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            READOUT__AVERAGING_SAMPLE_PERIOD = 0x10A,
            FIRMWARE__BOOTUP = 0x119,  //  0x01
            FIRMWARE__RESULT_SCALER = 0x120,  //  0x01
            //                                          0x198   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x199   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x1A6   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x1A7   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x1AD   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x1B0   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x207   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            //                                          0x208   UNKNOWN PURPOSE, BUT NECESSARY FOR INITIALIZATION  
            I2C_SLAVE__DEVICE_ADDRESS = 0x212,  //  0x29
            INTERLEAVED_MODE__ENABLE = 0x2A3,  //  0x00
        };

        #region bits
        private enum GPIO_0_MODE_BITS
        {
            SELECT = 1,
            POLARITY = 5,
            XSHUTDOWN = 6
        };

        private enum GPIO_1_MODE_BITS
        {
            SELECT = 1,
            POLARITY = 5,
        };

        private enum SYSTEM_HISTORY_CONTROL_BITS
        {
            ENABLED = 0,
            BUFFER_MODE = 1,
            CLEAR = 2,
        };

        private enum INTERRUPT_CONFIG_GPIO
        {
            RANGE = 0,
            ALS = 3
        };

        private enum INTERRUPT_CLEAR
        {
            ERROR = 0,
            ALS = 1,
            RANGE = 2
        };

        private enum RANGE_START_BITS
        {
            STARTSTOP = 0,
            MODE = 1
        };

        private enum RANGE_CHECK_ENABLE_BITS
        {
            EARLY_CONVERGENCE = 0,
            RANGE_IGNORE = 1,
            SIGNAL_TO_NOISE = 4
        };

        private enum VHV_RECALIBRATE_BITS
        {
            VHV_RECALIBRATE = 0,
            VHV_STATUS = 1
        };

        private enum ALS_START_BITS
        {
            STARTSTOP = 0,
            MODE = 1
        };

        private enum RANGE_STATUS
        {
            DEVICE_READY = 0,
            MEASUREMENT_READY = 1,
            MAX_THRESHOLD_HIT = 2,
            MIN_THRESHOLD_HIT = 3,
            ERROR_CODE = 4  //  bits 4 - 7
        };

        private enum ALS_STATUS
        {
            DEVICE_READY = 0,
            MEASUREMENT_READY = 1,
            MAX_THRESHOLD_HIT = 2,
            MIN_THRESHOLD_HIT = 3,
            ERROR_CODE = 4  //  bits 4 - 7
        };

        private enum INTERRUPT_STATUS_GPIO_BITS
        {
            RANGE = 0,  // bits 0 - 2
            ALS = 3,    // bits 3 - 5
            ERROR = 6   // bits 6 - 7
        };

        #endregion bits
        #endregion enums

        I2C i2c = new I2C(0x29, I2cBusSpeed.StandardMode);

        public VL6180X()
        {
            Initialize();
        }

        private int Initialize()
        {
            int success = 0;

            success += CommandWrite(0x207, 0x01);
            success += CommandWrite(0x208, 0x01);
            success += CommandWrite(0x096, 0x00);
            success += CommandWrite(0x097, 0xfd);
            success += CommandWrite(0x0e3, 0x00);
            success += CommandWrite(0x0e4, 0x04);
            success += CommandWrite(0x0e5, 0x02);
            success += CommandWrite(0x0e6, 0x01);
            success += CommandWrite(0x0e7, 0x03);
            success += CommandWrite(0x0f5, 0x02);
            success += CommandWrite(0x0d9, 0x05);
            success += CommandWrite(0x0db, 0xce);
            success += CommandWrite(0x0dc, 0x03);
            success += CommandWrite(0x0dd, 0xf8);
            success += CommandWrite(0x09f, 0x00);
            success += CommandWrite(0x0a3, 0x3c);
            success += CommandWrite(0x0b7, 0x00);
            success += CommandWrite(0x0bb, 0x3c);
            success += CommandWrite(0x0b2, 0x09);
            success += CommandWrite(0x0ca, 0x09);
            success += CommandWrite(0x198, 0x01);
            success += CommandWrite(0x1b0, 0x17);
            success += CommandWrite(0x1ad, 0x00);
            success += CommandWrite(0x0ff, 0x05);
            success += CommandWrite(0x100, 0x05);
            success += CommandWrite(0x199, 0x05);
            success += CommandWrite(0x1a6, 0x1b);
            success += CommandWrite(0x1a7, 0x1f);
            success += CommandWrite(0x030, 0x00);
            success += CommandWrite((int)Register.SYSTEM__FRESH_OUT_OF_RESET, 0x00);

            success += CommandWrite((int)Register.READOUT__AVERAGING_SAMPLE_PERIOD, 0x30);
            success += CommandWrite((int)Register.SYSALS__ANALOGUE_GAIN, 0x46);
            success += CommandWrite((int)Register.SYSRANGE__VHV_REPEAT_RATE, 0xff);
            success += CommandWrite((int)Register.SYSALS__INTEGRATION_PERIOD, 0x00);
            success += CommandWrite((int)Register.SYSALS__INTEGRATION_PERIOD + 1, 0x63);
            success += CommandWrite((int)Register.SYSRANGE__VHV_RECALIBRATE, 0x01);
            success += CommandWrite((int)Register.SYSRANGE__INTERMEASUREMENT_PERIOD, 0x09);
            success += CommandWrite((int)Register.SYSALS__INTERMEASUREMENT_PERIOD, 0x31);
            success += CommandWrite((int)Register.SYSTEM__INTERRUPT_CONFIG_GPIO, 0x24);
            success += CommandWrite((int)Register.SYSRANGE__MAX_CONVERGENCE_TIME, 0x31);
            success += CommandWrite((int)Register.INTERLEAVED_MODE__ENABLE, 0x00);

            return success;
        }

        public int Range()
        {
            int success = 0;
            int range = 0;
            byte[] readData = new byte[1];

            readData = CommandRead((int)Register.IDENTIFICATION__MODEL_ID, 1);

            success = CommandWrite((int)Register.SYSRANGE__START, 0x01);
            readData[0] = 0;
            while ((readData[0] & 0x04) == 0)
                readData = CommandRead((int)Register.RESULT__INTERRUPT_STATUS_GPIO, 1);
            readData = CommandRead((int)Register.RESULT__RANGE_RAW, 1);
            range = readData[0];
            CommandWrite((int)Register.SYSTEM__INTERRUPT_CLEAR, 0x01);

            return range;
        }

        private int CommandWrite(int reg, byte data)
        {
            int success = 0;

            byte[] commandArray = new byte[3];

            commandArray[0] = (byte)(reg >> 8);
            commandArray[1] = (byte)(reg & 0xFF);
            commandArray[2] = data;

            success = i2c.Write(commandArray);

            return success;
        }

        private byte[] CommandRead(int reg, int numberOfBytes)
        {

            byte[] writeBuffer = new byte[2];
            byte[] readBuffer = new byte[numberOfBytes];

            writeBuffer[0] = (byte)(reg >> 8);
            writeBuffer[1] = (byte)(reg & 0xFF);

            readBuffer = i2c.WriteRead(writeBuffer, 1);
            if (readBuffer != null)
                return readBuffer;
            else
                return null;
        }
    }
}