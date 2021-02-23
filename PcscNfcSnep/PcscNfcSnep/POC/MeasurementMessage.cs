using System;
using System.Collections.Generic;
using System.Text;

namespace PcscNfcSnep.POC
{
    class MeasurementMessage : Serialization
    {
        const byte COMMAND_REMAINING = 0x55;
        const byte COMMAND_MEASUREMENT_MESSAGE = 0xAA;
        const byte RESERVED = 0x00;
        public const uint MEASUREMENT_MESSAGE_SIZE = 139;
        byte Result;
        byte Object;
        byte Unit;
        byte Type;
        byte Comment;
        byte MealInformation;
        byte Second;
        byte Minute;
        byte Hour;
        byte Day;
        byte Month;
        UInt16 Year;
        UInt16 Value;
        char[] ControlLotNumber = new char[30];
        char[] StripLotNumber = new char[30];
        char[] PatientId = new char[30];
        char[] OperatorId = new char[30];
        UInt32 SequenceNumber;

        List<MeasurementMessage> measurementMessages = new List<MeasurementMessage>();

        public List<MeasurementMessage> GetMeasurementMessages()
        {
            return measurementMessages;
        }

        public override void ResponseMessage(byte[] rawData)
        {
            // TODO Command verified
            var convSize = rawData.Length - 2;
            var conv = new byte[convSize];

            Array.Copy(rawData, 2, conv, 0, convSize);

            for (uint j = 0; j < (conv.Length / MEASUREMENT_MESSAGE_SIZE); j++)
            {
                MeasurementMessage measurementMessage = new MeasurementMessage();

                var oneMeasurement = new byte[MEASUREMENT_MESSAGE_SIZE];

                Array.Copy(conv, j*MEASUREMENT_MESSAGE_SIZE, oneMeasurement, 0, MEASUREMENT_MESSAGE_SIZE);

                /* Big endian to little endian */
                for (int i = 0; i < 4; i++)
                {
                    Array.Reverse(oneMeasurement, 4 + 30 * i, 30);
                }

                Array.Reverse(oneMeasurement);

                measurementMessage.Deserialize(oneMeasurement);

                measurementMessages.Add(measurementMessage);
            }
        }

        public override byte[] RequestMessage()
        {
            return null;
        }

        public uint ResponseRemaingCount(byte[] rawData)
        {
            // TODO Command verified
            var convSize = rawData.Length - 2;
            var conv = new byte[convSize];
            Array.Copy(rawData, 2, conv, 0, convSize);
            Array.Reverse(conv);
            return BitConverter.ToUInt32(conv, 0);
        }

        public byte[] RequestRemaingCount(uint lastSequenceNumber)
        {
            var conv = new byte[6];
            conv[0] = (byte)COMMAND_REMAINING;
            conv[1] = (byte)RESERVED;
            var byteConv = BitConverter.GetBytes(lastSequenceNumber);
            Array.Reverse(byteConv);
            Array.Copy(byteConv, 0, conv, 2, byteConv.Length);
            return conv;
        }

        public byte[] RequestMeasurementMessage(uint lastSequenceNumber)
        {
            var conv = new byte[6];
            conv[0] = (byte)COMMAND_MEASUREMENT_MESSAGE;
            conv[1] = (byte)RESERVED;
            var byteConv = BitConverter.GetBytes(lastSequenceNumber);
            Array.Reverse(byteConv);
            Array.Copy(byteConv, 0, conv, 2, byteConv.Length);
            return conv;
        }

        public override string ToString()
        {
            return $"Result - " + Result.ToString() + "\n" +
            "Object- " + Object.ToString() + "\n" +
            "Unit- " + Unit.ToString() + "\n" +
            "Type- " + Type.ToString() + "\n" +
            "Comment- " + Comment.ToString() + "\n" +
            "MealInformation- " + MealInformation.ToString() + "\n" +
            "Second- " + Second.ToString() + "\n" +
            "Minute- " + Minute.ToString() + "\n" +
            "Hour- " + Hour.ToString() + "\n" +
            "Day- " + Day.ToString() + "\n" +
            "Month- " + Month.ToString() + "\n" +
            "Year- " + Year.ToString() + "\n" +
            "Value- " + Value.ToString() + "\n" +
            "ControlLotNumber- " + new string(ControlLotNumber) + "\n" +
            "StripLotNumber- " + new string(StripLotNumber) + "\n" +
            "PatientId- " + new string(PatientId) + "\n" +
            "OperatorId- " + new string(OperatorId) + "\n" +
            "SequenceNumber - " + SequenceNumber.ToString();
        }

    }
}
