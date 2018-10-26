// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.ComponentModel;

    /// <include file='doc\GridItem.uex' path='docs/doc[@for="GridItem"]/*' />
    /// <devdoc>
    /// <para>Representaion of one row item in the PropertyGrid.  These items represent the
    /// hierarchy of the grid's "tree-like" view and can be used to get information about
    /// the grid's state and contents.</para>
    /// <para>These objects should not be cached because they represent a snapshot of the PropertyGrid's state
    ///  and may be disposed by grid activity.  The PropertyGrid often recretates these objects internally even if
    ///  it doesn't appear to change to the user. </para>
    /// </devdoc>
    public abstract class GridItem {

        private object userData;

        /// <include file='doc\GridItem.uex' path='docs/doc[@for="GridItem.Tag"]/*' />
        [
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ControlTagDescr)),
        DefaultValue(null),
        TypeConverter(typeof(StringConverter)),
        ]
        public object Tag {
            get {
                return userData;
            }
            set {
                userData = value;
            }
        }
    
        /// <include file='doc\GridItem.uex' path='docs/doc[@for="GridItem.GridItems"]/*' />
        /// <devdoc>
        /// <para>Retrieves the child GridItems, if any, of this GridItem</para>
        /// </devdoc>
        public abstract GridItemCollection GridItems {
            get;
        }
        
        /// <include file='doc\GridItem.uex' path='docs/doc[@for="GridItem.GridItemType"]/*' />
        /// <devdoc>
        /// <para>Retrieves type of this GridItem, as a value from System.Windows.Forms.GridItemType</para>
        /// </devdoc>
        public abstract GridItemType GridItemType {
            get;
        }
        
        /// <include file='doc\GridItem.uex' path='docs/doc[@for="GridItem.Label"]/*' />
        /// <devdoc>
        /// <para>Retrieves the text label of this GridItem.  This may be different from the actual PropertyName.
        ///       For GridItemType.Property GridItems, retrieve the PropertyDescriptor and check its Name property.</para>
        /// </devdoc>
        public abstract string Label {
            get;
        }
        
        /// <include file='doc\GridItem.uex' path='docs/doc[@for="GridItem.Parent"]/*' />
        /// <devdoc>
        /// <para>Retrieves parent GridItem of this GridItem, if any</para>
        /// </devdoc>
        public abstract GridItem Parent {
            get;
        }
        
        /// <include file='doc\GridItem.uex' path='docs/doc[@for="GridItem.PropertyDescriptor"]/*' />
        /// <devdoc>
        /// <para>If this item is a GridItemType.Property GridItem, this retreives the System.ComponentModel.PropertyDescriptor that is
        ///       associated with this GridItem.  This can be used to retrieve infomration such as property Type, Name, or TypeConverter.</para>
        /// </devdoc>
        public abstract PropertyDescriptor PropertyDescriptor {
            get;
        }
        
        /// <include file='doc\GridItem.uex' path='docs/doc[@for="GridItem.Value"]/*' />
        /// <devdoc>
        /// <para>Retrieves the current Value of this grid Item.  This may be null. </para>
        /// </devdoc>
        public abstract object Value {
            get;
            // note: we don't do set because of the value class semantics, etc.
        }
        
        /// <include file='doc\GridItem.uex' path='docs/doc[@for="GridItem.Expandable"]/*' />
        /// <devdoc>
        /// <para>Retreives whether the given property is expandable.</para>
        /// </devdoc>
        public virtual bool Expandable {
            get {
                return false;
            }
        }
        
        /// <include file='doc\GridItem.uex' path='docs/doc[@for="GridItem.Expanded"]/*' />
        /// <devdoc>
        /// <para>Retreives or sets whether the GridItem is in an expanded state.</para>
        /// </devdoc>
        public virtual bool Expanded {
            get {
                return false;
            }
            set {
                throw new NotSupportedException(SR.GridItemNotExpandable);
            }
        }
        
        /// <include file='doc\GridItem.uex' path='docs/doc[@for="GridItem.Select"]/*' />
        /// <devdoc>
        /// <para>Attempts to select this GridItem in the PropertyGrid.</para>
        /// </devdoc>
        public abstract bool Select();
    }
}
