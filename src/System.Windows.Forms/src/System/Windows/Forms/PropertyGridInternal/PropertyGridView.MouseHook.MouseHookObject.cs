// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        internal partial class MouseHook
        {
            /// <summary>
            ///  Forwards message hook calls.
            /// </summary>
            private class MouseHookObject
            {
                private readonly WeakReference _reference;

                public MouseHookObject(MouseHook parent)
                {
                    _reference = new WeakReference(parent, trackResurrection: false);
                }

                public virtual nint Callback(User32.HC nCode, nint wparam, nint lparam)
                {
                    nint result = 0;
                    try
                    {
                        if (_reference.Target is MouseHook hook)
                        {
                            result = hook.MouseHookProc(nCode, wparam, lparam);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail(ex.Message);
                    }

                    return result;
                }
            }
        }
    }
}
