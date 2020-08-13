// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class ListBoxAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ListBoxAccessibleObjectTests_Ctor_Default()
        {
            using ListBox listBox = new ListBox();
            listBox.Items.AddRange(new object[] {
                "a",
                "b",
                "c",
                "d",
                "e",
                "f",
                "g"
            });

            var childCount = listBox.AccessibilityObject.GetChildCount();

            for (int i = 0; i < childCount; i++)
            {
                var child = listBox.AccessibilityObject.GetChild(i);
                Assert.True(child.IsPatternSupported(UiaCore.UIA.ScrollItemPatternId));
            }
        }
    }
}
