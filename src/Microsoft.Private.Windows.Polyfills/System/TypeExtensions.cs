// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static partial class TypeExtensions
{
    extension(Type type)
    {
        /// <summary>
        ///  Determines whether the current type can be assigned to a variable of the specified <paramref name="targetType"/>.
        /// </summary>
        /// <param name="targetType">The target type to check assignability against.</param>
        /// <returns>
        ///  <see langword="true"/> if the current type is assignable to <paramref name="targetType"/>;
        ///  otherwise, <see langword="false"/>.
        /// </returns>
        public bool IsAssignableTo(Type targetType) => targetType?.IsAssignableFrom(type) ?? false;

        /// <summary>
        ///  Gets a value that indicates whether the type is a type definition.
        /// </summary>
        public bool IsTypeDefinition => !type.IsArray
            && !type.IsByRef
            && !type.IsPointer
            && !type.IsConstructedGenericType
            && !type.IsGenericParameter;
    }
}
