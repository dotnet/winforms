// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class Splitter_SplitterAccessibleObjectTests
    {
        [WinFormsFact]
        public void SplitterAccessibleObject_Ctor_NullControl_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("ownerControl", () => new Splitter.SplitterAccessibleObject(null));
        }

        [WinFormsFact]
        public void SplitterAccessibleObject_Ctor_Default()
        {
            using var splitter = new Splitter();
            var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

            Assert.NotNull(splitterAccessibleObject.Owner);
            Assert.Equal(AccessibleRole.Client, splitterAccessibleObject.Role);
        }

        [WinFormsFact]
        public void SplitterAccessibleObject_Property_returns_correct_value()
        {
            using var splitter = new Splitter
            {
                Name = "Splitter1",
                AccessibleName = "TestName",
                AccessibleDescription = "TestDescription",
                AccessibleRole = AccessibleRole.PushButton
            };

            var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);
            Assert.Equal("TestName", splitterAccessibleObject.Name);
            Assert.Equal("TestDescription", splitterAccessibleObject.Description);
            Assert.Equal(AccessibleRole.PushButton, splitterAccessibleObject.Role);
        }

        [WinFormsTheory]
        [InlineData(UIA.NamePropertyId, "TestName")]
        [InlineData(UIA.ControlTypePropertyId, UIA.PaneControlTypeId)]
        [InlineData(UIA.IsKeyboardFocusablePropertyId, true)]
        [InlineData(UIA.AutomationIdPropertyId, "Splitter1")]
        internal void SplitterAccessibleObject_GetPropertyValue_returns_correct_values(UIA propertyID, object expected)
        {
            using var splitter = new Splitter
            {
                Name = "Splitter1",
                AccessibleName = "TestName"
            };

            var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(splitter.IsHandleCreated);
            object value = splitterAccessibleObject.GetPropertyValue(propertyID);
            Assert.Equal(expected, value);

            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(splitter.IsHandleCreated);
        }

        [WinFormsFact]
        internal void SplitterAccessibleObject_IsPatternSupported_returns_correct_value()
        {
            using var splitter = new Splitter
            {
                Name = "Splitter1"
            };

            var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);
            Assert.True(splitterAccessibleObject.IsPatternSupported(UIA.LegacyIAccessiblePatternId));
        }
    }
}
