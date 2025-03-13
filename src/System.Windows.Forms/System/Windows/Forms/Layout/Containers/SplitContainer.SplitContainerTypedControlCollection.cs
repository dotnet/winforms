// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class SplitContainer
{
    /// <summary>
    ///  This control collection only allows a specific type of control
    ///  into the controls collection. It optionally supports readonlyness.
    /// </summary>
    internal class SplitContainerTypedControlCollection : TypedControlCollection
    {
        private readonly SplitContainer _owner;

        public SplitContainerTypedControlCollection(SplitContainer splitContainer, Type type, bool isReadOnly)
            : base(splitContainer, type, isReadOnly)
        {
            _owner = splitContainer;
        }

        public override void Remove(Control? value)
        {
            if (value is SplitterPanel && !_owner.DesignMode && IsReadOnly)
            {
                throw new NotSupportedException(SR.ReadonlyControlsCollection);
            }

            base.Remove(value);
        }

        internal override void SetChildIndexInternal(Control child, int newIndex)
        {
            if (child is not SplitterPanel)
            {
                base.SetChildIndexInternal(child, newIndex);
                return;
            }

            if (_owner.DesignMode)
            {
                // just no-op it at DT.
                return;
            }

            if (IsReadOnly)
            {
                throw new NotSupportedException(SR.ReadonlyControlsCollection);
            }

            base.SetChildIndexInternal(child, newIndex);
        }
    }
}
