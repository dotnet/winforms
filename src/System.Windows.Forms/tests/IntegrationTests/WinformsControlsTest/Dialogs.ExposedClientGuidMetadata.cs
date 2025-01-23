// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

namespace WinFormsControlsTest;

public partial class Dialogs
{
    /// <summary>
    /// Changes the <see cref="ClientGuid"/> metadata to be configurable in the <see cref="PropertyGrid"/> used
    /// to prepare tests of <see cref="FileDialog"/> and <see cref="FolderBrowserDialog"/>.
    /// </summary>
    private sealed class ExposedClientGuidMetadata
    {
        [Browsable(true)]
        [TypeConverter(typeof(ClientGuidConverter))]
        public Guid? ClientGuid { get; set; }
    }
}
