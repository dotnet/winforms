// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Windows.Forms
{
    public partial class DataObject
    {
        /// <summary>
        ///  Binder that restricts DataObject content deserialization to Bitmap type and
        ///  serialization to strings and Bitmaps.
        ///  Deserialization of known safe types(strings and arrays of primitives) does not invoke the binder.
        /// </summary>
        private class BitmapBinder : SerializationBinder
        {
            // Bitmap type lives in defferent assemblies in the .NET Framework and in .NET Core.
            // However we allow desktop content to be deserializated in Core and Core content
            // deserialized on desktop. To support this roundtrip,
            // Bitmap type identity is unified to the desktop type during serialization
            // and we use the desktop type name when filtering as well.
            private const string AllowedTypeName = "System.Drawing.Bitmap";
            private const string AllowedAssemblyName = "System.Drawing";
            // PublicKeyToken=b03f5f7f11d50a3a
            private static readonly byte[] s_allowedToken = new byte[] { 0xB0, 0x3F, 0x5F, 0x7F, 0x11, 0xD5, 0x0A, 0x3A };

            /// <summary>
            ///  Only safe to deserialize types are bypassing this callback, Strings
            ///  and arrays of primitive types in particular. We are explicitly allowing
            ///  System.Drawing.Bitmap type to bind using the default binder.
            /// </summary>
            /// <param name="assemblyName"></param>
            /// <param name="typeName"></param>
            /// <returns>null - continue with the default binder.</returns>
            public override Type BindToType(string assemblyName, string typeName)
            {
                if (string.CompareOrdinal(typeName, AllowedTypeName) == 0)
                {
                    AssemblyName nameToBind = null;
                    try
                    {
                        nameToBind = new AssemblyName(assemblyName);
                    }
                    catch
                    {
                    }
                    if (nameToBind != null)
                    {
                        if (string.CompareOrdinal(nameToBind.Name, AllowedAssemblyName) == 0)
                        {
                            byte[] tokenToBind = nameToBind.GetPublicKeyToken();
                            if ((tokenToBind != null) &&
                                (s_allowedToken != null) &&
                                (tokenToBind.Length == s_allowedToken.Length))
                            {
                                bool block = false;
                                for (int i = 0; i < s_allowedToken.Length; i++)
                                {
                                    if (s_allowedToken[i] != tokenToBind[i])
                                    {
                                        block = true;
                                        break;
                                    }
                                }
                                if (!block)
                                {
                                    return null;
                                }
                            }
                        }
                    }
                }
                throw new RestrictedTypeDeserializationException(SR.UnexpectedClipboardType);
            }

            /// <summary>
            ///  Bitmap and string types are safe type to serialize/deserialize.
            /// </summary>
            /// <param name="serializedType"></param>
            /// <param name="assemblyName"></param>
            /// <param name="typeName"></param>
            public override void BindToName(Type serializedType, out string assemblyName, out string typeName)
            {
                // null strings will follow the default codepath in BinaryFormatter
                assemblyName = null;
                typeName = null;
                if (serializedType != null && !serializedType.Equals(typeof(string)) && !serializedType.Equals(typeof(Bitmap)))
                {
                    throw new SerializationException(string.Format(SR.UnexpectedTypeForClipboardFormat, serializedType.FullName));
                }
            }
        }
    }
}
