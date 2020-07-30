// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.PropertyGridInternal
{
    public class PropertiesTab : PropertyTab
    {
        public override string TabName
        {
            get
            {
                return SR.PBRSToolTipProperties;
            }
        }

        public override string HelpKeyword
        {
            get
            {
                return "vs.properties"; // do not localize.
            }
        }

        public override PropertyDescriptor GetDefaultProperty(object obj)
        {
            PropertyDescriptor def = base.GetDefaultProperty(obj);

            if (def is null)
            {
                PropertyDescriptorCollection props = GetProperties(obj);
                if (props != null)
                {
                    for (int i = 0; i < props.Count; i++)
                    {
                        if ("Name".Equals(props[i].Name))
                        {
                            def = props[i];
                            break;
                        }
                    }
                }
            }
            return def;
        }

        public override PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes)
        {
            return GetProperties(null, component, attributes);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
        {
            if (attributes is null)
            {
                attributes = new Attribute[] { BrowsableAttribute.Yes };
            }

            if (context is null)
            {
                return TypeDescriptor.GetProperties(component, attributes);
            }
            else
            {
                TypeConverter tc = (context.PropertyDescriptor is null ? TypeDescriptor.GetConverter(component) : context.PropertyDescriptor.Converter);
                if (tc is null || !tc.GetPropertiesSupported(context))
                {
                    return TypeDescriptor.GetProperties(component, attributes);
                }
                else
                {
                    return tc.GetProperties(context, component, attributes);
                }
            }
        }
    }
}
