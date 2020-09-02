// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Windows.Forms.Automation;
using Accessibility;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ControlControlAccessibleObject : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ControlAccessibleObject_Ctor_ControlWithoutHandle()
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.False(ownerControl.IsHandleCreated);

            Assert.True(accessibleObject.Bounds.X >= 0);
            Assert.True(accessibleObject.Bounds.Y >= 0);
            Assert.Equal(0, accessibleObject.Bounds.Width);
            Assert.Equal(0, accessibleObject.Bounds.Height);
            Assert.Equal(IntPtr.Zero, accessibleObject.HandleInternal);
            Assert.Null(accessibleObject.DefaultAction);
            Assert.Null(accessibleObject.Description);
            Assert.Null(accessibleObject.Help);
            Assert.Null(accessibleObject.KeyboardShortcut);
            Assert.Null(accessibleObject.Name);
            Assert.Same(ownerControl, accessibleObject.Owner);
            Assert.Null(accessibleObject.Parent);
            Assert.Equal(AccessibleRole.None, accessibleObject.Role);
            Assert.Equal(AccessibleStates.None, accessibleObject.State);
            Assert.Equal(string.Empty, accessibleObject.Value);
        }

        [WinFormsFact]
        public void ControlAccessibleObject_Ctor_ControlWithHandle()
        {
            using var ownerControl = new Control();
            Assert.NotEqual(IntPtr.Zero, ownerControl.Handle);
            int invalidatedCallCount = 0;
            ownerControl.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            ownerControl.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            ownerControl.HandleCreated += (sender, e) => createdCallCount++;

            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.True(ownerControl.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            Assert.True(accessibleObject.Bounds.X >= 0);
            Assert.True(accessibleObject.Bounds.Y >= 0);
            Assert.Equal(0, accessibleObject.Bounds.Width);
            Assert.Equal(0, accessibleObject.Bounds.Height);
            Assert.Null(accessibleObject.DefaultAction);
            Assert.Null(accessibleObject.Description);
            Assert.Equal(ownerControl.Handle, accessibleObject.Handle);
            Assert.Null(accessibleObject.Help);
            Assert.Null(accessibleObject.KeyboardShortcut);
            Assert.Null(accessibleObject.Name);
            Assert.Same(ownerControl, accessibleObject.Owner);
            Assert.NotNull(accessibleObject.Parent);
            Assert.Equal(AccessibleRole.Client, accessibleObject.Role);
            Assert.Equal(AccessibleStates.Focusable, accessibleObject.State);
            Assert.Null(accessibleObject.Value);
        }

        [WinFormsFact]
        public void ControlAccessibleObject_Ctor_NullControl_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("ownerControl", () => new Control.ControlAccessibleObject(null));
        }

        [WinFormsFact]
        public void ControlAccessibleObject_created_via_owner_ensure_handle_set_when_owner_created()
        {
            using var ownerControl = new Control();

            AccessibleObject accessibleObject = ownerControl.AccessibilityObject;
            Assert.IsType<Control.ControlAccessibleObject>(accessibleObject);

            Control.ControlAccessibleObject controlAccessibleObject = (Control.ControlAccessibleObject)accessibleObject;
            Assert.False(ownerControl.IsHandleCreated);
            Assert.Equal(IntPtr.Zero, controlAccessibleObject.HandleInternal);

            // force the owner contrl to create its handle
            ownerControl.CreateControl();

            Assert.True(ownerControl.IsHandleCreated);
            Assert.Equal(ownerControl.Handle, controlAccessibleObject.Handle);
        }

        [WinFormsFact]
        public void ControlAccessibleObject_created_via_owner_ensure_handle_reset_when_owner_destroyed()
        {
            using var ownerControl = new Control();
            ownerControl.CreateControl();

            AccessibleObject accessibleObject = ownerControl.AccessibilityObject;
            Assert.IsType<Control.ControlAccessibleObject>(accessibleObject);

            Control.ControlAccessibleObject controlAccessibleObject = (Control.ControlAccessibleObject)accessibleObject;
            Assert.True(ownerControl.IsHandleCreated);
            Assert.Equal(ownerControl.Handle, controlAccessibleObject.Handle);

            ownerControl.Dispose();

            Assert.False(ownerControl.IsHandleCreated);
            Assert.Equal(IntPtr.Zero, controlAccessibleObject.Handle);
        }

        [WinFormsFact]
        public void ControlAccessibleObject_created_detached_owner_ensure_handle_set_when_owner_created()
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.False(ownerControl.IsHandleCreated);
            Assert.Equal(IntPtr.Zero, accessibleObject.HandleInternal);

            // force the owner contrl to create its handle
            ownerControl.CreateControl();

            Assert.True(ownerControl.IsHandleCreated);
            Assert.Equal(ownerControl.Handle, accessibleObject.Handle);
        }

        [WinFormsFact]
        public void ControlAccessibleObject_created_detached_owner_ensure_handle_reset_when_owner_destroyed()
        {
            using var ownerControl = new Control();
            var controlAccessibleObject = new Control.ControlAccessibleObject(ownerControl);
            ownerControl.CreateControl();

            Assert.True(ownerControl.IsHandleCreated);
            Assert.Equal(ownerControl.Handle, controlAccessibleObject.Handle);

            ownerControl.Dispose();

            Assert.False(ownerControl.IsHandleCreated);

            // NB: Detached object, so we don't get notifications
            Assert.NotEqual(IntPtr.Zero, controlAccessibleObject.Handle);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ControlAccessibleObject_DefaultAction_GetWithAccessibleDefaultActionDescription_ReturnsExpected(string accessibleDefaultActionDescription)
        {
            using var ownerControl = new Control
            {
                AccessibleDefaultActionDescription = accessibleDefaultActionDescription
            };
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(accessibleDefaultActionDescription, accessibleObject.DefaultAction);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ControlAccessibleObject_Description_GetWithAccessibleDescription_ReturnsExpected(string accessibleDescription)
        {
            using var ownerControl = new Control
            {
                AccessibleDescription = accessibleDescription
            };
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(accessibleDescription, accessibleObject.Description);
        }

        public static IEnumerable<object[]> Handle_Set_TestData()
        {
            yield return new object[] { IntPtr.Zero };
            yield return new object[] { (IntPtr)(-1) };
            yield return new object[] { (IntPtr)1 };
            yield return new object[] { (IntPtr)250 };
            yield return new object[] { new Control().Handle };
        }

        [WinFormsTheory]
        [MemberData(nameof(Handle_Set_TestData))]
        public void ControlAccessibleObject_Handle_Set_Success(IntPtr value)
        {
            using var ownerControl = new Control();
            ownerControl.CreateControl();
            Assert.True(ownerControl.IsHandleCreated);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.True(ownerControl.IsHandleCreated);

            // Set empty.
            accessibleObject.Handle = value;
            Assert.Equal(value, accessibleObject.Handle);

            // Set same.
            accessibleObject.Handle = value;
            Assert.Equal(value, accessibleObject.Handle);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ControlAccessibleObject_Help_GetWithQueryAccessibilityHelpEvent_Success(string result)
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);

            int callCount = 0;
            void handler(object sender, QueryAccessibilityHelpEventArgs e)
            {
                Assert.Same(ownerControl, sender);
                Assert.Null(e.HelpKeyword);
                Assert.Null(e.HelpNamespace);
                Assert.Null(e.HelpString);

                e.HelpString = result;
                callCount++;
            };
            ownerControl.QueryAccessibilityHelp += handler;

            // Get with handler.
            Assert.Equal(result, accessibleObject.Help);
            Assert.Equal(1, callCount);

            // Get again.
            Assert.Equal(result, accessibleObject.Help);
            Assert.Equal(2, callCount);

            // Remove handler.
            ownerControl.QueryAccessibilityHelp -= handler;
            Assert.Null(accessibleObject.Help);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> KeyboardShortcut_Get_TestData()
        {
            yield return new object[] { true, null, null };
            yield return new object[] { true, string.Empty, null };
            yield return new object[] { true, "Text", null };
            yield return new object[] { false, null, null };
            yield return new object[] { false, string.Empty, null };
            yield return new object[] { false, "Text", null };

            // With mnemonic.
            yield return new object[] { true, "&", null };
            yield return new object[] { true, "&&a", null };
            yield return new object[] { true, "a&", null };
            yield return new object[] { true, "a&b", "Alt+b" };
            yield return new object[] { true, "a&bc", "Alt+b" };
            yield return new object[] { false, "&", null };
            yield return new object[] { false, "&&a", null };
            yield return new object[] { false, "a&", null };
            yield return new object[] { false, "a&b", null };
            yield return new object[] { false, "a&bc", null };
        }

        [WinFormsTheory]
        [MemberData(nameof(KeyboardShortcut_Get_TestData))]
        public void ControlAccessibleObject_KeyboardShortcut_Get_ReturnsExpected(bool useTextForAccessibility, string text, string expected)
        {
            using var ownerControl = new SubControl
            {
                Text = text
            };
            ownerControl.SetStyle(ControlStyles.UseTextForAccessibility, useTextForAccessibility);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(expected, accessibleObject.KeyboardShortcut);
        }

        [WinFormsTheory]
        [MemberData(nameof(KeyboardShortcut_Get_TestData))]
        public void ControlAccessibleObject_KeyboardShortcut_GetWithNonContainerControlParent_IgnoresParent(bool useTextForAccessibility, string text, string expected)
        {
            using var parent = new Control();
            using var ownerControl = new SubControl
            {
                Parent = parent,
                Text = text
            };
            ownerControl.SetStyle(ControlStyles.UseTextForAccessibility, useTextForAccessibility);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(expected, accessibleObject.KeyboardShortcut);
        }

        [WinFormsTheory]
        [MemberData(nameof(KeyboardShortcut_Get_TestData))]
        public void ControlAccessibleObject_KeyboardShortcut_GetWithContainerControlParentWithoutPreviousLabel_IgnoresParent(bool useTextForAccessibility, string text, string expected)
        {
            using var parent = new ContainerControl();
            using var ownerControl = new SubControl
            {
                Parent = parent,
                Text = text
            };
            ownerControl.SetStyle(ControlStyles.UseTextForAccessibility, useTextForAccessibility);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(expected, accessibleObject.KeyboardShortcut);
        }

        public static IEnumerable<object[]> KeyboardShortcut_GetWithPreviousLabel_TestData()
        {
            yield return new object[] { true, null, null, null };
            yield return new object[] { true, null, string.Empty, null };
            yield return new object[] { true, null, "LabelText", null };
            yield return new object[] { true, string.Empty, null, null };
            yield return new object[] { true, string.Empty, string.Empty, null };
            yield return new object[] { true, string.Empty, "LabelText", null };
            yield return new object[] { true, "Text", null, null };
            yield return new object[] { true, "Text", string.Empty, null };
            yield return new object[] { true, "Text", "LabelText", null };
            yield return new object[] { false, null, null, null };
            yield return new object[] { false, null, string.Empty, null };
            yield return new object[] { false, null, "LabelText", null };
            yield return new object[] { false, string.Empty, null, null };
            yield return new object[] { false, string.Empty, string.Empty, null };
            yield return new object[] { false, string.Empty, "LabelText", null };
            yield return new object[] { false, "Text", null, null };
            yield return new object[] { false, "Text", string.Empty, null };
            yield return new object[] { false, "Text", "LabelText", null };

            // With mnemonic.
            yield return new object[] { true, "&", null, null };
            yield return new object[] { true, "&&a", null, null };
            yield return new object[] { true, "a&", null, null };
            yield return new object[] { true, "a&b", null, "Alt+b" };
            yield return new object[] { true, "a&bc", null, "Alt+b" };
            yield return new object[] { true, null, "&", null };
            yield return new object[] { true, null, "&&a", null };
            yield return new object[] { true, null, "a&", null };
            yield return new object[] { true, null, "a&b", "Alt+b" };
            yield return new object[] { true, null, "a&bc", "Alt+b" };

            yield return new object[] { false, "&", null, null };
            yield return new object[] { false, "&&a", null, null };
            yield return new object[] { false, "a&", null, null };
            yield return new object[] { false, "a&b", null, null };
            yield return new object[] { false, "a&bc", null, null };
            yield return new object[] { false, null, "&", null };
            yield return new object[] { false, null, "&&a", null };
            yield return new object[] { false, null, "a&", null };
            yield return new object[] { false, null, "a&b", "Alt+b" };
            yield return new object[] { false, null, "a&bc", "Alt+b" };
        }

        [WinFormsTheory]
        [MemberData(nameof(KeyboardShortcut_GetWithPreviousLabel_TestData))]
        public void ControlAccessibleObject_KeyboardShortcut_GetWithContainerControlParentWithPreviousLabel_ReturnsExpected(bool useTextForAccessibility, string text, string labelText, string expected)
        {
            using var parent = new ContainerControl();
            using var previousLabel = new Label
            {
                Parent = parent,
                Text = labelText
            };
            using var previousControl = new Control
            {
                Parent = parent,
                Visible = false
            };
            using var ownerControl = new SubControl
            {
                Parent = parent,
                Text = text
            };
            ownerControl.SetStyle(ControlStyles.UseTextForAccessibility, useTextForAccessibility);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(expected, accessibleObject.KeyboardShortcut);
        }

        [WinFormsTheory]
        [MemberData(nameof(KeyboardShortcut_GetWithPreviousLabel_TestData))]
        public void ControlAccessibleObject_KeyboardShortcut_GetWithPreviousNotTabStopAndPreviousLabel_IgnoresPreviousLabel(bool useTextForAccessibility, string text, string labelText, string expected)
        {
            using var parent = new ContainerControl();
            using var previousLabel = new Label
            {
                Parent = parent,
                Text = labelText
            };
            using var previousControl = new Control
            {
                Parent = parent,
                TabStop = false
            };
            using var ownerControl = new SubControl
            {
                Parent = parent,
                Text = text
            };
            ownerControl.SetStyle(ControlStyles.UseTextForAccessibility, useTextForAccessibility);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(expected, accessibleObject.KeyboardShortcut);
        }

        [WinFormsTheory]
        [MemberData(nameof(KeyboardShortcut_Get_TestData))]
        public void ControlAccessibleObject_KeyboardShortcut_GetWithPreviousTabStopVisibleAndPreviousLabel_IgnoresPreviousLabel(bool useTextForAccessibility, string text, string expected)
        {
            using var parent = new ContainerControl();
            using var previousLabel = new Label
            {
                Parent = parent,
                Text = "LabelText"
            };
            using var previousControl = new Control
            {
                Parent = parent
            };
            using var ownerControl = new SubControl
            {
                Parent = parent,
                Text = text
            };
            ownerControl.SetStyle(ControlStyles.UseTextForAccessibility, useTextForAccessibility);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(expected, accessibleObject.KeyboardShortcut);
        }

        public static IEnumerable<object[]> Name_Get_TestData()
        {
            yield return new object[] { true, null, null, null };
            yield return new object[] { true, null, string.Empty, null };
            yield return new object[] { true, null, "text", "text" };
            yield return new object[] { true, string.Empty, null, string.Empty };
            yield return new object[] { true, string.Empty, string.Empty, string.Empty };
            yield return new object[] { true, string.Empty, "text", string.Empty };
            yield return new object[] { true, "name", null, "name" };
            yield return new object[] { true, "name", string.Empty, "name" };
            yield return new object[] { true, "name", "text", "name" };

            yield return new object[] { false, null, null, null };
            yield return new object[] { false, null, string.Empty, null };
            yield return new object[] { false, null, "text", null };
            yield return new object[] { false, string.Empty, null, string.Empty };
            yield return new object[] { false, string.Empty, string.Empty, string.Empty };
            yield return new object[] { false, string.Empty, "text", string.Empty };
            yield return new object[] { false, "name", null, "name" };
            yield return new object[] { false, "name", string.Empty, "name" };
            yield return new object[] { false, "name", "text", "name" };

            // With mnemonic.
            yield return new object[] { true, "Name1&Name2", "text", "Name1&Name2" };
            yield return new object[] { true, null, "&", string.Empty };
            yield return new object[] { true, null, "&&Name", "&Name" };
            yield return new object[] { true, null, "Name1&Name2", "Name1Name2" };
            yield return new object[] { true, null, "Name1&Name2&Name3", "Name1Name2Name3" };
            yield return new object[] { true, null, "Name1&Name2&", "Name1Name2" };
            yield return new object[] { false, "Name1&Name2", "text", "Name1&Name2" };
            yield return new object[] { false, null, "&", null };
            yield return new object[] { false, null, "&&Name", null };
            yield return new object[] { false, null, "Name1&Name2", null };
            yield return new object[] { false, null, "Name1&Name2&Name3", null };
            yield return new object[] { false, null, "Name1&Name2&", null };
        }

        [WinFormsTheory]
        [MemberData(nameof(Name_Get_TestData))]
        public void ControlAccessibleObject_Name_Get_ReturnsExpected(bool useTextForAccessibility, string accessibleName, string text, string expected)
        {
            using var ownerControl = new SubControl
            {
                AccessibleName = accessibleName,
                Text = text
            };
            ownerControl.SetStyle(ControlStyles.UseTextForAccessibility, useTextForAccessibility);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(expected, accessibleObject.Name);
        }

        [WinFormsTheory]
        [MemberData(nameof(Name_Get_TestData))]
        public void ControlAccessibleObject_Name_GetWithNonContainerControlParent_IgnoresParent(bool useTextForAccessibility, string accessibleName, string text, string expected)
        {
            using var parent = new Control();
            using var ownerControl = new SubControl
            {
                Parent = parent,
                AccessibleName = accessibleName,
                Text = text
            };
            ownerControl.SetStyle(ControlStyles.UseTextForAccessibility, useTextForAccessibility);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(expected, accessibleObject.Name);
        }

        [WinFormsTheory]
        [MemberData(nameof(Name_Get_TestData))]
        public void ControlAccessibleObject_Name_GetWithContainerControlParentEmpty_IgnoresParent(bool useTextForAccessibility, string accessibleName, string text, string expected)
        {
            using var parent = new ContainerControl();
            using var ownerControl = new SubControl
            {
                Parent = parent,
                AccessibleName = accessibleName,
                Text = text
            };
            ownerControl.SetStyle(ControlStyles.UseTextForAccessibility, useTextForAccessibility);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(expected, accessibleObject.Name);
        }

        public static IEnumerable<object[]> Name_GetWithPreviousLabel_TestData()
        {
            yield return new object[] { true, null, null, null, null };
            yield return new object[] { true, null, null, string.Empty, null };
            yield return new object[] { true, null, null, "LabelText", "LabelText" };
            yield return new object[] { true, null, string.Empty, null, null };
            yield return new object[] { true, null, string.Empty, string.Empty, null };
            yield return new object[] { true, null, string.Empty, "LabelText", "LabelText" };
            yield return new object[] { true, null, "text", null, "text" };
            yield return new object[] { true, null, "text", string.Empty, "text" };
            yield return new object[] { true, null, "text", "LabelText", "text" };
            yield return new object[] { true, string.Empty, null, null, string.Empty };
            yield return new object[] { true, string.Empty, null, string.Empty, string.Empty };
            yield return new object[] { true, string.Empty, null, "LabelText", string.Empty };
            yield return new object[] { true, string.Empty, string.Empty, null, string.Empty };
            yield return new object[] { true, string.Empty, string.Empty, string.Empty, string.Empty };
            yield return new object[] { true, string.Empty, string.Empty, "LabelText", string.Empty };
            yield return new object[] { true, string.Empty, "text", null, string.Empty };
            yield return new object[] { true, string.Empty, "text", string.Empty, string.Empty };
            yield return new object[] { true, string.Empty, "text", "LabelText", string.Empty };
            yield return new object[] { true, "name", null, null, "name" };
            yield return new object[] { true, "name", null, string.Empty, "name" };
            yield return new object[] { true, "name", null, "LabelText", "name" };
            yield return new object[] { true, "name", string.Empty, null, "name" };
            yield return new object[] { true, "name", string.Empty, string.Empty, "name" };
            yield return new object[] { true, "name", string.Empty, "LabelText", "name" };
            yield return new object[] { true, "name", "text", null, "name" };
            yield return new object[] { true, "name", "text", string.Empty, "name" };
            yield return new object[] { true, "name", "text", "LabelText", "name" };

            yield return new object[] { false, null, null, null, null };
            yield return new object[] { false, null, null, string.Empty, null };
            yield return new object[] { false, null, null, "LabelText", "LabelText" };
            yield return new object[] { false, null, string.Empty, null, null };
            yield return new object[] { false, null, string.Empty, string.Empty, null };
            yield return new object[] { false, null, string.Empty, "LabelText", "LabelText" };
            yield return new object[] { false, null, "text", null, null };
            yield return new object[] { false, null, "text", string.Empty, null };
            yield return new object[] { false, null, "text", "LabelText", "LabelText" };
            yield return new object[] { false, string.Empty, null, null, string.Empty };
            yield return new object[] { false, string.Empty, null, string.Empty, string.Empty };
            yield return new object[] { false, string.Empty, null, "LabelText", string.Empty };
            yield return new object[] { false, string.Empty, string.Empty, null, string.Empty };
            yield return new object[] { false, string.Empty, string.Empty, string.Empty, string.Empty };
            yield return new object[] { false, string.Empty, string.Empty, "LabelText", string.Empty };
            yield return new object[] { false, string.Empty, "text", null, string.Empty };
            yield return new object[] { false, string.Empty, "text", string.Empty, string.Empty };
            yield return new object[] { false, string.Empty, "text", "LabelText", string.Empty };
            yield return new object[] { false, "name", null, null, "name" };
            yield return new object[] { false, "name", null, string.Empty, "name" };
            yield return new object[] { false, "name", null, "LabelText", "name" };
            yield return new object[] { false, "name", string.Empty, null, "name" };
            yield return new object[] { false, "name", string.Empty, string.Empty, "name" };
            yield return new object[] { false, "name", string.Empty, "LabelText", "name" };
            yield return new object[] { false, "name", "text", null, "name" };
            yield return new object[] { false, "name", "text", string.Empty, "name" };
            yield return new object[] { false, "name", "text", "LabelText", "name" };

            // With mnemonic.
            yield return new object[] { true, "Name1&Name2", "text", null, "Name1&Name2" };
            yield return new object[] { true, null, "&", null, string.Empty };
            yield return new object[] { true, null, "&&Name", null, "&Name" };
            yield return new object[] { true, null, "Name1&Name2", null, "Name1Name2" };
            yield return new object[] { true, null, "Name1&Name2&Name3", null, "Name1Name2Name3" };
            yield return new object[] { true, null, "Name1&Name2&", null, "Name1Name2" };
            yield return new object[] { true, null, null, "&", string.Empty };
            yield return new object[] { true, null, null, "&&Name", "&Name" };
            yield return new object[] { true, null, null, "Name1&Name2", "Name1Name2" };
            yield return new object[] { true, null, null, "Name1&Name2&Name3", "Name1Name2Name3" };
            yield return new object[] { true, null, null, "Name1&Name2&", "Name1Name2" };
            yield return new object[] { false, "Name1&Name2", "text", null, "Name1&Name2" };
            yield return new object[] { false, null, "&", null, null };
            yield return new object[] { false, null, "&&Name", null, null };
            yield return new object[] { false, null, "Name1&Name2", null, null };
            yield return new object[] { false, null, "Name1&Name2&Name3", null, null };
            yield return new object[] { false, null, "Name1&Name2&", null, null };
            yield return new object[] { false, null, null, "&", string.Empty };
            yield return new object[] { false, null, null, "&&Name", "&Name" };
            yield return new object[] { false, null, null, "Name1&Name2", "Name1Name2" };
            yield return new object[] { false, null, null, "Name1&Name2&Name3", "Name1Name2Name3" };
            yield return new object[] { false, null, null, "Name1&Name2&", "Name1Name2" };
        }

        [WinFormsTheory]
        [MemberData(nameof(Name_GetWithPreviousLabel_TestData))]
        public void ControlAccessibleObject_Name_GetWithContainerControlParentWithPreviousLabel_ReturnsExpected(bool useTextForAccessibility, string accessibleName, string text, string labelText, string expected)
        {
            using var parent = new ContainerControl();
            using var previousLabel = new Label
            {
                Parent = parent,
                Text = labelText
            };
            using var previousControl = new Control
            {
                Parent = parent,
                Visible = false
            };
            using var ownerControl = new SubControl
            {
                Parent = parent,
                AccessibleName = accessibleName,
                Text = text
            };
            ownerControl.SetStyle(ControlStyles.UseTextForAccessibility, useTextForAccessibility);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(expected, accessibleObject.Name);
        }

        [WinFormsTheory]
        [MemberData(nameof(Name_GetWithPreviousLabel_TestData))]
        public void ControlAccessibleObject_Name_GetWithPreviousNotTabStopAndPreviousLabel_ReturnsExpected(bool useTextForAccessibility, string accessibleName, string text, string labelText, string expected)
        {
            using var parent = new ContainerControl();
            using var previousLabel = new Label
            {
                Parent = parent,
                Text = labelText
            };
            using var previousControl = new Control
            {
                Parent = parent,
                TabStop = false
            };
            using var ownerControl = new SubControl
            {
                Parent = parent,
                AccessibleName = accessibleName,
                Text = text
            };
            ownerControl.SetStyle(ControlStyles.UseTextForAccessibility, useTextForAccessibility);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(expected, accessibleObject.Name);
        }

        [WinFormsTheory]
        [MemberData(nameof(Name_Get_TestData))]
        public void ControlAccessibleObject_Name_GetWithPreviousTabStopVisibleAndPreviousLabel_ReturnsExpected(bool useTextForAccessibility, string accessibleName, string text, string expected)
        {
            using var parent = new ContainerControl();
            using var previousLabel = new Label
            {
                Parent = parent,
                Text = "LabelText"
            };
            using var previousControl = new Control
            {
                Parent = parent
            };
            using var ownerControl = new SubControl
            {
                Parent = parent,
                AccessibleName = accessibleName,
                Text = text
            };
            ownerControl.SetStyle(ControlStyles.UseTextForAccessibility, useTextForAccessibility);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(expected, accessibleObject.Name);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ControlAccessibleObject_Name_Set_GetReturnsExpected(string value)
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);

            accessibleObject.Name = value;
            Assert.Equal(value, accessibleObject.Name);
            Assert.Empty(ownerControl.Name);
            Assert.Equal(value, ownerControl.AccessibleName);

            // Set same.
            accessibleObject.Name = value;
            Assert.Equal(value, accessibleObject.Name);
            Assert.Empty(ownerControl.Name);
            Assert.Equal(value, ownerControl.AccessibleName);
        }

        public static IEnumerable<object[]> Role_Get_TestData()
        {
            yield return new object[] { AccessibleRole.None };
            yield return new object[] { AccessibleRole.TitleBar };
            yield return new object[] { AccessibleRole.MenuBar };
            yield return new object[] { AccessibleRole.ScrollBar };
            yield return new object[] { AccessibleRole.Grip };
            yield return new object[] { AccessibleRole.Sound };
            yield return new object[] { AccessibleRole.Cursor };
            yield return new object[] { AccessibleRole.Caret };
            yield return new object[] { AccessibleRole.Alert };
            yield return new object[] { AccessibleRole.Window };
            yield return new object[] { AccessibleRole.Client };
            yield return new object[] { AccessibleRole.MenuPopup };
            yield return new object[] { AccessibleRole.MenuItem };
            yield return new object[] { AccessibleRole.ToolTip };
            yield return new object[] { AccessibleRole.Application };
            yield return new object[] { AccessibleRole.Document };
            yield return new object[] { AccessibleRole.Pane };
            yield return new object[] { AccessibleRole.Chart };
            yield return new object[] { AccessibleRole.Dialog };
            yield return new object[] { AccessibleRole.Border };
            yield return new object[] { AccessibleRole.Grouping };
            yield return new object[] { AccessibleRole.Separator };
            yield return new object[] { AccessibleRole.ToolBar };
            yield return new object[] { AccessibleRole.StatusBar };
            yield return new object[] { AccessibleRole.Table };
            yield return new object[] { AccessibleRole.ColumnHeader };
            yield return new object[] { AccessibleRole.RowHeader };
            yield return new object[] { AccessibleRole.Column };
            yield return new object[] { AccessibleRole.Row };
            yield return new object[] { AccessibleRole.Cell };
            yield return new object[] { AccessibleRole.Link };
            yield return new object[] { AccessibleRole.HelpBalloon };
            yield return new object[] { AccessibleRole.Character };
            yield return new object[] { AccessibleRole.List };
            yield return new object[] { AccessibleRole.ListItem };
            yield return new object[] { AccessibleRole.Outline };
            yield return new object[] { AccessibleRole.OutlineItem };
            yield return new object[] { AccessibleRole.PageTab };
            yield return new object[] { AccessibleRole.PropertyPage };
            yield return new object[] { AccessibleRole.Indicator };
            yield return new object[] { AccessibleRole.Graphic };
            yield return new object[] { AccessibleRole.StaticText };
            yield return new object[] { AccessibleRole.Text };
            yield return new object[] { AccessibleRole.PushButton };
            yield return new object[] { AccessibleRole.CheckButton };
            yield return new object[] { AccessibleRole.RadioButton };
            yield return new object[] { AccessibleRole.ComboBox };
            yield return new object[] { AccessibleRole.DropList };
            yield return new object[] { AccessibleRole.ProgressBar };
            yield return new object[] { AccessibleRole.Dial };
            yield return new object[] { AccessibleRole.HotkeyField };
            yield return new object[] { AccessibleRole.Slider };
            yield return new object[] { AccessibleRole.SpinButton };
            yield return new object[] { AccessibleRole.Diagram };
            yield return new object[] { AccessibleRole.Animation };
            yield return new object[] { AccessibleRole.Equation };
            yield return new object[] { AccessibleRole.ButtonDropDown };
            yield return new object[] { AccessibleRole.ButtonMenu };
            yield return new object[] { AccessibleRole.ButtonDropDownGrid };
            yield return new object[] { AccessibleRole.WhiteSpace };
            yield return new object[] { AccessibleRole.PageTabList };
            yield return new object[] { AccessibleRole.Clock };
            yield return new object[] { AccessibleRole.SplitButton };
            yield return new object[] { AccessibleRole.IpAddress };
            yield return new object[] { AccessibleRole.OutlineButton };
        }

        [WinFormsTheory]
        [MemberData(nameof(Role_Get_TestData))]
        public void ControlAccessibleObject_Role_GetWithAccessibleRole_ReturnsExpected(AccessibleRole accessibleRole)
        {
            using var ownerControl = new Control
            {
                AccessibleRole = accessibleRole
            };
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(accessibleRole, accessibleObject.Role);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ControlAccessibleObject_Value_Set_GetReturnsNull_IfHandleIsCreated(string value)
        {
            using var ownerControl = new Control();
            ownerControl.CreateControl();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);

            accessibleObject.Value = value;
            Assert.Null(accessibleObject.Value);

            // Set same.
            accessibleObject.Value = value;
            Assert.Null(accessibleObject.Value);
            Assert.True(ownerControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ControlAccessibleObject_Value_Set_GetReturnsNull_IfHandleIsNotCreated(string value)
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);

            accessibleObject.Value = value;
            Assert.Equal(string.Empty, accessibleObject.Value);

            // Set same.
            accessibleObject.Value = value;
            Assert.Equal(string.Empty, accessibleObject.Value);
            Assert.False(ownerControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void ControlAccessibleObject_DoDefaultAction_InvokeDefault_Nop()
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            accessibleObject.DoDefaultAction();
        }

        [WinFormsFact]
        public void ControlAccessibleObject_GetChildCount_InvokeDefault_ReturnsMinusOne()
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(-1, accessibleObject.GetChildCount());
        }

        [WinFormsFact]
        public void ControlAccessibleObject_GetChildCount_InvokeWithChildren_ReturnsMinusOne()
        {
            using var child = new Control();
            using var ownerControl = new Control();
            ownerControl.Controls.Add(child);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(-1, accessibleObject.GetChildCount());
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        public void ControlAccessibleObject_GetChild_InvokeDefault_ReturnsNull(int index)
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Null(accessibleObject.GetChild(index));
        }

        [WinFormsFact]
        public void ControlAccessibleObject_GetFocused_InvokeDefault_ReturnsNull()
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Null(accessibleObject.GetFocused());
        }

        [WinFormsFact]
        public void ControlAccessibleObject_GetHelpTopic_InvokeDefault_ReturnsMinusOne()
        {
            var accessibleObject = new AccessibleObject();
            Assert.Equal(-1, accessibleObject.GetHelpTopic(out string fileName));
            Assert.Null(fileName);
        }

        [WinFormsTheory]
        [InlineData(null, null, 0, true, 0)]
        [InlineData("", "", 0, true, 0)]
        [InlineData("HelpNamespace", "invalid", 0, true, 0)]
        [InlineData("HelpNamespace", "1", 1, true, 0)]
        [InlineData(null, null, 0, false, -1)]
        [InlineData("", "", 0, false, -1)]
        [InlineData("HelpNamespace", "invalid", 0, false, -1)]
        [InlineData("HelpNamespace", "1", 1, false, -1)]
        public void ControlAccessibleObject_GetHelpTopic_InvokeWithQueryAccessibilityHelpEvent_ReturnsExpected(
            string helpNamespace,
            string helpKeyword,
            int expectedResult,
            bool createControl,
            int exectedResultWithoutHandler)
        {
            using var ownerControl = new Control();
            if (createControl)
            {
                ownerControl.CreateControl();
            }

            Assert.Equal(createControl, ownerControl.IsHandleCreated);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(createControl, ownerControl.IsHandleCreated);

            int callCount = 0;
            void handler(object sender, QueryAccessibilityHelpEventArgs e)
            {
                Assert.Same(ownerControl, sender);
                Assert.Null(e.HelpKeyword);
                Assert.Null(e.HelpNamespace);
                Assert.Null(e.HelpString);

                e.HelpNamespace = helpNamespace;
                e.HelpKeyword = helpKeyword;
                callCount++;
            };
            ownerControl.QueryAccessibilityHelp += handler;

            // Get with handler.
            Assert.Equal(expectedResult, accessibleObject.GetHelpTopic(out string fileName));
            Assert.Equal(helpNamespace, fileName);
            Assert.Equal(1, callCount);

            // Get again.
            Assert.Equal(expectedResult, accessibleObject.GetHelpTopic(out fileName));
            Assert.Equal(helpNamespace, fileName);
            Assert.Equal(2, callCount);

            // Remove handler.
            ownerControl.QueryAccessibilityHelp -= handler;
            Assert.Equal(exectedResultWithoutHandler, accessibleObject.GetHelpTopic(out fileName));
            Assert.Null(fileName);
            Assert.Equal(2, callCount);
        }

        [WinFormsFact]
        public void ControlAccessibleObject_GetSelected_InvokeDefault_ReturnsNull()
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Null(accessibleObject.GetSelected());
        }

        [WinFormsTheory]
        [InlineData(-1, -2)]
        [InlineData(0, 0)]
        [InlineData(1, 2)]
        public void AccessibleObject_HitTest_InvokeDefault_ReturnsNull(int x, int y)
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Null(accessibleObject.HitTest(x, y));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(AccessibleNavigation))]
        public void AccessibleObject_Navigate_InvokeDefault_ReturnsNull(AccessibleNavigation navdir)
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Null(accessibleObject.Navigate(navdir));
        }

        [WinFormsTheory]
        [InlineData(AccessibleEvents.Create)]
        public void ControlAccessibleObject_NotifyClients_InvokeAccessibleEvents_Success(AccessibleEvents accEvent)
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            accessibleObject.NotifyClients(accEvent);
        }

        [WinFormsTheory]
        [InlineData(AccessibleEvents.Create, -1)]
        [InlineData(AccessibleEvents.Create, 0)]
        [InlineData(AccessibleEvents.Create, 1)]
        public void ControlAccessibleObject_NotifyClients_InvokeAccessibleEventsInt_Success(AccessibleEvents accEvent, int childID)
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            accessibleObject.NotifyClients(accEvent, childID);
        }

        [WinFormsTheory]
        [InlineData(AccessibleEvents.Create, -1, -1)]
        [InlineData(AccessibleEvents.Create, 1, 0)]
        [InlineData(AccessibleEvents.Create, 2, 1)]
        public void ControlAccessibleObject_NotifyClients_InvokeAccessibleEventsIntInt_Success(AccessibleEvents accEvent, int objectID, int childID)
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            accessibleObject.NotifyClients(accEvent, objectID, childID);
        }

        [WinFormsFact]
        public void ControlAccessibleObject_RaiseLiveRegionChanged_Invoke_Success()
        {
            using var ownerControl = new AutomationLiveRegionControl();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            accessibleObject.RaiseLiveRegionChanged();
        }

        [WinFormsFact]
        public void ControlAccessibleObject_RaiseLiveRegionChanged_InvokeNotIAutomationLiveRegion_ThrowsInvalidOperationException()
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Throws<InvalidOperationException>(() => accessibleObject.RaiseLiveRegionChanged());
        }

        [WinFormsTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void ControlAccessibleObject_RaiseAutomationEvent_IsHandleCreatedFlag(bool isHandleCreated)
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);

            Assert.False(ownerControl.IsHandleCreated);

            if (isHandleCreated)
            {
                Assert.NotEqual(IntPtr.Zero, ownerControl.Handle);
            }

            Assert.Equal(isHandleCreated, accessibleObject.RaiseAutomationEvent(UiaCore.UIA.AutomationPropertyChangedEventId));
            Assert.Equal(isHandleCreated, ownerControl.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(false)]
        [InlineData(true)]
        public void ControlAccessibleObject_RaiseAutomationPropertyChangedEvent_IsHandleCreatedFlag(bool isHandleCreated)
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);

            Assert.False(ownerControl.IsHandleCreated);

            if (isHandleCreated)
            {
                Assert.NotEqual(IntPtr.Zero, ownerControl.Handle);
            }

            Assert.Equal(isHandleCreated, accessibleObject.RaiseAutomationPropertyChangedEvent(UiaCore.UIA.NamePropertyId, ownerControl.Name, ownerControl.Name));
            Assert.Equal(isHandleCreated, ownerControl.IsHandleCreated);
        }

        [WinFormsFact]
        public void ControlAccessibleObject_ToString_Invoke_Success()
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal("ControlAccessibleObject: Owner = System.Windows.Forms.Control", accessibleObject.ToString());
        }

        [WinFormsFact]
        public void ControlAccessibleObject_ToString_InvokeNotInitialized_Success()
        {
            Control.ControlAccessibleObject accessibleObject = Assert.IsType<Control.ControlAccessibleObject>(FormatterServices.GetUninitializedObject(typeof(Control.ControlAccessibleObject)));
            Assert.Equal("ControlAccessibleObject: Owner = null", accessibleObject.ToString());
        }

        [WinFormsFact]
        public void ControlAccessibleObject_IAccessibleaccFocus_InvokeDefault_ReturnsNull()
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            IAccessible iAccessible = accessibleObject;
            Assert.Null(iAccessible.accFocus);
        }

        [WinFormsTheory]
        [InlineData(true, -1, -2, 0)]
        [InlineData(true, 0, 0, 0)]
        [InlineData(true, 1, 2, 0)]
        [InlineData(false, -1, -2, null)]
        [InlineData(false, 0, 0, null)]
        [InlineData(false, 1, 2, null)]
        public void AccessibleObject_IAccessibleaccHitTest_InvokeDefault_ReturnsExpectedValue(bool createControl, int x, int y, int? expectedValue)
        {
            using var ownerControl = new Control();
            if (createControl)
            {
                ownerControl.CreateControl();
            }

            Assert.Equal(createControl, ownerControl.IsHandleCreated);

            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(createControl, ownerControl.IsHandleCreated);
            IAccessible iAccessible = accessibleObject;
            Assert.Equal(expectedValue, iAccessible.accHitTest(x, y));
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void ControlAccessibleObject_IAccessibleaccParent_InvokeDefault_ReturnsExpectedValue(bool createControl)
        {
            using var ownerControl = new Control();
            if (createControl)
            {
                ownerControl.CreateControl();
            }

            Assert.Equal(createControl, ownerControl.IsHandleCreated);

            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(createControl, ownerControl.IsHandleCreated);
            IAccessible iAccessible = accessibleObject;
            Assert.Equal(createControl, iAccessible.accParent != null);
        }

        [WinFormsFact]
        public void ControlAccessibleObject_IAccessibleaccSelection_InvokeDefault_ReturnsNull()
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            IAccessible iAccessible = accessibleObject;
            Assert.Null(iAccessible.accSelection);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ControlAccessibleObject_IAccessibleget_accChild_InvokeNoSuchChild_ReturnsNull(int childID)
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            IAccessible iAccessible = accessibleObject;
            Assert.Null(iAccessible.get_accChild(childID));
        }

        [WinFormsFact]
        public void ControlAccessibleObject_IAccessibleget_accChildCount_InvokeDefault_ReturnsExpected()
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            IAccessible iAccessible = accessibleObject;
            Assert.Equal(0, iAccessible.accChildCount);
        }

        [WinFormsTheory]
        [InlineData(true, 1)]
        [InlineData(false, 0)]
        public void ControlAccessibleObject_IAccessibleget_accChildCount_InvokeWithChildren_ReturnsExpected(bool createControl, int expectedCount)
        {
            using var child = new Control();
            using var ownerControl = new Control();
            if (createControl)
            {
                ownerControl.CreateControl();
            }

            Assert.Equal(createControl, ownerControl.IsHandleCreated);
            ownerControl.Controls.Add(child);
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            Assert.Equal(createControl, ownerControl.IsHandleCreated);
            IAccessible iAccessible = accessibleObject;
            Assert.Equal(expectedCount, iAccessible.accChildCount);
        }

        [WinFormsTheory]
        [InlineData(-1)]
        [InlineData(1)]
        public void ControlAccessibleObject_IAccessibleget_accRole_InvokeNoSuchChild_ReturnsNull(object varChild)
        {
            using var ownerControl = new Control();
            var accessibleObject = new Control.ControlAccessibleObject(ownerControl);
            IAccessible iAccessible = accessibleObject;
            Assert.Null(iAccessible.get_accRole(varChild));
        }

        [WinFormsFact]
        public void ControlAccessibleObject_DoesntSupport_LegacyIAccessiblePattern()
        {
            using var control = new Control();
            var accessibleObject = control.AccessibilityObject;

            bool expected = control.SupportsUiaProviders;
            Assert.False(expected);

            bool actual = accessibleObject.IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId);
            Assert.Equal(expected, actual);
        }

        [WinFormsFact]
        public void ControlAccessibleObject_Supports_LegacyIAccessiblePattern_IfOwnerSupportsUia()
        {
            using var control = new Label();
            var accessibleObject = new Control.ControlAccessibleObject(control);

            Assert.True(control.SupportsUiaProviders);
            bool actual = accessibleObject.IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId);
            Assert.True(actual);
        }

        public static IEnumerable<object[]> ControlAccessibleObject_TestData()
        {
            return ReflectionHelper.GetPublicNotAbstractClasses<Control>().Select(type => new object[] { type });
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlAccessibleObject_TestData))]
        public void ControlAccessibleObject_Custom_Role_ReturnsExpected(Type type)
        {
            using Control control = ReflectionHelper.InvokePublicConstructor<Control>(type);

            if (!control.SupportsUiaProviders)
            {
                return;
            }

            control.AccessibleRole = AccessibleRole.Link;
            AccessibleObject controlAccessibleObject = control.AccessibilityObject;

            var accessibleObjectRole = controlAccessibleObject.Role;

            Assert.Equal(AccessibleRole.Link, accessibleObjectRole);
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlAccessibleObject_TestData))]
        public void ControlAccessibleObject_IsPatternSupported_LegacyIAccessible_ReturnsTrue(Type type)
        {
            using Control control = ReflectionHelper.InvokePublicConstructor<Control>(type);

            if (!control.SupportsUiaProviders)
            {
                return;
            }

            AccessibleObject controlAccessibleObject = control.AccessibilityObject;

            bool supportsLegacyIAccessiblePatternId = controlAccessibleObject.IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId);

            Assert.True(supportsLegacyIAccessiblePatternId);
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlAccessibleObject_TestData))]
        public void ControlAccessibleObject_Custom_Description_ReturnsExpected(Type type)
        {
            using Control control = ReflectionHelper.InvokePublicConstructor<Control>(type);

            if (!control.SupportsUiaProviders)
            {
                return;
            }

            control.AccessibleDescription = "Test Accessible Description";
            AccessibleObject controlAccessibleObject = control.AccessibilityObject;

            var accessibleObjectDescription = controlAccessibleObject.Description;

            Assert.Equal("Test Accessible Description", accessibleObjectDescription);
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlAccessibleObject_TestData))]
        public void ControlAccessibleObject_GetPropertyValue_Custom_Name_ReturnsExpected(Type type)
        {
            using Control control = ReflectionHelper.InvokePublicConstructor<Control>(type);

            if (!control.SupportsUiaProviders)
            {
                return;
            }

            AccessibleObject controlAccessibleObject = control.AccessibilityObject;
            control.Name = "Name1";
            control.AccessibleName = "Test Name";

            var accessibleName = controlAccessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId);

            Assert.Equal("Test Name", accessibleName);
        }

        public static IEnumerable<object[]> ControlAccessibleObject_DefaultName_TestData()
        {
            // These controls have AccessibleName defined.
            // MonthCalendar has "Month" view by default and returns current date as AccessibleName
            var typeDefaultValues = new Dictionary<Type, string> {
                { typeof(DataGridViewTextBoxEditingControl), SR.DataGridView_AccEditingControlAccName},
                { typeof(PrintPreviewDialog), SR.PrintPreviewDialog_PrintPreview},
                { typeof(MonthCalendar), string.Format(SR.MonthCalendarSingleDateSelected, DateTime.Now.ToLongDateString())}
            };

            foreach (Type type in ReflectionHelper.GetPublicNotAbstractClasses<Control>())
            {
                yield return new object[] {
                    type,
                    typeDefaultValues.ContainsKey(type) ? typeDefaultValues[type] : null
                };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(ControlAccessibleObject_DefaultName_TestData))]
        public void ControlAccessibleObject_GetPropertyValue_Default_Name_ReturnsExpected(Type type, string expectedName)
        {
            using Control control = ReflectionHelper.InvokePublicConstructor<Control>(type);

            if (!control.SupportsUiaProviders)
            {
                return;
            }

            AccessibleObject controlAccessibleObject = control.AccessibilityObject;
            Assert.Equal(expectedName, controlAccessibleObject.GetPropertyValue(UiaCore.UIA.NamePropertyId));
        }

        private class AutomationLiveRegionControl : Control, IAutomationLiveRegion
        {
            public AutomationLiveSetting LiveSetting { get; set; }
        }

        private class SubControl : Control
        {
            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}
