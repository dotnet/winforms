// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using ReflectTools;
using WFCTestLib.Log;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public partial class MauiScrollBarTests
    {
        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_LTR_Value_0(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(VFirstPageButton.Bounds.Width == 0 || VFirstPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle_LTR_Value_50(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(VFirstPageButton.Bounds.Width > 0 && VFirstPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle_LTR_Value_100(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(VFirstPageButton.Bounds.Width > 0 && VFirstPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_RTL_Value_0(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(VFirstPageButton.Bounds.Width == 0 || VFirstPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle_RTL_Value_50(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(VFirstPageButton.Bounds.Width > 0 && VFirstPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle_RTL_Value_100(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(VFirstPageButton.Bounds.Width > 0 && VFirstPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_LTR_MiniumEqualsMaximum(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(VFirstPageButton.Bounds.Width == 0 || VFirstPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_RTL_MiniumEqualsMaximum(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(VFirstPageButton.Bounds.Width == 0 || VFirstPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_State_ReturnInvisible_LTR_Value_0(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(VFirstPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_State_ReturnNone_LTR_Value_50(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(VFirstPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_State_ReturnNone_LTR_Value_100(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(VFirstPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_State_ReturnInvisible_RTL_Value_0(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(VFirstPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_State_ReturnNone_RTL_Value_50(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(VFirstPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_State_ReturnNone_RTL_Value_100(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(VFirstPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_State_ReturnInvisible_LTR_MiniumEqualsMaximum(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(VFirstPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarFirstPageButtonAccessibleObject_State_ReturnInvisible_RTL_MiniumEqualsMaximum(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(VFirstPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle_LTR_Value_0(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(VLastPageButton.Bounds.Width > 0 && VLastPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle_LTR_Value_50(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(VLastPageButton.Bounds.Width > 0 && VLastPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_LTR_Value_100(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(VLastPageButton.Bounds.Width == 0 || VLastPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle_RTL_Value_0(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(VLastPageButton.Bounds.Width > 0 && VLastPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle_RTL_Value_50(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(VLastPageButton.Bounds.Width > 0 && VLastPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_RTL_Value_100(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(VLastPageButton.Bounds.Width == 0 || VLastPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_LTR_MiniumEqualsMaximum(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(VLastPageButton.Bounds.Width == 0 || VLastPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_RTL_MiniumEqualsMaximum(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(VLastPageButton.Bounds.Width == 0 || VLastPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_State_ReturnNone_LTR_Value_0(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(VLastPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_State_ReturnNone_LTR_Value_50(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(VLastPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_State_ReturnInvisible_LTR_Value_100(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(VLastPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_State_ReturnNone_RTL_Value_0(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(VLastPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_State_ReturnNone_RTL_Value_50(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(VLastPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_State_ReturnNone_Invisible_Value_100(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(VLastPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_State_ReturnInvisible_LTR_MiniumEqualsMaximum(TParams p)
        {
            SetVScrollBar(RightToLeft.No, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(VLastPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult VScrollBarLastPageButtonAccessibleObject_State_ReturnInvisible_RTL_MiniumEqualsMaximum(TParams p)
        {
            SetVScrollBar(RightToLeft.Yes, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(VLastPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        private void SetVScrollBar(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            _vScrollBar.RightToLeft = rightToLeft;
            _vScrollBar.Minimum = minimum;
            _vScrollBar.Maximum = maximum;
            _vScrollBar.Value = value;
            Application.DoEvents();
        }

        private ScrollBar.ScrollBarFirstPageButtonAccessibleObject VFirstPageButton
            => ((VScrollBar.ScrollBarAccessibleObject)_vScrollBar.AccessibilityObject).FirstPageButtonAccessibleObject;

        private ScrollBar.ScrollBarLastPageButtonAccessibleObject VLastPageButton
            => ((VScrollBar.ScrollBarAccessibleObject)_vScrollBar.AccessibilityObject).LastPageButtonAccessibleObject;
    }
}
