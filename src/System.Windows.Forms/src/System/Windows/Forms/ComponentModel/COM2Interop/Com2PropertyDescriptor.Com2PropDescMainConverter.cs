// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal partial class Com2PropertyDescriptor
    {
        /// <summary>
        ///  We wrap all value editors in this one so we can intercept the GetTextFromValue calls for objects that
        ///  we would like to modify the display name.
        /// </summary>
        private class Com2PropDescMainConverter : Com2ExtendedTypeConverter
        {
            readonly Com2PropertyDescriptor pd;

            private const int CheckSubprops = 0;
            private const int AllowSubprops = 1;
            private const int SuppressSubprops = 2;

            private int subprops = CheckSubprops;

            public Com2PropDescMainConverter(Com2PropertyDescriptor pd, TypeConverter baseConverter) : base(baseConverter)
            {
                this.pd = pd;
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                object baseConversion = base.ConvertTo(context, culture, value, destinationType);
                if (destinationType == typeof(string))
                {
                    // if this is our current value, ask if it should be changed for display,
                    // otherwise we'll ask for our enum drop downs, which we don't wanna do!
                    //
                    if (pd.IsCurrentValue(value))
                    {
                        // don't ever do this for enum types
                        if (!pd.PropertyType.IsEnum)
                        {
                            Com2EnumConverter baseConverter = (Com2EnumConverter)GetWrappedConverter(typeof(Com2EnumConverter));
                            if (baseConverter is null)
                            {
                                return pd.GetDisplayValue((string)baseConversion);
                            }
                            else
                            {
                                return baseConverter.ConvertTo(value, destinationType);
                            }
                        }
                    }
                }

                return baseConversion;
            }

            [RequiresUnreferencedCode(TrimmingConstants.TypeConverterGetPropertiesMessage)]
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(value, attributes);

                if (props is not null && props.Count > 0)
                {
                    // Return sorted read-only collection (can't sort original because its read-only)
                    props = props.Sort();
                    PropertyDescriptor[] descs = new PropertyDescriptor[props.Count];
                    props.CopyTo(descs, 0);
                    props = new PropertyDescriptorCollection(descs, true);
                }

                return props;
            }

            public override bool GetPropertiesSupported(ITypeDescriptorContext context)
            {
                if (subprops == CheckSubprops)
                {
                    if (!base.GetPropertiesSupported(context))
                    {
                        subprops = SuppressSubprops;
                    }
                    else
                    {
                        // special case the font converter here.
                        //
                        if ((pd.valueConverter is not null && pd.valueConverter.AllowExpand) || Com2IVsPerPropertyBrowsingHandler.AllowChildProperties(pd))
                        {
                            subprops = AllowSubprops;
                        }
                    }
                }

                return (subprops == AllowSubprops);
            }
        }
    }
}
