﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace DemoConsole;

public class CustomButtonDesigner : ControlDesigner
{
    private DesignerActionListCollection _actionLists;

    public override DesignerActionListCollection ActionLists =>
        _actionLists ??= new DesignerActionListCollection
        {
            new CustomButtonDesignerActionList(Component)
        };
}
