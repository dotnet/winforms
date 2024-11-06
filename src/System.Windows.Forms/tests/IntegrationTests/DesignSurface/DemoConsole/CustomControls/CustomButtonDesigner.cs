// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace TestConsole;

public class CustomButtonDesigner : ControlDesigner
{
    private DesignerActionListCollection _actionLists;

    public override DesignerActionListCollection ActionLists =>
        _actionLists ??= new DesignerActionListCollection
        {
            new CustomButtonDesignerActionList(Component)
        };
}
