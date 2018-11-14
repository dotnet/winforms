// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms.PropertyGridInternal {

    using System.Diagnostics;

    using System;
    using System.Collections;
    using System.Reflection;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Windows.Forms;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Globalization;

    internal class ArrayElementGridEntry : GridEntry {

        protected int index;


        public ArrayElementGridEntry(PropertyGrid ownerGrid, GridEntry peParent, int index)
        : base(ownerGrid, peParent) {
            this.index = index;
            this.SetFlag(FLAG_RENDER_READONLY, (peParent.Flags & FLAG_RENDER_READONLY) != 0 || peParent.ForceReadOnly);
        }

        
        public override GridItemType GridItemType {
            get {
                return GridItemType.ArrayValue;
            }
        }


        public override bool IsValueEditable {
            get{
                return ParentGridEntry.IsValueEditable;
            }
        }

        public override string PropertyLabel {
            get {
                return "[" + index.ToString(CultureInfo.CurrentCulture) + "]";
            }
        }



        public override Type PropertyType {
            get {
                return parentPE.PropertyType.GetElementType();
            }
        }

        public override object PropertyValue {
            get {
                object owner = GetValueOwner();
                Debug.Assert(owner is Array, "Owner is not array type!");
                return((Array)owner).GetValue(index);
            }
            set {
                object owner = GetValueOwner();
                Debug.Assert(owner is Array, "Owner is not array type!");
                ((Array)owner).SetValue(value,index);
            }
        }

        public override bool ShouldRenderReadOnly {
            get {
                return ParentGridEntry.ShouldRenderReadOnly;
            }
        }

        /*
        /// <summary>
        /// Checks if the value of the current item can be modified by the user.
        /// </summary>
        /// <returns>
        /// True if the value can be modified
        /// </returns>
        public override bool CanSetPropertyValue() {
           return this.ParentGridEntry.CanSetPropertyValue();
        }
        */

        /*
        /// <summary>
        /// Returns if it's an editable item.  An example of a readonly
        /// editable item is a collection property -- the property itself
        /// can not be modified, but it's value (e.g. it's children) can, so
        /// we don't want to draw it as readonly.
        /// </summary>
        /// <returns>
        /// True if the value associated with this property (e.g. it's children) can be modified even if it's readonly.
        /// </returns>
        public override bool CanSetReadOnlyPropertyValue() {
           return this.ParentGridEntry.CanSetReadOnlyPropertyValue();
        }*/
       
    }
}
