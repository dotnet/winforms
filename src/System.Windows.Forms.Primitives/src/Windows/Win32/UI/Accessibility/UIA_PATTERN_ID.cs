// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Accessibility;

// Remove this definition and use Cswin32 definition instead once
// https://github.com/microsoft/win32metadata/issues/1723 is resolved
// and we update Cswin32 version that includes the fix.
internal enum UIA_PATTERN_ID : int
{
    UIA_InvokePatternId = 10000,
    UIA_SelectionPatternId = 10001,
    UIA_ValuePatternId = 10002,
    UIA_RangeValuePatternId = 10003,
    UIA_ScrollPatternId = 10004,
    UIA_ExpandCollapsePatternId = 10005,
    UIA_GridPatternId = 10006,
    UIA_GridItemPatternId = 10007,
    UIA_MultipleViewPatternId = 10008,
    UIA_WindowPatternId = 10009,
    UIA_SelectionItemPatternId = 10010,
    UIA_DockPatternId = 10011,
    UIA_TablePatternId = 10012,
    UIA_TableItemPatternId = 10013,
    UIA_TextPatternId = 10014,
    UIA_TogglePatternId = 10015,
    UIA_TransformPatternId = 10016,
    UIA_ScrollItemPatternId = 10017,
    UIA_LegacyIAccessiblePatternId = 10018,
    UIA_ItemContainerPatternId = 10019,
    UIA_VirtualizedItemPatternId = 10020,
    UIA_SynchronizedInputPatternId = 10021,
    UIA_ObjectModelPatternId = 10022,
    UIA_AnnotationPatternId = 10023,
    UIA_TextPattern2Id = 10024,
    UIA_StylesPatternId = 10025,
    UIA_SpreadsheetPatternId = 10026,
    UIA_SpreadsheetItemPatternId = 10027,
    UIA_TransformPattern2Id = 10028,
    UIA_TextChildPatternId = 10029,
    UIA_DragPatternId = 10030,
    UIA_DropTargetPatternId = 10031,
    UIA_TextEditPatternId = 10032,
    UIA_CustomNavigationPatternId = 10033,
    UIA_SelectionPattern2Id = 10034,
}
