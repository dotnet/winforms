// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class AccessibleObject
    {
        private class EnumVariantObject : Oleaut32.IEnumVariant
        {
            private uint currentChild;
            private readonly AccessibleObject owner;

            public EnumVariantObject(AccessibleObject owner)
            {
                Debug.Assert(owner != null, "Cannot create EnumVariantObject with a null owner");
                this.owner = owner;
            }

            public EnumVariantObject(AccessibleObject owner, uint currentChild)
            {
                Debug.Assert(owner != null, "Cannot create EnumVariantObject with a null owner");
                this.owner = owner;
                this.currentChild = currentChild;
            }

            HRESULT Oleaut32.IEnumVariant.Clone(Oleaut32.IEnumVariant[]? ppEnum)
            {
                if (ppEnum is null)
                {
                    return HRESULT.E_INVALIDARG;
                }

                ppEnum[0] = new EnumVariantObject(owner, currentChild);
                return HRESULT.S_OK;
            }

            /// <summary>
            ///  Resets the child accessible object enumerator.
            /// </summary>
            HRESULT Oleaut32.IEnumVariant.Reset()
            {
                currentChild = 0;
                owner.systemIEnumVariant?.Reset();
                return HRESULT.S_OK;
            }

            /// <summary>
            ///  Skips the next <paramref name="celt"/> child accessible objects.
            /// </summary>
            HRESULT Oleaut32.IEnumVariant.Skip(uint celt)
            {
                currentChild += celt;
                owner.systemIEnumVariant?.Skip(celt);
                return HRESULT.S_OK;
            }

            /// <summary>
            ///  Gets the next n child accessible objects.
            /// </summary>
            unsafe HRESULT Oleaut32.IEnumVariant.Next(uint celt, IntPtr rgVar, uint* pCeltFetched)
            {
                // NOTE: rgvar is a pointer to an array of variants
                if (owner.IsClientObject)
                {
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "EnumVariantObject: owner = " + owner.ToString() + ", celt = " + celt);

                    Debug.Indent();

                    int childCount;
                    int[]? newOrder;

                    if ((childCount = owner.GetChildCount()) >= 0)
                    {
                        NextFromChildCollection(celt, rgVar, pCeltFetched, childCount);
                    }
                    else if (owner.systemIEnumVariant is null)
                    {
                        NextEmpty(celt, rgVar, pCeltFetched);
                    }
                    else if ((newOrder = owner.GetSysChildOrder()) != null)
                    {
                        NextFromSystemReordered(celt, rgVar, pCeltFetched, newOrder);
                    }
                    else
                    {
                        NextFromSystem(celt, rgVar, pCeltFetched);
                    }

                    Debug.Unindent();
                }
                else
                {
                    NextFromSystem(celt, rgVar, pCeltFetched);
                }

                if (pCeltFetched is null)
                {
                    return HRESULT.S_OK;
                }

                // Tell caller whether requested number of items was returned. Once list of items has
                // been exhausted, we return S_FALSE so that caller knows to stop calling this method.
                return *pCeltFetched == celt ? HRESULT.S_OK : HRESULT.S_FALSE;
            }

            /// <summary>
            ///  When we have the IEnumVariant of an accessible proxy provided by the system (ie.
            ///  OLEACC.DLL), we can fall back on that to return the children. Generally, the system
            ///  proxy will enumerate the child windows, create a suitable kind of child accessible
            ///  proxy for each one, and return a set of IDispatch interfaces to these proxy objects.
            /// </summary>
            private unsafe void NextFromSystem(uint celt, IntPtr rgVar, uint* pCeltFetched)
            {
                owner.systemIEnumVariant?.Next(celt, rgVar, pCeltFetched);
                if (pCeltFetched != null)
                {
                    currentChild += *pCeltFetched;
                }

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.IEV.Next: Delegating to systemIEnumVariant");
            }

            /// <summary>
            ///  Sometimes we want to rely on the system-provided behavior to create
            ///  and return child accessible objects, but we want to impose a new
            ///  order on those objects (or even filter some objects out).
            ///
            ///  This method takes an array of ints that dictates the new order.
            ///  It queries the system for each child individually, and inserts the
            ///  result into the correct *new* position.
            ///
            ///  Note: This code has to make certain *assumptions* about OLEACC.DLL
            ///  proxy object behavior. However, this behavior is well documented.
            ///  We *assume* the proxy will return a set of child accessible objects
            ///  that correspond 1:1 with the owning control's child windows, and
            ///  that the default order it returns these objects in is z-order
            ///  (which also happens to be the order that children appear in the
            ///  Control.Controls[] collection).
            /// </summary>
            private unsafe void NextFromSystemReordered(uint celt, IntPtr rgVar, uint* pCeltFetched, int[] newOrder)
            {
                if (owner.systemIEnumVariant is null)
                {
                    return;
                }

                uint i;
                for (i = 0; i < celt && currentChild < newOrder.Length; ++i)
                {
                    if (!GotoItem(owner.systemIEnumVariant, newOrder[currentChild], GetAddressOfVariantAtIndex(rgVar, i)))
                    {
                        break;
                    }

                    currentChild++;
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.IEV.Next: adding sys child " + currentChild + " of " + newOrder.Length);
                }

                if (pCeltFetched != null)
                {
                    *pCeltFetched = i;
                }
            }

            /// <summary>
            ///  If we have our own custom accessible child collection, return a set
            ///  of 1-based integer child ids, that the caller will eventually pass
            ///  back to us via IAccessible.get_accChild().
            /// </summary>
            private unsafe void NextFromChildCollection(uint celt, IntPtr rgVar, uint* pCeltFetched, int childCount)
            {
                uint i;
                for (i = 0; i < celt && currentChild < childCount; ++i)
                {
                    ++currentChild;
                    Marshal.GetNativeVariantForObject(((object)currentChild), GetAddressOfVariantAtIndex(rgVar, i));
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.IEV.Next: adding own child " + currentChild + " of " + childCount);
                }

                if (pCeltFetched != null)
                {
                    *pCeltFetched = i;
                }
            }

            /// <summary>
            ///  Default behavior if there is no custom child collection or
            ///  system-provided proxy to fall back on. In this case, we return
            ///  an empty child collection.
            /// </summary>
            private unsafe void NextEmpty(uint celt, IntPtr rgvar, uint* pCeltFetched)
            {
                if (pCeltFetched != null)
                {
                    *pCeltFetched = 0;
                }

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.IEV.Next: no children to add");
            }

            /// <summary>
            ///  Given an IEnumVariant interface, this method jumps to a specific
            ///  item in the collection and extracts the result for that one item.
            /// </summary>
            private unsafe static bool GotoItem(Oleaut32.IEnumVariant iev, int index, IntPtr variantPtr)
            {
                uint celtFetched = 0;

                iev.Reset();
                iev.Skip((uint)index);
                iev.Next(1, variantPtr, &celtFetched);

                return celtFetched == 1;
            }

            /// <summary>
            ///  Given an array of pointers to variants, calculate address of a given array element.
            /// </summary>
            private static IntPtr GetAddressOfVariantAtIndex(IntPtr variantArrayPtr, uint index)
            {
                int variantSize = 8 + (IntPtr.Size * 2);
                return (IntPtr)((ulong)variantArrayPtr + ((ulong)index) * ((ulong)variantSize));
            }
        }
    }
}
