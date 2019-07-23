// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices.ComTypes;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents an internal class that is used bu ComboBox and TextBox AutoCompleteCustomSoucr property.
    ///  This class is reponsible for initializing the SHAutoComplete COM object and setting options in it.
    ///  The StringSource contains an array of Strings which is passed to the COM object as the custom source.
    /// </summary>
    internal class StringSource : IEnumString
    {
        private string[] strings;
        private int current;
        private int size;
        private UnsafeNativeMethods.IAutoComplete2 autoCompleteObject2;

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
            size = (strings == null) ? 0 : strings.Length;

            Guid iid_iunknown = typeof(UnsafeNativeMethods.IAutoComplete2).GUID;
            object obj = UnsafeNativeMethods.CoCreateInstance(ref autoCompleteClsid, null, NativeMethods.CLSCTX_INPROC_SERVER, ref iid_iunknown);

            autoCompleteObject2 = (UnsafeNativeMethods.IAutoComplete2)obj;
        }

        /// <summary>
        ///  This is the method that binds the custom source with the IAutoComplete interface.The "hWndEdit" is the handle
        ///  to the edit Control and the "options' are the options that need to be set in the AUTOCOMPLETE mode.
        /// </summary>
        public bool Bind(HandleRef edit, int options)
        {
            bool retVal = false;

            if (autoCompleteObject2 != null)
            {
                try
                {
                    autoCompleteObject2.SetOptions(options);
                    autoCompleteObject2.Init(edit, (IEnumString)this, null, null);
                    retVal = true;
                }
                catch
                {
                    retVal = false;
                }
            }
            return retVal;
        }

        public void ReleaseAutoComplete()
        {
            if (autoCompleteObject2 != null)
            {
                Marshal.ReleaseComObject(autoCompleteObject2);
                autoCompleteObject2 = null;
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
            size = (strings == null) ? 0 : strings.Length;
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
                return NativeMethods.E_INVALIDARG;
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
            return celt == 0 ? NativeMethods.S_OK : NativeMethods.S_FALSE;
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
                return (NativeMethods.S_FALSE);
            }
            return NativeMethods.S_OK;
        }

        #endregion
    }
}

