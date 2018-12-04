// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.ComponentModel;

    /// <include file='doc\DataGridViewAdvancedBorderStyle.uex' path='docs/doc[@for="DataGridViewAdvancedBorderStyle"]/*' />
    public sealed class DataGridViewAdvancedBorderStyle : ICloneable
    {
        private DataGridView owner;
        private bool all = true;
        private DataGridViewAdvancedCellBorderStyle banned1, banned2, banned3;
        private DataGridViewAdvancedCellBorderStyle top = DataGridViewAdvancedCellBorderStyle.None;
        private DataGridViewAdvancedCellBorderStyle left = DataGridViewAdvancedCellBorderStyle.None;
        private DataGridViewAdvancedCellBorderStyle right = DataGridViewAdvancedCellBorderStyle.None;
        private DataGridViewAdvancedCellBorderStyle bottom = DataGridViewAdvancedCellBorderStyle.None;

        /// <include file='doc\DataGridViewAdvancedBorderStyle.uex' path='docs/doc[@for="DataGridViewAdvancedBorderStyle.DataGridViewAdvancedBorderStyle"]/*' />
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

        /// <devdoc>
        ///     Creates a new DataGridViewAdvancedBorderStyle. The specified owner will
        ///     be notified when the values are changed.
        /// </devdoc>
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

        /// <include file='doc\DataGridViewAdvancedBorderStyle.uex' path='docs/doc[@for="DataGridViewAdvancedBorderStyle.All"]/*' />
        public DataGridViewAdvancedCellBorderStyle All
        {
            get 
            {
                return this.all ? this.top : DataGridViewAdvancedCellBorderStyle.NotSet;
            }
            set 
            {

                // Sequential enum.  Valid values are 0x0 to 0x7
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DataGridViewAdvancedCellBorderStyle.NotSet, (int)DataGridViewAdvancedCellBorderStyle.OutsetPartial))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DataGridViewAdvancedCellBorderStyle)); 
                }
                if (value == DataGridViewAdvancedCellBorderStyle.NotSet ||
                    value == this.banned1 ||
                    value == this.banned2 ||
                    value == this.banned3)
                {
                    throw new ArgumentException(string.Format(SR.DataGridView_AdvancedCellBorderStyleInvalid, "All"));
                }
                if (!this.all || this.top != value)
                {
                    this.all = true;
                    this.top = this.left = this.right = this.bottom = value;
                    if (this.owner != null)
                    {
                        this.owner.OnAdvancedBorderStyleChanged(this);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewAdvancedBorderStyle.uex' path='docs/doc[@for="DataGridViewAdvancedBorderStyle.Bottom"]/*' />
        public DataGridViewAdvancedCellBorderStyle Bottom
        {
            get 
            {
                if (this.all) 
                {
                    return this.top;
                }
                return this.bottom;
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
                this.BottomInternal = value;
            }
        }

        internal DataGridViewAdvancedCellBorderStyle BottomInternal
        {
            set
            {
                //Debug.Assert(Enum.IsDefined(typeof(DataGridViewAdvancedCellBorderStyle), value));
                //Debug.Assert(value != DataGridViewAdvancedCellBorderStyle.NotSet);
                if ((this.all && this.top != value) || (!this.all && this.bottom != value))
                {
                    if (this.all)
                    {
                        if (this.right == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                        {
                            this.right = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                    }
                    this.all = false;
                    this.bottom = value;
                    if (this.owner != null)
                    {
                        this.owner.OnAdvancedBorderStyleChanged(this);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewAdvancedBorderStyle.uex' path='docs/doc[@for="DataGridViewAdvancedBorderStyle.Left"]/*' />
        public DataGridViewAdvancedCellBorderStyle Left
        {
            get 
            {
                if (this.all) 
                {
                    return this.top;
                }
                return this.left;
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
                this.LeftInternal = value;
            }
        }

        internal DataGridViewAdvancedCellBorderStyle LeftInternal
        {
            set
            {
                //Debug.Assert(Enum.IsDefined(typeof(DataGridViewAdvancedCellBorderStyle), value));
                //Debug.Assert(value != DataGridViewAdvancedCellBorderStyle.NotSet);
                if ((this.all && this.top != value) || (!this.all && this.left != value))
                {
                    if ((this.owner != null && this.owner.RightToLeftInternal) &&
                        (value == DataGridViewAdvancedCellBorderStyle.InsetDouble || value == DataGridViewAdvancedCellBorderStyle.OutsetDouble))
                    {
                        throw new ArgumentException(string.Format(SR.DataGridView_AdvancedCellBorderStyleInvalid, "Left"));
                    }
                    if (this.all)
                    {
                        if (this.right == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                        {
                            this.right = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                        if (this.bottom == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                        {
                            this.bottom = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                    }
                    this.all = false;
                    this.left = value;
                    if (this.owner != null)
                    {
                        this.owner.OnAdvancedBorderStyleChanged(this);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewAdvancedBorderStyle.uex' path='docs/doc[@for="DataGridViewAdvancedBorderStyle.Right"]/*' />
        public DataGridViewAdvancedCellBorderStyle Right
        {
            get 
            {
                if (this.all) 
                {
                    return this.top;
                }
                return this.right;
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
                this.RightInternal = value;
            }
        }

        internal DataGridViewAdvancedCellBorderStyle RightInternal
        {
            set
            {
                //Debug.Assert(Enum.IsDefined(typeof(DataGridViewAdvancedCellBorderStyle), value));
                //Debug.Assert(value != DataGridViewAdvancedCellBorderStyle.NotSet);
                if ((this.all && this.top != value) || (!this.all && this.right != value))
                {
                    if ((this.owner != null && !this.owner.RightToLeftInternal) &&
                        (value == DataGridViewAdvancedCellBorderStyle.InsetDouble || value == DataGridViewAdvancedCellBorderStyle.OutsetDouble))
                    {
                        throw new ArgumentException(string.Format(SR.DataGridView_AdvancedCellBorderStyleInvalid, "Right"));
                    }
                    if (this.all)
                    {
                        if (this.bottom == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                        {
                            this.bottom = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                    }
                    this.all = false;
                    this.right = value;
                    if (this.owner != null)
                    {
                        this.owner.OnAdvancedBorderStyleChanged(this);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewAdvancedBorderStyle.uex' path='docs/doc[@for="DataGridViewAdvancedBorderStyle.Top"]/*' />
        public DataGridViewAdvancedCellBorderStyle Top
        {
            get 
            {
                return this.top;
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
                this.TopInternal = value;
            }
        }

        internal DataGridViewAdvancedCellBorderStyle TopInternal
        {
            set
            {
                //Debug.Assert(Enum.IsDefined(typeof(DataGridViewAdvancedCellBorderStyle), value));
                //Debug.Assert(value != DataGridViewAdvancedCellBorderStyle.NotSet);
                if ((this.all && this.top != value) || (!this.all && this.top != value))
                {
                    if (this.all)
                    {
                        if (this.right == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                        {
                            this.right = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                        if (this.bottom == DataGridViewAdvancedCellBorderStyle.OutsetDouble)
                        {
                            this.bottom = DataGridViewAdvancedCellBorderStyle.Outset;
                        }
                    }
                    this.all = false;
                    this.top = value;
                    if (this.owner != null)
                    {
                        this.owner.OnAdvancedBorderStyleChanged(this);
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewAdvancedBorderStyle.uex' path='docs/doc[@for="DataGridViewAdvancedBorderStyle.Equals"]/*' />
        /// <internalonly/>
        public override bool Equals(object other) 
        {
            DataGridViewAdvancedBorderStyle dgvabsOther = other as DataGridViewAdvancedBorderStyle;

            if (dgvabsOther == null)
            {
                return false;
            }

            return dgvabsOther.all == this.all && 
                dgvabsOther.top == this.top && 
                dgvabsOther.left == this.left && 
                dgvabsOther.bottom == this.bottom && 
                dgvabsOther.right == this.right;
        }

        /// <include file='doc\DataGridViewAdvancedBorderStyle.uex' path='docs/doc[@for="DataGridViewAdvancedBorderStyle.GetHashCode"]/*' />
        /// <internalonly/>
        public override int GetHashCode() 
        {
            return WindowsFormsUtils.GetCombinedHashCodes((int) this.top,
                                                              (int) this.left,
                                                              (int) this.bottom,
                                                              (int) this.right);
        }

        /// <include file='doc\DataGridViewAdvancedBorderStyle.uex' path='docs/doc[@for="DataGridViewAdvancedBorderStyle.ToString"]/*' />
        /// <internalonly/>
        public override string ToString() 
        {
            return "DataGridViewAdvancedBorderStyle { All=" + this.All.ToString() + ", Left=" + this.Left.ToString() + ", Right=" + this.Right.ToString() + ", Top=" + this.Top.ToString() + ", Bottom=" + this.Bottom.ToString() + " }";
        }

        /// <include file='doc\DataGridViewAdvancedBorderStyle.uex' path='docs/doc[@for="DataGridViewAdvancedBorderStyle.ICloneable.Clone"]/*' />
        /// <internalonly/>
        object ICloneable.Clone() 
        {
            DataGridViewAdvancedBorderStyle dgvabs = new DataGridViewAdvancedBorderStyle(this.owner, this.banned1, this.banned2, this.banned3);
            dgvabs.all = this.all;
            dgvabs.top = this.top;
            dgvabs.right = this.right;
            dgvabs.bottom = this.bottom;
            dgvabs.left = this.left;
            return dgvabs;
        }
    }
}
