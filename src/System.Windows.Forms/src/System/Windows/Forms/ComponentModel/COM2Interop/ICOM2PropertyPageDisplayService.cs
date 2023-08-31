// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  <para>
///   Defines a method that shows the property page for an ActiveX control.
///  </para>
///  <para>
///   This API supports the product infrastructure and is not intended to be used directly from your code.
///  </para>
/// </summary>
public interface ICom2PropertyPageDisplayService
{
    /// <summary>
    ///  <para>
    ///   Shows a property page for the specified object.
    ///  </para>
    ///  <para>
    ///   This API supports the product infrastructure and is not intended to be used directly from your code.
    ///  </para>
    /// </summary>
    /// <param name="title">The title of the property page.</param>
    /// <param name="component">The object for which the property page is created.</param>
    /// <param name="dispid">The DispID of the property that is highlighted when the property page is created.</param>
    /// <param name="pageGuid">The GUID for the property page.</param>
    /// <param name="parentHandle">The handle of the parent control of the property page.</param>
    void ShowPropertyPage(string title, object component, int dispid, Guid pageGuid, IntPtr parentHandle);
}
