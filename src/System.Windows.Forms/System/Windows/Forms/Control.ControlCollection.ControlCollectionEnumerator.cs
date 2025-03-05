// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

public partial class Control
{
    public partial class ControlCollection
    {
        // This is the same as ArraySubsetEnumerator
        // however since we're no longer an array, we've gotta employ a
        // special version of this.
        private class ControlCollectionEnumerator : IEnumerator
        {
            private readonly ControlCollection _controls;
            private int _current;
            private readonly int _originalCount;

            public ControlCollectionEnumerator(ControlCollection controls)
            {
                _controls = controls;
                _originalCount = controls.Count;
                _current = -1;
            }

            public bool MoveNext()
            {
                // We have to use Controls.Count here because someone could have deleted
                // an item from the array.
                //
                // this can happen if someone does:
                //     foreach (Control c in Controls) { c.Dispose(); }
                //
                // We also don't want to iterate past the original size of the collection
                //
                // this can happen if someone does
                //     foreach (Control c in Controls) { c.Controls.Add(new Label()); }

                if (_current < _controls.Count - 1 && _current < _originalCount - 1)
                {
                    _current++;
                    return true;
                }
                else
                {
                    return false;
                }
            }

            public void Reset()
            {
                _current = -1;
            }

            public object? Current
            {
                get
                {
                    if (_current == -1)
                    {
                        return null;
                    }
                    else
                    {
                        return _controls[_current];
                    }
                }
            }
        }
    }
}
