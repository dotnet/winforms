// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolBarButtonClickEventArgsTests
    {
        public static IEnumerable<object[]> ToolBarButton_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ToolBarButton() };
        }

        [Theory]
        [MemberData(nameof(ToolBarButton_TestData))]
        public void Ctor_ToolBarButton(ToolBarButton button)
        {
            var e = new ToolBarButtonClickEventArgs(button);
            Assert.Equal(button, e.Button);
        }

        [Theory]
        [MemberData(nameof(ToolBarButton_TestData))]
        public void Button_Set_GetReturnsExpected(ToolBarButton value)
        {
            var e = new ToolBarButtonClickEventArgs(new ToolBarButton())
            {
                Button = value
            };
            Assert.Equal(value, e.Button);
        }
    }
}
