using System;
using System.Collections.Generic;
using System.Text;

namespace WinDHCP.Library
{
    internal class AddressLease
    {
        private PhysicalAddress m_Owner;
        private InternetAddress m_Address;
        private DateTime m_Expiration;
        private Int32 m_SessionId;
        private Boolean m_Acknowledged;

        public PhysicalAddress Owner
        {
            get { return this.m_Owner; }
            set { this.m_Owner = value; }
        }

        public InternetAddress Address
        {
            get { return this.m_Address; }
            set { this.m_Address = value; }
        }

        public DateTime Expiration
        {
            get { return this.m_Expiration; }
            set { this.m_Expiration = value; }
        }

        public Int32 SessionId
        {
            get { return m_SessionId; }
            set { m_SessionId = value; }
        }

        public Boolean Acknowledged
        {
            get { return m_Acknowledged; }
            set { m_Acknowledged = value; }
        }

        public AddressLease(PhysicalAddress owner, InternetAddress address)
            : this(owner, address, DateTime.Now.AddDays(1))
        {
        }

        public AddressLease(PhysicalAddress owner, InternetAddress address, DateTime expiration)
        {
            this.m_Owner = owner;
            this.m_Address = address;
            this.m_Expiration = expiration;
        }
    }
}
