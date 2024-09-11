// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using System.Reflection;

namespace System.ComponentModel.Design;

/// <summary>
///  The UndoEngine is a class that can be instantiated to support automatic undo.
///  Normally, to support undo, a developer must create individual undo units that consist of an undo action
///  and a redo action. This is fragile because each and every action the user performs must be wrapped in an undo unit.
///  Worse, if a user action is not wrapped in an undo unit its absence on the undo stack will break undo because each
///  individual unit always assumes that the previous unit's state is maintained.
///  The UndoEngine, on the other hand, listens to change events and can create undo and redo actions automatically.
///  All that is necessary to implement undo is to add these actions to an undo/redo stack and instantiate this class.
/// </summary>
public abstract partial class UndoEngine : IDisposable
{
    private IServiceProvider _provider;
    private readonly Stack<UndoUnit> _unitStack; // the stack of active (non-committed) units.
    private UndoUnit? _executingUnit; // the unit currently executing an undo.
    private readonly IDesignerHost _host;
    private readonly ComponentSerializationService _serializationService;
    private EventHandler? _undoingEvent;
    private EventHandler? _undoneEvent;
    private Dictionary<IComponent, List<ReferencingComponent>>? _refToRemovedComponent;

    /// <summary>
    ///  Creates a new UndoEngine. UndoEngine requires a service provider for access to various services.
    ///  The following services must be available or else UndoEngine will throw an exception:
    ///  IDesignerHost
    ///  IComponentChangeService
    ///  IDesignerSerializationService
    /// </summary>
    protected UndoEngine(IServiceProvider provider)
    {
        _provider = provider.OrThrowIfNull();
        _unitStack = new Stack<UndoUnit>();
        Enabled = true;

        // Validate that all required services are available. Because undo is a passive activity we must know
        // up front if it is going to work or not.
        _host = GetRequiredService<IDesignerHost>();
        ComponentChangeService = GetRequiredService<IComponentChangeService>();
        _serializationService = GetRequiredService<ComponentSerializationService>();

        // We need to listen to a slew of events to determine undo state.
        _host.TransactionOpening += OnTransactionOpening;
        _host.TransactionClosed += OnTransactionClosed;
        ComponentChangeService.ComponentAdding += OnComponentAdding;
        ComponentChangeService.ComponentChanging += OnComponentChanging;
        ComponentChangeService.ComponentRemoving += OnComponentRemoving;
        ComponentChangeService.ComponentAdded += OnComponentAdded;
        ComponentChangeService.ComponentChanged += OnComponentChanged;
        ComponentChangeService.ComponentRemoved += OnComponentRemoved;
        ComponentChangeService.ComponentRename += OnComponentRename;
    }

    /// <summary>
    ///  This property indicates if an undo is in progress.
    /// </summary>
    public bool UndoInProgress => _executingUnit is not null;

    /// <summary>
    ///  This property returns true if the Undo engine is currently enabled. When enabled, the undo engine tracks
    ///  changes made to the designer. When disabled, changes are ignored.
    ///  If the UndoEngine is set to disabled while in the middle of processing change notifications from the designer,
    ///  the undo engine will only ignore additional changes. That is, it will finish recording the changes that
    ///  are in process and only ignore additional changes. Caution should be used when disabling undo.
    ///  If undo is disabled it is easy to make a change that would cause other undo actions to become invalid.
    ///  For example, if myButton.Text was changed, and then myButton was renamed while undo was disabled, attempting
    ///  to undo the text change would fail because there is no longer a control called myButton.
    ///  Generally, you should never make changes to components with undo disabled unless you are certain
    ///  to put the components back the way they were before undo was disabled. An example of this would be to
    ///  replace one instance of "Button" with another, say "SuperButton", fixing up all the property values as you go.
    ///  The result is a new component, but because it has the same component name and property values,
    ///  undo state will still be consistent.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    ///  This event is raised immediately before an undo action is performed.
    /// </summary>
    public event EventHandler? Undoing
    {
        add => _undoingEvent += value;
        remove => _undoingEvent -= value;
    }

    /// <summary>
    ///  This event is raised immediately after an undo action is performed. It will always be raised even if an exception is thrown.
    /// </summary>
    public event EventHandler? Undone
    {
        add => _undoneEvent += value;
        remove => _undoneEvent -= value;
    }

    /// <summary>
    ///  Adds the given undo unit into the undo stack. UndoEngine does not maintain its own undo stack,
    ///  so you must implement this method yourself.
    /// </summary>
    protected abstract void AddUndoUnit(UndoUnit unit);

    /// <summary>
    ///  This method will check to see if the current undo unit needs to be popped from the stack. If it does,
    ///  it will pop it and add it to the undo stack.
    ///  There must be at least one unit on the stack to call this method.
    ///
    ///  When calling CheckPopUnit you must supply a reason for the call.
    ///  There are three reasons:
    ///
    ///  Normal
    ///  Call with Normal if you are not calling in response to a closing transaction. For normal pop reasons,
    ///  the unit will be popped if there is no current transaction. If the unit is not empty it will be added
    ///  to the undo engine. If there is a transaction in progress, this method will do nothing.
    ///
    ///  TransactionCommit
    ///  Call with TransactionCommit if you are calling in response to a transaction closing,
    ///  and if that transaction is marked as being committed. CheckPopUnit will pop the unit off of the stack
    ///  and add it to the undo engine if it is not empty.
    ///
    ///  TransactionCancel
    ///  Call with TransactionCancel if you are calling in response to a transaction closing,
    ///  and if that transaction is marked as being cancelled. CheckPopUnit will pop the unit off of the stack.
    ///  If the unit is not empty Undo will be called on the unit to roll back the transaction work.
    ///  The unit will never be added to the undo engine.
    /// </summary>
    private void CheckPopUnit(PopUnitReason reason)
    {
        // The logic in here is subtle. This code handles both committing and cancelling of nested transactions.
        // Here's a summary of how it works:
        // 1. Each time a transaction is opened, a new unit is pushed onto the unit stack.
        // 2. When a change occurs, the change event checks to see if there is a currently executing unit.
        //        It also checks to see if the current unit stack is empty. If there is no executing unit
        //        (meaning that nothing is performing an undo right now), and if the unit stack is empty,
        //        the change event will create a new undo unit and push it on the stack.
        // 3. The change event always runs through all undo units in the undo stack and calls the corresponding
        //        change method. In the normal case of a single transaction or no transaction, this will operate on just one unit.
        // In the case of nested transactions there are two possibilities:
        //        a)  We are adding undo information to a nested transaction. We want to add the undo information
        //            to all levels of nested transactions. Why?  Because as a nested transaction is closed,
        //            it is either committed or cancelled. If committed, and if the transaction is not the top-most
        //            transaction, the transaction is actually just thrown away because its data is redundantly stored
        //            in the next transaction on the stack.
        //        b)  We are adding undo information to a nested transaction, but that undo information is being
        //            created because an undo unit is being "undone". Remember that for nested transactions each
        //            undo unit higher on the stack has all the data that the lower units have. When a lower unit
        //            is undone, it is popped from the stack and all of the changes it makes are recorded on the
        //            higher level units. This combines the "do" and "undo" data into the higher level unit, which
        //            in effect subtracts the undone data from the higher level unit.
        // 4. When a unit is undone it stores itself in a member variable called _executingUnit.
        //        All change events examine this variable and if it is set they do not create a new unit in response
        //        to a change. Instead, they just run through all the existing units. This builds the undo history
        //        for a transaction that is being rolled back.
        if (reason != PopUnitReason.Normal || !_host.InTransaction)
        {
            UndoUnit unit = _unitStack.Pop();

            if (!unit.IsEmpty)
            {
                unit.Close();

                if (reason == PopUnitReason.TransactionCancel)
                {
                    unit.Undo();
                    if (_unitStack.Count == 0)
                    {
                        DiscardUndoUnit(unit);
                    }
                }
                else
                {
                    if (_unitStack.Count == 0)
                    {
                        AddUndoUnit(unit);
                    }
                }
            }
            else
            {
                if (_unitStack.Count == 0)
                {
                    DiscardUndoUnit(unit);
                }
            }
        }
    }

    /// <summary>
    ///  This virtual method creates a new instance of an UndoUnit class. The default implementation just returns a
    ///  new instance of UndoUnit. Those providing their own UndoEngine can derive from UndoUnit to customize
    ///  the actions it performs. This is also a handy way to connect UndoEngine into an existing undo stack.
    ///  If the primary parameter is set to true, the undo unit will eventually be passed to either the AddUndoUnit
    ///  or DiscardUndoUnit methods. If the primary parameter is false, the undo unit is part of a nested transaction
    ///  and will never be passed to AddUndoUnit or DiscardUndoUnit; only the encompassing unit will be passed,
    ///  because the undo engine will either include or exclude the contents of the nested unit when it is closed.
    /// </summary>
    protected virtual UndoUnit CreateUndoUnit(string? name, bool primary)
    {
        return new UndoUnit(this, name);
    }

    internal IComponentChangeService ComponentChangeService { get; }

    /// <summary>
    ///  This method is called instead of AddUndoUnit for undo units that have been canceled.
    ///  For undo systems that just treat undo as a simple stack of undo units,
    ///  typically you do not need to override this method.
    ///  This method does give you a chance to perform any clean-up for a unit.
    /// </summary>
    protected virtual void DiscardUndoUnit(UndoUnit unit)
    {
    }

    /// <summary>
    ///  Public dispose method.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
    }

    /// <summary>
    ///  Protected dispose implementation.
    /// </summary>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            _host.TransactionOpening -= OnTransactionOpening;
            _host.TransactionClosed -= OnTransactionClosed;

            ComponentChangeService.ComponentAdding -= OnComponentAdding;
            ComponentChangeService.ComponentChanging -= OnComponentChanging;
            ComponentChangeService.ComponentRemoving -= OnComponentRemoving;
            ComponentChangeService.ComponentAdded -= OnComponentAdded;
            ComponentChangeService.ComponentChanged -= OnComponentChanged;
            ComponentChangeService.ComponentRemoved -= OnComponentRemoved;
            ComponentChangeService.ComponentRename -= OnComponentRename;

            _provider = null!;
        }
    }

    /// <summary>
    ///  Helper function to retrieve the name of an object.
    /// </summary>
    internal string? GetName(object? obj, bool generateNew)
    {
        string? componentName = null;
        if (obj is not null)
        {
            if (TryGetService(out IReferenceService? rs))
            {
                componentName = rs.GetName(obj);
            }
            else
            {
                if (obj is IComponent comp)
                {
                    ISite? site = comp.Site;
                    if (site is not null)
                    {
                        componentName = site.Name;
                    }
                }
            }
        }

        if (componentName is null && generateNew)
        {
            componentName = obj is null ? "(null)" : obj.GetType().Name;
        }

        return componentName;
    }

    /// <summary>
    ///  Similar to GetService, but this will throw a NotSupportedException if the service is not present.
    /// </summary>
    protected object GetRequiredService(Type serviceType)
    {
        object? service = GetService(serviceType);
        if (service is null)
        {
            Exception ex = new InvalidOperationException(string.Format(SR.UndoEngineMissingService, serviceType.Name))
            {
                HelpLink = SR.UndoEngineMissingService
            };
            throw ex;
        }

        return service;
    }

    private protected T GetRequiredService<T>() => (T)GetRequiredService(typeof(T));

    /// <summary>
    ///  This just calls through to the service provider passed into the constructor.
    /// </summary>
    protected object? GetService(Type serviceType)
    {
        ArgumentNullException.ThrowIfNull(serviceType);

        return _provider?.GetService(serviceType);
    }

    private protected bool TryGetService<T>([NotNullWhen(true)] out T? service) where T : class
    {
        service = GetService(typeof(T)) as T;
        return service is not null;
    }

    private void OnComponentAdded(object? sender, ComponentEventArgs e)
    {
        foreach (UndoUnit unit in _unitStack)
        {
            unit.ComponentAdded(e);
        }

        if (_unitStack.Count > 0)
        {
            CheckPopUnit(PopUnitReason.Normal);
        }
    }

    private void OnComponentAdding(object? sender, ComponentEventArgs e)
    {
        // Open a new unit unless there is already one open or we are currently executing a unit.
        // If we need to create a unit, we will have to fabricate a good name.
        if (Enabled && _executingUnit is null && _unitStack.Count == 0)
        {
            string name;
            if (e.Component is not null)
            {
                name = string.Format(SR.UndoEngineComponentAdd1, GetName(e.Component, true));
            }
            else
            {
                name = SR.UndoEngineComponentAdd0;
            }

            _unitStack.Push(CreateUndoUnit(name, true));
        }

        // Now walk all the units and notify them. We don't care which order the units are notified.
        foreach (UndoUnit unit in _unitStack)
        {
            unit.ComponentAdding(e);
        }
    }

    private void OnComponentChanged(object? sender, ComponentChangedEventArgs e)
    {
        foreach (UndoUnit unit in _unitStack)
        {
            unit.ComponentChanged(e);
        }

        if (_unitStack.Count > 0)
        {
            CheckPopUnit(PopUnitReason.Normal);
        }
    }

    private void OnComponentChanging(object? sender, ComponentChangingEventArgs e)
    {
        // Open a new unit unless there is already one open or we are currently executing a unit.
        // If we need to create a unit, we will have to fabricate a good name.
        if (Enabled && _executingUnit is null && _unitStack.Count == 0)
        {
            string name;

            if (e.Component is null)
            {
                name = SR.UndoEngineComponentChange0;
            }
            else if (e.Member is null)
            {
                name = string.Format(SR.UndoEngineComponentChange1, GetName(e.Component, true));
            }
            else
            {
                name = string.Format(SR.UndoEngineComponentChange2, GetName(e.Component, true), e.Member.Name);
            }

            _unitStack.Push(CreateUndoUnit(name, true));
        }

        // Now walk all the units and notify them. We don't care which order the units are notified.
        foreach (UndoUnit unit in _unitStack)
        {
            unit.ComponentChanging(e);
        }
    }

    private void OnComponentRemoved(object? sender, ComponentEventArgs e)
    {
        foreach (UndoUnit unit in _unitStack)
        {
            unit.ComponentRemoved(e);
        }

        if (_unitStack.Count > 0)
        {
            CheckPopUnit(PopUnitReason.Normal);
        }

        // Now we need to raise ComponentChanged events for every component that had a reference to this removed component
        if (_refToRemovedComponent is not null && _refToRemovedComponent.Remove(e.Component!, out List<ReferencingComponent>? propsToUpdate))
        {
            foreach (ReferencingComponent ro in propsToUpdate)
            {
                ComponentChangeService.OnComponentChanged(ro.component, ro.member);
            }
        }
    }

    private void OnComponentRemoving(object? sender, ComponentEventArgs e)
    {
        // Open a new unit unless there is already one open or we are currently executing a unit.
        // If we need to create a unit, we will have to fabricate a good name.
        if (Enabled && _executingUnit is null && _unitStack.Count == 0)
        {
            string name;
            if (e.Component is not null)
            {
                name = string.Format(SR.UndoEngineComponentRemove1, GetName(e.Component, true));
            }
            else
            {
                name = SR.UndoEngineComponentRemove0;
            }

            _unitStack.Push(CreateUndoUnit(name, true));
        }

        // We need to keep track of all references in the container to the deleted component so that those references
        // can be fixed up if an undo of this "remove" occurs.
        if (Enabled && _host is not null && _host.Container is not null && ComponentChangeService is not null)
        {
            List<ReferencingComponent>? propsToUpdate = null;
            foreach (IComponent comp in _host.Container.Components)
            {
                if (comp == e.Component)
                {
                    continue;
                }

                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);
                foreach (PropertyDescriptor prop in props)
                {
                    if (prop.PropertyType.IsInstanceOfType(e.Component) &&
                        !prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden) &&
                        !prop.IsReadOnly)
                    {
                        object? obj = null;
                        try
                        {
                            obj = prop.GetValue(comp);
                        }
                        catch (TargetInvocationException)
                        {
                            continue;
                        }

                        if (obj is not null && ReferenceEquals(obj, e.Component))
                        {
                            if (propsToUpdate is null)
                            {
                                propsToUpdate = [];

                                _refToRemovedComponent ??= [];

                                _refToRemovedComponent[e.Component] = propsToUpdate;
                            }

                            ComponentChangeService.OnComponentChanging(comp, prop);
                            propsToUpdate.Add(new ReferencingComponent(comp, prop));
                        }
                    }
                }
            }
        }

        // Now walk all the units and notify them. We don't care which order the units are notified.
        // By notifying all transactions we automatically support the cancelling of nested transactions.
        foreach (UndoUnit unit in _unitStack)
        {
            unit.ComponentRemoving(e);
        }
    }

    private void OnComponentRename(object? sender, ComponentRenameEventArgs e)
    {
        // Open a new unit unless there is already one open or we are currently executing a unit.
        // If we need to create a unit, we will have to fabricate a good name.
        if (Enabled && _executingUnit is null && _unitStack.Count == 0)
        {
            string name = string.Format(SR.UndoEngineComponentRename, e.OldName, e.NewName);
            _unitStack.Push(CreateUndoUnit(name, true));
        }

        // Now walk all the units and notify them. We don't care which order the units are notified.
        // By notifying all transactions we automatically support the cancelling of nested transactions.
        foreach (UndoUnit unit in _unitStack)
        {
            unit.ComponentRename(e);
        }
    }

    private void OnTransactionClosed(object? sender, DesignerTransactionCloseEventArgs e)
    {
        if (_executingUnit is null && _unitStack.Count > 0)
        {
            PopUnitReason reason = e.TransactionCommitted ? PopUnitReason.TransactionCommit : PopUnitReason.TransactionCancel;
            CheckPopUnit(reason);
        }
    }

    private void OnTransactionOpening(object? sender, EventArgs e)
    {
        // When a transaction is opened, we always push a new unit unless we're executing a unit.
        // We can push multiple units onto the stack to handle nested transactions.
        if (Enabled && _executingUnit is null)
        {
            _unitStack.Push(CreateUndoUnit(_host.TransactionDescription, _unitStack.Count == 0));
        }
    }

    /// <summary>
    ///  This event is raised immediately before an undo action is performed.
    /// </summary>
    protected virtual void OnUndoing(EventArgs e)
    {
        _undoingEvent?.Invoke(this, e);
    }

    /// <summary>
    ///  This event is raised immediately after an undo action is performed. It will always be raised even if an exception is thrown.
    /// </summary>
    protected virtual void OnUndone(EventArgs e)
    {
        _undoneEvent?.Invoke(this, e);
    }

    private enum PopUnitReason
    {
        Normal, TransactionCommit, TransactionCancel,
    }

    private readonly struct ReferencingComponent
    {
        public readonly IComponent component;
        public readonly MemberDescriptor member;

        public ReferencingComponent(IComponent component, MemberDescriptor member)
        {
            this.component = component;
            this.member = member;
        }
    }
}
