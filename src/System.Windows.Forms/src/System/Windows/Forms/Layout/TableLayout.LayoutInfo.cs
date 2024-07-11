// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Layout;

internal partial class TableLayout
{
    /// <summary>
    ///  This class contains layout related information pertaining to a child control of the
    ///  container being laid out. It contains Row,column assignments as well as RowSpan/ColumnSpan.
    ///  This class is used from ContainerInfo as a way of caching information about child controls.
    /// </summary>
    internal sealed class LayoutInfo
    {
        public LayoutInfo(IArrangedElement element)
        {
            Element = element;
        }

        public bool IsAbsolutelyPositioned => RowPosition >= 0 && ColumnPosition >= 0;

        public IArrangedElement Element { get; }

        /// <summary>
        ///  Corresponds to TableLayoutSettings.SetRow. Can be -1 indicating that it is a
        ///  "flow" element and will fit in as necessary. This occurs when a control is
        ///  just added without specific position.
        /// </summary>
        public int RowPosition { get; set; } = -1;

        /// <summary>
        ///  Corresponds to TableLayoutSettings.SetColumn. Can be -1 indicating that it is a
        ///  "flow" element and will fit in as necessary. This occurs when a control is
        ///  just added without specific position.
        /// </summary>
        public int ColumnPosition { get; set; } = -1;

        public int RowStart { get; set; } = -1;

        public int ColumnStart { get; set; } = -1;

        public int ColumnSpan { get; set; } = 1;

        public int RowSpan { get; set; } = 1;

#if DEBUG
        public LayoutInfo Clone() => new(Element)
        {
            RowStart = RowStart,
            ColumnStart = ColumnStart,
            RowSpan = RowSpan,
            ColumnSpan = ColumnSpan,
            RowPosition = RowPosition,
            ColumnPosition = ColumnPosition
        };

        public override bool Equals(object? obj) =>
            obj is LayoutInfo other
                && other.RowStart == RowStart
                && other.ColumnStart == ColumnStart
                && other.RowSpan == RowSpan
                && other.ColumnSpan == ColumnSpan
                && other.RowPosition == RowPosition
                && other.ColumnPosition == ColumnPosition;

        public override int GetHashCode()
        {
            HashCode hash = default;
            hash.Add(RowStart);
            hash.Add(ColumnStart);
            hash.Add(RowSpan);
            hash.Add(ColumnSpan);
            hash.Add(RowPosition);
            hash.Add(ColumnPosition);
            return hash.ToHashCode();
        }
#endif
    }
}
