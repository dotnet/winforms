// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;

namespace System.Windows.Forms.BinaryFormat;

/// <summary>
///  Object model for the binary format put out by BinaryFormatter. It parses and creates a model but does not
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
internal sealed partial class BinaryFormattedObject
{
    // Don't reserve space in collections based on read lengths for more than this size to defend against corrupted lengths.
    internal const int MaxNewCollectionSize = 1024 * 10;

    private static readonly Options s_defaultOptions = new();

    private readonly List<IRecord> _records = [];
    private readonly RecordMap _recordMap = new();

    private readonly Options _options;
    private TypeResolver? _typeResolver;

    /// <summary>
    ///  Creates <see cref="BinaryFormattedObject"/> by parsing <paramref name="stream"/>.
    /// </summary>
    public BinaryFormattedObject(Stream stream, Options? options = null)
    {
        ArgumentNullException.ThrowIfNull(stream);
        using BinaryReader reader = new(stream, Encoding.UTF8, leaveOpen: true);

        _options = options ?? s_defaultOptions;

        ParseState state = new(reader, this);

        IRecord? currentRecord;
        do
        {
            try
            {
                currentRecord = Record.ReadBinaryFormatRecord(state);
            }
            catch (Exception ex) when (ex is ArgumentException or InvalidCastException or ArithmeticException or IOException)
            {
                // Make the exception easier to catch, but retain the original stack trace.
                throw ex.ConvertToSerializationException();
            }
            catch (TargetInvocationException ex)
            {
                throw ExceptionDispatchInfo.Capture(ex.InnerException!).SourceException.ConvertToSerializationException();
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
    ///  Gets a record by it's identifier. Not all records have identifiers, only ones that
    ///  can be referenced by other records.
    /// </summary>
    public IRecord this[Id id] => _recordMap[id];

    /// <summary>
    ///  Resolves the given type name against the specified library.
    /// </summary>
    /// <param name="libraryId">The library id, or <see cref="Id.Null"/> for the "system" assembly.</param>
    [RequiresUnreferencedCode("Calls System.Windows.Forms.BinaryFormat.BinaryFormattedObject.TypeResolver.GetType(String, Id)")]
    [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    internal Type GetType(string typeName, Id libraryId)
    {
        _typeResolver ??= new(_options, _recordMap);
        return _typeResolver.GetType(typeName, libraryId);
    }
}
