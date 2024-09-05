// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

// The original code was borrowed from https://github.com/xunit/samples.xunit/blob/93f87d5/UseCulture/UseCultureAttribute.cs
// Licensed under http://www.apache.org/licenses/LICENSE-2.0.

using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using Xunit.Sdk;

namespace Xunit;

/// <summary>
///  Apply this attribute to your test method to replace the <see cref="Thread.CurrentThread" />
///  <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" /> with another culture.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public class UseCultureAttribute : BeforeAfterTestAttribute
{
    private readonly Lazy<CultureInfo> _culture;
    private readonly Lazy<CultureInfo> _uiCulture;

    // These will be set to a value that is never null in the Before method
    // which will always run before it is used in the After method.
    private CultureInfo _originalCulture = null!;
    private CultureInfo _originalUICulture = null!;

    private bool _updateUnmanagedUiThreadCulture;

    /// <summary>
    ///  Replaces the culture and UI culture of the current thread with <paramref name="culture" />.
    /// </summary>
    /// <param name="culture">The name of the culture to set for both <see cref="Culture" /> and <see cref="UICulture" />.</param>
    public UseCultureAttribute(string culture)
        : this(culture, culture)
    {
    }

    /// <summary>
    ///  Replaces the culture and UI culture of the current thread with <paramref name="culture" /> and <paramref name="uiCulture" />.
    /// </summary>
    /// <param name="culture">The name of the culture.</param>
    /// <param name="uiCulture">The name of the UI culture.</param>
    public UseCultureAttribute(string culture, string uiCulture)
    {
        _culture = new Lazy<CultureInfo>(() => new(culture, useUserOverride: false));
        _uiCulture = new Lazy<CultureInfo>(() => new(uiCulture, useUserOverride: false));
    }

    /// <summary>
    /// Gets the culture.
    /// </summary>
    public CultureInfo Culture => _culture.Value;

    /// <summary>
    ///  Indicates whether the native thread UI culture should also be set.
    /// </summary>
    public bool SetUnmanagedUiThreadCulture { get; set; }

    /// <summary>
    /// Gets the UI culture.
    /// </summary>
    public CultureInfo UICulture => _uiCulture.Value;

    /// <summary>
    /// Stores the current <see cref="Thread.CurrentPrincipal" />
    /// <see cref="CultureInfo.CurrentCulture" /> and <see cref="CultureInfo.CurrentUICulture" />
    /// and replaces them with the new cultures defined in the constructor.
    /// </summary>
    /// <param name="methodUnderTest">The method under test</param>
    public override unsafe void Before(MethodInfo methodUnderTest)
    {
        _originalCulture = Thread.CurrentThread.CurrentCulture;
        _originalUICulture = Thread.CurrentThread.CurrentUICulture;

        CultureInfo.DefaultThreadCurrentCulture = Culture;
        Thread.CurrentThread.CurrentCulture = Culture;
        Thread.CurrentThread.CurrentUICulture = UICulture;

        _updateUnmanagedUiThreadCulture = !_originalUICulture.Equals(UICulture);
        if (SetUnmanagedUiThreadCulture && _updateUnmanagedUiThreadCulture)
        {
            SetNativeUiThreadCulture(UICulture);
        }

        CultureInfo.CurrentCulture.ClearCachedData();
        CultureInfo.CurrentUICulture.ClearCachedData();
    }

    /// <summary>
    /// Restores the original <see cref="CultureInfo.CurrentCulture" /> and
    /// <see cref="CultureInfo.CurrentUICulture" /> to <see cref="Thread.CurrentPrincipal" />
    /// </summary>
    /// <param name="methodUnderTest">The method under test</param>
    public override void After(MethodInfo methodUnderTest)
    {
        Thread.CurrentThread.CurrentCulture = _originalCulture;
        Thread.CurrentThread.CurrentUICulture = _originalUICulture;

        if (SetUnmanagedUiThreadCulture && _updateUnmanagedUiThreadCulture)
        {
            SetNativeUiThreadCulture(_originalUICulture);
        }

        CultureInfo.CurrentCulture.ClearCachedData();
        CultureInfo.CurrentUICulture.ClearCachedData();
    }

    // Thread.CurrentThread.CurrentUICulture only sets the UI culture for the managed resources.
    private static unsafe void SetNativeUiThreadCulture(CultureInfo uiCulture)
    {
        uint pulNumLanguages = 0;
        string lcid = uiCulture.LCID.ToString("X4");
        fixed (char* plcid = lcid)
        {
            if (Interop.SetThreadPreferredUILanguages(Interop.MUI_LANGUAGE_ID, plcid, &pulNumLanguages) == Interop.BOOL.FALSE)
            {
                throw new InvalidOperationException("Unable to set the desired UI language.");
            }
        }
    }

    private static class Interop
    {
        internal const uint MUI_LANGUAGE_ID = 0x4;

        internal enum BOOL : int
        {
            FALSE = 0,
            TRUE = 1,
        }

        [DllImport("Kernel32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        internal static extern unsafe BOOL SetThreadPreferredUILanguages(
            uint dwFlags,
            char* pwszLanguagesBuffer,
            uint* pulNumLanguages);
    }
}
