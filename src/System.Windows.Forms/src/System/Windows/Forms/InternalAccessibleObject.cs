// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Accessibility;
using static Interop;
using static Interop.UiaCore;

namespace System.Windows.Forms
{
    /// <Summary>
    ///  Internal object passed out to OLEACC clients via WM_GETOBJECT.
    /// </Summary>
    internal sealed class InternalAccessibleObject :
        StandardOleMarshalObject,
        IAccessibleInternal,
        IReflect,
        Ole32.IServiceProvider,
        IAccessibleEx,
        IRawElementProviderSimple,
        IRawElementProviderFragment,
        IRawElementProviderFragmentRoot,
        IInvokeProvider,
        IValueProvider,
        IRangeValueProvider,
        IExpandCollapseProvider,
        IToggleProvider,
        ITableProvider,
        ITableItemProvider,
        IGridProvider,
        IGridItemProvider,
        Oleaut32.IEnumVariant,
        Ole32.IOleWindow,
        ILegacyIAccessibleProvider,
        ISelectionProvider,
        ISelectionItemProvider,
        IScrollItemProvider,
        IRawElementProviderHwndOverride,
        IMultipleViewProvider,
        ITextProvider,
        ITextProvider2
    {
        private IAccessible publicIAccessible;                      // AccessibleObject as IAccessible
        private readonly Oleaut32.IEnumVariant publicIEnumVariant;  // AccessibleObject as Oleaut32.IEnumVariant
        private readonly Ole32.IOleWindow publicIOleWindow;         // AccessibleObject as IOleWindow
        private readonly IReflect publicIReflect;                   // AccessibleObject as IReflect

        private readonly Ole32.IServiceProvider publicIServiceProvider; // AccessibleObject as IServiceProvider
        private readonly IAccessibleEx publicIAccessibleEx;       // AccessibleObject as IAccessibleEx

        // UIAutomation
        private readonly IRawElementProviderSimple publicIRawElementProviderSimple;                // AccessibleObject as IRawElementProviderSimple
        private readonly IRawElementProviderFragment publicIRawElementProviderFragment;            // AccessibleObject as IRawElementProviderFragment
        private readonly IRawElementProviderFragmentRoot publicIRawElementProviderFragmentRoot;    // AccessibleObject as IRawElementProviderFragmentRoot
        private readonly IInvokeProvider publicIInvokeProvider;                                    // AccessibleObject as IInvokeProvider
        private readonly IValueProvider publicIValueProvider;                                      // AccessibleObject as IValueProvider
        private readonly IRangeValueProvider publicIRangeValueProvider;                            // AccessibleObject as IRangeValueProvider
        private readonly IExpandCollapseProvider publicIExpandCollapseProvider;                    // AccessibleObject as IExpandCollapseProvider
        private readonly IToggleProvider publicIToggleProvider;                                    // AccessibleObject as IToggleProvider
        private readonly ITableProvider publicITableProvider;                                      // AccessibleObject as ITableProvider
        private readonly ITableItemProvider publicITableItemProvider;                              // AccessibleObject as ITableItemProvider
        private readonly IGridProvider publicIGridProvider;                                        // AccessibleObject as IGridProvider
        private readonly IGridItemProvider publicIGridItemProvider;                                // AccessibleObject as IGridItemProvider
        private readonly ILegacyIAccessibleProvider publicILegacyIAccessibleProvider;              // AccessibleObject as ILegayAccessibleProvider
        private readonly ISelectionProvider publicISelectionProvider;                              // AccessibleObject as ISelectionProvider
        private readonly ISelectionItemProvider publicISelectionItemProvider;                      // AccessibleObject as ISelectionItemProvider
        private readonly IScrollItemProvider publicIScrollItemProvider;                            // AccessibleObject as IScrollItemProvider
        private readonly IRawElementProviderHwndOverride publicIRawElementProviderHwndOverride;    // AccessibleObject as IRawElementProviderHwndOverride
        private readonly IMultipleViewProvider publicIMultiViewProvider;                           // AccessibleObject as IMultipleViewProvider
        private readonly ITextProvider publicITextProvider;                                        // AccessibleObject as ITextProvider
        private readonly ITextProvider2 publicITextProvider2;                                      // AccessibleObject as ITextProvider2

        /// <summary>
        ///  Create a new wrapper.
        /// </summary>
        internal InternalAccessibleObject(AccessibleObject accessibleImplemention)
        {
            // Get all the casts done here to catch any issues early
            publicIAccessible = (IAccessible)accessibleImplemention;
            publicIEnumVariant = (Oleaut32.IEnumVariant)accessibleImplemention;
            publicIOleWindow = (Ole32.IOleWindow)accessibleImplemention;
            publicIReflect = (IReflect)accessibleImplemention;
            publicIServiceProvider = (Ole32.IServiceProvider)accessibleImplemention;
            publicIAccessibleEx = (IAccessibleEx)accessibleImplemention;
            publicIRawElementProviderSimple = (IRawElementProviderSimple)accessibleImplemention;
            publicIRawElementProviderFragment = (IRawElementProviderFragment)accessibleImplemention;
            publicIRawElementProviderFragmentRoot = (IRawElementProviderFragmentRoot)accessibleImplemention;
            publicIInvokeProvider = (IInvokeProvider)accessibleImplemention;
            publicIValueProvider = (IValueProvider)accessibleImplemention;
            publicIRangeValueProvider = (IRangeValueProvider)accessibleImplemention;
            publicIExpandCollapseProvider = (IExpandCollapseProvider)accessibleImplemention;
            publicIToggleProvider = (IToggleProvider)accessibleImplemention;
            publicITableProvider = (ITableProvider)accessibleImplemention;
            publicITableItemProvider = (ITableItemProvider)accessibleImplemention;
            publicIGridProvider = (IGridProvider)accessibleImplemention;
            publicIGridItemProvider = (IGridItemProvider)accessibleImplemention;
            publicILegacyIAccessibleProvider = (ILegacyIAccessibleProvider)accessibleImplemention;
            publicISelectionProvider = (ISelectionProvider)accessibleImplemention;
            publicISelectionItemProvider = (ISelectionItemProvider)accessibleImplemention;
            publicIScrollItemProvider = (IScrollItemProvider)accessibleImplemention;
            publicIRawElementProviderHwndOverride = (IRawElementProviderHwndOverride)accessibleImplemention;
            publicIMultiViewProvider = (IMultipleViewProvider)accessibleImplemention;
            publicITextProvider = (ITextProvider)accessibleImplemention;
            publicITextProvider2 = (ITextProvider2)accessibleImplemention;
            // Note: Deliberately not holding onto AccessibleObject to enforce all access through the interfaces
        }

        /// <summary>
        ///  If the given object is an AccessibleObject return it as a InternalAccessibleObject
        ///  This ensures we wrap all AccessibleObjects before handing them out to OLEACC
        /// </summary>
        [return: NotNullIfNotNull("accObject")]
        private object? AsNativeAccessible(object? accObject)
        {
            if (accObject is AccessibleObject accessibleObject)
            {
                return new InternalAccessibleObject(accessibleObject);
            }

            return accObject;
        }

        /// <summary>
        ///  Wraps AccessibleObject elements of a given array into InternalAccessibleObjects
        /// </summary>
        private object[]? AsArrayOfNativeAccessibles(object[]? accObjectArray)
        {
            if (accObjectArray != null && accObjectArray.Length > 0)
            {
                for (int i = 0; i < accObjectArray.Length; i++)
                {
                    accObjectArray[i] = AsNativeAccessible(accObjectArray[i]);
                }
            }

            return accObjectArray;
        }

        void IAccessibleInternal.accDoDefaultAction(object childID)
            => publicIAccessible.accDoDefaultAction(childID);

        object? IAccessibleInternal.accHitTest(int xLeft, int yTop)
            => AsNativeAccessible(publicIAccessible.accHitTest(xLeft, yTop));

        void IAccessibleInternal.accLocation(out int l, out int t, out int w, out int h, object childID)
            => publicIAccessible.accLocation(out l, out t, out w, out h, childID);

        object? IAccessibleInternal.accNavigate(int navDir, object childID)
            => AsNativeAccessible(publicIAccessible.accNavigate(navDir, childID));

        void IAccessibleInternal.accSelect(int flagsSelect, object childID)
            => publicIAccessible.accSelect(flagsSelect, childID);

        object? IAccessibleInternal.get_accChild(object childID)
            => AsNativeAccessible(publicIAccessible.get_accChild(childID));

        int IAccessibleInternal.get_accChildCount() => publicIAccessible.accChildCount;

        string? IAccessibleInternal.get_accDefaultAction(object childID)
            => publicIAccessible.get_accDefaultAction(childID);

        string? IAccessibleInternal.get_accDescription(object childID)
            => publicIAccessible.get_accDescription(childID);

        object? IAccessibleInternal.get_accFocus()
            => AsNativeAccessible(publicIAccessible.accFocus);

        string? IAccessibleInternal.get_accHelp(object childID)
            => publicIAccessible.get_accHelp(childID);

        int IAccessibleInternal.get_accHelpTopic(out string pszHelpFile, object childID)
            => publicIAccessible.get_accHelpTopic(out pszHelpFile, childID);

        string? IAccessibleInternal.get_accKeyboardShortcut(object childID)
            => publicIAccessible.get_accKeyboardShortcut(childID);

        string? IAccessibleInternal.get_accName(object childID)
            => publicIAccessible.get_accName(childID);

        object? IAccessibleInternal.get_accParent()
            => AsNativeAccessible(publicIAccessible.accParent);

        object? IAccessibleInternal.get_accRole(object childID)
            => publicIAccessible.get_accRole(childID);

        object? IAccessibleInternal.get_accSelection()
            => AsNativeAccessible(publicIAccessible.accSelection);

        object? IAccessibleInternal.get_accState(object childID)
            => publicIAccessible.get_accState(childID);

        string? IAccessibleInternal.get_accValue(object childID)
            => publicIAccessible.get_accValue(childID);

        void IAccessibleInternal.set_accName(object childID, string newName)
            => publicIAccessible.set_accName(childID, newName);

        void IAccessibleInternal.set_accValue(object childID, string newValue)
            => publicIAccessible.set_accValue(childID, newValue);

        HRESULT Oleaut32.IEnumVariant.Clone(Oleaut32.IEnumVariant[]? ppEnum)
            => publicIEnumVariant.Clone(ppEnum);

        unsafe HRESULT Oleaut32.IEnumVariant.Next(uint celt, IntPtr rgVar, uint* pCeltFetched)
            => publicIEnumVariant.Next(celt, rgVar, pCeltFetched);

        HRESULT Oleaut32.IEnumVariant.Reset() => publicIEnumVariant.Reset();

        HRESULT Oleaut32.IEnumVariant.Skip(uint celt) => publicIEnumVariant.Skip(celt);

        unsafe HRESULT Ole32.IOleWindow.GetWindow(IntPtr* phwnd)
            => publicIOleWindow.GetWindow(phwnd);

        HRESULT Ole32.IOleWindow.ContextSensitiveHelp(BOOL fEnterMode)
            => publicIOleWindow.ContextSensitiveHelp(fEnterMode);

        MethodInfo? IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder? binder, Type[] types, ParameterModifier[]? modifiers)
            => publicIReflect.GetMethod(name, bindingAttr, binder, types, modifiers);

        MethodInfo? IReflect.GetMethod(string name, BindingFlags bindingAttr)
            => publicIReflect.GetMethod(name, bindingAttr);

        MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
            => publicIReflect.GetMethods(bindingAttr);

        FieldInfo? IReflect.GetField(string name, BindingFlags bindingAttr)
            => publicIReflect.GetField(name, bindingAttr);

        FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
            => publicIReflect.GetFields(bindingAttr);

        PropertyInfo? IReflect.GetProperty(string name, BindingFlags bindingAttr)
            => publicIReflect.GetProperty(name, bindingAttr);

        PropertyInfo? IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder? binder, Type? returnType, Type[] types, ParameterModifier[]? modifiers)
            => publicIReflect.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);

        PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
            => publicIReflect.GetProperties(bindingAttr);

        MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
            => publicIReflect.GetMember(name, bindingAttr);

        MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
            => publicIReflect.GetMembers(bindingAttr);

        object? IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder? binder, object? target, object?[]? args, ParameterModifier[]? modifiers, CultureInfo? culture, string[]? namedParameters)
            => publicIReflect.InvokeMember(name, invokeAttr, binder, publicIAccessible, args, modifiers, culture, namedParameters);

        Type IReflect.UnderlyingSystemType => publicIReflect.UnderlyingSystemType;

        unsafe HRESULT Ole32.IServiceProvider.QueryService(Guid* service, Guid* riid, IntPtr* ppvObject)
        {
            HRESULT hr = publicIServiceProvider.QueryService(service, riid, ppvObject);
            if (hr.Succeeded())
            {
                // we always want to return the internal accessible object
                *ppvObject = Marshal.GetComInterfaceForObject(this, typeof(IAccessibleEx));
            }

            return hr;
        }

        IAccessibleEx? IAccessibleEx.GetObjectForChild(int idChild)
        {
            return publicIAccessibleEx.GetObjectForChild(idChild);
        }

        unsafe HRESULT IAccessibleEx.GetIAccessiblePair(out object? ppAcc, int* pidChild)
        {
            if (pidChild is null)
            {
                ppAcc = null;
                return HRESULT.E_INVALIDARG;
            }

            // We always want to return the internal accessible object
            ppAcc = this;
            *pidChild = NativeMethods.CHILDID_SELF;
            return HRESULT.S_OK;
        }

        int[]? IAccessibleEx.GetRuntimeId() => publicIAccessibleEx.GetRuntimeId();

        HRESULT IAccessibleEx.ConvertReturnedElement(IRawElementProviderSimple pIn, out IAccessibleEx? ppRetValOut)
            => publicIAccessibleEx.ConvertReturnedElement(pIn, out ppRetValOut);

        ProviderOptions IRawElementProviderSimple.ProviderOptions
            => publicIRawElementProviderSimple.ProviderOptions;

        IRawElementProviderSimple? IRawElementProviderSimple.HostRawElementProvider
            => publicIRawElementProviderSimple.HostRawElementProvider;

        object? IRawElementProviderSimple.GetPatternProvider(UIA patternId)
        {
            object? obj = publicIRawElementProviderSimple.GetPatternProvider(patternId);
            if (obj is null)
            {
                return null;
            }

            // we always want to return the internal accessible object
            return patternId switch
            {
                UIA.ExpandCollapsePatternId => (IExpandCollapseProvider)this,
                UIA.ValuePatternId => (IValueProvider)this,
                UIA.RangeValuePatternId => (IRangeValueProvider)this,
                UIA.TogglePatternId => (IToggleProvider)this,
                UIA.TablePatternId => (ITableProvider)this,
                UIA.TableItemPatternId => (ITableItemProvider)this,
                UIA.GridPatternId => (IGridProvider)this,
                UIA.GridItemPatternId => (IGridItemProvider)this,
                UIA.InvokePatternId => (IInvokeProvider)this,
                UIA.LegacyIAccessiblePatternId => (ILegacyIAccessibleProvider)this,
                UIA.SelectionPatternId => (ISelectionProvider)this,
                UIA.SelectionItemPatternId => (ISelectionItemProvider)this,
                UIA.ScrollItemPatternId => (IScrollItemProvider)this,
                UIA.MultipleViewPatternId => (IMultipleViewProvider)this,
                UIA.TextPatternId => (ITextProvider)this,
                UIA.TextPattern2Id => (ITextProvider2)this,
                _ => null
            };
        }

        object? IRawElementProviderSimple.GetPropertyValue(UIA propertyID)
            => publicIRawElementProviderSimple.GetPropertyValue(propertyID);

        object? IRawElementProviderFragment.Navigate(NavigateDirection direction)
            => AsNativeAccessible(publicIRawElementProviderFragment.Navigate(direction));

        int[]? IRawElementProviderFragment.GetRuntimeId()
            => publicIRawElementProviderFragment.GetRuntimeId();

        object[]? IRawElementProviderFragment.GetEmbeddedFragmentRoots()
            => AsArrayOfNativeAccessibles(publicIRawElementProviderFragment.GetEmbeddedFragmentRoots());

        void IRawElementProviderFragment.SetFocus()
            => publicIRawElementProviderFragment.SetFocus();

        UiaRect IRawElementProviderFragment.BoundingRectangle
            => publicIRawElementProviderFragment.BoundingRectangle;

        IRawElementProviderFragmentRoot? IRawElementProviderFragment.FragmentRoot
            => publicIRawElementProviderFragment.FragmentRoot;

        object? IRawElementProviderFragmentRoot.ElementProviderFromPoint(double x, double y)
            => AsNativeAccessible(publicIRawElementProviderFragmentRoot.ElementProviderFromPoint(x, y));

        object? IRawElementProviderFragmentRoot.GetFocus()
            => AsNativeAccessible(publicIRawElementProviderFragmentRoot.GetFocus());

        string? ILegacyIAccessibleProvider.DefaultAction => publicILegacyIAccessibleProvider.DefaultAction;

        string? ILegacyIAccessibleProvider.Description => publicILegacyIAccessibleProvider.Description;

        string? ILegacyIAccessibleProvider.Help => publicILegacyIAccessibleProvider.Help;

        string? ILegacyIAccessibleProvider.KeyboardShortcut => publicILegacyIAccessibleProvider.KeyboardShortcut;

        string? ILegacyIAccessibleProvider.Name => publicILegacyIAccessibleProvider.Name;

        uint ILegacyIAccessibleProvider.Role => publicILegacyIAccessibleProvider.Role;

        uint ILegacyIAccessibleProvider.State => publicILegacyIAccessibleProvider.State;

        string? ILegacyIAccessibleProvider.Value => publicILegacyIAccessibleProvider.Value;

        int ILegacyIAccessibleProvider.ChildId => publicILegacyIAccessibleProvider.ChildId;

        void ILegacyIAccessibleProvider.DoDefaultAction() => publicILegacyIAccessibleProvider.DoDefaultAction();

        IAccessible? ILegacyIAccessibleProvider.GetIAccessible() => publicILegacyIAccessibleProvider.GetIAccessible();

        IRawElementProviderSimple[] ILegacyIAccessibleProvider.GetSelection() => publicILegacyIAccessibleProvider.GetSelection();

        void ILegacyIAccessibleProvider.Select(int flagsSelect) => publicILegacyIAccessibleProvider.Select(flagsSelect);

        void ILegacyIAccessibleProvider.SetValue(string szValue) => publicILegacyIAccessibleProvider.SetValue(szValue);

        void IInvokeProvider.Invoke() => publicIInvokeProvider.Invoke();

        ITextRangeProvider[]? ITextProvider.GetSelection() => publicITextProvider.GetSelection();

        ITextRangeProvider[]? ITextProvider.GetVisibleRanges() => publicITextProvider.GetVisibleRanges();

        ITextRangeProvider? ITextProvider.RangeFromChild(IRawElementProviderSimple childElement)
            => publicITextProvider.RangeFromChild(childElement);

        ITextRangeProvider? ITextProvider.RangeFromPoint(Point screenLocation)
            => publicITextProvider.RangeFromPoint(screenLocation);

        SupportedTextSelection ITextProvider.SupportedTextSelection => publicITextProvider.SupportedTextSelection;

        ITextRangeProvider? ITextProvider.DocumentRange => publicITextProvider.DocumentRange;

        ITextRangeProvider[]? ITextProvider2.GetSelection() => publicITextProvider2.GetSelection();

        ITextRangeProvider[]? ITextProvider2.GetVisibleRanges() => publicITextProvider2.GetVisibleRanges();

        ITextRangeProvider? ITextProvider2.RangeFromChild(IRawElementProviderSimple childElement)
            => publicITextProvider2.RangeFromChild(childElement);

        ITextRangeProvider? ITextProvider2.RangeFromPoint(Point screenLocation)
            => publicITextProvider2.RangeFromPoint(screenLocation);

        SupportedTextSelection ITextProvider2.SupportedTextSelection => publicITextProvider2.SupportedTextSelection;

        ITextRangeProvider? ITextProvider2.DocumentRange => publicITextProvider2.DocumentRange;

        ITextRangeProvider? ITextProvider2.GetCaretRange(out BOOL isActive)
            => publicITextProvider2.GetCaretRange(out isActive);

        ITextRangeProvider? ITextProvider2.RangeFromAnnotation(IRawElementProviderSimple annotationElement)
            => publicITextProvider2.RangeFromAnnotation(annotationElement);

        BOOL IValueProvider.IsReadOnly => publicIValueProvider.IsReadOnly;

        string? IValueProvider.Value => publicIValueProvider.Value;

        void IValueProvider.SetValue(string? newValue)
            => publicIValueProvider.SetValue(newValue);

        BOOL IRangeValueProvider.IsReadOnly => publicIValueProvider.IsReadOnly;

        double IRangeValueProvider.LargeChange => publicIRangeValueProvider.LargeChange;

        double IRangeValueProvider.Maximum => publicIRangeValueProvider.Maximum;

        double IRangeValueProvider.Minimum => publicIRangeValueProvider.Minimum;

        double IRangeValueProvider.SmallChange => publicIRangeValueProvider.SmallChange;

        double IRangeValueProvider.Value => publicIRangeValueProvider.Value;

        void IRangeValueProvider.SetValue(double newValue)
            => publicIRangeValueProvider.SetValue(newValue);

        void IExpandCollapseProvider.Expand() => publicIExpandCollapseProvider.Expand();

        void IExpandCollapseProvider.Collapse() => publicIExpandCollapseProvider.Collapse();

        ExpandCollapseState IExpandCollapseProvider.ExpandCollapseState
            => publicIExpandCollapseProvider.ExpandCollapseState;

        void IToggleProvider.Toggle() => publicIToggleProvider.Toggle();

        ToggleState IToggleProvider.ToggleState => publicIToggleProvider.ToggleState;

        object[]? ITableProvider.GetRowHeaders()
            => AsArrayOfNativeAccessibles(publicITableProvider.GetRowHeaders());

        object[]? ITableProvider.GetColumnHeaders()
            => AsArrayOfNativeAccessibles(publicITableProvider.GetColumnHeaders());

        RowOrColumnMajor ITableProvider.RowOrColumnMajor => publicITableProvider.RowOrColumnMajor;

        object[]? ITableItemProvider.GetRowHeaderItems()
            => AsArrayOfNativeAccessibles(publicITableItemProvider.GetRowHeaderItems());

        object[]? ITableItemProvider.GetColumnHeaderItems()
            => AsArrayOfNativeAccessibles(publicITableItemProvider.GetColumnHeaderItems());

        object? IGridProvider.GetItem(int row, int column)
            => AsNativeAccessible(publicIGridProvider.GetItem(row, column));

        int IGridProvider.RowCount => publicIGridProvider.RowCount;

        int IGridProvider.ColumnCount => publicIGridProvider.ColumnCount;

        int IGridItemProvider.Row => publicIGridItemProvider.Row;

        int IGridItemProvider.Column => publicIGridItemProvider.Column;

        int IGridItemProvider.RowSpan => publicIGridItemProvider.RowSpan;

        int IGridItemProvider.ColumnSpan => publicIGridItemProvider.ColumnSpan;

        IRawElementProviderSimple? IGridItemProvider.ContainingGrid
            => publicIGridItemProvider.ContainingGrid;

        /// <summary>
        ///  Get the currently selected elements
        /// </summary>
        /// <returns>An AutomationElement array containing the currently selected elements</returns>
        object[]? ISelectionProvider.GetSelection() => publicISelectionProvider.GetSelection();

        /// <summary>
        ///  Indicates whether the control allows more than one element to be selected
        /// </summary>
        /// <returns>Boolean indicating whether the control allows more than one element to be selected</returns>
        /// <remarks>If this is false, then the control is a single-select ccntrol</remarks>
        BOOL ISelectionProvider.CanSelectMultiple => publicISelectionProvider.CanSelectMultiple;

        /// <summary>
        ///  Indicates whether the control requires at least one element to be selected
        /// </summary>
        /// <returns>Boolean indicating whether the control requires at least one element to be selected</returns>
        /// <remarks>If this is false, then the control allows all elements to be unselected</remarks>
        BOOL ISelectionProvider.IsSelectionRequired => publicISelectionProvider.IsSelectionRequired;

        /// <summary>
        ///  Sets the current element as the selection
        ///  This clears the selection from other elements in the container.
        /// </summary>
        void ISelectionItemProvider.Select() => publicISelectionItemProvider.Select();

        /// <summary>
        ///  Adds current element to selection.
        /// </summary>
        void ISelectionItemProvider.AddToSelection() => publicISelectionItemProvider.AddToSelection();

        /// <summary>
        ///  Removes current element from selection.
        /// </summary>
        void ISelectionItemProvider.RemoveFromSelection() => publicISelectionItemProvider.RemoveFromSelection();

        /// <summary>
        ///  Check whether an element is selected.
        /// </summary>
        /// <returns>Returns true if the element is selected.</returns>
        BOOL ISelectionItemProvider.IsSelected => publicISelectionItemProvider.IsSelected;

        void IScrollItemProvider.ScrollIntoView() => publicIScrollItemProvider.ScrollIntoView();

        /// <summary>
        ///  The logical element that supports the SelectionPattern for this Item.
        /// </summary>
        /// <returns>Returns a IRawElementProviderSimple.</returns>
        IRawElementProviderSimple? ISelectionItemProvider.SelectionContainer
            => publicISelectionItemProvider.SelectionContainer;

        /// <summary>
        ///  Request a provider for the specified component. The returned provider can supply additional
        ///  properties or override properties of the specified component.
        /// </summary>
        /// <param name="hwnd">The window handle of the component.</param>
        /// <returns>Return the provider for the specified component, or null if the component is not being overridden.</returns>
        IRawElementProviderSimple? IRawElementProviderHwndOverride.GetOverrideProviderForHwnd(IntPtr hwnd)
            => publicIRawElementProviderHwndOverride.GetOverrideProviderForHwnd(hwnd);

        int IMultipleViewProvider.CurrentView
            => publicIMultiViewProvider.CurrentView;

        int[]? IMultipleViewProvider.GetSupportedViews()
            => publicIMultiViewProvider.GetSupportedViews();

        string? IMultipleViewProvider.GetViewName(int viewId)
            => publicIMultiViewProvider.GetViewName(viewId);

        void IMultipleViewProvider.SetCurrentView(int viewId)
            => publicIMultiViewProvider.SetCurrentView(viewId);
    }
}
