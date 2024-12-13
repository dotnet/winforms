// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Formats.Nrbf;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Windows.Forms.Nrbf;
using System.Windows.Forms.BinaryFormat;

namespace System.Resources;

public sealed class ResXDataNode : ISerializable
{
    private static readonly char[] s_specialChars = [' ', '\r', '\n'];

    private DataNodeInfo? _nodeInfo;

    private string? _name;
    private string? _comment;

    private string? _typeName; // is only used when we create a resxdatanode manually with an object and contains the FQN

    private string? _fileRefFullPath;
    private string? _fileRefType;
    private string? _fileRefTextEncoding;

    private object? _value;
    private ResXFileRef? _fileRef;

    [Obsolete(DiagnosticId = "SYSLIB0051")]
    private BinaryFormatter? _binaryFormatter;

    // This is going to be used to check if a ResXDataNode is of type ResXFileRef
    private static readonly AssemblyNamesTypeResolutionService s_internalTypeResolver = new([new("System.Windows.Forms")]);

    // Callback function to get type name for multitargeting.
    // No public property to force using constructors for the following reasons:
    // 1. one of the constructors needs this field (if used) to initialize the object, make it consistent with the
    //    other constructors to avoid errors.
    // 2. once the object is constructed the delegate should not be changed to avoid getting inconsistent results.
    private Func<Type?, string>? _typeNameConverter;

    private ResXDataNode()
    {
    }

    internal ResXDataNode DeepClone()
    {
        return new ResXDataNode()
        {
            // Nodeinfo is just made up of immutable objects, we don't need to clone it
            _nodeInfo = _nodeInfo?.Clone(),
            _name = _name,
            _comment = _comment,
            _typeName = _typeName,
            _fileRefFullPath = _fileRefFullPath,
            _fileRefType = _fileRefType,
            _fileRefTextEncoding = _fileRefTextEncoding,
            // We don't clone the value, because we don't know how
            _value = _value,
            _fileRef = _fileRef?.Clone(),
            _typeNameConverter = _typeNameConverter
        };
    }

    public ResXDataNode(string name, object? value) : this(name, value, typeNameConverter: null)
    {
    }

    public ResXDataNode(string name, object? value, Func<Type?, string>? typeNameConverter)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentException.ThrowIfNullOrEmpty(name);

        _typeNameConverter = typeNameConverter;

        Type valueType = (value is null) ? typeof(object) : value.GetType();

#pragma warning disable SYSLIB0050 // Type or member is obsolete
        if (value is not null && !valueType.IsSerializable)
        {
            throw new InvalidOperationException(string.Format(SR.NotSerializableType, name, valueType.FullName));
        }
#pragma warning restore SYSLIB0050

        if (value is not null)
        {
            _typeName = MultitargetUtil.GetAssemblyQualifiedName(valueType, _typeNameConverter);
        }

        _name = name;
        _value = value;
    }

    public ResXDataNode(string name, ResXFileRef fileRef) : this(name, fileRef, typeNameConverter: null)
    {
    }

    public ResXDataNode(string name, ResXFileRef fileRef, Func<Type?, string>? typeNameConverter)
    {
        _name = name.OrThrowIfNullOrEmpty();
        _fileRef = fileRef.OrThrowIfNull();
        _typeNameConverter = typeNameConverter;
    }

    internal ResXDataNode(DataNodeInfo nodeInfo, string? basePath)
    {
        _nodeInfo = nodeInfo;
        _name = nodeInfo.Name;

        // We can only use our internal type resolver here because we only want to check if this is a ResXFileRef
        // node and we can't be sure that we have a typeResolutionService that can recognize this.
        Type? nodeType = null;

        // Default for string is TypeName == null
        if (!string.IsNullOrEmpty(_nodeInfo.TypeName))
        {
            nodeType = s_internalTypeResolver.GetType(_nodeInfo.TypeName, throwOnError: false, ignoreCase: true);
        }

        if (nodeType is not null && nodeType.Equals(typeof(ResXFileRef)))
        {
            // We have a fileref, split the value data and populate the fields.
            string[] fileRefDetails = ResXFileRef.Converter.ParseResxFileRefString(_nodeInfo.ValueData);
            if (fileRefDetails is not null && fileRefDetails.Length > 1)
            {
                _fileRefFullPath = !Path.IsPathRooted(fileRefDetails[0]) && basePath is not null
                    ? Path.Combine(basePath, fileRefDetails[0])
                    : fileRefDetails[0];

                _fileRefType = fileRefDetails[1];
                if (fileRefDetails.Length > 2)
                {
                    _fileRefTextEncoding = fileRefDetails[2];
                }
            }
        }
    }

    [AllowNull]
    public string Comment
    {
        get => _comment ?? _nodeInfo?.Comment ?? string.Empty;
        set => _comment = value;
    }

    public string Name
    {
        get
        {
            Debug.Assert(_name is not null || _nodeInfo?.Name is not null);
            return _name ?? _nodeInfo?.Name ?? string.Empty;
        }
        set
        {
            ArgumentException.ThrowIfNullOrEmpty(value);
            _name = value;
        }
    }

    public ResXFileRef? FileRef
    {
        get
        {
            if (FileRefFullPath is null)
            {
                return null;
            }

            Debug.Assert(FileRefType is not null);

            return _fileRef ??= string.IsNullOrEmpty(FileRefTextEncoding)
                ? new ResXFileRef(FileRefFullPath, FileRefType!)
                : new ResXFileRef(FileRefFullPath, FileRefType!, Encoding.GetEncoding(FileRefTextEncoding));
        }
    }

    private string? FileRefFullPath => _fileRef?.FileName ?? _fileRefFullPath;

    private string? FileRefType => _fileRef?.TypeName ?? _fileRefType;

    private string? FileRefTextEncoding => _fileRef?.TextFileEncoding?.BodyName ?? _fileRefTextEncoding;

    private static string ToBase64WrappedString(byte[] data)
    {
        const int lineWrap = 80;
        const string prefix = "        ";
        string raw = Convert.ToBase64String(data);
        if (raw.Length > lineWrap)
        {
            // Word wrap on lineWrap chars, \r\n
            StringBuilder output = new(raw.Length + (raw.Length / lineWrap) * 3);
            int current = 0;
            for (; current < raw.Length - lineWrap; current += lineWrap)
            {
                output.AppendLine();
                output.Append(prefix);
                output.Append(raw, current, lineWrap);
            }

            output.AppendLine();
            output.Append(prefix);
            output.Append(raw, current, raw.Length - current);
            output.AppendLine();
            return output.ToString();
        }

        return raw;
    }

    private void FillDataNodeInfoFromObject(DataNodeInfo nodeInfo, object? value)
    {
        if (value is CultureInfo cultureInfo)
        {
            // Special-case CultureInfo, cannot use CultureInfoConverter for serialization.
            nodeInfo.ValueData = cultureInfo.Name;
            nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(CultureInfo), _typeNameConverter);
            return;
        }
        else if (value is string @string)
        {
            nodeInfo.ValueData = @string;
            return;
        }
        else if (value is byte[] bytes)
        {
            nodeInfo.ValueData = ToBase64WrappedString(bytes);
            nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(byte[]), _typeNameConverter);
            return;
        }

        Type valueType = value?.GetType() ?? typeof(object);
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        if (value is not null && !valueType.IsSerializable)
        {
            throw new InvalidOperationException(string.Format(SR.NotSerializableType, _name, valueType.FullName));
        }
#pragma warning restore SYSLIB0050

        TypeConverter converter = TypeDescriptor.GetConverter(valueType);

        try
        {
            // Can round trip through string.
            if (converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
            {
                nodeInfo.ValueData = converter.ConvertToInvariantString(value) ?? string.Empty;
                nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(valueType, _typeNameConverter);
                return;
            }
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            // Some custom type converters will throw in ConvertTo(string) to indicate that the object should
            // be serialized through ISerializable instead of as a string. This is semi-wrong, but something we
            // will have to live with to allow user created Cursors to be serializable.
        }

        if (converter.CanConvertTo(typeof(byte[])) && converter.CanConvertFrom(typeof(byte[])))
        {
            // Can round trip through byte[]
            byte[]? data = (byte[]?)converter.ConvertTo(value, typeof(byte[]));
            nodeInfo.ValueData = data is null ? string.Empty : ToBase64WrappedString(data);
            nodeInfo.MimeType = ResXResourceWriter.ByteArraySerializedObjectMimeType;
            nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(valueType, _typeNameConverter);
            return;
        }

        if (value is null)
        {
            nodeInfo.ValueData = string.Empty;
            nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(ResXNullRef), _typeNameConverter);
            return;
        }

#pragma warning disable SYSLIB0051 // Type or member is obsolete
        SerializeWithBinaryFormatter(_binaryFormatter, nodeInfo, value, _typeNameConverter);
#pragma warning restore SYSLIB0051

#pragma warning disable SYSLIB0011 // Type or member is obsolete
        static void SerializeWithBinaryFormatter(
            IFormatter? binaryFormatter,
            DataNodeInfo nodeInfo,
            object value,
            Func<Type?, string>? typeNameConverter)
        {
            binaryFormatter ??= new BinaryFormatter
            {
                Binder = new ResXSerializationBinder(typeNameConverter)
            };

            using (MemoryStream stream = new())
            {
                bool success = false;
                try
                {
                    success = WinFormsBinaryFormatWriter.TryWriteObject(stream, value);
                }
                catch (Exception ex) when (!ex.IsCriticalException())
                {
                    // Being extra cautious here, but the Try method above should never throw in normal circumstances.
                    Debug.Fail($"Unexpected exception writing binary formatted data. {ex.Message}");
                }

                if (!success)
                {
                    stream.SetLength(0);
                    binaryFormatter.Serialize(stream, value);
                }

                nodeInfo.ValueData = ToBase64WrappedString(stream.ToArray());
            }

            nodeInfo.MimeType = ResXResourceWriter.DefaultSerializedObjectMimeType;
        }
#pragma warning restore SYSLIB0011
    }

    private object? GenerateObjectFromDataNodeInfo(DataNodeInfo dataNodeInfo, ITypeResolutionService? typeResolver)
    {
        string? mimeTypeName = dataNodeInfo.MimeType;

        // Default behavior: if we don't have a type name, it's a string.
        string? typeName = string.IsNullOrEmpty(dataNodeInfo.TypeName)
            ? MultitargetUtil.GetAssemblyQualifiedName(typeof(string), _typeNameConverter)
            : dataNodeInfo.TypeName;

        if (!string.IsNullOrEmpty(mimeTypeName))
        {
            // Handle application/x-microsoft.net.object.bytearray.base64.
            return ResolveMimeType(mimeTypeName);
        }

        if (string.IsNullOrEmpty(typeName))
        {
            // If mimeTypeName and typeName are not filled in, the value must be a string.
            Debug.Assert(_value is string, "Resource entries with no Type or MimeType must be encoded as strings");
            return null;
        }

        Type type = ResolveTypeName(typeName);

        if (type == typeof(ResXNullRef))
        {
            return null;
        }

        if (type == typeof(byte[])
            || (typeName.Contains("System.Byte[]") && (typeName.Contains("mscorlib") || typeName.Contains("System.Private.CoreLib"))))
        {
            // Handle byte[]'s, which are stored as base-64 encoded strings. We can't hard-code byte[] type
            // name due to version number updates & potential whitespace issues with ResX files.
            return FromBase64WrappedString(dataNodeInfo.ValueData);
        }

        TypeConverter converter = TypeDescriptor.GetConverter(type);
        if (!converter.CanConvertFrom(typeof(string)))
        {
            Debug.WriteLine($"Converter for {type.FullName} doesn't support string conversion");
            return null;
        }

        try
        {
            return converter.ConvertFromInvariantString(dataNodeInfo.ValueData);
        }
        catch (NotSupportedException nse)
        {
            string newMessage = string.Format(SR.NotSupported, typeName, dataNodeInfo.ReaderPosition.Y, dataNodeInfo.ReaderPosition.X, nse.Message);
            XmlException xml = new(newMessage, nse, dataNodeInfo.ReaderPosition.Y, dataNodeInfo.ReaderPosition.X);
            throw new NotSupportedException(newMessage, xml);
        }

        Type ResolveTypeName(string typeName)
        {
            if (ResolveType(typeName, typeResolver) is not Type type)
            {
                string newMessage = string.Format(
                    SR.TypeLoadException,
                    typeName,
                    dataNodeInfo.ReaderPosition.Y,
                    dataNodeInfo.ReaderPosition.X);

                throw new TypeLoadException(
                    newMessage,
                    new XmlException(newMessage, null, dataNodeInfo.ReaderPosition.Y, dataNodeInfo.ReaderPosition.X));
            }

            return type;
        }

        object? ResolveMimeType(string mimeTypeName)
        {
            if (string.Equals(mimeTypeName, ResXResourceWriter.ByteArraySerializedObjectMimeType)
                && !string.IsNullOrEmpty(typeName)
                && TypeDescriptor.GetConverter(ResolveTypeName(typeName)) is { } converter
                && converter.CanConvertFrom(typeof(byte[]))
                && FromBase64WrappedString(dataNodeInfo.ValueData) is { } serializedData)
            {
                return converter.ConvertFrom(serializedData);
            }

            return null;
        }
    }

    [Obsolete(DiagnosticId = "SYSLIB0051")]
    private object? GenerateObjectFromBinaryDataNodeInfo(DataNodeInfo dataNodeInfo, ITypeResolutionService? typeResolver)
    {
        if (!string.Equals(dataNodeInfo.MimeType, ResXResourceWriter.BinSerializedObjectMimeType))
        {
            return null;
        }

        byte[] serializedData = FromBase64WrappedString(dataNodeInfo.ValueData);

        if (serializedData.Length <= 0)
        {
            return null;
        }

        using MemoryStream stream = new(serializedData, writable: false);

        try
        {
            SerializationRecord rootRecord = stream.Decode();
            if (rootRecord.TryGetResXObject(out object? value))
            {
                return value;
            }
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            // Being extra cautious here, but the Try method above should never throw in normal circumstances.
            Debug.Fail($"Unexpected exception getting binary formatted data.");
        }

        stream.Position = 0;
        _binaryFormatter ??= new BinaryFormatter
        {
            Binder = new ResXSerializationBinder(typeResolver)
        };

        // cs/dangerous-binary-deserialization
#pragma warning disable CA2300 // Do not use insecure deserializer BinaryFormatter
#pragma warning disable CA2302 // Ensure BinaryFormatter.Binder is set before calling BinaryFormatter.Deserialize
        object? result = _binaryFormatter.Deserialize(stream); // CodeQL[SM03722] : BinaryFormatter is intended to be used as a fallback for unsupported types. Users must explicitly opt into this behavior
#pragma warning restore CA2302
#pragma warning restore CA2300
        if (result is ResXNullRef)
        {
            result = null;
        }

        return result;
    }

    internal DataNodeInfo GetDataNodeInfo()
    {
        bool shouldSerialize = true;
        if (_nodeInfo is not null)
        {
            shouldSerialize = false;
            _nodeInfo.Name = Name;
        }
        else
        {
            _nodeInfo = new()
            {
                Name = Name
            };
        }

        _nodeInfo.Comment = Comment;

        // We always serialize if this node represents a FileRef. This is because FileRef is a public property,
        // so someone could have modified it.
        if (shouldSerialize || FileRefFullPath is not null)
        {
            // If we don't have a datanodeinfo it could be either a direct object OR a fileref.
            if (FileRefFullPath is not null)
            {
                Debug.Assert(FileRef is not null);
                _nodeInfo.ValueData = FileRef?.ToString() ?? string.Empty;
                _nodeInfo.MimeType = null;
                _nodeInfo.TypeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(ResXFileRef), _typeNameConverter);
            }
            else
            {
                // Serialize to string inside the _nodeInfo.
                FillDataNodeInfoFromObject(_nodeInfo, _value);
            }
        }

        return _nodeInfo;
    }

    /// <summary>
    ///  Retrieves the position of the resource in the resource file.
    /// </summary>
    /// <returns>
    ///  A structure that specifies the location of this resource in the resource file as a line position (X) and
    ///  a column position (Y). If this resource is not part of a resource file, this method returns a structure
    ///  that has an X of 0 and a Y of 0.
    /// </returns>
    public Point GetNodePosition() => _nodeInfo?.ReaderPosition ?? default;

    /// <summary>
    ///  Retrieves the type name for the value by using the specified type resolution service
    /// </summary>
    public string? GetValueTypeName(ITypeResolutionService? typeResolver)
    {
        // The type name here is always a fully qualified name.
        if (!string.IsNullOrEmpty(_typeName))
        {
            return _typeName == MultitargetUtil.GetAssemblyQualifiedName(typeof(ResXNullRef), _typeNameConverter)
                ? MultitargetUtil.GetAssemblyQualifiedName(typeof(object), _typeNameConverter)
                : _typeName;
        }

        string? typeName = FileRefType;
        Type? objectType = null;

        // Do we have a fileref?
        if (typeName is not null)
        {
            // Try to resolve this type.
            objectType = ResolveType(typeName, typeResolver);
        }
        else if (_nodeInfo is not null)
        {
            // We don't have a fileref, try to resolve the type of the datanode.
            typeName = _nodeInfo.TypeName;

            // If typename is null, the default is just a string.
            if (!string.IsNullOrEmpty(typeName))
            {
                objectType = ResolveType(typeName, typeResolver);
            }
            else
            {
                if (string.IsNullOrEmpty(_nodeInfo.MimeType))
                {
                    // No typename, no mimetype, we have a string.
                    typeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(string), _typeNameConverter);
                }
                else
                {
                    // Have a mimetype, our only option is to deserialize to know what we're dealing with.
                    try
                    {
#pragma warning disable SYSLIB0051 // Type or member is obsolete
                        Type? type = _nodeInfo.MimeType == ResXResourceWriter.BinSerializedObjectMimeType
                            ? GenerateObjectFromBinaryDataNodeInfo(_nodeInfo, typeResolver)?.GetType()
                            : GenerateObjectFromDataNodeInfo(_nodeInfo, typeResolver)?.GetType();
#pragma warning restore SYSLIB0051
                        typeName = type is null ? null : MultitargetUtil.GetAssemblyQualifiedName(type, _typeNameConverter);
                    }
                    catch (Exception ex)
                    {
                        // It would be better to catch SerializationException but the underlying type resolver
                        // can throw things like FileNotFoundException.
                        if (ex.IsCriticalException())
                        {
                            throw;
                        }

                        // Something went wrong, type is not specified at all or stream is corrupted return system.object.
                        typeName = MultitargetUtil.GetAssemblyQualifiedName(typeof(object), _typeNameConverter);
                    }
                }
            }
        }

        if (objectType is not null)
        {
            typeName = objectType == typeof(ResXNullRef)
                ? MultitargetUtil.GetAssemblyQualifiedName(typeof(object), _typeNameConverter)
                : MultitargetUtil.GetAssemblyQualifiedName(objectType, _typeNameConverter);
        }

        return typeName;
    }

    /// <summary>
    ///  Retrieves the type name for the value by examining the specified assemblies.
    /// </summary>
    public string? GetValueTypeName(AssemblyName[]? names)
        => GetValueTypeName(new AssemblyNamesTypeResolutionService(names));

    /// <summary>
    ///  Retrieves the object that is stored by this node by using the specified type resolution service.
    /// </summary>
    public object? GetValue(ITypeResolutionService? typeResolver)
    {
        if (_value is not null)
        {
            return _value;
        }

        if (FileRefFullPath is not null)
        {
            if (FileRefType is not null && ResolveType(FileRefType, typeResolver) is not null)
            {
                // We have the fully qualified name for this type
                _fileRef = FileRefTextEncoding is not null
                    ? new ResXFileRef(FileRefFullPath, FileRefType, Encoding.GetEncoding(FileRefTextEncoding))
                    : new ResXFileRef(FileRefFullPath, FileRefType);
                return TypeDescriptor.GetConverter(typeof(ResXFileRef)).ConvertFrom(_fileRef.ToString());
            }

            throw new TypeLoadException(string.Format(SR.TypeLoadExceptionShort, FileRefType));
        }
        else if (_nodeInfo?.ValueData is not null)
        {
            // It's embedded, deserialize it.
#pragma warning disable SYSLIB0051 // Type or member is obsolete
            return _nodeInfo.MimeType == ResXResourceWriter.BinSerializedObjectMimeType
                ? GenerateObjectFromBinaryDataNodeInfo(_nodeInfo, typeResolver)
                : GenerateObjectFromDataNodeInfo(_nodeInfo, typeResolver);
#pragma warning restore SYSLIB0051
        }

        // Schema is wrong and says minOccur for Value is 0, but it's too late to change it.
        return null;
    }

    /// <summary>
    ///  Retrieves the object that is stored by this node by searching the specified assemblies.
    /// </summary>
    public object? GetValue(AssemblyName[]? names) => GetValue(new AssemblyNamesTypeResolutionService(names));

    private static byte[] FromBase64WrappedString(string text)
    {
        if (text.IndexOfAny(s_specialChars) != -1)
        {
            StringBuilder builder = new(text.Length);
            foreach (char c in text)
            {
                switch (c)
                {
                    case ' ':
                    case '\r':
                    case '\n':
                        break;
                    default:
                        builder.Append(c);
                        break;
                }
            }

            return Convert.FromBase64String(builder.ToString());
        }

        return Convert.FromBase64String(text);
    }

    private static Type? ResolveType(string typeName, ITypeResolutionService? typeResolver)
    {
        Type? resolvedType = null;
        if (typeResolver is not null)
        {
            // If we cannot find the strong-named type, then try to see if the TypeResolver can bind to partial
            // names. For this, we will strip out the partial names and keep the rest of the strong-name
            // information to try again.

            resolvedType = typeResolver.GetType(typeName, false);
            if (resolvedType is null)
            {
                string[] typeParts = typeName.Split(',', StringSplitOptions.TrimEntries);

                // Break up the type name from the rest of the assembly strong name.
                if (typeParts is not null && typeParts.Length >= 2)
                {
                    resolvedType = typeResolver.GetType($"{typeParts[0]}, {typeParts[1]}", false);
                }
            }
        }

        return resolvedType ?? Type.GetType(typeName, throwOnError: false);
    }

    void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        => throw new PlatformNotSupportedException();
}
