// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;
using static System.Windows.Forms.ErrorProvider;

namespace System.Windows.Forms.Tests
{
    public class ErrorProvider_ControlItem_ControlItemAccessibleObjectTests
    {
        [WinFormsFact]
        public void ErrorWindowAccessibleObject_Ctor_Default()
        {
            Type type = typeof(ControlItem)
                .GetNestedType("ControlItemAccessibleObject", BindingFlags.NonPublic | BindingFlags.Instance);
            var accessibleObject = (AccessibleObject)Activator.CreateInstance(type, new object[] { null, null, null, null });

            Assert.Null(accessibleObject.TestAccessor().Dynamic._controlItem);
            Assert.Null(accessibleObject.TestAccessor().Dynamic._window);
            Assert.Null(accessibleObject.TestAccessor().Dynamic._control);
            Assert.Null(accessibleObject.TestAccessor().Dynamic._provider);
            Assert.Equal(AccessibleRole.Alert, accessibleObject.Role);
        }
    }
}
