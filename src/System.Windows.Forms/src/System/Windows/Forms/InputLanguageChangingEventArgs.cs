// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='Form.InputLanguageChanging'/> event.
    /// </summary>
    public class InputLanguageChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref='InputLanguageChangingEventArgs'/> class with the
        ///  specified locale, character set, and acceptance.
        /// </summary>
        public InputLanguageChangingEventArgs(CultureInfo culture, bool sysCharSet)
        {
            InputLanguage? language = InputLanguage.FromCulture(culture);
            if (language is null)
            {
                throw new ArgumentException(string.Format(SR.InputLanguageCultureNotFound, culture), nameof(culture));
            }

            InputLanguage = language;
            Culture = culture;
            SysCharSet = sysCharSet;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='InputLanguageChangingEventArgs'/> class with the
        ///  specified input language, character set, and acceptance of a language change.
        /// </summary>
        public InputLanguageChangingEventArgs(InputLanguage inputLanguage, bool sysCharSet)
        {
            InputLanguage = inputLanguage ?? throw new ArgumentNullException(nameof(inputLanguage));
            Culture = inputLanguage.Culture;
            SysCharSet = sysCharSet;
        }

        /// <summary>
        ///  Gets the requested input language.
        /// </summary>
        public InputLanguage InputLanguage { get; }

        /// <summary>
        ///  Gets the locale of the requested input language.
        /// </summary>
        public CultureInfo Culture { get; }

        /// <summary>
        ///  Gets a value indicating whether the system default font supports the character set
        ///  required for the requested input language.
        /// </summary>
        public bool SysCharSet { get; }
    }
}
