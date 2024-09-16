// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

public abstract partial class UndoEngine
{
    protected partial class UndoUnit
    {
        private abstract class UndoEvent
        {
            /// <summary>
            ///  Indicates that undoing this event may cause side effects in other objects.
            ///  Change events fall into this category because, for example, a change involving adding an object to
            ///  one collection may have a side effect of removing it from another collection.
            ///  Events with side effects are grouped at undo time so all their BeforeUndo methods
            ///  are called before their Undo methods. Events without side effects have their BeforeUndo
            ///  called and then their Undo called immediately after.
            /// </summary>
            public virtual bool CausesSideEffects => false;

            /// <summary>
            ///  Called before Undo is called. All undo events get their BeforeUndo called,
            ///  and then they all get their Undo called. This allows the undo event to examine the state of the
            ///  world before other undo events mess with it. BeforeUndo returns true if before undo was supported,
            ///  and false if not. If before undo is not supported, the undo unit should be undone immediately.
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
