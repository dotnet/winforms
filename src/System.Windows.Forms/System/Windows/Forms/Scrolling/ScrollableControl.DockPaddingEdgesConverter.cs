// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public partial class ScrollableControl
{
    /// <summary>
    ///  Determines the border padding for docked controls.
    /// </summary>
    [TypeConverter(typeof(DockPaddingEdgesConverter))]
    public class DockPaddingEdges : ICloneable
    {
        private readonly ScrollableControl? _owner;
        private int _left;
        private int _right;
        private int _top;
        private int _bottom;

        /// <summary>
        ///  Creates a new DockPaddingEdges. The specified owner will be notified when
        ///  the values are changed.
        /// </summary>
        internal DockPaddingEdges(ScrollableControl owner)
        {
            _owner = owner;
        }

        internal DockPaddingEdges(int left, int right, int top, int bottom)
        {
            _left = left;
            _right = right;
            _top = top;
            _bottom = bottom;
        }

        /// <summary>
        ///  Gets or sets the padding width for all edges of a docked control.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [SRDescription(nameof(SR.PaddingAllDescr))]
        public int All
        {
            get
            {
                if (_owner is null)
                {
                    if (_left == _right && _top == _bottom && _left == _top)
                    {
                        return _left;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    // The Padding struct uses -1 to indicate that LRTB are in disagreement.
                    // For backwards compatibility, we need to remap -1 to 0, but we need
                    // to be careful because it could be that they explicitly set All to -1.
                    if (_owner.Padding.All == -1
                        &&
                            (_owner.Padding.Left != -1
                            || _owner.Padding.Top != -1
                            || _owner.Padding.Right != -1
                            || _owner.Padding.Bottom != -1))
                    {
                        return 0;
                    }

                    return _owner.Padding.All;
                }
            }
            set
            {
                if (_owner is null)
                {
                    _left = value;
                    _top = value;
                    _right = value;
                    _bottom = value;
                }
                else
                {
                    _owner.Padding = new Padding(value);
                }
            }
        }

        /// <summary>
        ///  Gets or sets the padding width for the bottom edge of a docked control.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [SRDescription(nameof(SR.PaddingBottomDescr))]
        public int Bottom
        {
            get => _owner is null ? _bottom : _owner.Padding.Bottom;
            set
            {
                if (_owner is null)
                {
                    _bottom = value;
                }
                else
                {
                    Padding padding = _owner.Padding;
                    padding.Bottom = value;
                    _owner.Padding = padding;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the padding width for the left edge of a docked control.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [SRDescription(nameof(SR.PaddingLeftDescr))]
        public int Left
        {
            get => _owner is null ? _left : _owner.Padding.Left;
            set
            {
                if (_owner is null)
                {
                    _left = value;
                }
                else
                {
                    Padding padding = _owner.Padding;
                    padding.Left = value;
                    _owner.Padding = padding;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the padding width for the right edge of a docked control.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [SRDescription(nameof(SR.PaddingRightDescr))]
        public int Right
        {
            get => _owner is null ? _right : _owner.Padding.Right;
            set
            {
                if (_owner is null)
                {
                    _right = value;
                }
                else
                {
                    Padding padding = _owner.Padding;
                    padding.Right = value;
                    _owner.Padding = padding;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the padding width for the top edge of a docked control.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [SRDescription(nameof(SR.PaddingTopDescr))]
        public int Top
        {
            get => _owner is null ? _top : _owner.Padding.Top;
            set
            {
                if (_owner is null)
                {
                    _top = value;
                }
                else
                {
                    Padding padding = _owner.Padding;
                    padding.Top = value;
                    _owner.Padding = padding;
                }
            }
        }

        public override bool Equals(object? other)
        {
            return other is DockPaddingEdges dpeOther &&
                Left == dpeOther.Left &&
                Top == dpeOther.Top &&
                Right == dpeOther.Right &&
                Bottom == dpeOther.Bottom;
        }

        public override int GetHashCode() => HashCode.Combine(Left, Top, Right, Bottom);

        private void ResetAll() => All = 0;

        private void ResetBottom() => Bottom = 0;

        private void ResetLeft() => Left = 0;

        private void ResetRight() => Right = 0;

        private void ResetTop() => Top = 0;

        internal void Scale(float dx, float dy)
        {
            _owner?.Padding.Scale(dx, dy);
        }

        public override string ToString() => $"{{Left={Left},Top={Top},Right={Right},Bottom={Bottom}}}";

        object ICloneable.Clone() => new DockPaddingEdges(Left, Right, Top, Bottom);
    }
}
