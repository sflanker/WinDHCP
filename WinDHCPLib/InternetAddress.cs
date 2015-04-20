using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

namespace WinDHCP.Library
{
    public class InternetAddress : IComparable, IEquatable<InternetAddress>
    {
        public static readonly InternetAddress Empty = new InternetAddress(0, 0, 0, 0);
        public static readonly InternetAddress Broadcast = new InternetAddress(255, 255, 255, 255);

        private Byte[] m_Address = new Byte[] { 0, 0, 0, 0 };

        public InternetAddress(params Byte[] address)
        {
            if (address == null || address.Length != 4)
            {
                throw new ArgumentException("Address must have a length of 4.", "address");
            }

            address.CopyTo(this.m_Address, 0);
        }

        public Byte this[Int32 index]
        {
            get { return this.m_Address[index]; }
        }

        public Boolean IsEmpty
        {
            get { return this.Equals(Empty); }
        }

        public Boolean IsBroadcast
        {
            get { return this.Equals(Broadcast); }
        }

        internal InternetAddress NextAddress()
        {
            InternetAddress next = this.Copy();

            if (this.m_Address[3] == 255)
            {
                next.m_Address[3] = 0;

                if (this.m_Address[2] == 255)
                {
                    next.m_Address[2] = 0;

                    if (this.m_Address[1] == 255)
                    {
                        next.m_Address[1] = 0;

                        if (this.m_Address[0] == 255)
                        {
                            throw new InvalidOperationException();
                        }
                        else
                        {
                            next.m_Address[0] = (Byte)(this.m_Address[0] + 1);
                        }
                    }
                    else
                    {
                        next.m_Address[1] = (Byte)(this.m_Address[1] + 1);
                    }
                }
                else
                {
                    next.m_Address[2] = (Byte)(this.m_Address[2] + 1);
                }
            }
            else
            {
                next.m_Address[3] = (Byte)(this.m_Address[3] + 1);
            }

            return next;
        }

        public Int32 CompareTo(Object obj)
        {
            InternetAddress other = obj as InternetAddress;

            if (other == null)
            {
                return 1;
            }

            for (Int32 i = 0; i < 4; i++)
            {
                if (this.m_Address[i] > other.m_Address[i])
                {
                    return 1;
                }
                else if (this.m_Address[i] < other.m_Address[i])
                {
                    return -1;
                }
            }

            return 0;
        }

        public override bool Equals(Object obj)
        {
            return this.Equals(obj as InternetAddress);
        }

        public bool Equals(InternetAddress other)
        {
            return other != null &&
                this.m_Address[0] == other.m_Address[0] &&
                this.m_Address[1] == other.m_Address[1] &&
                this.m_Address[2] == other.m_Address[2] &&
                this.m_Address[3] == other.m_Address[3];
        }

        public override Int32 GetHashCode()
        {
            return BitConverter.ToInt32(this.m_Address, 0);
        }

        public override string ToString()
        {
            return String.Format("{0}.{1}.{2}.{3}", this[0], this[1], this[2], this[3]);
        }

        public InternetAddress Copy()
        {
            return new InternetAddress(this.m_Address[0], this.m_Address[1], this.m_Address[2], this.m_Address[3]);
        }

        public byte[] ToArray()
        {
            Byte[] array = new Byte[4];
            this.m_Address.CopyTo(array, 0);
            return array;
        }

        public static InternetAddress Parse(String address)
        {
            return new InternetAddress(IPAddress.Parse(address).GetAddressBytes());
        }
    }
}
