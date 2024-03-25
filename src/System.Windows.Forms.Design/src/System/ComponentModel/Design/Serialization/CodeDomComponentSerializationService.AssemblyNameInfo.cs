// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.Serialization;

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomComponentSerializationService
{
    [Serializable]
    internal sealed class AssemblyNameInfo : ISerializable
    {
        private const string StrContentType = "ContentType";
        private const string StrCultureName = "CultureName";
        private const string StrFlags = "Flags";
        private const string StrFullName = "FullName";
        private const string StrName = "Name";
        private const string StrVersion = "Version";

        public AssemblyContentType ContentType { get; set; }
        public string? CultureName { get; set; }
        public AssemblyNameFlags Flags { get; set; }
        public string FullName { get; set; }
        public string? Name { get; set; }
        public Version? Version { get; set; }

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

            info.AddValue(StrContentType, ContentType);
            info.AddValue(StrCultureName, CultureName);
            info.AddValue(StrFlags, Flags);
            info.AddValue(StrFullName, FullName);
            info.AddValue(StrName, Name);
            info.AddValue(StrVersion, Version);
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private AssemblyNameInfo(SerializationInfo info, StreamingContext streamingContext)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            ArgumentNullException.ThrowIfNull(info);

            CultureName = (string?)info.GetValue(StrCultureName, type: typeof(string));
            Name = (string?)info.GetValue(StrName, type: typeof(string));
            Version = (Version?)info.GetValue(StrVersion, type: typeof(Version));
#nullable disable
            FullName = (string)info.GetValue(StrFullName, type: typeof(string));
            Flags = (AssemblyNameFlags)info.GetValue(StrFlags, type: typeof(AssemblyNameFlags));
            ContentType = (AssemblyContentType)info.GetValue(StrContentType, type: typeof(AssemblyContentType));
        }
    }
}
