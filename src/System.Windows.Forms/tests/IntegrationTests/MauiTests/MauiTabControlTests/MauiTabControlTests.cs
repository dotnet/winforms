// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools;
using WFCTestLib.Log;
using static Interop;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiTabControlTests : ReflectBase
    {
        private readonly TabControl _tabControl;
        private readonly TabPage _tabPage1;
        private readonly TabPage _tabPage2;

        public MauiTabControlTests(string[] args) : base(args)
        {
            this.BringToForeground();
            _tabControl = new TabControl() { Location = new Point(0, 0) };
            _tabPage1 = new TabPage();
            _tabPage2 = new TabPage();
            _tabControl.TabPages.Add(_tabPage1);
            _tabControl.TabPages.Add(_tabPage2);
            Controls.Add(_tabControl);
        }

        static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiTabControlTests(args));
        }

        [Scenario(true)]
        public ScenarioResult TabPage_IsHoveredWithMouse_IsTrue_WhenMouseIsOn_FirstTab(TParams p)
            => new ScenarioResult(IsHoveredWithMouse(_tabControl, 10, 10));

        [Scenario(true)]
        public ScenarioResult TabPage_IsHoveredWithMouse_IsTrue_WhenMouseIsOn_SecondTab(TParams p)
            => new ScenarioResult(IsHoveredWithMouse(_tabControl, 60, 10));

        [Scenario(true)]
        public ScenarioResult TabPage_IsHoveredWithMouse_IsTrue_WhenMouseIsOn_SelectedPage(TParams p)
            => new ScenarioResult(IsHoveredWithMouse(_tabControl, 50, 50));

        [Scenario(true)]
        public ScenarioResult TabPage_IsHoveredWithMouse_IsFalse_WhenMouseIs_OutsideControl(TParams p)
            => new ScenarioResult(!IsHoveredWithMouse(_tabControl, 300, 300));

        [Scenario(true)]
        public ScenarioResult TabPage_IsHoveredWithMouse_IsFalse_WhenMouseIs_OutsideMainScreen(TParams p)
            => new ScenarioResult(!IsHoveredWithMouse(_tabControl, -1000, -1000));

        private bool IsHoveredWithMouse(TabControl tabControl, int mousePositionX, int mousePositionY)
        {
            Point previousPosition = new Point();
            BOOL setOldCursorPosition = User32.GetPhysicalCursorPos(ref previousPosition);

            try
            {
                // Offset the mouse cursor relative to the test app window
                Point pt = tabControl.PointToScreen(new Point(mousePositionX, mousePositionY));
                MouseHelper.ChangeMousePosition(pt.X, pt.Y);

                bool resultOfPage1 = ((IKeyboardToolTip)tabControl.TabPages[0]).IsHoveredWithMouse();
                bool resultOfPage2 = ((IKeyboardToolTip)tabControl.TabPages[1]).IsHoveredWithMouse();

                return resultOfPage1 && resultOfPage2;
            }
            finally
            {
                if (setOldCursorPosition.IsTrue())
                {
                    // Move cursor to old position
                    MouseHelper.ChangeMousePosition(previousPosition.X, previousPosition.Y);
                    Application.DoEvents();
                }
            }
        }
    }
}
