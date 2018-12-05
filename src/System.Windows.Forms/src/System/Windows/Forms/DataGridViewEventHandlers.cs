// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewAutoSizeColumnModeEventHandler(object sender, DataGridViewAutoSizeColumnModeEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewAutoSizeColumnsModeEventHandler(object sender, DataGridViewAutoSizeColumnsModeEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewAutoSizeModeEventHandler(object sender, DataGridViewAutoSizeModeEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewBindingCompleteEventHandler(object sender, DataGridViewBindingCompleteEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellCancelEventHandler(object sender, DataGridViewCellCancelEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellContextMenuStripNeededEventHandler(object sender, DataGridViewCellContextMenuStripNeededEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellErrorTextNeededEventHandler(object sender, DataGridViewCellErrorTextNeededEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellEventHandler(object sender, DataGridViewCellEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellFormattingEventHandler(object sender, DataGridViewCellFormattingEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellMouseEventHandler(object sender, DataGridViewCellMouseEventArgs e);
    
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellPaintingEventHandler(object sender, DataGridViewCellPaintingEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellParsingEventHandler(object sender, DataGridViewCellParsingEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellStateChangedEventHandler(object sender, DataGridViewCellStateChangedEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellStyleContentChangedEventHandler(object sender, DataGridViewCellStyleContentChangedEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellToolTipTextNeededEventHandler(object sender, DataGridViewCellToolTipTextNeededEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellValidatingEventHandler(object sender, DataGridViewCellValidatingEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellValueEventHandler(object sender, DataGridViewCellValueEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewColumnDividerDoubleClickEventHandler(object sender, DataGridViewColumnDividerDoubleClickEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewColumnEventHandler(object sender, DataGridViewColumnEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewColumnStateChangedEventHandler(object sender, DataGridViewColumnStateChangedEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewEditingControlShowingEventHandler(object sender, DataGridViewEditingControlShowingEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewDataErrorEventHandler(object sender, DataGridViewDataErrorEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowCancelEventHandler(object sender, DataGridViewRowCancelEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowContextMenuStripNeededEventHandler(object sender, DataGridViewRowContextMenuStripNeededEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowDividerDoubleClickEventHandler(object sender, DataGridViewRowDividerDoubleClickEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowEventHandler(object sender, DataGridViewRowEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowErrorTextNeededEventHandler(object sender, DataGridViewRowErrorTextNeededEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowHeightInfoNeededEventHandler(object sender, DataGridViewRowHeightInfoNeededEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowHeightInfoPushedEventHandler(object sender, DataGridViewRowHeightInfoPushedEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowPostPaintEventHandler(object sender, DataGridViewRowPostPaintEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowPrePaintEventHandler(object sender, DataGridViewRowPrePaintEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowsAddedEventHandler(object sender, DataGridViewRowsAddedEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowsRemovedEventHandler(object sender, DataGridViewRowsRemovedEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowStateChangedEventHandler(object sender, DataGridViewRowStateChangedEventArgs e);

    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewSortCompareEventHandler(object sender, DataGridViewSortCompareEventArgs e);
}
