// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using Xunit;

namespace System
{
    public static class BinarySerialization
    {
        public static void EnsureSerializableAttribute(Assembly assemblyUnderTest, List<string> serializableTypes)
        {
            foreach (Type type in assemblyUnderTest.GetTypes())
            {
                var serializableAttribute = Attribute.GetCustomAttribute(type, typeof(SerializableAttribute));

                // Ensure SerializableAttribute is not added to any types not in the known list.
                if (serializableTypes.Contains(type.FullName))
                {
                    Assert.NotNull(serializableAttribute);
                }
                else
                {
                    Assert.True(null == serializableAttribute, $"Serializable attribute is not expected on {type.FullName}");
                }
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
                return binaryFormatter.Deserialize(serializedStream);
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
                binaryFormatter.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
    }
}
