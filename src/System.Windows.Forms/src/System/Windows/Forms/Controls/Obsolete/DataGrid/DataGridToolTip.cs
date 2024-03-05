// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

#nullable disable
internal class DataGridToolTip : MarshalByRefObject
{
    // CONSTRUCTOR
    public DataGridToolTip(DataGrid dataGrid)
    {
        throw new PlatformNotSupportedException();
    }

    // will ensure that the toolTip window was created
    public static void CreateToolTipHandle()
    {
        throw new PlatformNotSupportedException();
    }

    // this function will add a toolTip to the
    // windows system
    public static void AddToolTip(string toolTipString, IntPtr toolTipId, Rectangle iconBounds)
    {
        throw new PlatformNotSupportedException();
    }

    public static void RemoveToolTip(IntPtr toolTipId)
    {
        throw new PlatformNotSupportedException();
    }

    // will destroy the tipWindow
    public static void Destroy()
    {
        throw new PlatformNotSupportedException();
    }
}
