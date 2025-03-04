// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using static System.Windows.Forms.ToolStripSplitButton;

namespace System.Windows.Forms.Tests;

public class ToolStripSplitButton_ToolStripSplitButtonUiaProviderTests
{
    [WinFormsFact]
    public void ToolStripSplitButtonUiaProvider_Ctor_OwnerToolStripSplitButtonCannotBeNull()
    {
        Assert.Throws<ArgumentNullException>(() => new ToolStripSplitButtonUiaProvider(null));
    }

    [WinFormsFact]
    public void ToolStripSplitButtonUiaProvider_IsIAccessibleExSupported_ReturnsExpected()
    {
        using ToolStripSplitButton toolStripSplitButton = new();

        ToolStripSplitButtonUiaProvider accessibleObject = new(toolStripSplitButton);

        Assert.True(accessibleObject.IsIAccessibleExSupported());
    }
}
