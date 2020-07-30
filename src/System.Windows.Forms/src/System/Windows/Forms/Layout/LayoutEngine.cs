// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.Layout
{
    public abstract class LayoutEngine
    {
        internal IArrangedElement CastToArrangedElement(object obj)
        {
            if (!(obj is IArrangedElement element))
            {
                throw new NotSupportedException(string.Format(SR.LayoutEngineUnsupportedType, obj.GetType()));
            }

            return element;
        }

        internal virtual Size GetPreferredSize(IArrangedElement container, Size proposedConstraints)
        {
            return Size.Empty;
        }

        public virtual void InitLayout(object child, BoundsSpecified specified)
        {
            if (child is null)
            {
                throw new ArgumentNullException(nameof(child));
            }

            InitLayoutCore(CastToArrangedElement(child), specified);
        }

        private protected virtual void InitLayoutCore(IArrangedElement element, BoundsSpecified bounds)
        {
        }

        internal virtual void ProcessSuspendedLayoutEventArgs(IArrangedElement container, LayoutEventArgs args)
        {
        }

        public virtual bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            return LayoutCore(CastToArrangedElement(container), layoutEventArgs);
        }

        private protected virtual bool LayoutCore(IArrangedElement container, LayoutEventArgs layoutEventArgs)
        {
            return false;
        }
    }
}
