// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyGridView
{
    private enum ErrorState
    {
        None,
        Thrown,
        MessageBoxUp
    }
}
