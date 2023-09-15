﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;

namespace System.Windows.Forms.Design.Tests;

public class EmbeddedResourceTests
{
    // Get System.Windows.Forms.Design assembly to verify that it contains all the icons that the code uses.
    private readonly Assembly assembly = Assembly.GetAssembly(typeof(AnchorEditor));

    private static string s_expectedIconNames = """
            System.ComponentModel.Design.Arrow
            System.ComponentModel.Design.ComponentEditorPage
            System.ComponentModel.Design.DateTimeFormat
            System.ComponentModel.Design.DefaultComponent
            System.ComponentModel.Design.NumericFormat
            System.ComponentModel.Design.OrderImages
            System.ComponentModel.Design.SortDown
            System.ComponentModel.Design.SortUp
            System.ComponentModel.Design.UncheckedBox
            System.Windows.Forms.Design.256_1
            System.Windows.Forms.Design.256_2
            System.Windows.Forms.Design.AddNewDataSource
            System.Windows.Forms.Design.AddNewDataSource.bmp
            System.Windows.Forms.Design.Behavior.bottomclose
            System.Windows.Forms.Design.Behavior.bottomopen
            System.Windows.Forms.Design.Behavior.Close_left
            System.Windows.Forms.Design.Behavior.DesignerShortcutBox
            System.Windows.Forms.Design.Behavior.leftClose
            System.Windows.Forms.Design.Behavior.leftOpen
            System.Windows.Forms.Design.Behavior.MoverGlyph
            System.Windows.Forms.Design.Behavior.Open_left
            System.Windows.Forms.Design.Behavior.rightclose
            System.Windows.Forms.Design.Behavior.rightopen
            System.Windows.Forms.Design.Behavior.topclose
            System.Windows.Forms.Design.Behavior.topopen
            System.Windows.Forms.Design.BindingFormattingDialog.Arrow
            System.Windows.Forms.Design.BindingFormattingDialog.Bound
            System.Windows.Forms.Design.BindingFormattingDialog.Unbound
            System.Windows.Forms.Design.BoundProperty
            System.Windows.Forms.Design.ChildFolder
            System.Windows.Forms.Design.classic
            System.Windows.Forms.Design.colorful1
            System.Windows.Forms.Design.DataGridViewColumnsDialog.delete
            System.Windows.Forms.Design.DataGridViewColumnsDialog.moveDown
            System.Windows.Forms.Design.DataGridViewColumnsDialog.moveUp
            System.Windows.Forms.Design.DataGridViewColumnsDialog.selectedColumns
            System.Windows.Forms.Design.DataGridViewColumnsDialog.selectedColumns.bmp
            System.Windows.Forms.Design.DataPickerImages
            System.Windows.Forms.Design.DataPickerImages.bmp
            System.Windows.Forms.Design.default
            System.Windows.Forms.Design.Delete
            System.Windows.Forms.Design.DummyNodeImage
            System.Windows.Forms.Design.Folder
            System.Windows.Forms.Design.ImageEditor
            System.Windows.Forms.Design.InheritedGlyph
            System.Windows.Forms.Design.InsertableObject
            System.Windows.Forms.Design.Professional1
            System.Windows.Forms.Design.Professional2
            System.Windows.Forms.Design.ToolStripTemplateNode
            System.Windows.Forms.Design.UserControlToolboxItem
            """;

    private static string s_expectedBitmapNames = """
            System.Windows.Forms.Design.Behavior.BottomClose
            System.Windows.Forms.Design.Behavior.BottomOpen
            System.Windows.Forms.Design.Behavior.LeftClose
            System.Windows.Forms.Design.Behavior.LeftOpen
            System.Windows.Forms.Design.Behavior.RightClose
            System.Windows.Forms.Design.Behavior.RightOpen
            System.Windows.Forms.Design.Behavior.ToolStripContainer_BottomToolStripPanel
            System.Windows.Forms.Design.Behavior.ToolStripContainer_LeftToolStripPanel
            System.Windows.Forms.Design.Behavior.ToolStripContainer_RightToolStripPanel
            System.Windows.Forms.Design.Behavior.ToolStripContainer_TopToolStripPanel
            System.Windows.Forms.Design.Behavior.TopClose
            System.Windows.Forms.Design.Behavior.TopOpen
            """;

    public static TheoryData ExpectedIconNames()
        => s_expectedIconNames.Split(Environment.NewLine).Where(item => !item.EndsWith(".bmp")).ToTheoryData();

    public static TheoryData ExpectedBitmapNames()
        => s_expectedBitmapNames.Split(Environment.NewLine).ToTheoryData();

    [Theory]
    [MemberData(nameof(ExpectedIconNames))]
    public void EmbeddedResource_ResourcesExist_Icon(string resourceName)
    {
        using Stream stream = assembly.GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using Icon icon = new(stream);
        Assert.NotNull(icon);
    }

    [Theory]
    [MemberData(nameof(ExpectedBitmapNames))]
    public void EmbeddedResource_ResourcesExist_Bitmap(string resourceName)
    {
        using Stream stream = assembly.GetManifestResourceStream(resourceName);
        Assert.NotNull(stream);

        using Bitmap bitmap = new(stream);
        Assert.NotNull(bitmap);
    }

    private const string expectedResourceNames = """
            System.ComponentModel.Design.BinaryEditor.resources
            System.ComponentModel.Design.CollectionEditor.resources
            System.SR.resources
            System.Windows.Forms.Design.BorderSidesEditor.resources
            System.Windows.Forms.Design.colordlg.data
            System.Windows.Forms.Design.DataGridViewAddColumnDialog.resources
            System.Windows.Forms.Design.DataGridViewColumnCollectionDialog.resources
            System.Windows.Forms.Design.FormatControl.resources
            System.Windows.Forms.Design.LinkAreaEditor.resources
            System.Windows.Forms.Design.MaskDesignerDialog.resources
            System.Windows.Forms.Design.ShortcutKeysEditor.resources
            System.Windows.Forms.Design.StringCollectionEditor.resources
            """;

    [Fact]
    public void EmbeddedResource_VerifyList()
    {
        string[] actual = assembly.GetManifestResourceNames();
        Array.Sort(actual, StringComparer.Ordinal);

        string resourceNames = s_expectedIconNames + "\r\n" + s_expectedBitmapNames;
        string[] expected = $"{resourceNames}{Environment.NewLine}{expectedResourceNames}".Split(Environment.NewLine);
        Array.Sort(expected, StringComparer.Ordinal);

        AssertExtensions.Equal(expected, actual);
    }
}
