using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Spi;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Windows;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using Windows.Storage.Streams;
using Communications;

namespace Sensor.SensorClasses
{
    class ADXL345
    {
        #region structs, enums and consts
        struct Acceleration
        {
            public double X;
            public double Y;
            public double Z;
        };

        enum Register : byte
        {                       // Description          R/!W        Reset Value
            DEVID = 0x00,       // Device ID            R           0xE5
            // 0x01 - 0x28      RESERVED
            THRESH_TAP = 0x1D,  // Tap Threshold        R/!W        0x00
            OFSX,               // X-axis offset        R/!W        0x00
            OFSY,               // Y-axis offset        R/!W        0x00
            OFSZ,               // Z-axis offset        R/!W        0x00
            DUR,                // Tap duration         R/!W        0x00
            Latent,             // Tap latency          R/!W        0x00
            Window,             // Tap window           R/!W        0x00
            THRESH_ACT,         // Activity threshold   R/!W        0x00
            THRESH_INACT,       // Inactivity threshold R/!W        0x00
            TIME_INACT,         // Inactivity time      R/!W        0x00
            ACT_INACT_CTL,      // Axis enable control for activity and inactivity detection
                                //                                          R/!W        0x00  
            THRESH_FF,          // Free-fall threshold  R/!W        0x00
            TIME_FF,            // Free-fall time       R/!W        0x00
            TAP_AXES,           // Axis control for single/double tap       R/!W        0x00
            ACT_TAP_STATUS,     // Single/double tap source                 R/!W        0x00
            BW_RATE,            // Data rate and power mode control         R/!W        0x0A
            POWER_CTL,          // Power-saving features control            R/!W        0x00
            INT_ENABLE,         // Interrupt enable control                 R/!W        0x00
            INT_MAP,            // Interrupt mapping control                R/!W        0x00
            INT_SOURCE,         // Interrupt source     R           0x02
            DATA_FORMAT,        // Data format control  R/!W        0x00
            DATAX0,             // X-axis data 0        R           0x00
            DATAX1,             // X-axis data 1        R           0x00
            DATAY0,             // Y-axis data 0        R           0x00
            DATAY1,             // Y-axis data 1        R           0x00
            DATAZ0,             // Z-axis data 0        R           0x00
            DATAZ1,             // Z-axis data 1        R           0x00
            FIFO_CTL,           // FIFO control         R/!W        0x00
            FIFO_STATUS         // FIFO status          R           0x00
        };

        private enum ACT_INACT_CTL_BIT
        {
            INACT_Z_EN = 0,
            INACT_Y_EN,
            INACT_X_EN,
            INACT_AC_DC,
            ACT_Z_EN,
            ACT_Y_EN,
            ACT_X_EN,
            ACT_AC_DC,
        };

        private enum ACT_INACT_CTL_MASK
        {
            INACT_Z_EN =  0x01,
            INACT_Y_EN =  0x02,
            INACT_X_EN =  0x04,
            INACT_AC_DC = 0x08,
            ACT_Z_EN =    0x10,
            ACT_Y_EN =    0x20,
            ACT_X_EN =    0x40,
            ACT_AC_DC =   0x80
        };

        private enum TAP_AXES_BIT
        {
            TAP_Z_EN = 0,
            TAP_Y_EN,
            TAP_X_EN,
            SUPPRESS,
        };

        private enum TAP_AXES_MASK
        {
            TAP_Z_EN = 0x01,
            TAP_Y_EN = 0x02,
            TAP_X_EN = 0x04,
            SUPPRESS = 0x08,
        };

        private enum ACT_TAP_STATUS_BIT
        {
            TAP_Z_SOURCE = 0,
            TAP_Y_SOURCE,
            TAP_X_SOURCE,
            ASLEEP,
            ACT_Z_SOURCE,
            ACT_Y_SOURCE,
            ACT_X_SOURCE,
        };

        private enum ACT_TAP_STATUS_MASK
        {
            TAP_Z_SOURCE = 0x01,
            TAP_Y_SOURCE = 0x02,
            TAP_X_SOURCE = 0x04,
            ASLEEP =       0x08,
            ACT_Z_SOURCE = 0x10,
            ACT_Y_SOURCE = 0x20,
            ACT_X_SOURCE = 0x40,
        };

        private enum BW_RATE_BIT
        {
            RATE = 0,
            LOW_POWER = 4,
        };

        private enum BW_RATE_MASK
        {
            RATE =      0x0F,
            LOW_POWER = 0x10,
        };

        private enum POWER_CTL_BIT
        {
            WAKEUP = 0,
            SLEEP = 2,
            MEASURE = 3,
            AUT_SLEEP = 4,
            LINK = 5,
        };

        private enum POWER_CTL_MASK
        {
            WAKEUP =  0x03,
            SLEEP =   0x04,
            MEASURE = 0x08,
            LINK =    0x10,
        };

        private enum INT_ENABLE_BIT
        {
            OVERRUN = 0,
            WATERMARK,
            FREE_FALL,
            INACTIVITY,
            ACTIVITY,
            DOUBLE_TAP,
            SINGLE_TAP,
            DATA_READY,
        };

        private enum INT_ENABLE_MASK
        {
            OVERRUN =       0x01,
            WATERMARK =     0x02,
            FREE_FALL =     0x04,
            INACTIVITY =    0x08,
            ACTIVITY =      0x10,
            DOUBLE_TAP =    0x20,
            SINGLE_TAP =    0x40,
            DATA_READY =    0x80,
        };

        private enum INT_MAP_BIT
        {
            OVERRUN = 0,
            WATERMARK,
            FREE_FALL,
            INACTIVITY,
            ACTIVITY,
            DOUBLE_TAP,
            SINGLE_TAP,
            DATA_READY,
        };

        private enum INT_MAP_MASK
        {
            OVERRUN = 0x01,
            WATERMARK = 0x02,
            FREE_FALL = 0x04,
            INACTIVITY = 0x08,
            ACTIVITY = 0x10,
            DOUBLE_TAP = 0x20,
            SINGLE_TAP = 0x40,
            DATA_READY = 0x80,
        };

        private enum INT_SOURCE_BIT
        {
            OVERRUN = 0,
            WATERMARK,
            FREE_FALL,
            INACTIVITY,
            ACTIVITY,
            DOUBLE_TAP,
            SINGLE_TAP,
            DATA_READY,
        };

        private enum INT_SOURCE_MASK
        {
            OVERRUN = 0x01,
            WATERMARK = 0x02,
            FREE_FALL = 0x04,
            INACTIVITY = 0x08,
            ACTIVITY = 0x10,
            DOUBLE_TAP = 0x20,
            SINGLE_TAP = 0x40,
            DATA_READY = 0x80,
        };

        private enum DATA_FORMAT_BIT
        {
            RANGE =      0,
            JUSTIFY =    2,
            FULL_RES =   3,
            INT_INVERT = 5,
            SPI =        6,
            SELF_TEST =  7,
        };

        private enum DATA_FORMAT_MASK
        {
            RANGE =      0x03,
            JUSTIFY =    0x04,
            FULL_RES =   0x08,
            INT_INVERT = 0x20,
            SPI =        0x40,
            SELF_TEST =  0x80,
        };

        private enum FIFO_CTL_BIT
        {
            SAMPLES =   0,
            TRIGGER =   5,
            FIFO_MODE = 6,
        };

        private enum FIFO_CTL_MASK
        {
            SAMPLES = 0x1F,
            TRIGGER = 0x20,
            FIFO_MODE = 0xC0,
        };

        private enum FIFO_STATUS_BIT
        {
            ENTRIES = 0,
            FIF_TRIG = 7,
        };

        private enum FIFO_STATUS_MASK
        {
            ENTRIES = 0x3F,
            FIFO_TRIG = 0x80,
        };

        private const byte ACCEL_SPI_RW_BIT = 0x80;         /* Bit used in SPI transactions to indicate read/write  */
        private const byte ACCEL_SPI_MB_BIT = 0x40;         /* Bit used to indicate multi-byte SPI transactions     */

        #endregion structs, enums and consts

        SPI adxlSpi;
        private Timer periodicTimer;
        private int TimerTimeout { get; set; } = 25;

        private Acceleration accelerationData;
        private Acceleration rawAccel;

        public double currentX, previousX, currentY, previousY;
        short AccelerationRawX, AccelerationRawY, AccelerationRawZ;
        byte calX = 0, calY = 0, calZ = 0;
        public double Degrees { get; private set; }
        private bool CalibrateMode { get; set; } = false;

        private int count = 0;

        public ADXL345(Int32 chipSelect)
        {
            Initialize(chipSelect);
        }

        private void Initialize(Int32 chipSelect)
        {
            adxlSpi = new SPI(chipSelect);

            adxlSpi.SetMode(Windows.Devices.Spi.SpiMode.Mode3);
            adxlSpi.SetSharingMode(Windows.Devices.Spi.SpiSharingMode.Shared);

            byte[] WriteBuf_DataFormat = new byte[] { Convert.ToByte(Register.DATA_FORMAT), 0x00 };    // 0x00 sets range to +- 2Gs
            byte[] WriteBuf_PowerControl = new byte[] { Convert.ToByte(Register.POWER_CTL), 0x08 };    // 0x08 puts the accelerometer into measurement mode

            /* Write the register settings */
            try
            {
                adxlSpi.Write(WriteBuf_DataFormat);
                adxlSpi.Write(WriteBuf_PowerControl);
            }
            /* If the write fails display the error and stop running */
            catch (Exception ex)
            {
                throw new Exception("Failed to communicate with device: " + ex.Message + "\n\r");
                //if (uart != null)
                //{
                //    textOut = "Failed to communicate with device: " + ex.Message + "\n\r";
                //    await rootPage.uart.SendText(textOut);
                //}
                //returnValue = 4;
            }

            /* Now that everything is initialized, create a timer so we read data every 100mS */
            periodicTimer = new Timer(this.TimerCallback, null, 0, 25);
        }

        private async void TimerCallback(object state)
        {
            //string xText, yText, zText;
            string statusText;

            /* Read and format accelerometer data */
            try
            {
                accelerationData = ReadAccelerationValues();
            }
            catch (Exception ex)
            {
                //xText = "X Axis: Error";
                //yText = "Y Axis: Error";
                //zText = "Z Axis: Error";
                throw new Exception("Failed to read from Accelerometer: " + ex.Message);
            }

            if (count++ < 5)
            {
                currentX = (0.025 * accelerationData.X) + (0.975 * previousX);
                currentY = (0.025 * accelerationData.Y) + (0.975 * previousY);
                previousX = currentX;
                previousY = currentY;
            }
            else
            {
                previousX = 0.0;
                previousY = 0.0;
                double radians = Math.Atan2(currentY, currentX);
                Degrees = radians * (180 / Math.PI);
                count = 0;
            }
        }

        private Acceleration ReadAccelerationValues()
        {
            const int ACCEL_RES = 1024;                             // The ADXL345 has 10 bit resolution giving 1024 unique values
            const int ACCEL_DYN_RANGE_G = 8;                        // The ADXL345 had a total dynamic range of 8G, since we're configuring it to +-4G
            const int UNITS_PER_G = ACCEL_RES / ACCEL_DYN_RANGE_G;  // Ratio of raw int values to G units

            byte[] ReadBuf;
            byte[] RegAddrBuf;

            /* 
             * Read from the accelerometer 
             * We first write the address of the X-Axis register, then read all 3 axes into ReadBuf
             */
            ReadBuf = new byte[6 + 1];      /* Read buffer of size 6 bytes (2 bytes * 3 axes) + 1 byte padding */
            RegAddrBuf = new byte[1 + 6];   /* Register address buffer of size 1 byte + 6 bytes padding        */
            /* Register address we want to read from with read and multi-byte bit set                          */
            RegAddrBuf[0] = (byte)(Register.DATAX0) | ACCEL_SPI_RW_BIT | ACCEL_SPI_MB_BIT;
            adxlSpi.TransferFullDuplex(RegAddrBuf, 7);
            Array.Copy(ReadBuf, 1, ReadBuf, 0, 6);  /* Discard first dummy byte from read                      */

            /* Check the endianness of the system and flip the bytes if necessary */
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(ReadBuf, 0, 2);
                Array.Reverse(ReadBuf, 2, 2);
                Array.Reverse(ReadBuf, 4, 2);
            }

            /* In order to get the raw 16-bit data values, we need to concatenate two 8-bit bytes for each axis */
            AccelerationRawX = BitConverter.ToInt16(ReadBuf, 0);
            AccelerationRawY = BitConverter.ToInt16(ReadBuf, 2);
            AccelerationRawZ = BitConverter.ToInt16(ReadBuf, 4);

            rawAccel.X = AccelerationRawX;
            rawAccel.Y = AccelerationRawY;
            rawAccel.Z = AccelerationRawZ;

            /* Convert raw values to G's */
            Acceleration accel;
            accel.X = (double)AccelerationRawX / UNITS_PER_G;
            accel.Y = (double)AccelerationRawY / UNITS_PER_G;
            accel.Z = (double)AccelerationRawZ / UNITS_PER_G;

            return accel;
        }

        private async void WriteCalibrationDataToFile(byte[] data)
        {
            try
            {
                // Create calibration data file; replace if exists.
                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile sampleFile =
                    await storageFolder.CreateFileAsync("calibration.dat",
                        Windows.Storage.CreationCollisionOption.ReplaceExisting);

                string calibrationData = data[0].ToString() + " " +
                                         data[1].ToString() + " " +
                                         data[2].ToString();

                await Windows.Storage.FileIO.WriteTextAsync(sampleFile, calibrationData);
                string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            }
            catch (Exception e)
            {
                throw new Exception("Write to file error occured: " + e.ToString() + "\n\r");
            }
        }

        private async Task<byte[]> ReadCalibrationData()
        {
            try
            {
                Windows.Storage.StorageFolder storageFolder =
                    Windows.Storage.ApplicationData.Current.LocalFolder;
                Windows.Storage.StorageFile calibrationFile =
                    await storageFolder.GetFileAsync("calibration.dat");

                string data = await Windows.Storage.FileIO.ReadTextAsync(calibrationFile);
                string[] parts = data.Split(' ');
                calX = Convert.ToByte(parts[0]);
                calY = Convert.ToByte(parts[1]);
                calZ = Convert.ToByte(parts[2]);

                WriteCalibrationData(calX, calY, calZ); // write the cal data to registers
            }
            catch (Exception e)
            {
                // couldn't read file for some reason
                CalibrateMode = true;
                return new byte[] { 0, 0, 0 };
            }

            return new byte[] { calX, calY, calZ };
        }

        public void WriteCalibrationData(byte calibrationOffsetX, byte calibrationOffsetY, byte calibrationOffsetZ)
        {
            byte[] WriteBuf = { 0, calibrationOffsetX, calibrationOffsetY, calibrationOffsetZ };

            adxlSpi.Write(WriteBuf);
            WriteCalibrationDataToFile(WriteBuf);
        }




    }
}
