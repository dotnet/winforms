// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;
using static Windows.Win32.System.Com.ADVANCED_FEATURE_FLAGS;
using static Windows.Win32.System.Variant.VARENUM;

namespace System.Windows.Forms.Tests.Interop.SafeArrayTests;

public unsafe class SAFEARRAYTests
{
    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
    public void SAFEARRAY_Sizeof_InvokeX86_ReturnsExpected()
    {
        if (Environment.Is64BitProcess)
        {
            return;
        }

        Assert.Equal(24, Marshal.SizeOf<SAFEARRAY>());
        Assert.Equal(24, sizeof(SAFEARRAY));
    }

    [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
    public void SAFEARRAY_Sizeof_InvokeX64_ReturnsExpected()
    {
        if (!Environment.Is64BitProcess)
        {
            return;
        }

        Assert.Equal(32, Marshal.SizeOf<SAFEARRAY>());
        Assert.Equal(32, sizeof(SAFEARRAY));
    }

    public static IEnumerable<object[]> Create_TestData()
    {
        yield return new object[] { VT_I4, FADF_HAVEVARTYPE, 4 };
        yield return new object[] { VT_I8, FADF_HAVEVARTYPE, 8 };
        yield return new object[] { VT_BSTR, FADF_HAVEVARTYPE | FADF_BSTR, IntPtr.Size };
        yield return new object[] { VT_UNKNOWN, FADF_HAVEIID | FADF_UNKNOWN, IntPtr.Size };
        yield return new object[] { VT_DISPATCH, FADF_HAVEIID | FADF_DISPATCH, IntPtr.Size };
    }

    [StaTheory]
    [MemberData(nameof(Create_TestData))]
    public void SAFEARRAY_CreateSingleDimension_GetProperties_Success(ushort vt, ushort expectedFeatures, uint expectedCbElements)
    {
        SAFEARRAYBOUND saBound = new()
        {
            cElements = 10,
            lLbound = 1
        };

        SAFEARRAY* psa = PInvokeCore.SafeArrayCreate((VARENUM)vt, 1, &saBound);
        NativeAssert.NotNull(psa);

        try
        {
            Assert.Equal(1u, psa->cDims);
            Assert.Equal((ADVANCED_FEATURE_FLAGS)expectedFeatures, psa->fFeatures);
            Assert.Equal(expectedCbElements, psa->cbElements);
            Assert.Equal(0u, psa->cLocks);
            NativeAssert.NotNull(psa->pvData);
            Assert.Equal(10u, psa->rgsabound.AsSpan(1)[0].cElements);
            Assert.Equal(1, psa->rgsabound.AsSpan(1)[0].lLbound);

            VARENUM arrayVt = VT_EMPTY;
            HRESULT hr = PInvokeCore.SafeArrayGetVartype(psa, &arrayVt);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal((VARENUM)vt, arrayVt);
        }
        finally
        {
            HRESULT hr = PInvokeCore.SafeArrayDestroy(psa);
            Assert.Equal(HRESULT.S_OK, hr);
        }
    }

    [StaFact]
    public void SAFEARRAY_CreateSingleDimensionRECORD_GetProperties_Success()
    {
        SAFEARRAYBOUND saBound = new()
        {
            cElements = 10,
            lLbound = 1
        };

        using ComScope<IRecordInfo> recordInfo = new(new CustomRecordInfo().GetComInterface());

        SAFEARRAY* psa = PInvokeCore.SafeArrayCreateEx(VT_RECORD, 1, &saBound, recordInfo);
        NativeAssert.NotNull(psa);

        try
        {
            Assert.Equal(1u, psa->cDims);
            Assert.Equal(FADF_RECORD, psa->fFeatures);
            Assert.Equal((uint)sizeof(int), psa->cbElements);
            Assert.Equal(0u, psa->cLocks);
            NativeAssert.NotNull(psa->pvData);
            Assert.Equal(10u, psa->rgsabound.AsSpan(1)[0].cElements);
            Assert.Equal(1, psa->rgsabound.AsSpan(1)[0].lLbound);

            VARENUM arrayVt = VT_EMPTY;
            HRESULT hr = PInvokeCore.SafeArrayGetVartype(psa, &arrayVt);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal(VT_RECORD, arrayVt);
        }
        finally
        {
            HRESULT hr = PInvokeCore.SafeArrayDestroy(psa);
            Assert.Equal(HRESULT.S_OK, hr);
        }
    }

    private class CustomRecordInfo : IRecordInfo.Interface
    {
        public IRecordInfo* GetComInterface() => (IRecordInfo*)Marshal.GetComInterfaceForObject<CustomRecordInfo, IRecordInfo.Interface>(this);

        public HRESULT RecordInit(void* pvNew) => throw new NotImplementedException();

        public HRESULT RecordClear(void* pvExisting) => throw new NotImplementedException();

        public HRESULT RecordCopy(void* pvExisting, void* pvNew) => throw new NotImplementedException();

        public Func<(Guid, HRESULT)> GetGuidAction { get; set; }

        public HRESULT GetGuid(Guid* pguid)
        {
            (Guid guid, HRESULT hr) = GetGuidAction();
            *pguid = guid;
            return hr;
        }

        public HRESULT GetName(BSTR* pbstrName) => throw new NotImplementedException();

        public HRESULT GetSize(uint* pcbSize)
        {
            *pcbSize = sizeof(int);
            return HRESULT.S_OK;
        }

        public HRESULT GetTypeInfo(ITypeInfo** ppTypeInfo) => throw new NotImplementedException();

        public HRESULT GetField(void* pvData, PCWSTR szFieldName, VARIANT* pvarField) => throw new NotImplementedException();

        public HRESULT GetFieldNoCopy(void* pvData, PCWSTR szFieldName, VARIANT* pvarField, void** ppvDataCArray) => throw new NotImplementedException();

        public HRESULT PutField(uint wFlags, void* pvData, PCWSTR szFieldName, VARIANT* pvarField) => throw new NotImplementedException();

        public HRESULT PutFieldNoCopy(uint wFlags, void* pvData, PCWSTR szFieldName, VARIANT* pvarField) => throw new NotImplementedException();

        public HRESULT GetFieldNames(uint* pcNames, BSTR* rgBstrNames) => throw new NotImplementedException();

        public BOOL IsMatchingType(IRecordInfo* pRecordInfoInfo) => throw new NotImplementedException();

        public void* RecordCreate() => throw new NotImplementedException();

        public HRESULT RecordCreateCopy(void* pvSource, void** ppvDest) => throw new NotImplementedException();

        public HRESULT RecordDestroy(void* pvRecord) => throw new NotImplementedException();
    }

    [StaTheory]
    [MemberData(nameof(Create_TestData))]
    public void SAFEARRAY_CreateMultipleDimensions_GetProperties_Success(ushort vt, ushort expectedFeatures, uint expectedCbElements)
    {
        SAFEARRAYBOUND* saBounds = stackalloc SAFEARRAYBOUND[2];
        saBounds[0] = new SAFEARRAYBOUND
        {
            cElements = 10,
            lLbound = 1
        };
        saBounds[1] = new SAFEARRAYBOUND
        {
            cElements = 20,
            lLbound = 0
        };

        SAFEARRAY* psa = PInvokeCore.SafeArrayCreate((VARENUM)vt, 2, saBounds);
        NativeAssert.NotNull(psa);

        try
        {
            Assert.Equal(2u, psa->cDims);
            Assert.Equal((ADVANCED_FEATURE_FLAGS)expectedFeatures, psa->fFeatures);
            Assert.Equal(expectedCbElements, psa->cbElements);
            Assert.Equal(0u, psa->cLocks);
            NativeAssert.NotNull(psa->pvData);
            Assert.Equal(20u, psa->rgsabound.AsSpan(1)[0].cElements);
            Assert.Equal(0, psa->rgsabound.AsSpan(1)[0].lLbound);
            Assert.Equal(10u, ((SAFEARRAYBOUND*)&psa->rgsabound)[1].cElements);
            Assert.Equal(1, ((SAFEARRAYBOUND*)&psa->rgsabound)[1].lLbound);

            VARENUM arrayVt = VT_EMPTY;
            HRESULT hr = PInvokeCore.SafeArrayGetVartype(psa, &arrayVt);
            Assert.Equal(HRESULT.S_OK, hr);
            Assert.Equal((VARENUM)vt, arrayVt);
        }
        finally
        {
            HRESULT hr = PInvokeCore.SafeArrayDestroy(psa);
            Assert.Equal(HRESULT.S_OK, hr);
        }
    }

    [StaFact]
    public void SAFEARRAY_GetValue_InvokeSingleDimensional_ReturnsExpected()
    {
        SAFEARRAYBOUND saBound = new()
        {
            cElements = 10,
            lLbound = 0
        };

        SAFEARRAY* psa = PInvokeCore.SafeArrayCreate(VT_I4, 1, &saBound);
        NativeAssert.NotNull(psa);

        try
        {
            Span<int> indices1 = [0];
            Span<int> indices2 = [1];

            fixed (int* pIndices1 = indices1)
            fixed (int* pIndices2 = indices2)
            {
                int value1 = 1;
                HRESULT hr = PInvokeCore.SafeArrayPutElement(psa, pIndices1, &value1);
                Assert.Equal(HRESULT.S_OK, hr);

                int value2 = 2;
                hr = PInvokeCore.SafeArrayPutElement(psa, pIndices2, &value2);
                Assert.Equal(HRESULT.S_OK, hr);

                int result = -1;
                hr = PInvokeCore.SafeArrayGetElement(psa, pIndices1, &result);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(1, result);

                hr = PInvokeCore.SafeArrayGetElement(psa, pIndices2, &result);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(2, result);
            }

            Assert.Equal(1, psa->GetValue<int>(indices1));
            Assert.Equal(2, psa->GetValue<int>(indices2));
        }
        finally
        {
            PInvokeCore.SafeArrayDestroy(psa);
        }
    }

    [StaFact]
    public void SAFEARRAY_GetValue_InvokeSingleDimensionalNonZeroLowerBound_ReturnsExpected()
    {
        SAFEARRAYBOUND saBound = new()
        {
            cElements = 10,
            lLbound = -5
        };

        SAFEARRAY* psa = PInvokeCore.SafeArrayCreate(VT_I4, 1, &saBound);
        NativeAssert.NotNull(psa);

        try
        {
            Span<int> indices1 = [-5];
            Span<int> indices2 = [-4];

            fixed (int* pIndices1 = indices1)
            fixed (int* pIndices2 = indices2)
            {
                int value1 = 1;
                HRESULT hr = PInvokeCore.SafeArrayPutElement(psa, pIndices1, &value1);
                Assert.Equal(HRESULT.S_OK, hr);

                int value2 = 2;
                hr = PInvokeCore.SafeArrayPutElement(psa, pIndices2, &value2);
                Assert.Equal(HRESULT.S_OK, hr);

                int result = -1;
                hr = PInvokeCore.SafeArrayGetElement(psa, pIndices1, &result);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(1, result);

                hr = PInvokeCore.SafeArrayGetElement(psa, pIndices2, &result);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(2, result);
            }

            Assert.Equal(1, psa->GetValue<int>(indices1));
            Assert.Equal(2, psa->GetValue<int>(indices2));
        }
        finally
        {
            PInvokeCore.SafeArrayDestroy(psa);
        }
    }

    [StaFact]
    public void SAFEARRAY_GetValue_InvokeMultiDimensional_ReturnsExpected()
    {
        SAFEARRAYBOUND* saBounds = stackalloc SAFEARRAYBOUND[2];
        saBounds[0] = new SAFEARRAYBOUND
        {
            cElements = 10,
            lLbound = 0
        };

        saBounds[1] = new SAFEARRAYBOUND
        {
            cElements = 20,
            lLbound = 0
        };

        SAFEARRAY* psa = PInvokeCore.SafeArrayCreate(VT_I4, 2, saBounds);
        NativeAssert.NotNull(psa);

        try
        {
            Span<int> indices1 = [0, 0];
            Span<int> indices2 = [1, 2];

            fixed (int* pIndices1 = indices1)
            fixed (int* pIndices2 = indices2)
            {
                int value1 = 1;
                HRESULT hr = PInvokeCore.SafeArrayPutElement(psa, pIndices1, &value1);
                Assert.Equal(HRESULT.S_OK, hr);

                int value2 = 2;
                hr = PInvokeCore.SafeArrayPutElement(psa, pIndices2, &value2);
                Assert.Equal(HRESULT.S_OK, hr);

                int result = -1;
                hr = PInvokeCore.SafeArrayGetElement(psa, pIndices1, &result);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(1, result);

                hr = PInvokeCore.SafeArrayGetElement(psa, pIndices2, &result);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(2, result);
            }

            Assert.Equal(1, psa->GetValue<int>(indices1));
            Assert.Equal(2, psa->GetValue<int>(indices2));
        }
        finally
        {
            PInvokeCore.SafeArrayDestroy(psa);
        }
    }

    [StaFact]
    public void SAFEARRAY_GetValue_InvokeMultiDimensionalNonZeroLowerBound_ReturnsExpected()
    {
        SAFEARRAYBOUND* saBounds = stackalloc SAFEARRAYBOUND[2];
        saBounds[0] = new SAFEARRAYBOUND
        {
            cElements = 10,
            lLbound = -5
        };

        saBounds[1] = new SAFEARRAYBOUND
        {
            cElements = 20,
            lLbound = -4
        };

        SAFEARRAY* psa = PInvokeCore.SafeArrayCreate(VT_I4, 2, saBounds);
        NativeAssert.NotNull(psa);

        try
        {
            Span<int> indices1 = [-5, -4];
            Span<int> indices2 = [-4, -3];

            fixed (int* pIndices1 = indices1)
            fixed (int* pIndices2 = indices2)
            {
                int value1 = 1;
                HRESULT hr = PInvokeCore.SafeArrayPutElement(psa, pIndices1, &value1);
                Assert.Equal(HRESULT.S_OK, hr);

                int value2 = 2;
                hr = PInvokeCore.SafeArrayPutElement(psa, pIndices2, &value2);
                Assert.Equal(HRESULT.S_OK, hr);

                int result = -1;
                hr = PInvokeCore.SafeArrayGetElement(psa, pIndices1, &result);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(1, result);

                hr = PInvokeCore.SafeArrayGetElement(psa, pIndices2, &result);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal(2, result);
            }

            Assert.Equal(1, psa->GetValue<int>(indices1));
            Assert.Equal(2, psa->GetValue<int>(indices2));
        }
        finally
        {
            PInvokeCore.SafeArrayDestroy(psa);
        }
    }
}
