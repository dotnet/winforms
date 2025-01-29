// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Nrbf;
using System.Private.Windows.Ole;

namespace System.Private.Windows;

/// <summary>
///  Unifying interface for runtime specific services and types.
/// </summary>
/// <typeparam name="TDataFormat">The platform specific DataFormat struct type.</typeparam>
internal interface IRuntime<TDataFormat>
    : INrbfSerializer, IOleServices
    where TDataFormat : IDataFormat<TDataFormat>
{
}
