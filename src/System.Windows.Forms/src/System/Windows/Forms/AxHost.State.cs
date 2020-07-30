// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
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
        [TypeConverterAttribute(typeof(TypeConverter))]
        [Serializable] // This exchanges with the native code.
        public class State : ISerializable
        {
            private readonly int VERSION = 1;
            private int length;
            private byte[] buffer;
            internal int type;
            private MemoryStream ms;
            private Ole32.IStorage storage;
            private Ole32.ILockBytes iLockBytes;
            private bool manualUpdate;
            private string licenseKey;
#pragma warning disable IDE1006
            private readonly PropertyBagStream PropertyBagBinary; // Do NOT rename (binary serialization).
#pragma warning restore IDE1006

            // create on save from ipersist stream
            internal State(MemoryStream ms, int storageType, AxHost ctl, PropertyBagStream propBag)
            {
                type = storageType;
                PropertyBagBinary = propBag;
                // dangerous?
                length = (int)ms.Length;
                this.ms = ms;
                manualUpdate = ctl.GetAxState(AxHost.manualUpdate);
                licenseKey = ctl.GetLicenseKey();
            }

            internal State(PropertyBagStream propBag)
            {
                PropertyBagBinary = propBag;
            }

            internal State(MemoryStream ms)
            {
                this.ms = ms;
                length = (int)ms.Length;
                InitializeFromStream(ms);
            }

            // create on init new w/ storage...
            internal State(AxHost ctl)
            {
                CreateStorage();
                manualUpdate = ctl.GetAxState(AxHost.manualUpdate);
                licenseKey = ctl.GetLicenseKey();
                type = STG_STORAGE;
            }

            public State(Stream ms, int storageType, bool manualUpdate, string licKey)
            {
                type = storageType;
                // dangerous?
                length = (int)ms.Length;
                this.manualUpdate = manualUpdate;
                licenseKey = licKey;

                InitializeBufferFromStream(ms);
            }

            /**
             * Constructor used in deserialization
             */
            protected State(SerializationInfo info, StreamingContext context)
            {
                SerializationInfoEnumerator sie = info.GetEnumerator();
                if (sie is null)
                {
                    return;
                }
                for (; sie.MoveNext();)
                {
                    if (string.Compare(sie.Name, "Data", true, CultureInfo.InvariantCulture) == 0)
                    {
                        try
                        {
                            byte[] dat = (byte[])sie.Value;
                            if (dat != null)
                            {
                                using var datMemoryStream = new MemoryStream(dat);
                                InitializeFromStream(datMemoryStream);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Fail("failure: " + e.ToString());
                        }
                    }
                    else if (string.Compare(sie.Name, nameof(PropertyBagBinary), true, CultureInfo.InvariantCulture) == 0)
                    {
                        try
                        {
                            Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Loading up property bag from stream...");
                            byte[] dat = (byte[])sie.Value;
                            if (dat != null)
                            {
                                PropertyBagBinary = new PropertyBagStream();
                                using var datMemoryStream = new MemoryStream(dat);
                                PropertyBagBinary.Read(datMemoryStream);
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Fail("failure: " + e.ToString());
                        }
                    }
                }
            }

            internal int Type
            {
                get
                {
                    return type;
                }
                set
                {
                    type = value;
                }
            }

            internal bool _GetManualUpdate()
            {
                return manualUpdate;
            }

            internal string _GetLicenseKey()
            {
                return licenseKey;
            }

            private void CreateStorage()
            {
                Debug.Assert(storage is null, "but we already have a storage!!!");
                IntPtr hglobal = IntPtr.Zero;
                if (buffer != null)
                {
                    hglobal = Kernel32.GlobalAlloc(Kernel32.GMEM.MOVEABLE, (uint)length);
                    IntPtr pointer = Kernel32.GlobalLock(hglobal);
                    try
                    {
                        if (pointer != IntPtr.Zero)
                        {
                            Marshal.Copy(buffer, 0, pointer, length);
                        }
                    }
                    finally
                    {
                        Kernel32.GlobalUnlock(hglobal);
                    }
                }

                try
                {
                    iLockBytes = Ole32.CreateILockBytesOnHGlobal(hglobal, BOOL.TRUE);
                    if (buffer is null)
                    {
                        storage = Ole32.StgCreateDocfileOnILockBytes(
                            iLockBytes,
                            Ole32.STGM.CREATE | Ole32.STGM.READWRITE | Ole32.STGM.SHARE_EXCLUSIVE,
                            0);
                    }
                    else
                    {
                        storage = Ole32.StgOpenStorageOnILockBytes(
                            iLockBytes,
                            null,
                            Ole32.STGM.READWRITE | Ole32.STGM.SHARE_EXCLUSIVE,
                            IntPtr.Zero,
                            0);
                    }
                }
                catch (Exception)
                {
                    if (iLockBytes is null && hglobal != IntPtr.Zero)
                    {
                        Kernel32.GlobalFree(hglobal);
                    }
                    else
                    {
                        iLockBytes = null;
                    }

                    storage = null;
                }
            }

            internal Oleaut32.IPropertyBag GetPropBag()
            {
                return PropertyBagBinary;
            }

            internal Ole32.IStorage GetStorage()
            {
                if (storage is null)
                {
                    CreateStorage();
                }

                return storage;
            }

            internal Ole32.IStream GetStream()
            {
                if (ms is null)
                {
                    Debug.Assert(buffer != null, "gotta have the buffer already...");
                    if (buffer is null)
                    {
                        return null;
                    }

                    ms = new MemoryStream(buffer);
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                }
                return new Ole32.GPStream(ms);
            }

            private void InitializeFromStream(Stream ids)
            {
                using var br = new BinaryReader(ids);

                type = br.ReadInt32();
                int version = br.ReadInt32();
                manualUpdate = br.ReadBoolean();
                int cc = br.ReadInt32();
                if (cc != 0)
                {
                    licenseKey = new string(br.ReadChars(cc));
                }
                for (int skipUnits = br.ReadInt32(); skipUnits > 0; skipUnits--)
                {
                    int len = br.ReadInt32();
                    ids.Position += len;
                }

                length = br.ReadInt32();
                if (length > 0)
                {
                    buffer = br.ReadBytes(length);
                }
            }

            private void InitializeBufferFromStream(Stream ids)
            {
                using var br = new BinaryReader(ids);

                length = br.ReadInt32();
                if (length > 0)
                {
                    buffer = br.ReadBytes(length);
                }
            }

            internal State RefreshStorage(Ole32.IPersistStorage iPersistStorage)
            {
                Debug.Assert(storage != null, "how can we not have a storage object?");
                Debug.Assert(iLockBytes != null, "how can we have a storage w/o ILockBytes?");
                if (storage is null || iLockBytes is null)
                {
                    return null;
                }

                iPersistStorage.Save(storage, BOOL.TRUE);
                storage.Commit(0);
                iPersistStorage.HandsOffStorage();
                try
                {
                    buffer = null;
                    ms = null;
                    iLockBytes.Stat(out Ole32.STATSTG stat, Ole32.STATFLAG.NONAME);
                    length = (int)stat.cbSize;
                    buffer = new byte[length];
                    IntPtr hglobal = Ole32.GetHGlobalFromILockBytes(iLockBytes);
                    IntPtr pointer = Kernel32.GlobalLock(hglobal);
                    try
                    {
                        if (pointer != IntPtr.Zero)
                        {
                            Marshal.Copy(pointer, buffer, 0, length);
                        }
                        else
                        {
                            length = 0;
                            buffer = null;
                        }
                    }
                    finally
                    {
                        Kernel32.GlobalUnlock(hglobal);
                    }
                }
                finally
                {
                    iPersistStorage.SaveCompleted(storage);
                }
                return this;
            }

            internal void Save(MemoryStream stream)
            {
                using var bw = new BinaryWriter(stream);

                bw.Write(type);
                bw.Write(VERSION);
                bw.Write(manualUpdate);
                if (licenseKey != null)
                {
                    bw.Write(licenseKey.Length);
                    bw.Write(licenseKey.ToCharArray());
                }
                else
                {
                    bw.Write((int)0);
                }
                bw.Write((int)0); // skip units
                bw.Write(length);
                if (buffer != null)
                {
                    bw.Write(buffer);
                }
                else if (ms != null)
                {
                    ms.Position = 0;
                    ms.WriteTo(stream);
                }
                else
                {
                    Debug.Assert(length == 0, "if we have no data, then our length has to be 0");
                }
            }

            /// <summary>
            ///  ISerializable private implementation
            /// </summary>
            void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
            {
                using var stream = new MemoryStream();
                Save(stream);

                si.AddValue("Data", stream.ToArray());

                if (PropertyBagBinary != null)
                {
                    try
                    {
                        using var propertyBagBinaryStream = new MemoryStream();
                        PropertyBagBinary.Write(propertyBagBinaryStream);
                        si.AddValue(nameof(PropertyBagBinary), propertyBagBinaryStream.ToArray());
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLineIf(AxHTraceSwitch.TraceVerbose, "Failed to serialize the property bag into ResX : " + e.ToString());
                    }
                }
            }
        }
    }
}
