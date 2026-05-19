// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

internal static class DataObjectFactory<TDataObject, TIDataObject>
    where TDataObject : class, IDataObjectInternal<TDataObject, TIDataObject>, TIDataObject, new()
    where TIDataObject : class
{
    public static IDataObjectInternal<TDataObject, TIDataObject> Instance { get; } = new TDataObject();
}
