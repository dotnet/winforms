// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Threading;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.IntegrationTests.Common;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiListViewTests : ReflectBase
    {
        private readonly ListView _listView;

        public MauiListViewTests(string[] args) : base(args)
        {
            this.BringToForeground();
            _listView = new ListView { Size = new System.Drawing.Size(439, 103) };
            Controls.Add(_listView);
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiListViewTests(args));
        }

        [Scenario(true)]
        public ScenarioResult Click_On_Second_Column_Does_Not_Alter_Checkboxes(TParams p)
        {
            InitializeItems(_listView, p);

            foreach (ListViewItem item in _listView.Items)
            {
                if (item.StateImageIndex != 0)
                    return new ScenarioResult(false, "Precondition failed: all checkboxes must be unmarked");
            }

            foreach (ListViewItem item in _listView.Items)
            {
                if (item.Selected)
                    return new ScenarioResult(false, "Precondition failed: all items must be unselected");
            }

            KeyboardHelper.SendKey(Keys.ShiftKey, true);

            var pt = MouseHelper.GetCenter(_listView.RectangleToScreen(_listView.Items[0].SubItems[1].Bounds));
            MouseHelper.SendClick(pt.x, pt.y);

            pt = MouseHelper.GetCenter(_listView.RectangleToScreen(_listView.Items[2].SubItems[1].Bounds));
            MouseHelper.SendClick(pt.x, pt.y);

            KeyboardHelper.SendKey(Keys.ShiftKey, false);
            Application.DoEvents();

            foreach (ListViewItem item in _listView.Items)
            {
                if (item.StateImageIndex != 0)
                    return new ScenarioResult(false, "All checkboxes must be unmarked");
            }

            foreach (ListViewItem item in _listView.Items)
            {
                if (!item.Selected)
                    return new ScenarioResult(false, "All items must be selected");
            }

            return new ScenarioResult(true);
        }

        private void InitializeItems(ListView listView, TParams p)
        {
            listView.Items.Clear();
            listView.CheckBoxes = true;
            listView.FullRowSelect = true;
            listView.View = View.Details;

            var columnHeader1 = new ColumnHeader { Text = "ColumnHeader1", Width = 140 };
            var columnHeader2 = new ColumnHeader { Text = "ColumnHeader2", Width = 140 };
            var columnHeader3 = new ColumnHeader { Text = "ColumnHeader3", Width = 140 };
            listView.Columns.AddRange(new[] { columnHeader1, columnHeader2, columnHeader3 });

            var listViewItem1 = new ListViewItem(new[] { "row1", "row1Col2", "row1Col3" }, -1) { StateImageIndex = 0 };
            var listViewItem2 = new ListViewItem(new[] { "row2", "row2Col2", "row2Col3" }, -1) { StateImageIndex = 0 };
            var listViewItem3 = new ListViewItem(new[] { "row3", "row3Col2", "row3Col3" }, -1) { StateImageIndex = 0 };
            listView.Items.AddRange(new[] { listViewItem1, listViewItem2, listViewItem3 });
        }
    }
}
