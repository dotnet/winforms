// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Basic Properties for Scrollbars.
    /// </summary>
    public abstract class ScrollProperties
    {
        internal int _minimum = 0;
        internal int _maximum = 100;
        internal int _smallChange = 1;
        internal int _largeChange = 10;
        internal int _value = 0;
        internal bool _maximumSetExternally;
        internal bool _smallChangeSetExternally;
        internal bool _largeChangeSetExternally;

        private readonly ScrollableControl _parent;

        protected ScrollableControl ParentControl => _parent;

        internal bool _visible = false;

        private bool _enabled = true;

        protected ScrollProperties(ScrollableControl container)
        {
            _parent = container;
        }

        /// <summary>
        ///  Gets or sets a bool value controlling whether the scrollbar is enabled.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.ScrollBarEnableDescr))]
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_parent != null && _parent.AutoScroll)
                {
                    return;
                }

                if (value != _enabled)
                {
                    _enabled = value;
                    if (_parent != null)
                    {
                        UnsafeNativeMethods.EnableScrollBar(
                            new HandleRef(_parent, _parent.Handle),
                            Orientation,
                            value ? NativeMethods.ESB_ENABLE_BOTH : NativeMethods.ESB_DISABLE_BOTH
                        );
                    }
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value to be added or subtracted to the <see cref='LargeChange'/>
        ///  property when the scroll box is moved a large distance.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(10)]
        [SRDescription(nameof(SR.ScrollBarLargeChangeDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int LargeChange
        {
            get
            {
                // We preserve the actual large change value that has been set, but when we come to
                // get the value of this property, make sure it's within the maximum allowable value.
                // This way we ensure that we don't depend on the order of property sets when
                // code is generated at design-time.
                return Math.Min(_largeChange, _maximum - _minimum + 1);
            }
            set
            {
                if (_largeChange != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidLowBoundArgumentEx, nameof(LargeChange), value, 0));
                    }

                    _largeChange = value;
                    _largeChangeSetExternally = true;
                    UpdateScrollInfo();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the upper limit of values of the scrollable range.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(100)]
        [SRDescription(nameof(SR.ScrollBarMaximumDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int Maximum
        {
            get => _maximum;
            set
            {
                if (_parent != null && _parent.AutoScroll)
                {
                    return;
                }

                if (_maximum != value)
                {
                    if (_minimum > value)
                    {
                        _minimum = value;
                    }
                    if (value < _value)
                    {
                        Value = value;
                    }

                    _maximum = value;
                    _maximumSetExternally = true;
                    UpdateScrollInfo();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the lower limit of values of the scrollable range.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(0)]
        [SRDescription(nameof(SR.ScrollBarMinimumDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int Minimum
        {
            get => _minimum;
            set
            {
                if (_parent != null && _parent.AutoScroll)
                {
                    return;
                }

                if (_minimum != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidLowBoundArgumentEx, nameof(Minimum), value, 0));
                    }

                    if (_maximum < value)
                    {
                        _maximum = value;
                    }
                    if (value > _value)
                    {
                        _value = value;
                    }

                    _minimum = value;
                    UpdateScrollInfo();
                }
            }
        }

        internal abstract int PageSize { get; }

        internal abstract int Orientation { get; }

        internal abstract int HorizontalDisplayPosition { get; }

        internal abstract int VerticalDisplayPosition { get; }

        /// <summary>
        ///  Gets or sets the value to be added or subtracted to the <see cref='ScrollBar.Value'/>
        ///  property when the scroll box is moved a small distance.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(1)]
        [SRDescription(nameof(SR.ScrollBarSmallChangeDescr))]
        public int SmallChange
        {
            get
            {
                // We can't have SmallChange > LargeChange, but we shouldn't manipulate
                // the set values for these properties, so we just return the smaller
                // value here.
                return Math.Min(_smallChange, LargeChange);
            }
            set
            {
                if (_smallChange != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidLowBoundArgumentEx, nameof(SmallChange), value, 0));
                    }

                    _smallChange = value;
                    _smallChangeSetExternally = true;
                    UpdateScrollInfo();
                }
            }
        }

        /// <summary>
        ///  Gets or sets a numeric value that represents the current position of the scroll box
        ///  on the scroll bar control.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(0)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ScrollBarValueDescr))]
        public int Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    if (value < _minimum || value > _maximum)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidBoundArgument, nameof(Value), value, $"'{nameof(Minimum)}'", $"'{nameof(Maximum)}'"));
                    }

                    _value = value;
                    UpdateScrollInfo();
                    _parent?.SetDisplayFromScrollProps(HorizontalDisplayPosition, VerticalDisplayPosition);
                }
            }
        }

        /// <summary>
        ///  Gets or sets a bool value controlling whether the scrollbar is showing.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ScrollBarVisibleDescr))]
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_parent != null && _parent.AutoScroll)
                {
                    return;
                }

                if (value != _visible)
                {
                    _visible = value;
                    _parent?.UpdateStylesCore();
                    UpdateScrollInfo();
                    _parent?.SetDisplayFromScrollProps(HorizontalDisplayPosition, VerticalDisplayPosition);
                }
            }
        }

        internal void UpdateScrollInfo()
        {
            if (_parent != null && _parent.IsHandleCreated && _visible)
            {
                var si = new NativeMethods.SCROLLINFO
                {
                    cbSize = Marshal.SizeOf<NativeMethods.SCROLLINFO>(),
                    fMask = NativeMethods.SIF_ALL,
                    nMin = _minimum,
                    nMax = _maximum,
                    nPage = _parent.AutoScroll ? PageSize : LargeChange,
                    nPos = _value,
                    nTrackPos = 0
                };
                UnsafeNativeMethods.SetScrollInfo(new HandleRef(_parent, _parent.Handle), Orientation, si, true);
            }
        }
    }
}
