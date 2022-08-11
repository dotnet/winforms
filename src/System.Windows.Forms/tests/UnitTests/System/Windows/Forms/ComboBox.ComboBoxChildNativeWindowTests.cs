// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ComboBox_ComboBoxChildNativeWindowTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ComboBoxChildNativeWindow_GetChildAccessibleObject()
        {
            using ComboBox comboBox = new() { DropDownStyle = ComboBoxStyle.DropDown };
            comboBox.CreateControl();

            var childNativeWindow = comboBox.GetListNativeWindow();
            Type childWindowTypeEnum = typeof(ComboBox).GetNestedType("ChildWindowType", Reflection.BindingFlags.NonPublic);

            foreach (var childWindowType in Enum.GetValues(childWindowTypeEnum))
            {
                Assert.True(childNativeWindow.TestAccessor().Dynamic.GetChildAccessibleObject() is ComboBox.ChildAccessibleObject);
            }
        }
    }
}
