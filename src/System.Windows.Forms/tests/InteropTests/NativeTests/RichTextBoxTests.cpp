// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "testhelpers.h"
#include <cassert>
#include <richedit.h>

void WINAPI RichTextBox_Enlink(ENLINK* enlink)
{
    enlink->nmhdr.code = 132;
    enlink->nmhdr.hwndFrom = (HWND)765;
    enlink->nmhdr.idFrom = 432;
    enlink->msg = 22;
    enlink->wParam = 6578;
    enlink->lParam = 54425;
    enlink->chrg.cpMin = 109;
    enlink->chrg.cpMax = 1577;
}

void WINAPI RichTextBox_Enprotected(ENPROTECTED* enprotected)
{
    enprotected->nmhdr.code = 132;
    enprotected->nmhdr.hwndFrom = (HWND)765;
    enprotected->nmhdr.idFrom = 432;
    enprotected->msg = 22;
    enprotected->wParam = 6578;
    enprotected->lParam = 54425;
    enprotected->chrg.cpMin = 109;
    enprotected->chrg.cpMax = 1577;
}
