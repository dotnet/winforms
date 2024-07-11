// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

/// <summary>
///  This is a nested container.static Anything added to the nested container
///  will be hostable in a designer.
/// </summary>
internal sealed class SiteNestedContainer : NestedContainer
{
    private readonly DesignerHost _host;
    private ServiceContainer? _services;
    private readonly string? _containerName;
    private bool _safeToCallOwner;

    internal SiteNestedContainer(IComponent owner, string? containerName, DesignerHost host) : base(owner)
    {
        _containerName = containerName;
        _host = host;
        _safeToCallOwner = true;
    }

    /// <summary>
    ///  Override to support named containers.
    /// </summary>
    protected override string? OwnerName
    {
        get
        {
            string? ownerName = base.OwnerName;
            if (string.IsNullOrEmpty(_containerName))
            {
                return ownerName;
            }

            return $"{ownerName}.{_containerName}";
        }
    }

    /// <summary>
    ///  Called to add a component to its container.
    /// </summary>
    public override void Add(IComponent? component, string? name)
    {
        if (component is null || !_host.AddToContainerPreProcess(component, name, this))
        {
            return;
        }

        // Site creation fabricates a name for this component.
        base.Add(component, name);
        try
        {
            _host.AddToContainerPostProcess(component, this);
        }
        catch (Exception t)
        {
            if (t != CheckoutException.Canceled)
            {
                Remove(component);
            }

            throw;
        }
    }

    /// <summary>
    ///  Creates a site for the component within the container.
    /// </summary>
    protected override ISite CreateSite(IComponent component, string? name)
    {
        ArgumentNullException.ThrowIfNull(component);

        return new NestedSite(component, _host, name, this);
    }

    /// <summary>
    ///  Called to remove a component from its container.
    /// </summary>
    public override void Remove(IComponent? component)
    {
        if (!_host.RemoveFromContainerPreProcess(component, this))
        {
            return;
        }

        RemoveWithoutUnsiting(component);
        _host.RemoveFromContainerPostProcess(component);
    }

    protected override object? GetService(Type serviceType)
    {
        object? service = base.GetService(serviceType);
        if (service is not null)
        {
            return service;
        }

        if (serviceType == typeof(IServiceContainer))
        {
            return _services ??= new ServiceContainer(_host);
        }

        if (_services is not null)
        {
            return _services.GetService(serviceType);
        }

        if (Owner.Site is null || !_safeToCallOwner)
        {
            return null;
        }

        try
        {
            _safeToCallOwner = false;
            return Owner.Site.GetService(serviceType);
        }
        finally
        {
            _safeToCallOwner = true;
        }
    }

    internal object? GetServiceInternal(Type serviceType) => GetService(serviceType);

    private sealed class NestedSite : DesignerHost.Site, INestedSite
    {
        private readonly SiteNestedContainer _container;
        private readonly string? _name;

        internal NestedSite(IComponent component, DesignerHost host, string? name, SiteNestedContainer container) : base(component, host, name, container)
        {
            _container = container;
            _name = name;
        }

        public string? FullName
        {
            get
            {
                if (_name is null)
                {
                    return null;
                }

                string? ownerName = _container.OwnerName;
                string? childName = ((ISite)this).Name;
                if (ownerName is null)
                {
                    return childName;
                }

                return $"{ownerName}.{childName}";
            }
        }
    }
}
