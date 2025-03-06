// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace UnsupportedTypes;

// For details see
// https://learn.microsoft.com/dotnet/desktop/winforms/controls/datagrid-control-overview-windows-forms
public sealed class CreateFrameworkTypes : IDisposable
{
    private ContextMenu _contextMenu;
    private MenuItem _menuItem1;
    private MenuItem _menuItem2;
    private DataSet _dataSet;
    private int _count;

    public void Dispose()
    {
        _contextMenu?.Dispose();
        _menuItem1?.Dispose();
        _menuItem2?.Dispose();
        Console.WriteLine(_count);
    }

    // This test creates ContextMenu, Menu, Menu.MenuItemCollection, MenuItem, MenuMerge enum.
    public void CreateMenus(Button button)
    {
        _contextMenu = new();
        _contextMenu.Popup += ContextMenu_Popup;
        _contextMenu.MenuItems.Clear();
        _menuItem1 = new("Item1", OnClick);
        _contextMenu.MenuItems.Add(_menuItem1);
        _menuItem2 = new(MenuMerge.Add, mergeOrder: 0, Shortcut.Alt0, "Item2", onClick: null, OnPopup, OnSelect, [new("Item2-1"), new("Item2-2")]);
        _menuItem2.MergeMenu(_menuItem1.CloneMenu());
        _contextMenu.MenuItems.Add(_menuItem2);

        _contextMenu.Show(button, button.Location);

        void OnClick(object sender, EventArgs e) => MessageBox.Show("Clicked Item1", "CreateContextMenu");
        void OnSelect(object sender, EventArgs e) => MessageBox.Show("Selected Item2", "CreateContextMenu");
        void OnPopup(object sender, EventArgs e) => MessageBox.Show("Item2 popped up", "CreateContextMenu");
        void ContextMenu_Popup(object sender, EventArgs e) => _contextMenu.MenuItems.Add(new MenuItem("Item3"));
    }

    // This test creates MainMenu, MenuItem.
    public static MainMenu CreateMainMenu()
    {
        MainMenu mainMenu = new();
        MenuItem fileMenuItem = new("MainMenuItem", (s, e) => MessageBox.Show("New menu item clicked", "MainMenu"));
        mainMenu.MenuItems.Add(fileMenuItem);

        return mainMenu;
    }

    // This method creates DataGrid, DataGridCell, DataGridLineStyle, DataGridParentRowsLabelStyle, DataGrid.HitTestInfo,
    // DataGrid.HitTestType, DataGridTableStyle, DataGridColumnStyle, DataGridBoolColumn, DataGridTextBoxColumn,
    // GridColumnStylesCollection, GridTableStylesCollection
    public void CreateDataGrid(Form form)
    {
        DataGrid dataGrid = new()
        {
            DataMember = "",
            HeaderForeColor = SystemColors.ControlText,
            Location = new Point(200, 60),
            Name = "dataGrid",
            Size = new Size(300, 300),
            TabIndex = 2
        };

        form.Controls.Add(dataGrid);

        _dataSet = CreateDataSet();

        // Bind the DataGrid to the DataSet. The dataMember
        // specifies that the Customers table should be displayed.
        dataGrid.SetDataBinding(_dataSet, "Customers");

        AddCustomDataTableStyle(form, dataGrid, _dataSet);

        if (dataGrid.CurrentCell is DataGridCell dataGridCell)
        {
            dataGrid.CurrentCellChanged += (sender, e) => _count++;
        }

        if (dataGrid.GridLineStyle == DataGridLineStyle.None)
        {
            dataGrid.GridLineStyle = DataGridLineStyle.Solid;
        }

        if (dataGrid.ParentRowsLabelStyle == DataGridParentRowsLabelStyle.Both)
        {
            dataGrid.ParentRowsLabelStyle = DataGridParentRowsLabelStyle.TableName;
        }

        DataGridTextBox textBox = new();
        textBox.SetDataGrid(dataGrid);

        dataGrid.MouseUp += DataGrid_MouseUp;
    }

    // Creates ToolBar, ToolBarButton, ToolBar.ButtonCollection, ToolBarButtonClickEventArgs, ToolBarButtonClickEventHandler,
    // ToolBarButtonCollection, ToolBarButtonStyle, ToolBarTextAlign, ToolBarAppearance.
    public void CreateToolBar(Form form)
    {
        TestToolBar toolBar = new()
        {
            Appearance = ToolBarAppearance.Flat,
            Buttons =
            {
                new ToolBarButton { Text = "toolBarButton1" },
                new ToolBarButton { Text = "toolBarButton2", Style = ToolBarButtonStyle.DropDownButton}
            },
            DropDownArrows = true,
            Location = new Point(0, 0),
            Name = "toolBar1",
            ShowToolTips = true,
            Size = new Size(100, 42),
            TabIndex = 0,
            TextAlign = ToolBarTextAlign.Underneath
        };

        toolBar.ButtonClick += (sender, e) => _count++;

        form.Controls.Add(toolBar);
    }

    // Creates StatusBar, StatusBarPanel, StatusBarPanelAutoSize, StatusBarPanelBorderStyle, StatusBarPanelCollection,
    // StatusBarPanelClickEventHandler, StatusBarPanelStyle, StatusBarDrawItemEventArgs, StatusBarPanelClickEventArgs.
    public void CreateStatusBar(Form form)
    {
        TestStatusBar statusBar = new()
        {
            Location = new Point(0, 0),
            Name = "statusBar",
            Size = new Size(100, 22),
            TabIndex = 0,
            Text = "Status Bar"
        };

        StatusBarPanel statusBarPanel = new()
        {
            Alignment = HorizontalAlignment.Center,
            AutoSize = StatusBarPanelAutoSize.Contents,
            BorderStyle = StatusBarPanelBorderStyle.Sunken,
            MinWidth = 100,
            Name = "statusBarPanel",
            Style = StatusBarPanelStyle.OwnerDraw,
            Text = "statusBarPanel",
            Width = 100
        };

        statusBar.PanelClick += (sender, e) => _count++;
        statusBar.DrawItem += (sender, e) => _count++;

        statusBar.Panels.Add(statusBarPanel);

        form.Controls.Add(statusBar);
    }

    /// <summary>
    ///  Create a DataSet with two tables and one relation.
    /// </summary>
    private static DataSet CreateDataSet()
    {
        // Create a DataSet.
        DataSet dataSet = new("myDataSet");

        // Create two DataTables.
        DataTable customerDataTable = new("Customers");
        DataTable orderDataTable = new("Orders");

        // Create two columns, and add them to the first table.
        DataColumn customerIdDataColumn = new("CustID", typeof(int));
        DataColumn customerNameDataColumn = new("CustName");
        DataColumn currentDataColumn = new("Current", typeof(bool));
        customerDataTable.Columns.Add(customerIdDataColumn);
        customerDataTable.Columns.Add(customerNameDataColumn);
        customerDataTable.Columns.Add(currentDataColumn);

        // Create three columns, and add them to the second table.
        DataColumn idDataColumn = new("CustID", typeof(int));
        DataColumn orderDateDataColumn = new("orderDate", typeof(DateTime));
        DataColumn orderAmountDataColumn = new("OrderAmount", typeof(decimal));
        orderDataTable.Columns.Add(orderAmountDataColumn);
        orderDataTable.Columns.Add(idDataColumn);
        orderDataTable.Columns.Add(orderDateDataColumn);

        // Add the tables to the DataSet.
        dataSet.Tables.Add(customerDataTable);
        dataSet.Tables.Add(orderDataTable);

        // Create a DataRelation, and add it to the DataSet.
        DataRelation dataRelation = new DataRelation("customerToOrders", customerIdDataColumn, idDataColumn);
        dataSet.Relations.Add(dataRelation);

        // Populates the tables. For each customer and order, creates two DataRow variables.
        DataRow newRow1;
        DataRow newRow2;

        // Create three customers in the Customers Table.
        for (int i = 1; i < 4; i++)
        {
            newRow1 = customerDataTable.NewRow();
            newRow1["CustID"] = i;
            // Add the row to the Customers table.
            customerDataTable.Rows.Add(newRow1);
        }

        // Give each customer a distinct name.
        customerDataTable.Rows[0]["CustName"] = "Customer1";
        customerDataTable.Rows[1]["CustName"] = "Customer2";
        customerDataTable.Rows[2]["CustName"] = "Customer3";

        // Give the Current column a value.
        customerDataTable.Rows[0]["Current"] = true;
        customerDataTable.Rows[1]["Current"] = true;
        customerDataTable.Rows[2]["Current"] = false;

        // For each customer, create five rows in the Orders table.
        for (int i = 1; i < 4; i++)
        {
            for (int j = 1; j < 6; j++)
            {
                newRow2 = orderDataTable.NewRow();
                newRow2["CustID"] = i;
                newRow2["orderDate"] = new DateTime(2001, i, j * 2);
                newRow2["OrderAmount"] = (i * 10) + (j * .1);
                // Add the row to the Orders table.
                orderDataTable.Rows.Add(newRow2);
            }
        }

        return dataSet;
    }

    // This method creates DataGrid, DataGridTableStyle, DataGridColumnStyle, DataGridBoolColumn, DataGridTextBoxColumn,
    // GridColumnStylesCollection, GridTableStylesCollection
    public void AddCustomDataTableStyle(Form form, DataGrid dataGrid = null, DataSet dataSet = null)
    {
        dataGrid ??= new DataGrid();

        dataSet ??= CreateDataSet();

        DataGridTableStyle customerDGTableStyle = new()
        {
            MappingName = "Customers",
            // Set other properties.
            AlternatingBackColor = Color.LightGray
        };

        customerDGTableStyle.GridColumnStyles.CollectionChanged += (sender, e) => _count++;

        // Add a GridColumnStyle and set its MappingName
        // to the name of a DataColumn in the DataTable.
        // Set the HeaderText and Width properties.

        DataGridColumnStyle currentDGTableStyle = new DataGridBoolColumn
        {
            MappingName = "Current",
            HeaderText = "IsCurrent Customer",
            Width = 150
        };
        customerDGTableStyle.GridColumnStyles.Add(currentDGTableStyle);

        // Add a second column style.
        DataGridColumnStyle customerNameDGTableStyle = new DataGridTextBoxColumn
        {
            MappingName = "customerName",
            HeaderText = "Customer Name",
            Width = 250
        };
        customerDGTableStyle.GridColumnStyles.Add(customerNameDGTableStyle);

        // Create the second table style with columns.
        DataGridTableStyle OrderDGTableStyle = new()
        {
            MappingName = "Orders",

            // Set other properties.
            AlternatingBackColor = Color.LightBlue
        };

        // Create new ColumnStyle objects
        DataGridColumnStyle orderDateDGColumnStyle = new DataGridTextBoxColumn
        {
            MappingName = "OrderDate",
            HeaderText = "Order Date",
            Width = 100
        };
        OrderDGTableStyle.GridColumnStyles.Add(orderDateDGColumnStyle);

        // Use a PropertyDescriptor to create a formatted
        // column. First get the PropertyDescriptorCollection
        // for the data source and data member.
        PropertyDescriptorCollection propertyDescriptorCollection =
            form.BindingContext[dataSet, "Customers.customerToOrders"].GetItemProperties();

        // Create a formatted column using a PropertyDescriptor.
        // The formatting character "c" specifies a currency format.
        DataGridColumnStyle csOrderAmount = new DataGridTextBoxColumn(propertyDescriptorCollection["OrderAmount"], "c", isDefault: true)
        {
            MappingName = "OrderAmount",
            HeaderText = "Total",
            Width = 100
        };
        OrderDGTableStyle.GridColumnStyles.Add(csOrderAmount);

        // Add the DataGridTableStyle instances to the GridTableStylesCollection.
        dataGrid.TableStyles.Add(customerDGTableStyle);
        dataGrid.TableStyles.Add(OrderDGTableStyle);
    }

    public static void DataGrid_MouseUp(object sender, MouseEventArgs e)
    {
        DataGrid.HitTestInfo hitInfo = ((DataGrid)sender).HitTest(e.X, e.Y);
        if (hitInfo == DataGrid.HitTestInfo.Nowhere)
        {
            return;
        }

        if (hitInfo.Type == DataGrid.HitTestType.Cell)
        {
            MessageBox.Show($"Cell [{hitInfo.Column}, {hitInfo.Row}]  clicked", "DataGrid");
        }
    }

    public static bool InteropWithUnsupportedEnums(
        DataGrid.HitTestType hitTestType,
        DataGridLineStyle dataGridLineStyle,
        DataGridParentRowsLabelStyle dataGridParentRowsLabelStyle,
        MenuMerge menuMerge,
        StatusBarPanelAutoSize statusBarPanelAutoSize,
        StatusBarPanelBorderStyle statusBarPanelBorderStyle,
        StatusBarPanelStyle statusBarPanelStyle,
        ToolBarAppearance toolBarAppearance,
        ToolBarButtonStyle toolBarButtonStyle,
        ToolBarTextAlign toolBarTextAlign)
    {
        if (hitTestType != DataGrid.HitTestType.Caption)
        {
            return false;
        }

        if (dataGridLineStyle != DataGridLineStyle.Solid)
        {
            return false;
        }

        if (dataGridParentRowsLabelStyle != DataGridParentRowsLabelStyle.TableName)
        {
            return false;
        }

        if (menuMerge != MenuMerge.Remove)
        {
            return false;
        }

        if (statusBarPanelAutoSize != StatusBarPanelAutoSize.Spring)
        {
            return false;
        }

        if (statusBarPanelBorderStyle != StatusBarPanelBorderStyle.Sunken)
        {
            return false;
        }

        if (statusBarPanelStyle != StatusBarPanelStyle.OwnerDraw)
        {
            return false;
        }

        if (toolBarAppearance != ToolBarAppearance.Flat)
        {
            return false;
        }

        if (toolBarButtonStyle != ToolBarButtonStyle.DropDownButton)
        {
            return false;
        }

        if (toolBarTextAlign != ToolBarTextAlign.Underneath)
        {
            return false;
        }

        return true;
    }

    private class TestToolBar : ToolBar
    {
        public void MyClick()
        {
            if (Buttons[0] is ToolBarButton button)
            {
                OnButtonClick(new ToolBarButtonClickEventArgs(button));
            }
        }
    }

    private class TestStatusBar : StatusBar
    {
        public void MyClick()
        {
            if (Panels[0] is StatusBarPanel panel)
            {
                OnPanelClick(new StatusBarPanelClickEventArgs(panel, MouseButtons.Middle, clicks: 1, x: 2, y: 3));
            }
        }

        public void MyDrawItem()
        {
            OnDrawItem(new StatusBarDrawItemEventArgs(CreateGraphics(), font: null!, r: new(1, 1, 1, 1), itemId: 1, DrawItemState.Default, Panels[0]));
        }
    }
}
