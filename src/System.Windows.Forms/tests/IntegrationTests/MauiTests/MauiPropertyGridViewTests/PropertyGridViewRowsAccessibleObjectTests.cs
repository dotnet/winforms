// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;
using System.Windows.Forms.PropertyGridInternal;
using ReflectTools;
using WFCTestLib.Log;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class PropertyGridViewRowsAccessibleObjectTests : ReflectBase
    {
        private readonly DomainUpDown _domainUpDown;
        private readonly PropertyGrid _propertyGrid;

        private const int bottomBorder = 1;
        private const int topBorder = 1;

        public PropertyGridViewRowsAccessibleObjectTests(string[] args) : base(args)
        {
            this.BringToForeground();

            _domainUpDown = new DomainUpDown();
            _propertyGrid = new PropertyGrid();
            _propertyGrid.Size = new Size(223, 244);
            ClientSize = new Size(508, 367);
            Controls.Add(_propertyGrid);
            Controls.Add(_domainUpDown);
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new PropertyGridViewRowsAccessibleObjectTests(args));
        }

        [Scenario(true)]
        public ScenarioResult PropertyGridViewRowsAccessibleObject_Ctor_Default(TParams p)
        {
            _propertyGrid.SelectedObject = _domainUpDown;
            int heightSum = 0;
            int entriesBorders = 0;

            GridEntryCollection entries = _propertyGrid.GetPropEntries();
            PropertyGridView propertyGridView = (PropertyGridView)_propertyGrid.ActiveControl;

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

            if (heightSum != propertyGridView.AccessibilityObject.Bounds.Height - topBorder - bottomBorder - entriesBorders)
            {
                return new ScenarioResult(false, "Incorrect dimensions");
            }

            return new ScenarioResult(true);
        }
    }
}
