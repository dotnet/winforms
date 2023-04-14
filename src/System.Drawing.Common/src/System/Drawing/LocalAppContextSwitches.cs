// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System;

internal static partial class LocalAppContextSwitches
{
    private static int s_dontSupportPngFramesInIcons;
    public static bool DontSupportPngFramesInIcons
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(@"Switch.System.Drawing.DontSupportPngFramesInIcons", ref s_dontSupportPngFramesInIcons);
    }

    private static int s_optimizePrintPreview;
    public static bool OptimizePrintPreview
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(@"Switch.System.Drawing.Printing.OptimizePrintPreview", ref s_optimizePrintPreview);
    }

    private static int s_nullOutFontFamily;
    public static bool DoNotNullOutFontFamilyOnDispose
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(@"Switch.System.Drawing.DoNotNullOutFontFamilyOnDispose", ref s_nullOutFontFamily);
    }

    private static int s_fontConsiderDisposedState;
    public static bool DoNotConsiderDisposedStateInFontEquals
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(@"Switch.System.Drawing.DoNotNullOutFontFamilyOnDispose", ref s_fontConsiderDisposedState);
    }
}
