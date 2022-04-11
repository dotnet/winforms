// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public partial class SplitContainer
    {
        /// <summary>
        ///  This control collection only allows a specific type of control
        ///  into the controls collection.  It optionally supports readonlyness.
        /// </summary>
        internal class SplitContainerTypedControlCollection : TypedControlCollection
        {
            private readonly SplitContainer _owner;

            public SplitContainerTypedControlCollection(Control c, Type type, bool isReadOnly) : base(c, type, isReadOnly)
            {
                _owner = c as SplitContainer;
            }

            public override void Remove(Control value)
            {
                if (value is SplitterPanel)
                {
                    if (!_owner.DesignMode)
                    {
                        if (IsReadOnly)
                        {
                            throw new NotSupportedException(SR.ReadonlyControlsCollection);
                        }
                    }
                }

                base.Remove(value);
            }

            internal override void SetChildIndexInternal(Control child, int newIndex)
            {
                if (child is SplitterPanel)
                {
                    if (!_owner.DesignMode)
                    {
                        if (IsReadOnly)
                        {
                            throw new NotSupportedException(SR.ReadonlyControlsCollection);
                        }
                    }
                    else
                    {
                        // just no-op it at DT.
                        return;
                    }
                }

                base.SetChildIndexInternal(child, newIndex);
            }
        }
    }
}
