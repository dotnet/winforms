// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace System.ComponentModel.Design;

[Guid("ABE5C1F0-C96E-40c4-A22D-4A5CEC899BDC")]
public abstract class DataSourceProviderService
{
    public abstract bool SupportsAddNewDataSource { get; }

    public abstract bool SupportsConfigureDataSource { get; }

    public abstract DataSourceGroupCollection GetDataSources();

    public abstract DataSourceGroup InvokeAddNewDataSource(IWin32Window parentWindow, FormStartPosition startPosition);

    public abstract bool InvokeConfigureDataSource(IWin32Window parentWindow, FormStartPosition startPosition, DataSourceDescriptor dataSourceDescriptor);

    public abstract object AddDataSourceInstance(IDesignerHost host, DataSourceDescriptor dataSourceDescriptor);

    public abstract void NotifyDataSourceComponentAdded(object dsc);
}
