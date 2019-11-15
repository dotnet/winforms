// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  A service container that supports "fixed" services.  Fixed 
    ///  services cannot be removed.
    /// </summary>
    public class DesignSurfaceManager : IServiceProvider, IDisposable
    {
        private readonly IServiceProvider _parentProvider;
        private ServiceContainer _serviceContainer;
        private ActiveDesignSurfaceChangedEventHandler _activeDesignSurfaceChanged;
        private DesignSurfaceEventHandler _designSurfaceCreated;
        private DesignSurfaceEventHandler _designSurfaceDisposed;
        private EventHandler _selectionChanged;

        /// <summary>
        ///  Creates a new designer application.
        /// </summary>
        public DesignSurfaceManager()
            : this(null)
        {
        }

        /// <summary>
        ///  Creates a new designer application and provides a
        ///  parent service provider.
        /// </summary>
        public DesignSurfaceManager(IServiceProvider parentProvider)
        {
            _parentProvider = parentProvider;

            ServiceCreatorCallback callback = new ServiceCreatorCallback(OnCreateService);
            ServiceContainer.AddService(typeof(IDesignerEventService), callback);
        }

        /// <summary>
        ///  This property should be set by the designer's user interface
        ///  whenever a designer becomes the active window.  The default
        ///  implementation of this property works with the default
        ///  implementation of IDesignerEventService to notify interested
        ///  parties that a new designer is now active.  If you provide
        ///  your own implementation of IDesignerEventService you should
        ///  override this property to notify your service appropriately.
        /// </summary>
        public virtual DesignSurface ActiveDesignSurface
        {
            get
            {
                IDesignerEventService eventService = EventService;
                if (eventService == null)
                {
                    return null;
                }

                IDesignerHost activeHost = eventService.ActiveDesigner;
                if (activeHost == null)
                {
                    return null;
                }

                return activeHost.GetService(typeof(DesignSurface)) as DesignSurface;
            }
            set
            {
                // If we are providing IDesignerEventService, then we are responsible for 
                // notifying it of new designers coming into place.  If we aren't 
                // the ones providing the event service, then whoever is providing 
                // it will be responsible for updating it when new designers
                // are created.
                if (EventService is DesignerEventService eventService)
                {
                    eventService.OnActivateDesigner(value);
                }
            }
        }

        /// <summary>
        ///  A collection of design surfaces.  This is offered
        ///  for convience, and simply maps to IDesignerEventService.
        /// </summary>
        public DesignSurfaceCollection DesignSurfaces
        {
            get
            {
                IDesignerEventService eventService = EventService;
                if (eventService != null)
                {
                    return new DesignSurfaceCollection(eventService.Designers);
                }

                return new DesignSurfaceCollection(null);
            }
        }

        /// <summary>
        ///  We access this a lot.
        /// </summary>
        private IDesignerEventService EventService => GetService(typeof(IDesignerEventService)) as IDesignerEventService;

        /// <summary>
        ///  Provides access to the designer application's 
        ///  ServiceContainer. This property allows 
        ///  inheritors to add their own services.
        /// </summary>
        protected ServiceContainer ServiceContainer
        {
            get
            {
                if (_serviceContainer == null)
                {
                    _serviceContainer = new ServiceContainer(_parentProvider);
                }

                return _serviceContainer;
            }
        }

        /// <summary>
        ///  This event is raised when a new design surface gains
        ///  activation.  This is mapped through IDesignerEventService.
        /// </summary>
        public event ActiveDesignSurfaceChangedEventHandler ActiveDesignSurfaceChanged
        {
            add
            {
                if (_activeDesignSurfaceChanged == null)
                {
                    IDesignerEventService eventService = EventService;
                    if (eventService != null)
                    {
                        eventService.ActiveDesignerChanged += new ActiveDesignerEventHandler(OnActiveDesignerChanged);
                    }
                }

                _activeDesignSurfaceChanged += value;
            }
            remove
            {
                _activeDesignSurfaceChanged -= value;
                if (_activeDesignSurfaceChanged != null)
                {
                    return;
                }

                IDesignerEventService eventService = EventService;
                if (eventService != null)
                {
                    eventService.ActiveDesignerChanged -= new ActiveDesignerEventHandler(OnActiveDesignerChanged);
                }
            }
        }

        /// <summary>
        ///  This event is raised when a new design surface is
        ///  created.  This is mapped through IDesignerEventService.
        /// </summary>
        public event DesignSurfaceEventHandler DesignSurfaceCreated
        {
            add
            {
                if (_designSurfaceCreated == null)
                {
                    IDesignerEventService eventService = EventService;
                    if (eventService != null)
                    {
                        eventService.DesignerCreated += new DesignerEventHandler(OnDesignerCreated);
                    }
                }

                _designSurfaceCreated += value;
            }
            remove
            {
                _designSurfaceCreated -= value;
                if (_designSurfaceCreated != null)
                {
                    return;
                }

                IDesignerEventService eventService = EventService;
                if (eventService != null)
                {
                    eventService.DesignerCreated -= new DesignerEventHandler(OnDesignerCreated);
                }
            }
        }

        /// <summary>
        ///  This event is raised when a design surface is disposed.
        ///  This is mapped through IDesignerEventService.
        /// </summary>
        public event DesignSurfaceEventHandler DesignSurfaceDisposed
        {
            add
            {
                if (_designSurfaceDisposed == null)
                {
                    IDesignerEventService eventService = EventService;
                    if (eventService != null)
                    {
                        eventService.DesignerDisposed += new DesignerEventHandler(OnDesignerDisposed);
                    }
                }

                _designSurfaceDisposed += value;
            }
            remove
            {
                _designSurfaceDisposed -= value;
                if (_designSurfaceDisposed != null)
                {
                    return;
                }

                IDesignerEventService eventService = EventService;
                if (eventService != null)
                {
                    eventService.DesignerDisposed -= new DesignerEventHandler(OnDesignerDisposed);
                }
            }
        }

        /// <summary>
        ///  This event is raised when the active designer's
        ///  selection of component set changes.  This is mapped
        ///  through IDesignerEventService.
        /// </summary>
        public event EventHandler SelectionChanged
        {
            add
            {
                if (_selectionChanged == null)
                {
                    IDesignerEventService eventService = EventService;
                    if (eventService != null)
                    {
                        eventService.SelectionChanged += new EventHandler(OnSelectionChanged);
                    }
                }

                _selectionChanged += value;
            }
            remove
            {
                _selectionChanged -= value;
                if (_selectionChanged != null)
                {
                    return;
                }

                IDesignerEventService eventService = EventService;
                if (eventService != null)
                {
                    eventService.SelectionChanged -= new EventHandler(OnSelectionChanged);
                }
            }
        }

        /// <summary>
        ///  Public method to create a design surface.
        /// </summary>
        public DesignSurface CreateDesignSurface()
        {
            DesignSurface surface = CreateDesignSurfaceCore(this);

            // If we are providing IDesignerEventService, then we are responsible for 
            // notifying it of new designers coming into place.  If we aren't 
            // the ones providing the event service, then whoever is providing 
            // it will be responsible for updating it when new designers are created.
            if (GetService(typeof(IDesignerEventService)) is DesignerEventService eventService)
            {
                eventService.OnCreateDesigner(surface);
            }

            return surface;
        }

        /// <summary>
        ///  Public method to create a design surface.  This method
        ///  takes an additional service provider.  This service 
        ///  provider will be combined with the service provider 
        ///  already contained within DesignSurfaceManager.  Service
        ///  requests will go to this provider first, and then bubble
        ///  up to the service provider owned by DesignSurfaceManager.
        ///  This allows for services to be tailored for each design surface.
        /// </summary>
        public DesignSurface CreateDesignSurface(IServiceProvider parentProvider)
        {
            if (parentProvider == null)
            {
                throw new ArgumentNullException(nameof(parentProvider));
            }

            IServiceProvider mergedProvider = new MergedServiceProvider(parentProvider, this);

            DesignSurface surface = CreateDesignSurfaceCore(mergedProvider);

            // If we are providing IDesignerEventService, then we are responsible for 
            // notifying it of new designers coming into place.  If we aren't 
            // the ones providing the event service, then whoever is providing 
            // it will be responsible for updating it when new designers are created.
            DesignerEventService eventService = GetService(typeof(IDesignerEventService)) as DesignerEventService;
            if (eventService != null)
            {
                eventService.OnCreateDesigner(surface);
            }

            return surface;
        }

        /// <summary>
        ///  Creates an instance of a design surface.  This can be 
        ///  overridden to provide a derived version of DesignSurface.
        /// </summary>
        protected virtual DesignSurface CreateDesignSurfaceCore(IServiceProvider parentProvider) => new DesignSurface(parentProvider);

        /// <summary>
        ///  Disposes the designer application.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        ///  Protected override of Dispose that allows for cleanup.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _serviceContainer == null)
            {
                return;
            }

            _serviceContainer.Dispose();
            _serviceContainer = null;
        }

        /// <summary>
        ///  Retrieves a service in this design surface's service
        ///  container.
        /// </summary>
        public object GetService(Type serviceType) => _serviceContainer?.GetService(serviceType);

        /// <summary>
        ///  Private method that demand-creates services we offer.
        /// </summary>
        /// <param name="container">
        ///  The service container requesting the service.
        /// </param>
        /// <param name="serviceType">
        ///  The type of service being requested.
        /// </param>
        /// <returns>
        ///  A new instance of the service.  It is an error to call this with
        ///  a service type it doesn't know how to create
        /// </returns>
        private object OnCreateService(IServiceContainer container, Type serviceType)
        {

            if (serviceType == typeof(IDesignerEventService))
            {
                return new DesignerEventService();
            }

            Debug.Fail("Demand created service not supported: " + serviceType.Name);
            return null;
        }

        /// <summary>
        ///  Handles the IDesignerEventService event and relays it to
        ///  DesignSurfaceManager's similar event.
        /// </summary>
        private void OnActiveDesignerChanged(object sender, ActiveDesignerEventArgs e)
        {
            Debug.Assert(_activeDesignSurfaceChanged != null, "Should have detached this event handler.");
            if (_activeDesignSurfaceChanged != null)
            {

                DesignSurface newSurface = null;
                DesignSurface oldSurface = null;

                if (e.OldDesigner != null)
                {
                    oldSurface = e.OldDesigner.GetService(typeof(DesignSurface)) as DesignSurface;
                }

                if (e.NewDesigner != null)
                {
                    newSurface = e.NewDesigner.GetService(typeof(DesignSurface)) as DesignSurface;
                }

                _activeDesignSurfaceChanged(this, new ActiveDesignSurfaceChangedEventArgs(oldSurface, newSurface));
            }
        }

        /// <summary>
        ///  Handles the IDesignerEventService event and relays it to
        ///  DesignSurfaceManager's similar event.
        /// </summary>
        private void OnDesignerCreated(object sender, DesignerEventArgs e)
        {
            Debug.Assert(_designSurfaceCreated != null, "Should have detached this event handler.");
            if (_designSurfaceCreated == null)
            {
                return;
            }

            if (e.Designer.GetService(typeof(DesignSurface)) is DesignSurface surface)
            {
                _designSurfaceCreated(this, new DesignSurfaceEventArgs(surface));
            }
        }

        /// <summary>
        ///  Handles the IDesignerEventService event and relays it to
        ///  DesignSurfaceManager's similar event.
        /// </summary>
        private void OnDesignerDisposed(object sender, DesignerEventArgs e)
        {
            Debug.Assert(_designSurfaceDisposed != null, "Should have detached this event handler.");
            if (_designSurfaceDisposed == null)
            {
                return;
            }

            if (e.Designer.GetService(typeof(DesignSurface)) is DesignSurface surface)
            {
                _designSurfaceDisposed(this, new DesignSurfaceEventArgs(surface));
            }
        }

        /// <summary>
        ///  Handles the IDesignerEventService event and relays it to
        ///  DesignSurfaceManager's similar event.
        /// </summary>
        private void OnSelectionChanged(object sender, EventArgs e)
        {
            Debug.Assert(_selectionChanged != null, "Should have detached this event handler.");
            _selectionChanged?.Invoke(this, e);
        }

        /// <summary>
        ///  Simple service provider that merges two providers together.  
        /// </summary>
        private sealed class MergedServiceProvider : IServiceProvider
        {
            private IServiceProvider _primaryProvider;
            private IServiceProvider _secondaryProvider;

            internal MergedServiceProvider(IServiceProvider primaryProvider, IServiceProvider secondaryProvider)
            {
                _primaryProvider = primaryProvider;
                _secondaryProvider = secondaryProvider;
            }

            object IServiceProvider.GetService(Type serviceType)
            {
                if (serviceType == null)
                {
                    throw new ArgumentNullException(nameof(serviceType));
                }

                object service = _primaryProvider.GetService(serviceType);

                if (service == null)
                {
                    service = _secondaryProvider.GetService(serviceType);
                }

                return service;
            }
        }
    }
}
