// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class PropertyGrid : Form
{
    public PropertyGrid(object selectedObject)
    {
        InitializeComponent();
        propertyGrid1.SelectedObject = selectedObject;
    }

    // VS designer specific
    private PropertyGrid()
    {
        InitializeComponent();
    }
}
