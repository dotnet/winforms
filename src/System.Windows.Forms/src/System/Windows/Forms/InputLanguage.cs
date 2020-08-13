// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text;
using Microsoft.Win32;
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
        public CultureInfo Culture => new CultureInfo((int)_handle & 0xFFFF);

        /// <summary>
        ///  Gets or sets the input language for the current thread.
        /// </summary>
        [AllowNull]
        public static InputLanguage CurrentInputLanguage
        {
            get
            {
                Application.OleRequired();
                return new InputLanguage(User32.GetKeyboardLayout(0));
            }
            set
            {
                // OleInitialize needs to be called before we can call ActivateKeyboardLayout.
                Application.OleRequired();
                if (value is null)
                {
                    value = DefaultInputLanguage;
                }
                IntPtr handleOld = User32.ActivateKeyboardLayout(value.Handle, 0);
                if (handleOld == IntPtr.Zero)
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
                IntPtr handle = IntPtr.Zero;
                User32.SystemParametersInfoW(User32.SPI.GETDEFAULTINPUTLANG, ref handle);
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
        public unsafe static InputLanguageCollection InstalledInputLanguages
        {
            get
            {
                int size = User32.GetKeyboardLayoutList(0, null);

                var handles = new IntPtr[size];
                fixed (IntPtr *pHandles = handles)
                {
                    User32.GetKeyboardLayoutList(size, pHandles);
                }

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

                /*
                            HKL is a 32 bit value. HIWORD is a Device Handle. LOWORD is Language ID.
                HKL

                +------------------------+-------------------------+
                |     Device Handle      |       Language ID       |
                +------------------------+-------------------------+
                31                     16 15                      0   bit

                Language ID
                +---------------------------+-----------------------+
                |     Sublanguage ID        | Primary Language ID   |
                +---------------------------+-----------------------+
                15                        10 9                     0   bit
                WORD LangId  = MAKELANGID(primary, sublang)
                BYTE primary = PRIMARYLANGID(LangId)
                BYTE sublang = PRIMARYLANGID(LangID)

                How Preload is interpreted: example US-Dvorak
                Look in HKEY_CURRENT_USER\Keyboard Layout\Preload
                Name="4"  (may vary)
                Value="d0000409"  -> Language ID = 0409
                Look in HKEY_CURRENT_USER\Keyboard Layout\Substitutes
                Name="d0000409"
                Value="00010409"
                Look in HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Keyboard Layouts\00010409
                "Layout File": name of keyboard layout DLL (KBDDV.DLL)
                "Layout Id": ID of this layout (0002)
                Windows will change the top nibble of layout ID to F, which makes F002.
                Combined with Language ID, the final HKL is F0020409.
                */

                IntPtr currentHandle = _handle;
                int language = unchecked((int)(long)currentHandle) & 0xffff;
                int device = (unchecked((int)(long)currentHandle) >> 16) & 0x0fff;

                if (device == language || device == 0)
                {
                    // Default keyboard for language
                    string keyName = Convert.ToString(language, 16);
                    keyName = PadWithZeroes(keyName, 8);
                    using RegistryKey? key = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Keyboard Layouts\\" + keyName);

                    // Attempt to extract the localized keyboard layout name using the SHLoadIndirectString API.
                    // Default back to our legacy codepath and obtain the name
                    // directly through the registry value
                    return GetLocalizedKeyboardLayoutName(key?.GetValue("Layout Display Name") as string)
                        ?? (string?)key?.GetValue("Layout Text")
                        ?? SR.UnknownInputLanguageLayout;
                }

                // Look for a substitution
                RegistryKey? substitutions = Registry.CurrentUser.OpenSubKey("Keyboard Layout\\Substitutes");
                string[]? encodings = null;
                if (substitutions != null)
                {
                    encodings = substitutions.GetValueNames();

                    foreach (string encoding in encodings)
                    {
                        int encodingValue = Convert.ToInt32(encoding, 16);
                        if (encodingValue == unchecked((int)(long)currentHandle) ||
                            (encodingValue & 0x0FFFFFFF) == (unchecked((int)(long)currentHandle) & 0x0FFFFFFF) ||
                            (encodingValue & 0xFFFF) == language)
                        {
                            string? encodingSubstitution = (string?)substitutions.GetValue(encoding);
                            if (encodingSubstitution is null)
                            {
                                continue;
                            }

                            currentHandle = (IntPtr)Convert.ToInt32(encodingSubstitution, 16);
                            language = unchecked((int)(long)currentHandle) & 0xFFFF;
                            device = (unchecked((int)(long)currentHandle) >> 16) & 0xFFF;
                            break;
                        }
                    }

                    substitutions.Close();
                }

                using RegistryKey? layouts = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Control\\Keyboard Layouts");
                if (layouts != null)
                {
                    encodings = layouts.GetSubKeyNames();

                    // Check to see if the encoding directly matches the handle -- some do.
                    foreach (string encoding in encodings)
                    {
                        Debug.Assert(encoding.Length == 8, "unexpected key in registry: hklm\\SYSTEM\\CurrentControlSet\\Control\\Keyboard Layouts\\" + encoding);
                        if (currentHandle == (IntPtr)Convert.ToInt32(encoding, 16))
                        {
                            using RegistryKey? key = layouts.OpenSubKey(encoding);
                            if (key is null)
                            {
                                continue;
                            }

                            // Attempt to extract the localized keyboard layout name using the SHLoadIndirectString API.
                            string? layoutName = GetLocalizedKeyboardLayoutName(key.GetValue("Layout Display Name") as string);

                            // Default back to our legacy codepath and obtain the name
                            // directly through the registry value
                            if (layoutName is null)
                            {
                                layoutName = (string?)key.GetValue("Layout Text");
                            }

                            if (layoutName != null)
                            {
                                return layoutName;
                            }

                            break;
                        }
                    }

                    // No luck there.  Match the language first, then try to find a layout ID
                    foreach (string encoding in encodings)
                    {
                        Debug.Assert(encoding.Length == 8, "unexpected key in registry: hklm\\SYSTEM\\CurrentControlSet\\Control\\Keyboard Layouts\\" + encoding);
                        if (language == (0xffff & Convert.ToInt32(encoding.Substring(4, 4), 16)))
                        {
                            using RegistryKey? key = layouts.OpenSubKey(encoding);
                            if (key is null)
                            {
                                continue;
                            }

                            string? codeValue = (string?)key.GetValue("Layout Id");
                            if (codeValue is null)
                            {
                                continue;
                            }

                            int value = Convert.ToInt32(codeValue, 16);
                            if (value == device)
                            {
                                // Attempt to extract the localized keyboard layout name using the SHLoadIndirectString API.
                                string? layoutName = GetLocalizedKeyboardLayoutName(key.GetValue("Layout Display Name") as string);

                                // Default back to our legacy codepath and obtain the name
                                // directly through the registry value
                                if (layoutName is null)
                                {
                                    layoutName = (string?)key.GetValue("Layout Text");
                                }

                                if (layoutName != null)
                                {
                                    return layoutName;
                                }
                            }
                        }
                    }
                }

                return SR.UnknownInputLanguageLayout;
            }
        }

        /// <summary>
        ///  Attempts to extract the localized keyboard layout name using the SHLoadIndirectString API.
        ///  Returning null from this method will force us to use the legacy codepath (pulling the text
        ///  directly from the registry).
        /// </summary>
        private static string? GetLocalizedKeyboardLayoutName(string? layoutDisplayName)
        {
            if (layoutDisplayName != null)
            {
                var sb = new StringBuilder(512);
                HRESULT res = Shlwapi.SHLoadIndirectString(layoutDisplayName, sb, (uint)sb.Capacity, IntPtr.Zero);
                if (res == HRESULT.S_OK)
                {
                    return sb.ToString();
                }
            }

            return null;
        }

        /// <summary>
        ///  Creates an InputLanguageChangedEventArgs given a windows message.
        /// </summary>
        internal static InputLanguageChangedEventArgs CreateInputLanguageChangedEventArgs(Message m)
        {
            return new InputLanguageChangedEventArgs(new InputLanguage(m.LParam), unchecked((byte)(long)m.WParam));
        }

        /// <summary>
        ///  Creates an InputLanguageChangingEventArgs given a windows message.
        /// </summary>
        internal static InputLanguageChangingEventArgs CreateInputLanguageChangingEventArgs(Message m)
        {
            var inputLanguage = new InputLanguage(m.LParam);

            // NOTE: by default we should allow any locale switch
            bool localeSupportedBySystem = m.WParam != IntPtr.Zero;
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
            if (culture is null)
            {
                throw new ArgumentNullException(nameof(culture));
            }

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

        private static string PadWithZeroes(string input, int length)
        {
            return "0000000000000000".Substring(0, length - input.Length) + input;
        }
    }
}
