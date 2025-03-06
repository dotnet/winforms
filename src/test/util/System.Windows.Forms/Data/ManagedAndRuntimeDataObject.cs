// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using ComTypes = System.Runtime.InteropServices.ComTypes;

namespace System.Windows.Forms.TestUtilities;

internal class ManagedAndRuntimeDataObject : ManagedDataObject, ComTypes.IDataObject
{
    public int DAdvise(ref ComTypes.FORMATETC pFormatetc, ComTypes.ADVF advf, ComTypes.IAdviseSink adviseSink, out int connection) => throw new NotImplementedException();
    public void DUnadvise(int connection) => throw new NotImplementedException();
    public int EnumDAdvise(out ComTypes.IEnumSTATDATA enumAdvise) => throw new NotImplementedException();
    public ComTypes.IEnumFORMATETC EnumFormatEtc(ComTypes.DATADIR direction) => throw new NotImplementedException();
    public int GetCanonicalFormatEtc(ref ComTypes.FORMATETC formatIn, out ComTypes.FORMATETC formatOut) => throw new NotImplementedException();
    public void GetData(ref ComTypes.FORMATETC format, out ComTypes.STGMEDIUM medium) => throw new NotImplementedException();
    public void GetDataHere(ref ComTypes.FORMATETC format, ref ComTypes.STGMEDIUM medium) => throw new NotImplementedException();
    public int QueryGetData(ref ComTypes.FORMATETC format) => throw new NotImplementedException();
    public void SetData(ref ComTypes.FORMATETC formatIn, ref ComTypes.STGMEDIUM medium, bool release) => throw new NotImplementedException();
}
