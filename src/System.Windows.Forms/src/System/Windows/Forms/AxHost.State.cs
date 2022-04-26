﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using static Interop;

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
        public class State : ISerializable
        {
            private const int VERSION = 1;
            private int _length;
            private byte[]? _buffer;
            private MemoryStream? _memoryStream;
            private Ole32.IStorage? _storage;
            private WinFormsComWrappers.LockBytesWrapper? _iLockBytes;
            private bool _manualUpdate;
            private string? _licenseKey;
            private readonly PropertyBagStream? _propertyBag;
            private const string PropertyBagSerializationName = "PropertyBagBinary";
            private const string DataSerializationName = "Data";

            // create on save from ipersist stream
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
                            Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, "Loading up property bag from stream...");
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

            internal bool _GetManualUpdate()
            {
                return _manualUpdate;
            }

            internal string? _GetLicenseKey()
            {
                return _licenseKey;
            }

            private void CreateStorage()
            {
                Debug.Assert(_storage is null, "but we already have a storage!");
                IntPtr hglobal = IntPtr.Zero;
                if (_buffer is not null)
                {
                    hglobal = Kernel32.GlobalAlloc(Kernel32.GMEM.MOVEABLE, (uint)_length);
                    IntPtr pointer = Kernel32.GlobalLock(hglobal);
                    try
                    {
                        if (pointer != IntPtr.Zero)
                        {
                            Marshal.Copy(_buffer, 0, pointer, _length);
                        }
                    }
                    finally
                    {
                        Kernel32.GlobalUnlock(hglobal);
                    }
                }

                try
                {
                    _iLockBytes = Ole32.CreateILockBytesOnHGlobal(hglobal, BOOL.TRUE);
                    if (_buffer is null)
                    {
                        _storage = Ole32.StgCreateDocfileOnILockBytes(
                            _iLockBytes,
                            Ole32.STGM.CREATE | Ole32.STGM.READWRITE | Ole32.STGM.SHARE_EXCLUSIVE);
                    }
                    else
                    {
                        _storage = Ole32.StgOpenStorageOnILockBytes(
                            _iLockBytes,
                            null,
                            Ole32.STGM.READWRITE | Ole32.STGM.SHARE_EXCLUSIVE,
                            IntPtr.Zero);
                    }
                }
                catch (Exception)
                {
                    if (_iLockBytes is null && hglobal != IntPtr.Zero)
                    {
                        Kernel32.GlobalFree(hglobal);
                    }
                    else
                    {
                        _iLockBytes?.Dispose();
                        _iLockBytes = null;
                    }

                    _storage = null;
                }
            }

            internal Oleaut32.IPropertyBag? GetPropBag()
            {
                return _propertyBag;
            }

            internal Ole32.IStorage? GetStorage()
            {
                if (_storage is null)
                {
                    CreateStorage();
                }

                return _storage;
            }

            internal Ole32.IStream? GetStream()
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

            internal State? RefreshStorage(Ole32.IPersistStorage iPersistStorage)
            {
                Debug.Assert(_storage is not null, "how can we not have a storage object?");
                Debug.Assert(_iLockBytes is not null, "how can we have a storage w/o ILockBytes?");
                if (_storage is null || _iLockBytes is null)
                {
                    return null;
                }

                iPersistStorage.Save(_storage, BOOL.TRUE);
                _storage.Commit(0);
                iPersistStorage.HandsOffStorage();
                try
                {
                    _buffer = null;
                    _memoryStream = null;
                    _iLockBytes.Stat(out Ole32.STATSTG stat, Ole32.STATFLAG.NONAME);
                    _length = (int)stat.cbSize;
                    _buffer = new byte[_length];
                    IntPtr hglobal = Ole32.GetHGlobalFromILockBytes(_iLockBytes);
                    IntPtr pointer = Kernel32.GlobalLock(hglobal);
                    try
                    {
                        if (pointer != IntPtr.Zero)
                        {
                            Marshal.Copy(pointer, _buffer, 0, _length);
                        }
                        else
                        {
                            _length = 0;
                            _buffer = null;
                        }
                    }
                    finally
                    {
                        Kernel32.GlobalUnlock(hglobal);
                    }
                }
                finally
                {
                    iPersistStorage.SaveCompleted(_storage);
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
                        Debug.WriteLineIf(s_axHTraceSwitch.TraceVerbose, $"Failed to serialize the property bag into ResX : {e}");
                    }
                }
            }
        }
    }
}
