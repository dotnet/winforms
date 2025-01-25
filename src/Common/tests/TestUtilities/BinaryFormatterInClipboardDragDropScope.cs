// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public readonly ref struct BinaryFormatterInClipboardDragDropScope
{
    private readonly AppContextSwitchScope _switchScope;

    public BinaryFormatterInClipboardDragDropScope(bool enable)
    {
        Monitor.Enter(typeof(BinaryFormatterInClipboardDragDropScope));
        _switchScope = new(
            AppContextSwitchNames.ClipboardDragDropEnableUnsafeBinaryFormatterSerializationSwitchName,
            GetDefaultValue,
            enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(BinaryFormatterInClipboardDragDropScope));
        }
    }

    internal static bool GetDefaultValue()
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        foreach (var assembly in assemblies)
        {
            if (assembly.FullName?.StartsWith("System.Private.Windows.Core,", StringComparison.InvariantCultureIgnoreCase) == true)
            {
                var type = assembly.GetType("System.Private.Windows.CoreAppContextSwitches")
                    ?? throw new InvalidOperationException("Could not find CoreAppContextSwitches type in System.Private.Windows.Core assembly.");

                bool value = type.TestAccessor().Dynamic.GetSwitchDefaultValue
                    (AppContextSwitchNames.ClipboardDragDropEnableUnsafeBinaryFormatterSerializationSwitchName);
                return value;
            }
        }

        throw new InvalidOperationException("Could not find System.Private.Windows.Core assembly in the test process.");
    }
}
