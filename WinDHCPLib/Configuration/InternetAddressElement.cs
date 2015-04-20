using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace WinDHCP.Library.Configuration
{
    public class InternetAddressElement : ConfigurationElement
    {
        [ConfigurationProperty("ipAddress", IsRequired = true, IsKey = true)]
        public String IPAddress
        {
            get { return (String)this["ipAddress"]; }
        }
    }
}
