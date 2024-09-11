// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;

namespace System.ComponentModel.Design;

public abstract partial class UndoEngine
{
    protected partial class UndoUnit
    {
        /// <summary>
        ///  This undo event handles addition and removal of components.
        /// </summary>
        private sealed class AddRemoveUndoEvent : UndoEvent
        {
            private readonly SerializationStore _serializedData;
            private readonly string? _componentName;

            /// <summary>
            ///  Creates a new object that contains the state of the event.
            ///  The last parameter, add, determines the initial mode of this event.
            ///  If true, it means this event is being created in response to a component add.
            ///  If false, it is being created in response to a component remove.
            /// </summary>
            public AddRemoveUndoEvent(UndoEngine engine, IComponent component, bool add)
            {
                _componentName = component.Site!.Name;
                NextUndoAdds = !add;
                OpenComponent = component;

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
            internal void Commit()
            {
                if (!Committed)
                {
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
                    // We need to add this component. To add it, we deserialize it and then we add it to
                    // the designer host's container.
                    IDesignerHost host = engine.GetRequiredService<IDesignerHost>();

                    engine._serializationService.DeserializeTo(_serializedData, host.Container);
                }
                else
                {
                    // We need to remove this component. Take the name and match it to an object,
                    // and then ask that object to delete itself.
                    IDesignerHost host = engine.GetRequiredService<IDesignerHost>();

                    IComponent? component = host.Container.Components[_componentName];

                    // Note: It's ok for the component to be null here.
                    // This could happen if the parent to this control is disposed first. Ex:SplitContainer
                    if (component is not null)
                    {
                        host.DestroyComponent(component);
                    }
                }

                NextUndoAdds = !NextUndoAdds;
            }
        }
    }
}
