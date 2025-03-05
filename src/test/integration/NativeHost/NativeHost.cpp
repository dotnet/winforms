// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#include "pch.h"

class CMainWindow : public CFrameWnd
{
public:
	CWnd m_control;

	BOOL Create()
	{
		CREATESTRUCT cs{};
		cs.style = WS_OVERLAPPEDWINDOW;

		if (!PreCreateWindow(cs))
			return FALSE;

		if (!CFrameWnd::Create(cs.lpszClass, m_strTitle))
			return FALSE;

		return TRUE;
	}

	afx_msg int OnCreate(LPCREATESTRUCT lpCreateStruct)
	{
		if (CFrameWnd::OnCreate(lpCreateStruct) == -1)
			return -1;

		CLSID clsid;
		if (FAILED(IIDFromString(L"{54479E5D-EABC-448C-A767-EAFF17BC28C9}", &clsid)))
			return -1;

		if (!m_control.CreateControl(clsid, NULL, AFX_WS_DEFAULT_VIEW, CRect(), this, AFX_IDW_PANE_FIRST))
			return -1;

		return 0;
	}

	afx_msg void OnSize(UINT nType, int cx, int cy)
	{
		if (m_control)
			m_control.SetWindowPos(NULL, 0, 0, cx, cy, SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOZORDER);
	}

	afx_msg void OnSetFocus(CWnd* pOldWnd)
	{
		if (m_control)
			m_control.SetFocus();
	}

	DECLARE_MESSAGE_MAP()
};

BEGIN_MESSAGE_MAP(CMainWindow, CFrameWnd)
	ON_WM_CREATE()
	ON_WM_SIZE()
	ON_WM_SETFOCUS()
END_MESSAGE_MAP()

class CNativeHostApp : public CWinApp
{
public:
	BOOL InitInstance() override
	{
		CWinApp::InitInstance();

		AfxEnableControlContainer();

		auto pWindow = new CMainWindow;
		if (!pWindow)
			return FALSE;

		m_pMainWnd = pWindow;
		
		pWindow->SetTitle(L"Native WinForms Host");

		if (!pWindow->Create())
			return FALSE;

		pWindow->ShowWindow(SW_SHOW);
		pWindow->UpdateWindow();

		return TRUE;
	}

	int ExitInstance() override
	{
		// a clean exit is currently not possible, we would have to force a garbage collection
		// to ensure WinForms had a chance to release its COM objects before MFC exits
		TerminateProcess(GetCurrentProcess(), S_OK);

		return CWinApp::ExitInstance();
	}
};

CNativeHostApp app;
