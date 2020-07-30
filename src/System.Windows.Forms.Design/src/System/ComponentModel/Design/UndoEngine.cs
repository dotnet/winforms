// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  The UndoEngine is a class that can be instantiated to support automatic undo.
    ///  Normally, to support undo, a developer must create individual undo units that consist of an undo action and a redo action.
    ///  This is fragile because each and every action the user performs must be wrapped in an undo unit.
    ///  Worse, if a user action is not wrapped in an undo unit its absence on the undo stack will break undo because each individual unit always assumes that the previous unit's state is maintained.
    ///  The UndoEngine, on the other hand, listens to change events and can create undo and redo actions automatically.
    ///  All that is necessary to implement undo is to add these actions to an undo/redo stack and instantiate this class.
    /// </summary>
    public abstract class UndoEngine : IDisposable
    {
        private static readonly TraceSwitch s_traceUndo = new TraceSwitch("UndoEngine", "Trace UndoRedo");

        private IServiceProvider _provider;
        private readonly Stack _unitStack; // the stack of active (non-committed) units.
        private UndoUnit _executingUnit; // the unit currently executing an undo.
        private readonly IDesignerHost _host;
        private readonly ComponentSerializationService _serializationService;
        private EventHandler _undoingEvent;
        private EventHandler _undoneEvent;
        private readonly IComponentChangeService _componentChangeService;
        private Dictionary<IComponent, List<ReferencingComponent>> _refToRemovedComponent;
        private bool _enabled;

        /// <summary>
        ///  Creates a new UndoEngine.  UndoEngine requires a service provider for access to various services.  The following services must be available or else UndoEngine will  throw an exception:
        ///  IDesignerHost
        ///  IComponentChangeService
        ///  IDesignerSerializationService
        /// </summary>
        protected UndoEngine(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _unitStack = new Stack();
            _enabled = true;

            // Validate that all required services are available.  Because undo is a passive activity we must know up front if it is going to work or not.
            _host = GetRequiredService(typeof(IDesignerHost)) as IDesignerHost;
            _componentChangeService = GetRequiredService(typeof(IComponentChangeService)) as IComponentChangeService;
            _serializationService = GetRequiredService(typeof(ComponentSerializationService)) as ComponentSerializationService;

            // We need to listen to a slew of events to determine undo state.
            _host.TransactionOpening += new EventHandler(OnTransactionOpening);
            _host.TransactionClosed += new DesignerTransactionCloseEventHandler(OnTransactionClosed);
            _componentChangeService.ComponentAdding += new ComponentEventHandler(OnComponentAdding);
            _componentChangeService.ComponentChanging += new ComponentChangingEventHandler(OnComponentChanging);
            _componentChangeService.ComponentRemoving += new ComponentEventHandler(OnComponentRemoving);
            _componentChangeService.ComponentAdded += new ComponentEventHandler(OnComponentAdded);
            _componentChangeService.ComponentChanged += new ComponentChangedEventHandler(OnComponentChanged);
            _componentChangeService.ComponentRemoved += new ComponentEventHandler(OnComponentRemoved);
            _componentChangeService.ComponentRename += new ComponentRenameEventHandler(OnComponentRename);
        }

        /// <summary>
        ///  Retrieves the current unit from the stack.
        /// </summary>
        private UndoUnit CurrentUnit
        {
            get
            {
                if (_unitStack.Count > 0)
                {
                    return (UndoUnit)_unitStack.Peek();
                }
                return null;
            }
        }

        /// <summary>
        ///  This property indicates if an undo is in progress.
        /// </summary>
        public bool UndoInProgress
        {
            get => _executingUnit != null;
        }

        /// <summary>
        ///  This property returns true if the Undo engine is currently enabled.  When enabled, the undo engine tracks changes made to the designer.  When disabled, changes are ignored.
        ///  If the UndoEngine is set to disabled while in the middle of processing change notifications from the designer, the undo engine will only ignore additional changes.
        ///  That is, it will finish recording the changes that are in process and only ignore additional changes.
        ///  Caution should be used when disabling undo.  If undo is disabled it is easy to make a change that would cause other undo actions to become invalid.
        ///  For example, if myButton.Text was changed, and then myButton was renamed while undo was disabled, attempting to undo the text change would fail because there is no longer a control called myButton.
        ///  Generally, you should never make changes to components with undo disabled unless you are certain to put the components back the way they were before undo was disabled.  An example of this would be to replace one instance of "Button" with another, say "SuperButton", fixing up all the property values as you go.  The result is a new component, but because it has the same component name and property values, undo state will still be consistent.
        /// </summary>
        public bool Enabled
        {
            get => _enabled;
            set => _enabled = value;
        }

        /// <summary>
        ///  This event is raised immediately before an undo action is performed.
        /// </summary>
        public event EventHandler Undoing
        {
            add => _undoingEvent += value;
            remove => _undoingEvent -= value;
        }

        /// <summary>
        ///  This event is raised immediately after an undo action is performed. It will always be raised even if an exception is thrown.
        /// </summary>
        public event EventHandler Undone
        {
            add => _undoneEvent += value;
            remove => _undoneEvent -= value;
        }

        /// <summary>
        ///  Adds the given undo unit into the undo stack.  UndoEngine does not maintain its own undo stack, so you must implement this method yourself.
        /// </summary>
        protected abstract void AddUndoUnit(UndoUnit unit);

        /// <summary>
        ///  This method will check to see if the current undo unit needs to be popped from the stack.  If it does, it will pop it and add it to the undo stack.
        ///  There must be at least one unit on the stack to call this method.
        ///
        ///  When calling CheckPopUnit you must supply a reason for the call.
        ///  There are three reasons:
        ///
        ///  Normal
        ///  Call with Normal if you are not calling in response to a closing transaction.  For normal pop reasons, the unit will be popped if there is no current transaction.  If the unit is not empty it will be added to the undo engine.  If there is a transaction in progress, this method will do nothing.
        ///
        ///  TransactionCommit
        ///  Call with TransactionCommit if you are calling in response to a transaction closing, and if that transaction is marked as being committed.  CheckPopUnit will pop the unit off of the stack and add it to the undo engine if it is not empty.
        ///
        ///  TransactionCancel
        ///  Call with TransactionCancel if you are calling in response to a transaction closing, and if that transaction is marked as being cancelled.  CheckPopUnit will pop the unit off of the stack.  If the unit is not empty Undo will be called on the unit to roll back the transaction work.  The unit will never be added to the undo engine.
        /// </summary>
        private void CheckPopUnit(PopUnitReason reason)
        {
            // The logic in here is subtle.  This code handles both committing and cancelling of nested transactions.  Here's a summary of how it works:
            // 1.  Each time a transaction is opened, a new unit is pushed onto  the unit stack.
            // 2.  When a change occurs, the change event checks to see if there is a currently executing unit.  It also checks to see if the current unit stack is empty.  If there is no executing unit (meaning that nothing is performing an undo right now), and if the unit stack is empty, the change event will create a new undo unit and push it on the stack.
            // 3.  The change event always runs through all undo units in the undo stack and calls the corresponding change method.  In the normal case of a single transaction or no transaction, this will operate on just one unit.  In the case of nested transactions there are two possibilities:
            //          a)  We are adding undo information to a nested transaction. We want to add the undo information to all levels of nested transactions.  Why?  Because as a nested transaction is closed, it is either committed or cancelled.  If committed, and if the transaction is not the top-most transaction, the transaction is actually just thrown away because its data is redundantly stored in the next transaction on the stack.
            //          b)  We are adding undo information to a nested transaction, but that undo information is being created because an undo unit is being "undone".  Remember that for nested transactions each undo unit higher on the stack has all the data that the lower units have.  When a lower unit is undone, it is popped from the stack and all of the changes it makes are recorded on the higher level units.  This combines the "do" and "undo" data into the higher level unit, which in effect subtracts the undone data from the higher level unit.
            // 4.  When a unit is undone it stores itself in a member variable called _executingUnit.  All change events examine this variable and if it is set they do not create a new unit in response to a change.  Instead, they just run through all the existing units.  This builds the undo history for a transaction that is being rolled back.
            if (reason != PopUnitReason.Normal || !_host.InTransaction)
            {
                Trace("Popping unit {0}.  Reason: {1}", _unitStack.Peek(), reason);
                UndoUnit unit = (UndoUnit)_unitStack.Pop();

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
        ///  This virtual method creates a new instance of an  UndoUnit class.  The default implementation just returns a new instance of UndoUnit.  Those providing their own UndoEngine can derive from UndoUnit to customize the actions it performs.  This is also a handy way to connect UndoEngine into an existing undo stack.
        ///  If the primary parameter is set to true, the undo unit will eventually be passed to either the AddUndoUnit or DiscardUndoUnit methods.  If the primary parameter is false, the undo unit is part of a nested transaction and will never be passed to AddUndoUnit or DiscardUndoUnit; only the encompasing unit will be passed, because the undo engine will either include or exclude the contents of the nested unit when it is closed.
        /// </summary>
        protected virtual UndoUnit CreateUndoUnit(string name, bool primary)
        {
            return new UndoUnit(this, name);
        }

        internal IComponentChangeService ComponentChangeService
        {
            get => _componentChangeService;
        }

        /// <summary>
        ///  This method is called instead of AddUndoUnit for undo units that have been canceled.  For undo systems that just treat undo as a simple stack of undo units, typically you do not need to override this method.  This method does give you a chance to perform any clean-up for a unit
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
                Trace("Disposing undo engine");

                if (_host != null)
                {
                    _host.TransactionOpening -= new EventHandler(OnTransactionOpening);
                    _host.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnTransactionClosed);
                }

                if (_componentChangeService != null)
                {
                    _componentChangeService.ComponentAdding -= new ComponentEventHandler(OnComponentAdding);
                    _componentChangeService.ComponentChanging -= new ComponentChangingEventHandler(OnComponentChanging);
                    _componentChangeService.ComponentRemoving -= new ComponentEventHandler(OnComponentRemoving);
                    _componentChangeService.ComponentAdded -= new ComponentEventHandler(OnComponentAdded);
                    _componentChangeService.ComponentChanged -= new ComponentChangedEventHandler(OnComponentChanged);
                    _componentChangeService.ComponentRemoved -= new ComponentEventHandler(OnComponentRemoved);
                    _componentChangeService.ComponentRename -= new ComponentRenameEventHandler(OnComponentRename);
                }
                _provider = null;
            }
        }

        /// <summary>
        ///  Helper function to retrieve the name of an object.
        /// </summary>
        internal string GetName(object obj, bool generateNew)
        {
            string componentName = null;
            if (obj != null)
            {
                if (GetService(typeof(IReferenceService)) is IReferenceService rs)
                {
                    componentName = rs.GetName(obj);
                }
                else
                {
                    if (obj is IComponent comp)
                    {
                        ISite site = comp.Site;
                        if (site != null)
                        {
                            componentName = site.Name;
                        }
                    }
                }
            }

            if (componentName is null && generateNew)
            {
                if (obj is null)
                {
                    componentName = "(null)";
                }
                else
                {
                    componentName = obj.GetType().Name;
                }
            }
            return componentName;
        }

        /// <summary>
        ///  Similar to GetService, but this will throw a NotSupportedException if the service is not present.
        /// </summary>
        protected object GetRequiredService(Type serviceType)
        {
            object service = GetService(serviceType);
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

        /// <summary>
        ///  This just calls through to the service provider passed into the constructor.
        /// </summary>
        protected object GetService(Type serviceType)
        {
            if (serviceType is null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (_provider != null)
            {
                return _provider.GetService(serviceType);
            }
            return null;
        }

        private void OnComponentAdded(object sender, ComponentEventArgs e)
        {
            foreach (UndoUnit unit in _unitStack)
            {
                unit.ComponentAdded(e);
            }

            if (CurrentUnit != null)
            {
                CheckPopUnit(PopUnitReason.Normal);
            }
        }

        private void OnComponentAdding(object sender, ComponentEventArgs e)
        {
            // Open a new unit unless there is already one open or we are currently executing a unit. If we need to create a unit, we will have to fabricate a good name.
            if (_enabled && _executingUnit is null && _unitStack.Count == 0)
            {
                string name;
                if (e.Component != null)
                {
                    name = string.Format(SR.UndoEngineComponentAdd1, GetName(e.Component, true));
                }
                else
                {
                    name = SR.UndoEngineComponentAdd0;
                }
                _unitStack.Push(CreateUndoUnit(name, true));
            }

            // Now walk all the units and notify them.  We don't care which order the units are notified.
            foreach (UndoUnit unit in _unitStack)
            {
                unit.ComponentAdding(e);
            }
        }

        private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
        {
            foreach (UndoUnit unit in _unitStack)
            {
                unit.ComponentChanged(e);
            }

            if (CurrentUnit != null)
            {
                CheckPopUnit(PopUnitReason.Normal);
            }
        }

        private void OnComponentChanging(object sender, ComponentChangingEventArgs e)
        {
            // Open a new unit unless there is already one open or we are currently executing a unit. If we need to create a unit, we will have to fabricate a good name.
            if (_enabled && _executingUnit is null && _unitStack.Count == 0)
            {
                string name;

                if (e.Member != null && e.Component != null)
                {
                    name = string.Format(SR.UndoEngineComponentChange2, GetName(e.Component, true), e.Member.Name);
                }
                else if (e.Component != null)
                {
                    name = string.Format(SR.UndoEngineComponentChange1, GetName(e.Component, true));
                }
                else
                {
                    name = SR.UndoEngineComponentChange0;
                }
                _unitStack.Push(CreateUndoUnit(name, true));
            }

            // Now walk all the units and notify them.  We don't care which order the units are notified.
            foreach (UndoUnit unit in _unitStack)
            {
                unit.ComponentChanging(e);
            }
        }

        private void OnComponentRemoved(object sender, ComponentEventArgs e)
        {
            foreach (UndoUnit unit in _unitStack)
            {
                unit.ComponentRemoved(e);
            }

            if (CurrentUnit != null)
            {
                CheckPopUnit(PopUnitReason.Normal);
            }

            // Now we need to raise ComponentChanged events for every component that had a reference to this removed component
            if (_refToRemovedComponent != null && _refToRemovedComponent.TryGetValue(e.Component, out List<ReferencingComponent> propsToUpdate) && propsToUpdate != null && _componentChangeService != null)
            {
                foreach (ReferencingComponent ro in propsToUpdate)
                {
                    _componentChangeService.OnComponentChanged(ro.component, ro.member, null, null);
                }
                _refToRemovedComponent.Remove(e.Component);
            }
        }

        private void OnComponentRemoving(object sender, ComponentEventArgs e)
        {
            // Open a new unit unless there is already one open or we are currently executing a unit. If we need to create a unit, we will have to fabricate a good name.
            if (_enabled && _executingUnit is null && _unitStack.Count == 0)
            {
                string name;
                if (e.Component != null)
                {
                    name = string.Format(SR.UndoEngineComponentRemove1, GetName(e.Component, true));
                }
                else
                {
                    name = SR.UndoEngineComponentRemove0;
                }
                _unitStack.Push(CreateUndoUnit(name, true));
            }

            // We need to keep track of all references in the container to the deleted component so  that those references can be fixed up if an undo of this "remove" occurs.
            if (_enabled && _host != null && _host.Container != null && _componentChangeService != null)
            {
                List<ReferencingComponent> propsToUpdate = null;
                foreach (IComponent comp in _host.Container.Components)
                {
                    if (comp == e.Component)
                    {
                        continue;
                    }
                    PropertyDescriptorCollection props = TypeDescriptor.GetProperties(comp);
                    foreach (PropertyDescriptor prop in props)
                    {
                        if (prop.PropertyType.IsAssignableFrom(e.Component.GetType()) &&
                            !prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden) &&
                            !prop.IsReadOnly)
                        {
                            object obj = null;
                            try
                            {
                                obj = prop.GetValue(comp);
                            }
                            catch (TargetInvocationException)
                            {
                                continue;
                            }

                            if (obj != null && object.ReferenceEquals(obj, e.Component))
                            {
                                if (propsToUpdate is null)
                                {
                                    propsToUpdate = new List<ReferencingComponent>();

                                    if (_refToRemovedComponent is null)
                                    {
                                        _refToRemovedComponent = new Dictionary<IComponent, List<ReferencingComponent>>();
                                    }
                                    _refToRemovedComponent[e.Component] = propsToUpdate;
                                }
                                _componentChangeService.OnComponentChanging(comp, prop);
                                propsToUpdate.Add(new ReferencingComponent(comp, prop));
                            }
                        }
                    }
                }
            }

            // Now walk all the units and notify them.  We don't care which order the units are notified.  By notifying all transactions we automatically support the cancelling of nested transactions.
            foreach (UndoUnit unit in _unitStack)
            {
                unit.ComponentRemoving(e);
            }
        }

        private void OnComponentRename(object sender, ComponentRenameEventArgs e)
        {
            // Open a new unit unless there is already one open or we are currently executing a unit. If we need to create a unit, we will have to fabricate a good name.
            if (_enabled && _executingUnit is null && _unitStack.Count == 0)
            {
                string name = string.Format(SR.UndoEngineComponentRename, e.OldName, e.NewName);
                _unitStack.Push(CreateUndoUnit(name, true));
            }

            // Now walk all the units and notify them.  We don't care which order the units are notified.  By notifying all transactions we automatically support the cancelling of nested transactions.
            foreach (UndoUnit unit in _unitStack)
            {
                unit.ComponentRename(e);
            }
        }

        private void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs e)
        {
            if (_executingUnit is null && CurrentUnit != null)
            {
                PopUnitReason reason = e.TransactionCommitted ? PopUnitReason.TransactionCommit : PopUnitReason.TransactionCancel;
                CheckPopUnit(reason);
            }
        }

        private void OnTransactionOpening(object sender, EventArgs e)
        {
            // When a transaction is opened, we always push a new unit unless we're executing a unit.  We can push multiple units onto the stack to handle nested transactions.
            if (_enabled && _executingUnit is null)
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

        [Conditional("DEBUG")]
        private static void Trace(string text, params object[] values)
        {
            Debug.WriteLineIf(s_traceUndo.TraceVerbose, "UndoEngine: " + string.Format(CultureInfo.CurrentCulture, text, values));
        }

        private enum PopUnitReason
        {
            Normal, TransactionCommit, TransactionCancel,
        }

        private struct ReferencingComponent
        {
            public IComponent component;
            public MemberDescriptor member;

            public ReferencingComponent(IComponent component, MemberDescriptor member)
            {
                this.component = component;
                this.member = member;
            }
        }

        /// <summary>
        ///  This class embodies a unit of undoable work.  The undo engine creates an undo unit when a change to the designer is about to be made.  The undo unit is responsible for tracking changes.  The undo engine will call Close on the unit when it no longer needs to track changes.
        /// </summary>
        protected class UndoUnit
        {
            private ArrayList _events; // the list of events we've captured
            private ArrayList _changeEvents; // the list of change events we're currently capturing.  Only valid until Commit is called.
            private ArrayList _removeEvents; // the list of remove events we're currently capturing.  Only valid until a matching Removed is encountered.
            private ArrayList _ignoreAddingList; // the list of objects that are currently being added.  We ignore change events between adding and added.
            private ArrayList _ignoreAddedList; // the list of objects that are added. We do not serialize before state for change events that happen in the same transaction
            private bool _reverse; // if true, we walk the events list from the bottom up
            private readonly Hashtable _lastSelection; // the selection as it was before we gathered undo info

            public UndoUnit(UndoEngine engine, string name)
            {
                if (name is null)
                {
                    name = string.Empty;
                }

                UndoEngine.Trace("Creating undo unit '{0}'", name);

                Name = name;
                UndoEngine = engine ?? throw new ArgumentNullException(nameof(engine));
                _reverse = true;
                if (UndoEngine.GetService(typeof(ISelectionService)) is ISelectionService ss)
                {
                    ICollection selection = ss.GetSelectedComponents();
                    Hashtable selectedNames = new Hashtable();
                    foreach (object sel in selection)
                    {
                        if (sel is IComponent comp && comp.Site != null)
                        {
                            selectedNames[comp.Site.Name] = comp.Site.Container;
                        }
                    }
                    _lastSelection = selectedNames;
                }
            }

            public string Name { get; }

            /// <summary>
            ///  This returns true if the undo unit has nothing in it to undo.  The unit will be discarded.
            /// </summary>
            public virtual bool IsEmpty => _events is null || _events.Count == 0;

            protected UndoEngine UndoEngine { get; }

            /// <summary>
            ///  Adds the given event to our event list.
            /// </summary>
            private void AddEvent(UndoEvent e)
            {
                if (_events is null)
                {
                    _events = new ArrayList();
                }

                _events.Add(e);
            }

            /// <summary>
            ///  Called by the undo engine when it wants to close this unit.  The unit should do any final work it needs to do to close.
            /// </summary>
            public virtual void Close()
            {
                if (_changeEvents != null)
                {
                    foreach (ChangeUndoEvent e in _changeEvents)
                    {
                        e.Commit(UndoEngine);
                    }
                }

                if (_removeEvents != null)
                {
                    foreach (AddRemoveUndoEvent e in _removeEvents)
                    {
                        e.Commit(UndoEngine);
                    }
                }

                // At close time we are done with this list.  All change events were simultaneously added to the _events list.
                _changeEvents = null;
                _removeEvents = null;
                _ignoreAddingList = null;
                _ignoreAddedList = null;
            }

            /// <summary>
            ///  The undo engine will call this on the active undo unit in response to a component added event.
            /// </summary>
            public virtual void ComponentAdded(ComponentEventArgs e)
            {
                if (e.Component.Site != null &&
                    e.Component.Site.Container is INestedContainer)
                {
                    // do nothing
                }
                else
                {
                    AddEvent(new AddRemoveUndoEvent(UndoEngine, e.Component, true));
                }

                if (_ignoreAddingList != null)
                {
                    _ignoreAddingList.Remove(e.Component);
                }

                if (_ignoreAddedList is null)
                {
                    _ignoreAddedList = new ArrayList();
                }
                _ignoreAddedList.Add(e.Component);
            }

            /// <summary>
            ///  The undo engine will call this on the active undo unit in response to a component adding event.
            /// </summary>
            public virtual void ComponentAdding(ComponentEventArgs e)
            {
                if (_ignoreAddingList is null)
                {
                    _ignoreAddingList = new ArrayList();
                }
                _ignoreAddingList.Add(e.Component);
            }

            private static bool ChangeEventsSymmetric(ComponentChangingEventArgs changing, ComponentChangedEventArgs changed)
            {
                if (changing is null || changed is null)
                {
                    return false;
                }
                return changing.Component == changed.Component && changing.Member == changed.Member;
            }

            private bool CanRepositionEvent(int startIndex, ComponentChangedEventArgs e)
            {
                bool containsAdd = false;
                bool containsRename = false;
                bool containsSymmetricChange = false;
                for (int i = startIndex + 1; i < _events.Count; i++)
                {
                    if (_events[i] is AddRemoveUndoEvent addEvt && !addEvt.NextUndoAdds)
                    {
                        containsAdd = true;
                    }
                    else if (_events[i] is ChangeUndoEvent changeEvt && ChangeEventsSymmetric(changeEvt.ComponentChangingEventArgs, e))
                    {
                        containsSymmetricChange = true;
                    }
                    else if (_events[i] is RenameUndoEvent)
                    {
                        containsRename = true;
                    }
                }
                return containsAdd && !containsRename && !containsSymmetricChange;
            }

            /// <summary>
            ///  The undo engine will call this on the active undo unit in response to a component changed event.
            /// </summary>
            public virtual void ComponentChanged(ComponentChangedEventArgs e)
            {
                if (_events != null && e != null)
                {
                    for (int i = 0; i < _events.Count; i++)
                    {
                        // Determine if we've located the UndoEvent which was  created as a result of a corresponding ComponentChanging event.
                        // If so, reposition to the "Changed" spot in the list if the following is true:
                        //          - It must be for a DSV.Content property
                        //          - There must be a AddEvent between the Changing and Changed
                        //          - There are no renames in between Changing and Changed.
                        if (_events[i] is ChangeUndoEvent ce && ChangeEventsSymmetric(ce.ComponentChangingEventArgs, e) && i != _events.Count - 1)
                        {
                            if (e.Member != null && e.Member.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content) &&
                                CanRepositionEvent(i, e))
                            {
                                _events.RemoveAt(i);
                                _events.Add(ce);
                            }
                        }
                    }
                }
            }

            /// <summary>
            ///  The undo engine will call this on the active undo unit in response to a component changing event.
            /// </summary>
            public virtual void ComponentChanging(ComponentChangingEventArgs e)
            {
                // If we are in the process of adding this component, ignore any changes to it.  The ending "Added" event will capture the component's state.  This not just an optimization.  If we get a change during an add, we can have an undo order that specifies a remove, and then a change to a removed component.
                if (_ignoreAddingList != null && _ignoreAddingList.Contains(e.Component))
                {
                    return;
                }

                if (_changeEvents is null)
                {
                    _changeEvents = new ArrayList();
                }

                // The site check here is done because the data team is calling us for components that are not yet sited.  We end up writing them out as Guid-named locals.  That's fine, except that we cannot capture after state for these types of things so we assert.
                if (UndoEngine.GetName(e.Component, false) != null)
                {
                    // The caller provided us with a component.  This is the common case.  We will add a new change event provided there is not already one open for this component.
                    bool hasChange = false;

                    for (int idx = 0; idx < _changeEvents.Count; idx++)
                    {
                        ChangeUndoEvent ce = (ChangeUndoEvent)_changeEvents[idx];
                        if (ce.OpenComponent == e.Component && ce.ContainsChange(e.Member))
                        {
                            hasChange = true;
                            break;
                        }
                    }

                    if (!hasChange ||
                        (e.Member != null && e.Member.Attributes != null && e.Member.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content)))
                    {
#if DEBUG
                        string name = UndoEngine.GetName(e.Component, false);
                        string memberName = "(none)";
                        if (e.Member != null && e.Member.Name != null)
                        {
                            memberName = e.Member.Name;
                        }
                        if (name != null)
                        {
                            Debug.WriteLineIf(s_traceUndo.TraceVerbose && hasChange, "Adding second ChangeEvent for " + name + " Member: " + memberName);
                        }
                        else
                        {
                            Debug.Fail("UndoEngine: GetName is failing on successive calls");
                        }
#endif
                        ChangeUndoEvent changeEvent = null;
                        bool serializeBeforeState = true;
                        //perf: if this object was added in this undo unit we do not want to serialize before state for ChangeEvent since undo will remove it anyway
                        if (_ignoreAddedList != null && _ignoreAddedList.Contains(e.Component))
                        {
                            serializeBeforeState = false;
                        }

                        if (e.Component is IComponent comp && comp.Site != null)
                        {
                            changeEvent = new ChangeUndoEvent(UndoEngine, e, serializeBeforeState);
                        }
                        else if (e.Component != null)
                        {
                            if (GetService(typeof(IReferenceService)) is IReferenceService rs)
                            {
                                IComponent owningComp = rs.GetComponent(e.Component);

                                if (owningComp != null)
                                {
                                    changeEvent = new ChangeUndoEvent(UndoEngine, new ComponentChangingEventArgs(owningComp, null), serializeBeforeState);
                                }
                            }
                        }

                        if (changeEvent != null)
                        {
                            AddEvent(changeEvent);
                            _changeEvents.Add(changeEvent);
                        }
                    }
                }
            }

            /// <summary>
            ///  The undo engine will call this on the active undo unit in response to a component removed event.
            /// </summary>
            public virtual void ComponentRemoved(ComponentEventArgs e)
            {
                // We should gather undo state in ComponentRemoved, but by this time the component's designer has been destroyed so it's too late.  Instead, we captured state in the Removing method.  But, it is possible for there to be component changes to other objects that happen between removing and removed,  so we need to reorder the removing event so it's positioned after any changes.
                if (_events != null && e != null)
                {
                    ChangeUndoEvent changeEvt = null;
                    int changeIdx = -1;
                    for (int idx = _events.Count - 1; idx >= 0; idx--)
                    {
                        if (changeEvt is null)
                        {
                            changeEvt = _events[idx] as ChangeUndoEvent;
                            changeIdx = idx;
                        }

                        if (_events[idx] is AddRemoveUndoEvent evt && evt.OpenComponent == e.Component)
                        {
                            evt.Commit(UndoEngine);
                            // We should only reorder events if there  are change events coming between OnRemoving and OnRemoved.
                            // If there are other events (such as AddRemoving), the serialization  done in OnComponentRemoving might refer to components that aren't available.
                            if (idx != _events.Count - 1 && changeEvt != null)
                            {
                                // ensure only change change events exist between these two events
                                bool onlyChange = true;
                                for (int i = idx + 1; i < changeIdx; i++)
                                {
                                    if (!(_events[i] is ChangeUndoEvent))
                                    {
                                        onlyChange = false;
                                        break;
                                    }
                                }

                                if (onlyChange)
                                {
                                    // reposition event after final ComponentChangingEvent
                                    _events.RemoveAt(idx);
                                    _events.Insert(changeIdx, evt);
                                }
                            }
                            break;
                        }
                    }
                }
            }

            /// <summary>
            ///  The undo engine will call this on the active undo unit in response to a component removing event.
            /// </summary>
            public virtual void ComponentRemoving(ComponentEventArgs e)
            {
                if (e.Component.Site != null &&
                    e.Component.Site is INestedContainer)
                {
                    return;
                }

                if (_removeEvents is null)
                {
                    _removeEvents = new ArrayList();
                }
                try
                {
                    AddRemoveUndoEvent evt = new AddRemoveUndoEvent(UndoEngine, e.Component, false);
                    AddEvent(evt);
                    _removeEvents.Add(evt);
                }
                catch (TargetInvocationException) { }
            }

            /// <summary>
            ///  The undo engine will cal this on the active undo unit in response to a component rename event.
            /// </summary>
            public virtual void ComponentRename(ComponentRenameEventArgs e)
            {
                AddEvent(new RenameUndoEvent(e.OldName, e.NewName));
            }

            /// <summary>
            ///  Returns an instance of the rquested service.
            /// </summary>
            protected object GetService(Type serviceType)
            {
                return UndoEngine.GetService(serviceType);
            }

            /// <summary>
            ///  Override for object.ToString()
            /// </summary>
            public override string ToString()
            {
                return Name;
            }

            /// <summary>
            ///  Either performs undo, or redo, depending on the state of the unit.  UndoUnit initially assumes that the undoable work has already been "done", so the first call to undo will undo the work.  The next call will undo the "undo", performing a redo.
            /// </summary>
            public void Undo()
            {
                UndoEngine.Trace("Performing undo '{0}'", Name);
                UndoUnit savedUnit = UndoEngine._executingUnit;
                UndoEngine._executingUnit = this;
                DesignerTransaction transaction = null;
                try
                {
                    if (savedUnit is null)
                    {
                        UndoEngine.OnUndoing(EventArgs.Empty);
                    }
                    // create a transaction here so things that do work on componentchanged can ignore that while the transaction is opened...big perf win.
                    transaction = UndoEngine._host.CreateTransaction();
                    UndoCore();
                }
                catch (CheckoutException)
                {
                    transaction.Cancel();
                    transaction = null;
                    throw;
                }
                finally
                {
                    if (transaction != null)
                    {
                        transaction.Commit();
                    }

                    UndoEngine._executingUnit = savedUnit;
                    if (savedUnit is null)
                    {
                        UndoEngine.OnUndone(EventArgs.Empty);
                    }
                }
            }

            /// <summary>
            ///  The undo method invokes this method to perform the actual undo / redo work.  You should never call this method directly; override it if you wish, but always call the public Undo method to perform undo work.  Undo notifies the undo engine to suspend undo data gathering until  this undo is completed, which prevents new undo units from being created in response to this unit doing work.
            /// </summary>
            protected virtual void UndoCore()
            {
                if (_events != null)
                {
                    if (_reverse)
                    {
                        // How does BeforeUndo work?  You'd think you should just call this in one pass, and then call Undo in another, but you'd be wrong. The complexity arises because there are undo events that have dependencies on other undo events.  There are also undo events that have side effects with respect to other events.  Here are examples:
                        // Rename is an undo event that other undo events depend on, because they store names.  It must be performed in the right order and it must be  performed before any subsequent event's BeforeUndo is called.
                        // Property change is an undo event that may have an unknown side effect if changing the property results in other property changes (for example, reparenting a control removes the control from its former parent).  A property change undo event must have all BeforeUndo methods called before any Undo method is called. To do this, we have a property on UndoEvent called CausesSideEffects.
                        // As we run through UndoEvents, consecutive events that return true from this property are grouped so that their BeforeUndo methods are all called before their Undo methods.  For events that do not have  side effects, their BeforeUndo and Undo are invoked immediately.
                        for (int idx = _events.Count - 1; idx >= 0; idx--)
                        {
                            int groupEndIdx = idx;
                            for (int groupIdx = idx; groupIdx >= 0; groupIdx--)
                            {
                                if (((UndoEvent)_events[groupIdx]).CausesSideEffects)
                                {
                                    groupEndIdx = groupIdx;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            for (int beforeIdx = idx; beforeIdx >= groupEndIdx; beforeIdx--)
                            {
                                ((UndoEvent)_events[beforeIdx]).BeforeUndo(UndoEngine);
                            }

                            for (int undoIdx = idx; undoIdx >= groupEndIdx; undoIdx--)
                            {
                                ((UndoEvent)_events[undoIdx]).Undo(UndoEngine);
                            }

                            Debug.Assert(idx >= groupEndIdx, "We're going backwards");
                            idx = groupEndIdx;
                        }

                        // Now, if we have a selection, apply it.
                        if (_lastSelection != null)
                        {
                            if (UndoEngine.GetService(typeof(ISelectionService)) is ISelectionService ss)
                            {
                                string[] names = new string[_lastSelection.Keys.Count];
                                _lastSelection.Keys.CopyTo(names, 0);
                                ArrayList list = new ArrayList(names.Length);
                                foreach (string name in names)
                                {
                                    if (name != null)
                                    {
                                        object comp = ((Container)_lastSelection[name]).Components[name];
                                        if (comp != null)
                                        {
                                            list.Add(comp);
                                        }
                                    }
                                }
                                ss.SetSelectedComponents(list, SelectionTypes.Replace);
                            }
                        }
                    }
                    else
                    {
                        int count = _events.Count;
                        for (int idx = 0; idx < count; idx++)
                        {
                            int groupEndIdx = idx;

                            for (int groupIdx = idx; groupIdx < count; groupIdx++)
                            {
                                if (((UndoEvent)_events[groupIdx]).CausesSideEffects)
                                {
                                    groupEndIdx = groupIdx;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            for (int beforeIdx = idx; beforeIdx <= groupEndIdx; beforeIdx++)
                            {
                                ((UndoEvent)_events[beforeIdx]).BeforeUndo(UndoEngine);
                            }

                            for (int undoIdx = idx; undoIdx <= groupEndIdx; undoIdx++)
                            {
                                ((UndoEvent)_events[undoIdx]).Undo(UndoEngine);
                            }

                            Debug.Assert(idx <= groupEndIdx, "We're going backwards");
                            idx = groupEndIdx;
                        }
                    }
                }
                _reverse = !_reverse;
            }

            /// <summary>
            ///  This undo event handles addition and removal of components.
            /// </summary>
            private sealed class AddRemoveUndoEvent : UndoEvent
            {
                private readonly SerializationStore _serializedData;
                private readonly string _componentName;
                private bool _nextUndoAdds;
                private bool _committed;
                private readonly IComponent _openComponent;

                /// <summary>
                ///  Creates a new object that contains the state of the event.  The last parameter, add, determines the initial mode of this event.  If true, it means this event is being created in response to a component add.  If false, it is being created in response to   a component remove.
                /// </summary>
                public AddRemoveUndoEvent(UndoEngine engine, IComponent component, bool add)
                {
                    _componentName = component.Site.Name;
                    _nextUndoAdds = !add;
                    _openComponent = component;

                    UndoEngine.Trace("---> Creating {0} undo event for '{1}'", (add ? "Add" : "Remove"), _componentName);
                    using (_serializedData = engine._serializationService.CreateStore())
                    {
                        engine._serializationService.Serialize(_serializedData, component);
                    }
                    // For add events, we commit as soon as we receive the event.
                    _committed = add;
                }

                /// <summary>
                ///  Returns true if the add remove event has been comitted.
                /// </summary>
                internal bool Committed
                {
                    get => _committed;
                }

                /// <summary>
                ///  If this add/remove event is still open, OpenCompnent will contain the component it is operating on.
                /// </summary>
                internal IComponent OpenComponent
                {
                    get => _openComponent;
                }

                /// <summary>
                ///  Returns true if undoing this event will add a component.
                /// </summary>
                internal bool NextUndoAdds
                {
                    get => _nextUndoAdds;
                }

                /// <summary>
                ///  Commits this event.
                /// </summary>
                internal void Commit(UndoEngine engine)
                {
                    if (!Committed)
                    {
                        UndoEngine.Trace("---> Committing remove of '{0}'", _componentName);
                        _committed = true;
                    }
                }

                /// <summary>
                ///  Actually performs the undo action.
                /// </summary>
                public override void Undo(UndoEngine engine)
                {
                    if (_nextUndoAdds)
                    {
                        UndoEngine.Trace("---> Adding '{0}'", _componentName);
                        // We need to add this component.  To add it, we deserialize it and then we add it to the designer host's container.
                        if (engine.GetRequiredService(typeof(IDesignerHost)) is IDesignerHost host)
                        {
                            engine._serializationService.DeserializeTo(_serializedData, host.Container);
                        }
                    }
                    else
                    {
                        UndoEngine.Trace("---> Removing '{0}'", _componentName);
                        // We need to remove this component.  Take the name and match it to an object, and then ask that object to delete itself.
                        IDesignerHost host = engine.GetRequiredService(typeof(IDesignerHost)) as IDesignerHost;

                        IComponent component = host.Container.Components[_componentName];

                        // Note: It's ok for the component to be null here.  This could happen if the parent to this control is disposed first. Ex:SplitContainer
                        if (component != null)
                        {
                            host.DestroyComponent(component);
                        }
                    }
                    _nextUndoAdds = !_nextUndoAdds;
                }
            }

            private sealed class ChangeUndoEvent : UndoEvent
            {
                // This is only valid while the change is still open. The change is committed.
                private object _openComponent;
                // Static data we hang onto about this change.
                private readonly string _componentName;
                private readonly MemberDescriptor _member;
                // Before and after state.  Before state is built in the constructor. After state is built right before we undo for the first time.
                private SerializationStore _before;
                private SerializationStore _after;
                private bool _savedAfterState;

                /// <summary>
                ///  Creates a new component change undo event.  This event consists of a before and after snapshot of a single component.  A snapshot will not be taken if a name for the component cannot be determined.
                /// </summary>
                public ChangeUndoEvent(UndoEngine engine, ComponentChangingEventArgs e, bool serializeBeforeState)
                {
                    _componentName = engine.GetName(e.Component, true);
                    _openComponent = e.Component;
                    _member = e.Member;

                    UndoEngine.Trace("---> Creating change undo event for '{0}'", _componentName);
                    UndoEngine.Trace("---> Saving before snapshot for change to '{0}'", _componentName);
                    if (serializeBeforeState)
                    {
                        _before = Serialize(engine, _openComponent, _member);
                    }
                }

                public ComponentChangingEventArgs ComponentChangingEventArgs
                {
                    get => new ComponentChangingEventArgs(_openComponent, _member);
                }

                /// <summary>
                ///  Indicates that undoing this event may cause side effects in other objects.
                ///  Chagne events fall into this category because, for example, a change involving adding an object to one collection may have a side effect of removing it from another collection.  Events with side effects are grouped at undo time so all their BeforeUndo methods are called before their Undo methods.
                ///  Events without side effects have their BeforeUndo called and then their Undo called immediately after.
                /// </summary>
                public override bool CausesSideEffects { get { return true; } }

                /// <summary>
                ///  Returns true if the change event has been comitted.
                /// </summary>
                public bool Committed
                {
                    get
                    {
                        return _openComponent is null;
                    }
                }

                /// <summary>
                ///  Returns the component this change event is currently tracking. This will return null once the change event is committed.
                /// </summary>
                public object OpenComponent
                {
                    get
                    {
                        return _openComponent;
                    }
                }

                /// <summary>
                ///  Called before Undo is called. All undo events get their BeforeUndo called, and then they all get their Undo called. This allows the undo event to examine the state of the world before other undo events mess with it.
                /// </summary>
                public override void BeforeUndo(UndoEngine engine)
                {
                    if (!_savedAfterState)
                    {
                        _savedAfterState = true;
                        SaveAfterState(engine);
                    }
                }

                /// <summary>
                ///  Determines if this
                /// </summary>
                public bool ContainsChange(MemberDescriptor desc)
                {
                    if (_member is null)
                    {
                        return true;
                    }

                    if (desc is null)
                    {
                        return false;
                    }

                    return desc.Equals(_member);
                }

                /// <summary>
                ///  Commits the unit.  Comitting the unit saves the "after" snapshot of the unit.  If commit is called multiple times only the first commit is registered.
                /// </summary>
                public void Commit(UndoEngine engine)
                {
                    if (!Committed)
                    {
                        UndoEngine.Trace("---> Committing change to '{0}'", _componentName);
                        _openComponent = null;
                    }
                }

                private void SaveAfterState(UndoEngine engine)
                {
                    Debug.Assert(_after is null, "Change undo saving state twice.");
                    UndoEngine.Trace("---> Saving after snapshot for change to '{0}'", _componentName);
                    object component = null;

                    if (engine.GetService(typeof(IReferenceService)) is IReferenceService rs)
                    {
                        component = rs.GetReference(_componentName);
                    }
                    else
                    {
                        if (engine.GetService(typeof(IDesignerHost)) is IDesignerHost host)
                        {
                            component = host.Container.Components[_componentName];
                        }
                    }

                    // It is OK for us to not find a component here.  That can happen if our "after" state is owned by another change, like an add of the component.
                    if (component != null)
                    {
                        _after = Serialize(engine, component, _member);
                    }
                }

                private SerializationStore Serialize(UndoEngine engine, object component, MemberDescriptor member)
                {
                    SerializationStore store;
                    using (store = engine._serializationService.CreateStore())
                    {
                        if (member != null && !(member.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden)))
                        {
                            engine._serializationService.SerializeMemberAbsolute(store, component, member);
                        }
                        else
                        {
                            engine._serializationService.SerializeAbsolute(store, component);
                        }
                    }
                    return store;
                }

                /// <summary>
                ///  Performs the actual undo.  AFter it finishes it will reverse the role of _before and _after
                /// </summary>
                public override void Undo(UndoEngine engine)
                {
                    UndoEngine.Trace("---> Applying changes to '{0}'", _componentName);
                    Debug.Assert(_savedAfterState, "After state not saved.  BeforeUndo was not called?");

                    if (_before != null)
                    {
                        if (engine.GetService(typeof(IDesignerHost)) is IDesignerHost host)
                        {
                            engine._serializationService.DeserializeTo(_before, host.Container);
                        }
                    }

                    SerializationStore temp = _after;
                    _after = _before;
                    _before = temp;
                }
            }

            private sealed class RenameUndoEvent : UndoEvent
            {
                private string _before;
                private string _after;

                /// <summary>
                ///  Creates a new rename undo event.
                /// </summary>
                public RenameUndoEvent(string before, string after)
                {
                    _before = before;
                    _after = after;
                    UndoEngine.Trace("---> Creating rename undo event for '{0}'->'{1}'", _before, _after);
                }

                /// <summary>
                ///  Simply undoes a rename by setting the name back to the saved value.
                /// </summary>
                public override void Undo(UndoEngine engine)
                {
                    UndoEngine.Trace("---> Renaming '{0}'->'{1}'", _after, _before);
                    IComponent comp = engine._host.Container.Components[_after];
                    if (comp != null)
                    {
                        engine.ComponentChangeService.OnComponentChanging(comp, null);
                        comp.Site.Name = _before;
                        string temp = _after;
                        _after = _before;
                        _before = temp;
                    }
                }
            }

            private abstract class UndoEvent
            {
                /// <summary>
                ///  Indicates that undoing this event may cause side effects in other objects.
                ///  Chagne events fall into this category because, for example, a change involving adding an object to one collection may have a side effect of removing it from another collection.
                ///  Events with side effects are grouped at undo time so all their BeforeUndo methods are called before their Undo methods.
                ///  Events without side effects have their BeforeUndo called and then their Undo called immediately after.
                /// </summary>
                public virtual bool CausesSideEffects { get { return false; } }

                /// <summary>
                ///  Called before Undo is called.  All undo events get their BeforeUndo called, and then they all get their Undo called. This allows the undo event to examine the state of the world before other undo events mess with it. BeforeUndo returns true if before undo was supported, and false if not.  If before undo is not supported, the undo unit should be undone immediately.
                /// </summary>
                public virtual void BeforeUndo(UndoEngine engine)
                {
                }

                /// <summary>
                ///  Called by the undo unit when it wants to undo this bit of work.
                /// </summary>
                public abstract void Undo(UndoEngine engine);
            }
        }
    }
}
