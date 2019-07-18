// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Layout
{
    // LayoutEventArgs takes a string for AffectedProperty.  This class contains const
    // strings to use as property names.  Doing this allows us to use reference comparisons
    // which is advantageous because 1) pref and 2) we will not accidently collide with
    // names that extenders provide.
    internal class PropertyNames
    {
        public static readonly string Alignment = "Alignment";
        public static readonly string Anchor = "Anchor";
        public static readonly string AutoScroll = "AutoScroll";
        public static readonly string AutoSize = "AutoSize";
        public static readonly string Appearance = "Appearance";
        public static readonly string AutoEllipsis = "AutoEllipsis";
        public static readonly string BorderStyle = "BorderStyle";
        public static readonly string CellBorderStyle = "CellBorderStyle";
        public static readonly string Bounds = "Bounds";
        public static readonly string CheckAlign = "CheckAlign";
        public static readonly string ChildIndex = "ChildIndex";
        public static readonly string ColumnHeadersHeight = "ColumnHeadersHeight";
        public static readonly string ColumnHeadersVisible = "ColumnHeadersVisible";
        public static readonly string Columns = "Columns";
        public static readonly string ColumnSpan = "ColumnSpan";
        public static readonly string ColumnStyles = "ColumnStyles";
        public static readonly string Controls = "Controls";
        public static readonly string Dock = "Dock";
        public static readonly string DisplayRectangle = "DisplayRectangle";
        public static readonly string DisplayStyle = "DisplayStyle";
        public static readonly string DrawMode = "DrawMode";
        public static readonly string DropDownButtonWidth = "DropDownButtonWidth";
        public static readonly string FlatAppearanceBorderSize = "FlatAppearance.BorderSize";
        public static readonly string FlatStyle = "FlatStyle";
        public static readonly string FlowBreak = "FlowBreak";
        public static readonly string FlowDirection = "FlowDirection";
        public static readonly string Font = "Font";
        public static readonly string GripStyle = "GripStyle";
        public static readonly string GrowStyle = "GrowStyle";
        public static readonly string Image = "Image";
        public static readonly string ImageIndex = nameof(ImageIndex);
        public static readonly string ImageScaling = "ImageScaling";
        public static readonly string ImageScalingSize = "ImageScalingSize";
        public static readonly string ImageKey = "ImageKey";
        public static readonly string ImageAlign = "ImageAlign";
        public static readonly string Items = "Items";
        public static readonly string LayoutSettings = "LayoutSettings";
        public static readonly string LinkArea = "LinkArea";
        public static readonly string Links = "Links";
        public static readonly string LayoutStyle = "LayoutStyle";
        public static readonly string Location = "Location";
        public static readonly string Margin = "Margin";
        public static readonly string MaximumSize = "MaximumSize";
        public static readonly string MinimumSize = "MinimumSize";
        public static readonly string Multiline = "Multiline";
        public static readonly string Orientation = "Orientation";
        public static readonly string PreferredSize = "PreferredSize";
        public static readonly string Padding = "Padding";
        public static readonly string Parent = "Parent";
        public static readonly string RightToLeft = "RightToLeft";
        public static readonly string RightToLeftLayout = "RightToLeftLayout";
        public static readonly string RowHeadersVisible = "RowHeadersVisible";
        public static readonly string RowHeadersWidth = "RowHeadersWidth";
        public static readonly string Rows = "Rows";
        public static readonly string RowSpan = "RowSpan";
        public static readonly string RowStyles = "RowStyles";
        public static readonly string Renderer = "Renderer";
        public static readonly string ScrollBars = "ScrollBars";
        public static readonly string Size = "Size";
        public static readonly string ShowDropDownArrow = "ShowDropDownArrow";
        public static readonly string ShowImageMargin = "ShowCheckMargin";
        public static readonly string ShowCheckMargin = "ShowCheckMargin";
        public static readonly string Spring = "Spring";
        public static readonly string Style = "Style";
        public static readonly string TableIndex = "TableIndex";
        public static readonly string Text = "Text";
        public static readonly string TextAlign = "TextAlign";
        public static readonly string TextImageRelation = "TextImageRelation";
        public static readonly string UseCompatibleTextRendering = "UseCompatibleTextRendering";
        public static readonly string Visible = "Visible";
        public static readonly string WordWrap = "WordWrap";
        public static readonly string WrapContents = "WrapContents";
    }
}
