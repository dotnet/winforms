// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal class FocusableLabel : Label
{
    public FocusableLabel()
    {
        SetStyle(ControlStyles.Selectable, true);
        TabStop = true;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams createParams = base.CreateParams;
            createParams.ClassName = null;
            return createParams;
        }
    }

    internal override bool SupportsUiaProviders => false;
}
