// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComVisible(true)]
        [Guid("3050f673-98b5-11cf-bb82-00aa00bdce0b")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLElement3
        {
            void MergeAttributes(object mergeThis, object pvarFlags);
            bool IsMultiLine();
            bool CanHaveHTML();
            void SetOnLayoutComplete(object v);
            object GetOnLayoutComplete();
            void SetOnPage(object v);
            object GetOnPage();
            void SetInflateBlock(bool v);
            bool GetInflateBlock();
            void SetOnBeforeDeactivate(object v);
            object GetOnBeforeDeactivate();
            void SetActive();
            void SetContentEditable(string v);
            string GetContentEditable();
            bool IsContentEditable();
            void SetHideFocus(bool v);
            bool GetHideFocus();
            void SetDisabled(bool v);
            bool GetDisabled();
            bool IsDisabled();
            void SetOnMove(object v);
            object GetOnMove();
            void SetOnControlSelect(object v);
            object GetOnControlSelect();
            bool FireEvent(string bstrEventName, IntPtr pvarEventObject);
            void SetOnResizeStart(object v);
            object GetOnResizeStart();
            void SetOnResizeEnd(object v);
            object GetOnResizeEnd();
            void SetOnMoveStart(object v);
            object GetOnMoveStart();
            void SetOnMoveEnd(object v);
            object GetOnMoveEnd();
            void SetOnMouseEnter(object v);
            object GetOnMouseEnter();
            void SetOnMouseLeave(object v);
            object GetOnMouseLeave();
            void SetOnActivate(object v);
            object GetOnActivate();
            void SetOnDeactivate(object v);
            object GetOnDeactivate();
            bool DragDrop();
            int GlyphMode();
        }
    }
}
