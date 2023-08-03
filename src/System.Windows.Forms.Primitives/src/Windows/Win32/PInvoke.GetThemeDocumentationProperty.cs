// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    public static unsafe string GetThemeDocumentationProperty(string pszThemeName, string pszPropertyName)
    {
        Span<char> buffer = stackalloc char[512];
        fixed (char* pBuffer = buffer)
        {
            GetThemeDocumentationProperty(pszThemeName, pszPropertyName, pBuffer, buffer.Length);
        }

        return buffer.SliceAtFirstNull().ToString();
    }
}
