// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design.Tests
{
    internal class TestControlDesigner : ControlDesigner
    {
        internal AccessibleObject GetAccessibleObjectField()
        {
            return accessibilityObj;
        }

        internal BehaviorService GetBehaviorServiceProperty()
        {
            return BehaviorService;
        }

        internal bool GetEnableDragRectProperty()
        {
            return EnableDragRect;
        }

        internal IComponent GetParentComponentProperty()
        {
            return ParentComponent;
        }

        internal InheritanceAttribute GetInheritanceAttributeProperty()
        {
            return InheritanceAttribute;
        }

        internal void BaseWndProcMethod(ref Message m)
        {
            BaseWndProc(ref m);
        }

        internal void DefWndProcMethod(ref Message m)
        {
            DefWndProc(ref m);
        }

        internal void DisplayErrorMethod(Exception e)
        {
            DisplayError(e);
        }

        internal void DisposeMethod(bool disposing)
        {
            Dispose(disposing);
        }

        internal bool EnableDesignModeMethod(Control child, string name)
        {
            return EnableDesignMode(child, name);
        }

        internal void EnableDragDropMethod(bool value)
        {
            EnableDragDrop(value);
        }

        internal ControlBodyGlyph GetControlGlyphMethod(GlyphSelectionType selectionType)
        {
            return GetControlGlyph(selectionType);
        }

        internal bool GetHitTestMethod(Point point)
        {
            return GetHitTest(point);
        }

        internal void HookChildControlsMethod(Control firstChild)
        {
            HookChildControls(firstChild);
        }

        internal void OnContextMenuMethod(int x, int y)
        {
            OnContextMenu(x, y);
        }

        internal void OnCreateHandleMethod()
        {
            OnCreateHandle();
        }
    }
}
