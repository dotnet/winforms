// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Buffers;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Com = Windows.Win32.System.Com;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        public sealed unsafe class GPStream : Com.IStream.Interface
        {
            private readonly Stream _dataStream;

            // to support seeking ahead of the stream length...
            private long _virtualPosition = -1;

            internal GPStream(Stream stream)
            {
                _dataStream = stream;
            }

            private void ActualizeVirtualPosition()
            {
                if (_virtualPosition == -1)
                    return;

                if (_virtualPosition > _dataStream.Length)
                    _dataStream.SetLength(_virtualPosition);

                _dataStream.Position = _virtualPosition;

                _virtualPosition = -1;
            }

            public Stream GetDataStream() => _dataStream;

            HRESULT Com.IStream.Interface.Clone(Com.IStream** ppstm)
            {
                if (ppstm is null)
                {
                    return HRESULT.E_POINTER;
                }

                // The cloned object should have the same current "position"
                bool result = ComHelpers.TryQueryInterface(
                    new GPStream(_dataStream) { _virtualPosition = _virtualPosition },
                    out Com.IStream* pStream);
                Debug.Assert(result);
                *ppstm = pStream;
                return HRESULT.S_OK;
            }

            HRESULT Com.IStream.Interface.Commit(Com.STGC grfCommitFlags)
            {
                _dataStream.Flush();

                // Extend the length of the file if needed.
                ActualizeVirtualPosition();
                return HRESULT.S_OK;
            }

            HRESULT Com.IStream.Interface.CopyTo(Com.IStream* pstm, ulong cb, ulong* pcbRead, ulong* pcbWritten)
            {
                if (pstm is null)
                {
                    return HRESULT.STG_E_INVALIDPOINTER;
                }

                byte[] buffer = ArrayPool<byte>.Shared.Rent(4096);

                ulong remaining = cb;
                ulong totalWritten = 0;
                ulong totalRead = 0;

                fixed (byte* b = buffer)
                {
                    while (remaining > 0)
                    {
                        uint read = remaining < (ulong)buffer.Length ? (uint)remaining : (uint)buffer.Length;

                        ((Com.IStream.Interface)this).Read(b, read, &read);
                        remaining -= read;
                        totalRead += read;

                        if (read == 0)
                        {
                            break;
                        }

                        uint written;
                        pstm->Write(b, read, &written).ThrowOnFailure();
                        totalWritten += written;
                    }
                }

                ArrayPool<byte>.Shared.Return(buffer);

                if (pcbRead is not null)
                    *pcbRead = totalRead;

                if (pcbWritten is not null)
                    *pcbWritten = totalWritten;

                return HRESULT.S_OK;
            }

            HRESULT Com.ISequentialStream.Interface.Read(void* pv, uint cb, uint* pcbRead)
            {
                if (pv is null)
                {
                    return HRESULT.STG_E_INVALIDPOINTER;
                }

                ActualizeVirtualPosition();

                Span<byte> buffer = new(pv, checked((int)cb));
                int read = _dataStream.Read(buffer);

                if (pcbRead is not null)
                    *pcbRead = (uint)read;

                return HRESULT.S_OK;
            }

            HRESULT Com.IStream.Interface.Read(void* pv, uint cb, uint* pcbRead) =>
                ((Com.ISequentialStream.Interface)this).Read(pv, cb, pcbRead);

            HRESULT Com.IStream.Interface.Seek(long dlibMove, SeekOrigin dwOrigin, ulong* plibNewPosition)
            {
                long position = _virtualPosition == -1 ? _dataStream.Position : _virtualPosition;
                long length = _dataStream.Length;

                switch (dwOrigin)
                {
                    case SeekOrigin.Begin:
                        if (dlibMove <= length)
                        {
                            _dataStream.Position = dlibMove;
                            _virtualPosition = -1;
                        }
                        else
                        {
                            _virtualPosition = dlibMove;
                        }

                        break;
                    case SeekOrigin.End:
                        if (dlibMove <= 0)
                        {
                            _dataStream.Position = length + dlibMove;
                            _virtualPosition = -1;
                        }
                        else
                        {
                            _virtualPosition = length + dlibMove;
                        }

                        break;
                    case SeekOrigin.Current:
                        if (dlibMove + position <= length)
                        {
                            _dataStream.Position = position + dlibMove;
                            _virtualPosition = -1;
                        }
                        else
                        {
                            _virtualPosition = dlibMove + position;
                        }

                        break;
                }

                if (plibNewPosition is null)
                    return HRESULT.S_OK;

                *plibNewPosition = _virtualPosition == -1 ? (ulong)_dataStream.Position : (ulong)_virtualPosition;
                return HRESULT.S_OK;
            }

            HRESULT Com.IStream.Interface.SetSize(ulong libNewSize)
            {
                _dataStream.SetLength(checked((long)libNewSize));
                return HRESULT.S_OK;
            }

            HRESULT Com.IStream.Interface.Stat(Com.STATSTG* pstatstg, Com.STATFLAG grfStatFlag)
            {
                if (pstatstg is null)
                {
                    return HRESULT.STG_E_INVALIDPOINTER;
                }

                *pstatstg = new Com.STATSTG
                {
                    cbSize = (ulong)_dataStream.Length,
                    type = (uint)STGTY.STREAM,

                    // Default read/write access is READ, which == 0
                    grfMode = _dataStream.CanWrite
                        ? _dataStream.CanRead
                            ? Com.STGM.STGM_READWRITE
                            : Com.STGM.STGM_WRITE
                        : Com.STGM.STGM_READ
                };

                if (grfStatFlag == Com.STATFLAG.STATFLAG_DEFAULT)
                {
                    // Caller wants a name
                    pstatstg->pwcsName = (char*)Marshal.StringToCoTaskMemUni(_dataStream is FileStream fs ? fs.Name : _dataStream.ToString());
                }

                return HRESULT.S_OK;
            }

            /// Returns HRESULT.STG_E_INVALIDFUNCTION as a documented way to say we don't support locking
            HRESULT Com.IStream.Interface.LockRegion(ulong libOffset, ulong cb, Com.LOCKTYPE dwLockType) => HRESULT.STG_E_INVALIDFUNCTION;

            // We never report ourselves as Transacted, so we can just ignore this.
            HRESULT Com.IStream.Interface.Revert() => HRESULT.S_OK;

            /// Returns HRESULT.STG_E_INVALIDFUNCTION as a documented way to say we don't support locking
            HRESULT Com.IStream.Interface.UnlockRegion(ulong libOffset, ulong cb, uint dwLockType) => HRESULT.STG_E_INVALIDFUNCTION;

            HRESULT Com.ISequentialStream.Interface.Write(void* pv, uint cb, uint* pcbWritten)
            {
                if (pv is null)
                {
                    return HRESULT.STG_E_INVALIDPOINTER;
                }

                ActualizeVirtualPosition();

                ReadOnlySpan<byte> buffer = new(pv, checked((int)cb));
                _dataStream.Write(buffer);

                if (pcbWritten is not null)
                    *pcbWritten = cb;

                return HRESULT.S_OK;
            }

            HRESULT Com.IStream.Interface.Write(void* pv, uint cb, uint* pcbWritten) =>
                ((Com.ISequentialStream.Interface)this).Write(pv, cb, pcbWritten);
        }
    }
}
