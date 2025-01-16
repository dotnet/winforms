// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Core.OLE;

/// <summary>
///  Specifies the formats that can be used with Clipboard.GetText and
///  Clipboard.SetText methods
/// </summary>
internal enum DesktopTextDataFormat
{
    Text,
    UnicodeText,
    Rtf,
    Html,
    CommaSeparatedValue
}
