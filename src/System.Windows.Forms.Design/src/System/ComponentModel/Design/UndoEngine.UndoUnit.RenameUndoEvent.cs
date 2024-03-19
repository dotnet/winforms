// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

public abstract partial class UndoEngine
{
    protected partial class UndoUnit
    {
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
            }

            /// <summary>
            ///  Simply undoes a rename by setting the name back to the saved value.
            /// </summary>
            public override void Undo(UndoEngine engine)
            {
                IComponent? comp = engine._host.Container.Components[_after];
                if (comp is not null)
                {
                    engine.ComponentChangeService.OnComponentChanging(comp, null);
                    comp.Site!.Name = _before;
                    (_after, _before) = (_before, _after);
                }
            }
        }
    }
}
