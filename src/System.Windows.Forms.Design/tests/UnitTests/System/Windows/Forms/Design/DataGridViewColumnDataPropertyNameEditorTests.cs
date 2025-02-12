// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Design;

namespace System.Windows.Forms.Design.Tests;

public class DataGridViewColumnDataPropertyNameEditorTests
{
    [Fact]
    public void DataGridViewColumnDataPropertyNameEditor_GetEditStyle()
    {
        new DataGridViewColumnDataPropertyNameEditor().GetEditStyle().Should().Be(UITypeEditorEditStyle.DropDown);
    }

    [Fact]
    public void DataGridViewColumnDataPropertyNameEditor_IsDropDownResizable()
    {
        new DataGridViewColumnDataPropertyNameEditor().IsDropDownResizable.Should().Be(true);
    }
}
