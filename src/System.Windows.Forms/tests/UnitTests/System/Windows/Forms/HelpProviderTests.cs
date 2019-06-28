﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class HelpProviderTests
    {
        [Fact]
        public void HelpProvider_Ctor_Default()
        {
            var provider = new SubHelpProvider();
            Assert.True(provider.CanRaiseEvents);
            Assert.Null(provider.Container);
            Assert.False(provider.DesignMode);
            Assert.NotNull(provider.Events);
            Assert.Same(provider.Events, provider.Events);
            Assert.Null(provider.HelpNamespace);
            Assert.Null(provider.Site);
            Assert.Null(provider.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void HelpProvider_HelpNamespace_Set_GetReturnsExpected(string value)
        {
            var provider = new HelpProvider
            {
                HelpNamespace = value
            };
            Assert.Equal(value, provider.HelpNamespace);

            // Set same.
            provider.HelpNamespace = value;
            Assert.Equal(value, provider.HelpNamespace);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void HelpProvider_HelpNamespace_SetWithBoundControls_GetReturnsExpected(string value)
        {
            var provider = new HelpProvider
            {
                HelpNamespace = value
            };
            Assert.Equal(value, provider.HelpNamespace);

            var control = new Control();
            provider.SetShowHelp(control, true);
            Assert.Equal(0, control.AccessibilityObject.GetHelpTopic(out string fileName));
            Assert.Equal(value, fileName);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void HelpProvider_Tag_Set_GetReturnsExpected(string value)
        {
            var provider = new HelpProvider
            {
                Tag = value
            };
            Assert.Same(value, provider.Tag);

            // Set same.
            provider.Tag = value;
            Assert.Same(value, provider.Tag);
        }

        public static IEnumerable<object[]> CanExtend_TestData()
        {
            yield return new object[] { null, false };
            yield return new object[] { new object(), false };
            yield return new object[] { new Control(), true };
        }

        [Theory]
        [MemberData(nameof(CanExtend_TestData))]
        public void HelpProvider_CanExtend_Invoke_ReturnsExpected(object target, bool expected)
        {
            var provider = new HelpProvider();
            Assert.Equal(expected, provider.CanExtend(target));
        }

        [Fact]
        public void HelpProvider_GetHelpKeyword_NoSuchControl_ReturnsNull()
        {
            var provider = new HelpProvider();
            Assert.Null(provider.GetHelpKeyword(new Control()));
        }

        [Fact]
        public void HelpProvider_GetHelpKeyword_NullCtl_ThrowsArgumentNullException()
        {
            var provider = new HelpProvider();
            Assert.Throws<ArgumentNullException>("key", () => provider.GetHelpKeyword(null));
        }

        [Fact]
        public void HelpProvider_GetHelpNavigator_NoSuchControl_ReturnsAssociateIndex()
        {
            var provider = new HelpProvider();
            Assert.Equal(HelpNavigator.AssociateIndex, provider.GetHelpNavigator(new Control()));
        }

        [Fact]
        public void HelpProvider_GetHelpNavigator_NullCtl_ThrowsArgumentNullException()
        {
            var provider = new HelpProvider();
            Assert.Throws<ArgumentNullException>("key", () => provider.GetHelpNavigator(null));
        }

        [Fact]
        public void HelpProvider_GetHelpString_NoSuchControl_ReturnsNull()
        {
            var provider = new HelpProvider();
            Assert.Null(provider.GetHelpString(new Control()));
        }

        [Fact]
        public void HelpProvider_GetHelpString_NullCtl_ThrowsArgumentNullException()
        {
            var provider = new HelpProvider();
            Assert.Throws<ArgumentNullException>("key", () => provider.GetHelpString(null));
        }

        [Fact]
        public void HelpProvider_GetShowHelp_NoSuchControl_ReturnsFalse()
        {
            var provider = new HelpProvider();
            Assert.False(provider.GetShowHelp(new Control()));
        }

        [Fact]
        public void HelpProvider_GetShowHelp_NullCtl_ThrowsArgumentNullException()
        {
            var provider = new HelpProvider();
            Assert.Throws<ArgumentNullException>("key", () => provider.GetShowHelp(null));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void HelpProvider_ResetShowHelp_WithShowHelp_Success(bool showHelp)
        {
            var provider = new HelpProvider();
            var control = new Control();
            provider.SetShowHelp(control, showHelp);
            provider.ResetShowHelp(control);
            Assert.False(provider.GetShowHelp(control));
            Assert.False(provider.ShouldSerializeShowHelp(control));
        }

        [Fact]
        public void HelpProvider_ResetShowHelp_NoSuchControl_Success()
        {
            var provider = new HelpProvider();
            var control = new Control();
            provider.ResetShowHelp(control);
            Assert.False(provider.GetShowHelp(control));
            Assert.False(provider.ShouldSerializeShowHelp(control));
        }

        [Fact]
        public void HelpProvider_ResetShowHelp_NullCtl_ThrowsArgumentNullException()
        {
            var provider = new HelpProvider();
            Assert.Throws<ArgumentNullException>("key", () => provider.ResetShowHelp(null));
        }

        [Theory]
        [InlineData(null, 0, null)]
        [InlineData("", 0, null)]
        [InlineData("helpKeyword", 0, "HelpNamespace")]
        [InlineData("1", 1, "HelpNamespace")]
        public void HelpProvider_SetHelpKeyword_GetHelpKeyword_ReturnsExpected(string keyword, int expectedHelpTopic, string expectedFileName)
        {
            var provider = new HelpProvider
            {
                HelpNamespace = "HelpNamespace"
            };
            var control = new Control();

            provider.SetHelpKeyword(control, keyword);
            Assert.Same(keyword, provider.GetHelpKeyword(control));
            Assert.Equal(!string.IsNullOrEmpty(keyword), provider.GetShowHelp(control));
            Assert.Equal(expectedHelpTopic, control.AccessibilityObject.GetHelpTopic(out string fileName));
            Assert.Equal(expectedFileName, fileName);

            // Set same.
            provider.SetHelpKeyword(control, keyword);
            Assert.Same(keyword, provider.GetHelpKeyword(control));
            Assert.Equal(!string.IsNullOrEmpty(keyword), provider.GetShowHelp(control));
            Assert.Equal(expectedHelpTopic, control.AccessibilityObject.GetHelpTopic(out fileName));
            Assert.Equal(expectedFileName, fileName);
        }

        [Theory]
        [InlineData(null, 0)]
        [InlineData("", 0)]
        [InlineData("helpKeyword", 0)]
        [InlineData("1", 1)]
        public void HelpProvider_SetHelpKeyword_WithShowHelpTrue_ReturnsExpected(string keyword, int expectedHelpTopic)
        {
            var provider = new HelpProvider
            {
                HelpNamespace = "HelpNamespace"
            };
            var control = new Control();
            provider.SetShowHelp(control, true);

            provider.SetHelpKeyword(control, keyword);
            Assert.Same(keyword, provider.GetHelpKeyword(control));
            Assert.True(provider.GetShowHelp(control));
            Assert.Equal(expectedHelpTopic, control.AccessibilityObject.GetHelpTopic(out string fileName));
            Assert.Equal("HelpNamespace", fileName);

            // Set same.
            provider.SetHelpKeyword(control, keyword);
            Assert.Same(keyword, provider.GetHelpKeyword(control));
            Assert.True(provider.GetShowHelp(control));
            Assert.Equal(expectedHelpTopic, control.AccessibilityObject.GetHelpTopic(out fileName));
            Assert.Equal("HelpNamespace", fileName);
        }

        [Theory]
        [InlineData(null, 0, null)]
        [InlineData("", 0, null)]
        [InlineData("helpKeyword", 0, "HelpNamespace")]
        [InlineData("1", 1, "HelpNamespace")]
        public void HelpProvider_SetHelpKeyword_WithShowHelpFalse_ReturnsExpected(string keyword, int expectedHelpTopic, string expectedFileName)
        {
            var provider = new HelpProvider
            {
                HelpNamespace = "HelpNamespace"
            };
            var control = new Control();
            provider.SetShowHelp(control, false);

            provider.SetHelpKeyword(control, keyword);
            Assert.Same(keyword, provider.GetHelpKeyword(control));
            Assert.Equal(!string.IsNullOrEmpty(keyword), provider.GetShowHelp(control));
            Assert.Equal(expectedHelpTopic, control.AccessibilityObject.GetHelpTopic(out string fileName));
            Assert.Equal(expectedFileName, fileName);

            // Set same.
            provider.SetHelpKeyword(control, keyword);
            Assert.Same(keyword, provider.GetHelpKeyword(control));
            Assert.Equal(!string.IsNullOrEmpty(keyword), provider.GetShowHelp(control));
            Assert.Equal(expectedFileName, fileName);
        }

        [Fact]
        public void HelpProvider_SetHelpKeyword_NullCtl_ThrowsArgumentNullException()
        {
            var provider = new HelpProvider();
            Assert.Throws<ArgumentNullException>("key", () => provider.SetHelpKeyword(null, "keyword"));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HelpNavigator))]
        public void HelpProvider_SetHelpNavigator_GetHelpNavigator_ReturnsExpected(HelpNavigator navigator)
        {
            var provider = new HelpProvider();
            var control = new Control();

            provider.SetHelpNavigator(control, navigator);
            Assert.Equal(navigator, provider.GetHelpNavigator(control));
            Assert.True(provider.GetShowHelp(control));

            // Set same.
            provider.SetHelpNavigator(control, navigator);
            Assert.Equal(navigator, provider.GetHelpNavigator(control));
            Assert.True(provider.GetShowHelp(control));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HelpNavigator))]
        public void HelpProvider_SetHelpNavigator_WithShowHelpTrue_ReturnsExpected(HelpNavigator navigator)
        {
            var provider = new HelpProvider();
            var control = new Control();
            provider.SetShowHelp(control, true);

            provider.SetHelpNavigator(control, navigator);
            Assert.Equal(navigator, provider.GetHelpNavigator(control));
            Assert.True(provider.GetShowHelp(control));

            // Set same.
            provider.SetHelpNavigator(control, navigator);
            Assert.Equal(navigator, provider.GetHelpNavigator(control));
            Assert.True(provider.GetShowHelp(control));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(HelpNavigator))]
        public void HelpProvider_SetHelpNavigator_WithShowHelpFalse_ReturnsExpected(HelpNavigator navigator)
        {
            var provider = new HelpProvider();
            var control = new Control();
            provider.SetShowHelp(control, false);

            provider.SetHelpNavigator(control, navigator);
            Assert.Equal(navigator, provider.GetHelpNavigator(control));
            Assert.True(provider.GetShowHelp(control));

            // Set same.
            provider.SetHelpNavigator(control, navigator);
            Assert.Equal(navigator, provider.GetHelpNavigator(control));
            Assert.True(provider.GetShowHelp(control));
        }

        [Fact]
        public void HelpProvider_SetHelpNavigator_NullCtl_ThrowsArgumentNullException()
        {
            var provider = new HelpProvider();
            Assert.Throws<ArgumentNullException>("key", () => provider.SetHelpNavigator(null, HelpNavigator.AssociateIndex));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(HelpNavigator))]
        public void HelpProvider_SetHelpNavigator_InvalidNavigator_ThrowsInvalidEnumArgumentException(HelpNavigator navigator)
        {
            var provider = new HelpProvider();
            Assert.Throws<InvalidEnumArgumentException>("navigator", () => provider.SetHelpNavigator(null, navigator));
            Assert.Throws<InvalidEnumArgumentException>("navigator", () => provider.SetHelpNavigator(new Control(), navigator));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void HelpProvider_SetHelpString_GetHelpString_ReturnsExpected(string helpString)
        {
            var provider = new HelpProvider();
            var control = new Control();

            provider.SetHelpString(control, helpString);
            Assert.Same(helpString, provider.GetHelpString(control));
            Assert.Equal(!string.IsNullOrEmpty(helpString), provider.GetShowHelp(control));
            Assert.Equal(string.IsNullOrEmpty(helpString) ? null : helpString, control.AccessibilityObject.Help);

            // Set same.
            provider.SetHelpString(control, helpString);
            Assert.Same(helpString, provider.GetHelpString(control));
            Assert.Equal(!string.IsNullOrEmpty(helpString), provider.GetShowHelp(control));
            Assert.Equal(string.IsNullOrEmpty(helpString) ? null : helpString, control.AccessibilityObject.Help);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void HelpProvider_SetHelpString_WithShowHelpTrue_ReturnsExpected(string helpString)
        {
            var provider = new HelpProvider();
            var control = new Control();
            provider.SetShowHelp(control, true);

            provider.SetHelpString(control, helpString);
            Assert.Same(helpString, provider.GetHelpString(control));
            Assert.True(provider.GetShowHelp(control));
            Assert.Equal(helpString, control.AccessibilityObject.Help);

            // Set same.
            provider.SetHelpString(control, helpString);
            Assert.Same(helpString, provider.GetHelpString(control));
            Assert.True(provider.GetShowHelp(control));
            Assert.Equal(helpString, control.AccessibilityObject.Help);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void HelpProvider_SetHelpString_WithShowHelpFalse_ReturnsExpected(string helpString)
        {
            var provider = new HelpProvider();
            var control = new Control();
            provider.SetShowHelp(control, false);

            provider.SetHelpString(control, helpString);
            Assert.Same(helpString, provider.GetHelpString(control));
            Assert.Equal(!string.IsNullOrEmpty(helpString), provider.GetShowHelp(control));
            Assert.Equal(string.IsNullOrEmpty(helpString) ? null : helpString, control.AccessibilityObject.Help);

            // Set same.
            provider.SetHelpString(control, helpString);
            Assert.Same(helpString, provider.GetHelpString(control));
            Assert.Equal(!string.IsNullOrEmpty(helpString), provider.GetShowHelp(control));
            Assert.Equal(string.IsNullOrEmpty(helpString) ? null : helpString, control.AccessibilityObject.Help);
        }

        [Fact]
        public void HelpProvider_SetHelpString_NullCtl_ThrowsArgumentNullException()
        {
            var provider = new HelpProvider();
            Assert.Throws<ArgumentNullException>("key", () => provider.SetHelpString(null, "keyword"));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void HelpProvider_SetShowHelp_GetShowHelp_ReturnsExpected(bool value)
        {
            var provider = new HelpProvider();
            var control = new Control();
            provider.SetShowHelp(control, value);

            Assert.Equal(value, provider.GetShowHelp(control));
            Assert.True(provider.ShouldSerializeShowHelp(control));

            // Set same.
            provider.SetShowHelp(control, value);
            Assert.Equal(value, provider.GetShowHelp(control));
            Assert.True(provider.ShouldSerializeShowHelp(control));

            // Set opposite.
            provider.SetShowHelp(control, !value);
            Assert.Equal(!value, provider.GetShowHelp(control));
            Assert.True(provider.ShouldSerializeShowHelp(control));
        }

        [Fact]
        public void HelpProvider_SetShowHelp_SetFalseThenTrue_UnbindsAndBindsControl()
        {
            var provider = new HelpProvider
            {
                HelpNamespace = "HelpNamespace"
            };
            var control = new Control();
            provider.SetShowHelp(control, true);
            provider.SetHelpKeyword(control, "1");
            provider.SetHelpString(control, "HelpString");

            Assert.Equal(1, control.AccessibilityObject.GetHelpTopic(out string fileName));
            Assert.Equal("HelpNamespace", fileName);
            Assert.Equal("HelpString", control.AccessibilityObject.Help);

            // Set false.
            provider.SetShowHelp(control, false);
            Assert.Equal(0, control.AccessibilityObject.GetHelpTopic(out fileName));
            Assert.Null(fileName);
            Assert.Null(control.AccessibilityObject.Help);

            // Set true.
            provider.SetShowHelp(control, true);
            Assert.Equal(1, control.AccessibilityObject.GetHelpTopic(out fileName));
            Assert.Equal("HelpNamespace", fileName);
            Assert.Equal("HelpString", control.AccessibilityObject.Help);
        }

        [Fact]
        public void HelpProvider_SetShowHelp_NullCtl_ThrowsArgumentNullException()
        {
            var provider = new HelpProvider();
            Assert.Throws<ArgumentNullException>("key", () => provider.SetShowHelp(null, true));
        }

        [Fact]
        public void HelpProvider_ShouldSerializeShowHelp_NoSuchControl_ReturnsFalse()
        {
            var provider = new HelpProvider();
            Assert.False(provider.ShouldSerializeShowHelp(new Control()));
        }

        [Fact]
        public void HelpProvider_ShouldSerializeShowHelp_NullCtl_ThrowsArgumentNullException()
        {
            var provider = new HelpProvider();
            Assert.Throws<ArgumentNullException>("key", () => provider.ShouldSerializeShowHelp(null));
        }

        [Theory]
        [InlineData(null, "System.Windows.Forms.HelpProvider, HelpNamespace: ")]
        [InlineData("", "System.Windows.Forms.HelpProvider, HelpNamespace: ")]
        [InlineData("helpNamespace", "System.Windows.Forms.HelpProvider, HelpNamespace: helpNamespace")]
        public void HelpProvider_ToString_Invoke_ReturnsExpected(string helpNamespace, string expected)
        {
            var provider = new HelpProvider
            {
                HelpNamespace = helpNamespace
            };
            Assert.Equal(expected, provider.ToString());
        }

        [Fact]
        public void HelpProvider_BoundControl_ValidEventArgs_Nop()
        {
            var provider = new HelpProvider();
            var control = new SubControl();
            provider.SetShowHelp(control, true);

            control.OnHelpRequested(new HelpEventArgs(new Point(1, 2)));
        }

        [Fact]
        public void HelpProvider_BoundControl_NoInformation_Nop()
        {
            var provider = new HelpProvider();
            var control = new SubControl();
            provider.SetShowHelp(control, true);

            var e = new HelpEventArgs(new Point(1, 2));
            control.OnHelpRequested(e);
            Assert.True(e.Handled);
        }

        [Theory]
        [InlineData("")]
        [InlineData("InvalidUri")]
        [InlineData("file://")]
        [InlineData("file://C:/NoSuchFile")]
        public void HelpProvider_BoundControl_InvalidNamespace_ThrowsArgumentException(string helpNamespace)
        {
            var provider = new HelpProvider
            {
                HelpNamespace = helpNamespace
            };
            var control = new SubControl();
            provider.SetShowHelp(control, true);

            var e = new HelpEventArgs(new Point(1, 2));
            Assert.Throws<ArgumentException>("url", () => control.OnHelpRequested(e));
            Assert.False(e.Handled);
        }

        [Fact]
        public void HelpProvider_BoundControl_NullEventEventArgs_Nop()
        {
            var provider = new HelpProvider();
            var control = new SubControl();
            provider.SetShowHelp(control, true);

            control.OnHelpRequested(null);
        }

        private class SubHelpProvider : HelpProvider
        {
            public new bool CanRaiseEvents => base.CanRaiseEvents;
            public new bool DesignMode => base.DesignMode;
            public new EventHandlerList Events => base.Events;
        }

        private class SubControl : Control
        {
            public new void OnHelpRequested(HelpEventArgs e) => base.OnHelpRequested(e);
        }
    }
}
