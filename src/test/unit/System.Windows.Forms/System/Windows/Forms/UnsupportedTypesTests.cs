// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using UnsupportedTypes;

namespace System.Windows.Forms.Tests;

// Test for https://github.com/dotnet/winforms/issues/3783
public class UnsupportedTypesTests
{
#pragma warning disable WFDEV006, CS0618 // Type or member is obsolete

    public static TheoryData<Action> UnsupportedControlsConstructors =>
    [
        new(() => new ContextMenu()),
        new(() => new ContextMenu(menuItems: null!)),
        new(() => new DataGrid()),
        new(() => new DataGridBoolColumn()),
        new(() => new DataGridBoolColumn(prop: null!)),
        new(() => new DataGridBoolColumn(prop: null!, isDefault: true)),
        new(() => new DataGridCell(r:1, c: 2)),
        new(() => new DataGridPreferredColumnWidthTypeConverter()),
        new(() => new DataGridTableStyle()),
        new(() => new DataGridTableStyle(isDefaultTableStyle: true)),
        new(() => new DataGridTableStyle(listManager: null!)),
        new(() => new DataGridTextBox()),
        new(() => new DataGridTextBoxColumn()),
        new(() => new DataGridTextBoxColumn(prop: null!)),
        new(() => new DataGridTextBoxColumn(prop: null!, format: "format")),
        new(() => new DataGridTextBoxColumn(prop: null!, format: "format", isDefault: true)),
        new(() => new DataGridTextBoxColumn(prop : null!, isDefault: false)),
        new(() => new TestDataGridColumnStyle()),
        new(TestDataGridColumnStyle.CreateCompModSwitches),
        new(() => new TestDataGridColumnStyle(prop: null!)),
        new(TestDataGridColumnStyle.Create_DataGridColumnHeaderAccessibleObject),
        new(TestDataGridColumnStyle.Create_DataGridColumnHeaderAccessibleObject1),
        new(() => GridTablesFactory.CreateGridTables(gridTable: null!, dataSource: null!, dataMember: "data member", bindingManager: null!)),
        new(() => new Menu.MenuItemCollection(owner: null!)),
        new(() => new MainMenu()),
        new(() => new MainMenu(container: null!)),
        new(() => new MainMenu(items: null!)),
        new(() => new MenuItem()),
        new(() => new MenuItem(text: "text")),
        new(() => new MenuItem(text: "text", onClick: null!)),
        new(() => new MenuItem(text: "text", onClick: null!, Shortcut.Alt0)),
        new(() => new MenuItem(text: "text", items: null!)),
        new(() => new MenuItem(MenuMerge.Add, mergeOrder: 1, Shortcut.Alt0, text: "text", onClick: null!, onPopup: null!, onSelect: null, items: null!)),
        new(() => new StatusBar()),
        new(() => new StatusBar.StatusBarPanelCollection(owner: null!)),
        new(() => new StatusBarPanel()),
        new(() => new StatusBarPanelClickEventArgs(statusBarPanel: null!, MouseButtons.Left, clicks: 1, x: 1, y: 1)),
        new(() => new ToolBar()),
        new(() => new ToolBarButton()),
        new(() => new ToolBarButton(text: "text")),
        new(() => new ToolBar.ToolBarButtonCollection(owner: null!)),
        new(() => new ToolBarButtonClickEventArgs(button: null!))
    ];

    [Theory]
    [MemberData(nameof(UnsupportedControlsConstructors))]
    public void UnsupportedControl_Constructor_Throws(Action action) =>
        action.Should().Throw<PlatformNotSupportedException>();

    [Fact]
    public void CompModSwitches_Throw() =>
        ((Func<bool>)(() => TestDataGridColumnStyle.Call_DGEditColumnEditing())).Should().Throw<PlatformNotSupportedException>();

    [Fact]
    public void DataGridTableStyle_static_IsDefault() =>
        DataGridTableStyle.DefaultTableStyle.Should().BeNull();

    [Fact]
    public void DataGrid_HitTestInfo_Nowhere_static_IsDefault() =>
        DataGrid.HitTestInfo.Nowhere.Should().BeNull();

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

    [Fact]
    public void CreateMenus_Throws()
    {
        using Button button = new();
        using CreateFrameworkTypes createFrameworkTypes = new();
        ((Action)(() => createFrameworkTypes.CreateMenus(button))).Should().Throw<PlatformNotSupportedException>();
    }

    [Fact]
    public void CreateMainMenu_Throws()
    {
        using Form form = new();
        ((Action)(() => form.Menu = CreateFrameworkTypes.CreateMainMenu())).Should().Throw<PlatformNotSupportedException>();
    }

    [Fact]
    public void CreateDataGrid_Throws()
    {
        using Form form = new();
        using CreateFrameworkTypes createFrameworkTypes = new();
        ((Action)(() => createFrameworkTypes.CreateDataGrid(form))).Should().Throw<PlatformNotSupportedException>();
    }

    [Fact]
    public void AddCustomDataTableStyle_Throws()
    {
        using Form form = new();
        using CreateFrameworkTypes createFrameworkTypes = new();
        ((Action)(() => createFrameworkTypes.AddCustomDataTableStyle(form))).Should().Throw<PlatformNotSupportedException>();
    }

    [Fact]
    public void AccessHitTestInfo_DoesNotThrowMissingField() =>
        ((Action)(() => CreateFrameworkTypes.DataGrid_MouseUp(null, null))).Should().Throw<NullReferenceException>();

    [Fact]
    public void CreateToolBar_Throws()
    {
        using Form form = new();
        using CreateFrameworkTypes createFrameworkTypes = new();
        ((Action)(() => createFrameworkTypes.CreateToolBar(form))).Should().Throw<PlatformNotSupportedException>();
    }

    [Fact]
    public void CreateStatusBar_Throws()
    {
        using Form form = new();
        using CreateFrameworkTypes createFrameworkTypes = new();
        ((Action)(() => createFrameworkTypes.CreateStatusBar(form))).Should().Throw<PlatformNotSupportedException>();
    }

    [Fact]
    public void InteropWithUnsupportedEnums()
    {
        CreateFrameworkTypes.InteropWithUnsupportedEnums(
            DataGrid.HitTestType.Caption,
            DataGridLineStyle.Solid,
            DataGridParentRowsLabelStyle.TableName,
            MenuMerge.Remove,
            StatusBarPanelAutoSize.Spring,
            StatusBarPanelBorderStyle.Sunken,
            StatusBarPanelStyle.OwnerDraw,
            ToolBarAppearance.Flat,
            ToolBarButtonStyle.DropDownButton,
            ToolBarTextAlign.Underneath).Should().BeTrue();
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
        public static void Create_DataGridColumnHeaderAccessibleObject() => _ = new DataGridColumnHeaderAccessibleObject();
        public static void Create_DataGridColumnHeaderAccessibleObject1() => _ = new DataGridColumnHeaderAccessibleObject(owner: null!);
        public static void CreateCompModSwitches() => _ = new CompModSwitches();
        public static bool Call_DGEditColumnEditing() => CompModSwitches.DGEditColumnEditing.TraceError;
        protected internal override void Abort(int rowNum) => throw new NotImplementedException();
        protected internal override bool Commit(CurrencyManager dataSource, int rowNum) => throw new NotImplementedException();
        protected internal override void Edit(CurrencyManager source, int rowNum, Rectangle bounds, bool readOnly, string displayText, bool cellIsVisible) => throw new NotImplementedException();
        protected internal override int GetMinimumHeight() => throw new NotImplementedException();
        protected internal override int GetPreferredHeight(Graphics g, object value) => throw new NotImplementedException();
        protected internal override Size GetPreferredSize(Graphics g, object value) => throw new NotImplementedException();
        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight) => throw new NotImplementedException();
        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum) => throw new NotImplementedException();
    }
}
