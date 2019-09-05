// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Representaion of one row item in the PropertyGrid. These items represent the
    ///  hierarchy of the grid's "tree-like" view and can be used to get information about
    ///  the grid's state and contents.
    ///  These objects should not be cached because they represent a snapshot of the
    ///  PropertyGrid's state and may be disposed by grid activity. The PropertyGrid often
    ///  recretates these objects internally even if it doesn't appear to change to the user.
    /// </summary>
    public abstract class GridItem
    {
        [SRCategory(nameof(SR.CatData))]
        [Localizable(false)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ControlTagDescr))]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag { get; set; }

        /// <summary>
        ///  Retrieves the child GridItems, if any, of this GridItem
        /// </summary>
        public abstract GridItemCollection GridItems { get; }

        /// <summary>
        ///  Retrieves type of this GridItem, as a value from System.Windows.Forms.GridItemType
        /// </summary>
        public abstract GridItemType GridItemType { get; }

        /// <summary>
        ///  Retrieves the text label of this GridItem. This may be different from the actual
        ///  PropertyName. For GridItemType.Property GridItems, retrieve the PropertyDescriptor
        ///  and check its Name property.
        /// </summary>
        public abstract string Label { get; }

        /// <summary>
        ///  Retrieves parent GridItem of this GridItem, if any.
        /// </summary>
        public abstract GridItem Parent { get; }

        /// <summary>
        ///  If this item is a GridItemType.Property GridItem, this retreives the
        ///  System.ComponentModel.PropertyDescriptor that is associated with this GridItem.
        ///  This can be used to retrieve infomration such as property Type, Name, or
        ///  TypeConverter.
        /// </summary>
        public abstract PropertyDescriptor PropertyDescriptor { get; }

        /// <summary>
        ///  Retrieves the current Value of this grid Item. This may be null.
        /// </summary>
        // We don't do set because of the value class semantics, etc.
        public abstract object Value { get; }

        /// <summary>
        ///  Retreives whether the given property is expandable.
        /// </summary>
        public virtual bool Expandable => false;

        /// <summary>
        ///  Retreives or sets whether the GridItem is in an expanded state.
        /// </summary>
        public virtual bool Expanded
        {
            get => false;
            set => throw new NotSupportedException(SR.GridItemNotExpandable);
        }

        /// <summary>
        ///  Attempts to select this GridItem in the PropertyGrid.
        /// </summary>
        public abstract bool Select();
    }
}
