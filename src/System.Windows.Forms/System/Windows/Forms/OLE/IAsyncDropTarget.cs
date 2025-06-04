// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;

namespace System.Windows.Forms;

/// <summary>
///  Interface for a drop target that supports asynchronous processing.
/// </summary>
/// <remarks>
///  <para>
///   This is currently marked as experimental as there is some uncertainty around the API that might need
///   to be addressed in the future. With additional scenario feedback, we will make changes if needed.
///  </para>
/// </remarks>
[Experimental(DiagnosticIDs.ExperimentalAsyncDropTarget, UrlFormat = DiagnosticIDs.UrlFormat)]
public interface IAsyncDropTarget : IDropTarget
{
    /// <summary>
    ///  When supporting this interface, this method will be callled if the drop source supports asynchronous processing.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Similar to <see cref="IDropTarget.OnDragDrop"/>, but this method is called when a drop operation supports
    ///   asyncronous processing. It will not block the UI thread, any UI updates will need to be invoked to occur
    ///   on the UI thread.
    ///  </para>
    ///  <para>
    ///   Avoid dispatching the <see cref="DragEventArgs"/> back to the UI thread as invoking <see cref="DragEventArgs.Data"/>
    ///   on the UI thread will block it until the data is available. If existing code needs <see cref="DragEventArgs"/>
    ///   consider creating a new instance with a new <see cref="DataObject"/> that has extracted the data you're looking for.
    ///  </para>
    /// </remarks>
    void OnAsyncDragDrop(DragEventArgs e);
}
