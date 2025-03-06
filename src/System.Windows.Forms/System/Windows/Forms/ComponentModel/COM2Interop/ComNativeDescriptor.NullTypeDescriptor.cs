// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using static System.TrimmingConstants;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal partial class ComNativeDescriptor
{
    /// <summary>
    ///  Stub descriptor for when we're passed a null to <see cref="ComNativeDescriptor"/>. This used to be
    ///  contained in <see cref="ComTypeDescriptor"/>.
    /// </summary>
    [RequiresUnreferencedCode(ComTypeDescriptorsMessage + " Uses ComNativeDescriptor which is not trim-compatible.")]
    private sealed class NullTypeDescriptor : ICustomTypeDescriptor
    {
        AttributeCollection ICustomTypeDescriptor.GetAttributes() => new(s_staticAttributes);
        string? ICustomTypeDescriptor.GetClassName() => string.Empty;
        string? ICustomTypeDescriptor.GetComponentName() => string.Empty;
        [RequiresUnreferencedCode(AttributesRequiresUnreferencedCodeMessage)]
        TypeConverter? ICustomTypeDescriptor.GetConverter() => GetIComponentConverter();
        [RequiresUnreferencedCode(EventDescriptorRequiresUnreferencedCodeMessage)]
        EventDescriptor? ICustomTypeDescriptor.GetDefaultEvent() => null;
        [RequiresUnreferencedCode(PropertyDescriptorPropertyTypeMessage)]
        PropertyDescriptor? ICustomTypeDescriptor.GetDefaultProperty() => null;
        [RequiresUnreferencedCode(EditorRequiresUnreferencedCode)]
        object? ICustomTypeDescriptor.GetEditor(Type editorBaseType) => null;
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => EventDescriptorCollection.Empty;
        [RequiresUnreferencedCode(FilterRequiresUnreferencedCodeMessage)]
        EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[]? attributes) => EventDescriptorCollection.Empty;
        [RequiresUnreferencedCode(PropertyDescriptorPropertyTypeMessage)]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() => PropertyDescriptorCollection.Empty;
        [RequiresUnreferencedCode($"{PropertyDescriptorPropertyTypeMessage} {FilterRequiresUnreferencedCodeMessage}")]
        PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[]? attributes) => PropertyDescriptorCollection.Empty;
        object? ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor? pd) => null;
    }
}
