// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms
{
    internal class DataGridViewRowConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (value is DataGridView row && destinationType == typeof(InstanceDescriptor))
            {
                ConstructorInfo ctor = row.GetType().GetConstructor(Array.Empty<Type>());
                if (ctor != null)
                {
                    return new InstanceDescriptor(ctor, Array.Empty<object>(), isComplete: false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
