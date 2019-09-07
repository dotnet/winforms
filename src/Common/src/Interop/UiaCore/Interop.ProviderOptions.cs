// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [Flags]
        public enum ProviderOptions
        {
            /// <summary>
            /// Indicates that this is a client-side provider
            /// </summary>
            ClientSideProvider = 0x0001,

            /// <summary>
            /// Indicates that this is a server-side provider
            /// </summary>
            ServerSideProvider = 0x0002,

            /// <summary>
            /// Indicates that this is a non-client-area provider
            /// </summary>
            NonClientAreaProvider = 0x0004,

            /// <summary>
            /// Indicates that this is an override provider
            /// </summary>
            OverrideProvider = 0x0008,

            /// <summary>
            /// Indicates that this provider handles its own focus, and does not want
            ///  UIA to set focus to the nearest HWND on its behalf when AutomationElement.SetFocus
            ///  is used. This option is typically used by providers for HWNDs that appear to take
            ///  focus without actually receiving actual Win32 focus, such as menus and dropdowns
            /// </summary>
            ProviderOwnsSetFocus = 0x0010,

            /// <summary>
            ///  Indicates that this provider expects to be called according to COM threading rules:
            ///  if the provider is in a Single-Threaded Apartment, it will be called only on the apartment
            ///  thread. Only Server-side providers can use this option.
            /// </summary>
            UseComThreading = 0x0020
        }
    }
}
