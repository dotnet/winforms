// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Windows.Forms;

namespace DesignSurfaceExt;

public class PropertyGridExt : PropertyGrid
{
    public IDesignerHost DesignerHost { get; set; }

    protected override void OnSelectedObjectsChanged(EventArgs e) => base.OnSelectedObjectsChanged(e);

    protected override object GetService(Type service) => DesignerHost?.GetService(service) ?? base.GetService(service);
}
