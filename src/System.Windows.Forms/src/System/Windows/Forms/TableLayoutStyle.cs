// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    [TypeConverter(typeof(TableLayoutSettings.StyleConverter))]
    public abstract class TableLayoutStyle
    {
        private IArrangedElement _owner;
        private SizeType _sizeType = SizeType.AutoSize;
        private float _size;

        [DefaultValue(SizeType.AutoSize)]
        public SizeType SizeType
        {
            get { return _sizeType; }
            set
            {
                if (_sizeType != value)
                {
                    _sizeType = value;
                    if (Owner != null)
                    {
                        LayoutTransaction.DoLayout(Owner, Owner, PropertyNames.Style);
                        if (Owner is Control owner)
                        {
                            owner.Invalidate();
                        }
                    }
                }
            }
        }

        internal float Size
        {
            get { return _size; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(Size), value, 0));
                }
                if (_size != value)
                {
                    _size = value;
                    if (Owner != null)
                    {
                        LayoutTransaction.DoLayout(Owner, Owner, PropertyNames.Style);
                        if (Owner is Control owner)
                        {
                            owner.Invalidate();
                        }
                    }
                }
            }
        }

        private bool ShouldSerializeSize()
        {
            return SizeType != SizeType.AutoSize;
        }

        internal IArrangedElement Owner
        {
            get { return _owner; }
            set { _owner = value; }
        }

        //set the size without doing a layout
        internal void SetSize(float size)
        {
            Debug.Assert(size >= 0);
            _size = size;
        }
    }
}
