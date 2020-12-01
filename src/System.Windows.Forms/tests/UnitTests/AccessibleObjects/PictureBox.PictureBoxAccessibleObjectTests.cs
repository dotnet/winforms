﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;
using static Interop;
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
            Assert.False(pictureBox.IsHandleCreated);
            var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);

            Assert.NotNull(pictureBoxAccessibleObject.Owner);
            Assert.False(pictureBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void PictureBoxAccessibleObject_Description_ReturnsExpected()
        {
            using var pictureBox = new PictureBox
            {
                AccessibleDescription = "TestDescription",
            };

            Assert.False(pictureBox.IsHandleCreated);
            var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);

            Assert.Equal("TestDescription", pictureBoxAccessibleObject.Description);
            Assert.False(pictureBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void PictureBoxAccessibleObject_Name_ReturnsExpected()
        {
            using var pictureBox = new PictureBox
            {
                AccessibleName = "TestName"
            };

            Assert.False(pictureBox.IsHandleCreated);
            var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);

            Assert.Equal("TestName", pictureBoxAccessibleObject.Name);
            Assert.False(pictureBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void PictureBoxAccessibleObject_CustomRole_ReturnsExpected()
        {
            using var pictureBox = new PictureBox
            {
                AccessibleRole = AccessibleRole.PushButton
            };

            Assert.False(pictureBox.IsHandleCreated);
            var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);

            Assert.Equal(AccessibleRole.PushButton, pictureBoxAccessibleObject.Role);
            Assert.False(pictureBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(true, AccessibleRole.Client)]
        [InlineData(false, AccessibleRole.None)]
        public void PictureBoxAccessibleObject_DefaultRole_ReturnsExpected(bool createControl, AccessibleRole accessibleRole)
        {
            using var pictureBox = new PictureBox();

            if (createControl)
            {
                pictureBox.CreateControl();
            }

            var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);
            Assert.Equal(accessibleRole, pictureBoxAccessibleObject.Role);
            Assert.Equal(createControl, pictureBox.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "TestName")]
        [InlineData((int)UIA.ControlTypePropertyId, UIA.PaneControlTypeId)] // If AccessibleRole is Default
        [InlineData((int)UIA.IsKeyboardFocusablePropertyId, true)]
        [InlineData((int)UIA.AutomationIdPropertyId, "PictureBox1")]
        public void PictureBoxAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using var pictureBox = new PictureBox
            {
                Name = "PictureBox1",
                AccessibleName = "TestName"
            };

            Assert.False(pictureBox.IsHandleCreated);
            var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);
            object value = pictureBoxAccessibleObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, value);
            Assert.False(pictureBox.IsHandleCreated);
        }

        [WinFormsFact]
        public void PictureBoxAccessibleObject_IsPatternSupported_Invoke_ReturnsTrue_ForLegacyIAccessiblePatternId()
        {
            using var pictureBox = new PictureBox();
            Assert.False(pictureBox.IsHandleCreated);
            var pictureBoxAccessibleObject = new PictureBox.PictureBoxAccessibleObject(pictureBox);

            Assert.True(pictureBoxAccessibleObject.IsPatternSupported(UIA.LegacyIAccessiblePatternId));
            Assert.False(pictureBox.IsHandleCreated);
        }

        public static IEnumerable<object[]> PictureBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
        {
            Array roles = Enum.GetValues(typeof(AccessibleRole));

            foreach (AccessibleRole role in roles)
            {
                if (role == AccessibleRole.Default)
                {
                    continue; // The test checks custom roles
                }

                yield return new object[] { role };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(PictureBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void PictureBoxAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using PictureBox pictureBox = new PictureBox();
            pictureBox.AccessibleRole = role;

            object actual = pictureBox.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            UiaCore.UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(pictureBox.IsHandleCreated);
        }
    }
}

