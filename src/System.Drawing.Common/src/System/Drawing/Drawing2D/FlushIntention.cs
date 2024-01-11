// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Drawing2D;

public enum FlushIntention
{
    /// <summary>
    ///  Flush all batched rendering operations.
    /// </summary>
    Flush = GdiPlus.FlushIntention.FlushIntentionFlush,

    /// <summary>
    ///  Flush all batched rendering operations and wait for them to complete.
    /// </summary>
    Sync = GdiPlus.FlushIntention.FlushIntentionSync
}
