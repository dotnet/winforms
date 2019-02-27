// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel.Design;

namespace WinformsControlsTest
{
    public partial class PropertyGrid : Form
    {
        public PropertyGrid()
        {
            InitializeComponent();
            propertyGrid1.SelectedObject = new UserControlWithObjectCollectionEditor();
        }
    }

    public partial class UserControlWithObjectCollectionEditor : UserControl
    {
        public UserControlWithObjectCollectionEditor()
        {
            AutoScaleMode = AutoScaleMode.Font;
        }

        [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public int[] SomeCollection
        {
            get { return new int[] { 1, 2, 3 }; }
            set { }
        }
    }

}
