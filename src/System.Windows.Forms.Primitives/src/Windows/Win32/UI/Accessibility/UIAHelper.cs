// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;

namespace Windows.Win32.UI.Accessibility;

internal static class UIAHelper
{
    /// <summary>
    ///  Converts a window handle to a <see cref="VARIANT"/> for UIA purposes.
    ///  Specifically, for
    ///  <see cref="IRawElementProviderSimple.GetPropertyValue(IRawElementProviderSimple*, UIA_PROPERTY_ID, VARIANT*)"/>
    ///  with <see cref="UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId"/> passed.
    /// </summary>
    /// <param name="handle">The handle to the window</param>
    /// <returns>The <see cref="VARIANT"/> version of the window handle.</returns>
    /// <remarks>
    ///  <para>
    ///   Although <see cref="global::System.Runtime.InteropServices.Marshal.GetNativeVariantForObject"/>
    ///   with the nint/IntPtr passed in returns a VARIANT with <see cref="VARENUM.VT_INT"/>,
    ///   <see cref="IRawElementProviderSimple.GetPropertyValue(IRawElementProviderSimple*, UIA_PROPERTY_ID, VARIANT*)"/>
    ///   with <see cref="UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId"/> is expecting a VARIANT with <see cref="VARENUM.VT_I4"/>.
    ///   See <see href="https://learn.microsoft.com/windows/win32/winauto/uiauto-automation-element-propids"/>
    ///   for more details.
    ///  </para>
    /// </remarks>
    public static VARIANT WindowHandleToVariant(nint handle)
        => new()
        {
            vt = VARENUM.VT_I4,
            // Only the lower 32 bits in window handles contain significant information -
            // https://learn.microsoft.com/windows/win32/winprog64/interprocess-communication
            data = new() { intVal = (int)(handle & 0xFFFF_FFFF) }
        };
}
