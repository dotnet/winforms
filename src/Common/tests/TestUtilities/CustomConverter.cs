// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System;

public static class CustomConverter
{
    /// <summary>
    ///  Registers a TypeConverter with the specified type.
    /// </summary>
    public static RegistrationScope RegisterConverter(Type type, TypeConverter converter)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(converter);

        TypeDescriptionProvider parentProvider = TypeDescriptor.GetProvider(type);
        CustomTypeDescriptionProvider newProvider = new(parentProvider, converter);
        TypeDescriptor.AddProvider(newProvider, type);
        return new RegistrationScope(type, newProvider);
    }

    /// <summary>
    ///  Manages the registration lifetime of a TypeConverter to a type.
    ///  This is meant to be utilized in a <see langword="using"/> statement to ensure
    ///  <see cref="TypeDescriptor.RemoveProvider(TypeDescriptionProvider, Type)"/> is called when going out of scope with the using.
    /// </summary>
    public readonly ref struct RegistrationScope
    {
        private readonly Type _type;
        private readonly TypeDescriptionProvider _provider;

        public RegistrationScope(Type type, TypeDescriptionProvider provider)
        {
            _type = type;
            _provider = provider;
        }

        public void Dispose() => TypeDescriptor.RemoveProvider(_provider, _type);
    }

    public class CustomTypeDescriptionProvider : TypeDescriptionProvider
    {
        private readonly TypeConverter _converter;

        public CustomTypeDescriptionProvider(TypeDescriptionProvider parent, TypeConverter converter) : base(parent)
            => _converter = converter;

        public override ICustomTypeDescriptor GetTypeDescriptor(
            [DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] Type objectType,
            object? instance) => new TypeConverterProvider(base.GetTypeDescriptor(objectType, instance), _converter);

        private sealed class TypeConverterProvider : CustomTypeDescriptor
        {
            private readonly TypeConverter _converter;

            public TypeConverterProvider(ICustomTypeDescriptor? parent, TypeConverter converter) : base(parent)
                => _converter = converter;

            public override TypeConverter GetConverter() => _converter;
        }
    }
}
