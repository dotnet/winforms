// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms.Design.Behavior;

public class BehaviorServiceAdornerCollectionEnumerator : object, IEnumerator
{
    private readonly IEnumerator baseEnumerator;

    public BehaviorServiceAdornerCollectionEnumerator(BehaviorServiceAdornerCollection mappings)
    {
        baseEnumerator = ((IEnumerable)mappings).GetEnumerator();
    }

#nullable disable // explicitly leaving Current as "oblivious" to avoid spurious warnings in foreach over non-generic enumerables
    public Adorner Current => (Adorner)baseEnumerator.Current;

    object IEnumerator.Current => baseEnumerator.Current;
#nullable restore

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
