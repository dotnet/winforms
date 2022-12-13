// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.System.Com
{
    internal unsafe partial struct IDispatch
    {
        /// <summary>
        ///  Get the specified <paramref name="dispId"/> property.
        /// </summary>
        internal HRESULT GetProperty(
            uint dispId,
            VARIANT* pVar,
            uint lcid = 0)
        {
            fixed (IDispatch* dispatch = &this)
            {
                Guid riid = Guid.Empty;
                DISPPARAMS disparams = default;
                return dispatch->Invoke(
                    (int)dispId,
                    &riid,
                    lcid,
                    DISPATCH_FLAGS.DISPATCH_PROPERTYGET,
                    &disparams,
                    pVar,
                    pExcepInfo: null,
                    puArgErr: null);
            }
        }

        /// <summary>
        ///  Get the specified <paramref name="dispId"/> property.
        /// </summary>
        internal VARIANT GetProperty(
            uint dispId,
            uint lcid = 0)
        {
            VARIANT variant = default;
            GetProperty(dispId, &variant, lcid).ThrowOnFailure();
            return variant;
        }
    }
}
