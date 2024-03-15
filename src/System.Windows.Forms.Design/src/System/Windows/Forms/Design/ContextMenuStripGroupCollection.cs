// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms.Design;

internal class ContextMenuStripGroupCollection : DictionaryBase
{
    public ContextMenuStripGroupCollection()
    {
    }

    public ContextMenuStripGroup this[string key]
    {
        get
        {
            if (!InnerHashtable.ContainsKey(key))
            {
                InnerHashtable[key] = new ContextMenuStripGroup();
            }

            return (ContextMenuStripGroup)InnerHashtable[key]!;
        }
    }

    public bool ContainsKey(string key)
    {
        return InnerHashtable.ContainsKey(key);
    }

    protected override void OnInsert(object key, object? value)
    {
        if (value is not ContextMenuStripGroup)
        {
            throw new NotSupportedException();
        }

        base.OnInsert(key, value);
    }

    protected override void OnSet(object key, object? oldValue, object? newValue)
    {
        if (newValue is not ContextMenuStripGroup)
        {
            throw new NotSupportedException();
        }

        base.OnSet(key, oldValue, newValue);
    }
}
