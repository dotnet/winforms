// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    internal enum NTSTATUS : int
    {
        STATUS_SUCCESS = 0x00000000,
        STATUS_PENDING = 0x00000103
    }
}
