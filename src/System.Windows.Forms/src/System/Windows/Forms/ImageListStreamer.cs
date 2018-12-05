// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Diagnostics;
    using System;
    using System.Drawing;
    using System.ComponentModel;
    using System.IO;
    using System.Windows.Forms;
    using Microsoft.Win32;
    using System.Globalization;
    using System.Security.Permissions;

    /// <include file='doc\ImageListStreamer.uex' path='docs/doc[@for="ImageListStreamer"]/*' />
    /// <devdoc>
    /// </devdoc>
    [Serializable]
    public sealed class ImageListStreamer : ISerializable, IDisposable {
    
        // compressed magic header.  If we see this, the image stream is compressed.
        // (unicode for MSFT).
        //
        private static readonly byte[] HEADER_MAGIC = new byte[] {0x4D, 0x53, 0x46, 0X74};
        private static object internalSyncObject = new object();
        
        private ImageList imageList;
        private ImageList.NativeImageList nativeImageList;

        internal ImageListStreamer(ImageList il) {
            imageList = il;
        }

        /**
         * Constructor used in deserialization
         */
        private ImageListStreamer(SerializationInfo info, StreamingContext context)
        {
            SerializationInfoEnumerator sie = info.GetEnumerator();
            if (sie == null)
            {
                return;
            }
            while (sie.MoveNext())
            {
                if (String.Equals(sie.Name, "Data", StringComparison.OrdinalIgnoreCase))
                {
#if DEBUG
                    try {
#endif
                    byte[] dat = (byte[])sie.Value;
                    if (dat != null)
                    {
                        // We enclose this imagelist handle create in a theming scope.                        
                        IntPtr userCookie = UnsafeNativeMethods.ThemingScope.Activate();

                        try
                        {
                            MemoryStream ms = new MemoryStream(Decompress(dat));

                            lock (internalSyncObject) {
                                SafeNativeMethods.InitCommonControls();
                                nativeImageList = new ImageList.NativeImageList(
                                    SafeNativeMethods.ImageList_Read(new UnsafeNativeMethods.ComStreamFromDataStream(ms)));
                            }
                        }
                        finally
                        {
                            UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
                        }
                        if (nativeImageList.Handle == IntPtr.Zero)
                        {
                            throw new InvalidOperationException(SR.ImageListStreamerLoadFailed);
                        }
                    }
#if DEBUG
                    }
                    catch (Exception e) {
                        Debug.Fail("ImageList serialization failure: " + e.ToString());
                        throw;
                    }
#endif
                }
            }
        }
             
        /// <devdoc>
        ///     Compresses the given input, returning a new array that represents
        ///     the compressed data.
        /// </devdoc>
        private byte[] Compress(byte[] input) {
        
            int finalLength = 0;
            int idx = 0;
            int compressedIdx = 0;
            
            while(idx < input.Length) {
            
                byte current = input[idx++];
                byte runLength = 1;
                
                while(idx < input.Length && input[idx] == current && runLength < 0xFF) {
                    runLength++;
                    idx++;
                }
                
                finalLength += 2;
            }
            
            byte[] output = new byte[finalLength + HEADER_MAGIC.Length];
            
            Buffer.BlockCopy(HEADER_MAGIC, 0, output, 0, HEADER_MAGIC.Length);
            int idxOffset = HEADER_MAGIC.Length;
            idx = 0;
            
            while(idx < input.Length) {
            
                byte current = input[idx++];
                byte runLength = 1;
                
                while(idx < input.Length && input[idx] == current && runLength < 0xFF) {
                    runLength++;
                    idx++;
                }
                
                output[idxOffset + compressedIdx++] = runLength;
                output[idxOffset + compressedIdx++] = current;
            }
            
            Debug.Assert(idxOffset + compressedIdx == output.Length, "RLE Compression failure in ImageListStreamer -- didn't fill array");
            
            // Validate that our compression routine works
            #if DEBUG
            byte[] debugCompare = Decompress(output);
            Debug.Assert(debugCompare.Length == input.Length, "RLE Compression in ImageListStreamer is broken.");
            int debugMaxCompare = input.Length;
            for(int debugIdx = 0; debugIdx < debugMaxCompare; debugIdx++) {
                if (debugCompare[debugIdx] != input[debugIdx]) {
                    Debug.Fail("RLE Compression failure in ImageListStreamer at byte offset " + debugIdx);
                    break;
                }
            }
            #endif // DEBUG
            
            return output;
        }
        
        /// <devdoc>
        ///     Decompresses the given input, returning a new array that represents
        ///     the uncompressed data.
        /// </devdoc>
        private byte[] Decompress(byte[] input) {
            
            int finalLength = 0;
            int idx = 0;
            int outputIdx = 0;
            
            // Check for our header. If we don't have one,
            // we're not actually decompressed, so just return
            // the original.
            //
            if (input.Length < HEADER_MAGIC.Length) {
                return input;
            }
            
            for(idx = 0; idx < HEADER_MAGIC.Length; idx++) {
                if (input[idx] != HEADER_MAGIC[idx]) {  
                    return input;
                }
            }
            
            // Ok, we passed the magic header test.
            
            for (idx = HEADER_MAGIC.Length; idx < input.Length; idx+=2) {
                finalLength += input[idx];
            }
            
            byte[] output = new byte[finalLength];
            
            idx = HEADER_MAGIC.Length;
            
            while(idx < input.Length) {
                byte runLength = input[idx++];
                byte current = input[idx++];
                
                int startIdx = outputIdx;
                int endIdx = outputIdx + runLength;
                
                while(startIdx < endIdx) {
                    output[startIdx++] = current;
                }
                
                outputIdx += runLength;
            }
            
            return output;
        }

        /// <include file='doc\ImageListStreamer.uex' path='docs/doc[@for="ImageListStreamer.GetObjectData"]/*' />
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)] 
        public void /*cpr: ISerializable*/GetObjectData(SerializationInfo si, StreamingContext context) {
            MemoryStream stream = new MemoryStream();

            IntPtr handle = IntPtr.Zero;
            if (imageList != null) {
                handle = imageList.Handle;
            }
            else if (nativeImageList != null) {
                handle = nativeImageList.Handle;
            }

            if (handle == IntPtr.Zero || !WriteImageList(handle, stream))
                throw new InvalidOperationException(SR.ImageListStreamerSaveFailed);

            si.AddValue("Data", Compress(stream.ToArray()));
        }

        /// <include file='doc\ImageListStreamer.uex' path='docs/doc[@for="ImageListStreamer.GetNativeImageList"]/*' />
        /// <internalonly/>
        internal ImageList.NativeImageList GetNativeImageList() {
            return nativeImageList;
        }

        private bool WriteImageList(IntPtr imagelistHandle, Stream stream) {
            // What we need to do here is use WriteEx if comctl 6 or above, and Write otherwise. However, till we can fix 
            // There isn't a reliable way to tell which version of comctl fusion is binding to. 
            // So for now, we try to bind to WriteEx, and if that entry point isn't found, we use Write.

            try {
                int hResult = SafeNativeMethods.ImageList_WriteEx(new HandleRef(this, imagelistHandle), NativeMethods.ILP_DOWNLEVEL, new UnsafeNativeMethods.ComStreamFromDataStream(stream));
                return (hResult == NativeMethods.S_OK);
            }
            catch (EntryPointNotFoundException) {
                // WriteEx wasn't found - that's fine - we will use Write.
            }

            return SafeNativeMethods.ImageList_Write(new HandleRef(this, imagelistHandle), new UnsafeNativeMethods.ComStreamFromDataStream(stream));
        }

        /// <include file='doc\ImageListStreamer.uex' path='docs/doc[@for="ImageListStreamer.GetNativeImageList"]/*' />
        /// <devdoc>
        ///     Disposes the native image list handle.
        /// </devdoc>
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if (disposing) {
                if (nativeImageList != null) {
                    nativeImageList.Dispose();
                    nativeImageList = null;
                }
            }
        }

    }
}
