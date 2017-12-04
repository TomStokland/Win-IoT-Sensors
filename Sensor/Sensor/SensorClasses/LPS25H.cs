using System;

namespace Sensor
{
    public class LPS25H
    {
        #region enums
        public enum Register
        {
            REFERENCE_PRESSURE_LOW = 0x08,  // low 8 bits
            REFERENCE_PRESSURE_MIDDLE,      // middle 8 bits
            REFERENCE_PRESSURE_HIGH,        // high 8 bits
            DEVICE_ID = 0x0f,               // value = 0xBD
            RESOLUTION,
            CONTROL_1 = 0x20,
            CONTROL_2,
            CONTROL_3,
            CONTROL_4,
            INTERRUPT_CONFIGURATION,
            INTERRUPT_SOURCE,
            STATUS = 0x27,
            PRESSURE_LOW,
            PRESSURE_MIDDLE,
            PRESSURE_HIGH,
            TEMPEERATURE_LOW,
            TEMPEERATURE_HIGH,
            FIFO_CONTROL = 0x2e,
            FIFO_STATUS,
            PRESSURE_INTERRUPT_THRESHOLD_LOW,
            PRESSURE_INTERRUPT_THRESHOLD_HIGH,
            PRESSURE_OFFSET_LOW = 0x39,
            PRESSURE_OFFSET_HIGH
        };

        public enum FIFO_MODE
        {
            BYPASS = 0x000,
            FIFO,
            STREAM,
            MEAN = 0x06,
        };

        public enum CONTROL_1_BITS
        {
            SPI_MODE = 0,
            RESET_AUTOZERO,
            BLOCK_DATA_UPDATE,
            DIFFERENTIAL_PRESSUE_ENABLE,
            OUTPUT_DATA_RATE = 6,
            POWER_DOWN
        };

        public enum CONTROL_2_BITS
        {
            ONE_SHOT_ENABLE = 0,
            AUTOZERO_ENABLE,
            SOFTWARE_RESET,
            I2C_ENABLE,
            FIFO_MEAN_DECIMATION,
            FIFO_WATERMARK_ENABLE,
            FIFO_ENABLE,
            REBOOT_MEMORY
        };

        public enum CONTROL_3_BITS
        {
            INTERRUPT_SOURCE = 1,
            INTERRUPT_PIN_CINFIGURATION = 6,
            INTERRUPT_LEVEL
        };

        public enum CONTROL_4_BITS
        {
            DATA_READY_INTERRUPT,
            OVERRUN_INTERRUPT,
            WATERMARK_INTERRUPT,
            EMPTY_INTERRUPT
        };

        public enum INTERRUPT_SOURCE_BITS
        {
            DIFFERENTIAL_PRESSURE_HIGH,
            DIFFERENTIAL_PRESSURE_LOW,
            INTERRUPT_ACTIVE
        };

        public enum STATUS_REGISTER_BITS
        {
            TEMPERATURE_DATA_AVAILABLE = 0,
            PRESSURE_DATA_AVAILABLE,
            TEMPERATURE_DATA_OVERRUN = 4,
            TPRESSURE_DATA_OVERRUN,
        };

        public enum FIFO_CONTROL_BITS
        {
            WATERMARK_SAMPLE = 4,
            FIFO_MODE
        };

        public enum FIFO_STATUS_BITS
        {
            FIFO_EMPTY = 5,
            FIFO_FULL,
            WATERMARK_STATUS
        };

        #endregion enums

        public LPS25H()
        {
            Initialize();
        }

        private void Initialize()
        {
            // i2c @ standard or high speed
            // address = 0x5c or 0x5d
            // need to add spi
        }

        public int GetPressureResolution()
        {
            int resolution = 0;

            return resolution;
        }

        public void SetPressureResolution(int resolution)
        {

        }

        public int GetTemperatureResolution()
        {
            int resolution = 0;

            return resolution;
        }

        public void SetTemperatureResolution(int resolution)
        {

        }

        public void SetUpdateDataRate(int rate)
        {
            // read control register 1
            // insert 3 bits int bits 6:4
            switch (rate)
            {
                case 0:
                    break;

                case 1:
                    break;

                case 7:
                    break;

                case 125:
                    break;

                case 25:
                    break;

                default: break;
            }
        }
    }
}
