// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms.Design;

[ToolboxItem(false)]
[DesignTimeVisible(false)]
internal partial class DataGridViewColumnTypePicker : ContainerControl
{
    private readonly ListBox _typesListBox;
    private Type? _selectedType;

    // The current editor service that we need to close the drop down.
    private IWindowsFormsEditorService? _windowsFormsEditorService;
    private static readonly Type s_dataGridViewColumnType = typeof(DataGridViewColumn);

    private const int MinimumHeight = 90;
    private const int MinimumWidth = 100;

    public DataGridViewColumnTypePicker()
    {
        _typesListBox = new();
        Size = _typesListBox.Size;
        _typesListBox.Dock = DockStyle.Fill;
        _typesListBox.Sorted = true;
        _typesListBox.HorizontalScrollbar = true;
        _typesListBox.SelectedIndexChanged += typesListBox_SelectedIndexChanged;
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
            using Graphics g = _typesListBox.CreateGraphics();

            for (int i = 0; i < _typesListBox.Items.Count; i++)
            {
                ListBoxItem item = (ListBoxItem)_typesListBox.Items[i];
                width = Math.Max(width, Size.Ceiling(g.MeasureString(item.ToString(), _typesListBox.Font)).Width);
            }

            return width;
        }
    }

    private void CloseDropDown() => _windowsFormsEditorService?.CloseDropDown();

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
    ///  Setup the picker, and fill it with type information
    /// </devdoc>
    public void Start(IWindowsFormsEditorService edSvc, ITypeDiscoveryService discoveryService, Type defaultType)
    {
        _windowsFormsEditorService = edSvc;

        _typesListBox.Items.Clear();

        ICollection columnTypes = DesignerUtils.FilterGenericTypes(discoveryService.GetTypes(s_dataGridViewColumnType, excludeGlobalTypes: false));

        foreach (Type t in columnTypes)
        {
            if (t == s_dataGridViewColumnType)
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

        // Set our default width.
        Width = Math.Max(Width, PreferredWidth + (SystemInformation.VerticalScrollBarWidth * 2));
    }

    private void typesListBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        _selectedType = _typesListBox.SelectedItem is ListBoxItem selectedItem ? selectedItem.ColumnType : null;
        _windowsFormsEditorService?.CloseDropDown();
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
}
