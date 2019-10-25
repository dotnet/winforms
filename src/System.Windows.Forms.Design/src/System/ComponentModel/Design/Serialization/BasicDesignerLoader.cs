// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  This is a class that derives from DesignerLoader but provides some default functionality.  
    ///  This class tracks changes from the loader host and sets its "Modified" bit to true when a 
    ///  change occurs.  Also, this class implements IDesignerLoaderService to support multiple 
    ///  load dependencies.  To use BaseDesignerLoader, you need to implement the PerformLoad 
    ///  and PerformFlush methods.
    /// </summary>
    public abstract partial class BasicDesignerLoader : DesignerLoader, IDesignerLoaderService
    {
        private static readonly int s_stateLoaded = BitVector32.CreateMask();                                       // Have we loaded, or tried to load, the document?
        private static readonly int s_stateLoadFailed = BitVector32.CreateMask(s_stateLoaded);                      // True if we loaded, but had a fatal error.
        private static readonly int s_stateFlushInProgress = BitVector32.CreateMask(s_stateLoadFailed);             // True if we are in the process of flushing code.
        private static readonly int s_stateModified = BitVector32.CreateMask(s_stateFlushInProgress);               // True if the designer is modified.
        private static readonly int s_stateReloadSupported = BitVector32.CreateMask(s_stateModified);               // True if the serializer supports reload.
        private static readonly int s_stateActiveDocument = BitVector32.CreateMask(s_stateReloadSupported);         // Is this the currently active document?
        private static readonly int s_stateDeferredReload = BitVector32.CreateMask(s_stateActiveDocument);          // Set to true if a reload was requested but we aren't the active doc.
        private static readonly int s_stateReloadAtIdle = BitVector32.CreateMask(s_stateDeferredReload);            // Set if we are waiting to reload at idle.  Prevents multiple idle event handlers.
        private static readonly int s_stateForceReload = BitVector32.CreateMask(s_stateReloadAtIdle);               // True if we should always reload, False if we should check the code dom for changes first.
        private static readonly int s_stateFlushReload = BitVector32.CreateMask(s_stateForceReload);                // True if we should flush before reloading.
        private static readonly int s_stateModifyIfErrors = BitVector32.CreateMask(s_stateFlushReload);             // True if we we should modify the buffer if we have fatal errors after load.
        private static readonly int s_stateEnableComponentEvents = BitVector32.CreateMask(s_stateModifyIfErrors);   // True if we are currently listening to OnComponent* events

        // State for the designer loader.
        private BitVector32 _state = new BitVector32();
        private IDesignerLoaderHost _host;
        private int _loadDependencyCount;
        private string _baseComponentClassName;
        private bool _hostInitialized;
        private bool _loading;

        // State for serialization.
        private DesignerSerializationManager _serializationManager;
        private IDisposable _serializationSession;

        /// <summary>
        ///  Creates a new BasicDesignerLoader
        /// </summary>
        protected BasicDesignerLoader()
        {
            _state[s_stateFlushInProgress] = false;
            _state[s_stateReloadSupported] = true;
            _state[s_stateEnableComponentEvents] = false;
            _hostInitialized = false;
            _loading = false;
        }

        /// <summary>
        ///  This protected property indicates if there have been any
        ///  changes made to the design surface.  The Flush method 
        ///  gets the value of this property to determine if it needs
        ///  to generate a code dom tree.  This property is set by
        ///  the designer loader when it detects a change to the 
        ///  design surface.  You can override this to perform
        ///  additional work, such as checking out a file from source
        ///  code control.
        /// </summary>
        protected virtual bool Modified
        {
            get => _state[s_stateModified];
            set => _state[s_stateModified] = value;
        }

        /// <summary>
        ///  Returns the loader host that was given to this designer loader. This can be null if BeginLoad has not
        ///  been called yet, or if this designer loader has been disposed.
        /// </summary>
        protected IDesignerLoaderHost LoaderHost
        {
            get
            {
                if (_host != null)
                {
                    return _host;
                }

                if (_hostInitialized)
                {
                    throw new ObjectDisposedException(GetType().Name);
                }

                throw new InvalidOperationException(SR.BasicDesignerLoaderNotInitialized);
            }
        }

        /// <summary>
        ///  Returns true when the designer is in the process of loading.  
        ///  Clients that are sinking notifications from the designer often 
        ///  want to ignore them while the desingner is loading
        ///  and only respond to them if they result from user interatcions.
        /// </summary>
        public override bool Loading => _loadDependencyCount > 0 || _loading;

        /// <summary>
        ///  Provides an object whose public properties will be made available to the designer serialization manager's
        ///  Properties property.  The default value of this property is null.
        /// </summary>
        protected object PropertyProvider
        {
            get
            {
                if (_serializationManager == null)
                {
                    throw new InvalidOperationException(SR.BasicDesignerLoaderNotInitialized);
                }

                return _serializationManager.PropertyProvider;
            }
            set
            {
                if (_serializationManager == null)
                {
                    throw new InvalidOperationException(SR.BasicDesignerLoaderNotInitialized);
                }

                _serializationManager.PropertyProvider = value;
            }
        }

        /// <summary>
        ///  Calling Reload doesn't actually perform a reload immediately - it just schedules an asynchronous
        ///  reload. This property is used to determine if there is currently a reload pending.
        /// </summary>
        protected bool ReloadPending => _state[s_stateReloadAtIdle];

        /// <summary>
        ///  Called by the designer host to begin the loading process.  
        ///  The designer host passes in an instance of a designer loader 
        ///  host.  This loader host allows the designer loader to reload 
        ///  the design document and also allows the designer loader to indicate
        ///  that it has finished loading the design document.
        /// </summary>
        public override void BeginLoad(IDesignerLoaderHost host)
        {
            if (host == null)
            {
                throw new ArgumentNullException(nameof(host));
            }

            if (_state[s_stateLoaded])
            {
                Exception ex = new InvalidOperationException(SR.BasicDesignerLoaderAlreadyLoaded);
                ex.HelpLink = SR.BasicDesignerLoaderAlreadyLoaded;

                throw ex;
            }

            if (_host != null && _host != host)
            {
                Exception ex = new InvalidOperationException(SR.BasicDesignerLoaderDifferentHost);
                ex.HelpLink = SR.BasicDesignerLoaderDifferentHost;

                throw ex;
            }

            _state[s_stateLoaded | s_stateLoadFailed] = false;
            _loadDependencyCount = 0;

            if (_host == null)
            {
                _host = host;
                _hostInitialized = true;
                _serializationManager = new DesignerSerializationManager(_host);

                // Add our services.  We do IDesignerSerializationManager separate because
                // it is not something the user can replace.
                DesignSurfaceServiceContainer dsc = GetService(typeof(DesignSurfaceServiceContainer)) as DesignSurfaceServiceContainer;

                if (dsc != null)
                {
                    dsc.AddFixedService(typeof(IDesignerSerializationManager), _serializationManager);
                }
                else
                {
                    IServiceContainer sc = GetService(typeof(IServiceContainer)) as IServiceContainer;

                    if (sc == null)
                    {
                        ThrowMissingService(typeof(IServiceContainer));
                    }

                    sc.AddService(typeof(IDesignerSerializationManager), _serializationManager);
                }

                Initialize();
                host.Activated += new EventHandler(OnDesignerActivate);
                host.Deactivated += new EventHandler(OnDesignerDeactivate);
            }

            // Now that we're initialized, let's begin the load.  We assume 
            // we support reload until the codeLoader tells us we
            // can't.  That way, we will do the reload if we didn't get a
            // valid loader to start with.
            //
            // StartTimingMark();
            bool successful = true;
            ArrayList localErrorList = null;
            IDesignerLoaderService ls = GetService(typeof(IDesignerLoaderService)) as IDesignerLoaderService;

            try
            {
                if (ls != null)
                {
                    ls.AddLoadDependency();
                }
                else
                {
                    _loading = true;
                    OnBeginLoad();
                }

                PerformLoad(_serializationManager);
            }
            catch (Exception e)
            {
                while (e is TargetInvocationException)
                {
                    e = e.InnerException;
                }

                localErrorList = new ArrayList();
                localErrorList.Add(e);
                successful = false;
            }

            if (ls != null)
            {
                ls.DependentLoadComplete(successful, localErrorList);
            }
            else
            {
                OnEndLoad(successful, localErrorList);
                _loading = false;
            }
        }

        /// <summary>
        ///  Disposes this designer loader.  The designer host will call 
        ///  this method when the design document itself is being destroyed.  
        ///  Once called, the designer loader will never be called again.
        ///  This implementation removes any previously added services.  It
        ///  does not flush changes, which allows for fast teardown of a 
        ///  designer that wasn't saved.
        /// </summary>
        public override void Dispose()
        {
            if (_state[s_stateReloadAtIdle])
            {
                Application.Idle -= new EventHandler(OnIdle);
            }

            UnloadDocument();
            IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            if (cs != null)
            {
                cs.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
                cs.ComponentAdding -= new ComponentEventHandler(OnComponentAdding);
                cs.ComponentRemoving -= new ComponentEventHandler(OnComponentRemoving);
                cs.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
                cs.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
                cs.ComponentChanging -= new ComponentChangingEventHandler(OnComponentChanging);
                cs.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
            }

            if (_host != null)
            {
                _host.RemoveService(typeof(IDesignerLoaderService));
                _host.Activated -= new EventHandler(OnDesignerActivate);
                _host.Deactivated -= new EventHandler(OnDesignerDeactivate);
                _host = null;
            }
        }

        /// <summary>
        ///  The designer host will call this periodically when it wants to
        ///  ensure that any changes that have been made to the document
        ///  have been saved by the designer loader.  This method allows
        ///  designer loaders to implement a lazy-write scheme to improve
        ///  performance.  This designer loader implements lazy writes by
        ///  listening to component change events.  If a component has 
        ///  changed it sets a "modified" bit.  When Flush is called the
        ///  loader will write out a new code dom tree.
        /// </summary>
        public override void Flush()
        {
            if (_state[s_stateFlushInProgress] || !_state[s_stateLoaded] || !Modified)
            {
                return;
            }

            _state[s_stateFlushInProgress] = true;
            Cursor oldCursor = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                IDesignerLoaderHost host = _host;
                Debug.Assert(host != null, "designer loader was asked to flush after it has been disposed.");

                // If the host has a null root component, it probably failed
                // its last load.  In that case, there is nothing to flush.
                bool shouldChangeModified = true;

                if (host != null && host.RootComponent != null)
                {
                    using (_serializationManager.CreateSession())
                    {
                        try
                        {
                            PerformFlush(_serializationManager);
                        }
                        catch (CheckoutException)
                        {
                            shouldChangeModified = false; // don't need to report that one it already has shown an error message
                            throw;
                        }
                        catch (Exception ex)
                        {
                            _serializationManager.Errors.Add(ex);
                        }

                        ICollection errors = _serializationManager.Errors;

                        if (errors != null && errors.Count > 0)
                        {
                            ReportFlushErrors(errors);
                        }
                    }
                }

                if (shouldChangeModified)
                {
                    Modified = false;
                }
            }
            finally
            {
                _state[s_stateFlushInProgress] = false;
                Cursor.Current = oldCursor;
            }
        }

        /// <summary>
        ///  Helper method that gives access to the service provider.
        /// </summary>
        protected object GetService(Type serviceType)
        {
            object service = null;

            if (_host != null)
            {
                service = _host.GetService(serviceType);
            }

            return service;
        }

        /// <summary>
        ///  This method is called immediately after the first time
        ///  BeginLoad is invoked.  This is an appopriate place to
        ///  add custom services to the loader host.  Remember to
        ///  remove any custom services you add here by overriding
        ///  Dispose.
        /// </summary>
        protected virtual void Initialize() => LoaderHost.AddService(typeof(IDesignerLoaderService), this);

        /// <summary>
        ///  This method an be overridden to provide some intelligent
        ///  logic to determine if a reload is required.  This method is
        ///  called when someone requests a reload but doesn't force
        ///  the reload.  It gives the loader an opportunity to scan
        ///  the underlying storage to determine if a reload is acutually
        ///  needed.  The default implementation of this method always
        ///  returns true.
        /// </summary>
        protected virtual bool IsReloadNeeded() => true;

        /// <summary>
        ///  This method should be called by the designer loader service
        ///  when the first dependent load has started.  This initializes
        ///  the state of the code dom loader and prepares it for loading.
        ///  By default, the designer loader provides
        ///  IDesignerLoaderService itself, so this is called automatically.
        ///  If you provide your own loader service, or if you choose not
        ///  to provide a loader service, you are responsible for calling
        ///  this method.  BeginLoad will automatically call this, either
        ///  indirectly by calling AddLoadDependency if IDesignerLoaderService
        ///  is available, or directly if it is not.
        /// </summary>
        protected virtual void OnBeginLoad()
        {
            _serializationSession = _serializationManager.CreateSession();
            _state[s_stateLoaded] = false;

            // Make sure that we're removed any event sinks we added after we finished the load.
            // Make sure that we're removed any event sinks we added after we finished the load.
            EnableComponentNotification(false);
            IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));

            if (cs == null)
            {
                return;
            }

            cs.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
            cs.ComponentAdding -= new ComponentEventHandler(OnComponentAdding);
            cs.ComponentRemoving -= new ComponentEventHandler(OnComponentRemoving);
            cs.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
            cs.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
            cs.ComponentChanging -= new ComponentChangingEventHandler(OnComponentChanging);
            cs.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
        }

        /// <summary>
        /// This method can be used to Enable or Disable component notification by the DesignerLoader.
        /// </summary>
        protected virtual bool EnableComponentNotification(bool enable)
        {
            bool previouslyEnabled = _state[s_stateEnableComponentEvents];

            if (!previouslyEnabled && enable)
            {
                _state[s_stateEnableComponentEvents] = true;
            }
            else if (previouslyEnabled && !enable)
            {
                _state[s_stateEnableComponentEvents] = false;
            }

            return previouslyEnabled;
        }

        /// <summary>
        ///  This method is called immediately before the document is unloaded.
        ///  The document may be unloaded in preparation for reload, or 
        ///  if the document failed the load.  If you added document-specific
        ///  services in OnBeginLoad or OnEndLoad, you should remove them
        ///  here.
        /// </summary>
        protected virtual void OnBeginUnload()
        { }

        /// <summary>
        ///  This is called whenever a new component is added to the design surface.
        /// </summary>
        private void OnComponentAdded(object sender, ComponentEventArgs e)
        {
            // We check the loader host here.  We do not actually listen to
            // this event until the loader has finished loading but if we
            // succeeded the load and the loader then failed later, we might
            // be listening when asked to unload.
            if (_state[s_stateEnableComponentEvents] && !LoaderHost.Loading)
            {
                Modified = true;
            }
        }

        /// <summary>
        ///  This is called right before a component is added to the design surface.
        /// </summary>
        private void OnComponentAdding(object sender, ComponentEventArgs e)
        {
            // We check the loader host here.  We do not actually listen to
            // this event until the loader has finished loading but if we
            // succeeded the load and the loader then failed later, we might
            // be listening when asked to unload.
            if (_state[s_stateEnableComponentEvents] && !LoaderHost.Loading)
            {
                OnModifying();
            }
        }

        /// <summary>
        ///  This is called whenever a component on the design surface changes.
        /// </summary>
        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            // We check the loader host here.  We do not actually listen to
            // this event until the loader has finished loading but if we
            // succeeded the load and the loader then failed later, we might
            // be listening when asked to unload.
            if (_state[s_stateEnableComponentEvents] && !LoaderHost.Loading)
            {
                Modified = true;
            }
        }

        /// <summary>
        ///  This is called right before a component on the design surface changes.
        /// </summary>
        private void OnComponentChanging(object sender, ComponentChangingEventArgs e)
        {
            // We check the loader host here.  We do not actually listen to
            // this event until the loader has finished loading but if we
            // succeeded the load and the loader then failed later, we might
            // be listening when asked to unload.
            if (_state[s_stateEnableComponentEvents] && !LoaderHost.Loading)
            {
                OnModifying();
            }
        }

        /// <summary>
        ///  This is called whenever a component is removed from the design surface.
        /// </summary>
        private void OnComponentRemoved(object sender, ComponentEventArgs e)
        {
            // We check the loader host here.  We do not actually listen to
            // this event until the loader has finished loading but if we
            // succeeded the load and the loader then failed later, we might
            // be listening when asked to unload.
            if (_state[s_stateEnableComponentEvents] && !LoaderHost.Loading)
            {
                Modified = true;
            }
        }

        /// <summary>
        ///  This is called right before a component is removed from the design surface.
        /// </summary>
        private void OnComponentRemoving(object sender, ComponentEventArgs e)
        {
            // We check the loader host here.  We do not actually listen to
            // this event until the loader has finished loading but if we
            // succeeded the load and the loader then failed later, we might
            // be listening when asked to unload.
            if (_state[s_stateEnableComponentEvents] && !LoaderHost.Loading)
            {
                OnModifying();
            }
        }

        /// <summary>
        ///  Raised by the host when a component is renamed.  Here we modify ourselves
        ///  and then whack the component declaration.  At the next code gen
        ///  cycle we will recreate the declaration.
        /// </summary>
        private void OnComponentRename(object sender, ComponentRenameEventArgs e)
        {
            // We check the loader host here.  We do not actually listen to
            // this event until the loader has finished loading but if we
            // succeeded the load and the loader then failed later, we might
            // be listening when asked to unload.
            if (_state[s_stateEnableComponentEvents] && !LoaderHost.Loading)
            {
                OnModifying();
                Modified = true;
            }
        }

        /// <summary>
        ///  Called when this document becomes active.  here we check to see if
        ///  someone else has modified the contents of our buffer.  If so, we
        ///  ask the designer to reload.
        /// </summary>
        private void OnDesignerActivate(object sender, EventArgs e)
        {
            _state[s_stateActiveDocument] = true;

            if (!_state[s_stateDeferredReload] || _host == null)
            {
                return;
            }

            _state[s_stateDeferredReload] = false;
            ReloadOptions flags = ReloadOptions.Default;

            if (_state[s_stateForceReload])
            {
                flags |= ReloadOptions.Force;
            }

            if (!_state[s_stateFlushReload])
            {
                flags |= ReloadOptions.NoFlush;
            }

            if (_state[s_stateModifyIfErrors])
            {
                flags |= ReloadOptions.ModifyOnError;
            }

            Reload(flags);
        }

        /// <summary>
        ///  Called when this document loses activation.  We just remember this
        ///  for later.
        /// </summary>
        private void OnDesignerDeactivate(object sender, EventArgs e) => _state[s_stateActiveDocument] = false;

        /// <summary>
        ///  This method should be called by the designer loader service
        ///  when all dependent loads have been completed.  This
        ///  "shuts down" the loading process that was initiated by
        ///  BeginLoad.  By default, the designer loader provides
        ///  IDesignerLoaderService itself, so this is called automatically.
        ///  If you provide your own loader service, or if you choose not
        ///  to provide a loader service, you are responsible for calling
        ///  this method.  BeginLoad will automatically call this, either
        ///  indirectly by calling DependentLoadComplete if IDesignerLoaderService
        ///  is available, or directly if it is not.
        /// </summary>
        protected virtual void OnEndLoad(bool successful, ICollection errors)
        {
            //we don't want successful to be true here if there were load errors.
            //this may allow a situation where we have a dirtied WSOD and might allow
            //a user to save a partially loaded designer docdata.
            successful = successful && (errors == null || errors.Count == 0)
                                    && (_serializationManager.Errors == null
                                    || _serializationManager.Errors.Count == 0);
            try
            {
                _state[s_stateLoaded] = true;
                IDesignerLoaderHost2 lh2 = GetService(typeof(IDesignerLoaderHost2)) as IDesignerLoaderHost2;

                if (!successful && (lh2 == null || !lh2.IgnoreErrorsDuringReload))
                {
                    // Can we even show the Continue Ignore errors in DTEL?
                    if (lh2 != null)
                    {
                        lh2.CanReloadWithErrors = LoaderHost.RootComponent != null;
                    }

                    UnloadDocument();
                }
                else
                {
                    successful = true;
                }

                // Inform the serialization manager that we are all done.  The serialization 
                // manager clears state at this point to help enforce a stateless serialization
                // mechanism.
                if (errors != null)
                {
                    foreach (object err in errors)
                    {
                        _serializationManager.Errors.Add(err);
                    }
                }

                errors = _serializationManager.Errors;
            }
            finally
            {
                _serializationSession.Dispose();
                _serializationSession = null;
            }

            if (successful)
            {
                // After a successful load we will want to monitor a bunch of events so we know when
                // to make the loader modified.
                IComponentChangeService cs = (IComponentChangeService)GetService(typeof(IComponentChangeService));

                if (cs != null)
                {
                    cs.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
                    cs.ComponentAdding += new ComponentEventHandler(OnComponentAdding);
                    cs.ComponentRemoving += new ComponentEventHandler(OnComponentRemoving);
                    cs.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
                    cs.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
                    cs.ComponentChanging += new ComponentChangingEventHandler(OnComponentChanging);
                    cs.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
                }

                EnableComponentNotification(true);
            }

            LoaderHost.EndLoad(_baseComponentClassName, successful, errors);

            // if we got errors in the load, set ourselves as modified so we'll regen code.  If this fails, we don't
            // care; the Modified bit was only a hint.
            if (_state[s_stateModifyIfErrors] && errors != null && errors.Count > 0)
            {
                try
                {
                    OnModifying();
                    Modified = true;
                }
                catch (CheckoutException ex)
                {
                    if (ex != CheckoutException.Canceled)
                    {
                        throw;
                    }
                }
            }
        }

        /// <summary>
        ///  This method is called in response to a component changing, adding or removing event to indicate
        ///  that the designer is about to be modified.  Those interested in implementing source code
        ///  control may do so by overriding this method.  A call to OnModifying does not mean that the
        ///  Modified property will later be set to true; it is merly an intention to do so.
        /// </summary>
        protected virtual void OnModifying()
        { }

        /// <summary>
        ///  Invoked by the loader host when it actually performs the reload, but before
        ///  the reload actually happens.  Here we unload our part of the loader
        ///  and get us ready for the pending reload.
        /// </summary>
        private void OnIdle(object sender, EventArgs e)
        {
            Application.Idle -= new EventHandler(OnIdle);

            if (!_state[s_stateReloadAtIdle])
            {
                return;
            }

            _state[s_stateReloadAtIdle] = false;

            //check to see if we are actually the active document.
            DesignSurfaceManager mgr = (DesignSurfaceManager)GetService(typeof(DesignSurfaceManager));
            DesignSurface thisSurface = (DesignSurface)GetService(typeof(DesignSurface));
            Debug.Assert(mgr != null && thisSurface != null);

            if (mgr != null && thisSurface != null)
            {
                if (!object.ReferenceEquals(mgr.ActiveDesignSurface, thisSurface))
                {
                    //somehow, we got deactivated and weren't told.
                    _state[s_stateActiveDocument] = false;
                    _state[s_stateDeferredReload] = true; //reload on activate
                    return;
                }
            }

            IDesignerLoaderHost host = LoaderHost;

            if (host == null)
            {
                return;
            }

            if (!_state[s_stateForceReload] && !IsReloadNeeded())
            {
                return;
            }

            try
            {
                if (_state[s_stateFlushReload])
                {
                    Flush();
                }

                UnloadDocument();
                host.Reload();
            }
            finally
            {
                _state[s_stateForceReload | s_stateModifyIfErrors | s_stateFlushReload] = false;
            }
        }

        /// <summary>
        ///  This method is called when it is time to flush the 
        ///  contents of the loader.  You should save any state
        ///  at this time.
        /// </summary>
        protected abstract void PerformFlush(IDesignerSerializationManager serializationManager);

        /// <summary>
        ///  This method is called when it is time to load the
        ///  design surface.  If you are loading asynchronously 
        ///  you should ask for IDesignerLoaderService and call 
        ///  AddLoadDependency.  When loading asynchronously you
        ///  should at least create the root component during
        ///  PerformLoad.  The DesignSurface is only able to provide
        ///  a view when there is a root component.
        /// </summary>
        protected abstract void PerformLoad(IDesignerSerializationManager serializationManager);

        /// <summary>
        ///  This method schedules a reload of the designer.
        ///  Designer reloading happens asynchronously in order
        ///  to unwind the stack before the reload begins.  If
        ///  force is true, a reload is always performed.  If
        ///  it is false, a reload is only performed if the
        ///  underlying code dom tree has changed in a way that
        ///  would affect the form.
        ///  If flush is true, the designer is flushed before performing
        ///  a reload.  If false, any designer changes are abandonded.
        ///  If ModifyOnError is true, the designer loader will be put
        ///  in the modified state if any errors happened during the 
        ///  load.
        /// </summary>
        protected void Reload(ReloadOptions flags)
        {
            _state[s_stateForceReload] = ((flags & ReloadOptions.Force) != 0);
            _state[s_stateFlushReload] = ((flags & ReloadOptions.NoFlush) == 0);
            _state[s_stateModifyIfErrors] = ((flags & ReloadOptions.ModifyOnError) != 0);

            // Our implementation of Reload only reloads if we are the 
            // active designer.  Otherwise, we wait until we become
            // active and reload at that time.  We also never do a 
            // reload if we are flushing code.
            if (_state[s_stateFlushInProgress])
            {
                return;
            }

            if (!_state[s_stateActiveDocument])
            {
                _state[s_stateDeferredReload] = true;
                return;
            }

            if (_state[s_stateReloadAtIdle])
            {
                return;
            }

            Application.Idle += new EventHandler(OnIdle);
            _state[s_stateReloadAtIdle] = true;
        }

        /// <summary>
        ///  This method is called during flush if one or more errors occurred while
        ///  flushing changes.  The values in the errors collection may either be
        ///  exceptions or objects whose ToString value describes the error.  The default
        ///  implementation of this method takes last exception in the collection and
        ///  raises it as an exception.
        /// </summary>
        protected virtual void ReportFlushErrors(ICollection errors)
        {
            object lastError = null;

            foreach (object e in errors)
            {
                lastError = e;
            }

            Debug.Assert(lastError != null, "Someone embedded a null in the error collection");

            if (lastError == null)
            {
                return;
            }

            Exception ex = lastError as Exception;

            if (ex == null)
            {
                ex = new InvalidOperationException(lastError.ToString());
            }

            throw ex;
        }

        /// <summary>
        ///  This property provides the name the designer surface
        ///  will use for the base class.  Normally this is a fully
        ///  qualified name such as "Project1.Form1".  You should set
        ///  this before finishing the load.  Generally this is set
        ///  during PerformLoad.
        /// </summary>
        protected void SetBaseComponentClassName(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            _baseComponentClassName = name;
        }

        /// <summary>
        ///  Simple helper routine that will throw an exception if we need a service, but cannot get
        ///  to it.  You should only throw for missing services that are absolutely essential for
        ///  operation.  If there is a way to gracefully degrade, then you should do it.
        /// </summary>
        private void ThrowMissingService(Type serviceType)
        {
            Exception ex = new InvalidOperationException(string.Format(SR.BasicDesignerLoaderMissingService, serviceType.Name));
            ex.HelpLink = SR.BasicDesignerLoaderMissingService;

            throw ex;
        }

        /// <summary>
        ///  This method will be called when the document is to be unloaded.  It
        ///  does not dispose us, but it gets us ready for a dispose or a reload.
        /// </summary>
        private void UnloadDocument()
        {
            OnBeginUnload();
            _state[s_stateLoaded] = false;
            _baseComponentClassName = null;
        }

        /// <summary>
        ///  Adds a load dependency to this loader.  This indicates that some other
        ///  object is also participating in the load, and that the designer loader
        ///  should not call EndLoad on the loader host until all load dependencies
        ///  have called DependentLoadComplete on the designer loader.
        /// </summary>
        void IDesignerLoaderService.AddLoadDependency()
        {
            if (_serializationManager == null)
            {
                throw new InvalidOperationException();
            }

            if (_loadDependencyCount++ == 0)
            {
                OnBeginLoad();
            }
        }

        /// <summary>
        ///  This is called by any object that has previously called
        ///  AddLoadDependency to signal that the dependent load has completed.
        ///  The caller should pass either an empty collection or null to indicate
        ///  a successful load, or a collection of exceptions that indicate the
        ///  reason(s) for failure.
        /// </summary>
        void IDesignerLoaderService.DependentLoadComplete(bool successful, ICollection errorCollection)
        {
            if (_loadDependencyCount == 0)
            {
                throw new InvalidOperationException();
            }

            // If the dependent load failed, remember it.  There may be multiple
            // dependent loads.  If any one fails, we're sunk.
            if (!successful)
            {
                _state[s_stateLoadFailed] = true;
            }

            if (--_loadDependencyCount == 0)
            {
                // We have just completed the last dependent load.  Report this.
                OnEndLoad(!_state[s_stateLoadFailed], errorCollection);
                return;
            }

            if (errorCollection == null)
            {
                return;
            }

            // Otherwise, add these errors to the serialization manager.
            foreach (object err in errorCollection)
            {
                _serializationManager.Errors.Add(err);
            }
        }

        /// <summary>
        ///  This can be called by an outside object to request that the loader
        ///  reload the design document.  If it supports reloading and wants to
        ///  comply with the reload, the designer loader should return true.  Otherwise
        ///  it should return false, indicating that the reload will not occur.
        ///  Callers should not rely on the reload happening immediately; the
        ///  designer loader may schedule this for some other time, or it may
        ///  try to reload at once.
        /// </summary>
        bool IDesignerLoaderService.Reload()
        {
            if (!_state[s_stateReloadSupported] || _loadDependencyCount != 0)
            {
                return false;
            }

            Reload(ReloadOptions.Force);

            return true;
        }
    }
}
