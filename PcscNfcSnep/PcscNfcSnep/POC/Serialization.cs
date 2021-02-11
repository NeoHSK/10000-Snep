using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace PcscNfcSnep.POC
{
    public abstract class Serialization
    {
        public abstract void ResponseMessage(byte[] rawData);
        public abstract byte[] RequestMessage();

        public byte[] Serialize()
        {
            Type type = this.GetType();
            FieldInfo[] properties = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            List<byte> list = new List<byte>();
            foreach (var p in properties)
            {
                list.AddRange(ConvertToByte(p.GetValue(this)));
            }
            return list.ToArray();
        }

        public void Deserialize(byte[] value)
        {
            Type type = this.GetType();
            FieldInfo[] properties = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            int idx = 0;

            foreach (var p in properties)
            {
                p.SetValue(this, ConvertToField(p.GetValue(this), value, ref idx));
            }
        }

        private object ConvertToField(object value, byte[] data, ref int idx)
        {
            object convertedValue = null;

            if (value is byte) { convertedValue = data[idx++]; }
            else if (value is sbyte) { convertedValue = (sbyte)data[idx++];  }
            else if (value is ushort) { convertedValue = BitConverter.ToUInt16(data, idx); idx += sizeof(ushort); }
            else if (value is uint) { convertedValue = BitConverter.ToUInt32(data, idx); idx += sizeof(uint); }
            else if (value is char[]) { char[] temp = (char[])value; Array.Copy(data, idx, temp, 0, temp.Length); idx += temp.Length; convertedValue = temp; }
            return convertedValue;
        }

        private byte[] ConvertToByte(object value)
        {
            if (value is byte) { return new byte[] { (byte)value }; }
            else if(value is sbyte) { return new byte[] { Convert.ToByte((sbyte)value) }; }
            else if (value is ushort) { return BitConverter.GetBytes((ushort)value); }
            else if (value is uint) { return BitConverter.GetBytes((uint)value); }
            else if (value is char[]) { return (byte[])value; }
            return null;
        }
    }
}
