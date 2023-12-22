// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Printing;

/// <summary>
///  Standard paper sources.
/// </summary>
public enum PaperSourceKind
{
    /// <summary>
    ///  The upper bin of a printer (or, if the printer only has one bin, the only bin).
    /// </summary>
    Upper = (int)PInvoke.DMBIN_UPPER,

    /// <summary>
    ///  The lower bin of a printer.
    /// </summary>
    Lower = (int)PInvoke.DMBIN_LOWER,

    /// <summary>
    ///  The middle bin of a printer.
    /// </summary>
    Middle = (int)PInvoke.DMBIN_MIDDLE,

    /// <summary>
    ///  Manually-fed paper.
    /// </summary>
    Manual = (int)PInvoke.DMBIN_MANUAL,

    /// <summary>
    ///  An envelope.
    /// </summary>
    Envelope = (int)PInvoke.DMBIN_ENVELOPE,

    /// <summary>
    ///  A manually-fed envelope.
    /// </summary>
    ManualFeed = (int)PInvoke.DMBIN_ENVMANUAL,

    /// <summary>
    ///  Automatic-fed paper.
    /// </summary>
    AutomaticFeed = (int)PInvoke.DMBIN_AUTO,

    /// <summary>
    ///  A tractor feed.
    /// </summary>
    TractorFeed = (int)PInvoke.DMBIN_TRACTOR,

    /// <summary>
    ///  Small-format paper.
    /// </summary>
    SmallFormat = (int)PInvoke.DMBIN_SMALLFMT,

    /// <summary>
    ///  Large-format paper.
    /// </summary>
    LargeFormat = (int)PInvoke.DMBIN_LARGEFMT,

    /// <summary>
    ///  A large-capacity bin printer.
    /// </summary>
    LargeCapacity = (int)PInvoke.DMBIN_LARGECAPACITY,

    /// <summary>
    ///  A paper cassette.
    /// </summary>
    Cassette = (int)PInvoke.DMBIN_CASSETTE,

    FormSource = (int)PInvoke.DMBIN_FORMSOURCE,

    /// <summary>
    ///  A printer-specific paper source.
    /// </summary>
    Custom = (int)PInvoke.DMBIN_USER + 1,
}
