// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using static Interop.Shell32;

namespace System.Windows.Forms
{
    public sealed partial class OpenFileDialog
    {
        [ComImport]
        [Guid("d57c7288-d4ad-4768-be02-9d969532d960")]
        [CoClass(typeof(FileOpenDialogRCW))]
        internal interface NativeFileOpenDialog : IFileOpenDialog
        {
        }
    }
}
