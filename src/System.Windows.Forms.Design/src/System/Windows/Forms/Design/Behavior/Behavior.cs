// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  This abstract class represents the Behavior objects that are managed
///  by the BehaviorService. This class can be extended to develop any
///  type of UI 'behavior'. Ex: selection, drag, and resize behaviors.
/// </summary>
public abstract class Behavior
{
    private readonly bool _callParentBehavior;
    private readonly BehaviorService? _behaviorService;

    protected Behavior()
    {
    }

    /// <param name="callParentBehavior">
    ///  `true` if the parentBehavior should be called if it exists. The parentBehavior is the next behavior on
    ///  the behaviorService stack.If true, <paramref name="behaviorService"/> must be non-null.
    /// </param>
    protected Behavior(bool callParentBehavior, BehaviorService? behaviorService)
    {
        if ((callParentBehavior) && (behaviorService is null))
        {
            throw new ArgumentException(null, nameof(behaviorService));
        }

        _callParentBehavior = callParentBehavior;
        _behaviorService = behaviorService;
    }

    private Behavior? GetNextBehavior => _behaviorService?.GetNextBehavior(this);

    /// <summary>
    ///  The cursor that should be displayed for this behavior.
    /// </summary>
    public virtual Cursor Cursor => Cursors.Default;

    /// <summary>
    ///  Returning true from here indicates to the BehaviorService that all MenuCommands the designer receives
    ///  should have their state set to 'Enabled = false' when this Behavior is active.
    /// </summary>
    public virtual bool DisableAllCommands
    {
        get
        {
            if (_callParentBehavior && GetNextBehavior is not null)
            {
                return GetNextBehavior.DisableAllCommands;
            }
            else
            {
                return false;
            }
        }
    }

    /// <summary>
    ///  Called from the BehaviorService, this function provides an opportunity for the Behavior to return its
    ///  own custom MenuCommand thereby intercepting this message.
    /// </summary>
    public virtual MenuCommand? FindCommand(CommandID commandId)
    {
        try
        {
            if (_callParentBehavior && GetNextBehavior is not null)
            {
                return GetNextBehavior.FindCommand(commandId);
            }
            else
            {
                return null;
            }
        }
        catch // Catch any exception and return null MenuCommand.
        {
            return null;
        }
    }

    /// <summary>
    ///  The heuristic we will follow when any of these methods are called
    ///  is that we will attempt to pass the message along to the glyph.
    ///  This is a helper method to ensure validity before forwarding the message.
    /// </summary>
    private Behavior? GetGlyphBehavior(Glyph? g)
    {
        return g?.Behavior is not null && g.Behavior != this ? g.Behavior : null;
    }

    /// <summary>
    ///  A behavior can request mouse capture through the behavior service by pushing itself with
    ///  PushCaptureBehavior. If it does so, it will be notified through OnLoseCapture when capture is lost.
    ///  Generally the behavior pops itself at this time. Capture is lost when one of the following occurs:
    ///
    ///  1. Someone else requests capture.
    ///  2. Another behavior is pushed.
    ///  3. This behavior is popped.
    ///
    ///  In each of these cases OnLoseCapture on the behavior will be called.
    /// </summary>
    public virtual void OnLoseCapture(Glyph? g, EventArgs e)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            GetNextBehavior.OnLoseCapture(g, e);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            behavior.OnLoseCapture(g, e);
        }
    }

    /// <summary>
    ///  When any MouseDown message enters the BehaviorService's AdornerWindow (nclbuttondown, lbuttondown,
    ///  rbuttondown, nclrbuttondown) it is first passed here, to the top-most Behavior in the BehaviorStack.
    ///  Returning 'true' from this function signifies that the Message was 'handled' by the Behavior and should
    ///  not continue to be processed.
    /// </summary>
    public virtual bool OnMouseDoubleClick(Glyph? g, MouseButtons button, Point mouseLoc)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            return GetNextBehavior.OnMouseDoubleClick(g, button, mouseLoc);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            return behavior.OnMouseDoubleClick(g, button, mouseLoc);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///  When any MouseDown message enters the BehaviorService's AdornerWindow (nclbuttondown, lbuttondown,
    ///  rbuttondown, nclrbuttondown) it is first passed here, to the top-most Behavior in the BehaviorStack.
    ///  Returning 'true' from this function signifies that the Message was 'handled' by the Behavior and
    ///  should not continue to be processed.
    /// </summary>
    public virtual bool OnMouseDown(Glyph? g, MouseButtons button, Point mouseLoc)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            return GetNextBehavior.OnMouseDown(g, button, mouseLoc);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            return behavior.OnMouseDown(g, button, mouseLoc);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///  When the mouse pointer's location is positively hit-tested with a different Glyph than previous
    ///  hit-tests, this event is fired on the Behavior associated with the Glyph.
    /// </summary>
    public virtual bool OnMouseEnter(Glyph? g)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            return GetNextBehavior.OnMouseEnter(g);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            return behavior.OnMouseEnter(g);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///  When a MouseHover message enters the BehaviorService's AdornerWindow it is first passed here, to the
    ///  top-most Behavior in the BehaviorStack. Returning 'true' from this function signifies that the Message
    ///  was 'handled' by the Behavior and should not continue to be processed.
    /// </summary>
    public virtual bool OnMouseHover(Glyph? g, Point mouseLoc)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            return GetNextBehavior.OnMouseHover(g, mouseLoc);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            return behavior.OnMouseHover(g, mouseLoc);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///  When the mouse pointer leaves a positively hit-tested Glyph with a valid Behavior, this method is invoked.
    /// </summary>
    public virtual bool OnMouseLeave(Glyph? g)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            return GetNextBehavior.OnMouseLeave(g);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            return behavior.OnMouseLeave(g);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///  When any MouseMove message enters the BehaviorService's AdornerWindow (mousemove, ncmousemove) it is
    ///  first passed here, to the top-most Behavior in the BehaviorStack. Returning 'true' from this method
    ///  signifies that the Message was 'handled' by the Behavior and should not continue to be processed.
    /// </summary>
    public virtual bool OnMouseMove(Glyph? g, MouseButtons button, Point mouseLoc)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            return GetNextBehavior.OnMouseMove(g, button, mouseLoc);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            return behavior.OnMouseMove(g, button, mouseLoc);
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    ///  When any MouseUp message enters the BehaviorService's AdornerWindow
    ///  (nclbuttonupown, lbuttonup, rbuttonup, nclrbuttonup) it is first
    ///  passed here, to the top-most Behavior in the BehaviorStack. Returning
    ///  'true' from this function signifies that the Message was 'handled' by
    ///  the Behavior and should not continue to be processed.
    /// </summary>
    public virtual bool OnMouseUp(Glyph? g, MouseButtons button)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            return GetNextBehavior.OnMouseUp(g, button);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            return behavior.OnMouseUp(g, button);
        }
        else
        {
            return false;
        }
    }

    // OLE DragDrop virtual methods

    /// <summary>
    ///  OnDragDrop can be overridden so that a Behavior can specify its own Drag/Drop rules.
    /// </summary>
    public virtual void OnDragDrop(Glyph? g, DragEventArgs e)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            GetNextBehavior.OnDragDrop(g, e);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            behavior.OnDragDrop(g, e);
        }
    }

    /// <summary>
    ///  OnDragEnter can be overridden so that a Behavior can specify its own Drag/Drop rules.
    /// </summary>
    public virtual void OnDragEnter(Glyph? g, DragEventArgs e)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            GetNextBehavior.OnDragEnter(g, e);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            behavior.OnDragEnter(g, e);
        }
    }

    /// <summary>
    ///  OnDragLeave can be overridden so that a Behavior can specify its own Drag/Drop rules.
    /// </summary>
    public virtual void OnDragLeave(Glyph? g, EventArgs e)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            GetNextBehavior.OnDragLeave(g, e);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            behavior.OnDragLeave(g, e);
        }
    }

    /// <summary>
    ///  OnDragOver can be overridden so that a Behavior can specify its own Drag/Drop rules.
    /// </summary>
    public virtual void OnDragOver(Glyph? g, DragEventArgs e)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            GetNextBehavior.OnDragOver(g, e);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            behavior.OnDragOver(g, e);
        }
        else if (e.Effect != DragDropEffects.None)
        {
            e.Effect = (Control.ModifierKeys == Keys.Control) ? DragDropEffects.Copy : DragDropEffects.Move;
        }
    }

    /// <summary>
    ///  OnGiveFeedback can be overridden so that a Behavior can specify its own Drag/Drop rules.
    /// </summary>
    public virtual void OnGiveFeedback(Glyph? g, GiveFeedbackEventArgs e)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            GetNextBehavior.OnGiveFeedback(g, e);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            behavior.OnGiveFeedback(g, e);
        }
    }

    /// <summary>
    ///  QueryContinueDrag can be overridden so that a Behavior can specify its own Drag/Drop rules.
    /// </summary>
    public virtual void OnQueryContinueDrag(Glyph? g, QueryContinueDragEventArgs e)
    {
        if (_callParentBehavior && GetNextBehavior is not null)
        {
            GetNextBehavior.OnQueryContinueDrag(g, e);
        }
        else if (GetGlyphBehavior(g) is Behavior behavior)
        {
            behavior.OnQueryContinueDrag(g, e);
        }
    }
}
