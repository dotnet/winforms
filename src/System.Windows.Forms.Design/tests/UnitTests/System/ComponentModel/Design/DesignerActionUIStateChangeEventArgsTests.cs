// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design.Tests;

public class DesignerActionUIStateChangeEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Object_DesignerActionUIStateChangeType_TestData()
    {
        yield return new object[] { null, DesignerActionUIStateChangeType.Show - 1 };
        yield return new object[] { new(), DesignerActionUIStateChangeType.Show };
    }

    [Theory]
    [MemberData(nameof(Ctor_Object_DesignerActionUIStateChangeType_TestData))]
    public void Ctor_Object_DesignerActionUIStateChangeType(object relatedObject, DesignerActionUIStateChangeType changeType)
    {
        DesignerActionUIStateChangeEventArgs e = new(relatedObject, changeType);
        Assert.Same(relatedObject, e.RelatedObject);
        Assert.Equal(changeType, e.ChangeType);
    }
}
