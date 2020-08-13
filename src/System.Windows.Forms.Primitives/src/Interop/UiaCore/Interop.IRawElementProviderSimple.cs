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
        [Guid("D6DD68D1-86FD-4332-8666-9ABEDEA2D24C")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IRawElementProviderSimple
        {
            /// <summary>
            ///  Indicates the type of provider this is, for example, whether it is a client-side
            ///  or server-side provider.
            /// </summary>
            /// <remarks>
            ///  Providers must specify at least either one of ProviderOptions.ClientSideProvider
            ///  or ProviderOptions.ServerSideProvider.
            ///
            ///  UIAutomation treats different types of providers
            ///  differently - for example, events from server-side provider are broadcast to all listening
            ///  clients, whereas events from client-side providers remain in that client.
            /// </remarks>
            ProviderOptions ProviderOptions { get; }

            /// <summary>
            ///  Get a pattern interface from this object
            /// </summary>
            /// <param name="patternId">Identifier indicating the interface to return</param>
            /// <returns>Returns the interface as an object, if supported; otherwise returns null/</returns>
            [return: MarshalAs(UnmanagedType.IUnknown)]
            object? GetPatternProvider(UIA patternId);

            /// <summary>
            ///  Request value of specified property from an element.
            /// </summary>
            /// <param name="propertyId">Identifier indicating the property to return</param>
            /// <returns>Returns a ValInfo indicating whether the element supports this property, or has no value for it.</returns>
            object? GetPropertyValue(UIA propertyId);

            // Only native impl roots need to return something for this,
            // proxies always return null (cause we already know their HWNDs)
            // If proxies create themselves when handling winvents events, then they
            // also need to implement this so we can determine the HWND. Still only
            // lives on a root, however.
            /// <summary>
            ///  Returns a base provider for this element.
            ///
            ///  Typically only used by elements that correspond directly to a Win32 Window Handle,
            ///  in which case the implementation returns AutomationInteropProvider.BaseElementFromHandle( hwnd ).
            /// </summary>
            IRawElementProviderSimple? HostRawElementProvider { get; }
        }
    }
}
