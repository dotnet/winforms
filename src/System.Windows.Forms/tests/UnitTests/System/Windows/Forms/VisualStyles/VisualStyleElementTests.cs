// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.VisualStyles.Tests
{
    // NB: doesn't require thread affinity
    public class VisualStyleElementTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> CreateElement_TestData()
        {
            yield return new object[] { string.Empty, 0, 0 };
            yield return new object[] { "className", -1, -2 };
        }

        [Theory]
        [MemberData(nameof(CreateElement_TestData))]
        public void VisualStyleElement_CreateElement_Invoke_ReturnsExpected(string className, int part, int state)
        {
            VisualStyleElement element = VisualStyleElement.CreateElement(className, part, state);
            Assert.Same(className, element.ClassName);
            Assert.Equal(part, element.Part);
            Assert.Equal(state, element.State);
        }

        [WinFormsFact]
        public void VisualStyleElement_CreateElement_NullClassName_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("className", () => VisualStyleElement.CreateElement(null, 1, 2));
        }

        public static IEnumerable<object[]> KnownElements_TestData()
        {
            // Identity function to avoid constant casting
            Func<VisualStyleElement> I(Func<VisualStyleElement> factory) => factory;

            yield return new object[] { I(() => VisualStyleElement.Button.PushButton.Normal), "BUTTON", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.Button.PushButton.Hot), "BUTTON", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.Button.PushButton.Pressed), "BUTTON", 1, 3 };
            yield return new object[] { I(() => VisualStyleElement.Button.PushButton.Disabled), "BUTTON", 1, 4 };
            yield return new object[] { I(() => VisualStyleElement.Button.PushButton.Default), "BUTTON", 1, 5 };

            yield return new object[] { I(() => VisualStyleElement.Button.RadioButton.UncheckedNormal), "BUTTON", 2, 1 };
            yield return new object[] { I(() => VisualStyleElement.Button.RadioButton.UncheckedHot), "BUTTON", 2, 2 };
            yield return new object[] { I(() => VisualStyleElement.Button.RadioButton.UncheckedPressed), "BUTTON", 2, 3 };
            yield return new object[] { I(() => VisualStyleElement.Button.RadioButton.UncheckedDisabled), "BUTTON", 2, 4 };
            yield return new object[] { I(() => VisualStyleElement.Button.RadioButton.CheckedNormal), "BUTTON", 2, 5 };
            yield return new object[] { I(() => VisualStyleElement.Button.RadioButton.CheckedHot), "BUTTON", 2, 6 };
            yield return new object[] { I(() => VisualStyleElement.Button.RadioButton.CheckedPressed), "BUTTON", 2, 7 };
            yield return new object[] { I(() => VisualStyleElement.Button.RadioButton.CheckedDisabled), "BUTTON", 2, 8 };

            yield return new object[] { I(() => VisualStyleElement.Button.CheckBox.UncheckedNormal), "BUTTON", 3, 1 };
            yield return new object[] { I(() => VisualStyleElement.Button.CheckBox.UncheckedHot), "BUTTON", 3, 2 };
            yield return new object[] { I(() => VisualStyleElement.Button.CheckBox.UncheckedPressed), "BUTTON", 3, 3 };
            yield return new object[] { I(() => VisualStyleElement.Button.CheckBox.UncheckedDisabled), "BUTTON", 3, 4 };
            yield return new object[] { I(() => VisualStyleElement.Button.CheckBox.CheckedNormal), "BUTTON", 3, 5 };
            yield return new object[] { I(() => VisualStyleElement.Button.CheckBox.CheckedHot), "BUTTON", 3, 6 };
            yield return new object[] { I(() => VisualStyleElement.Button.CheckBox.CheckedPressed), "BUTTON", 3, 7 };
            yield return new object[] { I(() => VisualStyleElement.Button.CheckBox.CheckedDisabled), "BUTTON", 3, 8 };
            yield return new object[] { I(() => VisualStyleElement.Button.CheckBox.MixedNormal), "BUTTON", 3, 9 };
            yield return new object[] { I(() => VisualStyleElement.Button.CheckBox.MixedHot), "BUTTON", 3, 10 };
            yield return new object[] { I(() => VisualStyleElement.Button.CheckBox.MixedPressed), "BUTTON", 3, 11 };
            yield return new object[] { I(() => VisualStyleElement.Button.CheckBox.MixedDisabled), "BUTTON", 3, 12 };

            yield return new object[] { I(() => VisualStyleElement.Button.GroupBox.Normal), "BUTTON", 4, 1 };
            yield return new object[] { I(() => VisualStyleElement.Button.GroupBox.Disabled), "BUTTON", 4, 2 };

            yield return new object[] { I(() => VisualStyleElement.Button.UserButton.Normal), "BUTTON", 5, 0 };

            yield return new object[] { I(() => VisualStyleElement.ComboBox.DropDownButton.Normal), "COMBOBOX", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.ComboBox.DropDownButton.Hot), "COMBOBOX", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.ComboBox.DropDownButton.Pressed), "COMBOBOX", 1, 3 };
            yield return new object[] { I(() => VisualStyleElement.ComboBox.DropDownButton.Disabled), "COMBOBOX", 1, 4 };

            yield return new object[] { I(() => VisualStyleElement.ComboBox.Border.Normal), "COMBOBOX", 4, 3 };

            yield return new object[] { I(() => VisualStyleElement.ComboBox.ReadOnlyButton.Normal), "COMBOBOX", 5, 2 };

            yield return new object[] { I(() => VisualStyleElement.ComboBox.DropDownButtonRight.Normal), "COMBOBOX", 6, 1 };

            yield return new object[] { I(() => VisualStyleElement.ComboBox.DropDownButtonLeft.Normal), "COMBOBOX", 7, 2 };

            yield return new object[] { I(() => VisualStyleElement.Page.Up.Normal), "PAGE", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.Page.Up.Hot), "PAGE", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.Page.Up.Pressed), "PAGE", 1, 3 };
            yield return new object[] { I(() => VisualStyleElement.Page.Up.Disabled), "PAGE", 1, 4 };

            yield return new object[] { I(() => VisualStyleElement.Page.Down.Normal), "PAGE", 2, 1 };
            yield return new object[] { I(() => VisualStyleElement.Page.Down.Hot), "PAGE", 2, 2 };
            yield return new object[] { I(() => VisualStyleElement.Page.Down.Pressed), "PAGE", 2, 3 };
            yield return new object[] { I(() => VisualStyleElement.Page.Down.Disabled), "PAGE", 2, 4 };

            yield return new object[] { I(() => VisualStyleElement.Page.UpHorizontal.Normal), "PAGE", 3, 1 };
            yield return new object[] { I(() => VisualStyleElement.Page.UpHorizontal.Hot), "PAGE", 3, 2 };
            yield return new object[] { I(() => VisualStyleElement.Page.UpHorizontal.Pressed), "PAGE", 3, 3 };
            yield return new object[] { I(() => VisualStyleElement.Page.UpHorizontal.Disabled), "PAGE", 3, 4 };

            yield return new object[] { I(() => VisualStyleElement.Page.DownHorizontal.Normal), "PAGE", 4, 1 };
            yield return new object[] { I(() => VisualStyleElement.Page.DownHorizontal.Hot), "PAGE", 4, 2 };
            yield return new object[] { I(() => VisualStyleElement.Page.DownHorizontal.Pressed), "PAGE", 4, 3 };
            yield return new object[] { I(() => VisualStyleElement.Page.DownHorizontal.Disabled), "PAGE", 4, 4 };

            yield return new object[] { I(() => VisualStyleElement.Spin.Up.Normal), "SPIN", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.Spin.Up.Hot), "SPIN", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.Spin.Up.Pressed), "SPIN", 1, 3 };
            yield return new object[] { I(() => VisualStyleElement.Spin.Up.Disabled), "SPIN", 1, 4 };

            yield return new object[] { I(() => VisualStyleElement.Spin.Down.Normal), "SPIN", 2, 1 };
            yield return new object[] { I(() => VisualStyleElement.Spin.Down.Hot), "SPIN", 2, 2 };
            yield return new object[] { I(() => VisualStyleElement.Spin.Down.Pressed), "SPIN", 2, 3 };
            yield return new object[] { I(() => VisualStyleElement.Spin.Down.Disabled), "SPIN", 2, 4 };

            yield return new object[] { I(() => VisualStyleElement.Spin.UpHorizontal.Normal), "SPIN", 3, 1 };
            yield return new object[] { I(() => VisualStyleElement.Spin.UpHorizontal.Hot), "SPIN", 3, 2 };
            yield return new object[] { I(() => VisualStyleElement.Spin.UpHorizontal.Pressed), "SPIN", 3, 3 };
            yield return new object[] { I(() => VisualStyleElement.Spin.UpHorizontal.Disabled), "SPIN", 3, 4 };

            yield return new object[] { I(() => VisualStyleElement.Spin.DownHorizontal.Normal), "SPIN", 4, 1 };
            yield return new object[] { I(() => VisualStyleElement.Spin.DownHorizontal.Hot), "SPIN", 4, 2 };
            yield return new object[] { I(() => VisualStyleElement.Spin.DownHorizontal.Pressed), "SPIN", 4, 3 };
            yield return new object[] { I(() => VisualStyleElement.Spin.DownHorizontal.Disabled), "SPIN", 4, 4 };

            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.UpNormal), "SCROLLBAR", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.UpHot), "SCROLLBAR", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.UpPressed), "SCROLLBAR", 1, 3 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.UpDisabled), "SCROLLBAR", 1, 4 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.DownNormal), "SCROLLBAR", 1, 5 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.DownHot), "SCROLLBAR", 1, 6 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.DownPressed), "SCROLLBAR", 1, 7 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.DownDisabled), "SCROLLBAR", 1, 8 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.LeftNormal), "SCROLLBAR", 1, 9 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.LeftHot), "SCROLLBAR", 1, 10 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.LeftPressed), "SCROLLBAR", 1, 11 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.LeftDisabled), "SCROLLBAR", 1, 12 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.RightNormal), "SCROLLBAR", 1, 13 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.RightHot), "SCROLLBAR", 1, 14 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.RightPressed), "SCROLLBAR", 1, 15 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ArrowButton.RightDisabled), "SCROLLBAR", 1, 16 };

            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Normal), "SCROLLBAR", 2, 1 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Hot), "SCROLLBAR", 2, 2 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Pressed), "SCROLLBAR", 2, 3 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ThumbButtonHorizontal.Disabled), "SCROLLBAR", 2, 4 };

            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ThumbButtonVertical.Normal), "SCROLLBAR", 3, 1 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ThumbButtonVertical.Hot), "SCROLLBAR", 3, 2 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ThumbButtonVertical.Pressed), "SCROLLBAR", 3, 3 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.ThumbButtonVertical.Disabled), "SCROLLBAR", 3, 4 };

            yield return new object[] { I(() => VisualStyleElement.ScrollBar.RightTrackHorizontal.Normal), "SCROLLBAR", 4, 1 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.RightTrackHorizontal.Hot), "SCROLLBAR", 4, 2 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.RightTrackHorizontal.Pressed), "SCROLLBAR", 4, 3 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.RightTrackHorizontal.Disabled), "SCROLLBAR", 4, 4 };

            yield return new object[] { I(() => VisualStyleElement.ScrollBar.LeftTrackHorizontal.Normal), "SCROLLBAR", 5, 1 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.LeftTrackHorizontal.Hot), "SCROLLBAR", 5, 2 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.LeftTrackHorizontal.Pressed), "SCROLLBAR", 5, 3 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.LeftTrackHorizontal.Disabled), "SCROLLBAR", 5, 4 };

            yield return new object[] { I(() => VisualStyleElement.ScrollBar.LowerTrackVertical.Normal), "SCROLLBAR", 6, 1 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.LowerTrackVertical.Hot), "SCROLLBAR", 6, 2 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.LowerTrackVertical.Pressed), "SCROLLBAR", 6, 3 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.LowerTrackVertical.Disabled), "SCROLLBAR", 6, 4 };

            yield return new object[] { I(() => VisualStyleElement.ScrollBar.UpperTrackVertical.Normal), "SCROLLBAR", 7, 1 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.UpperTrackVertical.Hot), "SCROLLBAR", 7, 2 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.UpperTrackVertical.Pressed), "SCROLLBAR", 7, 3 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.UpperTrackVertical.Disabled), "SCROLLBAR", 7, 4 };

            yield return new object[] { I(() => VisualStyleElement.ScrollBar.GripperHorizontal.Normal), "SCROLLBAR", 8, 0 };

            yield return new object[] { I(() => VisualStyleElement.ScrollBar.GripperVertical.Normal), "SCROLLBAR", 9, 0 };

            yield return new object[] { I(() => VisualStyleElement.ScrollBar.SizeBox.RightAlign), "SCROLLBAR", 10, 1 };
            yield return new object[] { I(() => VisualStyleElement.ScrollBar.SizeBox.LeftAlign), "SCROLLBAR", 10, 2 };

            yield return new object[] { I(() => VisualStyleElement.Tab.TabItem.Normal), "TAB", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TabItem.Hot), "TAB", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TabItem.Pressed), "TAB", 1, 3 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TabItem.Disabled), "TAB", 1, 4 };

            yield return new object[] { I(() => VisualStyleElement.Tab.TabItemLeftEdge.Normal), "TAB", 2, 1 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TabItemLeftEdge.Hot), "TAB", 2, 2 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TabItemLeftEdge.Pressed), "TAB", 2, 3 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TabItemLeftEdge.Disabled), "TAB", 2, 4 };

            yield return new object[] { I(() => VisualStyleElement.Tab.TabItemRightEdge.Normal), "TAB", 3, 1 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TabItemRightEdge.Hot), "TAB", 3, 2 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TabItemRightEdge.Pressed), "TAB", 3, 3 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TabItemRightEdge.Disabled), "TAB", 3, 4 };

            yield return new object[] { I(() => VisualStyleElement.Tab.TabItemBothEdges.Normal), "TAB", 4, 0 };

            yield return new object[] { I(() => VisualStyleElement.Tab.TopTabItem.Normal), "TAB", 5, 1 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TopTabItem.Hot), "TAB", 5, 2 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TopTabItem.Pressed), "TAB", 5, 3 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TopTabItem.Disabled), "TAB", 5, 4 };

            yield return new object[] { I(() => VisualStyleElement.Tab.TopTabItemLeftEdge.Normal), "TAB", 6, 1 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TopTabItemLeftEdge.Hot), "TAB", 6, 2 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TopTabItemLeftEdge.Pressed), "TAB", 6, 3 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TopTabItemLeftEdge.Disabled), "TAB", 6, 4 };

            yield return new object[] { I(() => VisualStyleElement.Tab.TopTabItemRightEdge.Normal), "TAB", 7, 1 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TopTabItemRightEdge.Hot), "TAB", 7, 2 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TopTabItemRightEdge.Pressed), "TAB", 7, 3 };
            yield return new object[] { I(() => VisualStyleElement.Tab.TopTabItemRightEdge.Disabled), "TAB", 7, 4 };

            yield return new object[] { I(() => VisualStyleElement.Tab.TopTabItemBothEdges.Normal), "TAB", 8, 0 };

            yield return new object[] { I(() => VisualStyleElement.Tab.Pane.Normal), "TAB", 9, 0 };

            yield return new object[] { I(() => VisualStyleElement.Tab.Body.Normal), "TAB", 10, 0 };

            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.HeaderBackground.Normal), "EXPLORERBAR", 1, 0 };

            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.HeaderClose.Normal), "EXPLORERBAR", 2, 1 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.HeaderClose.Hot), "EXPLORERBAR", 2, 2 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.HeaderClose.Pressed), "EXPLORERBAR", 2, 3 };

            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.HeaderPin.Normal), "EXPLORERBAR", 3, 1 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.HeaderPin.Hot), "EXPLORERBAR", 3, 2 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.HeaderPin.Pressed), "EXPLORERBAR", 3, 3 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.HeaderPin.SelectedNormal), "EXPLORERBAR", 3, 4 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.HeaderPin.SelectedHot), "EXPLORERBAR", 3, 5 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.HeaderPin.SelectedPressed), "EXPLORERBAR", 3, 6 };

            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.IEBarMenu.Normal), "EXPLORERBAR", 4, 1 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.IEBarMenu.Hot), "EXPLORERBAR", 4, 2 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.IEBarMenu.Pressed), "EXPLORERBAR", 4, 3 };

            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.NormalGroupBackground.Normal), "EXPLORERBAR", 5, 0 };

            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.NormalGroupCollapse.Normal), "EXPLORERBAR", 6, 1 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.NormalGroupCollapse.Hot), "EXPLORERBAR", 6, 2 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.NormalGroupCollapse.Pressed), "EXPLORERBAR", 6, 3 };

            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.NormalGroupExpand.Normal), "EXPLORERBAR", 7, 1 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.NormalGroupExpand.Hot), "EXPLORERBAR", 7, 2 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.NormalGroupExpand.Pressed), "EXPLORERBAR", 7, 3 };

            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.NormalGroupHead.Normal), "EXPLORERBAR", 8, 0 };

            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.SpecialGroupBackground.Normal), "EXPLORERBAR", 9, 0 };

            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.SpecialGroupCollapse.Normal), "EXPLORERBAR", 10, 1 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.SpecialGroupCollapse.Hot), "EXPLORERBAR", 10, 2 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.SpecialGroupCollapse.Pressed), "EXPLORERBAR", 10, 3 };

            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.SpecialGroupExpand.Normal), "EXPLORERBAR", 11, 1 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.SpecialGroupExpand.Hot), "EXPLORERBAR", 11, 2 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.SpecialGroupExpand.Pressed), "EXPLORERBAR", 11, 3 };

            yield return new object[] { I(() => VisualStyleElement.ExplorerBar.SpecialGroupHead.Normal), "EXPLORERBAR", 12, 0 };

            yield return new object[] { I(() => VisualStyleElement.Header.Item.Normal), "HEADER", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.Header.Item.Hot), "HEADER", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.Header.Item.Pressed), "HEADER", 1, 3 };

            yield return new object[] { I(() => VisualStyleElement.Header.ItemLeft.Normal), "HEADER", 2, 1 };
            yield return new object[] { I(() => VisualStyleElement.Header.ItemLeft.Hot), "HEADER", 2, 2 };
            yield return new object[] { I(() => VisualStyleElement.Header.ItemLeft.Pressed), "HEADER", 2, 3 };

            yield return new object[] { I(() => VisualStyleElement.Header.ItemRight.Normal), "HEADER", 3, 1 };
            yield return new object[] { I(() => VisualStyleElement.Header.ItemRight.Hot), "HEADER", 3, 2 };
            yield return new object[] { I(() => VisualStyleElement.Header.ItemRight.Pressed), "HEADER", 3, 3 };

            yield return new object[] { I(() => VisualStyleElement.Header.SortArrow.SortedUp), "HEADER", 4, 1 };
            yield return new object[] { I(() => VisualStyleElement.Header.SortArrow.SortedDown), "HEADER", 4, 2 };

            yield return new object[] { I(() => VisualStyleElement.ListView.Item.Normal), "LISTVIEW", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.ListView.Item.Hot), "LISTVIEW", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.ListView.Item.Selected), "LISTVIEW", 1, 3 };
            yield return new object[] { I(() => VisualStyleElement.ListView.Item.Disabled), "LISTVIEW", 1, 4 };
            yield return new object[] { I(() => VisualStyleElement.ListView.Item.SelectedNotFocus), "LISTVIEW", 1, 5 };

            yield return new object[] { I(() => VisualStyleElement.ListView.Group.Normal), "LISTVIEW", 2, 0 };

            yield return new object[] { I(() => VisualStyleElement.ListView.Detail.Normal), "LISTVIEW", 3, 0 };

            yield return new object[] { I(() => VisualStyleElement.ListView.SortedDetail.Normal), "LISTVIEW", 4, 0 };

            yield return new object[] { I(() => VisualStyleElement.ListView.EmptyText.Normal), "LISTVIEW", 5, 0 };

            yield return new object[] { I(() => VisualStyleElement.MenuBand.NewApplicationButton.Normal), "MENUBAND", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.MenuBand.NewApplicationButton.Hot), "MENUBAND", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.MenuBand.NewApplicationButton.Pressed), "MENUBAND", 1, 3 };
            yield return new object[] { I(() => VisualStyleElement.MenuBand.NewApplicationButton.Disabled), "MENUBAND", 1, 4 };
            yield return new object[] { I(() => VisualStyleElement.MenuBand.NewApplicationButton.Checked), "MENUBAND", 1, 5 };
            yield return new object[] { I(() => VisualStyleElement.MenuBand.NewApplicationButton.HotChecked), "MENUBAND", 1, 6 };

            yield return new object[] { I(() => VisualStyleElement.MenuBand.Separator.Normal), "MENUBAND", 2, 0 };

            yield return new object[] { I(() => VisualStyleElement.Menu.Item.Normal), "MENU", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.Menu.Item.Selected), "MENU", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.Menu.Item.Demoted), "MENU", 1, 3 };

            yield return new object[] { I(() => VisualStyleElement.Menu.DropDown.Normal), "MENU", 2, 0 };

            yield return new object[] { I(() => VisualStyleElement.Menu.BarItem.Normal), "MENU", 3, 0 };

            yield return new object[] { I(() => VisualStyleElement.Menu.BarDropDown.Normal), "MENU", 4, 0 };

            yield return new object[] { I(() => VisualStyleElement.Menu.Chevron.Normal), "MENU", 5, 0 };

            yield return new object[] { I(() => VisualStyleElement.Menu.Separator.Normal), "MENU", 6, 0 };

            yield return new object[] { I(() => VisualStyleElement.ProgressBar.Bar.Normal), "PROGRESS", 1, 0 };

            yield return new object[] { I(() => VisualStyleElement.ProgressBar.BarVertical.Normal), "PROGRESS", 2, 0 };

            yield return new object[] { I(() => VisualStyleElement.ProgressBar.Chunk.Normal), "PROGRESS", 3, 0 };

            yield return new object[] { I(() => VisualStyleElement.ProgressBar.ChunkVertical.Normal), "PROGRESS", 4, 0 };

            yield return new object[] { I(() => VisualStyleElement.Rebar.Gripper.Normal), "REBAR", 1, 0 };

            yield return new object[] { I(() => VisualStyleElement.Rebar.GripperVertical.Normal), "REBAR", 2, 0 };

            yield return new object[] { I(() => VisualStyleElement.Rebar.Band.Normal), "REBAR", 3, 0 };

            yield return new object[] { I(() => VisualStyleElement.Rebar.Chevron.Normal), "REBAR", 4, 1 };
            yield return new object[] { I(() => VisualStyleElement.Rebar.Chevron.Hot), "REBAR", 4, 2 };
            yield return new object[] { I(() => VisualStyleElement.Rebar.Chevron.Pressed), "REBAR", 4, 3 };

            yield return new object[] { I(() => VisualStyleElement.Rebar.ChevronVertical.Normal), "REBAR", 5, 1 };
            yield return new object[] { I(() => VisualStyleElement.Rebar.ChevronVertical.Hot), "REBAR", 5, 2 };
            yield return new object[] { I(() => VisualStyleElement.Rebar.ChevronVertical.Pressed), "REBAR", 5, 3 };

            yield return new object[] { I(() => VisualStyleElement.StartPanel.UserPane.Normal), "STARTPANEL", 1, 0 };

            yield return new object[] { I(() => VisualStyleElement.StartPanel.MorePrograms.Normal), "STARTPANEL", 2, 0 };

            yield return new object[] { I(() => VisualStyleElement.StartPanel.MoreProgramsArrow.Normal), "STARTPANEL", 3, 1 };
            yield return new object[] { I(() => VisualStyleElement.StartPanel.MoreProgramsArrow.Hot), "STARTPANEL", 3, 2 };
            yield return new object[] { I(() => VisualStyleElement.StartPanel.MoreProgramsArrow.Pressed), "STARTPANEL", 3, 3 };

            yield return new object[] { I(() => VisualStyleElement.StartPanel.ProgList.Normal), "STARTPANEL", 4, 0 };

            yield return new object[] { I(() => VisualStyleElement.StartPanel.ProgListSeparator.Normal), "STARTPANEL", 5, 0 };

            yield return new object[] { I(() => VisualStyleElement.StartPanel.PlaceList.Normal), "STARTPANEL", 6, 0 };

            yield return new object[] { I(() => VisualStyleElement.StartPanel.PlaceListSeparator.Normal), "STARTPANEL", 7, 0 };

            yield return new object[] { I(() => VisualStyleElement.StartPanel.LogOff.Normal), "STARTPANEL", 8, 0 };

            yield return new object[] { I(() => VisualStyleElement.StartPanel.LogOffButtons.Normal), "STARTPANEL", 9, 1 };
            yield return new object[] { I(() => VisualStyleElement.StartPanel.LogOffButtons.Hot), "STARTPANEL", 9, 2 };
            yield return new object[] { I(() => VisualStyleElement.StartPanel.LogOffButtons.Pressed), "STARTPANEL", 9, 3 };

            yield return new object[] { I(() => VisualStyleElement.StartPanel.UserPicture.Normal), "STARTPANEL", 10, 0 };

            yield return new object[] { I(() => VisualStyleElement.StartPanel.Preview.Normal), "STARTPANEL", 11, 0 };

            yield return new object[] { I(() => VisualStyleElement.Status.Bar.Normal), "STATUS", 0, 0 };

            yield return new object[] { I(() => VisualStyleElement.Status.Pane.Normal), "STATUS", 1, 0 };

            yield return new object[] { I(() => VisualStyleElement.Status.GripperPane.Normal), "STATUS", 2, 0 };

            yield return new object[] { I(() => VisualStyleElement.Status.Gripper.Normal), "STATUS", 3, 0 };

            yield return new object[] { I(() => VisualStyleElement.TaskBand.GroupCount.Normal), "TASKBAND", 1, 0 };

            yield return new object[] { I(() => VisualStyleElement.TaskBand.FlashButton.Normal), "TASKBAND", 2, 0 };

            yield return new object[] { I(() => VisualStyleElement.TaskBand.FlashButtonGroupMenu.Normal), "TASKBAND", 3, 0 };

            yield return new object[] { I(() => VisualStyleElement.TaskbarClock.Time.Normal), "CLOCK", 1, 1 };

            yield return new object[] { I(() => VisualStyleElement.Taskbar.BackgroundBottom.Normal), "TASKBAR", 1, 0 };

            yield return new object[] { I(() => VisualStyleElement.Taskbar.BackgroundRight.Normal), "TASKBAR", 2, 0 };

            yield return new object[] { I(() => VisualStyleElement.Taskbar.BackgroundTop.Normal), "TASKBAR", 3, 0 };

            yield return new object[] { I(() => VisualStyleElement.Taskbar.BackgroundLeft.Normal), "TASKBAR", 4, 0 };

            yield return new object[] { I(() => VisualStyleElement.Taskbar.SizingBarBottom.Normal), "TASKBAR", 5, 0 };

            yield return new object[] { I(() => VisualStyleElement.Taskbar.SizingBarRight.Normal), "TASKBAR", 6, 0 };

            yield return new object[] { I(() => VisualStyleElement.Taskbar.SizingBarTop.Normal), "TASKBAR", 7, 0 };

            yield return new object[] { I(() => VisualStyleElement.Taskbar.SizingBarLeft.Normal), "TASKBAR", 8, 0 };

            yield return new object[] { I(() => VisualStyleElement.ToolBar.Bar.Normal), "TOOLBAR", 0, 0 };

            yield return new object[] { I(() => VisualStyleElement.ToolBar.Button.Normal), "TOOLBAR", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.Button.Hot), "TOOLBAR", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.Button.Pressed), "TOOLBAR", 1, 3 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.Button.Disabled), "TOOLBAR", 1, 4 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.Button.Checked), "TOOLBAR", 1, 5 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.Button.HotChecked), "TOOLBAR", 1, 6 };

            yield return new object[] { I(() => VisualStyleElement.ToolBar.DropDownButton.Normal), "TOOLBAR", 2, 1 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.DropDownButton.Hot), "TOOLBAR", 2, 2 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.DropDownButton.Pressed), "TOOLBAR", 2, 3 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.DropDownButton.Disabled), "TOOLBAR", 2, 4 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.DropDownButton.Checked), "TOOLBAR", 2, 5 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.DropDownButton.HotChecked), "TOOLBAR", 2, 6 };

            yield return new object[] { I(() => VisualStyleElement.ToolBar.SplitButton.Normal), "TOOLBAR", 3, 1 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.SplitButton.Hot), "TOOLBAR", 3, 2 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.SplitButton.Pressed), "TOOLBAR", 3, 3 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.SplitButton.Disabled), "TOOLBAR", 3, 4 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.SplitButton.Checked), "TOOLBAR", 3, 5 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.SplitButton.HotChecked), "TOOLBAR", 3, 6 };

            yield return new object[] { I(() => VisualStyleElement.ToolBar.SplitButtonDropDown.Normal), "TOOLBAR", 4, 1 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.SplitButtonDropDown.Hot), "TOOLBAR", 4, 2 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.SplitButtonDropDown.Pressed), "TOOLBAR", 4, 3 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.SplitButtonDropDown.Disabled), "TOOLBAR", 4, 4 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.SplitButtonDropDown.Checked), "TOOLBAR", 4, 5 };
            yield return new object[] { I(() => VisualStyleElement.ToolBar.SplitButtonDropDown.HotChecked), "TOOLBAR", 4, 6 };

            yield return new object[] { I(() => VisualStyleElement.ToolBar.SeparatorHorizontal.Normal), "TOOLBAR", 5, 0 };

            yield return new object[] { I(() => VisualStyleElement.ToolBar.SeparatorVertical.Normal), "TOOLBAR", 6, 0 };

            yield return new object[] { I(() => VisualStyleElement.ToolTip.Standard.Normal), "TOOLTIP", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.ToolTip.Standard.Link), "TOOLTIP", 1, 2 };

            yield return new object[] { I(() => VisualStyleElement.ToolTip.StandardTitle.Normal), "TOOLTIP", 2, 0 };

            yield return new object[] { I(() => VisualStyleElement.ToolTip.Balloon.Normal), "TOOLTIP", 3, 1 };
            yield return new object[] { I(() => VisualStyleElement.ToolTip.Balloon.Link), "TOOLTIP", 3, 2 };

            yield return new object[] { I(() => VisualStyleElement.ToolTip.BalloonTitle.Normal), "TOOLTIP", 4, 0 };

            yield return new object[] { I(() => VisualStyleElement.ToolTip.Close.Normal), "TOOLTIP", 5, 1 };
            yield return new object[] { I(() => VisualStyleElement.ToolTip.Close.Hot), "TOOLTIP", 5, 2 };
            yield return new object[] { I(() => VisualStyleElement.ToolTip.Close.Pressed), "TOOLTIP", 5, 3 };

            yield return new object[] { I(() => VisualStyleElement.TrackBar.Track.Normal), "TRACKBAR", 1, 1 };

            yield return new object[] { I(() => VisualStyleElement.TrackBar.TrackVertical.Normal), "TRACKBAR", 2, 1 };

            yield return new object[] { I(() => VisualStyleElement.TrackBar.Thumb.Normal), "TRACKBAR", 3, 1 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.Thumb.Hot), "TRACKBAR", 3, 2 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.Thumb.Pressed), "TRACKBAR", 3, 3 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.Thumb.Focused), "TRACKBAR", 3, 4 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.Thumb.Disabled), "TRACKBAR", 3, 5 };

            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbBottom.Normal), "TRACKBAR", 4, 1 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbBottom.Hot), "TRACKBAR", 4, 2 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbBottom.Pressed), "TRACKBAR", 4, 3 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbBottom.Focused), "TRACKBAR", 4, 4 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbBottom.Disabled), "TRACKBAR", 4, 5 };

            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbTop.Normal), "TRACKBAR", 5, 1 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbTop.Hot), "TRACKBAR", 5, 2 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbTop.Pressed), "TRACKBAR", 5, 3 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbTop.Focused), "TRACKBAR", 5, 4 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbTop.Disabled), "TRACKBAR", 5, 5 };

            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbVertical.Normal), "TRACKBAR", 6, 1 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbVertical.Hot), "TRACKBAR", 6, 2 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbVertical.Pressed), "TRACKBAR", 6, 3 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbVertical.Focused), "TRACKBAR", 6, 4 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbVertical.Disabled), "TRACKBAR", 6, 5 };

            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbLeft.Normal), "TRACKBAR", 7, 1 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbLeft.Hot), "TRACKBAR", 7, 2 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbLeft.Pressed), "TRACKBAR", 7, 3 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbLeft.Focused), "TRACKBAR", 7, 4 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbLeft.Disabled), "TRACKBAR", 7, 5 };

            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbRight.Normal), "TRACKBAR", 8, 1 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbRight.Hot), "TRACKBAR", 8, 2 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbRight.Pressed), "TRACKBAR", 8, 3 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbRight.Focused), "TRACKBAR", 8, 4 };
            yield return new object[] { I(() => VisualStyleElement.TrackBar.ThumbRight.Disabled), "TRACKBAR", 8, 5 };

            yield return new object[] { I(() => VisualStyleElement.TrackBar.Ticks.Normal), "TRACKBAR", 9, 1 };

            yield return new object[] { I(() => VisualStyleElement.TrackBar.TicksVertical.Normal), "TRACKBAR", 10, 1 };

            yield return new object[] { I(() => VisualStyleElement.TreeView.Item.Normal), "TREEVIEW", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.TreeView.Item.Hot), "TREEVIEW", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.TreeView.Item.Selected), "TREEVIEW", 1, 3 };
            yield return new object[] { I(() => VisualStyleElement.TreeView.Item.Disabled), "TREEVIEW", 1, 4 };
            yield return new object[] { I(() => VisualStyleElement.TreeView.Item.SelectedNotFocus), "TREEVIEW", 1, 5 };

            yield return new object[] { I(() => VisualStyleElement.TreeView.Glyph.Closed), "TREEVIEW", 2, 1 };
            yield return new object[] { I(() => VisualStyleElement.TreeView.Glyph.Opened), "TREEVIEW", 2, 2 };

            yield return new object[] { I(() => VisualStyleElement.TreeView.Branch.Normal), "TREEVIEW", 3, 0 };

            yield return new object[] { I(() => VisualStyleElement.ExplorerTreeView.Glyph.Closed), "Explorer::TreeView", 2, 1 };
            yield return new object[] { I(() => VisualStyleElement.ExplorerTreeView.Glyph.Opened), "Explorer::TreeView", 2, 2 };

            yield return new object[] { I(() => VisualStyleElement.TextBox.TextEdit.Normal), "EDIT", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.TextBox.TextEdit.Hot), "EDIT", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.TextBox.TextEdit.Selected), "EDIT", 1, 3 };
            yield return new object[] { I(() => VisualStyleElement.TextBox.TextEdit.Disabled), "EDIT", 1, 4 };
            yield return new object[] { I(() => VisualStyleElement.TextBox.TextEdit.Focused), "EDIT", 1, 5 };
            yield return new object[] { I(() => VisualStyleElement.TextBox.TextEdit.ReadOnly), "EDIT", 1, 6 };
            yield return new object[] { I(() => VisualStyleElement.TextBox.TextEdit.Assist), "EDIT", 1, 7 };

            yield return new object[] { I(() => VisualStyleElement.TextBox.Caret.Normal), "EDIT", 2, 0 };

            yield return new object[] { I(() => VisualStyleElement.TrayNotify.Background.Normal), "TRAYNOTIFY", 1, 0 };

            yield return new object[] { I(() => VisualStyleElement.TrayNotify.AnimateBackground.Normal), "TRAYNOTIFY", 2, 0 };

            yield return new object[] { I(() => VisualStyleElement.Window.Caption.Active), "WINDOW", 1, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.Caption.Inactive), "WINDOW", 1, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.Caption.Disabled), "WINDOW", 1, 3 };

            yield return new object[] { I(() => VisualStyleElement.Window.SmallCaption.Active), "WINDOW", 2, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.SmallCaption.Inactive), "WINDOW", 2, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.SmallCaption.Disabled), "WINDOW", 2, 3 };

            yield return new object[] { I(() => VisualStyleElement.Window.MinCaption.Active), "WINDOW", 3, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.MinCaption.Inactive), "WINDOW", 3, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.MinCaption.Disabled), "WINDOW", 3, 3 };

            yield return new object[] { I(() => VisualStyleElement.Window.SmallMinCaption.Active), "WINDOW", 4, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.SmallMinCaption.Inactive), "WINDOW", 4, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.SmallMinCaption.Disabled), "WINDOW", 4, 3 };

            yield return new object[] { I(() => VisualStyleElement.Window.MaxCaption.Active), "WINDOW", 5, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.MaxCaption.Inactive), "WINDOW", 5, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.MaxCaption.Disabled), "WINDOW", 5, 3 };

            yield return new object[] { I(() => VisualStyleElement.Window.SmallMaxCaption.Active), "WINDOW", 6, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.SmallMaxCaption.Inactive), "WINDOW", 6, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.SmallMaxCaption.Disabled), "WINDOW", 6, 3 };

            yield return new object[] { I(() => VisualStyleElement.Window.FrameLeft.Active), "WINDOW", 7, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.FrameLeft.Inactive), "WINDOW", 7, 2 };

            yield return new object[] { I(() => VisualStyleElement.Window.FrameRight.Active), "WINDOW", 8, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.FrameRight.Inactive), "WINDOW", 8, 2 };

            yield return new object[] { I(() => VisualStyleElement.Window.FrameBottom.Active), "WINDOW", 9, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.FrameBottom.Inactive), "WINDOW", 9, 2 };

            yield return new object[] { I(() => VisualStyleElement.Window.SmallFrameLeft.Active), "WINDOW", 10, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.SmallFrameLeft.Inactive), "WINDOW", 10, 2 };

            yield return new object[] { I(() => VisualStyleElement.Window.SmallFrameRight.Active), "WINDOW", 11, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.SmallFrameRight.Inactive), "WINDOW", 11, 2 };

            yield return new object[] { I(() => VisualStyleElement.Window.SmallFrameBottom.Active), "WINDOW", 12, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.SmallFrameBottom.Inactive), "WINDOW", 12, 2 };

            yield return new object[] { I(() => VisualStyleElement.Window.SysButton.Normal), "WINDOW", 13, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.SysButton.Hot), "WINDOW", 13, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.SysButton.Pressed), "WINDOW", 13, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.SysButton.Disabled), "WINDOW", 13, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.MdiSysButton.Normal), "WINDOW", 14, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiSysButton.Hot), "WINDOW", 14, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiSysButton.Pressed), "WINDOW", 14, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiSysButton.Disabled), "WINDOW", 14, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.MinButton.Normal), "WINDOW", 15, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.MinButton.Hot), "WINDOW", 15, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.MinButton.Pressed), "WINDOW", 15, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.MinButton.Disabled), "WINDOW", 15, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.MdiMinButton.Normal), "WINDOW", 16, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiMinButton.Hot), "WINDOW", 16, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiMinButton.Pressed), "WINDOW", 16, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiMinButton.Disabled), "WINDOW", 16, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.MaxButton.Normal), "WINDOW", 17, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.MaxButton.Hot), "WINDOW", 17, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.MaxButton.Pressed), "WINDOW", 17, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.MaxButton.Disabled), "WINDOW", 17, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.CloseButton.Normal), "WINDOW", 18, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.CloseButton.Hot), "WINDOW", 18, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.CloseButton.Pressed), "WINDOW", 18, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.CloseButton.Disabled), "WINDOW", 18, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.SmallCloseButton.Normal), "WINDOW", 19, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.SmallCloseButton.Hot), "WINDOW", 19, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.SmallCloseButton.Pressed), "WINDOW", 19, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.SmallCloseButton.Disabled), "WINDOW", 19, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.MdiCloseButton.Normal), "WINDOW", 20, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiCloseButton.Hot), "WINDOW", 20, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiCloseButton.Pressed), "WINDOW", 20, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiCloseButton.Disabled), "WINDOW", 20, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.RestoreButton.Normal), "WINDOW", 21, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.RestoreButton.Hot), "WINDOW", 21, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.RestoreButton.Pressed), "WINDOW", 21, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.RestoreButton.Disabled), "WINDOW", 21, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.MdiRestoreButton.Normal), "WINDOW", 22, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiRestoreButton.Hot), "WINDOW", 22, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiRestoreButton.Pressed), "WINDOW", 22, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiRestoreButton.Disabled), "WINDOW", 22, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.HelpButton.Normal), "WINDOW", 23, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.HelpButton.Hot), "WINDOW", 23, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.HelpButton.Pressed), "WINDOW", 23, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.HelpButton.Disabled), "WINDOW", 23, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.MdiHelpButton.Normal), "WINDOW", 24, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiHelpButton.Hot), "WINDOW", 24, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiHelpButton.Pressed), "WINDOW", 24, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.MdiHelpButton.Disabled), "WINDOW", 24, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.HorizontalScroll.Normal), "WINDOW", 25, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.HorizontalScroll.Hot), "WINDOW", 25, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.HorizontalScroll.Pressed), "WINDOW", 25, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.HorizontalScroll.Disabled), "WINDOW", 25, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.HorizontalThumb.Normal), "WINDOW", 26, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.HorizontalThumb.Hot), "WINDOW", 26, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.HorizontalThumb.Pressed), "WINDOW", 26, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.HorizontalThumb.Disabled), "WINDOW", 26, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.VerticalScroll.Normal), "WINDOW", 27, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.VerticalScroll.Hot), "WINDOW", 27, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.VerticalScroll.Pressed), "WINDOW", 27, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.VerticalScroll.Disabled), "WINDOW", 27, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.VerticalThumb.Normal), "WINDOW", 28, 1 };
            yield return new object[] { I(() => VisualStyleElement.Window.VerticalThumb.Hot), "WINDOW", 28, 2 };
            yield return new object[] { I(() => VisualStyleElement.Window.VerticalThumb.Pressed), "WINDOW", 28, 3 };
            yield return new object[] { I(() => VisualStyleElement.Window.VerticalThumb.Disabled), "WINDOW", 28, 4 };

            yield return new object[] { I(() => VisualStyleElement.Window.Dialog.Normal), "WINDOW", 29, 0 };

            yield return new object[] { I(() => VisualStyleElement.Window.CaptionSizingTemplate.Normal), "WINDOW", 30, 0 };

            yield return new object[] { I(() => VisualStyleElement.Window.SmallCaptionSizingTemplate.Normal), "WINDOW", 31, 0 };

            yield return new object[] { I(() => VisualStyleElement.Window.FrameLeftSizingTemplate.Normal), "WINDOW", 32, 0 };

            yield return new object[] { I(() => VisualStyleElement.Window.SmallFrameLeftSizingTemplate.Normal), "WINDOW", 33, 0 };

            yield return new object[] { I(() => VisualStyleElement.Window.FrameRightSizingTemplate.Normal), "WINDOW", 34, 0 };

            yield return new object[] { I(() => VisualStyleElement.Window.SmallFrameRightSizingTemplate.Normal), "WINDOW", 35, 0 };

            yield return new object[] { I(() => VisualStyleElement.Window.FrameBottomSizingTemplate.Normal), "WINDOW", 36, 0 };

            yield return new object[] { I(() => VisualStyleElement.Window.SmallFrameBottomSizingTemplate.Normal), "WINDOW", 37, 0 };
        }

        [Theory]
        [MemberData(nameof(KnownElements_TestData))]
        public void VisualStyleElement_KnownElements_Get_ReturnsExpected(Func<VisualStyleElement> elementFactory, string expectedClassName, int expectedPart, int expectedState)
        {
            VisualStyleElement element = elementFactory();
            Assert.Same(expectedClassName, element.ClassName);
            Assert.Equal(expectedPart, element.Part);
            Assert.Equal(expectedState, element.State);
            Assert.Same(element, elementFactory());
        }
    }
}
