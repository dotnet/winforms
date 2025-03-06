// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

/// <summary>
///  Used to filter to fully COM visible data objects. Notably the platform specific DataObject class.
/// </summary>
internal interface IComVisibleDataObject : IDataObject.Interface, IManagedWrapper<IDataObject>, IDataObjectInternal
{
}
