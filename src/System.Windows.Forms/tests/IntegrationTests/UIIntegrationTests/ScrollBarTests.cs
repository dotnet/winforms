// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.UI.IntegrationTests.Infra;
using Xunit;

namespace System.Windows.Forms.UI.IntegrationTests
{
    [ConfigureJoinableTaskFactory]
    public class ScrollBarTests
    {
        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_LTR_Value_0()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 100, value: 0);

                Assert.True(HFirstPageButton(hScrollBar).Bounds.Width == 0 || HFirstPageButton(hScrollBar).Bounds.Height == 0);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle_LTR_Value_50()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 100, value: 50);

                Assert.True(HFirstPageButton(hScrollBar).Bounds.Width > 0);

                Assert.True(HFirstPageButton(hScrollBar).Bounds.Height > 0);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle_LTR_Value_100()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 100, value: 100);

                Assert.True(HFirstPageButton(hScrollBar).Bounds.Width > 0);

                Assert.True(HFirstPageButton(hScrollBar).Bounds.Height > 0);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle_RTL_Value_0()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 100, value: 0);

                Assert.True(HFirstPageButton(hScrollBar).Bounds.Width > 0);

                Assert.True(HFirstPageButton(hScrollBar).Bounds.Height > 0);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle_RTL_Value_50()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);

                Assert.True(HFirstPageButton(hScrollBar).Bounds.Width > 0);

                Assert.True(HFirstPageButton(hScrollBar).Bounds.Height > 0);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_RTL_Value_100()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 100, value: 100);

                Assert.True(HFirstPageButton(hScrollBar).Bounds.Width == 0 || HFirstPageButton(hScrollBar).Bounds.Height == 0);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_LTR_MiniumEqualsMaximum()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 0, value: 0);

                Assert.True(HFirstPageButton(hScrollBar).Bounds.Width == 0 || HFirstPageButton(hScrollBar).Bounds.Height == 0);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_RTL_MiniumEqualsMaximum()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 0, value: 0);

                Assert.True(HFirstPageButton(hScrollBar).Bounds.Width == 0 || HFirstPageButton(hScrollBar).Bounds.Height == 0);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_State_ReturnInvisible_LTR_Value_0()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 100, value: 0);

                Assert.Equal(AccessibleStates.Invisible, HFirstPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_State_ReturnNone_LTR_Value_50()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 100, value: 50);

                Assert.Equal(AccessibleStates.None, HFirstPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_State_ReturnNone_LTR_Value_100()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 100, value: 100);

                Assert.Equal(AccessibleStates.None, HFirstPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_State_ReturnNone_RTL_Value_0()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 100, value: 0);

                Assert.Equal(AccessibleStates.None, HFirstPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_State_ReturnNone_RTL_Value_50()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);

                Assert.Equal(AccessibleStates.None, HFirstPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_State_ReturnInvisible_RTL_Value_100()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 100, value: 100);

                Assert.Equal(AccessibleStates.Invisible, HFirstPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_State_ReturnInvisible_LTR_MiniumEqualsMaximum()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 0, value: 0);

                Assert.Equal(AccessibleStates.Invisible, HFirstPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarFirstPageButtonAccessibleObject_State_ReturnInvisible_RTL_MiniumEqualsMaximum()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 0, value: 0);

                Assert.Equal(AccessibleStates.Invisible, HFirstPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle_LTR_Value_0()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 100, value: 0);

                Assert.True(HLastPageButton(hScrollBar).Bounds.Width > 0);

                Assert.True(HLastPageButton(hScrollBar).Bounds.Height > 0);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle_LTR_Value_50()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 100, value: 50);

                Assert.True(HLastPageButton(hScrollBar).Bounds.Width > 0);

                Assert.True(HLastPageButton(hScrollBar).Bounds.Height > 0);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_LTR_Value_100()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 100, value: 100);

                Assert.True(HLastPageButton(hScrollBar).Bounds.Width == 0 || HLastPageButton(hScrollBar).Bounds.Height == 0);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_RTL_Value_0()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 100, value: 0);

                Assert.True(HLastPageButton(hScrollBar).Bounds.Width == 0 || HLastPageButton(hScrollBar).Bounds.Height == 0);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle_RTL_Value_50()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);

                Assert.True(HLastPageButton(hScrollBar).Bounds.Width > 0);

                Assert.True(HLastPageButton(hScrollBar).Bounds.Height > 0);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle_RTL_Value_100()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 100, value: 100);

                Assert.True(HLastPageButton(hScrollBar).Bounds.Width > 0);

                Assert.True(HLastPageButton(hScrollBar).Bounds.Height > 0);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_LTR_MiniumEqualsMaximum()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 0, value: 0);

                Assert.True(HLastPageButton(hScrollBar).Bounds.Width == 0 || HLastPageButton(hScrollBar).Bounds.Height == 0);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_RTL_MiniumEqualsMaximum()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 0, value: 0);

                Assert.True(HLastPageButton(hScrollBar).Bounds.Width == 0 || HLastPageButton(hScrollBar).Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_State_ReturnNone_LTR_Value_0()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 100, value: 0);

                Assert.Equal(AccessibleStates.None, HLastPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_State_ReturnNone_LTR_Value_50()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 100, value: 50);

                Assert.Equal(AccessibleStates.None, HLastPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_State_ReturnInvisible_LTR_Value_100()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 100, value: 100);

                Assert.Equal(AccessibleStates.Invisible, HLastPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_State_ReturnInvisible_RTL_Value_0()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 100, value: 0);

                Assert.Equal(AccessibleStates.Invisible, HLastPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_State_ReturnNone_RTL_Value_50()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);

                Assert.Equal(AccessibleStates.None, HLastPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_State_ReturnNone_RTL_Value_100()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 100, value: 100);

                Assert.Equal(AccessibleStates.None, HLastPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_State_ReturnInvisible_LTR_MiniumEqualsMaximum()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.No, minimum: 0, maximum: 0, value: 0);

                Assert.Equal(AccessibleStates.Invisible, HLastPageButton(hScrollBar).State);
            });
        }

        [StaFact]
        public void HScrollBarLastPageButtonAccessibleObject_State_ReturnInvisible_RTL_MiniumEqualsMaximum()
        {
            RunTestForHScrollBar(hScrollBar =>
            {
                SetHScrollBar(hScrollBar, RightToLeft.Yes, minimum: 0, maximum: 0, value: 0);

                Assert.Equal(AccessibleStates.Invisible, HLastPageButton(hScrollBar).State);
            });
        }

        private void SetHScrollBar(HScrollBar hScrollBar, RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            hScrollBar.RightToLeft = rightToLeft;
            hScrollBar.Minimum = minimum;
            hScrollBar.Maximum = maximum;
            hScrollBar.Value = value;
            Application.DoEvents();
        }

        private ScrollBar.ScrollBarFirstPageButtonAccessibleObject HFirstPageButton(HScrollBar hScrollBar)
           =>((VScrollBar.ScrollBarAccessibleObject) hScrollBar.AccessibilityObject).FirstPageButtonAccessibleObject;

        private ScrollBar.ScrollBarLastPageButtonAccessibleObject HLastPageButton(HScrollBar hScrollBar)
            => ((VScrollBar.ScrollBarAccessibleObject)hScrollBar.AccessibilityObject).LastPageButtonAccessibleObject;

        private void RunTestForHScrollBar(Action<HScrollBar> runTest)
        {
            UITest.RunControl(
                createControl: form =>
                {
                    HScrollBar hScrollBar = new()
                    {
                        Parent = form,
                    };
                    return hScrollBar;
                },
                runTestAsync: async hScrollBar =>
                {
                    // Wait for pending operations so the Control is loaded completely before testing it
                    await AsyncTestHelper.JoinPendingOperationsAsync(AsyncTestHelper.UnexpectedTimeout);

                    runTest(hScrollBar);
                });
        }
    }
}
