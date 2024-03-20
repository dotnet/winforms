// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public unsafe partial class Control
{
    internal readonly ref struct SuspendLayoutScope
    {
        private readonly Control? _control;
        private readonly bool _performLayout;

#if DEBUG
        private readonly int _layoutSuspendCount;
#endif

        public SuspendLayoutScope(Control? control, bool performLayout = true)
        {
            _control = control;
            _performLayout = performLayout;

#if DEBUG
            _layoutSuspendCount = _control?.LayoutSuspendCount ?? 0;
#endif
            _control?.SuspendLayout();
        }

        public readonly void Dispose()
        {
            _control?.ResumeLayout(_performLayout);
#if DEBUG
            Debug.Assert(_control is null || _layoutSuspendCount == _control.LayoutSuspendCount, "Suspend/Resume layout mismatch!");
#endif
        }
    }
}
