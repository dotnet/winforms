// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.Drawing;
using System.Formats.Nrbf;
using System.Reflection.Metadata;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Private.Windows.Core.OLE;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods to place data on and retrieve data from the system clipboard. This class cannot be inherited.
/// </summary>
public static class Clipboard
{
    private static DesktopClipboard? s_internalClipboard;

    private static DesktopClipboard InternalClipboard => s_internalClipboard ??= new WinFormsClipboard();
    /// <summary>
    ///  Places non-persistent data on the system <see cref="Clipboard"/>.
    /// </summary>
    /// <inheritdoc cref="SetDataObject(object, bool, int, int)"/>
    public static void SetDataObject(object data) => InternalClipboard.SetDataObject(data);

    /// <summary>
    ///  Overload that uses default values for retryTimes and retryDelay.
    /// </summary>
    /// <inheritdoc cref="SetDataObject(object, bool, int, int)"/>
    public static void SetDataObject(object data, bool copy) =>
        InternalClipboard.SetDataObject(data, copy);

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
        if (Application.OleRequired() != ApartmentState.STA)
        {
            throw new ThreadStateException(SR.ThreadMustBeSTA);
        }

        if (data is DataObject dataObject)
        {
            InternalClipboard.SetDataObject(dataObject._innerDataObject, copy, retryTimes, retryDelay);
        }
        else if (data is IDataObject iDataObject)
        {
            InternalClipboard.SetDataObject(new DataObjectAdapter(iDataObject), copy, retryTimes, retryDelay);
        }
        else
        {
            InternalClipboard.SetDataObject(data, copy, retryTimes, retryDelay);
        }
    }

    /// <summary>
    ///  Retrieves the data that is currently on the system <see cref="Clipboard"/>.
    /// </summary>
    public static unsafe IDataObject? GetDataObject()
    {
        if (Application.OleRequired() != ApartmentState.STA)
        {
            // Only throw if a message loop was started. This makes the case of trying to query the clipboard from the
            // finalizer or non-UI MTA thread silently fail, instead of making the application die.
            return Application.MessageLoop ? throw new ThreadStateException(SR.ThreadMustBeSTA) : null;
        }

        IDataObjectDesktop? iDataObjectInner = InternalClipboard.GetDataObject();
        if (iDataObjectInner is DataObjectAdapter adapter)
        {
            return adapter.OriginalDataObject;
        }
        else if (iDataObjectInner is DesktopDataObject dataObjectInner)
        {
            return new DataObject(dataObjectInner);
        }

        // There should be no other cases..
        return null;
    }

    /// <summary>
    ///  Removes all data from the Clipboard.
    /// </summary>
    public static unsafe void Clear()
    {
        if (Application.OleRequired() != ApartmentState.STA)
        {
            throw new ThreadStateException(SR.ThreadMustBeSTA);
        }

        DesktopClipboard.Clear();
    }

    /// <summary>
    ///  Indicates whether there is data on the Clipboard in the <see cref="DataFormats.WaveAudio"/> format.
    /// </summary>
    public static bool ContainsAudio() => InternalClipboard.ContainsAudio();

    /// <summary>
    ///  Indicates whether there is data on the Clipboard that is in the specified format
    ///  or can be converted to that format.
    /// </summary>
    public static bool ContainsData(string? format) => InternalClipboard.ContainsData(format);

    /// <summary>
    ///  Indicates whether there is data on the Clipboard that is in the <see cref="DataFormats.FileDrop"/> format
    ///  or can be converted to that format.
    /// </summary>
    public static bool ContainsFileDropList() => InternalClipboard.ContainsFileDropList();

    /// <summary>
    ///  Indicates whether there is data on the Clipboard that is in the <see cref="DataFormats.Bitmap"/> format
    ///  or can be converted to that format.
    /// </summary>
    public static bool ContainsImage() => InternalClipboard.ContainsImage();

    /// <summary>
    ///  Indicates whether there is text data on the Clipboard in <see cref="TextDataFormat.UnicodeText"/> format.
    /// </summary>
    public static bool ContainsText() => InternalClipboard.ContainsText();

    /// <summary>
    ///  Indicates whether there is text data on the Clipboard in the format indicated by the specified
    ///  <see cref="TextDataFormat"/> value.
    /// </summary>
    public static bool ContainsText(TextDataFormat format)
    {
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        return InternalClipboard.ContainsText((DesktopTextDataFormat)format);
    }

    /// <summary>
    ///  Retrieves an audio stream from the <see cref="Clipboard"/>.
    /// </summary>
    public static Stream? GetAudioStream() => InternalClipboard.GetAudioStream();

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
        InternalClipboard.GetData(format);

    /// <summary>
    ///  Retrieves data from the <see cref="Clipboard"/> in the specified format if that data is of type <typeparamref name="T"/>.
    ///  This is an alternative to <see cref="GetData(string)"/> that uses <see cref="BinaryFormatter"/> only when application
    ///  enabled the <see cref="AppContext"/> switch named "Windows.ClipboardDragDrop.EnableUnsafeBinaryFormatterSerialization".
    ///  By default the NRBF deserializer attempts to deserialize the stream. It can be disabled in favor of <see cref="BinaryFormatter"/>
    ///  with <see cref="AppContext"/> switch named "Windows.ClipboardDragDrop.EnableNrbfSerialization".
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
    ///   expected. It should not return a <see langword="null"/>. It should resolve type requested by the user as
    ///   <typeparamref name="T"/>, as well as types of its fields, unless they are primitive or known types.
    ///  </para>
    ///  <para>
    ///   The following types are resolved automatically:
    ///  </para>
    ///  <list type="bullet">
    ///   <item>
    ///    <description>
    ///     NRBF primitive types <see href="https://learn.microsoft.com/openspecs/windows_protocols/ms-nrbf/4e77849f-89e3-49db-8fb9-e77ee4bc7214"/>
    ///     (bool, byte, char, decimal, double, short, int, long, sbyte, ushort, uint, ulong, float, string, TimeSpan, DateTime).
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <description>
    ///     System.Drawing.Primitive.dll exchange types (PointF, RectangleF, Point, Rectangle, SizeF, Size, Color).
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <description>
    ///     Types commonly used in WinForms applications (System.Drawing.Bitmap, System.Windows.Forms.ImageListStreamer,
    ///     System.NotSupportedException, only the message is re-hydrated, List{T} where T is an NRBF primitive type,
    ///     and arrays of NRBF primitive types).
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
    ///   Out parameter that contains the retrieved data in the specified format, or <see lanfword="null"/> if the data is
    ///   unavailable in the specified format, or is of a wrong <see cref="Type"/>.
    ///  </para>
    /// </param>
    /// <returns>
    ///  <see langword="true"/> if the data of this format is present on the clipboard and the value is
    ///  of a matching type and that value can be successfully retrieved, or <see langword="false"/>
    ///  if the format is not present or the value is of a wrong  <see cref="Type"/>.
    /// </returns>
    /// <remarks>
    ///  <para>
    ///   Avoid loading assemblies named in the <see cref="TypeName"/> argument of the resolver function. Resolve only types
    ///   available at the compile time, for example do not call the <see cref="Type.GetType(string)"/> method.
    ///  </para>
    ///  <para>
    ///   Some common types, for example <see cref="Bitmap"/>, are type-forwarded from .NET Framework assemblies using the
    ///   <see cref="Runtime.CompilerServices.TypeForwardedFromAttribute"/>. <see cref="BinaryFormatter"/> serializes these types
    ///   using the forwarded from assembly information. The resolver function should take this into account and either
    ///   match only namespace qualified type names or read the <see cref="Runtime.CompilerServices.TypeForwardedFromAttribute.AssemblyFullName"/>
    ///   from the allowed type and match it to the <see cref="AssemblyNameInfo.FullName"/> property of <see cref="TypeName.AssemblyName"/>.
    ///  </para>
    ///  <para>
    ///   Make sure to match short assembly names if other information, such as version, is not needed, for example, when your
    ///   application can read multiple versions of the type. For exact matching, including assembly version, resolver
    ///   function is required, however primitive and common types are always matched after assembly version is removed.
    ///  </para>
    ///  <para>
    ///   Arrays, generic types, and nullable value types have full element name, including its assembly name, in the
    ///   <see cref="TypeName.FullName"/> property. Resolver function should either remove or type-forward these assembly
    ///   names when matching.
    ///  </para>
    /// </remarks>
    /// <exception cref="NotSupportedException">
    ///  If application does not support <see cref="BinaryFormatter"/> and the object can't be deserialized otherwise, or
    ///  application supports <see cref="BinaryFormatter"/> but <typeparamref name="T"/> is an <see cref="object"/>,
    ///  or not a concrete type, or if <paramref name="resolver"/> does not resolve the actual payload type. Or
    ///  the <see cref="IDataObject"/> on the <see cref="Clipboard"/> does not implement <see cref="ITypedDataObject"/>
    ///  interface.
    ///  </exception>
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
        Func<TypeName, Type> resolver,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        return InternalClipboard.TryGetData(format, resolver, out data);
    }

    /// <inheritdoc cref="TryGetData{T}(string, Func{TypeName, Type}, out T)"/>
    public static bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        [NotNullWhen(true), MaybeNullWhen(false)] out T data)
    {
        return InternalClipboard.TryGetData(format, out data);
    }

    /// <summary>
    ///  Retrieves a collection of file names from the <see cref="Clipboard"/>.
    /// </summary>
    public static StringCollection GetFileDropList() => InternalClipboard.GetFileDropList();

    /// <summary>
    ///  Retrieves a <see cref="Bitmap"/> from the <see cref="Clipboard"/>.
    /// </summary>
    /// <devdoc>
    ///  <see cref="Bitmap"/>s are re-hydrated from a <see cref="SerializationRecord"/> by reading a byte array.
    /// </devdoc>
    public static Image? GetImage() => InternalClipboard.GetTypedDataIfAvailable<Image>(DataFormats.Bitmap);

    /// <summary>
    ///  Retrieves text data from the <see cref="Clipboard"/> in the <see cref="TextDataFormat.UnicodeText"/> format.
    /// </summary>
    public static string GetText() => InternalClipboard.GetText();

    /// <summary>
    ///  Retrieves text data from the <see cref="Clipboard"/> in the format indicated by the specified
    ///  <see cref="TextDataFormat"/> value.
    /// </summary>
    public static string GetText(TextDataFormat format)
    {
        SourceGenerated.EnumValidator.Validate(format, nameof(format));

        return InternalClipboard.GetText((DesktopTextDataFormat)format);
    }

    /// <summary>
    ///  Clears the <see cref="Clipboard"/> and then adds data in the <see cref="DataFormats.WaveAudio"/> format.
    /// </summary>
    public static void SetAudio(byte[] audioBytes) => InternalClipboard.SetAudio(audioBytes);

    /// <summary>
    ///  Clears the <see cref="Clipboard"/> and then adds data in the <see cref="DataFormats.WaveAudio"/> format.
    /// </summary>
    public static void SetAudio(Stream audioStream) => InternalClipboard.SetAudio(audioStream);

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
        InternalClipboard.SetData(format, data);
    }

    /// <summary>
    ///  Saves the data onto the clipboard in the specified format.
    ///  If the data is a non intrinsic type, the object will be serialized using JSON.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///  <see langword="null"/>, empty string, or whitespace was passed as the format.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    ///  If <paramref name="data"/> is a non derived <see cref="DataObject"/>. This is for better error reporting as <see cref="DataObject"/> will serialize as empty.
    ///  If <see cref="DataObject"/> needs to be placed on the clipboard, use <see cref="DataObject.SetDataAsJson{T}(string, T)"/>
    ///  to JSON serialize the data to be held in the <paramref name="data"/>, then set the <paramref name="data"/>
    ///  onto the clipboard via <see cref="SetDataObject(object)"/>.
    /// </exception>
    /// <remarks>
    ///  <para>
    ///   If your data is an intrinsically handled type such as primitives, string, or Bitmap
    ///   and you are using a custom format or <see cref="DataFormats.Serializable"/>,
    ///   it is recommended to use the <see cref="SetData(string, object?)"/> APIs to avoid unnecessary overhead.
    ///  </para>
    ///  <para>
    ///   The default behavior of <see cref="JsonSerializer"/> is used to serialize the data.
    ///  </para>
    ///  <para>
    ///   See
    ///   <see href="https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/how-to#serialization-behavior"/>
    ///   and <see href="https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/reflection-vs-source-generation#metadata-collection"/>
    ///   for more details on default <see cref="JsonSerializer"/> behavior.
    ///  </para>
    ///  <para>
    ///   If custom JSON serialization behavior is needed, manually JSON serialize the data and then use <see cref="SetData"/>
    ///   to save the data onto the clipboard, or create a custom <see cref="Text.Json.Serialization.JsonConverter"/>, attach the
    ///   <see cref="Text.Json.Serialization.JsonConverterAttribute"/>, and then recall this method.
    ///   See <see href="https://learn.microsoft.com/dotnet/standard/serialization/system-text-json/converters-how-to"/> for more details
    ///   on custom converters for JSON serialization.
    ///  </para>
    /// </remarks>
    [RequiresUnreferencedCode("Uses default System.Text.Json behavior which is not trim-compatible.")]
    public static void SetDataAsJson<T>(string format, T data)
    {
        if (typeof(T) == typeof(DataObject))
        {
            throw new InvalidOperationException(string.Format(SR.ClipboardOrDragDrop_CannotJsonSerializeDataObject, nameof(SetDataObject)));
        }

        InternalClipboard.SetDataAsJson(format, data);
    }

    /// <summary>
    ///  Clears the Clipboard and then adds a collection of file names in the <see cref="DataFormats.FileDrop"/> format.
    /// </summary>
    public static void SetFileDropList(StringCollection filePaths) => InternalClipboard.SetFileDropList(filePaths);

    /// <summary>
    ///  Clears the Clipboard and then adds an <see cref="Image"/> in the <see cref="DataFormats.Bitmap"/> format.
    /// </summary>
    public static void SetImage(Image image) => SetDataObject(new DataObject(DataFormats.Bitmap, autoConvert: true, image.OrThrowIfNull()), copy: true);

    /// <summary>
    ///  Clears the Clipboard and then adds text data in the <see cref="TextDataFormat.UnicodeText"/> format.
    /// </summary>
    public static void SetText(string text) => InternalClipboard.SetText(text);

    /// <summary>
    ///  Clears the Clipboard and then adds text data in the format indicated by the specified
    ///  <see cref="TextDataFormat"/> value.
    /// </summary>
    public static void SetText(string text, TextDataFormat format)
    {
        SourceGenerated.EnumValidator.Validate(format, nameof(format));
        InternalClipboard.SetText(text, (DesktopTextDataFormat)format);
    }
}
