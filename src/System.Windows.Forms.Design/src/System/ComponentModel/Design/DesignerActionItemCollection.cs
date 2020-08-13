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
            get => (DesignerActionItem)(List[index]);
            set => List[index] = value;
        }

        public int Add(DesignerActionItem value) => List.Add(value);

        public bool Contains(DesignerActionItem value) => List.Contains(value);

        public void CopyTo(DesignerActionItem[] array, int index) => List.CopyTo(array, index);

        public int IndexOf(DesignerActionItem value) => List.IndexOf(value);

        public void Insert(int index, DesignerActionItem value) => List.Insert(index, value);

        public void Remove(DesignerActionItem value) => List.Remove(value);
    }
}
