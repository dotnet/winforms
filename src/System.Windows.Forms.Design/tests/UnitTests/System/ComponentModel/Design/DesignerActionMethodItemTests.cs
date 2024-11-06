// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Reflection;

namespace System.ComponentModel.Design.Tests;

public class DesignerActionMethodItemTests
{
    public static IEnumerable<object[]> Ctor_DesignerActionList_String_String_String_String_Bool_TestData()
    {
        yield return new object[] { new DesignerActionList(null), "memberName", "displayName", "category", "description", false, "displayName" };
        yield return new object[] { new DesignerActionList(null), "member(&a)Name", "displa(&a)yName", "cate(&a)gory", "description", true, "displayName" };
        yield return new object[] { null, string.Empty, string.Empty, string.Empty, string.Empty, false, string.Empty };
        yield return new object[] { null, null, null, null, null, false, null };
    }

    [Theory]
    [MemberData(nameof(Ctor_DesignerActionList_String_String_String_String_Bool_TestData))]
    public void DesignerActionMethodItem_Ctor_DesignerActionList_String_String_String_String_Bool(DesignerActionList actionList, string memberName, string displayName, string category, string description, bool includeAsDesignerVerb, string expectedDisplayName)
    {
        DesignerActionMethodItem item = new(actionList, memberName, displayName, category, description, includeAsDesignerVerb);
        Assert.Equal(memberName, item.MemberName);
        Assert.Equal(expectedDisplayName, item.DisplayName);
        Assert.Equal(category, item.Category);
        Assert.Equal(description, item.Description);
        Assert.Equal(includeAsDesignerVerb, item.IncludeAsDesignerVerb);
        Assert.False(item.AllowAssociate);
        Assert.Empty(item.Properties);
        Assert.Same(item.Properties, item.Properties);
        Assert.IsType<HybridDictionary>(item.Properties);
        Assert.True(item.ShowInSourceView);
        Assert.Null(item.RelatedComponent);
    }

    public static IEnumerable<object[]> Ctor_DesignerActionList_String_String_String_String_TestData()
    {
        yield return new object[] { new DesignerActionList(null), "memberName", "displayName", "category", "description", "displayName" };
        yield return new object[] { new DesignerActionList(null), "member(&a)Name", "displa(&a)yName", "cate(&a)gory", "description", "displayName" };
        yield return new object[] { null, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty };
        yield return new object[] { null, null, null, null, null, null };
    }

    [Theory]
    [MemberData(nameof(Ctor_DesignerActionList_String_String_String_String_TestData))]
    public void DesignerActionMethodItem_Ctor_DesignerActionList_String_String_String_String(DesignerActionList actionList, string memberName, string displayName, string category, string description, string expectedDisplayName)
    {
        DesignerActionMethodItem item = new(actionList, memberName, displayName, category, description);
        Assert.Equal(memberName, item.MemberName);
        Assert.Equal(expectedDisplayName, item.DisplayName);
        Assert.Equal(category, item.Category);
        Assert.Equal(description, item.Description);
        Assert.False(item.IncludeAsDesignerVerb);
        Assert.False(item.AllowAssociate);
        Assert.Empty(item.Properties);
        Assert.Same(item.Properties, item.Properties);
        Assert.IsType<HybridDictionary>(item.Properties);
        Assert.True(item.ShowInSourceView);
        Assert.Null(item.RelatedComponent);
    }

    public static IEnumerable<object[]> Ctor_DesignerActionList_String_String_String_Bool_TestData()
    {
        yield return new object[] { new DesignerActionList(null), "memberName", "displayName", "category", false, "displayName" };
        yield return new object[] { new DesignerActionList(null), "member(&a)Name", "displa(&a)yName", "cate(&a)gory", true, "displayName" };
        yield return new object[] { null, string.Empty, string.Empty, string.Empty, false, string.Empty };
        yield return new object[] { null, null, null, null, false, null };
    }

    [Theory]
    [MemberData(nameof(Ctor_DesignerActionList_String_String_String_Bool_TestData))]
    public void DesignerActionMethodItem_Ctor_DesignerActionList_String_String_String_Bool(DesignerActionList actionList, string memberName, string displayName, string category, bool includeAsDesignerVerb, string expectedDisplayName)
    {
        DesignerActionMethodItem item = new(actionList, memberName, displayName, category, includeAsDesignerVerb);
        Assert.Equal(memberName, item.MemberName);
        Assert.Equal(expectedDisplayName, item.DisplayName);
        Assert.Equal(category, item.Category);
        Assert.Null(item.Description);
        Assert.Equal(includeAsDesignerVerb, item.IncludeAsDesignerVerb);
        Assert.False(item.AllowAssociate);
        Assert.Empty(item.Properties);
        Assert.Same(item.Properties, item.Properties);
        Assert.IsType<HybridDictionary>(item.Properties);
        Assert.True(item.ShowInSourceView);
        Assert.Null(item.RelatedComponent);
    }

    public static IEnumerable<object[]> Ctor_DesignerActionList_String_String_String_TestData()
    {
        yield return new object[] { new DesignerActionList(null), "memberName", "displayName", "category", "displayName" };
        yield return new object[] { new DesignerActionList(null), "member(&a)Name", "displa(&a)yName", "cate(&a)gory", "displayName" };
        yield return new object[] { null, string.Empty, string.Empty, string.Empty, string.Empty };
        yield return new object[] { null, null, null, null, null };
    }

    [Theory]
    [MemberData(nameof(Ctor_DesignerActionList_String_String_String_TestData))]
    public void DesignerActionMethodItem_Ctor_DesignerActionList_String_String_String(DesignerActionList actionList, string memberName, string displayName, string category, string expectedDisplayName)
    {
        DesignerActionMethodItem item = new(actionList, memberName, displayName, category);
        Assert.Equal(memberName, item.MemberName);
        Assert.Equal(expectedDisplayName, item.DisplayName);
        Assert.Equal(category, item.Category);
        Assert.Null(item.Description);
        Assert.False(item.IncludeAsDesignerVerb);
        Assert.False(item.AllowAssociate);
        Assert.Empty(item.Properties);
        Assert.Same(item.Properties, item.Properties);
        Assert.IsType<HybridDictionary>(item.Properties);
        Assert.True(item.ShowInSourceView);
        Assert.Null(item.RelatedComponent);
    }

    public static IEnumerable<object[]> Ctor_DesignerActionList_String_String_Bool_TestData()
    {
        yield return new object[] { new DesignerActionList(null), "memberName", "displayName", false, "displayName" };
        yield return new object[] { new DesignerActionList(null), "member(&a)Name", "displa(&a)yName", true, "displayName" };
        yield return new object[] { null, string.Empty, string.Empty, false, string.Empty };
        yield return new object[] { null, null, null, false, null };
    }

    [Theory]
    [MemberData(nameof(Ctor_DesignerActionList_String_String_Bool_TestData))]
    public void DesignerActionMethodItem_Ctor_DesignerActionList_String_String_Bool(DesignerActionList actionList, string memberName, string displayName, bool includeAsDesignerVerb, string expectedDisplayName)
    {
        DesignerActionMethodItem item = new(actionList, memberName, displayName, includeAsDesignerVerb);
        Assert.Equal(memberName, item.MemberName);
        Assert.Equal(expectedDisplayName, item.DisplayName);
        Assert.Null(item.Category);
        Assert.Null(item.Description);
        Assert.Equal(includeAsDesignerVerb, item.IncludeAsDesignerVerb);
        Assert.False(item.AllowAssociate);
        Assert.Empty(item.Properties);
        Assert.Same(item.Properties, item.Properties);
        Assert.IsType<HybridDictionary>(item.Properties);
        Assert.True(item.ShowInSourceView);
        Assert.Null(item.RelatedComponent);
    }

    public static IEnumerable<object[]> Ctor_DesignerActionList_String_String_TestData()
    {
        yield return new object[] { new DesignerActionList(null), "memberName", "displayName", "displayName" };
        yield return new object[] { new DesignerActionList(null), "member(&a)Name", "displa(&a)yName", "displayName" };
        yield return new object[] { null, string.Empty, string.Empty, string.Empty };
        yield return new object[] { null, null, null, null };
    }

    [Theory]
    [MemberData(nameof(Ctor_DesignerActionList_String_String_TestData))]
    public void DesignerActionMethodItem_Ctor_DesignerActionList_String_String(DesignerActionList actionList, string memberName, string displayName, string expectedDisplayName)
    {
        DesignerActionMethodItem item = new(actionList, memberName, displayName);
        Assert.Equal(memberName, item.MemberName);
        Assert.Equal(expectedDisplayName, item.DisplayName);
        Assert.Null(item.Category);
        Assert.Null(item.Description);
        Assert.False(item.IncludeAsDesignerVerb);
        Assert.False(item.AllowAssociate);
        Assert.Empty(item.Properties);
        Assert.Same(item.Properties, item.Properties);
        Assert.IsType<HybridDictionary>(item.Properties);
        Assert.True(item.ShowInSourceView);
        Assert.Null(item.RelatedComponent);
    }

    public static IEnumerable<object[]> RelatedComponent_Set_TestData()
    {
        yield return new object[] { new Component() };
        yield return new object[] { null };
    }

    [Theory]
    [MemberData(nameof(RelatedComponent_Set_TestData))]
    public void DesignerActionMethodItem_RelatedComponent_Set_GetReturnsExpected(IComponent value)
    {
        DesignerActionMethodItem item = new(null, "memberName", "displayName", "category", "description")
        {
            RelatedComponent = value
        };
        Assert.Same(value, item.RelatedComponent);

        // Set same.
        item.RelatedComponent = value;
        Assert.Same(value, item.RelatedComponent);
    }

    [Theory]
    [InlineData(nameof(SubDesignerActionList.PublicMethod))]
    [InlineData("PrivateMethod")]
    public void Invoke_ValidMemberName_ReturnsExpected(string memberName)
    {
        SubDesignerActionList list = new();
        DesignerActionMethodItem item = new(list, memberName, "displayName", "category", "description");
        item.Invoke();
        Assert.Equal(memberName, list.CalledMethod);

        // Call again to test caching behavior.
        list.CalledMethod = null;
        item.Invoke();
        Assert.Equal(memberName, list.CalledMethod);
    }

    [Fact]
    public void Invoke_NullActionList_ThrowsInvalidOperationException()
    {
        DesignerActionMethodItem item = new(null, "memberName", "displayName", "category", "description");
        Assert.Throws<InvalidOperationException>(item.Invoke);
    }

    [Theory]
    [InlineData("")]
    [InlineData("NoSuchMember")]
    [InlineData(nameof(SubDesignerActionList.StaticMethod))]
    public void Invoke_NoSuchMemberName_ThrowsInvalidOperationException(string memberName)
    {
        SubDesignerActionList list = new();
        DesignerActionMethodItem item = new(list, memberName, "displayName", "category", "description");
        Assert.Throws<InvalidOperationException>(item.Invoke);
    }

    [Fact]
    public void Invoke_NullMemberName_ThrowsArgumentNullException()
    {
        SubDesignerActionList list = new();
        DesignerActionMethodItem item = new(list, null, "displayName", "category", "description");
        Assert.Throws<ArgumentNullException>("name", item.Invoke);
    }

    [Fact]
    public void Invoke_MemberWithParameters_ThrowsTargetParameterCountException()
    {
        SubDesignerActionList list = new();
        DesignerActionMethodItem item = new(list, nameof(SubDesignerActionList.MethodWithParameters), "displayName", "category", "description");
        Assert.Throws<TargetParameterCountException>(item.Invoke);
    }

    private class SubDesignerActionList : DesignerActionList
    {
        public SubDesignerActionList() : base(null)
        {
        }

        public string CalledMethod { get; set; }

        public void PublicMethod()
        {
            Assert.Null(CalledMethod);
            CalledMethod = nameof(PublicMethod);
        }

        private void PrivateMethod()
        {
            Assert.Null(CalledMethod);
            CalledMethod = nameof(PrivateMethod);
        }

        public static void StaticMethod()
        {
            throw new InvalidOperationException();
        }

        public void MethodWithParameters(object o)
        {
            throw new InvalidOperationException();
        }
    }
}
