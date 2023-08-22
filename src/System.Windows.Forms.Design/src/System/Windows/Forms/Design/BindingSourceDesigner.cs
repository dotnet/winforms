// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class BindingSourceDesigner : ComponentDesigner
{
    private bool _bindingUpdatedByUser;

    public bool BindingUpdatedByUser
    {
        set
        {
            _bindingUpdatedByUser = value;
        }
    }

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        IComponentChangeService componentChangeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
        if (componentChangeSvc is not null)
        {
            componentChangeSvc.ComponentChanged += OnComponentChanged;
            componentChangeSvc.ComponentRemoving += OnComponentRemoving;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            IComponentChangeService componentChangeSvc = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            if (componentChangeSvc is not null)
            {
                componentChangeSvc.ComponentChanged -= OnComponentChanged;
                componentChangeSvc.ComponentRemoving -= OnComponentRemoving;
            }
        }

        base.Dispose(disposing);
    }

    private void OnComponentChanged(object? sender, ComponentChangedEventArgs e)
    {
        if (_bindingUpdatedByUser && e.Component == Component &&
            e.Member is not null && (e.Member.Name == "DataSource" || e.Member.Name == "DataMember"))
        {
            _bindingUpdatedByUser = false;

            DataSourceProviderService dspSvc = (DataSourceProviderService)GetService(typeof(DataSourceProviderService));
            dspSvc?.NotifyDataSourceComponentAdded(Component);
        }
    }

    private void OnComponentRemoving(object? sender, ComponentEventArgs e)
    {
        BindingSource? bingSource = Component as BindingSource;
        if (bingSource is not null && bingSource.DataSource == e.Component)
        {
            IComponentChangeService changeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
            string previousDataMember = bingSource.DataMember;

            PropertyDescriptorCollection descriptorCollection = TypeDescriptor.GetProperties(bingSource);
            PropertyDescriptor? descriptor = descriptorCollection?["DataMember"];

            if (changeService is not null)
            {
                if (descriptor is not null)
                {
                    changeService.OnComponentChanging(bingSource, descriptor);
                }
            }

            bingSource.DataSource = null;

            if (changeService is not null)
            {
                if (descriptor is not null)
                {
                    changeService.OnComponentChanged(bingSource, descriptor, previousDataMember, "");
                }
            }
        }
    }
}
