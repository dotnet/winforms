// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class performs basic operation for marshaling data passed
    ///  in from native in one of the CA*** structs (CADWORD, CAUUID, etc),
    ///  which are structs in which the first word is the number of elements
    ///  and the second is a pointer to an array of such elements.
    /// </summary>
    internal unsafe abstract class BaseCAMarshaler
    {
        private static readonly TraceSwitch s_caMarshalSwitch = new TraceSwitch("CAMarshal", "BaseCAMarshaler: Debug CA* struct marshaling");

        private void* _caArrayAddress;
        private readonly uint _count;
        private object[] _itemArray;

        /// <summary>
        ///  Base ctor
        /// </summary>
        protected BaseCAMarshaler(in Ole32.CA caStruct) : base()
        {
            // first 4 bytes is the count
            _count = caStruct.cElems;
            _caArrayAddress = caStruct.pElems;
            Debug.WriteLineIf(s_caMarshalSwitch.TraceVerbose, "Marshaling " + _count.ToString(CultureInfo.InvariantCulture) + " items of type " + ItemType.Name);
        }

        ~BaseCAMarshaler()
        {
            try
            {
                if (_itemArray is null && _caArrayAddress != null)
                {
                    object[] items = Items;
                }
            }
            catch
            {
            }
        }

        protected abstract Array CreateArray();

        /// <summary>
        ///  Returns the type of item this marshaler will return in the items array.
        /// </summary>
        public abstract Type ItemType { get; }

        /// <summary>
        ///  Returns the count of items that will be or have been marshaled.
        /// </summary>
        public uint Count => _count;

        /// <summary>
        ///  The marshaled items.
        /// </summary>
        public virtual object[] Items
        {
            get
            {
                try
                {
                    _itemArray ??= GetItems();
                }
                catch (Exception ex)
                {
                    Debug.WriteLineIf(s_caMarshalSwitch.TraceVerbose, "Marshaling failed: " + ex.GetType().Name + ", " + ex.Message);
                }
#if DEBUG
                if (_itemArray != null)
                {
                    Debug.WriteLineIf(s_caMarshalSwitch.TraceVerbose, "Marshaled: " + _itemArray.Length.ToString(CultureInfo.InvariantCulture) + " items, array type=" + _itemArray.GetType().Name);
                }
#endif
                return _itemArray;
            }
        }

        /// <summary>
        ///  Override this member to perform marshalling of a single item
        ///  given it's native address.
        /// </summary>
        protected abstract object GetItemFromAddress(IntPtr addr);

        // Retrieve the items
        private object[] GetItems()
        {
            // cycle through the addresses and get an item for each addr
            var items = new object[Count];
            var nativeItems = new ReadOnlySpan<IntPtr>(_caArrayAddress, (int)_count);
            for (int i = 0; i < _count; i++)
            {
                try
                {
                    object curItem = GetItemFromAddress(nativeItems[i]);
                    if (curItem != null && ItemType.IsInstanceOfType(curItem))
                    {
                        items[i] = curItem;
                    }

                    Debug.WriteLineIf(s_caMarshalSwitch.TraceVerbose, "Marshaled " + ItemType.Name + " item, value=" + (curItem is null ? "(null)" : curItem.ToString()));
                }
                catch (Exception ex)
                {
                    Debug.WriteLineIf(s_caMarshalSwitch.TraceVerbose, "Failed to marshal " + ItemType.Name + " item, exception=" + ex.GetType().Name + ", " + ex.Message);
                }
            }

            // free the array
            Marshal.FreeCoTaskMem((IntPtr)_caArrayAddress);
            _caArrayAddress = null;
            return items;
        }
    }
}
