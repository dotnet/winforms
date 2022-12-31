﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Microsoft.Win32;
using Windows.Win32.UI.TextServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides methods and fields to manage the input language.
    /// </summary>
    public sealed class InputLanguage
    {
        /// <summary>
        ///  The HKL handle.
        /// </summary>
        private readonly IntPtr _handle;

        internal InputLanguage(IntPtr handle)
        {
            _handle = handle;
        }

        /// <summary>
        ///  Returns the culture of the current input language.
        /// </summary>
        public CultureInfo Culture => new CultureInfo(PARAM.ToInt(_handle) & 0xFFFF);

        /// <summary>
        ///  Gets or sets the input language for the current thread.
        /// </summary>
        [AllowNull]
        public static InputLanguage CurrentInputLanguage
        {
            get
            {
                Application.OleRequired();
                return new InputLanguage(PInvoke.GetKeyboardLayout(0));
            }
            set
            {
                // OleInitialize needs to be called before we can call ActivateKeyboardLayout.
                Application.OleRequired();
                value ??= DefaultInputLanguage;

                HKL handleOld = PInvoke.ActivateKeyboardLayout(new HKL(value.Handle), 0);
                if (handleOld == default)
                {
                    throw new ArgumentException(SR.ErrorBadInputLanguage, nameof(value));
                }
            }
        }

        /// <summary>
        ///  Returns the default input language for the system.
        /// </summary>
        public static InputLanguage DefaultInputLanguage
        {
            get
            {
                nint handle = 0;
                PInvoke.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_GETDEFAULTINPUTLANG, ref handle);
                return new InputLanguage(handle);
            }
        }

        /// <summary>
        ///  Returns the handle for the input language.
        /// </summary>
        public IntPtr Handle => _handle;

        /// <summary>
        ///  Returns a list of all installed input languages.
        /// </summary>
        public static unsafe InputLanguageCollection InstalledInputLanguages
        {
            get
            {
                int size = PInvoke.GetKeyboardLayoutList(0, null);

                var handles = new HKL[size];
                PInvoke.GetKeyboardLayoutList(handles);

                InputLanguage[] ils = new InputLanguage[size];
                for (int i = 0; i < size; i++)
                {
                    ils[i] = new InputLanguage(handles[i]);
                }

                return new InputLanguageCollection(ils);
            }
        }

        /// <summary>
        ///  Returns the name of the current keyboard layout as it appears in the Windows
        ///  Regional Settings on the computer.
        /// </summary>
        public string LayoutName
        {
            get
            {
                // There is no good way to do this in Windows.
                // GetKeyboardLayoutName does what we want, but only for the current input
                // language; setting and resetting the current input language would generate
                // spurious InputLanguageChanged events.
                // Try to extract needed information manually from the HKL.
                string layoutName = GetKeyboardLayoutNameForHKL(_handle);

                return GetLocalizedKeyboardLayoutName(layoutName);
            }
        }

        private static RegistryKey? KeyboardLayoutsRegistryKey
            => Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Keyboard Layouts");

        /// <summary>
        ///  Returns the KLID string for provided HKL handle.
        ///  Same as GetKeyboardLayoutName API but for any HKL.
        ///  See https://learn.microsoft.com/windows-hardware/manufacture/desktop/windows-language-pack-default-values
        /// </summary>
        internal static string GetKeyboardLayoutNameForHKL(IntPtr hkl)
        {
            // HKL is a 32 bit value.
            // HIWORD is a Device Handle.
            // LOWORD is Language Id.
            int language = unchecked((int)(long)hkl) & 0xFFFF;
            int device = (unchecked((int)(long)hkl) >> 16) & 0xFFFF;

            // Device Handle contains Layout Id if its high nibble is 0xF
            if ((device & 0xF000) == 0xF000)
            {
                // Extract Layout Id from the Device Handle
                int layoutId = device & 0x0FFF;

                if (KeyboardLayoutsRegistryKey is RegistryKey subKeys)
                {
                    // Match keyboard layout by Layout Id
                    foreach (string subKeyName in subKeys.GetSubKeyNames())
                    {
                        if (subKeys.OpenSubKey(subKeyName) is not RegistryKey subKey)
                        {
                            continue;
                        }

                        if (subKey.GetValue("Layout Id") is not string subKeyLayoutId)
                        {
                            continue;
                        }

                        if (layoutId == Convert.ToInt32(subKeyLayoutId, 16))
                        {
                            Debug.Assert(subKeyName.Length == 8, "unexpected key length in registry: " + subKey.Name);
                            return subKeyName;
                        }
                    }
                }
            }
            else
            {
                // Device Handle contains keyboard layout Language Id.
                // This is crucial when keyboard layout is installed more than once under different languages.
                if (device != 0)
                {
                    language = device;
                }
            }

            // Pad with zeros to its left to produce arbitrary length string
            return language.ToString("x8");
        }

        /// <summary>
        ///  Return localized keyboard layout name for provided KLID string.
        ///  See https://learn.microsoft.com/windows/win32/intl/using-registry-string-redirection#create-resources-for-keyboard-layout-strings
        /// </summary>
        private static string GetLocalizedKeyboardLayoutName(string layoutName)
        {
            if (KeyboardLayoutsRegistryKey?.OpenSubKey(layoutName) is RegistryKey key)
            {
                // Obtain localizable string resource associated with the keyboard layout
                // that should be passed to SHLoadIndirectString API.
                if (key.GetValue("Layout Display Name") is string layoutDisplayName)
                {
                    unsafe
                    {
                        var ppvReserved = (void*)IntPtr.Zero;
                        Span<char> buffer = stackalloc char[512];
                        fixed (char* pBuffer = buffer)
                        {
                            HRESULT res = PInvoke.SHLoadIndirectString(layoutDisplayName, pBuffer, (uint)buffer.Length, ref ppvReserved);
                            if (res == HRESULT.S_OK)
                            {
                                return buffer.SliceAtFirstNull().ToString();
                            }
                        }
                    }
                }

                // Fallback to human-readable name for backward compatibility
                if (key.GetValue("Layout Text") is string layoutText)
                {
                    return layoutText;
                }
            }

            return SR.UnknownInputLanguageLayout;
        }

        /// <summary>
        ///  Creates an InputLanguageChangedEventArgs given a windows message.
        /// </summary>
        internal static InputLanguageChangedEventArgs CreateInputLanguageChangedEventArgs(Message m)
        {
            return new InputLanguageChangedEventArgs(new InputLanguage(m.LParamInternal), (byte)(nint)m.WParamInternal);
        }

        /// <summary>
        ///  Creates an InputLanguageChangingEventArgs given a windows message.
        /// </summary>
        internal static InputLanguageChangingEventArgs CreateInputLanguageChangingEventArgs(Message m)
        {
            var inputLanguage = new InputLanguage(m.LParamInternal);

            // NOTE: by default we should allow any locale switch
            bool localeSupportedBySystem = m.WParamInternal != 0u;
            return new InputLanguageChangingEventArgs(inputLanguage, localeSupportedBySystem);
        }

        /// <summary>
        ///  Specifies whether two input languages are equal.
        /// </summary>
        public override bool Equals(object? value)
            => value is InputLanguage other && _handle == other._handle;

        /// <summary>
        ///  Returns the input language associated with the specified culture.
        /// </summary>
        public static InputLanguage? FromCulture(CultureInfo culture)
        {
            ArgumentNullException.ThrowIfNull(culture);

            // KeyboardLayoutId is the LCID for built-in cultures, but it
            // is the CU-preferred keyboard language for custom cultures.
            int lcid = culture.KeyboardLayoutId;

            foreach (InputLanguage? lang in InstalledInputLanguages)
            {
                if ((unchecked((int)(long)lang!._handle) & 0xFFFF) == lcid)
                {
                    return lang;
                }
            }

            return null;
        }

        /// <summary>
        ///  Hash code for this input language.
        /// </summary>
        public override int GetHashCode() => unchecked((int)(long)_handle);
    }
}
