// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal partial class Mshtml
    {
        [ComImport]
        [Guid("3050f434-98b5-11cf-bb82-00aa00bdce0b")]
        [InterfaceType(ComInterfaceType.InterfaceIsDual)]
        public interface IHTMLElement2
        {
            string ScopeName();
            void SetCapture(bool containerCapture);
            void ReleaseCapture();
            void SetOnLoseCapture(object v);
            object GetOnLoseCapture();
            string GetComponentFromPoint(int x, int y);
            void DoScroll(object component);
            void SetOnScroll(object v);
            object GetOnScroll();
            void SetOnDrag(object v);
            object GetOnDrag();
            void SetOnDragEnd(object v);
            object GetOnDragEnd();
            void SetOnDragEnter(object v);
            object GetOnDragEnter();
            void SetOnDragOver(object v);
            object GetOnDragOver();
            void SetOnDragleave(object v);
            object GetOnDragLeave();
            void SetOnDrop(object v);
            object GetOnDrop();
            void SetOnBeforeCut(object v);
            object GetOnBeforeCut();
            void SetOnCut(object v);
            object GetOnCut();
            void SetOnBeforeCopy(object v);
            object GetOnBeforeCopy();
            void SetOnCopy(object v);
            object GetOnCopy(object p);
            void SetOnBeforePaste(object v);
            object GetOnBeforePaste(object p);
            void SetOnPaste(object v);
            object GetOnPaste(object p);
            object GetCurrentStyle();
            void SetOnPropertyChange(object v);
            object GetOnPropertyChange(object p);
            object GetClientRects();
            object GetBoundingClientRect();
            void SetExpression(string propName, string expression, string language);
            object GetExpression(string propName);
            bool RemoveExpression(string propName);
            void SetTabIndex(int v);
            short GetTabIndex();
            void Focus();
            void SetAccessKey(string v);
            string GetAccessKey();
            void SetOnBlur(object v);
            object GetOnBlur();
            void SetOnFocus(object v);
            object GetOnFocus();
            void SetOnResize(object v);
            object GetOnResize();
            void Blur();
            void AddFilter(object pUnk);
            void RemoveFilter(object pUnk);
            int ClientHeight();
            int ClientWidth();
            int ClientTop();
            int ClientLeft();
            bool AttachEvent(string ev, [In, MarshalAs(UnmanagedType.IDispatch)] object pdisp);
            void DetachEvent(string ev, [In, MarshalAs(UnmanagedType.IDispatch)] object pdisp);
            object ReadyState();
            void SetOnReadyStateChange(object v);
            object GetOnReadyStateChange();
            void SetOnRowsDelete(object v);
            object GetOnRowsDelete();
            void SetOnRowsInserted(object v);
            object GetOnRowsInserted();
            void SetOnCellChange(object v);
            object GetOnCellChange();
            void SetDir(string v);
            string GetDir();
            object CreateControlRange();
            int GetScrollHeight();
            int GetScrollWidth();
            void SetScrollTop(int v);
            int GetScrollTop();
            void SetScrollLeft(int v);
            int GetScrollLeft();
            void ClearAttributes();
            void MergeAttributes(object mergeThis);
            void SetOnContextMenu(object v);
            object GetOnContextMenu();
            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElement
                InsertAdjacentElement(string @where,
                    [In, MarshalAs(UnmanagedType.Interface)] IHTMLElement insertedElement);
            [return: MarshalAs(UnmanagedType.Interface)]
            IHTMLElement
                applyElement([In, MarshalAs(UnmanagedType.Interface)] IHTMLElement apply,
                    string @where);
            string GetAdjacentText(string @where);
            string ReplaceAdjacentText(string @where, string newText);
            bool CanHaveChildren();
            int AddBehavior(string url, ref object oFactory);
            bool RemoveBehavior(int cookie);
            object GetRuntimeStyle();
            object GetBehaviorUrns();
            void SetTagUrn(string v);
            string GetTagUrn();
            void SetOnBeforeEditFocus(object v);
            object GetOnBeforeEditFocus();
            int GetReadyStateValue();
            [return: MarshalAs(UnmanagedType.Interface)] IHTMLElementCollection GetElementsByTagName(string v);
        }
    }
}
