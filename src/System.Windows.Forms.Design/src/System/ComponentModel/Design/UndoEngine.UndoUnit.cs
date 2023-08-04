// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.Windows.Forms;

namespace System.ComponentModel.Design;

public abstract partial class UndoEngine
{
    /// <summary>
    ///  This class embodies a unit of undoable work.  The undo engine creates an undo unit when a change to the designer is about to be made.  The undo unit is responsible for tracking changes.  The undo engine will call Close on the unit when it no longer needs to track changes.
    /// </summary>
    protected class UndoUnit
    {
        private List<UndoEvent>? _events; // the list of events we've captured
        private List<ChangeUndoEvent>? _changeEvents; // the list of change events we're currently capturing.  Only valid until Commit is called.
        private List<AddRemoveUndoEvent>? _removeEvents; // the list of remove events we're currently capturing.  Only valid until a matching Removed is encountered.
        private List<IComponent>? _ignoreAddingList; // the list of objects that are currently being added.  We ignore change events between adding and added.
        private List<IComponent>? _ignoreAddedList; // the list of objects that are added. We do not serialize before state for change events that happen in the same transaction
        private bool _reverse; // if true, we walk the events list from the bottom up
        private readonly Dictionary<string, IContainer>? _lastSelection; // the selection as it was before we gathered undo info

        public UndoUnit(UndoEngine engine, string? name)
        {
            Name = name ?? string.Empty;

            Debug.WriteLineIf(s_traceUndo.TraceVerbose, $"UndoEngine: Creating undo unit '{Name}'");

            UndoEngine = engine.OrThrowIfNull();
            _reverse = true;
            if (UndoEngine.TryGetService(out ISelectionService? ss))
            {
                ICollection selection = ss.GetSelectedComponents();
                Dictionary<string, IContainer> selectedNames = new();
                foreach (object sel in selection)
                {
                    if (sel is IComponent { Site: ISite site })
                    {
                        selectedNames[site.Name!] = site.Container!;
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
            _events ??= new();

            _events.Add(e);
        }

        /// <summary>
        ///  Called by the undo engine when it wants to close this unit.  The unit should do any final work it needs to do to close.
        /// </summary>
        public virtual void Close()
        {
            if (_changeEvents is not null)
            {
                foreach (ChangeUndoEvent e in _changeEvents)
                {
                    e.Commit(UndoEngine);
                }
            }

            if (_removeEvents is not null)
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
            if (e.Component!.Site?.Container is INestedContainer)
            {
                // do nothing
            }
            else
            {
                AddEvent(new AddRemoveUndoEvent(UndoEngine, e.Component, true));
            }

            _ignoreAddingList?.Remove(e.Component);

            _ignoreAddedList ??= new();

            _ignoreAddedList.Add(e.Component);
        }

        /// <summary>
        ///  The undo engine will call this on the active undo unit in response to a component adding event.
        /// </summary>
        public virtual void ComponentAdding(ComponentEventArgs e)
        {
            _ignoreAddingList ??= new();

            _ignoreAddingList.Add(e.Component!);
        }

        private static bool ChangeEventsSymmetric(
            [NotNullWhen(true)] ComponentChangingEventArgs? changing,
            [NotNullWhen(true)] ComponentChangedEventArgs? changed)
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
            for (int i = startIndex + 1; i < _events!.Count; i++)
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
            if (_events is not null && e is not null)
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
                        if (e.Member is not null && e.Member.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content) &&
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
            if (_ignoreAddingList is not null && _ignoreAddingList.Contains(e.Component))
            {
                return;
            }

            _changeEvents ??= new();

            // The site check here is done because the data team is calling us for components that are not yet sited.  We end up writing them out as Guid-named locals.  That's fine, except that we cannot capture after state for these types of things so we assert.
            if (UndoEngine.GetName(e.Component, false) is not null)
            {
                // The caller provided us with a component.  This is the common case.  We will add a new change event provided there is not already one open for this component.
                bool hasChange = false;

                for (int idx = 0; idx < _changeEvents.Count; idx++)
                {
                    ChangeUndoEvent ce = _changeEvents[idx];
                    if (ce.OpenComponent == e.Component && ce.ContainsChange(e.Member))
                    {
                        hasChange = true;
                        break;
                    }
                }

                if (!hasChange ||
                    (e.Member?.Attributes is not null && e.Member.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content)))
                {
#if DEBUG
                    string? name = UndoEngine.GetName(e.Component, false);

                    if (name is not null)
                    {
                        string memberName = e.Member?.Name ?? "(none)";
                        Debug.WriteLineIf(s_traceUndo.TraceVerbose && hasChange, $"Adding second ChangeEvent for {name} Member: {memberName}");
                    }
                    else
                    {
                        Debug.Fail("UndoEngine: GetName is failing on successive calls");
                    }
#endif
                    ChangeUndoEvent? changeEvent = null;
                    bool serializeBeforeState = true;
                    //perf: if this object was added in this undo unit we do not want to serialize before state for ChangeEvent since undo will remove it anyway
                    if (_ignoreAddedList is not null && _ignoreAddedList.Contains(e.Component))
                    {
                        serializeBeforeState = false;
                    }

                    if (e.Component is IComponent { Site: not null })
                    {
                        changeEvent = new ChangeUndoEvent(UndoEngine, e, serializeBeforeState);
                    }
                    else if (e.Component is not null)
                    {
                        if (GetService(typeof(IReferenceService)) is IReferenceService rs)
                        {
                            IComponent? owningComp = rs.GetComponent(e.Component);

                            if (owningComp is not null)
                            {
                                changeEvent = new ChangeUndoEvent(UndoEngine, new ComponentChangingEventArgs(owningComp, null), serializeBeforeState);
                            }
                        }
                    }

                    if (changeEvent is not null)
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
            if (_events is not null && e is not null)
            {
                ChangeUndoEvent? changeEvt = null;
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
                        if (idx != _events.Count - 1 && changeEvt is not null)
                        {
                            // ensure only change change events exist between these two events
                            bool onlyChange = true;
                            for (int i = idx + 1; i < changeIdx; i++)
                            {
                                if (_events[i] is not ChangeUndoEvent)
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
            if (e.Component!.Site is INestedContainer)
            {
                return;
            }

            _removeEvents ??= new();

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
        public virtual void ComponentRename(ComponentRenameEventArgs e) =>
            AddEvent(new RenameUndoEvent(e.OldName, e.NewName));

        /// <summary>
        ///  Returns an instance of the requested service.
        /// </summary>
        protected object? GetService(Type serviceType) => UndoEngine.GetService(serviceType);

        /// <summary>
        ///  Override for object.ToString()
        /// </summary>
        public override string ToString() => Name;

        /// <summary>
        ///  Either performs undo, or redo, depending on the state of the unit.  UndoUnit initially assumes that the undoable work has already been "done", so the first call to undo will undo the work.  The next call will undo the "undo", performing a redo.
        /// </summary>
        public void Undo()
        {
            Debug.WriteLineIf(s_traceUndo.TraceVerbose, $"UndoEngine: Performing undo '{Name}'");
            UndoUnit? savedUnit = UndoEngine._executingUnit;
            UndoEngine._executingUnit = this;
            DesignerTransaction? transaction = null;
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
                transaction!.Cancel();
                transaction = null;
                throw;
            }
            finally
            {
                transaction?.Commit();

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
            if (_events is not null)
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
                            if (_events[groupIdx].CausesSideEffects)
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
                            (_events[beforeIdx]).BeforeUndo(UndoEngine);
                        }

                        for (int undoIdx = idx; undoIdx >= groupEndIdx; undoIdx--)
                        {
                            (_events[undoIdx]).Undo(UndoEngine);
                        }

                        Debug.Assert(idx >= groupEndIdx, "We're going backwards");
                        idx = groupEndIdx;
                    }

                    // Now, if we have a selection, apply it.
                    if (_lastSelection is not null)
                    {
                        if (UndoEngine.TryGetService(out ISelectionService? ss))
                        {
                            List<IComponent> list = new(_lastSelection.Count);
                            foreach ((string name, IContainer container) in _lastSelection)
                            {
                                IComponent? comp = container.Components[name];
                                if (comp is not null)
                                {
                                    list.Add(comp);
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
                            if (_events[groupIdx].CausesSideEffects)
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
                            (_events[beforeIdx]).BeforeUndo(UndoEngine);
                        }

                        for (int undoIdx = idx; undoIdx <= groupEndIdx; undoIdx++)
                        {
                            (_events[undoIdx]).Undo(UndoEngine);
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
            private readonly string? _componentName;

            /// <summary>
            ///  Creates a new object that contains the state of the event.  The last parameter, add, determines the initial mode of this event.  If true, it means this event is being created in response to a component add.  If false, it is being created in response to   a component remove.
            /// </summary>
            public AddRemoveUndoEvent(UndoEngine engine, IComponent component, bool add)
            {
                _componentName = component.Site!.Name;
                NextUndoAdds = !add;
                OpenComponent = component;

                Debug.WriteLineIf(s_traceUndo.TraceVerbose, $"UndoEngine: ---> Creating {(add ? "Add" : "Remove")} undo event for '{_componentName}'");
                using (_serializedData = engine._serializationService.CreateStore())
                {
                    engine._serializationService.Serialize(_serializedData, component);
                }

                // For add events, we commit as soon as we receive the event.
                Committed = add;
            }

            /// <summary>
            ///  Returns true if the add remove event has been comitted.
            /// </summary>
            internal bool Committed { get; private set; }

            /// <summary>
            ///  If this add/remove event is still open, OpenComponent will contain the component it is operating on.
            /// </summary>
            internal IComponent OpenComponent { get; }

            /// <summary>
            ///  Returns true if undoing this event will add a component.
            /// </summary>
            internal bool NextUndoAdds { get; private set; }

            /// <summary>
            ///  Commits this event.
            /// </summary>
            internal void Commit(UndoEngine engine)
            {
                if (!Committed)
                {
                    Debug.WriteLineIf(s_traceUndo.TraceVerbose, $"UndoEngine: ---> Committing remove of '{_componentName}'");
                    Committed = true;
                }
            }

            /// <summary>
            ///  Actually performs the undo action.
            /// </summary>
            public override void Undo(UndoEngine engine)
            {
                if (NextUndoAdds)
                {
                    Debug.WriteLineIf(s_traceUndo.TraceVerbose, $"UndoEngine: ---> Adding '{_componentName}'");
                    // We need to add this component.  To add it, we deserialize it and then we add it to the designer host's container.
                    if (engine.GetRequiredService(typeof(IDesignerHost)) is IDesignerHost host)
                    {
                        engine._serializationService.DeserializeTo(_serializedData, host.Container);
                    }
                }
                else
                {
                    Debug.WriteLineIf(s_traceUndo.TraceVerbose, $"UndoEngine: ---> Removing '{_componentName}'");
                    // We need to remove this component.  Take the name and match it to an object, and then ask that object to delete itself.
                    IDesignerHost host = engine.GetRequiredService<IDesignerHost>();

                    IComponent? component = host.Container.Components[_componentName];

                    // Note: It's ok for the component to be null here.  This could happen if the parent to this control is disposed first. Ex:SplitContainer
                    if (component is not null)
                    {
                        host.DestroyComponent(component);
                    }
                }

                NextUndoAdds = !NextUndoAdds;
            }
        }

        private sealed class ChangeUndoEvent : UndoEvent
        {
            // Static data we hang onto about this change.
            private readonly string _componentName;
            private readonly MemberDescriptor? _member;
            // Before and after state.  Before state is built in the constructor. After state is built right before we undo for the first time.
            private SerializationStore? _before;
            private SerializationStore? _after;
            private bool _savedAfterState;

            /// <summary>
            ///  Creates a new component change undo event.  This event consists of a before and after snapshot of a single component.  A snapshot will not be taken if a name for the component cannot be determined.
            /// </summary>
            public ChangeUndoEvent(UndoEngine engine, ComponentChangingEventArgs e, bool serializeBeforeState)
            {
                _componentName = engine.GetName(e.Component, true)!;
                OpenComponent = e.Component;
                _member = e.Member;

                Debug.WriteLineIf(s_traceUndo.TraceVerbose, $"UndoEngine: ---> Creating change undo event for '{_componentName}'");
                if (serializeBeforeState)
                {
                    Debug.WriteLineIf(s_traceUndo.TraceVerbose, $"UndoEngine: ---> Saving before snapshot for change to '{_componentName}'");
                    _before = Serialize(engine, OpenComponent!, _member);
                }
            }

            public ComponentChangingEventArgs ComponentChangingEventArgs => new(OpenComponent, _member);

            /// <summary>
            ///  Indicates that undoing this event may cause side effects in other objects.
            ///  Change events fall into this category because, for example, a change involving adding an object to one collection may have a side effect of removing it from another collection.  Events with side effects are grouped at undo time so all their BeforeUndo methods are called before their Undo methods.
            ///  Events without side effects have their BeforeUndo called and then their Undo called immediately after.
            /// </summary>
            public override bool CausesSideEffects => true;

            /// <summary>
            ///  Returns true if the change event has been comitted.
            /// </summary>
            [MemberNotNullWhen(false, nameof(OpenComponent))]
            public bool Committed => OpenComponent is null;

            /// <summary>
            ///  Returns the component this change event is currently tracking. This will return null once the change event is committed.
            /// </summary>
            public object? OpenComponent { get; private set; }

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
            public bool ContainsChange(MemberDescriptor? desc)
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
            ///  Commits the unit.  Committing the unit saves the "after" snapshot of the unit.  If commit is called multiple times only the first commit is registered.
            /// </summary>
            public void Commit(UndoEngine engine)
            {
                if (!Committed)
                {
                    Debug.WriteLineIf(s_traceUndo.TraceVerbose, $"UndoEngine: ---> Committing change to '{_componentName}'");
                    OpenComponent = null;
                }
            }

            private void SaveAfterState(UndoEngine engine)
            {
                Debug.Assert(_after is null, "Change undo saving state twice.");
                Debug.WriteLineIf(s_traceUndo.TraceVerbose, $"UndoEngine: ---> Saving after snapshot for change to '{_componentName}'");
                object? component = null;

                if (engine.TryGetService(out IReferenceService? rs))
                {
                    component = rs.GetReference(_componentName);
                }
                else if (engine.TryGetService(out IDesignerHost? host))
                {
                    component = host.Container.Components[_componentName];
                }

                // It is OK for us to not find a component here.  That can happen if our "after" state is owned by another change, like an add of the component.
                if (component is not null)
                {
                    _after = Serialize(engine, component, _member);
                }
            }

            private static SerializationStore Serialize(UndoEngine engine, object component, MemberDescriptor? member)
            {
                SerializationStore store;
                using (store = engine._serializationService.CreateStore())
                {
                    if (member is not null && !(member.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden)))
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
                Debug.WriteLineIf(s_traceUndo.TraceVerbose, $"UndoEngine: ---> Applying changes to '{_componentName}'");
                Debug.Assert(_savedAfterState, "After state not saved.  BeforeUndo was not called?");

                if (_before is not null)
                {
                    if (engine.TryGetService(out IDesignerHost? host))
                    {
                        engine._serializationService.DeserializeTo(_before, host.Container);
                    }
                }

                (_after, _before) = (_before, _after);
            }
        }

        private sealed class RenameUndoEvent : UndoEvent
        {
            private string? _before;
            private string? _after;

            /// <summary>
            ///  Creates a new rename undo event.
            /// </summary>
            public RenameUndoEvent(string? before, string? after)
            {
                _before = before;
                _after = after;
                Debug.WriteLineIf(s_traceUndo.TraceVerbose, $"UndoEngine: ---> Creating rename undo event for '{_before}'->'{_after}'");
            }

            /// <summary>
            ///  Simply undoes a rename by setting the name back to the saved value.
            /// </summary>
            public override void Undo(UndoEngine engine)
            {
                Debug.WriteLineIf(s_traceUndo.TraceVerbose, $"UndoEngine: ---> Renaming '{_after}'->'{_before}'");
                IComponent? comp = engine._host.Container.Components[_after];
                if (comp is not null)
                {
                    engine.ComponentChangeService.OnComponentChanging(comp, null);
                    comp.Site!.Name = _before;
                    (_after, _before) = (_before, _after);
                }
            }
        }

        private abstract class UndoEvent
        {
            /// <summary>
            ///  Indicates that undoing this event may cause side effects in other objects.
            ///  Change events fall into this category because, for example, a change involving adding an object to one collection may have a side effect of removing it from another collection.
            ///  Events with side effects are grouped at undo time so all their BeforeUndo methods are called before their Undo methods.
            ///  Events without side effects have their BeforeUndo called and then their Undo called immediately after.
            /// </summary>
            public virtual bool CausesSideEffects => false;

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
