// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

public partial class ControlDesigner
{
    /// <summary>
    ///  This TransparentBehavior is associated with the BodyGlyph for this ControlDesigner. When the
    ///  BehaviorService hittests a glyph w/a TransparentBehavior, all messages will be passed through the
    ///  BehaviorService directly to the ControlDesigner. During a Drag operation, when the BehaviorService hittests
    /// </summary>
    internal class TransparentBehavior : Behavior.Behavior
    {
        private readonly ControlDesigner _designer;
        private Rectangle _controlRect = Rectangle.Empty;

        /// <summary>
        ///  Constructor that accepts the related ControlDesigner.
        /// </summary>
        internal TransparentBehavior(ControlDesigner designer) => _designer = designer.OrThrowIfNull();

        /// <summary>
        ///  This property performs a hit test on the ControlDesigner to determine if the BodyGlyph should return
        ///  '-1' for hit testing (letting all messages pass directly to the control).
        /// </summary>
        internal bool IsTransparent(Point p) => _designer.GetHitTest(p);

        /// <summary>
        ///  Forwards DragDrop notification from the BehaviorService to the related ControlDesigner.
        /// </summary>
        public override void OnDragDrop(Glyph? g, DragEventArgs e)
        {
            _controlRect = Rectangle.Empty;
            _designer.OnDragDrop(e);
        }

        /// <summary>
        ///  Forwards DragDrop notification from the BehaviorService to the related ControlDesigner.
        /// </summary>
        public override void OnDragEnter(Glyph? g, DragEventArgs e)
        {
            if (_designer.Control is { } control)
            {
                _controlRect = control.RectangleToScreen(control.ClientRectangle);
            }

            _designer.OnDragEnter(e);
        }

        /// <summary>
        ///  Forwards DragDrop notification from the BehaviorService to the related ControlDesigner.
        /// </summary>
        public override void OnDragLeave(Glyph? g, EventArgs e)
        {
            _controlRect = Rectangle.Empty;
            _designer.OnDragLeave(e);
        }

        /// <summary>
        ///  Forwards DragDrop notification from the BehaviorService to the related ControlDesigner.
        /// </summary>
        public override void OnDragOver(Glyph? g, DragEventArgs e)
        {
            // If we are not over a valid drop area, then do not allow the drag/drop. Now that all
            // dragging/dropping is done via the behavior service and adorner window, we have to do our own
            // validation, and cannot rely on the OS to do it for us.
            if (e is not null && _controlRect != Rectangle.Empty && !_controlRect.Contains(new Point(e.X, e.Y)))
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            _designer.OnDragOver(e!);
        }

        /// <summary>
        ///  Forwards DragDrop notification from the BehaviorService to the related ControlDesigner.
        /// </summary>
        public override void OnGiveFeedback(Glyph? g, GiveFeedbackEventArgs e)
        {
            _designer.OnGiveFeedback(e);
        }
    }
}
