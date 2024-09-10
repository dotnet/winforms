// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Moq;

namespace System.Windows.Forms.Tests;

public class ErrorProviderTests
{
    [WinFormsFact]
    public void ErrorProvider_Ctor_Default()
    {
        using SubErrorProvider provider = new();
        Assert.Equal(250, provider.BlinkRate);
        Assert.Equal(ErrorBlinkStyle.BlinkIfDifferentError, provider.BlinkStyle);
        Assert.True(provider.CanRaiseEvents);
        Assert.Null(provider.Container);
        Assert.Null(provider.ContainerControl);
        Assert.Null(provider.DataMember);
        Assert.Null(provider.DataSource);
        Assert.False(provider.DesignMode);
        Assert.NotNull(provider.Events);
        Assert.Same(provider.Events, provider.Events);
        Assert.NotNull(provider.Icon);
        Assert.Same(provider.Icon, provider.Icon);
        Assert.Null(provider.Site);
        Assert.Null(provider.Tag);
        Assert.Equal(provider.Icon.Width, PInvokeCore.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXSMICON));
        Assert.Equal(provider.Icon.Height, PInvokeCore.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYSMICON));
    }

    [WinFormsFact]
    public void ErrorProvider_Ctor_ContainerControl()
    {
        using ContainerControl parentControl = new();
        using SubErrorProvider provider = new(parentControl);
        Assert.Equal(250, provider.BlinkRate);
        Assert.Equal(ErrorBlinkStyle.BlinkIfDifferentError, provider.BlinkStyle);
        Assert.True(provider.CanRaiseEvents);
        Assert.Null(provider.Container);
        Assert.Same(parentControl, provider.ContainerControl);
        Assert.Null(provider.DataMember);
        Assert.Null(provider.DataSource);
        Assert.False(provider.DesignMode);
        Assert.NotNull(provider.Events);
        Assert.Same(provider.Events, provider.Events);
        Assert.NotNull(provider.Icon);
        Assert.Same(provider.Icon, provider.Icon);
        Assert.Null(provider.Site);
        Assert.Null(provider.Tag);
    }

    [WinFormsFact]
    public void ErrorProvider_Ctor_NullParentControl_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("parentControl", () => new ErrorProvider((ContainerControl)null));
    }

    [WinFormsFact]
    public void ErrorProvider_Ctor_IContainer()
    {
        using Container container = new();
        using SubErrorProvider provider = new(container);
        Assert.Equal(250, provider.BlinkRate);
        Assert.Equal(ErrorBlinkStyle.BlinkIfDifferentError, provider.BlinkStyle);
        Assert.True(provider.CanRaiseEvents);
        Assert.Same(container, provider.Container);
        Assert.Null(provider.ContainerControl);
        Assert.Null(provider.DataMember);
        Assert.Null(provider.DataSource);
        Assert.False(provider.DesignMode);
        Assert.NotNull(provider.Events);
        Assert.Same(provider.Events, provider.Events);
        Assert.NotNull(provider.Icon);
        Assert.Same(provider.Icon, provider.Icon);
        Assert.NotNull(provider.Site);
        Assert.Null(provider.Tag);
    }

    [WinFormsFact]
    public void ErrorProvider_Ctor_NullContainer_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>("container", () => new ErrorProvider((IContainer)null));
    }

    [WinFormsTheory]
    [InlineData(0, ErrorBlinkStyle.NeverBlink)]
    [InlineData(1, ErrorBlinkStyle.BlinkIfDifferentError)]
    [InlineData(250, ErrorBlinkStyle.BlinkIfDifferentError)]
    public void ErrorProvider_BlinkRate_Set_GetReturnsExpected(int value, ErrorBlinkStyle expectedBlinkStyle)
    {
        using ErrorProvider provider = new()
        {
            BlinkRate = value
        };
        Assert.Equal(value, provider.BlinkRate);
        Assert.Equal(expectedBlinkStyle, provider.BlinkStyle);

        // Set same.
        provider.BlinkRate = value;
        Assert.Equal(value, provider.BlinkRate);
        Assert.Equal(expectedBlinkStyle, provider.BlinkStyle);

        // Set blink style.
        provider.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
        Assert.Equal(value, provider.BlinkRate);
        Assert.Equal(expectedBlinkStyle, provider.BlinkStyle);
    }

    [WinFormsFact]
    public void ErrorProvider_BlinkRate_SetNegative_ThrowsArgumentOutOfRangeException()
    {
        using ErrorProvider provider = new();
        Assert.Throws<ArgumentOutOfRangeException>("value", () => provider.BlinkRate = -1);
    }

    [WinFormsTheory]
    [EnumData<ErrorBlinkStyle>]
    public void ErrorProvider_BlinkStyle_Set_GetReturnsExpected(ErrorBlinkStyle value)
    {
        using ErrorProvider provider = new()
        {
            BlinkStyle = value
        };
        Assert.Equal(value, provider.BlinkStyle);
        Assert.Equal(250, provider.BlinkRate);

        // Set same.
        provider.BlinkStyle = value;
        Assert.Equal(value, provider.BlinkStyle);
        Assert.Equal(250, provider.BlinkRate);
    }

    [WinFormsTheory]
    [EnumData<ErrorBlinkStyle>]
    public void ErrorProvider_BlinkStyle_SetAlreadyBlink_GetReturnsExpected(ErrorBlinkStyle value)
    {
        using ErrorProvider provider = new()
        {
            BlinkStyle = ErrorBlinkStyle.AlwaysBlink
        };

        provider.BlinkStyle = value;
        Assert.Equal(value, provider.BlinkStyle);
        Assert.Equal(250, provider.BlinkRate);

        // Set same.
        provider.BlinkStyle = value;
        Assert.Equal(value, provider.BlinkStyle);
        Assert.Equal(250, provider.BlinkRate);
    }

    [WinFormsTheory]
    [EnumData<ErrorBlinkStyle>]
    public void ErrorProvider_BlinkStyle_SetWithZeroBlinkRate_GetReturnsExpected(ErrorBlinkStyle value)
    {
        using ErrorProvider provider = new()
        {
            BlinkRate = 0,
            BlinkStyle = value
        };
        Assert.Equal(ErrorBlinkStyle.NeverBlink, provider.BlinkStyle);
        Assert.Equal(0, provider.BlinkRate);

        // Set same.
        provider.BlinkStyle = value;
        Assert.Equal(ErrorBlinkStyle.NeverBlink, provider.BlinkStyle);
        Assert.Equal(0, provider.BlinkRate);
    }

    [WinFormsTheory]
    [InvalidEnumData<ErrorBlinkStyle>]
    public void ErrorProvider_BlinkStyle_SetInvalidValue_ThrowsInvalidEnumArgumentException(ErrorBlinkStyle value)
    {
        using ErrorProvider provider = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => provider.BlinkStyle = value);
    }

    public static IEnumerable<object[]> ContainerControl_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new ContainerControl() };
        yield return new object[] { new SubContainerControl() };
    }

    [WinFormsTheory]
    [MemberData(nameof(ContainerControl_TestData))]
    public void ErrorProvider_ContainerControl_Set_GetReturnsExpected(ContainerControl value)
    {
        using ErrorProvider provider = new()
        {
            ContainerControl = value
        };
        Assert.Same(value, provider.ContainerControl);

        // Set same.
        provider.ContainerControl = value;
        Assert.Same(value, provider.ContainerControl);
    }

    [WinFormsTheory]
    [MemberData(nameof(ContainerControl_TestData))]
    public void ErrorProvider_ContainerControl_SetWithNonNullOldValue_GetReturnsExpected(ContainerControl value)
    {
        using ErrorProvider provider = new()
        {
            ContainerControl = new ContainerControl()
        };

        provider.ContainerControl = value;
        Assert.Same(value, provider.ContainerControl);

        // Set same.
        provider.ContainerControl = value;
        Assert.Same(value, provider.ContainerControl);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ErrorProvider_DataMember_Set_GetReturnsExpected(string value, string expected)
    {
        using ErrorProvider provider = new()
        {
            DataMember = value
        };
        Assert.Equal(expected, provider.DataMember);

        // Set same.
        provider.DataMember = value;
        Assert.Equal(expected, provider.DataMember);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public void ErrorProvider_DataMember_SetWithNonNullOldValue_GetReturnsExpected(string value, string expected)
    {
        using ErrorProvider provider = new()
        {
            DataMember = "OldMember"
        };

        provider.DataMember = value;
        Assert.Equal(expected, provider.DataMember);

        // Set same.
        provider.DataMember = value;
        Assert.Equal(expected, provider.DataMember);
    }

    public static IEnumerable<object[]> DataMember_SetWithContainerControl_TestData()
    {
        foreach (string value in new string[] { null, string.Empty, "dataMember" })
        {
            yield return new object[] { null, value, value ?? string.Empty };
            yield return new object[] { new ContainerControl(), value, value ?? string.Empty };
            yield return new object[] { new SubContainerControl(), value, value ?? string.Empty };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DataMember_SetWithContainerControl_TestData))]
    public void ErrorProvider_DataMember_SetWithContainerControl_GetReturnsExpected(ContainerControl containerControl, string value, string expected)
    {
        using ErrorProvider provider = new()
        {
            ContainerControl = containerControl,
            DataMember = value
        };
        Assert.Equal(expected, provider.DataMember);

        // Set same.
        provider.DataMember = value;
        Assert.Equal(expected, provider.DataMember);
    }

    public static IEnumerable<object[]> DataMember_SetWithValidDataMemberWithContainerControl_TestData()
    {
        foreach (string dataMember in new string[] { nameof(DataClass.Value), nameof(DataClass.ListValue) })
        {
            yield return new object[] { null, dataMember };
            yield return new object[] { new ContainerControl(), dataMember };
            yield return new object[] { new SubContainerControl(), dataMember };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DataMember_SetWithValidDataMemberWithContainerControl_TestData))]
    public void ErrorProvider_DataMember_SetWithValidDataSourceWithContainerControl_ReturnsExpected(ContainerControl containerControl, string dataMember)
    {
        DataClass value = new();
        using ErrorProvider provider = new()
        {
            ContainerControl = containerControl,
            DataSource = value,
            DataMember = dataMember
        };
        Assert.Same(value, provider.DataSource);
        Assert.Same(dataMember, provider.DataMember);
    }

    [WinFormsFact]
    public void ErrorProvider_DataMember_ShouldSerializeValue_ReturnsExpected()
    {
        using ErrorProvider provider = new();
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ErrorProvider))[nameof(ErrorProvider.DataMember)];
        Assert.False(property.ShouldSerializeValue(provider));

        provider.DataMember = "dataMember";
        Assert.True(property.ShouldSerializeValue(provider));
    }

    [WinFormsFact]
    public void ErrorProvider_DataMember_CanResetValue_ReturnsExpected()
    {
        using ErrorProvider provider = new();
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ErrorProvider))[nameof(ErrorProvider.DataMember)];
        Assert.False(property.CanResetValue(provider));

        provider.DataMember = "dataMember";
        Assert.True(property.CanResetValue(provider));

        property.ResetValue(provider);
        Assert.True(property.CanResetValue(provider));
        Assert.Empty(provider.DataMember);
    }

    public static IEnumerable<object[]> NoBindingContextContainerControl_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new SubContainerControl() };
    }

    [WinFormsTheory]
    [MemberData(nameof(NoBindingContextContainerControl_TestData))]
    public void ErrorProvider_DataMember_SetWithInvalidDataSourceWithContainerControl_ReturnsExpected(ContainerControl containerControl)
    {
        DataClass value = new();
        using ErrorProvider provider = new()
        {
            ContainerControl = containerControl,
            DataSource = value,
            DataMember = "NoSuchValue"
        };
        Assert.Same(value, provider.DataSource);
        Assert.Equal("NoSuchValue", provider.DataMember);
    }

    [WinFormsFact]
    public void ErrorProvider_DataMember_SetWithInvalidDataSourceWithContainerControl_ResetsDataMember()
    {
        using ContainerControl containerControl = new();
        DataClass value = new();
        using ErrorProvider provider = new()
        {
            ContainerControl = containerControl,
            DataSource = value
        };
        Assert.Throws<ArgumentException>(() => provider.DataMember = "NoSuchValue");
        Assert.Same(value, provider.DataSource);
        Assert.Equal("NoSuchValue", provider.DataMember);
    }

    public static IEnumerable<object[]> DataSource_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new() };
        yield return new object[] { new DataClass() };
    }

    [WinFormsTheory]
    [MemberData(nameof(DataSource_Set_TestData))]
    public void ErrorProvider_DataSource_SetWithNullDataMember_GetReturnsExpected(object value)
    {
        using ErrorProvider provider = new()
        {
            DataSource = value
        };
        Assert.Same(value, provider.DataSource);
        Assert.Null(provider.DataMember);

        // Set same.
        provider.DataSource = value;
        Assert.Same(value, provider.DataSource);
        Assert.Null(provider.DataMember);
    }

    [WinFormsTheory]
    [MemberData(nameof(DataSource_Set_TestData))]
    public void ErrorProvider_DataSource_SetWithEmptyDataMember_GetReturnsExpected(object value)
    {
        using ErrorProvider provider = new()
        {
            DataMember = string.Empty,
            DataSource = value
        };
        Assert.Same(value, provider.DataSource);
        Assert.Empty(provider.DataMember);

        // Set same.
        provider.DataSource = value;
        Assert.Same(value, provider.DataSource);
        Assert.Empty(provider.DataMember);
    }

    [WinFormsTheory]
    [MemberData(nameof(DataSource_Set_TestData))]
    public void ErrorProvider_DataSource_SetWithNonNullOldValue_GetReturnsExpected(object value)
    {
        using ErrorProvider provider = new()
        {
            DataSource = new object()
        };

        provider.DataSource = value;
        Assert.Same(value, provider.DataSource);

        // Set same.
        provider.DataSource = value;
        Assert.Same(value, provider.DataSource);
    }

    public static IEnumerable<object[]> DataSource_SetWithContainerControl_TestData()
    {
        foreach (object value in new object[] { null, new(), new DataClass() })
        {
            yield return new object[] { null, value };
            yield return new object[] { new ContainerControl(), value };
            yield return new object[] { new SubContainerControl(), value };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DataSource_SetWithContainerControl_TestData))]
    public void ErrorProvider_DataSource_SetWithContainerControl_GetReturnsExpected(ContainerControl containerControl, object value)
    {
        using ErrorProvider provider = new()
        {
            ContainerControl = containerControl,
            DataSource = value
        };
        Assert.Same(value, provider.DataSource);

        // Set same.
        provider.DataSource = value;
        Assert.Same(value, provider.DataSource);
    }

    public static IEnumerable<object[]> DataSource_SetWithValidDataMemberWithContainerControl_TestData()
    {
        foreach (string dataMember in new string[] { nameof(DataClass.Value), nameof(DataClass.ListValue) })
        {
            yield return new object[] { null, dataMember };
            yield return new object[] { new ContainerControl(), dataMember };
            yield return new object[] { new SubContainerControl(), dataMember };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(DataSource_SetWithValidDataMemberWithContainerControl_TestData))]
    public void ErrorProvider_DataSource_SetWithValidDataMemberWithContainerControl_ReturnsExpected(ContainerControl containerControl, string dataMember)
    {
        DataClass value = new();
        using ErrorProvider provider = new()
        {
            ContainerControl = containerControl,
            DataMember = dataMember,
            DataSource = value
        };
        Assert.Same(value, provider.DataSource);
        Assert.Same(dataMember, provider.DataMember);
    }

    [WinFormsTheory]
    [MemberData(nameof(NoBindingContextContainerControl_TestData))]
    public void ErrorProvider_DataSource_SetWithInvalidDataMemberWithContainerControl_ReturnsExpected(ContainerControl containerControl)
    {
        DataClass value = new();
        using ErrorProvider provider = new()
        {
            ContainerControl = containerControl,
            DataMember = "NoSuchValue",
            DataSource = value
        };
        Assert.Same(value, provider.DataSource);
        Assert.Equal("NoSuchValue", provider.DataMember);
    }

    [WinFormsFact]
    public void ErrorProvider_DataSource_SetWithInvalidDataMemberWithContainerControl_ResetsDataMember()
    {
        using ContainerControl containerControl = new();
        DataClass value = new();
        using ErrorProvider provider = new()
        {
            ContainerControl = containerControl,
            DataMember = "NoSuchValue",
            DataSource = value
        };
        Assert.Same(value, provider.DataSource);
        Assert.Empty(provider.DataMember);
    }

    [WinFormsFact]
    public void ErrorProvider_DataSource_ShouldSerializeValue_ReturnsExpected()
    {
        using ErrorProvider provider = new();
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ErrorProvider))[nameof(ErrorProvider.DataSource)];
        Assert.False(property.ShouldSerializeValue(provider));

        provider.DataSource = new object();
        Assert.True(property.ShouldSerializeValue(provider));
    }

    [WinFormsFact]
    public void ErrorProvider_DataSource_CanResetValue_ReturnsExpected()
    {
        using ErrorProvider provider = new();
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ErrorProvider))[nameof(ErrorProvider.DataSource)];
        Assert.False(property.CanResetValue(provider));

        provider.DataSource = new object();
        Assert.True(property.CanResetValue(provider));

        property.ResetValue(provider);
        Assert.False(property.CanResetValue(provider));
        Assert.Null(provider.DataSource);
    }

    public static IEnumerable<object[]> Icon_Set_TestData()
    {
        // TODO: we're leaking a handle here, see TaskDialogIcon.BitmapToIcon for correct implementation
        yield return new object[] { Icon.FromHandle(new Bitmap(10, 10).GetHicon()) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Icon_Set_TestData))]
    public void ErrorProvider_Icon_Set_GetReturnsExpected(Icon value)
    {
        using ErrorProvider provider = new()
        {
            Icon = value
        };
        Assert.Same(value, provider.Icon);

        // Set same.
        provider.Icon = value;
        Assert.Same(value, provider.Icon);
    }

    [WinFormsFact]
    public void ErrorProvider_Icon_ShouldSerializeValue_ReturnsExpected()
    {
        using ErrorProvider provider = new();
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ErrorProvider))[nameof(ErrorProvider.Icon)];
        Assert.False(property.ShouldSerializeValue(provider));

        // TODO: we're leaking a handle here, see TaskDialogIcon.BitmapToIcon for correct implementation
        provider.Icon = Icon.FromHandle(new Bitmap(10, 10).GetHicon());
        Assert.True(property.ShouldSerializeValue(provider));
    }

    [WinFormsFact]
    public void ErrorProvider_Icon_CanResetValue_ReturnsExpected()
    {
        using ErrorProvider provider = new();
        PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ErrorProvider))[nameof(ErrorProvider.Icon)];
        Assert.False(property.CanResetValue(provider));

        // TODO: we're leaking a handle here, see TaskDialogIcon.BitmapToIcon for correct implementation
        provider.Icon = Icon.FromHandle(new Bitmap(10, 10).GetHicon());
        Assert.True(property.CanResetValue(provider));

        property.ResetValue(provider);
        Assert.False(property.CanResetValue(provider));
        Assert.Null(provider.DataSource);
    }

    [WinFormsFact]
    public void ErrorProvider_Icon_Null_ThrowsArgumentNullException()
    {
        using ErrorProvider provider = new();
        Assert.Throws<ArgumentNullException>("value", () => provider.Icon = null);
    }

    [WinFormsTheory]
    [BoolData]
    public void ErrorProvider_RightToLeft_Set_GetReturnsExpected(bool value)
    {
        using ErrorProvider provider = new()
        {
            RightToLeft = value
        };
        Assert.Equal(value, provider.RightToLeft);

        // Set same.
        provider.RightToLeft = value;
        Assert.Equal(value, provider.RightToLeft);

        // Set different.
        provider.RightToLeft = !value;
        Assert.Equal(!value, provider.RightToLeft);
    }

    [WinFormsFact]
    public void ErrorProvider_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
    {
        using ErrorProvider provider = new()
        {
            RightToLeft = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(provider, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        provider.RightToLeftChanged += handler;

        // Set different.
        provider.RightToLeft = false;
        Assert.False(provider.RightToLeft);
        Assert.Equal(1, callCount);

        // Set same.
        provider.RightToLeft = false;
        Assert.False(provider.RightToLeft);
        Assert.Equal(1, callCount);

        // Set different.
        provider.RightToLeft = true;
        Assert.True(provider.RightToLeft);
        Assert.Equal(2, callCount);

        // Remove handler.
        provider.RightToLeftChanged -= handler;
        provider.RightToLeft = false;
        Assert.False(provider.RightToLeft);
        Assert.Equal(2, callCount);
    }

    public static IEnumerable<object[]> Site_Set_TestData()
    {
        yield return new object[] { null };

        Mock<ISite> mockNullHostSite = new(MockBehavior.Strict);
        mockNullHostSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(null);
        yield return new object[] { mockNullHostSite.Object };

        Mock<ISite> mockInvalidHostSite = new(MockBehavior.Strict);
        mockInvalidHostSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(new object());
        yield return new object[] { mockInvalidHostSite.Object };
    }

    [WinFormsTheory]
    [MemberData(nameof(Site_Set_TestData))]
    public void ErrorProvider_Site_Set_GetReturnsExpected(ISite value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(null);
        using ErrorProvider provider = new()
        {
            Site = value
        };
        Assert.Same(value, provider.Site);

        // Set same.
        provider.Site = value;
        Assert.Same(value, provider.Site);

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        provider.Site = null;
    }

    [WinFormsTheory]
    [MemberData(nameof(Site_Set_TestData))]
    public void ErrorProvider_Site_SetWithNonNullOldValue_GetReturnsExpected(ISite value)
    {
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(null);
        using ErrorProvider provider = new()
        {
            Site = mockSite.Object
        };

        provider.Site = value;
        Assert.Same(value, provider.Site);

        // Set same.
        provider.Site = value;
        Assert.Same(value, provider.Site);

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        provider.Site = null;
    }

    public static IEnumerable<object[]> Site_SetWithIDesignerHost_TestData()
    {
        yield return new object[] { null, null };
        yield return new object[] { new Component(), null };

        ContainerControl containerControl = new();
        yield return new object[] { containerControl, containerControl };
    }

    [WinFormsTheory]
    [MemberData(nameof(Site_SetWithIDesignerHost_TestData))]
    public void ErrorProvider_Site_SetWithIDesignerHost_SetsContainerControl(IComponent rootComponent, ContainerControl expected)
    {
        Mock<IDesignerHost> mockDesignerHost = new(MockBehavior.Strict);
        mockDesignerHost
            .Setup(h => h.RootComponent)
            .Returns(rootComponent);
        Mock<ISite> mockSite = new(MockBehavior.Strict);
        mockSite
            .Setup(s => s.GetService(typeof(IDesignerHost)))
            .Returns(mockDesignerHost.Object)
            .Verifiable();
        using ErrorProvider provider = new()
        {
            Site = mockSite.Object
        };
        mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
        mockDesignerHost.Verify(h => h.RootComponent, Times.Once());
        Assert.Same(mockSite.Object, provider.Site);
        Assert.Same(expected, provider.ContainerControl);
        mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
        mockDesignerHost.Verify(h => h.RootComponent, Times.Once());

        // NB: disposing the component with strictly mocked object causes tests to fail
        // Moq.MockException : ISite.Container invocation failed with mock behavior Strict.
        // All invocations on the mock must have a corresponding setup.
        provider.Site = null;
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ErrorProvider_Tag_Set_GetReturnsExpected(object value)
    {
        using ErrorProvider provider = new()
        {
            Tag = value
        };
        Assert.Same(value, provider.Tag);

        // Set same.
        provider.Tag = value;
        Assert.Same(value, provider.Tag);
    }

    public static IEnumerable<object[]> BindToDataAndErrors_TestData()
    {
        foreach (ContainerControl containerControl in new ContainerControl[] { null, new SubContainerControl() })
        {
            foreach (string dataMember in new string[] { null, string.Empty, "dataMember" })
            {
                yield return new object[] { containerControl, null, dataMember };
                yield return new object[] { containerControl, new(), dataMember };
                yield return new object[] { containerControl, new DataClass(), dataMember };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(BindToDataAndErrors_TestData))]
    public void BindToDataAndErrors_Invoke_SetsDataSourceAndDataMember(ContainerControl containerControl, object newDataSource, string newDataMember)
    {
        using ErrorProvider provider = new()
        {
            ContainerControl = containerControl
        };
        provider.BindToDataAndErrors(newDataSource, newDataMember);
        Assert.Same(newDataSource, provider.DataSource);
        Assert.Same(newDataMember, provider.DataMember);

        // Call again.
        provider.BindToDataAndErrors(newDataSource, newDataMember);
        Assert.Same(newDataSource, provider.DataSource);
        Assert.Same(newDataMember, provider.DataMember);
    }

    public static IEnumerable<object[]> BindToDataAndErrors_WithBindingContext_TestData()
    {
        foreach (string dataMember in new string[] { null, string.Empty })
        {
            yield return new object[] { null, dataMember };
            yield return new object[] { new(), dataMember };
            yield return new object[] { new DataClass(), dataMember };
        }

        yield return new object[] { null, nameof(DataClass.Value) };
        yield return new object[] { null, nameof(DataClass.ListValue) };
        yield return new object[] { new DataClass(), nameof(DataClass.Value) };
        yield return new object[] { new DataClass(), nameof(DataClass.ListValue) };
    }

    [WinFormsTheory]
    [MemberData(nameof(BindToDataAndErrors_WithBindingContext_TestData))]
    public void BindToDataAndErrors_InvokeValidDataMemberWithBindingContext_SetsDataSourceAndDataMember(object newDataSource, string newDataMember)
    {
        using ContainerControl containerControl = new();
        using ErrorProvider provider = new()
        {
            ContainerControl = containerControl
        };
        provider.BindToDataAndErrors(newDataSource, newDataMember);
        Assert.Same(newDataSource, provider.DataSource);
        Assert.Same(newDataMember, provider.DataMember);

        // Call again.
        provider.BindToDataAndErrors(newDataSource, newDataMember);
        Assert.Same(newDataSource, provider.DataSource);
        Assert.Same(newDataMember, provider.DataMember);
    }

    [WinFormsFact]
    public void BindToDataAndErrors_InvokeInvalidDataMemberWithBindingContext_ThrowsArgumentException()
    {
        using ContainerControl containerControl = new();
        using ErrorProvider provider = new()
        {
            ContainerControl = containerControl
        };
        DataClass newDataSource = new();
        Assert.Throws<ArgumentException>(() => provider.BindToDataAndErrors(newDataSource, "NoSuchValue"));
        Assert.Same(newDataSource, provider.DataSource);
        Assert.Equal("NoSuchValue", provider.DataMember);

        // Call again.
        provider.BindToDataAndErrors(newDataSource, "NoSuchValue");
        Assert.Same(newDataSource, provider.DataSource);
        Assert.Equal("NoSuchValue", provider.DataMember);
    }

    public static IEnumerable<object[]> CanExtend_TestData()
    {
        yield return new object[] { null, false };
        yield return new object[] { new(), false };
        yield return new object[] { new Component(), false };
        yield return new object[] { new Form(), false };
        yield return new object[] { new Control(), true };
    }

    [WinFormsTheory]
    [MemberData(nameof(CanExtend_TestData))]
    public void ErrorProvider_CanExtend_Invoke_ReturnsExpected(object extendee, bool expected)
    {
        using ErrorProvider provider = new();
        Assert.Equal(expected, provider.CanExtend(extendee));
    }

    [WinFormsFact]
    public void ErrorProvider_Clear_InvokeMultipleTimesWithItems_Success()
    {
        using ErrorProvider provider = new();
        using Control control = new();
        provider.SetError(control, "error");
        Assert.Equal("error", provider.GetError(control));

        provider.Clear();
        Assert.Empty(provider.GetError(control));

        provider.Clear();
        Assert.Empty(provider.GetError(control));
    }

    [WinFormsFact]
    public void ErrorProvider_Clear_InvokeMultipleTimesWithoutItems_Nop()
    {
        using ErrorProvider provider = new();
        provider.Clear();
        provider.Clear();
    }

    [WinFormsFact]
    public void ErrorProvider_Dispose_InvokeWithItems_Clears()
    {
        using ErrorProvider provider = new();
        using Control control = new();
        provider.SetError(control, "error");
        Assert.Equal("error", provider.GetError(control));

        provider.Dispose();
        Assert.NotNull(provider.Icon);
        Assert.Empty(provider.GetError(control));

        provider.Dispose();
        Assert.NotNull(provider.Icon);
        Assert.Empty(provider.GetError(control));
    }

    [WinFormsFact]
    public void ErrorProvider_Dispose_InvokeMultipleTimesWithoutItems_Nop()
    {
        using ErrorProvider provider = new();
        provider.Dispose();
        Assert.NotNull(provider.Icon);

        provider.Dispose();
        Assert.NotNull(provider.Icon);
    }

    [WinFormsTheory]
    [InlineData(true, "")]
    [InlineData(false, "error")]
    public void ErrorProvider_Dispose_InvokeBoolWithItems_ClearsIfDisposing(bool disposing, string expectedError)
    {
        using SubErrorProvider provider = new();
        using Control control = new();
        provider.SetError(control, "error");
        Assert.Equal("error", provider.GetError(control));

        provider.Dispose(disposing);
        Assert.NotNull(provider.Icon);
        Assert.Equal(expectedError, provider.GetError(control));

        provider.Dispose(disposing);
        Assert.NotNull(provider.Icon);
        Assert.Equal(expectedError, provider.GetError(control));
    }

    [WinFormsTheory]
    [BoolData]
    public void ErrorProvider_Dispose_InvokeBoolMultipleTimesDefault_Nop(bool disposing)
    {
        using SubErrorProvider provider = new();
        provider.Dispose(disposing);
        Assert.NotNull(provider.Icon);

        provider.Dispose(disposing);
        Assert.NotNull(provider.Icon);
    }

    [WinFormsFact]
    public void ErrorProvider_GetError_InvokeWithoutError_ReturnsEmpty()
    {
        using ErrorProvider provider = new();
        using Control control = new();
        Assert.Empty(provider.GetError(control));

        // Call again.
        Assert.Empty(provider.GetError(control));
    }

    [WinFormsFact]
    public void ErrorProvider_GetError_NullControl_ThrowsArgumentNullException()
    {
        using ErrorProvider provider = new();
        Assert.Throws<ArgumentNullException>("control", () => provider.GetError(null));
    }

    [WinFormsFact]
    public void ErrorProvider_GetIconAlignment_InvokeWithoutError_ReturnsMiddleRight()
    {
        using ErrorProvider provider = new();
        using Control control = new();
        Assert.Equal(ErrorIconAlignment.MiddleRight, provider.GetIconAlignment(control));

        // Call again.
        Assert.Equal(ErrorIconAlignment.MiddleRight, provider.GetIconAlignment(control));
    }

    [WinFormsFact]
    public void ErrorProvider_GetIconAlignment_NullControl_ThrowsArgumentNullException()
    {
        using ErrorProvider provider = new();
        Assert.Throws<ArgumentNullException>("control", () => provider.GetIconAlignment(null));
    }

    [WinFormsFact]
    public void ErrorProvider_GetIconPadding_InvokeWithoutError_ReturnsZero()
    {
        using ErrorProvider provider = new();
        using Control control = new();
        Assert.Equal(0, provider.GetIconPadding(control));

        // Call again.
        Assert.Equal(0, provider.GetIconPadding(control));
    }

    [WinFormsFact]
    public void ErrorProvider_GetIconPadding_NullControl_ThrowsArgumentNullException()
    {
        using ErrorProvider provider = new();
        Assert.Throws<ArgumentNullException>("control", () => provider.GetIconPadding(null));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void ErrorProvider_OnRightToLeftChanged_Invoke_CallsRightToLeftChanged(EventArgs eventArgs)
    {
        using SubErrorProvider provider = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(provider, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        provider.RightToLeftChanged += handler;
        provider.OnRightToLeftChanged(eventArgs);
        Assert.Equal(1, callCount);

        // Remove handler.
        provider.RightToLeftChanged -= handler;
        provider.OnRightToLeftChanged(eventArgs);
        Assert.Equal(1, callCount);
    }

    public static IEnumerable<object[]> SetError_TestData()
    {
        foreach (ErrorBlinkStyle blinkStyle in Enum.GetValues(typeof(ErrorBlinkStyle)))
        {
            yield return new object[] { blinkStyle, null, string.Empty };
            yield return new object[] { blinkStyle, string.Empty, string.Empty };
            yield return new object[] { blinkStyle, "value", "value" };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(SetError_TestData))]
    public void ErrorProvider_SetError_Invoke_GetErrorReturnsExpected(ErrorBlinkStyle blinkStyle, string value, string expected)
    {
        using ErrorProvider provider = new()
        {
            BlinkStyle = blinkStyle
        };
        using Control control = new();

        provider.SetError(control, value);
        Assert.Equal(expected, provider.GetError(control));

        // Call again.
        provider.SetError(control, value);
        Assert.Equal(expected, provider.GetError(control));

        // Set empty.
        provider.SetError(control, string.Empty);
        Assert.Empty(provider.GetError(control));
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void ErrorProvider_SetError_NullControl_ThrowsArgumentNullException(string value)
    {
        using ErrorProvider provider = new();
        Assert.Throws<ArgumentNullException>("control", () => provider.SetError(null, value));
    }

    [WinFormsTheory]
    [EnumData<ErrorIconAlignment>]
    public void ErrorProvider_SetIconAlignment_Invoke_GetIconAlignmentReturnsExpected(ErrorIconAlignment value)
    {
        using ErrorProvider provider = new();
        using Control control = new();

        provider.SetIconAlignment(control, value);
        Assert.Equal(value, provider.GetIconAlignment(control));

        // Call again.
        provider.SetIconAlignment(control, value);
        Assert.Equal(value, provider.GetIconAlignment(control));
    }

    [WinFormsTheory]
    [EnumData<ErrorIconAlignment>]
    [InvalidEnumData<ErrorIconAlignment>]
    public void ErrorProvider_SetIconAlignment_NullControl_ThrowsArgumentNullException(ErrorIconAlignment value)
    {
        using ErrorProvider provider = new();
        Assert.Throws<ArgumentNullException>("control", () => provider.SetIconAlignment(null, value));
    }

    [WinFormsTheory]
    [InvalidEnumData<ErrorIconAlignment>]
    public void ErrorProvider_SetIconAlignment_InvalidValue_ThrowsInvalidEnumArgumentException(ErrorIconAlignment value)
    {
        using ErrorProvider provider = new();
        using Control control = new();
        Assert.Throws<InvalidEnumArgumentException>("value", () => provider.SetIconAlignment(control, value));
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void ErrorProvider_SetIconPadding_Invoke_GetIconPaddingReturnsExpected(int value)
    {
        using ErrorProvider provider = new();
        using Control control = new();

        provider.SetIconPadding(control, value);
        Assert.Equal(value, provider.GetIconPadding(control));

        // Call again.
        provider.SetIconPadding(control, value);
        Assert.Equal(value, provider.GetIconPadding(control));
    }

    [WinFormsFact]
    public void ErrorProvider_SetIconPadding_NullControl_ThrowsArgumentNullException()
    {
        using ErrorProvider provider = new();
        Assert.Throws<ArgumentNullException>("control", () => provider.SetIconPadding(null, 0));
    }

    public static IEnumerable<object[]> CallEvents_TestData()
    {
        foreach (ErrorBlinkStyle blinkStyle in Enum.GetValues(typeof(ErrorBlinkStyle)))
        {
            foreach (string error in new string[] { null, string.Empty, "error" })
            {
                yield return new object[] { blinkStyle, new SubControl(), error, error ?? string.Empty };
                yield return new object[] { blinkStyle, new SubControl { Visible = false }, error, error ?? string.Empty };
                yield return new object[] { blinkStyle, new SubControl { Parent = new() }, error, error ?? string.Empty };
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(CallEvents_TestData))]
    public void ErrorProvider_Items_CallEvents_Success(ErrorBlinkStyle blinkStyle, SubControl control, string error, string expected)
    {
        bool originalVisible = control.Visible;
        using Control originalParent = new();
        using Control newParent = new();

        using ErrorProvider provider = new()
        {
            BlinkStyle = blinkStyle
        };
        provider.SetError(control, error);
        Assert.Equal(expected, provider.GetError(control));

        // Call event properties - without handle.
        control.Location = new Point(1, 2);
        Assert.Equal(new Point(1, 2), control.Location);

        control.Size = new Size(2, 3);
        Assert.Equal(new Size(2, 3), control.Size);

        control.Visible = !originalVisible;
        Assert.Equal(!originalVisible, control.Visible);

        control.Visible = originalVisible;
        Assert.Equal(originalVisible, control.Visible);

        control.Parent = newParent;
        Assert.Same(newParent, control.Parent);

        control.Parent = null;
        Assert.Null(control.Parent);

        control.Parent = originalParent;
        Assert.Same(originalParent, control.Parent);

        // Call event methods - without handle.
        control.OnHandleCreated(EventArgs.Empty);
        control.OnHandleDestroyed(EventArgs.Empty);
        control.OnLocationChanged(EventArgs.Empty);
        control.OnSizeChanged(EventArgs.Empty);
        control.OnVisibleChanged(EventArgs.Empty);
        control.OnParentChanged(EventArgs.Empty);

        // Call event properties - with handle.
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Location = new Point(2, 3);
        Assert.Equal(new Point(2, 3), control.Location);

        control.Size = new Size(4, 5);
        Assert.Equal(new Size(4, 5), control.Size);

        control.Visible = !originalVisible;
        Assert.Equal(!originalVisible, control.Visible);

        control.Visible = originalVisible;
        Assert.Equal(originalVisible, control.Visible);

        control.Parent = newParent;
        Assert.Same(newParent, control.Parent);

        control.Parent = null;
        Assert.Null(control.Parent);

        control.Parent = originalParent;
        Assert.Same(originalParent, control.Parent);

        // Call event methods - with handle.
        control.OnHandleCreated(EventArgs.Empty);
        control.OnHandleDestroyed(EventArgs.Empty);
        control.OnLocationChanged(EventArgs.Empty);
        control.OnSizeChanged(EventArgs.Empty);
        control.OnVisibleChanged(EventArgs.Empty);
        control.OnParentChanged(EventArgs.Empty);

        control.Dispose();
    }

    [WinFormsFact]
    public void ErrorProvider_HasErrors_NoControl_Set()
    {
        using ErrorProvider provider = new();

        Assert.False(provider.HasErrors);
    }

    [WinFormsFact]
    public void ErrorProvider_HasErrors_SomeControlsSet()
    {
        using ErrorProvider provider = new();
        using Control control1 = new();

        provider.SetError(control1, "error");

        Assert.True(provider.HasErrors);
    }

    [WinFormsFact]
    public void ErrorProvider_HasErrors_Cleared()
    {
        using ErrorProvider provider = new();
        using Control control1 = new();

        provider.SetError(control1, "Some error");
        Assert.True(provider.HasErrors);

        provider.Clear();
        Assert.False(provider.HasErrors);
    }

    [WinFormsFact]
    public void ErrorProvider_HasErrors_After_ErrorSetNull()
    {
        using ErrorProvider provider = new();
        using Control control1 = new();

        provider.SetError(control1, "Some error");
        Assert.True(provider.HasErrors);

        provider.SetError(control1, null);
        Assert.False(provider.HasErrors);
    }

    [WinFormsFact]
    public void ErrorProvider_HasErrors_After_ErrorSetEmptyString()
    {
        using ErrorProvider provider = new();
        using Control control1 = new();

        provider.SetError(control1, "Some error");
        Assert.True(provider.HasErrors);

        provider.SetError(control1, string.Empty);
        Assert.False(provider.HasErrors);
    }

    [WinFormsFact]
    public void ErrorProvider_HasErrors_After_ErrorSetEmptyStringMultiple()
    {
        using ErrorProvider provider = new();
        using Control control1 = new();

        provider.SetError(control1, "Some error");
        Assert.True(provider.HasErrors);

        provider.SetError(control1, string.Empty);
        provider.SetError(control1, string.Empty);
        provider.SetError(control1, string.Empty);
        provider.SetError(control1, string.Empty);
        Assert.False(provider.HasErrors);
    }

    [WinFormsFact]
    public void ErrorProvider_HasErrors_SetErrorMultiple()
    {
        using ErrorProvider provider = new();
        using Control control1 = new();

        provider.SetError(control1, "Some error");
        provider.SetError(control1, "Some error");
        provider.SetError(control1, "Some error");
        provider.SetError(control1, "Some error");
        Assert.True(provider.HasErrors);
    }

    [WinFormsFact]
    public void ErrorProvider_HasErrors_After_GetError()
    {
        using ErrorProvider provider = new();
        using Control control1 = new();

        _ = provider.GetError(control1);
        Assert.False(provider.HasErrors);
    }

    [WinFormsFact]
    public void ErrorProvider_CustomDataSource_DoesNotThrowInvalidCastException()
    {
        using Form form = new();
        CustomDataSource customDataSource = new();
        form.DataBindings.Add("Text", customDataSource, "Error");
        using ErrorProvider errorProvider = new(form);

        var exception = Record.Exception(() => errorProvider.DataSource = customDataSource);

        Assert.Null(exception);
    }

    [WinFormsFact]
    public void ErrorProvider_Icon_NotDisposed_Unexpectedly()
    {
        // Unit test for https://github.com/dotnet/winforms/issues/8513.
        using ErrorProvider provider = new();
        var icon = provider.Icon;

        Assert.NotNull(icon);

        nint handle = icon.Handle;
        using Icon newIcon = new("bitmaps/10x16_one_entry_32bit.ico");
        provider.Icon = newIcon;

        Assert.NotNull(provider.Icon);

        // Make sure the old icon that is not owned by us, is not disposed
        Assert.Equal(handle, icon.Handle);
    }

    private class CustomDataSource : IDataErrorInfo
    {
        public string this[string columnName] => string.Empty;

        public string Error => string.Empty;
    }

    private class SubErrorProvider : ErrorProvider
    {
        public SubErrorProvider() : base()
        {
        }

        public SubErrorProvider(ContainerControl parentControl) : base(parentControl)
        {
        }

        public SubErrorProvider(IContainer container) : base(container)
        {
        }

        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new bool DesignMode => base.DesignMode;

        public new EventHandlerList Events => base.Events;

        public new void Dispose(bool disposing) => base.Dispose(disposing);

        public new void OnRightToLeftChanged(EventArgs e) => base.OnRightToLeftChanged(e);
    }

    private class DataClass
    {
        public int Value { get; set; }

        public List<int> ListValue { get; set; }
    }

    private class SubContainerControl : ContainerControl
    {
        public override BindingContext BindingContext => null;
    }

    public class SubControl : Control
    {
        public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

        public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

        public new void OnLocationChanged(EventArgs e) => base.OnLocationChanged(e);

        public new void OnParentChanged(EventArgs e) => base.OnParentChanged(e);

        public new void OnSizeChanged(EventArgs e) => base.OnSizeChanged(e);

        public new void OnVisibleChanged(EventArgs e) => base.OnVisibleChanged(e);
    }
}
