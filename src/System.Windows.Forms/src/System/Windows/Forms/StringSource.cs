// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents an internal class that is used bu ComboBox and TextBox AutoCompleteCustomSource property.
    ///  This class is reponsible for initializing the SHAutoComplete COM object and setting options in it.
    ///  The StringSource contains an array of Strings which is passed to the COM object as the custom source.
    /// </summary>
    internal class StringSource : IEnumString
    {
        private string[] strings;
        private int current;
        private int size;
        private Shell32.IAutoComplete2 _autoCompleteObject2;

        /// <summary>
        ///  SHAutoComplete COM object CLSID.
        /// </summary>
        private static Guid autoCompleteClsid = new Guid("{00BB2763-6A77-11D0-A535-00C04FD7D062}");

        /// <summary>
        ///  Constructor.
        /// </summary>
        public StringSource(string[] strings)
        {
            Array.Clear(strings, 0, size);

            if (strings != null)
            {
                this.strings = strings;
            }
            current = 0;
            size = (strings is null) ? 0 : strings.Length;

            Guid iid_iunknown = typeof(Shell32.IAutoComplete2).GUID;
            HRESULT hr = Ole32.CoCreateInstance(
                ref autoCompleteClsid,
                IntPtr.Zero,
                Ole32.CLSCTX.INPROC_SERVER,
                ref iid_iunknown,
                out object obj);
            if (!hr.Succeeded())
            {
                throw Marshal.GetExceptionForHR((int)hr);
            }

            _autoCompleteObject2 = (Shell32.IAutoComplete2)obj;
        }

        /// <summary>
        ///  This is the method that binds the custom source with the IAutoComplete interface.The "hWndEdit" is the handle
        ///  to the edit Control and the "options' are the options that need to be set in the AUTOCOMPLETE mode.
        /// </summary>
        public bool Bind(HandleRef edit, Shell32.AUTOCOMPLETEOPTIONS options)
        {
            if (_autoCompleteObject2 is null)
            {
                return false;
            }
            if (!_autoCompleteObject2.SetOptions(options).Succeeded())
            {
                return false;
            }

            HRESULT hr = _autoCompleteObject2.Init(edit.Handle, (IEnumString)this, null, null);
            GC.KeepAlive(edit.Wrapper);
            return hr.Succeeded();
        }

        public void ReleaseAutoComplete()
        {
            if (_autoCompleteObject2 != null)
            {
                Marshal.ReleaseComObject(_autoCompleteObject2);
                _autoCompleteObject2 = null;
            }
        }

        public void RefreshList(string[] newSource)
        {
            Array.Clear(strings, 0, size);

            if (strings != null)
            {
                strings = newSource;
            }
            current = 0;
            size = (strings is null) ? 0 : strings.Length;
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
                return (int)HRESULT.E_INVALIDARG;
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
            return celt == 0 ? (int)HRESULT.S_OK : (int)HRESULT.S_FALSE;
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
                return (int)HRESULT.S_FALSE;
            }
            return (int)HRESULT.S_OK;
        }

        #endregion
    }
}
