// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.Serialization;

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomComponentSerializationService
{
    /// <summary>
    /// Because AssemblyName and CultureInfo are no longer serializable in .NET, we define this binary serializable class instead.
    /// This class contains the text content from AssemblyName and CultureInfo classes.
    /// Note that this breaks binary compatibility with the .NET Framework.
    /// </summary>
    [Serializable]
    internal sealed class AssemblyNameInfo : ISerializable
    {
        private const string ContentTypeKey = "ContentType";
        private const string CultureNameKey = "CultureName";
        private const string FlagsKey = "Flags";
        private const string FullNameKey = "FullName";
        private const string NameKey = "Name";
        private const string VersionKey = "Version";

        public AssemblyContentType? ContentType { get; set; }
        public string? CultureName { get; set; }
        public AssemblyNameFlags? Flags { get; set; }
        public string? FullName { get; set; }
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

            info.AddValue(ContentTypeKey, ContentType);
            info.AddValue(CultureNameKey, CultureName);
            info.AddValue(FlagsKey, Flags);
            info.AddValue(FullNameKey, FullName);
            info.AddValue(NameKey, Name);
            info.AddValue(VersionKey, Version);
        }

        private AssemblyNameInfo(SerializationInfo info, StreamingContext streamingContext)
        {
            ArgumentNullException.ThrowIfNull(info);

            CultureName = (string?)info.GetValue(CultureNameKey, type: typeof(string));
            Name = (string?)info.GetValue(NameKey, type: typeof(string));
            Version = (Version?)info.GetValue(VersionKey, type: typeof(Version));
            FullName = (string?)info.GetValue(FullNameKey, type: typeof(string));
            Flags = (AssemblyNameFlags?)info.GetValue(FlagsKey, type: typeof(AssemblyNameFlags));
            ContentType = (AssemblyContentType?)info.GetValue(ContentTypeKey, type: typeof(AssemblyContentType));
        }
    }
}
