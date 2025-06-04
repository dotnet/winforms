// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public sealed class DataGridViewAdvancedBorderStyle : ICloneable
{
    private readonly DataGridView? _owner;
    private bool _all = true;
    private readonly DataGridViewAdvancedCellBorderStyle _banned1;
    private readonly DataGridViewAdvancedCellBorderStyle _banned2;
    private readonly DataGridViewAdvancedCellBorderStyle _banned3;
    private DataGridViewAdvancedCellBorderStyle _top = DataGridViewAdvancedCellBorderStyle.None;
    private DataGridViewAdvancedCellBorderStyle _left = DataGridViewAdvancedCellBorderStyle.None;
    private DataGridViewAdvancedCellBorderStyle _right = DataGridViewAdvancedCellBorderStyle.None;
    private DataGridViewAdvancedCellBorderStyle _bottom = DataGridViewAdvancedCellBorderStyle.None;

    public DataGridViewAdvancedBorderStyle()
        : this(
            null,
            DataGridViewAdvancedCellBorderStyle.NotSet,
            DataGridViewAdvancedCellBorderStyle.NotSet,
            DataGridViewAdvancedCellBorderStyle.NotSet)
    {
    }

    internal DataGridViewAdvancedBorderStyle(DataGridView owner)
        : this(
            owner,
            DataGridViewAdvancedCellBorderStyle.NotSet,
            DataGridViewAdvancedCellBorderStyle.NotSet,
            DataGridViewAdvancedCellBorderStyle.NotSet)
    {
    }

    /// <summary>
    ///  Creates a new DataGridViewAdvancedBorderStyle. The specified owner will
    ///  be notified when the values are changed.
    /// </summary>
    internal DataGridViewAdvancedBorderStyle(
        DataGridView? owner,
        DataGridViewAdvancedCellBorderStyle banned1,
        DataGridViewAdvancedCellBorderStyle banned2,
        DataGridViewAdvancedCellBorderStyle banned3)
    {
        _owner = owner;
        _banned1 = banned1;
        _banned2 = banned2;
        _banned3 = banned3;
    }

    public DataGridViewAdvancedCellBorderStyle All
    {
        get
        {
            return _all ? _top : DataGridViewAdvancedCellBorderStyle.NotSet;
        }
        set
        {
            // Sequential enum. Valid values are 0x0 to 0x7
            SourceGenerated.EnumValidator.Validate(value);
            if (value == DataGridViewAdvancedCellBorderStyle.NotSet ||
                value == _banned1 ||
                value == _banned2 ||
                value == _banned3)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_AdvancedCellBorderStyleInvalid, "All"));
            }

            if (!_all || _top != value)
            {
                _all = true;
                _top = _left = _right = _bottom = value;
                _owner?.OnAdvancedBorderStyleChanged(this);
            }
        }
    }

    public DataGridViewAdvancedCellBorderStyle Bottom
    {
        get
        {
            if (_all)
            {
                return _top;
            }

            return _bottom;
        }
        set
        {
            // Sequential enum. Valid values are 0x0 to 0x7
            SourceGenerated.EnumValidator.Validate(value);
            if (value == DataGridViewAdvancedCellBorderStyle.NotSet)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_AdvancedCellBorderStyleInvalid, "Bottom"));
            }

            BottomInternal = value;
        }
    }

    internal DataGridViewAdvancedCellBorderStyle BottomInternal
    {
        set
        {
            if ((_all && _top != value) || (!_all && _bottom != value))
            {
                if (_all)
                {
                    if (_right == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                    {
                        _right = DataGridViewAdvancedCellBorderStyle.Outset;
                    }
                }

                _all = false;
                _bottom = value;
                _owner?.OnAdvancedBorderStyleChanged(this);
            }
        }
    }

    public DataGridViewAdvancedCellBorderStyle Left
    {
        get
        {
            if (_all)
            {
                return _top;
            }

            return _left;
        }
        set
        {
            // Sequential enum. Valid values are 0x0 to 0x7
            SourceGenerated.EnumValidator.Validate(value);
            if (value == DataGridViewAdvancedCellBorderStyle.NotSet)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_AdvancedCellBorderStyleInvalid, "Left"));
            }

            LeftInternal = value;
        }
    }

    internal DataGridViewAdvancedCellBorderStyle LeftInternal
    {
        set
        {
            if ((_all && _top != value) || (!_all && _left != value))
            {
                if ((_owner is not null && _owner.RightToLeftInternal) &&
                    (value == DataGridViewAdvancedCellBorderStyle.InsetDouble || value == DataGridViewAdvancedCellBorderStyle.OutsetDouble))
                {
                    throw new ArgumentException(string.Format(SR.DataGridView_AdvancedCellBorderStyleInvalid, "Left"));
                }

                if (_all)
                {
                    if (_right == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                    {
                        _right = DataGridViewAdvancedCellBorderStyle.Outset;
                    }

                    if (_bottom == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                    {
                        _bottom = DataGridViewAdvancedCellBorderStyle.Outset;
                    }
                }

                _all = false;
                _left = value;
                _owner?.OnAdvancedBorderStyleChanged(this);
            }
        }
    }

    public DataGridViewAdvancedCellBorderStyle Right
    {
        get
        {
            if (_all)
            {
                return _top;
            }

            return _right;
        }
        set
        {
            // Sequential enum. Valid values are 0x0 to 0x7
            SourceGenerated.EnumValidator.Validate(value);
            if (value == DataGridViewAdvancedCellBorderStyle.NotSet)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_AdvancedCellBorderStyleInvalid, "Right"));
            }

            RightInternal = value;
        }
    }

    internal DataGridViewAdvancedCellBorderStyle RightInternal
    {
        set
        {
            if ((_all && _top != value) || (!_all && _right != value))
            {
                if ((_owner is not null && !_owner.RightToLeftInternal) &&
                    (value == DataGridViewAdvancedCellBorderStyle.InsetDouble || value == DataGridViewAdvancedCellBorderStyle.OutsetDouble))
                {
                    throw new ArgumentException(string.Format(SR.DataGridView_AdvancedCellBorderStyleInvalid, "Right"));
                }

                if (_all)
                {
                    if (_bottom == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                    {
                        _bottom = DataGridViewAdvancedCellBorderStyle.Outset;
                    }
                }

                _all = false;
                _right = value;
                _owner?.OnAdvancedBorderStyleChanged(this);
            }
        }
    }

    public DataGridViewAdvancedCellBorderStyle Top
    {
        get
        {
            return _top;
        }
        set
        {
            // Sequential enum. Valid values are 0x0 to 0x7
            SourceGenerated.EnumValidator.Validate(value);
            if (value == DataGridViewAdvancedCellBorderStyle.NotSet)
            {
                throw new ArgumentException(string.Format(SR.DataGridView_AdvancedCellBorderStyleInvalid, "Top"));
            }

            TopInternal = value;
        }
    }

    internal DataGridViewAdvancedCellBorderStyle TopInternal
    {
        set
        {
            if ((_all && _top != value) || (!_all && _top != value))
            {
                if (_all)
                {
                    if (_right == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                    {
                        _right = DataGridViewAdvancedCellBorderStyle.Outset;
                    }

                    if (_bottom == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                    {
                        _bottom = DataGridViewAdvancedCellBorderStyle.Outset;
                    }
                }

                _all = false;
                _top = value;
                _owner?.OnAdvancedBorderStyleChanged(this);
            }
        }
    }

    public override bool Equals(object? other)
    {
        if (other is not DataGridViewAdvancedBorderStyle dgvabsOther)
        {
            return false;
        }

        return dgvabsOther._all == _all &&
            dgvabsOther._top == _top &&
            dgvabsOther._left == _left &&
            dgvabsOther._bottom == _bottom &&
            dgvabsOther._right == _right;
    }

    public override int GetHashCode() => HashCode.Combine(_top, _left, _bottom, _right);

    public override string ToString()
    {
        return $"DataGridViewAdvancedBorderStyle {{ All={All}, Left={Left}, Right={Right}, Top={Top}, Bottom={Bottom} }}";
    }

    object ICloneable.Clone()
    {
        DataGridViewAdvancedBorderStyle dgvabs = new(_owner, _banned1, _banned2, _banned3)
        {
            _all = _all,
            _top = _top,
            _right = _right,
            _bottom = _bottom,
            _left = _left
        };
        return dgvabs;
    }
}
