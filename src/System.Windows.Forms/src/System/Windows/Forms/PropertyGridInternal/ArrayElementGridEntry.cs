// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal class ArrayElementGridEntry : GridEntry
    {
        protected int index;

        public ArrayElementGridEntry(PropertyGrid ownerGrid, GridEntry peParent, int index)
        : base(ownerGrid, peParent)
        {
            this.index = index;
            SetFlag(FLAG_RENDER_READONLY, (peParent.Flags & FLAG_RENDER_READONLY) != 0 || peParent.ForceReadOnly);
        }

        public override GridItemType GridItemType
        {
            get
            {
                return GridItemType.ArrayValue;
            }
        }

        public override bool IsValueEditable
        {
            get
            {
                return ParentGridEntry.IsValueEditable;
            }
        }

        public override string PropertyLabel
        {
            get
            {
                return "[" + index.ToString(CultureInfo.CurrentCulture) + "]";
            }
        }

        public override Type PropertyType
        {
            get
            {
                return parentPE.PropertyType.GetElementType();
            }
        }

        public override object PropertyValue
        {
            get
            {
                object owner = GetValueOwner();
                Debug.Assert(owner is Array, "Owner is not array type!");
                return ((Array)owner).GetValue(index);
            }
            set
            {
                object owner = GetValueOwner();
                Debug.Assert(owner is Array, "Owner is not array type!");
                ((Array)owner).SetValue(value, index);
            }
        }

        public override bool ShouldRenderReadOnly
        {
            get
            {
                return ParentGridEntry.ShouldRenderReadOnly;
            }
        }
    }
}
