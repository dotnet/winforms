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
            private OleStrCAMarshaler _nameMarshaller;
            private Int32CAMarshaler _valueMarshaller;
            private bool _arraysFetched;

            public AxPerPropertyBrowsingEnum(
                AxPropertyDescriptor targetObject,
                AxHost owner,
                OleStrCAMarshaler names,
                Int32CAMarshaler values,
                bool allowUnknowns) : base(Array.Empty<string>(), Array.Empty<object>(), allowUnknowns)
            {
                _target = targetObject;
                _nameMarshaller = names;
                _valueMarshaller = values;
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
                    object[] nameItems = _nameMarshaller.Items;
                    object[] cookieItems = _valueMarshaller.Items;
                    Oleaut32.IPerPropertyBrowsing ppb = _owner.GetPerPropertyBrowsing();
                    int itemCount = 0;

                    Debug.Assert(cookieItems is not null && nameItems is not null, "An item array is null");

                    if (nameItems.Length > 0)
                    {
                        object[] valueItems = new object[cookieItems.Length];
                        int cookie;

                        Debug.Assert(cookieItems.Length == nameItems.Length, "Got uneven names and cookies");

                        // For each name item, we ask the object for it's corresponding value.
                        for (int i = 0; i < nameItems.Length; i++)
                        {
                            cookie = (int)cookieItems[i];
                            if (nameItems[i] is null || !(nameItems[i] is string))
                            {
                                Debug.Fail($"Bad IPerPropertyBrowsing item [{i}], name={(nameItems is null ? "(unknown)" : nameItems[i].ToString())}");
                                continue;
                            }

                            using var var = new Oleaut32.VARIANT();
                            HRESULT hr = ppb.GetPredefinedValue(_target.Dispid, (uint)cookie, &var);
                            if (hr == HRESULT.S_OK && var.vt != Ole32.VARENUM.EMPTY)
                            {
                                valueItems[i] = var.ToObject();
                            }
                            itemCount++;
                        }

                        // Pass the data to the base Com2Enum object.
                        if (itemCount > 0)
                        {
                            string[] strings = new string[itemCount];
                            Array.Copy(nameItems, 0, strings, 0, itemCount);
                            base.PopulateArrays(strings, valueItems);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.Fail($"Failed to build IPerPropertyBrowsing editor. {ex.GetType().Name}, {ex.Message}");
                }
            }

            internal void RefreshArrays(OleStrCAMarshaler names, Int32CAMarshaler values)
            {
                _nameMarshaller = names;
                _valueMarshaller = values;
                _arraysFetched = false;
            }

            protected override void PopulateArrays(string[] names, object[] values)
            {
                // We call base.PopulateArrays directly when we actually want to do this.
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
