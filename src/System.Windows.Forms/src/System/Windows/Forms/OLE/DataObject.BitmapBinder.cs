// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Windows.Forms;

public partial class DataObject
{
    /// <summary>
    ///  Binder that restricts deserialization to Bitmap type and serialization to strings and Bitmaps.
    ///  Deserialization of known safe types (strings and arrays of primitives) does not invoke the binder.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This gets skipped when our code handles its known types.
    ///   While there are more types allowed (such as <see cref="List{String}"/>, they are all safe.
    ///  </para>
    /// </remarks>
    private sealed class BitmapBinder : SerializationBinder
    {
        // Bitmap type lives in different assemblies in the .NET Framework and in .NET Core. To support serialization
        // between both runtimes the .NET Framework identities are used.
        private const string AllowedTypeName = "System.Drawing.Bitmap";
        private const string AllowedAssemblyName = "System.Drawing";

        // .NET Framework PublicKeyToken=b03f5f7f11d50a3a
        private static ReadOnlySpan<byte> AllowedToken => [0xB0, 0x3F, 0x5F, 0x7F, 0x11, 0xD5, 0x0A, 0x3A];

        public override Type? BindToType(string assemblyName, string typeName)
        {
            // Only safe to deserialize types are bypassing this callback. Strings and arrays of primitive types in
            // particular.

            if (AllowedTypeName.Equals(typeName, StringComparison.Ordinal))
            {
                try
                {
                    AssemblyName nameToBind = new(assemblyName);
                    if (AllowedAssemblyName.Equals(nameToBind.Name, StringComparison.Ordinal)
                        && AllowedToken.SequenceEqual(nameToBind.GetPublicKeyToken()))
                    {
                        return typeof(Bitmap);
                    }
                }
                catch (Exception ex) when (ex is ArgumentException or FileLoadException)
                {
                }
            }

            throw new RestrictedTypeDeserializationException(SR.UnexpectedClipboardType);
        }

        public override void BindToName(Type serializedType, out string? assemblyName, out string? typeName)
        {
            // Null values will follow the default codepath in BinaryFormatter.
            assemblyName = null;
            typeName = null;

            // Bitmap and string types are safe types to serialize/deserialize.
            if (!serializedType.Equals(typeof(string)) && !serializedType.Equals(typeof(Bitmap)))
            {
                throw new SerializationException(string.Format(SR.UnexpectedTypeForClipboardFormat, serializedType.FullName));
            }
        }
    }
}
