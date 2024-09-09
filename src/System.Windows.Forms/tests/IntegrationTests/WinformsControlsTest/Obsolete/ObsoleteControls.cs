// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Data;
using System.Drawing;
#pragma warning disable WFDEV006, WFDEV007, WFDEV009, WFDEV011, WFDEV015, WFDEV017, WFDEV022, WFDEV025 // Type or member is obsolete
using static System.Windows.Forms.DataGrid;

namespace WinFormsControlsTest;

/// <summary>
/// This is added to test compile time compatibility only. 
/// </summary>
// Obsolete controls test for https://github.com/dotnet/winforms/issues/3783
public partial class ObsoleteControls : Form
{
    private bool _tablesAlreadyAdded;

    public ObsoleteControls()
    {
        try
        {
            InitializeComponent();
            CreateMainMenu();
            SetUp(); // This code is only valid when redirected to NET48.
        }
        catch (PlatformNotSupportedException ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void CreateMainMenu()
    {
        MainMenu mainMenu = new();
        MenuItem fileMenuItem = new MenuItem("File", new EventHandler(menuItem2_Click));
        mainMenu.MenuItems.Add(fileMenuItem);
    }

    private void menuItem1_Click(object sender, System.EventArgs e) => MessageBox.Show("New menu item clicked", "DataGrid");

    private void menuItem2_Click(object sender, System.EventArgs e) => MessageBox.Show("New menu item clicked", "MainMenu");

    private void button1_Click(object sender, System.EventArgs e)
    {
        if (_tablesAlreadyAdded)
            return;

        AddCustomDataTableStyle();
    }

    private void button2_Click(object sender, System.EventArgs e)
    {
        BindingManagerBase bmGrid = BindingContext[myDataSet, "Customers"];
        MessageBox.Show($"Current BindingManager Position: { bmGrid.Position }");
    }

    private void Grid_MouseUp(object sender, MouseEventArgs e)
    {
        try
        {
            HitTestInfo myHitInfo = dataGrid1.HitTest(e.X, e.Y);
        }
        catch (PlatformNotSupportedException ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void AddCustomDataTableStyle()
    {
        try
        {
            DataGridTableStyle customerDGTableStyle = new DataGridTableStyle
            {
                MappingName = "Customers",
                // Set other properties.
                AlternatingBackColor = Color.LightGray
            };

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
            DataGridTableStyle OrderDGTableStyle = new DataGridTableStyle
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
            PropertyDescriptorCollection propertyDescriptorCollection = BindingContext[myDataSet, "Customers.customerToOrders"].GetItemProperties();

            // Create a formatted column using a PropertyDescriptor.
            // The formatting character "c" specifies a currency format.
            DataGridColumnStyle csOrderAmount = new DataGridTextBoxColumn(propertyDescriptorCollection["OrderAmount"], "c", true)
            {
                MappingName = "OrderAmount",
                HeaderText = "Total",
                Width = 100
            };
            OrderDGTableStyle.GridColumnStyles.Add(csOrderAmount);

            // Add the DataGridTableStyle instances to the GridTableStylesCollection.
            dataGrid1.TableStyles.Add(customerDGTableStyle);
            dataGrid1.TableStyles.Add(OrderDGTableStyle);

            // Sets the TablesAlreadyAdded to true so this doesn't happen again.
            _tablesAlreadyAdded = true;
        }
        catch (PlatformNotSupportedException ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void SetUp()
    {
        // Create a DataSet with two tables and one relation.
        MakeDataSet();
        // Bind the DataGrid to the DataSet. The dataMember
        // specifies that the Customers table should be displayed.
        dataGrid1.SetDataBinding(myDataSet, "Customers");
    }

    // Create a DataSet with two tables and populate it.
    private void MakeDataSet()
    {
        // Create a DataSet.
        myDataSet = new DataSet("myDataSet");

        // Create two DataTables.
        DataTable customerDataTable = new DataTable("Customers");
        DataTable orderDataTable = new DataTable("Orders");

        // Create two columns, and add them to the first table.
        DataColumn customerIdDataColumn = new DataColumn("CustID", typeof(int));
        DataColumn customerNameDataColumn = new DataColumn("CustName");
        DataColumn currentDataColumn = new DataColumn("Current", typeof(bool));
        customerDataTable.Columns.Add(customerIdDataColumn);
        customerDataTable.Columns.Add(customerNameDataColumn);
        customerDataTable.Columns.Add(currentDataColumn);

        // Create three columns, and add them to the second table.
        DataColumn idDataColumn = new DataColumn("CustID", typeof(int));
        DataColumn orderDateDataColumn = new DataColumn("orderDate", typeof(DateTime));
        DataColumn orderAmountDataColumn = new DataColumn("OrderAmount", typeof(decimal));
        orderDataTable.Columns.Add(orderAmountDataColumn);
        orderDataTable.Columns.Add(idDataColumn);
        orderDataTable.Columns.Add(orderDateDataColumn);

        // Add the tables to the DataSet.
        myDataSet.Tables.Add(customerDataTable);
        myDataSet.Tables.Add(orderDataTable);

        // Create a DataRelation, and add it to the DataSet.
        DataRelation dataRelation = new DataRelation("customerToOrders", customerIdDataColumn, idDataColumn);
        myDataSet.Relations.Add(dataRelation);

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
    }
}
