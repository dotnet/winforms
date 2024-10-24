// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

public readonly ref struct BinaryFormatterInClipboardScope
{
    private readonly AppContextSwitchScope _switchScope;

    public BinaryFormatterInClipboardScope(bool enable)
    {
        Monitor.Enter(typeof(BinaryFormatterInClipboardScope));
        _switchScope = new(AppContextSwitchNames.ClipboardDragDropEnableUnsafeBinaryFormatterSerializationSwitchName, enable);
    }

    public void Dispose()
    {
        try
        {
            _switchScope.Dispose();
        }
        finally
        {
            Monitor.Exit(typeof(BinaryFormatterInClipboardScope));
        }
    }
}
