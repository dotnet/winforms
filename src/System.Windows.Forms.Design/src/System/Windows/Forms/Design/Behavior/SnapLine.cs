// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  The SnapLine class represents a UI-guideline that will be rendered
///  during control movement (drag, keyboard, and resize) operations.
///  SnapLines will assist a user in aligning controls relative to one
///  one another. Each SnapLine will have a type: top, bottom, etc...
///  Only SnapLines of like-types are allowed to align with each other.
///  The 'offset' will represent the distance from the origin (upper-left
///  corner) of the control to where the SnapLine is located. And finally
///  the 'filter' is a string used to define custom types of SnapLines.
///  This enables a SnapLine with a filter of "TypeX" to only snap to
///  other "TypeX" filtered lines.
/// </summary>
public sealed class SnapLine
{
    // These are used in the SnapLine filter to define custom margin/padding SnapLines.
    // Margins will have special rules of equality, basically opposites will attract one another
    // (ex: margin right == margin left) and paddings will be attracted to like-margins.
    internal const string Margin = "Margin";
    internal const string MarginRight = Margin + ".Right";
    internal const string MarginLeft = Margin + ".Left";
    internal const string MarginBottom = Margin + ".Bottom";
    internal const string MarginTop = Margin + ".Top";
    internal const string Padding = "Padding";
    internal const string PaddingRight = Padding + ".Right";
    internal const string PaddingLeft = Padding + ".Left";
    internal const string PaddingBottom = Padding + ".Bottom";
    internal const string PaddingTop = Padding + ".Top";

    /// <summary>
    ///  SnapLine constructor that takes the type and offset of SnapLine.
    /// </summary>
    public SnapLine(SnapLineType type, int offset)
        : this(type, offset, filter: null, SnapLinePriority.Low)
    {
    }

    /// <summary>
    ///  SnapLine constructor that takes the type, offset and filter of SnapLine.
    /// </summary>
    public SnapLine(SnapLineType type, int offset, string? filter)
        : this(type, offset, filter, SnapLinePriority.Low)
    {
    }

    /// <summary>
    ///  SnapLine constructor that takes the type, offset, and priority of SnapLine.
    /// </summary>
    public SnapLine(SnapLineType type, int offset, SnapLinePriority priority)
        : this(type, offset, filter: null, priority)
    {
    }

    /// <summary>
    ///  SnapLine constructor that takes the type, offset, filter, and priority of the SnapLine.
    /// </summary>
    public SnapLine(SnapLineType type, int offset, string? filter, SnapLinePriority priority)
    {
        SnapLineType = type;
        Offset = offset;
        Filter = filter;
        Priority = priority;
    }

    /// <summary>
    ///  This property returns a string representing an optional user-defined filter.
    ///  Setting this filter will allow only those SnapLines with similar filters to align
    ///  to one another.
    /// </summary>
    public string? Filter { get; }

    /// <summary>
    ///  Returns true if the SnapLine is of a horizontal type.
    /// </summary>
    public bool IsHorizontal => SnapLineType is SnapLineType.Top
        or SnapLineType.Bottom
        or SnapLineType.Horizontal
        or SnapLineType.Baseline;

    /// <summary>
    ///  Returns true if the SnapLine is of a vertical type.
    /// </summary>
    public bool IsVertical => SnapLineType is SnapLineType.Left
        or SnapLineType.Right
        or SnapLineType.Vertical;

    /// <summary>
    ///  Read-only property that returns the distance from the origin to where this SnapLine is defined.
    /// </summary>
    public int Offset { get; private set; }

    /// <summary>
    ///  Read-only property that returns the priority of the SnapLine.
    /// </summary>
    public SnapLinePriority Priority { get; }

    /// <summary>
    ///  Read-only property that represents the 'type' of SnapLine.
    /// </summary>
    public SnapLineType SnapLineType { get; }

    /// <summary>
    ///  Adjusts the offset property of the SnapLine.
    /// </summary>
    public void AdjustOffset(int adjustment)
    {
        Offset += adjustment;
    }

    /// <summary>
    ///  Returns true if SnapLine s1 should snap to SnapLine s2.
    /// </summary>
    public static bool ShouldSnap(SnapLine line1, SnapLine line2)
    {
        // types must first be equal
        if (line1.SnapLineType != line2.SnapLineType)
        {
            return false;
        }

        // if the filters are both null - then return true
        if (line1.Filter is null && line2.Filter is null)
        {
            return true;
        }

        // at least one filter is non-null so if the other is null
        // then we don't have a match
        if (line1.Filter is null || line2.Filter is null)
        {
            return false;
        }

        // check for our special-cased margin filter
        if (line1.Filter.Contains(Margin))
        {
            if ((line1.Filter.Equals(MarginRight) && (line2.Filter.Equals(MarginLeft) || line2.Filter.Equals(PaddingRight))) ||
              (line1.Filter.Equals(MarginLeft) && (line2.Filter.Equals(MarginRight) || line2.Filter.Equals(PaddingLeft))) ||
              (line1.Filter.Equals(MarginTop) && (line2.Filter.Equals(MarginBottom) || line2.Filter.Equals(PaddingTop))) ||
              (line1.Filter.Equals(MarginBottom) && (line2.Filter.Equals(MarginTop) || line2.Filter.Equals(PaddingBottom))))
            {
                return true;
            }

            return false;
        }

        // check for padding & margins
        if (line1.Filter.Contains(Padding))
        {
            if ((line1.Filter.Equals(PaddingLeft) && line2.Filter.Equals(MarginLeft)) ||
              (line1.Filter.Equals(PaddingRight) && line2.Filter.Equals(MarginRight)) ||
              (line1.Filter.Equals(PaddingTop) && line2.Filter.Equals(MarginTop)) ||
              (line1.Filter.Equals(PaddingBottom) && line2.Filter.Equals(MarginBottom)))
            {
                return true;
            }

            return false;
        }

        // basic filter equality
        if (line1.Filter.Equals(line2.Filter))
        {
            return true;
        }

        // not equal!
        return false;
    }

    /// <summary>
    ///  ToString implementation for SnapLines.
    /// </summary>
    public override string ToString()
    {
        return $"SnapLine: {{type = {SnapLineType}, offset = {Offset}, priority = {Priority}, filter = {Filter ?? "<null>"}}}";
    }
}
