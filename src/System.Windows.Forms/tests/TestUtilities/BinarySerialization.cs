// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace System
{
    public static class BinarySerialization
    {
        public static void EnsureSerializableAttribute(Assembly assemblyUnderTest, HashSet<string> serializableTypes)
        {
            foreach (Type type in assemblyUnderTest.GetTypes())
            {
                var attributes = Attribute.GetCustomAttributes(type);

                if (!attributes.Any(a => a is SerializableAttribute))
                {
                    // the type isn't marked as serialisable, verify it is not one of the types
                    // that we expect to be serializable
                    if (serializableTypes.Contains(type.FullName))
                    {
                        throw new NotSupportedException($"Serializable attribute is expected on {type.FullName}");
                    }

                    continue;
                }

                if (attributes.Any(a => a is CompilerGeneratedAttribute))
                {
                    // ignore compiler generated types, we have no control over them
                    continue;
                }

                // Ensure SerializableAttribute is not added to any types not in the known list.
                if (serializableTypes.Contains(type.FullName))
                {
                    // we have marked the type as serialisable, all good
                    continue;
                }

                throw new NotSupportedException($"Serializable attribute is not expected on {type.FullName}");
            }
        }

        public static T EnsureDeserialize<T>(string blob)
        {
            var @object = FromBase64String(blob);
            Assert.NotNull(@object);
            return Assert.IsType<T>(@object);
        }

        public static string ToBase64String(object @object,
            FormatterAssemblyStyle assemblyStyle = FormatterAssemblyStyle.Simple)
        {
            byte[] raw = ToByteArray(@object, assemblyStyle);
            return Convert.ToBase64String(raw);
        }

        private static object FromBase64String(string base64String,
            FormatterAssemblyStyle assemblyStyle = FormatterAssemblyStyle.Simple)
        {
            byte[] raw = Convert.FromBase64String(base64String);
            return FromByteArray(raw, assemblyStyle);
        }

        private static object FromByteArray(byte[] raw,
            FormatterAssemblyStyle assemblyStyle = FormatterAssemblyStyle.Simple)
        {
            var binaryFormatter = new BinaryFormatter
            {
                AssemblyFormat = assemblyStyle
            };

            using (var serializedStream = new MemoryStream(raw))
            {
#pragma warning disable SYSLIB0011
                return binaryFormatter.Deserialize(serializedStream);
#pragma warning restore SYSLIB0011
            }
        }

        private static byte[] ToByteArray(object obj,
            FormatterAssemblyStyle assemblyStyle = FormatterAssemblyStyle.Simple)
        {
            var binaryFormatter = new BinaryFormatter
            {
                AssemblyFormat = assemblyStyle
            };

            using (MemoryStream ms = new MemoryStream())
            {
#pragma warning disable SYSLIB0011
                binaryFormatter.Serialize(ms, obj);
#pragma warning restore SYSLIB0011
                return ms.ToArray();
            }
        }
    }
}
