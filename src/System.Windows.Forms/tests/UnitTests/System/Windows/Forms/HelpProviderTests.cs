// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Tests;

public class HelpProviderTests
{
    [WinFormsFact]
    public void HelpProvider_Ctor_Default()
    {
        using SubHelpProvider provider = new();
        Assert.True(provider.CanRaiseEvents);
        Assert.Null(provider.Container);
        Assert.False(provider.DesignMode);
        Assert.NotNull(provider.Events);
        Assert.Same(provider.Events, provider.Events);
        Assert.Null(provider.HelpNamespace);
        Assert.Null(provider.Site);
        Assert.Null(provider.Tag);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void HelpProvider_HelpNamespace_Set_GetReturnsExpected(string value)
    {
        using HelpProvider provider = new()
        {
            HelpNamespace = value
        };
        Assert.Equal(value, provider.HelpNamespace);

        // Set same.
        provider.HelpNamespace = value;
        Assert.Equal(value, provider.HelpNamespace);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void HelpProvider_HelpNamespace_SetWithBoundControls_GetReturnsExpected(string value)
    {
        using HelpProvider provider = new()
        {
            HelpNamespace = value
        };
        Assert.Equal(value, provider.HelpNamespace);

        using Control control = new();
        provider.SetShowHelp(control, true);
        Assert.Equal(0, control.AccessibilityObject.GetHelpTopic(out string fileName));
        Assert.Equal(value, fileName);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void HelpProvider_Tag_Set_GetReturnsExpected(string value)
    {
        using HelpProvider provider = new()
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
        yield return new object[] { new(), false };
        yield return new object[] { new Control(), true };
    }

    [WinFormsTheory]
    [MemberData(nameof(CanExtend_TestData))]
    public void HelpProvider_CanExtend_Invoke_ReturnsExpected(object target, bool expected)
    {
        using HelpProvider provider = new();
        Assert.Equal(expected, provider.CanExtend(target));
    }

    [WinFormsFact]
    public void HelpProvider_GetHelpKeyword_NoSuchControl_ReturnsNull()
    {
        using HelpProvider provider = new();
        Assert.Null(provider.GetHelpKeyword(new Control()));
    }

    [WinFormsFact]
    public void HelpProvider_GetHelpKeyword_NullCtl_ThrowsArgumentNullException()
    {
        using HelpProvider provider = new();
        Assert.Throws<ArgumentNullException>("ctl", () => provider.GetHelpKeyword(null));
    }

    [WinFormsFact]
    public void HelpProvider_GetHelpNavigator_NoSuchControl_ReturnsAssociateIndex()
    {
        using HelpProvider provider = new();
        Assert.Equal(HelpNavigator.AssociateIndex, provider.GetHelpNavigator(new Control()));
    }

    [WinFormsFact]
    public void HelpProvider_GetHelpNavigator_NullCtl_ThrowsArgumentNullException()
    {
        using HelpProvider provider = new();
        Assert.Throws<ArgumentNullException>("ctl", () => provider.GetHelpNavigator(null));
    }

    [WinFormsFact]
    public void HelpProvider_GetHelpString_NoSuchControl_ReturnsNull()
    {
        using HelpProvider provider = new();
        Assert.Null(provider.GetHelpString(new Control()));
    }

    [WinFormsFact]
    public void HelpProvider_GetHelpString_NullCtl_ThrowsArgumentNullException()
    {
        using HelpProvider provider = new();
        Assert.Throws<ArgumentNullException>("ctl", () => provider.GetHelpString(null));
    }

    [WinFormsFact]
    public void HelpProvider_GetShowHelp_NoSuchControl_ReturnsFalse()
    {
        using HelpProvider provider = new();
        Assert.False(provider.GetShowHelp(new Control()));
    }

    [WinFormsFact]
    public void HelpProvider_GetShowHelp_NullCtl_ThrowsArgumentNullException()
    {
        using HelpProvider provider = new();
        Assert.Throws<ArgumentNullException>("ctl", () => provider.GetShowHelp(null));
    }

    [WinFormsTheory]
    [BoolData]
    public void HelpProvider_ResetShowHelp_WithShowHelp_Success(bool showHelp)
    {
        using HelpProvider provider = new();
        using Control control = new();
        provider.SetShowHelp(control, showHelp);
        provider.ResetShowHelp(control);
        Assert.False(provider.GetShowHelp(control));
        Assert.False(provider.ShouldSerializeShowHelp(control));
    }

    [WinFormsFact]
    public void HelpProvider_ResetShowHelp_NoSuchControl_Success()
    {
        using HelpProvider provider = new();
        using Control control = new();
        provider.ResetShowHelp(control);
        Assert.False(provider.GetShowHelp(control));
        Assert.False(provider.ShouldSerializeShowHelp(control));
    }

    [WinFormsFact]
    public void HelpProvider_ResetShowHelp_NullCtl_ThrowsArgumentNullException()
    {
        using HelpProvider provider = new();
        Assert.Throws<ArgumentNullException>("ctl", () => provider.ResetShowHelp(null));
    }

    [WinFormsTheory]
    [InlineData(null, 0, null, true)]
    [InlineData("", 0, null, true)]
    [InlineData("helpKeyword", 0, "HelpNamespace", true)]
    [InlineData("1", 1, "HelpNamespace", true)]
    [InlineData(null, -1, null, false)]
    [InlineData("", -1, null, false)]
    [InlineData("helpKeyword", 0, "HelpNamespace", false)]
    [InlineData("1", 1, "HelpNamespace", false)]
    public void HelpProvider_SetHelpKeyword_GetHelpKeyword_ReturnsExpected(string keyword, int expectedHelpTopic, string expectedFileName, bool createControl)
    {
        using HelpProvider provider = new()
        {
            HelpNamespace = "HelpNamespace"
        };

        using Control control = new();
        if (createControl)
        {
            control.CreateControl();
        }

        Assert.Equal(createControl, control.IsHandleCreated);

        provider.SetHelpKeyword(control, keyword);
        Assert.Same(keyword, provider.GetHelpKeyword(control));
        Assert.Equal(!string.IsNullOrEmpty(keyword), provider.GetShowHelp(control));
        Assert.Equal(expectedHelpTopic, control.AccessibilityObject.GetHelpTopic(out string fileName));
        Assert.Equal(createControl, control.IsHandleCreated);
        Assert.Equal(expectedFileName, fileName);

        // Set same.
        provider.SetHelpKeyword(control, keyword);
        Assert.Same(keyword, provider.GetHelpKeyword(control));
        Assert.Equal(!string.IsNullOrEmpty(keyword), provider.GetShowHelp(control));
        Assert.Equal(expectedHelpTopic, control.AccessibilityObject.GetHelpTopic(out fileName));
        Assert.Equal(expectedFileName, fileName);
    }

    [WinFormsTheory]
    [InlineData(null, 0)]
    [InlineData("", 0)]
    [InlineData("helpKeyword", 0)]
    [InlineData("1", 1)]
    public void HelpProvider_SetHelpKeyword_WithShowHelpTrue_ReturnsExpected(string keyword, int expectedHelpTopic)
    {
        using HelpProvider provider = new()
        {
            HelpNamespace = "HelpNamespace"
        };
        using Control control = new();
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

    [WinFormsTheory]
    [InlineData(null, 0, null, true)]
    [InlineData("", 0, null, true)]
    [InlineData("helpKeyword", 0, "HelpNamespace", true)]
    [InlineData("1", 1, "HelpNamespace", true)]
    [InlineData(null, -1, null, false)]
    [InlineData("", -1, null, false)]
    [InlineData("helpKeyword", 0, "HelpNamespace", false)]
    [InlineData("1", 1, "HelpNamespace", false)]
    public void HelpProvider_SetHelpKeyword_WithShowHelpFalse_ReturnsExpected(string keyword, int expectedHelpTopic, string expectedFileName, bool createControl)
    {
        using HelpProvider provider = new()
        {
            HelpNamespace = "HelpNamespace"
        };
        using Control control = new();
        if (createControl)
        {
            control.CreateControl();
        }

        Assert.Equal(createControl, control.IsHandleCreated);
        provider.SetShowHelp(control, false);

        provider.SetHelpKeyword(control, keyword);
        Assert.Same(keyword, provider.GetHelpKeyword(control));
        Assert.Equal(!string.IsNullOrEmpty(keyword), provider.GetShowHelp(control));
        Assert.Equal(expectedHelpTopic, control.AccessibilityObject.GetHelpTopic(out string fileName));
        Assert.Equal(createControl, control.IsHandleCreated);
        Assert.Equal(expectedFileName, fileName);

        // Set same.
        provider.SetHelpKeyword(control, keyword);
        Assert.Same(keyword, provider.GetHelpKeyword(control));
        Assert.Equal(!string.IsNullOrEmpty(keyword), provider.GetShowHelp(control));
        Assert.Equal(expectedFileName, fileName);
    }

    [WinFormsFact]
    public void HelpProvider_SetHelpKeyword_NullCtl_ThrowsArgumentNullException()
    {
        using HelpProvider provider = new();
        Assert.Throws<ArgumentNullException>("ctl", () => provider.SetHelpKeyword(null, "keyword"));
    }

    [WinFormsTheory]
    [EnumData<HelpNavigator>]
    public void HelpProvider_SetHelpNavigator_GetHelpNavigator_ReturnsExpected(HelpNavigator navigator)
    {
        using HelpProvider provider = new();
        using Control control = new();

        provider.SetHelpNavigator(control, navigator);
        Assert.Equal(navigator, provider.GetHelpNavigator(control));
        Assert.True(provider.GetShowHelp(control));

        // Set same.
        provider.SetHelpNavigator(control, navigator);
        Assert.Equal(navigator, provider.GetHelpNavigator(control));
        Assert.True(provider.GetShowHelp(control));
    }

    [WinFormsTheory]
    [EnumData<HelpNavigator>]
    public void HelpProvider_SetHelpNavigator_WithShowHelpTrue_ReturnsExpected(HelpNavigator navigator)
    {
        using HelpProvider provider = new();
        using Control control = new();
        provider.SetShowHelp(control, true);

        provider.SetHelpNavigator(control, navigator);
        Assert.Equal(navigator, provider.GetHelpNavigator(control));
        Assert.True(provider.GetShowHelp(control));

        // Set same.
        provider.SetHelpNavigator(control, navigator);
        Assert.Equal(navigator, provider.GetHelpNavigator(control));
        Assert.True(provider.GetShowHelp(control));
    }

    [WinFormsTheory]
    [EnumData<HelpNavigator>]
    public void HelpProvider_SetHelpNavigator_WithShowHelpFalse_ReturnsExpected(HelpNavigator navigator)
    {
        using HelpProvider provider = new();
        using Control control = new();
        provider.SetShowHelp(control, false);

        provider.SetHelpNavigator(control, navigator);
        Assert.Equal(navigator, provider.GetHelpNavigator(control));
        Assert.True(provider.GetShowHelp(control));

        // Set same.
        provider.SetHelpNavigator(control, navigator);
        Assert.Equal(navigator, provider.GetHelpNavigator(control));
        Assert.True(provider.GetShowHelp(control));
    }

    [WinFormsTheory]
    [EnumData<HelpNavigator>]
    [InvalidEnumData<HelpNavigator>]
    public void HelpProvider_SetHelpNavigator_NullCtl_ThrowsArgumentNullException(HelpNavigator navigator)
    {
        using HelpProvider provider = new();
        Assert.Throws<ArgumentNullException>("ctl", () => provider.SetHelpNavigator(null, navigator));
    }

    [WinFormsTheory]
    [InvalidEnumData<HelpNavigator>]
    public void HelpProvider_SetHelpNavigator_InvalidNavigator_ThrowsInvalidEnumArgumentException(HelpNavigator navigator)
    {
        using HelpProvider provider = new();
        Assert.Throws<InvalidEnumArgumentException>("navigator", () => provider.SetHelpNavigator(new Control(), navigator));
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void HelpProvider_SetHelpString_GetHelpString_ReturnsExpected(string helpString)
    {
        using HelpProvider provider = new();
        using Control control = new();

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

    [WinFormsTheory]
    [StringWithNullData]
    public void HelpProvider_SetHelpString_WithShowHelpTrue_ReturnsExpected(string helpString)
    {
        using HelpProvider provider = new();
        using Control control = new();
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

    [WinFormsTheory]
    [StringWithNullData]
    public void HelpProvider_SetHelpString_WithShowHelpFalse_ReturnsExpected(string helpString)
    {
        using HelpProvider provider = new();
        using Control control = new();
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

    [WinFormsFact]
    public void HelpProvider_SetHelpString_NullCtl_ThrowsArgumentNullException()
    {
        using HelpProvider provider = new();
        Assert.Throws<ArgumentNullException>("ctl", () => provider.SetHelpString(null, "keyword"));
    }

    [WinFormsTheory]
    [BoolData]
    public void HelpProvider_SetShowHelp_GetShowHelp_ReturnsExpected(bool value)
    {
        using HelpProvider provider = new();
        using Control control = new();
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

    [WinFormsTheory]
    [InlineData(true, 0)]
    [InlineData(false, -1)]
    public void HelpProvider_SetShowHelp_SetFalseThenTrue_UnbindsAndBindsControl(bool createControl, int expectedHelpTopic)
    {
        using HelpProvider provider = new()
        {
            HelpNamespace = "HelpNamespace"
        };
        using Control control = new();
        if (createControl)
        {
            control.CreateControl();
        }

        Assert.Equal(createControl, control.IsHandleCreated);
        provider.SetShowHelp(control, true);
        provider.SetHelpKeyword(control, "1");
        provider.SetHelpString(control, "HelpString");

        Assert.Equal(1, control.AccessibilityObject.GetHelpTopic(out string fileName));
        Assert.Equal(createControl, control.IsHandleCreated);
        Assert.Equal("HelpNamespace", fileName);
        Assert.Equal("HelpString", control.AccessibilityObject.Help);

        // Set false.
        provider.SetShowHelp(control, false);
        Assert.Equal(expectedHelpTopic, control.AccessibilityObject.GetHelpTopic(out fileName));
        Assert.Null(fileName);
        Assert.Null(control.AccessibilityObject.Help);

        // Set true.
        provider.SetShowHelp(control, true);
        Assert.Equal(1, control.AccessibilityObject.GetHelpTopic(out fileName));
        Assert.Equal("HelpNamespace", fileName);
        Assert.Equal("HelpString", control.AccessibilityObject.Help);
    }

    [WinFormsFact]
    public void HelpProvider_SetShowHelp_NullCtl_ThrowsArgumentNullException()
    {
        using HelpProvider provider = new();
        Assert.Throws<ArgumentNullException>("ctl", () => provider.SetShowHelp(null, true));
    }

    [WinFormsFact]
    public void HelpProvider_ShouldSerializeShowHelp_NoSuchControl_ReturnsFalse()
    {
        using HelpProvider provider = new();
        Assert.False(provider.ShouldSerializeShowHelp(new Control()));
    }

    [WinFormsFact]
    public void HelpProvider_ShouldSerializeShowHelp_NullCtl_ThrowsArgumentNullException()
    {
        using HelpProvider provider = new();
        Assert.Throws<ArgumentNullException>("ctl", () => provider.ShouldSerializeShowHelp(null));
    }

    [WinFormsTheory]
    [InlineData(null, "System.Windows.Forms.HelpProvider, HelpNamespace: ")]
    [InlineData("", "System.Windows.Forms.HelpProvider, HelpNamespace: ")]
    [InlineData("helpNamespace", "System.Windows.Forms.HelpProvider, HelpNamespace: helpNamespace")]
    public void HelpProvider_ToString_Invoke_ReturnsExpected(string helpNamespace, string expected)
    {
        using HelpProvider provider = new()
        {
            HelpNamespace = helpNamespace
        };
        Assert.Equal(expected, provider.ToString());
    }

    [WinFormsFact]
    public void HelpProvider_BoundControl_ValidEventArgs_Nop()
    {
        using HelpProvider provider = new();
        using SubControl control = new();
        provider.SetShowHelp(control, true);

        control.OnHelpRequested(new HelpEventArgs(new Point(1, 2)));
    }

    [WinFormsFact]
    public void HelpProvider_BoundControl_NoInformation_Nop()
    {
        using HelpProvider provider = new();
        using SubControl control = new();
        provider.SetShowHelp(control, true);

        HelpEventArgs e = new(new Point(1, 2));
        control.OnHelpRequested(e);
        Assert.True(e.Handled);
    }

    [WinFormsTheory]
    [InlineData("")]
    [InlineData("InvalidUri")]
    [InlineData("file://")]
    [InlineData("file://C:/NoSuchFile")]
    public void HelpProvider_BoundControl_InvalidNamespace_ThrowsArgumentException(string helpNamespace)
    {
        using HelpProvider provider = new()
        {
            HelpNamespace = helpNamespace
        };
        using SubControl control = new();
        provider.SetShowHelp(control, true);

        HelpEventArgs e = new(new Point(1, 2));
        Assert.Throws<ArgumentException>("url", () => control.OnHelpRequested(e));
        Assert.False(e.Handled);
    }

    [WinFormsFact]
    public void HelpProvider_BoundControl_NullEventEventArgs_Nop()
    {
        using HelpProvider provider = new();
        using SubControl control = new();
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
