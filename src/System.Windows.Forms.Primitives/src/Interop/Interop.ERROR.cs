// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal partial class Interop
{
    // https://docs.microsoft.com/windows/win32/debug/system-error-codes--0-499-
    internal static class ERROR
    {
        public const int ACCESS_DENIED = 0x0005;
        public const int INVALID_HANDLE = 0x0006;
        public const int INVALID_PARAMETER = 0x0057;
        public const int INSUFFICIENT_BUFFER = 0x007A;
    }
}
