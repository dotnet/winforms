// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.UI.Controls.Dialogs;

// https://github.com/microsoft/win32metadata/issues/1299
internal unsafe struct OFNOTIFY
{
    public NMHDR hdr;
    public OPENFILENAME* lpOFN;
    public PWSTR pszFile;
}
