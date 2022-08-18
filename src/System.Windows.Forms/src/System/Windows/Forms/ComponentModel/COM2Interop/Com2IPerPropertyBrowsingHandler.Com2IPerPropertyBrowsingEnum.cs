// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Globalization;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal partial class Com2IPerPropertyBrowsingHandler
    {
        // This exists for perf reasons.   We delay doing this until we
        // are actually asked for the array of values.
        //
        private class Com2IPerPropertyBrowsingEnum : Com2Enum
        {
            internal Com2PropertyDescriptor _target;
            private readonly string[] _names;
            private readonly uint[] _cookies;
            internal bool _arraysFetched;

            public Com2IPerPropertyBrowsingEnum(
                Com2PropertyDescriptor targetObject,
                string[] names,
                uint[] cookies)
            {
                _target = targetObject;
                _names = names;
                _cookies = cookies;
                _target = targetObject;
                _arraysFetched = false;
            }

            /// <summary>
            ///  Retrieve a copy of the value array
            /// </summary>
            public override object[] Values
            {
                get
                {
                    EnsureArrays();
                    return base.Values;
                }
            }

            /// <summary>
            ///  Retrieve a copy of the nme array.
            /// </summary>
            public override string[] Names
            {
                get
                {
                    EnsureArrays();
                    return base.Names;
                }
            }

            /// <summary>
            ///  Ensure that we have processed the caStructs into arrays
            ///  of values and strings
            /// </summary>
            private unsafe void EnsureArrays()
            {
                if (_arraysFetched)
                {
                    return;
                }

                _arraysFetched = true;

                try
                {
                    // Marshal the items.

                    Oleaut32.IPerPropertyBrowsing ppb = (Oleaut32.IPerPropertyBrowsing)_target.TargetObject;
                    int itemCount = 0;

                    Debug.Assert(_cookies is not null && _names is not null, "An item array is null");

                    if (_names.Length > 0)
                    {
                        object[] valueItems = new object[_names.Length];
                        uint cookie;

                        Debug.Assert(_cookies.Length == _names.Length, "Got uneven names and cookies");

                        // For each name item, we ask the object for it's corresponding value.

                        Type targetType = _target.PropertyType;
                        for (int i = _names.Length - 1; i >= 0; i--)
                        {
                            cookie = _cookies[i];
                            if (_names[i] is null)
                            {
                                Debug.Fail($"Bad IPerPropertyBrowsing item [{i}], name={_names?[i] ?? "(unknown)"}");
                                continue;
                            }

                            using var variant = new Oleaut32.VARIANT();
                            HRESULT hr = ppb.GetPredefinedValue(_target.DISPID, cookie, &variant);
                            if (hr == HRESULT.Values.S_OK && variant.vt != Ole32.VARENUM.EMPTY)
                            {
                                valueItems[i] = variant.ToObject();
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
                                            // oh well...
                                        }
                                    }
                                }
                            }

                            if (hr == HRESULT.Values.S_OK)
                            {
                                itemCount++;
                                continue;
                            }

                            if (itemCount > 0)
                            {
                                // Shorten the arrays to ignore the failed ones.  This isn't terribly
                                // efficient but shouldn't happen very often.  It's rare for these to fail.
                                Array.Copy(_names, i, _names, i + 1, itemCount);
                                Array.Copy(valueItems, i, valueItems, i + 1, itemCount);
                            }
                        }

                        // Pass the data to the base Com2Enum object.
                        string[] strings = new string[itemCount];
                        Array.Copy(_names, 0, strings, 0, itemCount);
                        PopulateArrays(strings, valueItems);
                    }
                }
                catch (Exception ex)
                {
                    PopulateArrays(Array.Empty<string>(), Array.Empty<object>());
                    Debug.Fail($"Failed to build IPerPropertyBrowsing editor. {ex.GetType().Name}, {ex.Message}");
                }
            }

            public override object FromString(string s)
            {
                EnsureArrays();
                return base.FromString(s);
            }

            public override string ToString(object v)
            {
                // If the value is the object's current value, then ask GetDisplay string first. This is a perf
                // improvement because this way we don't populate the arrays when an object is selected, only
                // when the dropdown is actually opened.

                if (_target.IsCurrentValue(v))
                {
                    bool success = false;

                    string displayString = GetDisplayString((Oleaut32.IPerPropertyBrowsing)_target.TargetObject, _target.DISPID, ref success);

                    if (success)
                    {
                        return displayString;
                    }
                }

                // Couldn't get a display string... do the normal thing.
                EnsureArrays();
                return base.ToString(v);
            }
        }
    }
}
