// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal;

internal sealed partial class PropertyGridView
{
    [Flags]
    private enum Flags : ushort
    {
        NeedsRefresh            = 0x0001,
        IsNewSelection          = 0x0002,
        IsSplitterMove          = 0x0004,
        IsSpecialKey            = 0x0008,
        InPropertySet           = 0x0010,
        DropDownClosing         = 0x0020,
        DropDownCommit          = 0x0040,
        NeedUpdateUIBasedOnFont = 0x0080,

        /// <summary>
        ///  The editor is currently launched via the drop-down button.
        /// </summary>
        ButtonLaunchedEditor    = 0x0100,
        NoDefault               = 0x0200,
        ResizableDropDown       = 0x0400
    }
}
