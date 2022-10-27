// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.UpDownBase;
using static System.Windows.Forms.UpDownBase.UpDownButtons;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class UpDownBase_UpDownButtons_UpDownButtonsAccessibleObject_DirectionButtonAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsTheory]
        [InlineData(((int)UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId))]
        [InlineData(((int)UiaCore.UIA.IsInvokePatternAvailablePropertyId))]
        public void UpDownButtonsAccessibleObject_DirectionButtonAccessibleObject_GetPropertyValue_IsPatternSupported(int propertyId)
        {
            using SubUpDownBase upDownBase = new();
            using UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
            UpDownButtonsAccessibleObject accessibleObject = new(upDownButtons);
            // UpButton has 0 index
            AccessibleObject upButton = accessibleObject.GetChild(index: 0);
            bool actual = (bool)upButton.GetPropertyValue((UiaCore.UIA)propertyId);

            Assert.True(actual);
            Assert.False(upDownButtons.IsHandleCreated);
            Assert.False(upDownBase.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.LegacyIAccessibleRolePropertyId, AccessibleRole.PushButton)]
        [InlineData((int)UiaCore.UIA.LegacyIAccessibleStatePropertyId, AccessibleStates.None)]
        [InlineData((int)UiaCore.UIA.ValueValuePropertyId, null)]
        public void NumericUpDownAccessibleObject_DirectionButtonAccessibleObject_GetPropertyValue_ReturnsExpected(int property, object expected)
        {
            using SubUpDownBase upDownBase = new();
            using UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
            UpDownButtonsAccessibleObject accessibleObject = new(upDownButtons);
            // UpButton has 0 index
            AccessibleObject upButton = accessibleObject.GetChild(index: 0);
            object actual = upButton.GetPropertyValue((UiaCore.UIA)property);

            Assert.Equal(expected, actual);
            Assert.False(upDownBase.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void UpDownButtonsAccessibleObject_DirectionButtonAccessibleObject_FragmentNavigate_Parent_ReturnsExpected(int childIndex)
        {
            using SubUpDownBase upDownBase = new();
            using UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
            UpDownButtonsAccessibleObject accessibleObject = new(upDownButtons);

            // UpButton has 0 index, DownButton has 1 index
            AccessibleObject directionButton = accessibleObject.GetChild(childIndex);

            Assert.Equal(accessibleObject, directionButton.FragmentNavigate(UiaCore.NavigateDirection.Parent));
            Assert.False(upDownButtons.IsHandleCreated);
            Assert.False(upDownBase.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(0)]
        [InlineData(1)]
        public void UpDownButtonsAccessibleObject_DirectionButtonAccessibleObject_FragmentNavigate_Child_ReturnsNull(int childIndex)
        {
            using SubUpDownBase upDownBase = new();
            using UpDownButtons upDownButtons = upDownBase.UpDownButtonsInternal;
            UpDownButtonsAccessibleObject accessibleObject = new(upDownButtons);

            // UpButton has 0 index, DownButton has 1 index
            AccessibleObject directionButton = accessibleObject.GetChild(childIndex);

            Assert.Null(directionButton.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(directionButton.FragmentNavigate(UiaCore.NavigateDirection.LastChild));

            Assert.False(upDownButtons.IsHandleCreated);
            Assert.False(upDownBase.IsHandleCreated);
        }

        private class SubUpDownBase : UpDownBase
        {
            protected override void UpdateEditText() => throw new NotImplementedException();

            public override void UpButton() => throw new NotImplementedException();

            public override void DownButton() => throw new NotImplementedException();
        }
    }
}
