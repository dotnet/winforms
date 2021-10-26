// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Globalization;

namespace System.Windows.Forms
{
    public partial class ToolStripContainer
    {
        internal class ToolStripContainerTypedControlCollection : ReadOnlyControlCollection
        {
            readonly ToolStripContainer owner;
            readonly Type contentPanelType = typeof(ToolStripContentPanel);
            readonly Type panelType = typeof(ToolStripPanel);

            public ToolStripContainerTypedControlCollection(Control c, bool isReadOnly)
                : base(c, isReadOnly)
            {
                owner = c as ToolStripContainer;
            }

            public override void Add(Control value)
            {
                ArgumentNullException.ThrowIfNull(value);

                if (IsReadOnly)
                {
                    throw new NotSupportedException(SR.ToolStripContainerUseContentPanel);
                }

                Type controlType = value.GetType();
                if (!contentPanelType.IsAssignableFrom(controlType) && !panelType.IsAssignableFrom(controlType))
                {
                    throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, string.Format(SR.TypedControlCollectionShouldBeOfTypes, contentPanelType.Name, panelType.Name)), value.GetType().Name);
                }

                base.Add(value);
            }

            public override void Remove(Control value)
            {
                if (value is ToolStripPanel || value is ToolStripContentPanel)
                {
                    if (!owner.DesignMode)
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
                if (child is ToolStripPanel || child is ToolStripContentPanel)
                {
                    if (!owner.DesignMode)
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
