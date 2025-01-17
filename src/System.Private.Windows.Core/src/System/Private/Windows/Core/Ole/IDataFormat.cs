// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.Ole;

internal interface IDataFormat
{
    string Name { get; }
    int Id { get; }
}
