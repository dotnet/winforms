// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.Design;

/// <include file='doc\DesignBindingConverter.uex' path='docs/doc[@for="DesignBindingConverter"]/*' />
/// <devdoc>
///    <para>
///       Converts data bindings for use in the design-time environment.
///    </para>
/// </devdoc>
internal class DesignBindingConverter : TypeConverter
{

    public override bool CanConvertTo(ITypeDescriptorContext? context, Type? sourceType)
    {
        return (typeof(string) == sourceType);
    }

    public override bool CanConvertFrom(ITypeDescriptorContext? context, Type? destType)
    {
        return (typeof(string) == destType);
    }

    public override object ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type? sourceType)
    {
        var designBinding = value as DesignBinding;

        if (designBinding.IsNull)
        {
            return SR.DataGridNoneString);
        }
        else
        {
            string? name = "";

            if (designBinding.DataSource is IComponent)
            {
                var component = designBinding.DataSource as IComponent;
                if (component?.Site is not null)
                {
                    name = component.Site?.Name;
                }
            }

            if (name?.Length == 0)
            {
                if (designBinding.DataSource is IListSource or IList or Array)
                {
                    name = "(List)";
                }
                else
                {
                    string? typeName = TypeDescriptor.GetClassName(designBinding.DataSource);
                    int lastDot = typeName.LastIndexOf('.');
                    if (lastDot != -1)
                    {
                        typeName = typeName[(lastDot + 1)..];
                    }

                    name = string.Format(CultureInfo.CurrentCulture, "({0})", typeName);
                }
            }

            name += " - " + designBinding.DataMember;
            return name;
        }
    }

    public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
    {
        string text = (string)value;
        if (text is null || text.Length == 0 || string.Compare(text, SR.DataGridNoneString, true, CultureInfo.CurrentCulture) == 0)
        {
            return DesignBinding.Null;
        }
        else
        {
            int dash = text.IndexOf('-');
            if (dash == -1)
            {
                throw new ArgumentException($"{SR.DesignBindingBadParseString} {text}");
            }

            string componentName = text[..(dash - 1)].Trim();
            string dataMember = text[(dash + 1)..].Trim();

            if (context is null || context.Container is null)
            {
                throw new ArgumentException($"{SR.DesignBindingContextRequiredWhenParsing} {text}");
            }

            IContainer container = DesignerUtils.CheckForNestedContainer(context.Container); // ...necessary to support SplitterPanel components

            IComponent dataSource = container.Components[componentName];
            if (dataSource is null)
            {
                if (String.Equals(componentName, "(List)", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                throw new ArgumentException($"{SR.DesignBindingComponentNotFound} {componentName}");
            }

            return new DesignBinding(dataSource, dataMember);
        }
    }
}
