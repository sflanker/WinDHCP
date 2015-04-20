using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace WinDHCP.Library.Configuration
{
    [ConfigurationCollection(typeof(InternetAddressElement), AddItemName = "ipAddress")]
    public class InternetAddressElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new InternetAddressElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((InternetAddressElement)element).IPAddress;
        }
    }
}
