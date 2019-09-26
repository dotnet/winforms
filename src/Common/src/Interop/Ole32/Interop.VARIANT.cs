// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [StructLayout(LayoutKind.Sequential)]
        public sealed class VARIANT
        {
            public VARENUM vt;
            public short reserved1;
            public short reserved2;
            public short reserved3;

            public IntPtr data1;

            public IntPtr data2;

            public bool Byref => (vt & VARENUM.BYREF) != 0;

            public void Clear()
            {
                if ((vt == VARENUM.UNKNOWN || vt == VARENUM.DISPATCH) && data1 != IntPtr.Zero)
                {
                    Marshal.Release(data1);
                }

                if (vt == VARENUM.BSTR && data1 != IntPtr.Zero)
                {
                    Oleaut32.SysFreeString(data1);
                }

                data1 = data2 = IntPtr.Zero;
                vt = VARENUM.EMPTY;
            }

            ~VARIANT() => Clear();

            public object ToObject()
            {
                IntPtr val = data1;
                long longVal;

                VARENUM vtType = vt & ~VARENUM.TYPEMASK;
                switch (vtType)
                {
                    case VARENUM.EMPTY:
                        return null;
                    case VARENUM.NULL:
                        return Convert.DBNull;

                    case VARENUM.I1:
                        if (Byref)
                        {
                            val = (IntPtr)Marshal.ReadByte(val);
                        }
                        return (sbyte)(0xFF & (sbyte)val);

                    case VARENUM.UI1:
                        if (Byref)
                        {
                            val = (IntPtr)Marshal.ReadByte(val);
                        }

                        return (byte)(0xFF & (byte)val);

                    case VARENUM.I2:
                        if (Byref)
                        {
                            val = (IntPtr)Marshal.ReadInt16(val);
                        }
                        return (short)(0xFFFF & (short)val);

                    case VARENUM.UI2:
                        if (Byref)
                        {
                            val = (IntPtr)Marshal.ReadInt16(val);
                        }
                        return (ushort)(0xFFFF & (ushort)val);

                    case VARENUM.I4:
                    case VARENUM.INT:
                        if (Byref)
                        {
                            val = (IntPtr)Marshal.ReadInt32(val);
                        }
                        return (int)val;

                    case VARENUM.UI4:
                    case VARENUM.UINT:
                        if (Byref)
                        {
                            val = (IntPtr)Marshal.ReadInt32(val);
                        }
                        return (uint)val;

                    case VARENUM.I8:
                    case VARENUM.UI8:
                        if (Byref)
                        {
                            longVal = Marshal.ReadInt64(val);
                        }
                        else
                        {
                            longVal = ((uint)data1 & 0xffffffff) | ((uint)data2 << 32);
                        }

                        if (vt == VARENUM.I8)
                        {
                            return (long)longVal;
                        }
                        else
                        {
                            return (ulong)longVal;
                        }
                }

                if (Byref)
                {
                    val = GetRefInt(val);
                }

                switch (vtType)
                {
                    case VARENUM.R4:
                    case VARENUM.R8:

                        // can I use unsafe here?
                        throw new FormatException(SR.CannotConvertIntToFloat);

                    case VARENUM.CY:
                        // internally currency is 8-byte int scaled by 10,000
                        longVal = ((uint)data1 & 0xffffffff) | ((uint)data2 << 32);
                        return new decimal(longVal);
                    case VARENUM.DATE:
                        throw new FormatException(SR.CannotConvertDoubleToDate);

                    case VARENUM.BSTR:
                    case VARENUM.LPWSTR:
                        return Marshal.PtrToStringUni(val);

                    case VARENUM.LPSTR:
                        return Marshal.PtrToStringAnsi(val);

                    case VARENUM.DISPATCH:
                    case VARENUM.UNKNOWN:
                        {
                            return Marshal.GetObjectForIUnknown(val);
                        }

                    case VARENUM.HRESULT:
                        return val;

                    case VARENUM.DECIMAL:
                        longVal = ((uint)data1 & 0xffffffff) | ((uint)data2 << 32);
                        return new decimal(longVal);

                    case VARENUM.BOOL:
                        return (val != IntPtr.Zero);

                    case VARENUM.VARIANT:
                        VARIANT varStruct = Marshal.PtrToStructure<VARIANT>(val);
                        return varStruct.ToObject();

                    case VARENUM.CLSID:
                        Guid guid = Marshal.PtrToStructure<Guid>(val);
                        return guid;

                    case VARENUM.FILETIME:
                        longVal = ((uint)data1 & 0xffffffff) | ((uint)data2 << 32);
                        return new DateTime(longVal);

                    case VARENUM.USERDEFINED:
                        throw new ArgumentException(string.Format(SR.COM2UnhandledVT, "USERDEFINED"));

                    case VARENUM.ARRAY:
                    case VARENUM.VOID:
                    case VARENUM.PTR:
                    case VARENUM.SAFEARRAY:
                    case VARENUM.CARRAY:

                    case VARENUM.RECORD:
                    case VARENUM.BLOB:
                    case VARENUM.STREAM:
                    case VARENUM.STORAGE:
                    case VARENUM.STREAMED_OBJECT:
                    case VARENUM.STORED_OBJECT:
                    case VARENUM.BLOB_OBJECT:
                    case VARENUM.CF:
                    case VARENUM.BSTR_BLOB:
                    case VARENUM.VECTOR:
                    case VARENUM.BYREF:
                    default:
                        throw new ArgumentException(string.Format(SR.COM2UnhandledVT, vt));
                }
            }

            private static IntPtr GetRefInt(IntPtr value) => Marshal.ReadIntPtr(value);
        }
    }
}
