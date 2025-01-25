// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows;

internal static class FeatureSwitches
{
    // Feature switch, when set to false, BinaryFormatter is not supported in trimmed applications.
    // This field, using the default BinaryFormatter switch, is used to control trim warnings related
    // to using BinaryFormatter in WinForms trimming. The trimmer will generate a warning when set
    // to true and will not generate a warning when set to false.
#if NET9_0_OR_GREATER
    [FeatureSwitchDefinition("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization")]
#endif
    internal static bool EnableUnsafeBinaryFormatterInNativeObjectSerialization =>
        !AppContext.TryGetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", out bool isEnabled) || isEnabled;
}
