// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        internal class DataGridViewEditingPanel : Panel
        {
            private readonly DataGridView _owningDataGridView;

            public DataGridViewEditingPanel(DataGridView owningDataGridView)
            {
                _owningDataGridView = owningDataGridView;
            }

            internal override bool SupportsUiaProviders => true;

            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new DataGridViewEditingPanelAccessibleObject(_owningDataGridView, this);
            }
        }
    }
}
