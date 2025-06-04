// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System;

/// <summary>
///  Facilitates temporarily changing the <see cref="CultureInfo.CurrentCulture"/> and <see cref="CultureInfo.CurrentUICulture"/>.
/// </summary>
public sealed class ThreadCultureChange : IDisposable
{
    private readonly CultureInfo _originalCulture = CultureInfo.CurrentCulture;
    private readonly CultureInfo _originalUICulture = CultureInfo.CurrentUICulture;

    public ThreadCultureChange(string? cultureName) :
        this(cultureName is not null ? new CultureInfo(cultureName) : null)
    {
    }

    public ThreadCultureChange(CultureInfo? newCulture) :
        this(newCulture, null)
    {
    }

    public ThreadCultureChange(CultureInfo? newCulture, CultureInfo? newUICulture)
    {
        if (newCulture is not null)
        {
            _originalCulture = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = newCulture;
        }

        if (newUICulture is not null)
        {
            _originalUICulture = CultureInfo.CurrentUICulture;
            CultureInfo.CurrentUICulture = newUICulture;
        }
    }

    public void Dispose()
    {
        CultureInfo.CurrentCulture = _originalCulture;
        CultureInfo.CurrentUICulture = _originalUICulture;
    }
}
