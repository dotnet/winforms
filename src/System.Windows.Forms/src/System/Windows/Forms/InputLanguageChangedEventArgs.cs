// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the <see cref='System.Windows.Forms.Form.InputLanguageChanged'/> event.
    /// </devdoc>
    public class InputLanguageChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.InputLanguageChangedEventArgs'/> class with the
        /// specified locale and character set.
        /// </devdoc>
        public InputLanguageChangedEventArgs(CultureInfo culture, byte charSet)
        {
            InputLanguage = InputLanguage.FromCulture(culture);
            Culture = culture;
            CharSet = charSet;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.InputLanguageChangedEventArgs'/>class with the specified input language and
        /// character set.
        /// </devdoc>
        public InputLanguageChangedEventArgs(InputLanguage inputLanguage, byte charSet)
        {
            if (inputLanguage == null)
            {
                throw new ArgumentNullException(nameof(inputLanguage));
            }

            InputLanguage = inputLanguage;
            Culture = inputLanguage.Culture;
            CharSet = charSet;
        }

        /// <summary>
        /// Gets the input language.
        /// </devdoc>
        public InputLanguage InputLanguage { get; }

        /// <summary>
        /// Gets the locale of the input language.
        /// </devdoc>
        public CultureInfo Culture { get; }

        /// <summary>
        /// Gets the character set associated with the new input language.
        /// </devdoc>
        public byte CharSet { get; }
    }
}
