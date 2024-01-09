// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

/// <summary>
///  Interface to provide a contract to provide access to the underlying type.
/// </summary>
/// <typeparam name="T">The type of object being wrapped.</typeparam>
internal interface IWrapper<T>
{
    T Unwrap();
}
