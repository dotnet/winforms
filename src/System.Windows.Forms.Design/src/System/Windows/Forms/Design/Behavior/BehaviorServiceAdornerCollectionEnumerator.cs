// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Design.Behavior;

public class BehaviorServiceAdornerCollectionEnumerator : object, IEnumerator
{
    private readonly IEnumerator _baseEnumerator;

    public BehaviorServiceAdornerCollectionEnumerator(BehaviorServiceAdornerCollection mappings)
    {
        _baseEnumerator = ((IEnumerable)mappings).GetEnumerator();
    }

#nullable disable // explicitly leaving Current as "oblivious" to avoid spurious warnings in foreach over non-generic enumerables
    public Adorner Current => (Adorner)_baseEnumerator.Current;

    object IEnumerator.Current => _baseEnumerator.Current;
#nullable restore

    public bool MoveNext()
    {
        return _baseEnumerator.MoveNext();
    }

    bool IEnumerator.MoveNext()
    {
        return _baseEnumerator.MoveNext();
    }

    public void Reset()
    {
        _baseEnumerator.Reset();
    }

    void IEnumerator.Reset()
    {
        _baseEnumerator.Reset();
    }
}
