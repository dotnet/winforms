// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using Xunit;
using static System.Windows.Forms.ErrorProvider;

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
    }
}
