// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using Microsoft.Win32;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="RegLoadMUIString(System.Registry.HKEY, string, PWSTR, uint, uint*, uint, string)"/>
    [SkipLocalsInit]
    public static unsafe bool RegLoadMUIString(RegistryKey key, string keyName, out string localizedValue)
    {
        using BufferScope<char> buffer = new(stackalloc char[128]);

        while (true)
        {
            fixed (char* pszOutBuf = buffer)
            {
                uint bytes = 0;
                var errorCode = RegLoadMUIString(
                    (System.Registry.HKEY)key.Handle.DangerousGetHandle(),
                    keyName,
                    pszOutBuf,
                    (uint)(buffer.Length * sizeof(char)),
                    &bytes,
                    0,
                    null);

                // The buffer is too small. Try again with a larger buffer.
                if (errorCode == WIN32_ERROR.ERROR_MORE_DATA)
                {
                    buffer.EnsureCapacity((int)(bytes / sizeof(char)));
                    continue;
                }

                localizedValue = errorCode == WIN32_ERROR.ERROR_SUCCESS
                    ? buffer[..Math.Max((int)(bytes / sizeof(char)) - 1, 0)].ToString()
                    : string.Empty;

                return errorCode == WIN32_ERROR.ERROR_SUCCESS;
            }
        }
    }
}
