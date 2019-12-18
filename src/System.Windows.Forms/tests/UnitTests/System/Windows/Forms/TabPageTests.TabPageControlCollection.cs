// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class TabPageTabPageControlCollectionTests
    {
        public TabPageTabPageControlCollectionTests()
        {
            Application.ThreadException += (sender, e) => throw new Exception(e.Exception.StackTrace.ToString());
        }

        public static IEnumerable<object[]> Ctor_TabPage_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new TabPage() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_TabPage_TestData))]
        public void TabPageControlCollection_Ctor_TabPage(TabPage owner)
        {
            var collection = new TabPage.TabPageControlCollection(owner);
            Assert.Empty(collection);
            Assert.False(collection.IsReadOnly);
            Assert.Same(owner, collection.Owner);
        }


        [WinFormsFact]
        public void TabPageControlCollection_Add_ControlExistingCollection_Success()
        {
            using var owner = new TabPage();
            using var control1 = new Control();
            using var control2 = new Control();
            TabPage.TabPageControlCollection collection = Assert.IsType<TabPage.TabPageControlCollection>(owner.Controls);
            int parentLayoutCallCount = 0;
            string affectedProperty = null;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                Assert.Same(collection.Cast<Control>().Last(), e.AffectedControl);
                Assert.Equal(affectedProperty, e.AffectedProperty);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            int layoutCallCount1 = 0;
            control1.Layout += (sender, e) => layoutCallCount1++;
            int layoutCallCount2 = 0;
            control2.Layout += (sender, e) => layoutCallCount2++;

            try
            {
                affectedProperty = "Parent";
                collection.Add(control1);
                Assert.Same(control1, Assert.Single(collection));
                Assert.Same(owner, control1.Parent);
                Assert.Equal(0, control1.TabIndex);
                Assert.Same(control1, Assert.Single(owner.Controls));
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control1.IsHandleCreated);

                // Add another.
                collection.Add(control2);
                Assert.Equal(new Control[] { control1, control2 }, collection.Cast<Control>());
                Assert.Same(owner, control1.Parent);
                Assert.Equal(0, control1.TabIndex);
                Assert.Same(owner, control2.Parent);
                Assert.Equal(1, control2.TabIndex);
                Assert.Equal(new Control[] { control1, control2 }, owner.Controls.Cast<Control>());
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control1.IsHandleCreated);
                Assert.False(control2.IsHandleCreated);

                // Add existing.
                affectedProperty = "ChildIndex";
                collection.Add(control1);
                Assert.Equal(new Control[] { control2, control1 }, collection.Cast<Control>());
                Assert.Same(owner, control1.Parent);
                Assert.Equal(0, control1.TabIndex);
                Assert.Same(owner, control2.Parent);
                Assert.Equal(1, control2.TabIndex);
                Assert.Equal(new Control[] { control2, control1}, owner.Controls.Cast<Control>());
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(3, parentLayoutCallCount);
                Assert.False(owner.IsHandleCreated);
                Assert.False(control1.IsHandleCreated);
                Assert.False(control2.IsHandleCreated);

                // Add null.
                collection.Add(null);
                Assert.Equal(new Control[] { control2, control1 }, collection.Cast<Control>());
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void ControlCollection_Add_TabPageValue_ThrowsArgumentException()
        {
            using var owner = new TabPage();
            using var control = new TabPage();
            var collection = new TabPage.TabPageControlCollection(owner);
            Assert.Throws<ArgumentException>(null, () => collection.Add(control));
        }
    }
}
