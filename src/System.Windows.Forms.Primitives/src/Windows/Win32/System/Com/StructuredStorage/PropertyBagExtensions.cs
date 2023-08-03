// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;

namespace Windows.Win32.System.Com.StructuredStorage;

internal static class PropertyBagExtensions
{
    /// <inheritdoc cref="IPropertyBag.Interface.Read(PCWSTR, VARIANT*, IErrorLog*)"/>
    internal static unsafe HRESULT Read(this IPropertyBag.Interface @this, string pszPropName, VARIANT* pVar, IErrorLog* pErrorLog)
    {
        fixed (char* c = pszPropName)
        {
            return @this.Read(c, pVar, pErrorLog);
        }
    }

    /// <inheritdoc cref="IPropertyBag.Interface.Write(PCWSTR, VARIANT*)"/>
    internal static unsafe HRESULT Write(this IPropertyBag.Interface @this, string pszPropName, VARIANT* pVar)
    {
        fixed (char* c = pszPropName)
        {
            return @this.Write(c, pVar);
        }
    }
}
