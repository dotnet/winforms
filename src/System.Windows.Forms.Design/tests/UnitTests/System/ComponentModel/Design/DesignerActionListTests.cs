// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Moq;

namespace System.ComponentModel.Design.Tests;

public class DesignerActionListTests
{
    public static IEnumerable<object[]> Ctor_IComponent_TestDat()
    {
        yield return new object[] { null };
        yield return new object[] { new Component() };
    }

    [Theory]
    [MemberData(nameof(Ctor_IComponent_TestDat))]
    public void DesignerActionList_Ctor_IComponent(IComponent component)
    {
        DesignerActionList list = new(component);
        Assert.Equal(component, list.Component);
        Assert.False(list.AutoShow);
    }

    [Theory]
    [BoolData]
    public void DesignerActionList_AutoShow_Set_GetReturnsExpected(bool value)
    {
        DesignerActionList list = new(new Component())
        {
            AutoShow = value
        };
        Assert.Equal(value, list.AutoShow);

        // Set same.
        list.AutoShow = value;
        Assert.Equal(value, list.AutoShow);
    }

    public static IEnumerable<object[]> GetService_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new Component(), null };

        object o = new();
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(int)))
            .Returns(o);
        mockSite
            .Setup(s => s.Container)
            .Returns((IContainer)null);
        yield return new object[] { new Component { Site = mockSite.Object }, o };
    }

    [Theory]
    [MemberData(nameof(GetService_TestData))]
    public void DesignerActionList_GetService_Invoke_ReturnsExpected(Component component, object expected)
    {
        DesignerActionList list = new(component);
        Assert.Equal(expected, list.GetService(typeof(int)));
    }

    [Fact]
    public void DesignerActionList_GetSortedActionItems_CustomClass_ReturnsExpected()
    {
        SubDesignerActionList list = new();
        DesignerActionItemCollection items = list.GetSortedActionItems();
        Assert.Equal(8, items.Count);

        DesignerActionMethodItem item1 = Assert.IsType<DesignerActionMethodItem>(items[0]);
        Assert.Equal("AnnotatedMethod", item1.MemberName);
        Assert.Equal("DisplayName", item1.DisplayName);
        Assert.Equal("Description", item1.Description);
        Assert.Equal("Category", item1.Category);

        DesignerActionPropertyItem item2 = Assert.IsType<DesignerActionPropertyItem>(items[1]);
        Assert.Equal("AnnotatedProperty", item2.MemberName);
        Assert.Equal("DisplayName", item2.DisplayName);
        Assert.Equal("Description", item2.Description);
        Assert.Equal("Category", item2.Category);

        DesignerActionMethodItem item3 = Assert.IsType<DesignerActionMethodItem>(items[2]);
        Assert.Equal("EmptyAnnotatedMethod", item3.MemberName);
        Assert.Equal("EmptyAnnotatedMethod", item3.DisplayName);
        Assert.Empty(item3.Description);
        Assert.Empty(item3.Category);

        DesignerActionPropertyItem item4 = Assert.IsType<DesignerActionPropertyItem>(items[3]);
        Assert.Equal("EmptyAnnotatedProperty", item4.MemberName);
        Assert.Equal("EmptyAnnotatedProperty", item4.DisplayName);
        Assert.Empty(item4.Description);
        Assert.Empty(item4.Category);

        DesignerActionMethodItem item5 = Assert.IsType<DesignerActionMethodItem>(items[4]);
        Assert.Equal("NullAnnotatedMethod", item5.MemberName);
        Assert.Equal("NullAnnotatedMethod", item5.DisplayName);
        Assert.Null(item5.Description);
        Assert.Null(item5.Category);

        DesignerActionPropertyItem item6 = Assert.IsType<DesignerActionPropertyItem>(items[5]);
        Assert.Equal("NullAnnotatedProperty", item6.MemberName);
        Assert.Equal("NullAnnotatedProperty", item6.DisplayName);
        Assert.Null(item6.Description);
        Assert.Null(item6.Category);

        DesignerActionMethodItem item7 = Assert.IsType<DesignerActionMethodItem>(items[6]);
        Assert.Equal("PublicMethod", item7.MemberName);
        Assert.Equal("PublicMethod", item7.DisplayName);
        Assert.Empty(item7.Description);
        Assert.Empty(item7.Category);

        DesignerActionPropertyItem item8 = Assert.IsType<DesignerActionPropertyItem>(items[7]);
        Assert.Equal("PublicProperty", item8.MemberName);
        Assert.Equal("PublicProperty", item8.DisplayName);
        Assert.Empty(item8.Description);
        Assert.Empty(item8.Category);
    }

    [Fact]
    public void DesignerActionList_GetSortedActionItems_BaseClass_ReturnsEmpty()
    {
        DesignerActionList list = new(null);
        DesignerActionItemCollection items = list.GetSortedActionItems();
        Assert.Empty(items);
    }

    private class SubDesignerActionList : DesignerActionList
    {
        public SubDesignerActionList() : base(null)
        {
        }

        public void PublicMethod()
        {
        }

        [Description("Description")]
        [DisplayName("DisplayName")]
        [Category("Category")]
        public void AnnotatedMethod()
        {
        }

        [Description("")]
        [DisplayName("")]
        [Category("")]
        public void EmptyAnnotatedMethod()
        {
        }

        [Description(null)]
        [DisplayName(null)]
        [Category(null)]
        public void NullAnnotatedMethod()
        {
        }

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0060 // Remove unused parameter
        private void PrivateMethod()
        {
        }

        public void MethodWithParameters(object o)
        {
        }

        [SpecialName]
        public void MethodWithSpecialName()
        {
        }

        public static void StaticMethod()
        {
        }

        public int PublicProperty { get; set; }

        [Description("Description")]
        [DisplayName("DisplayName")]
        [Category("Category")]
        public int AnnotatedProperty { get; set; }

        [Description("")]
        [DisplayName("")]
        [Category("")]
        public int EmptyAnnotatedProperty { get; set; }

        [Description(null)]
        [DisplayName(null)]
        [Category(null)]
        public int NullAnnotatedProperty { get; set; }

        private int PrivateProperty { get; set; }

        public static int StaticProperty { get; set; }

#pragma warning restore IDE0051
#pragma warning restore IDE0060
    }
}
