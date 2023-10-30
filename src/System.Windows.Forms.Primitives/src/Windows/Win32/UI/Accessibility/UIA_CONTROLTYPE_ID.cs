// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Accessibility;

// Remove this definition and use Cswin32 definition instead once
// https://github.com/microsoft/win32metadata/issues/1723 is resolved
// and we update Cswin32 version that includes the fix.
internal enum UIA_CONTROLTYPE_ID : int
{
    UIA_ButtonControlTypeId = 50000,
    UIA_CalendarControlTypeId = 50001,
    UIA_CheckBoxControlTypeId = 50002,
    UIA_ComboBoxControlTypeId = 50003,
    UIA_EditControlTypeId = 50004,
    UIA_HyperlinkControlTypeId = 50005,
    UIA_ImageControlTypeId = 50006,
    UIA_ListItemControlTypeId = 50007,
    UIA_ListControlTypeId = 50008,
    UIA_MenuControlTypeId = 50009,
    UIA_MenuBarControlTypeId = 50010,
    UIA_MenuItemControlTypeId = 50011,
    UIA_ProgressBarControlTypeId = 50012,
    UIA_RadioButtonControlTypeId = 50013,
    UIA_ScrollBarControlTypeId = 50014,
    UIA_SliderControlTypeId = 50015,
    UIA_SpinnerControlTypeId = 50016,
    UIA_StatusBarControlTypeId = 50017,
    UIA_TabControlTypeId = 50018,
    UIA_TabItemControlTypeId = 50019,
    UIA_TextControlTypeId = 50020,
    UIA_ToolBarControlTypeId = 50021,
    UIA_ToolTipControlTypeId = 50022,
    UIA_TreeControlTypeId = 50023,
    UIA_TreeItemControlTypeId = 50024,
    UIA_CustomControlTypeId = 50025,
    UIA_GroupControlTypeId = 50026,
    UIA_ThumbControlTypeId = 50027,
    UIA_DataGridControlTypeId = 50028,
    UIA_DataItemControlTypeId = 50029,
    UIA_DocumentControlTypeId = 50030,
    UIA_SplitButtonControlTypeId = 50031,
    UIA_WindowControlTypeId = 50032,
    UIA_PaneControlTypeId = 50033,
    UIA_HeaderControlTypeId = 50034,
    UIA_HeaderItemControlTypeId = 50035,
    UIA_TableControlTypeId = 50036,
    UIA_TitleBarControlTypeId = 50037,
    UIA_SeparatorControlTypeId = 50038,
    UIA_SemanticZoomControlTypeId = 50039,
    UIA_AppBarControlTypeId = 50040,
}
