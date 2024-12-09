// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public partial class BinaryFormatUtilitiesTests
{
    internal readonly ref struct BinaryFormatterFullCompatScope : IDisposable
    {
        private readonly BinaryFormatterScope _binaryFormatterScope;
        private readonly BinaryFormatterInClipboardDragDropScope _binaryFormatterInClipboardDragDropScope;
        private readonly NrbfSerializerInClipboardDragDropScope _nrbfSerializerInClipboardDragDropScope;

        public BinaryFormatterFullCompatScope()
        {
            _binaryFormatterScope = new(enable: true);
            _binaryFormatterInClipboardDragDropScope = new(enable: true);
            _nrbfSerializerInClipboardDragDropScope = new(enable: false);
        }

        public void Dispose()
        {
            _binaryFormatterScope.Dispose();
            _binaryFormatterInClipboardDragDropScope.Dispose();
            _nrbfSerializerInClipboardDragDropScope.Dispose();
        }
    }
}
