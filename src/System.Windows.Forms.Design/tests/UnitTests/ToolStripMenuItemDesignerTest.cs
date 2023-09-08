// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms.Design.Tests;

public class ToolStripMenuItemDesignerTest
{
    [WinFormsFact]
    public void ToolStripMenuItemDesignerTest_AssociatedComponentsTest()
    {
        TestToolStripMenuItemDesigner toolStripMenuItemDesigner = new();
        ToolStripMenuItem toolStripDropDown = new();
        toolStripMenuItemDesigner.Initialize(toolStripDropDown);

        Assert.Empty(toolStripMenuItemDesigner.AssociatedComponents);

        toolStripDropDown.DropDownItems.Add("123");

        Assert.Equal(1, toolStripMenuItemDesigner.AssociatedComponents.Count);
        Assert.Null(toolStripMenuItemDesigner.Editor);
        Assert.Null(toolStripMenuItemDesigner.GetParentComponentProperty());
    }

    [Fact]
    public void CommitTest()
    {
        TestToolStripMenuItemDesigner toolStripMenuItemDesigner = new();
        ToolStripMenuItem toolStripMenuItem = new();
        toolStripMenuItemDesigner.Initialize(toolStripMenuItem);

        toolStripMenuItemDesigner.Commit();
    }

    [Fact]
    public void UnHookEvents()
    {
        TestToolStripMenuItemDesigner toolStripMenuItemDesigner = new();
        ToolStripMenuItem toolStripDropDown = new();
        toolStripMenuItemDesigner.Initialize(toolStripDropDown);

        toolStripMenuItemDesigner.UnHookEvents();
    }

    [Theory]
    [BoolData]
    public void DisposeTest(bool disposing)
    {
        TestToolStripMenuItemDesigner toolStripMenuItemDesigner = new();
        ToolStripMenuItem toolStripMenuItem = new();
        toolStripMenuItemDesigner.Initialize(toolStripMenuItem);

        toolStripMenuItemDesigner.DisposeMethod(disposing);
    }

    [Theory]
    [BoolData]
    public void EditTemplateNodeTest(bool clicked)
    {
        TestToolStripMenuItemDesigner toolStripMenuItemDesigner = new();
        ToolStripMenuItem toolStripMenuItem = new();
        toolStripMenuItemDesigner.Initialize(toolStripMenuItem);

        toolStripMenuItemDesigner.EditTemplateNode(clicked);
    }

    [Fact]
    public void GetOwnerForActionListTest()
    {
        TestToolStripMenuItemDesigner toolStripMenuItemDesigner = new();
        ToolStripMenuItem toolStripMenuItem = new();
        toolStripMenuItemDesigner.Initialize(toolStripMenuItem);

        Assert.NotNull(toolStripMenuItemDesigner.GetOwnerForActionListMethod());
    }

    [Fact]
    public void HookEventsTest()
    {
        TestToolStripMenuItemDesigner toolStripMenuItemDesigner = new();
        ToolStripMenuItem toolStripMenuItem = new();
        toolStripMenuItemDesigner.Initialize(toolStripMenuItem);

        toolStripMenuItemDesigner.HookEvents();
    }

    private class TestToolStripMenuItemDesigner : ToolStripMenuItemDesigner
    {
        internal IComponent GetParentComponentProperty()
        {
            return ParentComponent;
        }

        internal void DisposeMethod(bool disposing)
        {
            Dispose(disposing);
        }

        internal Component GetOwnerForActionListMethod()
        {
            return GetOwnerForActionList();
        }

        internal void PreFilterPropertiesMethod(IDictionary properties)
        {
            PreFilterProperties(properties);
        }
    }

    private class SubToolStripItem : ToolStripItem
    {
        public SubToolStripItem() : base()
        {
        }
    }

    private class SubToolStripDropDownItem : ToolStripDropDownItem
    {
        public SubToolStripDropDownItem() : base()
        {
        }
    }
}
