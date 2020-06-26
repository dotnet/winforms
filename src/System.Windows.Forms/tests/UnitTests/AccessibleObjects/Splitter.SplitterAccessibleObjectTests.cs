﻿// Licensed to the .NET Foundation under one or more agreements.
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
            Assert.False(splitter.IsHandleCreated);
            var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

            Assert.NotNull(splitterAccessibleObject.Owner);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(splitter.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitterAccessibleObject_Descrpition_ReturnsExpected()
        {
            using var splitter = new Splitter
            {
                AccessibleDescription = "TestDescription"
            };

            Assert.False(splitter.IsHandleCreated);
            var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

            Assert.Equal("TestDescription", splitterAccessibleObject.Description);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(splitter.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitterAccessibleObject_Name_ReturnsExpected()
        {
            using var splitter = new Splitter
            {
                AccessibleName = "TestName"
            };

            Assert.False(splitter.IsHandleCreated);
            var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

            Assert.Equal("TestName", splitterAccessibleObject.Name);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(splitter.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitterAccessibleObject_CustomRole_ReturnsExpected()
        {
            using var splitter = new Splitter
            {
                AccessibleRole = AccessibleRole.PushButton
            };

            Assert.False(splitter.IsHandleCreated);
            var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

            Assert.Equal(AccessibleRole.PushButton, splitterAccessibleObject.Role);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(splitter.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitterAccessibleObject_DefaultRole_ReturnsExpected()
        {
            using var splitter = new Splitter();
            Assert.False(splitter.IsHandleCreated);
            var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

            Assert.Equal(AccessibleRole.Client, splitterAccessibleObject.Role);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(splitter.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.ControlTypePropertyId, UIA.PaneControlTypeId)]
        [InlineData((int)UIA.IsKeyboardFocusablePropertyId, true)]
        [InlineData((int)UIA.AutomationIdPropertyId, "Splitter1")]
        public void SplitterAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using var splitter = new Splitter
            {
                Name = "Splitter1",
                AccessibleName = "TestName"
            };

            Assert.False(splitter.IsHandleCreated);
            var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);
            object value = splitterAccessibleObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, value);
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(splitter.IsHandleCreated);
        }

        [WinFormsFact]
        public void SplitterAccessibleObject_IsPatternSupported_Invoke_ReturnsTrue_ForLegacyIAccessiblePattern()
        {
            using var splitter = new Splitter
            {
                Name = "Splitter1"
            };
            Assert.False(splitter.IsHandleCreated);
            var splitterAccessibleObject = new Splitter.SplitterAccessibleObject(splitter);

            Assert.True(splitterAccessibleObject.IsPatternSupported(UIA.LegacyIAccessiblePatternId));
            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(splitter.IsHandleCreated);
        }
    }
}
