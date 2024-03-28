// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.System.Com.StructuredStorage;

/// <summary>
///  Untyped representation of CA* typed arrays in Windows. <see cref="CAUB"/>, etc.
/// </summary>
internal unsafe struct CA
{
    public uint cElems;
    public void* pElems;
}
