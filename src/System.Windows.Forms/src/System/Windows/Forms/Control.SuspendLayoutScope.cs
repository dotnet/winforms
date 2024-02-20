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

            if (_control is not null)
            {
                _layoutSuspendCount = _control.LayoutSuspendCount;
                _control.SuspendLayout();
            }
        }

        public readonly void Dispose()
        {
            if (_control is not null)
            {
                _control.ResumeLayout(_performLayout);
                Debug.Assert(_layoutSuspendCount == _control.LayoutSuspendCount, "Suspend/Resume layout mismatch!");
            }
        }
    }
}
