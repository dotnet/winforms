// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.PropertyGridInternal;
using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class PropertyGridViewRowsAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void PropertyGridViewRowsAccessibleObject_Ctor_Default()
        {
            const int bottomBorder = 1;
            const int topBorder = 1;

            // track that the form has been shown and executed the main assert
            bool formShown = false;
            Dimensions dimensions = default;

            using TestForm form = new TestForm();
            form.Load += (s, e) =>
            {
                dimensions = form.CalculateDimensions();
                formShown = true;
            };
            form.Show();

            Assert.True(formShown);
            Assert.True(dimensions.AccRowHeightSum == dimensions.AccPropertyGridViewHeight - topBorder - bottomBorder - dimensions.EntriesBorders);
        }

        private struct Dimensions
        {
            public int AccPropertyGridViewHeight;
            public int AccRowHeightSum;
            public int EntriesBorders;
        }

        private class TestForm : Form
        {
            private DomainUpDown domainUpDown;
            private PropertyGrid propertyGrid;

            public Dimensions CalculateDimensions()
            {
                propertyGrid.SelectedObject = domainUpDown;
                int heightSum = 0;
                int entriesBorders = 0;

                GridEntryCollection entries = propertyGrid.GetPropEntries();
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

                return new Dimensions
                {
                    AccPropertyGridViewHeight = propertyGridView.AccessibilityObject.Bounds.Height,
                    AccRowHeightSum = heightSum,
                    EntriesBorders = entriesBorders
                };
            }

            public TestForm()
            {
                InitializeComponent();
            }

            private void InitializeComponent()
            {
                domainUpDown = new DomainUpDown();
                propertyGrid = new PropertyGrid();
                //
                // propertyGrid
                //
                propertyGrid.Size = new Size(223, 244);
                //
                // TestForm
                //
                ClientSize = new Size(508, 367);
                Controls.Add(propertyGrid);
                Controls.Add(domainUpDown);
            }
        }
    }
}
