// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Design;

namespace System.Windows.Forms.Design.Tests;

public class DataMemberListEditorTests
{
    [Fact]
    public void DataMemberListEditor_GetEditStyle()
    {
        new DataMemberListEditor().GetEditStyle().Should().Be(UITypeEditorEditStyle.DropDown);
    }

    [Fact]
    public void DataMemberListEditor_IsDropDownResizable()
    {
        new DataMemberListEditor().IsDropDownResizable.Should().Be(true);
    }
}
