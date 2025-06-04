// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public readonly ref struct NrbfSerializerInClipboardDragDropScope
{
    private readonly AppContextSwitchScope _switchScope;

    public NrbfSerializerInClipboardDragDropScope(bool enable)
    {
        Monitor.Enter(typeof(NrbfSerializerInClipboardDragDropScope));
        _switchScope = new(
            AppContextSwitchNames.ClipboardDragDropEnableNrbfSerializationSwitchName,
            () => AppContextSwitchScope.GetDefaultValueForSwitchInAssembly(
                AppContextSwitchNames.ClipboardDragDropEnableNrbfSerializationSwitchName,
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
            Monitor.Exit(typeof(NrbfSerializerInClipboardDragDropScope));
        }
    }
}
