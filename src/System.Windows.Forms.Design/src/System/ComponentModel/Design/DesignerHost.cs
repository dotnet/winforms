// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel.Design;

/// <summary>
///  This is the main hosting object. <see cref="DesignerHost"/> implements services and interfaces specific to the
///  design time <see cref="IContainer"/> object. The services this class implements are generally non-removable (they
///  work as a unit so removing them would break things).
/// </summary>
internal sealed partial class DesignerHost : Container, IDesignerLoaderHost2, IDesignerHostTransactionState, IComponentChangeService, IReflect
{
    // State flags for the state of the designer host
    // Designer is currently loading from the loader host.
    private static readonly int s_stateLoading = BitVector32.CreateMask();
    // Designer is currently unloading.
    private static readonly int s_stateUnloading = BitVector32.CreateMask(s_stateLoading);
    // A transaction is in the process of being Canceled or Committed.
    private static readonly int s_stateIsClosingTransaction = BitVector32.CreateMask(s_stateUnloading);

    private static readonly Type[] s_defaultServices = [typeof(IDesignerHost), typeof(IContainer), typeof(IComponentChangeService), typeof(IDesignerLoaderHost2)];

    // IDesignerHost events
    private static readonly object s_eventActivated = new(); // Designer has been activated
    private static readonly object s_eventDeactivated = new(); // Designer has been deactivated
    private static readonly object s_eventLoadComplete = new(); // Loading has been completed
    private static readonly object s_eventTransactionClosed = new(); // The last transaction has been closed
    private static readonly object s_eventTransactionClosing = new(); // The last transaction is about to be closed
    private static readonly object s_eventTransactionOpened = new(); // The first transaction has been opened
    private static readonly object s_eventTransactionOpening = new(); // The first transaction is about to be opened

    // IComponentChangeService events
    private static readonly object s_eventComponentAdding = new(); // A component is about to be added to the container
    private static readonly object s_eventComponentAdded = new(); // A component was just added to the container
    private static readonly object s_eventComponentChanging = new(); // A component is about to be changed
    private static readonly object s_eventComponentChanged = new(); // A component has changed
    // A component is about to be removed from the container
    private static readonly object s_eventComponentRemoving = new();
    private static readonly object s_eventComponentRemoved = new(); // A component has been removed from the container
    private static readonly object s_eventComponentRename = new(); // A component has been renamed

    // Member variables
    private BitVector32 _state; // state for this host
    private DesignSurface? _surface; // the owning designer surface.
    private string? _newComponentName; // transient value indicating the name of a component that is being created
    private Stack<DesignerTransaction>? _transactions; // stack of transactions
    private IComponent? _rootComponent; // the root of our design
    private string? _rootComponentClassName; // class name of the root of our design
    private readonly Dictionary<IComponent, IDesigner> _designers;  // designer -> component mapping
    private readonly EventHandlerList _events; // event list
    private DesignerLoader? _loader; // the loader that loads our designers
    private List<string>? _savedSelection; // set of selected components names saved across reloads
    private HostDesigntimeLicenseContext? _licenseCtx;
    private IDesignerEventService? _designerEventService;
    private static readonly object s_selfLock = new();
    private bool _ignoreErrorsDuringReload;
    private TypeDescriptionProviderService? _typeService;
    private bool _typeServiceChecked;

    public DesignerHost(DesignSurface surface)
    {
        _surface = surface;
        _state = default;
        _designers = [];
        _events = new EventHandlerList();

        // Add the relevant services. We try to add these as "fixed" services. A fixed service cannot be removed by
        // the user. The reason for this is that each of these services depends on each other, so you can't really
        // remove and replace just one of them. If we can't get our own service container that supports fixed services,
        // we add these as regular services.

        if (this.TryGetService(out DesignSurfaceServiceContainer? dsc))
        {
            foreach (Type t in s_defaultServices)
            {
                dsc.AddFixedService(t, this);
            }
        }
        else
        {
            if (this.TryGetService(out IServiceContainer? sc))
            {
                foreach (Type t in s_defaultServices)
                {
                    sc.AddService(t, this);
                }
            }
            else
            {
                Debug.Fail("DesignerHost: Ctor needs a service provider that provides IServiceContainer");
            }
        }
    }

    [MemberNotNull(nameof(_licenseCtx))]
    internal HostDesigntimeLicenseContext LicenseContext => _licenseCtx ??= new HostDesigntimeLicenseContext(this);

    // Internal flag which is used to track when we are in the process of committing or canceling a transaction.
    internal bool IsClosingTransaction
    {
        get { return _state[s_stateIsClosingTransaction]; }
        set { _state[s_stateIsClosingTransaction] = value; }
    }

    bool IDesignerHostTransactionState.IsClosingTransaction => IsClosingTransaction;

    /// <summary>
    ///  Override of Container.Add
    /// </summary>
    public override void Add(IComponent? component, string? name)
    {
        if (!_typeServiceChecked)
        {
            this.TryGetService(out _typeService);
            _typeServiceChecked = true;
        }

        if (component is null)
        {
            return;
        }

        // TypeDescriptionProviderService is attached at design time only
        if (_typeService is not null)
        {
            // Check for the attribute that VsTargetFrameworkProvider injects on reflection types to see
            // if VsTargetFrameworkProvider is already attached.
            Type type = TypeDescriptor.GetProvider(component).GetReflectionType(typeof(object));
            if (!type.IsDefined(typeof(ProjectTargetFrameworkAttribute), false))
            {
                TypeDescriptionProvider typeProvider = _typeService.GetProvider(component);
                if (typeProvider is not null)
                {
                    TypeDescriptor.AddProvider(typeProvider, component);
                }
            }
        }

        PerformAdd(component, name);
    }

    private void PerformAdd(IComponent component, string? name)
    {
        if (!AddToContainerPreProcess(component, name, this))
        {
            return;
        }

        // Site creation fabricates a name for this component.
        base.Add(component, name);

        try
        {
            AddToContainerPostProcess(component, this);
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
    ///  We support adding to either our main IDesignerHost container or to a private per-site container for nested
    ///  objects. This code is the stock add code that creates a designer, etc. See Add (above) for an example of how
    ///  to call this correctly. This method is called before the component is actually added. It returns true if the
    ///  component can be added to this container or false if the add should not occur (because the component may
    ///  already be in this container, for example.) It may also throw if adding this component is illegal.
    /// </summary>
    internal bool AddToContainerPreProcess(IComponent component, string? name, IContainer containerToAddTo)
    {
        ArgumentNullException.ThrowIfNull(component);

        // We should never add anything while we're unloading.
        if (_state[s_stateUnloading])
        {
            throw new InvalidOperationException(SR.DesignerHostUnloading)
            {
                HelpLink = SR.DesignerHostUnloading
            };
        }

        // Make sure we're not adding an instance of the root component to itself.
        if (_rootComponent is not null && string.Equals(component.GetType().FullName, _rootComponentClassName, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(string.Format(
                SR.DesignerHostCyclicAdd,
                component.GetType().FullName,
                _rootComponentClassName))
            {
                HelpLink = SR.DesignerHostCyclicAdd
            };
        }

        ISite? existingSite = component.Site;

        // If the component is already in our container, we just rename.
        if (existingSite is not null && existingSite.Container == this)
        {
            if (name is not null)
            {
                existingSite.Name = name;
            }

            return false;
        }

        // Raise an adding event for our container if the container is us.
        ComponentEventArgs ce = new(component);
        (_events[s_eventComponentAdding] as ComponentEventHandler)?.Invoke(containerToAddTo, ce);
        return true;
    }

    /// <summary>
    ///  We support adding to either our main IDesignerHost container or to a private per-site container for nested
    ///  objects. This code is the stock add code that creates a designer, etc. See Add (above) for an example of how
    ///  to call this correctly.
    /// </summary>
    internal void AddToContainerPostProcess(IComponent component, IContainer containerToAddTo)
    {
        // Now that we've added, check to see if this is an extender provider. If it is, add it to our extender provider
        // service so it is available.
        if (component is IExtenderProvider extenderComponent
            && !TypeDescriptor.GetAttributes(extenderComponent).Contains(InheritanceAttribute.InheritedReadOnly)
            && this.TryGetService(out IExtenderProviderService? eps))
        {
            eps.AddExtenderProvider(extenderComponent);
        }

        IDesigner? designer;

        // Is this the first component the loader has created? If so, then it must be the root component (by definition)
        // so we will expect there to be a root designer associated with the component.
        // Otherwise, we search for a normal designer, which can be optionally provided.
        if (_rootComponent is null)
        {
            designer = _surface!.CreateDesigner(component, true) as IRootDesigner;
            if (designer is null)
            {
                throw new InvalidOperationException(string.Format(SR.DesignerHostNoTopLevelDesigner, component.GetType().FullName))
                {
                    HelpLink = SR.DesignerHostNoTopLevelDesigner
                };
            }

            _rootComponent = component;

            // Check and see if anyone has set the class name of the root component. We default to the component name.
            _rootComponentClassName ??= component.Site!.Name;
        }
        else
        {
            designer = _surface!.CreateDesigner(component, false);
        }

        if (designer is not null)
        {
            // The presence of a designer in this table allows the designer to filter the component's properties, which
            // is often needed during designer initialization. So, we stuff it in the table first, initialize, and if
            // it throws we remove it from the table.
            _designers[component] = designer;
            try
            {
                designer.Initialize(component);
                if (designer.Component is null)
                {
                    throw new InvalidOperationException(SR.DesignerHostDesignerNeedsComponent);
                }
            }
            catch
            {
                _designers.Remove(component);
                throw;
            }

            // Designers can also implement IExtenderProvider.
            if (designer is IExtenderProvider extenderProvider)
            {
                if (this.TryGetService(out eps))
                {
                    eps.AddExtenderProvider(extenderProvider);
                }
            }
        }

        // The component has been added. Note that it is tempting to move this above the designer because the designer
        // will never need to know that its own component just got added, but this would be bad because the designer is
        // needed to extract shadowed properties from the component.
        ComponentEventArgs ce = new(component);
        (_events[s_eventComponentAdded] as ComponentEventHandler)?.Invoke(containerToAddTo, ce);
    }

    /// <summary>
    ///  Called by DesignerSurface to begin loading the designer.
    /// </summary>
    internal void BeginLoad(DesignerLoader loader)
    {
        if (_loader is not null && _loader != loader)
        {
            throw new InvalidOperationException(SR.DesignerHostLoaderSpecified)
            {
                HelpLink = SR.DesignerHostLoaderSpecified
            };
        }

        bool reloading = _loader is not null;
        _loader = loader;

        if (!reloading)
        {
            if (loader is IExtenderProvider extenderProvider && this.TryGetService(out IExtenderProviderService? eps))
            {
                eps.AddExtenderProvider(extenderProvider);
            }

            if (this.TryGetService(out IDesignerEventService? des))
            {
                des.ActiveDesignerChanged += OnActiveDesignerChanged;
                _designerEventService = des;
            }
        }

        _state[s_stateLoading] = true;
        _surface!.OnLoading();

        try
        {
            _loader?.BeginLoad(this);
        }
        catch (Exception e)
        {
            if (e is TargetInvocationException && e.InnerException is { } inner)
            {
                e = inner;
            }

            string message = e.Message;

            // We must handle the case of an exception with no message.
            if (string.IsNullOrEmpty(message))
            {
                e = new InvalidOperationException(string.Format(SR.DesignSurfaceFatalError, e), e);
            }

            // Loader blew up. Add this exception to our error list.
            ((IDesignerLoaderHost)this).EndLoad(string.Empty, successful: false, new object[] { e });
        }

        if (_designerEventService is null)
        {
            // If there is no designer event service, make this designer the currently active designer.
            // It will remain active.
            OnActiveDesignerChanged(sender: null, new ActiveDesignerEventArgs(oldDesigner: null, this));
        }
    }

    /// <summary>
    ///  Override of CreateSite. We create a custom site here, called Site, which is an inner class of DesignerHost.
    ///  DesignerSite contains an instance of the designer for the component.
    /// </summary>
    /// <param name="component">The component to create the site for.</param>
    /// <param name="name">The name of the component. If no name is provided this will fabricate a name for you.</param>
    /// <returns>The newly created site.</returns>
    protected override ISite CreateSite(IComponent component, string? name)
    {
        Debug.Assert(component is not null, "Caller should have guarded against a null component");

        // We need to handle the case where a component's ctor adds itself to the container. We don't want to do the
        // work of creating a name, and then immediately renaming. So, DesignerHost's CreateComponent will set
        // _newComponentName to the newly created name before creating the component.
        if (_newComponentName is not null)
        {
            name = _newComponentName;
            _newComponentName = null;
        }

        this.TryGetService(out INameCreationService? nameCreate);

        // Fabricate a name if one wasn't provided. We try to use the name creation service, but if it is not available
        // we will just use an empty string.
        if (name is null)
        {
            if (nameCreate is not null)
            {
                // VirtualTypes and Compact framework types will need to use reflection type in order to get their
                // "real" name (the one available in the compact FX, for example).
                Type reflectType = TypeDescriptor.GetReflectionType(component);
                if (string.Equals(reflectType.FullName, component.GetType().FullName, StringComparison.Ordinal))
                {
                    reflectType = component.GetType();
                }

                name = nameCreate.CreateName(this, reflectType);
            }
            else
            {
                name = string.Empty;
            }
        }
        else
        {
            nameCreate?.ValidateName(name);
        }

        return new Site(component, this, name, this);
    }

    /// <summary>
    ///  Override of dispose to clean up our state.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            throw new InvalidOperationException(SR.DesignSurfaceContainerDispose);
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  We move all "dispose" functionality to the DisposeHost method. The reason for this is that Dispose is inherited
    ///  from our container implementation, and we do not want someone disposing the container. That would leave the
    ///  design surface still alive, but it would kill the host. Instead, DesignSurface always calls DisposeHost, which
    ///  calls the base version of Dispose to clean out the container.
    /// </summary>
    internal void DisposeHost()
    {
        try
        {
            if (_loader is not null)
            {
                _loader.Dispose();
                Unload();
            }

            if (_surface is not null)
            {
                if (_designerEventService is not null)
                {
                    _designerEventService.ActiveDesignerChanged -= OnActiveDesignerChanged;
                }

                if (this.TryGetService(out DesignSurfaceServiceContainer? dsc))
                {
                    foreach (Type t in s_defaultServices)
                    {
                        dsc.RemoveFixedService(t);
                    }
                }
                else
                {
                    if (this.TryGetService(out IServiceContainer? sc))
                    {
                        foreach (Type t in s_defaultServices)
                        {
                            sc.RemoveService(t);
                        }
                    }
                    else
                    {
                        Debug.Fail("DesignerHost: Ctor needs a service provider that provides IServiceContainer");
                    }
                }
            }
        }
        finally
        {
            _loader = null;
            _surface = null;
            _events.Dispose();
        }

        base.Dispose(true);
    }

    /// <summary>
    ///  Invokes flush on the designer loader.
    /// </summary>
    internal void Flush() => _loader?.Flush();

    /// <summary>
    ///  Override of Container's GetService method. This just delegates to the parent service provider.
    /// </summary>
    /// <param name="service">The type of service to retrieve.</param>
    /// <returns>An instance of the service.</returns>
    protected override object? GetService(Type service)
    {
        object? serviceInstance = null;
        ArgumentNullException.ThrowIfNull(service);

        if (service == typeof(IMultitargetHelperService))
        {
            if (_loader is IServiceProvider provider)
            {
                serviceInstance = provider.GetService(service);
            }
        }
        else
        {
            serviceInstance = base.GetService(service) ?? _surface?.GetService(service);
        }

        return serviceInstance;
    }

    /// <summary>
    ///  Called in response to a designer becoming active or inactive.
    /// </summary>
    private void OnActiveDesignerChanged(object? sender, ActiveDesignerEventArgs? e)
    {
        // NOTE: sender can be null (we call this directly in BeginLoad)
        if (e is null)
        {
            return;
        }

        object eventobj;

        if (e.OldDesigner == this)
        {
            eventobj = s_eventDeactivated;
        }
        else if (e.NewDesigner == this)
        {
            eventobj = s_eventActivated;
        }
        else
        {
            // Not our document, so we don't fire.
            return;
        }

        // If we are deactivating, flush any code changes. We always route through the design surface so it can
        // correctly raise its Flushed event.
        if (e.OldDesigner == this && _surface is not null)
        {
            _surface.Flush();
        }

        // Fire the appropriate event.
        (_events[eventobj] as EventHandler)?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///  Method is called by the site when a component is renamed.
    /// </summary>
    private void OnComponentRename(IComponent component, string? oldName, string newName)
    {
        // If the root component is being renamed we need to update RootComponentClassName.
        if (component == _rootComponent)
        {
            string className = _rootComponentClassName!;

            // If the old name occurs at the end of the class name and is preceeded by a period.
            if (oldName is not null && className.EndsWith(oldName, StringComparison.Ordinal)
                && className.Length > oldName.Length && className[className.Length - oldName.Length - 1] == '.')
            {
                // We assume the preceeding chars are the namespace and preserve it.
                _rootComponentClassName = string.Concat(className.AsSpan(0, className.Length - oldName.Length), newName);
            }
            else
            {
                _rootComponentClassName = newName;
            }
        }

        (_events[s_eventComponentRename] as ComponentRenameEventHandler)?.Invoke(this, new ComponentRenameEventArgs(component, oldName, newName));
    }

    /// <summary>
    ///  Method is called when the designer has finished loading.
    /// </summary>
    private void OnLoadComplete(EventArgs e) => (_events[s_eventLoadComplete] as EventHandler)?.Invoke(this, e);

    /// <summary>
    ///  Method is called when the last transaction has closed.
    /// </summary>
    private void OnTransactionClosed(DesignerTransactionCloseEventArgs e) =>
        (_events[s_eventTransactionClosed] as DesignerTransactionCloseEventHandler)?.Invoke(this, e);

    /// <summary>
    ///  Method is called when the last transaction is closing.
    /// </summary>
    private void OnTransactionClosing(DesignerTransactionCloseEventArgs e) =>
        (_events[s_eventTransactionClosing] as DesignerTransactionCloseEventHandler)?.Invoke(this, e);

    /// <summary>
    ///  Method is called when the first transaction has opened.
    /// </summary>
    private void OnTransactionOpened(EventArgs e) =>
        (_events[s_eventTransactionOpened] as EventHandler)?.Invoke(this, e);

    /// <summary>
    ///  Method is called when the first transaction is opening.
    /// </summary>
    private void OnTransactionOpening(EventArgs e) =>
        (_events[s_eventTransactionOpening] as EventHandler)?.Invoke(this, e);

    /// <summary>
    ///  Called to remove a component from its container.
    /// </summary>
    public override void Remove(IComponent? component)
    {
        if (RemoveFromContainerPreProcess(component, this))
        {
            Site? site = component.Site as Site;
            RemoveWithoutUnsiting(component);
            RemoveFromContainerPostProcess(component);
            if (site is not null)
            {
                site.Disposed = true;
            }
        }
    }

    internal bool RemoveFromContainerPreProcess([NotNullWhen(true)] IComponent? component, IContainer container)
    {
        if (component is null)
        {
            return false;
        }

        ISite? site = component.Site;
        if (site is null || site.Container != container)
        {
            return false;
        }

        ComponentEventArgs ce = new(component);

        ComponentEventHandler? eh = _events[s_eventComponentRemoving] as ComponentEventHandler;
        eh?.Invoke(this, ce);

        // If the component is an extender provider, remove it from the extender provider service, should one exist.
        if (component is IExtenderProvider extenderComponent && this.TryGetService(out IExtenderProviderService? eps))
        {
            eps.RemoveExtenderProvider(extenderComponent);
        }

        // Same for the component's designer
        _designers.TryGetValue(component, out IDesigner? designer);

        if (designer is IExtenderProvider extenderDesigner && this.TryGetService(out eps))
        {
            eps.RemoveExtenderProvider(extenderDesigner);
        }

        if (designer is not null)
        {
            designer.Dispose();
            _designers.Remove(component);
        }

        if (component == _rootComponent)
        {
            _rootComponent = null;
            _rootComponentClassName = null;
        }

        return true;
    }

    internal void RemoveFromContainerPostProcess(IComponent component)
    {
        // At one point during Whidbey, the component used to be un-sited earlier in this process and
        // it would be temporarily re-sited here before raising OnComponentRemoved. The problem with
        // re-siting it is that some 3rd party controls take action when a component is sited (such as
        // displaying a dialog a control is dropped on the form) and re-siting here caused them to think
        // they were being initialized for the first time. To preserve compat, we shouldn't re-site the
        // component during Remove.
        try
        {
            ComponentEventHandler? eh = _events[s_eventComponentRemoved] as ComponentEventHandler;
            ComponentEventArgs ce = new(component);
            eh?.Invoke(this, ce);
        }
        finally
        {
            component.Site = null;
        }
    }

    /// <summary>
    ///  Called to unload the design surface.
    /// </summary>
    private void Unload()
    {
        _surface?.OnUnloading();

        if (this.TryGetService(out IHelpService? helpService)
            && _rootComponent is not null
            && _designers.TryGetValue(_rootComponent, out IDesigner? designer))
        {
            helpService.RemoveContextAttribute("Keyword", $"Designer_{designer.GetType().FullName}");
        }

        ISelectionService? selectionService = (ISelectionService?)GetService(typeof(ISelectionService));
        selectionService?.SetSelectedComponents(null, SelectionTypes.Replace);

        // Now remove all the designers and their components. We save the root for last. Note that we eat any
        // exceptions that components or their designers generate. A bad component or designer should not prevent
        // an unload from happening. We do all of this in a transaction to help reduce the number of events we generate.
        _state[s_stateUnloading] = true;
        DesignerTransaction t = ((IDesignerHost)this).CreateTransaction();
        List<Exception> exceptions = [];

        try
        {
            IComponent[] components = new IComponent[Components.Count];
            Components.CopyTo(components, 0);

            foreach (IComponent comp in components)
            {
                if (!ReferenceEquals(comp, _rootComponent))
                {
                    if (_designers.Remove(comp, out IDesigner? compDesigner))
                    {
                        try
                        {
                            compDesigner.Dispose();
                        }
                        catch (Exception e)
                        {
                            exceptions.Add(e);
                        }
                    }

                    try
                    {
                        comp.Dispose();
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                }
            }

            if (_rootComponent is not null)
            {
                if (_designers.Remove(_rootComponent, out IDesigner? rootComponentDesigner))
                {
                    try
                    {
                        rootComponentDesigner.Dispose();
                    }
                    catch (Exception e)
                    {
                        exceptions.Add(e);
                    }
                }

                try
                {
                    _rootComponent.Dispose();
                }
                catch (Exception e)
                {
                    exceptions.Add(e);
                }
            }

            _designers.Clear();
            while (Components.Count > 0)
            {
                Remove(Components[0]);
            }
        }
        finally
        {
            t.Commit();
            _state[s_stateUnloading] = false;
        }

        // There should be no open transactions. Commit all of the ones that are open.
        if (_transactions is not null && _transactions.Count > 0)
        {
            Debug.Fail("There are open transactions at unload");

            // Transactions will get popped in the OnCommit for DesignerHostTransaction.
            while (_transactions.TryPeek(out DesignerTransaction transaction))
            {
                transaction.Commit();
            }
        }

        _surface?.OnUnloaded();

        if (exceptions.Count > 0)
        {
            throw new ExceptionCollection(exceptions);
        }
    }

    event ComponentEventHandler IComponentChangeService.ComponentAdded
    {
        add => _events.AddHandler(s_eventComponentAdded, value);
        remove => _events.RemoveHandler(s_eventComponentAdded, value);
    }

    event ComponentEventHandler IComponentChangeService.ComponentAdding
    {
        add => _events.AddHandler(s_eventComponentAdding, value);
        remove => _events.RemoveHandler(s_eventComponentAdding, value);
    }

    event ComponentChangedEventHandler IComponentChangeService.ComponentChanged
    {
        add => _events.AddHandler(s_eventComponentChanged, value);
        remove => _events.RemoveHandler(s_eventComponentChanged, value);
    }

    event ComponentChangingEventHandler IComponentChangeService.ComponentChanging
    {
        add => _events.AddHandler(s_eventComponentChanging, value);
        remove => _events.RemoveHandler(s_eventComponentChanging, value);
    }

    event ComponentEventHandler IComponentChangeService.ComponentRemoved
    {
        add => _events.AddHandler(s_eventComponentRemoved, value);
        remove => _events.RemoveHandler(s_eventComponentRemoved, value);
    }

    event ComponentEventHandler IComponentChangeService.ComponentRemoving
    {
        add => _events.AddHandler(s_eventComponentRemoving, value);
        remove => _events.RemoveHandler(s_eventComponentRemoving, value);
    }

    event ComponentRenameEventHandler IComponentChangeService.ComponentRename
    {
        add => _events.AddHandler(s_eventComponentRename, value);
        remove => _events.RemoveHandler(s_eventComponentRename, value);
    }

    void IComponentChangeService.OnComponentChanged(object component, MemberDescriptor? member, object? oldValue, object? newValue)
    {
        if (!((IDesignerHost)this).Loading)
        {
            (_events[s_eventComponentChanged] as ComponentChangedEventHandler)?.Invoke(
                this,
                new ComponentChangedEventArgs(component, member, oldValue, newValue));
        }
    }

    void IComponentChangeService.OnComponentChanging(object component, MemberDescriptor? member)
    {
        if (!((IDesignerHost)this).Loading)
        {
            (_events[s_eventComponentChanging] as ComponentChangingEventHandler)?.Invoke(this, new ComponentChangingEventArgs(component, member));
        }
    }

    bool IDesignerHost.Loading =>
        _state[s_stateLoading] || _state[s_stateUnloading] || (_loader is not null && _loader.Loading);

    bool IDesignerHost.InTransaction => (_transactions is not null && _transactions.Count > 0) || IsClosingTransaction;

    IContainer IDesignerHost.Container => this;

    IComponent IDesignerHost.RootComponent => _rootComponent!;

    string IDesignerHost.RootComponentClassName => _rootComponentClassName!;

    string IDesignerHost.TransactionDescription =>
        _transactions is not null && _transactions.TryPeek(out DesignerTransaction? transaction)
            ? transaction.Description
            : string.Empty;

    event EventHandler IDesignerHost.Activated
    {
        add => _events.AddHandler(s_eventActivated, value);
        remove => _events.RemoveHandler(s_eventActivated, value);
    }

    event EventHandler IDesignerHost.Deactivated
    {
        add => _events.AddHandler(s_eventDeactivated, value);
        remove => _events.RemoveHandler(s_eventDeactivated, value);
    }

    event EventHandler IDesignerHost.LoadComplete
    {
        add => _events.AddHandler(s_eventLoadComplete, value);
        remove => _events.RemoveHandler(s_eventLoadComplete, value);
    }

    event DesignerTransactionCloseEventHandler IDesignerHost.TransactionClosed
    {
        add => _events.AddHandler(s_eventTransactionClosed, value);
        remove => _events.RemoveHandler(s_eventTransactionClosed, value);
    }

    event DesignerTransactionCloseEventHandler IDesignerHost.TransactionClosing
    {
        add => _events.AddHandler(s_eventTransactionClosing, value);
        remove => _events.RemoveHandler(s_eventTransactionClosing, value);
    }

    event EventHandler IDesignerHost.TransactionOpened
    {
        add => _events.AddHandler(s_eventTransactionOpened, value);
        remove => _events.RemoveHandler(s_eventTransactionOpened, value);
    }

    event EventHandler IDesignerHost.TransactionOpening
    {
        add => _events.AddHandler(s_eventTransactionOpening, value);
        remove => _events.RemoveHandler(s_eventTransactionOpening, value);
    }

    void IDesignerHost.Activate() => _surface?.OnViewActivate();

    // The CreateComponent implementation has special handling of null and string.Empty,
    // and we want to preserve this distinction.
    IComponent IDesignerHost.CreateComponent(Type componentType) =>
        ((IDesignerHost)this).CreateComponent(componentType, null!);

    IComponent IDesignerHost.CreateComponent(Type componentType, string name)
    {
        ArgumentNullException.ThrowIfNull(componentType);

        IComponent? component;
        LicenseContext oldContext = LicenseManager.CurrentContext;

        // We don't want if there is a recursively (creating a component create another one) to change the context again.
        // We already have the one we want and that would create a locking problem.
        bool changingContext = false;
        if (oldContext != LicenseContext)
        {
            LicenseManager.CurrentContext = LicenseContext;
            LicenseManager.LockContext(s_selfLock);
            changingContext = true;
        }

        try
        {
            try
            {
                _newComponentName = name;
                if (_surface is null)
                {
                    throw new InvalidOperationException();
                }

                component = _surface.CreateInstance(componentType) as IComponent;
            }
            finally
            {
                _newComponentName = null;
            }

            if (component is null)
            {
                throw new InvalidOperationException(string.Format(SR.DesignerHostFailedComponentCreate, componentType.Name))
                {
                    HelpLink = SR.DesignerHostFailedComponentCreate
                };
            }

            // Add this component to our container
            if (component.Site is null || component.Site.Container != this)
            {
                PerformAdd(component, name);
            }
        }
        finally
        {
            if (changingContext)
            {
                LicenseManager.UnlockContext(s_selfLock);
                LicenseManager.CurrentContext = oldContext;
            }
        }

        return component;
    }

    DesignerTransaction IDesignerHost.CreateTransaction() =>
        ((IDesignerHost)this).CreateTransaction(SR.DesignerHostGenericTransactionName);

    DesignerTransaction IDesignerHost.CreateTransaction(string description) =>
        new DesignerHostTransaction(this, description ?? SR.DesignerHostGenericTransactionName);

    void IDesignerHost.DestroyComponent(IComponent component)
    {
        ArgumentNullException.ThrowIfNull(component);

        string name = component.Site?.Name ?? component.GetType().Name;

        // Make sure the component is not being inherited -- we can't delete these!
        if (TypeDescriptorHelper.TryGetAttribute(component, out InheritanceAttribute? ia) && ia.InheritanceLevel != InheritanceLevel.NotInherited)
        {
            throw new InvalidOperationException(string.Format(SR.DesignerHostCantDestroyInheritedComponent, name))
            {
                HelpLink = SR.DesignerHostCantDestroyInheritedComponent
            };
        }

        if (((IDesignerHost)this).InTransaction)
        {
            Remove(component);
            component.Dispose();
        }
        else
        {
            DesignerTransaction t;
            using (t = ((IDesignerHost)this).CreateTransaction(string.Format(SR.DesignerHostDestroyComponentTransaction, name)))
            {
                // We need to signal changing and then perform the remove. Remove must be done by us and not by Dispose
                // because (a) people need a chance to cancel through a Removing event, and (b) Dispose removes from the
                // container last and anything that would sync Removed would end up with a dead component.
                Remove(component);
                component.Dispose();
                t.Commit();
            }
        }
    }

    IDesigner? IDesignerHost.GetDesigner(IComponent component)
    {
        ArgumentNullException.ThrowIfNull(component);
        _designers.TryGetValue(component, out IDesigner? designer);
        return designer;
    }

    Type? IDesignerHost.GetType(string typeName)
    {
        ArgumentNullException.ThrowIfNull(typeName);

        if (this.TryGetService(out ITypeResolutionService? ts))
        {
            return ts.GetType(typeName);
        }

        return Type.GetType(typeName);
    }

    void IDesignerLoaderHost.EndLoad(string? rootClassName, bool successful, ICollection? errorCollection)
    {
        bool wasLoading = _state[s_stateLoading];
        _state[s_stateLoading] = false;

        if (rootClassName is not null)
        {
            _rootComponentClassName = rootClassName;
        }
        else if (_rootComponent?.Site is not null)
        {
            _rootComponentClassName = _rootComponent.Site.Name;
        }

        // If the loader indicated success, but it never created a component, that is an error.
        if (successful && _rootComponent is null)
        {
            errorCollection = new List<object>()
            {
                new InvalidOperationException(SR.DesignerHostNoBaseClass)
                {
                    HelpLink = SR.DesignerHostNoBaseClass
                }
            };

            successful = false;
        }

        // If we failed, unload the doc so that the OnLoaded event can't get to anything that actually did work.
        if (!successful)
        {
            Unload();
        }

        if (wasLoading)
        {
            _surface?.OnLoaded(successful, errorCollection);
        }

        // We may be invoked to do an EndLoad when we are already loaded. This can happen if the user called
        // AddLoadDependency, essentially putting us in a loading state while we are already loaded. This is OK,
        // and is used as a hint that the user is going to monkey with settings but doesn't want the code engine
        // to report it.

        if (!successful || !wasLoading)
        {
            return;
        }

        IRootDesigner rootDesigner = (((IDesignerHost)this).GetDesigner(_rootComponent!) as IRootDesigner)
            ?? throw new InvalidOperationException("Root designer is null.");

        // Offer up our base help attribute.
        if (this.TryGetService(out IHelpService? helpService))
        {
            helpService.AddContextAttribute("Keyword", $"Designer_{rootDesigner.GetType().FullName}", HelpKeywordType.F1Keyword);
        }

        // And let everyone know that we're loaded.
        try
        {
            OnLoadComplete(EventArgs.Empty);
        }
        catch (Exception ex)
        {
            Debug.Fail($"Exception thrown on LoadComplete event handler. You should not throw here : {ex}");

            // The load complete failed. Put us back in the loading state and unload.
            _state[s_stateLoading] = true;
            Unload();

            List<object> errorList = errorCollection is null ? [] : [..errorCollection.Cast<object>()];
            errorList.Insert(0, ex);

            errorCollection = errorList;

            successful = false;

            _surface?.OnLoaded(successful, errorCollection);

            // We re-throw. If this was a synchronous load this will error back to BeginLoad (and, as a side effect,
            // may call us again). For asynchronous loads we need to throw so the caller knows what happened.
            throw;
        }

        // If we saved a selection as a result of a reload, try to replace it.
        if (successful && _savedSelection is not null && this.TryGetService(out ISelectionService? ss))
        {
            List<IComponent> selectedComponents = new(_savedSelection.Count);
            foreach (string name in _savedSelection)
            {
                IComponent? component = Components[name];
                if (component is not null)
                {
                    selectedComponents.Add(component);
                }
            }

            _savedSelection = null;
            ss.SetSelectedComponents(selectedComponents, SelectionTypes.Replace);
        }
    }

    /// <summary>
    ///  This is called by the designer loader when it wishes to reload the design document.
    ///  The reload will happen immediately so the caller should ensure that it is in a state
    ///  where BeginLoad may be called again.
    /// </summary>
    void IDesignerLoaderHost.Reload()
    {
        if (_loader is null)
        {
            return;
        }

        // Flush the loader to make sure there aren't any pending changes. We always route through the design
        // surface so it can correctly raise its Flushed event.
        _surface!.Flush();

        // Next, stash off the set of selected objects by name. After the reload we will attempt to re-select them.
        if (this.TryGetService(out ISelectionService? ss))
        {
            List<string> list = new(ss.SelectionCount);
            foreach (object item in ss.GetSelectedComponents())
            {
                if (item is IComponent component && component.Site?.Name is string name)
                {
                    list.Add(name);
                }
            }

            _savedSelection = list;
        }

        Unload();
        BeginLoad(_loader);
    }

    bool IDesignerLoaderHost2.IgnoreErrorsDuringReload
    {
        get => _ignoreErrorsDuringReload;
        set
        {
            // Only allow to set to true if we CanReloadWithErrors.
            if (!value || ((IDesignerLoaderHost2)this).CanReloadWithErrors)
            {
                _ignoreErrorsDuringReload = value;
            }
        }
    }

    bool IDesignerLoaderHost2.CanReloadWithErrors { get; set; }

    // We implement IReflect to keep our reflection surface constrained to IDesignerHost.

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
    MethodInfo? IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder? binder, Type[] types, ParameterModifier[]? modifiers) =>
        typeof(IDesignerHost).GetMethod(name, bindingAttr, binder, types, modifiers);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
    MethodInfo? IReflect.GetMethod(string name, BindingFlags bindingAttr) => typeof(IDesignerHost).GetMethod(name, bindingAttr);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
    MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr) => typeof(IDesignerHost).GetMethods(bindingAttr);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
    FieldInfo? IReflect.GetField(string name, BindingFlags bindingAttr) => typeof(IDesignerHost).GetField(name, bindingAttr);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
    FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr) => typeof(IDesignerHost).GetFields(bindingAttr);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
    PropertyInfo? IReflect.GetProperty(string name, BindingFlags bindingAttr) => typeof(IDesignerHost).GetProperty(name, bindingAttr);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
    PropertyInfo? IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[] types, ParameterModifier[]? modifiers) =>
        typeof(IDesignerHost).GetProperty(name, bindingAttr, binder, returnType, types, modifiers);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
    PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr) => typeof(IDesignerHost).GetProperties(bindingAttr);

    internal const DynamicallyAccessedMemberTypes GetAllMembers = DynamicallyAccessedMemberTypes.PublicFields
        | DynamicallyAccessedMemberTypes.NonPublicFields
        | DynamicallyAccessedMemberTypes.PublicMethods
        | DynamicallyAccessedMemberTypes.NonPublicMethods
        | DynamicallyAccessedMemberTypes.PublicEvents
        | DynamicallyAccessedMemberTypes.NonPublicEvents
        | DynamicallyAccessedMemberTypes.PublicProperties
        | DynamicallyAccessedMemberTypes.NonPublicProperties
        | DynamicallyAccessedMemberTypes.PublicConstructors
        | DynamicallyAccessedMemberTypes.NonPublicConstructors
        | DynamicallyAccessedMemberTypes.PublicNestedTypes
        | DynamicallyAccessedMemberTypes.NonPublicNestedTypes;

    [DynamicallyAccessedMembers(GetAllMembers)]
    MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr) => typeof(IDesignerHost).GetMember(name, bindingAttr);

    [DynamicallyAccessedMembers(GetAllMembers)]
    MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr) => typeof(IDesignerHost).GetMembers(bindingAttr);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    object? IReflect.InvokeMember(
        string name,
        BindingFlags invokeAttr,
        Binder? binder,
        object? target,
        object?[]? args,
        ParameterModifier[]? modifiers,
        CultureInfo? culture,
        string[]? namedParameters) =>
        typeof(IDesignerHost).InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);

    Type IReflect.UnderlyingSystemType => typeof(IDesignerHost).UnderlyingSystemType;

    void IServiceContainer.AddService(Type serviceType, object serviceInstance)
    {
        ObjectDisposedException.ThrowIf(!this.TryGetService(out IServiceContainer? sc), typeof(IServiceContainer));
        sc.AddService(serviceType, serviceInstance);
    }

    void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
    {
        ObjectDisposedException.ThrowIf(!this.TryGetService(out IServiceContainer? sc), typeof(IServiceContainer));
        sc.AddService(serviceType, serviceInstance, promote);
    }

    void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback)
    {
        ObjectDisposedException.ThrowIf(!this.TryGetService(out IServiceContainer? sc), typeof(IServiceContainer));
        sc.AddService(serviceType, callback);
    }

    void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
    {
        ObjectDisposedException.ThrowIf(!this.TryGetService(out IServiceContainer? sc), typeof(IServiceContainer));
        sc.AddService(serviceType, callback, promote);
    }

    void IServiceContainer.RemoveService(Type serviceType)
    {
        ObjectDisposedException.ThrowIf(!this.TryGetService(out IServiceContainer? sc), typeof(IServiceContainer));
        sc.RemoveService(serviceType);
    }

    void IServiceContainer.RemoveService(Type serviceType, bool promote)
    {
        ObjectDisposedException.ThrowIf(!this.TryGetService(out IServiceContainer? sc), typeof(IServiceContainer));
        sc.RemoveService(serviceType, promote);
    }

    object? IServiceProvider.GetService(Type serviceType) => GetService(serviceType);
}
