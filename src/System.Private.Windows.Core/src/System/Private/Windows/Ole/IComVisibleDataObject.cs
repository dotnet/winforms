// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

#if NET
internal partial interface IComVisibleDataObject : IManagedWrapper<IDataObject>
{
}
#endif

/// <summary>
///  Used to filter to fully COM visible data objects. Notably the platform specific DataObject class.
/// </summary>
internal partial interface IComVisibleDataObject : IDataObject.Interface, IDataObjectInternal
{
}
