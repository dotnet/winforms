// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents an internal class that is used by ComboBox and TextBox AutoCompleteCustomSource property.
    ///  This class is responsible for initializing the SHAutoComplete COM object and setting options in it.
    ///  The StringSource contains an array of Strings which is passed to the COM object as the custom source.
    /// </summary>
    internal class StringSource : IEnumString
    {
        private string[] strings;
        private int current;
        private int size;
        private WinFormsComWrappers.AutoCompleteWrapper? _autoCompleteObject2;

        /// <summary>
        ///  Constructor.
        /// </summary>
        public StringSource(string[] strings)
        {
            Array.Clear(strings, 0, size);
            this.strings = strings;

            current = 0;
            size = strings.Length;

            var autoCompleteIID = IID.IAutoComplete2;
            Ole32.CoCreateInstance(
                in CLSID.AutoComplete,
                IntPtr.Zero,
                Ole32.CLSCTX.INPROC_SERVER,
                in autoCompleteIID,
                out IntPtr autoComplete2Ptr).ThrowOnFailure();

            var obj = WinFormsComWrappers.Instance
                .GetOrCreateObjectForComInstance(autoComplete2Ptr, CreateObjectFlags.UniqueInstance);
            _autoCompleteObject2 = (WinFormsComWrappers.AutoCompleteWrapper)obj;
        }

        /// <summary>
        ///  This is the method that binds the custom source with the IAutoComplete interface.The "hWndEdit" is the handle
        ///  to the edit Control and the "options' are the options that need to be set in the AUTOCOMPLETE mode.
        /// </summary>
        public bool Bind(HandleRef edit, AUTOCOMPLETEOPTIONS options)
        {
            if (_autoCompleteObject2 is null)
            {
                return false;
            }

            if (!_autoCompleteObject2.SetOptions(options).Succeeded)
            {
                return false;
            }

            HRESULT hr = _autoCompleteObject2.Init(edit.Handle, this, IntPtr.Zero, IntPtr.Zero);
            GC.KeepAlive(edit.Wrapper);
            return hr.Succeeded;
        }

        public void ReleaseAutoComplete()
        {
            if (_autoCompleteObject2 is not null)
            {
                _autoCompleteObject2.Dispose();
                _autoCompleteObject2 = null;
            }
        }

        public void RefreshList(string[] newSource)
        {
            Array.Clear(strings, 0, size);
            strings = newSource;
            current = 0;
            size = strings.Length;
        }

        #region IEnumString Members

        void IEnumString.Clone(out IEnumString ppenum)
        {
            ppenum = new StringSource(strings);
        }

        int IEnumString.Next(int celt, string[] rgelt, IntPtr pceltFetched)
        {
            if (celt < 0)
            {
                return (int)HRESULT.Values.E_INVALIDARG;
            }

            int fetched = 0;

            while (current < size && celt > 0)
            {
                rgelt[fetched] = strings[current];
                current++;
                fetched++;
                celt--;
            }

            if (pceltFetched != IntPtr.Zero)
            {
                Marshal.WriteInt32(pceltFetched, fetched);
            }

            return celt == 0 ? (int)HRESULT.Values.S_OK : (int)HRESULT.Values.S_FALSE;
        }

        void IEnumString.Reset()
        {
            current = 0;
        }

        int IEnumString.Skip(int celt)
        {
            current += celt;
            if (current >= size)
            {
                return (int)HRESULT.Values.S_FALSE;
            }

            return (int)HRESULT.Values.S_OK;
        }

        #endregion
    }
}
