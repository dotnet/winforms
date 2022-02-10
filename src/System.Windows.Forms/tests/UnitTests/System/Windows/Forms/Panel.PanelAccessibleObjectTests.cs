// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;
using static Interop.UiaCore;

namespace System.Windows.Forms.Tests
{
    public class Panel_PanelAccessibleObjectTests
    {
        [WinFormsFact]
        public void PanelAccessibleObject_Ctor_NullControl_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("ownerControl", () => new Panel.PanelAccessibleObject(owner: null));
        }

        [WinFormsFact]
        public void PanelAccessibleObject_Ctor_Default()
        {
            using Panel panel = new();
            Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

            Assert.NotNull(panelAccessibleObject.Owner);
            Assert.False(panel.IsHandleCreated);
        }

        [WinFormsFact]
        public void PanelAccessibleObject_Description_ReturnsExpected()
        {
            var expected = "PanelTestDescription";

            using Panel panel = new()
            {
                AccessibleDescription = expected,
            };

            Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

            Assert.Equal(expected, panelAccessibleObject.Description);
            Assert.False(panel.IsHandleCreated);
        }

        [WinFormsFact]
        public void PanelAccessibleObject_Name_ReturnsExpected()
        {
            var expected = "PanelTestName";

            using Panel panel = new()
            {
                AccessibleName = expected
            };

            Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

            Assert.Equal(expected, panelAccessibleObject.Name);
            Assert.False(panel.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData((int)UIA.NamePropertyId, "PanelTestName")]
        [InlineData((int)UIA.AutomationIdPropertyId, "Panel1")]
        [InlineData((int)UIA.ControlTypePropertyId, UIA.PaneControlTypeId)] // If AccessibleRole is Default
        [InlineData((int)UIA.IsKeyboardFocusablePropertyId, false)]
        [InlineData((int)UIA.LegacyIAccessibleDefaultActionPropertyId, null)]
        public void PanelAccessibleObject_GetPropertyValue_Invoke_ReturnsExpected(int propertyID, object expected)
        {
            using Panel panel = new()
            {
                Name = "Panel1",
                AccessibleName = "PanelTestName"
            };

            Panel.PanelAccessibleObject panelAccessibleObject = new(panel);
            object actual = panelAccessibleObject.GetPropertyValue((UIA)propertyID);

            Assert.Equal(expected, actual);
            Assert.False(panel.IsHandleCreated);
        }

        public static IEnumerable<object[]> PanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(PanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void PanelAccessibleObject_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using Panel panel = new();
            panel.AccessibleRole = role;

            object actual = panel.AccessibilityObject.GetPropertyValue(UIA.ControlTypePropertyId);
            UIA expected = AccessibleRoleControlTypeMap.GetControlType(role);

            Assert.Equal(expected, actual);
            Assert.False(panel.IsHandleCreated);
        }

        [WinFormsFact]
        public void PanelAccessibleObject_IsPatternSupported_Invoke_ReturnsTrue_ForLegacyIAccessiblePatternId()
        {
            using Panel panel = new();
            Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

            Assert.True(panelAccessibleObject.IsPatternSupported(UIA.LegacyIAccessiblePatternId));
            Assert.False(panel.IsHandleCreated);
        }

        [WinFormsFact]
        public void PanelAccessibleObject_Bounds_ReturnsEmpty_IfControlIsNotCreated()
        {
            using Panel panel = new();
            Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

            Assert.Equal(Rectangle.Empty, panelAccessibleObject.Bounds);
            Assert.False(panel.IsHandleCreated);
        }

        [WinFormsFact]
        public void PanelAccessibleObject_Bounds_ReturnsExpected()
        {
            Rectangle expected = new(0, 0, 10, 10);

            using Panel panel = new()
            {
                Bounds = expected
            };

            panel.CreateControl();
            Assert.True(panel.IsHandleCreated);

            Panel.PanelAccessibleObject panelAccessibleObject = new(panel);
            Assert.Equal(expected, panel.RectangleToClient(panelAccessibleObject.Bounds));
        }

        [WinFormsFact]
        public void PanelAccessibleObject_FragmentRoot_ReturnsExpected()
        {
            using Panel panel = new();
            Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

            Assert.Equal(panelAccessibleObject, panelAccessibleObject.FragmentRoot);
            Assert.False(panel.IsHandleCreated);
        }

        [WinFormsFact]
        public void PanelAccessibleObject_GetChildCount_ReturnsMinusOne_IfHandleIsNotCreated()
        {
            using Panel panel = new();
            Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

            Assert.False(panel.IsHandleCreated);
            Assert.Equal(-1, panelAccessibleObject.GetChildCount());
        }

        [WinFormsFact]
        public void PanelAccessibleObject_GetChildCount_ReturnsZero_IfPanelHasNoControls()
        {
            using Panel panel = new();
            panel.CreateControl();
            Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

            Assert.True(panel.IsHandleCreated);
            Assert.Equal(0, panelAccessibleObject.GetChildCount());
        }

        [WinFormsFact]
        public void PanelAccessibleObject_GetChildCount_ReturnsExpected()
        {
            using Panel parentPanel = new();
            parentPanel.CreateControl();

            using Panel childPanel = new();
            childPanel.CreateControl();
            parentPanel.Controls.Add(childPanel);

            Panel.PanelAccessibleObject panelAccessibleObject = new(parentPanel);

            Assert.True(parentPanel.IsHandleCreated);
            Assert.True(childPanel.IsHandleCreated);
            Assert.Equal(1, panelAccessibleObject.GetChildCount());
        }

        [WinFormsFact]
        public void PanelAccessibleObject_GetChild_ReturnsNull_IfHandleIsNotCreated()
        {
            using Panel panel = new();
            Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

            Assert.False(panel.IsHandleCreated);
            Assert.Null(panelAccessibleObject.GetChild(0));
        }

        [WinFormsFact]
        public void PanelAccessibleObject_GetChild_ReturnsNull_IfPanelHasNoControls()
        {
            using Panel panel = new();
            panel.CreateControl();
            Panel.PanelAccessibleObject panelAccessibleObject = new(panel);

            Assert.True(panel.IsHandleCreated);
            Assert.Null(panelAccessibleObject.GetChild(0));
        }

        [WinFormsFact]
        public void PanelAccessibleObject_GetChild_ReturnsExpected()
        {
            using Panel parentPanel = new();
            parentPanel.CreateControl();

            using Panel childPanel = new();
            childPanel.CreateControl();
            parentPanel.Controls.Add(childPanel);

            Panel.PanelAccessibleObject panelAccessibleObject = new(parentPanel);

            Assert.True(parentPanel.IsHandleCreated);
            Assert.True(childPanel.IsHandleCreated);
            Assert.Equal(childPanel.AccessibilityObject, panelAccessibleObject.GetChild(0));
        }

        [WinFormsFact]
        public void PanelAccessibleObject_ChildrenButton_ReturnsExpected()
        {
            using Panel panel = new();
            panel.CreateControl();
            using Button buttonFirst = new();
            buttonFirst.CreateControl();
            panel.Controls.Add(buttonFirst);
            using Button buttonLast = new();
            buttonLast.CreateControl();
            panel.Controls.Add(buttonLast);

            Assert.Equal(buttonFirst.AccessibilityObject, panel.AccessibilityObject.Navigate(AccessibleNavigation.FirstChild));
            Assert.Equal(buttonLast.AccessibilityObject, panel.AccessibilityObject.Navigate(AccessibleNavigation.LastChild));
        }
    }
}
