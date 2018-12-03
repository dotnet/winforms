// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design.Serialization
{
    /// <summary>
    ///     This is the serialization provider for all code dom serializers.
    /// </summary>
    internal sealed class CodeDomSerializationProvider : IDesignerSerializationProvider
    {
        public object GetSerializer(IDesignerSerializationManager manager, object currentSerializer, Type objectType,
            Type serializerType)
        {
            throw new NotImplementedException();
        }
    }
}
