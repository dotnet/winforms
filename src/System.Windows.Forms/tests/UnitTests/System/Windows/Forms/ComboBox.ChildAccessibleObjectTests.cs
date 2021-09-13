// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ComboBox_ChildAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ChildAccessibleObject_Ctor_Default()
        {
            using var control = new ComboBox();
            control.CreateControl();

            Assert.True(control.IsHandleCreated);
            var accessibleObject = new ComboBox.ChildAccessibleObject(control, IntPtr.Zero);
            Assert.NotNull(accessibleObject.TestAccessor().Dynamic._owner);
        }
    }
}
