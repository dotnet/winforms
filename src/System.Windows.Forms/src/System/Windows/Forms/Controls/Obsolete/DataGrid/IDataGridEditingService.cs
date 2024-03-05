// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#pragma warning disable RS0016
#nullable disable
[Obsolete("IDataGridEditingService has been deprecated.")]
public interface IDataGridEditingService
{
    bool BeginEdit(DataGridColumnStyle gridColumn, int rowNumber);

    bool EndEdit(DataGridColumnStyle gridColumn, int rowNumber, bool shouldAbort);
}
