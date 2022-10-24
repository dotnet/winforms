// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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
            private readonly object _instance;

            /// <summary>
            ///  Creates a new WalkingTypeDescriptor.
            /// </summary>
            internal ComTypeDescriptor(ComNativeDescriptor handler, object instance)
            {
                _handler = handler;
                _instance = instance;
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            AttributeCollection ICustomTypeDescriptor.GetAttributes()
            {
                return _handler.GetAttributes(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            string ICustomTypeDescriptor.GetClassName()
            {
                return GetClassName(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            string ICustomTypeDescriptor.GetComponentName()
            {
                return GetName(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            [RequiresUnreferencedCode(AttributesRequiresUnreferencedCodeMessage)]
            TypeConverter ICustomTypeDescriptor.GetConverter()
            {
                return GetConverter();
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            [RequiresUnreferencedCode(EventDescriptorRequiresUnreferencedCodeMessage)]
            EventDescriptor ICustomTypeDescriptor.GetDefaultEvent()
            {
                return GetDefaultEvent();
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            [RequiresUnreferencedCode(PropertyDescriptorPropertyTypeMessage)]
            PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty()
            {
                return _handler.GetDefaultProperty(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            [RequiresUnreferencedCode(EditorRequiresUnreferencedCode)]
            object ICustomTypeDescriptor.GetEditor(Type editorBaseType)
            {
                return GetEditor(_instance, editorBaseType);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents()
            {
                return GetEvents();
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            [RequiresUnreferencedCode(FilterRequiresUnreferencedCodeMessage)]
            EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes)
            {
                return GetEvents();
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            [RequiresUnreferencedCode(PropertyDescriptorPropertyTypeMessage)]
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties()
            {
                return _handler.GetProperties(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            [RequiresUnreferencedCode($"{PropertyDescriptorPropertyTypeMessage} {FilterRequiresUnreferencedCodeMessage}")]
            PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes)
            {
                return _handler.GetProperties(_instance);
            }

            /// <summary>
            ///  ICustomTypeDescriptor implementation.
            /// </summary>
            object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd)
            {
                return _instance;
            }
        }
    }
}
