// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using static System.TrimmingConstants;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal partial class ComNativeDescriptor
    {
        /// <summary>
        ///  This type descriptor sits on top of a ComNativeDescriptor
        /// </summary>
        private sealed class ComTypeDescriptor : ICustomTypeDescriptor
        {
            private readonly ComNativeDescriptor _handler;
            private readonly object? _instance;

            /// <summary>
            ///  Creates a new WalkingTypeDescriptor.
            /// </summary>
            internal ComTypeDescriptor(ComNativeDescriptor handler, object? instance)
            {
                _handler = handler;
                _instance = instance;
            }

            AttributeCollection ICustomTypeDescriptor.GetAttributes() => _handler.GetAttributes(_instance);

            string? ICustomTypeDescriptor.GetClassName() => _instance is null ? string.Empty : GetClassName(_instance);

            string? ICustomTypeDescriptor.GetComponentName() => _instance is null ? string.Empty : GetName(_instance);

            [RequiresUnreferencedCode(AttributesRequiresUnreferencedCodeMessage)]
            TypeConverter ICustomTypeDescriptor.GetConverter() => GetConverter();

            [RequiresUnreferencedCode(EventDescriptorRequiresUnreferencedCodeMessage)]
            EventDescriptor? ICustomTypeDescriptor.GetDefaultEvent() => GetDefaultEvent();

            [RequiresUnreferencedCode(PropertyDescriptorPropertyTypeMessage)]
            PropertyDescriptor? ICustomTypeDescriptor.GetDefaultProperty()
                => _instance is null ? null : _handler.GetDefaultProperty(_instance);

            [RequiresUnreferencedCode(EditorRequiresUnreferencedCode)]
            object? ICustomTypeDescriptor.GetEditor(Type editorBaseType)
                => _instance is null ? null : GetEditor(_instance, editorBaseType);

            EventDescriptorCollection ICustomTypeDescriptor.GetEvents() => GetEvents();

            [RequiresUnreferencedCode(FilterRequiresUnreferencedCodeMessage)]
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[]? attributes) => GetEvents();

            [RequiresUnreferencedCode(PropertyDescriptorPropertyTypeMessage)]
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
                => _instance is null ? PropertyDescriptorCollection.Empty : _handler.GetProperties(_instance);

            [RequiresUnreferencedCode($"{PropertyDescriptorPropertyTypeMessage} {FilterRequiresUnreferencedCodeMessage}")]
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[]? attributes)
                => _instance is null ? PropertyDescriptorCollection.Empty : _handler.GetProperties(_instance);

            // Not much we can do here as we don't control the _instance being passed to us from the ComponentModel.
            object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor? pd) => _instance!;
        }
    }
}
