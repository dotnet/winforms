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

    private IComponentChangeService ComponentChangeService
    {
        get
        {
            _componentChangeService ??= (IComponentChangeService)_host.GetService(typeof(IComponentChangeService));
            return _componentChangeService;
        }
    }

    private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
    {
        MethodInfo methodInfo = typeof(PropertyGrid).GetMethod(nameof(OnComponentChanged), BindingFlags.NonPublic | BindingFlags.Instance);
        methodInfo.Invoke(this, [sender, e]);
    }

    protected override void Dispose(bool disposing)
    {
        if (_componentChangeService is not null)
        {
            _componentChangeService.ComponentChanged -= OnComponentChanged;
        }

        base.Dispose(disposing);
    }

    public IDesignerHost DesignerHost
    {
        get
        {
            return _host;
        }
        set
        {
            if (_host == value)
            {
                return;
            }

            _host = value;

            if (ComponentChangeService is null)
            {
                return;
            }

            if (value is not null)
            {
                ComponentChangeService.ComponentChanged += OnComponentChanged;
            }
            else
            {
                ComponentChangeService.ComponentChanged -= OnComponentChanged;
            }
        }
    }

    protected override void OnSelectedObjectsChanged(EventArgs e) => base.OnSelectedObjectsChanged(e);

    protected override object GetService(Type service) => DesignerHost?.GetService(service) ?? base.GetService(service);
}
