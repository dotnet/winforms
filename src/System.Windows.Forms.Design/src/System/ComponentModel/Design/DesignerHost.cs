// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  This is the main hosting object.  DesignerHost implements services and interfaces specific to the design time IContainer object.  The services this class implements are generally non-removable (they work as a unit so removing them would break things).
    /// </summary>
    internal sealed class DesignerHost : Container, IDesignerLoaderHost2, IDesignerHostTransactionState, IComponentChangeService, IReflect
    {
        // State flags for the state of the designer host
        private static readonly int s_stateLoading = BitVector32.CreateMask(); // Designer is currently loading from the loader host.
        private static readonly int s_stateUnloading = BitVector32.CreateMask(s_stateLoading); // Designer is currently unloading.
        private static readonly int s_stateIsClosingTransaction = BitVector32.CreateMask(s_stateUnloading); // A transaction is in the process of being Canceled or Commited.

        private static readonly Type[] s_defaultServices = new Type[] { typeof(IDesignerHost), typeof(IContainer), typeof(IComponentChangeService), typeof(IDesignerLoaderHost2) };

        // IDesignerHost events
        private static readonly object s_eventActivated = new object(); // Designer has been activated
        private static readonly object s_eventDeactivated = new object(); // Designer has been deactivated
        private static readonly object s_eventLoadComplete = new object(); // Loading has been completed
        private static readonly object s_eventTransactionClosed = new object(); // The last transaction has been closed
        private static readonly object s_eventTransactionClosing = new object(); // The last transaction is about to be closed
        private static readonly object s_eventTransactionOpened = new object(); // The first transaction has been opened
        private static readonly object s_eventTransactionOpening = new object(); // The first transaction is about to be opened

        // IComponentChangeService events
        private static readonly object s_eventComponentAdding = new object(); // A component is about to be added to the container
        private static readonly object s_eventComponentAdded = new object(); // A component was just added to the container
        private static readonly object s_eventComponentChanging = new object(); // A component is about to be changed
        private static readonly object s_eventComponentChanged = new object(); // A component has changed
        private static readonly object s_eventComponentRemoving = new object(); // A component is about to be removed from the container
        private static readonly object s_eventComponentRemoved = new object(); // A component has been removed from the container
        private static readonly object s_eventComponentRename = new object(); // A component has been renamed

        // Member variables
        private BitVector32 _state; // state for this host
        private DesignSurface _surface; // the owning designer surface.
        private string _newComponentName; // transient value indicating the name of a component that is being created
        private Stack _transactions; // stack of transactions.  Each entry in the stack is a DesignerTransaction
        private IComponent _rootComponent; // the root of our design
        private string _rootComponentClassName; // class name of the root of our design
        private readonly Hashtable _designers;  // designer -> component mapping
        private readonly EventHandlerList _events; // event list
        private DesignerLoader _loader; // the loader that loads our designers
        private ICollection _savedSelection; // set of selected components saved across reloads
        private HostDesigntimeLicenseContext _licenseCtx;
        private IDesignerEventService _designerEventService;
        private static readonly object s_selfLock = new object();
        private bool _ignoreErrorsDuringReload;
        private bool _canReloadWithErrors;
        private TypeDescriptionProviderService _typeService;
        private bool _typeServiceChecked;

        public DesignerHost(DesignSurface surface)
        {
            _surface = surface;
            _state = new BitVector32();
            _designers = new Hashtable();
            _events = new EventHandlerList();

            // Add the relevant services.  We try to add these as "fixed" services.  A fixed service cannot be removed by the user.  The reason for this is that each of these services depends on each other, so you can't really remove and replace just one of them. If we can't get our own service container that supports fixed services, we add these as regular services.
            if (GetService(typeof(DesignSurfaceServiceContainer)) is DesignSurfaceServiceContainer dsc)
            {
                foreach (Type t in s_defaultServices)
                {
                    dsc.AddFixedService(t, this);
                }
            }
            else
            {
                IServiceContainer sc = GetService(typeof(IServiceContainer)) as IServiceContainer;
                Debug.Assert(sc != null, "DesignerHost: Ctor needs a service provider that provides IServiceContainer");
                if (sc != null)
                {
                    foreach (Type t in s_defaultServices)
                    {
                        sc.AddService(t, this);
                    }
                }
            }
        }

        internal HostDesigntimeLicenseContext LicenseContext
        {
            get
            {
                if (_licenseCtx is null)
                {
                    _licenseCtx = new HostDesigntimeLicenseContext(this);
                }
                return _licenseCtx;
            }
        }

        // Internal flag which is used to track when we are in the process of commiting or canceling a transaction.
        internal bool IsClosingTransaction
        {
            get { return _state[s_stateIsClosingTransaction]; }
            set { _state[s_stateIsClosingTransaction] = value; }
        }

        bool IDesignerHostTransactionState.IsClosingTransaction
        {
            get { return IsClosingTransaction; }
        }

        /// <summary>
        ///  Override of Container.Add
        /// </summary>
        public override void Add(IComponent component, string name)
        {
            if (!_typeServiceChecked)
            {
                _typeService = GetService(typeof(TypeDescriptionProviderService)) as TypeDescriptionProviderService;
                _typeServiceChecked = true;
            }
            // TypeDescriptionProviderService is attached at design time only
            if (_typeService != null)
            {
                // Check for the attribute that VsTargetFrameworkProvider injects on reflection types to see if VsTargetFrameworkProvider is already attached.
                Type type = TypeDescriptor.GetProvider(component).GetReflectionType(typeof(object));
                if (!type.IsDefined(typeof(ProjectTargetFrameworkAttribute), false))
                {
                    TypeDescriptionProvider typeProvider = _typeService.GetProvider(component);
                    if (typeProvider != null)
                    {
                        TypeDescriptor.AddProvider(typeProvider, component);
                    }
                }
            }
            PerformAdd(component, name);
        }

        private void PerformAdd(IComponent component, string name)
        {
            if (AddToContainerPreProcess(component, name, this))
            {
                // Site creation fabricates a name for this component.
                base.Add(component, name);
                try
                {
                    AddToContainerPostProcess(component, name, this);
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
        }

        /// <summary>
        ///  We support adding to either our main IDesignerHost container or to a private per-site container for nested objects.  This code is the stock add code that creates a designer, etc.  See Add (above) for an example of how to call this correctly.
        ///  This method is called before the component is actually added.  It returns true if the component can be added to this container or false if the add should not occur (because the component may already be in this container, for example.) It may also throw if adding this component is illegal.
        /// </summary>
        internal bool AddToContainerPreProcess(IComponent component, string name, IContainer containerToAddTo)
        {
            if (component is null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            // We should never add anything while we're unloading.
            if (_state[s_stateUnloading])
            {
                Exception ex = new Exception(SR.DesignerHostUnloading)
                {
                    HelpLink = SR.DesignerHostUnloading
                };
                throw ex;
            }

            // Make sure we're not adding an instance of the root component to itself.
            if (_rootComponent != null)
            {
                if (string.Equals(component.GetType().FullName, _rootComponentClassName, StringComparison.OrdinalIgnoreCase))
                {
                    Exception ex = new Exception(string.Format(SR.DesignerHostCyclicAdd, component.GetType().FullName, _rootComponentClassName))
                    {
                        HelpLink = SR.DesignerHostCyclicAdd
                    };
                    throw ex;
                }
            }

            ISite existingSite = component.Site;
            // If the component is already in our container, we just rename.
            if (existingSite != null && existingSite.Container == this)
            {
                if (name != null)
                {
                    existingSite.Name = name;
                }
                return false;
            }
            // Raise an adding event for our container if the container is us.
            ComponentEventArgs ce = new ComponentEventArgs(component);
            (_events[s_eventComponentAdding] as ComponentEventHandler)?.Invoke(containerToAddTo, ce);
            return true;
        }

        /// <summary>
        ///  We support adding to either our main IDesignerHost container or to a private     per-site container for nested objects.  This code is the stock add code     that creates a designer, etc.  See Add (above) for an example of how to call     this correctly.
        /// </summary>
        internal void AddToContainerPostProcess(IComponent component, string name, IContainer containerToAddTo)
        {
            // Now that we've added, check to see if this is an extender provider.  If it is, add it to our extender provider service so it is available.
            if (component is IExtenderProvider &&
                // UNDONE.  Try to get Inheritance knowledge out of this basic code.
                !TypeDescriptor.GetAttributes(component).Contains(InheritanceAttribute.InheritedReadOnly))
            {
                if (GetService(typeof(IExtenderProviderService)) is IExtenderProviderService eps)
                {
                    eps.AddExtenderProvider((IExtenderProvider)component);
                }
            }

            IDesigner designer;
            // Is this the first component the loader has created?  If so, then it must be the root component (by definition) so we will expect there to be a root designer associated with the component.  Otherwise, we search for a normal designer, which can be optionally provided.
            if (_rootComponent is null)
            {
                designer = _surface.CreateDesigner(component, true) as IRootDesigner;
                if (designer is null)
                {
                    Exception ex = new Exception(string.Format(SR.DesignerHostNoTopLevelDesigner, component.GetType().FullName))
                    {
                        HelpLink = SR.DesignerHostNoTopLevelDesigner
                    };
                    throw ex;
                }

                _rootComponent = component;
                // Check and see if anyone has set the class name of the root component. we default to the component name.
                if (_rootComponentClassName is null)
                {
                    _rootComponentClassName = component.Site.Name;
                }
            }
            else
            {
                designer = _surface.CreateDesigner(component, false);
            }

            if (designer != null)
            {
                // The presence of a designer in this table allows the designer to filter the component's properties, which is often needed during designer initialization.  So, we stuff it in the table first, initialize, and if it throws we remove it from the table.
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
                if (designer is IExtenderProvider)
                {
                    if (GetService(typeof(IExtenderProviderService)) is IExtenderProviderService eps)
                    {
                        eps.AddExtenderProvider((IExtenderProvider)designer);
                    }
                }
            }

            // The component has been added.  Note that it is tempting to move this above the designer because the designer will never need to know that its own component just got added, but this would be bad because the designer is needed to extract shadowed properties from the component.
            ComponentEventArgs ce = new ComponentEventArgs(component);
            (_events[s_eventComponentAdded] as ComponentEventHandler)?.Invoke(containerToAddTo, ce);
        }

        /// <summary>
        ///  Called by DesignerSurface to begin loading the designer.
        /// </summary>
        internal void BeginLoad(DesignerLoader loader)
        {
            if (_loader != null && _loader != loader)
            {
                Exception ex = new InvalidOperationException(SR.DesignerHostLoaderSpecified)
                {
                    HelpLink = SR.DesignerHostLoaderSpecified
                };
                throw ex;
            }

            IDesignerEventService des = null;
            bool reloading = (_loader != null);
            _loader = loader;
            if (!reloading)
            {
                if (loader is IExtenderProvider)
                {
                    if (GetService(typeof(IExtenderProviderService)) is IExtenderProviderService eps)
                    {
                        eps.AddExtenderProvider((IExtenderProvider)loader);
                    }
                }

                des = GetService(typeof(IDesignerEventService)) as IDesignerEventService;
                if (des != null)
                {
                    des.ActiveDesignerChanged += new ActiveDesignerEventHandler(OnActiveDesignerChanged);
                    _designerEventService = des;
                }
            }
            _state[s_stateLoading] = true;
            _surface.OnLoading();

            try
            {
                _loader?.BeginLoad(this);
            }
            catch (Exception e)
            {
                if (e is TargetInvocationException)
                {
                    e = e.InnerException;
                }

                string message = e.Message;
                // We must handle the case of an exception with no message.
                if (message is null || message.Length == 0)
                {
                    e = new Exception(string.Format(SR.DesignSurfaceFatalError, e.ToString()), e);
                }

                // Loader blew up.  Add this exception to our error list.
                ((IDesignerLoaderHost)this).EndLoad(null, false, new object[] { e });
            }

            if (_designerEventService is null)
            {
                // If there is no designer event service, make this designer the currently active designer.  It will remain active.
                OnActiveDesignerChanged(null, new ActiveDesignerEventArgs(null, this));
            }
        }

        /// <summary>
        ///  Override of CreateSite.  We create a custom site here, called Site, which is an inner class of DesignerHost.  DesignerSite contains an instance of the designer for the component.
        /// </summary>
        /// <param name="component"> The component to create the site for </param>
        /// <param name="name"> The name of the component.  If no name is provided this will fabricate a name for you. </param>
        /// <returns> The newly created site </returns>
        protected override ISite CreateSite(IComponent component, string name)
        {
            Debug.Assert(component != null, "Caller should have guarded against a null component");
            // We need to handle the case where a component's ctor adds itself to the container.  We don't want to do the work of creating a name, and then immediately renaming.  So, DesignerHost's CreateComponent will set _newComponentName to the newly created name before creating the component.
            if (_newComponentName != null)
            {
                name = _newComponentName;
                _newComponentName = null;
            }

            INameCreationService nameCreate = GetService(typeof(INameCreationService)) as INameCreationService;
            // Fabricate a name if one wasn't provided.  We try to use the name creation service, but if it is not available we will just use an empty string.
            if (name is null)
            {
                if (nameCreate != null)
                {
                    // VirtualTypes and Compact framework types will need to use  reflection type in order to get their "real" name (the one  available in the compact FX, for example)
                    Type reflectType = TypeDescriptor.GetReflectionType(component);
                    if (reflectType.FullName.Equals(component.GetType().FullName))
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
                if (nameCreate != null)
                {
                    nameCreate.ValidateName(name);
                }
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
        ///  We move all "dispose" functionality to the DisposeHost method.  The reason for this is that Dispose is inherited from our container implementation, and we do not want someone disposing the container.  That would leave the design surface still alive, but it would kill the host.  Instead, DesignSurface always calls DisposeHost, which calls the base version of Dispose to clean out the container.
        /// </summary>
        internal void DisposeHost()
        {
            try
            {
                if (_loader != null)
                {
                    _loader.Dispose();
                    Unload();
                }

                if (_surface != null)
                {
                    if (_designerEventService != null)
                    {
                        _designerEventService.ActiveDesignerChanged -= new ActiveDesignerEventHandler(OnActiveDesignerChanged);
                    }

                    if (GetService(typeof(DesignSurfaceServiceContainer)) is DesignSurfaceServiceContainer dsc)
                    {
                        foreach (Type t in s_defaultServices)
                        {
                            dsc.RemoveFixedService(t);
                        }
                    }
                    else
                    {
                        IServiceContainer sc = GetService(typeof(IServiceContainer)) as IServiceContainer;
                        Debug.Assert(sc != null, "DesignerHost: Ctor needs a service provider that provides IServiceContainer");
                        if (sc != null)
                        {
                            foreach (Type t in s_defaultServices)
                            {
                                sc.RemoveService(t);
                            }
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
        internal void Flush()
        {
            if (_loader != null)
            {
                _loader.Flush();
            }
        }

        /// <summary>
        ///  Override of Container's GetService method.  This just delegates to the  parent service provider.
        /// </summary>
        /// <param name="service"> The type of service to retrieve </param>
        /// <returns> An instance of the service. </returns>
        protected override object GetService(Type service)
        {
            object serviceInstance = null;
            if (service is null)
            {
                throw new ArgumentNullException(nameof(service));
            }

            if (service == typeof(IMultitargetHelperService))
            {
                if (_loader is IServiceProvider provider)
                {
                    serviceInstance = provider.GetService(typeof(IMultitargetHelperService));
                }
            }
            else
            {
                serviceInstance = base.GetService(service);
                if (serviceInstance is null && _surface != null)
                {
                    serviceInstance = _surface.GetService(service);
                }
            }
            return serviceInstance;
        }

        /// <summary>
        ///  Called in response to a designer becoming active or inactive.
        /// </summary>
        private void OnActiveDesignerChanged(object sender, ActiveDesignerEventArgs e)
        {
            // NOTE: sender can be null (we call this directly in BeginLoad)
            if (e is null)
            {
                return;
            }

            object eventobj = null;

            if (e.OldDesigner == this)
            {
                eventobj = s_eventDeactivated;
            }
            else if (e.NewDesigner == this)
            {
                eventobj = s_eventActivated;
            }

            // Not our document, so we don't fire.
            if (eventobj is null)
            {
                return;
            }

            // If we are deactivating, flush any code changes. We always route through the design surface so it can correctly raise its Flushed event.
            if (e.OldDesigner == this && _surface != null)
            {
                _surface.Flush();
            }

            // Fire the appropriate event.
            (_events[eventobj] as EventHandler)?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///  Method is called by the site when a component is renamed.
        /// </summary>
        private void OnComponentRename(IComponent component, string oldName, string newName)
        {
            // If the root component is being renamed we need to update RootComponentClassName.
            if (component == _rootComponent)
            {
                string className = _rootComponentClassName;
                int oldNameIndex = className.LastIndexOf(oldName);
                if (oldNameIndex + oldName.Length == className.Length // If oldName occurs at the end of className
                    && (oldNameIndex - 1 >= 0 && className[oldNameIndex - 1] == '.')) // and is preceeded by a period
                {
                    // We assume the preceeding chars are the namespace and preserve it.
                    _rootComponentClassName = className.Substring(0, oldNameIndex) + newName;
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
        private void OnLoadComplete(EventArgs e)
        {
            (_events[s_eventLoadComplete] as EventHandler)?.Invoke(this, e);
        }

        /// <summary>
        ///  Method is called when the last transaction has closed.
        /// </summary>
        private void OnTransactionClosed(DesignerTransactionCloseEventArgs e)
        {
            (_events[s_eventTransactionClosed] as DesignerTransactionCloseEventHandler)?.Invoke(this, e);
        }

        /// <summary>
        ///  Method is called when the last transaction is closing.
        /// </summary>
        private void OnTransactionClosing(DesignerTransactionCloseEventArgs e)
        {
            (_events[s_eventTransactionClosing] as DesignerTransactionCloseEventHandler)?.Invoke(this, e);
        }

        /// <summary>
        ///  Method is called when the first transaction has opened.
        /// </summary>
        private void OnTransactionOpened(EventArgs e)
        {
            (_events[s_eventTransactionOpened] as EventHandler)?.Invoke(this, e);
        }

        /// <summary>
        ///  Method is called when the first transaction is opening.
        /// </summary>
        private void OnTransactionOpening(EventArgs e)
        {
            (_events[s_eventTransactionOpening] as EventHandler)?.Invoke(this, e);
        }

        /// <summary>
        ///  Called to remove a component from its container.
        /// </summary>
        public override void Remove(IComponent component)
        {
            if (RemoveFromContainerPreProcess(component, this))
            {
                Site site = component.Site as Site;
                RemoveWithoutUnsiting(component);
                RemoveFromContainerPostProcess(component, this);
                if (site != null)
                {
                    site.Disposed = true;
                }
            }
        }

        internal bool RemoveFromContainerPreProcess(IComponent component, IContainer container)
        {
            if (component is null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            ISite site = component.Site;
            if (site is null || site.Container != container)
            {
                return false;
            }

            ComponentEventHandler eh;
            ComponentEventArgs ce = new ComponentEventArgs(component);

            eh = _events[s_eventComponentRemoving] as ComponentEventHandler;
            eh?.Invoke(this, ce);

            // If the component is an extender provider, remove it from the extender provider service, should one exist.
            if (component is IExtenderProvider)
            {
                if (GetService(typeof(IExtenderProviderService)) is IExtenderProviderService eps)
                {
                    eps.RemoveExtenderProvider((IExtenderProvider)component);
                }
            }

            // Same for the component's designer
            IDesigner designer = _designers[component] as IDesigner;

            if (designer is IExtenderProvider)
            {
                if (GetService(typeof(IExtenderProviderService)) is IExtenderProviderService eps)
                {
                    eps.RemoveExtenderProvider((IExtenderProvider)designer);
                }
            }

            if (designer != null)
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

        internal void RemoveFromContainerPostProcess(IComponent component, IContainer container)
        {
            // At one point during Whidbey, the component used to be unsited earlier in this process and it would be temporarily resited here before raising OnComponentRemoved. The problem with resiting it is that some 3rd party controls take action when a component is sited (such as displaying  a dialog a control is dropped on the form) and resiting here caused them to think they were being initialized for the first time.  To preserve compat, we shouldn't resite the component  during Remove.
            try
            {
                ComponentEventHandler eh = _events[s_eventComponentRemoved] as ComponentEventHandler;
                ComponentEventArgs ce = new ComponentEventArgs(component);
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

            if (GetService(typeof(IHelpService)) is IHelpService helpService && _rootComponent != null && _designers[_rootComponent] != null)
            {
                helpService.RemoveContextAttribute("Keyword", "Designer_" + _designers[_rootComponent].GetType().FullName);
            }

            ISelectionService selectionService = (ISelectionService)GetService(typeof(ISelectionService));
            if (selectionService != null)
            {
                selectionService.SetSelectedComponents(null, SelectionTypes.Replace);
            }

            // Now remove all the designers and their components.  We save the root for last.  Note that we eat any exceptions that components or their designers generate.  A bad component or designer should not prevent an unload from happening.  We do all of this in a transaction to help reduce the number of events we generate.
            _state[s_stateUnloading] = true;
            DesignerTransaction t = ((IDesignerHost)this).CreateTransaction();
            ArrayList exceptions = new ArrayList();
            try
            {
                IComponent[] components = new IComponent[Components.Count];
                Components.CopyTo(components, 0);

                foreach (IComponent comp in components)
                {
                    if (!object.ReferenceEquals(comp, _rootComponent))
                    {
                        if (_designers[comp] is IDesigner designer)
                        {
                            _designers.Remove(comp);
                            try
                            {
                                designer.Dispose();
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

                if (_rootComponent != null)
                {
                    if (_designers[_rootComponent] is IDesigner designer)
                    {
                        _designers.Remove(_rootComponent);
                        try
                        {
                            designer.Dispose();
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

            // There should be no open transactions.  Commit all of the ones that are open.
            if (_transactions != null && _transactions.Count > 0)
            {
                Debug.Fail("There are open transactions at unload");
                while (_transactions.Count > 0)
                {
                    DesignerTransaction trans = (DesignerTransaction)_transactions.Peek(); // it'll get pop'ed in the OnCommit for DesignerHostTransaction
                    trans.Commit();
                }
            }

            _surface?.OnUnloaded();

            if (exceptions.Count > 0)
            {
                throw new ExceptionCollection(exceptions);
            }
        }

        /// <summary>
        ///  Adds an event handler for the System.ComponentModel.Design.IComponentChangeService.ComponentAdded event.
        /// </summary>
        event ComponentEventHandler IComponentChangeService.ComponentAdded
        {
            add => _events.AddHandler(s_eventComponentAdded, value);
            remove => _events.RemoveHandler(s_eventComponentAdded, value);
        }

        /// <summary>
        ///  Adds an event handler for the System.ComponentModel.Design.IComponentChangeService.ComponentAdding event.
        /// </summary>
        event ComponentEventHandler IComponentChangeService.ComponentAdding
        {
            add => _events.AddHandler(s_eventComponentAdding, value);
            remove => _events.RemoveHandler(s_eventComponentAdding, value);
        }

        /// <summary>
        ///  Adds an event handler for the System.ComponentModel.Design.IComponentChangeService.ComponentChanged event.
        /// </summary>
        event ComponentChangedEventHandler IComponentChangeService.ComponentChanged
        {
            add => _events.AddHandler(s_eventComponentChanged, value);
            remove => _events.RemoveHandler(s_eventComponentChanged, value);
        }

        /// <summary>
        ///  Adds an event handler for the System.ComponentModel.Design.IComponentChangeService.ComponentChanging event.
        /// </summary>
        event ComponentChangingEventHandler IComponentChangeService.ComponentChanging
        {
            add => _events.AddHandler(s_eventComponentChanging, value);
            remove => _events.RemoveHandler(s_eventComponentChanging, value);
        }

        /// <summary>
        ///  Adds an event handler for the System.ComponentModel.Design.IComponentChangeService.OnComponentRemoved event.
        /// </summary>
        event ComponentEventHandler IComponentChangeService.ComponentRemoved
        {
            add => _events.AddHandler(s_eventComponentRemoved, value);
            remove => _events.RemoveHandler(s_eventComponentRemoved, value);
        }

        /// <summary>
        ///  Adds an event handler for the System.ComponentModel.Design.IComponentChangeService.OnComponentRemoving event.
        /// </summary>
        event ComponentEventHandler IComponentChangeService.ComponentRemoving
        {
            add => _events.AddHandler(s_eventComponentRemoving, value);
            remove => _events.RemoveHandler(s_eventComponentRemoving, value);
        }

        /// <summary>
        ///  Adds an event handler for the System.ComponentModel.Design.IComponentChangeService.OnComponentRename event.
        /// </summary>
        event ComponentRenameEventHandler IComponentChangeService.ComponentRename
        {
            add => _events.AddHandler(s_eventComponentRename, value);
            remove => _events.RemoveHandler(s_eventComponentRename, value);
        }

        /// <summary>
        ///  Announces to the component change service that a particular component has changed.
        /// </summary>
        void IComponentChangeService.OnComponentChanged(object component, MemberDescriptor member, object oldValue, object newValue)
        {
            if (!((IDesignerHost)this).Loading)
            {
                (_events[s_eventComponentChanged] as ComponentChangedEventHandler)?.Invoke(this, new ComponentChangedEventArgs(component, member, oldValue, newValue));
            }
        }

        /// <summary>
        ///  Announces to the component change service that a particular component is changing.
        /// </summary>
        void IComponentChangeService.OnComponentChanging(object component, MemberDescriptor member)
        {
            if (!((IDesignerHost)this).Loading)
            {
                (_events[s_eventComponentChanging] as ComponentChangingEventHandler)?.Invoke(this, new ComponentChangingEventArgs(component, member));
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the designer host is currently loading the document.
        /// </summary>
        bool IDesignerHost.Loading
        {
            get => _state[s_stateLoading] || _state[s_stateUnloading] || (_loader != null && _loader.Loading);
        }

        /// <summary>
        ///  Gets a value indicating whether the designer host is currently in a transaction.
        /// </summary>
        bool IDesignerHost.InTransaction
        {
            get => (_transactions != null && _transactions.Count > 0) || IsClosingTransaction;
        }

        /// <summary>
        ///  Gets the container for this designer host.
        /// </summary>
        IContainer IDesignerHost.Container
        {
            get => this;
        }

        /// <summary>
        ///  Gets the instance of the base class used as the base class for the current design.
        /// </summary>
        IComponent IDesignerHost.RootComponent
        {
            get => _rootComponent;
        }

        /// <summary>
        ///  Gets the fully qualified name of the class that is being designed.
        /// </summary>
        string IDesignerHost.RootComponentClassName
        {
            get => _rootComponentClassName;
        }

        /// <summary>
        ///  Gets the description of the current transaction.
        /// </summary>
        string IDesignerHost.TransactionDescription
        {
            get
            {
                if (_transactions != null && _transactions.Count > 0)
                {
                    return ((DesignerTransaction)_transactions.Peek()).Description;
                }
                return null;
            }
        }

        /// <summary>
        ///  Adds an event handler for the <see cref='System.ComponentModel.Design.IDesignerHost.Activated'/> event.
        /// </summary>
        event EventHandler IDesignerHost.Activated
        {
            add => _events.AddHandler(s_eventActivated, value);
            remove => _events.RemoveHandler(s_eventActivated, value);
        }

        /// <summary>
        ///  Adds an event handler for the <see cref='System.ComponentModel.Design.IDesignerHost.Deactivated'/> event.
        /// </summary>
        event EventHandler IDesignerHost.Deactivated
        {
            add => _events.AddHandler(s_eventDeactivated, value);
            remove => _events.RemoveHandler(s_eventDeactivated, value);
        }

        /// <summary>
        ///  Adds an event handler for the <see cref='System.ComponentModel.Design.IDesignerHost.LoadComplete'/> event.
        /// </summary>
        event EventHandler IDesignerHost.LoadComplete
        {
            add => _events.AddHandler(s_eventLoadComplete, value);
            remove => _events.RemoveHandler(s_eventLoadComplete, value);
        }

        /// <summary>
        ///  Adds an event handler for the <see cref='System.ComponentModel.Design.IDesignerHost.TransactionClosed'/> event.
        /// </summary>
        event DesignerTransactionCloseEventHandler IDesignerHost.TransactionClosed
        {
            add => _events.AddHandler(s_eventTransactionClosed, value);
            remove => _events.RemoveHandler(s_eventTransactionClosed, value);
        }

        /// <summary>
        ///  Adds an event handler for the <see cref='System.ComponentModel.Design.IDesignerHost.TransactionClosing'/> event.
        /// </summary>
        event DesignerTransactionCloseEventHandler IDesignerHost.TransactionClosing
        {
            add => _events.AddHandler(s_eventTransactionClosing, value);
            remove => _events.RemoveHandler(s_eventTransactionClosing, value);
        }

        /// <summary>
        ///  Adds an event handler for the <see cref='System.ComponentModel.Design.IDesignerHost.TransactionOpened'/> event.
        /// </summary>
        event EventHandler IDesignerHost.TransactionOpened
        {
            add => _events.AddHandler(s_eventTransactionOpened, value);
            remove => _events.RemoveHandler(s_eventTransactionOpened, value);
        }

        /// <summary>
        ///  Adds an event handler for the <see cref='System.ComponentModel.Design.IDesignerHost.TransactionOpening'/> event.
        /// </summary>
        event EventHandler IDesignerHost.TransactionOpening
        {
            add => _events.AddHandler(s_eventTransactionOpening, value);
            remove => _events.RemoveHandler(s_eventTransactionOpening, value);
        }

        /// <summary>
        ///  Activates the designer that this host is hosting.
        /// </summary>
        void IDesignerHost.Activate()
        {
            _surface?.OnViewActivate();
        }

        /// <summary>
        ///  Creates a component of the specified class type.
        /// </summary>
        IComponent IDesignerHost.CreateComponent(Type componentType)
        {
            return ((IDesignerHost)this).CreateComponent(componentType, null);
        }

        /// <summary>
        ///  Creates a component of the given class type and name and places it into the designer container.
        /// </summary>
        IComponent IDesignerHost.CreateComponent(Type componentType, string name)
        {
            if (componentType is null)
            {
                throw new ArgumentNullException(nameof(componentType));
            }

            IComponent component;
            LicenseContext oldContext = LicenseManager.CurrentContext;
            bool changingContext = false; // we don't want if there is a recursivity (creating a component create another one) to change the context again. we already have the one we want and that would create a locking problem.
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
                    component = _surface.CreateInstance(componentType) as IComponent;
                }
                finally
                {
                    _newComponentName = null;
                }

                if (component is null)
                {
                    InvalidOperationException ex = new InvalidOperationException(string.Format(SR.DesignerHostFailedComponentCreate, componentType.Name))
                    {
                        HelpLink = SR.DesignerHostFailedComponentCreate
                    };
                    throw ex;
                }

                // Add this component to our container
                //
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

        /// <summary>
        ///  Lengthy operations that involve multiple components may raise many events.  These events may cause other side-effects, such as flicker or performance degradation.  When operating on multiple components at one time, or setting multiple properties on a single component, you should encompass these changes inside a transaction.  Transactions are used to improve performance and reduce flicker.  Slow operations can listen to  transaction events and only do work when the transaction completes.
        /// </summary>
        DesignerTransaction IDesignerHost.CreateTransaction()
        {
            return ((IDesignerHost)this).CreateTransaction(null);
        }

        /// <summary>
        ///  Lengthy operations that involve multiple components may raise many events.  These events may cause other side-effects, such as flicker or performance degradation.  When operating on multiple components at one time, or setting multiple properties on a single component, you should encompass these changes inside a transaction.  Transactions are used to improve performance and reduce flicker.  Slow operations can listen to  transaction events and only do work when the transaction completes.
        /// </summary>
        DesignerTransaction IDesignerHost.CreateTransaction(string description)
        {
            if (description is null)
            {
                description = SR.DesignerHostGenericTransactionName;
            }
            return new DesignerHostTransaction(this, description);
        }

        /// <summary>
        ///  Destroys the given component, removing it from the design container.
        /// </summary>
        void IDesignerHost.DestroyComponent(IComponent component)
        {
            string name;
            if (component is null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if (component.Site != null && component.Site.Name != null)
            {
                name = component.Site.Name;
            }
            else
            {
                name = component.GetType().Name;
            }

            // Make sure the component is not being inherited -- we can't delete these!
            // UNDONE.  Try to get Inheritance knowledge out of this basic code.
            InheritanceAttribute ia = (InheritanceAttribute)TypeDescriptor.GetAttributes(component)[typeof(InheritanceAttribute)];
            if (ia != null && ia.InheritanceLevel != InheritanceLevel.NotInherited)
            {
                Exception ex = new InvalidOperationException(string.Format(SR.DesignerHostCantDestroyInheritedComponent, name))
                {
                    HelpLink = SR.DesignerHostCantDestroyInheritedComponent
                };
                throw ex;
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
                    // We need to signal changing and then perform the remove.  Remove must be done by us and not by Dispose because (a) people need a chance to cancel through a Removing event, and (b) Dispose removes from the container last and anything that would sync Removed would end up with a dead component.
                    Remove(component);
                    component.Dispose();
                    t.Commit();
                }
            }
        }

        /// <summary>
        ///  Gets the designer instance for the specified component.
        /// </summary>
        IDesigner IDesignerHost.GetDesigner(IComponent component)
        {
            if (component is null)
            {
                throw new ArgumentNullException(nameof(component));
            }
            return _designers[component] as IDesigner;
        }

        /// <summary>
        ///  Gets the type instance for the specified fully qualified type name <paramref name="typeName"/>.
        /// </summary>
        Type IDesignerHost.GetType(string typeName)
        {
            if (typeName is null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            if (GetService(typeof(ITypeResolutionService)) is ITypeResolutionService ts)
            {
                return ts.GetType(typeName);
            }
            return Type.GetType(typeName);
        }

        /// <summary>
        ///  This is called by the designer loader to indicate that the load has  terminated.  If there were errors, they should be passed in the errorCollection as a collection of exceptions (if they are not exceptions the designer loader host may just call ToString on them).  If the load was successful then errorCollection should either be null or contain an empty collection.
        /// </summary>
        void IDesignerLoaderHost.EndLoad(string rootClassName, bool successful, ICollection errorCollection)
        {
            bool wasLoading = _state[s_stateLoading];
            _state[s_stateLoading] = false;

            if (rootClassName != null)
            {
                _rootComponentClassName = rootClassName;
            }
            else if (_rootComponent != null && _rootComponent.Site != null)
            {
                _rootComponentClassName = _rootComponent.Site.Name;
            }

            // If the loader indicated success, but it never created a component, that is an error.
            if (successful && _rootComponent is null)
            {
                ArrayList errorList = new ArrayList();
                InvalidOperationException ex = new InvalidOperationException(SR.DesignerHostNoBaseClass)
                {
                    HelpLink = SR.DesignerHostNoBaseClass
                };
                errorList.Add(ex);
                errorCollection = errorList;
                successful = false;
            }

            // If we failed, unload the doc so that the OnLoaded event can't get to anything that actually did work.
            if (!successful)
            {
                Unload();
            }

            if (wasLoading && _surface != null)
            {
                _surface.OnLoaded(successful, errorCollection);
            }

            if (successful)
            {
                // We may be invoked to do an EndLoad when we are already loaded.  This can happen if the user called AddLoadDependency, essentially putting us in a loading state while we are already loaded.  This is OK, and is used as a hint that the user is going to monkey with settings but doesn't want the code engine to report it.
                if (wasLoading)
                {
                    IRootDesigner rootDesigner = ((IDesignerHost)this).GetDesigner(_rootComponent) as IRootDesigner;
                    // Offer up our base help attribute
                    if (GetService(typeof(IHelpService)) is IHelpService helpService)
                    {
                        helpService.AddContextAttribute("Keyword", "Designer_" + rootDesigner.GetType().FullName, HelpKeywordType.F1Keyword);
                    }

                    // and let everyone know that we're loaded
                    try
                    {
                        OnLoadComplete(EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail("Exception thrown on LoadComplete event handler.  You should not throw here : " + ex.ToString());
                        // The load complete failed.  Put us back in the loading state and unload.
                        _state[s_stateLoading] = true;
                        Unload();

                        ArrayList errorList = new ArrayList
                        {
                            ex
                        };
                        if (errorCollection != null)
                        {
                            errorList.AddRange(errorCollection);
                        }
                        errorCollection = errorList;
                        successful = false;

                        if (_surface != null)
                        {
                            _surface.OnLoaded(successful, errorCollection);
                        }

                        // We re-throw.  If this was a synchronous load this will error back to BeginLoad (and, as a side effect, may call us again).  For asynchronous loads we need to throw so the caller knows what happened.
                        throw;
                    }

                    // If we saved a selection as a result of a reload, try to replace it.
                    if (successful && _savedSelection != null)
                    {
                        if (GetService(typeof(ISelectionService)) is ISelectionService ss)
                        {
                            ArrayList selectedComponents = new ArrayList(_savedSelection.Count);
                            foreach (string name in _savedSelection)
                            {
                                IComponent comp = Components[name];
                                if (comp != null)
                                {
                                    selectedComponents.Add(comp);
                                }
                            }
                            _savedSelection = null;
                            ss.SetSelectedComponents(selectedComponents, SelectionTypes.Replace);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  This is called by the designer loader when it wishes to reload the design document.  The reload will happen immediately so the caller should ensure that it is in a state where BeginLoad may be called again.
        /// </summary>
        void IDesignerLoaderHost.Reload()
        {
            if (_loader != null)
            {
                // Flush the loader to make sure there aren't any pending  changes.  We always route through the design surface so it can correctly raise its Flushed event.
                _surface.Flush();
                // Next, stash off the set of selected objects by name.  After the reload we will attempt to re-select them.
                if (GetService(typeof(ISelectionService)) is ISelectionService ss)
                {
                    ArrayList list = new ArrayList(ss.SelectionCount);
                    foreach (object o in ss.GetSelectedComponents())
                    {
                        if (o is IComponent comp && comp.Site != null && comp.Site.Name != null)
                        {
                            list.Add(comp.Site.Name);
                        }
                    }
                    _savedSelection = list;
                }
                Unload();
                BeginLoad(_loader);
            }
        }

        bool IDesignerLoaderHost2.IgnoreErrorsDuringReload
        {
            get => _ignoreErrorsDuringReload;
            set
            {
                // Only allow to set to true if we CanReloadWithErrors
                if (!value || ((IDesignerLoaderHost2)this).CanReloadWithErrors)
                {
                    _ignoreErrorsDuringReload = value;
                }
            }
        }

        bool IDesignerLoaderHost2.CanReloadWithErrors
        {
            get => _canReloadWithErrors;
            set => _canReloadWithErrors = value;
        }

        /// <summary>
        ///  IReflect implementation to map DesignerHost to IDesignerHost.  This helps keep us private.
        /// </summary>
        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
        {
            return typeof(IDesignerHost).GetMethod(name, bindingAttr, binder, types, modifiers);
        }

        /// <summary>
        ///  IReflect implementation to map DesignerHost to IDesignerHost.  This helps keep us private.
        /// </summary>
        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr)
        {
            return typeof(IDesignerHost).GetMethod(name, bindingAttr);
        }

        /// <summary>
        ///  IReflect implementation to map DesignerHost to IDesignerHost.  This helps keep us private.
        /// </summary>
        MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
        {
            return typeof(IDesignerHost).GetMethods(bindingAttr);
        }

        /// <summary>
        ///  IReflect implementation to map DesignerHost to IDesignerHost.  This helps keep us private.
        /// </summary>
        FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr)
        {
            return typeof(IDesignerHost).GetField(name, bindingAttr);
        }

        /// <summary>
        ///  IReflect implementation to map DesignerHost to IDesignerHost.  This helps keep us private.
        /// </summary>
        FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
        {
            return typeof(IDesignerHost).GetFields(bindingAttr);
        }

        /// <summary>
        ///  IReflect implementation to map DesignerHost to IDesignerHost.  This helps keep us private.
        /// </summary>
        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr)
        {
            return typeof(IDesignerHost).GetProperty(name, bindingAttr);
        }

        /// <summary>
        ///  IReflect implementation to map DesignerHost to IDesignerHost.  This helps keep us private.
        /// </summary>
        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return typeof(IDesignerHost).GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
        }

        /// <summary>
        ///  IReflect implementation to map DesignerHost to IDesignerHost.  This helps keep us private.
        /// </summary>
        PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
        {
            return typeof(IDesignerHost).GetProperties(bindingAttr);
        }

        /// <summary>
        ///  IReflect implementation to map DesignerHost to IDesignerHost.  This helps keep us private.
        /// </summary>
        MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
        {
            return typeof(IDesignerHost).GetMember(name, bindingAttr);
        }

        /// <summary>
        ///  IReflect implementation to map DesignerHost to IDesignerHost.  This helps keep us private.
        /// </summary>
        MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
        {
            return typeof(IDesignerHost).GetMembers(bindingAttr);
        }

        /// <summary>
        ///  IReflect implementation to map DesignerHost to IDesignerHost.  This helps keep us private.
        /// </summary>
        object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            return typeof(IDesignerHost).InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
        }

        /// <summary>
        ///  IReflect implementation to map DesignerHost to IDesignerHost.  This helps keep us private.
        /// </summary>
        Type IReflect.UnderlyingSystemType
        {
            get => typeof(IDesignerHost).UnderlyingSystemType;
        }

        /// <summary>
        ///  Adds the given service to the service container.
        /// </summary>
        void IServiceContainer.AddService(Type serviceType, object serviceInstance)
        {
            // Our service container is implemented on the parenting DesignSurface object, so we just ask for its service container and run with it.
            if (!(GetService(typeof(IServiceContainer)) is IServiceContainer sc))
            {
                throw new ObjectDisposedException("IServiceContainer");
            }
            sc.AddService(serviceType, serviceInstance);
        }

        /// <summary>
        ///  Adds the given service to the service container.
        /// </summary>
        void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
        {
            // Our service container is implemented on the parenting DesignSurface object, so we just ask for its service container and run with it.
            if (!(GetService(typeof(IServiceContainer)) is IServiceContainer sc))
            {
                throw new ObjectDisposedException("IServiceContainer");
            }
            sc.AddService(serviceType, serviceInstance, promote);
        }

        /// <summary>
        ///  Adds the given service to the service container.
        /// </summary>
        void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback)
        {
            // Our service container is implemented on the parenting DesignSurface object, so we just ask for its service container and run with it.
            if (!(GetService(typeof(IServiceContainer)) is IServiceContainer sc))
            {
                throw new ObjectDisposedException("IServiceContainer");
            }
            sc.AddService(serviceType, callback);
        }

        /// <summary>
        ///  Adds the given service to the service container.
        /// </summary>
        void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
        {
            // Our service container is implemented on the parenting DesignSurface object, so we just ask for its service container and run with it.
            if (!(GetService(typeof(IServiceContainer)) is IServiceContainer sc))
            {
                throw new ObjectDisposedException("IServiceContainer");
            }
            sc.AddService(serviceType, callback, promote);
        }

        /// <summary>
        ///  Removes the given service type from the service container.
        /// </summary>
        void IServiceContainer.RemoveService(Type serviceType)
        {
            // Our service container is implemented on the parenting DesignSurface object, so we just ask for its service container and run with it.
            if (!(GetService(typeof(IServiceContainer)) is IServiceContainer sc))
            {
                throw new ObjectDisposedException("IServiceContainer");
            }
            sc.RemoveService(serviceType);
        }

        /// <summary>
        ///  Removes the given service type from the service container.
        /// </summary>
        void IServiceContainer.RemoveService(Type serviceType, bool promote)
        {
            // Our service container is implemented on the parenting DesignSurface object, so we just ask for its service container and run with it.
            if (!(GetService(typeof(IServiceContainer)) is IServiceContainer sc))
            {
                throw new ObjectDisposedException("IServiceContainer");
            }
            sc.RemoveService(serviceType, promote);
        }

        /// <summary>
        ///  IServiceProvider implementation.  We just delegate to the  protected GetService method we are inheriting from our container.
        /// </summary>
        object IServiceProvider.GetService(Type serviceType)
        {
            return GetService(serviceType);
        }

        /// <summary>
        ///  DesignerHostTransaction is our implementation of the  DesignerTransaction abstract class.
        /// </summary>
        private sealed class DesignerHostTransaction : DesignerTransaction
        {
            private DesignerHost _host;

            public DesignerHostTransaction(DesignerHost host, string description) : base(description)
            {
                _host = host;
                if (_host._transactions is null)
                {
                    _host._transactions = new Stack();
                }
                _host._transactions.Push(this);
                _host.OnTransactionOpening(EventArgs.Empty);
                _host.OnTransactionOpened(EventArgs.Empty);
            }

            /// <summary>
            ///  User code should implement this method to perform the actual work of committing a transaction.
            /// </summary>
            protected override void OnCancel()
            {
                if (_host != null)
                {
                    if (_host._transactions.Peek() != this)
                    {
                        string nestedDescription = ((DesignerTransaction)_host._transactions.Peek()).Description;
                        throw new InvalidOperationException(string.Format(SR.DesignerHostNestedTransaction, Description, nestedDescription));
                    }
                    _host.IsClosingTransaction = true;
                    try
                    {
                        _host._transactions.Pop();
                        DesignerTransactionCloseEventArgs e = new DesignerTransactionCloseEventArgs(false, _host._transactions.Count == 0);
                        _host.OnTransactionClosing(e);
                        _host.OnTransactionClosed(e);
                    }
                    finally
                    {
                        _host.IsClosingTransaction = false;
                        _host = null;
                    }
                }
            }

            /// <summary>
            ///  User code should implement this method to perform the actual work of committing a transaction.
            /// </summary>
            protected override void OnCommit()
            {
                if (_host != null)
                {
                    if (_host._transactions.Peek() != this)
                    {
                        string nestedDescription = ((DesignerTransaction)_host._transactions.Peek()).Description;
                        throw new InvalidOperationException(string.Format(SR.DesignerHostNestedTransaction, Description, nestedDescription));
                    }

                    _host.IsClosingTransaction = true;
                    try
                    {
                        _host._transactions.Pop();
                        DesignerTransactionCloseEventArgs e = new DesignerTransactionCloseEventArgs(true, _host._transactions.Count == 0);
                        _host.OnTransactionClosing(e);
                        _host.OnTransactionClosed(e);
                    }
                    finally
                    {
                        _host.IsClosingTransaction = false;
                        _host = null;
                    }
                }
            }
        }

        /// <summary>
        ///  Site is the site we use at design time when we host components.
        /// </summary>
        internal class Site : ISite, IServiceContainer, IDictionaryService
        {
            private readonly IComponent _component;
            private Hashtable _dictionary;
            private readonly DesignerHost _host;
            private string _name;
            private bool _disposed;
            private SiteNestedContainer _nestedContainer;
            private readonly Container _container;

            internal Site(IComponent component, DesignerHost host, string name, Container container)
            {
                _component = component;
                _host = host;
                _name = name;
                _container = container;
            }

            /// <summary>
            ///  Used by the IServiceContainer implementation to return a container-specific service container.
            /// </summary>
            private IServiceContainer SiteServiceContainer
            {
                get
                {
                    SiteNestedContainer nc = ((IServiceProvider)this).GetService(typeof(INestedContainer)) as SiteNestedContainer;
                    Debug.Assert(nc != null, "We failed to resolve a nested container.");
                    IServiceContainer sc = nc.GetServiceInternal(typeof(IServiceContainer)) as IServiceContainer;
                    Debug.Assert(sc != null, "We failed to resolve a service container from the nested container.");
                    return sc;
                }
            }

            /// <summary>
            ///  Retrieves the key corresponding to the given value.
            /// </summary>
            object IDictionaryService.GetKey(object value)
            {
                if (_dictionary != null)
                {
                    foreach (DictionaryEntry de in _dictionary)
                    {
                        object o = de.Value;
                        if (value != null && value.Equals(o))
                        {
                            return de.Key;
                        }
                    }
                }
                return null;
            }

            /// <summary>
            ///  Retrieves the value corresponding to the given key.
            /// </summary>
            object IDictionaryService.GetValue(object key)
            {
                if (_dictionary != null)
                {
                    return _dictionary[key];
                }
                return null;
            }

            /// <summary>
            ///  Stores the given key-value pair in an object's site.  This key-value pair is stored on a per-object basis, and is a handy place to save additional information about a component.
            /// </summary>
            void IDictionaryService.SetValue(object key, object value)
            {
                if (_dictionary is null)
                {
                    _dictionary = new Hashtable();
                }
                if (value is null)
                {
                    _dictionary.Remove(key);
                }
                else
                {
                    _dictionary[key] = value;
                }
            }

            /// <summary>
            ///  Adds the given service to the service container.
            /// </summary>
            void IServiceContainer.AddService(Type serviceType, object serviceInstance)
            {
                SiteServiceContainer.AddService(serviceType, serviceInstance);
            }

            /// <summary>
            ///  Adds the given service to the service container.
            /// </summary>
            void IServiceContainer.AddService(Type serviceType, object serviceInstance, bool promote)
            {
                SiteServiceContainer.AddService(serviceType, serviceInstance, promote);
            }

            /// <summary>
            ///  Adds the given service to the service container.
            /// </summary>
            void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback)
            {
                SiteServiceContainer.AddService(serviceType, callback);
            }

            /// <summary>
            ///  Adds the given service to the service container.
            /// </summary>
            void IServiceContainer.AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
            {
                SiteServiceContainer.AddService(serviceType, callback, promote);
            }

            /// <summary>
            ///  Removes the given service type from the service container.
            /// </summary>
            void IServiceContainer.RemoveService(Type serviceType)
            {
                SiteServiceContainer.RemoveService(serviceType);
            }

            /// <summary>
            ///  Removes the given service type from the service container.
            /// </summary>
            void IServiceContainer.RemoveService(Type serviceType, bool promote)
            {
                SiteServiceContainer.RemoveService(serviceType, promote);
            }

            /// <summary>
            ///  Returns the requested service.
            /// </summary>
            object IServiceProvider.GetService(Type service)
            {
                if (service is null)
                {
                    throw new ArgumentNullException(nameof(service));
                }

                // We always resolve IDictionaryService to ourselves.
                if (service == typeof(IDictionaryService))
                {
                    return this;
                }

                // NestedContainer is demand created
                if (service == typeof(INestedContainer))
                {
                    if (_nestedContainer is null)
                    {
                        _nestedContainer = new SiteNestedContainer(_component, null, _host);

                        // Initialize IServiceContainer in the nested container as soon as INestedContainer is created,
                        // otherwise site has no access to the DesignerHost's services.
                        _ = _nestedContainer.GetServiceInternal(typeof(IServiceContainer));
                    }
                    return _nestedContainer;
                }

                // SiteNestedContainer does offer IServiceContainer and IContainer as services, but we always want a default site query for these services to delegate to the host.
                // Because it is more common to add  services to the host than it is to add them to the site itself, and also because we need this for backward compatibility.
                if (service != typeof(IServiceContainer) && service != typeof(IContainer) && _nestedContainer != null)
                {
                    return _nestedContainer.GetServiceInternal(service);
                }
                return _host.GetService(service);
            }

            /// <summary>
            ///  The component sited by this component site.
            /// </summary>
            IComponent ISite.Component
            {
                get => _component;
            }

            /// <summary>
            ///  The container in which the component is sited.
            /// </summary>
            IContainer ISite.Container
            {
                get => _container;
            }

            /// <summary>
            ///  Indicates whether the component is in design mode.
            /// </summary>
            bool ISite.DesignMode
            {
                get => true;
            }

            /// <summary>
            ///  Indicates whether this Site has been disposed.
            /// </summary>
            internal bool Disposed
            {
                get => _disposed;
                set
                {
                    _disposed = value;
                    //We need to do the cleanup when the site is set as disposed by its user
                    if (_disposed)
                    {
                        _dictionary = null;
                    }
                }
            }

            /// <summary>
            ///  The name of the component.
            /// </summary>
            string ISite.Name
            {
                get => _name;
                set
                {
                    if (value is null)
                    {
                        value = string.Empty;
                    }

                    if (_name != value)
                    {
                        bool validateName = true;
                        if (value.Length > 0)
                        {
                            IComponent namedComponent = _container.Components[value];
                            validateName = (_component != namedComponent);
                            // allow renames that are just case changes of the current name.
                            if (namedComponent != null && validateName)
                            {
                                Exception ex = new Exception(string.Format(SR.DesignerHostDuplicateName, value))
                                {
                                    HelpLink = SR.DesignerHostDuplicateName
                                };
                                throw ex;
                            }
                        }

                        if (validateName)
                        {
                            if (((IServiceProvider)this).GetService(typeof(INameCreationService)) is INameCreationService nameService)
                            {
                                nameService.ValidateName(value);
                            }
                        }

                        // It is OK to change the name to this value.  Announce the change and do it.
                        string oldName = _name;
                        _name = value;
                        _host.OnComponentRename(_component, oldName, _name);
                    }
                }
            }
        }
    }
}
