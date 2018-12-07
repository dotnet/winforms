// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace System.ComponentModel.Design
{
    [ComVisible(true)]
    [PermissionSet(SecurityAction.InheritanceDemand, Name = "FullTrust")]
    public class DesignerActionListCollection : CollectionBase
    {
        public DesignerActionListCollection()
        {
        }

        public DesignerActionListCollection(DesignerActionList[] value)
        {
            AddRange(value);
        }

        public DesignerActionList this[int index]
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public int Add(DesignerActionList value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void AddRange(DesignerActionList[] value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void AddRange(DesignerActionListCollection value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void Insert(int index, DesignerActionList value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public int IndexOf(DesignerActionList value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public bool Contains(DesignerActionList value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void Remove(DesignerActionList value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void CopyTo(DesignerActionList[] array, int index)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        protected override void OnSet(int index, object oldValue, object newValue)
        {
        }

        protected override void OnInsert(int index, object value)
        {
        }

        protected override void OnClear()
        {
        }

        protected override void OnRemove(int index, object value)
        {
        }

        protected override void OnValidate(object value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
