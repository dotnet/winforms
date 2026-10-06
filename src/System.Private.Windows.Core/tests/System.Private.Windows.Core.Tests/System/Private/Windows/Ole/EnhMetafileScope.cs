// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

internal sealed class EnhMetafileScope : IDisposable
{
    private nint _handle;

    private EnhMetafileScope(nint handle) => _handle = handle;

    public nint Handle => _handle;

    public static EnhMetafileScope Create() => new(EmfNativeMethods.CreateDeterministicEnhMetafile());

    public EmfPayload CreatePayload() => EmfPayload.FromHandle(_handle);

    public void Dispose()
    {
        if (_handle != 0)
        {
            EmfNativeMethods.Delete(_handle).Should().BeTrue();
            _handle = 0;
        }
    }
}
