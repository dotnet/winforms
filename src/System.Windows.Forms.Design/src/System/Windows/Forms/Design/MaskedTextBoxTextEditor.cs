﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

internal class MaskedTextBoxTextEditor : UITypeEditor
{
    public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
    {
        if (context?.Instance is null | !provider.TryGetService(out IWindowsFormsEditorService editorService))
        {
            return value;
        }

        var maskedTextBox = context.Instance as MaskedTextBox;
        if (maskedTextBox is null)
        {
            maskedTextBox = new();
            maskedTextBox.Text = value as string;
        }

        MaskedTextBoxTextEditorDropDown dropDown = new(maskedTextBox);
        editorService.DropDownControl(dropDown);

        if (dropDown.Value is not null)
        {
            value = dropDown.Value;
        }

        return value;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
    {
        if (context?.Instance is not null)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        return base.GetEditStyle(context);
    }

    public override bool GetPaintValueSupported(ITypeDescriptorContext context)
    {
        if (context?.Instance is not null)
        {
            return false;
        }

        return base.GetPaintValueSupported(context);
    }

    public override bool IsDropDownResizable => false;
}
