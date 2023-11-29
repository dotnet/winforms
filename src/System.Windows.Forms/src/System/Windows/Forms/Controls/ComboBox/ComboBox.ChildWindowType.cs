// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ComboBox
{
    /// <summary>
    ///  Represents the ComboBox child native window type.
    /// </summary>
    private enum ChildWindowType
    {
        ListBox,
        Edit,
        DropDownList
    }
}
