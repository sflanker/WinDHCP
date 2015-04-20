using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace WinDHCP.Library
{
    public class PhysicalAddress : IComparable, IEquatable<PhysicalAddress>
    {
        private Byte[] m_Address = new Byte[] { 0, 0, 0, 0, 0, 0 };

        public Byte this[Int32 index]
        {
            get { return this.m_Address[index]; }
        }

        public PhysicalAddress(params Byte[] address)
        {
            if (address == null || address.Length != 6)
            {
                throw new ArgumentException("Address must have a length of 6.", "address");
            }

            address.CopyTo(this.m_Address, 0);
        }

        public Int32 CompareTo(Object obj)
        {
            PhysicalAddress other = obj as PhysicalAddress;

            if (other == null)
            {
                return 1;
            }

            for (Int32 i = 0; i < 6; i++)
            {
                if (this.m_Address[i] > other.m_Address[i])
                {
                    return -1;
                }
                else if (this.m_Address[i] < other.m_Address[i])
                {
                    return 1;
                }
            }

            return 0;
        }

        public override bool Equals(Object obj)
        {
            return this.Equals(obj as PhysicalAddress);
        }

        public bool Equals(PhysicalAddress other)
        {
            return other != null &&
                this.m_Address[0] == other.m_Address[0] &&
                this.m_Address[1] == other.m_Address[1] &&
                this.m_Address[2] == other.m_Address[2] &&
                this.m_Address[3] == other.m_Address[3] &&
                this.m_Address[4] == other.m_Address[4] &&
                this.m_Address[5] == other.m_Address[5];
        }

        public override Int32 GetHashCode()
        {
            MD5 hashProvider = MD5.Create();
            return BitConverter.ToInt32(hashProvider.ComputeHash(this.m_Address), 0);
        }

        public InternetAddress Copy()
        {
            return new InternetAddress(this.m_Address[0], this.m_Address[1], this.m_Address[2], this.m_Address[3], this.m_Address[4], this.m_Address[5]);
        }

        public byte[] ToArray()
        {
            Byte[] array = new Byte[6];
            this.m_Address.CopyTo(array, 0);
            return array;
        }

        public static PhysicalAddress Parse(String address)
        {
            PhysicalAddress physical = new PhysicalAddress(0, 0, 0, 0, 0, 0);

            Int32 index = 0;
            Byte currentValue = 0;
            Boolean first = true;

            foreach (Char c in address)
            {
                Byte digitValue;

                switch (c)
                {
                    case '0':
                        digitValue = 0;
                        break;
                    case '1':
                        digitValue = 1;
                        break;
                    case '2':
                        digitValue = 2;
                        break;
                    case '3':
                        digitValue = 3;
                        break;
                    case '4':
                        digitValue = 4;
                        break;
                    case '5':
                        digitValue = 5;
                        break;
                    case '6':
                        digitValue = 6;
                        break;
                    case '7':
                        digitValue = 7;
                        break;
                    case '8':
                        digitValue = 8;
                        break;
                    case '9':
                        digitValue = 9;
                        break;
                    case 'A':
                    case 'a':
                        digitValue = 10;
                        break;
                    case 'B':
                    case 'b':
                        digitValue = 11;
                        break;
                    case 'C':
                    case 'c':
                        digitValue = 12;
                        break;
                    case 'D':
                    case 'd':
                        digitValue = 13;
                        break;
                    case 'E':
                    case 'e':
                        digitValue = 14;
                        break;
                    case 'F':
                    case 'f':
                        digitValue = 15;
                        break;
                    default:
                        continue;
                }

                if (first)
                {
                    currentValue = (Byte)(digitValue << 4);
                    first = false;
                }
                else
                {
                    currentValue |= digitValue;
                    physical.m_Address[index++] = currentValue;
                    first = true;
                }
            }

            return physical;
        }
    }
}
