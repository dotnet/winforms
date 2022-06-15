// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;
using static Interop;

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
                if (destType == typeof(string) && !_itemsEnum._arraysFetched)
                {
                    object? curValue = _itemsEnum._target.GetValue(_itemsEnum._target.TargetObject);
                    if (curValue == value || (curValue is not null && curValue.Equals(value)))
                    {
                        bool success = false;
                        string? val = GetDisplayString(
                            (Oleaut32.IPerPropertyBrowsing)_itemsEnum._target.TargetObject,
                            _itemsEnum._target.DISPID,
                            ref success);

                        if (success)
                        {
                            return val;
                        }
                    }
                }

                return base.ConvertTo(context, culture, value, destType);
            }
        }
    }
}
