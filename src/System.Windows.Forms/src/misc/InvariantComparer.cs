﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Globalization;

namespace System;

internal class InvariantComparer : IComparer
{
    private readonly CompareInfo m_compareInfo;
    internal static readonly InvariantComparer Default = new();

    internal InvariantComparer()
    {
        m_compareInfo = CultureInfo.InvariantCulture.CompareInfo;
    }

    public int Compare(object? a, object? b)
    {
        if (a is string sa && b is string sb)
        {
            return m_compareInfo.Compare(sa, sb);
        }
        else
        {
            return Comparer.Default.Compare(a, b);
        }
    }
}
