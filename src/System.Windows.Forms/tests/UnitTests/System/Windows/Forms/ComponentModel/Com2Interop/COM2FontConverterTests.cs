// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.ComponentModel.Com2Interop;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.Tests.ComponentModel.Com2Interop;

public unsafe class COM2FontConverterTests
{
    private static readonly Com2PropertyDescriptor s_stubDescriptor = new(
        default,
        "Foo",
        Array.Empty<Attribute>(),
        default,
        default,
        default,
        default);

    [Fact]
    public void COM2FontConverter_ConvertNativeToManaged()
    {
        fixed (char* n = "Arial")
        {
            FONTDESC fontDesc = new()
            {
                cbSizeofstruct = (uint)sizeof(FONTDESC),
                lpstrName = n,
                cySize = (CY)12.0f
            };

            using ComScope<IFont> iFont = new(null);
            PInvoke.OleCreateFontIndirect(&fontDesc, IID.Get<IFont>(), iFont).ThrowOnFailure();

            Com2FontConverter converter = new();
            using Font? font = (Font?)converter.ConvertNativeToManaged((VARIANT)(IUnknown*)iFont, s_stubDescriptor);
            Assert.NotNull(font);

            // Converter might have failed and returned DefaultFont.
            Assert.NotEqual(font, Control.DefaultFont);

            Assert.Equal("Arial", font.Name);
            Assert.Equal(12, font.Size);

            bool cancelSet = false;
            using VARIANT result = converter.ConvertManagedToNative(font, s_stubDescriptor, ref cancelSet);
            Assert.True(result.IsEmpty);
            Assert.True(cancelSet);
        }
    }

    [Fact]
    public void COM2FontConverter_ConvertManagedToNative()
    {
        fixed (char* n = "Arial")
        {
            FONTDESC fontDesc = new()
            {
                cbSizeofstruct = (uint)sizeof(FONTDESC),
                lpstrName = n,
                cySize = (CY)12.0f
            };

            using ComScope<IFont> iFont = new(null);
            PInvoke.OleCreateFontIndirect(&fontDesc, IID.Get<IFont>(), iFont).ThrowOnFailure();

            Com2FontConverter converter = new();
            using Font? font = (Font?)converter.ConvertNativeToManaged((VARIANT)(IUnknown*)iFont, s_stubDescriptor);
            Assert.NotNull(font);

            using Font newFont = new(font.Name, 20.0f);

            bool cancelSet = false;

            // Need to addref here as ConvertManagedToNative will release the VARIANT we cast to below.
            iFont.Value->AddRef();

            using VARIANT result = converter.ConvertManagedToNative(
                newFont,
                new CustomGetNativeValueDescriptor((VARIANT)(IUnknown*)iFont.Value),
                ref cancelSet);

            Assert.True(cancelSet);
            Assert.True(result.IsEmpty);
            Assert.Equal("Arial", iFont.Value->Name.ToStringAndFree());
            Assert.Equal(20.0f, (float)iFont.Value->Size, precision: 0);
        }
    }

    private class CustomGetNativeValueDescriptor : Com2PropertyDescriptor
    {
        private readonly ICustomTypeDescriptor _descriptor;

        public CustomGetNativeValueDescriptor(VARIANT nativeValue)
            : base(default, "Foo", Array.Empty<Attribute>(), default, default, default, default)
        {
            _descriptor = new CustomDescriptor(nativeValue);
        }

        public override object TargetObject => _descriptor;

        private class CustomDescriptor : ICustomTypeDescriptor
        {
            private readonly object _propertyOwner;

            public CustomDescriptor(VARIANT variant) => _propertyOwner = new DispatchStub(variant);

            public AttributeCollection GetAttributes() => throw new NotImplementedException();

            public string? GetClassName() => throw new NotImplementedException();

            public string? GetComponentName() => throw new NotImplementedException();

            public TypeConverter? GetConverter() => throw new NotImplementedException();

            public EventDescriptor? GetDefaultEvent() => throw new NotImplementedException();

            public PropertyDescriptor? GetDefaultProperty() => throw new NotImplementedException();

            public object? GetEditor(Type editorBaseType) => throw new NotImplementedException();

            public EventDescriptorCollection GetEvents() => throw new NotImplementedException();

            public EventDescriptorCollection GetEvents(Attribute[]? attributes) => throw new NotImplementedException();

            public PropertyDescriptorCollection GetProperties() => throw new NotImplementedException();

            public PropertyDescriptorCollection GetProperties(Attribute[]? attributes) => throw new NotImplementedException();

            public object? GetPropertyOwner(PropertyDescriptor? pd) => _propertyOwner;

            private class DispatchStub : IDispatch.Interface, IManagedWrapper<IDispatch>
            {
                private readonly VARIANT _variant;
                public DispatchStub(VARIANT variant) => _variant = variant;

                HRESULT IDispatch.Interface.GetTypeInfoCount(uint* pctinfo) => throw new NotImplementedException();

                HRESULT IDispatch.Interface.GetTypeInfo(uint iTInfo, uint lcid, ITypeInfo** ppTInfo) => throw new NotImplementedException();

                HRESULT IDispatch.Interface.GetIDsOfNames(Guid* riid, PWSTR* rgszNames, uint cNames, uint lcid, int* rgDispId) => throw new NotImplementedException();

                HRESULT IDispatch.Interface.Invoke(
                    int dispIdMember,
                    Guid* riid,
                    uint lcid,
                    DISPATCH_FLAGS dwFlags,
                    DISPPARAMS* pDispParams,
                    VARIANT* pVarResult,
                    EXCEPINFO* pExcepInfo,
                    uint* pArgErr)
                {
                    *pVarResult = _variant;
                    return HRESULT.S_OK;
                }
            }
        }
    }
}
