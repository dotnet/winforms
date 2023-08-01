// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "ocidl.h"
#include "testhelpers.h"
#include <sstream>
#include <atlbase.h>
using namespace ATL;

static HRESULT Test_IOleControlSite_OnControlInfoChanged(IOleControlSite* pControlSite, std::wstringstream& output)
{
    HRESULT hr;

    hr = pControlSite->OnControlInfoChanged();
    assertEqualHr(S_OK, hr);

    return S_OK;
}

static HRESULT Test_IOleControlSite_LockInPlaceActive(IOleControlSite* pControlSite, std::wstringstream& output)
{
    HRESULT hr;

    // Negative tests.
    hr = pControlSite->LockInPlaceActive(TRUE);
    assertEqualHr(E_NOTIMPL, hr);

    hr = pControlSite->LockInPlaceActive(TRUE);
    assertEqualHr(E_NOTIMPL, hr);

    return S_OK;
}

static HRESULT Test_IOleControlSite_GetExtendedControl(IOleControlSite* pControlSite, std::wstringstream& output)
{
    HRESULT hr;
    IDispatch* pDisp;

    pDisp = (IDispatch*)(long)0xdeadbeef;
    hr = pControlSite->GetExtendedControl(&pDisp);
    assertEqualHr(E_NOTIMPL, hr);
    assertNull(pDisp);

    // Negative tests.
    hr = pControlSite->GetExtendedControl(NULL);
    assertEqualHr(E_POINTER, hr);

    return S_OK;
}

static HRESULT Test_IOleControlSite_TransformCoords(IOleControlSite* pControlSite, std::wstringstream& output)
{
    HRESULT hr;
    POINTL ptlHimetric;
    POINTF ptfContainer;

    // HIMETRICTOCONTAINER, SIZE
    ptlHimetric.x = 1000;
    ptlHimetric.y = 2000;
    ptfContainer.x = 0;
    ptfContainer.y = 0;
    hr = pControlSite->TransformCoords(&ptlHimetric, &ptfContainer, XFORMCOORDS_HIMETRICTOCONTAINER | XFORMCOORDS_SIZE);
    assertEqualHr(S_OK, hr);
    assertEqualInt(1000, ptlHimetric.x);
    assertEqualInt(2000, ptlHimetric.y);
    assertEqualFloat(38, ptfContainer.x);
    assertEqualFloat(76, ptfContainer.y);

    ptlHimetric.x = 0;
    ptlHimetric.y = 0;
    ptfContainer.x = -1;
    ptfContainer.y = -2;
    hr = pControlSite->TransformCoords(&ptlHimetric, &ptfContainer, XFORMCOORDS_HIMETRICTOCONTAINER | XFORMCOORDS_SIZE);
    assertEqualHr(S_OK, hr);
    assertEqualInt(0, ptlHimetric.x);
    assertEqualInt(0, ptlHimetric.y);
    assertEqualFloat(0, ptfContainer.x);
    assertEqualFloat(0, ptfContainer.y);

    // HIMETRICTOCONTAINER, POSITION
    ptlHimetric.x = 1000;
    ptlHimetric.y = 2000;
    ptfContainer.x = 0;
    ptfContainer.y = 0;
    hr = pControlSite->TransformCoords(&ptlHimetric, &ptfContainer, XFORMCOORDS_HIMETRICTOCONTAINER | XFORMCOORDS_POSITION);
    assertEqualHr(S_OK, hr);
    assertEqualInt(1000, ptlHimetric.x);
    assertEqualInt(2000, ptlHimetric.y);
    assertEqualFloat(38, ptfContainer.x);
    assertEqualFloat(76, ptfContainer.y);

    ptlHimetric.x = 0;
    ptlHimetric.y = 0;
    ptfContainer.x = 1;
    ptfContainer.y = 2;
    hr = pControlSite->TransformCoords(&ptlHimetric, &ptfContainer, XFORMCOORDS_HIMETRICTOCONTAINER | XFORMCOORDS_POSITION);
    assertEqualHr(S_OK, hr);
    assertEqualInt(0, ptlHimetric.x);
    assertEqualInt(0, ptlHimetric.y);
    assertEqualFloat(0, ptfContainer.x);
    assertEqualFloat(0, ptfContainer.y);

    // CONTAINERTOHIMETRIC, SIZE
    ptlHimetric.x = 0;
    ptlHimetric.y = 0;
    ptfContainer.x = 38;
    ptfContainer.y = 76;
    hr = pControlSite->TransformCoords(&ptlHimetric, &ptfContainer, XFORMCOORDS_CONTAINERTOHIMETRIC | XFORMCOORDS_SIZE);
    assertEqualHr(S_OK, hr);
    assertEqualInt(1005, ptlHimetric.x);
    assertEqualInt(2011, ptlHimetric.y);
    assertEqualFloat(38, ptfContainer.x);
    assertEqualFloat(76, ptfContainer.y);

    ptlHimetric.x = 1;
    ptlHimetric.y = 2;
    ptfContainer.x = 0;
    ptfContainer.y = 0;
    hr = pControlSite->TransformCoords(&ptlHimetric, &ptfContainer, XFORMCOORDS_CONTAINERTOHIMETRIC | XFORMCOORDS_SIZE);
    assertEqualHr(S_OK, hr);
    assertEqualInt(0, ptlHimetric.x);
    assertEqualInt(0, ptlHimetric.y);
    assertEqualFloat(0, ptfContainer.x);
    assertEqualFloat(0, ptfContainer.y);

    // CONTAINERTOHIMETRIC, POSITION
    ptlHimetric.x = 0;
    ptlHimetric.y = 0;
    ptfContainer.x = 38;
    ptfContainer.y = 76;
    hr = pControlSite->TransformCoords(&ptlHimetric, &ptfContainer, XFORMCOORDS_CONTAINERTOHIMETRIC | XFORMCOORDS_POSITION);
    assertEqualHr(S_OK, hr);
    assertEqualInt(1005, ptlHimetric.x);
    assertEqualInt(2011, ptlHimetric.y);
    assertEqualFloat(38, ptfContainer.x);
    assertEqualFloat(76, ptfContainer.y);

    ptlHimetric.x = 1;
    ptlHimetric.y = 2;
    ptfContainer.x = 0;
    ptfContainer.y = 0;
    hr = pControlSite->TransformCoords(&ptlHimetric, &ptfContainer, XFORMCOORDS_CONTAINERTOHIMETRIC | XFORMCOORDS_POSITION);
    assertEqualHr(S_OK, hr);
    assertEqualInt(0, ptlHimetric.x);
    assertEqualInt(0, ptlHimetric.y);
    assertEqualFloat(0, ptfContainer.x);
    assertEqualFloat(0, ptfContainer.y);

    // Negative tests.
    hr = pControlSite->TransformCoords(NULL, &ptfContainer, XFORMCOORDS_HIMETRICTOCONTAINER | XFORMCOORDS_SIZE);
    assertEqualHr(E_POINTER, hr);

    hr = pControlSite->TransformCoords(&ptlHimetric, NULL, XFORMCOORDS_HIMETRICTOCONTAINER | XFORMCOORDS_SIZE);
    assertEqualHr(E_POINTER, hr);

    hr = pControlSite->TransformCoords(&ptlHimetric, &ptfContainer, XFORMCOORDS_HIMETRICTOCONTAINER);
    assertEqualHr(E_INVALIDARG, hr);

    hr = pControlSite->TransformCoords(&ptlHimetric, &ptfContainer, XFORMCOORDS_CONTAINERTOHIMETRIC);
    assertEqualHr(E_INVALIDARG, hr);

    return S_OK;
}

static HRESULT Test_IOleControlSite_TranslateAccelerator(IOleControlSite* pControlSite, std::wstringstream& output)
{
    HRESULT hr;
    MSG msg = { 0 };

    hr = pControlSite->TranslateAccelerator(&msg, 0);
    assertEqualHr(S_FALSE, hr);

    // Negative tests.
    hr = pControlSite->TranslateAccelerator(NULL, 0);
    assertEqualHr(E_POINTER, hr);

    return S_OK;
}

static HRESULT Test_IOleControlSite_OnFocus(IOleControlSite* pControlSite, std::wstringstream& output)
{
    HRESULT hr;

    hr = pControlSite->OnFocus(TRUE);
    assertEqualHr(S_OK, hr);

    hr = pControlSite->OnFocus(TRUE);
    assertEqualHr(S_OK, hr);

    return S_OK;
}

static HRESULT Test_IOleControlSite_ShowPropertyFrame(IOleControlSite* pControlSite, std::wstringstream& output)
{
    HRESULT hr;

    // Negative tests.
    hr = pControlSite->ShowPropertyFrame();
    assertEqualHr(E_NOTIMPL, hr);

    return S_OK;
}

static HRESULT Test_IOleControlSite(IOleControlSite* pControlSite, std::wstringstream& output)
{
    HRESULT hr;

    hr = Test_IOleControlSite_OnControlInfoChanged(pControlSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleControlSite_GetExtendedControl(pControlSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleControlSite_LockInPlaceActive(pControlSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleControlSite_TransformCoords(pControlSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleControlSite_TranslateAccelerator(pControlSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleControlSite_OnFocus(pControlSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleControlSite_ShowPropertyFrame(pControlSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    return S_OK;
}

static HRESULT Test_ISimpleFrameSite_PreMessageFilter(ISimpleFrameSite* pSimpleFrameSite, std::wstringstream& output)
{
    HRESULT hr;

    hr = pSimpleFrameSite->PreMessageFilter(NULL, 0, NULL, NULL, NULL, NULL);
    assertEqualHr(S_OK, hr);

    hr = pSimpleFrameSite->PreMessageFilter((HWND)1, 1, (WPARAM)1, (LPARAM)1, (LRESULT*)1, (DWORD*)1);
    assertEqualHr(S_OK, hr);

    return S_OK;
}

static HRESULT Test_ISimpleFrameSite_PostMessageFilter(ISimpleFrameSite* pSimpleFrameSite, std::wstringstream& output)
{
    HRESULT hr;

    hr = pSimpleFrameSite->PostMessageFilter(NULL, 0, NULL, NULL, NULL, 0);
    assertEqualHr(S_FALSE, hr);

    hr = pSimpleFrameSite->PostMessageFilter((HWND)1, 1, (WPARAM)1, (LPARAM)1, (LRESULT*)1, 1);
    assertEqualHr(S_FALSE, hr);

    return S_OK;
}

static HRESULT Test_ISimpleFrameSite(ISimpleFrameSite* pSimpleFrameSite, std::wstringstream& output)
{
    HRESULT hr;

    hr = Test_ISimpleFrameSite_PreMessageFilter(pSimpleFrameSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_ISimpleFrameSite_PostMessageFilter(pSimpleFrameSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    return S_OK;
}

static HRESULT Test_IOleClientSite_SaveObject(IOleClientSite* pClientSite, std::wstringstream& output)
{
    HRESULT hr;

    // Negative tests.
    hr = pClientSite->SaveObject();
    assertEqualHr(E_NOTIMPL, hr);

    return S_OK;
}

static HRESULT Test_IOleClientSite_GetMoniker(IOleClientSite* pClientSite, std::wstringstream& output)
{
    HRESULT hr;
    IMoniker* pmk;

    // Negative tests.
    pmk = (IMoniker*)(long)0xdeadbeef;
    hr = pClientSite->GetMoniker(0, 0, &pmk);
    assertEqualHr(E_NOTIMPL, hr);
    assertNull(pmk);

    hr = pClientSite->GetMoniker(0, 0, NULL);
    assertEqualHr(E_POINTER, hr);

    return S_OK;
}

static HRESULT Test_IOleClientSite_GetContainer(IOleClientSite* pClientSite, std::wstringstream& output)
{
    HRESULT hr;
    IOleContainer* pContainer;

    pContainer = (IOleContainer*)(long)0xdeadbeef;
    hr = pClientSite->GetContainer(&pContainer);
    assertEqualHr(S_OK, hr);
    assertNotNull(pContainer);

#if false
    // Negative tests.
    hr = pClientSite->GetContainer(NULL);
    assertEqualHr(E_POINTER, hr);
#endif

    return S_OK;
}

static HRESULT Test_IOleClientSite_ShowObject(IOleClientSite* pClientSite, std::wstringstream& output)
{
    HRESULT hr;

    hr = pClientSite->ShowObject();
    assertEqualHr(S_OK, hr);

    return S_OK;
}

static HRESULT Test_IOleClientSite_OnShowWindow(IOleClientSite* pClientSite, std::wstringstream& output)
{
    HRESULT hr;

    hr = pClientSite->OnShowWindow(TRUE);
    assertEqualHr(S_OK, hr);

    hr = pClientSite->OnShowWindow(FALSE);
    assertEqualHr(S_OK, hr);

    return S_OK;
}

static HRESULT Test_IOleClientSite_RequestNewObjectLayout(IOleClientSite* pClientSite, std::wstringstream& output)
{
    HRESULT hr;

    // Negative tests.
    hr = pClientSite->RequestNewObjectLayout();
    assertEqualHr(E_NOTIMPL, hr);

    return S_OK;
}

static HRESULT Test_IOleClientSite(IOleClientSite* pClientSite, std::wstringstream& output)
{
    HRESULT hr;

    hr = Test_IOleClientSite_SaveObject(pClientSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleClientSite_GetMoniker(pClientSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleClientSite_GetContainer(pClientSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleClientSite_ShowObject(pClientSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleClientSite_OnShowWindow(pClientSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleClientSite_RequestNewObjectLayout(pClientSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    return S_OK;
}

static HRESULT Test_IOleInPlaceSite_GetWindow(IOleInPlaceSite* pInPlaceSite, std::wstringstream& output)
{
    HRESULT hr;
    HWND hwnd;

    hwnd = NULL;
    hr = pInPlaceSite->GetWindow(&hwnd);
    assertEqualHr(S_OK, hr);
    assertNotNull(hwnd);

    // Negative tests.
    hr = pInPlaceSite->GetWindow(NULL);
    assertEqualHr(E_POINTER, hr);

    return S_OK;
}

static HRESULT Test_IOleInPlaceSite_ContextSensitiveHelp(IOleInPlaceSite* pInPlaceSite, std::wstringstream& output)
{
    HRESULT hr;

    // Negative tests.
    hr = pInPlaceSite->ContextSensitiveHelp(TRUE);
    assertEqualHr(E_NOTIMPL, hr);

    hr = pInPlaceSite->ContextSensitiveHelp(FALSE);
    assertEqualHr(E_NOTIMPL, hr);

    return S_OK;
}

static HRESULT Test_IOleInPlaceSite_CanInPlaceActivate(IOleInPlaceSite* pInPlaceSite, std::wstringstream& output)
{
    HRESULT hr;

    hr = pInPlaceSite->CanInPlaceActivate();
    assertEqualHr(S_OK, hr);

    return S_OK;
}

static HRESULT Test_IOleInPlaceSite_OnInPlaceActivate(IOleInPlaceSite* pInPlaceSite, std::wstringstream& output)
{
    HRESULT hr;

    hr = pInPlaceSite->OnInPlaceActivate();
    assertEqualHr(S_OK, hr);

    return S_OK;
}

static HRESULT Test_IOleInPlaceSite_OnUIActivate(IOleInPlaceSite* pInPlaceSite, std::wstringstream& output)
{
    HRESULT hr;

    hr = pInPlaceSite->OnUIActivate();
    assertEqualHr(S_OK, hr);

    return S_OK;
}

static HRESULT Test_IOleInPlaceSite_GetWindowContext(IOleInPlaceSite* pInPlaceSite, std::wstringstream& output)
{
    HRESULT hr;
    IOleInPlaceFrame* pFrame;
    IOleInPlaceUIWindow* pDoc;
    RECT posRect;
    RECT clipRect;
    OLEINPLACEFRAMEINFO frameInfo;

    pFrame = NULL;
    pDoc = NULL;
    posRect = { 0 };
    clipRect = { 0 };
    frameInfo = { 0 };
    hr = pInPlaceSite->GetWindowContext(&pFrame, &pDoc, &posRect, &clipRect, NULL);
    assertEqualHr(S_OK, hr);
    assertNotNull(pFrame);
    assertNull(pDoc);
    assertEqualInt(0, posRect.left);
    assertEqualInt(0, posRect.top);
    assertEqualInt(250, posRect.right);
    assertEqualInt(250, posRect.bottom);
    assertEqualInt(0, clipRect.left);
    assertEqualInt(0, clipRect.top);
    assertEqualInt(32000, clipRect.right);
    assertEqualInt(32000, clipRect.bottom);

    pFrame = NULL;
    pDoc = NULL;
    posRect = { 0 };
    clipRect = { 0 };
    frameInfo = { 0 };
    hr = pInPlaceSite->GetWindowContext(&pFrame, &pDoc, &posRect, &clipRect, &frameInfo);
    assertEqualHr(S_OK, hr);
    assertNotNull(pFrame);
    assertNull(pDoc);
    assertEqualInt(0, posRect.left);
    assertEqualInt(0, posRect.top);
    assertEqualInt(250, posRect.right);
    assertEqualInt(250, posRect.bottom);
    assertEqualInt(0, clipRect.left);
    assertEqualInt(0, clipRect.top);
    assertEqualInt(32000, clipRect.right);
    assertEqualInt(32000, clipRect.bottom);
    assertEqualInt(sizeof(OLEINPLACEFRAMEINFO), frameInfo.cb);
    assertEqualBool(FALSE, frameInfo.fMDIApp);
    assertEqualInt(0, frameInfo.cAccelEntries);
    assertNull(frameInfo.haccel);
    assertNull(frameInfo.hwndFrame);

    // Negative tests.
#if false
    hr = pInPlaceSite->GetWindowContext(NULL, &pDoc, &posRect, &clipRect, &frameInfo);
    assertEqualHr(E_POINTER, hr);

    hr = pInPlaceSite->GetWindowContext(&pFrame, NULL, &posRect, &clipRect, &frameInfo);
    assertEqualHr(E_POINTER, hr);
#endif

    hr = pInPlaceSite->GetWindowContext(&pFrame, &pDoc, NULL, &clipRect, &frameInfo);
    assertEqualHr(E_POINTER, hr);

    hr = pInPlaceSite->GetWindowContext(&pFrame, &pDoc, &posRect, NULL, &frameInfo);
    assertEqualHr(E_POINTER, hr);

    return S_OK;
}

static HRESULT Test_IOleInPlaceSite(IOleInPlaceSite* pInPlaceSite, std::wstringstream& output)
{
    HRESULT hr;

    hr = Test_IOleInPlaceSite_GetWindow(pInPlaceSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleInPlaceSite_ContextSensitiveHelp(pInPlaceSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleInPlaceSite_CanInPlaceActivate(pInPlaceSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleInPlaceSite_OnInPlaceActivate(pInPlaceSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleInPlaceSite_OnUIActivate(pInPlaceSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    hr = Test_IOleInPlaceSite_GetWindowContext(pInPlaceSite, output);
    if (hr != S_OK)
    {
        return hr;
    }

    return S_OK;
}

TEST const WCHAR* WINAPI Test_WebBrowserSiteBase(IUnknown* pUnknown)
{
    return RunTest([&](std::wstringstream& output)
    {
        HRESULT hr;
        CComPtr<IOleControlSite> pControlSite;
        CComPtr<ISimpleFrameSite> pSimpleFrameSite;
        CComPtr<IOleClientSite> pClientSite;
        CComPtr<IOleInPlaceSite> pInPlaceSite;

        hr = pUnknown->QueryInterface(IID_IOleControlSite, (void**)&pControlSite);
        hr = Test_IOleControlSite(pControlSite, output);
        if (hr != S_OK)
        {
            return hr;
        }

        hr = pUnknown->QueryInterface(IID_ISimpleFrameSite, (void**)&pSimpleFrameSite);
        hr = Test_ISimpleFrameSite(pSimpleFrameSite, output);
        if (hr != S_OK)
        {
            return hr;
        }

        hr = pUnknown->QueryInterface(IID_IOleClientSite, (void**)&pClientSite);
        hr = Test_IOleClientSite(pClientSite, output);
        if (hr != S_OK)
        {
            return hr;
        }

        hr = pUnknown->QueryInterface(IID_IOleInPlaceSite, (void**)&pInPlaceSite);
        hr = Test_IOleInPlaceSite(pInPlaceSite, output);
        if (hr != S_OK)
        {
            return hr;
        }

        return S_OK;
    });
}
