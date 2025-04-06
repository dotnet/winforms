// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.Formatters.Binary;

namespace System.Private.Windows;

internal static class FeatureSwitches
{
    /// <summary>
    ///  Feature switch that wraps whether or not <see cref="BinaryFormatter"/> is enabled.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   By wrapping in a feature switch, the setting can be used to control trim warnings related to usage of
    ///   the <see cref="BinaryFormatter"/>. Code past a feature switch isn't considered for warnings if the
    ///   switch isn't enabled.
    ///  </para>
    /// </remarks>
#if NET9_0_OR_GREATER
    [FeatureSwitchDefinition("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization")]
#endif
    internal static bool EnableUnsafeBinaryFormatterSerialization =>
        !AppContext.TryGetSwitch("System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization", out bool isEnabled)
        || isEnabled;
}
