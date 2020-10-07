// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class FlowLayoutPanelAccessibilityObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void FlowLayoutPanelAccessibilityObject_Ctor_Default()
        {
            using FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.CreateControl();

            Assert.NotNull(flowLayoutPanel.AccessibilityObject);
            Assert.True(flowLayoutPanel.IsHandleCreated);
        }

        [WinFormsFact]
        public void FlowLayoutPanelAccessibilityObject_ControlType_IsPane_IfAccessibleRoleIsDefault()
        {
            using FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.CreateControl();
            // AccessibleRole is not set = Default

            object actual = flowLayoutPanel.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.PaneControlTypeId, actual);
            Assert.True(flowLayoutPanel.IsHandleCreated);
        }

        [WinFormsFact]
        public void FlowLayoutPanelAccessibilityObject_Role_IsClient_ByDefault()
        {
            using FlowLayoutPanel flowLayoutPanel = new FlowLayoutPanel();
            flowLayoutPanel.CreateControl();
            // AccessibleRole is not set = Default

            AccessibleRole actual = flowLayoutPanel.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Client, actual);
            Assert.True(flowLayoutPanel.IsHandleCreated);
        }
    }
}
