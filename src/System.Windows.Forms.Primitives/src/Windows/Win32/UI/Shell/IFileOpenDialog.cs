// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.CompilerServices;
using Windows.Win32.System.Com;

namespace Windows.Win32.UI.Shell;

internal unsafe partial struct IFileOpenDialog : INativeGuid, IUnknown.Interface
{
    static Guid* INativeGuid.NativeGuid => (Guid*)Unsafe.AsPointer(ref Unsafe.AsRef(in Guid));
}
