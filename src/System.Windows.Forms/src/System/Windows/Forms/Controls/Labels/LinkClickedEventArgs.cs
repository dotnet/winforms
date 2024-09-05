// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Provides data for the <see cref="RichTextBox.LinkClicked"/> event.
/// </summary>
public class LinkClickedEventArgs : EventArgs
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="LinkClickedEventArgs"/> class.
    /// </summary>
    /// <param name="linkText">The text of the link being clicked.</param>
    public LinkClickedEventArgs(string? linkText) : this(linkText, 0, 0) { }

    /// <summary>
    ///  Initializes a new instance of the <see cref="LinkClickedEventArgs"/> class.
    /// </summary>
    /// <param name="linkText">The text of the link being clicked.</param>
    /// <param name="linkStart">The start of the link span being clicked.</param>
    /// <param name="linkLength">The length of the link span being clicked.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para>The value for <paramref name="linkStart"/> or <paramref name="linkLength"/> is negative.</para>
    /// <para>-or-</para>
    /// <para>The values for <paramref name="linkStart"/> and <paramref name="linkLength"/> would overflow addition.</para>
    /// </exception>
    public LinkClickedEventArgs(string? linkText, int linkStart, int linkLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(linkStart);
        ArgumentOutOfRangeException.ThrowIfNegative(linkLength);

        if (linkStart + linkLength < 0)
            throw new ArgumentOutOfRangeException(nameof(linkLength));

        LinkText = linkText;
        LinkStart = linkStart;
        LinkLength = linkLength;
    }

    /// <summary>
    ///  Gets the length of the link span being clicked.
    /// </summary>
    public int LinkLength { get; }

    /// <summary>
    ///  Gets the start of the link span being clicked.
    /// </summary>
    public int LinkStart { get; }

    /// <summary>
    ///  Gets the text of the link being clicked.
    /// </summary>
    public string? LinkText { get; }
}
