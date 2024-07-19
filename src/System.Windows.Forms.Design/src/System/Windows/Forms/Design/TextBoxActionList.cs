﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal class TextBoxActionList : DesignerActionList
{
    private readonly TextBox _textBox;

    public TextBoxActionList(TextBoxDesigner designer)
        : base(designer.Component)
    {
        _textBox = (TextBox)designer.Component;
    }

    public bool Multiline
    {
        get
        {
            return _textBox.Multiline;
        }
        set
        {
            TypeDescriptor.GetProperties(_textBox)["Multiline"]!.SetValue(Component, value);
        }
    }

    public override DesignerActionItemCollection GetSortedActionItems()
    {
        DesignerActionItemCollection items =
        [
            new DesignerActionPropertyItem("Multiline", string.Format(SR.MultiLineDisplayName, SR.PropertiesCategoryName, SR.MultiLineDescription)),
        ];
        return items;
    }
}
