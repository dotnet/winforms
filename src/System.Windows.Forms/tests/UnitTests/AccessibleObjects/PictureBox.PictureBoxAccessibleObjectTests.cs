// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class PictureBox_PictureBoxAccessibleObjectTests
    {
        [WinFormsFact]
        public void PictureBoxAccessibleObject_Ctor_NullControl_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("ownerControl", () => new PictureBox.PictureBoxAccessibleObject(null));
        }

        [WinFormsFact]
        public void PictureBoxAccessibleObject_Ctor_Default()
        {
            using var pictureBox = new PictureBox();
            var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);

            Assert.NotNull(pictureBoxAccessibleObject.Owner);
            Assert.Equal(AccessibleRole.Client, pictureBoxAccessibleObject.Role);
        }

        [WinFormsFact]
        public void PictureBoxAccessibleObject_Property_returns_correct_value()
        {
            using var pictureBox = new PictureBox
            {
                Name = "PictureBox1",
                AccessibleName = "TestName",
                AccessibleDescription = "TestDescription",
                AccessibleRole = AccessibleRole.PushButton
            };

            var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);
            Assert.Equal("TestName", pictureBoxAccessibleObject.Name);
            Assert.Equal("TestDescription", pictureBoxAccessibleObject.Description);
            Assert.Equal(AccessibleRole.PushButton, pictureBoxAccessibleObject.Role);
        }

        [WinFormsTheory]
        [InlineData(UIA.NamePropertyId, "TestName")]
        [InlineData(UIA.ControlTypePropertyId, UIA.PaneControlTypeId)]
        [InlineData(UIA.IsKeyboardFocusablePropertyId, true)]
        [InlineData(UIA.AutomationIdPropertyId, "PictureBox1")]
        internal void PictureBoxAccessibleObject_GetPropertyValue_returns_correct_values(UIA propertyID, object expected)
        {
            using var pictureBox = new PictureBox
            {
                Name = "PictureBox1",
                AccessibleName = "TestName"
            };

            var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);

            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(pictureBox.IsHandleCreated);
            object value = pictureBoxAccessibleObject.GetPropertyValue(propertyID);
            Assert.Equal(expected, value);

            // TODO: ControlAccessibleObject shouldn't force handle creation, tracked in https://github.com/dotnet/winforms/issues/3062
            Assert.True(pictureBox.IsHandleCreated);
        }

        [WinFormsFact]
        internal void PictureBoxAccessibleObject_IsPatternSupported_returns_correct_value()
        {
            using var pictureBox = new PictureBox
            {
                Name = "PictureBox1"
            };

            var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);
            Assert.True(pictureBoxAccessibleObject.IsPatternSupported(UIA.LegacyIAccessiblePatternId));
        }
    }
}

