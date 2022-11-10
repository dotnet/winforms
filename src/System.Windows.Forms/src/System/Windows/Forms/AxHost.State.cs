﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;
using static Interop;
using static Windows.Win32.System.Memory.GLOBAL_ALLOC_FLAGS;

namespace System.Windows.Forms
{
    public abstract partial class AxHost
    {
        /// <summary>
        ///  The class which encapsulates the persisted state of the underlying activeX control
        ///  An instance of this class my be obtained either by calling getOcxState on an
        ///  AxHost object, or by reading in from a stream.
        /// </summary>
        [TypeConverter(typeof(TypeConverter))]
        [Serializable] // This exchanges with the native code.
        public class State : ISerializable, IDisposable
        {
            private const int VERSION = 1;
            private int _length;
            private byte[]? _buffer;
            private MemoryStream? _memoryStream;
            private AgileComPointer<IStorage>? _storage;
            private AgileComPointer<ILockBytes>? _lockBytes;
            private bool _manualUpdate;
            private string? _licenseKey;
            private readonly PropertyBagStream? _propertyBag;
            private const string PropertyBagSerializationName = "PropertyBagBinary";
            private const string DataSerializationName = "Data";

            // Create on save from ipersist stream
            internal State(MemoryStream memoryStream, int storageType, AxHost control, PropertyBagStream propertyBag)
            {
                Type = storageType;
                _propertyBag = propertyBag;
                // dangerous?
                _length = (int)memoryStream.Length;
                _memoryStream = memoryStream;
                _manualUpdate = control.GetAxState(s_manualUpdate);
                _licenseKey = control.GetLicenseKey();
            }

            internal State(PropertyBagStream propertyBag)
            {
                _propertyBag = propertyBag;
            }

            internal State(MemoryStream memoryStream)
            {
                _memoryStream = memoryStream;
                _length = (int)memoryStream.Length;
                InitializeFromStream(memoryStream);
            }

            // create on init new w/ storage...
            internal State(AxHost control)
            {
                CreateStorage();
                _manualUpdate = control.GetAxState(s_manualUpdate);
                _licenseKey = control.GetLicenseKey();
                Type = STG_STORAGE;
            }

            public State(Stream ms, int storageType, bool manualUpdate, string? licKey)
            {
                Type = storageType;
                // dangerous?
                _length = (int)ms.Length;
                _manualUpdate = manualUpdate;
                _licenseKey = licKey;

                InitializeBufferFromStream(ms);
            }

            /// <summary>
            ///  Constructor used in deserialization.
            /// </summary>
            protected State(SerializationInfo info, StreamingContext context)
            {
                SerializationInfoEnumerator enumerator = info.GetEnumerator();
                if (enumerator is null)
                {
                    return;
                }

                for (; enumerator.MoveNext();)
                {
                    if (string.Equals(enumerator.Name, DataSerializationName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            byte[]? data = enumerator.Value as byte[];
                            if (data is not null)
                            {
                                using var datMemoryStream = new MemoryStream(data);
                                InitializeFromStream(datMemoryStream);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Fail($"failure: {e}");
                        }
                    }
                    else if (string.Equals(enumerator.Name, PropertyBagSerializationName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        try
                        {
                            s_axHTraceSwitch.TraceVerbose("Loading up property bag from stream...");
                            byte[]? data = enumerator.Value as byte[];
                            if (data is not null)
                            {
                                _propertyBag = new PropertyBagStream();
                                using var datMemoryStream = new MemoryStream(data);
                                _propertyBag.Read(datMemoryStream);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Fail($"failure: {e}");
                        }
                    }
                }
            }

            internal int Type { get; set; }

            internal bool _GetManualUpdate() => _manualUpdate;

            internal string? _GetLicenseKey() => _licenseKey;

            private unsafe void CreateStorage()
            {
                Debug.Assert(_storage is null, "but we already have a storage!");
                nint hglobal = 0;
                if (_buffer is not null)
                {
                    hglobal = PInvoke.GlobalAlloc(GMEM_MOVEABLE, (uint)_length);
                    void* pointer = PInvoke.GlobalLock(hglobal);
                    try
                    {
                        if (pointer is not null)
                        {
                            Marshal.Copy(_buffer, 0, (nint)pointer, _length);
                        }
                    }
                    finally
                    {
                        PInvoke.GlobalUnlock(hglobal);
                    }
                }

                ILockBytes* lockBytes;
                if (PInvoke.CreateILockBytesOnHGlobal(hglobal, true, &lockBytes).Failed)
                {
                    PInvoke.GlobalFree(hglobal);
                    return;
                }

                IStorage* storage;

                HRESULT hr = _buffer is null
                    ? PInvoke.StgCreateDocfileOnILockBytes(
                        lockBytes,
                        STGM.STGM_CREATE | STGM.STGM_READWRITE | STGM.STGM_SHARE_EXCLUSIVE,
                        reserved: 0,
                        &storage)
                    : PInvoke.StgOpenStorageOnILockBytes(
                        lockBytes,
                        pstgPriority: null,
                        STGM.STGM_READWRITE | STGM.STGM_SHARE_EXCLUSIVE,
                        snbExclude: null,
                        reserved: 0,
                        &storage);

                if (hr.Failed)
                {
                    lockBytes->Release();
                    PInvoke.GlobalFree(hglobal);
                }

                _lockBytes = new(lockBytes);
                _storage = new(storage);
            }

            internal IPropertyBag.Interface? GetPropBag() => _propertyBag;

            internal unsafe ComScope<IStorage> GetStorage()
            {
                if (_storage is null)
                {
                    CreateStorage();
                }

                return _storage is null ? default : _storage.GetInterface();
            }

            internal IStream.Interface? GetStream()
            {
                if (_memoryStream is null)
                {
                    Debug.Assert(_buffer is not null, "gotta have the buffer already...");
                    if (_buffer is null)
                    {
                        return null;
                    }

                    _memoryStream = new MemoryStream(_buffer);
                }
                else
                {
                    _memoryStream.Seek(0, SeekOrigin.Begin);
                }

                return new Ole32.GPStream(_memoryStream);
            }

            private void InitializeFromStream(Stream ids)
            {
                using var br = new BinaryReader(ids);

                Type = br.ReadInt32();
                int version = br.ReadInt32();
                _manualUpdate = br.ReadBoolean();
                int cc = br.ReadInt32();
                if (cc != 0)
                {
                    _licenseKey = new string(br.ReadChars(cc));
                }

                for (int skipUnits = br.ReadInt32(); skipUnits > 0; skipUnits--)
                {
                    int len = br.ReadInt32();
                    ids.Position += len;
                }

                _length = br.ReadInt32();
                if (_length > 0)
                {
                    _buffer = br.ReadBytes(_length);
                }
            }

            private void InitializeBufferFromStream(Stream ids)
            {
                using var br = new BinaryReader(ids);

                _length = br.ReadInt32();
                if (_length > 0)
                {
                    _buffer = br.ReadBytes(_length);
                }
            }

            internal unsafe State? RefreshStorage(IPersistStorage.Interface iPersistStorage)
            {
                Debug.Assert(_storage is not null, "how can we not have a storage object?");
                Debug.Assert(_lockBytes is not null, "how can we have a storage w/o ILockBytes?");
                if (_storage is null || _lockBytes is null)
                {
                    return null;
                }

                using var storage = _storage.GetInterface();
                iPersistStorage.Save(storage, fSameAsLoad: true).ThrowOnFailure();
                storage.Value->Commit(0).ThrowOnFailure();
                iPersistStorage.HandsOffStorage().ThrowOnFailure();
                try
                {
                    _buffer = null;
                    _memoryStream = null;
                    using var lockBytes = _lockBytes.GetInterface();
                    lockBytes.Value->Stat(out STATSTG stat, STATFLAG.STATFLAG_NONAME).ThrowOnFailure();
                    _length = (int)stat.cbSize;
                    _buffer = new byte[_length];
                    PInvoke.GetHGlobalFromILockBytes(lockBytes, out nint hglobal).ThrowOnFailure();
                    void* pointer = PInvoke.GlobalLock(hglobal);
 
                    if (pointer is not null)
                    {
                        try
                        {
                            Marshal.Copy((nint)pointer, _buffer, 0, _length);
                        }
                        finally
                        {
                            PInvoke.GlobalUnlock(hglobal);
                        }
                    }
                    else
                    {
                        _length = 0;
                        _buffer = null;
                    }
                }
                finally
                {
                    iPersistStorage.SaveCompleted(storage).ThrowOnFailure();
                }

                return this;
            }

            internal void Save(MemoryStream stream)
            {
                using var bw = new BinaryWriter(stream);

                bw.Write(Type);
                bw.Write(VERSION);
                bw.Write(_manualUpdate);
                if (_licenseKey is not null)
                {
                    bw.Write(_licenseKey.Length);
                    bw.Write(_licenseKey.ToCharArray());
                }
                else
                {
                    bw.Write(0);
                }

                bw.Write(0); // skip units
                bw.Write(_length);
                if (_buffer is not null)
                {
                    bw.Write(_buffer);
                }
                else if (_memoryStream is not null)
                {
                    _memoryStream.Position = 0;
                    _memoryStream.WriteTo(stream);
                }
                else
                {
                    Debug.Assert(_length == 0, "if we have no data, then our length has to be 0");
                }
            }

            /// <summary>
            ///  ISerializable private implementation
            /// </summary>
            void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
            {
                using var stream = new MemoryStream();
                Save(stream);

                si.AddValue(DataSerializationName, stream.ToArray());

                if (_propertyBag is not null)
                {
                    try
                    {
                        using var propertyBagBinaryStream = new MemoryStream();
                        _propertyBag.Write(propertyBagBinaryStream);
                        si.AddValue(PropertyBagSerializationName, propertyBagBinaryStream.ToArray());
                    }
                    catch (Exception e)
                    {
                        s_axHTraceSwitch.TraceVerbose($"Failed to serialize the property bag into ResX : {e}");
                    }
                }
            }

            protected virtual void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _lockBytes?.Dispose();
                    _storage?.Dispose();
                    _lockBytes = null;
                    _storage = null;
                }
            }

            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
