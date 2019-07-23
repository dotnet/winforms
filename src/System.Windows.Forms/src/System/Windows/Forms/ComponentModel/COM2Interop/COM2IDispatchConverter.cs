// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2IDispatchConverter : Com2ExtendedTypeConverter
    {
        readonly Com2PropertyDescriptor propDesc;

        /// <summary>
        ///  What we return textually for null.
        /// </summary>
        protected static readonly string none = SR.toStringNone;

        private readonly bool allowExpand;

        public Com2IDispatchConverter(Com2PropertyDescriptor propDesc, bool allowExpand, TypeConverter baseConverter) : base(baseConverter)
        {
            this.propDesc = propDesc;
            this.allowExpand = allowExpand;
        }

        public Com2IDispatchConverter(Com2PropertyDescriptor propDesc, bool allowExpand) : base(propDesc.PropertyType)
        {
            this.propDesc = propDesc;
            this.allowExpand = allowExpand;
        }

        /// <summary>
        ///  Determines if this converter can convert an object in the given source
        ///  type to the native type of the converter.
        /// </summary>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return false;
        }

        /// <summary>
        ///  Determines if this converter can convert an object to the given destination
        ///  type.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <summary>
        ///  Converts the given object to another type.  The most common types to convert
        ///  are to and from a string object.  The default implementation will make a call
        ///  to ToString on the object if the object is valid and if the destination
        ///  type is string.  If this cannot convert to the desitnation type, this will
        ///  throw a NotSupportedException.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value == null)
                {
                    return none;
                }

                string text = ComNativeDescriptor.Instance.GetName(value);

                if (text == null || text.Length == 0)
                {
                    text = ComNativeDescriptor.Instance.GetClassName(value);
                }

                if (text == null)
                {
                    return "(Object)";
                }
                return text;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
        {
            return TypeDescriptor.GetProperties(value, attributes);
        }

        /// <summary>
        ///  Determines if this object supports properties.  By default, this
        ///  is false.
        /// </summary>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return allowExpand;
        }

        // no dropdown, please!
        //
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return false;
        }
    }
}
