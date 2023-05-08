// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Object model for the binary format put out by BinaryFormatter. It parses andcreates a model but does not
///  instantiate any reference types outside of string.
/// </summary>
/// <remarks>
///  <para>
///   This is useful for explicitly controlling the rehydration of binary formatted data. BinaryFormatter is
///   depreciated for security concerns (it has no way to constrain what it hydrates from an incoming stream).
///  </para>
///  <para>
///   NOTE: Multidimensional and jagged arrays are not yet implemented.
///  </para>
/// </remarks>
internal sealed class BinaryFormattedObject
{
    private static readonly string[] s_systemPrimitiveTypeNames = new string[]
    {
        typeof(int).FullName!,
        typeof(long).FullName!,
        typeof(bool).FullName!,
        typeof(char).FullName!,
        typeof(float).FullName!,
        typeof(double).FullName!,
        typeof(sbyte).FullName!,
        typeof(byte).FullName!,
        typeof(short).FullName!,
        typeof(ushort).FullName!,
        typeof(uint).FullName!,
        typeof(ulong).FullName!,
    };

    // Don't reserve space in collections based on read lengths for more than this size to defend against corrupted lengths.
#if DEBUG
    internal const int MaxNewCollectionSize = 1024 * 10;
#else
    internal const int MaxNewCollectionSize = 10;
#endif

    private readonly List<IRecord> _records = new();
    private readonly RecordMap _recordMap = new();

    /// <summary>
    ///  Creates <see cref="BinaryFormattedObject"/> by parsing <paramref name="stream"/>.
    /// </summary>
    /// <param name="leaveOpen"></param>
    public BinaryFormattedObject(Stream stream, bool leaveOpen = false)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using BinaryReader reader = new(stream, Encoding.UTF8, leaveOpen: leaveOpen);

        IRecord? currentRecord;
        do
        {
            try
            {
                currentRecord = Record.ReadBinaryFormatRecord(reader, _recordMap);
            }
            catch (SerializationException)
            {
                throw;
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidCastException or ArithmeticException or IOException)
            {
                // Make the exception easier to catch, but retain the original stack trace.
                throw ex.ConvertToSerializationException();
            }

            _records.Add(currentRecord);
        }
        while (currentRecord is not MessageEnd);
    }

    /// <summary>
    ///  Total count of top-level records.
    /// </summary>
    public int RecordCount => _records.Count;

    /// <summary>
    ///  Gets a record by it's index.
    /// </summary>
    public IRecord this[int index] => _records[index];

    /// <summary>
    ///  Gets a record by it's identfier. Not all records have identifiers, only ones that
    ///  can be referenced by other records.
    /// </summary>
    public IRecord this[Id id] => _recordMap[id];

    /// <summary>
    ///  Trys to get this object as a primitive type or string.
    /// </summary>
    /// <returns><see langword="true"/> if this represented a primitive type or string.</returns>
    public bool TryGetPrimitiveType([NotNullWhen(true)] out object? value)
    {
        try
        {
            return TryGetPrimitiveTypeInternal(out value);
        }
        catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
        {
            value = default;
            return false;
        }
    }

    private bool TryGetPrimitiveTypeInternal([NotNullWhen(true)] out object? value)
    {
        value = null;
        if (RecordCount != 3)
        {
            return false;
        }

        if (_records[1] is BinaryObjectString binaryString)
        {
            value = binaryString.Value;
            return true;
        }

        if (_records[1] is not SystemClassWithMembersAndTypes systemClass)
        {
            return false;
        }

        // Basic primitive types
        if (s_systemPrimitiveTypeNames.Contains(systemClass.Name)
            && systemClass.MemberTypeInfo[0].Type == BinaryType.Primitive)
        {
            value = systemClass.MemberValues[0];
            return true;
        }

        // Handle decimal, nint, nuint, TimeSpan, DateTime

        if (systemClass.Name == typeof(TimeSpan).FullName)
        {
            value = new TimeSpan((long)systemClass.MemberValues[0]);
            return true;
        }

        if (systemClass.Name == typeof(DateTime).FullName)
        {
            value = BinaryFormatReader.CreateDateTimeFromData((long)systemClass["dateData"]);
            return true;
        }

        if (systemClass.Name == typeof(nint).FullName)
        {
            // Rehydrating still throws even though casting doesn't any more
            value = checked((nint)(long)systemClass.MemberValues[0]);
            return true;
        }

        if (systemClass.Name == typeof(nuint).FullName)
        {
            value = checked((nuint)(ulong)systemClass.MemberValues[0]);
            return true;
        }

        if (systemClass.Name == typeof(decimal).FullName)
        {
            Span<int> bits = stackalloc int[4]
            {
                (int)systemClass["lo"],
                (int)systemClass["mid"],
                (int)systemClass["hi"],
                (int)systemClass["flags"]
            };

            value = new decimal(bits);
            return true;
        }

        return false;
    }
}
