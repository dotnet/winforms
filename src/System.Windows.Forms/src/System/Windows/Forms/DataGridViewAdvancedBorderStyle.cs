// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    public sealed class DataGridViewAdvancedBorderStyle : ICloneable
    {
        private readonly DataGridView owner;
        private bool all = true;
        private readonly DataGridViewAdvancedCellBorderStyle banned1, banned2, banned3;
        private DataGridViewAdvancedCellBorderStyle top = DataGridViewAdvancedCellBorderStyle.None;
        private DataGridViewAdvancedCellBorderStyle left = DataGridViewAdvancedCellBorderStyle.None;
        private DataGridViewAdvancedCellBorderStyle right = DataGridViewAdvancedCellBorderStyle.None;
        private DataGridViewAdvancedCellBorderStyle bottom = DataGridViewAdvancedCellBorderStyle.None;

        public DataGridViewAdvancedBorderStyle() : this(null,
                                                        DataGridViewAdvancedCellBorderStyle.NotSet,
                                                        DataGridViewAdvancedCellBorderStyle.NotSet,
                                                        DataGridViewAdvancedCellBorderStyle.NotSet)
        {
        }

        internal DataGridViewAdvancedBorderStyle(DataGridView owner) : this(owner,
                                                                            DataGridViewAdvancedCellBorderStyle.NotSet,
                                                                            DataGridViewAdvancedCellBorderStyle.NotSet,
                                                                            DataGridViewAdvancedCellBorderStyle.NotSet)
        {
        }

        /// <summary>
        ///  Creates a new DataGridViewAdvancedBorderStyle. The specified owner will
        ///  be notified when the values are changed.
        /// </summary>
        internal DataGridViewAdvancedBorderStyle(DataGridView owner,
            DataGridViewAdvancedCellBorderStyle banned1,
            DataGridViewAdvancedCellBorderStyle banned2,
            DataGridViewAdvancedCellBorderStyle banned3)
        {
            this.owner = owner;
            this.banned1 = banned1;
            this.banned2 = banned2;
            this.banned3 = banned3;
        }

        public DataGridViewAdvancedCellBorderStyle All
        {
            get
            {
                return all ? top : DataGridViewAdvancedCellBorderStyle.NotSet;
            }
            set
            {

                // Sequential enum.  Valid values are 0x0 to 0x7
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewAdvancedCellBorderStyle.NotSet, (int)DataGridViewAdvancedCellBorderStyle.OutsetPartial))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewAdvancedCellBorderStyle));
                }
                if (value == DataGridViewAdvancedCellBorderStyle.NotSet ||
                    value == banned1 ||
                    value == banned2 ||
                    value == banned3)
                {
                    throw new ArgumentException(string.Format(SR.DataGridView_AdvancedCellBorderStyleInvalid, "All"));
                }
                if (!all || top != value)
                {
                    all = true;
                    top = left = right = bottom = value;
                    if (owner != null)
                    {
                        owner.OnAdvancedBorderStyleChanged(this);
                    }
                }
            }
        }

        public DataGridViewAdvancedCellBorderStyle Bottom
        {
            get
            {
                if (all)
                {
                    return top;
                }
                return bottom;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x7
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewAdvancedCellBorderStyle.NotSet, (int)DataGridViewAdvancedCellBorderStyle.OutsetPartial))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewAdvancedCellBorderStyle));
                }
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
                //Debug.Assert(Enum.IsDefined(typeof(DataGridViewAdvancedCellBorderStyle), value));
                //Debug.Assert(value != DataGridViewAdvancedCellBorderStyle.NotSet);
                if ((all && top != value) || (!all && bottom != value))
                {
                    if (all)
                    {
                        if (right == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                        {
                            right = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                    }
                    all = false;
                    bottom = value;
                    if (owner != null)
                    {
                        owner.OnAdvancedBorderStyleChanged(this);
                    }
                }
            }
        }

        public DataGridViewAdvancedCellBorderStyle Left
        {
            get
            {
                if (all)
                {
                    return top;
                }
                return left;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x7
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewAdvancedCellBorderStyle.NotSet, (int)DataGridViewAdvancedCellBorderStyle.OutsetPartial))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewAdvancedCellBorderStyle));
                }
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
                //Debug.Assert(Enum.IsDefined(typeof(DataGridViewAdvancedCellBorderStyle), value));
                //Debug.Assert(value != DataGridViewAdvancedCellBorderStyle.NotSet);
                if ((all && top != value) || (!all && left != value))
                {
                    if ((owner != null && owner.RightToLeftInternal) &&
                        (value == DataGridViewAdvancedCellBorderStyle.InsetDouble || value == DataGridViewAdvancedCellBorderStyle.OutsetDouble))
                    {
                        throw new ArgumentException(string.Format(SR.DataGridView_AdvancedCellBorderStyleInvalid, "Left"));
                    }
                    if (all)
                    {
                        if (right == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                        {
                            right = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                        if (bottom == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                        {
                            bottom = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                    }
                    all = false;
                    left = value;
                    if (owner != null)
                    {
                        owner.OnAdvancedBorderStyleChanged(this);
                    }
                }
            }
        }

        public DataGridViewAdvancedCellBorderStyle Right
        {
            get
            {
                if (all)
                {
                    return top;
                }
                return right;
            }
            set
            {

                // Sequential enum.  Valid values are 0x0 to 0x7
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewAdvancedCellBorderStyle.NotSet, (int)DataGridViewAdvancedCellBorderStyle.OutsetPartial))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewAdvancedCellBorderStyle));
                }
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
                //Debug.Assert(Enum.IsDefined(typeof(DataGridViewAdvancedCellBorderStyle), value));
                //Debug.Assert(value != DataGridViewAdvancedCellBorderStyle.NotSet);
                if ((all && top != value) || (!all && right != value))
                {
                    if ((owner != null && !owner.RightToLeftInternal) &&
                        (value == DataGridViewAdvancedCellBorderStyle.InsetDouble || value == DataGridViewAdvancedCellBorderStyle.OutsetDouble))
                    {
                        throw new ArgumentException(string.Format(SR.DataGridView_AdvancedCellBorderStyleInvalid, "Right"));
                    }
                    if (all)
                    {
                        if (bottom == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                        {
                            bottom = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                    }
                    all = false;
                    right = value;
                    if (owner != null)
                    {
                        owner.OnAdvancedBorderStyleChanged(this);
                    }
                }
            }
        }

        public DataGridViewAdvancedCellBorderStyle Top
        {
            get
            {
                return top;
            }
            set
            {
                // Sequential enum.  Valid values are 0x0 to 0x7
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewAdvancedCellBorderStyle.NotSet, (int)DataGridViewAdvancedCellBorderStyle.OutsetPartial))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewAdvancedCellBorderStyle));
                }
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
                //Debug.Assert(Enum.IsDefined(typeof(DataGridViewAdvancedCellBorderStyle), value));
                //Debug.Assert(value != DataGridViewAdvancedCellBorderStyle.NotSet);
                if ((all && top != value) || (!all && top != value))
                {
                    if (all)
                    {
                        if (right == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                        {
                            right = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                        if (bottom == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                        {
                            bottom = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                    }
                    all = false;
                    top = value;
                    if (owner != null)
                    {
                        owner.OnAdvancedBorderStyleChanged(this);
                    }
                }
            }
        }

        public override bool Equals(object other)
        {
            if (!(other is DataGridViewAdvancedBorderStyle dgvabsOther))
            {
                return false;
            }

            return dgvabsOther.all == all &&
                dgvabsOther.top == top &&
                dgvabsOther.left == left &&
                dgvabsOther.bottom == bottom &&
                dgvabsOther.right == right;
        }

        public override int GetHashCode() => HashCode.Combine(top, left, bottom, right);

        public override string ToString()
        {
            return "DataGridViewAdvancedBorderStyle { All=" + All.ToString() + ", Left=" + Left.ToString() + ", Right=" + Right.ToString() + ", Top=" + Top.ToString() + ", Bottom=" + Bottom.ToString() + " }";
        }

        object ICloneable.Clone()
        {
            DataGridViewAdvancedBorderStyle dgvabs = new DataGridViewAdvancedBorderStyle(owner, banned1, banned2, banned3)
            {
                all = all,
                top = top,
                right = right,
                bottom = bottom,
                left = left
            };
            return dgvabs;
        }
    }
}
