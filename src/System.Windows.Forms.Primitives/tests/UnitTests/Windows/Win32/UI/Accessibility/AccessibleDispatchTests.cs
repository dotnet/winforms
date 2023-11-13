// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace Windows.Win32.System.Accessibility.Tests;

public partial class AccessibleDispatchTests
{
    [StaFact]
    public unsafe void AccessibleDispatch_InvokeViaDispatch()
    {
        AccessibleTestObject accessibleObject = new();
        using var dispatch = ComHelpers.GetComScope<IDispatch>(accessibleObject);
        dispatch.Value->GetIDOfName("accChildCount", out int dispId).Should().Be(HRESULT.S_OK);
        dispId.Should().Be(-5001);
        using VARIANT result = dispatch.Value->GetProperty(dispId);
        result.vt.Should().Be(VARENUM.VT_I4);
        ((int)result).Should().Be(42);
    }

    private unsafe class AccessibleTestObject : AccessibleDispatch, IAccessible.Interface, IManagedWrapper<IAccessible, IDispatch, IDispatchEx>
    {
        HRESULT IAccessible.Interface.get_accParent(IDispatch** ppdispParent) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accChildCount(int* pcountChildren)
        {
            *pcountChildren = 42;
            return HRESULT.S_OK;
        }

        HRESULT IAccessible.Interface.get_accChild(VARIANT varChild, IDispatch** ppdispChild) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accName(VARIANT varChild, BSTR* pszName) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accValue(VARIANT varChild, BSTR* pszValue) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accDescription(VARIANT varChild, BSTR* pszDescription) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accRole(VARIANT varChild, VARIANT* pvarRole) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accState(VARIANT varChild, VARIANT* pvarState) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accHelp(VARIANT varChild, BSTR* pszHelp) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accHelpTopic(BSTR* pszHelpFile, VARIANT varChild, int* pidTopic) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accKeyboardShortcut(VARIANT varChild, BSTR* pszKeyboardShortcut) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accFocus(VARIANT* pvarChild) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accSelection(VARIANT* pvarChildren) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.get_accDefaultAction(VARIANT varChild, BSTR* pszDefaultAction) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.accSelect(int flagsSelect, VARIANT varChild) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.accLocation(int* pxLeft, int* pyTop, int* pcxWidth, int* pcyHeight, VARIANT varChild) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.accNavigate(int navDir, VARIANT varStart, VARIANT* pvarEndUpAt) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.accHitTest(int xLeft, int yTop, VARIANT* pvarChild) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.accDoDefaultAction(VARIANT varChild) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.put_accName(VARIANT varChild, BSTR szName) => HRESULT.E_NOTIMPL;
        HRESULT IAccessible.Interface.put_accValue(VARIANT varChild, BSTR szValue) => HRESULT.E_NOTIMPL;
    }
}
