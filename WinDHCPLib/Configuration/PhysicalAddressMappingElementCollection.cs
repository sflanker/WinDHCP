using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace WinDHCP.Library.Configuration
{
    public class PhysicalAddressMappingElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new PhysicalAddressMappingElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((PhysicalAddressMappingElement)element).PhysicalAddress;
        }
    }
}
