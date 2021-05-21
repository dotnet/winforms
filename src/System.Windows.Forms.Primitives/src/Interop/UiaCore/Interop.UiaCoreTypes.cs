// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        private static object? s_notSupportedValue;

        [DllImport(Libraries.UiaCore, ExactSpelling = true)]
        private static extern int UiaGetReservedNotSupportedValue([MarshalAs(UnmanagedType.IUnknown)] out object notSupportedValue);

        public static object UiaGetReservedNotSupportedValue()
        {
            if (s_notSupportedValue == null)
            {
                UiaGetReservedNotSupportedValue(out s_notSupportedValue);
            }

            return s_notSupportedValue;
        }
    }
}
