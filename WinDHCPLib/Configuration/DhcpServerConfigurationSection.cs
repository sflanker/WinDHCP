using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace WinDHCP.Library.Configuration
{
    public class DhcpServerConfigurationSection : ConfigurationSection
    {
        private ConfigurationPropertyCollection m_Properties;

        [ConfigurationProperty("networkInterface")]
        public Int32 NetworkInterface
        {
            get { return (Int32)this["networkInterface"]; }
        }

        [ConfigurationProperty("startAddress", IsRequired = true)]
        public String StartAddress
        {
            get { return (String)this["startAddress"]; }
        }

        [ConfigurationProperty("endAddress", IsRequired = true)]
        public String EndAddress
        {
            get { return (String)this["endAddress"]; }
        }

        [ConfigurationProperty("subnet", IsRequired = true)]
        public String Subnet
        {
            get { return (String)this["subnet"]; }
        }

        [ConfigurationProperty("gateway", IsRequired = true)]
        public String Gateway
        {
            get { return (String)this["gateway"]; }
        }

        [ConfigurationProperty("dnsSuffix")]
        public String DnsSuffix
        {
            get { return (String)this["dnsSuffix"]; }
        }

        [ConfigurationProperty("leaseDuration")]
        public TimeSpan LeaseDuration
        {
            get { return (TimeSpan)this["leaseDuration"]; }
        }

        [ConfigurationProperty("offerTimeout")]
        public TimeSpan OfferTimeout
        {
            get { return (TimeSpan)this["offerTimeout"]; }
        }

        [ConfigurationProperty("macAllowList")]
        public PhysicalAddressElementCollection MacAllowList
        {
            get { return (PhysicalAddressElementCollection)this["macAllowList"]; }
        }

        [ConfigurationProperty("macDenyList")]
        public PhysicalAddressElementCollection MacDenyList
        {
            get { return (PhysicalAddressElementCollection)this["macDenyList"]; }
        }

        [ConfigurationProperty("macReservationList")]
        public PhysicalAddressMappingElementCollection MacReservationList
        {
            get { return (PhysicalAddressMappingElementCollection)this["macReservationList"]; }
        }

        [ConfigurationProperty("dnsServers")]
        public InternetAddressElementCollection DnsServers
        {
            get { return (InternetAddressElementCollection)this["dnsServers"]; }
        }

        protected override ConfigurationPropertyCollection Properties
        {
            get
            {
                if (this.m_Properties == null)
                {
                    this.m_Properties = new ConfigurationPropertyCollection();

                    this.m_Properties.Add(new ConfigurationProperty("networkInterface", typeof(Int32), -1));
                    this.m_Properties.Add(new ConfigurationProperty("startAddress", typeof(String), "192.168.1.100", ConfigurationPropertyOptions.IsRequired));
                    this.m_Properties.Add(new ConfigurationProperty("endAddress", typeof(String), "192.168.1.150", ConfigurationPropertyOptions.IsRequired));
                    this.m_Properties.Add(new ConfigurationProperty("subnet", typeof(String), "255.255.255.0", ConfigurationPropertyOptions.IsRequired));
                    this.m_Properties.Add(new ConfigurationProperty("gateway", typeof(String), "192.168.1.", ConfigurationPropertyOptions.IsRequired));
                    this.m_Properties.Add(new ConfigurationProperty("dnsSuffix", typeof(String), ""));
                    this.m_Properties.Add(new ConfigurationProperty("leaseDuration", typeof(TimeSpan), TimeSpan.FromDays(1)));
                    this.m_Properties.Add(new ConfigurationProperty("offerTimeout", typeof(TimeSpan), TimeSpan.FromSeconds(30)));
                    this.m_Properties.Add(new ConfigurationProperty("macAllowList", typeof(PhysicalAddressElementCollection), new PhysicalAddressElementCollection()));
                    this.m_Properties.Add(new ConfigurationProperty("macDenyList", typeof(PhysicalAddressElementCollection), new PhysicalAddressElementCollection()));
                    this.m_Properties.Add(new ConfigurationProperty("macReservationList", typeof(PhysicalAddressMappingElementCollection), new PhysicalAddressMappingElementCollection()));
                    this.m_Properties.Add(new ConfigurationProperty("dnsServers", typeof(InternetAddressElementCollection), new InternetAddressElementCollection()));
                }

                return this.m_Properties;
            }
        }
    }
}
