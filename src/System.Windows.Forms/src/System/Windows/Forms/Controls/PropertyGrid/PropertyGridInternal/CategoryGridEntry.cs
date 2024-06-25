// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.PropertyGridInternal;

/// <summary>
///  Virtual, collapsible parent <see cref="GridEntry"/>.
/// </summary>
internal sealed partial class CategoryGridEntry : GridEntry
{
    private readonly string _name;
    private static Dictionary<string, bool>? s_categoryStates;
    private static readonly object s_lock = new();

    public CategoryGridEntry(PropertyGrid ownerGrid, GridEntry parent, string name, IEnumerable<GridEntry> children)
        : base(ownerGrid, parent)
    {
        _name = name;

        lock (s_lock)
        {
            s_categoryStates ??= [];

            if (!s_categoryStates.ContainsKey(name))
            {
                s_categoryStates.Add(name, true);
            }
        }

        IsExpandable = true;

        ChildCollection = new GridEntryCollection(children);
        foreach (var child in ChildCollection)
        {
            child.ParentGridEntry = this;
        }

        lock (s_lock)
        {
            InternalExpanded = s_categoryStates[name];
        }

        SetFlag(Flags.LabelBold, true);
    }

    // We have no value to display for a category entry.
    internal override bool HasValue => false;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ChildCollection = null;
        }

        base.Dispose(disposing);
    }

    // Categories should never dispose
    public override void DisposeChildren() { }

    // Don't want this participating in property depth.
    public override int PropertyDepth => base.PropertyDepth - 1;

    /// <summary>
    ///  Gets the accessibility object for the current category grid entry.
    /// </summary>
    protected override GridEntryAccessibleObject GetAccessibilityObject() => new CategoryGridEntryAccessibleObject(this);

    protected override Color BackgroundColor => OwnerGridView?.LineColor ?? default;

    protected override Color LabelTextColor => OwnerGrid.CategoryForeColor;

    public override bool Expandable => !GetFlagSet(Flags.ExpandableFailed);

    internal override bool InternalExpanded
    {
        set
        {
            base.InternalExpanded = value;
            lock (s_lock)
            {
                s_categoryStates![_name] = value;
            }
        }
    }

    public override GridItemType GridItemType => GridItemType.Category;

    public override string? HelpKeyword => null;

    public override string PropertyLabel => _name;

    internal override int PropertyLabelIndent
    {
        get
        {
            PropertyGridView? gridHost = OwnerGridView;

            // Give an extra pixel for breathing room.
            // Calling base.PropertyDepth to avoid the -1 in our override.
            return 1 + (gridHost?.OutlineIconSize ?? 0) + OutlineIconPadding
                + (base.PropertyDepth * PropertyGridView.DefaultOutlineIndent);
        }
    }

    public override string GetPropertyTextValue(object? o) => string.Empty;

    public override Type PropertyType => typeof(void);

    internal override object? GetValueOwnerInternal() => ParentGridEntry?.GetValueOwnerInternal();

    protected override bool CreateChildren(bool diffOldChildren) => true;

    public override string GetTestingInfo() => $"object = ({FullLabel}), Category = ({PropertyLabel})";

    public override void PaintLabel(
        Graphics g,
        Rectangle rect,
        Rectangle clipRect,
        bool selected,
        bool paintFullLabel)
    {
        base.PaintLabel(g, rect, clipRect, false, true);

        // Draw the focus rect.
        if (selected && HasFocus)
        {
            bool bold = EntryFlags.HasFlag(Flags.LabelBold);
            Font font = GetFont(boldFont: bold);
            int labelWidth = GetLabelTextWidth(PropertyLabel, g, font);

            int indent = PropertyLabelIndent - 2;
            Rectangle focusRect = new(indent, rect.Y, labelWidth + 3, rect.Height - 1);
            if (SystemInformation.HighContrast && !OwnerGrid.HasCustomLineColor)
            {
                // Line color is Application.SystemColors.ControlDarkDark in high contrast mode.
                ControlPaint.DrawFocusRectangle(g, focusRect, Application.ApplicationColors.ControlText, OwnerGrid.LineColor);
            }
            else
            {
                ControlPaint.DrawFocusRectangle(g, focusRect);
            }
        }

        // Draw the line along the top.
        if (ParentGridEntry is not null && ParentGridEntry.GetChildIndex(this) > 0)
        {
            using var topLinePen = OwnerGrid.CategorySplitterColor.GetCachedPenScope();
            g.DrawLine(topLinePen, rect.X - 1, rect.Y - 1, rect.Width + 2, rect.Y - 1);
        }
    }

    public override void PaintValue(
        Graphics g,
        Rectangle rect,
        Rectangle clipRect,
        PaintValueFlags paintFlags,
        string? text)
    {
        base.PaintValue(g, rect, clipRect, paintFlags & ~PaintValueFlags.DrawSelected, text);

        // Draw the line along the top.
        if (ParentGridEntry is not null && ParentGridEntry.GetChildIndex(this) > 0)
        {
            using var topLinePen = OwnerGrid.CategorySplitterColor.GetCachedPenScope();
            g.DrawLine(topLinePen, rect.X - 2, rect.Y - 1, rect.Width + 1, rect.Y - 1);
        }
    }

    internal override bool SendNotification(GridEntry entry, Notify notification)
        => ParentGridEntry?.SendNotification(entry, notification) ?? false;
}
