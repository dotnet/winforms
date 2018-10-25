// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.CompilerServices;

    internal static class LocalAppContextSwitches {
        internal const string DontSupportReentrantFilterMessageSwitchName = @"Switch.System.Windows.Forms.DontSupportReentrantFilterMessage";
        internal const string DoNotSupportSelectAllShortcutInMultilineTextBoxSwitchName = @"Switch.System.Windows.Forms.DoNotSupportSelectAllShortcutInMultilineTextBox";
        internal const string DoNotLoadLatestRichEditControlSwitchName = @"Switch.System.Windows.Forms.DoNotLoadLatestRichEditControl";
        internal const string UseLegacyContextMenuStripSourceControlValueSwitchName = @"Switch.System.Windows.Forms.UseLegacyContextMenuStripSourceControlValue";
        internal const string DomainUpDownUseLegacyScrollingSwitchName = @"Switch.System.Windows.Forms.DomainUpDown.UseLegacyScrolling";
        internal const string AllowUpdateChildControlIndexForTabControlsSwitchName = @"Switch.System.Windows.Forms.AllowUpdateChildControlIndexForTabControls";
        internal const string UseLegacyImagesSwitchName = @"Switch.System.Windows.Forms.UseLegacyImages";
        internal const string EnableVisualStyleValidationSwitchName = @"Switch.System.Windows.Forms.EnableVisualStyleValidation";
        private static int _dontSupportReentrantFilterMessage;
        private static int _doNotSupportSelectAllShortcutInMultilineTextBox;
        private static int _doNotLoadLatestRichEditControl;
        private static int _useLegacyContextMenuStripSourceControlValue;
        private static int _useLegacyDomainUpDownScrolling;
        private static int _allowUpdateChildControlIndexForTabControls;
        private static int _useLegacyImages;
        private static int _enableVisualStyleValidation;

        public static bool DontSupportReentrantFilterMessage {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return LocalAppContext.GetCachedSwitchValue(LocalAppContextSwitches.DontSupportReentrantFilterMessageSwitchName, ref _dontSupportReentrantFilterMessage);
            }
        }

        public static bool DoNotSupportSelectAllShortcutInMultilineTextBox {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return LocalAppContext.GetCachedSwitchValue(LocalAppContextSwitches.DoNotSupportSelectAllShortcutInMultilineTextBoxSwitchName, ref _doNotSupportSelectAllShortcutInMultilineTextBox);
            }
        }

        public static bool DoNotLoadLatestRichEditControl {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return LocalAppContext.GetCachedSwitchValue(LocalAppContextSwitches.DoNotLoadLatestRichEditControlSwitchName, ref _doNotLoadLatestRichEditControl);
            }
        }

        public static bool UseLegacyContextMenuStripSourceControlValue {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return LocalAppContext.GetCachedSwitchValue(LocalAppContextSwitches.UseLegacyContextMenuStripSourceControlValueSwitchName, ref _useLegacyContextMenuStripSourceControlValue);
            }
        }

        public static bool UseLegacyDomainUpDownControlScrolling {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return LocalAppContext.GetCachedSwitchValue(LocalAppContextSwitches.DomainUpDownUseLegacyScrollingSwitchName, ref _useLegacyDomainUpDownScrolling);
            }
        }

        public static bool AllowUpdateChildControlIndexForTabControls {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return LocalAppContext.GetCachedSwitchValue(LocalAppContextSwitches.AllowUpdateChildControlIndexForTabControlsSwitchName, ref _allowUpdateChildControlIndexForTabControls);
            }
        }

        public static bool UseLegacyImages {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return LocalAppContext.GetCachedSwitchValue(LocalAppContextSwitches.UseLegacyImagesSwitchName, ref _useLegacyImages);
            }
        }

        public static bool EnableVisualStyleValidation {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get {
                return LocalAppContext.GetCachedSwitchValue(LocalAppContextSwitches.EnableVisualStyleValidationSwitchName, ref _enableVisualStyleValidation);
            }
        }
    }
}
