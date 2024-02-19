using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design;
[
ToolboxItem(false),
DesignTimeVisible(false)
]
internal class DataGridViewColumnTypePicker : ContainerControl
{
    private ListBox _typesListBox;
    private Type? _selectedType;

    private IWindowsFormsEditorService? _edSvc;        // the current editor service that we need to close the drop down.
    private static Type _dataGridViewColumnType = typeof(DataGridViewColumn);

    private const int MinimumHeight = 90;
    private const int MinimumWidth = 100;

    public DataGridViewColumnTypePicker()
    {
        _typesListBox = new ListBox();
        Size = _typesListBox.Size;
        _typesListBox.Dock = DockStyle.Fill;
        _typesListBox.Sorted = true;
        _typesListBox.HorizontalScrollbar = true;
        _typesListBox.SelectedIndexChanged += new EventHandler(typesListBox_SelectedIndexChanged);
        Controls.Add(_typesListBox);
        BackColor = SystemColors.Control;
        ActiveControl = _typesListBox;
    }

    public Type? SelectedType => _selectedType;

    private int PreferredWidth
    {
        get
        {
            int width = 0;
            Graphics g = _typesListBox.CreateGraphics();
            try
            {
                for (int i = 0; i < _typesListBox.Items.Count; i++)
                {
                    ListBoxItem item = (ListBoxItem)_typesListBox.Items[i];
                    width = Math.Max(width, Size.Ceiling(g.MeasureString(item.ToString(), _typesListBox.Font)).Width);
                }
            }
            finally
            {
                g.Dispose();
            }

            return width;
        }
    }

    private void CloseDropDown()
    {
        if (_edSvc is not null)
        {
            _edSvc.CloseDropDown();
        }
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        if ((BoundsSpecified.Width & specified) == BoundsSpecified.Width)
        {
            width = Math.Max(width, MinimumWidth);
        }

        if ((BoundsSpecified.Height & specified) == BoundsSpecified.Height)
        {
            height = Math.Max(height, MinimumHeight);
        }

        base.SetBoundsCore(x, y, width, height, specified);
    }

    /// <devdoc>
    /// Setup the picker, and fill it with type information
    /// </devdoc>
    public void Start(IWindowsFormsEditorService edSvc, ITypeDiscoveryService discoveryService, Type defaultType)
    {
        _edSvc = edSvc;

        _typesListBox.Items.Clear();

        ICollection columnTypes = DesignerUtils.FilterGenericTypes(discoveryService.GetTypes(_dataGridViewColumnType, false /*excludeGlobalTypes*/));

        foreach (Type t in columnTypes)
        {
            if (t == _dataGridViewColumnType)
            {
                continue;
            }

            if (t.IsAbstract)
            {
                continue;
            }

            if (!t.IsPublic && !t.IsNestedPublic)
            {
                continue;
            }

            DataGridViewColumnDesignTimeVisibleAttribute? attr = TypeDescriptor.GetAttributes(t)[typeof(DataGridViewColumnDesignTimeVisibleAttribute)] as DataGridViewColumnDesignTimeVisibleAttribute;
            if (attr is not null && !attr.Visible)
            {
                continue;
            }

            _typesListBox.Items.Add(new ListBoxItem(t));
        }

        _typesListBox.SelectedIndex = TypeToSelectedIndex(defaultType);

        _selectedType = null;

        // set our default width.
        //
        Width = Math.Max(Width, PreferredWidth + (SystemInformation.VerticalScrollBarWidth * 2));
    }

    private void typesListBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _selectedType = _typesListBox.SelectedItem is ListBoxItem selectedItem ? selectedItem.ColumnType : null;
        _edSvc?.CloseDropDown();
    }

    private int TypeToSelectedIndex(Type type)
    {
        for (int i = 0; i < _typesListBox.Items.Count; i++)
        {
            if (type == ((ListBoxItem)_typesListBox.Items[i]).ColumnType)
            {
                return i;
            }
        }

        Debug.Assert(false, "we should have found a type by now");

        return -1;
    }

    private class ListBoxItem
    {
        private Type _columnType;
        public ListBoxItem(Type columnType)
        {
            _columnType = columnType;
        }

        public override string ToString()
        {
            return _columnType.Name;
        }

        public Type ColumnType
        {
            get
            {
                return _columnType;
            }
        }
    }
}
