// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.Form;
using static Interop;

namespace System.Windows.Forms.Tests
{
    public class Form_FormAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void FormAccessibleObject_Ctor_Default()
        {
            using Form form = new Form();
            FormAccessibleObject accessibleObject = new FormAccessibleObject(form);

            Assert.Equal(form, accessibleObject.Owner);
            Assert.False(form.IsHandleCreated);
        }

        [WinFormsFact]
        public void FormAccessibleObject_ControlType_IsNull()
        {
            using Form form = new Form();
            object actual = form.AccessibilityObject.GetPropertyValue(UiaCore.UIA.ControlTypePropertyId);
            Assert.Null(actual);
            Assert.False(form.IsHandleCreated);
        }

        [WinFormsFact]
        public void FormAccessibleObject_Role_IsClient_ByDefault()
        {
            using Form form = new Form();
            // AccessibleRole is not set = Default

            AccessibleRole actual = form.AccessibilityObject.Role;

            Assert.Equal(AccessibleRole.Client, actual);
            Assert.False(form.IsHandleCreated);
        }
    }
}
