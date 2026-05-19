// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

public readonly ref struct ClipboardBinaryFormatterFullCompatScope : IDisposable
{
    private readonly BinaryFormatterScope _binaryFormatterScope;
    private readonly BinaryFormatterInClipboardDragDropScope _binaryFormatterInClipboardDragDropScope;
    private readonly NrbfSerializerInClipboardDragDropScope _nrbfSerializerInClipboardDragDropScope;

    public ClipboardBinaryFormatterFullCompatScope()
    {
        _binaryFormatterScope = new(enable: true);
        _binaryFormatterInClipboardDragDropScope = new(enable: true);
        _nrbfSerializerInClipboardDragDropScope = new(enable: false);
    }

    public void Dispose()
    {
        try
        {
            _nrbfSerializerInClipboardDragDropScope.Dispose();
        }
        finally
        {
            try
            {
                _binaryFormatterInClipboardDragDropScope.Dispose();
            }
            finally
            {
                _binaryFormatterScope.Dispose();
            }
        }
    }
}
