// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using Microsoft.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods and fields to manage the input language.
/// </summary>
public sealed class InputLanguage
{
    /// <summary>
    ///  The HKL handle.
    /// </summary>
    private readonly nint _handle;

    internal InputLanguage(IntPtr handle)
    {
        _handle = handle;
    }

    /// <summary>
    ///  Returns the culture of the current input language.
    /// </summary>
    public CultureInfo Culture => new(LanguageTag);

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
            PInvokeCore.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_GETDEFAULTINPUTLANG, ref handle);
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

            fixed (HKL* h = handles)
            {
                PInvoke.GetKeyboardLayoutList(size, h);
            }

            InputLanguage[] ils = new InputLanguage[size];
            for (int i = 0; i < size; i++)
            {
                ils[i] = new InputLanguage(handles[i]);
            }

            return new InputLanguageCollection(ils);
        }
    }

    private const string KeyboardLayoutsRegistryPath = @"SYSTEM\CurrentControlSet\Control\Keyboard Layouts";

    /// <summary>
    ///  Returns the name of the current keyboard layout as it appears in the Windows
    ///  Regional Settings on the computer.
    /// </summary>
    public string LayoutName
    {
        get
        {
            // https://learn.microsoft.com/windows/win32/intl/using-registry-string-redirection#create-resources-for-keyboard-layout-strings
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey($@"{KeyboardLayoutsRegistryPath}\{LayoutId}");
            return key.GetMUIString("Layout Display Name", "Layout Text") ?? SR.UnknownInputLanguageLayout;
        }
    }

    /// <summary>
    ///  Returns the
    ///  <see href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-getkeyboardlayoutnamew">
    ///   keyboard layout identifier
    ///  </see>
    ///  of the current input language.
    /// </summary>
    /// <seealso href="https://learn.microsoft.com/windows-hardware/manufacture/desktop/windows-language-pack-default-values">
    ///  Keyboard identifiers and input method editors for Windows
    /// </seealso>
    internal string LayoutId
    {
        get
        {
            // There is no good way to do this in Windows. GetKeyboardLayoutName does what we want, but only for the
            // current input language; setting and resetting the current input language would generate spurious
            // InputLanguageChanged events. Try to extract needed information manually.

            // High word of HKL contains a device handle to the physical layout of the keyboard but exact format of this
            // handle is not documented. For older keyboard layouts device handle seems contains keyboard layout
            // identifier.
            int device = PARAM.HIWORD(_handle);

            // But for newer keyboard layouts device handle contains special layout id if its high nibble is 0xF. This
            // id may be used to search for keyboard layout under registry.
            //
            // NOTE: this logic may break in future versions of Windows since it is not documented.
            if ((device & 0xF000) == 0xF000)
            {
                // Extract special layout id from the device handle
                int layoutId = device & 0x0FFF;

                using RegistryKey? key = Registry.LocalMachine.OpenSubKey(KeyboardLayoutsRegistryPath);
                if (key is not null)
                {
                    // Match keyboard layout by layout id
                    foreach (string subKeyName in key.GetSubKeyNames())
                    {
                        using RegistryKey? subKey = key.OpenSubKey(subKeyName);
                        if (subKey is not null
                            && subKey.GetValue("Layout Id") is string subKeyLayoutId
                            && Convert.ToInt32(subKeyLayoutId, 16) == layoutId)
                        {
                            Debug.Assert(subKeyName.Length == 8, $"unexpected key length in registry: {subKey.Name}");
                            return subKeyName.ToUpperInvariant();
                        }
                    }
                }
            }
            else
            {
                // Use input language only if keyboard layout language is not available. This is crucial in cases when
                // keyboard is installed more than once or under different languages. For example when French keyboard
                // is installed under US input language we need to return French keyboard identifier.
                if (device == 0)
                {
                    // According to the GetKeyboardLayout API function docs low word of HKL contains input language.
                    device = PARAM.LOWORD(_handle);
                }
            }

            return device.ToString("X8");
        }
    }

    private const string UserProfileRegistryPath = @"Control Panel\International\User Profile";

    /// <summary>
    ///  Returns the
    ///  <see href="https://learn.microsoft.com/globalization/locale/standard-locale-names">
    ///   BCP 47 language tag
    ///  </see>
    ///  of the current input language.
    /// </summary>
    private string LanguageTag
    {
        get
        {
            // According to the GetKeyboardLayout API function docs low word of HKL contains input language identifier.
            int langId = PARAM.LOWORD(_handle);

            // We need to convert the language identifier to a language tag, because they are deprecated and may have a
            // transient value.
            // https://learn.microsoft.com/globalization/locale/other-locale-names#lcid
            // https://learn.microsoft.com/windows/win32/winmsg/wm-inputlangchange#remarks
            //
            // It turns out that the LCIDToLocaleName API, which is used inside CultureInfo, may return incorrect
            // language tags for transient language identifiers. For example, it returns "nqo-GN" and "jv-Java-ID"
            // instead of the "nqo" and "jv-Java" (as seen in the Get-WinUserLanguageList PowerShell cmdlet).
            //
            // Try to extract proper language tag from registry as a workaround approved by a Windows team.
            // https://github.com/dotnet/winforms/pull/8573#issuecomment-1542600949
            //
            // NOTE: this logic may break in future versions of Windows since it is not documented.
            if (langId is (int)PInvoke.LOCALE_TRANSIENT_KEYBOARD1
                or (int)PInvoke.LOCALE_TRANSIENT_KEYBOARD2
                or (int)PInvoke.LOCALE_TRANSIENT_KEYBOARD3
                or (int)PInvoke.LOCALE_TRANSIENT_KEYBOARD4)
            {
                using RegistryKey? key = Registry.CurrentUser.OpenSubKey(UserProfileRegistryPath);
                if (key is not null && key.GetValue("Languages") is string[] languages)
                {
                    foreach (string language in languages)
                    {
                        using RegistryKey? subKey = key.OpenSubKey(language);
                        if (subKey is not null
                            && subKey.GetValue("TransientLangId") is int transientLangId
                            && transientLangId == langId)
                        {
                            return language;
                        }
                    }
                }
            }

            return CultureInfo.GetCultureInfo(langId).Name;
        }
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
        InputLanguage inputLanguage = new(m.LParamInternal);

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

        foreach (InputLanguage? lang in InstalledInputLanguages)
        {
            if (culture.Equals(lang?.Culture))
            {
                return lang;
            }
        }

        return null;
    }

    /// <summary>
    ///  Hash code for this input language.
    /// </summary>
    public override int GetHashCode() => (int)_handle;
}
