// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class performs basic operation for marshaling data passed
    ///  in from native in one of the CA*** structs (CADWORD, CAUUID, etc),
    ///  which are structs in which the first word is the number of elements
    ///  and the second is a pointer to an array of such elements.
    /// </summary>
    internal abstract class BaseCAMarshaler
    {
        private static readonly TraceSwitch CAMarshalSwitch = new TraceSwitch("CAMarshal", "BaseCAMarshaler: Debug CA* struct marshaling");

        private IntPtr caArrayAddress;
        private readonly int count;
        private object[] itemArray;

        /// <summary>
        ///  Base ctor
        /// </summary>
        protected BaseCAMarshaler(NativeMethods.CA_STRUCT caStruct) : base()
        {
            if (caStruct == null)
            {
                count = 0;
                Debug.WriteLineIf(CAMarshalSwitch.TraceVerbose, "BaseCAMarshaler: null passed in!");
            }

            // first 4 bytes is the count
            count = caStruct.cElems;
            caArrayAddress = caStruct.pElems;
            Debug.WriteLineIf(CAMarshalSwitch.TraceVerbose, "Marshaling " + count.ToString(CultureInfo.InvariantCulture) + " items of type " + ItemType.Name);
        }

        ~BaseCAMarshaler()
        {
            try
            {
                if (itemArray == null && caArrayAddress != IntPtr.Zero)
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
        ///  Returns the type of item this marshaler will
        ///  return in the items array.
        /// </summary>
        public abstract Type ItemType
        {
            get;
        }

        /// <summary>
        ///  Returns the count of items that will be or have been
        ///  marshaled.
        /// </summary>
        public int Count
        {
            get
            {
                return count;
            }
        }

        /// <summary>
        ///  The marshaled items.
        /// </summary>
        public virtual object[] Items
        {
            get
            {
                try
                {
                    if (itemArray == null)
                    {
                        itemArray = Get_Items();
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLineIf(CAMarshalSwitch.TraceVerbose, "Marshaling failed: " + ex.GetType().Name + ", " + ex.Message);
                }
#if DEBUG
                if (itemArray != null)
                {
                    Debug.WriteLineIf(CAMarshalSwitch.TraceVerbose, "Marshaled: " + itemArray.Length.ToString(CultureInfo.InvariantCulture) + " items, array type=" + itemArray.GetType().Name);
                }
#endif
                return itemArray;
            }
        }

        /// <summary>
        ///  Override this member to perform marshalling of a single item
        ///  given it's native address.
        /// </summary>
        protected abstract object GetItemFromAddress(IntPtr addr);

        // Retrieve the items
        private object[] Get_Items()
        {
            // cycle through the addresses and get an item for each addr
            IntPtr addr;
            Array items = new object[Count]; //cpb vs38262 System.Array.CreateInstance(this.ItemType,count);
            object curItem;
            for (int i = 0; i < count; i++)
            {
                try
                {
                    addr = Marshal.ReadIntPtr(caArrayAddress, i * IntPtr.Size);
                    curItem = GetItemFromAddress(addr);
                    if (curItem != null && ItemType.IsInstanceOfType(curItem))
                    {
                        items.SetValue(curItem, i);
                    }
                    Debug.WriteLineIf(CAMarshalSwitch.TraceVerbose, "Marshaled " + ItemType.Name + " item, value=" + (curItem == null ? "(null)" : curItem.ToString()));
                }
                catch (Exception ex)
                {
                    Debug.WriteLineIf(CAMarshalSwitch.TraceVerbose, "Failed to marshal " + ItemType.Name + " item, exception=" + ex.GetType().Name + ", " + ex.Message);
                }
            }
            // free the array
            Marshal.FreeCoTaskMem(caArrayAddress);
            caArrayAddress = IntPtr.Zero;
            return (object[])items;
        }
    }
}
