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

    /// <include file='doc\InputLangChangeRequestEvent.uex' path='docs/doc[@for="InputLanguageChangingEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.Form.InputLanguageChanging'/>
    ///       event.
    ///    </para>
    /// </devdoc>
    public class InputLanguageChangingEventArgs : CancelEventArgs {

        /// <include file='doc\InputLangChangeRequestEvent.uex' path='docs/doc[@for="InputLanguageChangingEventArgs.inputLanguage"]/*' />
        /// <devdoc>
        ///     The requested input language.
        /// </devdoc>
        private readonly InputLanguage inputLanguage;

        /// <include file='doc\InputLangChangeRequestEvent.uex' path='docs/doc[@for="InputLanguageChangingEventArgs.culture"]/*' />
        /// <devdoc>
        ///     The locale of the requested input langugage.
        /// </devdoc>
        private readonly CultureInfo culture;
        /// <include file='doc\InputLangChangeRequestEvent.uex' path='docs/doc[@for="InputLanguageChangingEventArgs.sysCharSet"]/*' />
        /// <devdoc>
        ///     Set to true if the system default font supports the character
        ///     set required for the requested input language.
        /// </devdoc>
        private readonly bool sysCharSet;

        /**
         * @deprecated Should use the new constructor instead.
         */
        /// <include file='doc\InputLangChangeRequestEvent.uex' path='docs/doc[@for="InputLanguageChangingEventArgs.InputLanguageChangingEventArgs"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.InputLanguageChangingEventArgs'/> class with the
        ///       specified locale, character set, and acceptance.
        ///    </para>
        /// </devdoc>
        public InputLanguageChangingEventArgs(CultureInfo culture, bool sysCharSet) {

            this.inputLanguage = System.Windows.Forms.InputLanguage.FromCulture(culture);
            this.culture = culture;
            this.sysCharSet = sysCharSet;
        }

        /// <include file='doc\InputLangChangeRequestEvent.uex' path='docs/doc[@for="InputLanguageChangingEventArgs.InputLanguageChangingEventArgs1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.InputLanguageChangingEventArgs'/> class with the
        ///       specified input language, character set, and acceptance of
        ///       a language change.
        ///    </para>
        /// </devdoc>
        public InputLanguageChangingEventArgs(InputLanguage inputLanguage, bool sysCharSet) {

            if (inputLanguage == null)
                throw new ArgumentNullException(nameof(inputLanguage));

            this.inputLanguage = inputLanguage;
            this.culture = inputLanguage.Culture;
            this.sysCharSet = sysCharSet;
        }

        /// <include file='doc\InputLangChangeRequestEvent.uex' path='docs/doc[@for="InputLanguageChangingEventArgs.InputLanguage"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the requested input language.
        ///    </para>
        /// </devdoc>
        public InputLanguage InputLanguage {
            get {
                return inputLanguage;
            }
        }

        /// <include file='doc\InputLangChangeRequestEvent.uex' path='docs/doc[@for="InputLanguageChangingEventArgs.Culture"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the locale of the requested input language.
        ///    </para>
        /// </devdoc>
        public CultureInfo Culture {
            get {
                return culture;
            }
        }

        /// <include file='doc\InputLangChangeRequestEvent.uex' path='docs/doc[@for="InputLanguageChangingEventArgs.SysCharSet"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the system default font supports the character
        ///       set required for the requested input language.
        ///    </para>
        /// </devdoc>
        public bool SysCharSet {
            get {
                return sysCharSet;
            }
        }
    }
}
