// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms.TestUtilities;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ListViewGroupTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ListViewGroup_Ctor_Default()
        {
            var group = new ListViewGroup();
            Assert.Empty(group.Footer);
            Assert.Equal(HorizontalAlignment.Left, group.FooterAlignment);
            Assert.Equal("ListViewGroup", group.Header);
            Assert.Equal(HorizontalAlignment.Left, group.HeaderAlignment);
            Assert.Empty(group.Subtitle);
            Assert.Empty(group.Items);
            Assert.Same(group.Items, group.Items);
            Assert.Null(group.ListView);
            Assert.Null(group.Name);
            Assert.Null(group.Tag);
            Assert.Equal(ListViewGroupCollapsedState.Default, group.CollapsedState);
            Assert.Empty(group.TaskLink);
            Assert.Empty(group.TitleImageKey);
            Assert.Equal(-1, group.TitleImageIndex);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListViewGroup_Ctor_String(string header, string expectedHeader)
        {
            var group = new ListViewGroup(header);
            Assert.Empty(group.Footer);
            Assert.Equal(HorizontalAlignment.Left, group.FooterAlignment);
            Assert.Equal(expectedHeader, group.Header);
            Assert.Equal(HorizontalAlignment.Left, group.HeaderAlignment);
            Assert.Empty(group.Subtitle);
            Assert.Empty(group.Items);
            Assert.Same(group.Items, group.Items);
            Assert.Null(group.ListView);
            Assert.Null(group.Name);
            Assert.Null(group.Tag);
            Assert.Equal(ListViewGroupCollapsedState.Default, group.CollapsedState);
            Assert.Empty(group.TaskLink);
            Assert.Empty(group.TitleImageKey);
            Assert.Equal(-1, group.TitleImageIndex);
        }

        public static IEnumerable<object[]> Ctor_String_HorizontalAlignment_TestData()
        {
            yield return new object[] { null, HorizontalAlignment.Left, string.Empty };
            yield return new object[] { string.Empty, HorizontalAlignment.Right, string.Empty };
            yield return new object[] { "reasonable", HorizontalAlignment.Center, "reasonable" };
            yield return new object[] { "reasonable", (HorizontalAlignment)(HorizontalAlignment.Left - 1), "reasonable" };
            yield return new object[] { "reasonable", (HorizontalAlignment)(HorizontalAlignment.Center + 1), "reasonable" };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_String_HorizontalAlignment_TestData))]
        public void ListViewGroup_Ctor_String_HorizontalAlignment(string header, HorizontalAlignment headerAlignment, string expectedHeader)
        {
            var group = new ListViewGroup(header, headerAlignment);
            Assert.Empty(group.Footer);
            Assert.Equal(HorizontalAlignment.Left, group.FooterAlignment);
            Assert.Equal(expectedHeader, group.Header);
            Assert.Equal(headerAlignment, group.HeaderAlignment);
            Assert.Empty(group.Subtitle);
            Assert.Empty(group.Items);
            Assert.Equal(group.Items, group.Items);
            Assert.Null(group.ListView);
            Assert.Null(group.Name);
            Assert.Null(group.Tag);
            Assert.Equal(ListViewGroupCollapsedState.Default, group.CollapsedState);
            Assert.Empty(group.TaskLink);
            Assert.Empty(group.TitleImageKey);
            Assert.Equal(-1, group.TitleImageIndex);
        }

        public static IEnumerable<object[]> Ctor_String_String_TestData()
        {
            yield return new object[] { null, null, string.Empty };
            yield return new object[] { string.Empty, string.Empty, string.Empty };
            yield return new object[] { "key", "header", "header" };
        }

        [WinFormsTheory]
        [MemberData(nameof(Ctor_String_String_TestData))]
        public void ListViewGroup_Ctor_String_String(string key, string header, string expectedHeader)
        {
            var group = new ListViewGroup(key, header);
            Assert.Empty(group.Footer);
            Assert.Equal(HorizontalAlignment.Left, group.FooterAlignment);
            Assert.Equal(expectedHeader, group.Header);
            Assert.Equal(HorizontalAlignment.Left, group.HeaderAlignment);
            Assert.Empty(group.Subtitle);
            Assert.Empty(group.Items);
            Assert.Same(group.Items, group.Items);
            Assert.Null(group.ListView);
            Assert.Equal(key, group.Name);
            Assert.Null(group.Tag);
            Assert.Equal(ListViewGroupCollapsedState.Default, group.CollapsedState);
            Assert.Empty(group.TaskLink);
            Assert.Empty(group.TitleImageKey);
            Assert.Equal(-1, group.TitleImageIndex);
        }

        [WinFormsTheory]
        [InlineData(ListViewGroupCollapsedState.Default)]
        [InlineData(ListViewGroupCollapsedState.Collapsed)]
        [InlineData(ListViewGroupCollapsedState.Expanded)]
        public void ListViewGroup_GetNativeCollapsedState_Succeeds(ListViewGroupCollapsedState collapsedState)
        {
            using var listView = new ListView();
            var group = new ListViewGroup() { CollapsedState = collapsedState };
            listView.Groups.Add(group);

            Assert.Equal(collapsedState, group.GetNativeCollapsedState());
        }

        [WinFormsFact]
        public void ListViewGroup_GetNativeCollapsedState_NoListView_Throws()
        {
            var group = new ListViewGroup();
            Assert.Throws<InvalidOperationException>(() => group.GetNativeCollapsedState());
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetNonNegativeIntTheoryData))]
        [InlineData(-1)]
        public void ListViewGroup_TitleImageIndex_SetWithoutListView_GetReturnsExpected(int value)
        {
            var group = new ListViewGroup
            {
                TitleImageIndex = value
            };

            Assert.Equal(value, group.TitleImageIndex);

            // Set same.
            group.TitleImageIndex = value;
            Assert.Equal(value, group.TitleImageIndex);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetNonNegativeIntTheoryData))]
        [InlineData(-1)]
        public void ListViewGroup_TitleImageIndex_SetWithListView_GetReturnsExpected(int value)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);

            group.TitleImageIndex = value;
            Assert.Equal(value, group.TitleImageIndex);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            group.TitleImageIndex = value;
            Assert.Equal(value, group.TitleImageIndex);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetNonNegativeIntTheoryData))]
        [InlineData(-1)]
        public void ListViewGroup_TitleImageIndex_SetWithListViewWithHandle_GetReturnsExpected(int value)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            group.TitleImageIndex = value;
            Assert.Equal(value, group.TitleImageIndex);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            group.TitleImageIndex = value;
            Assert.Equal(value, group.TitleImageIndex);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsFact]
        public void ListViewGroup_TitleImageIndex_SetInvalid_ThrowsArgumentOutOfRangeException()
        {
            var random = new Random();
            int value = random.Next(2, int.MaxValue) * -1;
            var group = new ListViewGroup();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => group.TitleImageIndex = value);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListViewGroup_TitleImageIndex_SetTitleImageKey_GetReturnsExpected(string key, string expected)
        {
            var group = new ListViewGroup
            {
                TitleImageIndex = 0
            };

            Assert.Equal(0, group.TitleImageIndex);

            // verify TitleImageIndex resets to default once TitleImageKey is set
            group.TitleImageKey = key;
            Assert.Equal(expected, group.TitleImageKey);
            Assert.Equal(ImageList.Indexer.DefaultIndex, group.TitleImageIndex);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("te\0xt", "te\0xt")]
        public void ListViewGroup_TitleImageKey_SetWithoutListView_GetReturnsExpected(string value, string expected)
        {
            var group = new ListViewGroup
            {
                TitleImageKey = value
            };

            Assert.Equal(expected, group.TitleImageKey);

            // Set same.
            group.TitleImageKey = value;
            Assert.Equal(expected, group.TitleImageKey);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("te\0xt", "te\0xt")]
        public void ListViewGroup_TitleImageKey_SetWithListView_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);

            group.TitleImageKey = value;
            Assert.Equal(expected, group.TitleImageKey);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            group.TitleImageKey = value;
            Assert.Equal(expected, group.TitleImageKey);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("header", "header")]
        [InlineData("te\0xt", "te\0xt")]
        [InlineData("ListViewGroup", "ListViewGroup")]
        public void ListViewGroup_TitleImageKey_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            group.TitleImageKey = value;
            Assert.Equal(expected, group.TitleImageKey);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            group.TitleImageKey = value;
            Assert.Equal(expected, group.TitleImageKey);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetNonNegativeIntTheoryData))]
        [InlineData(-1)]
        public void ListViewGroup_TitleImageKey_SetTitleImageIndex_GetReturnsExpected(int value)
        {
            var group = new ListViewGroup
            {
                TitleImageKey = "key"
            };

            Assert.Equal("key", group.TitleImageKey);

            // verify TitleImageKey resets to default once TitleImageIndex is set
            group.TitleImageIndex = value;
            Assert.Equal(value, group.TitleImageIndex);
            Assert.Equal(ImageList.Indexer.DefaultKey, group.TitleImageKey);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("te\0xt", "te\0xt")]
        public void ListViewGroup_Subtitle_SetWithoutListView_GetReturnsExpected(string value, string expected)
        {
            var group = new ListViewGroup
            {
                Subtitle = value
            };

            Assert.Equal(expected, group.Subtitle);

            // Set same.
            group.Subtitle = value;
            Assert.Equal(expected, group.Subtitle);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("te\0xt", "te\0xt")]
        public void ListViewGroup_Subtitle_SetWithListView_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);

            group.Subtitle = value;
            Assert.Equal(expected, group.Subtitle);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            group.Subtitle = value;
            Assert.Equal(expected, group.Subtitle);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("header", "header")]
        [InlineData("te\0xt", "te\0xt")]
        [InlineData("ListViewGroup", "ListViewGroup")]
        public void ListViewGroup_Subtitle_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            group.Subtitle = value;
            Assert.Equal(expected, group.Subtitle);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            group.Subtitle = value;
            Assert.Equal(expected, group.Subtitle);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Property_TypeString_GetGroupInfo_TestData()
        {
            yield return new object[] { null, string.Empty };
            yield return new object[] { string.Empty, string.Empty };
            yield return new object[] { "text", "text" };
            yield return new object[] { "te\0t", "te" };
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("te\0xt", "te\0xt")]
        public void ListViewGroup_Footer_SetWithoutListView_GetReturnsExpected(string value, string expected)
        {
            var group = new ListViewGroup
            {
                Footer = value
            };

            Assert.Equal(expected, group.Footer);

            // Set same.
            group.Footer = value;
            Assert.Equal(expected, group.Footer);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("te\0xt", "te\0xt")]
        public void ListViewGroup_Footer_SetWithListView_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);

            group.Footer = value;
            Assert.Equal(expected, group.Footer);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            group.Footer = value;
            Assert.Equal(expected, group.Footer);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("header", "header")]
        [InlineData("te\0xt", "te\0xt")]
        [InlineData("ListViewGroup", "ListViewGroup")]
        public void ListViewGroup_Footer_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            group.Footer = value;
            Assert.Equal(expected, group.Footer);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            group.Footer = value;
            Assert.Equal(expected, group.Footer);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        public static IEnumerable<object[]> Alignment_Set_TestData()
        {
            foreach (HorizontalAlignment value in Enum.GetValues(typeof(HorizontalAlignment)))
            {
                yield return new object[] { null, value, string.Empty };
                yield return new object[] { string.Empty, value, string.Empty };
                yield return new object[] { "text", value, "text" };
            }
        }

        [WinFormsTheory]
        [MemberData(nameof(Alignment_Set_TestData))]
        public void ListViewGroup_FooterAlignment_SetWithoutListView_GetReturnsExpected(string footer, HorizontalAlignment value, string expectedFooter)
        {
            var group = new ListViewGroup
            {
                Footer = footer,
                FooterAlignment = value
            };

            Assert.Equal(value, group.FooterAlignment);
            Assert.Equal(expectedFooter, group.Footer);

            // Set same.
            group.FooterAlignment = value;
            Assert.Equal(value, group.FooterAlignment);
            Assert.Equal(expectedFooter, group.Footer);
        }

        [WinFormsTheory]
        [MemberData(nameof(Alignment_Set_TestData))]
        public void ListViewGroup_FooterAlignment_SetWithListView_GetReturnsExpected(string footer, HorizontalAlignment value, string expectedFooter)
        {
            using var listView = new ListView();
            var group = new ListViewGroup
            {
                Footer = footer
            };

            listView.Groups.Add(group);

            group.FooterAlignment = value;
            Assert.Equal(value, group.FooterAlignment);
            Assert.Equal(expectedFooter, group.Footer);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            group.FooterAlignment = value;
            Assert.Equal(value, group.FooterAlignment);
            Assert.Equal(expectedFooter, group.Footer);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Alignment_Set_TestData))]
        public void ListViewGroup_FooterAlignment_SetWithListViewWithHandle_GetReturnsExpected(string footer, HorizontalAlignment value, string expectedFooter)
        {
            using var listView = new ListView();
            var group = new ListViewGroup
            {
                Footer = footer
            };

            listView.Groups.Add(group);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            group.FooterAlignment = value;
            Assert.Equal(value, group.FooterAlignment);
            Assert.Equal(expectedFooter, group.Footer);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            group.FooterAlignment = value;
            Assert.Equal(value, group.FooterAlignment);
            Assert.Equal(expectedFooter, group.Footer);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(HorizontalAlignment))]
        public void ListViewGroup_FooterAlignment_SetInvalid_ThrowsInvalidEnumArgumentException(HorizontalAlignment value)
        {
            var group = new ListViewGroup();
            Assert.Throws<InvalidEnumArgumentException>("value", () => group.FooterAlignment = value);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("te\0xt", "te\0xt")]
        public void ListViewGroup_Header_SetWithoutListView_GetReturnsExpected(string value, string expected)
        {
            var group = new ListViewGroup
            {
                Header = value
            };

            Assert.Equal(expected, group.Header);

            // Set same.
            group.Header = value;
            Assert.Equal(expected, group.Header);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("te\0xt", "te\0xt")]
        public void ListViewGroup_Header_SetWithListView_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);

            group.Header = value;
            Assert.Equal(expected, group.Header);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            group.Header = value;
            Assert.Equal(expected, group.Header);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("header", "header")]
        [InlineData("te\0xt", "te\0xt")]
        [InlineData("ListViewGroup", "ListViewGroup")]
        public void ListViewGroup_Header_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            group.Header = value;
            Assert.Equal(expected, group.Header);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            group.Header = value;
            Assert.Equal(expected, group.Header);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [MemberData(nameof(Alignment_Set_TestData))]
        public void ListViewGroup_HeaderAlignment_SetWithoutListView_GetReturnsExpected(string header, HorizontalAlignment value, string expectedHeader)
        {
            var group = new ListViewGroup
            {
                Header = header,
                HeaderAlignment = value
            };

            Assert.Equal(value, group.HeaderAlignment);
            Assert.Equal(expectedHeader, group.Header);

            // Set same.
            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);
            Assert.Equal(expectedHeader, group.Header);
        }

        [WinFormsTheory]
        [MemberData(nameof(Alignment_Set_TestData))]
        public void ListViewGroup_HeaderAlignment_SetWithListView_GetReturnsExpected(string header, HorizontalAlignment value, string expectedHeader)
        {
            using var listView = new ListView();
            var group = new ListViewGroup
            {
                Header = header
            };

            listView.Groups.Add(group);

            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);
            Assert.Equal(expectedHeader, group.Header);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);
            Assert.Equal(expectedHeader, group.Header);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(Alignment_Set_TestData))]
        public void ListViewGroup_HeaderAlignment_SetWithListViewWithHandle_GetReturnsExpected(string header, HorizontalAlignment value, string expectedHeader)
        {
            using var listView = new ListView();
            var group = new ListViewGroup
            {
                Header = header
            };

            listView.Groups.Add(group);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);
            Assert.Equal(expectedHeader, group.Header);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            group.HeaderAlignment = value;
            Assert.Equal(value, group.HeaderAlignment);
            Assert.Equal(expectedHeader, group.Header);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(HorizontalAlignment))]
        public void ListViewGroup_HeaderAlignment_SetInvalid_ThrowsInvalidEnumArgumentException(HorizontalAlignment value)
        {
            var group = new ListViewGroup();
            Assert.Throws<InvalidEnumArgumentException>("value", () => group.HeaderAlignment = value);
        }

        public static IEnumerable<object[]> CollapsedState_TestData()
        {
            yield return new object[] { ListViewGroupCollapsedState.Default, ListViewGroupCollapsedState.Default };
            yield return new object[] { ListViewGroupCollapsedState.Expanded, ListViewGroupCollapsedState.Expanded };
            yield return new object[] { ListViewGroupCollapsedState.Collapsed, ListViewGroupCollapsedState.Collapsed };
        }

        [WinFormsTheory]
        [MemberData(nameof(CollapsedState_TestData))]
        public void ListViewGroup_Collapse_SetWithoutListView_GetReturnsExpected(ListViewGroupCollapsedState value, ListViewGroupCollapsedState expected)
        {
            var group = new ListViewGroup()
            {
                CollapsedState = value
            };

            Assert.Equal(expected, group.CollapsedState);

            // Set same.
            group.CollapsedState = value;
            Assert.Equal(expected, group.CollapsedState);
        }

        [WinFormsTheory]
        [MemberData(nameof(CollapsedState_TestData))]
        public void ListViewGroup_Collapse_SetWithListView_GetReturnsExpected(ListViewGroupCollapsedState value, ListViewGroupCollapsedState expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup
            {
                CollapsedState = value
            };

            listView.Groups.Add(group);

            Assert.Equal(expected, group.CollapsedState);
            Assert.Equal(group.CollapsedState, listView.Groups[0].CollapsedState);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            group.CollapsedState = value;
            Assert.Equal(expected, group.CollapsedState);
            Assert.Equal(group.CollapsedState, listView.Groups[0].CollapsedState);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [MemberData(nameof(CollapsedState_TestData))]
        public void ListViewGroup_Collapse_SetWithListViewWithHandle_GetReturnsExpected(ListViewGroupCollapsedState value, ListViewGroupCollapsedState expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup
            {
                CollapsedState = value
            };

            listView.Groups.Add(group);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(expected, group.CollapsedState);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            group.CollapsedState = value;
            Assert.Equal(expected, group.CollapsedState);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ListViewGroupCollapsedState))]
        public void ListViewGroup_CollapsedState_SetInvalid_ThrowsInvalidEnumArgumentException(ListViewGroupCollapsedState value)
        {
            var group = new ListViewGroup();
            Assert.Throws<InvalidEnumArgumentException>("value", () => group.CollapsedState = value);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ListViewGroup_Name_Set_GetReturnsExpected(string value)
        {
            var group = new ListViewGroup
            {
                Name = value
            };

            Assert.Same(value, group.Name);

            // Set same.
            group.Name = value;
            Assert.Same(value, group.Name);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("te\0xt", "te\0xt")]
        public void ListViewGroup_Task_SetWithoutListView_GetReturnsExpected(string value, string expected)
        {
            var group = new ListViewGroup
            {
                TaskLink = value
            };

            Assert.Equal(expected, group.TaskLink);

            // Set same.
            group.TaskLink = value;
            Assert.Equal(expected, group.TaskLink);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        [InlineData("te\0xt", "te\0xt")]
        public void ListViewGroup_Task_SetWithListView_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);

            group.TaskLink = value;
            Assert.Equal(expected, group.TaskLink);
            Assert.False(listView.IsHandleCreated);

            // Set same.
            group.TaskLink = value;
            Assert.Equal(expected, group.TaskLink);
            Assert.False(listView.IsHandleCreated);
        }

        [WinFormsTheory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("header", "header")]
        [InlineData("te\0xt", "te\0xt")]
        [InlineData("ListViewGroup", "ListViewGroup")]
        public void ListViewGroup_Task_SetWithListViewWithHandle_GetReturnsExpected(string value, string expected)
        {
            using var listView = new ListView();
            var group = new ListViewGroup();
            listView.Groups.Add(group);
            Assert.NotEqual(IntPtr.Zero, listView.Handle);
            int invalidatedCallCount = 0;
            listView.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            listView.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            listView.HandleCreated += (sender, e) => createdCallCount++;

            group.TaskLink = value;
            Assert.Equal(expected, group.TaskLink);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);

            // Set same.
            group.TaskLink = value;
            Assert.Equal(expected, group.TaskLink);
            Assert.True(listView.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ListViewGroup_Tag_Set_GetReturnsExpected(string value)
        {
            var group = new ListViewGroup
            {
                Tag = value
            };

            Assert.Same(value, group.Tag);

            // Set same.
            group.Tag = value;
            Assert.Same(value, group.Tag);
        }

        public static IEnumerable<object[]> Serialize_Deserialize_TestData()
        {
            yield return new object[] { new ListViewGroup() };
            yield return new object[] { new ListViewGroup("header", HorizontalAlignment.Center) { Name = "name", Tag = "tag" } };

            var groupWithEmptyItems = new ListViewGroup();
            Assert.Empty(groupWithEmptyItems.Items);
            yield return new object[] { groupWithEmptyItems };

            var groupWithItems = new ListViewGroup();
            groupWithItems.Items.Add(new ListViewItem("text"));
            yield return new object[] { groupWithItems };
        }

        [WinFormsTheory]
        [MemberData(nameof(Serialize_Deserialize_TestData))]
        public void ListViewGroup_Serialize_Deserialize_Success(ListViewGroup group)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                formatter.Serialize(stream, group);
                stream.Seek(0, SeekOrigin.Begin);

                ListViewGroup result = Assert.IsType<ListViewGroup>(formatter.Deserialize(stream));
#pragma warning restore SYSLIB0011 // Type or member is obsolete
                Assert.Equal(group.Header, result.Header);
                Assert.Equal(group.HeaderAlignment, result.HeaderAlignment);
                Assert.Equal(group.Items.Cast<ListViewItem>().Select(i => i.Text), result.Items.Cast<ListViewItem>().Select(i => i.Text));
                Assert.Equal(group.Name, result.Name);
                Assert.Equal(group.Tag, result.Tag);
            }
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ListViewGroup_ToString_Invoke_ReturnsExpected(string header, string expected)
        {
            var group = new ListViewGroup(header);
            Assert.Equal(expected, group.ToString());
        }

        [WinFormsFact]
        public void ListViewGroup_ISerializableGetObjectData_InvokeSimple_Success()
        {
            var group = new ListViewGroup();
            ISerializable iSerializable = group;
            var info = new SerializationInfo(typeof(ListViewGroup), new FormatterConverter());
            var context = new StreamingContext();

            iSerializable.GetObjectData(info, context);
            Assert.Equal("ListViewGroup", info.GetString("Header"));
            Assert.Equal(HorizontalAlignment.Left, info.GetValue("HeaderAlignment", typeof(HorizontalAlignment)));
            Assert.Throws<SerializationException>(() => info.GetString("Name"));
            Assert.Null(info.GetValue("Tag", typeof(object)));
            Assert.Throws<SerializationException>(() => info.GetInt32("ItemsCount"));
        }

        [WinFormsFact]
        public void ListViewGroup_ISerializableGetObjectData_InvokeWithEmptyItems_Success()
        {
            var group = new ListViewGroup();
            Assert.Empty(group.Items);

            ISerializable iSerializable = group;
            var info = new SerializationInfo(typeof(ListViewGroup), new FormatterConverter());
            var context = new StreamingContext();

            iSerializable.GetObjectData(info, context);
            Assert.Equal("ListViewGroup", info.GetString("Header"));
            Assert.Equal(HorizontalAlignment.Left, info.GetValue("HeaderAlignment", typeof(HorizontalAlignment)));
            Assert.Throws<SerializationException>(() => info.GetString("Name"));
            Assert.Null(info.GetValue("Tag", typeof(object)));
            Assert.Throws<SerializationException>(() => info.GetInt32("ItemsCount"));
        }

        [WinFormsFact]
        public void ListViewGroup_ISerializableGetObjectData_InvokeWithItems_Success()
        {
            var group = new ListViewGroup();
            group.Items.Add(new ListViewItem("text"));

            ISerializable iSerializable = group;
            var info = new SerializationInfo(typeof(ListViewGroup), new FormatterConverter());
            var context = new StreamingContext();

            iSerializable.GetObjectData(info, context);
            Assert.Equal("ListViewGroup", info.GetString("Header"));
            Assert.Equal(HorizontalAlignment.Left, info.GetValue("HeaderAlignment", typeof(HorizontalAlignment)));
            Assert.Throws<SerializationException>(() => info.GetString("Name"));
            Assert.Null(info.GetValue("Tag", typeof(object)));
            Assert.Equal(1, info.GetInt32("ItemsCount"));
            Assert.Equal("text", ((ListViewItem)info.GetValue("Item0", typeof(ListViewItem))).Text);
        }

        [WinFormsFact]
        public void ListViewGroup_ISerializableGetObjectData_NullInfo_ThrowsArgumentNullException()
        {
            var group = new ListViewGroup();
            ISerializable iSerializable = group;
            var context = new StreamingContext();
            Assert.Throws<ArgumentNullException>("info", () => iSerializable.GetObjectData(null, context));
        }

        [WinFormsFact]
        public void ListViewGroup_InvokeAdd_DoesNotAddTreeViewItemToList()
        {
            using var listView = new ListView();
            ListViewItem listViewItem = new ListViewItem();
            ListViewItem listViewItemGroup = new ListViewItem();
            ListViewGroup listViewGroup = new ListViewGroup();
            var accessor = KeyboardToolTipStateMachine.Instance.TestAccessor();
            listView.Groups.Add(listViewGroup);
            listView.Items.Add(listViewItem);
            listViewGroup.Items.Add(listViewItemGroup);

            Assert.True(accessor.IsToolTracked(listViewItem));
            Assert.False(accessor.IsToolTracked(listViewItemGroup));
        }

        [WinFormsFact]
        public void ListViewGroup_InvokeRemove_DoesNotRemoveTreeViewItemFromList()
        {
            using var listView = new ListView();
            ListViewItem listViewItem = new ListViewItem();
            ListViewGroup listViewGroup = new ListViewGroup();
            var accessor = KeyboardToolTipStateMachine.Instance.TestAccessor();
            listView.Groups.Add(listViewGroup);
            listView.Items.Add(listViewItem);
            listViewGroup.Items.Add(listViewItem);

            Assert.True(accessor.IsToolTracked(listViewItem));

            listViewGroup.Items.Remove(listViewItem);
            Assert.True(accessor.IsToolTracked(listViewItem));
        }
    }
}
