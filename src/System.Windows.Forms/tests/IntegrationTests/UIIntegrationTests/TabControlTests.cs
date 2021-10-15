// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.UI.IntegrationTests.Infra;
using Xunit;
using static Interop;

namespace System.Windows.Forms.UI.IntegrationTests
{
    [ConfigureJoinableTaskFactory]
    public class TabControlTests
    {
        [StaFact]
        public void TabControlTests_TabPage_IsHoveredWithMouse_IsTrue_WhenMouseIsOn_FirstTab()
        {
            RunTest(tabControl =>
            {
                bool result = IsHoveredWithMouse(tabControl, 10, 10);
                Assert.True(result);
            });
        }

        [StaFact]
        public void TabControlTests_TabPage_IsHoveredWithMouse_IsTrue_WhenMouseIsOn_SecondTab()
        {
            RunTest(tabControl =>
            {
                bool result = IsHoveredWithMouse(tabControl, 60, 10);
                Assert.True(result);
            });
        }

        [StaFact]
        public void TabControlTests_TabPage_IsHoveredWithMouse_IsTrue_WhenMouseIsOn_SelectedPage()
        {
            RunTest(tabControl =>
            {
                bool result = IsHoveredWithMouse(tabControl, 50, 50);
                Assert.True(result);
            });
        }

        [StaFact]
        public void TabControlTests_TabPage_IsHoveredWithMouse_IsFalse_WhenMouseIs_OutsideControl()
        {
            RunTest(tabControl =>
            {
                bool result = IsHoveredWithMouse(tabControl, 300, 300);
                Assert.False(result);
            });
        }

        [StaFact]
        public void TabControlTests_TabPage_IsHoveredWithMouse_IsFalse_WhenMouseIs_OutsideMainScreen()
        {
            RunTest(tabControl =>
            {
                bool result = IsHoveredWithMouse(tabControl, -1000, -1000);
                Assert.False(result);
            });
        }

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

        private void RunTest(Action<TabControl> runTest)
        {
            UITest.RunControl(
                createControl: form =>
                {
                    TabControl tabControl = new()
                    {
                        Parent = form,
                        Location = new Point(0, 0)
                    };

                    TabPage tabPage1 = new();
                    TabPage tabPage2 = new();
                    tabControl.TabPages.Add(tabPage1);
                    tabControl.TabPages.Add(tabPage2);

                    return tabControl;
                },
                runTestAsync: async tabControl =>
                {
                    // Wait for pending operations so the Control is loaded completely before testing it
                    await AsyncTestHelper.JoinPendingOperationsAsync(AsyncTestHelper.UnexpectedTimeout);

                    runTest(tabControl);
                });
        }
    }
}
