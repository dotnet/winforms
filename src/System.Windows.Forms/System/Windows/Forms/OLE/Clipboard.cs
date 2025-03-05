// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing;
using System.Formats.Nrbf;
using System.Private.Windows.Ole;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods to place data on and retrieve data from the system clipboard. This class cannot be inherited.
/// </summary>
public static class Clipboard
{
    /// <summary>
    ///  Places non-persistent data on the system <see cref="Clipboard"/>.
    /// </summary>
    /// <inheritdoc cref="SetDataObject(object, bool, int, int)"/>
    public static void SetDataObject(object data) => SetDataObject(data, copy: false);

    /// <summary>
    ///  Overload that uses default values for retryTimes and retryDelay.
    /// </summary>
    /// <inheritdoc cref="SetDataObject(object, bool, int, int)"/>
    public static void SetDataObject(object data, bool copy) =>
        SetDataObject(data, copy, retryTimes: 10, retryDelay: 100);

    /// <summary>
    ///  Places data on the system <see cref="Clipboard"/> and uses copy to specify whether the data
    ///  should remain on the <see cref="Clipboard"/> after the application exits.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   See remarks for <see cref="DataObject(object)"/> for recommendations on how to implement custom <paramref name="data"/>.
    ///  </para>
    /// </remarks>
    public static unsafe void SetDataObject(object data, bool copy, int retryTimes, int retryDelay)
    {
        ArgumentNullException.ThrowIfNull(data);

        // Wrap if we're not already a DataObject
        DataObject dataObject = data as DataObject ?? new WrappingDataObject(data);
        HRESULT result = ClipboardCore.SetData(dataObject, copy, retryTimes, retryDelay);

        if (result.Failed)
        {
            throw new ExternalException(SR.ClipboardOperationFailed, (int)result);
        }
    }

    /// <summary>
    ///  Retrieves the data that is currently on the system <see cref="Clipboard"/>.
    /// </summary>
    public static unsafe IDataObject? GetDataObject()
    {
        HRESULT result = ClipboardCore.GetDataObject<DataObject, IDataObject>(out IDataObject? dataObject);
        if (result.Failed)
        {
            Debug.Assert(dataObject is null);
            throw new ExternalException(SR.ClipboardOperationFailed, (int)result);
        }

        return dataObject;
    }

    /// <summary>
    ///  Removes all data from the Clipboard.
    /// </summary>
    public static unsafe void Clear()
    {
        HRESULT result = ClipboardCore.Clear();
        if (result.Failed)
        {
            throw new ExternalException(SR.ClipboardOperationFailed, (int)result);
        }
    }

    /// <summary>
    ///  Indicates whether there is data on the Clipboard in the <see cref="DataFormats.WaveAudio"/> format.
    /// </summary>
    public static bool ContainsAudio() => ContainsData(DataFormatNames.WaveAudio);

    /// <summary>
    ///  Indicates whether there is data on the Clipboard that is in the specified format
    ///  or can be converted to that format.
    /// </summary>
    public static bool ContainsData(string? format) =>
        !string.IsNullOrWhiteSpace(format) && ContainsData(format, autoConvert: false);

    private static bool ContainsData(string format, bool autoConvert) =>
        GetDataObject() is IDataObject dataObject && dataObject.GetDataPresent(format, autoConvert: autoConvert);

    /// <summary>
    ///  Indicates whether there is data on the Clipboard that is in the <see cref="DataFormats.FileDrop"/> format
    ///  or can be converted to that format.
    /// </summary>
    public static bool ContainsFileDropList() => ContainsData(DataFormats.FileDrop, autoConvert: true);

    /// <summary>
    ///  Indicates whether there is data on the Clipboard that is in the <see cref="DataFormats.Bitmap"/> format
    ///  or can be converted to that format.
    /// </summary>
    public static bool ContainsImage() => ContainsData(DataFormats.Bitmap, autoConvert: true);

    /// <summary>
    ///  Indicates whether there is text data on the Clipboard in <see cref="TextDataFormat.UnicodeText"/> format.
    /// </summary>
    public static bool ContainsText() => ContainsText(TextDataFormat.UnicodeText);

    /// <summary>
    ///  Indicates whether there is text data on the Clipboard in the format indicated by the specified
    ///  <see cref="TextDataFormat"/> value.
    /// </summary>
    public static bool ContainsText(TextDataFormat format)
    {
        SourceGenerated.EnumValidator.Validate(format, nameof(format));

        // Historically we didn't pass true for autoConvert, but it effectively was always
        // true because getting the DataObject would always give us an IDataObject RCW which
        // would call back through the format enumerator which defaults to true.
        //
        // We now unwrap original objects when we can, so we need to emulate the old behavior.
        return ContainsData(ConvertToDataFormats(format), autoConvert: true);
    }

    /// <summary>
    ///  Retrieves an audio stream from the <see cref="Clipboard"/>.
    /// </summary>
    public static Stream? GetAudioStream() => GetTypedDataIfAvailable<Stream>(DataFormatNames.WaveAudio);

    /// <summary>
    ///  Retrieves data from the <see cref="Clipboard"/> in the specified format.
    /// </summary>
    /// <exception cref="ThreadStateException">
    ///  The current thread is not in single-threaded apartment (STA) mode.
    /// </exception>
    [Obsolete(
        Obsoletions.ClipboardGetDataMessage,
        error: false,
        DiagnosticId = Obsoletions.ClipboardGetDataDiagnosticId,
        UrlFormat = Obsoletions.SharedUrlFormat)]
    public static object? GetData(string format) =>
        string.IsNullOrWhiteSpace(format) ? null : GetData(format, autoConvert: false);

    private static object? GetData(string format, bool autoConvert) =>
        GetDataObject() is IDataObject dataObject ? dataObject.GetData(format, autoConvert) : null;

    /// <summary>
    ///  Retrieves data in the specified format if that data is of type <typeparamref name="T"/>. This is the only
    ///  overload of TryGetData that has the possibility of falling back to the <see cref="BinaryFormatter"/> and
    ///  should only be used if you need <see cref="BinaryFormatter"/> support.
    /// </summary>
    /// <param name="format">
    ///  <para>
    ///   The format of the data to retrieve. See the <see cref="DataFormats"/> class for a set of predefined data formats.
    ///  </para>
    /// </param>
    /// <param name="resolver">
    ///  <para>
    ///   A <see cref="Func{Type, TypeName}"/> that is used only when deserializing non-OLE formats. It returns the type if
    ///   <see cref="TypeName"/> is allowed or throws a <see cref="NotSupportedException"/> if <see cref="TypeName"/> is not
    ///   expected. If the resolver returns <see langword="null"/>, the following types will be resolved automatically:
    ///  </para>
    ///  <list type="bullet">
    ///   <item>
    ///    <description>
    ///     <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/4e77849f-89e3-49db-8fb9-e77ee4bc7214">
    ///      NRBF primitive types
    ///     </see>
    ///     (bool, byte, char, decimal, double, short, int, long, sbyte, ushort, uint, ulong, float, string, TimeSpan, DateTime).
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <description>
    ///     Arrays and List{} of NRBF primitive types.
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <description>
    ///     Core System.Drawing types (Bitmap, PointF, RectangleF, Point, Rectangle, SizeF, Size, Color).
    ///    </description>
    ///   </item>
    ///  </list>
    ///  <para>
    ///   <see cref="TypeName"/> parameter can be matched according to the user requirements, for example, only namespace-qualified
    ///   type names, or full type and assembly names, or full type names and short assembly names.
    ///  </para>
    /// </param>
    /// <param name="data">
    ///  <para>
    ///   Out parameter that contains the retrieved data in the specified format, or <see langword="null"/> if the data is
    ///   unavailable in the specified format, or is of a wrong <see cref="Type"/>.
    ///  </para>
    /// </param>
    /// <typeparam name="T">
    ///  <para>
    ///   The expected type. A resolver must be provided to handle derived types.
    ///  </para>
    /// </typeparam>
    /// <returns>
    ///  <see langword="true"/> if the data of this format is present on the clipboard and the value is of a matching
    ///  type and that value can be successfully retrieved, or <see langword="false"/> if the format is not present or
    ///  the value is of a wrong <see cref="Type"/>.
    /// </returns>
    /// <remarks>
    ///  <para>
    ///   This API will fall back to the <see cref="BinaryFormatter"/> if the application has enabled it and taken
    ///   the <see href="https://learn.microsoft.com/dotnet/standard/serialization/binaryformatter-migration-guide/">
    ///   unsupported System.Runtime.Serialization.Formatters package</see>. You also must have enabled the OLE specific
    ///   switch "Windows.ClipboardDragDrop.EnableUnsafeBinaryFormatterSerialization" to allow fallback to the
    ///   <see cref="BinaryFormatter"/>.
    ///  </para>
    ///  <para>
    ///   Pre-defined <see cref="DataFormats"/> or other data that was serialized via <see cref="SetDataAsJson{T}(string, T)"/>
    ///   or <see cref="DataObject.SetDataAsJson{T}(string, T)"/> will always be able to be deserialized without enabling
    ///   the <see cref="BinaryFormatter"/>. <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/4e77849f-89e3-49db-8fb9-e77ee4bc7214">
    ///   NRBF primitive types</see> are also handled, as well as <see cref="List{T}"/> or arrays of these type. Basic
    ///   System.Drawing exchange types and Bitmap are also handled.
    ///  </para>
    ///  <para>
    ///   If the data is serialized in the NRBF format, passing <see cref="SerializationRecord"/> for
    ///   <typeparamref name="T"/> will return the decoded data. This can be used for full deserialization customization.
    ///  </para>
    ///  <para>
    ///   Avoid loading assemblies named in the <see cref="TypeName"/> argument of your <paramref name="resolver"/>.
    ///   Calling the <see cref="Type.GetType(string)"/> method can cause assembly loads and is not safe to trim.
    ///   Use <see langword="typeof"/> where possible.
    ///  </para>
    ///  <para>
    ///   For compatibility, .NET types are usually serialized using their .NET Framework assembly names. The resolver
    ///   should be aware of <see cref="TypeName"/>s coming in with either .NET Framework assembly names or .NET ones.
    ///  </para>
    ///  <para>
    ///   Make sure to consider other assembly information when matching, such as version, if you expect to be able to
    ///   deserialize from multiple assembly versions.
    ///  </para>
    ///  <para>
    ///   Also consider that Arrays, generic types, and nullable value types will have assembly names nested, in the
    ///   <see cref="TypeName.FullName"/> property.
    ///  </para>
    /// </remarks>
    /// <exception cref="NotSupportedException">
    ///  If application does not support <see cref="BinaryFormatter"/> and the object can't be deserialized otherwise, or
    ///  application supports <see cref="BinaryFormatter"/> but <typeparamref name="T"/> is an <see cref="object"/>,
    ///  or not a concrete type, or if <paramref name="resolver"/> does not resolve the actual payload type. Or
    ///  the <see cref="IDataObject"/> on the <see cref="Clipboard"/> does not implement <see cref="ITypedDataObject"/>
    ///  interface.
    /// </exception>
    /// <exception cref="ThreadStateException">
    ///  The current thread is not in single-threaded apartment (STA) mode.
    /// </exception>
    /// <example>
    ///  <![CDATA[
    ///   using System.Reflection.Metadata;
    ///
    ///   internal static Type MyExactMatchResolver(TypeName typeName)
    ///   {
    ///        // The preferred approach is to resolve types at build time to avoid assembly loading at runtime.
    ///        (Type type, TypeName typeName)[] allowedTypes =
    ///        [
    ///            (typeof(MyClass1), TypeName.Parse(typeof(MyClass1).AssemblyQualifiedName)),
    ///            (typeof(MyClass2), TypeName.Parse(typeof(MyClass2).AssemblyQualifiedName))
    ///        ];
    ///
    ///        foreach (var (type, name) in allowedTypes)
    ///        {
    ///            // Namespace-qualified type name, using case-sensitive comparison for C#.
    ///            if (name.FullName != typeName.FullName)
    ///            {
    ///                continue;
    ///            }
    ///
    ///            AssemblyNameInfo? info1 = typeName.AssemblyName;
    ///            AssemblyNameInfo? info2 = name.AssemblyName;
    ///
    ///            if (info1 is null && info2 is null)
    ///            {
    ///                return type;
    ///            }
    ///
    ///            if (info1 is null || info2 is null)
    ///            {
    ///                continue;
    ///            }
    ///
    ///            // Full assembly name comparison, case sensitive.
    ///            if (info1.Name == info2.Name
    ///                 && info1.Version == info2.Version
    ///                 && ((info1.CultureName ?? string.Empty) == info2.CultureName)
    ///                 && info1.PublicKeyOrToken.AsSpan().SequenceEqual(info2.PublicKeyOrToken.AsSpan()))
    ///            {
    ///                return type;
    ///            }
    ///        }
    ///
    ///        throw new NotSupportedException($"Can't resolve {typeName.AssemblyQualifiedName}");
    ///    }
    ///  ]]>
    /// </example>
    [CLSCompliant(false)]
    public static bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type?> resolver,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        data = default;
        resolver.OrThrowIfNull();
        if (!ClipboardCore.IsValidTypeForFormat(typeof(T), format)
            || GetDataObject() is not { } dataObject)
        {
            // Invalid format or no object on the clipboard at all.
            return false;
        }

        return dataObject.TryGetData(format, resolver, autoConvert: false, out data);
    }

    /// <remarks>
    ///  <para>
    ///   This method will never allow falling back to the <see cref="BinaryFormatter"/>, even if it is fully enabled.
    ///   You must use the <see cref="TryGetData{T}(string, Func{TypeName, Type?}, out T)"/> with an explicit resolver.
    ///  </para>
    ///  <para>
    ///   Pre-defined <see cref="DataFormats"/> or other data that was serialized via <see cref="SetDataAsJson{T}(string, T)"/>
    ///   or <see cref="DataObject.SetDataAsJson{T}(string, T)"/> will always be able to be deserialized without enabling
    ///   the <see cref="BinaryFormatter"/>. <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/4e77849f-89e3-49db-8fb9-e77ee4bc7214">
    ///   NRBF primitive types</see> are also handled, as well as <see cref="List{T}"/> or arrays of these type. Basic
    ///   System.Drawing exchange types and Bitmap are also handled.
    ///  </para>
    ///  <para>
    ///   If the data is serialized in the NRBF format, passing <see cref="SerializationRecord"/> for
    ///   <typeparamref name="T"/> will return the decoded data. This can be used for full deserialization customization.
    ///  </para>
    /// </remarks>
    /// <inheritdoc cref="TryGetData{T}(string, Func{TypeName, Type}, out T)"/>
    public static bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        data = default;
        if (!ClipboardCore.IsValidTypeForFormat(typeof(T), format)
            || GetDataObject() is not { } dataObject)
        {
            // Invalid format or no object on the clipboard at all.
            return false;
        }

        return dataObject.TryGetData(format, out data);
    }

    /// <summary>
    ///  Retrieves a collection of file names from the <see cref="Clipboard"/>.
    /// </summary>
    public static StringCollection GetFileDropList()
    {
        StringCollection result = [];

        if (GetTypedDataIfAvailable<string[]?>(DataFormatNames.FileDrop) is string[] strings)
        {
            result.AddRange(strings);
        }

        return result;
    }

    /// <summary>
    ///  Retrieves a <see cref="Bitmap"/> from the <see cref="Clipboard"/>.
    /// </summary>
    /// <devdoc>
    ///  <see cref="Bitmap"/>s are re-hydrated from a <see cref="SerializationRecord"/> by reading a byte array.
    /// </devdoc>
    public static Image? GetImage() => GetTypedDataIfAvailable<Image>(DataFormats.Bitmap);

    /// <summary>
    ///  Retrieves text data from the <see cref="Clipboard"/> in the <see cref="TextDataFormat.UnicodeText"/> format.
    /// </summary>
    public static string GetText() => GetText(TextDataFormat.UnicodeText);

    /// <summary>
    ///  Retrieves text data from the <see cref="Clipboard"/> in the format indicated by the specified
    ///  <see cref="TextDataFormat"/> value.
    /// </summary>
    public static string GetText(TextDataFormat format)
    {
        SourceGenerated.EnumValidator.Validate(format, nameof(format));

        return GetTypedDataIfAvailable<string>(ConvertToDataFormats(format)) is string text ? text : string.Empty;
    }

    private static T? GetTypedDataIfAvailable<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format)
    {
        IDataObject? data = GetDataObject();
        if (data is ITypedDataObject typed)
        {
            return typed.TryGetData(format, autoConvert: true, out T? value) ? value : default;
        }

        if (data is IDataObject dataObject)
        {
            return dataObject.GetData(format, autoConvert: true) is T value ? value : default;
        }

        return default;
    }

    /// <summary>
    ///  Clears the <see cref="Clipboard"/> and then adds data in the <see cref="DataFormats.WaveAudio"/> format.
    /// </summary>
    public static void SetAudio(byte[] audioBytes) => SetAudio(new MemoryStream(audioBytes.OrThrowIfNull()));

    /// <summary>
    ///  Clears the <see cref="Clipboard"/> and then adds data in the <see cref="DataFormats.WaveAudio"/> format.
    /// </summary>
    public static void SetAudio(Stream audioStream) =>
        SetDataObject(new DataObject(DataFormatNames.WaveAudio, audioStream.OrThrowIfNull()), copy: true);

    /// <summary>
    ///  Clears the Clipboard and then adds data in the specified format.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   See remarks for <see cref="DataObject(object)"/> for recommendations on how to implement custom <paramref name="data"/>.
    ///  </para>
    /// </remarks>
    public static void SetData(string format, object data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(format);
        ArgumentNullException.ThrowIfNull(data);

        // Note: We delegate argument checking to IDataObject.SetData, if it wants to do so.
        SetDataObject(new DataObject(format, data), copy: true);
    }

    /// <inheritdoc cref="DataObject.SetDataAsJson{T}(string, T)"/>
    public static void SetDataAsJson<T>(string format, T data)
    {
        ArgumentNullException.ThrowIfNull(data);
        ArgumentException.ThrowIfNullOrWhiteSpace(format);

        DataObject dataObject = new();
        dataObject.SetDataAsJson(format, data);
        SetDataObject(dataObject, copy: true);
    }

    /// <summary>
    ///  Clears the Clipboard and then adds a collection of file names in the <see cref="DataFormats.FileDrop"/> format.
    /// </summary>
    public static void SetFileDropList(StringCollection filePaths) =>
        ClipboardCore.SetFileDropList(filePaths);

    /// <summary>
    ///  Clears the Clipboard and then adds an <see cref="Image"/> in the <see cref="DataFormats.Bitmap"/> format.
    /// </summary>
    public static void SetImage(Image image) =>
        SetDataObject(new DataObject(DataFormatNames.Bitmap, autoConvert: true, image.OrThrowIfNull()), copy: true);

    /// <summary>
    ///  Clears the Clipboard and then adds text data in the <see cref="TextDataFormat.UnicodeText"/> format.
    /// </summary>
    public static void SetText(string text) => SetText(text, TextDataFormat.UnicodeText);

    /// <summary>
    ///  Clears the Clipboard and then adds text data in the format indicated by the specified
    ///  <see cref="TextDataFormat"/> value.
    /// </summary>
    public static void SetText(string text, TextDataFormat format)
    {
        text.ThrowIfNullOrEmpty();
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        SetDataObject(new DataObject(ConvertToDataFormats(format), text), copy: true);
    }

    private static string ConvertToDataFormats(TextDataFormat format) => format switch
    {
        TextDataFormat.Text => DataFormats.Text,
        TextDataFormat.UnicodeText => DataFormats.UnicodeText,
        TextDataFormat.Rtf => DataFormats.Rtf,
        TextDataFormat.Html => DataFormats.Html,
        TextDataFormat.CommaSeparatedValue => DataFormats.CommaSeparatedValue,
        _ => DataFormats.UnicodeText,
    };
}
