// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Reflection;
using System.Windows.Forms;

namespace DesignSurfaceExt;

public class PropertyGridExt : PropertyGrid
{
    private IDesignerHost _host;
    private IComponentChangeService _componentChangeService;
    public IDesignerHost DesignerHost
    {
        get
        {
            return _host;
        }
        set
        {
            if (value is not null)
            {
                _componentChangeService ??= (IComponentChangeService)value.GetService(typeof(IComponentChangeService));
                if (_componentChangeService is not null)
                {
                    _componentChangeService.ComponentChanged += (sender, e) =>
                    {
                        MethodInfo methodInfo = typeof(PropertyGrid).GetMethod("OnComponentChanged", BindingFlags.NonPublic | BindingFlags.Instance);

                        methodInfo.Invoke(this, [sender, e]);
                    };
                }
            }

            _host = value;
        }
    }

    protected override void OnSelectedObjectsChanged(EventArgs e) => base.OnSelectedObjectsChanged(e);

    protected override object GetService(Type service) => DesignerHost?.GetService(service) ?? base.GetService(service);
}
