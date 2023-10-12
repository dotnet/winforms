// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design.Tests;

public class DesignerActionListsChangedEventArgsTests
{
    public static IEnumerable<object[]> Ctor_Object_DesignerActionListsChangedType_DesignerActionListCollection_TestData()
    {
        yield return new object[] { null, DesignerActionListsChangedType.ActionListsAdded - 1, null };
        yield return new object[] { new(), DesignerActionListsChangedType.ActionListsAdded, new DesignerActionListCollection() };
    }

    [Theory]
    [MemberData(nameof(Ctor_Object_DesignerActionListsChangedType_DesignerActionListCollection_TestData))]
    public void Ctor_Object_DesignerActionListsChangedType_DesignerActionListCollection(object relatedObject, DesignerActionListsChangedType changeType, DesignerActionListCollection actionLists)
    {
        DesignerActionListsChangedEventArgs e = new(relatedObject, changeType, actionLists);
        Assert.Same(relatedObject, e.RelatedObject);
        Assert.Equal(changeType, e.ChangeType);
        Assert.Same(actionLists, e.ActionLists);
    }
}
