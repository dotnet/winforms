﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms.Layout
{
    // Frequently when you need to do a PreformLayout, you also need to invalidate the
    // PreferredSizeCache (you are laying out because you know that the action has changed
    // the PreferredSize of the control and/or its container).  LayoutTransaction wraps both
    // of these operations into one, plus adds a check for null to make our code more
    // concise.
    //
    // Usage1: (When we are not calling to other code which may cause a layout:)
    //
    //  LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);
    //
    // Usage2: (When we need to wrap code which may cause additional layouts:)
    //
    //  using(new LayoutTransaction(ParentInternal, this, PropertyNames.Bounds)) {
    //      OnBoundsChanged();
    //  }
    //
    // The second usage spins off 12b for garbage collection, but I did some profiling and
    // it didn't seem significant (we were spinning off more from LayoutEventArgs.)
    internal sealed class LayoutTransaction : IDisposable
    {
        readonly Control? _controlToLayout;
        readonly bool _resumeLayout;

#if DEBUG
        readonly int _layoutSuspendCount;
#endif
        public LayoutTransaction(Control? controlToLayout, IArrangedElement controlCausingLayout, string? property)
            : this(controlToLayout, controlCausingLayout, property, true)
        {
        }

        public LayoutTransaction(Control? controlToLayout, IArrangedElement controlCausingLayout, string? property, bool resumeLayout)
        {
            CommonProperties.xClearPreferredSizeCache(controlCausingLayout);
            _controlToLayout = controlToLayout;

            _resumeLayout = resumeLayout;
            if (_controlToLayout is not null)
            {
#if DEBUG
                _layoutSuspendCount = _controlToLayout.LayoutSuspendCount;
#endif
                _controlToLayout.SuspendLayout();
                CommonProperties.xClearPreferredSizeCache(_controlToLayout);

                // Same effect as calling performLayout on Dispose but then we would have to keep
                // controlCausingLayout and property around as state.
                if (resumeLayout)
                {
                    _controlToLayout.PerformLayout(new LayoutEventArgs(controlCausingLayout, property));
                }
            }
        }

        public void Dispose()
        {
#pragma warning disable IDE0031
            if (_controlToLayout is not null)
            {
                _controlToLayout.ResumeLayout(_resumeLayout);

#if DEBUG
                Debug.Assert(_controlToLayout.LayoutSuspendCount == _layoutSuspendCount, "Suspend/Resume layout mismatch!");
#endif
            }
#pragma warning restore IDE0031
        }

        // This overload should be used when a property has changed that affects preferred size,
        // but you only want to layout if a certain condition exists (say you want to layout your
        // parent because your preferred size has changed).
        public static IDisposable CreateTransactionIf(bool condition, Control? controlToLayout, IArrangedElement elementCausingLayout, string? property)
        {
            if (condition)
            {
                return new LayoutTransaction(controlToLayout, elementCausingLayout, property);
            }
            else
            {
                CommonProperties.xClearPreferredSizeCache(elementCausingLayout);
                return default(NullLayoutTransaction);
            }
        }

        public static void DoLayout(IArrangedElement? elementToLayout, IArrangedElement? elementCausingLayout, string? property)
        {
            if (elementCausingLayout is not null)
            {
                CommonProperties.xClearPreferredSizeCache(elementCausingLayout);
                if (elementToLayout is not null)
                {
                    CommonProperties.xClearPreferredSizeCache(elementToLayout);
                    elementToLayout.PerformLayout(elementCausingLayout, property);
                }
            }

            Debug.Assert(elementCausingLayout is not null, "LayoutTransaction.DoLayout - elementCausingLayout is null, no layout performed - did you mix up your parameters?");
        }

        // This overload should be used when a property has changed that affects preferred size,
        // but you only want to layout if a certain condition exists (say you want to layout your
        // parent because your preferred size has changed).
        public static void DoLayoutIf(bool condition, IArrangedElement? elementToLayout, IArrangedElement? elementCausingLayout, string? property)
        {
            if (!condition)
            {
                if (elementCausingLayout is not null)
                {
                    CommonProperties.xClearPreferredSizeCache(elementCausingLayout);
                }
            }
            else
            {
                LayoutTransaction.DoLayout(elementToLayout, elementCausingLayout, property);
            }
        }
    }
}
