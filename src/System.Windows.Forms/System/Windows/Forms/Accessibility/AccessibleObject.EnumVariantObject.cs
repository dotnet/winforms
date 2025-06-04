// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms;

public partial class AccessibleObject
{
    private unsafe class EnumVariantObject : IEnumVARIANT.Interface, IManagedWrapper<IEnumVARIANT>
    {
        private uint _currentChild;
        private readonly AccessibleObject _owner;

        public EnumVariantObject(AccessibleObject owner)
        {
            Debug.Assert(owner is not null, "Cannot create EnumVariantObject with a null owner");
            _owner = owner;
        }

        public EnumVariantObject(AccessibleObject owner, uint currentChild)
        {
            Debug.Assert(owner is not null, "Cannot create EnumVariantObject with a null owner");
            _owner = owner;
            _currentChild = currentChild;
        }

        HRESULT IEnumVARIANT.Interface.Clone(IEnumVARIANT** ppEnum)
        {
            if (ppEnum is null)
            {
                return HRESULT.E_POINTER;
            }

            ppEnum[0] = ComHelpers.GetComPointer<IEnumVARIANT>(new EnumVariantObject(_owner, _currentChild));
            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Resets the child accessible object enumerator.
        /// </summary>
        HRESULT IEnumVARIANT.Interface.Reset()
        {
            _currentChild = 0;
            using ComScope<IEnumVARIANT> enumVariant = TryGetSystemEnumVARIANT(out HRESULT result);
            if (result.Succeeded)
            {
                enumVariant.Value->Reset();
            }

            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Skips the next <paramref name="celt"/> child accessible objects.
        /// </summary>
        HRESULT IEnumVARIANT.Interface.Skip(uint celt)
        {
            _currentChild += celt;

            using ComScope<IEnumVARIANT> enumVariant = TryGetSystemEnumVARIANT(out HRESULT result);
            if (result.Succeeded)
            {
                enumVariant.Value->Skip(celt);
            }

            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Gets the next n child accessible objects.
        /// </summary>
        HRESULT IEnumVARIANT.Interface.Next(uint celt, VARIANT* rgVar, uint* pCeltFetched)
        {
            // NOTE: rgvar is a pointer to an array of variants
            if (_owner.IsClientObject)
            {
                int childCount;
                int[]? newOrder;

                if ((childCount = _owner.GetChildCount()) >= 0)
                {
                    NextFromChildCollection(celt, rgVar, pCeltFetched, childCount);
                }
                else if (_owner._systemIEnumVariant is null)
                {
                    if (pCeltFetched is not null)
                    {
                        *pCeltFetched = 0;
                    }
                }
                else if ((newOrder = _owner.GetSysChildOrder()) is not null)
                {
                    NextFromSystemReordered(celt, rgVar, pCeltFetched, newOrder);
                }
                else
                {
                    NextFromSystem(celt, rgVar, pCeltFetched);
                }
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
        private unsafe void NextFromSystem(uint celt, VARIANT* rgVar, uint* pCeltFetched)
        {
            uint fetched = 0;

            using ComScope<IEnumVARIANT> enumVariant = TryGetSystemEnumVARIANT(out HRESULT result);
            if (result.Succeeded)
            {
                enumVariant.Value->Next(celt, rgVar, &fetched);
                _currentChild += fetched;
            }

            if (pCeltFetched is not null)
            {
                *pCeltFetched = fetched;
            }
        }

        /// <remarks>
        ///  <para>
        ///   Sometimes we want to rely on the system-provided behavior to create and return child accessible objects,
        ///   but we want to impose a new order on those objects (or even filter some objects out).
        ///  </para>
        ///  <para>
        ///   This method takes an array of ints that dictates the new order. It queries the system for each child
        ///   individually, and inserts the result into the correct *new* position.
        ///  </para>
        ///  <para>
        ///   Note: This code has to make certain *assumptions* about OLEACC.DLL proxy object behavior. However, this
        ///   behavior is well documented. We *assume* the proxy will return a set of child accessible objects that
        ///   correspond 1:1 with the owning control's child windows, and that the default order it returns these
        ///   objects in is z-order (which also happens to be the order that children appear in the
        ///   <see cref="Control.Controls"/> collection).
        ///  </para>
        /// </remarks>
        private unsafe void NextFromSystemReordered(uint celt, VARIANT* rgVar, uint* pCeltFetched, int[] newOrder)
        {
            using ComScope<IEnumVARIANT> enumVariant = TryGetSystemEnumVARIANT(out HRESULT result);
            if (result.Failed)
            {
                return;
            }

            uint i;
            for (i = 0; i < celt && _currentChild < newOrder.Length; ++i)
            {
                uint fetched;

                enumVariant.Value->Reset();
                enumVariant.Value->Skip(i);
                enumVariant.Value->Next(1, rgVar + i, &fetched);

                if (fetched != 1)
                {
                    break;
                }

                _currentChild++;
            }

            if (pCeltFetched is not null)
            {
                *pCeltFetched = i;
            }
        }

        /// <summary>
        ///  If we have our own custom accessible child collection, return a set of 1-based integer child ids, that the
        ///  caller will eventually pass back to us via IAccessible.get_accChild().
        /// </summary>
        private unsafe void NextFromChildCollection(uint celt, VARIANT* rgVar, uint* pCeltFetched, int childCount)
        {
            uint i;
            for (i = 0; i < celt && _currentChild < childCount; ++i)
            {
                ++_currentChild;

                // The type needs to be `int` or controls without UIA support build an incorrect Accessibility tree.
                rgVar[i] = (VARIANT)(int)_currentChild;
            }

            if (pCeltFetched is not null)
            {
                *pCeltFetched = i;
            }
        }

        private ComScope<IEnumVARIANT> TryGetSystemEnumVARIANT(out HRESULT result)
        {
            if (_owner._systemIEnumVariant is { } systemEnum)
            {
                return systemEnum.TryGetInterface(out result);
            }

            result = HRESULT.E_NOINTERFACE;
            return default;
        }
    }
}
