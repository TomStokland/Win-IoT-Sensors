using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;
using Communications;

namespace Sensor
{
    class BME280
    {
        #region enums and consts

        public enum Register
        {
            DEVICE_ID =             0xD0,   // reads 0x60
            RESET =                 0xE0,   // write 0xB6 to reset
            HUMIDITY_CONTROL =      0xF2,
            STATUS =                0xF3,
            MEAS_CONTROL =          0xF4,
            CONFIGURATION =         0xF5,
            PRESSURE_MSB =          0xF7,
            PRESSURE_MID_LSB =      0xF8,
            PRESSURE_LSB =          0xF9,
            TEMPERATURE_MSB =       0xFA,
            TEMPERATURE_MID_LSB =   0xFB,
            TEMPERATURE_LSB =       0xFC,
            HUMIDITY_MSB =          0xFD,
            HUMIDITY_LSB =          0xFE,
        };

        private enum CalibrationRegister
        {
            DIG_T1 = 0x88,   // UInt16
            //       0x89
            DIG_T2 = 0x8A,   // Int16
            //       0x8B
            DIG_T3 = 0x8C,   // Int16
            //       0x8D
            DIG_P1 = 0x8E,   // Int16
            //       0x8F
            DIG_P2 = 0x90,   // Int16
            //       0x91
            DIG_P3 = 0x92,   // Int16
            //       0x93
            DIG_P4 = 0x94,   // Int16
            //       0x95
            DIG_P5 = 0x96,   // Int16
            //       0x97
            DIG_P6 = 0x98,   // Int16
            //       0x99
            DIG_P7 = 0x9A,   // Int16
            //       0x9B
            DIG_P8 = 0x9C,   // Int16
            //       0x9D
            DIG_P9 = 0x9E,   // Int16
            //       0x9F
            DIG_H1 = 0xA1,
            DIG_H2 = 0xE1,   // Int16
            //       0xE2
            DIG_H3 = 0xE3,
            DIG_H4 = 0xE4,   // Int16
            //       0xE5
            DIG_H5 = 0xE5,   // Int16
            //       0xE6
            DIG_H6 = 0xE7,
        };

        private enum P_OVERSAMPLING
        {
            OFF = 0 << 2, // no pressure samples
            ONE = 1 << 2,
            TWO = 2 << 2,
            FOUR = 3 << 2,
            EIGHT = 4 << 2,
            SIXTEEN = 5 << 2
        }

        private enum T_OVERSAMPLING
        {
            OFF = 0 << 5, // no temperature samples
            ONE = 1 << 5,
            TWO = 2 << 5,
            FOUR = 3 << 5,
            EIGHT = 4 << 5,
            SIXTEEN = 5 << 5
        }

        private enum H_OVERSAMPLING
        {
            OFF = 0, // no humidity samples
            ONE = 1,
            TWO = 2,
            FOUR = 3,
            EIGHT = 4,
            SIXTEEN = 5
        }

        private enum MODE
        {
            SLEEP = 0,
            FORCED = 1,
            NORMAL = 3
        };

        private enum T_STANDBY // keys are in milliseconds
        {
            _5 = 0 << 5,
            _10 = 6 << 5,
            _20 = 7 << 5,
            _62_5 = 1 << 5,
            _125 = 2 << 5,
            _250 = 3 << 5,
            _500 = 4 << 5,
            _1000 = 5 << 5,
        };

        private enum FILTER
        {
            OFF = 0 << 2,
            _2 = 1 << 2,
            _4 = 2 << 2,
            _8 = 3 << 2,
            _16 = 4 << 2,
        };

        private enum STATUS
        {
            IMAGE_UPDATING = 0,
            MEASURING = 3,
        };

        private const byte SPI_3_WIRE_ENABLED = 1;

        #endregion enums and consts

        #region calibration data

        UInt16 dig_T1 = 0;
        Int16  dig_T2 = 0;
        Int16  dig_T3 = 0;

        UInt16 dig_P1 = 0;
        Int16  dig_P2 = 0;
        Int16  dig_P3 = 0;
        Int16  dig_P4 = 0;
        Int16  dig_P5 = 0;
        Int16  dig_P6 = 0;
        Int16  dig_P7 = 0;
        Int16  dig_P8 = 0;
        Int16  dig_P9 = 0;

        Byte   dig_H1 = 0;
        Int16  dig_H2 = 0;
        Byte   dig_H3 = 0;
        Int16  dig_H4 = 0;
        Int16  dig_H5 = 0;
        SByte  dig_H6 = 0;

        #endregion calibration data

        byte[] rawValues = new byte[8];
        float temperature = 0;
        float pressure = 0;
        float humidity = 0;

        I2C bmeSensor = new I2C(0x77, I2cBusSpeed.StandardMode);

        public BME280()
        {
            Initialize();
        }

        private void Initialize()
        {
            byte deviceId = 0;
            deviceId = ReadDeviceId();
            // set oversampling
            WriteRegister((byte)Register.HUMIDITY_CONTROL, (byte)H_OVERSAMPLING.ONE);
            WriteRegister((byte)Register.MEAS_CONTROL, (byte)T_OVERSAMPLING.ONE | (byte)P_OVERSAMPLING.ONE | (byte)MODE.FORCED);
            WriteRegister((byte)Register.CONFIGURATION, (byte)T_STANDBY._250 | (byte)FILTER.OFF);

            // get compensation data
            ReadCalibrationData();

            // get raw values
            ReadRawValues();
            CalculateTemperature();
        }

        private void WriteRegister(byte reg, byte data)
        {
            byte[] command = { reg, data };
            bmeSensor.Write(command);
        }

        public void StartMeasurement()
        {
            byte[] configurationRegister;

            configurationRegister = bmeSensor.WriteRead(new byte[] { (byte)Register.CONFIGURATION }, 1);
            configurationRegister[0] = (byte)((configurationRegister[0] & 0xFC) | (byte)MODE.FORCED);
            WriteRegister((byte)Register.CONFIGURATION, configurationRegister[0]);
        }

        private byte ReadDeviceId()
        {
            byte[] deviceId = new byte[1];
            byte[] command = { (byte)Register.DEVICE_ID };

            deviceId = bmeSensor.WriteRead(command, 1);

            return deviceId[0];
        }

        private void CalculateTemperature()
        {
            float temperature = 0.0F;
            Int32 rawTemperature = 0;
            Int32 var1 = 0, var2 = 0;

            rawTemperature = ((Int32)rawValues[3] << 12) + ((Int32)rawValues[4] << 4) + ((Int32)rawValues[5] >> 4);

            //double uncompensatedTemperature = Convert.ToDouble(rawTemperature);
            //double var1 = ((uncompensatedTemperature / 16384.0) - (dig_T1 / 1024.0)) * Convert.ToDouble(dig_T2);
            //double var2 = (((uncompensatedTemperature / 131072.0) - (Convert.ToDouble(dig_T1 / 8192.0)
            //                * (uncompensatedTemperature / 131072.0) - Convert.ToDouble(dig_T1) / 8192.0)) * Convert.ToDouble(dig_T3));

            var1 = (Int32)((((rawTemperature >> 3) - ((UInt32)dig_T1 << 1))) * ((Int32)dig_T2)) >> 11;
            var2 = (Int32)((((((rawTemperature >> 4) - (UInt32)dig_T1))) * ((rawTemperature>>4) - (UInt32)dig_T1)) >> 12) * dig_T3 >> 14;
            Int32 t_fine = var1 + var2;
            temperature = (t_fine * 5 + 128) >> 8;

            //return temperature;
        }

        private byte[] ReadRawValues()
        {
            byte[] command = { (byte)Register.PRESSURE_MSB };

            StartMeasurement();
            //while (!MeasurementDone()) ;
            rawValues = bmeSensor.WriteRead(command, 8);

            return rawValues;
        }

        private bool MeasurementDone()
        {
            bool measurementDone = false;
            byte[] status = new byte[1];
            byte[] command = { (byte)Register.STATUS };

            status = bmeSensor.WriteRead(command, 1);
            //if ((byte)(status[0] & (byte)(1 << (byte)STATUS.MEASURING)) == (byte)(1 << (byte)STATUS.MEASURING))
            if ((status[0] & 8) == 8)
                    measurementDone = true;

            return measurementDone;
        }

        private Int32 ReadPressure()
        {
            Int32 pressure = 0;

            return pressure;
        }

        private Int16 ReadHumidity()
        {
            Int16 humidity = 0;

            return humidity;
        }

        private void ReadCalibrationData()
        {
            byte[] calibrationBuffer16 = new byte[2];
            byte[] calibrationBuffer8 = new byte[1];
            byte[] commandBuffer = new byte[1];
            UInt16 ur0;
            UInt16 ur1;
            Int16 sr0;
            Int16 sr1;
            //Byte u8r0;
            //Byte s8r0;


            commandBuffer[0] = (byte)CalibrationRegister.DIG_T1;
            calibrationBuffer16 = bmeSensor.WriteRead(commandBuffer, 2);
            ur0 = (UInt16)calibrationBuffer16[0];
            ur1 = (UInt16)calibrationBuffer16[1];
            dig_T1 = (UInt16)((ur0 << 8) + ur1);

            commandBuffer[0] = (byte)CalibrationRegister.DIG_T2;
            calibrationBuffer16 = bmeSensor.WriteRead(commandBuffer, 2);
            ur0 = (UInt16)calibrationBuffer16[0];
            ur1 = (UInt16)calibrationBuffer16[1];
            dig_T2 = (Int16)((ur0 << 8) + ur1);

            commandBuffer[0] = (byte)CalibrationRegister.DIG_T3;
            calibrationBuffer16 = bmeSensor.WriteRead(commandBuffer, 2);
            ur0 = (UInt16)calibrationBuffer16[0];
            ur1 = (UInt16)calibrationBuffer16[1];
            dig_T3 = (Int16)((ur0 << 8) + ur1);

            commandBuffer[0] = (byte)CalibrationRegister.DIG_P1;
            calibrationBuffer16 = bmeSensor.WriteRead(commandBuffer, 2);
            ur0 = (UInt16)calibrationBuffer16[0];
            ur1 = (UInt16)calibrationBuffer16[1];
            dig_P1 = (UInt16)((ur0 << 8) + ur1);

            commandBuffer[0] = (byte)CalibrationRegister.DIG_P2;
            calibrationBuffer16 = bmeSensor.WriteRead(commandBuffer, 2);
            sr0 = (Int16)calibrationBuffer16[0];
            sr1 = (Int16)calibrationBuffer16[1];
            dig_P2 = (Int16)((sr0 << 8) + sr1);

            commandBuffer[0] = (byte)CalibrationRegister.DIG_P3;
            calibrationBuffer16 = bmeSensor.WriteRead(commandBuffer, 2);
            sr0 = (Int16)calibrationBuffer16[0];
            sr1 = (Int16)calibrationBuffer16[1];
            dig_P3 = (Int16)((sr0 << 8) + sr1);

            commandBuffer[0] = (byte)CalibrationRegister.DIG_P4;
            calibrationBuffer16 = bmeSensor.WriteRead(commandBuffer, 2);
            sr0 = (Int16)calibrationBuffer16[0];
            sr1 = (Int16)calibrationBuffer16[1];
            dig_P4 = (Int16)((sr0 << 8) + sr1);

            commandBuffer[0] = (byte)CalibrationRegister.DIG_P5;
            calibrationBuffer16 = bmeSensor.WriteRead(commandBuffer, 2);
            sr0 = (Int16)calibrationBuffer16[0];
            sr1 = (Int16)calibrationBuffer16[1];
            dig_P5 = (Int16)((sr0 << 8) + sr1);

            commandBuffer[0] = (byte)CalibrationRegister.DIG_P6;
            calibrationBuffer16 = bmeSensor.WriteRead(commandBuffer, 2);
            sr0 = (Int16)calibrationBuffer16[0];
            sr1 = (Int16)calibrationBuffer16[1];
            dig_P6 = (Int16)((sr0 << 8) + sr1);

            commandBuffer[0] = (byte)CalibrationRegister.DIG_P7;
            calibrationBuffer16 = bmeSensor.WriteRead(commandBuffer, 2);
            sr0 = (Int16)calibrationBuffer16[0];
            sr1 = (Int16)calibrationBuffer16[1];
            dig_P7 = (Int16)((sr0 << 8) + sr1);

            commandBuffer[0] = (byte)CalibrationRegister.DIG_P8;
            calibrationBuffer16 = bmeSensor.WriteRead(commandBuffer, 2);
            sr0 = (Int16)calibrationBuffer16[0];
            sr1 = (Int16)calibrationBuffer16[1];
            dig_P8 = (Int16)((sr0 << 8) + sr1);

            commandBuffer[0] = (byte)CalibrationRegister.DIG_P9;
            calibrationBuffer16 = bmeSensor.WriteRead(commandBuffer, 2);
            sr0 = (Int16)calibrationBuffer16[0];
            sr1 = (Int16)calibrationBuffer16[1];
            dig_P9 = (Int16)((sr0 << 8) + sr1);

            commandBuffer[0] = (byte)CalibrationRegister.DIG_H1;
            calibrationBuffer8 = bmeSensor.WriteRead(commandBuffer, 1);
            dig_H1 = calibrationBuffer8[0];

            commandBuffer[0] = (byte)CalibrationRegister.DIG_H2;
            calibrationBuffer16 = bmeSensor.WriteRead(commandBuffer, 2);
            sr0 = (Int16)calibrationBuffer16[0];
            sr1 = (Int16)calibrationBuffer16[1];
            dig_H2 = (Int16)((sr0 << 8) + sr1);

            commandBuffer[0] = (byte)CalibrationRegister.DIG_H3;
            calibrationBuffer8 = bmeSensor.WriteRead(commandBuffer, 1);
            dig_H3 = calibrationBuffer8[0];

            commandBuffer[0] = (byte)CalibrationRegister.DIG_H4;
            calibrationBuffer8 = bmeSensor.WriteRead(commandBuffer, 1);
            sr0 = Convert.ToInt16(calibrationBuffer8[0]);
            commandBuffer[0] = (byte)CalibrationRegister.DIG_H5;
            calibrationBuffer8 = bmeSensor.WriteRead(commandBuffer, 1);
            sr1 = Convert.ToSByte(calibrationBuffer8[0]);
            dig_H4 = Convert.ToSByte(sr0 | sr1);

            //commandBuffer[0] = (byte)CalibrationRegister.DIG_H6;
            //calibrationBuffer8 = bmeSensor.WriteRead(commandBuffer, 1);
            //sr0 = Convert.ToSByte((calibrationBuffer8[0]&(byte)0x0F) << 4);
            //commandBuffer[0] = (byte)CalibrationRegister.DIG_H5;
            //calibrationBuffer8 = bmeSensor.WriteRead(commandBuffer, 1);
            //sr1 = calibrationBuffer8[0];
            //dig_H5 = Convert.ToSByte((byte)sr0 | (byte)(sr1 >> 4));

        }
    }
}
