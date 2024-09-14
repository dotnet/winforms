// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.TestUtilities;

namespace System.Windows.Forms.Tests;

public class OwnerDrawPropertyBagTests
{
    [WinFormsFact]
    public void OwnerDrawPropertyBag_Ctor_Default()
    {
        using SubTreeView treeView = new();
        OwnerDrawPropertyBag bag = treeView.GetItemRenderStyles(null, 0);
        Assert.Equal(Color.Empty, bag.BackColor);
        Assert.Null(bag.Font);
        Assert.Equal(Color.Empty, bag.ForeColor);
        Assert.True(bag.IsEmpty());
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
    public void OwnerDrawPropertyBag_BackColor_Set_GetReturnsExpected(Color value)
    {
        using SubTreeView treeView = new();
        OwnerDrawPropertyBag bag = treeView.GetItemRenderStyles(null, 0);

        bag.BackColor = value;
        Assert.Equal(value, bag.BackColor);
        Assert.Equal(value.IsEmpty, bag.IsEmpty());

        // Set same.
        bag.BackColor = value;
        Assert.Equal(value, bag.BackColor);
        Assert.Equal(value.IsEmpty, bag.IsEmpty());
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetFontTheoryData))]
    public void OwnerDrawPropertyBag_Font_Set_GetReturnsExpected(Font value)
    {
        using SubTreeView treeView = new();
        OwnerDrawPropertyBag bag = treeView.GetItemRenderStyles(null, 0);

        bag.Font = value;
        Assert.Same(value, bag.Font);
        Assert.Equal(value is null, bag.IsEmpty());

        // Set same.
        bag.Font = value;
        Assert.Same(value, bag.Font);
        Assert.Equal(value is null, bag.IsEmpty());
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
    public void OwnerDrawPropertyBag_ForeColor_Set_GetReturnsExpected(Color value)
    {
        using SubTreeView treeView = new();
        OwnerDrawPropertyBag bag = treeView.GetItemRenderStyles(null, 0);

        bag.ForeColor = value;
        Assert.Equal(value, bag.ForeColor);
        Assert.Equal(value.IsEmpty, bag.IsEmpty());

        // Set same.
        bag.ForeColor = value;
        Assert.Equal(value, bag.ForeColor);
        Assert.Equal(value.IsEmpty, bag.IsEmpty());
    }

    [WinFormsFact]
    public void OwnerDrawPropertyBag_Copy_CustomValue_ReturnsClone()
    {
        using SubTreeView treeView = new();
        OwnerDrawPropertyBag value = treeView.GetItemRenderStyles(null, 0);
        value.BackColor = Color.Blue;
        value.Font = SystemFonts.MenuFont;
        value.ForeColor = Color.Red;

        OwnerDrawPropertyBag bag = OwnerDrawPropertyBag.Copy(value);
        Assert.NotSame(value, bag);
        Assert.Equal(Color.Blue, bag.BackColor);
        Assert.Equal(SystemFonts.MenuFont.Name, bag.Font.Name);
        Assert.Equal(Color.Red, bag.ForeColor);
        Assert.False(bag.IsEmpty());
    }

    [WinFormsFact]
    public void OwnerDrawPropertyBag_Copy_NullValue_ReturnsDefault()
    {
        using SubTreeView treeView = new();
        OwnerDrawPropertyBag value = treeView.GetItemRenderStyles(null, 0);
        OwnerDrawPropertyBag bag = OwnerDrawPropertyBag.Copy(value);
        Assert.NotSame(value, bag);
        Assert.Equal(Color.Empty, bag.BackColor);
        Assert.Null(bag.Font);
        Assert.Equal(Color.Empty, bag.ForeColor);
        Assert.True(bag.IsEmpty());
    }

    [WinFormsFact]
    public void OwnerDrawPropertyBag_Serialize_Deserialize_Success()
    {
        using BinaryFormatterScope formatterScope = new(enable: true);
        using SubTreeView treeView = new();
        OwnerDrawPropertyBag original = treeView.GetItemRenderStyles(null, 0);
        original.BackColor = Color.Blue;
        original.Font = SystemFonts.MenuFont;
        original.ForeColor = Color.Red;

        using MemoryStream stream = new();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
        // cs/binary-formatter-without-binder
        BinaryFormatter formatter = new(); // CodeQL [SM04191] : This is a test. Safe use because the deserialization process is performed on trusted data and the types are controlled and validated.
        formatter.Serialize(stream, original);

        stream.Position = 0;
        // cs/dangerous-binary-deserialization
        OwnerDrawPropertyBag bag = Assert.IsType<OwnerDrawPropertyBag>(formatter.Deserialize(stream)); // CodeQL[SM03722] : Testing legacy feature. This is a safe use of BinaryFormatter because the data is trusted and the types are controlled and validated.
#pragma warning restore SYSLIB0011
        Assert.Equal(Color.Blue, bag.BackColor);
        Assert.Equal(SystemFonts.MenuFont.Name, bag.Font.Name);
        Assert.Equal(Color.Red, bag.ForeColor);
        Assert.False(bag.IsEmpty());
    }

    private class SubTreeView : TreeView
    {
        public new OwnerDrawPropertyBag GetItemRenderStyles(TreeNode node, int state) => base.GetItemRenderStyles(node, state);
    }
}
