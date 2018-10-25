// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Layout {
    using System.Diagnostics.CodeAnalysis;

    // LayoutEventArgs takes a string for AffectedProperty.  This class contains const
    // strings to use as property names.  Doing this allows us to use reference comparisons
    // which is advantageous because 1) pref and 2) we will not accidently collide with
    // names that extenders provide.
    internal class PropertyNames {
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Alignment = "Alignment";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Anchor = "Anchor";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string AutoScroll = "AutoScroll";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string AutoSize = "AutoSize";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Appearance = "Appearance";        
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string AutoEllipsis = "AutoEllipsis";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string BorderStyle = "BorderStyle";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string CellBorderStyle = "CellBorderStyle";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Bounds = "Bounds";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string CheckAlign = "CheckAlign";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ChildIndex = "ChildIndex";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ColumnHeadersHeight = "ColumnHeadersHeight";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ColumnHeadersVisible = "ColumnHeadersVisible";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Columns = "Columns";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ColumnSpan = "ColumnSpan";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ColumnStyles = "ColumnStyles";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Controls = "Controls";        
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Dock = "Dock";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string DisplayRectangle = "DisplayRectangle";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string DisplayStyle = "DisplayStyle";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string DrawMode = "DrawMode";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string DropDownButtonWidth = "DropDownButtonWidth";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string FlatAppearanceBorderSize = "FlatAppearance.BorderSize";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string FlatStyle = "FlatStyle";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string FlowBreak = "FlowBreak";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string FlowDirection = "FlowDirection";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Font = "Font";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string GripStyle = "GripStyle";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string GrowStyle = "GrowStyle";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Image= "Image";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ImageIndex= "ImageIndex";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ImageScaling= "ImageScaling";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ImageScalingSize= "ImageScalingSize";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ImageKey= "ImageKey";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ImageAlign = "ImageAlign";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Items = "Items";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string LayoutSettings = "LayoutSettings";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string LinkArea = "LinkArea";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Links = "Links";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string LayoutStyle = "LayoutStyle";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Location = "Location";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Margin = "Margin";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string MaximumSize = "MaximumSize";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string MinimumSize = "MinimumSize";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Multiline = "Multiline";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Orientation = "Orientation";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string PreferredSize = "PreferredSize";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Padding = "Padding";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Parent = "Parent";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string RightToLeft = "RightToLeft";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string RightToLeftLayout = "RightToLeftLayout";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string RowHeadersVisible = "RowHeadersVisible";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string RowHeadersWidth = "RowHeadersWidth";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Rows = "Rows";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string RowSpan = "RowSpan";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string RowStyles = "RowStyles";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Renderer = "Renderer";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ScrollBars = "ScrollBars";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Size = "Size";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ShowDropDownArrow = "ShowDropDownArrow";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ShowImageMargin = "ShowCheckMargin";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string ShowCheckMargin = "ShowCheckMargin";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Spring = "Spring";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Style = "Style";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string TableIndex = "TableIndex";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Text = "Text";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string TextAlign = "TextAlign";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string TextImageRelation = "TextImageRelation";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string UseCompatibleTextRendering = "UseCompatibleTextRendering";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string Visible = "Visible";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string WordWrap = "WordWrap";
        [SuppressMessage("Microsoft.Performance", "CA1802:UseLiteralsWhereAppropriate")]
        public static readonly string WrapContents = "WrapContents";
    }
}
