﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using Xunit;
using static System.Windows.Forms.ToolStripItem;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ToolStrip_ToolStripAccessibleObjectWrapperForItemsOnOverflowTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripAccessibleObjectWrapperForItemsOnOverflow_Ctor_Default()
        {
            using ToolStripButton toolStripItem = new ToolStripButton();

            Type type = typeof(ToolStrip).GetNestedType("ToolStripAccessibleObjectWrapperForItemsOnOverflow", BindingFlags.Instance | BindingFlags.NonPublic);
            ToolStripItemAccessibleObject accessibleObject = (ToolStripItemAccessibleObject)Activator.CreateInstance(type, toolStripItem);

            Assert.Equal(toolStripItem, accessibleObject.Owner);
        }

        [WinFormsFact]
        public void ToolStripAccessibleObjectWrapperForItemsOnOverflow_ControlType_IsPane_IfAccessibleRoleIsDefault()
        {
            using ToolStrip toolStrip = new ToolStrip();
            using ToolStripButton toolStripItem = new ToolStripButton();
            toolStrip.Items.Add(toolStripItem);
            toolStripItem.SetPlacement(ToolStripItemPlacement.Overflow);
            // AccessibleRole is not set = Default

            AccessibleObject accessibleObject = toolStrip.AccessibilityObject.GetChild(1);
            object actual = accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);

            Assert.Equal(UiaCore.UIA.ButtonControlTypeId, actual);
            Assert.False(toolStrip.IsHandleCreated);
        }

        [WinFormsFact]
        public void ToolStripAccessibleObjectWrapperForItemsOnOverflow_Role_IsPushButton_ByDefault()
        {
            using ToolStrip toolStrip = new ToolStrip();
            using ToolStripButton toolStripItem = new ToolStripButton();
            toolStrip.Items.Add(toolStripItem);
            toolStripItem.SetPlacement(ToolStripItemPlacement.Overflow);
            // AccessibleRole is not set = Default

            AccessibleObject accessibleObject = toolStrip.AccessibilityObject.GetChild(1);
            AccessibleRole actual = accessibleObject.Role;

            Assert.Equal(AccessibleRole.PushButton, actual);
            Assert.False(toolStrip.IsHandleCreated);
        }

        public static IEnumerable<object[]> ToolStripAccessibleObjectWrapperForItemsOnOverflow_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData()
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
        [MemberData(nameof(ToolStripAccessibleObjectWrapperForItemsOnOverflow_GetPropertyValue_ControlType_IsExpected_ForCustomRole_TestData))]
        public void ToolStripAccessibleObjectWrapperForItemsOnOverflow_GetPropertyValue_ControlType_IsExpected_ForCustomRole(AccessibleRole role)
        {
            using ToolStrip toolStrip = new ToolStrip();
            using ToolStripButton toolStripItem = new ToolStripButton();
            toolStrip.Items.Add(toolStripItem);
            toolStripItem.SetPlacement(ToolStripItemPlacement.Overflow);
            toolStripItem.AccessibleRole = role;

            AccessibleObject accessibleObject = toolStrip.AccessibilityObject.GetChild(1);
            AccessibleRole actual = accessibleObject.Role;

            Assert.Equal(role, actual);
            Assert.False(toolStrip.IsHandleCreated);
        }
    }
}
