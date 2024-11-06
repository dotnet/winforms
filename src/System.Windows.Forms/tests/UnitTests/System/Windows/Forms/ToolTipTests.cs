// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Automation;
using Moq;
using Moq.Protected;

namespace System.Windows.Forms.Tests;

public class ToolTipTests
{
    [WinFormsFact]
    public void ToolTip_Ctor_Default()
    {
        using SubToolTip toolTip = new();
        Assert.True(toolTip.Active);
        Assert.Equal(500, toolTip.AutomaticDelay);
        Assert.Equal(5000, toolTip.AutoPopDelay);
        Assert.Equal(SystemColors.Info, toolTip.BackColor);
        Assert.True(toolTip.CanRaiseEvents);
        Assert.Null(toolTip.Container);
        Assert.False(toolTip.DesignMode);
        Assert.NotNull(toolTip.Events);
        Assert.Same(toolTip.Events, toolTip.Events);
        Assert.Equal(SystemColors.InfoText, toolTip.ForeColor);
        Assert.Equal(500, toolTip.InitialDelay);
        Assert.False(toolTip.IsBalloon);
        Assert.False(toolTip.OwnerDraw);
        Assert.Equal(100, toolTip.ReshowDelay);
        Assert.False(toolTip.ShowAlways);
        Assert.Null(toolTip.Site);
        Assert.False(toolTip.StripAmpersands);
        Assert.Null(toolTip.Tag);
        Assert.Equal(ToolTipIcon.None, toolTip.ToolTipIcon);
        Assert.Empty(toolTip.ToolTipTitle);
        Assert.True(toolTip.UseAnimation);
        Assert.True(toolTip.UseFading);
        Assert.False(toolTip.GetHandleCreated());
    }

    [WinFormsFact]
    public void Ctor_IContainer_TestData()
    {
        using Container container = new();
        using SubToolTip toolTip = new(container);
        Assert.True(toolTip.Active);
        Assert.Equal(500, toolTip.AutomaticDelay);
        Assert.Equal(5000, toolTip.AutoPopDelay);
        Assert.Equal(SystemColors.Info, toolTip.BackColor);
        Assert.True(toolTip.CanRaiseEvents);
        Assert.Same(container, toolTip.Container);
        Assert.False(toolTip.DesignMode);
        Assert.NotNull(toolTip.Events);
        Assert.Same(toolTip.Events, toolTip.Events);
        Assert.Equal(SystemColors.InfoText, toolTip.ForeColor);
        Assert.Equal(500, toolTip.InitialDelay);
        Assert.False(toolTip.IsBalloon);
        Assert.False(toolTip.OwnerDraw);
        Assert.Equal(100, toolTip.ReshowDelay);
        Assert.False(toolTip.ShowAlways);
        Assert.NotNull(toolTip.Site);
        Assert.False(toolTip.StripAmpersands);
        Assert.Null(toolTip.Tag);
        Assert.Equal(ToolTipIcon.None, toolTip.ToolTipIcon);
        Assert.Empty(toolTip.ToolTipTitle);
        Assert.True(toolTip.UseAnimation);
        Assert.True(toolTip.UseFading);
        Assert.False(toolTip.GetHandleCreated());
    }

    [WinFormsFact]
    public void ToolTip_Ctor_NullCont_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("cont", () => new ToolTip(null));
    }

    [WinFormsFact]
    public void ToolTip_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubToolTip toolTip = new();
        CreateParams createParams = toolTip.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Equal("tooltips_class32", createParams.ClassName);
        Assert.Equal(0, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(0, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x2, createParams.Style);
        Assert.Equal(0, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.NotSame(createParams, toolTip.CreateParams);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolTip_Active_Set_GetReturnsExpected(bool value)
    {
        using ToolTip toolTip = new()
        {
            Active = value
        };
        Assert.Equal(value, toolTip.Active);

        // Set same
        toolTip.Active = value;
        Assert.Equal(value, toolTip.Active);

        // Set different
        toolTip.Active = !value;
        Assert.Equal(!value, toolTip.Active);
        Assert.False(toolTip.GetHandleCreated());
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolTip_Active_SetDesignMode_GetReturnsExpected(bool value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        using ToolTip toolTip = new()
        {
            Site = mockSite.Object,
            Active = value
        };
        Assert.Equal(value, toolTip.Active);

        // Set same
        toolTip.Active = value;
        Assert.Equal(value, toolTip.Active);

        // Set different
        toolTip.Active = !value;
        Assert.Equal(!value, toolTip.Active);
        Assert.False(toolTip.GetHandleCreated());

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        toolTip.Site = null;
    }

    [WinFormsTheory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 10, 0)]
    [InlineData(2, 20, 0)]
    [InlineData(100, 1000, 20)]
    [InlineData(500, 5000, 100)]
    [InlineData(5000, 50000, 1000)]
    public void ToolTip_AutomaticDelay_Set_GetReturnsExpected(int value, int expectedAutoPopDelay, int expectedReshowDelay)
    {
        using ToolTip toolTip = new()
        {
            InitialDelay = 80,
            AutoPopDelay = 70,
            ReshowDelay = 60,
            AutomaticDelay = value
        };
        Assert.Equal(value, toolTip.AutomaticDelay);
        Assert.Equal(expectedAutoPopDelay, toolTip.AutoPopDelay);
        Assert.Equal(value, toolTip.InitialDelay);
        Assert.Equal(expectedReshowDelay, toolTip.ReshowDelay);

        // Set same
        toolTip.InitialDelay = 80;
        toolTip.AutoPopDelay = 70;
        toolTip.ReshowDelay = 60;
        toolTip.AutomaticDelay = value;
        Assert.Equal(value, toolTip.AutomaticDelay);
        Assert.Equal(expectedAutoPopDelay, toolTip.AutoPopDelay);
        Assert.Equal(value, toolTip.InitialDelay);
        Assert.Equal(expectedReshowDelay, toolTip.ReshowDelay);
    }

    [WinFormsFact]
    public void ToolTip_AutomaticDelay_ShouldSerialize_ReturnsExpected()
    {
        using ToolTip toolTip = new();

        var properties = TypeDescriptor.GetProperties(typeof(ToolTip));
        PropertyDescriptor automaticProperty = properties[nameof(ToolTip.AutomaticDelay)];
        PropertyDescriptor initialProperty = properties[nameof(ToolTip.InitialDelay)];
        PropertyDescriptor reshowProperty = properties[nameof(ToolTip.ReshowDelay)];
        PropertyDescriptor autoPopProperty = properties[nameof(ToolTip.AutoPopDelay)];

        // No delays were set, thus we have nothing to serialize.
        Assert.False(automaticProperty.ShouldSerializeValue(toolTip));
        Assert.False(initialProperty.ShouldSerializeValue(toolTip));
        Assert.False(reshowProperty.ShouldSerializeValue(toolTip));
        Assert.False(autoPopProperty.ShouldSerializeValue(toolTip));

        toolTip.AutomaticDelay = toolTip.AutomaticDelay;

        // No delays were were changed compared to the defaultvalues, thus we have nothing to serialize.
        Assert.False(automaticProperty.ShouldSerializeValue(toolTip));
        Assert.False(initialProperty.ShouldSerializeValue(toolTip));
        Assert.False(reshowProperty.ShouldSerializeValue(toolTip));
        Assert.False(autoPopProperty.ShouldSerializeValue(toolTip));

        toolTip.AutomaticDelay = 0;

        // Automatic delay will be used to calculate all other delays, thus it is the only one to serialize.
        Assert.True(automaticProperty.ShouldSerializeValue(toolTip));
        Assert.False(initialProperty.ShouldSerializeValue(toolTip));
        Assert.False(reshowProperty.ShouldSerializeValue(toolTip));
        Assert.False(autoPopProperty.ShouldSerializeValue(toolTip));

        toolTip.InitialDelay = 10;

        // Serializing all delays because we ignore automatic delay at least in a single case.
        Assert.True(automaticProperty.ShouldSerializeValue(toolTip));
        Assert.True(initialProperty.ShouldSerializeValue(toolTip));
        Assert.True(reshowProperty.ShouldSerializeValue(toolTip));
        Assert.True(autoPopProperty.ShouldSerializeValue(toolTip));

        Assert.False(toolTip.GetHandleCreated());
    }

    [WinFormsFact]
    public void ToolTip_AutomaticDelay_SetNegativeValue_ThrowsArgumentOutOfRangeException()
    {
        using ToolTip toolTip = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => toolTip.AutomaticDelay = -1);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(5000)]
    public void ToolTip_AutoPopDelay_Set_GetReturnsExpected(int value)
    {
        using ToolTip toolTip = new()
        {
            AutoPopDelay = value
        };
        Assert.Equal(500, toolTip.AutomaticDelay);
        Assert.Equal(value, toolTip.AutoPopDelay);
        Assert.Equal(500, toolTip.InitialDelay);
        Assert.Equal(100, toolTip.ReshowDelay);

        // Set same
        toolTip.AutoPopDelay = value;
        Assert.Equal(500, toolTip.AutomaticDelay);
        Assert.Equal(value, toolTip.AutoPopDelay);
        Assert.Equal(500, toolTip.InitialDelay);
        Assert.Equal(100, toolTip.ReshowDelay);
    }

    [WinFormsFact]
    public void ToolTip_AutoPopDelay_ShouldSerialize_ReturnsExpected()
    {
        using ToolTip toolTip = new();
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolTip))[nameof(ToolTip.AutoPopDelay)];
        Assert.False(property.ShouldSerializeValue(toolTip));

        toolTip.AutoPopDelay = toolTip.AutoPopDelay;
        Assert.True(property.ShouldSerializeValue(toolTip));

        toolTip.AutoPopDelay = 0;
        Assert.True(property.ShouldSerializeValue(toolTip));
    }

    [WinFormsFact]
    public void ToolTip_AutoPopDelay_SetNegativeValue_ThrowsArgumentOutOfRangeException()
    {
        using ToolTip toolTip = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => toolTip.AutoPopDelay = -1);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
    public void ToolTip_BackColor_Set_GetReturnsExpected(Color value)
    {
        using ToolTip toolTip = new()
        {
            BackColor = value
        };
        Assert.Equal(value, toolTip.BackColor);

        // Set same.
        toolTip.BackColor = value;
        Assert.Equal(value, toolTip.BackColor);
    }

    [WinFormsFact]
    public void ToolTip_IsPersistent_Get_ReturnsExpected()
    {
        bool persistentToolTipSupported = OsVersion.IsWindows11_OrGreater();

        using ToolTip toolTip = new();
        Assert.Equal(persistentToolTipSupported, toolTip.IsPersistent);

        _ = toolTip.Handle;

        toolTip.AutomaticDelay = toolTip.AutomaticDelay;
        Assert.Equal(persistentToolTipSupported, toolTip.IsPersistent);

        toolTip.AutoPopDelay = 0x7FFF;
        Assert.Equal(persistentToolTipSupported, toolTip.IsPersistent);

        toolTip.AutoPopDelay = toolTip.AutoPopDelay;
        Assert.Equal(persistentToolTipSupported, toolTip.IsPersistent);

        toolTip.ReshowDelay = 30;
        Assert.Equal(persistentToolTipSupported, toolTip.IsPersistent);
    }

    [WinFormsFact]
    public void ToolTip_IsPersistent_Get_ReturnsExpected_AutoPopChanged()
    {
        bool persistentToolTipSupported = OsVersion.IsWindows11_OrGreater();

        using ToolTip toolTip = new();
        // IsPersistent is not set until the tooltip window is created.
        toolTip.AutoPopDelay = 30;
        Assert.Equal(persistentToolTipSupported, toolTip.IsPersistent);

        _ = toolTip.Handle;
        Assert.False(toolTip.IsPersistent);

        // We can not make tooltip persistent again programmatically.
        toolTip.AutomaticDelay = toolTip.AutomaticDelay;
        Assert.False(toolTip.IsPersistent);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetColorTheoryData))]
    public void ToolTip_ForeColor_Set_GetReturnsExpected(Color value)
    {
        using ToolTip toolTip = new()
        {
            ForeColor = value
        };
        Assert.Equal(value, toolTip.ForeColor);

        // Set same.
        toolTip.ForeColor = value;
        Assert.Equal(value, toolTip.ForeColor);
    }

    [WinFormsFact]
    public void ToolTip_ForeColor_SetEmpty_ThrowsArgumentException()
    {
        using ToolTip toolTip = new();
        Assert.Throws<ArgumentException>("value", () => toolTip.ForeColor = Color.Empty);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(5000)]
    public void ToolTip_InitialDelay_Set_GetReturnsExpected(int value)
    {
        using ToolTip toolTip = new()
        {
            InitialDelay = value
        };
        Assert.Equal(500, toolTip.AutomaticDelay);
        Assert.Equal(5000, toolTip.AutoPopDelay);
        Assert.Equal(value, toolTip.InitialDelay);
        Assert.Equal(100, toolTip.ReshowDelay);

        // Set same
        toolTip.InitialDelay = value;
        Assert.Equal(500, toolTip.AutomaticDelay);
        Assert.Equal(5000, toolTip.AutoPopDelay);
        Assert.Equal(value, toolTip.InitialDelay);
        Assert.Equal(100, toolTip.ReshowDelay);
    }

    [WinFormsFact]
    public void ToolTip_InitialDelay_ShouldSerialize_ReturnsExpected()
    {
        using ToolTip toolTip = new();
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolTip))[nameof(ToolTip.InitialDelay)];
        Assert.False(property.ShouldSerializeValue(toolTip));

        toolTip.InitialDelay = toolTip.InitialDelay;
        Assert.True(property.ShouldSerializeValue(toolTip));

        toolTip.InitialDelay = 0;
        Assert.True(property.ShouldSerializeValue(toolTip));
    }

    [WinFormsFact]
    public void ToolTip_InitialDelay_SetNegativeValue_ThrowsArgumentOutOfRangeException()
    {
        using ToolTip toolTip = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => toolTip.InitialDelay = -1);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolTip_IsBalloon_Set_GetReturnsExpected(bool value)
    {
        using ToolTip toolTip = new()
        {
            IsBalloon = value
        };
        Assert.Equal(value, toolTip.IsBalloon);

        // Set same
        toolTip.IsBalloon = value;
        Assert.Equal(value, toolTip.IsBalloon);

        // Set different
        toolTip.IsBalloon = !value;
        Assert.Equal(!value, toolTip.IsBalloon);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolTip_OwnerDraw_Set_GetReturnsExpected(bool value)
    {
        using ToolTip toolTip = new()
        {
            OwnerDraw = value
        };
        Assert.Equal(value, toolTip.OwnerDraw);

        // Set same
        toolTip.OwnerDraw = value;
        Assert.Equal(value, toolTip.OwnerDraw);

        // Set different
        toolTip.OwnerDraw = !value;
        Assert.Equal(!value, toolTip.OwnerDraw);
    }

    [WinFormsTheory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(100)]
    [InlineData(500)]
    [InlineData(5000)]
    public void ToolTip_ReshowDelay_Set_GetReturnsExpected(int value)
    {
        using ToolTip toolTip = new()
        {
            ReshowDelay = value
        };
        Assert.Equal(500, toolTip.AutomaticDelay);
        Assert.Equal(5000, toolTip.AutoPopDelay);
        Assert.Equal(500, toolTip.InitialDelay);
        Assert.Equal(value, toolTip.ReshowDelay);

        // Set same
        toolTip.ReshowDelay = value;
        Assert.Equal(500, toolTip.AutomaticDelay);
        Assert.Equal(5000, toolTip.AutoPopDelay);
        Assert.Equal(500, toolTip.InitialDelay);
        Assert.Equal(value, toolTip.ReshowDelay);
    }

    [WinFormsFact]
    public void ToolTip_ReshowDelay_ShouldSerialize_ReturnsExpected()
    {
        using ToolTip toolTip = new();
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolTip))[nameof(ToolTip.ReshowDelay)];
        Assert.False(property.ShouldSerializeValue(toolTip));

        toolTip.ReshowDelay = toolTip.ReshowDelay;
        Assert.True(property.ShouldSerializeValue(toolTip));

        toolTip.ReshowDelay = 0;
        Assert.True(property.ShouldSerializeValue(toolTip));
    }

    [WinFormsFact]
    public void ToolTip_ReshowDelay_SetNegativeValue_ThrowsArgumentOutOfRangeException()
    {
        using ToolTip toolTip = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => toolTip.ReshowDelay = -1);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolTip_ShowAlways_Set_GetReturnsExpected(bool value)
    {
        using ToolTip toolTip = new()
        {
            ShowAlways = value
        };
        Assert.Equal(value, toolTip.ShowAlways);

        // Set same
        toolTip.ShowAlways = value;
        Assert.Equal(value, toolTip.ShowAlways);

        // Set different
        toolTip.ShowAlways = !value;
        Assert.Equal(!value, toolTip.ShowAlways);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolTip_StripAmpersands_Set_GetReturnsExpected(bool value)
    {
        using ToolTip toolTip = new()
        {
            StripAmpersands = value
        };
        Assert.Equal(value, toolTip.StripAmpersands);

        // Set same
        toolTip.StripAmpersands = value;
        Assert.Equal(value, toolTip.StripAmpersands);

        // Set different
        toolTip.StripAmpersands = !value;
        Assert.Equal(!value, toolTip.StripAmpersands);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ToolTip_Tag_Set_GetReturnsExpected(object value)
    {
        using ToolTip toolTip = new()
        {
            Tag = value
        };
        Assert.Same(value, toolTip.Tag);

        // Set same
        toolTip.Tag = value;
        Assert.Same(value, toolTip.Tag);
    }

    [WinFormsTheory]
    [EnumData<ToolTipIcon>]
    public void ToolTip_ToolTipIcon_Set_GetReturnsExpected(ToolTipIcon value)
    {
        using ToolTip toolTip = new()
        {
            ToolTipIcon = value
        };
        Assert.Equal(value, toolTip.ToolTipIcon);

        // Set same
        toolTip.ToolTipIcon = value;
        Assert.Equal(value, toolTip.ToolTipIcon);
    }

    [WinFormsTheory]
    [InvalidEnumData<ToolTipIcon>]
    public void ToolTip_ToolTipIcon_SetInvalidValue_ThrowsInvalidEnumArgumentException(ToolTipIcon value)
    {
        using ToolTip toolTip = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => toolTip.ToolTipIcon = value);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolTip_ToolTipTitle_Set_GetReturnsExpected(string value, string expected)
    {
        using ToolTip toolTip = new()
        {
            ToolTipTitle = value
        };
        Assert.Equal(expected, toolTip.ToolTipTitle);

        // Set same
        toolTip.ToolTipTitle = value;
        Assert.Equal(expected, toolTip.ToolTipTitle);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolTip_UseAnimation_Set_GetReturnsExpected(bool value)
    {
        using ToolTip toolTip = new()
        {
            UseAnimation = value
        };
        Assert.Equal(value, toolTip.UseAnimation);

        // Set same
        toolTip.UseAnimation = value;
        Assert.Equal(value, toolTip.UseAnimation);

        // Set different
        toolTip.UseAnimation = !value;
        Assert.Equal(!value, toolTip.UseAnimation);
    }

    [WinFormsTheory]
    [BoolData]
    public void ToolTip_UseFading_Set_GetReturnsExpected(bool value)
    {
        using ToolTip toolTip = new()
        {
            UseFading = value
        };
        Assert.Equal(value, toolTip.UseFading);

        // Set same
        toolTip.UseFading = value;
        Assert.Equal(value, toolTip.UseFading);

        // Set different
        toolTip.UseFading = !value;
        Assert.Equal(!value, toolTip.UseFading);
    }

    public static IEnumerable<object[]> CanExtend_TestData()
    {
        yield return new object[] { null, false };
        yield return new object[] { new(), false };
        yield return new object[] { new ToolTip(), false };
        yield return new object[] { new Control(), true };
    }

    [WinFormsTheory]
    [MemberData(nameof(CanExtend_TestData))]
    public void ToolTip_CanExtend_Invoke_ReurnsExpected(object target, bool expected)
    {
        using ToolTip toolTip = new();
        Assert.Equal(expected, toolTip.CanExtend(target));
    }

    public static IEnumerable<object[]> GetToolTip_NoSuchControl_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Control() };
    }

    [WinFormsTheory]
    [MemberData(nameof(GetToolTip_NoSuchControl_TestData))]
    public void ToolTip_GetToolTip_NoSuchControl_ReturnsEmpty(Control control)
    {
        using ToolTip toolTip = new();
        Assert.Empty(toolTip.GetToolTip(control));
    }

    [WinFormsFact]
    public void ToolTip_RemoveAll_InvokeWithTools_GetToolTipReturnsEmpty()
    {
        using Control control = new();
        using ToolTip toolTip = new();
        toolTip.SetToolTip(control, "caption");
        toolTip.RemoveAll();
        Assert.Empty(toolTip.GetToolTip(control));

        toolTip.RemoveAll();
        Assert.Empty(toolTip.GetToolTip(control));
    }

    [WinFormsFact]
    public void ToolTip_RemoveAll_InvokeWithoutTools_Nop()
    {
        using ToolTip toolTip = new();
        toolTip.RemoveAll();
        toolTip.RemoveAll();
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolTip_SetToolTip_Invoke_GetToolTipReturnsExpected(string caption, string expected)
    {
        using ToolTip toolTip = new();
        using Control control = new();
        toolTip.SetToolTip(control, caption);
        Assert.Equal(expected, toolTip.GetToolTip(control));

        // Set same.
        toolTip.SetToolTip(control, caption);
        Assert.Equal(expected, toolTip.GetToolTip(control));
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ToolTip_SetToolTip_InvokeDesignMode_GetToolTipReturnsExpected(string caption, string expected)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.DesignMode)
            .Returns(true);
        using ToolTip toolTip = new()
        {
            Site = mockSite.Object
        };
        using Control control = new();
        toolTip.SetToolTip(control, caption);
        Assert.Equal(expected, toolTip.GetToolTip(control));

        // Set same.
        toolTip.SetToolTip(control, caption);
        Assert.Equal(expected, toolTip.GetToolTip(control));

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        toolTip.Site = null;
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ToolTip_SetToolTip_NullControl_ThrowsArgumentNullException(string caption)
    {
        using ToolTip toolTip = new();
        Assert.Throws<ArgumentNullException>("control", () => toolTip.SetToolTip(null, caption));
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ToolTip_Show_InvokeStringIWin32WindowControlWindow_Nop(string text)
    {
        using ToolTip toolTip = new();
        using Control control = new();
        toolTip.Show(text, control);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ToolTip_Show_InvokeStringIWin32WindowNonControlWindow_Nop(string text)
    {
        using ToolTip toolTip = new();
        var mockWindow = new Mock<IWin32Window>(MockBehavior.Strict);
        toolTip.Show(text, mockWindow.Object);
    }

    public static IEnumerable<object[]> Show_StringIWin32WindowInt_TestData()
    {
        foreach (int duration in new int[] { 0, 10 })
        {
            yield return new object[] { null, duration };
            yield return new object[] { string.Empty, duration };
            yield return new object[] { "text", duration };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(Show_StringIWin32WindowInt_TestData))]
    public void ToolTip_Show_InvokeStringIWin32WindowIntControlWindow_Nop(string text, int duration)
    {
        using ToolTip toolTip = new();
        using Control control = new();
        toolTip.Show(text, control, duration);
    }

    [WinFormsTheory]
    [MemberData(nameof(Show_StringIWin32WindowInt_TestData))]
    public void ToolTip_Show_InvokeStringIWin32WindowIntNonControlWindow_Nop(string text, int duration)
    {
        using ToolTip toolTip = new();
        var mockWindow = new Mock<IWin32Window>(MockBehavior.Strict);
        toolTip.Show(text, mockWindow.Object, duration);
    }

    [WinFormsFact]
    public void ToolTip_Show_NullWindow_ThrowsArgumentNullException()
    {
        using ToolTip toolTip = new();
        Assert.Throws<ArgumentNullException>("window", () => toolTip.Show("text", null));
        Assert.Throws<ArgumentNullException>("window", () => toolTip.Show("text", null, 1));
    }

    [WinFormsFact]
    public void ToolTip_Show_NegativeDuration_ThrowsArgumentOutOfRangeException()
    {
        using ToolTip toolTip = new();
        var mockWindow = new Mock<IWin32Window>(MockBehavior.Strict);
        Assert.Throws<ArgumentOutOfRangeException>("duration", () => toolTip.Show("text", mockWindow.Object, -1));
    }

    [WinFormsFact]
    public void ToolTip_ToString_Invoke_ReturnsExpected()
    {
        using ToolTip toolTip = new();
        Assert.Equal("System.Windows.Forms.ToolTip InitialDelay: 500, ShowAlways: False", toolTip.ToString());
    }

    [WinFormsFact]
    public void ToolTip_SetToolTipToControl_Invokes_SetToolTip_OfControl()
    {
        using ToolTip toolTip = new();
        SubControl control = new();
        control.CreateControl();

        Assert.NotEqual(IntPtr.Zero, toolTip.Handle); // A workaround to create the toolTip native window Handle

        toolTip.SetToolTip(control, "Some test text");

        Assert.Equal(1, control.InvokeSetCount);
    }

    [WinFormsFact]
    public void ToolTip_RemoveAll_Invokes_RemoveToolTip_OfControl()
    {
        using ToolTip toolTip = new();
        using SubControl control = new();

        // Create a top level control to the toolTip consider
        // the tested control as created when destroying regions
        using Control topLevelControl = new();
        topLevelControl.Controls.Add(control);
        topLevelControl.CreateControl();
        control.CreateControl();

        Assert.True(toolTip.Handle != IntPtr.Zero); // A workaround to create the toolTip native window Handle

        toolTip.SetToolTip(control, "Some test text");
        toolTip.RemoveAll();

        Assert.Equal(1, control.InvokeRemoveCount);
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/11236")]
    [WinFormsFact]
    public unsafe void ToolTip_WmShow_Invokes_AnnounceText_WithExpectedText_ForTabControlTabs()
    {
        using NoAssertContext context = new();
        Mock<TabControl> mockTabControl = new() { CallBase = true, Object = { ShowToolTips = true } };
        Mock<Control.ControlAccessibleObject> mockAccessibleObject = new(MockBehavior.Strict, mockTabControl.Object);
        mockAccessibleObject
            .Setup(a => a.InternalRaiseAutomationNotification(
                It.IsAny<AutomationNotificationKind>(),
                It.IsAny<AutomationNotificationProcessing>(),
                It.IsAny<string>()))
            .Returns(true);
        mockTabControl.Protected().Setup<AccessibleObject>("CreateAccessibilityInstance").Returns(mockAccessibleObject.Object);

        // We need a Form because tooltips don't work on controls without a valid parent.
        using Form form = new();
        using ToolTip toolTip = new();
        using TabControl tabControl = mockTabControl.Object;
        using TabPage tabPage = new() { ToolTipText = "TabPage" };

        toolTip.SetToolTip(tabControl, "TabControl");
        tabControl.Controls.Add(tabPage);
        form.Controls.Add(tabControl);
        form.Show();

        Assert.NotEqual(IntPtr.Zero, tabControl.InternalHandle);
        Assert.True(toolTip.GetHandleCreated());

        // Enforce AccessibilityObject creation.
        Assert.Equal(mockAccessibleObject.Object, tabControl.AccessibilityObject);

        // Post MOUSEMOVE to the tooltip queue and then just remove it from the queue without handling.
        // This will update the point returned by GetMessagePos which is used by PInvoke.TTM_POPUP to determine the tool to display.
        Assert.True(PInvokeCore.PostMessage(toolTip, PInvokeCore.WM_MOUSEMOVE, lParam: PARAM.FromPoint(tabPage.GetToolNativeScreenRectangle().Location)));
        MSG msg = default;
        Assert.True(PInvokeCore.PeekMessage(&msg, toolTip, PInvokeCore.WM_MOUSEMOVE, PInvokeCore.WM_MOUSEMOVE, PEEK_MESSAGE_REMOVE_TYPE.PM_REMOVE));

        // Show the tooltip.

        // Comment out the validation here due to the active issue "https://github.com/dotnet/winforms/issues/11236"
        // PInvokeCore.SendMessage(toolTip, PInvoke.TTM_POPUP);

        // mockAccessibleObject.Verify(a => a.InternalRaiseAutomationNotification(
        //     AutomationNotificationKind.ActionCompleted,
        //     AutomationNotificationProcessing.All,
        //     $" {tabPage.ToolTipText}"),
        //     Times.Once);
    }

    [ActiveIssue("https://github.com/dotnet/winforms/issues/11234")]
    [WinFormsFact]
    [SkipOnArchitecture(TestArchitectures.X64,
        "Flaky tests, see: https://github.com/dotnet/winforms/issues/11234")]
    public void ToolTip_SetToolTip_TabControl_DoesNotAddToolForTabControlItself()
    {
        // We need a Form because tooltips don't work on controls without a valid parent.
        using Form form = new();
        using ToolTip toolTip = new();
        using TabControl tabControl = new() { ShowToolTips = true };
        using TabPage tabPage1 = new();
        using TabPage tabPage2 = new();

        toolTip.SetToolTip(tabControl, "Test");
        tabControl.Controls.Add(tabPage1);
        tabControl.Controls.Add(tabPage2);
        form.Controls.Add(tabControl);
        form.Show();

        Assert.NotEqual(IntPtr.Zero, tabControl.InternalHandle);
        Assert.True(toolTip.GetHandleCreated());

        // Only tools for TabPages were added.
        Assert.Equal(tabControl.TabCount, (int)PInvokeCore.SendMessage(toolTip, PInvoke.TTM_GETTOOLCOUNT));
    }

    [WinFormsFact]
    public unsafe void ToolTip_TTTOOLINFOW_Struct_Size_IsExpected()
    {
        TTTOOLINFOW toolInfo = default;
        int size = (int)&toolInfo.lParam - (int)&toolInfo + sizeof(LPARAM);
        int expected = (int)default(ToolInfoWrapper<Control>).TestAccessor().Dynamic.TTTOOLINFO_V2_Size;
        size.Should().Be(expected);
    }

    private class SubToolTip : ToolTip
    {
        public SubToolTip() : base()
        {
        }

        public SubToolTip(IContainer cont) : base(cont)
        {
        }

        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new CreateParams CreateParams => base.CreateParams;

        public new bool DesignMode => base.DesignMode;

        public new EventHandlerList Events => base.Events;
    }

    private class SubControl : Control
    {
        public int InvokeSetCount { get; set; }
        public int InvokeRemoveCount { get; set; }

        internal override void SetToolTip(ToolTip toolTip)
        {
            InvokeSetCount++;
            base.SetToolTip(toolTip);
        }

        internal override void RemoveToolTip(ToolTip toolTip)
        {
            InvokeRemoveCount++;
            base.RemoveToolTip(toolTip);
        }
    }
}
