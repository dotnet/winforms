#include "AxTestControl.h"

#pragma warning(push)
#pragma warning(disable: 4355) // 'this' : used in base member initializer list

CAxTestControl::CAxTestControl() : m_ctlButton(_T("Button"), this, 1), m_count(0)
{
    m_bWindowOnly = TRUE;
}

#pragma warning(pop)

HRESULT CAxTestControl::FinalConstruct()
{
    return S_OK;
}

void CAxTestControl::FinalRelease()
{
}

LRESULT CAxTestControl::OnSetFocus(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
    LRESULT lRes = CComControl<CAxTestControl>::OnSetFocus(uMsg, wParam, lParam, bHandled);
    if (m_bInPlaceActive && !IsChild(::GetFocus()))
        m_ctlButton.SetFocus();
    return lRes;
}

LRESULT CAxTestControl::OnCreate(UINT uMsg, WPARAM wParam, LPARAM lParam, BOOL& bHandled)
{
    RECT rc;
    GetWindowRect(&rc);
    rc.right -= rc.left;
    rc.bottom -= rc.top;
    rc.top = rc.left = 0;
    m_ctlButton.Create(m_hWnd, rc);
    return 0;
}

STDMETHODIMP CAxTestControl::SetObjectRects(LPCRECT prcPos, LPCRECT prcClip)
{
    IOleInPlaceObjectWindowlessImpl<CAxTestControl>::SetObjectRects(prcPos, prcClip);
    int cx = prcPos->right - prcPos->left;
    int cy = prcPos->bottom - prcPos->top;
    ::SetWindowPos(m_ctlButton.m_hWnd, NULL, 0, 0, cx, cy, SWP_NOZORDER | SWP_NOACTIVATE);
    return S_OK;
}

void CAxTestControl::OnTextChanged()
{
    m_ctlButton.SetWindowText(CString(m_bstrText));
    Fire_OnTextChanged(m_bstrText);
}

LRESULT CAxTestControl::OnButtonClicked(WORD wNotifyCode, WORD wID, HWND hWndCtl, BOOL& bHandled)
{
    Fire_OnButtonClick(++m_count);
    Fire_OnClick();
    return 0;
}

HRESULT CAxTestControl::Fire_OnTextChanged(BSTR text)
{
    HRESULT hr = S_OK;
    int cConnections = m_vec.GetSize();

    for (int iConnection = 0; iConnection < cConnections; iConnection++)
    {
        CComPtr<IUnknown> punkConnection = m_vec.GetAt(iConnection);
        IDispatch* pConnection = static_cast<IDispatch*>(punkConnection.p);

        if (pConnection)
        {
            CComVariant avarParams[1];
            avarParams[0] = text;
            CComVariant varResult;
            DISPPARAMS params = { avarParams, NULL, 1, 0 };
            hr = pConnection->Invoke(1, IID_NULL, LOCALE_USER_DEFAULT, DISPATCH_METHOD, &params, &varResult, NULL, NULL);
        }
    }

    return hr;
}

HRESULT CAxTestControl::Fire_OnButtonClick(LONG count)
{
    HRESULT hr = S_OK;
    int cConnections = m_vec.GetSize();

    for (int iConnection = 0; iConnection < cConnections; iConnection++)
    {
        CComPtr<IUnknown> punkConnection = m_vec.GetAt(iConnection);
        IDispatch* pConnection = static_cast<IDispatch*>(punkConnection.p);

        if (pConnection)
        {
            CComVariant avarParams[1];
            avarParams[0] = count;
            CComVariant varResult;
            DISPPARAMS params = { avarParams, NULL, 1, 0 };
            hr = pConnection->Invoke(2, IID_NULL, LOCALE_USER_DEFAULT, DISPATCH_METHOD, &params, &varResult, NULL, NULL);
        }
    }

    return hr;
}

HRESULT CAxTestControl::Fire_OnClick()
{
    HRESULT hr = S_OK;
    int cConnections = m_vec.GetSize();

    for (int iConnection = 0; iConnection < cConnections; iConnection++)
    {
        CComPtr<IUnknown> punkConnection = m_vec.GetAt(iConnection);
        IDispatch* pConnection = static_cast<IDispatch*>(punkConnection.p);

        if (pConnection)
        {
            CComVariant varResult;
            DISPPARAMS params = { NULL, NULL, 0, 0 };
            hr = pConnection->Invoke(DISPID_CLICK, IID_NULL, LOCALE_USER_DEFAULT, DISPATCH_METHOD, &params, &varResult, NULL, NULL);
        }
    }

    return hr;
}

OBJECT_ENTRY_AUTO(__uuidof(AxTestControl), CAxTestControl);
