// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Reflection;

namespace DesignSurfaceExt;

public class TabOrderHooker
{
    private const string Name = "TabOrderHooker";

    private object _tabOrder;

    // - Enables/Disables visual TabOrder on the view.
    // - internal override
    public void HookTabOrder(IDesignerHost host)
    {
        // - the TabOrder must be called AFTER the DesignSurface has been loaded
        // - therefore we do a little check
        if (host.RootComponent is null)
            throw new InvalidOperationException($"{Name}::HookTabOrder() - Exception: the TabOrder must be invoked after the DesignSurface has been loaded! ");

        try
        {
            Assembly designAssembly = Assembly.Load("System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            Type tabOrderType = designAssembly.GetType("System.Windows.Forms.Design.TabOrder");
            if (_tabOrder is null)
            {
                // - call the ctor passing the IDesignerHost target object
                _tabOrder = Activator.CreateInstance(tabOrderType, [host]);
            }
            else
            {
                DisposeTabOrder();
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"{Name}::HookTabOrder() - Exception: (see Inner Exception)", ex);
        }
    }

    // - Disposes the tab order
    public void DisposeTabOrder()
    {
        if (_tabOrder is null)
        {
            return;
        }

        try
        {
            Assembly designAssembly = Assembly.Load("System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
            Type tabOrderType = designAssembly.GetType("System.Windows.Forms.Design.TabOrder");
            tabOrderType.InvokeMember("Dispose", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod, null, _tabOrder, [true]);
            _tabOrder = null;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"{Name}::DisposeTabOrder() - Exception: (see Inner Exception)", ex);
        }
    }
}
