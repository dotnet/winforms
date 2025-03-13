// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.PropertyGridInternal;

public class PropertiesTab : PropertyTab
{
    public override string TabName => SR.PBRSToolTipProperties;

    public override string HelpKeyword => "vs.properties"; // do not localize.

#pragma warning disable CA1725 // Parameter names should match base declaration - publicly shipped API
    public override PropertyDescriptor? GetDefaultProperty(object obj)
#pragma warning restore CA1725
    {
        PropertyDescriptor? defaultProperty = base.GetDefaultProperty(obj);

        if (defaultProperty is null)
        {
            PropertyDescriptorCollection? properties = GetProperties(obj);
            if (properties is not null)
            {
                for (int i = 0; i < properties.Count; i++)
                {
                    if ("Name".Equals(properties[i].Name))
                    {
                        defaultProperty = properties[i];
                        break;
                    }
                }
            }
        }

        return defaultProperty;
    }

    public override PropertyDescriptorCollection? GetProperties(object component, Attribute[]? attributes)
        => GetProperties(context: null, component, attributes);

    public override PropertyDescriptorCollection? GetProperties(ITypeDescriptorContext? context, object component, Attribute[]? attributes)
    {
        attributes ??= [BrowsableAttribute.Yes];

        if (context is null)
        {
            return TypeDescriptor.GetProperties(component, attributes);
        }

        TypeConverter typeConverter = context.PropertyDescriptor is null
            ? TypeDescriptor.GetConverter(component)
            : context.PropertyDescriptor.Converter;

        return typeConverter is null || !typeConverter.GetPropertiesSupported(context)
            ? TypeDescriptor.GetProperties(component, attributes)
            : typeConverter.GetProperties(context, component, attributes);
    }
}
