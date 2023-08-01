// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#include "testhelpers.h"
#include <cassert>
#include <richedit.h>

extern "C" __declspec(dllexport) void WINAPI RichTextBox_EnLink(ENLINK* value)
{
    value->nmhdr.code = 132;
    value->nmhdr.hwndFrom = (HWND)765;
    value->nmhdr.idFrom = 432;
    value->msg = 22;
    value->wParam = 6578;
    value->lParam = 54425;
    value->chrg.cpMin = 109;
    value->chrg.cpMax = 1577;
}

extern "C" __declspec(dllexport) void WINAPI RichTextBox_EnProtected(ENPROTECTED* value)
{
    value->nmhdr.code = 132;
    value->nmhdr.hwndFrom = (HWND)765;
    value->nmhdr.idFrom = 432;
    value->msg = 22;
    value->wParam = 6578;
    value->lParam = 54425;
    value->chrg.cpMin = 109;
    value->chrg.cpMax = 1577;
}

extern "C" __declspec(dllexport) void WINAPI RichTextBox_EnDropFiles(ENDROPFILES* value)
{
    value->nmhdr.code = 132;
    value->nmhdr.hwndFrom = (HWND)765;
    value->nmhdr.idFrom = 432;
    value->hDrop = (HANDLE)22;
    value->cp = 6578;
    value->fProtected = TRUE;
}

extern "C" __declspec(dllexport) void WINAPI RichTextBox_CharRange(CHARRANGE* value)
{
    value->cpMin = 109;
    value->cpMax = 1577;
}

extern "C" __declspec(dllexport) void WINAPI RichTextBox_EditStream(EDITSTREAM* value)
{
    value->dwCookie = 109;
    value->dwError = 1577;
    value->pfnCallback = (EDITSTREAMCALLBACK)6578;
}

extern "C" __declspec(dllexport) void WINAPI RichTextBox_FindTextW(FINDTEXTW* value)
{
    value->lpstrText = L"Fine Text";
    value->chrg.cpMin = 109;
    value->chrg.cpMax = 1577;
}

extern "C" __declspec(dllexport) void WINAPI RichTextBox_GetTextEx(GETTEXTEX* value)
{
    value->cb = 132;
    value->flags = GT_RAWTEXT;
    value->codepage = 432;
    value->lpDefaultChar = (LPCSTR)22;
    value->lpUsedDefChar = (LPBOOL)6578;
}

extern "C" __declspec(dllexport) void WINAPI RichTextBox_GetTextLengthEx(GETTEXTLENGTHEX* value)
{
    value->flags = GTL_NUMBYTES;
    value->codepage = 432;
}

extern "C" __declspec(dllexport) void WINAPI RichTextBox_ParaFormat(PARAFORMAT* value)
{
    value->cbSize = 132;
    value->dwMask = PFM_ALIGNMENT;
    value->wNumbering = PFN_UCROMAN;
    value->wReserved = 6578;
    value->dxStartIndent = 109;
    value->dxRightIndent = 432;
    value->dxOffset = 54425;
    value->wAlignment = PFA_JUSTIFY;
    value->cTabCount = 6565;
    value->rgxTabs[0] = 8989;
    value->rgxTabs[31] = 812;
}

extern "C" __declspec(dllexport) void WINAPI RichTextBox_ReqResize(REQRESIZE* value)
{
    value->nmhdr.code = 132;
    value->nmhdr.hwndFrom = (HWND)765;
    value->nmhdr.idFrom = 432;
    value->rc.left = 6578;
    value->rc.right = 109;
    value->rc.top = 54425;
    value->rc.bottom = 8989;
}

extern "C" __declspec(dllexport) void WINAPI RichTextBox_SelChange(SELCHANGE* value)
{
    value->nmhdr.code = 132;
    value->nmhdr.hwndFrom = (HWND)765;
    value->nmhdr.idFrom = 432;
    value->seltyp = SEL_MULTICHAR;
    value->chrg.cpMin = 109;
    value->chrg.cpMax = 1577;
}

extern "C" __declspec(dllexport) void WINAPI RichTextBox_TextRange(TEXTRANGE* value)
{
    value->lpstrText = "Fine Text";
    value->chrg.cpMin = 109;
    value->chrg.cpMax = 1577;
}
