// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class DataGridViewInVirtualModeTest : Form
{
    // Declare fields for testing DGV in the Virtual mode
    // The example was taken from:
    // https://docs.microsoft.com/dotnet/desktop/winforms/controls/implementing-virtual-mode-wf-datagridview-control

    // Declare a List to serve as the test data source
    private readonly List<TestCustomer> _customers = [];

    // Declare a TestCustomer object to store data for a row being edited
    private TestCustomer _customerInEdit;

    // Declare a variable to store the index of a row being edited.
    // A value of -1 indicates that there is no row currently in edit
    private int _rowInEdit = -1;

    // Declare a variable to indicate the commit scope.
    // Set this value to false to use cell-level commit scope
    private readonly bool _rowScopeCommit = true;

    public DataGridViewInVirtualModeTest()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        // Connect the virtual-mode events to event handlers
        dataGridView1.CellValueNeeded += dataGridView1_CellValueNeeded;
        dataGridView1.CellValuePushed += dataGridView1_CellValuePushed;
        dataGridView1.NewRowNeeded += dataGridView1_NewRowNeeded;
        dataGridView1.RowValidated += dataGridView1_RowValidated;
        dataGridView1.RowDirtyStateNeeded += dataGridView1_RowDirtyStateNeeded;
        dataGridView1.CancelRowEdit += dataGridView1_CancelRowEdit;
        dataGridView1.UserDeletingRow += dataGridView1_UserDeletingRow;

        // Add some sample entries to the data store
        _customers.Add(new TestCustomer("Ann", 23, true, "Female"));
        _customers.Add(new TestCustomer("John", 45, true, "Male"));
        _customers.Add(new TestCustomer("Sarah", 67, false, "Female"));

        // Set the row count, including the row for new records
        dataGridView1.RowCount = _customers.Count + 1; // Add 1 as a new edit row
    }

    private void dataGridView1_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
    {
        // If this is the row for new records, no values are needed
        if (e.RowIndex == dataGridView1.RowCount - 1)
        {
            return;
        }

        // Store a reference to the TestCustomer object for the row being painted
        TestCustomer customer = e.RowIndex == _rowInEdit ? _customerInEdit : _customers[e.RowIndex];

        // Set the cell value to paint using the TestCustomer object retrieved
        switch (dataGridView1.Columns[e.ColumnIndex].Name)
        {
            case "personNameColumn":
                e.Value = customer.Name;
                break;
            case "ageColumn":
                e.Value = customer.Age;
                break;
            case "hasAJobColumn":
                e.Value = customer.HasAJob;
                break;
            case "genderColumn":
                e.Value = customer.Gender;
                break;
        }
    }

    private void dataGridView1_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
    {
        // Store a reference to the TestCustomer object for the row being edited
        if (e.RowIndex < _customers.Count)
        {
            // If the user is editing a new row, create a new TestCustomer object
            TestCustomer customer = _customers[e.RowIndex];
            _customerInEdit ??= new TestCustomer(customer.Name, customer.Age, customer.HasAJob, customer.Gender);
            _rowInEdit = e.RowIndex;
        }

        // Set the appropriate TestCustomer property to the cell value entered
        object newValue = e.Value;

        switch (dataGridView1.Columns[e.ColumnIndex].Name)
        {
            case "personNameColumn":
                _customerInEdit.Name = newValue.ToString();
                break;
            case "ageColumn":
                if (int.TryParse(newValue.ToString(), out int result))
                {
                    _customerInEdit.Age = result;
                }

                break;
            case "hasAJobColumn":
                _customerInEdit.HasAJob = (bool)newValue;
                break;
            case "genderColumn":
                _customerInEdit.Gender = newValue.ToString();
                break;
        }
    }

    private void dataGridView1_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
    {
        // Create a new TestCustomer object when the user edits
        // the row for new records
        _customerInEdit = new TestCustomer();
        _rowInEdit = dataGridView1.Rows.Count - 1;
    }

    private void dataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
    {
        // Save row changes if any were made and release the edited
        // TestCustomer object if there is one.
        if (e.RowIndex >= _customers.Count && e.RowIndex != dataGridView1.Rows.Count - 1)
        {
            // Add the new TestCustomer object to the data store
            _customers.Add(_customerInEdit);
            _customerInEdit = null;
            _rowInEdit = -1;
        }
        else if (_customerInEdit is not null && e.RowIndex < _customers.Count)
        {
            // Save the modified TestCustomer object in the data store
            _customers[e.RowIndex] = _customerInEdit;
            _customerInEdit = null;
            _rowInEdit = -1;
        }
        else if (dataGridView1.ContainsFocus)
        {
            _customerInEdit = null;
            _rowInEdit = -1;
        }
    }

    private void dataGridView1_RowDirtyStateNeeded(object sender, QuestionEventArgs e)
    {
        if (!_rowScopeCommit)
        {
            // In cell-level commit scope, indicate whether the value
            // of the current cell has been modified
            e.Response = dataGridView1.IsCurrentCellDirty;
        }
    }

    private void dataGridView1_CancelRowEdit(object sender, QuestionEventArgs e)
    {
        if (_rowInEdit == dataGridView1.Rows.Count - 2 && _rowInEdit == _customers.Count)
        {
            // If the user has canceled the edit of a newly created row,
            // replace the corresponding TestCustomer object with a new, empty one
            _customerInEdit = new TestCustomer();
        }
        else
        {
            // If the user has canceled the edit of an existing row,
            // release the corresponding TestCustomer object
            _customerInEdit = null;
            _rowInEdit = -1;
        }
    }

    private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
    {
        if (e.Row.Index < _customers.Count)
        {
            // If the user has deleted an existing row, remove the
            // corresponding TestCustomer object from the data store
            _customers.RemoveAt(e.Row.Index);
        }

        if (e.Row.Index == _rowInEdit)
        {
            // If the user has deleted a newly created row, release
            // the corresponding TestCustomer object
            _rowInEdit = -1;
            _customerInEdit = null;
        }
    }
}
