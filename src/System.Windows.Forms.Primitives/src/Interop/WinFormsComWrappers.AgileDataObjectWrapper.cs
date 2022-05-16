// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices.ComTypes;

internal partial class Interop
{
    internal unsafe partial class WinFormsComWrappers
    {
        internal class AgileDataObjectWrapper : IDataObject
        {
            private AgileReferenceWrapper _wrappedInstance;

            public AgileDataObjectWrapper(AgileReferenceWrapper wrappedInstance)
            {
                _wrappedInstance = wrappedInstance;
            }

            private DataObjectWrapper Unwrap()
            {
                var instance = _wrappedInstance.Resolve(IID.IDataObject);
                return new DataObjectWrapper(instance);
            }

            public void GetData(ref FORMATETC format, out STGMEDIUM medium)
            {
                using var dataObject = Unwrap();
                dataObject.GetData(ref format, out medium);
            }

            public void GetDataHere(ref FORMATETC format, ref STGMEDIUM medium)
            {
                using var dataObject = Unwrap();
                dataObject.GetDataHere(ref format, ref medium);
            }

            public int QueryGetData(ref FORMATETC format)
            {
                using var dataObject = Unwrap();
                return dataObject.QueryGetData(ref format);
            }

            public int GetCanonicalFormatEtc(ref FORMATETC formatIn, out FORMATETC formatOut)
            {
                using var dataObject = Unwrap();
                return dataObject.GetCanonicalFormatEtc(ref formatIn, out formatOut);
            }

            public void SetData(ref FORMATETC formatIn, ref STGMEDIUM medium, bool release)
            {
                using var dataObject = Unwrap();
                dataObject.SetData(ref formatIn, ref medium, release);
            }

            public IEnumFORMATETC EnumFormatEtc(DATADIR direction)
            {
                using var dataObject = Unwrap();
                return dataObject.EnumFormatEtc(direction);
            }

            public int DAdvise(ref FORMATETC pFormatetc, ADVF advf, IAdviseSink adviseSink, out int connection)
            {
                using var dataObject = Unwrap();
                return dataObject.DAdvise(ref pFormatetc, advf, adviseSink, out connection);
            }

            public void DUnadvise(int connection)
            {
                using var dataObject = Unwrap();
                dataObject.DUnadvise(connection);
            }

            public int EnumDAdvise(out IEnumSTATDATA? enumAdvise)
            {
                using var dataObject = Unwrap();
                return dataObject.EnumDAdvise(out enumAdvise);
            }
        }
    }
}
