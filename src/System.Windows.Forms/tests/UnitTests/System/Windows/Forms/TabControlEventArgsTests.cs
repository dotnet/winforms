// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TabControlEventArgsTests
    {
        public static IEnumerable<object[]> Ctor_TabPage_Int_TabControlAction_TestData()
        {
            yield return new object[] { null, -2, (TabControlAction)(TabControlAction.Selecting - 1) };
            yield return new object[] { new TabPage(), -1, TabControlAction.Selecting };
            yield return new object[] { new TabPage(), 0, TabControlAction.Selected };
            yield return new object[] { new TabPage(), 1, TabControlAction.Selected };
        }

        [Theory]
        [MemberData(nameof(Ctor_TabPage_Int_TabControlAction_TestData))]
        public void Ctor_TabPage_Int_TabControlAction(TabPage tabPage, int tabPageIndex, TabControlAction action)
        {
            var e = new TabControlEventArgs(tabPage, tabPageIndex, action);
            Assert.Equal(tabPage, e.TabPage);
            Assert.Equal(tabPageIndex, e.TabPageIndex);
            Assert.Equal(action, e.Action);
        }
    }
}
