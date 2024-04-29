// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat;

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
internal class MemberTypeInfo : IBinaryWriteable, IEnumerable<(BinaryType Type, object? Info)>
{
    private readonly IList<(BinaryType Type, object? Info)> _info;

    public MemberTypeInfo(IList<(BinaryType Type, object? Info)> info) => _info = info;

    public MemberTypeInfo(params (BinaryType Type, object? Info)[] info) => _info = info;

    public (BinaryType Type, object? Info) this[int index] => _info[index];
    public int Count => _info.Count;

    public static MemberTypeInfo Parse(BinaryReader reader, Count expectedCount)
    {
        List<(BinaryType Type, object? Info)> info = new(expectedCount);

        // Get all of the BinaryTypes
        for (int i = 0; i < expectedCount; i++)
        {
            info.Add(((BinaryType)reader.ReadByte(), null));
        }

        // Check for more clarifying information

        for (int i = 0; i < expectedCount; i++)
        {
            BinaryType type = info[i].Type;
            switch (type)
            {
                case BinaryType.Primitive:
                case BinaryType.PrimitiveArray:
                    info[i] = (type, (PrimitiveType)reader.ReadByte());
                    break;
                case BinaryType.SystemClass:
                    info[i] = (type, reader.ReadString());
                    break;
                case BinaryType.Class:
                    info[i] = (type, ClassTypeInfo.Parse(reader));
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

        return new MemberTypeInfo(info);
    }

    [UnconditionalSuppressMessage(
        "Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = """
                Incoming type names are coming off of the formatted stream. There is no way for user code to pass compile
                time context for preserialized data. If a type can't be found on deserialization it won't matter any more
                than any other case where the type can't be found (e.g. a missing assembly). The deserializer will fail
                with information on the missing type that can be used to attribute to keep said type.
                """)]
    internal static MemberTypeInfo CreateFromClassInfoAndLibrary(BinaryFormattedObject.IParseState state, ClassInfo classInfo, Id libraryId)
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
        var info = new (BinaryType Type, object? Info)[memberInfo.Length];

        for (int i = 0; i < memberNames.Count; i++)
        {
            // FormatterServices never returns anything other than FieldInfo.
            FieldInfo? field = (FieldInfo?)memberInfo.FirstOrDefault(m => m.Name == memberNames[i])
                ?? throw new SerializationException($"Could not find member '{memberNames[i]}' on type '{classInfo.Name}'.");

            BinaryType binaryType = TypeInfo.GetBinaryType(field.FieldType);

            info[i] = (binaryType, binaryType switch
            {
                BinaryType.PrimitiveArray => TypeInfo.GetPrimitiveArrayType(field.FieldType),
                BinaryType.Primitive => TypeInfo.GetPrimitiveType(field.FieldType),
                BinaryType.SystemClass => field.FieldType,
                BinaryType.Class => field.FieldType,
                _ => null
            });
        }

        return new(info);
    }

    public void Write(BinaryWriter writer)
    {
        foreach ((BinaryType type, _) in this)
        {
            writer.Write((byte)type);
        }

        foreach ((BinaryType type, object? info) in this)
        {
            switch (type)
            {
                case BinaryType.Primitive:
                case BinaryType.PrimitiveArray:
                    writer.Write((byte)info!);
                    break;
                case BinaryType.SystemClass:
                    writer.Write((string)info!);
                    break;
                case BinaryType.Class:
                    ((ClassTypeInfo)info!).Write(writer);
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

    IEnumerator<(BinaryType Type, object? Info)> IEnumerable<(BinaryType Type, object? Info)>.GetEnumerator()
        => _info.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _info.GetEnumerator();
}
