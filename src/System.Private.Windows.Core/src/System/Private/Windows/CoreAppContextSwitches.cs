// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Private.Windows;

// Borrowed from https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/LocalAppContextSwitches.Common.cs
internal static class CoreAppContextSwitches
{
    // Enabling switches in Core is different from Framework. See https://learn.microsoft.com/dotnet/core/runtime-config/
    // for details on how to set switches.
    internal const string ClipboardDragDropEnableUnsafeBinaryFormatterSerializationSwitchName = "Windows.ClipboardDragDrop.EnableUnsafeBinaryFormatterSerialization";
    internal const string ClipboardDragDropEnableNrbfSerializationSwitchName = "Windows.ClipboardDragDrop.EnableNrbfSerialization";

    private static int s_clipboardDragDropEnableUnsafeBinaryFormatterSerialization;
    private static int s_clipboardDragDropEnableNrbfSerialization;

    private static bool GetCachedSwitchValue(string switchName, ref int cachedSwitchValue)
    {
        // The cached switch value has 3 states: 0 - unknown, 1 - true, -1 - false
        if (cachedSwitchValue < 0)
        {
            return false;
        }

        if (cachedSwitchValue > 0)
        {
            return true;
        }

        return GetSwitchValue(switchName, ref cachedSwitchValue);
    }

    private static bool GetSwitchValue(string switchName, ref int cachedSwitchValue)
    {
        bool hasSwitch = AppContext.TryGetSwitch(switchName, out bool isSwitchEnabled);
        if (!hasSwitch)
        {
            isSwitchEnabled = GetSwitchDefaultValue(switchName);
        }

        AppContext.TryGetSwitch("TestSwitch.LocalAppContext.DisableCaching", out bool disableCaching);
        if (!disableCaching)
        {
            cachedSwitchValue = isSwitchEnabled ? 1 /*true*/ : -1 /*false*/;
        }
        else if (!hasSwitch)
        {
            AppContext.SetSwitch(switchName, isSwitchEnabled);
        }

        return isSwitchEnabled;
    }

    private static bool GetSwitchDefaultValue(string switchName)
    {
        if (switchName == ClipboardDragDropEnableNrbfSerializationSwitchName)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    ///  If <see langword="true"/>, then Clipboard and DataObject Get and Set methods will attempts to serialize or deserialize
    ///  binary formatted content using either <see cref="BinaryFormatter"/> or System.Windows.Forms.BinaryFormat.Deserializer.
    ///  To use <see cref="BinaryFormatter"/>, application should also opt in into the
    ///  "System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization" option and reference the out-of-band
    ///  "System.Runtime.Serialization.Formatters" NuGet package and opt out from using the System.Windows.Forms.BinaryFormat.Deserializer
    ///  by setting "Windows.ClipboardDragDrop.EnableNrbfSerialization" to <see langword="true"/>
    /// </summary>
    public static bool ClipboardDragDropEnableUnsafeBinaryFormatterSerialization
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(ClipboardDragDropEnableUnsafeBinaryFormatterSerializationSwitchName,
            ref s_clipboardDragDropEnableUnsafeBinaryFormatterSerialization);
    }

    /// <summary>
    ///  If <see langword="true"/>, then Clipboard Get methods will prefer System.Windows.Forms.BinaryFormat.Deserializer
    ///  to deserialize the payload, if needed. If <see langword="false"/>, then <see cref="BinaryFormatter"/> is used
    ///  to get full compatibility with the downlevel versions of .NET.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This switch has no effect if "Windows.ClipboardDragDrop.EnableUnsafeBinaryFormatterSerialization"
    ///   is set to <see langword="false"/>.
    ///  </para>
    /// </remarks>
    public static bool ClipboardDragDropEnableNrbfSerialization
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(ClipboardDragDropEnableNrbfSerializationSwitchName, ref s_clipboardDragDropEnableNrbfSerialization);
    }
}
