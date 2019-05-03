﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class ListBoxAccessibleObjectTests
    {
        [Fact]
        public void ListBoxAccessibleObjectTests_Ctor_Default()
        {
            ListBox listBox = new ListBox();
            listBox.Items.AddRange(new object[] {
            "a",
            "b",
            "c",
            "d",
            "e",
            "f",
            "g"});

            var childCount = listBox.AccessibilityObject.GetChildCount();

            for (int i = 1; i <= childCount; i++)
            {
                var child = (ListBox.ListBoxAccessibleObject)listBox.AccessibilityObject.GetObjectForChildInternal(i);
                Assert.True(child.IsPatternSupported(NativeMethods.UIA_ScrollItemPatternId));
            }
        }
    }
}
