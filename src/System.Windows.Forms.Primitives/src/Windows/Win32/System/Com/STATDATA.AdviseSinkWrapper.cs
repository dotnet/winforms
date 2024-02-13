// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using ComType = System.Runtime.InteropServices.ComTypes;

namespace Windows.Win32.System.Com;

internal unsafe partial struct STATDATA
{
    private class AdviseSinkWrapper : ComType.IAdviseSink
    {
        private readonly AgileComPointer<IAdviseSink> _adviseSink;

        public AdviseSinkWrapper(IAdviseSink* adviseSink)
        {
            _adviseSink = new(adviseSink, takeOwnership: false);
        }

        void ComType.IAdviseSink.OnClose()
        {
            using var adviseSink = _adviseSink.GetInterface();
            adviseSink.Value->OnClose();
        }

        void ComType.IAdviseSink.OnDataChange(ref ComType.FORMATETC format, ref ComType.STGMEDIUM stgmedium)
        {
            using var adviseSink = _adviseSink.GetInterface();
            STGMEDIUM comMedium = (STGMEDIUM)stgmedium;
            adviseSink.Value->OnDataChange(Unsafe.As<ComType.FORMATETC, FORMATETC>(ref format), comMedium);
            stgmedium = (ComType.STGMEDIUM)comMedium;

            if (comMedium.pUnkForRelease is not null)
            {
                comMedium.pUnkForRelease->Release();
            }
        }

        void ComType.IAdviseSink.OnRename(ComType.IMoniker moniker)
        {
            using var adviseSink = _adviseSink.GetInterface();
            using var comMoniker = ComHelpers.GetComScope<IMoniker>(moniker);
            adviseSink.Value->OnRename(comMoniker);
        }

        void ComType.IAdviseSink.OnSave()
        {
            using var adviseSink = _adviseSink.GetInterface();
            adviseSink.Value->OnSave();
        }

        void ComType.IAdviseSink.OnViewChange(int aspect, int index)
        {
            using var adviseSink = _adviseSink.GetInterface();
            adviseSink.Value->OnViewChange((uint)aspect, index);
        }
    }
}
