// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class ErrorProviderAccessibleObjectTests : IDisposable, IClassFixture<ThreadExceptionFixture>
    {
        private readonly Form _form;
        private readonly Control _control1;
        private readonly Control _control2;
        private readonly ErrorProvider _errorProvider;
        private readonly ErrorProvider.ErrorWindow _errorWindow;
        private readonly ErrorProvider.ControlItem _controlItem1;
        private readonly ErrorProvider.ControlItem _controlItem2;
        private readonly string _errorText1;
        private readonly string _errorText2;

        public ErrorProviderAccessibleObjectTests()
        {
            _form = new Form();
            _form.CreateControl();
            _form.Visible = true;

            _control1 = new Control();
            _control1.CreateControl();
            _control1.Visible = true;

            _control2 = new Control();
            _control2.CreateControl();
            _control2.Visible = true;

            _form.Controls.Add(_control1);
            _form.Controls.Add(_control2);

            _errorText1 = "Test text 1";
            _errorText2 = "Test text 2";

            _errorProvider = new ErrorProvider();
            _errorProvider.SetError(_control1, _errorText1);
            _errorProvider.SetError(_control2, _errorText2);

            _errorWindow = _errorProvider.EnsureErrorWindow(_form);
            _controlItem1 = _errorWindow.ControlItems.Count > 0 ? _errorWindow.ControlItems[0] : null;
            _controlItem2 = _errorWindow.ControlItems.Count > 0 ? _errorWindow.ControlItems[1] : null;
        }

        public void Dispose()
        {
            _errorProvider?.Dispose();
            _control2?.Dispose();
            _control1?.Dispose();
            _form?.Dispose();
        }

        [WinFormsFact]
        public void ErrorProviderAccessibleObject_Ctor_Default()
        {
            Assert.NotNull(_errorWindow);
            AccessibleObject windowAccessibleObject = _errorWindow.AccessibilityObject;
            Assert.NotNull(windowAccessibleObject);

            Assert.NotNull(_controlItem1);
            AccessibleObject controlItemAccessibleObject1 = _controlItem1.AccessibilityObject;
            Assert.NotNull(controlItemAccessibleObject1);

            Assert.NotNull(_controlItem2);
            AccessibleObject controlItemAccessibleObject2 = _controlItem2.AccessibilityObject;
            Assert.NotNull(controlItemAccessibleObject2);
        }

        [WinFormsFact]
        public void ErrorProvider_ControlItemAccessibleObject_CorrectControlType()
        {
            AccessibleObject controlItemAccessibleObject = _controlItem1.AccessibilityObject;
            object actual = controlItemAccessibleObject?.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            object expected = UiaCore.UIA.ImageControlTypeId;
            Assert.Equal(expected, actual);
        }

        [WinFormsFact]
        public void ErrorProvider_ErrorWindowAccessibleObject_CorrectControlType()
        {
            AccessibleObject errorWindowAccessibleObject = _errorWindow.AccessibilityObject;
            object actual = errorWindowAccessibleObject?.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            object expected = UiaCore.UIA.GroupControlTypeId;
            Assert.Equal(expected, actual);
        }

        [WinFormsFact]
        public void ErrorProvider_CorrectErrorValue()
        {
            // Check string for _control1
            string actual = _errorProvider.GetError(_control1);
            string expected = _errorText1;
            Assert.Equal(expected, actual);

            actual = _controlItem1.Error;
            Assert.Equal(expected, actual);

            // Check string for _control2
            actual = _errorProvider.GetError(_control2);
            expected = _errorText2;
            Assert.Equal(expected, actual);

            actual = _controlItem2.Error;
            Assert.Equal(expected, actual);
        }

        [WinFormsFact]
        public void ErrorProvider_CorrectAccessibilityTree()
        {
            AccessibleObject errorWindowAccessibilityObject = _errorWindow.AccessibilityObject;
            AccessibleObject controlItem1_AccessibilityObject = _controlItem1.AccessibilityObject;
            AccessibleObject controlItem2_AccessibilityObject = _controlItem2.AccessibilityObject;

            int childCound = errorWindowAccessibilityObject.GetChildCount();
            int expectedCount = 2;
            Assert.Equal(expectedCount, childCound);

            AccessibleObject actualAccessibilityObject = errorWindowAccessibilityObject.GetChild(0);
            Assert.Equal(controlItem1_AccessibilityObject, actualAccessibilityObject);

            actualAccessibilityObject = errorWindowAccessibilityObject.GetChild(1);
            Assert.Equal(controlItem2_AccessibilityObject, actualAccessibilityObject);

            actualAccessibilityObject = controlItem1_AccessibilityObject.Parent;
            Assert.Equal(errorWindowAccessibilityObject, actualAccessibilityObject);

            actualAccessibilityObject = controlItem2_AccessibilityObject.Parent;
            Assert.Equal(errorWindowAccessibilityObject, actualAccessibilityObject);

            actualAccessibilityObject = (AccessibleObject)controlItem1_AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            Assert.Equal(controlItem2_AccessibilityObject, actualAccessibilityObject);

            actualAccessibilityObject = (AccessibleObject)controlItem1_AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            Assert.Null(actualAccessibilityObject);

            actualAccessibilityObject = (AccessibleObject)controlItem2_AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.PreviousSibling);
            Assert.Equal(controlItem1_AccessibilityObject, actualAccessibilityObject);

            actualAccessibilityObject = (AccessibleObject)controlItem2_AccessibilityObject.FragmentNavigate(UiaCore.NavigateDirection.NextSibling);
            Assert.Null(actualAccessibilityObject);
        }

        [WinFormsFact]
        public void ErrorProvider_NameDoesntEqualControlTypeOrChildName()
        {
            // Mas requires us to have no same AccessibleName and LocalizedControlType or child AccessibleName.
            // So we need to check if these properties are not equal.
            // In this specific case, we can't have some positive Assert.
            // ToLower method used to be case insensitive.

            string errorWindowControlType = "group";
            string actualWindowAccessibleName = _errorWindow.AccessibilityObject.Name.ToLower();
            Assert.NotEqual(errorWindowControlType, actualWindowAccessibleName);

            string controlItemControlType = "image";
            string actualItemAccessibleName = _controlItem1.AccessibilityObject.Name.ToLower();
            Assert.NotEqual(controlItemControlType, actualItemAccessibleName);

            Assert.NotEqual(actualWindowAccessibleName, actualItemAccessibleName);
        }
    }
}
