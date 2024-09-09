// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;

namespace System.ComponentModel.Design;

public abstract partial class UndoEngine
{
    protected partial class UndoUnit
    {
        private sealed class ChangeUndoEvent : UndoEvent
        {
            // Static data we hang onto about this change.
            private readonly string _componentName;
            private readonly MemberDescriptor? _member;
            // Before and after state. Before state is built in the constructor.
            // After state is built right before we undo for the first time.
            private SerializationStore? _before;
            private SerializationStore? _after;
            private bool _savedAfterState;

            /// <summary>
            ///  Creates a new component change undo event.
            ///  This event consists of a before and after snapshot of a single component.
            ///  A snapshot will not be taken if a name for the component cannot be determined.
            /// </summary>
            public ChangeUndoEvent(UndoEngine engine, ComponentChangingEventArgs e, bool serializeBeforeState)
            {
                _componentName = engine.GetName(e.Component, true)!;
                OpenComponent = e.Component;
                _member = e.Member;

                if (serializeBeforeState)
                {
                    _before = Serialize(engine, OpenComponent!, _member);
                }
            }

            public ComponentChangingEventArgs ComponentChangingEventArgs => new(OpenComponent, _member);

            /// <summary>
            ///  Indicates that undoing this event may cause side effects in other objects.
            ///  Change events fall into this category because, for example, a change involving adding an object
            ///  to one collection may have a side effect of removing it from another collection.
            ///  Events with side effects are grouped at undo time so all their BeforeUndo methods
            ///  are called before their Undo methods.
            ///  Events without side effects have their BeforeUndo called and then their Undo called immediately after.
            /// </summary>
            public override bool CausesSideEffects => true;

            /// <summary>
            ///  Returns true if the change event has been comitted.
            /// </summary>
            [MemberNotNullWhen(false, nameof(OpenComponent))]
            public bool Committed => OpenComponent is null;

            /// <summary>
            ///  Returns the component this change event is currently tracking.
            ///  This will return null once the change event is committed.
            /// </summary>
            public object? OpenComponent { get; private set; }

            /// <summary>
            ///  Called before Undo is called. All undo events get their BeforeUndo called,
            ///  and then they all get their Undo called. This allows the undo event to examine
            ///  the state of the world before other undo events mess with it.
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
            ///  Commits the unit. Committing the unit saves the "after" snapshot of the unit.
            ///  If commit is called multiple times only the first commit is registered.
            /// </summary>
            public void Commit()
            {
                if (!Committed)
                {
                    OpenComponent = null;
                }
            }

            private void SaveAfterState(UndoEngine engine)
            {
                Debug.Assert(_after is null, "Change undo saving state twice.");
                object? component = null;

                if (engine.TryGetService(out IReferenceService? rs))
                {
                    component = rs.GetReference(_componentName);
                }
                else if (engine.TryGetService(out IDesignerHost? host))
                {
                    component = host.Container.Components[_componentName];
                }

                // It is OK for us to not find a component here.
                // That can happen if our "after" state is owned by another change,
                // like an add of the component.
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
            ///  Performs the actual undo. After it finishes it will reverse the role of _before and _after
            /// </summary>
            public override void Undo(UndoEngine engine)
            {
                Debug.Assert(_savedAfterState, "After state not saved. BeforeUndo was not called?");

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
    }
}
