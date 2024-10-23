// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Private.Windows.Core.BinaryFormat.Serializer;

namespace System.IO;

internal static class BinaryWriterExtensions
{
    /// <summary>
    ///  Writes a <see cref="DateTime"/> object to the given <paramref name="writer"/>.
    /// </summary>
    public static void Write(this BinaryWriter writer, DateTime value)
    {
        // Copied from System.Runtime.Serialization.Formatters.Binary.BinaryFormatterWriter

        // In .NET Framework, BinaryFormatter is able to access DateTime's ToBinaryRaw,
        // which just returns the value of its sole Int64 dateData field. Here, we don't
        // have access to that member (which doesn't even exist anymore, since it was only for
        // BinaryFormatter, which is now in a separate assembly). To address that,
        // we access the sole field directly via an unsafe cast.
        long dateData = Unsafe.As<DateTime, long>(ref value);
        writer.Write(dateData);
    }

    /// <summary>
    ///  Writes <see cref="MemberTypeInfo"/>.
    /// </summary>
    internal static void Write(this BinaryWriter writer, IReadOnlyList<MemberTypeInfo> memberTypeInfo)
    {
        for (int i = 0; i < memberTypeInfo.Count; i++)
        {
            writer.Write((byte)memberTypeInfo[i].Type);
        }

        for (int i = 0; i < memberTypeInfo.Count; i++)
        {
            switch (memberTypeInfo[i].Type)
            {
                case BinaryType.Primitive:
                case BinaryType.PrimitiveArray:
                    writer.Write((byte)memberTypeInfo[i].Info!);
                    break;
                case BinaryType.SystemClass:
                    writer.Write((string)memberTypeInfo[i].Info!);
                    break;
                case BinaryType.Class:
                    ((ClassTypeInfo)memberTypeInfo[i].Info!).Write(writer);
                    break;
                case BinaryType.String:
                case BinaryType.ObjectArray:
                case BinaryType.StringArray:
                case BinaryType.Object:
                    // Other types have no additional data.
                    break;
                default:
                    throw new SerializationException("Unexpected binary type.");
            }
        }
    }
}
