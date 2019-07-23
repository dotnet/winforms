// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//#define LAYOUT_PERFWATCH

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
            if (child == null)
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

#if LAYOUT_PERFWATCH
        private const int LayoutWatch = 100;
#endif

        public virtual bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }

#if LAYOUT_PERFWATCH
            Debug.WriteLine(container.GetType().Name + "::Layout("
                   + (layoutEventArgs.AffectedControl != null ? layoutEventArgs.AffectedControl.Name : "null")
                   + ", " + layoutEventArgs.AffectedProperty + ")");
            Debug.Indent();
            Stopwatch sw = new Stopwatch();
            sw.Start();
#endif
            bool parentNeedsLayout = LayoutCore(CastToArrangedElement(container), layoutEventArgs);

#if LAYOUT_PERFWATCH
            sw.Stop();
            if (sw.ElapsedMilliseconds > LayoutWatch && Debugger.IsAttached)
            {
                Debugger.Break();
            }
            Debug.Unindent();
            Debug.WriteLine(container.GetType().Name + "::Layout elapsed " + sw.ElapsedMilliseconds.ToString() + " returned: " + parentNeedsLayout);
#endif
            return parentNeedsLayout;
        }

        private protected virtual bool LayoutCore(IArrangedElement container, LayoutEventArgs layoutEventArgs)
        {
            return false;
        }
    }
}
