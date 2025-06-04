// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Design.Tests;

public partial class ColorEditor_CustomColorDialogTests
{
    [WinFormsFact]
    public void CustomColorDialog_Ctor_Default()
    {
        Type? typeCustomColorDialog = typeof(ColorEditor).Assembly.GetTypes().SingleOrDefault(t => t.Name == "CustomColorDialog");
        Assert.NotNull(typeCustomColorDialog);

        using ColorDialog dialog = (ColorDialog)Activator.CreateInstance(typeCustomColorDialog!)!;
        Assert.NotNull(dialog);
    }
}
