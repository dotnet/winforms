// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;
using System.Windows.Forms;

[assembly: MetadataUpdateHandler(typeof(WinFormsMetadataUpdateHandler))]

namespace System.Windows.Forms;

/// <summary>Handle notifications of metadata updates being applied.</summary>
internal static class WinFormsMetadataUpdateHandler
{
    /// <summary>Invoked after a metadata update is applied.</summary>
    /// <param name="types">The list of types known to be updated, or null if it couldn't be determined.</param>
    internal static void UpdateApplication(Type[]? types)
    {
        // Repaint all open forms.
        foreach (Form openForm in Application.OpenForms)
        {
            openForm.BeginInvoke((MethodInvoker)(() => openForm.Refresh()));
        }
    }
}
