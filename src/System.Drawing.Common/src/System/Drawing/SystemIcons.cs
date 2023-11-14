// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Drawing;

public static class SystemIcons
{
    private static Icon? s_application;
    private static Icon? s_asterisk;
    private static Icon? s_error;
    private static Icon? s_exclamation;
    private static Icon? s_hand;
    private static Icon? s_information;
    private static Icon? s_question;
    private static Icon? s_warning;
    private static Icon? s_winlogo;
    private static Icon? s_shield;

    public static Icon Application => GetIcon(ref s_application, SafeNativeMethods.IDI_APPLICATION);

    public static Icon Asterisk => GetIcon(ref s_asterisk, SafeNativeMethods.IDI_ASTERISK);

    public static Icon Error => GetIcon(ref s_error, SafeNativeMethods.IDI_ERROR);

    public static Icon Exclamation => GetIcon(ref s_exclamation, SafeNativeMethods.IDI_EXCLAMATION);

    public static Icon Hand => GetIcon(ref s_hand, SafeNativeMethods.IDI_HAND);

    public static Icon Information => GetIcon(ref s_information, SafeNativeMethods.IDI_INFORMATION);

    public static Icon Question => GetIcon(ref s_question, SafeNativeMethods.IDI_QUESTION);

    public static Icon Warning => GetIcon(ref s_warning, SafeNativeMethods.IDI_WARNING);

    public static Icon WinLogo => GetIcon(ref s_winlogo, SafeNativeMethods.IDI_WINLOGO);

    public static Icon Shield
    {
        get
        {
            if (s_shield is null)
            {
                s_shield = new Icon(typeof(SystemIcons), "ShieldIcon.ico");
                Debug.Assert(s_shield is not null, "ShieldIcon.ico must be present as an embedded resource in System.Drawing.Common.");
            }

            return s_shield;
        }
    }

    private static Icon GetIcon(ref Icon? icon, int iconId)
    {
        return icon ??= new Icon(User32.LoadIcon(NativeMethods.NullHandleRef, (nint)iconId));
    }

#if NET8_0_OR_GREATER
    /// <summary>
    ///  Gets the specified Windows shell stock icon.
    /// </summary>
    /// <param name="stockIcon">The stock icon to retrieve.</param>
    /// <param name="options">Options for retrieving the icon.</param>
    /// <returns>The requested <see cref="Icon"/>.</returns>
    /// <remarks>
    ///  <para>
    ///   Unlike the static icon properties in <see cref="SystemIcons"/>, this API returns icons that are themed
    ///   for the running version of Windows. Additionally, the returned <see cref="Icon"/> is not cached and
    ///   should be disposed when no longer needed.
    ///  </para>
    /// </remarks>
    /// <exception cref="ArgumentException"><paramref name="stockIcon"/> is an invalid <see cref="StockIconId"/>.</exception>
    public static unsafe Icon GetStockIcon(StockIconId stockIcon, StockIconOptions options = StockIconOptions.Default)
    {
        // Note that we don't explicitly check for invalid StockIconId to allow for accessing newer ids introduced
        // in later OSes. The HRESULT returned for undefined ids gets converted to an ArgumentException.

        Shell32.SHSTOCKICONINFO info = new()
        {
            cbSize = (uint)sizeof(Shell32.SHSTOCKICONINFO),
        };

        HRESULT result = Shell32.SHGetStockIconInfo(
            (uint)stockIcon,
            (uint)options | Shell32.SHGSI_ICON,
            ref info);

        // This only throws if there is an error.
        Marshal.ThrowExceptionForHR((int)result);

        return new Icon(info.hIcon, takeOwnership: true);
    }

    /// <inheritdoc cref="GetStockIcon(StockIconId, StockIconOptions)"/>
    /// <param name="size">
    ///  The desired size. If the specified size does not exist, an existing size will be resampled to give the
    ///  requested size.
    /// </param>
    public static unsafe Icon GetStockIcon(StockIconId stockIcon, int size)
    {
        // Note that we don't explicitly check for invalid StockIconId to allow for accessing newer ids introduced
        // in later OSes. The HRESULT returned for undefined ids gets converted to an ArgumentException.

        Shell32.SHSTOCKICONINFO info = new()
        {
            cbSize = (uint)sizeof(Shell32.SHSTOCKICONINFO),
        };

        HRESULT result = Shell32.SHGetStockIconInfo(
            (uint)stockIcon,
            Shell32.SHGSI_ICONLOCATION,
            ref info);

        // This only throws if there is an error.
        Marshal.ThrowExceptionForHR((int)result);

        nint hicon = 0;
        result = Shell32.SHDefExtractIcon(
            info.szPath,
            info.iIcon,
            0,
            &hicon,
            null,
            (uint)(ushort)size << 16 | (ushort)size);

        Marshal.ThrowExceptionForHR((int)result);

        return new Icon(hicon, takeOwnership: true);
    }
#endif
}
