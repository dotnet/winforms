// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//#define LAYOUT_PERFWATCH


namespace System.Windows.Forms.Layout {
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Drawing;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Security.Permissions;

    /// <include file='doc\LayoutEngine.uex' path='docs/doc[@for="LayoutEngine"]/*' />
    public abstract class LayoutEngine {        
        internal IArrangedElement CastToArrangedElement(object obj) {
            IArrangedElement element = obj as IArrangedElement;
            if(obj == null) {
                throw new NotSupportedException(string.Format(SR.LayoutEngineUnsupportedType, obj.GetType()));
            }
            return element;
        }

        internal virtual Size GetPreferredSize(IArrangedElement container, Size proposedConstraints) { return Size.Empty; }
    
        /// <include file='doc\LayoutEngine.uex' path='docs/doc[@for="LayoutEngine.InitLayout"]/*' />
        public virtual void InitLayout(object child, BoundsSpecified specified) {
            InitLayoutCore(CastToArrangedElement(child), specified);
        }

        internal virtual void InitLayoutCore(IArrangedElement element, BoundsSpecified bounds) {}

        internal virtual void ProcessSuspendedLayoutEventArgs(IArrangedElement container, LayoutEventArgs args) {}

#if LAYOUT_PERFWATCH
private static int LayoutWatch = 100;
#endif
        /// <include file='doc\LayoutEngine.uex' path='docs/doc[@for="LayoutEngine.Layout"]/*' />        
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")]
        public virtual bool Layout(object container, LayoutEventArgs layoutEventArgs) {

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
            if (sw.ElapsedMilliseconds > LayoutWatch && Debugger.IsAttached) {
                Debugger.Break();
            }
            Debug.Unindent();
            Debug.WriteLine(container.GetType().Name + "::Layout elapsed " + sw.ElapsedMilliseconds.ToString() + " returned: " + parentNeedsLayout);
#endif
            return parentNeedsLayout;
        }

        internal virtual bool LayoutCore(IArrangedElement container, LayoutEventArgs layoutEventArgs) { return false; }
    }
}
