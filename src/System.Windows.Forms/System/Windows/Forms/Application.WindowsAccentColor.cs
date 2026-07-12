// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.WinRT;
using Windows.Win32.UI.ViewManagement;

namespace System.Windows.Forms;

public sealed partial class Application
{
    /// <summary>
    ///  Gets the user's current Windows accent color.
    /// </summary>
    /// <returns>
    ///  The accent color the user has selected in the Windows personalization settings.
    /// </returns>
    /// <remarks>
    ///  <para>
    ///   The value is read from the Windows <c>UISettings</c> runtime component and reflects the color the
    ///   user picked (or that Windows derived automatically from the desktop background). If the user has not
    ///   chosen an accent color, Windows returns a default accent color defined by the operating system, so
    ///   this method always yields a usable color rather than throwing or returning an empty value.
    ///  </para>
    /// </remarks>
    public static unsafe Color GetWindowsAccentColor()
    {
        HSTRING className = default;

        fixed (char* pClassName = "Windows.UI.ViewManagement.UISettings")
        {
            PInvokeCore.WindowsCreateString((PCWSTR)pClassName, 36u, &className).ThrowOnFailure();
        }

        try
        {
            using ComScope<IInspectable> inspectable = new(null);
            PInvokeCore.RoActivateInstance(className, inspectable).ThrowOnFailure();

            using ComScope<IUISettings3> settings = inspectable.TryQuery<IUISettings3>(out HRESULT hr);
            hr.ThrowOnFailure();

            UIColor color;
            settings.Value->GetColorValue(UIColorType.Accent, &color).ThrowOnFailure();

            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        finally
        {
            PInvokeCore.WindowsDeleteString(className);
        }
    }
}
