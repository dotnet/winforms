// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal class EditClass : WindowClass
{
    public EditClass() : base(PInvoke.WC_EDIT)
    {
    }
}
