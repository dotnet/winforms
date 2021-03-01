// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    public sealed partial class OpenFileDialog
    {
        [ComImport]
        [ClassInterface(ClassInterfaceType.None)]
        [TypeLibType(TypeLibTypeFlags.FCanCreate)]
        [Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")]
        internal class FileOpenDialogRCW
        {
        }
    }
}
