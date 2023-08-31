// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ListView
{
    private class DisposingContext : IDisposable
    {
        private readonly ListView _owner;

        public DisposingContext(ListView owner)
        {
            _owner = owner;
            _owner.ClearingInnerListOnDispose = true;
        }

        public void Dispose()
        {
            _owner.ClearingInnerListOnDispose = false;
        }
    }
}
