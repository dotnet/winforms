// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        /// <summary>
        ///  Implemented by providers which want to provide information about or want to
        ///  reposition contained HWND-based elements.
        /// </summary>
        [Guid("1d5df27c-8947-4425-b8d9-79787bb460b8")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IRawElementProviderHwndOverride
        {
            /// <summary>
            ///  Request a provider for the specified component. The returned provider can supply additional
            ///  properties or override properties of the specified component.
            /// </summary>
            /// <param name="hwnd">The window handle of the component.</param>
            /// <returns>Return the provider for the specified component, or null if the component is not being overridden.</returns>
            IRawElementProviderSimple? GetOverrideProviderForHwnd(IntPtr hwnd);
        }
    }
}
