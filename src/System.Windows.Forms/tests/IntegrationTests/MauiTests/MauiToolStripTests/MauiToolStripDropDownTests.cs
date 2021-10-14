// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools;
using WFCTestLib.Log;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiToolStripDropDownTests : ReflectBase
    {
        private readonly SubToolStripDropDown _toolStrip;

        public MauiToolStripDropDownTests(string[] args) : base(args)
        {
            this.BringToForeground();
            _toolStrip = new SubToolStripDropDown();
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiToolStripDropDownTests(args));
        }

        [Scenario(true)]
        public ScenarioResult KeyboardAccelerators_Test(TParams p)
        {
            bool _result = true;

            _toolStrip.Enabled = true; // it needs for correct work of Control.CanProcessMnemonic method
            _toolStrip.Visible = true; //

            _result &= !_toolStrip.ProcessDialogChar('F');

            _toolStrip.DisplayedItems.Add("&First item");
            _toolStrip.DisplayedItems.Add("&Second item");
            _toolStrip.DisplayedItems.Add("Third item");
            _toolStrip.Visible = true; // it needs for correct work of Control.CanProcessMnemonic method

            _result &= _toolStrip.ProcessDialogChar('F');
            _result &= _toolStrip.ProcessDialogChar('S');
            _result &= !_toolStrip.ProcessDialogChar('T');

            return new ScenarioResult(_result);
        }

        [Scenario(true)]
        public ScenarioResult ToolStripScrollButton_Arrows_Size_ReturnsExpected(TParams p)
        {
            Type toolStripScrollButtonType = typeof(ToolStripScrollButton);
            var accessor = typeof(DpiHelper).TestAccessor();
            Size defaultSize = new(16, 16);
            int oldDeviceDpi = DpiHelper.DeviceDpi;
            List<DpiTestData> dpiTestData = new();
            dpiTestData.Add(new DpiTestData() { ResourceName = "ScrollButtonDown", Dpi = 96, ExpectedSide = 16 });
            dpiTestData.Add(new DpiTestData() { ResourceName = "ScrollButtonDown", Dpi = 120, ExpectedSide = 24 });
            dpiTestData.Add(new DpiTestData() { ResourceName = "ScrollButtonDown", Dpi = 144, ExpectedSide = 24 });
            dpiTestData.Add(new DpiTestData() { ResourceName = "ScrollButtonDown", Dpi = 168, ExpectedSide = 32 });
            dpiTestData.Add(new DpiTestData() { ResourceName = "ScrollButtonDown", Dpi = 288, ExpectedSide = 48 });
            dpiTestData.Add(new DpiTestData() { ResourceName = "ScrollButtonUp", Dpi = 96, ExpectedSide = 16 });
            dpiTestData.Add(new DpiTestData() { ResourceName = "ScrollButtonUp", Dpi = 120, ExpectedSide = 24 });
            dpiTestData.Add(new DpiTestData() { ResourceName = "ScrollButtonUp", Dpi = 144, ExpectedSide = 24 });
            dpiTestData.Add(new DpiTestData() { ResourceName = "ScrollButtonUp", Dpi = 168, ExpectedSide = 32 });
            dpiTestData.Add(new DpiTestData() { ResourceName = "ScrollButtonUp", Dpi = 288, ExpectedSide = 48 });

            try
            {
                foreach (DpiTestData data in dpiTestData)
                {
                    accessor.Dynamic.DeviceDpi = data.Dpi;
                    Bitmap bitmap = DpiHelper.GetScaledBitmapFromIcon(toolStripScrollButtonType, data.ResourceName, defaultSize);
                    if (data.ExpectedSide != bitmap.Width || data.ExpectedSide != bitmap.Height)
                    {
                        return new ScenarioResult(false);
                    }
                }
            }
            finally
            {
                accessor.Dynamic.DeviceDpi = oldDeviceDpi;
            }

            return new ScenarioResult(true);
        }

        private class DpiTestData
        {
            public string ResourceName { get; set; }

            public int Dpi { get; set; }

            public int ExpectedSide { get; set; }
        }

        private class SubToolStripDropDown : ToolStripDropDown
        {
            public new ToolStripItemCollection DisplayedItems => base.DisplayedItems;

            public new bool ProcessDialogChar(char charCode) => base.ProcessDialogChar(charCode);
        }
    }
}
