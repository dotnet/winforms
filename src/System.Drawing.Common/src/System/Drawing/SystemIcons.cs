// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Shell;

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

    public static Icon Application => GetIcon(ref s_application, PInvokeCore.IDI_APPLICATION);

    public static Icon Asterisk => GetIcon(ref s_asterisk, PInvokeCore.IDI_ASTERISK);

    public static Icon Error => GetIcon(ref s_error, PInvokeCore.IDI_ERROR);

    public static Icon Exclamation => GetIcon(ref s_exclamation, PInvokeCore.IDI_EXCLAMATION);

    public static Icon Hand => GetIcon(ref s_hand, PInvokeCore.IDI_HAND);

    public static Icon Information => GetIcon(ref s_information, PInvokeCore.IDI_INFORMATION);

    public static Icon Question => GetIcon(ref s_question, PInvokeCore.IDI_QUESTION);

    public static Icon Warning => GetIcon(ref s_warning, PInvokeCore.IDI_WARNING);

    public static Icon WinLogo => GetIcon(ref s_winlogo, PInvokeCore.IDI_WINLOGO);

    public static Icon Shield => s_shield ??= new Icon(typeof(SystemIcons), "ShieldIcon.ico");

    private static Icon GetIcon(ref Icon? icon, PCWSTR iconId) =>
        icon ??= new Icon(PInvokeCore.LoadIcon(HINSTANCE.Null, iconId));

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

        SHSTOCKICONINFO info = new()
        {
            cbSize = (uint)sizeof(SHSTOCKICONINFO),
        };

        HRESULT result = PInvoke.SHGetStockIconInfo(
            (SHSTOCKICONID)stockIcon,
            (SHGSI_FLAGS)options | SHGSI_FLAGS.SHGSI_ICON,
            &info);

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

        SHSTOCKICONINFO info = new()
        {
            cbSize = (uint)sizeof(SHSTOCKICONINFO),
        };

        HRESULT result = PInvoke.SHGetStockIconInfo(
            (SHSTOCKICONID)stockIcon,
            SHGSI_FLAGS.SHGSI_ICONLOCATION,
            &info);

        // This only throws if there is an error.
        Marshal.ThrowExceptionForHR((int)result);

        HICON hicon;
        result = PInvoke.SHDefExtractIcon(
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
