// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization.Formatters.Binary;

namespace System;

public static class AppContextSwitchNames
{
    /// <summary>
    ///  The switch that controls whether or not the <see cref="BinaryFormatter"/> is enabled.
    /// </summary>
    public const string EnableUnsafeBinaryFormatterSerialization
        = "System.Runtime.Serialization.EnableUnsafeBinaryFormatterSerialization";

    /// <summary>
    ///  Switch that controls <see cref="AppContext"/> switch caching. This switch is set to
    ///  <see langword="true" /> in our test assemblies.
    /// </summary>
    public const string LocalAppContext_DisableCaching
        = "TestSwitch.LocalAppContext.DisableCaching";

    /// <summary>
    ///  The switch that controls whether or not the <see cref="BinaryFormatter"/> is enabled in the
    ///  Clipboard or drag-and-drop operations.
    /// </summary>
    public const string ClipboardDragDropEnableUnsafeBinaryFormatterSerializationSwitchName
        = "Windows.ClipboardDragDrop.EnableUnsafeBinaryFormatterSerialization";

    /// <summary>
    ///  The switch that controls whether or not the System.Windows.Forms.BinaryFormat.Deserializer
    ///  is enabled in the Clipboard or drag-and-drop operations.
    /// </summary>
    public const string ClipboardDragDropEnableNrbfSerializationSwitchName
        = "Windows.ClipboardDragDrop.EnableNrbfSerialization";
}
