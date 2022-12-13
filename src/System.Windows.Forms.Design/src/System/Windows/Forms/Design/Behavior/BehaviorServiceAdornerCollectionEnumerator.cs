// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;

namespace System.Windows.Forms.Design.Behavior
{
    public class BehaviorServiceAdornerCollectionEnumerator : object, IEnumerator
    {
        private readonly IEnumerator baseEnumerator;
        private readonly IEnumerable temp;

        public BehaviorServiceAdornerCollectionEnumerator(BehaviorServiceAdornerCollection mappings)
        {
            temp = mappings;
            baseEnumerator = temp.GetEnumerator();
        }

        public Adorner Current
        {
            get
            {
                return ((Adorner)(baseEnumerator.Current));
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return baseEnumerator.Current;
            }
        }

        public bool MoveNext()
        {
            return baseEnumerator.MoveNext();
        }

        bool IEnumerator.MoveNext()
        {
            return baseEnumerator.MoveNext();
        }

        public void Reset()
        {
            baseEnumerator.Reset();
        }

        void IEnumerator.Reset()
        {
            baseEnumerator.Reset();
        }
    }
}
