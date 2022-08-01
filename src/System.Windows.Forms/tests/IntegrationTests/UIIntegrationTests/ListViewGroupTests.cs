// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Xunit.Abstractions;
using static Interop.ComCtl32;
using static Interop;
using System.Drawing;

namespace System.Windows.Forms.UITests
{
    public class ListViewGroupTests : ControlTestBase
    {
        public ListViewGroupTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        public static IEnumerable<object[]> CollapsedState_TestData()
        {
            yield return new object[] { ListViewGroupCollapsedState.Default, ListViewGroupCollapsedState.Default };
            yield return new object[] { ListViewGroupCollapsedState.Expanded, ListViewGroupCollapsedState.Expanded };
            yield return new object[] { ListViewGroupCollapsedState.Collapsed, ListViewGroupCollapsedState.Collapsed };
        }

        public static IEnumerable<object[]> FooterAlignment_GetGroupInfo_TestData()
        {
            yield return new object[] { string.Empty, HorizontalAlignment.Left, (int)LVGA.HEADER_LEFT };
            yield return new object[] { string.Empty, HorizontalAlignment.Center, (int)LVGA.HEADER_LEFT };
            yield return new object[] { string.Empty, HorizontalAlignment.Right, (int)LVGA.HEADER_LEFT };

            yield return new object[] { "footer", HorizontalAlignment.Left, 0x00000008 | (int)LVGA.HEADER_LEFT };
            yield return new object[] { "footer", HorizontalAlignment.Center, 0x00000010 | (int)LVGA.HEADER_LEFT };
            yield return new object[] { "footer", HorizontalAlignment.Right, 0x00000020 | (int)LVGA.HEADER_LEFT };
        }

        public static IEnumerable<object[]> HeaderAlignment_GetGroupInfo_TestData()
        {
            yield return new object[] { string.Empty, HorizontalAlignment.Left, 0x00000001 };
            yield return new object[] { string.Empty, HorizontalAlignment.Center, 0x00000002 };
            yield return new object[] { string.Empty, HorizontalAlignment.Right, 0x00000004 };

            yield return new object[] { "header", HorizontalAlignment.Left, 0x00000001 };
            yield return new object[] { "header", HorizontalAlignment.Center, 0x00000002 };
            yield return new object[] { "header", HorizontalAlignment.Right, 0x00000004 };
        }

        [WinFormsFact]
        public unsafe void ListViewGroup_TitleImageIndex_GetGroupInfo_Success()
        {
            var data = new (int, int)[] { (-1, -1), (0, 0), (1, 1), (2, 2) };
            foreach ((int Index, int Expected) value in data)
            {
                Application.EnableVisualStyles();

                using var listView = new ListView();

                using var groupImageList = new ImageList();
                groupImageList.Images.Add(new Bitmap(10, 10));
                groupImageList.Images.Add(new Bitmap(20, 20));
                listView.GroupImageList = groupImageList;
                Assert.Equal((nint)groupImageList.Handle,
                    User32.SendMessageW(listView.Handle, (User32.WM)LVM.SETIMAGELIST, (nint)LVSIL.GROUPHEADER, groupImageList.Handle));

                var group = new ListViewGroup();
                listView.Groups.Add(group);

                Assert.NotEqual(IntPtr.Zero, listView.Handle);
                group.TitleImageIndex = value.Index;

                Assert.Equal(2, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPCOUNT));
                var lvgroup = new LVGROUPW
                {
                    cbSize = (uint)sizeof(LVGROUPW),
                    mask = LVGF.TITLEIMAGE | LVGF.GROUPID,
                };

                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPINFOBYINDEX, 1, ref lvgroup));
                Assert.Equal(value.Expected, lvgroup.iTitleImage);
                Assert.True(lvgroup.iGroupId >= 0);
            }
        }

        [WinFormsFact]
        public unsafe void ListViewGroup_TitleImageKey_GetGroupInfo_Success()
        {
            var data = new (string?, int)[] { (null, -1), (string.Empty, -1), ("text", 0), ("te\0t", 0) };
            foreach ((string? Key, int ExpectedIndex) value in data)
            {
                Application.EnableVisualStyles();

                using var listView = new ListView();

                using var groupImageList = new ImageList();
                groupImageList.Images.Add(value.Key!, new Bitmap(10, 10));
                listView.GroupImageList = groupImageList;
                Assert.Equal((nint)groupImageList.Handle,
                    User32.SendMessageW(listView.Handle, (User32.WM)LVM.SETIMAGELIST, (nint)LVSIL.GROUPHEADER, groupImageList.Handle));

                var group = new ListViewGroup();
                listView.Groups.Add(group);

                Assert.NotEqual(IntPtr.Zero, listView.Handle);
                group.TitleImageKey = value.Key!;

                Assert.Equal(2, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPCOUNT));
                var lvgroup = new LVGROUPW
                {
                    cbSize = (uint)sizeof(LVGROUPW),
                    mask = LVGF.TITLEIMAGE | LVGF.GROUPID
                };

                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPINFOBYINDEX, 1, ref lvgroup));
                Assert.Equal(value.ExpectedIndex, lvgroup.iTitleImage);
                Assert.True(lvgroup.iGroupId >= 0);
            }
        }

        [WinFormsFact]
        public unsafe void ListViewGroup_Subtitle_GetGroupInfo_Success()
        {
            // This needs to be initialized out of the loop.
            char* buffer = stackalloc char[256];

            foreach (object[] data in Property_TypeString_GetGroupInfo_TestData())
            {
                string value = (string)data[0];
                string expected = (string)data[1];

                Application.EnableVisualStyles();

                using var listView = new ListView();
                var group = new ListViewGroup();
                listView.Groups.Add(group);

                Assert.NotEqual(IntPtr.Zero, listView.Handle);
                group.Subtitle = value;

                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPCOUNT));

                var lvgroup = new LVGROUPW
                {
                    cbSize = (uint)sizeof(LVGROUPW),
                    mask = LVGF.SUBTITLE | LVGF.GROUPID,
                    pszSubtitle = buffer,
                    cchSubtitle = string.IsNullOrEmpty(value) ? 0 : (uint)value.Length + 1
                };

                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPINFOBYINDEX, 0, ref lvgroup));
                Assert.Equal(expected, new string(lvgroup.pszSubtitle));
                Assert.True(lvgroup.iGroupId >= 0);
            }
        }

        [WinFormsFact]
        public unsafe void ListViewGroup_Footer_GetGroupInfo_Success()
        {
            foreach (object[] data in Property_TypeString_GetGroupInfo_TestData())
            {
                string value = (string)data[0];
                string expected = (string)data[1];

                Application.EnableVisualStyles();

                using var listView = new ListView();
                var group = new ListViewGroup();
                listView.Groups.Add(group);

                Assert.NotEqual(IntPtr.Zero, listView.Handle);
                group.Footer = value;

                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPCOUNT));

                LVGA expectedHeaderFooterAlignment = LVGA.HEADER_LEFT;
                int size = 0;

                if (!string.IsNullOrEmpty(value))
                {
                    size = value.Length + 1;
                    expectedHeaderFooterAlignment |= LVGA.FOOTER_LEFT;
                }

                char* buffer = stackalloc char[size];
                var lvgroup = new LVGROUPW
                {
                    cbSize = (uint)sizeof(LVGROUPW),
                    mask = LVGF.FOOTER | LVGF.GROUPID | LVGF.ALIGN,
                    pszFooter = buffer,
                    cchFooter = size
                };

                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPINFOBYINDEX, 0, ref lvgroup));
                Assert.Equal(expected, new string(lvgroup.pszFooter));
                Assert.True(lvgroup.iGroupId >= 0);
                Assert.Equal(expectedHeaderFooterAlignment, lvgroup.uAlign);
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(FooterAlignment_GetGroupInfo_TestData))]
        public unsafe void ListView_FooterAlignment_GetGroupInfo_Success(string footer, HorizontalAlignment value, int expectedAlign)
        {
            Application.EnableVisualStyles();
            using var listView = new ListView();
            var group1 = new ListViewGroup
            {
                Footer = footer
            };

            listView.Groups.Add(group1);

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            group1.FooterAlignment = value;

            Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPCOUNT));
            int size = !string.IsNullOrEmpty(footer) ? footer.Length + 1 : 0;
            char* buffer = stackalloc char[size];
            var lvgroup = new LVGROUPW
            {
                cbSize = (uint)sizeof(LVGROUPW),
                mask = LVGF.FOOTER | LVGF.GROUPID | LVGF.ALIGN,
                pszFooter = buffer,
                cchFooter = size
            };

            Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPINFOBYINDEX, 0, ref lvgroup));
            Assert.Equal(footer, new string(lvgroup.pszFooter));
            Assert.True(lvgroup.iGroupId >= 0);
            Assert.Equal(expectedAlign, (int)lvgroup.uAlign);
        }

        [WinFormsFact]
        public unsafe void ListViewGroup_Header_GetGroupInfo_Success()
        {
            foreach (object[] data in Property_TypeString_GetGroupInfo_TestData())
            {
                string value = (string)data[0];
                string expected = (string)data[1];

                Application.EnableVisualStyles();

                using var listView = new ListView();
                var group = new ListViewGroup();
                listView.Groups.Add(group);

                Assert.NotEqual(IntPtr.Zero, listView.Handle);
                group.Header = value;

                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPCOUNT));
                int size = !string.IsNullOrEmpty(value) ? value.Length + 1 : 0;
                char* buffer = stackalloc char[size];
                var lvgroup = new LVGROUPW
                {
                    cbSize = (uint)sizeof(LVGROUPW),
                    mask = LVGF.HEADER | LVGF.GROUPID | LVGF.ALIGN,
                    pszHeader = buffer,
                    cchHeader = size
                };

                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPINFOBYINDEX, 0, ref lvgroup));
                Assert.Equal(expected, new string(lvgroup.pszHeader));
                Assert.True(lvgroup.iGroupId >= 0);
                Assert.Equal(LVGA.HEADER_LEFT, lvgroup.uAlign);
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(HeaderAlignment_GetGroupInfo_TestData))]
        public unsafe void ListView_HeaderAlignment_GetGroupInfo_Success(string header, HorizontalAlignment value, int expectedAlign)
        {
            Application.EnableVisualStyles();
            using var listView = new ListView();
            var group1 = new ListViewGroup
            {
                Header = header
            };

            listView.Groups.Add(group1);

            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            group1.HeaderAlignment = value;

            Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPCOUNT));
            int size = !string.IsNullOrEmpty(header) ? header.Length + 1 : 0;
            char* buffer = stackalloc char[size];
            var lvgroup = new LVGROUPW
            {
                cbSize = (uint)sizeof(LVGROUPW),
                mask = LVGF.HEADER | LVGF.GROUPID | LVGF.ALIGN,
                pszHeader = buffer,
                cchHeader = size
            };

            Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPINFOBYINDEX, 0, ref lvgroup));
            Assert.Equal(header, new string(lvgroup.pszHeader));
            Assert.True(lvgroup.iGroupId >= 0);
            Assert.Equal(expectedAlign, (int)lvgroup.uAlign);
        }

        [WinFormsFact]
        public unsafe void ListViewGroup_Collapse_GetGroupInfo_Success()
        {
            foreach (object[] data in CollapsedState_TestData())
            {
                Application.EnableVisualStyles();

                using var listView = new ListView();
                var group = new ListViewGroup();
                listView.Groups.Add(group);

                Assert.NotEqual(IntPtr.Zero, listView.Handle);
                group.CollapsedState = (ListViewGroupCollapsedState)data[0];
                ListViewGroupCollapsedState expectedCollapsedState = (ListViewGroupCollapsedState)data[1];

                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPCOUNT));
                var lvgroup = new LVGROUPW
                {
                    cbSize = (uint)sizeof(LVGROUPW),
                    mask = LVGF.STATE | LVGF.GROUPID,
                    stateMask = LVGS.COLLAPSIBLE | LVGS.COLLAPSED
                };

                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPINFOBYINDEX, 0, ref lvgroup));
                Assert.True(lvgroup.iGroupId >= 0);
                Assert.Equal(expectedCollapsedState, group.CollapsedState);
                if (expectedCollapsedState == ListViewGroupCollapsedState.Default)
                {
                    Assert.Equal(LVGS.NORMAL, lvgroup.state);
                }
                else if (expectedCollapsedState == ListViewGroupCollapsedState.Expanded)
                {
                    Assert.Equal(LVGS.COLLAPSIBLE, lvgroup.state);
                }
                else
                {
                    Assert.Equal(LVGS.COLLAPSIBLE | LVGS.COLLAPSED, lvgroup.state);
                }
            }
        }

        [WinFormsFact]
        public unsafe void ListViewGroup_Task_GetGroupInfo_Success()
        {
            // This needs to be outside the loop.
            char* buffer = stackalloc char[256];

            foreach (object[] data in Property_TypeString_GetGroupInfo_TestData())
            {
                string value = (string)data[0];
                string expected = (string)data[1];

                Application.EnableVisualStyles();

                using var listView = new ListView();
                var group = new ListViewGroup();
                listView.Groups.Add(group);

                Assert.NotEqual(IntPtr.Zero, listView.Handle);
                group.TaskLink = value;

                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPCOUNT));
                var lvgroup = new LVGROUPW
                {
                    cbSize = (uint)sizeof(LVGROUPW),
                    mask = LVGF.TASK | LVGF.GROUPID,
                    pszTask = buffer,
                    cchTask = (uint)(!string.IsNullOrEmpty(value) ? value.Length + 1 : 0)
                };

                Assert.Equal(1, User32.SendMessageW(listView.Handle, (User32.WM)LVM.GETGROUPINFOBYINDEX, 0, ref lvgroup));
                Assert.Equal(expected, new string(lvgroup.pszTask));
                Assert.True(lvgroup.iGroupId >= 0);
            }
        }

        public static IEnumerable<object[]> Property_TypeString_GetGroupInfo_TestData()
        {
            yield return new object[] { null!, string.Empty };
            yield return new object[] { string.Empty, string.Empty };
            yield return new object[] { "text", "text" };
            yield return new object[] { "te\0t", "te" };
        }
    }
}
