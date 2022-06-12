﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        [LibraryImport(Libraries.UxTheme, EntryPoint = "GetThemeDocumentationProperty", StringMarshalling = StringMarshalling.Utf16)]
        private static unsafe partial int GetThemeDocumentationPropertyInternal(string pszThemeName, string pszPropertyName, char* pszValueBuff, int cchMaxValChars);

        public static unsafe string GetThemeDocumentationProperty(string pszThemeName, string pszPropertyName)
        {
            Span<char> buffer = stackalloc char[512];
            fixed (char* pBuffer = buffer)
            {
                GetThemeDocumentationPropertyInternal(pszThemeName, pszPropertyName, pBuffer, buffer.Length);
            }

            return buffer.SliceAtFirstNull().ToString();
        }

        public static class VisualStyleDocProperty
        {
            public const string DisplayName = "DisplayName";
            public const string Company = "Company";
            public const string Author = "Author";
            public const string Copyright = "Copyright";
            public const string Url = "Url";
            public const string Version = "Version";
            public const string Description = "Description";
        }
    }
}
