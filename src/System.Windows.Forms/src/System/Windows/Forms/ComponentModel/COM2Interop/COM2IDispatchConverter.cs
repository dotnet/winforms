// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal class Com2IDispatchConverter : Com2ExtendedTypeConverter
    {
        /// <summary>
        ///  What we return textually for null.
        /// </summary>
        protected static readonly string s_none = SR.toStringNone;

        private readonly bool _allowExpand;

        public Com2IDispatchConverter(bool allowExpand, TypeConverter baseConverter)
            : base(baseConverter)
        {
            _allowExpand = allowExpand;
        }

        public Com2IDispatchConverter(Com2PropertyDescriptor propDesc, bool allowExpand)
            : base(propDesc.PropertyType)
        {
            _allowExpand = allowExpand;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType) => false;

        public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
            => destinationType == typeof(string);

        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType != typeof(string))
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }

            if (value is null)
            {
                return s_none;
            }

            string text = ComNativeDescriptor.GetName(value);

            if (string.IsNullOrEmpty(text))
            {
                text = ComNativeDescriptor.GetClassName(value);
            }

            return text is null ? "(Object)" : (object)text;
        }

        [RequiresUnreferencedCode(TrimmingConstants.TypeConverterGetPropertiesMessage)]
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext? context, object value, Attribute[]? attributes)
            => TypeDescriptor.GetProperties(value, attributes);

        public override bool GetPropertiesSupported(ITypeDescriptorContext? context) => _allowExpand;

        public override bool GetStandardValuesSupported(ITypeDescriptorContext? context)
            // No dropdown, please!
            => false;
    }
}
