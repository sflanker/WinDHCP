using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace WinDHCP.Library.Configuration
{
    public class PhysicalAddressElement : ConfigurationElement
    {
        [ConfigurationProperty("physicalAddress", IsRequired = true, IsKey = true)]
        public String PhysicalAddress
        {
            get { return (String)this["physicalAddress"]; }
        }
    }
}
