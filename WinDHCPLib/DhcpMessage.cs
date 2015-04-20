using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace WinDHCP.Library
{
    internal class DhcpData
    {
        private IPEndPoint m_Source;
        private Byte[] m_MessageBuffer;
        private Int32 m_BufferSize;
        private IAsyncResult m_Result;

        public IPEndPoint Source
        {
            get { return this.m_Source; }
            set { this.m_Source = value; }
        }

        public Byte[] MessageBuffer
        {
            get { return this.m_MessageBuffer; }
            // set { this.m_MessageBuffer = value; }
        }

        public Int32 BufferSize
        {
            get
            {
                return this.m_BufferSize;
            }

            set
            {
                this.m_BufferSize = value;

                Byte[] oldBuffer = this.m_MessageBuffer;
                this.m_MessageBuffer = new Byte[this.m_BufferSize];

                Int32 copyLen = Math.Min(oldBuffer.Length, this.m_BufferSize);
                Array.Copy(oldBuffer, this.m_MessageBuffer, copyLen);
            }
        }

        public IAsyncResult Result
        {
            get { return this.m_Result; }
            set { this.m_Result = value; }
        }

        public DhcpData(Byte[] messageBuffer)
        {
            this.m_MessageBuffer = messageBuffer;
            this.m_BufferSize = messageBuffer.Length;
        }

        public DhcpData(IPEndPoint source, Byte[] messageBuffer)
        {
            this.m_Source = source;
            this.m_MessageBuffer = messageBuffer;
            this.m_BufferSize = messageBuffer.Length;
        }
    }

    public enum DhcpOperation : byte
    {
        BootRequest = 0x01,
        BootReply
    }

    public enum HardwareType : byte
    {
        Ethernet = 0x01,
        ExperimentalEthernet,
        AmateurRadio,
        ProteonTokenRing,
        Chaos,
        IEEE802Networks,
        ArcNet,
        Hyperchnnel,
        Lanstar
    }

    public enum DhcpMessageType
    {
        Discover = 0x01,
        Offer,
        Request,
        Decline,
        Ack,
        Nak,
        Release,
        Inform,
        ForceRenew,
        LeaseQuery,
        LeaseUnassigned,
        LeaseUnknown,
        LeaseActive
    }

    public enum DhcpOption : byte
    {
        Pad = 0x00,
        SubnetMask = 0x01,
        TimeOffset = 0x02,
        Router = 0x03,
        TimeServer = 0x04,
        NameServer = 0x05,
        DomainNameServer = 0x06,
        Hostname = 0x0C,
        DomainNameSuffix = 0x0F,
        AddressRequest = 0x32,
        AddressTime = 0x33,
        DhcpMessageType = 0x35,
        DhcpAddress = 0x36,
        ParameterList = 0x37,
        DhcpMessage = 0x38,
        DhcpMaxMessageSize = 0x39,
        ClassId = 0x3C,
        ClientId = 0x3D,
        AutoConfig = 0x74,
        End = 0xFF
    }

    public class DhcpMessage
    {
        private const UInt32 DhcpOptionsMagicNumber = 1669485411;
        private const UInt32 WinDhcpOptionsMagicNumber = 1666417251;
        private const Int32 DhcpMinimumMessageSize = 236;

        private DhcpOperation m_Operation = DhcpOperation.BootRequest;
        private HardwareType m_Hardware = HardwareType.Ethernet;
        private Byte m_HardwareAddressLength;
        private Byte m_Hops;
        private Int32 m_SessionId;
        private UInt16 m_SecondsElapsed;
        private UInt16 m_Flags;
        private Byte[] m_ClientAddress = new Byte[4];
        private Byte[] m_AssignedAddress = new Byte[4];
        private Byte[] m_NextServerAddress = new Byte[4];
        private Byte[] m_RelayAgentAddress = new Byte[4];
        private Byte[] m_ClientHardwareAddress = new Byte[16];
        private Byte[] m_OptionOrdering = new Byte[] {};

        private Int32 m_OptionDataSize = 0;
        private Dictionary<DhcpOption, Byte[]> m_Options = new Dictionary<DhcpOption, Byte[]>();

        public DhcpMessage()
        {
        }

        internal DhcpMessage(DhcpData data)
            : this(data.MessageBuffer)
        {
        }

        public DhcpMessage(Byte[] data)
        {
            Int32 offset = 0;
            this.m_Operation = (DhcpOperation)data[offset++];
            this.m_Hardware = (HardwareType)data[offset++];
            this.m_HardwareAddressLength = data[offset++];
            this.m_Hops = data[offset++];

            this.m_SessionId = BitConverter.ToInt32(data, offset);
            offset += 4;

            Byte[] secondsElapsed = new Byte[2];
            Array.Copy(data, offset, secondsElapsed, 0, 2);
            this.m_SecondsElapsed = BitConverter.ToUInt16(ReverseByteOrder(secondsElapsed), 0);
            offset += 2;

            this.m_Flags = BitConverter.ToUInt16(data, offset);
            offset += 2;

            Array.Copy(data, offset, this.m_ClientAddress, 0, 4);
            offset += 4;
            Array.Copy(data, offset, this.m_AssignedAddress, 0, 4);
            offset += 4;
            Array.Copy(data, offset, this.m_NextServerAddress, 0, 4);
            offset += 4;
            Array.Copy(data, offset, this.m_RelayAgentAddress, 0, 4);
            offset += 4;
            Array.Copy(data, offset, this.m_ClientHardwareAddress, 0, 16);
            offset += 16;

            offset += 192; // Skip server host name and boot file

            if (offset + 4 < data.Length &&
                (BitConverter.ToUInt32(data, offset) == DhcpOptionsMagicNumber || BitConverter.ToUInt32(data, offset) == WinDhcpOptionsMagicNumber))
            {
                offset += 4;
                Boolean end = false;

                while (!end && offset < data.Length)
                {
                    DhcpOption option = (DhcpOption)data[offset];
                    offset++;

                    Int32 optionLen;

                    switch (option)
                    {
                        case DhcpOption.Pad:
                            continue;
                        case DhcpOption.End:
                            end = true;
                            continue;
                        default:
                            optionLen = (Int32)data[offset];
                            offset++;
                            break;
                    }

                    Byte[] optionData = new Byte[optionLen];

                    Array.Copy(data, offset, optionData, 0, optionLen);
                    offset += optionLen;

                    this.m_Options.Add(option, optionData);
                    this.m_OptionDataSize += optionLen;
                }
            }
        }

        public DhcpOperation Operation
        {
            get { return this.m_Operation; }
            set { this.m_Operation = value; }
        }

        public HardwareType Hardware
        {
            get { return this.m_Hardware; }
            set { this.m_Hardware = value; }
        }

        public Byte HardwareAddressLength
        {
            get { return this.m_HardwareAddressLength; }
            set { this.m_HardwareAddressLength = value; }
        }

        public Byte Hops
        {
            get { return this.m_Hops; }
            set { this.m_Hops = value; }
        }

        public Int32 SessionId
        {
            get { return this.m_SessionId; }
            set { this.m_SessionId = value; }
        }

        public UInt16 SecondsElapsed
        {
            get { return this.m_SecondsElapsed; }
            set { this.m_SecondsElapsed = value; }
        }

        public UInt16 Flags
        {
            get { return this.m_Flags; }
            set { this.m_Flags = value; }
        }

        public Byte[] ClientAddress
        {
            get { return this.m_ClientAddress; }
            set { CopyData(value, this.m_ClientAddress); }
        }

        public Byte[] AssignedAddress
        {
            get { return this.m_AssignedAddress; }
            set { CopyData(value, this.m_AssignedAddress); }
        }

        public Byte[] NextServerAddress
        {
            get { return this.m_NextServerAddress; }
            set { CopyData(value, this.m_NextServerAddress); }
        }

        public Byte[] RelayAgentAddress
        {
            get { return this.m_RelayAgentAddress; }
            set { CopyData(value, this.m_RelayAgentAddress); }
        }

        public Byte[] ClientHardwareAddress
        {
            get
            {
                Byte[] hardwareAddress = new Byte[this.m_HardwareAddressLength];
                Array.Copy(this.m_ClientHardwareAddress, hardwareAddress, this.m_HardwareAddressLength);
                return hardwareAddress;
            }

            set
            {
                CopyData(value, this.m_ClientHardwareAddress);
                this.m_HardwareAddressLength = (Byte)value.Length;
                for (Int32 i = value.Length; i < 16; i++)
                {
                    this.m_ClientHardwareAddress[i] = 0;
                }
            }
        }

        public Byte[] OptionOrdering
        {
            get { return this.m_OptionOrdering; }
            set { this.m_OptionOrdering = value; }
        }

        public Byte[] GetOptionData(DhcpOption option)
        {
            if (this.m_Options.ContainsKey(option))
            {
                return this.m_Options[option];
            }
            else
            {
                return null;
            }
        }

        public void AddOption(DhcpOption option, params Byte[] data)
        {
            if (option == DhcpOption.Pad || option == DhcpOption.End)
            {
                throw new ArgumentException("The Pad and End DhcpOptions cannot be added explicitly.", "option");
            }

            this.m_Options.Add(option, data);
            this.m_OptionDataSize += data.Length;
        }

        public Boolean RemoveOption(DhcpOption option)
        {
            if (this.m_Options.ContainsKey(option))
            {
                this.m_OptionDataSize -= this.m_Options[option].Length;
            }

            return this.m_Options.Remove(option);
        }

        public void ClearOptions()
        {
            this.m_OptionDataSize = 0;
            this.m_Options.Clear();
        }

        public Byte[] ToArray()
        {
            Byte[] data = new Byte[DhcpMinimumMessageSize + (this.m_Options.Count > 0 ? 4 + this.m_Options.Count * 2 + this.m_OptionDataSize + 1 : 0)];

            Int32 offset = 0;

            data[offset++] = (Byte)this.m_Operation;
            data[offset++] = (Byte)this.m_Hardware;
            data[offset++] = this.m_HardwareAddressLength;
            data[offset++] = this.m_Hops;

            BitConverter.GetBytes(this.m_SessionId).CopyTo(data, offset);
            offset += 4;

            ReverseByteOrder(BitConverter.GetBytes(this.m_SecondsElapsed)).CopyTo(data, offset);
            offset += 2;

            BitConverter.GetBytes(this.m_Flags).CopyTo(data, offset);
            offset += 2;

            this.m_ClientAddress.CopyTo(data, offset);
            offset += 4;

            this.m_AssignedAddress.CopyTo(data, offset);
            offset += 4;

            this.m_NextServerAddress.CopyTo(data, offset);
            offset += 4;

            this.m_RelayAgentAddress.CopyTo(data, offset);
            offset += 4;

            this.m_ClientHardwareAddress.CopyTo(data, offset);
            offset += 16;

            offset += 192;

            if (this.m_Options.Count > 0)
            {
                BitConverter.GetBytes(WinDhcpOptionsMagicNumber).CopyTo(data, offset);
                offset += 4;

                foreach (Byte optionId in this.m_OptionOrdering)
                {
                    DhcpOption option = (DhcpOption)optionId;
                    if (this.m_Options.ContainsKey(option))
                    {
                        data[offset++] = optionId;
                        if (this.m_Options[option] != null && this.m_Options[option].Length > 0)
                        {
                            data[offset++] = (Byte)this.m_Options[option].Length;
                            Array.Copy(this.m_Options[option], 0, data, offset, this.m_Options[option].Length);
                            offset += this.m_Options[option].Length;
                        }
                    }
                }

                foreach (DhcpOption option in this.m_Options.Keys)
                {
                    if (Array.IndexOf(this.m_OptionOrdering, (Byte)option) == -1)
                    {
                        data[offset++] = (Byte)option;
                        if (this.m_Options[option] != null && this.m_Options[option].Length > 0)
                        {
                            data[offset++] = (Byte)this.m_Options[option].Length;
                            Array.Copy(this.m_Options[option], 0, data, offset, this.m_Options[option].Length);
                            offset += this.m_Options[option].Length;
                        }
                    }
                }

                data[offset] = (Byte)DhcpOption.End;
            }

            return data;
        }

        private void CopyData(Byte[] source, Byte[] dest)
        {
            if (source.Length > dest.Length)
            {
                throw new ArgumentException(String.Format("Values must be no more than {0} bytes.", dest.Length), "value");
            }

            source.CopyTo(dest, 0);
        }

        public static Byte[] ReverseByteOrder(Byte[] source)
        {
            Byte[] reverse = new Byte[source.Length];

            Int32 j = 0;
            for (Int32 i = source.Length - 1; i >= 0; i--)
            {
                reverse[j++] = source[i];
            }

            return reverse;
        }
    }
}
