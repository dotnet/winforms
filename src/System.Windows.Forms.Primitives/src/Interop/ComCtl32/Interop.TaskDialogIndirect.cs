// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class ComCtl32
    {
        // Note: The pTaskConfig parameter isn't declared as "ref TASKDIALOGCONFIG"
        // because we currently do manual marshalling. Otherwise, the struct might be
        // accidentally marshalled by the runtime when it is no longer blittable, but we
        // also need a pointer to it for SendMessage() calls where it wouldn't be marshalled.
        [DllImport(Libraries.Comctl32, ExactSpelling = true)]
        public static extern unsafe HRESULT TaskDialogIndirect(
            TASKDIALOGCONFIG* pTaskConfig,
            out int pnButton,
            out int pnRadioButton,
            out BOOL pfVerificationFlagChecked);
    }
}
