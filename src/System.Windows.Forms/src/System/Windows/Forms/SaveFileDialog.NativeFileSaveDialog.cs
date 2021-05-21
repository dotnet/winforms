// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop.Shell32;

namespace System.Windows.Forms
{
    public sealed partial class SaveFileDialog
    {
        [ComImport]
        [Guid("84bccd23-5fde-4cdb-aea4-af64b83d78ab")]
        [CoClass(typeof(FileSaveDialogRCW))]
        private interface NativeFileSaveDialog : IFileSaveDialog
        {
        }
    }
}
