// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal partial class Com2IPerPropertyBrowsingHandler
{
    // This exists for perf reasons. We delay doing this until we are actually asked for the array of values.
    [RequiresUnreferencedCode(ComNativeDescriptor.ComTypeDescriptorsMessage + " Uses Com2IPerPropertyBrowsingHandler which is not trim-compatible.")]
    private unsafe class Com2IPerPropertyBrowsingEnum : Com2Enum
    {
        private readonly string?[] _names;
        private readonly uint[] _cookies;

        public Com2IPerPropertyBrowsingEnum(
            Com2PropertyDescriptor targetObject,
            string?[] names,
            uint[] cookies)
        {
            Target = targetObject;
            _names = names;
            _cookies = cookies;
            ArraysFetched = false;
        }

        internal Com2PropertyDescriptor Target { get; private set; }
        internal bool ArraysFetched { get; private set; }

        public override object[] Values
        {
            get
            {
                EnsureArrays();
                return base.Values;
            }
        }

        public override string[] Names
        {
            get
            {
                EnsureArrays();
                return base.Names;
            }
        }

        private unsafe void EnsureArrays()
        {
            if (ArraysFetched)
            {
                return;
            }

            ArraysFetched = true;

            try
            {
                // Marshal the items.

                using var ppb = ComHelpers.TryGetComScope<IPerPropertyBrowsing>(Target.TargetObject, out HRESULT hr);

                if (hr.Failed)
                {
                    PopulateArrays([], []);
                    return;
                }

                int itemCount = 0;

                Debug.Assert(_cookies is not null && _names is not null, "An item array is null");

                if (_names.Length == 0)
                {
                    return;
                }

                object[] valueItems = new object[_names.Length];
                uint cookie;

                Debug.Assert(_cookies.Length == _names.Length, "Got uneven names and cookies");

                // For each name item, we ask the object for it's corresponding value.

                Type? targetType = Target.PropertyType;

                if (targetType is null)
                {
                    PopulateArrays([], []);
                    return;
                }

                for (int i = _names.Length - 1; i >= 0; i--)
                {
                    cookie = _cookies[i];
                    if (_names[i] is null)
                    {
                        Debug.Fail($"Bad IPerPropertyBrowsing item [{i}], name={_names?[i] ?? "(unknown)"}");
                        continue;
                    }

                    using VARIANT variant = default;
                    hr = ppb.Value->GetPredefinedValue(Target.DISPID, cookie, &variant);
                    if (hr.Succeeded && variant.Type != VARENUM.VT_EMPTY)
                    {
                        valueItems[i] = variant.ToObject()!;
                        if (valueItems[i].GetType() != targetType)
                        {
                            if (targetType.IsEnum)
                            {
                                valueItems[i] = Enum.ToObject(targetType, valueItems[i]);
                            }
                            else
                            {
                                try
                                {
                                    valueItems[i] = Convert.ChangeType(valueItems[i], targetType, CultureInfo.InvariantCulture);
                                }
                                catch
                                {
                                }
                            }
                        }
                    }

                    if (hr == HRESULT.S_OK)
                    {
                        itemCount++;
                        continue;
                    }

                    if (itemCount > 0)
                    {
                        // Shorten the arrays to ignore the failed ones. This isn't terribly
                        // efficient but shouldn't happen very often. It's rare for these to fail.
                        Array.Copy(_names, i, _names, i + 1, itemCount);
                        Array.Copy(valueItems, i, valueItems, i + 1, itemCount);
                    }
                }

                // Pass the data to the base Com2Enum object.
                string[] strings = new string[itemCount];
                Array.Copy(_names, 0, strings, 0, itemCount);
                PopulateArrays(strings, valueItems);
            }
            catch (Exception ex)
            {
                PopulateArrays([], []);
                Debug.Fail($"Failed to build IPerPropertyBrowsing editor. {ex.GetType().Name}, {ex.Message}");
            }
        }

        public override object FromString(string s)
        {
            EnsureArrays();
            return base.FromString(s);
        }

        public override string ToString(object? v)
        {
            // If the value is the object's current value, then ask GetDisplay string first. This is a perf
            // improvement because this way we don't populate the arrays when an object is selected, only
            // when the dropdown is actually opened.
            if (Target.IsLastKnownValue(v))
            {
                using var propertyBrowsing = ComHelpers.TryGetComScope<IPerPropertyBrowsing>(Target.TargetObject, out HRESULT hr);
                if (hr.Succeeded && TryGetDisplayString(propertyBrowsing, Target.DISPID, out string? displayString))
                {
                    return displayString;
                }
            }

            EnsureArrays();
            return base.ToString(v);
        }
    }
}
