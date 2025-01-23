// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads (instantiated via GUID)
public class AxHostPropertyDescriptorTests
{
    private const string EmptyClsidString = "00000000-0000-0000-0000-000000000000";
    private const string WebBrowserClsidString = "8856f961-340a-11d0-a96b-00c04fd705a2";

    [WinFormsFact]
    public void AxPropertyDescriptor_Attributes_Get_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];
        Assert.NotNull(property.Attributes[typeof(CustomAttribute)]);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.Equal("Misc", Assert.IsType<CategoryAttribute>(property.Attributes[typeof(CategoryAttribute)]).Category);
        Assert.Empty(Assert.IsType<DescriptionAttribute>(property.Attributes[typeof(DescriptionAttribute)]).Description);
        Assert.False(Assert.IsType<ReadOnlyAttribute>(property.Attributes[typeof(ReadOnlyAttribute)]).IsReadOnly);
        Assert.Null(property.Attributes[typeof(DispIdAttribute)]);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_Attributes_GetWithDispIdAttribute_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdProperty)];
        Assert.Null(property.Attributes[typeof(CustomAttribute)]);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.Equal("Misc", Assert.IsType<CategoryAttribute>(property.Attributes[typeof(CategoryAttribute)]).Category);
        Assert.Empty(Assert.IsType<DescriptionAttribute>(property.Attributes[typeof(DescriptionAttribute)]).Description);
        Assert.False(Assert.IsType<ReadOnlyAttribute>(property.Attributes[typeof(ReadOnlyAttribute)]).IsReadOnly);
        Assert.Equal(PInvokeCore.DISPID_TEXT, Assert.IsType<DispIdAttribute>(property.Attributes[typeof(DispIdAttribute)]).Value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_Attributes_GetWithDispIdAttributeNotBrowsable_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdNotBrowsableProperty)];
        Assert.Null(property.Attributes[typeof(CustomAttribute)]);
        Assert.False(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.Equal("Misc", Assert.IsType<CategoryAttribute>(property.Attributes[typeof(CategoryAttribute)]).Category);
        Assert.Empty(Assert.IsType<DescriptionAttribute>(property.Attributes[typeof(DescriptionAttribute)]).Description);
        Assert.False(Assert.IsType<ReadOnlyAttribute>(property.Attributes[typeof(ReadOnlyAttribute)]).IsReadOnly);
        Assert.Equal(PInvokeCore.DISPID_TEXT, Assert.IsType<DispIdAttribute>(property.Attributes[typeof(DispIdAttribute)]).Value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_Attributes_GetWithDispIdAttributeReadOnly_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdReadOnlyProperty)];
        Assert.Null(property.Attributes[typeof(CustomAttribute)]);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.Equal("Misc", Assert.IsType<CategoryAttribute>(property.Attributes[typeof(CategoryAttribute)]).Category);
        Assert.Empty(Assert.IsType<DescriptionAttribute>(property.Attributes[typeof(DescriptionAttribute)]).Description);
        Assert.True(Assert.IsType<ReadOnlyAttribute>(property.Attributes[typeof(ReadOnlyAttribute)]).IsReadOnly);
        Assert.Equal(PInvokeCore.DISPID_TEXT, Assert.IsType<DispIdAttribute>(property.Attributes[typeof(DispIdAttribute)]).Value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_Attributes_GetWithDispIdAttributeNotBrowsableReadOnly_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdNotBrowsableReadOnlyProperty)];
        Assert.Null(property.Attributes[typeof(CustomAttribute)]);
        Assert.False(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.Equal("Misc", Assert.IsType<CategoryAttribute>(property.Attributes[typeof(CategoryAttribute)]).Category);
        Assert.Empty(Assert.IsType<DescriptionAttribute>(property.Attributes[typeof(DescriptionAttribute)]).Description);
        Assert.True(Assert.IsType<ReadOnlyAttribute>(property.Attributes[typeof(ReadOnlyAttribute)]).IsReadOnly);
        Assert.Equal(PInvokeCore.DISPID_TEXT, Assert.IsType<DispIdAttribute>(property.Attributes[typeof(DispIdAttribute)]).Value);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_Category_Get_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];
        Assert.Equal("Misc", property.Category);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_Category_GetWithCategoryAttribute_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CategoryProperty)];
        Assert.Equal("Category", property.Category);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_ComponentType_Get_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];
        Assert.Equal(typeof(CustomAxHost), property.ComponentType);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_Converter_Get_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];
        Assert.IsType<StringConverter>(property.Converter);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_Converter_GetDispId_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdProperty)];
        Assert.IsType<StringConverter>(property.Converter);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_Converter_GetDispIdWithOcx_ReturnsExpected()
    {
        using CustomAxHost control = new(WebBrowserClsidString);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdProperty)];
        Assert.IsType<StringConverter>(property.Converter);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again to test caching behavior.
        Assert.IsType<StringConverter>(property.Converter);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_Converter_GetCustomConverter_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomConverterProperty)];
        Assert.IsType<CustomTypeConverter>(property.Converter);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_Converter_GetDispIdCustomConverter_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdCustomConverterProperty)];
        Assert.IsType<CustomTypeConverter>(property.Converter);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_Description_Get_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];
        Assert.Empty(property.Description);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_Description_GetWithDescriptionAttribute_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DescriptionProperty)];
        Assert.Equal("Description", property.Description);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_IsReadOnly_Get_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];
        Assert.False(property.IsReadOnly);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_IsReadOnly_GetWithoutSetter_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.GetOnlyProperty)];
        Assert.True(property.IsReadOnly);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_IsBrowsable_Get_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];
        Assert.True(property.IsBrowsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_IsBrowsable_GetBrowsableAttribute_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.NotBrowsableProperty)];
        Assert.False(property.IsBrowsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_IsReadOnly_GetReadOnlyAttribute_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.ReadOnlyProperty)];
        Assert.True(property.IsReadOnly);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_PropertyType_Get_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];
        Assert.Equal(typeof(string), property.PropertyType);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_CanResetValue_Invoke_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString)
        {
            CustomProperty = "CustomValue"
        };
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];
        Assert.True(property.CanResetValue(control));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_CanResetValue_InvokeReadOnly_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.ReadOnlyProperty)];
        Assert.False(property.CanResetValue(control));
        Assert.False(control.IsHandleCreated);
    }

#if false
    [WinFormsFact]
    public void AxPropertyDescriptor_GetEditor_Invoke_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString)
        {
            CustomProperty = "CustomValue"
        };
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];
        Assert.Null(property.GetEditor(typeof(UITypeEditor)));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_GetEditor_InvokeWithEditor_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.EditorProperty)];
        Assert.IsType<CustomEditor>(property.GetEditor(typeof(object)));
        Assert.Null(property.GetEditor(typeof(CustomEditor)));
        Assert.Null(property.GetEditor(typeof(UITypeEditor)));
        Assert.Null(property.GetEditor(typeof(int)));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_GetEditor_InvokeWithUITypeEditor_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.UITypeEditorProperty)];
        Assert.Null(property.GetEditor(typeof(object));
        Assert.Null(property.GetEditor(typeof(CustomUITypeEditor)));
        Assert.IsType<CustomUITypeEditor>(property.GetEditor(typeof(UITypeEditor)));
        Assert.Null(property.GetEditor(typeof(int)));
        Assert.False(control.IsHandleCreated);
    }
#endif

    [WinFormsFact]
    public void AxPropertyDescriptor_GetEditor_InvokeDispId_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdProperty)];
        Assert.Null(property.GetEditor(typeof(UITypeEditor)));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_GetEditor_InvokeDispIdWithEditor_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdEditorProperty)];
        Assert.IsType<CustomEditor>(property.GetEditor(typeof(object)));
        Assert.Null(property.GetEditor(typeof(CustomEditor)));
        Assert.Null(property.GetEditor(typeof(UITypeEditor)));
        Assert.Null(property.GetEditor(typeof(int)));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_GetEditor_InvokeDispIdWithUITypeEditor_ReturnsExpected()
    {
        using CustomAxHost control = new(WebBrowserClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdUITypeEditorProperty)];
        Assert.Null(property.GetEditor(typeof(object)));
        Assert.Null(property.GetEditor(typeof(CustomUITypeEditor)));
        Assert.IsType<CustomUITypeEditor>(property.GetEditor(typeof(UITypeEditor)));
        Assert.Null(property.GetEditor(typeof(int)));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_GetEditor_InvokeDispIdWithOcx_ReturnsExpected()
    {
        using CustomAxHost control = new(WebBrowserClsidString);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdProperty)];
        Assert.Null(property.GetEditor(typeof(UITypeEditor)));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_GetEditor_InvokeDispIdWithOcxWithEditor_ReturnsExpected()
    {
        using CustomAxHost control = new(WebBrowserClsidString);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdEditorProperty)];
        Assert.IsType<CustomEditor>(property.GetEditor(typeof(object)));
        Assert.Null(property.GetEditor(typeof(CustomEditor)));
        Assert.Null(property.GetEditor(typeof(UITypeEditor)));
        Assert.Null(property.GetEditor(typeof(int)));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_GetEditor_InvokeDispIdWithOcxWithUITypeEditor_ReturnsExpected()
    {
        using CustomAxHost control = new(WebBrowserClsidString);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdUITypeEditorProperty)];
        Assert.Null(property.GetEditor(typeof(object)));
        Assert.Null(property.GetEditor(typeof(CustomUITypeEditor)));
        Assert.IsType<CustomUITypeEditor>(property.GetEditor(typeof(UITypeEditor)));
        Assert.Null(property.GetEditor(typeof(int)));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

#if false
    [WinFormsFact]
    public void AxPropertyDescriptor_GetEditor_NullEditorBaseType_ThrowsArgumentNullException()
    {
        using CustomAxHost control = new(EmptyClsidString)
        {
            CustomProperty = "CustomValue"
        };
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdProperty)];
        Assert.Throws<ArgumentNullException>("editorBaseType", () => property.GetEditor(null));
    }
#endif

    [WinFormsFact]
    public void AxPropertyDescriptor_GetValue_Invoke_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString)
        {
            CustomProperty = "CustomProperty"
        };
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];

        Assert.Null(property.GetValue(control));
        Assert.NotNull(property.Attributes[typeof(CustomAttribute)]);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.Equal("Misc", Assert.IsType<CategoryAttribute>(property.Attributes[typeof(CategoryAttribute)]).Category);
        Assert.Empty(Assert.IsType<DescriptionAttribute>(property.Attributes[typeof(DescriptionAttribute)]).Description);
        Assert.False(Assert.IsType<ReadOnlyAttribute>(property.Attributes[typeof(ReadOnlyAttribute)]).IsReadOnly);
        Assert.Null(property.Attributes[typeof(DispIdAttribute)]);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_GetValue_InvokeDataSource_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString)
        {
            DataSourceProperty = new DataSource
            {
                Value = "CustomProperty"
            }
        };
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DataSourceProperty)];

        Assert.Null(property.GetValue(control));
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_GetValue_InvokeDispatchIdWithDataSource_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString)
        {
            DispIdDataSourceProperty = new DataSource
            {
                Value = "CustomProperty"
            }
        };
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdDataSourceProperty)];

        Assert.Equal("CustomProperty", Assert.IsType<DataSource>(property.GetValue(control)).Value);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_GetValue_InvokeWithOcx_ReturnsExpected()
    {
        using CustomAxHost control = new(WebBrowserClsidString)
        {
            CustomProperty = "CustomProperty"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];

        Assert.Equal("CustomProperty", property.GetValue(control));
        Assert.NotNull(property.Attributes[typeof(CustomAttribute)]);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.Equal("Misc", Assert.IsType<CategoryAttribute>(property.Attributes[typeof(CategoryAttribute)]).Category);
        Assert.Empty(Assert.IsType<DescriptionAttribute>(property.Attributes[typeof(DescriptionAttribute)]).Description);
        Assert.False(Assert.IsType<ReadOnlyAttribute>(property.Attributes[typeof(ReadOnlyAttribute)]).IsReadOnly);
        Assert.Null(property.Attributes[typeof(DispIdAttribute)]);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_GetValue_InvokeDataSourceWithOcx_ReturnsExpected()
    {
        using CustomAxHost control = new(WebBrowserClsidString)
        {
            DataSourceProperty = new DataSource
            {
                Value = "CustomProperty"
            }
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DataSourceProperty)];

        Assert.Equal("CustomProperty", Assert.IsType<DataSource>(property.GetValue(control)).Value);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_GetValue_InvokeDispatchIdWithDataSourceWithOcx_ReturnsExpected()
    {
        using CustomAxHost control = new(WebBrowserClsidString)
        {
            DispIdDataSourceProperty = new DataSource
            {
                Value = "CustomProperty"
            }
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdDataSourceProperty)];

        Assert.Equal("CustomProperty", Assert.IsType<DataSource>(property.GetValue(control)).Value);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(nameof(CustomAxHost.CustomProperty))]
    [InlineData(nameof(CustomAxHost.DispIdProperty))]
    public void AxPropertyDescriptor_GetValue_NullComponent_ReturnsExpected(string propertyName)
    {
        using CustomAxHost control = new(EmptyClsidString)
        {
            CustomProperty = "CustomProperty"
        };
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[propertyName];

        Assert.Null(property.GetValue(null));
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(nameof(CustomAxHost.CustomProperty))]
    [InlineData(nameof(CustomAxHost.DispIdProperty))]
    public void AxPropertyDescriptor_GetValue_NullComponentWithOcx_ThrowsArgumentNullException(string propertyName)
    {
        using CustomAxHost control = new(WebBrowserClsidString)
        {
            CustomProperty = "CustomProperty"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[propertyName];

        Assert.Null(property.GetValue(null));
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(nameof(CustomAxHost.CustomProperty))]
    [InlineData(nameof(CustomAxHost.DispIdProperty))]
    public void AxPropertyDescriptor_GetValueInvalidComponent_ReturnsExpected(string propertyName)
    {
        using CustomAxHost control = new(EmptyClsidString)
        {
            CustomProperty = "CustomProperty"
        };
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[propertyName];

        Assert.Null(property.GetValue(new object()));
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(nameof(CustomAxHost.CustomProperty))]
    [InlineData(nameof(CustomAxHost.DispIdProperty))]
    public void AxPropertyDescriptor_GetValueInvalidComponentWithOcx_ThrowsTargetInvocationException(string propertyName)
    {
        using CustomAxHost control = new(WebBrowserClsidString)
        {
            CustomProperty = "CustomProperty"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[propertyName];

        Assert.Throws<TargetInvocationException>(() => property.GetValue(new object()));
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Null(property.GetValue(new object()));
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_ResetValue_Invoke_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString)
        {
            CustomProperty = "CustomValue"
        };
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];
        property.ResetValue(control);
        Assert.Null(control.CustomProperty);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_ResetValue_InvokeReadOnly_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.ReadOnlyProperty)];
        property.ResetValue(control);
        Assert.Null(control.CustomProperty);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValue_Invoke_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString)
        {
            CustomProperty = "CustomProperty"
        };
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];

        property.SetValue(control, "CustomProperty");
        Assert.Equal("CustomProperty", control.CustomProperty);
        Assert.NotNull(property.Attributes[typeof(CustomAttribute)]);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.Equal("Misc", Assert.IsType<CategoryAttribute>(property.Attributes[typeof(CategoryAttribute)]).Category);
        Assert.Empty(Assert.IsType<DescriptionAttribute>(property.Attributes[typeof(DescriptionAttribute)]).Description);
        Assert.False(Assert.IsType<ReadOnlyAttribute>(property.Attributes[typeof(ReadOnlyAttribute)]).IsReadOnly);
        Assert.Null(property.Attributes[typeof(DispIdAttribute)]);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValue_InvokeDataSource_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DataSourceProperty)];

        property.SetValue(control, new DataSource
        {
            Value = "CustomProperty"
        });
        Assert.Null(control.DataSourceProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValue_InvokeDispatchIdWithDataSource_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdDataSourceProperty)];

        property.SetValue(control, new DataSource
        {
            Value = "CustomProperty"
        });
        Assert.Null(control.DataSourceProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValue_InvokeWithOcx_ReturnsExpected()
    {
        using CustomAxHost control = new(WebBrowserClsidString)
        {
            CustomProperty = "CustomProperty"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];

        property.SetValue(control, "CustomProperty");
        Assert.Equal("CustomProperty", control.CustomProperty);
        Assert.NotNull(property.Attributes[typeof(CustomAttribute)]);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.Equal("Misc", Assert.IsType<CategoryAttribute>(property.Attributes[typeof(CategoryAttribute)]).Category);
        Assert.Empty(Assert.IsType<DescriptionAttribute>(property.Attributes[typeof(DescriptionAttribute)]).Description);
        Assert.False(Assert.IsType<ReadOnlyAttribute>(property.Attributes[typeof(ReadOnlyAttribute)]).IsReadOnly);
        Assert.Null(property.Attributes[typeof(DispIdAttribute)]);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValue_InvokeWithOcxDataSource_ReturnsExpected()
    {
        using CustomAxHost control = new(WebBrowserClsidString);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DataSourceProperty)];

        property.SetValue(control, new DataSource
        {
            Value = "CustomProperty"
        });
        Assert.Equal("CustomProperty", Assert.IsType<DataSource>(control.DataSourceProperty).Value);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValue_InvokeWithOcxDispatchIdWithDataSource_ReturnsExpected()
    {
        using CustomAxHost control = new(WebBrowserClsidString);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.DispIdDataSourceProperty)];

        property.SetValue(control, new DataSource
        {
            Value = "CustomProperty"
        });
        Assert.Null(control.DataSourceProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValue_InvokeEnumWithOcx_ReturnsExpected()
    {
        using CustomAxHost control = new(WebBrowserClsidString)
        {
            CustomProperty = "CustomProperty"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.EnumProperty)];

        property.SetValue(control, ConsoleColor.Blue);
        Assert.Equal(ConsoleColor.Blue, control.EnumProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValue_InvokeEnumNotEnumTypeWithOcx_ReturnsExpected()
    {
        using CustomAxHost control = new(WebBrowserClsidString)
        {
            CustomProperty = "CustomProperty"
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.EnumProperty)];

        property.SetValue(control, (int)ConsoleColor.Blue);
        Assert.Equal(ConsoleColor.Blue, control.EnumProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(nameof(CustomAxHost.CustomProperty))]
    [InlineData(nameof(CustomAxHost.DispIdProperty))]
    public void AxPropertyDescriptor_SetValue_NullComponent_ReturnsExpected(string propertyName)
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[propertyName];

        property.SetValue(null, "CustomProperty");
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(nameof(CustomAxHost.CustomProperty))]
    [InlineData(nameof(CustomAxHost.DispIdProperty))]
    public void AxPropertyDescriptor_SetValue_NullComponentWithOcx_ThrowsArgumentNullException(string propertyName)
    {
        using CustomAxHost control = new(WebBrowserClsidString);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[propertyName];

        property.SetValue(null, "Value");
        Assert.Null(control.CustomProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(nameof(CustomAxHost.CustomProperty))]
    [InlineData(nameof(CustomAxHost.DispIdProperty))]
    public void AxPropertyDescriptor_SetValueInvalidComponent_ReturnsExpected(string propertyName)
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[propertyName];

        property.SetValue(new object(), "Value");
        Assert.Null(control.CustomProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(nameof(CustomAxHost.CustomProperty))]
    [InlineData(nameof(CustomAxHost.DispIdProperty))]
    public void AxPropertyDescriptor_SetValueInvalidComponentWithOcx_ThrowsTargetException(string propertyName)
    {
        using CustomAxHost control = new(WebBrowserClsidString);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[propertyName];

        Assert.Throws<TargetException>(() => property.SetValue(new object(), "Value"));
        Assert.Null(control.CustomProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Throws<TargetException>(() => property.SetValue(new object(), "Value"));
        Assert.Null(control.CustomProperty);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsTheory]
    [InlineData(nameof(CustomAxHost.CustomProperty))]
    [InlineData(nameof(CustomAxHost.DispIdProperty))]
    public void AxPropertyDescriptor_SetValueInvalidValue_ReturnsExpected(string propertyName)
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[propertyName];

        property.SetValue(control, new object());
        Assert.Null(control.CustomProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData(nameof(CustomAxHost.CustomProperty))]
    [InlineData(nameof(CustomAxHost.DispIdProperty))]
    public void AxPropertyDescriptor_SetValueInvalidValueWithOcx_ThrowsArgumentException(string propertyName)
    {
        using CustomAxHost control = new(WebBrowserClsidString);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[propertyName];

        Assert.Throws<ArgumentException>(() => property.SetValue(control, new object()));
        Assert.Null(control.CustomProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Throws<ArgumentException>(() => property.SetValue(control, new object()));
        Assert.Null(control.CustomProperty);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValueInvalidValueInt_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.IntProperty)];

        property.SetValue(control, new object());
        Assert.Equal(0, control.IntProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValueInvalidValueIntWithOcx_ThrowsArgumentException()
    {
        using CustomAxHost control = new(WebBrowserClsidString);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.IntProperty)];

        Assert.Throws<ArgumentException>(() => property.SetValue(control, new object()));
        Assert.Equal(0, control.IntProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Throws<ArgumentException>(() => property.SetValue(control, new object()));
        Assert.Equal(0, control.IntProperty);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValueInvalidValueEnum_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.EnumProperty)];

        property.SetValue(control, new object());
        Assert.Equal((ConsoleColor)0, control.EnumProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValueInvalidValueEnumWithOcx_ThrowsArgumentException()
    {
        using CustomAxHost control = new(WebBrowserClsidString);
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.EnumProperty)];

        Assert.Throws<ArgumentException>("value", () => property.SetValue(control, new object()));
        Assert.Equal((ConsoleColor)0, control.EnumProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        Assert.Throws<ArgumentException>("value", () => property.SetValue(control, new object()));
        Assert.Equal((ConsoleColor)0, control.EnumProperty);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValueNullValueInt_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString)
        {
            IntProperty = 1
        };
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.IntProperty)];

        property.SetValue(control, null);
        Assert.Equal(1, control.IntProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValueNullValueIntWithOcx_ThrowsArgumentException()
    {
        using CustomAxHost control = new(WebBrowserClsidString)
        {
            IntProperty = 1
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.IntProperty)];

        property.SetValue(control, null);
        Assert.Equal(0, control.IntProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        property.SetValue(control, null);
        Assert.Equal(0, control.IntProperty);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_SetValueNullValueEnum_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString)
        {
            EnumProperty = ConsoleColor.Blue
        };
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.EnumProperty)];

        property.SetValue(control, null);
        Assert.Equal(ConsoleColor.Blue, control.EnumProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.False(control.IsHandleCreated);
    }

#if false
    [WinFormsFact]
    public void AxPropertyDescriptor_SetValueNullValueEnumWithOcx_ThrowsArgumentException()
    {
        using CustomAxHost control = new(WebBrowserClsidString)
        {
            EnumProperty = ConsoleColor.Blue
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;
        Assert.NotNull(control.GetOcx());

        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.EnumProperty)];

        property.SetValue(control, null);
        Assert.Equal(ConsoleColor.Blue, control.EnumProperty);
        Assert.True(Assert.IsType<BrowsableAttribute>(property.Attributes[typeof(BrowsableAttribute)]).Browsable);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Call again.
        property.SetValue(control, new object());
        Assert.Equal(ConsoleColor.Blue, control.EnumProperty);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }
#endif

    [WinFormsFact]
    public void AxPropertyDescriptor_ShouldSerializeValue_Invoke_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString)
        {
            CustomProperty = "CustomValue"
        };
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.CustomProperty)];
        Assert.True(property.ShouldSerializeValue(control));
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void AxPropertyDescriptor_ShouldSerializeValue_InvokeReadOnly_ReturnsExpected()
    {
        using CustomAxHost control = new(EmptyClsidString);
        ICustomTypeDescriptor customTypeDescriptor = control;
        PropertyDescriptorCollection events = customTypeDescriptor.GetProperties();
        PropertyDescriptor property = events[nameof(CustomAxHost.ReadOnlyProperty)];
        Assert.False(property.ShouldSerializeValue(control));
        Assert.False(control.IsHandleCreated);
    }

    private class SubComponentEditor : ComponentEditor
    {
        public override bool EditComponent(ITypeDescriptorContext context, object component)
        {
            throw new NotImplementedException();
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    private class CustomAttribute : Attribute
    {
    }

    private class CustomTypeConverter : TypeConverter
    {
    }

    private class CustomEditor
    {
    }

    private class CustomUITypeEditor : UITypeEditor
    {
    }

    [Guid("7C0FFAB3-CD84-11D0-949A-00A0C91110ED")]
    private class DataSource
    {
        public string Value { get; set; }
    }

    [Custom]
    private class CustomAxHost : AxHost
    {
        public CustomAxHost(string clsid) : base(clsid)
        {
        }

        [Custom]
        [DefaultValue(null)]
        public string CustomProperty { get; set; }

        public DataSource DataSourceProperty { get; set; }

        public ConsoleColor EnumProperty { get; set; }

        public int IntProperty { get; set; }

        [TypeConverter(typeof(CustomTypeConverter))]
        public string CustomConverterProperty { get; set; }

        [TypeConverter(typeof(CustomTypeConverter))]
        public string DispIdCustomConverterProperty { get; set; }

        [Editor(typeof(CustomEditor), typeof(object))]
        public string EditorProperty { get; set; }

        [Editor(typeof(CustomUITypeEditor), typeof(UITypeEditor))]
        public string UITypeEditorProperty { get; set; }

        public string GetOnlyProperty { get; }

        [Category("Category")]
        public string CategoryProperty { get; set; }

        [DispId(PInvokeCore.DISPID_TEXT)]
        [Description("Description")]
        public string DescriptionProperty { get; set; }

        [Browsable(false)]
        public string NotBrowsableProperty { get; set; }

        [ReadOnly(true)]
        public string ReadOnlyProperty { get; set; }

        [DispId(PInvokeCore.DISPID_TEXT)]
        public string DispIdProperty { get; set; }

        [DispId(PInvokeCore.DISPID_TEXT)]
        public DataSource DispIdDataSourceProperty { get; set; }

        [DispId(PInvokeCore.DISPID_TEXT)]
        [Browsable(false)]
        public string DispIdNotBrowsableProperty { get; set; }

        [DispId(PInvokeCore.DISPID_TEXT)]
        [ReadOnly(true)]
        public string DispIdReadOnlyProperty { get; set; }

        [DispId(PInvokeCore.DISPID_TEXT)]
        [ReadOnly(true)]
        [Browsable(false)]
        public string DispIdNotBrowsableReadOnlyProperty { get; set; }

        [DispId(PInvokeCore.DISPID_TEXT)]
        [Editor(typeof(CustomEditor), typeof(object))]
        public string DispIdEditorProperty { get; set; }

        [DispId(PInvokeCore.DISPID_TEXT)]
        [Editor(typeof(CustomUITypeEditor), typeof(UITypeEditor))]
        public string DispIdUITypeEditorProperty { get; set; }
    }
}
