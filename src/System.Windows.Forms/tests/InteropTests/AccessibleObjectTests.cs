// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;
using static Interop;
using static Interop.Oleaut32;
using static Interop.UiaCore;

namespace System.Windows.Forms.InteropTests
{
    public class AccesibleObjectTests : InteropTestBase
    {
        [WinFormsFact]
        public unsafe void AccesibleObject_IAccessibleExConvertReturnedElement_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IAccessibleExConvertReturnedElement(o));
        }

        [WinFormsFact]
        public unsafe void AccesibleObject_IAccessibleExGetIAccessiblePair_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IAccessibleExGetIAccessiblePair(o));
        }

        [WinFormsFact]
        public unsafe void AccesibleObject_IAccessibleExGetRuntimeId_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IAccessibleExGetRuntimeId(o, null));
        }

        [WinFormsTheory]
        [InlineData(-2)]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(1)]
        public unsafe void AccesibleObject_IAccessibleExGetObjectForChild_Invoke_ReturnsExpected(int idChild)
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IAccessibleExGetObjectForChild(o, idChild));
        }

        [WinFormsFact]
        public void AccessibleObject_IServiceProvider_QueryService_Invoke_Success()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IServiceProviderQueryService(o));
        }

        [WinFormsFact]
        public void AccesibleObject_IRawElementProviderSimple_GetHostRawElementProvider_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRawElementProviderSimpleHostRawElementProvider(o, BOOL.FALSE));
        }

        [WinFormsFact]
        public void AccesibleObject_IRawElementProviderSimpleProviderOptions_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRawElementProviderSimpleProviderOptions(o, ProviderOptions.ServerSideProvider | ProviderOptions.UseComThreading));
        }

        public static IEnumerable<object[]> GetPatternProvider_TestData()
        {
            yield return new object[] { UIA.InvokePatternId, BOOL.FALSE };
            yield return new object[] { UIA.SelectionPatternId, BOOL.FALSE };
            yield return new object[] { UIA.IsInvokePatternAvailablePropertyId, BOOL.FALSE };
            yield return new object[] { UIA.InvokePatternId - 1, BOOL.FALSE };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPatternProvider_TestData))]
        public void AccesibleObject_IRawElementProviderSimpleGetPatternProvider_Invoke_ReturnsExpected(object patternId, object expected)
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRawElementProviderSimpleGetPatternProvider(o, (UIA)patternId, (BOOL)expected));
        }

        public static IEnumerable<object[]> GetPropertyValue_TestData()
        {
            yield return new object[] { UIA.InvokePatternId, null };
            yield return new object[] { UIA.SelectionPatternId, null };
            yield return new object[] { UIA.IsInvokePatternAvailablePropertyId, false };
            yield return new object[] { UIA.InvokePatternId - 1, null };
        }

        [WinFormsTheory]
        [MemberData(nameof(GetPropertyValue_TestData))]
        public void AccesibleObject_IRawElementProviderSimpleGetPropertyValue_Invoke_ReturnsExpected(object patternId, object expected)
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRawElementProviderSimpleGetPropertyValue(o, (UIA)patternId, out VARIANT variant));
            Assert.Equal(expected, variant.ToObject());
        }

        [WinFormsFact]
        public void AccesibleObject_IRangeValueProviderIsReadOnly_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRangeValueProviderGetIsReadOnly(o, BOOL.FALSE));
        }

        [WinFormsFact]
        public void AccesibleObject_IRangeValueProviderLargeChange_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRangeValueProviderGetLargeChange(o, double.NaN));
        }

        [WinFormsFact]
        public void AccesibleObject_IRangeValueProviderMaximum_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRangeValueProviderGetMaximum(o, double.NaN));
        }

        [WinFormsFact]
        public void AccesibleObject_IRangeValueProviderMinimum_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRangeValueProviderGetMinimum(o, double.NaN));
        }

        [WinFormsFact]
        public void AccesibleObject_IRangeValueProviderSmallChange_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRangeValueProviderGetSmallChange(o, double.NaN));
        }

        [WinFormsFact]
        public void AccesibleObject_IRangeValueProviderValue_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRangeValueProviderGetValue(o, double.NaN));
        }

        [WinFormsTheory]
        [InlineData(101, double.NaN)]
        [InlineData(100, double.NaN)]
        [InlineData(1, double.NaN)]
        [InlineData(0, double.NaN)]
        [InlineData(-1, double.NaN)]
        [InlineData(double.NaN, double.NaN)]
        public void AccesibleObject_IRangeValueProviderSetValue_Invoke_GetReturnsExpected(double value, double expected)
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRangeValueProviderSetValue(o, value, expected));
        }

        [WinFormsFact]
        public unsafe void AccesibleObject_IRawElementProviderFragmentBoundingRectangle_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRawElementProviderFragmentGetBoundingRectangle(o, new UiaRect()));
        }

        [WinFormsFact]
        public unsafe void AccesibleObject_IRawElementProviderFragmentFragmentRoot_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRawElementProviderFragmentGetFragmentRoot(o));
        }

        [WinFormsFact]
        public unsafe void AccesibleObject_IRawElementProviderFragmentGetEmbeddedFragmentRoots_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRawElementProviderFragmentGetEmbeddedFragmentRoots(o));
        }

        [WinFormsFact]
        public unsafe void AccesibleObject_IRawElementProviderFragmentGetRuntimeId_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRawElementProviderFragmentGetRuntimeId(o, null));
        }

        [WinFormsTheory]
        [InlineData((int)NavigateDirection.Parent - 1)]
        [InlineData((int)NavigateDirection.Parent)]
        [InlineData((int)NavigateDirection.NextSibling)]
        [InlineData((int)NavigateDirection.PreviousSibling)]
        [InlineData((int)NavigateDirection.FirstChild)]
        [InlineData((int)NavigateDirection.LastChild)]
        [InlineData((int)NavigateDirection.LastChild + 1)]
        public void AccesibleObject_IRawElementProviderFragmentNavigate_Invoke_ReturnsExpected(int direction)
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRawElementProviderFragmentNavigate(o, (NavigateDirection)direction));
        }

        [WinFormsFact]
        public unsafe void AccesibleObject_IRawElementProviderFragmentSetFocus_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRawElementProviderFragmentSetFocus(o));
        }

        [WinFormsTheory]
        [InlineData(1, 2)]
        [InlineData(0, 0)]
        [InlineData(-1, -2)]
        [InlineData(double.NaN, double.NaN)]
        public unsafe void AccesibleObject_IRawElementProviderFragmentRootElementProviderFromPoint_Invoke_ReturnsExpected(double x, double y)
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRawElementProviderFragmentRootElementProviderFromPoint(o, x, y));
        }

        [WinFormsFact]
        public unsafe void AccesibleObject_IRawElementProviderFragmentRootGetFocus_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRawElementProviderFragmentRootGetFocus(o));
        }

        [WinFormsFact]
        public unsafe void AccesibleObject_IInvokeProviderInvoke_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IInvokeProviderInvoke(o));
        }

        [WinFormsFact]
        public void AccesibleObject_IValueProviderIsReadOnly_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IValueProviderGetIsReadOnly(o, BOOL.FALSE));
        }

        [WinFormsFact]
        public void AccesibleObject_IValueProviderValue_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IValueProviderGetValue(o, string.Empty));
        }

        [WinFormsTheory]
        [InlineData("101", "")]
        [InlineData("100", "")]
        [InlineData("1", "")]
        [InlineData("0", "")]
        [InlineData("-1", "")]
        [InlineData("abc", "")]
        public void AccesibleObject_IValueProviderSetValue_Invoke_GetReturnsExpected(string value, string expected)
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IValueProviderSetValue(o, value, expected));
        }

        [WinFormsFact]
        public void AccesibleObject_IValueProviderCollapse_Invoke_Success()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IExpandCollapseProviderCollapse(o, ExpandCollapseState.Collapsed));
        }

        [WinFormsFact]
        public void AccesibleObject_IValueProviderCollapse_InvokeAfterExpand_Success()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IExpandCollapseProviderExpand(o, ExpandCollapseState.Collapsed));
            AssertSuccess(Test_IExpandCollapseProviderCollapse(o, ExpandCollapseState.Collapsed));
        }

        [WinFormsFact]
        public void AccesibleObject_IExpandCollapseProviderExpand_Invoke_Success()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IExpandCollapseProviderCollapse(o, ExpandCollapseState.Collapsed));
        }

        [WinFormsFact]
        public void AccesibleObject_IExpandCollapseProviderExpand_InvokeAfterCollapse_Success()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IExpandCollapseProviderCollapse(o, ExpandCollapseState.Collapsed));
            AssertSuccess(Test_IExpandCollapseProviderCollapse(o, ExpandCollapseState.Collapsed));
        }

        [WinFormsFact]
        public void AccesibleObject_IExpandCollapseProviderExpandCollapseState_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IExpandCollapseProviderGetExpandCollapseState(o, ExpandCollapseState.Collapsed));
        }

        [WinFormsFact]
        public void AccesibleObject_IToggleProviderToggleState_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IToggleProviderGetToggleState(o, ToggleState.Indeterminate));
        }

        [WinFormsFact]
        public void AccesibleObject_IToggleProviderToggle_Invoke_Success()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IToggleProviderToggle(o, ToggleState.Indeterminate));
        }

        [WinFormsFact]
        public void AccesibleObject_ITableProviderRowOrColumnMajor_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ITableProviderGetRowOrColumnMajor(o, RowOrColumnMajor.RowMajor));
        }

        [WinFormsFact]
        public void AccesibleObject_ITableProviderGetColumnHeaders_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ITableProviderGetColumnHeaders(o));
        }

        [WinFormsFact]
        public void AccesibleObject_ITableProviderGetRowHeaders_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ITableProviderGetRowHeaders(o));
        }

        [WinFormsFact]
        public void AccesibleObject_ITableItemProviderGetColumnHeaderItems_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ITableItemProviderGetColumnHeaderItems(o));
        }

        [WinFormsFact]
        public void AccesibleObject_ITableItemProviderGetRowHeaderItems_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ITableItemProviderGetRowHeaderItems(o));
        }

        [WinFormsFact]
        public void AccesibleObject_IGridProviderColumnCount_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IGridProviderGetColumnCount(o, -1));
        }

        [WinFormsFact]
        public void AccesibleObject_IGridProviderRowCount_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IGridProviderGetRowCount(o, -1));
        }

        [WinFormsTheory]
        [InlineData(-1, -2)]
        [InlineData(-1, 0)]
        [InlineData(0, -1)]
        [InlineData(0, 0)]
        [InlineData(1, 0)]
        [InlineData(0, 1)]
        [InlineData(1, 2)]
        public void AccesibleObject_IGridProviderGetItem_Get_ReturnsExpected(int row, int column)
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IGridProviderGetItem(o, row, column));
        }

        [WinFormsFact]
        public void AccesibleObject_IGridItemProviderColumn_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IGridItemProviderGetColumn(o, -1));
        }

        [WinFormsFact]
        public void AccesibleObject_IGridItemProviderColumnSpan_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IGridItemProviderGetColumnSpan(o, 1));
        }

        [WinFormsFact]
        public void AccesibleObject_IGridItemProviderContainingGrid_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IGridItemProviderGetContainingGrid(o));
        }

        [WinFormsFact]
        public void AccesibleObject_IGridItemProviderRow_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IGridItemProviderGetRow(o, -1));
        }

        [WinFormsFact]
        public void AccesibleObject_IGridItemProviderRowSpan_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IGridItemProviderGetRowSpan(o, 1));
        }

        [WinFormsFact]
        public void AccesibleObject_IEnumVARIANTClone_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IEnumVARIANTClone(o));
        }

        [WinFormsFact]
        public void AccesibleObject_IEnumVARIANTNextReset_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IEnumVARIANTNextReset(o));
        }

        [WinFormsFact]
        public void AccesibleObject_IEnumVARIANTSkip_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IEnumVARIANTSkip(o));
        }

        [WinFormsTheory]
        [InlineData((int)BOOL.TRUE)]
        [InlineData((int)BOOL.FALSE)]
        public void AccesibleObject_IOleWindowContextSensitiveHelp_Invoke_ReturnsExpected(object fEnterMode)
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IOleWindowContextSensitiveHelp(o, (BOOL)fEnterMode, HRESULT.S_OK));
        }

        [WinFormsTheory]
        [InlineData((int)BOOL.TRUE)]
        [InlineData((int)BOOL.FALSE)]
        public void AccesibleObject_IOleWindowContextSensitiveHelp_InvokeWithParent_ReturnsExpected(object fEnterMode)
        {
            var o = new CustomParentAccessibleObject
            {
                ParentResult = new AccessibleObject()
            };
            AssertSuccess(Test_IOleWindowContextSensitiveHelp(o, (BOOL)fEnterMode, HRESULT.S_OK));
        }

        [WinFormsTheory]
        [InlineData((int)BOOL.TRUE)]
        [InlineData((int)BOOL.FALSE)]
        public void AccesibleObject_IOleWindowContextSensitiveHelp_InvokeWithControlParent_ReturnsExpected(object fEnterMode)
        {
            using var control = new Control();
            var o = new CustomParentAccessibleObject
            {
                ParentResult = new Control.ControlAccessibleObject(control)
            };
            AssertSuccess(Test_IOleWindowContextSensitiveHelp(o, (BOOL)fEnterMode, HRESULT.S_OK));
        }

        [WinFormsFact]
        public void AccesibleObject_IOleWindowGetWindow_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IOleWindowGetWindow(o, IntPtr.Zero, HRESULT.E_FAIL));
        }

        [WinFormsFact]
        public void AccesibleObject_IOleWindowGetWindow_InvokeWithParent_ReturnsExpected()
        {
            var o = new CustomParentAccessibleObject
            {
                ParentResult = new AccessibleObject()
            };
            AssertSuccess(Test_IOleWindowGetWindow(o, IntPtr.Zero, HRESULT.E_FAIL));
        }

        // Crashes with Attempted to read or write protected memory. This is often an indication that other memory is corrupt..
#if false
        [WinFormsFact]
        public void AccesibleObject_IOleWindowGetWindow_InvokeWithControlParent_ReturnsExpected()
        {
            using var control = new Control();
            var o = new CustomParentAccessibleObject
            {
                ParentResult = new Control.ControlAccessibleObject(control)
            };
            AssertSuccess(Test_IOleWindowGetWindow(o, control.Handle, HRESULT.S_OK));
        }
#endif

        [WinFormsFact]
        public void AccesibleObject_ILegacyIAccessibleProviderChildId_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderGetChildId(o, 0));
        }

        [WinFormsFact]
        public void AccesibleObject_ILegacyIAccessibleProviderDefaultAction_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderGetDefaultAction(o, null));
        }

        [WinFormsFact]
        public void AccesibleObject_ILegacyIAccessibleProviderDescription_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderGetDescription(o, null));
        }

        [WinFormsFact]
        public void AccesibleObject_ILegacyIAccessibleProviderHelp_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderGetHelp(o, null));
        }

        [WinFormsFact]
        public void AccesibleObject_ILegacyIAccessibleProviderKeyboardShortcut_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderGetKeyboardShortcut(o, null));
        }

        [WinFormsFact]
        public void AccesibleObject_ILegacyIAccessibleProviderName_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderGetName(o, null));
        }

        [WinFormsFact]
        public void AccesibleObject_ILegacyIAccessibleProviderRole_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderGetRole(o, 0));
        }

        [WinFormsFact]
        public void AccesibleObject_ILegacyIAccessibleProviderState_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderGetState(o, 0));
        }

        [WinFormsFact]
        public void AccesibleObject_ILegacyIAccessibleProviderValue_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderGetValue(o, string.Empty));
        }

        [WinFormsFact]
        public void AccesibleObject_ILegacyIAccessibleProviderDoDefaultAction_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderDoDefaultAction(o));
        }

        [WinFormsFact]
        public void AccesibleObject_ILegacyIAccessibleProviderGetIAccessible_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderGetIAccessible(o));
        }

        [WinFormsFact]
        public void AccesibleObject_ILegacyIAccessibleProviderGetSelection_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderGetSelection(o, BOOL.FALSE));
        }

        [WinFormsTheory]
        [InlineData("101", "")]
        [InlineData("100", "")]
        [InlineData("1", "")]
        [InlineData("0", "")]
        [InlineData("-1", "")]
        [InlineData("abc", "")]
        public void AccesibleObject_ILegacyIAccessibleProviderSetValue_Invoke_GetReturnsExpected(string value, string expected)
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderSetValue(o, value, expected));
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(4)]
        [InlineData(8)]
        [InlineData(16)]
        [InlineData(int.MaxValue)]
        public void AccesibleObject_ILegacyIAccessibleProviderSelect_Invoke_ReturnsExpected(int flagsSelect)
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ILegacyIAccessibleProviderSelect(o, flagsSelect));
        }

        [WinFormsFact]
        public void AccesibleObject_ISelectionProviderCanSelectMultiple_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ISelectionProviderGetCanSelectMultiple(o, BOOL.FALSE));
        }

        [WinFormsFact]
        public void AccesibleObject_ISelectionProviderIsSelectionRequired_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ISelectionProviderGetIsSelectionRequired(o, BOOL.FALSE));
        }

        [WinFormsFact]
        public void AccesibleObject_ISelectionProviderGetSelection_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ISelectionProviderGetSelection(o, BOOL.FALSE));
        }

        [WinFormsFact]
        public void AccesibleObject_ISelectionProviderIsSelected_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ISelectionItemProviderGetIsSelected(o, BOOL.FALSE));
        }

        [WinFormsFact]
        public void AccesibleObject_ISelectionProviderSelectionContainer_Get_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ISelectionItemProviderGetSelectionContainer(o, BOOL.FALSE));
        }

        [WinFormsFact]
        public void AccesibleObject_ISelectionItemProviderAddToSelection_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ISelectionItemProviderAddToSelection(o));
        }

        [WinFormsFact]
        public void AccesibleObject_ISelectionItemProviderRemoveFromSelection_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ISelectionItemProviderRemoveFromSelection(o));
        }

        [WinFormsFact]
        public void AccesibleObject_ISelectionItemProviderSelect_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_ISelectionItemProviderSelect(o));
        }

        [WinFormsFact]
        public void AccesibleObject_IRawElementProviderHwndOverrideGetOverrideProviderForHwnd_Invoke_ReturnsExpected()
        {
            var o = new AccessibleObject();
            AssertSuccess(Test_IRawElementProviderHwndOverrideGetOverrideProviderForHwnd(o));
        }

        [WinFormsFact]
        public void AccesibleObject_IScrollItemProviderScrollIntoView_Invoke_ReturnsExpected()
        {
            using (new NoAssertContext())
            {
                var o = new AccessibleObject();
                AssertSuccess(Test_IScrollItemProviderScrollIntoView(o));
            }
        }

        private class CustomParentAccessibleObject : AccessibleObject
        {
            public AccessibleObject ParentResult { get; set; }

            public override AccessibleObject Parent => ParentResult;
        }

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private unsafe static extern string Test_IAccessibleExConvertReturnedElement(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private unsafe static extern string Test_IAccessibleExGetIAccessiblePair(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private unsafe static extern string Test_IAccessibleExGetRuntimeId(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            int* expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private unsafe static extern string Test_IAccessibleExGetObjectForChild(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            int idChild);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IServiceProviderQueryService(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IRawElementProviderSimpleHostRawElementProvider(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            BOOL expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IRawElementProviderSimpleProviderOptions(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            ProviderOptions expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IRawElementProviderSimpleGetPatternProvider(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            UIA patternId,
            BOOL expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IRawElementProviderSimpleGetPropertyValue(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            UIA patternId,
            out VARIANT expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IRangeValueProviderGetIsReadOnly(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            BOOL expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IRangeValueProviderGetLargeChange(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            double expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IRangeValueProviderGetSmallChange(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            double expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IRangeValueProviderGetMaximum(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            double expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IRangeValueProviderGetMinimum(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            double expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IRangeValueProviderGetValue(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            double expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IRangeValueProviderSetValue(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            double value, double expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private unsafe static extern string Test_IRawElementProviderFragmentGetBoundingRectangle(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            UiaRect expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private unsafe static extern string Test_IRawElementProviderFragmentGetFragmentRoot(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private unsafe static extern string Test_IRawElementProviderFragmentGetEmbeddedFragmentRoots(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private unsafe static extern string Test_IRawElementProviderFragmentGetRuntimeId(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            int* expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IRawElementProviderFragmentNavigate(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            NavigateDirection expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private unsafe static extern string Test_IRawElementProviderFragmentSetFocus(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private unsafe static extern string Test_IRawElementProviderFragmentRootElementProviderFromPoint(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            double x,
            double y);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private unsafe static extern string Test_IRawElementProviderFragmentRootGetFocus(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private unsafe static extern string Test_IInvokeProviderInvoke(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IValueProviderGetIsReadOnly(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            BOOL expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IValueProviderGetValue(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            string expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IValueProviderSetValue(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            string value, string expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IExpandCollapseProviderGetExpandCollapseState(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            ExpandCollapseState expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IExpandCollapseProviderCollapse(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            ExpandCollapseState expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IExpandCollapseProviderExpand(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            ExpandCollapseState expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IToggleProviderGetToggleState(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            ToggleState expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IToggleProviderToggle(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            ToggleState expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ITableProviderGetRowOrColumnMajor(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            RowOrColumnMajor expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ITableProviderGetColumnHeaders(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ITableProviderGetRowHeaders(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ITableItemProviderGetColumnHeaderItems(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ITableItemProviderGetRowHeaderItems(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IGridProviderGetColumnCount(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            int expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IGridProviderGetRowCount(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            int expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IGridProviderGetItem(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            int row,
            int column);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IGridItemProviderGetColumn(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            int expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IGridItemProviderGetColumnSpan(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            int expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IGridItemProviderGetContainingGrid(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IGridItemProviderGetRow(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            int expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IGridItemProviderGetRowSpan(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            int expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IEnumVARIANTClone(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IEnumVARIANTNextReset(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IEnumVARIANTSkip(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IOleWindowContextSensitiveHelp(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            BOOL fEnterMode,
            HRESULT expectedHr);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IOleWindowGetWindow(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            IntPtr expected,
            HRESULT expectedHr);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderGetChildId(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            int expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderGetDefaultAction(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            string expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderGetDescription(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            string expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderGetHelp(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            string expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderGetKeyboardShortcut(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            string expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderGetName(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            string expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderGetRole(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            uint expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderGetState(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            uint expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderGetValue(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            string expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderDoDefaultAction(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderGetIAccessible(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderGetSelection(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            BOOL expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderSelect(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            int flagsSelect);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ILegacyIAccessibleProviderSetValue(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            string value, string expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ISelectionProviderGetCanSelectMultiple(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            BOOL expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ISelectionProviderGetIsSelectionRequired(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            BOOL expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ISelectionProviderGetSelection(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            BOOL expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ISelectionItemProviderGetIsSelected(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            BOOL expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ISelectionItemProviderGetSelectionContainer(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk,
            BOOL expected);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ISelectionItemProviderAddToSelection(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ISelectionItemProviderRemoveFromSelection(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_ISelectionItemProviderSelect(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IRawElementProviderHwndOverrideGetOverrideProviderForHwnd(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);

        [DllImport(NativeTests, ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern string Test_IScrollItemProviderScrollIntoView(
            [MarshalAs(UnmanagedType.IUnknown)] object pUnk);
    }
}
