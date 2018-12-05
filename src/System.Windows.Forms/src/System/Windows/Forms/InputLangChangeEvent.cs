// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.InteropServices;

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.Globalization;
    using System.Windows.Forms;
    using System.ComponentModel;
    using Microsoft.Win32;

    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.Form.InputLanguageChanged'/>
    ///       event.
    ///    </para>
    /// </devdoc>
    public class InputLanguageChangedEventArgs : EventArgs {

        /// <devdoc>
        ///     The input language.
        /// </devdoc>
        private readonly InputLanguage inputLanguage;

        /// <devdoc>
        ///     The culture of the input language.
        /// </devdoc>
        private readonly CultureInfo culture;
        /// <devdoc>
        ///     The charSet associated with the new input language.
        /// </devdoc>
        private readonly byte   charSet;

        /**
         * @deprecated.  Use the other constructor instead.
         */
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.InputLanguageChangedEventArgs'/> class with the
        ///       specified locale and character set.
        ///    </para>
        /// </devdoc>
        public InputLanguageChangedEventArgs(CultureInfo culture, byte charSet) {
            this.inputLanguage = System.Windows.Forms.InputLanguage.FromCulture(culture);
            this.culture = culture;
            this.charSet = charSet;
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.InputLanguageChangedEventArgs'/>class with the specified input language and
        ///       character set.
        ///    </para>
        /// </devdoc>
        public InputLanguageChangedEventArgs(InputLanguage inputLanguage, byte charSet) {
            this.inputLanguage = inputLanguage;
            this.culture = inputLanguage.Culture;
            this.charSet = charSet;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the input language.
        ///    </para>
        /// </devdoc>
        public InputLanguage InputLanguage {
            get {
                return inputLanguage;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the locale of the input language.
        ///    </para>
        /// </devdoc>
        public CultureInfo Culture {
            get {
                return culture;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets the character set associated with the new input language.
        ///    </para>
        /// </devdoc>
        public byte CharSet {
            get {
                  return charSet;
            }
        }
    }
}
