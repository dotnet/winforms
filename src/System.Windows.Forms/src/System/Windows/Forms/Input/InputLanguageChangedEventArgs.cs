// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="Form.InputLanguageChanged"/> event.
/// </summary>
public class InputLanguageChangedEventArgs : EventArgs
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="InputLanguageChangedEventArgs"/> class with the
    ///  specified locale and character set.
    /// </summary>
    public InputLanguageChangedEventArgs(CultureInfo culture, byte charSet)
    {
        InputLanguage? language = InputLanguage.FromCulture(culture)
            ?? throw new ArgumentException(string.Format(SR.InputLanguageCultureNotFound, culture), nameof(culture));

        InputLanguage = language;
        Culture = culture;
        CharSet = charSet;
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="InputLanguageChangedEventArgs"/> class with the specified input language and
    ///  character set.
    /// </summary>
    public InputLanguageChangedEventArgs(InputLanguage inputLanguage, byte charSet)
    {
        InputLanguage = inputLanguage.OrThrowIfNull();
        Culture = inputLanguage.Culture;
        CharSet = charSet;
    }

    /// <summary>
    ///  Gets the input language.
    /// </summary>
    public InputLanguage InputLanguage { get; }

    /// <summary>
    ///  Gets the locale of the input language.
    /// </summary>
    public CultureInfo Culture { get; }

    /// <summary>
    ///  Gets the character set associated with the new input language.
    /// </summary>
    public byte CharSet { get; }
}
