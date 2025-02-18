// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.Design;

// TODO: Change to Internal
public class ControlBindingsConverter : TypeConverter
{
    public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
    {
        if (destinationType == typeof(string))
        {
            // return "(Bindings)";
            // return an empty string, since we don't want a meaningless
            // string displayed as the value for the expandable Bindings property
            return "";
        }

        return base.ConvertTo(context, culture, value, destinationType);
    }

    public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
    {
        if (value is ControlBindingsCollection)
        {
            ControlBindingsCollection collection = (ControlBindingsCollection)value;
            IBindableComponent control = collection.BindableComponent;

            PropertyDescriptorCollection bindableProperties = TypeDescriptor.GetProperties(control, null);
            ArrayList props = new ArrayList();
            for (int i = 0; i < bindableProperties.Count; i++)
            {
                // Create a read only binding if the data source is not one of the values we support.
                Binding binding = collection[bindableProperties[i].Name];
                bool readOnly = !(binding is null || binding.DataSource is IListSource || binding.DataSource is IList || binding.DataSource is Array);
                DesignBindingPropertyDescriptor property = new DesignBindingPropertyDescriptor(bindableProperties[i], null, readOnly);
                bool bindable = ((BindableAttribute)bindableProperties[i].Attributes[typeof(BindableAttribute)]).Bindable;
                if (bindable || !((DesignBinding)property.GetValue(collection)).IsNull)
                {
                    props.Add(property);
                }
            }

            props.Add(new AdvancedBindingPropertyDescriptor());
            PropertyDescriptor[] propArray = new PropertyDescriptor[props.Count];
            props.CopyTo(propArray, 0);
            return new PropertyDescriptorCollection(propArray);
        }

        return new PropertyDescriptorCollection([]);
    }

    public override bool GetPropertiesSupported(ITypeDescriptorContext context)
    {
        return true;
    }
}
