// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.PropertyGridInternal.TestUtilities;
using System.Windows.Forms.UITests;
using Xunit;
using Xunit.Abstractions;
using static Interop.UiaCore;

namespace System.Windows.Forms.PropertyGridInternal.UITests;

public class PropertyGridView_GridViewListBox_GridViewListBoxAccessibleObjectTests : ControlTestBase
{
    public PropertyGridView_GridViewListBox_GridViewListBoxAccessibleObjectTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task GridViewListBoxAccessibleObject_FragmentNavigate_PreviousSibling_IsNullAsync()
    {
        await RunSingleControlTestAsync<SubPropertyGrid<Button>>((form, grid) =>
        {
            grid.SelectedEntry = grid[nameof(Button.AccessibleRole)];

            grid.PopupEditorAndClose(() =>
            {
                Assert.Null(grid.GridView.DropDownListBoxAccessibleObject.FragmentNavigate(NavigateDirection.PreviousSibling));
            });

            return Task.CompletedTask;
        });
    }
}
