// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static partial class EnumExtensions
{
    extension(Enum)
    {
        /// <summary>
        ///  Returns a <see cref="bool"/> telling whether a given integral value, or its name as a string,
        ///  exists in a specified enumeration.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <param name="value">The value or name of a constant in <typeparamref name="TEnum"/>.</param>
        /// <returns>
        ///  <see langword="true"/> if a given integral value, or its name as a string, exists in a specified enumeration;
        ///  <see langword="false"/> otherwise.
        /// </returns>
        public static bool IsDefined<TEnum>(TEnum value) where TEnum : struct, Enum => Enum.IsDefined(typeof(TEnum), value);

        /// <summary>
        ///  Retrieves an array of the values of the constants in a specified enumeration type.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enumeration.</typeparam>
        /// <returns>An array that contains the values of the constants in <typeparamref name="TEnum"/>.</returns>
        public static TEnum[] GetValues<TEnum>() where TEnum : struct, Enum =>
            [.. Enum.GetValues(typeof(TEnum)).Cast<TEnum>()];
    }
}
