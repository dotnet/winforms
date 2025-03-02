// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.Foundation;

namespace System.Private.Windows.Ole;

/// <summary>
///  Ensures the CoreClipboard is cleared before and after a test.
/// </summary>
internal ref struct ClipboardScope<TOleServices> where TOleServices : IOleServices
{
    public ClipboardScope() => ClipboardCore<TOleServices>.Clear(retryTimes: 1, retryDelay: 0).Should().Be(HRESULT.S_OK);

#pragma warning disable CA1822 // Mark members as static - must be an instance for the dispose pattern matching to work
    public readonly void Dispose() => ClipboardCore<TOleServices>.Clear(retryTimes: 1, retryDelay: 0).Should().Be(HRESULT.S_OK);
#pragma warning restore CA1822
}
