// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.ComponentModel.Design
{
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
            get => (DesignerActionList)(List[index]);
            set => List[index] = value;
        }

        public int Add(DesignerActionList value) => List.Add(value);

        public void AddRange(DesignerActionList[] value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            for (int i = 0; i < value.Length; i++)
            {
                Add(value[i]);
            }
        }

        public void AddRange(DesignerActionListCollection value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            int currentCount = value.Count;
            for (int i = 0; i < currentCount; i++)
            {
                Add(value[i]);
            }
        }

        public void Insert(int index, DesignerActionList value) => List.Insert(index, value);

        public int IndexOf(DesignerActionList value) => List.IndexOf(value);

        public bool Contains(DesignerActionList value) => List.Contains(value);

        public void Remove(DesignerActionList value) => List.Remove(value);

        public void CopyTo(DesignerActionList[] array, int index) => List.CopyTo(array, index);

        protected override void OnValidate(object value)
        {
            // Don't perform any validation.
        }
    }
}
