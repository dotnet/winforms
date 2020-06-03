// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        public class AxComponentEditor : WindowsFormsComponentEditor
        {
            public override bool EditComponent(ITypeDescriptorContext context, object obj, IWin32Window parent)
            {
                if (obj is AxHost host)
                {
                    try
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "in AxComponentEditor.EditComponent");
                        ((Ole32.IOleControlSite)host.oleSite).ShowPropertyFrame();
                        return true;
                    }
                    catch (Exception t)
                    {
                        Debug.Fail(t.ToString());
                        throw;
                    }
                }
                return false;
            }
        }
    }
}
