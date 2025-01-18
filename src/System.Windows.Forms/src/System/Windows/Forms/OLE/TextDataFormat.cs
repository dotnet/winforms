// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using PrivateOle = System.Private.Windows.Core.Ole;

namespace System.Windows.Forms;

/// <summary>
///  Specifies the formats used with text-related methods of the
///  <see cref="Clipboard"/> and <see cref="DataObject"/> classes.
/// </summary>
public enum TextDataFormat
{
    /// <inheritdoc cref="PrivateOle.TextDataFormat.Text"/>
    Text = PrivateOle.TextDataFormat.Text,

    /// <inheritdoc cref="PrivateOle.TextDataFormat.UnicodeText"/>
    UnicodeText = PrivateOle.TextDataFormat.UnicodeText,

    /// <inheritdoc cref="PrivateOle.TextDataFormat.Rtf"/>
    Rtf = PrivateOle.TextDataFormat.Rtf,

    /// <inheritdoc cref="PrivateOle.TextDataFormat.Html"/>
    Html = PrivateOle.TextDataFormat.Html,

    /// <inheritdoc cref="PrivateOle.TextDataFormat.CommaSeparatedValue"/>
    CommaSeparatedValue = PrivateOle.TextDataFormat.CommaSeparatedValue
}
