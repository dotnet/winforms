// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

/// <summary>
///  This service tracks individual designer events. The class itself
///  receives event information by direct calls from DesignerApplication.
///  Those wishing to replace this service may do so but need to override
///  the appropriate virtual methods on DesignerApplication.
/// </summary>
internal sealed class DesignerEventService : IDesignerEventService
{
    private static readonly object s_eventActiveDesignerChanged = new();
    private static readonly object s_eventDesignerCreated = new();
    private static readonly object s_eventDesignerDisposed = new();
    private static readonly object s_eventSelectionChanged = new();

    private List<IDesignerHost>? _designerList;          // read write list used as data for the collection
    private DesignerCollection? _designerCollection;     // public read only view of the above list
    private IDesignerHost? _activeDesigner;              // the currently active designer. Can be null
    private EventHandlerList? _events;                   // list of events. Can be null
    private bool _inTransaction;                        // true if we are in a transaction
    private bool _deferredSelChange;                    // true if we have a deferred selection change notification pending

    /// <summary>
    ///  Internal ctor to prevent semitrust from creating us.
    /// </summary>
    internal DesignerEventService()
    {
    }

    /// <summary>
    ///  This is called by the DesignerApplication class when
    ///  a designer is activated. The passed in designer can
    ///  be null to signify no designer is currently active.
    /// </summary>
    internal void OnActivateDesigner(DesignSurface? surface)
    {
        IDesignerHost? host = null;
        if (surface is not null)
        {
            host = surface.GetService(typeof(IDesignerHost)) as IDesignerHost;
            Debug.Assert(host is not null, "Design surface did not provide us with a designer host");

            // If the designer host is not in our collection, add it.
            if (host is not null && (_designerList is null || !_designerList.Contains(host)))
            {
                OnCreateDesigner(surface);
            }
        }

        if (_activeDesigner == host)
        {
            return;
        }

        IDesignerHost? oldDesigner = _activeDesigner;
        _activeDesigner = host;

        if (oldDesigner is not null)
        {
            SinkChangeEvents(oldDesigner, false);
        }

        if (_activeDesigner is not null)
        {
            SinkChangeEvents(_activeDesigner, true);
        }

        (_events?[s_eventActiveDesignerChanged] as ActiveDesignerEventHandler)?.Invoke(this, new ActiveDesignerEventArgs(oldDesigner, host));

        // Activating a new designer automatically pushes a new selection.
        //
        OnSelectionChanged(this, EventArgs.Empty);
    }

    /// <summary>
    ///  Called when a component is added or removed from the active designer.
    ///  We raise a selection change event here.
    /// </summary>
    private void OnComponentAddedRemoved(object? sender, ComponentEventArgs ce)
    {
        if (ce.Component?.Site?.Container is IDesignerHost { Loading: true })
        {
            _deferredSelChange = true;
            return;
        }

        OnSelectionChanged(this, EventArgs.Empty);
    }

    /// <summary>
    ///  Called when a component has changed on the active designer. Here
    ///  we grab the active selection service and see if the component that
    ///  has changed is also selected. If it is, then we raise a global
    ///  selection changed event.
    /// </summary>
    private void OnComponentChanged(object? sender, ComponentChangedEventArgs ce)
    {
        if (ce.Component is not IComponent comp)
        {
            return;
        }

        if (comp.Site.TryGetService(out ISelectionService? ss) && ss.GetComponentSelected(comp))
        {
            OnSelectionChanged(this, EventArgs.Empty);
        }
    }

    /// <summary>
    ///  This is called by the DesignerApplication class when
    ///  a designer is created. Activation generally follows.
    /// </summary>
    [MemberNotNull(nameof(_designerList))]
    internal void OnCreateDesigner(DesignSurface surface)
    {
        Debug.Assert(surface is not null, "DesignerApplication should not pass null here");
        IDesignerHost? host = surface.GetService<IDesignerHost>();
        Debug.Assert(host is not null, "Design surface did not provide us with a designer host");

        _designerList ??= [];
        _designerList.Add(host);

        // Hookup an object disposed handler on the design surface so we know when it's gone.
        surface.Disposed += OnDesignerDisposed;

        if (_events?[s_eventDesignerCreated] is DesignerEventHandler eh)
        {
            eh(this, new DesignerEventArgs(host));
        }
    }

    /// <summary>
    ///  Called by DesignSurface when it is about to be disposed.
    /// </summary>
    private void OnDesignerDisposed(object? sender, EventArgs e)
    {
        DesignSurface surface = (DesignSurface)sender!;
        surface.Disposed -= OnDesignerDisposed;

        // Detach the selection change and add/remove events, if we were monitoring such events
        SinkChangeEvents(surface, false);

        IDesignerHost? host = surface.GetService<IDesignerHost>();
        Debug.Assert(host is not null, "Design surface removed host too early in dispose");
        if (host is null)
        {
            return;
        }

        if (_events?[s_eventDesignerDisposed] is DesignerEventHandler eh)
        {
            eh(this, new DesignerEventArgs(host));
        }

        _designerList?.Remove(host);
    }

    /// <summary>
    ///  Called by the active designer's selection service when the selection changes.
    ///  Also called directly by us when the active designer changes, as this is
    ///  also a change to the global selection context.
    /// </summary>
    private void OnSelectionChanged(object? sender, EventArgs e)
    {
        if (_inTransaction)
        {
            _deferredSelChange = true;
            return;
        }

        if (_events?[s_eventSelectionChanged] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Called by the designer host when it is done loading
    ///  Here we queue up selection notification.
    /// </summary>
    private void OnLoadComplete(object? sender, EventArgs e)
    {
        if (!_deferredSelChange)
        {
            return;
        }

        _deferredSelChange = false;
        OnSelectionChanged(this, EventArgs.Empty);
    }

    /// <summary>
    ///  Called by the designer host when it is entering or leaving a batch
    ///  operation. Here we queue up selection notification and we turn off
    ///  our UI.
    /// </summary>
    private void OnTransactionClosed(object? sender, DesignerTransactionCloseEventArgs e)
    {
        if (!e.LastTransaction)
        {
            return;
        }

        _inTransaction = false;
        if (!_deferredSelChange)
        {
            return;
        }

        _deferredSelChange = false;
        OnSelectionChanged(this, EventArgs.Empty);
    }

    /// <summary>
    ///  Called by the designer host when it is entering or leaving a batch
    ///  operation. Here we queue up selection notification and we turn off
    ///  our UI.
    /// </summary>
    private void OnTransactionOpened(object? sender, EventArgs e)
    {
        _inTransaction = true;
    }

    /// <summary>
    ///  Sinks or unsinks selection and component change events from the
    ///  provided service provider. We need to raise global selection change
    ///  notifications. A global selection change should be raised whenever
    ///  the selection of the active designer changes, whenever a component
    ///  is added or removed from the active designer, or whenever the
    ///  active designer itself changes.
    /// </summary>
    private void SinkChangeEvents(IServiceProvider provider, bool sink)
    {
        ISelectionService? ss = provider.GetService<ISelectionService>();
        IComponentChangeService? cs = provider.GetService<IComponentChangeService>();
        IDesignerHost? host = provider.GetService<IDesignerHost>();

        if (sink)
        {
            if (ss is not null)
            {
                ss.SelectionChanged += OnSelectionChanged;
            }

            if (cs is not null)
            {
                ComponentEventHandler ce = new(OnComponentAddedRemoved);
                cs.ComponentAdded += ce;
                cs.ComponentRemoved += ce;
                cs.ComponentChanged += OnComponentChanged;
            }

            if (host is not null)
            {
                host.TransactionOpened += OnTransactionOpened;
                host.TransactionClosed += OnTransactionClosed;
                host.LoadComplete += OnLoadComplete;

                if (host.InTransaction)
                {
                    OnTransactionOpened(host, EventArgs.Empty);
                }
            }
        }
        else
        {
            if (ss is not null)
            {
                ss.SelectionChanged -= OnSelectionChanged;
            }

            if (cs is not null)
            {
                ComponentEventHandler ce = new(OnComponentAddedRemoved);
                cs.ComponentAdded -= ce;
                cs.ComponentRemoved -= ce;
                cs.ComponentChanged -= OnComponentChanged;
            }

            if (host is not null)
            {
                host.TransactionOpened -= OnTransactionOpened;
                host.TransactionClosed -= OnTransactionClosed;
                host.LoadComplete -= OnLoadComplete;

                if (host.InTransaction)
                {
                    OnTransactionClosed(host, new(commit: false, lastTransaction: true));
                }
            }
        }
    }

    /// <summary>
    ///  Gets the currently active designer.
    /// </summary>
    IDesignerHost? IDesignerEventService.ActiveDesigner => _activeDesigner;

    /// <summary>
    ///  Gets or
    ///  sets a collection of running design documents in the development environment.
    /// </summary>
    [MemberNotNull(nameof(_designerList))]
    [MemberNotNull(nameof(_designerCollection))]
    DesignerCollection IDesignerEventService.Designers
    {
        get
        {
            _designerList ??= [];
            _designerCollection ??= new DesignerCollection(_designerList);
            return _designerCollection;
        }
    }

    /// <summary>
    ///  Adds an event that will be raised when the currently active designer
    ///  changes.
    /// </summary>
    event ActiveDesignerEventHandler? IDesignerEventService.ActiveDesignerChanged
    {
        [MemberNotNull(nameof(_events))]
        add
        {
            _events ??= new EventHandlerList();

            _events[s_eventActiveDesignerChanged] = Delegate.Combine(_events[s_eventActiveDesignerChanged], value);
        }
        remove
        {
            if (_events is null)
            {
                return;
            }

            _events[s_eventActiveDesignerChanged] = Delegate.Remove(_events[s_eventActiveDesignerChanged], value);
        }
    }

    /// <summary>
    ///  Adds an event that will be raised when a designer is created.
    /// </summary>
    event DesignerEventHandler? IDesignerEventService.DesignerCreated
    {
        [MemberNotNull(nameof(_events))]
        add
        {
            _events ??= new EventHandlerList();

            _events[s_eventDesignerCreated] = Delegate.Combine(_events[s_eventDesignerCreated], value);
        }
        remove
        {
            if (_events is null)
            {
                return;
            }

            _events[s_eventDesignerCreated] = Delegate.Remove(_events[s_eventDesignerCreated], value);
        }
    }

    /// <summary>
    ///  Adds an event that will be raised when a designer is disposed.
    /// </summary>
    event DesignerEventHandler? IDesignerEventService.DesignerDisposed
    {
        [MemberNotNull(nameof(_events))]
        add
        {
            _events ??= new EventHandlerList();

            _events[s_eventDesignerDisposed] = Delegate.Combine(_events[s_eventDesignerDisposed], value);
        }
        remove
        {
            if (_events is null)
            {
                return;
            }

            _events[s_eventDesignerDisposed] = Delegate.Remove(_events[s_eventDesignerDisposed], value);
        }
    }

    /// <summary>
    ///  Adds an event that will be raised when the global selection changes.
    /// </summary>
    event EventHandler? IDesignerEventService.SelectionChanged
    {
        [MemberNotNull(nameof(_events))]
        add
        {
            _events ??= new EventHandlerList();

            _events[s_eventSelectionChanged] = Delegate.Combine(_events[s_eventSelectionChanged], value);
        }
        remove
        {
            if (_events is null)
            {
                return;
            }

            _events[s_eventSelectionChanged] = Delegate.Remove(_events[s_eventSelectionChanged], value);
        }
    }
}
