using System;
using Communications;

namespace Sensor
{
    public class LSM9DS1
    {
        #region enums
        public enum AG_Register
        {
            ACTIVITY_THRESHOLD = 0x04,
            ACTIVITY_DURATION,
            ACCELERATION_INTERRUPT_GENERATOR_CONFIGURATION,
            ACCELERATION_INTERRUPT_THRESHOLD_X,
            ACCELERATION_INTERRUPT_THRESHOLD_Y,
            ACCELERATION_INTERRUPT_THRESHOLD_Z,
            ACCELERATION_INTERRUPT_DURATION,
            GYROSCOPE_ANGULAR_RATE_REFERENCE,
            AG_INTERRUPT_CONTROL_1,
            AG_INTERRUPT_CONTROL_2,
            DEVICE_ID,
            GYROSCOPE_CONTROL_1,
            GYROSCOPE_CONTROL_2,
            GYROSCOPE_CONTROL_3,
            GYROSCOPE_ORIENTATION_CONFIGURATION,
            GYROSCOPE_INTERRUPT_SOURCE,
            TEMPERATURE_LOW,
            TEMPERATURE_HIGH,
            STATUS_1,
            GYROSCOPE_VALUE_X,
            GYROSCOPE_VALUE_Y,
            GYROSCOPE_VALUE_Z,
            CONTROL_4,
            ACCELERATION_CONTROL_5,
            ACCELERATION_CONTROL_6,
            ACCELERATION_CONTROL_7,
            CONTROL_8,
            CONTROL_9,
            CONTROL_10,
            ACCELERATION_INTERRUPT_SOURCE,
            STATUS_2,
            ACCELERATION_VALUE_X = 0x28,
            ACCELERATION_VALUE_Y = 0x2a,
            ACCELERATION_VALUE_Z = 0x2c,
            FIFO_CONTROL = 0x2e,
            FIFO_SOURCE,
            GYROSCOPE_INTERRUPT_GENERATOR_CONFIGURATION,
            GYROSCOPE_INTERRUPT_THRESHOLD_X,
            GYROSCOPE_INTERRUPT_THRESHOLD_Y,
            GYROSCOPE_INTERRUPT_THRESHOLD_Z,
            GYROSCOPE_INTERRUPT_DURATION,
        };

        public enum M_Register
        {
            OFFSET_X_LOW = 0x05,
            OFFSET_X_HIGH,
            OFFSET_Y_LOW,
            OFFSET_Y_HIGH,
            OFFSET_Z_LOW,
            OFFSET_Z_HIGH,
            DEVICE_ID = 0x0f,
            CONTROL_1,
            CONTROL_2,
            CONTROL_3,
            CONTROL_4,
            CONTROL_5,
            STATUS,
            OUTPUT_X_LOW,
            OUTPUT_X_HIGH,
            OUTPUT_Y_LOW,
            OUTPUT_Y_HIGH,
            OUTPUT_Z_LOW,
            OUTPUT_Z_HIGH,
            INTERRUPT_CONFIGURATION,
            INTERRUPT_SOURCE,
            INTERRUPT_THRESHOLD_LOW,
            INTERRUPT_THRESHOLD_HIGH,
        };

        private enum Address    // i2c address
        {
            AG_Primary = 0x6a,
            AG_Secondary = 0x6b,
            Mag_Primary = 0x1c,
            Mag_Secondary = 0x1e
        };



        #endregion enums

        public LSM9DS1()
        {
            Initialize();
        }

        private void Initialize()
        {
            // can be either SPI up to 10 MHZ
            // or I2C in standard or fast mode
        }
    }
}