// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Controls.Dialogs;

// https://github.com/microsoft/win32metadata/issues/1300
internal unsafe struct OPENFILENAME
{
    public uint lStructSize;
    public HWND hwndOwner;
    public HINSTANCE hInstance;
    public PWSTR lpstrFilter;
    public PWSTR lpstrCustomFilter;
    public uint nMaxCustFilter;
    public uint nFilterIndex;
    public PWSTR lpstrFile;
    public uint nMaxFile;
    public PWSTR lpstrFileTitle;
    public uint nMaxFileTitle;
    public PWSTR lpstrInitialDir;
    public PWSTR lpstrTitle;
    public OPEN_FILENAME_FLAGS Flags;
    public ushort nFileOffset;
    public ushort nFileExtension;
    public PWSTR lpstrDefExt;
    public LPARAM lCustData;
    public void* lpfnHook;
    public PWSTR lpTemplateName;
    public unsafe void* pvReserved;
    public uint dwReserved;
    public OPEN_FILENAME_FLAGS_EX FlagsEx;
}
