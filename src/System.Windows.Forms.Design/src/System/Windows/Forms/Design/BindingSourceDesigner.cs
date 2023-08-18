// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class BindingSourceDesigner : ComponentDesigner
{
    private bool bindingUpdatedByUser;

    public bool BindingUpdatedByUser
    {
        set
        {
            bindingUpdatedByUser = value;
        }
    }

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        IComponentChangeService componentChangeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
        if (componentChangeSvc is not null)
        {
            componentChangeSvc.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
            componentChangeSvc.ComponentRemoving += new ComponentEventHandler(OnComponentRemoving);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            IComponentChangeService componentChangeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            if (componentChangeSvc is not null)
            {
                componentChangeSvc.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
                componentChangeSvc.ComponentRemoving -= new ComponentEventHandler(OnComponentRemoving);
            }
        }

        base.Dispose(disposing);
    }

    private void OnComponentChanged(object? sender, ComponentChangedEventArgs e)
    {
        if (bindingUpdatedByUser &&
            e.Component == this.Component &&
            e.Member is not null && (e.Member.Name == "DataSource" || e.Member.Name == "DataMember"))
        {
            bindingUpdatedByUser = false;

            DataSourceProviderService dspSvc = (DataSourceProviderService)GetService(typeof(DataSourceProviderService));
            dspSvc?.NotifyDataSourceComponentAdded(Component);
        }
    }

    private void OnComponentRemoving(object? sender, ComponentEventArgs e)
    {
        BindingSource? b = Component as BindingSource;
        if (b is not null && b.DataSource == e.Component)
        {
            IComponentChangeService ccs = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            string previousDataMember = b.DataMember;

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(b);
            PropertyDescriptor? dmPD = props?["DataMember"];

            if (ccs is not null)
            {
                if (dmPD is not null)
                {
                    ccs.OnComponentChanging(b, dmPD);
                }
            }

            b.DataSource = null;

            if (ccs is not null)
            {
                if (dmPD is not null)
                {
                    ccs.OnComponentChanged(b, dmPD, previousDataMember, "");
                }
            }
        }
    }
}
