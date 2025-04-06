// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Layout;

// LayoutEventArgs takes a string for AffectedProperty. This class contains const
// strings to use as property names. Doing this allows us to use reference comparisons
// which is advantageous because 1) pref and 2) we will not accidently collide with
// names that extenders provide.
internal static class PropertyNames
{
    public const string Alignment = "Alignment";
    public const string Anchor = "Anchor";
    public const string AutoScroll = "AutoScroll";
    public const string AutoSize = "AutoSize";
    public const string Appearance = "Appearance";
    public const string AutoEllipsis = "AutoEllipsis";
    public const string BorderStyle = "BorderStyle";
    public const string CellBorderStyle = "CellBorderStyle";
    public const string Bounds = "Bounds";
    public const string CheckAlign = "CheckAlign";
    public const string ChildIndex = "ChildIndex";
    public const string ColumnHeadersHeight = "ColumnHeadersHeight";
    public const string ColumnHeadersVisible = "ColumnHeadersVisible";
    public const string Columns = "Columns";
    public const string ColumnSpan = "ColumnSpan";
    public const string ColumnStyles = "ColumnStyles";
    public const string Controls = "Controls";
    public const string Dock = "Dock";
    public const string DisplayRectangle = "DisplayRectangle";
    public const string DisplayStyle = "DisplayStyle";
    public const string DrawMode = "DrawMode";
    public const string DropDownButtonWidth = "DropDownButtonWidth";
    public const string FlatAppearanceBorderSize = "FlatAppearance.BorderSize";
    public const string FlatStyle = "FlatStyle";
    public const string FlowBreak = "FlowBreak";
    public const string FlowDirection = "FlowDirection";
    public const string Font = "Font";
    public const string GripStyle = "GripStyle";
    public const string GrowStyle = "GrowStyle";
    public const string Image = "Image";
    public const string ImageIndex = nameof(ImageIndex);
    public const string ImageScaling = "ImageScaling";
    public const string ImageScalingSize = "ImageScalingSize";
    public const string ImageKey = "ImageKey";
    public const string ImageAlign = "ImageAlign";
    public const string Items = "Items";
    public const string LayoutSettings = "LayoutSettings";
    public const string LinkArea = "LinkArea";
    public const string Links = "Links";
    public const string LayoutStyle = "LayoutStyle";
    public const string Location = "Location";
    public const string Margin = "Margin";
    public const string MaximumSize = "MaximumSize";
    public const string MinimumSize = "MinimumSize";
    public const string Multiline = "Multiline";
    public const string Orientation = "Orientation";
    public const string PreferredSize = "PreferredSize";
    public const string Padding = "Padding";
    public const string Parent = "Parent";
    public const string RightToLeft = "RightToLeft";
    public const string RightToLeftLayout = "RightToLeftLayout";
    public const string RowHeadersVisible = "RowHeadersVisible";
    public const string RowHeadersWidth = "RowHeadersWidth";
    public const string Rows = "Rows";
    public const string RowSpan = "RowSpan";
    public const string RowStyles = "RowStyles";
    public const string Renderer = "Renderer";
    public const string ScrollBars = "ScrollBars";
    public const string Size = "Size";
    public const string ShowDropDownArrow = "ShowDropDownArrow";
    public const string ShowImageMargin = "ShowCheckMargin";
    public const string ShowCheckMargin = "ShowCheckMargin";
    public const string Spring = "Spring";
    public const string Style = "Style";
    public const string TableIndex = "TableIndex";
    public const string Text = "Text";
    public const string TextAlign = "TextAlign";
    public const string TextImageRelation = "TextImageRelation";
    public const string UseCompatibleTextRendering = "UseCompatibleTextRendering";
    public const string Visible = "Visible";
    public const string WordWrap = "WordWrap";
    public const string WrapContents = "WrapContents";
}
