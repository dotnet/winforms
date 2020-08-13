// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests.AccessibleObjects
{
    public class CheckedListBoxAccessibleObjectTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void CheckedListBoxAccessibleObject_CheckBounds()
        {
            using CheckedListBox checkedListBox = new CheckedListBox();
            checkedListBox.Size = new Size(120, 100);
            checkedListBox.Items.Add("a");
            checkedListBox.Items.Add("b");
            checkedListBox.Items.Add("c");
            checkedListBox.Items.Add("d");
            checkedListBox.Items.Add("e");
            checkedListBox.Items.Add("f");
            checkedListBox.Items.Add("g");
            checkedListBox.Items.Add("h");
            checkedListBox.Items.Add("i");

            int listBoxHeight = checkedListBox.AccessibilityObject.Bounds.Height;
            int sumItemsHeight = 0;

            for (int i = 0; i < checkedListBox.Items.Count; i++)
            {
                AccessibleObject item = checkedListBox.AccessibilityObject.GetChild(i);
                sumItemsHeight += item.Bounds.Height;
            }

            Assert.Equal(listBoxHeight, sumItemsHeight);
        }
    }
}
