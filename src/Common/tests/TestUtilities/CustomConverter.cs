// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System;

public static class CustomConverter
{
    /// <summary>
    ///  Registers a TypeConverter with the specified type.
    /// </summary>
    public static RegistrationScope RegisterConverter(Type type, TypeConverter converter)
    {
        TypeDescriptionProvider parentProvider = TypeDescriptor.GetProvider(type);
        TypeDescriptionProvider newProvider = new CustomTypeDescriptionProvider(parentProvider, converter);
        TypeDescriptor.AddProvider(newProvider, type);
        return new RegistrationScope(type, newProvider);
    }

    /// <summary>
    ///  Manages the registration lifetime of a TypeConverter to a type.
    ///  This is meant to be utilized in a <see langword="using"/> statement to ensure
    ///  <see cref="TypeDescriptor.RemoveProvider(TypeDescriptionProvider, Type)"/> is called when going out of scope with the using.
    /// </summary>
    public ref struct RegistrationScope
    {
        private readonly Type _type;
        private readonly TypeDescriptionProvider _provider;
        private TypeConverter _converter;

        public RegistrationScope(Type type, TypeDescriptionProvider provider)
        {
            _type = type;
            _provider = provider;
            _converter = TypeDescriptor.GetConverter(type);
        }

        public TypeConverter Converter => _converter;

        public void Dispose()
        {
            TypeDescriptor.RemoveProvider(_provider, _type);
            _converter = null;
        }
    }

    public class CustomTypeDescriptionProvider : TypeDescriptionProvider
    {
        private TypeConverter _converter;

        public CustomTypeDescriptionProvider(TypeDescriptionProvider parent, TypeConverter converter) : base(parent)
            => _converter = converter;

        public override ICustomTypeDescriptor GetTypeDescriptor(
            [DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] Type objectType,
            object instance) => new TypeConverterProvider(base.GetTypeDescriptor(objectType, instance), _converter);

        private class TypeConverterProvider : CustomTypeDescriptor
        {
            private static TypeConverter _converter;

            public TypeConverterProvider(ICustomTypeDescriptor parent, TypeConverter converter) : base(parent)
                => _converter = converter;

            public override TypeConverter GetConverter() => _converter;
        }
    }
}
