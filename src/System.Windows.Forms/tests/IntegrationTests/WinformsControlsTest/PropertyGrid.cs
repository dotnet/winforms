// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;

namespace WinformsControlsTest
{
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
}
