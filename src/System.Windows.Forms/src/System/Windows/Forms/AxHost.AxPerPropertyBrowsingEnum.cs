// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Windows.Forms.ComponentModel.Com2Interop;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        /// <summary>
        ///  This exists for perf reasons. We delay marshalling the data until we are actually asked for the
        ///  array of values.
        /// </summary>
        private class AxPerPropertyBrowsingEnum : Com2Enum
        {
            private readonly AxPropertyDescriptor _target;
            private readonly AxHost _owner;
            private string[] _names;
            private uint[] _cookies;
            private bool _arraysFetched;

            public AxPerPropertyBrowsingEnum(
                AxPropertyDescriptor targetObject,
                AxHost owner,
                string[] names,
                uint[] cookies)
            {
                _target = targetObject;
                _names = names;
                _cookies = cookies;
                _owner = owner;
                _arraysFetched = false;
            }

            /// <summary>
            ///  Retrieve a copy of the value array.
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
            ///  Ensure that we have processed the caStructs into arrays of values and strings.
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
                    Oleaut32.IPerPropertyBrowsing ppb = _owner.GetPerPropertyBrowsing();
                    int itemCount = 0;

                    Debug.Assert(_cookies is not null && _names is not null, "An item array is null");

                    if (_names.Length > 0)
                    {
                        object[] values = new object[_cookies.Length];
                        uint cookie;

                        Debug.Assert(_cookies.Length == _names.Length, "Got uneven names and cookies");

                        // For each name item, we ask the object for it's corresponding value.
                        for (int i = 0; i < _names.Length; i++)
                        {
                            cookie = _cookies[i];
                            if (_names[i] is null)
                            {
                                Debug.Fail($"Bad IPerPropertyBrowsing item [{i}], name={_names?[i] ?? "(unknown)"}");
                                continue;
                            }

                            using var var = new Oleaut32.VARIANT();
                            HRESULT hr = ppb.GetPredefinedValue(_target.Dispid, cookie, &var);
                            if (hr == HRESULT.Values.S_OK && var.vt != Ole32.VARENUM.EMPTY)
                            {
                                values[i] = var.ToObject();
                            }

                            itemCount++;
                        }

                        // Pass the data to the base Com2Enum object.
                        if (itemCount > 0)
                        {
                            string[] strings = new string[itemCount];
                            Array.Copy(_names, 0, strings, 0, itemCount);
                            PopulateArrays(strings, values);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Fail($"Failed to build IPerPropertyBrowsing editor. {ex.GetType().Name}, {ex.Message}");
                }
            }

            internal void RefreshArrays(string[] names, uint[] cookies)
            {
                _names = names;
                _cookies = cookies;
                _arraysFetched = false;
            }

            public override object FromString(string s)
            {
                EnsureArrays();
                return base.FromString(s);
            }

            public override string ToString(object v)
            {
                EnsureArrays();
                return base.ToString(v);
            }
        }
    }
}
