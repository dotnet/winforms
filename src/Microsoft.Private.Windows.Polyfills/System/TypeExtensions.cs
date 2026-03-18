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

        /// <summary>
        ///  Determines whether the given <paramref name="type"/> is a single-dimensional, zero-indexed (SZ) array,
        ///  equivalent to Type.IsSZArray which is not available on .NET Framework.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   The CLR distinguishes two kinds of array types at the metadata level:
        ///  </para>
        ///  <list type="bullet">
        ///   <item><c>ELEMENT_TYPE_SZARRAY</c><description> the "vector" type (e.g. <c>int[]</c>), single-dimensional
        ///   and always zero-indexed.</description></item>
        ///   <item><c>ELEMENT_TYPE_ARRAY</c><description> with rank 1 — a general array that happens to have one dimension
        ///   (e.g. <c>int[*]</c>), which can have a non-zero lower bound.</description></item>
        ///  </list>
        ///  <para>
        ///   Both return <see langword="true"/> for <see cref="Type.IsArray"/> and <c>1</c> for
        ///   <see cref="Type.GetArrayRank"/>, so those properties alone cannot distinguish them.
        ///  </para>
        ///  <para>
        ///   The trick here exploits <see cref="Type.MakeArrayType()"/> (parameterless), which always
        ///   produces the SZ array type for the element type. If the resulting type is reference-equal
        ///   to the original, the original is an SZ array. If it is not (as would be the case for
        ///   <c>int[*]</c>, whose <c>MakeArrayType()</c> yields <c>int[]</c>), the original is a
        ///   general rank-1 array. The comparison resolves to a <c>RuntimeTypeHandle</c> identity
        ///   check on cached <see cref="Type"/> objects — no reflection metadata walking or string
        ///   parsing is involved.
        ///  </para>
        /// </remarks>
        public bool IsSZArray =>
            type.IsArray && type.GetElementType()!.MakeArrayType() == type;
    }
}
