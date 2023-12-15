// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

public class HelpTests
{
    [Fact]
    public void ShowPopupTest()
    {
        Help.ShowPopup(null, "Popup", Point.Empty);
    }
}
