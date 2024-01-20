// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Text;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="CreateWindowEx(WINDOW_EX_STYLE, string, string, WINDOW_STYLE, int, int, int, int, HWND, HMENU, HINSTANCE, void*)"/>
    public static unsafe HWND CreateWindowEx(
        WINDOW_EX_STYLE dwExStyle,
        string? lpClassName,
        string? lpWindowName,
        WINDOW_STYLE dwStyle,
        int X,
        int Y,
        int nWidth,
        int nHeight,
        HWND hWndParent,
        HMENU hMenu,
        HINSTANCE hInstance,
        object? lpParam)
    {
        fixed (char* cn = lpClassName)
        fixed (char* wn = lpWindowName)
        {
            if (lpParam is null)
            {
                // No need to marshal.
                return CreateWindowEx(dwExStyle, cn, wn, dwStyle, X, Y, nWidth, nHeight, hWndParent, hMenu, hInstance, null);
            }

            // Trying to replicate [MarshalAs(UnmanagedType.AsAny)] here. Note that the runtime has an AsAnyMarshaller
            // in src/coreclr/System.Private.CoreLib/src/System/StubHelpers.cs, but it isn't publicly available.
            // The implmentations for these Marshal methods can be found in marshalnative.cpp.

            // The only case we are not handling is non-blittable arrays. This is a breaking change, but it
            // is very unlikely anyone will have been doing this. If we find this to be a problem we can revist
            // and potentially port the code from StubHelpers.

            if (lpParam is StringBuilder builder)
            {
                lpParam = builder.ToString();
            }

            if (lpParam is string stringValue)
            {
                fixed (char* sv = stringValue)
                {
                    return CreateWindowEx(dwExStyle, cn, wn, dwStyle, X, Y, nWidth, nHeight, hWndParent, hMenu, hInstance, sv);
                }
            }

            int size = Marshal.SizeOf(lpParam);
            nint native = Marshal.AllocCoTaskMem(size);
            try
            {
                // This will emit IL to create a marshaller for the given object if it isn't blittable.
                Marshal.StructureToPtr(lpParam, native, fDeleteOld: false);
                return CreateWindowEx(dwExStyle, cn, wn, dwStyle, X, Y, nWidth, nHeight, hWndParent, hMenu, hInstance, (void*)native);
            }
            finally
            {
                if (native != 0)
                {
                    Marshal.DestroyStructure(native, lpParam.GetType());
                    Marshal.FreeCoTaskMem(native);
                }
            }
        }
    }
}
