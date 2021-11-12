// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.PropertyGridInternal;
using System.Windows.Forms.UI.IntegrationTests.Infra;
using Xunit;

namespace System.Windows.Forms.UI.IntegrationTests
{
    [ConfigureJoinableTaskFactory]
    public class PropertyGridViewRowsAccessibleObjectTests
    {
        [StaFact]
        public void PropertyGridViewRowsAccessibleObject_Ctor_Default()
        {
            RunTest(propertyGrid =>
            {
                propertyGrid.SelectedObject = new DomainUpDown();
                int heightSum = 0;
                int entriesBorders = 0;
                int bottomBorder = 1;
                int topBorder = 1;

                GridEntryCollection entries = propertyGrid.GetCurrentEntries();

                Application.DoEvents();

                PropertyGridView propertyGridView = (PropertyGridView)propertyGrid.ActiveControl;

                foreach (GridEntry entry in entries)
                {
                    int entryHeight = propertyGridView.AccessibilityGetGridEntryBounds(entry).Height;
                    heightSum += entryHeight;
                    if (entryHeight > 0)
                    {
                        entriesBorders++;
                    }

                    foreach (GridEntry item in entry.GridItems)
                    {
                        int itemHeight = propertyGridView.AccessibilityGetGridEntryBounds(item).Height;
                        heightSum += itemHeight;
                        if (itemHeight > 0)
                        {
                            entriesBorders++;
                        }
                    }
                }

                int actualHeightSum = propertyGridView.AccessibilityObject.Bounds.Height - topBorder - bottomBorder - entriesBorders;

                Assert.Equal(heightSum, actualHeightSum);
            });
        }

        private void RunTest(Action<PropertyGrid> runTest)
        {
            UITest.RunControl(
                createControl: form =>
                {
                    PropertyGrid propertyGrid = new()
                    {
                        Parent = form,
                        Size = new Size(223, 244),
                };

                    return propertyGrid;
                },
                runTestAsync: async propertyGrid =>
                {
                    // Wait for pending operations so the Control is loaded completely before testing it
                    await AsyncTestHelper.JoinPendingOperationsAsync(AsyncTestHelper.UnexpectedTimeout);

                    runTest(propertyGrid);
                });
        }
    }
}
