// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#pragma once

// Including SDKDDKVer.h defines the highest available Windows platform.
// If you wish to build your application for a previous Windows platform, include WinSDKVer.h and
// set the _WIN32_WINNT macro to the platform you wish to support before including SDKDDKVer.h.
#include <sdkddkver.h>

#define _ATL_CSTRING_EXPLICIT_CONSTRUCTORS // some CString constructors will be explicit
#define _AFX_NO_MFC_CONTROLS_IN_DIALOGS // remove support for MFC controls in dialogs
#define VC_EXTRALEAN // Exclude rarely-used stuff from Windows headers

#include <afx.h>
#include <afxwin.h>
#include <afxdtctl.h>
#include <MsHTML.h>

#ifdef _DEBUG
#define new DEBUG_NEW
#endif
