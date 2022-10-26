// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal partial class Com2IPerPropertyBrowsingHandler
    {
        /// <summary>
        ///  Used to identify the enums that we added.
        /// </summary>
        private class Com2IPerPropertyEnumConverter : Com2EnumConverter
        {
            private readonly Com2IPerPropertyBrowsingEnum _itemsEnum;

            public Com2IPerPropertyEnumConverter(Com2IPerPropertyBrowsingEnum items) : base(items)
            {
                _itemsEnum = items;
            }

            public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destType)
            {
                if (destType == typeof(string) && !_itemsEnum.ArraysFetched)
                {
                    object? currentValue = _itemsEnum.Target.GetValue(_itemsEnum.Target.TargetObject);
                    if ((currentValue == value || (currentValue is not null && currentValue.Equals(value)))
                        && _itemsEnum.Target.TargetObject is IPerPropertyBrowsing.Interface propertyBrowsing
                        && TryGetDisplayString(propertyBrowsing, _itemsEnum.Target.DISPID, out string? displayString))
                    {
                        return displayString;
                    }
                }

                return base.ConvertTo(context, culture, value, destType);
            }
        }
    }
}
