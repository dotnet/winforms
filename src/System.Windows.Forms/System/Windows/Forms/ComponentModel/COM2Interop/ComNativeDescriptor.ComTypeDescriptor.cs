// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Windows.Win32.System.Com;
using static System.TrimmingConstants;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal partial class ComNativeDescriptor
{
    /// <summary>
    ///  The <see cref="ICustomTypeDescriptor"/> given by <see cref="ComNativeDescriptor"/> for a given type instance.
    /// </summary>
    [RequiresUnreferencedCode(ComTypeDescriptorsMessage + " Uses ComNativeDescriptor which is not trim-compatible.")]
    private sealed unsafe class ComTypeDescriptor : ICustomTypeDescriptor
    {
        private readonly ComNativeDescriptor _handler;
        private readonly object _instance;

        internal ComTypeDescriptor(ComNativeDescriptor handler, object instance)
        {
            _handler = handler;
            _instance = instance;
        }

        AttributeCollection ICustomTypeDescriptor.GetAttributes() => GetAttributes(_instance);

        string? ICustomTypeDescriptor.GetClassName() => GetClassName(_instance);

        string? ICustomTypeDescriptor.GetComponentName()
        {
            using var dispatch = ComHelpers.TryGetComScope<IDispatch>(_instance, out HRESULT hr);
            return hr.Succeeded ? GetName(dispatch) : string.Empty;
        }

        [RequiresUnreferencedCode(AttributesRequiresUnreferencedCodeMessage)]
        TypeConverter ICustomTypeDescriptor.GetConverter() => GetIComponentConverter();

        [RequiresUnreferencedCode(EventDescriptorRequiresUnreferencedCodeMessage)]
        EventDescriptor? ICustomTypeDescriptor.GetDefaultEvent() => null;

        [RequiresUnreferencedCode(PropertyDescriptorPropertyTypeMessage)]
        PropertyDescriptor? ICustomTypeDescriptor.GetDefaultProperty() => _handler.GetDefaultProperty(_instance);

        [RequiresUnreferencedCode(EditorRequiresUnreferencedCode)]
        object? ICustomTypeDescriptor.GetEditor(Type editorBaseType) => GetEditor(_instance, editorBaseType);

        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => new(null);

        [RequiresUnreferencedCode(FilterRequiresUnreferencedCodeMessage)]
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[]? attributes) => new(null);

        [RequiresUnreferencedCode(PropertyDescriptorPropertyTypeMessage)]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => _handler.GetProperties(_instance);

        [RequiresUnreferencedCode($"{PropertyDescriptorPropertyTypeMessage} {FilterRequiresUnreferencedCodeMessage}")]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[]? attributes) => _handler.GetProperties(_instance);

        object? ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor? pd) => _instance;
    }
}
