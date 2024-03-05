// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.Serialization;

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomComponentSerializationService
{
    internal sealed partial class CodeDomSerializationStore
    {
        [Serializable]
        public sealed class AssemblyNameInfo : ISerializable
        {
            private AssemblyContentType ContentType { get; set; }
            private string? CultureName { get; set; }
            private AssemblyNameFlags Flags { get; set; }
            public string FullName { get; set; }
            private string? Name { get; set; }
            private Version? Version { get; set; }

            public AssemblyNameInfo(AssemblyName assemblyName)
            {
                ContentType = assemblyName.ContentType;
                CultureName = assemblyName.CultureName;
                Flags = assemblyName.Flags;
                FullName = assemblyName.FullName;
                Name = assemblyName.Name;
                Version = assemblyName.Version;
            }

            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                ArgumentNullException.ThrowIfNull(info);

                info.AddValue("ContentType", ContentType);
                info.AddValue("CultureName", CultureName);
                info.AddValue("Flags", Flags);
                info.AddValue("FullName", FullName);
                info.AddValue("Name", Name);
                info.AddValue("Version", Version);
            }

            private AssemblyNameInfo(SerializationInfo serializationInfo, StreamingContext streamingContext)
            {
                throw new NotImplementedException();
            }
        }
    }
}
