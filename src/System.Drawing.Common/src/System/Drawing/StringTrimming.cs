// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing;

/// <summary>
///  Specifies how to trim characters from a string that does not completely fit into a layout shape.
/// </summary>
public enum StringTrimming
{
    /// <summary>
    ///  Specifies no trimming.
    /// </summary>
    None = GdiPlus.StringTrimming.StringTrimmingNone,

    /// <summary>
    ///  Specifies that the string is broken at the boundary of the last character
    ///  that is inside the layout rectangle. This is the default.
    /// </summary>
    Character = GdiPlus.StringTrimming.StringTrimmingCharacter,

    /// <summary>
    ///  Specifies that the string is broken at the boundary of the last word that is inside the layout rectangle.
    /// </summary>
    Word = GdiPlus.StringTrimming.StringTrimmingWord,

    /// <summary>
    ///  Specifies that the string is broken at the boundary of the last character that is inside
    ///  the layout rectangle and an ellipsis (...) is inserted after the character.
    /// </summary>
    EllipsisCharacter = GdiPlus.StringTrimming.StringTrimmingEllipsisCharacter,

    /// <summary>
    ///  Specifies that the string is broken at the boundary of the last word that is inside the
    ///  layout rectangle and an ellipsis (...) is inserted after the word.
    /// </summary>
    EllipsisWord = GdiPlus.StringTrimming.StringTrimmingEllipsisWord,

    /// <summary>
    ///  Specifies that the center is removed from the string and replaced by an ellipsis.
    ///  The algorithm keeps as much of the last portion of the string as possible.
    /// </summary>
    EllipsisPath = GdiPlus.StringTrimming.StringTrimmingEllipsisPath
}
