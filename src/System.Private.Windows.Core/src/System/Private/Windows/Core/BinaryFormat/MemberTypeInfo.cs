// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace System.Private.Windows.Core.BinaryFormat;

/// <summary>
///  Member type info.
/// </summary>
/// <remarks>
///  <para>
///   <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/aa509b5a-620a-4592-a5d8-7e9613e0a03e">
///    [MS-NRBF] 2.3.1.2
///   </see>
///  </para>
/// </remarks>
[DebuggerDisplay("{DebuggerDisplay,nq}")]
internal readonly struct MemberTypeInfo
{
    internal BinaryType Type { get; init; }
    internal object? Info { get; init; }

    internal MemberTypeInfo(BinaryType type, object? info)
    {
        Type = type;
        Info = info;
    }

    internal static MemberTypeInfo Parse(BinaryReader reader)
    {
        BinaryType type = (BinaryType)reader.ReadByte();
        return new(type, GetInfo(reader, type));
    }

    [SkipLocalsInit]
    internal static IReadOnlyList<MemberTypeInfo> Parse(BinaryReader reader, int expectedCount)
    {
        // Expected count is guarded by the member parser.
        MemberTypeInfo[] info = new MemberTypeInfo[expectedCount];

        using BufferScope<byte> buffer = new(stackalloc byte[64], expectedCount);

        if (reader.Read(buffer.Slice(0, expectedCount)) < expectedCount)
        {
            throw new EndOfStreamException();
        }

        // Check for more clarifying information
        for (int i = 0; i < expectedCount; i++)
        {
            BinaryType type = (BinaryType)buffer[i];
            info[i] = new(type, GetInfo(reader, type));
        }

        return info;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static object? GetInfo(BinaryReader reader, BinaryType type) => type switch
    {
        BinaryType.Primitive or BinaryType.PrimitiveArray => (PrimitiveType)reader.ReadByte(),
        BinaryType.SystemClass => reader.ReadString(),
        BinaryType.Class => ClassTypeInfo.Parse(reader),
        // Other types have no additional data.
        BinaryType.String or BinaryType.ObjectArray or BinaryType.StringArray or BinaryType.Object => null,
        _ => throw new SerializationException("Unexpected binary type."),
    };

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = """
                Incoming type names are coming off of the formatted stream. There is no way for user code to pass compile
                time context for preserialized data. If a type can't be found on deserialization it won't matter any more
                than any other case where the type can't be found (e.g. a missing assembly). The deserializer will fail
                with information on the missing type that can be used to attribute to keep said type.
                """)]
    internal static IReadOnlyList<MemberTypeInfo> CreateFromClassInfoAndLibrary(BinaryFormattedObject.IParseState state, ClassInfo classInfo, Id libraryId)
    {
        Type type = state.TypeResolver.GetType(classInfo.Name, libraryId);

        if (typeof(ISerializable).IsAssignableFrom(type)
            || state.Options.SurrogateSelector?.GetSurrogate(type, state.Options.StreamingContext, out _) is not null)
        {
            throw new SerializationException("Cannot intuit type information for ISerializable types.");
        }

#pragma warning disable SYSLIB0050 // Type or member is obsolete
        MemberInfo[] memberInfo = FormatterServices.GetSerializableMembers(type);
#pragma warning restore SYSLIB0050

        IReadOnlyList<string> memberNames = classInfo.MemberNames;
        MemberTypeInfo[] info = new MemberTypeInfo[memberInfo.Length];

        for (int i = 0; i < memberNames.Count; i++)
        {
            // FormatterServices never returns anything other than FieldInfo.
            FieldInfo? field = (FieldInfo?)memberInfo.FirstOrDefault(m => m.Name == memberNames[i])
                ?? throw new SerializationException($"Could not find member '{memberNames[i]}' on type '{classInfo.Name}'.");

            BinaryType binaryType = TypeInfo.GetBinaryType(field.FieldType);

            info[i] = new(binaryType, binaryType switch
            {
                BinaryType.PrimitiveArray => TypeInfo.GetPrimitiveArrayType(field.FieldType),
                BinaryType.Primitive => TypeInfo.GetPrimitiveType(field.FieldType),
                BinaryType.SystemClass => field.FieldType,
                BinaryType.Class => field.FieldType,
                _ => null
            });
        }

        return info;
    }

    private string DebuggerDisplay => Info switch
    {
        null => Type.ToString()!,
        Type type => type.Name,
        PrimitiveType primitive => primitive.ToString()!,
        ClassTypeInfo info => info.ToString()!,
        string typeName => typeName,
        _ => throw new InvalidOperationException()
    };
}
