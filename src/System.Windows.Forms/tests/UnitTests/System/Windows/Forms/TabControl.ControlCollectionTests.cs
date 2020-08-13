// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Moq;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    using Size = System.Drawing.Size;

    public class TabControlControlCollectionTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void TabControlControlCollection_Ctor_TabControl()
        {
            using var owner = new TabControl();
            var collection = new TabControl.ControlCollection(owner);
            Assert.Empty(collection);
            Assert.False(collection.IsReadOnly);
            Assert.Same(owner, collection.Owner);
        }

        [WinFormsFact]
        public void TabControlControlCollection_Ctor_NullOwner_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("owner", () => new TabControl.ControlCollection(null));
        }

        public static IEnumerable<object[]> Add_TestData()
        {
            yield return new object[] { TabAppearance.Buttons };
            yield return new object[] { TabAppearance.FlatButtons };
            yield return new object[] { TabAppearance.Normal };
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_TestData))]
        public void TabControlControlCollection_Add_InvokeValueWithoutHandleOwnerWithoutHandle_Success(TabAppearance appearance)
        {
            using var owner = new TabControl
            {
                Appearance = appearance,
                Bounds = new Rectangle(0, 0, 400, 300)
            };
            using var value1 = new TabPage();
            using var value2 = new TabPage();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);

            int layoutCallCount1 = 0;
            value1.Layout += (sender, e) => layoutCallCount1++;
            int layoutCallCount2 = 0;
            value2.Layout += (sender, e) => layoutCallCount2++;
            int parentLayoutCallCount = 0;
            var events = new List<LayoutEventArgs>();
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                events.Add(e);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;

            try
            {
                // Add first.
                collection.Add(value1);
                Assert.Same(value1, Assert.Single(collection));
                Assert.Same(value1, Assert.Single(owner.TabPages));
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.Same(value1, events[0].AffectedControl);
                Assert.Equal("Parent", events[0].AffectedProperty);
                Assert.Same(value1, events[1].AffectedControl);
                Assert.Equal("Visible", events[1].AffectedProperty);
                Assert.False(value1.IsHandleCreated);
                Assert.False(owner.IsHandleCreated);

                // Add another.
                collection.Add(value2);
                Assert.Equal(new Control[] { value1, value2 }, collection.Cast<Control>());
                Assert.Equal(new TabPage[] { value1, value2 }, owner.TabPages.Cast<TabPage>());
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Same(owner, value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(4, parentLayoutCallCount);
                Assert.Same(value1, events[0].AffectedControl);
                Assert.Equal("Parent", events[0].AffectedProperty);
                Assert.Same(value1, events[1].AffectedControl);
                Assert.Equal("Visible", events[1].AffectedProperty);
                Assert.Same(value2, events[2].AffectedControl);
                Assert.Equal("Parent", events[2].AffectedProperty);
                Assert.Same(value2, events[3].AffectedControl);
                Assert.Equal("Visible", events[3].AffectedProperty);
                Assert.False(value1.IsHandleCreated);
                Assert.False(value2.IsHandleCreated);
                Assert.False(owner.IsHandleCreated);

                // Add again.
                collection.Add(value1);
                Assert.Equal(new Control[] { value2, value1 }, collection.Cast<Control>());
                Assert.Equal(new TabPage[] { value1, value2, value1 }, owner.TabPages.Cast<TabPage>());
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Same(owner, value2.Parent);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(5, parentLayoutCallCount);
                Assert.Same(value1, events[0].AffectedControl);
                Assert.Equal("Parent", events[0].AffectedProperty);
                Assert.Same(value1, events[1].AffectedControl);
                Assert.Equal("Visible", events[1].AffectedProperty);
                Assert.Same(value2, events[2].AffectedControl);
                Assert.Equal("Parent", events[2].AffectedProperty);
                Assert.Same(value2, events[3].AffectedControl);
                Assert.Equal("Visible", events[3].AffectedProperty);
                Assert.Same(value1, events[4].AffectedControl);
                Assert.Equal("ChildIndex", events[4].AffectedProperty);
                Assert.False(value1.IsHandleCreated);
                Assert.False(value2.IsHandleCreated);
                Assert.False(owner.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> Add_WithHandle_TestData()
        {
            yield return new object[] { TabAppearance.Buttons, Size.Empty, 0 };
            yield return new object[] { TabAppearance.FlatButtons, Size.Empty, 1 };
            yield return new object[] { TabAppearance.Normal, Size.Empty, 0 };

            yield return new object[] { TabAppearance.Buttons, new Size(100, 120), 0 };
            yield return new object[] { TabAppearance.FlatButtons, new Size(100, 120), 1 };
            yield return new object[] { TabAppearance.Normal, new Size(100, 120), 0 };
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_WithHandle_TestData))]
        public void TabControlControlCollection_Add_InvokeValueWithoutHandleOwnerWithHandle_Success(TabAppearance appearance, Size itemSize, int expectedParentInvalidatedCallCount)
        {
            using var owner = new TabControl
            {
                Appearance = appearance,
                ItemSize = itemSize,
                Bounds = new Rectangle(0, 0, 400, 300)
            };
            using var value1 = new TabPage();
            using var value2 = new TabPage();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);

            int layoutCallCount1 = 0;
            value1.Layout += (sender, e) => layoutCallCount1++;
            int layoutCallCount2 = 0;
            value2.Layout += (sender, e) => layoutCallCount2++;
            int parentLayoutCallCount = 0;
            var events = new List<LayoutEventArgs>();
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                events.Add(e);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int parentInvalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

            try
            {
                // Add first.
                collection.Add(value1);
                Assert.Same(value1, Assert.Single(collection));
                Assert.Same(value1, Assert.Single(owner.TabPages));
                Assert.Same(owner, value1.Parent);
                Assert.True(value1.Visible);
                Assert.Equal(owner.DisplayRectangle, value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Equal(0, owner.SelectedIndex);
                Assert.Equal(2, layoutCallCount1);
                Assert.Equal(5, parentLayoutCallCount);
                Assert.Same(value1, events[0].AffectedControl);
                Assert.Equal("Parent", events[0].AffectedProperty);
                Assert.Same(value1, events[1].AffectedControl);
                Assert.Equal("Visible", events[1].AffectedProperty);
                Assert.Same(value1, events[2].AffectedControl);
                Assert.Equal("Bounds", events[2].AffectedProperty);
                Assert.Same(value1, events[3].AffectedControl);
                Assert.Equal("Bounds", events[3].AffectedProperty);
                Assert.Same(value1, events[4].AffectedControl);
                Assert.Equal("Visible", events[4].AffectedProperty);
                Assert.True(value1.IsHandleCreated);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(expectedParentInvalidatedCallCount, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Add another.
                collection.Add(value2);
                Assert.Equal(new Control[] { value1, value2 }, collection.Cast<Control>());
                Assert.Equal(new TabPage[] { value1, value2 }, owner.TabPages.Cast<TabPage>());
                Assert.Same(owner, value1.Parent);
                Assert.True(value1.Visible);
                Assert.Equal(owner.DisplayRectangle, value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Same(owner, value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(owner.DisplayRectangle, value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(0, owner.SelectedIndex);
                Assert.Equal(2, layoutCallCount1);
                Assert.Equal(1, layoutCallCount2);
                Assert.Equal(9, parentLayoutCallCount);
                Assert.Same(value1, events[0].AffectedControl);
                Assert.Equal("Parent", events[0].AffectedProperty);
                Assert.Same(value1, events[1].AffectedControl);
                Assert.Equal("Visible", events[1].AffectedProperty);
                Assert.Same(value1, events[2].AffectedControl);
                Assert.Equal("Bounds", events[2].AffectedProperty);
                Assert.Same(value1, events[3].AffectedControl);
                Assert.Equal("Bounds", events[3].AffectedProperty);
                Assert.Same(value1, events[4].AffectedControl);
                Assert.Equal("Visible", events[4].AffectedProperty);
                Assert.Same(value2, events[5].AffectedControl);
                Assert.Equal("Parent", events[5].AffectedProperty);
                Assert.Same(value2, events[6].AffectedControl);
                Assert.Equal("Visible", events[6].AffectedProperty);
                Assert.Same(value2, events[7].AffectedControl);
                Assert.Equal("Bounds", events[7].AffectedProperty);
                Assert.Same(value2, events[8].AffectedControl);
                Assert.Equal("Bounds", events[8].AffectedProperty);
                Assert.True(value1.IsHandleCreated);
                Assert.False(value2.IsHandleCreated);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(expectedParentInvalidatedCallCount * 2, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Add again.
                collection.Add(value1);
                Assert.Equal(new Control[] { value2, value1 }, collection.Cast<Control>());
                Assert.Equal(new TabPage[] { value1, value2, value1 }, owner.TabPages.Cast<TabPage>());
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(owner.DisplayRectangle, value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Same(owner, value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(owner.DisplayRectangle, value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(0, owner.SelectedIndex);
                Assert.Equal(3, layoutCallCount1);
                Assert.Equal(1, layoutCallCount2);
                Assert.Equal(13, parentLayoutCallCount);
                Assert.Same(value1, events[0].AffectedControl);
                Assert.Equal("Parent", events[0].AffectedProperty);
                Assert.Same(value1, events[1].AffectedControl);
                Assert.Equal("Visible", events[1].AffectedProperty);
                Assert.Same(value1, events[2].AffectedControl);
                Assert.Equal("Bounds", events[2].AffectedProperty);
                Assert.Same(value1, events[3].AffectedControl);
                Assert.Equal("Bounds", events[3].AffectedProperty);
                Assert.Same(value1, events[4].AffectedControl);
                Assert.Equal("Visible", events[4].AffectedProperty);
                Assert.Same(value2, events[5].AffectedControl);
                Assert.Equal("Parent", events[5].AffectedProperty);
                Assert.Same(value2, events[6].AffectedControl);
                Assert.Equal("Visible", events[6].AffectedProperty);
                Assert.Same(value2, events[7].AffectedControl);
                Assert.Equal("Bounds", events[7].AffectedProperty);
                Assert.Same(value2, events[8].AffectedControl);
                Assert.Equal("Bounds", events[8].AffectedProperty);
                Assert.Same(value1, events[9].AffectedControl);
                Assert.Equal("ChildIndex", events[9].AffectedProperty);
                Assert.Same(value1, events[10].AffectedControl);
                Assert.Equal("Visible", events[10].AffectedProperty);
                Assert.Same(value1, events[11].AffectedControl);
                Assert.Equal("Visible", events[11].AffectedProperty);
                Assert.Same(value1, events[12].AffectedControl);
                Assert.Equal("Visible", events[12].AffectedProperty);
                Assert.True(value1.IsHandleCreated);
                Assert.False(value2.IsHandleCreated);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(expectedParentInvalidatedCallCount * 3, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_TestData))]
        public void TabControlControlCollection_Add_InvokeValueWithHandleOwnerWithoutHandle_Success(TabAppearance appearance)
        {
            using var owner = new TabControl
            {
                Appearance = appearance,
                Bounds = new Rectangle(0, 0, 400, 300)
            };
            using var value1 = new TabPage();
            using var value2 = new TabPage();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);

            int layoutCallCount1 = 0;
            value1.Layout += (sender, e) => layoutCallCount1++;
            int layoutCallCount2 = 0;
            value2.Layout += (sender, e) => layoutCallCount2++;
            int parentLayoutCallCount = 0;
            var events = new List<LayoutEventArgs>();
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                events.Add(e);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, value1.Handle);
            int invalidatedCallCount1 = 0;
            value1.Invalidated += (sender, e) => invalidatedCallCount1++;
            int styleChangedCallCount1 = 0;
            value1.StyleChanged += (sender, e) => styleChangedCallCount1++;
            int createdCallCount1 = 0;
            value1.HandleCreated += (sender, e) => createdCallCount1++;
            Assert.NotEqual(IntPtr.Zero, value2.Handle);
            int invalidatedCallCount2 = 0;
            value2.Invalidated += (sender, e) => invalidatedCallCount2++;
            int styleChangedCallCount2 = 0;
            value2.StyleChanged += (sender, e) => styleChangedCallCount2++;
            int createdCallCount2 = 0;
            value2.HandleCreated += (sender, e) => createdCallCount2++;

            try
            {
                // Add first.
                collection.Add(value1);
                Assert.Same(value1, Assert.Single(collection));
                Assert.Same(value1, Assert.Single(owner.TabPages));
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.Same(value1, events[0].AffectedControl);
                Assert.Equal("Parent", events[0].AffectedProperty);
                Assert.Same(value1, events[1].AffectedControl);
                Assert.Equal("Visible", events[1].AffectedProperty);
                Assert.True(value1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount1);
                Assert.Equal(0, styleChangedCallCount1);
                Assert.Equal(0, createdCallCount1);
                Assert.False(owner.IsHandleCreated);

                // Add another.
                collection.Add(value2);
                Assert.Equal(new Control[] { value1, value2 }, collection.Cast<Control>());
                Assert.Equal(new TabPage[] { value1, value2 }, owner.TabPages.Cast<TabPage>());
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Same(owner, value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(4, parentLayoutCallCount);
                Assert.Same(value1, events[0].AffectedControl);
                Assert.Equal("Parent", events[0].AffectedProperty);
                Assert.Same(value1, events[1].AffectedControl);
                Assert.Equal("Visible", events[1].AffectedProperty);
                Assert.Same(value2, events[2].AffectedControl);
                Assert.Equal("Parent", events[2].AffectedProperty);
                Assert.Same(value2, events[3].AffectedControl);
                Assert.Equal("Visible", events[3].AffectedProperty);
                Assert.True(value1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount1);
                Assert.Equal(0, styleChangedCallCount1);
                Assert.Equal(0, createdCallCount1);
                Assert.True(value2.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount2);
                Assert.Equal(0, styleChangedCallCount2);
                Assert.Equal(0, createdCallCount2);
                Assert.False(owner.IsHandleCreated);

                // Add again.
                collection.Add(value1);
                Assert.Equal(new Control[] { value2, value1 }, collection.Cast<Control>());
                Assert.Equal(new TabPage[] { value1, value2, value1 }, owner.TabPages.Cast<TabPage>());
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Same(owner, value2.Parent);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(5, parentLayoutCallCount);
                Assert.Same(value1, events[0].AffectedControl);
                Assert.Equal("Parent", events[0].AffectedProperty);
                Assert.Same(value1, events[1].AffectedControl);
                Assert.Equal("Visible", events[1].AffectedProperty);
                Assert.Same(value2, events[2].AffectedControl);
                Assert.Equal("Parent", events[2].AffectedProperty);
                Assert.Same(value2, events[3].AffectedControl);
                Assert.Equal("Visible", events[3].AffectedProperty);
                Assert.Same(value1, events[4].AffectedControl);
                Assert.Equal("ChildIndex", events[4].AffectedProperty);
                Assert.True(value1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount1);
                Assert.Equal(0, styleChangedCallCount1);
                Assert.Equal(0, createdCallCount1);
                Assert.True(value2.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount2);
                Assert.Equal(0, styleChangedCallCount2);
                Assert.Equal(0, createdCallCount2);
                Assert.False(owner.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Add_WithHandle_TestData))]
        public void TabControlControlCollection_Add_InvokeValueWithHandleOwnerWithHandle_Success(TabAppearance appearance, Size itemSize, int expectedParentInvalidatedCallCount)
        {
            using var owner = new TabControl
            {
                Appearance = appearance,
                ItemSize = itemSize,
                Bounds = new Rectangle(0, 0, 400, 300)
            };
            using var value1 = new TabPage();
            using var value2 = new TabPage();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);

            int layoutCallCount1 = 0;
            value1.Layout += (sender, e) => layoutCallCount1++;
            int layoutCallCount2 = 0;
            value2.Layout += (sender, e) => layoutCallCount2++;
            int parentLayoutCallCount = 0;
            var events = new List<LayoutEventArgs>();
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                events.Add(e);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, value1.Handle);
            int invalidatedCallCount1 = 0;
            value1.Invalidated += (sender, e) => invalidatedCallCount1++;
            int styleChangedCallCount1 = 0;
            value1.StyleChanged += (sender, e) => styleChangedCallCount1++;
            int createdCallCount1 = 0;
            value1.HandleCreated += (sender, e) => createdCallCount1++;
            Assert.NotEqual(IntPtr.Zero, value2.Handle);
            int invalidatedCallCount2 = 0;
            value2.Invalidated += (sender, e) => invalidatedCallCount2++;
            int styleChangedCallCount2 = 0;
            value2.StyleChanged += (sender, e) => styleChangedCallCount2++;
            int createdCallCount2 = 0;
            value2.HandleCreated += (sender, e) => createdCallCount2++;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int parentInvalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

            try
            {
                // Add first.
                collection.Add(value1);
                Assert.Same(value1, Assert.Single(collection));
                Assert.Same(value1, Assert.Single(owner.TabPages));
                Assert.Same(owner, value1.Parent);
                Assert.True(value1.Visible);
                Assert.Equal(owner.DisplayRectangle, value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Equal(0, owner.SelectedIndex);
                Assert.Equal(2, layoutCallCount1);
                Assert.Equal(5, parentLayoutCallCount);
                Assert.Same(value1, events[0].AffectedControl);
                Assert.Equal("Parent", events[0].AffectedProperty);
                Assert.Same(value1, events[1].AffectedControl);
                Assert.Equal("Visible", events[1].AffectedProperty);
                Assert.Same(value1, events[2].AffectedControl);
                Assert.Equal("Bounds", events[2].AffectedProperty);
                Assert.Same(value1, events[3].AffectedControl);
                Assert.Equal("Bounds", events[3].AffectedProperty);
                Assert.Same(value1, events[4].AffectedControl);
                Assert.Equal("Visible", events[4].AffectedProperty);
                Assert.True(value1.IsHandleCreated);
                Assert.Equal(1, invalidatedCallCount1);
                Assert.Equal(0, styleChangedCallCount1);
                Assert.Equal(0, createdCallCount1);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(expectedParentInvalidatedCallCount, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Add another.
                collection.Add(value2);
                Assert.Equal(new Control[] { value1, value2 }, collection.Cast<Control>());
                Assert.Equal(new TabPage[] { value1, value2 }, owner.TabPages.Cast<TabPage>());
                Assert.Same(owner, value1.Parent);
                Assert.True(value1.Visible);
                Assert.Equal(owner.DisplayRectangle, value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Same(owner, value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(owner.DisplayRectangle, value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(0, owner.SelectedIndex);
                Assert.Equal(2, layoutCallCount1);
                Assert.Equal(1, layoutCallCount2);
                Assert.Equal(9, parentLayoutCallCount);
                Assert.Same(value1, events[0].AffectedControl);
                Assert.Equal("Parent", events[0].AffectedProperty);
                Assert.Same(value1, events[1].AffectedControl);
                Assert.Equal("Visible", events[1].AffectedProperty);
                Assert.Same(value1, events[2].AffectedControl);
                Assert.Equal("Bounds", events[2].AffectedProperty);
                Assert.Same(value1, events[3].AffectedControl);
                Assert.Equal("Bounds", events[3].AffectedProperty);
                Assert.Same(value1, events[4].AffectedControl);
                Assert.Equal("Visible", events[4].AffectedProperty);
                Assert.Same(value2, events[5].AffectedControl);
                Assert.Equal("Parent", events[5].AffectedProperty);
                Assert.Same(value2, events[6].AffectedControl);
                Assert.Equal("Visible", events[6].AffectedProperty);
                Assert.Same(value2, events[7].AffectedControl);
                Assert.Equal("Bounds", events[7].AffectedProperty);
                Assert.Same(value2, events[8].AffectedControl);
                Assert.Equal("Bounds", events[8].AffectedProperty);
                Assert.True(value1.IsHandleCreated);
                Assert.Equal(2, invalidatedCallCount1);
                Assert.Equal(0, styleChangedCallCount1);
                Assert.Equal(0, createdCallCount1);
                Assert.True(value2.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount2);
                Assert.Equal(0, styleChangedCallCount2);
                Assert.Equal(0, createdCallCount2);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(expectedParentInvalidatedCallCount * 2, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Add again.
                collection.Add(value1);
                Assert.Equal(new Control[] { value2, value1 }, collection.Cast<Control>());
                Assert.Equal(new TabPage[] { value1, value2, value1 }, owner.TabPages.Cast<TabPage>());
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(owner.DisplayRectangle, value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Same(owner, value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(owner.DisplayRectangle, value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(0, owner.SelectedIndex);
                Assert.Equal(3, layoutCallCount1);
                Assert.Equal(1, layoutCallCount2);
                Assert.Equal(13, parentLayoutCallCount);
                Assert.Same(value1, events[0].AffectedControl);
                Assert.Equal("Parent", events[0].AffectedProperty);
                Assert.Same(value1, events[1].AffectedControl);
                Assert.Equal("Visible", events[1].AffectedProperty);
                Assert.Same(value1, events[2].AffectedControl);
                Assert.Equal("Bounds", events[2].AffectedProperty);
                Assert.Same(value1, events[3].AffectedControl);
                Assert.Equal("Bounds", events[3].AffectedProperty);
                Assert.Same(value1, events[4].AffectedControl);
                Assert.Equal("Visible", events[4].AffectedProperty);
                Assert.Same(value2, events[5].AffectedControl);
                Assert.Equal("Parent", events[5].AffectedProperty);
                Assert.Same(value2, events[6].AffectedControl);
                Assert.Equal("Visible", events[6].AffectedProperty);
                Assert.Same(value2, events[7].AffectedControl);
                Assert.Equal("Bounds", events[7].AffectedProperty);
                Assert.Same(value2, events[8].AffectedControl);
                Assert.Equal("Bounds", events[8].AffectedProperty);
                Assert.Same(value1, events[9].AffectedControl);
                Assert.Equal("ChildIndex", events[9].AffectedProperty);
                Assert.Same(value1, events[10].AffectedControl);
                Assert.Equal("Visible", events[10].AffectedProperty);
                Assert.Same(value1, events[11].AffectedControl);
                Assert.Equal("Visible", events[11].AffectedProperty);
                Assert.Same(value1, events[12].AffectedControl);
                Assert.Equal("Visible", events[12].AffectedProperty);
                Assert.True(value1.IsHandleCreated);
                Assert.Equal(3, invalidatedCallCount1);
                Assert.Equal(0, styleChangedCallCount1);
                Assert.Equal(0, createdCallCount1);
                Assert.True(value2.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount2);
                Assert.Equal(0, styleChangedCallCount2);
                Assert.Equal(0, createdCallCount2);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(expectedParentInvalidatedCallCount * 3, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void TabControlControlCollection_Add_OwnerWithSiteContainer_AddsToContainer()
        {
            var container = new Container();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.Container)
                .Returns(container);
            using var value = new TabPage();
            using var owner = new TabControl
            {
                Site = mockSite.Object
            };
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);
            collection.Add(value);
            Assert.Same(owner, value.Parent);
            Assert.NotNull(value.Site);
            Assert.Same(value, Assert.Single(container.Components));
            mockSite.Verify(s => s.Container, Times.Once());
        }

        [WinFormsFact]
        public void TabControlControlCollection_Add_OwnerWithSiteContainerValueHasSite_DoesNotAddToContainer()
        {
            var container = new Container();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.Container)
                .Returns(container);
            var mockValueSite = new Mock<ISite>(MockBehavior.Strict);
            mockValueSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockValueSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            using var value = new TabPage
            {
                Site = mockValueSite.Object
            };
            using var owner = new TabControl
            {
                Site = mockSite.Object
            };
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);
            collection.Add(value);
            Assert.Same(owner, value.Parent);
            Assert.Same(mockValueSite.Object, value.Site);
            Assert.Empty(container.Components);
            mockSite.Verify(s => s.Container, Times.Never());
        }

        [WinFormsFact]
        public void TabControlControlCollection_Add_OwnerWithSiteNoContainer_DoesNotAddContainer()
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            using var value = new TabPage();
            using var owner = new TabControl
            {
                Site = mockSite.Object
            };
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);
            collection.Add(value);
            Assert.Same(owner, value.Parent);
            Assert.Null(value.Site);
            mockSite.Verify(s => s.Container, Times.Once());
        }

        [WinFormsFact]
        public void TabControlControlCollection_Add_ManyControls_Success()
        {
            using var owner = new TabControl();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);

            var items = new List<TabPage>();
            for (int i = 0; i < 24; i++)
            {
                var value = new TabPage();
                items.Add(value);
                collection.Add(value);
                Assert.Equal(items, collection.Cast<Control>());
                Assert.Equal(items, owner.TabPages.Cast<TabPage>());
                Assert.Same(owner, value.Parent);
            }
        }

        [WinFormsTheory]
        [InlineData("Text", "Text")]
        [InlineData("&&Text", "&&Text")]
        [InlineData("&", "&&")]
        [InlineData("&Text", "&&Text")]
        public unsafe void TabControlControlCollection_Add_GetItemsWithHandle_Success(string text, string expectedText)
        {
            using var owner = new TabControl();
            Assert.NotEqual(IntPtr.Zero, owner.Handle);

            using var page1 = new TabPage();
            using var page2 = new TabPage
            {
                Text = text,
                ImageIndex = 1
            };
            using var page3 = new NullTextTabPage();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);
            collection.Add(page1);
            collection.Add(page2);
            collection.Add(page3);
            Assert.Equal((IntPtr)3, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero));

            char* buffer = stackalloc char[256];
            ComCtl32.TCITEMW item = default;
            item.cchTextMax = int.MaxValue;
            item.pszText = buffer;
            item.dwStateMask = (ComCtl32.TCIS)uint.MaxValue;
            item.mask = (ComCtl32.TCIF)uint.MaxValue;

            // Get item 0.
            Assert.Equal((IntPtr)1, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)0, ref item));
            Assert.Equal(ComCtl32.TCIS.BUTTONPRESSED, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Empty(new string(item.pszText));
            Assert.Equal(-1, item.iImage);

            // Get item 1.
            Assert.Equal((IntPtr)1, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)1, ref item));
            Assert.Equal((ComCtl32.TCIS)0, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Equal(expectedText, new string(item.pszText));
            Assert.Equal(1, item.iImage);

            // Get item 2.
            Assert.Equal((IntPtr)1, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)2, ref item));
            Assert.Equal((ComCtl32.TCIS)0, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Empty(new string(item.pszText));
            Assert.Equal(-1, item.iImage);
        }

        [WinFormsTheory]
        [InlineData("Text", "Text")]
        [InlineData("&&Text", "&&Text")]
        [InlineData("&", "&&")]
        [InlineData("&Text", "&&Text")]
        public unsafe void TabControlControlCollection_Add_GetItemsDesignModeWithHandle_Success(string text, string expectedText)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            using var owner = new TabControl
            {
                Site = mockSite.Object
            };
            Assert.NotEqual(IntPtr.Zero, owner.Handle);

            using var page1 = new TabPage();
            using var page2 = new TabPage
            {
                Text = text,
                ImageIndex = 1
            };
            using var page3 = new NullTextTabPage();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);
            collection.Add(page1);
            collection.Add(page2);
            collection.Add(page3);
            Assert.Equal((IntPtr)3, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero));

            char* buffer = stackalloc char[256];
            ComCtl32.TCITEMW item = default;
            item.cchTextMax = int.MaxValue;
            item.pszText = buffer;
            item.dwStateMask = (ComCtl32.TCIS)uint.MaxValue;
            item.mask = (ComCtl32.TCIF)uint.MaxValue;

            // Get item 0.
            Assert.Equal((IntPtr)1, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)0, ref item));
            Assert.Equal(ComCtl32.TCIS.BUTTONPRESSED, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Empty(new string(item.pszText));
            Assert.Equal(-1, item.iImage);

            // Get item 1.
            Assert.Equal((IntPtr)1, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)1, ref item));
            Assert.Equal((ComCtl32.TCIS)0, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Equal(expectedText, new string(item.pszText));
            Assert.Equal(1, item.iImage);

            // Get item 2.
            Assert.Equal((IntPtr)1, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)2, ref item));
            Assert.Equal((ComCtl32.TCIS)0, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Empty(new string(item.pszText));
            Assert.Equal(-1, item.iImage);
        }

        [WinFormsFact]
        public void TabControlControlCollection_Add_InvalidValue_ThrowsArgumentException()
        {
            using var owner = new TabControl();
            var collection = new TabControl.ControlCollection(owner);
            using var value = new Control();
            Assert.Throws<ArgumentException>(null, () => collection.Add(value));
        }

        [WinFormsFact]
        public void TabControlControlCollection_Add_NullValue_ThrowsNullReferenceException()
        {
            using var owner = new TabControl();
            var collection = new TabControl.ControlCollection(owner);
            Assert.Throws<NullReferenceException>(() => collection.Add(null));
        }

        [WinFormsFact]
        public void TabControlControlCollection_Remove_InvokeValueWithoutHandleOwnerWithoutHandle_Success()
        {
            using var owner = new TabControl
            {
                Bounds = new Rectangle(0, 0, 400, 300)
            };
            using var value1 = new TabPage();
            using var value2 = new TabPage();
            var collection = new TabControl.ControlCollection(owner);
            collection.Add(value1);
            collection.Add(value2);

            int layoutCallCount1 = 0;
            value1.Layout += (sender, e) => layoutCallCount1++;
            int layoutCallCount2 = 0;
            value2.Layout += (sender, e) => layoutCallCount2++;
            int parentLayoutCallCount = 0;
            var events = new List<LayoutEventArgs>();
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                events.Add(e);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;

            try
            {
                // Remove last.
                collection.Remove(value2);
                Assert.Same(value1, Assert.Single(collection));
                Assert.Same(value1, Assert.Single(owner.TabPages));
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Null(value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(value1.IsHandleCreated);
                Assert.False(value2.IsHandleCreated);
                Assert.False(owner.IsHandleCreated);

                // Remove again.
                collection.Remove(value2);
                Assert.Same(value1, Assert.Single(collection));
                Assert.Same(value1, Assert.Single(owner.TabPages));
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Null(value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.False(value1.IsHandleCreated);
                Assert.False(value2.IsHandleCreated);
                Assert.False(owner.IsHandleCreated);

                // Remove first.
                collection.Remove(value1);
                Assert.Empty(collection);
                Assert.Empty(owner.TabPages);
                Assert.Null(value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Null(value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.False(value1.IsHandleCreated);
                Assert.False(value2.IsHandleCreated);
                Assert.False(owner.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void TabControlControlCollection_Remove_InvokeValueWithHandleOwnerWithoutHandle_Success()
        {
            using var owner = new TabControl
            {
                Bounds = new Rectangle(0, 0, 400, 300)
            };
            using var value1 = new TabPage();
            using var value2 = new TabPage();
            var collection = new TabControl.ControlCollection(owner);
            collection.Add(value1);
            collection.Add(value2);

            int layoutCallCount1 = 0;
            value1.Layout += (sender, e) => layoutCallCount1++;
            int layoutCallCount2 = 0;
            value2.Layout += (sender, e) => layoutCallCount2++;
            int parentLayoutCallCount = 0;
            var events = new List<LayoutEventArgs>();
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                events.Add(e);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, value1.Handle);
            int invalidatedCallCount1 = 0;
            value1.Invalidated += (sender, e) => invalidatedCallCount1++;
            int styleChangedCallCount1 = 0;
            value1.StyleChanged += (sender, e) => styleChangedCallCount1++;
            int createdCallCount1 = 0;
            value1.HandleCreated += (sender, e) => createdCallCount1++;
            Assert.NotEqual(IntPtr.Zero, value2.Handle);
            int invalidatedCallCount2 = 0;
            value2.Invalidated += (sender, e) => invalidatedCallCount2++;
            int styleChangedCallCount2 = 0;
            value2.StyleChanged += (sender, e) => styleChangedCallCount2++;
            int createdCallCount2 = 0;
            value2.HandleCreated += (sender, e) => createdCallCount2++;

            try
            {
                // Remove last.
                collection.Remove(value2);
                Assert.Same(value1, Assert.Single(collection));
                Assert.Same(value1, Assert.Single(owner.TabPages));
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Null(value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.True(value1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount1);
                Assert.Equal(0, styleChangedCallCount1);
                Assert.Equal(0, createdCallCount1);
                Assert.True(value2.IsHandleCreated);
                Assert.False(owner.IsHandleCreated);

                // Remove again.
                collection.Remove(value2);
                Assert.Same(value1, Assert.Single(collection));
                Assert.Same(value1, Assert.Single(owner.TabPages));
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Null(value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.True(value1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount1);
                Assert.Equal(0, styleChangedCallCount1);
                Assert.Equal(0, createdCallCount1);
                Assert.True(value2.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount2);
                Assert.Equal(0, styleChangedCallCount2);
                Assert.Equal(0, createdCallCount2);
                Assert.False(owner.IsHandleCreated);

                // Remove first.
                collection.Remove(value1);
                Assert.Empty(collection);
                Assert.Empty(owner.TabPages);
                Assert.Null(value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Null(value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.True(value1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount1);
                Assert.Equal(0, styleChangedCallCount1);
                Assert.Equal(0, createdCallCount1);
                Assert.True(value2.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount2);
                Assert.Equal(0, styleChangedCallCount2);
                Assert.Equal(0, createdCallCount2);
                Assert.False(owner.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void TabControlControlCollection_Remove_InvokeValueWithoutHandleOwnerWithHandle_Success()
        {
            using var owner = new TabControl
            {
                Bounds = new Rectangle(0, 0, 400, 300)
            };
            using var value1 = new TabPage();
            using var value2 = new TabPage();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);
            collection.Add(value1);
            collection.Add(value2);

            int layoutCallCount1 = 0;
            value1.Layout += (sender, e) => layoutCallCount1++;
            int layoutCallCount2 = 0;
            value2.Layout += (sender, e) => layoutCallCount2++;
            int parentLayoutCallCount = 0;
            var events = new List<LayoutEventArgs>();
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                events.Add(e);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            int parentInvalidatedCallCount = 0;
            owner.Invalidated += (sender, e) => parentInvalidatedCallCount++;
            int parentStyleChangedCallCount = 0;
            owner.StyleChanged += (sender, e) => parentStyleChangedCallCount++;
            int parentCreatedCallCount = 0;
            owner.HandleCreated += (sender, e) => parentCreatedCallCount++;

            try
            {
                // Remove last.
                collection.Remove(value2);
                Assert.Same(value1, Assert.Single(collection));
                Assert.Same(value1, Assert.Single(owner.TabPages));
                Assert.Same(owner, value1.Parent);
                Assert.True(value1.Visible);
                Assert.Equal(owner.DisplayRectangle, value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Null(value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(owner.DisplayRectangle, value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(0, owner.SelectedIndex);
                Assert.Equal(2, layoutCallCount1);
                Assert.Equal(1, layoutCallCount2);
                Assert.Equal(6, parentLayoutCallCount);
                Assert.True(value1.IsHandleCreated);
                Assert.False(value2.IsHandleCreated);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Remove again.
                collection.Remove(value2);
                Assert.Same(value1, Assert.Single(collection));
                Assert.Same(value1, Assert.Single(owner.TabPages));
                Assert.Same(owner, value1.Parent);
                Assert.True(value1.Visible);
                Assert.Equal(owner.DisplayRectangle, value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Null(value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(owner.DisplayRectangle, value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(0, owner.SelectedIndex);
                Assert.Equal(2, layoutCallCount1);
                Assert.Equal(1, layoutCallCount2);
                Assert.Equal(6, parentLayoutCallCount);
                Assert.True(value1.IsHandleCreated);
                Assert.False(value2.IsHandleCreated);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);

                // Remove first.
                collection.Remove(value1);
                Assert.Empty(collection);
                Assert.Empty(owner.TabPages);
                Assert.Null(value1.Parent);
                Assert.True(value1.Visible);
                Assert.Equal(new Rectangle(4, 24, 392, 272), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Null(value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(4, 24, 392, 272), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(3, layoutCallCount1);
                Assert.Equal(1, layoutCallCount2);
                Assert.Equal(7, parentLayoutCallCount);
                Assert.True(value1.IsHandleCreated);
                Assert.False(value2.IsHandleCreated);
                Assert.True(owner.IsHandleCreated);
                Assert.Equal(0, parentInvalidatedCallCount);
                Assert.Equal(0, parentStyleChangedCallCount);
                Assert.Equal(0, parentCreatedCallCount);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void TabControlControlCollection_Remove_InvokeValueWithHandleOwnerWithHandle_Success()
        {
            using var owner = new TabControl
            {
                Bounds = new Rectangle(0, 0, 400, 300)
            };
            using var value1 = new TabPage();
            using var value2 = new TabPage();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);
            collection.Add(value1);
            collection.Add(value2);

            int layoutCallCount1 = 0;
            value1.Layout += (sender, e) => layoutCallCount1++;
            int layoutCallCount2 = 0;
            value2.Layout += (sender, e) => layoutCallCount2++;
            int parentLayoutCallCount = 0;
            var events = new List<LayoutEventArgs>();
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(owner, sender);
                events.Add(e);
                parentLayoutCallCount++;
            }
            owner.Layout += parentHandler;
            Assert.NotEqual(IntPtr.Zero, value1.Handle);
            int invalidatedCallCount1 = 0;
            value1.Invalidated += (sender, e) => invalidatedCallCount1++;
            int styleChangedCallCount1 = 0;
            value1.StyleChanged += (sender, e) => styleChangedCallCount1++;
            int createdCallCount1 = 0;
            value1.HandleCreated += (sender, e) => createdCallCount1++;
            Assert.NotEqual(IntPtr.Zero, value2.Handle);
            int invalidatedCallCount2 = 0;
            value2.Invalidated += (sender, e) => invalidatedCallCount2++;
            int styleChangedCallCount2 = 0;
            value2.StyleChanged += (sender, e) => styleChangedCallCount2++;
            int createdCallCount2 = 0;
            value2.HandleCreated += (sender, e) => createdCallCount2++;

            try
            {
                // Remove last.
                collection.Remove(value2);
                Assert.Same(value1, Assert.Single(collection));
                Assert.Same(value1, Assert.Single(owner.TabPages));
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Null(value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.True(value1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount1);
                Assert.Equal(0, styleChangedCallCount1);
                Assert.Equal(0, createdCallCount1);
                Assert.True(value2.IsHandleCreated);
                Assert.False(owner.IsHandleCreated);

                // Remove again.
                collection.Remove(value2);
                Assert.Same(value1, Assert.Single(collection));
                Assert.Same(value1, Assert.Single(owner.TabPages));
                Assert.Same(owner, value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Null(value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(1, parentLayoutCallCount);
                Assert.True(value1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount1);
                Assert.Equal(0, styleChangedCallCount1);
                Assert.Equal(0, createdCallCount1);
                Assert.True(value2.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount2);
                Assert.Equal(0, styleChangedCallCount2);
                Assert.Equal(0, createdCallCount2);
                Assert.False(owner.IsHandleCreated);

                // Remove first.
                collection.Remove(value1);
                Assert.Empty(collection);
                Assert.Empty(owner.TabPages);
                Assert.Null(value1.Parent);
                Assert.False(value1.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value1.Bounds);
                Assert.Null(value1.Site);
                Assert.Null(value2.Parent);
                Assert.False(value2.Visible);
                Assert.Equal(new Rectangle(0, 0, 200, 100), value2.Bounds);
                Assert.Null(value2.Site);
                Assert.Equal(-1, owner.SelectedIndex);
                Assert.Equal(0, layoutCallCount1);
                Assert.Equal(0, layoutCallCount2);
                Assert.Equal(2, parentLayoutCallCount);
                Assert.True(value1.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount1);
                Assert.Equal(0, styleChangedCallCount1);
                Assert.Equal(0, createdCallCount1);
                Assert.True(value2.IsHandleCreated);
                Assert.Equal(0, invalidatedCallCount2);
                Assert.Equal(0, styleChangedCallCount2);
                Assert.Equal(0, createdCallCount2);
                Assert.False(owner.IsHandleCreated);
            }
            finally
            {
                owner.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void TabControlControlCollection_Remove_SelectedTabWithoutHandle_SetsSelectedToZero()
        {
            using var owner = new TabControl();
            using var value1 = new TabPage();
            using var value2 = new TabPage();
            using var value3 = new TabPage();
            using var value4 = new TabPage();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);
            collection.Add(value1);
            collection.Add(value2);
            collection.Add(value3);
            collection.Add(value4);
            owner.SelectedTab = value4;
            Assert.Same(value4, owner.SelectedTab);

            // Remove other.
            collection.Remove(value2);
            Assert.Equal(new Control[] { value1, value3, value4 }, collection.Cast<TabPage>());
            Assert.Null(owner.SelectedTab);

            // Remove selected.
            collection.Remove(value4);
            Assert.Equal(new Control[] { value1, value3 }, collection.Cast<TabPage>());
            Assert.Null(owner.SelectedTab);

            // Remove selected again.
            collection.Remove(value1);
            Assert.Equal(new Control[] { value3 }, collection.Cast<TabPage>());
            Assert.Null(owner.SelectedTab);

            // Remove selected again.
            collection.Remove(value3);
            Assert.Empty(collection);
            Assert.Null(owner.SelectedTab);
        }

        [WinFormsFact]
        public void TabControlControlCollection_Remove_SelectedTabWithHandle_SetsSelectedToZero()
        {
            using var owner = new TabControl();
            using var value1 = new TabPage();
            using var value2 = new TabPage();
            using var value3 = new TabPage();
            using var value4 = new TabPage();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);
            collection.Add(value1);
            collection.Add(value2);
            collection.Add(value3);
            collection.Add(value4);
            owner.SelectedTab = value4;
            Assert.Same(value4, owner.SelectedTab);
            Assert.NotEqual(IntPtr.Zero, owner.Handle);

            // Remove other.
            collection.Remove(value2);
            Assert.Equal(new Control[] { value1, value3, value4 }, collection.Cast<TabPage>());
            Assert.Same(value4, owner.SelectedTab);

            // Remove selected.
            collection.Remove(value4);
            Assert.Equal(new Control[] { value1, value3 }, collection.Cast<TabPage>());
            Assert.Same(value1, owner.SelectedTab);

            // Remove selected again.
            collection.Remove(value1);
            Assert.Equal(new Control[] { value3 }, collection.Cast<TabPage>());
            Assert.Same(value3, owner.SelectedTab);

            // Remove selected again.
            collection.Remove(value3);
            Assert.Empty(collection);
            Assert.Null(owner.SelectedTab);
        }

        [WinFormsFact]
        public void TabControlControlCollection_Remove_ManyControls_Success()
        {
            using var owner = new TabControl();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);

            var items = new List<TabPage>();
            for (int i = 0; i < 24; i++)
            {
                var value = new TabPage();
                items.Add(value);
                collection.Add(value);
                Assert.Equal(items, collection.Cast<Control>());
                Assert.Equal(items, owner.TabPages.Cast<TabPage>());
                Assert.Same(owner, value.Parent);
            }

            for (int i = 0; i < 24; i++)
            {
                items.RemoveAt(0);
                collection.Remove(collection[0]);
                Assert.Equal(items, collection.Cast<Control>());
                Assert.Equal(items, owner.TabPages.Cast<TabPage>());
            }
        }

        [WinFormsFact]
        public void TabControlControlCollection_Remove_NoSuchControl_Nop()
        {
            using var owner = new TabControl();
            var collection = new TabControl.ControlCollection(owner);
            using var value1 = new Control();
            using var value2 = new Control();
            collection.Remove(null);
            collection.Remove(value1);
            collection.Remove(value2);
        }

        [WinFormsFact]
        public unsafe void TabControlControlCollection_Remove_GetItemsWithHandle_Success()
        {
            using var owner = new TabControl();
            using var page1 = new TabPage();
            using var page2 = new TabPage
            {
                Text = "Text",
                ImageIndex = 1
            };
            using var page3 = new NullTextTabPage();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);
            collection.Add(page1);
            collection.Add(page2);
            collection.Add(page3);

            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            collection.Remove(page2);
            Assert.Equal((IntPtr)2, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero));

            char* buffer = stackalloc char[256];
            ComCtl32.TCITEMW item = default;
            item.cchTextMax = int.MaxValue;
            item.pszText = buffer;
            item.dwStateMask = (ComCtl32.TCIS)uint.MaxValue;
            item.mask = (ComCtl32.TCIF)uint.MaxValue;

            // Get item 0.
            Assert.Equal((IntPtr)1, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)0, ref item));
            Assert.Equal(ComCtl32.TCIS.BUTTONPRESSED, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Empty(new string(item.pszText));
            Assert.Equal(-1, item.iImage);

            // Get item 2.
            Assert.Equal((IntPtr)1, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)1, ref item));
            Assert.Equal((ComCtl32.TCIS)0, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Empty(new string(item.pszText));
            Assert.Equal(-1, item.iImage);
        }

        [WinFormsFact]
        public unsafe void TabControlControlCollection_Remove_GetItemsDesignModeWithHandle_Success()
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            using var owner = new TabControl
            {
                Site = mockSite.Object
            };

            using var page1 = new TabPage();
            using var page2 = new TabPage
            {
                Text = "Text",
                ImageIndex = 1
            };
            using var page3 = new NullTextTabPage();
            TabControl.ControlCollection collection = Assert.IsType<TabControl.ControlCollection>(owner.Controls);
            collection.Add(page1);
            collection.Add(page2);
            collection.Add(page3);

            Assert.NotEqual(IntPtr.Zero, owner.Handle);
            collection.Remove(page2);
            Assert.Equal((IntPtr)2, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMCOUNT, IntPtr.Zero, IntPtr.Zero));

            char* buffer = stackalloc char[256];
            ComCtl32.TCITEMW item = default;
            item.cchTextMax = int.MaxValue;
            item.pszText = buffer;
            item.dwStateMask = (ComCtl32.TCIS)uint.MaxValue;
            item.mask = (ComCtl32.TCIF)uint.MaxValue;

            // Get item 0.
            Assert.Equal((IntPtr)1, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)0, ref item));
            Assert.Equal(ComCtl32.TCIS.BUTTONPRESSED, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Empty(new string(item.pszText));
            Assert.Equal(-1, item.iImage);

            // Get item 1.
            Assert.Equal((IntPtr)1, User32.SendMessageW(owner.Handle, (User32.WM)ComCtl32.TCM.GETITEMW, (IntPtr)1, ref item));
            Assert.Equal((ComCtl32.TCIS)0, item.dwState);
            Assert.Equal(IntPtr.Zero, item.lParam);
            Assert.Equal(int.MaxValue, item.cchTextMax);
            Assert.Empty(new string(item.pszText));
            Assert.Equal(-1, item.iImage);
        }

        private class NullTextTabPage : TabPage
        {
            public override string Text
            {
                get => null;
                set { }
            }
        }
    }
}
