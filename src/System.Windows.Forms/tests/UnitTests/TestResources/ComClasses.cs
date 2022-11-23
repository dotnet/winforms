// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace System.Windows.Forms.Tests.TestResources;

internal static class ComClasses
{
    internal static ComClassFactory? VisualBasicSimpleControl { get; }
        = RuntimeInformation.ProcessArchitecture == Architecture.X86
            ? new ComClassFactory(
                // This is actually a .ocx
                Path.GetFullPath(@"TestResources\VB6\SimpleControl.vb6"),
                new Guid("366D0F1F-2D8A-4C9B-897D-CE1E74EDD6E5"))
            : null;
}
