// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.Windows.Forms;

public partial class ToolStripContainer
{
    internal class ToolStripContainerTypedControlCollection : ReadOnlyControlCollection
    {
        private readonly ToolStripContainer _owner;
        private readonly Type _contentPanelType = typeof(ToolStripContentPanel);
        private readonly Type _panelType = typeof(ToolStripPanel);

        public ToolStripContainerTypedControlCollection(ToolStripContainer c, bool isReadOnly)
            : base(c, isReadOnly)
        {
            _owner = c;
        }

        public override void Add(Control? value)
        {
            if (value is null)
            {
                return;
            }

            if (IsReadOnly)
            {
                throw new NotSupportedException(SR.ToolStripContainerUseContentPanel);
            }

            Type controlType = value.GetType();
            if (!_contentPanelType.IsAssignableFrom(controlType) && !_panelType.IsAssignableFrom(controlType))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, string.Format(SR.TypedControlCollectionShouldBeOfTypes, _contentPanelType.Name, _panelType.Name)), value.GetType().Name);
            }

            base.Add(value);
        }

        public override void Remove(Control? value)
        {
            if (value is ToolStripPanel or ToolStripContentPanel)
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
            if (child is ToolStripPanel or ToolStripContentPanel)
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
