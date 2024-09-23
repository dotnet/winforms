// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.IDataGridEditingServiceMessage,
    error: false,
    DiagnosticId = Obsoletions.IDataGridEditingServiceDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public interface IDataGridEditingService
{
    bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber);

    bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort);
}
