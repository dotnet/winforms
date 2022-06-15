// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;
using static System.Windows.Forms.ErrorProvider;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class ErrorProvider_ErrorWindow_ErrorWindowAccessibleObject
    {
        [WinFormsFact]
        public void ErrorWindowAccessibleObject_Ctor_Default()
        {
            Type type = typeof(ErrorWindow)
                .GetNestedType("ErrorWindowAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
            var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null });

            Assert.Null(accessibleObject.TestAccessor().Dynamic._owner);
            Assert.Equal(AccessibleRole.Grouping, accessibleObject.Role);
        }

        [WinFormsFact]
        public void ErrorWindowAccessibleObject_IsReadOnly_ReturnsExpected()
        {
            Type type = typeof(ErrorWindow)
                .GetNestedType("ErrorWindowAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
            var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null });

            Assert.True(accessibleObject.IsReadOnly);
        }

        [WinFormsFact]
        public void ErrorWindowAccessibleObject_State_ReturnsExpected()
        {
            Type type = typeof(ErrorWindow)
                .GetNestedType("ErrorWindowAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
            var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null });

            Assert.Equal(AccessibleStates.ReadOnly, accessibleObject.State);
        }

        [WinFormsTheory]
        [InlineData((int)UiaCore.UIA.LegacyIAccessiblePatternId)]
        public void ErrorWindowAccessibleObject_IsPatternSupported_ReturnsExpected(int patternId)
        {
            Type type = typeof(ErrorWindow)
                .GetNestedType("ErrorWindowAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
            var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null });

            Assert.True(accessibleObject.IsPatternSupported((UiaCore.UIA)patternId));
        }

        [WinFormsFact]
        public void ErrorWindowAccessibleObject_ControlType_ReturnsExpected()
        {
            Type type = typeof(ErrorWindow)
                .GetNestedType("ErrorWindowAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
            var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null });

            Assert.Equal(UiaCore.UIA.GroupControlTypeId, accessibleObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId));
        }

        [WinFormsFact]
        public void ErrorWindowAccessibleObject_GetPropertyValue_ReturnsExpected()
        {
            Type type = typeof(ErrorWindow)
                .GetNestedType("ErrorWindowAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
            var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null });

            Assert.Null(accessibleObject.GetPropertyValue(UiaCore.UIA.ValueValuePropertyId));
            Assert.Null(accessibleObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId));
            Assert.Equal(AccessibleStates.ReadOnly, accessibleObject.GetPropertyValue(UiaCore.UIA.LegacyIAccessibleStatePropertyId));
        }

        [WinFormsFact]
        public void ErrorWindowAccessibleObject_FragmentRoot_ReturnsExpected()
        {
            Type type = typeof(ErrorWindow)
                .GetNestedType("ErrorWindowAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
            var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null });

            Assert.Equal(accessibleObject, accessibleObject.FragmentRoot);
        }

        [WinFormsFact]
        public void ErrorWindowAccessibleObject_IsIAccessibleExSupported_ReturnsExpected()
        {
            using Control control = new();
            ErrorProvider provider = new();
            ErrorWindow window = new(provider, control);

            AccessibleObject accessibleObject = window.AccessibilityObject;

            Assert.True(accessibleObject.IsIAccessibleExSupported());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ErrorWindowAccessibleObject_GetChildCount_ReturnsExpected()
        {
            int testCount = 2;
            using Control control = new();
            ErrorProvider provider = new();
            ErrorWindow window = new(provider, control);
            for (int i = 0; i < testCount; i++)
            {
                window.ControlItems.Add(new(provider, control, (IntPtr)i));
            }

            AccessibleObject accessibleObject = window.AccessibilityObject;

            Assert.Equal(testCount, accessibleObject.GetChildCount());
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ErrorWindowAccessibleObject_GetChild_ReturnsExpected()
        {
            using Control control = new();
            ErrorProvider provider = new();
            ErrorWindow window = new(provider, control);

            ControlItem item1 = new(provider, control, (IntPtr)100);
            ControlItem item2 = new(provider, control, (IntPtr)200);

            window.ControlItems.AddRange(new[] { item1, item2 });
            AccessibleObject accessibleObject = window.AccessibilityObject;

            Assert.Null(accessibleObject.GetChild(-1));
            Assert.Equal(item1.AccessibilityObject, accessibleObject.GetChild(0));
            Assert.Equal(item2.AccessibilityObject, accessibleObject.GetChild(1));
            Assert.Null(accessibleObject.GetChild(2));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData("")]
        [InlineData(null)]
        public void ErrorWindowAccessibleObject_Name_ReturnsExpected_IfNullOrStringEmptyString(string testName)
        {
            using Control control = new();
            ErrorProvider provider = new();
            ErrorWindow window = new(provider, control);

            ControlItem item1 = new(provider, control, (IntPtr)100);
            ControlItem item2 = new(provider, control, (IntPtr)200);
            window.ControlItems.AddRange(new[] { item1, item2 });

            AccessibleObject accessibleObject = window.AccessibilityObject;
            accessibleObject.Name = testName;

            Assert.Equal(SR.ErrorProviderDefaultAccessibleName, accessibleObject.Name);
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ErrorWindowAccessibleObject_FragmentNavigate_ReturnsExpected()
        {
            using Control control = new();
            ErrorProvider provider = new();
            ErrorWindow window = new(provider, control);

            ControlItem item1 = new(provider, control, (IntPtr)100);
            ControlItem item2 = new(provider, control, (IntPtr)200);
            ControlItem item3 = new(provider, control, (IntPtr)300);

            window.ControlItems.AddRange(new[] { item1, item2, item3 });
            AccessibleObject accessibleObject = window.AccessibilityObject;

            Assert.Equal(item1.AccessibilityObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Equal(item3.AccessibilityObject, accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsFact]
        public void ErrorWindowAccessibleObject_FragmentNavigate_ReturnsExpected_NoItems()
        {
            using Control control = new();
            ErrorProvider provider = new();
            ErrorWindow window = new(provider, control);

            AccessibleObject accessibleObject = window.AccessibilityObject;

            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.FirstChild));
            Assert.Null(accessibleObject.FragmentNavigate(UiaCore.NavigateDirection.LastChild));
            Assert.False(control.IsHandleCreated);
        }
    }
}
