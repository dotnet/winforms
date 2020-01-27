// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Layout;
using Moq;
using WinForms.Common.Tests;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Tests
{
    using Point = System.Drawing.Point;
    using Size = System.Drawing.Size;

    public class StatusBarPanelTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void StatusBarPanel_Ctor_Default()
        {
            using var panel = new SubStatusBarPanel();
            Assert.Equal(HorizontalAlignment.Left, panel.Alignment);
            Assert.Equal(StatusBarPanelAutoSize.None, panel.AutoSize);
            Assert.Equal(StatusBarPanelBorderStyle.Sunken, panel.BorderStyle);
            Assert.True(panel.CanRaiseEvents);
            Assert.Null(panel.Container);
            Assert.False(panel.DesignMode);
            Assert.NotNull(panel.Events);
            Assert.Same(panel.Events, panel.Events);
            Assert.Null(panel.Icon);
            Assert.Equal(10, panel.MinWidth);
            Assert.Empty(panel.Name);
            Assert.Null(panel.Parent);
            Assert.Null(panel.Site);
            Assert.Equal(StatusBarPanelStyle.Text, panel.Style);
            Assert.Null(panel.Tag);
            Assert.Empty(panel.Text);
            Assert.Empty(panel.ToolTipText);
            Assert.Equal(100, panel.Width);
        }

        public static IEnumerable<object[]> Icon_Set_TestData()
        {
            foreach (StatusBarPanelAutoSize autoSize in Enum.GetValues(typeof(StatusBarPanelAutoSize)))
            {
                yield return new object[] { autoSize, new Icon("bitmaps/10x16_one_entry_32bit.ico"), new Size(10, 16) };
                yield return new object[] { autoSize, new Icon("bitmaps/256x256_one_entry_32bit.ico"), new Size(256, 256) };
                yield return new object[] { autoSize, null, null };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Icon_Set_TestData))]
        public void StatusBarPanel_Icon_Set_GetReturnsExpected(StatusBarPanelAutoSize autoSize, Icon value, Size? expectedSize)
        {
            using var panel = new StatusBarPanel
            {
                AutoSize = autoSize,
                Icon = value
            };
            Assert.Equal(expectedSize, panel.Icon?.Size);

            // Set same.
            panel.Icon = value;
            Assert.Equal(expectedSize, panel.Icon?.Size);
        }

        [WinFormsTheory]
        [MemberData(nameof(Icon_Set_TestData))]
        public void StatusBarPanel_Icon_SetWithNonNullOldValue_GetReturnsExpected(StatusBarPanelAutoSize autoSize, Icon value, Size? expectedSize)
        {
            using var oldValue = new Icon("bitmaps/10x16_one_entry_32bit.ico");
            using var panel = new StatusBarPanel
            {
                AutoSize = autoSize,
                Icon = oldValue
            };

            panel.Icon = value;
            Assert.Equal(expectedSize, panel.Icon?.Size);

            // Set same.
            panel.Icon = value;
            Assert.Equal(expectedSize, panel.Icon?.Size);
        }

        public static IEnumerable<object[]> Icon_SetWithParent_TestData()
        {
            foreach (bool showPanels in new bool[] { true, false })
            {
                foreach (StatusBarPanelAutoSize autoSize in Enum.GetValues(typeof(StatusBarPanelAutoSize)))
                {
                    bool create = autoSize == StatusBarPanelAutoSize.Contents;
                    yield return new object[] { showPanels, autoSize, new Icon("bitmaps/10x16_one_entry_32bit.ico"), new Size(10, 16), showPanels && create ? 1 : 0, create };
                    yield return new object[] { showPanels, autoSize, new Icon("bitmaps/256x256_one_entry_32bit.ico"), new Size(256, 256), showPanels && create ? 1 : 0, create };
                    yield return new object[] { showPanels, autoSize, null, null, 0, create };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Icon_SetWithParent_TestData))]
        public void StatusBarPanel_Icon_SetWithParent_GetReturnsExpected(bool showPanels, StatusBarPanelAutoSize autoSize, Icon value, Size? expectedSize, int expectedParentLayoutCallCount, bool expectedIsHandleCreated)
        {
            using var parent = new StatusBar
            {
                ShowPanels = showPanels
            };
            using var panel = new StatusBarPanel
            {
                AutoSize = autoSize
            };
            parent.Panels.Add(panel);

            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            try
            {
                panel.Icon = value;
                Assert.Equal(expectedSize, panel.Icon?.Size);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.Equal(expectedIsHandleCreated, parent.IsHandleCreated);

                // Set same.
                panel.Icon = value;
                Assert.Equal(expectedSize, panel.Icon?.Size);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.Equal(expectedIsHandleCreated, parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> Icon_SetWithNonNullOldValueWithParent_TestData()
        {
            foreach (bool showPanels in new bool[] { true, false })
            {
                foreach (StatusBarPanelAutoSize autoSize in Enum.GetValues(typeof(StatusBarPanelAutoSize)))
                {
                    bool create = autoSize == StatusBarPanelAutoSize.Contents;
                    yield return new object[] { showPanels, autoSize, new Icon("bitmaps/10x16_one_entry_32bit.ico"), new Size(10, 16), 0, create };
                    yield return new object[] { showPanels, autoSize, new Icon("bitmaps/256x256_one_entry_32bit.ico"), new Size(256, 256), showPanels && create ? 1 : 0, create };
                    yield return new object[] { showPanels, autoSize, null, null, showPanels && create ? 1 : 0, create };
                }
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Icon_SetWithNonNullOldValueWithParent_TestData))]
        public void StatusBarPanel_Icon_SetWithNonNullOldValueWithParent_GetReturnsExpected(bool showPanels, StatusBarPanelAutoSize autoSize, Icon value, Size? expectedSize, int expectedParentLayoutCallCount, bool expectedIsHandleCreated)
        {
            using var oldValue = new Icon("bitmaps/10x16_one_entry_32bit.ico");
            using var parent = new StatusBar
            {
                ShowPanels = showPanels
            };
            using var panel = new StatusBarPanel
            {
                AutoSize = autoSize,
                Icon = oldValue
            };
            parent.Panels.Add(panel);

            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;
            try
            {
                panel.Icon = value;
                Assert.Equal(expectedSize, panel.Icon?.Size);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.Equal(expectedIsHandleCreated, parent.IsHandleCreated);

                // Set same.
                panel.Icon = value;
                Assert.Equal(expectedSize, panel.Icon?.Size);
                Assert.Equal(expectedParentLayoutCallCount, parentLayoutCallCount);
                Assert.Equal(expectedIsHandleCreated, parent.IsHandleCreated);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> Icon_SetWithParentWithHandle_TestData()
        {
            foreach (StatusBarPanelAutoSize autoSize in Enum.GetValues(typeof(StatusBarPanelAutoSize)))
            {
                bool create = autoSize == StatusBarPanelAutoSize.Contents;
                yield return new object[] { true, autoSize, new Icon("bitmaps/10x16_one_entry_32bit.ico"), new Size(10, 16), 1, 1, create ? 1 : 2 };
                yield return new object[] { true, autoSize, new Icon("bitmaps/256x256_one_entry_32bit.ico"), new Size(256, 256), 1, 1, create ? 1 : 2 };
                yield return new object[] { true, autoSize, null, null, 1, create ? 0 : 1, create ? 0 : 2 };
                yield return new object[] { false, autoSize, new Icon("bitmaps/10x16_one_entry_32bit.ico"), new Size(10, 16), 0, 0, 0 };
                yield return new object[] { false, autoSize, new Icon("bitmaps/256x256_one_entry_32bit.ico"), new Size(256, 256), 0, 0, 0 };
                yield return new object[] { false, autoSize, null, null, 0, 0, 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Icon_SetWithParentWithHandle_TestData))]
        public void StatusBarPanel_Icon_SetWithParentWithHandle_GetReturnsExpected(bool showPanels, StatusBarPanelAutoSize autoSize, Icon value, Size? expectedSize, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
        {
            using var parent = new StatusBar
            {
                ShowPanels = showPanels
            };
            using var panel = new StatusBarPanel
            {
                AutoSize = autoSize
            };
            parent.Panels.Add(panel);
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            parent.HandleCreated += (sender, e) => createdCallCount++;

            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Null(e.AffectedControl);
                Assert.Null(e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                panel.Icon = value;
                Assert.Equal(expectedSize, panel.Icon?.Size);
                Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);

                // Set same.
                panel.Icon = value;
                Assert.Equal(expectedSize, panel.Icon?.Size);
                Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        public static IEnumerable<object[]> Icon_SetWithNonNullOldValueWithParentWithHandle_TestData()
        {
            foreach (StatusBarPanelAutoSize autoSize in Enum.GetValues(typeof(StatusBarPanelAutoSize)))
            {
                bool create = autoSize == StatusBarPanelAutoSize.Contents;
                yield return new object[] { true, autoSize, new Icon("bitmaps/10x16_one_entry_32bit.ico"), new Size(10, 16), 1, create ? 0 : 1, create ? 0 : 2 };
                yield return new object[] { true, autoSize, new Icon("bitmaps/256x256_one_entry_32bit.ico"), new Size(256, 256), 1, 1, create ? 1 : 2 };
                yield return new object[] { true, autoSize, null, null, 1, 1, create ? 1 : 2 };
                yield return new object[] { false, autoSize, new Icon("bitmaps/10x16_one_entry_32bit.ico"), new Size(10, 16), 0, 0, 0 };
                yield return new object[] { false, autoSize, new Icon("bitmaps/256x256_one_entry_32bit.ico"), new Size(256, 256), 0, 0, 0 };
                yield return new object[] { false, autoSize, null, null, 0, 0, 0 };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Icon_SetWithNonNullOldValueWithParentWithHandle_TestData))]
        public void StatusBarPanel_Icon_SetWithNonNullOldValueWithParentWithHandle_GetReturnsExpected(bool showPanels, StatusBarPanelAutoSize autoSize, Icon value, Size? expectedSize, int expectedInvalidatedCallCount, int expectedParentLayoutCallCount1, int expectedParentLayoutCallCount2)
        {
            using var oldValue = new Icon("bitmaps/10x16_one_entry_32bit.ico");
            using var parent = new StatusBar
            {
                ShowPanels = showPanels
            };
            using var panel = new StatusBarPanel
            {
                AutoSize = autoSize,
                Icon = oldValue
            };
            parent.Panels.Add(panel);
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            int invalidatedCallCount = 0;
            parent.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            parent.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            parent.HandleCreated += (sender, e) => createdCallCount++;

            int parentLayoutCallCount = 0;
            void parentHandler(object sender, LayoutEventArgs e)
            {
                Assert.Same(parent, sender);
                Assert.Null(e.AffectedControl);
                Assert.Null(e.AffectedProperty);
                parentLayoutCallCount++;
            };
            parent.Layout += parentHandler;

            try
            {
                panel.Icon = value;
                Assert.Equal(expectedSize, panel.Icon?.Size);
                Assert.Equal(expectedParentLayoutCallCount1, parentLayoutCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);

                // Set same.
                panel.Icon = value;
                Assert.Equal(expectedSize, panel.Icon?.Size);
                Assert.Equal(expectedParentLayoutCallCount2, parentLayoutCallCount);
                Assert.True(parent.IsHandleCreated);
                Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
                Assert.Equal(0, styleChangedCallCount);
                Assert.Equal(0, createdCallCount);
            }
            finally
            {
                parent.Layout -= parentHandler;
            }
        }

        [WinFormsFact]
        public void StatusBarPanel_Icon_GetIcon_Success()
        {
            using var value1 = new Icon("bitmaps/10x16_one_entry_32bit.ico");
            using var value2 = new Icon("bitmaps/10x16_one_entry_32bit.ico");
            using var parent = new StatusBar
            {
                ShowPanels = true
            };
            using var panel1 = new StatusBarPanel();
            using var panel2 = new StatusBarPanel();
            parent.Panels.Add(panel1);
            parent.Panels.Add(panel2);

            // Set first.
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            panel1.Icon = value1;
            Assert.NotEqual(IntPtr.Zero, User32.SendMessageW(parent.Handle, (User32.WM)ComCtl32.SB.GETICON, IntPtr.Zero));
            Assert.Equal(IntPtr.Zero, User32.SendMessageW(parent.Handle, (User32.WM)ComCtl32.SB.GETICON, (IntPtr)1));

            // Set second.
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            panel2.Icon = value2;
            Assert.NotEqual(IntPtr.Zero, User32.SendMessageW(parent.Handle, (User32.WM)ComCtl32.SB.GETICON, IntPtr.Zero));
            Assert.NotEqual(IntPtr.Zero, User32.SendMessageW(parent.Handle, (User32.WM)ComCtl32.SB.GETICON, (IntPtr)1));

            // Set null.
            Assert.NotEqual(IntPtr.Zero, parent.Handle);
            panel2.Icon = null;
            Assert.NotEqual(IntPtr.Zero, User32.SendMessageW(parent.Handle, (User32.WM)ComCtl32.SB.GETICON, IntPtr.Zero));
            Assert.Equal(IntPtr.Zero, User32.SendMessageW(parent.Handle, (User32.WM)ComCtl32.SB.GETICON, (IntPtr)1));
        }

        private class SubStatusBarPanel : StatusBarPanel
        {
            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new bool DesignMode => base.DesignMode;

            public new EventHandlerList Events => base.Events;
        }
    }
}
