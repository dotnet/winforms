using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.PropertyGridInternal;
using Xunit;
using static System.Windows.Forms.PropertyGridInternal.GridEntry;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class PropertyGridViewRowsAccessibleObjectTests
    {
        [Fact]
        public void PropertyGridViewRowsAccessibleObject_Ctor_Default()
        {
            TestForm form = new TestForm();
            Application.Run(form);

            const int topBorder = 1;
            const int bottomBorder = 1;
            int entriesBorders = form.entriesBorders;

            int a = form.AccRowHeightSum;
            int b = form.AccPropertyGridViewHeight;

            Assert.True(form.AccRowHeightSum == form.AccPropertyGridViewHeight - topBorder - bottomBorder - entriesBorders);
        }
    }

    public class TestForm : Form
    {
        public int AccPropertyGridViewHeight { get; set; }
        public int AccRowHeightSum { get; set; }

        DomainUpDown domainUpDown;
        PropertyGrid propertyGrid;

        public int entriesBorders = 0;

        private void TestForm_Load(object sender, EventArgs e)
        {
            this.propertyGrid.SelectedObject = this.domainUpDown;

            PropertyGrid propertyGrid = (PropertyGrid)((Form)sender).Controls[0];
            GridEntryCollection entries = propertyGrid.GetPropEntries();
            PropertyGridView propertyGridView = (PropertyGridView)propertyGrid.ActiveControl;

            foreach (GridEntry entry in entries)
            {
                int entryHeight = propertyGridView.AccessibilityGetGridEntryBounds(entry).Height;
                AccRowHeightSum += entryHeight;
                if (entryHeight > 0)
                {
                    entriesBorders++;
                }

                foreach (GridEntry item in entry.GridItems)
                {
                    int itemHeight = propertyGridView.AccessibilityGetGridEntryBounds(item).Height;
                    AccRowHeightSum += itemHeight;
                    if(itemHeight > 0)
                    {
                        entriesBorders++;
                    }
                }
            }
            AccPropertyGridViewHeight = propertyGridView.AccessibilityObject.Bounds.Height;

            Application.Exit();
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
            this.propertyGrid.Size = new Size(223, 244);
            // 
            // TestForm
            // 
            this.ClientSize = new Size(508, 367);
            this.Controls.Add(propertyGrid);
            this.Controls.Add(domainUpDown);
            this.Load += new EventHandler(this.TestForm_Load);
        }

    }

}
