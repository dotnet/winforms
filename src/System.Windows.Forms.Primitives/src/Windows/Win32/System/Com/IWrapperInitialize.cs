// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.System.Com;

/// <summary>
///  Implement to get called back after <see cref="Interop.WinFormsComWrappers"/> has generated the <see cref="IUnknown"/>
///  for a managed object.
/// </summary>
internal unsafe interface IWrapperInitialize
{
    void OnInitialized(IUnknown* unknown);
}
