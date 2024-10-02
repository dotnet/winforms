﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Tests;

// Test for https://github.com/dotnet/winforms/issues/3783
public class UnsupportedTypesTests
{
#pragma warning disable WFDEV005, WFDEV006, WFDEV007, WFDEV008, WFDEV009, WFDEV010, CS0618 // Type or member is obsolete

    public static TheoryData<Action> ObsoleteControlsConstructors => new()
    {
        () => new ContextMenu(),
        () => new ContextMenu(menuItems: null!),
        () => new DataGrid(),
        () => new DataGridBoolColumn(),
        () => new DataGridBoolColumn(prop: null!),
        () => new DataGridBoolColumn(prop: null!, isDefault: true),
        () => new DataGridCell(),
        () => new DataGridCell(r:1, c: 2),
        () => new DataGridPreferredColumnWidthTypeConverter(),
        () => new DataGridTableStyle(),
        () => new DataGridTableStyle(isDefaultTableStyle: true),
        () => new DataGridTableStyle(null!),
        () => new DataGridTextBox(),
        () => new DataGridTextBoxColumn(),
        () => new DataGridTextBoxColumn(prop: null!),
        () => new DataGridTextBoxColumn(prop: null!, format: "format"),
        () => new DataGridTextBoxColumn(prop: null!, format: "format", isDefault: true),
        () => new DataGridTextBoxColumn(prop : null!, isDefault: false),
        () => new GridColumnStylesCollection(),
        () => new TestDataGridColumnStyle(),
        () => new TestDataGridColumnStyle(prop: null!),
        TestDataGridColumnStyle.Create_DataGridColumnHeaderAccessibleObject,
        () => GridTablesFactory.CreateGridTables(gridTable: null!, dataSource: null!, dataMember: "data member", bindingManager: null!),
        () => new GridTableStylesCollection(),
        () => new Menu.MenuItemCollection(owner: null!),
        () => new MainMenu(),
        () => new MainMenu(container: null!),
        () => new MainMenu(items: null!),
        () => new MenuItem(),
        () => new MenuItem(text: "text"),
        () => new MenuItem(text: "text", onClick: null!),
        () => new MenuItem(text: "text", onClick: null!, Shortcut.Alt0),
        () => new MenuItem(text: "text", items: null!),
        () => new MenuItem(MenuMerge.Add, mergeOrder: 1, Shortcut.Alt0, text: "text", onClick: null!, onPopup: null!, onSelect: null, items: null!),
        () => new StatusBar(),
        () => new StatusBarPanel(),
        () => new StatusBarPanelClickEventArgs(statusBarPanel: null!, MouseButtons.Left, clicks: 1, x: 1, y: 1),
        () => new ToolBar(),
        () => new ToolBarButton(),
        () => new ToolBarButton(text: "text"),
        () => new ToolBar.ToolBarButtonCollection(owner: null!),
        () => new ToolBarButtonClickEventArgs(button: null!)
    };

    [Theory]
    [MemberData(nameof(ObsoleteControlsConstructors))]
    public void ObsoleteControl_Constructor_Throws(Action action) =>
        action.Should().Throw<PlatformNotSupportedException>();

    [Fact]
    public void CompModSwitches_Throw() =>
        ((Func<bool>)(() => TestDataGridColumnStyle.Call_DGEditColumnEditing())).Should().Throw<PlatformNotSupportedException>();

    [Fact]
    public void DataGridTableStyle_static_Throws() =>
        ((Func<object>)(() => DataGridTableStyle.s_defaultTableStyle)).Should().Throw<PlatformNotSupportedException>();

    [Fact]
    public void StatusBarDrawItemEventArgs_Constructor_Throws()
    {
        using Control control = new();
        using Graphics graphics = control.CreateGraphics();
        Rectangle rectangle = new(1, 2, 3, 4);

        ((Action)(() => new StatusBarDrawItemEventArgs(graphics, font: null!, rectangle, itemId: 0, DrawItemState.Checked, panel: null!)))
            .Should().Throw<PlatformNotSupportedException>();
        ((Action)(() => new StatusBarDrawItemEventArgs(graphics, font: null!, rectangle, itemId: 0, DrawItemState.Checked, panel: null!, foreColor: Color.Red, backColor: Color.Black)))
            .Should().Throw<PlatformNotSupportedException>();
    }

    [Fact]
    public void Control_ContextMenu()
    {
        using Control control = new();

        control.ContextMenu.Should().BeNull();
        control.ContextMenu = null;
    }

    [Fact]
    public void Control_ContextMenuChanged()
    {
        using ControlWithContextMenu control = new();
        int contextMenuChangedCount = 0;

        control.ContextMenuChanged += Control_ContextMenuChanged1;
        control.OnContextMenuChanged(EventArgs.Empty);
        contextMenuChangedCount.Should().Be(0);
        control.OnContextMenuChangedCount.Should().Be(1);
        control.ContextMenuChanged -= Control_ContextMenuChanged1;

        void Control_ContextMenuChanged1(object? sender, EventArgs e) => contextMenuChangedCount++;
    }

    [Fact]
    public void Form_Menu()
    {
        using Form form = new();

        form.Menu.Should().BeNull();
        form.Menu = null;
    }

    [Fact]
    public void Form_MergedMenu()
    {
        using Form form = new();
        form.MergedMenu.Should().BeNull();
    }

    internal class ControlWithContextMenu : Control
    {
        public int OnContextMenuChangedCount;

        public new void OnContextMenuChanged(EventArgs e)
        {
            OnContextMenuChangedCount++;
            base.OnContextMenuChanged(e);
        }
    }

    internal class TestDataGridColumnStyle : DataGridColumnStyle
    {
        public TestDataGridColumnStyle() : base() { }

        public TestDataGridColumnStyle(PropertyDescriptor prop) : base(prop) { }

        public static void Create_DataGridColumnHeaderAccessibleObject()
        {
            _ = new DataGridColumnHeaderAccessibleObject();
        }

        public static bool Call_DGEditColumnEditing() => CompModSwitches.DGEditColumnEditing.TraceError;

        protected internal override void Abort(int rowNum) => throw new NotImplementedException();
        protected internal override bool Commit(CurrencyManager dataSource, int rowNum) => throw new NotImplementedException();
        protected internal override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText, bool cellIsVisible) => throw new NotImplementedException();
        protected internal override int GetMinimumHeight() => throw new NotImplementedException();
        protected internal override int GetPreferredHeight(Graphics g, object value) => throw new NotImplementedException();
        protected internal override Size GetPreferredSize(Graphics g, object value) => throw new NotImplementedException();
        protected internal override void Paint(Graphics g1, Graphics g, Rectangle bounds, CurrencyManager source, int rowNum) => throw new NotImplementedException();
        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight) => throw new NotImplementedException();
    }
}
