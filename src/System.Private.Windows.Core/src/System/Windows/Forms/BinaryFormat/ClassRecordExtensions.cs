// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System.Windows.Forms.BinaryFormat;

internal static class ClassRecordExtensions
{
    private static bool IsPrimitiveType(this SystemClassWithMembersAndTypes systemClass) =>
        systemClass.IsPrimitiveTypeClassName() && systemClass.MemberTypeInfo[0].Type == BinaryType.Primitive;

    private static bool IsPrimitiveTypeClassName(this SystemClassWithMembersAndTypes systemClass) => TypeInfo.GetPrimitiveType(systemClass.Name) switch
    {
        PrimitiveType.Boolean => true,
        PrimitiveType.Byte => true,
        PrimitiveType.Char => true,
        PrimitiveType.Double => true,
        PrimitiveType.Int32 => true,
        PrimitiveType.Int64 => true,
        PrimitiveType.SByte => true,
        PrimitiveType.Single => true,
        PrimitiveType.Int16 => true,
        PrimitiveType.UInt16 => true,
        PrimitiveType.UInt32 => true,
        PrimitiveType.UInt64 => true,
        _ => false,
    };

    internal static bool TryGetSystemPrimitive(this SystemClassWithMembersAndTypes systemClass, [NotNullWhen(true)] out object? value)
    {
        value = null;

        if (systemClass.IsPrimitiveType())
        {
            value = systemClass.MemberValues[0];
            Debug.Assert(value is not null);
            return true;
        }

        if (systemClass.Name == typeof(TimeSpan).FullName)
        {
            value = new TimeSpan((long)systemClass.MemberValues[0]!);
            return true;
        }

        switch (systemClass.Name)
        {
            case TypeInfo.TimeSpanType:
                value = new TimeSpan((long)systemClass.MemberValues[0]!);
                return true;
            case TypeInfo.DateTimeType:
                ulong ulongValue = (ulong)systemClass["dateData"]!;
                value = Unsafe.As<ulong, DateTime>(ref ulongValue);
                return true;
            case TypeInfo.DecimalType:
                ReadOnlySpan<int> bits =
                [
                    (int)systemClass["lo"]!,
                    (int)systemClass["mid"]!,
                    (int)systemClass["hi"]!,
                    (int)systemClass["flags"]!
                ];

                value = new decimal(bits);
                return true;
            case TypeInfo.IntPtrType:
                // Rehydrating still throws even though casting doesn't any more
                value = checked((nint)(long)systemClass.MemberValues[0]!);
                return true;
            case TypeInfo.UIntPtrType:
                value = checked((nuint)(ulong)systemClass.MemberValues[0]!);
                return true;
            default:
                return false;
        }
    }
}
