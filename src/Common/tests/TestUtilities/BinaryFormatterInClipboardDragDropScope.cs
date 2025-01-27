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
            () => AppContextSwitchScope.GetDefaultValueForSwitchInAssembly(
                AppContextSwitchNames.ClipboardDragDropEnableUnsafeBinaryFormatterSerializationSwitchName,
                "System.Private.Windows.Core",
                "System.Private.Windows.CoreAppContextSwitches"),
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
}
