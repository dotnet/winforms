// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [ComImport]
        [Guid("c7935180-6fb3-4201-b174-7df73adbf64a")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IValueProvider
        {
            /// <summary>
            ///  Request to set the value that this UI element is representing
            /// </summary>
            /// <param name="value">Value to set the UI to</param>
            void SetValue([MarshalAs(UnmanagedType.LPWStr)] string? value);

            /// <summary>Value of a value control, as a a string.</summary>
            string? Value { get; }

            /// <summary>Indicates that the value can only be read, not modified.
            ///returns True if the control is read-only</summary>
            BOOL IsReadOnly { get; }
        }
    }
}
