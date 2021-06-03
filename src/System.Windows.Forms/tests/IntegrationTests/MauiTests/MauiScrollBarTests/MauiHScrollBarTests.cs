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
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_LTR_Value_0(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(HFirstPageButton.Bounds.Width == 0 || HFirstPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle_LTR_Value_50(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(HFirstPageButton.Bounds.Width > 0 && HFirstPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle_LTR_Value_100(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(HFirstPageButton.Bounds.Width > 0 && HFirstPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle_RTL_Value_0(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(HFirstPageButton.Bounds.Width > 0 && HFirstPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnRectangle_RTL_Value_50(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(HFirstPageButton.Bounds.Width > 0 && HFirstPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_RTL_Value_100(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(HFirstPageButton.Bounds.Width == 0 || HFirstPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_LTR_MiniumEqualsMaximum(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(HFirstPageButton.Bounds.Width == 0 || HFirstPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_RTL_MiniumEqualsMaximum(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(HFirstPageButton.Bounds.Width == 0 || HFirstPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_State_ReturnInvisible_LTR_Value_0(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(HFirstPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_State_ReturnNone_LTR_Value_50(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(HFirstPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_State_ReturnNone_LTR_Value_100(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(HFirstPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_State_ReturnNone_RTL_Value_0(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(HFirstPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_State_ReturnNone_RTL_Value_50(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(HFirstPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_State_ReturnInvisible_RTL_Value_100(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(HFirstPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_State_ReturnInvisible_LTR_MiniumEqualsMaximum(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(HFirstPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarFirstPageButtonAccessibleObject_State_ReturnInvisible_RTL_MiniumEqualsMaximum(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(HFirstPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle_LTR_Value_0(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(HLastPageButton.Bounds.Width > 0 && HLastPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle_LTR_Value_50(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(HLastPageButton.Bounds.Width > 0 && HLastPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_LTR_Value_100(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(HLastPageButton.Bounds.Width == 0 || HLastPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_RTL_Value_0(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(HLastPageButton.Bounds.Width == 0 || HLastPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle_RTL_Value_50(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(HLastPageButton.Bounds.Width > 0 && HLastPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnRectangle_RTL_Value_100(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(HLastPageButton.Bounds.Width > 0 && HLastPageButton.Bounds.Height > 0, "'Bounds' property return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_LTR_MiniumEqualsMaximum(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(HLastPageButton.Bounds.Width == 0 || HLastPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_Bounds_ReturnEmptyRectangle_RTL_MiniumEqualsMaximum(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(HLastPageButton.Bounds.Width == 0 || HLastPageButton.Bounds.Height == 0, "'Bounds' property does not return empty rectangle");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_State_ReturnNone_LTR_Value_0(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(HLastPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_State_ReturnNone_LTR_Value_50(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(HLastPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_State_ReturnInvisible_LTR_Value_100(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(HLastPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_State_ReturnInvisible_RTL_Value_0(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 0);

            return new ScenarioResult(HLastPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_State_ReturnNone_RTL_Value_50(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 50);

            return new ScenarioResult(HLastPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_State_ReturnNone_RTL_Value_100(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 100, value: 100);

            return new ScenarioResult(HLastPageButton.State == AccessibleStates.None, "'State' property does not return 'None' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_State_ReturnInvisible_LTR_MiniumEqualsMaximum(TParams p)
        {
            SetHScrollBar(RightToLeft.No, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(HLastPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        [Scenario(true)]
        public ScenarioResult HScrollBarLastPageButtonAccessibleObject_State_ReturnInvisible_RTL_MiniumEqualsMaximum(TParams p)
        {
            SetHScrollBar(RightToLeft.Yes, minimum: 0, maximum: 0, value: 0);

            return new ScenarioResult(HLastPageButton.State == AccessibleStates.Invisible, "'State' property does not return 'Invisible' state");
        }

        private void SetHScrollBar(RightToLeft rightToLeft, int minimum, int maximum, int value)
        {
            _hScrollBar.RightToLeft = rightToLeft;
            _hScrollBar.Minimum = minimum;
            _hScrollBar.Maximum = maximum;
            _hScrollBar.Value = value;
            Application.DoEvents();
        }

        private ScrollBar.ScrollBarFirstPageButtonAccessibleObject HFirstPageButton
            => ((VScrollBar.ScrollBarAccessibleObject)_hScrollBar.AccessibilityObject).FirstPageButtonAccessibleObject;

        private ScrollBar.ScrollBarLastPageButtonAccessibleObject HLastPageButton
            => ((VScrollBar.ScrollBarAccessibleObject)_hScrollBar.AccessibilityObject).LastPageButtonAccessibleObject;
    }
}
