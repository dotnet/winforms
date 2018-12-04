// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.ComponentModel.Design
{
    public class DesignerActionItemCollection : CollectionBase
    {
        public DesignerActionItem this[int index]
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public int Add(DesignerActionItem value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public bool Contains(DesignerActionItem value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void CopyTo(DesignerActionItem[] array, int index)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public int IndexOf(DesignerActionItem value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void Insert(int index, DesignerActionItem value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void Remove(DesignerActionItem value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
