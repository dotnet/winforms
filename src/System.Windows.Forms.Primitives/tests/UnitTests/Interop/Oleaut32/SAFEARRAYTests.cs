// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;
using static Interop;
using static Interop.Ole32;
using static Interop.Oleaut32;

namespace System.Windows.Forms.Tests.Interop.Oleaut32
{
    public unsafe class SAFEARRAYTests
    {
        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public void SAFEARRAY_Sizeof_InvokeX86_ReturnsExpected()
        {
            Assert.Equal(24, Marshal.SizeOf<SAFEARRAY>());
            Assert.Equal(24, sizeof(SAFEARRAY));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public void SAFEARRAY_Sizeof_InvokeX64_ReturnsExpected()
        {
            Assert.Equal(32, Marshal.SizeOf<SAFEARRAY>());
            Assert.Equal(32, sizeof(SAFEARRAY));
        }

        public static IEnumerable<object[]> Create_TestData()
        {
            yield return new object[] { VARENUM.I4, FADF.HAVEVARTYPE, 4 };
            yield return new object[] { VARENUM.I8, FADF.HAVEVARTYPE, 8 };
            yield return new object[] { VARENUM.BSTR, FADF.HAVEVARTYPE | FADF.BSTR, IntPtr.Size };
            yield return new object[] { VARENUM.UNKNOWN, FADF.HAVEIID | FADF.UNKNOWN, IntPtr.Size };
            yield return new object[] { VARENUM.DISPATCH, FADF.HAVEIID | FADF.DISPATCH, IntPtr.Size };
        }

        [StaTheory]
        [MemberData(nameof(Create_TestData))]
        public void SAFEARRAY_CreateSingleDimension_GetProperties_Success(ushort vt, ushort expectedFeatures, uint expectedCbElements)
        {
            var saBound = new SAFEARRAYBOUND
            {
                cElements = 10,
                lLbound = 1
            };
            SAFEARRAY *psa = SafeArrayCreate((VARENUM)vt, 1, &saBound);
            Assert.True(psa != null);

            try
            {
                Assert.Equal(1u, psa->cDims);
                Assert.Equal((FADF)expectedFeatures, psa->fFeatures);
                Assert.Equal((uint)expectedCbElements, psa->cbElements);
                Assert.Equal(0u, psa->cLocks);
                Assert.True(psa->pvData != null);
                Assert.Equal(10u, psa->rgsabound[0].cElements);
                Assert.Equal(1, psa->rgsabound[0].lLbound);

                VARENUM arrayVt = VARENUM.EMPTY;
                HRESULT hr = SafeArrayGetVartype(psa, &arrayVt);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal((VARENUM)vt, arrayVt);
            }
            finally
            {
                HRESULT hr = SafeArrayDestroy(psa);
                Assert.Equal(HRESULT.S_OK, hr);
            }
        }

        [StaFact]
        public void SAFEARRAY_CreateSingleDimensionRECORD_GetProperties_Success()
        {
            var saBound = new SAFEARRAYBOUND
            {
                cElements = 10,
                lLbound = 1
            };
            var record = new CustomRecordInfo();
            IntPtr pRecord = Marshal.GetComInterfaceForObject<CustomRecordInfo, IRecordInfo>(record);
            try
            {
                SAFEARRAY *psa = SafeArrayCreateEx(VARENUM.RECORD, 1, &saBound, pRecord);
                Assert.True(psa != null);

                try
                {
                    Assert.Equal(1u, psa->cDims);
                    Assert.Equal(FADF.RECORD, psa->fFeatures);
                    Assert.Equal((uint)sizeof(int), psa->cbElements);
                    Assert.Equal(0u, psa->cLocks);
                    Assert.True(psa->pvData != null);
                    Assert.Equal(10u, psa->rgsabound[0].cElements);
                    Assert.Equal(1, psa->rgsabound[0].lLbound);

                    VARENUM arrayVt = VARENUM.EMPTY;
                    HRESULT hr = SafeArrayGetVartype(psa, &arrayVt);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(VARENUM.RECORD, arrayVt);
                }
                finally
                {
                    HRESULT hr = SafeArrayDestroy(psa);
                    Assert.Equal(HRESULT.S_OK, hr);
                }
            }
            finally
            {
                Marshal.Release(pRecord);
            }
        }

        private class CustomRecordInfo : IRecordInfo
        {
            HRESULT IRecordInfo.RecordInit(void* pvNew) => throw new NotImplementedException();

            HRESULT IRecordInfo.RecordClear(void* pvExisting) => throw new NotImplementedException();

            HRESULT IRecordInfo.RecordCopy(void* pvExisting, void* pvNew) => throw new NotImplementedException();

            public Func<(Guid, HRESULT)> GetGuidAction { get; set; }

            HRESULT IRecordInfo.GetGuid(Guid* pguid)
            {
                (Guid guid, HRESULT hr) = GetGuidAction();
                *pguid = guid;
                return hr;
            }

            HRESULT IRecordInfo.GetName(BSTR* pbstrName) => throw new NotImplementedException();

            HRESULT IRecordInfo.GetSize(uint* pcbSize)
            {
                *pcbSize = (uint)sizeof(int);
                return HRESULT.S_OK;
            }

            HRESULT IRecordInfo.GetTypeInfo(out ITypeInfo ppTypeInfo) => throw new NotImplementedException();

            HRESULT IRecordInfo.GetField(void* pvData, out string szFieldName, VARIANT* pvarField) => throw new NotImplementedException();

            HRESULT IRecordInfo.GetFieldNoCopy(void* pvData, out string szFieldName, VARIANT* pvarField, void* ppvDataCArray) => throw new NotImplementedException();

            HRESULT IRecordInfo.PutField(INVOKEKIND wFlags, void* pvData, out string szFieldName, VARIANT* pvarField) => throw new NotImplementedException();

            HRESULT IRecordInfo.PutFieldNoCopy(INVOKEKIND wFlags, void* pvData, out string szFieldName, VARIANT* pvarField) => throw new NotImplementedException();

            HRESULT IRecordInfo.GetFieldNames(uint* pcNames, BSTR* rgBstrNames) => throw new NotImplementedException();

            BOOL IRecordInfo.IsMatchingType(ref IRecordInfo pRecordInfo) => throw new NotImplementedException();

            void* IRecordInfo.RecordCreate() => throw new NotImplementedException();

            HRESULT IRecordInfo.RecordCreateCopy(void* pvSource, void** ppvDest) => throw new NotImplementedException();

            HRESULT IRecordInfo.RecordDestroy(void* pvRecord) => throw new NotImplementedException();
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
            SAFEARRAY *psa = SafeArrayCreate((VARENUM)vt, 2, saBounds);
            Assert.True(psa != null);

            try
            {
                Assert.Equal(2u, psa->cDims);
                Assert.Equal((FADF)expectedFeatures, psa->fFeatures);
                Assert.Equal(expectedCbElements, psa->cbElements);
                Assert.Equal(0u, psa->cLocks);
                Assert.True(psa->pvData != null);
                Assert.Equal(20u, psa->rgsabound[0].cElements);
                Assert.Equal(0, psa->rgsabound[0].lLbound);
                Assert.Equal(10u, psa->rgsabound[1].cElements);
                Assert.Equal(1, psa->rgsabound[1].lLbound);

                VARENUM arrayVt = VARENUM.EMPTY;
                HRESULT hr = SafeArrayGetVartype(psa, &arrayVt);
                Assert.Equal(HRESULT.S_OK, hr);
                Assert.Equal((VARENUM)vt, arrayVt);
            }
            finally
            {
                HRESULT hr = SafeArrayDestroy(psa);
                Assert.Equal(HRESULT.S_OK, hr);
            }
        }

        [StaFact]
        public void SAFEARRAY_GetValue_InvokeSingleDimensional_ReturnsExpected()
        {
            var saBound = new SAFEARRAYBOUND
            {
                cElements = 10,
                lLbound = 0
            };
            SAFEARRAY *psa = SafeArrayCreate(VARENUM.I4, 1, &saBound);
            Assert.True(psa != null);

            try
            {
                Span<int> indices1 = stackalloc int[] { 0 };
                Span<int> indices2 = stackalloc int[] { 1 };

                fixed (int* pIndices1 = indices1)
                fixed (int* pIndices2 = indices2)
                {
                    int value1 = 1;
                    HRESULT hr = SafeArrayPutElement(psa, pIndices1, &value1);
                    Assert.Equal(HRESULT.S_OK, hr);

                    int value2 = 2;
                    hr = SafeArrayPutElement(psa, pIndices2, &value2);
                    Assert.Equal(HRESULT.S_OK, hr);

                    int result = -1;
                    hr = SafeArrayGetElement(psa, pIndices1, &result);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(1, result);

                    hr = SafeArrayGetElement(psa, pIndices2, &result);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(2, result);
                }

                Assert.Equal(1, psa->GetValue<int>(indices1));
                Assert.Equal(2, psa->GetValue<int>(indices2));
            }
            finally
            {
                SafeArrayDestroy(psa);
            }
        }

        [StaFact]
        public void SAFEARRAY_GetValue_InvokeSingleDimensionalNonZeroLowerBound_ReturnsExpected()
        {
            var saBound = new SAFEARRAYBOUND
            {
                cElements = 10,
                lLbound = -5
            };
            SAFEARRAY *psa = SafeArrayCreate(VARENUM.I4, 1, &saBound);
            Assert.True(psa != null);

            try
            {
                Span<int> indices1 = stackalloc int[] { -5 };
                Span<int> indices2 = stackalloc int[] { -4 };

                fixed (int* pIndices1 = indices1)
                fixed (int* pIndices2 = indices2)
                {
                    int value1 = 1;
                    HRESULT hr = SafeArrayPutElement(psa, pIndices1, &value1);
                    Assert.Equal(HRESULT.S_OK, hr);

                    int value2 = 2;
                    hr = SafeArrayPutElement(psa, pIndices2, &value2);
                    Assert.Equal(HRESULT.S_OK, hr);

                    int result = -1;
                    hr = SafeArrayGetElement(psa, pIndices1, &result);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(1, result);

                    hr = SafeArrayGetElement(psa, pIndices2, &result);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(2, result);
                }

                Assert.Equal(1, psa->GetValue<int>(indices1));
                Assert.Equal(2, psa->GetValue<int>(indices2));
            }
            finally
            {
                SafeArrayDestroy(psa);
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
            SAFEARRAY *psa = SafeArrayCreate(VARENUM.I4, 2, saBounds);
            Assert.True(psa != null);

            try
            {
                Span<int> indices1 = stackalloc int[] { 0, 0 };
                Span<int> indices2 = stackalloc int[] { 1, 2 };

                fixed (int* pIndices1 = indices1)
                fixed (int* pIndices2 = indices2)
                {
                    int value1 = 1;
                    HRESULT hr = SafeArrayPutElement(psa, pIndices1, &value1);
                    Assert.Equal(HRESULT.S_OK, hr);

                    int value2 = 2;
                    hr = SafeArrayPutElement(psa, pIndices2, &value2);
                    Assert.Equal(HRESULT.S_OK, hr);

                    int result = -1;
                    hr = SafeArrayGetElement(psa, pIndices1, &result);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(1, result);

                    hr = SafeArrayGetElement(psa, pIndices2, &result);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(2, result);
                }

                Assert.Equal(1, psa->GetValue<int>(indices1));
                Assert.Equal(2, psa->GetValue<int>(indices2));
            }
            finally
            {
                SafeArrayDestroy(psa);
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
            SAFEARRAY *psa = SafeArrayCreate(VARENUM.I4, 2, saBounds);
            Assert.True(psa != null);

            try
            {
                Span<int> indices1 = stackalloc int[] { -5, -4 };
                Span<int> indices2 = stackalloc int[] { -4, -3 };

                fixed (int* pIndices1 = indices1)
                fixed (int* pIndices2 = indices2)
                {
                    int value1 = 1;
                    HRESULT hr = SafeArrayPutElement(psa, pIndices1, &value1);
                    Assert.Equal(HRESULT.S_OK, hr);

                    int value2 = 2;
                    hr = SafeArrayPutElement(psa, pIndices2, &value2);
                    Assert.Equal(HRESULT.S_OK, hr);

                    int result = -1;
                    hr = SafeArrayGetElement(psa, pIndices1, &result);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(1, result);

                    hr = SafeArrayGetElement(psa, pIndices2, &result);
                    Assert.Equal(HRESULT.S_OK, hr);
                    Assert.Equal(2, result);
                }

                Assert.Equal(1, psa->GetValue<int>(indices1));
                Assert.Equal(2, psa->GetValue<int>(indices2));
            }
            finally
            {
                SafeArrayDestroy(psa);
            }
        }

        [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
        private static unsafe extern SAFEARRAY* SafeArrayCreate(VARENUM vt, uint cDims, SAFEARRAYBOUND* rgsabound);

        [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
        private static unsafe extern SAFEARRAY* SafeArrayCreateEx(VARENUM vt, uint cDims, SAFEARRAYBOUND* rgsabound, IntPtr pvExtra);

        [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
        private static unsafe extern HRESULT SafeArrayDestroy(SAFEARRAY* psa);

        [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
        private unsafe static extern HRESULT SafeArrayPutElement(SAFEARRAY* psa, int* rgIndices, void* pv);

        [DllImport(Libraries.Oleaut32, ExactSpelling = true)]
        private unsafe static extern HRESULT SafeArrayGetElement(SAFEARRAY* psa, int* rgIndices, void* pv);
    }
}
