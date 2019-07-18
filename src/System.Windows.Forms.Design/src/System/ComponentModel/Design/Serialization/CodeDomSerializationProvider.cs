// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Resources;

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///  This is the serialization provider for all code dom serializers.
    /// </summary>
    internal sealed class CodeDomSerializationProvider : IDesignerSerializationProvider
    {
        /// <summary>
        ///  This will be called by the serialization manager when it
        ///  is trying to locate a serialzer for an object type.
        ///  If this serialization provider can provide a serializer
        ///  that is of the correct type, it should return it.
        ///  Otherwise, it should return null.
        /// </summary>
        object IDesignerSerializationProvider.GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
        {
            if (serializerType == typeof(CodeDomSerializer))
            {
                return GetCodeDomSerializer(manager, currentSerializer, objectType, serializerType);
            }
            else if (serializerType == typeof(MemberCodeDomSerializer))
            {
                return GetMemberCodeDomSerializer(manager, currentSerializer, objectType, serializerType);
            }
            else if (serializerType == typeof(TypeCodeDomSerializer))
            {
                return GetTypeCodeDomSerializer(manager, currentSerializer, objectType, serializerType);
            }

            return null; // don't understand this type of serializer.
        }

        /// <summary>
        ///  Returns a code dom serializer
        /// </summary>
        private object GetCodeDomSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
        {
            // If this isn't a serializer type we recognize, do nothing.  Also, if metadata specified
            // a custom serializer, then use it.
            if (currentSerializer != null)
            {
                return null;
            }

            // Null is a valid value that can be passed into GetSerializer.  It indicates
            // that the value we need to serialize is null, in which case we handle it
            // through the PrimitiveCodeDomSerializer.
            //
            if (objectType == null)
            {
                return PrimitiveCodeDomSerializer.Default;
            }

            // Support for components.
            if (typeof(IComponent).IsAssignableFrom(objectType))
            {
                return ComponentCodeDomSerializer.Default;
            }

            // We special case enums.  They do have instance descriptors, but we want
            // better looking code than the instance descriptor can provide for flags,
            // so we do it ourselves.
            if (typeof(Enum).IsAssignableFrom(objectType))
            {
                return EnumCodeDomSerializer.Default;
            }

            // We will provide a serializer for any intrinsic types.
            if (objectType.IsPrimitive || objectType.IsEnum || objectType == typeof(string))
            {
                return PrimitiveCodeDomSerializer.Default;
            }

            // And one for collections.
            if (typeof(ICollection).IsAssignableFrom(objectType))
            {
                return CollectionCodeDomSerializer.Default;
            }

            // And one for IContainer
            if (typeof(IContainer).IsAssignableFrom(objectType))
            {
                return ContainerCodeDomSerializer.Default;
            }

            // And one for resources
            if (typeof(ResourceManager).IsAssignableFrom(objectType))
            {
                return ResourceCodeDomSerializer.Default;
            }

            // The default serializer can do any object including those with instance descriptors.
            return CodeDomSerializer.Default;
        }

        /// <summary>
        ///  Returns a code dom serializer for members
        /// </summary>
        private object GetMemberCodeDomSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
        {
            // Don't provide our serializer if someone else already had one
            if (currentSerializer != null)
            {
                return null;
            }

            if (typeof(PropertyDescriptor).IsAssignableFrom(objectType))
            {
                return PropertyMemberCodeDomSerializer.Default;
            }

            if (typeof(EventDescriptor).IsAssignableFrom(objectType))
            {
                return EventMemberCodeDomSerializer.Default;
            }

            return null;
        }

        /// <summary>
        ///  Returns a code dom serializer for types
        /// </summary>
        private object GetTypeCodeDomSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType, Type serializerType)
        {
            // Don't provide our serializer if someone else already had one
            if (currentSerializer != null)
            {
                return null;
            }

            if (typeof(IComponent).IsAssignableFrom(objectType))
            {
                return ComponentTypeCodeDomSerializer.Default;
            }

            return TypeCodeDomSerializer.Default;
        }
    }
}
