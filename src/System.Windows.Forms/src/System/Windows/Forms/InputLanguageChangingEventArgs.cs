// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the <see cref='System.Windows.Forms.Form.InputLanguageChanging'/> event.
    /// </devdoc>
    public class InputLanguageChangingEventArgs : CancelEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.InputLanguageChangingEventArgs'/> class with the
        /// specified locale, character set, and acceptance.
        /// </devdoc>
        public InputLanguageChangingEventArgs(CultureInfo culture, bool sysCharSet)
        {
            InputLanguage = InputLanguage.FromCulture(culture);
            Culture = culture;
            SysCharSet = sysCharSet;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.InputLanguageChangingEventArgs'/> class with the
        /// specified input language, character set, and acceptance of a language change.
        /// </devdoc>
        public InputLanguageChangingEventArgs(InputLanguage inputLanguage, bool sysCharSet)
        {
            if (inputLanguage == null)
            {
                throw new ArgumentNullException(nameof(inputLanguage));
            }

            InputLanguage = inputLanguage;
            Culture = inputLanguage.Culture;
            SysCharSet = sysCharSet;
        }

        /// <summary>
        /// Gets the requested input language.
        /// </devdoc>
        public InputLanguage InputLanguage { get; }

        /// <summary>
        /// Gets the locale of the requested input language.
        /// </devdoc>
        public CultureInfo Culture { get; }

        /// <summary>
        /// Gets a value indicating whether the system default font supports the character set
        /// required for the requested input language.
        /// </devdoc>
        public bool SysCharSet { get; }
    }
}
