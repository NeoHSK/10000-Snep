using System;
using System.Collections.Generic;
using System.Text;

namespace PcscNfcSnep.POC
{
    class MeasurementMessage : Serialization
    {
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

        public override void ParseMessage(byte[] rawData)
        {
            /* Big endian to little endian */
            for (int i = 0; i < 4; i++)
            {
                Array.Reverse(rawData, 4 + 30 * i, 30);
            }

            Array.Reverse(rawData);

            Deserialize(rawData);
        }
    }
}
