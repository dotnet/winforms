// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.PropertyGridInternal;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

public class PropertyGridViewTests : ControlTestBase
{
    public PropertyGridViewTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task PropertyGridViewRowsAccessibleObject_Ctor_DefaultAsync()
    {
        await RunControlPairTestAsync<DomainUpDown, PropertyGrid>((form, controls) =>
        {
            (DomainUpDown domainUpDown, PropertyGrid propertyGrid) = controls;
            propertyGrid.Size = new Size(223, 244);
            form.ClientSize = new Size(508, 367);
            propertyGrid.SelectedObject = domainUpDown;
            GridEntryCollection entries = propertyGrid.GetCurrentEntries()!;
            PropertyGridView propertyGridView = (PropertyGridView)propertyGrid.Controls[2];

            int borderHeight = 2;
            int heightSum = 0;
            int entriesBorders = 0;

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

            Assert.Equal(heightSum, propertyGridView.AccessibilityObject.Bounds.Height - borderHeight - entriesBorders);

            return Task.CompletedTask;
        });
    }
}
