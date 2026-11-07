// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.ExceptionServices;

namespace System.Windows.Forms.Tests;

public partial class ClipboardPublicationTests
{
    /// <summary>
    ///  Transfers a scenario and its failure across the native worker boundary.
    /// </summary>
    private sealed class StaInvocation(WindowStation station, Action action)
    {
        public WindowStation Station { get; } = station;
        public Action Action { get; } = action;
        public ExceptionDispatchInfo? Failure { get; set; }
    }
}
