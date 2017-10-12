using System;
using System.Net.Sockets;

namespace CtLib.Module.FESTO
{
    /// <summary>SVE Object</summary>
    public class SVE_Object
    {
        /// <summary>Index</summary>
        public ushort mIndex;
        /// <summary>Sub index</summary>
        public ushort mSubIndex;
        /// <summary>Object Value</summary>
        public long mValue;
        /// <summary>Data Type</summary>
        public SVE_Message.DataTypes mDataType;

        /// <summary>Send data</summary>
        /// <param name="value">Written Data</param>
        /// <param name="stream">Network Stream</param>
        public void Send(long value, ref NetworkStream stream)
        {
            if (stream != null) {
                SVE_Message Msg = new SVE_Message();

                // Satznummer
                Msg.ServiceID = 0x11;
                Msg.ObjectIndex = this.mIndex;
                Msg.ObjectSubIndex = (byte)this.mSubIndex;
                Msg.DataType = this.mDataType;
                Msg.DataLength = Msg.GetDataTypesLength();
                Msg.Data = BitConverter.GetBytes(value);

                lock (stream) {
                    stream.Write(Msg.Bytes, 0, 0x12 + (int)Msg.GetDataTypesLength() - 4);
                    stream.Read(Msg.Bytes, 0, 0x12);
                }
                this.mValue = value; 
            }
        }

        /// <summary>Read Data From Network Stream</summary>
        /// <param name="stream">Network Stream</param>
        public void Read(ref NetworkStream stream)
        {
            if (stream != null) {
                SVE_Message Msg = new SVE_Message();

                Msg.ServiceID = 0x10;
                Msg.ObjectIndex = this.mIndex;
                Msg.ObjectSubIndex = (byte)this.mSubIndex;
                Msg.DataType = this.mDataType;
                Msg.DataLength = 4;

                lock (stream) {
                    stream.Write(Msg.Bytes, 0, 0x12);
                    stream.Read(Msg.Bytes, 0, 0x12 + (int)Msg.GetDataTypesLength() - 4);
                }

                switch (this.mDataType) {
                    case SVE_Message.DataTypes.dtSINT32:
                        this.mValue = (Int32)(Msg.Data[0] | (Msg.Data[1] << 8) | (Msg.Data[2] << 16) | (Msg.Data[3] << 24));
                        break;
                    case SVE_Message.DataTypes.dtUINT32:
                        this.mValue = (UInt32)(Msg.Data[0] | (Msg.Data[1] << 8) | (Msg.Data[2] << 16) | (Msg.Data[3] << 24));
                        break;
                    case SVE_Message.DataTypes.dtSINT16:
                        this.mValue = (Int16)(Msg.Data[0] | (Msg.Data[1] << 8) | (Msg.Data[2] << 16) | (Msg.Data[3] << 24));
                        break;
                    case SVE_Message.DataTypes.dtUINT16:
                        this.mValue = (UInt16)(Msg.Data[0] | (Msg.Data[1] << 8) | (Msg.Data[2] << 16) | (Msg.Data[3] << 24));
                        break;
                    case SVE_Message.DataTypes.dtSINT08:
                        this.mValue = (sbyte)(Msg.Data[0] | (Msg.Data[1] << 8) | (Msg.Data[2] << 16) | (Msg.Data[3] << 24));
                        break;
                    case SVE_Message.DataTypes.dtUINT08:
                        this.mValue = (byte)(Msg.Data[0] | (Msg.Data[1] << 8) | (Msg.Data[2] << 16) | (Msg.Data[3] << 24));
                        break;
                } 
            }
        }

        /// <summary>Write Value</summary>
        /// <param name="value">The value want to write</param>
        /// <param name="stream">Network Stream</param>
        /// <returns>Response</returns>
        public string WriteValue(string value, ref NetworkStream stream)
        {
            //The argument 'sValue' must the provided as an hex string, with spaces (' ') between each byte
            string[] Data = value.Split(' ');
            int Size = Data.Length;
            byte[] WriteBytes = new byte[Size];

            for (int i = 0; i < Size; i++)
            {
                byte Temp = byte.Parse(Data[i], System.Globalization.NumberStyles.HexNumber);
                WriteBytes[i] = Temp;
            }

            //Writing of the message into the network stream
            stream.Write(WriteBytes, 0, WriteBytes.Length);

            //Creating a buffer array for storing the response message from the server 
            byte[] ReadBytes = new byte[256];
            string Response = "";
            Int32 ReadBytesSize = stream.Read(ReadBytes, 0, ReadBytes.Length);

            //Forming the response string with the byte array correctly displayed
            for(int j = 0 ; j < ReadBytesSize ; j++)
            {
                Response = Response + " " + ReadBytes[j].ToString("X2");
            }

            return Response;
        }

        /// <summary>Constructor of SVE Object</summary>
        /// <param name="index">index</param>
        /// <param name="subindex">Sub index</param>
        /// <param name="datatype">Data type</param>
        public SVE_Object(ushort index, ushort subindex, SVE_Message.DataTypes datatype)
        {
            this.mIndex = index;
            this.mSubIndex = subindex;
            this.mDataType = datatype;
        }
    }

    /// <summary>SVE Message</summary>
    public class SVE_Message
    {
        /// <summary>FESTO Data Types</summary>
        public enum DataTypes
        {
            /// <summary>Unsigned Integer (32Bit) : uint</summary>
            dtUINT32 = 2,
            /// <summary>Unsigned Integer (16Bit) : ushort</summary>
            dtUINT16 = 3,
            /// <summary>Unsigned Integer (8Bit) : byte</summary>
            dtUINT08 = 4,
            /// <summary>Signed Integer (32Bit) : int</summary>
            dtSINT32 = 6,
            /// <summary>Signed Integer (16Bit) : short</summary>
            dtSINT16 = 7,
            /// <summary>Signed Integer (8Bit) : sbyte</summary>
            dtSINT08 = 8
        };

        /// <summary>Buffer</summary>
        public byte[] Bytes = new byte[22];

        /// <summary>Get the length of Data Type</summary>
        /// <returns>Data Length (byte)</returns>
        public uint GetDataTypesLength()
        {
            uint ret = 4;

            switch (this.DataType)
            {
                case DataTypes.dtSINT32:
                case DataTypes.dtUINT32:
                    ret += 4;
                    break;

                case DataTypes.dtSINT16:
                case DataTypes.dtUINT16:
                    ret += 2;
                    break;

                case DataTypes.dtSINT08:
                case DataTypes.dtUINT08:
                    ret += 1;
                    break;
            }
            return ret;
        }

        /// <summary>ID</summary>
        public byte ServiceID
        {
            get 
            { 
                return Bytes[0]; 
            }
            set 
            { 
                Bytes[0] = value; 
            }
        }

        /// <summary>Message ID</summary>
        public UInt32 MessageID
        {
            get
            {
                return BitConverter.ToUInt32(Bytes, 1);
            }
            set
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                Bytes[1] = byteArray[0];
                Bytes[2] = byteArray[1];
                Bytes[3] = byteArray[2];
                Bytes[4] = byteArray[3];
            }
        }

        /// <summary>Data Length</summary>
        public UInt32 DataLength
        {
            get 
            { 
                return BitConverter.ToUInt32(Bytes, 5); 
            }
            set
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                Bytes[5] = byteArray[0];
                Bytes[6] = byteArray[1];
                Bytes[7] = byteArray[2];
                Bytes[8] = byteArray[3];
            }
        }

        /// <summary>Acknowledge</summary>
        public byte Acknowledge
        {
            get 
            { 
                return Bytes[9]; 
            }
            set 
            { 
                Bytes[9] = value; 
            }
        }

        /// <summary>Reserved</summary>
        public UInt32 Reserved
        {
            get 
            { 
                return BitConverter.ToUInt32(Bytes, 10); 
            }
            set
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                Bytes[10] = byteArray[0];
                Bytes[11] = byteArray[1];
                Bytes[12] = byteArray[2];
                Bytes[13] = byteArray[3];
            }
        }

        /// <summary>Object Index</summary>
        public UInt16 ObjectIndex
        {
            get 
            { 
                return BitConverter.ToUInt16(Bytes, 14); 
            }
            set
            {
                byte[] byteArray = BitConverter.GetBytes(value);
                Bytes[14] = byteArray[0];
                Bytes[15] = byteArray[1];
            }
        }

        /// <summary>Object Sub Index</summary>
        public byte ObjectSubIndex
        {
            get 
            { 
                return Bytes[16]; 
            }
            set 
            { 
                Bytes[16] = value; 
            }
        }

        /// <summary>Data Type</summary>
        public DataTypes DataType
        {
            get 
            { 
                return (DataTypes)Bytes[17]; 
            }
            set
            {
                Bytes[17] = (byte)value;
            }
        }

        /// <summary>Data</summary>
        public byte[] Data
        {
            get
            {
                byte[] TempArray = new byte[4];

                switch (this.DataType)
                {
                    case DataTypes.dtSINT32:
                    case DataTypes.dtUINT32:
                        TempArray[3] = Bytes[21];
                        TempArray[2] = Bytes[20];
                        TempArray[1] = Bytes[19];
                        TempArray[0] = Bytes[18];
                        break;

                    case DataTypes.dtSINT16:
                    case DataTypes.dtUINT16:
                        TempArray[3] = 0;
                        TempArray[2] = 0;
                        TempArray[1] = Bytes[19];
                        TempArray[0] = Bytes[18];
                        break;

                    case DataTypes.dtSINT08:
                    case DataTypes.dtUINT08:
                        TempArray[3] = 0;
                        TempArray[2] = 0;
                        TempArray[1] = 0;
                        TempArray[0] = Bytes[18];
                        break;
                }

                return TempArray;
            }
            set
            {
                switch (this.DataType)
                {
                    case DataTypes.dtSINT32:
                    case DataTypes.dtUINT32:
                        Bytes[18] = value[0];
                        Bytes[19] = value[1];
                        Bytes[20] = value[2];
                        Bytes[21] = value[3];
                        break;

                    case DataTypes.dtSINT16:
                    case DataTypes.dtUINT16:
                        Bytes[18] = value[0];
                        Bytes[19] = value[1];
                        Bytes[20] = 0;
                        Bytes[21] = 0;
                        break;

                    case DataTypes.dtSINT08:
                    case DataTypes.dtUINT08:
                        Bytes[18] = value[0];
                        Bytes[19] = 0;
                        Bytes[20] = 0;
                        Bytes[21] = 0;
                        break;
                }
            }
        }
    }
}
