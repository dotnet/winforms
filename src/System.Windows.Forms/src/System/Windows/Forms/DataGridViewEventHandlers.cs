// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnModeEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewAutoSizeColumnModeEventHandler(object sender, DataGridViewAutoSizeColumnModeEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewAutoSizeColumnsModeEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewAutoSizeColumnsModeEventHandler(object sender, DataGridViewAutoSizeColumnsModeEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewAutoSizeModeEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewAutoSizeModeEventHandler(object sender, DataGridViewAutoSizeModeEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewBindingCompleteEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewBindingCompleteEventHandler(object sender, DataGridViewBindingCompleteEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewCellCancelEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellCancelEventHandler(object sender, DataGridViewCellCancelEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewCellContextMenuStripNeededEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellContextMenuStripNeededEventHandler(object sender, DataGridViewCellContextMenuStripNeededEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewCellErrorTextNeededEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellErrorTextNeededEventHandler(object sender, DataGridViewCellErrorTextNeededEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewCellEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellEventHandler(object sender, DataGridViewCellEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewCellFormattingEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellFormattingEventHandler(object sender, DataGridViewCellFormattingEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewCellMouseEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellMouseEventHandler(object sender, DataGridViewCellMouseEventArgs e);
    
    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewCellPaintingEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellPaintingEventHandler(object sender, DataGridViewCellPaintingEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewCellParsingEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellParsingEventHandler(object sender, DataGridViewCellParsingEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewCellStateChangedEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellStateChangedEventHandler(object sender, DataGridViewCellStateChangedEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewCellStyleContentChangedEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellStyleContentChangedEventHandler(object sender, DataGridViewCellStyleContentChangedEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewCellToolTipTextNeededEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellToolTipTextNeededEventHandler(object sender, DataGridViewCellToolTipTextNeededEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewCellValidatingEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellValidatingEventHandler(object sender, DataGridViewCellValidatingEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewCellValueEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewCellValueEventHandler(object sender, DataGridViewCellValueEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewColumnDividerDoubleClickEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewColumnDividerDoubleClickEventHandler(object sender, DataGridViewColumnDividerDoubleClickEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewColumnEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewColumnEventHandler(object sender, DataGridViewColumnEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewColumnStateChangedEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewColumnStateChangedEventHandler(object sender, DataGridViewColumnStateChangedEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewEditingControlShowingEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewEditingControlShowingEventHandler(object sender, DataGridViewEditingControlShowingEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewDataErrorEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewDataErrorEventHandler(object sender, DataGridViewDataErrorEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewRowCancelEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowCancelEventHandler(object sender, DataGridViewRowCancelEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewRowContextMenuStripNeededEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowContextMenuStripNeededEventHandler(object sender, DataGridViewRowContextMenuStripNeededEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewRowDividerDoubleClickEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowDividerDoubleClickEventHandler(object sender, DataGridViewRowDividerDoubleClickEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewRowEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowEventHandler(object sender, DataGridViewRowEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewRowErrorTextNeededEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowErrorTextNeededEventHandler(object sender, DataGridViewRowErrorTextNeededEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewRowHeightInfoNeededEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowHeightInfoNeededEventHandler(object sender, DataGridViewRowHeightInfoNeededEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewRowHeightInfoPushedEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowHeightInfoPushedEventHandler(object sender, DataGridViewRowHeightInfoPushedEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewRowPostPaintEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowPostPaintEventHandler(object sender, DataGridViewRowPostPaintEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewRowPrePaintEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowPrePaintEventHandler(object sender, DataGridViewRowPrePaintEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewRowsAddedEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowsAddedEventHandler(object sender, DataGridViewRowsAddedEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewRowsRemovedEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowsRemovedEventHandler(object sender, DataGridViewRowsRemovedEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewRowStateChangedEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewRowStateChangedEventHandler(object sender, DataGridViewRowStateChangedEventArgs e);

    /// <include file='doc\DataGridViewEventHandlers.uex' path='docs/doc[@for="DataGridViewSortCompareEventHandler"]/*' />
    [SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
    public delegate void DataGridViewSortCompareEventHandler(object sender, DataGridViewSortCompareEventArgs e);
}
