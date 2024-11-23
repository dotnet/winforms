// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.CompilerServices;

namespace Xunit;

/// <summary>
///  Generates <see cref="TheoryAttribute"/> data for an invalid enum value.
/// </summary>
public class InvalidEnumDataAttribute<TEnum> : CommonMemberDataAttribute where TEnum : unmanaged, Enum
{
    public InvalidEnumDataAttribute() : base(typeof(InvalidEnumDataAttribute<TEnum>)) { }

    public static ReadOnlyTheoryData TheoryData { get; } = InitializeData();

    private static unsafe ReadOnlyTheoryData InitializeData()
    {
        ulong maxValue = ulong.MaxValue >>> ((sizeof(ulong) - sizeof(TEnum)) * 8);
        TEnum currentValue;
        bool defined;

        List<TEnum> data = [];

        if (typeof(TEnum).GetCustomAttribute<FlagsAttribute>() is not null)
        {
            // Bit flags, pull the first flag from the top.
            ulong currentFlagValue = 1ul << (sizeof(TEnum) * 8) - 1;
            do
            {
                currentValue = Unsafe.As<ulong, TEnum>(ref currentFlagValue);
                defined = Enum.IsDefined(currentValue);
                currentFlagValue >>>= 1;
            }
            while (defined && currentFlagValue > 0);

            if (defined)
            {
                throw new InvalidOperationException("Enum has all flags defined");
            }

            data.Add(currentValue);
            return new(data);
        }

        // Not a flags enum, add the smallest and largest undefined value.
        ulong minValue = 0;

        do
        {
            currentValue = Unsafe.As<ulong, TEnum>(ref minValue);
            defined = Enum.IsDefined(currentValue);
            minValue++;
        }
        while (defined);

        data.Add(currentValue);

        do
        {
            currentValue = Unsafe.As<ulong, TEnum>(ref maxValue);
            defined = Enum.IsDefined(currentValue);
            maxValue--;
        }
        while (defined);

        data.Add(currentValue);
        return new(data);
    }
}
