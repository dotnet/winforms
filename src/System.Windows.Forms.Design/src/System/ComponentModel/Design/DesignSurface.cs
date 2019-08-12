// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Reflection;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  A design surface is an object that contains multiple designers and presents a user-editable surface for them.
    /// </summary>
    public class DesignSurface : IDisposable, IServiceProvider
    {
        private readonly IServiceProvider _parentProvider;
        private ServiceContainer _serviceContainer;
        private DesignerHost _host;
        private ICollection _loadErrors;
        private bool _loaded;

        /// <summary>
        ///  Creates a new DesignSurface.
        /// </summary>
        public DesignSurface() : this((IServiceProvider)null)
        {
        }

        /// <summary>
        ///  Creates a new DesignSurface given a parent service provider.
        /// </summary>
        /// <param name="parentProvider"> The parent service provider. If there is no parent used to resolve services this can be null. </param>
        public DesignSurface(IServiceProvider parentProvider)
        {
            _parentProvider = parentProvider;
            _serviceContainer = new DesignSurfaceServiceContainer(_parentProvider);

            // Configure our default services
            ServiceCreatorCallback callback = new ServiceCreatorCallback(OnCreateService);
            ServiceContainer.AddService(typeof(ISelectionService), callback);
            ServiceContainer.AddService(typeof(IExtenderProviderService), callback);
            ServiceContainer.AddService(typeof(IExtenderListService), callback);
            ServiceContainer.AddService(typeof(ITypeDescriptorFilterService), callback);
            ServiceContainer.AddService(typeof(IReferenceService), callback);

            ServiceContainer.AddService(typeof(DesignSurface), this);
            _host = new DesignerHost(this);
        }

        /// <summary>
        ///  Creates a new DesignSurface.
        /// </summary>
        public DesignSurface(Type rootComponentType) : this(null, rootComponentType)
        {
        }

        /// <summary>
        ///  Creates a new DesignSurface given a parent service provider.
        /// </summary>
        /// <param name="parentProvider"> The parent service provider.  If there is no parent used to resolve services this can be null. </param>
        public DesignSurface(IServiceProvider parentProvider, Type rootComponentType) : this(parentProvider)
        {
            if (rootComponentType == null)
            {
                throw new ArgumentNullException(nameof(rootComponentType));
            }
            BeginLoad(rootComponentType);
        }

        /// <summary>
        ///  Provides access to the design surface's container, which contains all components currently being designed.
        /// </summary>
        public IContainer ComponentContainer
        {
            get
            {
                if (_host == null)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                return ((IDesignerHost)_host).Container;
            }
        }

        /// <summary>
        ///  Returns true if the design surface is currently loaded. This will be true when a successful load has completed, or false for all other cases.
        /// </summary>
        public bool IsLoaded
        {
            get
            {
                return _loaded;
            }
        }

        /// <summary>
        ///  Returns a collection of LoadErrors or a void collection.
        /// </summary>
        public ICollection LoadErrors
        {
            get
            {
                if (_loadErrors != null)
                {
                    return _loadErrors;
                }
                return Array.Empty<object>();
            }
        }

        /// <summary>
        ///  Returns true if DTEL (WSOD) is currently loading.
        /// </summary>
        public bool DtelLoading
        {
            get;
            set;
        }

        /// <summary>
        ///  Provides access to the design surface's ServiceContainer. This property allows inheritors to add their own services.
        /// </summary>
        protected ServiceContainer ServiceContainer
        {
            get
            {
                if (_serviceContainer == null)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                return _serviceContainer;
            }
        }

        /// <summary>
        ///  This property will return the view for the root designer. BeginLoad must have been called beforehand to start the loading process. It is possible to return a view before the designer loader finishes loading because the root designer, which supplies the view, is the first object created by the designer loader. If a view is unavailable this method will throw an exception.
        ///  Possible exceptions:
        ///  The design surface is not loading or the designer loader has not yet created a root designer: InvalidOperationException
        ///  The design surface finished the load, but failed. (Various. This will throw the first exception the designer loader added to the error collection).
        /// </summary>
        public object View
        {
            get
            {
                if (_host == null)
                {
                    throw new ObjectDisposedException(ToString());
                }

                IComponent rootComponent = ((IDesignerHost)_host).RootComponent;
                if (rootComponent == null)
                {
                    // Check to see if we have any load errors.  If so, use them.
                    if (_loadErrors != null)
                    {
                        foreach (object o in _loadErrors)
                        {
                            if (o is Exception ex)
                            {
                                throw new InvalidOperationException(ex.Message, ex);
                            }
                            else if (o != null)
                            {
                                throw new InvalidOperationException(o.ToString());
                            }
                        }
                    }
                    // loader didn't provide any help.  Just generally fail.
                    throw new InvalidOperationException(SR.DesignSurfaceNoRootComponent)
                    {
                        HelpLink = SR.DesignSurfaceNoRootComponent
                    };
                }

                if (!(((IDesignerHost)_host).GetDesigner(rootComponent) is IRootDesigner rootDesigner))
                {
                    throw new InvalidOperationException(SR.DesignSurfaceDesignerNotLoaded)
                    {
                        HelpLink = SR.DesignSurfaceDesignerNotLoaded
                    };
                }

                ViewTechnology[] designerViews = rootDesigner.SupportedTechnologies;
                if (designerViews == null || designerViews.Length == 0)
                {
                    throw new NotSupportedException(SR.DesignSurfaceNoSupportedTechnology)
                    {
                        HelpLink = SR.DesignSurfaceNoSupportedTechnology
                    };
                }

                // We just feed the available technologies back into the root designer. ViewTechnology itself is outdated.
                return rootDesigner.GetView(designerViews[0]);
            }
        }

        /// <summary>
        ///  Adds a event handler to listen to the Disposed event on the component.
        /// </summary>
        public event EventHandler Disposed;

        /// <summary>
        ///  Adds a event handler to listen to the Flushed event on the component. This is called after the design surface has asked the designer loader to flush its state.
        /// </summary>
        public event EventHandler Flushed;

        /// <summary>
        ///  Called when the designer load has completed.  This is called for successful loads as well as unsuccessful ones.  If code in this event handler throws an exception the designer will be unloaded.
        /// </summary>
        public event LoadedEventHandler Loaded;

        /// <summary>
        ///  Called when the designer load is about to begin the loading process.
        /// </summary>
        public event EventHandler Loading;

        /// <summary>
        ///  Called when the designer has completed the unloading
        ///  process.
        /// </summary>
        public event EventHandler Unloaded;

        /// <summary>
        ///  Called when a designer is about to begin reloading. When a designer reloads, all of the state for that designer is recreated, including the designer's view. The view should be unparented at this time.
        /// </summary>
        public event EventHandler Unloading;

        /// <summary>
        ///  Called when someone has called the Activate method on IDesignerHost.  You should attach a handler to this event that activates the window for this design surface.
        /// </summary>
        public event EventHandler ViewActivated;

        /// <summary>
        ///  This method begins the loading process with the given designer loader.  Designer loading can be asynchronous, so the loading may continue to  progress after this call has returned.  Listen to the Loaded event to know when the design surface has completed loading.
        /// </summary>
        public void BeginLoad(DesignerLoader loader)
        {
            if (loader == null)
            {
                throw new ArgumentNullException(nameof(loader));
            }

            if (_host == null)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            // Create the designer host.  We need the host so we can begin the loading process.
            _loadErrors = null;
            _host.BeginLoad(loader);
        }

        /// <summary>
        ///  This method begins the loading process for a component of the given type.  This will create an instance of the component type and initialize a designer for that instance.  Loaded is raised before this method returns.
        /// </summary>
        public void BeginLoad(Type rootComponentType)
        {
            if (rootComponentType == null)
            {
                throw new ArgumentNullException(nameof(rootComponentType));
            }

            if (_host == null)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }
            BeginLoad(new DefaultDesignerLoader(rootComponentType));
        }

        /// <summary>
        ///  This method is called to create a component of the given type.
        /// </summary>
        [Obsolete("CreateComponent has been replaced by CreateInstance and will be removed after Beta2")]
        protected internal virtual IComponent CreateComponent(Type componentType)
        {
            return CreateInstance(componentType) as IComponent;
        }

        /// <summary>
        ///  This method is called to create a designer for a component.
        /// </summary>
        protected internal virtual IDesigner CreateDesigner(IComponent component, bool rootDesigner)
        {
            if (component == null)
            {
                throw new ArgumentNullException(nameof(component));
            }

            if (_host == null)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            IDesigner designer;
            if (rootDesigner)
            {
                designer = TypeDescriptor.CreateDesigner(component, typeof(IRootDesigner)) as IRootDesigner;
            }
            else
            {
                designer = TypeDescriptor.CreateDesigner(component, typeof(IDesigner));
            }
            return designer;
        }

        /// <summary>
        ///  This method is called to create an instance of the given type.  If the type is a component
        ///  this will search for a constructor of type IContainer first, and then an empty constructor.
        /// </summary>
        protected internal virtual object CreateInstance(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            // Locate an appropriate constructor for IComponents.
            object instance = null;
            ConstructorInfo ctor = TypeDescriptor.GetReflectionType(type).GetConstructor(Array.Empty<Type>());
            if (ctor != null)
            {
                instance = TypeDescriptor.CreateInstance(this, type, Array.Empty<Type>(), Array.Empty<object>());
            }
            else
            {
                if (typeof(IComponent).IsAssignableFrom(type))
                {
                    ctor = TypeDescriptor.GetReflectionType(type).GetConstructor(BindingFlags.Public | BindingFlags.Instance | BindingFlags.ExactBinding, null, new Type[] { typeof(IContainer) }, null);
                }
                if (ctor != null)
                {
                    instance = TypeDescriptor.CreateInstance(this, type, new Type[] { typeof(IContainer) }, new object[] { ComponentContainer });
                }
            }

            if (instance == null)
            {
                instance = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.CreateInstance, null, null, null);
            }
            return instance;
        }

        /// <summary>
        ///  Creates a container suitable for nesting controls or components.  Adding a component to a  nested container creates its doesigner and makes it elligble for all all services available from the design surface.  Components added to nested containers do not participate in serialization. You may provide an additional name for this container by passing a value into containerName.
        /// </summary>
        public INestedContainer CreateNestedContainer(IComponent owningComponent)
        {
            return CreateNestedContainer(owningComponent, null);
        }

        /// <summary>
        ///  Creates a container suitable for nesting controls or components.  Adding a component to a  nested container creates its doesigner and makes it elligble for all all services available from the design surface.  Components added to nested containers do not participate in serialization. You may provide an additional name for this container by passing a value into containerName.
        /// </summary>
        public INestedContainer CreateNestedContainer(IComponent owningComponent, string containerName)
        {
            if (_host == null)
            {
                throw new ObjectDisposedException(GetType().FullName);
            }

            if (owningComponent == null)
            {
                throw new ArgumentNullException(nameof(owningComponent));
            }
            return new SiteNestedContainer(owningComponent, containerName, _host);
        }

        /// <summary>
        ///  Disposes the design surface.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///  Protected override of Dispose that allows for cleanup.
        /// </summary>
        /// <param name="disposing"> True if Dispose is being called or false if this is being invoked by a finalizer. </param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // technically we should raise this after we've destroyed ourselves.  Unfortunately, too many things query us for services so they can detatch.
                Disposed?.Invoke(this, EventArgs.Empty);

                // Destroying the host also destroys all components. In most cases destroying the root component will destroy its designer which also kills the view. So, we destroy the view below last (remember, this view is a "view container" so we are destroying the innermost view first and then destroying our own view).
                try
                {
                    try
                    {
                        if (_host != null)
                        {
                            _host.DisposeHost();
                        }
                    }
                    finally
                    {
                        if (_serviceContainer != null)
                        {
                            _serviceContainer.RemoveService(typeof(DesignSurface));
                            _serviceContainer.Dispose();
                        }
                    }
                }
                finally
                {
                    _host = null;
                    _serviceContainer = null;
                }
            }
        }

        /// <summary>
        ///  Flushes any design changes to the underlying loader.
        /// </summary>
        public void Flush()
        {
            if (_host != null)
            {
                _host.Flush();
            }

            Flushed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///  Retrieves a service in this design surface's service container.
        /// </summary>
        /// <param name="serviceType"> The type of service to retrieve. </param>
        /// <returns> An instance of the requested service or null if the service could not be found. </returns>
        public object GetService(Type serviceType)
        {
            if (_serviceContainer != null)
            {
                return _serviceContainer.GetService(serviceType);
            }
            return null;
        }

        /// <summary>
        ///  Called by the designer host in response to an Activate call on its interface.
        /// </summary>
        internal void OnViewActivate()
        {
            OnViewActivate(EventArgs.Empty);
        }

        /// <summary>
        ///  Private method that demand-creates services we offer.
        /// </summary>
        /// <param name="container"> The service container requesting the service. </param>
        /// <param name="serviceType"> The type of service being requested. </param>
        /// <returns> A new instance of the service.  It is an error to call this with a service type it doesn't know how to create </returns>
        private object OnCreateService(IServiceContainer container, Type serviceType)
        {
            if (serviceType == typeof(ISelectionService))
            {
                return new SelectionService(container);
            }

            if (serviceType == typeof(IExtenderProviderService))
            {
                return new ExtenderProviderService();
            }

            if (serviceType == typeof(IExtenderListService))
            {
                return GetService(typeof(IExtenderProviderService));
            }

            if (serviceType == typeof(ITypeDescriptorFilterService))
            {
                return new TypeDescriptorFilterService();
            }

            Debug.Assert(serviceType == typeof(IReferenceService), "Demand created service not supported: " + serviceType.Name);
            return new ReferenceService(container);
        }

        /// <summary>
        ///  This is invoked by the designer host when it has finished the load.
        /// </summary>
        internal void OnLoaded(bool successful, ICollection errors)
        {
            _loaded = successful;
            _loadErrors = errors;

            if (successful)
            {
                IComponent rootComponent = ((IDesignerHost)_host).RootComponent;
                if (rootComponent == null)
                {
                    ArrayList newErrors = new ArrayList();
                    Exception ex = new InvalidOperationException(SR.DesignSurfaceNoRootComponent)
                    {
                        HelpLink = SR.DesignSurfaceNoRootComponent
                    };
                    newErrors.Add(ex);
                    if (errors != null)
                    {
                        newErrors.AddRange(errors);
                    }
                    errors = newErrors;
                    successful = false;
                }
            }
            OnLoaded(new LoadedEventArgs(successful, errors));
        }

        /// <summary>
        ///  Called when the loading process has completed.  This is invoked for both successful and unsuccessful loads. The EventArgs passed into this method can be used to tell a successful from an unsuccessful load.  It can also be used to create a view for this design surface.  If code in this event handler or override throws an exception,
        ///  the designer will be unloaded.
        /// </summary>
        protected virtual void OnLoaded(LoadedEventArgs e)
        {
            Loaded?.Invoke(this, e);
        }

        /// <summary>
        ///  Called when the loading process is about to begin.
        /// </summary>
        internal void OnLoading()
        {
            OnLoading(EventArgs.Empty);
        }

        /// <summary>
        ///  Called when the loading process is about to begin.
        /// </summary>
        protected virtual void OnLoading(EventArgs e)
        {
            Loading?.Invoke(this, e);
        }

        /// <summary>
        ///  This is invoked by the designer host after it has unloaded a document.
        /// </summary>
        internal void OnUnloaded()
        {
            OnUnloaded(EventArgs.Empty);
        }

        /// <summary>
        ///  Called when a designer has finished unloading a document.
        /// </summary>
        protected virtual void OnUnloaded(EventArgs e)
        {
            Unloaded?.Invoke(this, e);
        }

        /// <summary>
        ///  This is invoked by the designer host when it is about to unload a document.
        /// </summary>
        internal void OnUnloading()
        {
            OnUnloading(EventArgs.Empty);
            _loaded = false;
        }

        /// <summary>
        ///  Called when a designer is about to begin reloading. When a designer reloads, all of the state for that designer is recreated, including the designer's view. The view should be unparented at this time.
        /// </summary>
        protected virtual void OnUnloading(EventArgs e)
        {
            Unloading?.Invoke(this, e);
        }

        /// <summary>
        ///  Called when someone has called the Activate method on IDesignerHost.  You should attach a handler to this event that activates the window for this design surface.
        /// </summary>
        protected virtual void OnViewActivate(EventArgs e)
        {
            ViewActivated?.Invoke(this, e);
        }

        /// <summary>
        ///  This is a simple designer loader that creates an instance of the given type and then calls EndLoad.  If a collection of objects was passed, this will simply add those objects to the container.
        /// </summary>
        private class DefaultDesignerLoader : DesignerLoader
        {
            private readonly Type _type;

            public DefaultDesignerLoader(Type type)
            {
                _type = type;
            }

            public override void BeginLoad(IDesignerLoaderHost loaderHost)
            {
                loaderHost.CreateComponent(_type);
                loaderHost.EndLoad(_type.FullName, true, null);
            }
            public override void Dispose()
            {
            }
        }
    }
}
