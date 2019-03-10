// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Representaion of one row item in the PropertyGrid. These items represent the
    /// hierarchy of the grid's "tree-like" view and can be used to get information about
    /// the grid's state and contents.
    /// These objects should not be cached because they represent a snapshot of the
    /// PropertyGrid's state and may be disposed by grid activity. The PropertyGrid often
    /// recretates these objects internally even if it doesn't appear to change to the user.
    /// </devdoc>
    public abstract class GridItem
    {
        [SRCategory(nameof(SR.CatData))]
        [Localizable(false)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ControlTagDescr))]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag { get; set; }

        /// <devdoc>
        /// Retrieves the child GridItems, if any, of this GridItem
        /// </devdoc>
        public abstract GridItemCollection GridItems { get; }

        /// <devdoc>
        /// Retrieves type of this GridItem, as a value from System.Windows.Forms.GridItemType
        /// </devdoc>
        public abstract GridItemType GridItemType { get; }

        /// <devdoc>
        /// Retrieves the text label of this GridItem. This may be different from the actual
        /// PropertyName. For GridItemType.Property GridItems, retrieve the PropertyDescriptor
        /// and check its Name property.
        /// </devdoc>
        public abstract string Label { get; }

        /// <devdoc>
        /// Retrieves parent GridItem of this GridItem, if any.
        /// </devdoc>
        public abstract GridItem Parent { get; }

        /// <devdoc>
        /// If this item is a GridItemType.Property GridItem, this retreives the
        /// System.ComponentModel.PropertyDescriptor that is associated with this GridItem.
        /// This can be used to retrieve infomration such as property Type, Name, or
        /// TypeConverter.
        /// </devdoc>
        public abstract PropertyDescriptor PropertyDescriptor { get; }

        /// <devdoc>
        /// Retrieves the current Value of this grid Item. This may be null.
        /// </devdoc>
        /// <remarks>
        /// We don't do set because of the value class semantics, etc.
        /// </remarks>
        public abstract object Value { get; }

        /// <devdoc>
        /// Retreives whether the given property is expandable.
        /// </devdoc>
        public virtual bool Expandable => false;

        /// <devdoc>
        /// Retreives or sets whether the GridItem is in an expanded state.
        /// </devdoc>
        public virtual bool Expanded
        {
            get => false;
            set => throw new NotSupportedException(SR.GridItemNotExpandable);
        }

        /// <devdoc>
        /// <para>Attempts to select this GridItem in the PropertyGrid.
        /// </devdoc>
        public abstract bool Select();
    }
}
