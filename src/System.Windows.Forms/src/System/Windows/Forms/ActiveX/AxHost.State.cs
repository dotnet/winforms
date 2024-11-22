// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using Windows.Win32.System.Com;
using Windows.Win32.System.Com.StructuredStorage;
using static Windows.Win32.System.Memory.GLOBAL_ALLOC_FLAGS;

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    /// <summary>
    ///  The class which encapsulates the persisted state of the underlying activeX control.
    ///  An instance of this class may be obtained either by calling <see cref="OcxState"/> on an
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

        [NonSerialized]
        private AgileComPointer<IStorage>? _storage;
        [NonSerialized]
        private AgileComPointer<ILockBytes>? _lockBytes;

        private readonly PropertyBagStream? _propertyBag;
        private const string PropertyBagSerializationName = "PropertyBagBinary";
        private const string DataSerializationName = "Data";

        // Create on save from IPersistStream.
        internal State(MemoryStream memoryStream, StorageType storageType, AxHost control)
        {
            Type = storageType;
            _length = checked((int)memoryStream.Length);
            _memoryStream = memoryStream;
            ManualUpdate = control.GetAxState(s_manualUpdate);
            LicenseKey = control.GetLicenseKey();
        }

        internal State(PropertyBagStream propertyBag)
        {
            Type = StorageType.PropertyBag;
            _propertyBag = propertyBag;
        }

        // Construct State using StateConverter information.
        // We do not want to save the memoryStream since it contains
        // extra information to construct the State. This same scenario
        // occurs in deserialization constructor.
        internal State(MemoryStream memoryStream) => InitializeFromStream(memoryStream);

        // Create on init new with storage.
        internal State(AxHost control)
        {
            CreateStorage();
            ManualUpdate = control.GetAxState(s_manualUpdate);
            LicenseKey = control.GetLicenseKey();
            Type = StorageType.Storage;
        }

        public State(Stream ms, int storageType, bool manualUpdate, string? licKey)
        {
            // Translate by +1 to match our internal storage values
            Type = (StorageType)(storageType + 1);
            _length = checked((int)ms.Length);
            ManualUpdate = manualUpdate;
            LicenseKey = licKey;

            InitializeFromStream(ms, initializeBufferOnly: true);
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

            while (enumerator.MoveNext())
            {
                if (string.Equals(enumerator.Name, DataSerializationName, StringComparison.InvariantCultureIgnoreCase))
                {
                    try
                    {
                        byte[]? data = enumerator.Value as byte[];
                        if (data is not null)
                        {
                            using MemoryStream memoryStream = new(data);
                            InitializeFromStream(memoryStream);
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
                        byte[]? data = enumerator.Value as byte[];
                        if (data is not null)
                        {
                            using MemoryStream memoryStream = new(data);
                            _propertyBag = new PropertyBagStream(memoryStream);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Fail($"failure: {e}");
                    }
                }
            }
        }

        internal StorageType Type { get; set; }

        internal bool ManualUpdate { get; private set; }

        internal string? LicenseKey { get; private set; }

        private unsafe void CreateStorage()
        {
            Debug.Assert(_storage is null, "but we already have a storage!");
            HGLOBAL hglobal = default;
            if (_buffer is not null)
            {
                hglobal = PInvokeCore.GlobalAlloc(GMEM_MOVEABLE, (uint)_length);
                void* pointer = PInvokeCore.GlobalLock(hglobal);
                try
                {
                    if (pointer is not null)
                    {
                        Marshal.Copy(_buffer, 0, (nint)pointer, _length);
                    }
                }
                finally
                {
                    PInvokeCore.GlobalUnlock(hglobal);
                }
            }

            ILockBytes* lockBytes;
            if (PInvoke.CreateILockBytesOnHGlobal(hglobal, true, &lockBytes).Failed)
            {
                PInvokeCore.GlobalFree(hglobal);
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
                PInvokeCore.GlobalFree(hglobal);
            }

            _lockBytes = new(lockBytes, takeOwnership: true);
            _storage = new(storage, takeOwnership: true);
        }

        internal ComScope<IPropertyBag> GetPropBag()
            => _propertyBag is null ? default : ComHelpers.GetComScope<IPropertyBag>(_propertyBag);

        internal unsafe ComScope<IStorage> GetStorage()
        {
            if (_storage is null)
            {
                CreateStorage();
            }

            return _storage is null ? default : _storage.GetInterface();
        }

        internal ComScope<IStream> GetStream()
        {
            if (_memoryStream is null)
            {
                Debug.Assert(_buffer is not null);
                if (_buffer is null)
                {
                    return default;
                }

                _memoryStream = new MemoryStream(_buffer);
            }
            else
            {
                _memoryStream.Seek(0, SeekOrigin.Begin);
            }

            return _memoryStream.ToIStream();
        }

        private void InitializeFromStream(Stream dataStream, bool initializeBufferOnly = false)
        {
            using BinaryReader binaryReader = new(dataStream);

            if (!initializeBufferOnly)
            {
                // For compatibility, always translate by adding 1 to match our new internal
                // storage values (unknown = 0, stream = 1, etc.).
                Type = (StorageType)(binaryReader.ReadInt32() + 1);

                // Version
                _ = binaryReader.ReadInt32();

                ManualUpdate = binaryReader.ReadBoolean();
                int cc = binaryReader.ReadInt32();
                if (cc != 0)
                {
                    LicenseKey = new string(binaryReader.ReadChars(cc));
                }

                for (int skipUnits = binaryReader.ReadInt32(); skipUnits > 0; skipUnits--)
                {
                    int lengthRead = binaryReader.ReadInt32();
                    dataStream.Position += lengthRead;
                }
            }

            _length = binaryReader.ReadInt32();
            if (_length > 0)
            {
                _buffer = binaryReader.ReadBytes(_length);
            }
        }

        internal unsafe State? RefreshStorage(IPersistStorage* iPersistStorage)
        {
            Debug.Assert(_storage is not null, "how can we not have a storage object?");
            Debug.Assert(_lockBytes is not null, "how can we have a storage w/o ILockBytes?");
            if (_storage is null || _lockBytes is null)
            {
                return null;
            }

            using var storage = _storage.GetInterface();
            iPersistStorage->Save(storage, fSameAsLoad: true).ThrowOnFailure();
            storage.Value->Commit(0);
            iPersistStorage->HandsOffStorage().ThrowOnFailure();
            try
            {
                _buffer = null;
                _memoryStream = null;
                using var lockBytes = _lockBytes.GetInterface();
                lockBytes.Value->Stat(out STATSTG stat, (uint)STATFLAG.STATFLAG_NONAME);
                _length = (int)stat.cbSize;
                _buffer = new byte[_length];
                HGLOBAL hglobal;
                PInvoke.GetHGlobalFromILockBytes(lockBytes, &hglobal).ThrowOnFailure();
                void* pointer = PInvokeCore.GlobalLock(hglobal);

                if (pointer is not null)
                {
                    try
                    {
                        Marshal.Copy((nint)pointer, _buffer, 0, _length);
                    }
                    finally
                    {
                        PInvokeCore.GlobalUnlock(hglobal);
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
                iPersistStorage->SaveCompleted(storage).ThrowOnFailure();
            }

            return this;
        }

        internal void Save(MemoryStream stream)
        {
            using BinaryWriter binaryWriter = new(stream);

            // For compatibility, always translate back to the original storage type values
            // (unknown = -1, stream = 0, etc.) by subtracting 1 when saving.
            binaryWriter.Write(((int)Type) - 1);
            binaryWriter.Write(VERSION);
            binaryWriter.Write(ManualUpdate);
            if (LicenseKey is { } licenseKey)
            {
                binaryWriter.Write(licenseKey.Length);
                binaryWriter.Write(licenseKey.ToCharArray());
            }
            else
            {
                binaryWriter.Write(0);
            }

            binaryWriter.Write(0); // skip units
            binaryWriter.Write(_length);
            if (_buffer is not null)
            {
                binaryWriter.Write(_buffer);
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

        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            using MemoryStream stream = new();
            Save(stream);

            info.AddValue(DataSerializationName, stream.ToArray());

            if (_propertyBag is not null)
            {
                try
                {
                    using MemoryStream propertyBagBinaryStream = new();
                    _propertyBag.Save(propertyBagBinaryStream);
                    info.AddValue(PropertyBagSerializationName, propertyBagBinaryStream.ToArray());
                }
                catch (Exception)
                {
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeHelper.NullAndDispose(ref _lockBytes);
                DisposeHelper.NullAndDispose(ref _storage);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
