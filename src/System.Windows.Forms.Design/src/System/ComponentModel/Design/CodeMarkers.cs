// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using static Interop;

namespace System.ComponentModel.Design
{
    internal sealed class CodeMarkers
    {
        // Singleton access
        public static readonly CodeMarkers Instance = new CodeMarkers();

        static class NativeMethods
        {
            [DllImport(TestDllName, EntryPoint = "PerfCodeMarker")]
            public static extern void TestDllPerfCodeMarker(int nTimerID, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] aUserParams, int cbParams);

            [DllImport(TestDllName, EntryPoint = "PerfCodeMarker")]
            public static extern void TestDllPerfCodeMarkerString(int nTimerID, [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 2)] string aUserParams, int cbParams);

            [DllImport(ProductDllName, EntryPoint = "PerfCodeMarker")]
            public static extern void ProductDllPerfCodeMarker(int nTimerID, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] aUserParams, int cbParams);

            [DllImport(ProductDllName, EntryPoint = "PerfCodeMarker")]
            public static extern void ProductDllPerfCodeMarkerString(int nTimerID, [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 2)] string aUserParams, int cbParams);

            ///  global native method imports
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern ushort FindAtom([MarshalAs(UnmanagedType.LPWStr)] string lpString);
        }

        // Atom name. This ATOM will be set by the host application when code markers are enabled in the registry.
        const string AtomName = "VSCodeMarkersEnabled";

        // Internal Test CodeMarkers DLL name
        const string TestDllName = "Microsoft.Internal.Performance.CodeMarkers.dll";

        // External Product CodeMarkers DLL name
        const string ProductDllName = "Microsoft.VisualStudio.CodeMarkers.dll";

        enum State
        {
            /// <summary>
            ///  The atom is present. CodeMarkers are enabled.
            /// </summary>
            Enabled,

            /// <summary>
            ///  The atom is not present, but InitPerformanceDll has not yet been called.
            /// </summary>
            Disabled,

            /// <summary>
            ///  Disabled because the CodeMarkers transport DLL could not be found or an import failed to resolve.
            /// </summary>
            DisabledDueToDllImportException
        }

        private State _state;

        /// <summary>
        ///  Are CodeMarkers enabled? Note that even if IsEnabled returns false, CodeMarkers may still be enabled later in another component.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return _state == State.Enabled;
            }
        }

        // should CodeMarker events be fired to the test or product CodeMarker DLL
        private readonly RegistryView _registryView = RegistryView.Default;
#pragma warning disable CS0649
        private readonly string _regroot;
#pragma warning restore CS0649

        private bool? _shouldUseTestDll;

        // This guid should match vscommon\testtools\PerfWatson2\Responsiveness\Listener\Microsoft.Performance.ResponseTime\ContextProviders\ScenarioContextProvider\ScenarioContextProvider.cs
        // And also match toolsrc\Telescope\Batch\PerfWatsonService.Reducer\SessionProcessors\ScenarioProcessor.cs
        private static readonly byte[] s_correlationMarkBytes = new Guid("AA10EEA0-F6AD-4E21-8865-C427DAE8EDB9").ToByteArray();

        public bool ShouldUseTestDll
        {
            get
            {
                if (!_shouldUseTestDll.HasValue)
                {
                    try
                    {
                        // this code can either be used in an InitPerf (loads CodeMarker DLL) or AttachPerf context (CodeMarker DLL already loaded)
                        // in the InitPerf context we have a regroot and should check for the test DLL registration
                        // in the AttachPerf context we should see which module is already loaded
                        if (_regroot is null)
                        {
                            _shouldUseTestDll = Kernel32.GetModuleHandleW(ProductDllName) == IntPtr.Zero;
                        }
                        else
                        {
                            // if CodeMarkers are explictly enabled in the registry then try to use the test DLL, otherwise fall back to trying to use the product DLL
                            _shouldUseTestDll = UsePrivateCodeMarkers(_regroot, _registryView);
                        }
                    }
                    catch (Exception)
                    {
                        _shouldUseTestDll = true;
                    }
                }

                return _shouldUseTestDll.Value;
            }
        }

        // Constructor. Do not call directly. Use CodeMarkers.Instance to access the singleton checks to see if code markers are enabled by looking for a named ATOM
        private CodeMarkers()
        {
            // This ATOM will be set by the native Code Markers host
            _state = (NativeMethods.FindAtom(AtomName) != 0) ? State.Enabled : State.Disabled;
        }

        /// <summary>
        ///  Sends a code marker event
        /// </summary>
        /// <param name="nTimerID">The code marker event ID</param>
        /// <returns>true if the code marker was successfully sent, false if code markers are not enabled or an error occurred.</returns>
        public bool CodeMarker(int nTimerID)
        {
            if (!IsEnabled)
            {
                return false;
            }

            try
            {
                if (ShouldUseTestDll)
                {
                    NativeMethods.TestDllPerfCodeMarker(nTimerID, null, 0);
                }
                else
                {
                    NativeMethods.ProductDllPerfCodeMarker(nTimerID, null, 0);
                }
            }
            catch (DllNotFoundException)
            {
                // If the DLL doesn't load or the entry point doesn't exist, then abandon all further attempts to send codemarkers.
                _state = State.DisabledDueToDllImportException;
                return false;
            }

            return true;
        }

        /// <summary>
        ///  Sends a code marker event with additional user data
        /// </summary>
        /// <param name="nTimerID">The code marker event ID</param>
        /// <param name="aBuff">User data buffer. May not be null.</param>
        /// <returns>true if the code marker was successfully sent, false if code markers are not enabled or an error occurred.</returns>
        /// <exception cref="ArgumentNullException">aBuff was null</exception>
        public bool CodeMarkerEx(int nTimerID, byte[] aBuff)
        {
            if (!IsEnabled)
            {
                return false;
            }

            // Check the arguments only after checking whether code markers are enabled
            // This allows the calling code to pass null value and avoid calculation of data if nothing is to be logged
            if (aBuff is null)
            {
                throw new ArgumentNullException(nameof(aBuff));
            }

            try
            {
                if (ShouldUseTestDll)
                {
                    NativeMethods.TestDllPerfCodeMarker(nTimerID, aBuff, aBuff.Length);
                }
                else
                {
                    NativeMethods.ProductDllPerfCodeMarker(nTimerID, aBuff, aBuff.Length);
                }
            }
            catch (DllNotFoundException)
            {
                // If the DLL doesn't load or the entry point doesn't exist, then
                // abandon all further attempts to send codemarkers.
                _state = State.DisabledDueToDllImportException;
                return false;
            }

            return true;
        }

        /// <summary>
        ///  Used by ManagedPerfTrack.cs to report errors accessing the DLL.
        /// </summary>
        public void SetStateDLLException()
        {
            _state = State.DisabledDueToDllImportException;
        }

        /// <summary>
        ///  Sends a code marker event with additional Guid user data
        /// </summary>
        /// <param name="nTimerID">The code marker event ID</param>
        /// <param name="guidData">The additional Guid to include with the event</param>
        /// <returns>true if the code marker was successfully sent, false if code markers are not enabled or an error occurred.</returns>
        public bool CodeMarkerEx(int nTimerID, Guid guidData)
        {
            return CodeMarkerEx(nTimerID, guidData.ToByteArray());
        }

        /// <summary>
        ///  Sends a code marker event with additional String user data
        /// </summary>
        /// <param name="nTimerID">The code marker event ID</param>
        /// <param name="stringData">The additional String to include with the event</param>
        /// <returns>true if the code marker was successfully sent, false if code markers are not enabled or an error occurred.</returns>
        public bool CodeMarkerEx(int nTimerID, string stringData)
        {
            if (!IsEnabled)
            {
                return false;
            }

            // Check the arguments only after checking whether code markers are enabled
            // This allows the calling code to pass null value and avoid calculation of data if nothing is to be logged
            if (stringData is null)
            {
                throw new ArgumentNullException(nameof(stringData));
            }

            try
            {
                int byteCount = Text.Encoding.Unicode.GetByteCount(stringData) + sizeof(char);
                if (ShouldUseTestDll)
                {
                    NativeMethods.TestDllPerfCodeMarkerString(nTimerID, stringData, byteCount);
                }
                else
                {
                    NativeMethods.ProductDllPerfCodeMarkerString(nTimerID, stringData, byteCount);
                }
            }
            catch (DllNotFoundException)
            {
                // If the DLL doesn't load or the entry point doesn't exist, then abandon all further attempts to send codemarkers.
                _state = State.DisabledDueToDllImportException;
                return false;
            }

            return true;
        }

        /// <summary>
        ///  Converts a string into a byte buffer including a zero terminator (needed for proper ETW message formatting)
        /// </summary>
        /// <param name="stringData">String to be converted to bytes</param>
        internal static byte[] StringToBytesZeroTerminated(string stringData)
        {
            Text.Encoding encoding = Text.Encoding.Unicode;
            int stringByteLength = encoding.GetByteCount(stringData);
            byte[] data = new byte[stringByteLength + sizeof(char)]; /* string + null termination */
            encoding.GetBytes(stringData, 0, stringData.Length, data, 0); // null terminator is already there, just write string over it
            return data;
        }

        public static byte[] AttachCorrelationId(byte[] buffer, Guid correlationId)
        {
            if (correlationId == Guid.Empty)
            {
                return buffer;
            }

            byte[] correlationIdBytes = correlationId.ToByteArray();
            byte[] bufferWithCorrelation = new byte[s_correlationMarkBytes.Length + correlationIdBytes.Length + (buffer != null ? buffer.Length : 0)];
            s_correlationMarkBytes.CopyTo(bufferWithCorrelation, 0);
            correlationIdBytes.CopyTo(bufferWithCorrelation, s_correlationMarkBytes.Length);
            if (buffer != null)
            {
                buffer.CopyTo(bufferWithCorrelation, s_correlationMarkBytes.Length + correlationIdBytes.Length);
            }

            return bufferWithCorrelation;
        }

        /// <summary>
        ///  Sends a code marker event with additional DWORD user data
        /// </summary>
        /// <param name="nTimerID">The code marker event ID</param>
        /// <param name="uintData">The additional DWORD to include with the event</param>
        /// <returns>true if the code marker was successfully sent, false if code markers are not enabled or an error occurred.</returns>
        public bool CodeMarkerEx(int nTimerID, uint uintData)
        {
            return CodeMarkerEx(nTimerID, BitConverter.GetBytes(uintData));
        }

        /// <summary>
        ///  Sends a code marker event with additional QWORD user data
        /// </summary>
        /// <param name="nTimerID">The code marker event ID</param>
        /// <param name="ulongData">The additional QWORD to include with the event</param>
        /// <returns>true if the code marker was successfully sent, false if code markers are not enabled or an error occurred.</returns>
        public bool CodeMarkerEx(int nTimerID, ulong ulongData)
        {
            return CodeMarkerEx(nTimerID, BitConverter.GetBytes(ulongData));
        }

        /// <summary>
        ///  Checks the registry to see if code markers are enabled
        /// </summary>
        /// <param name="regRoot">The registry root</param>
        /// <param name="registryView">The registry view.</param>
        /// <returns>Whether CodeMarkers are enabled in the registry</returns>
        private static bool UsePrivateCodeMarkers(string regRoot, RegistryView registryView)
        {
            if (regRoot is null)
            {
                throw new ArgumentNullException(nameof(regRoot));
            }

            // Reads the Performance subkey from the given registry key
            using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, registryView))
            using (RegistryKey key = baseKey.OpenSubKey(regRoot + "\\Performance"))
            {
                if (key != null)
                {
                    // Read the default value
                    // It doesn't matter what the value is, if it's present and not empty, code markers are enabled
                    string defaultValue = key.GetValue(string.Empty).ToString();
                    return !string.IsNullOrEmpty(defaultValue);
                }
            }

            return false;
        }
    }

    /// <summary>
    ///  Use CodeMarkerStartEnd in a using clause when you need to bracket an operation with a start/end CodeMarker event pair.  If you are using correlated codemarkers and providing your own event manifest, include two GUIDs (the correlation "marker" and the correlation ID itself) as the very first fields.
    /// </summary>
    internal struct CodeMarkerStartEnd : IDisposable
    {
        private int _end;
        private byte[] _buffer;

        internal CodeMarkerStartEnd(int begin, int end, bool correlated = false)
        {
            Debug.Assert(end != default);
            _buffer =
                correlated
                ? CodeMarkers.AttachCorrelationId(null, Guid.NewGuid())
                : null;
            _end = end;
            CodeMarker(begin);
        }

        public void Dispose()
        {
            if (_end != default) // Protect against multiple Dispose calls
            {
                CodeMarker(_end);
                _end = default;
                _buffer = null; // allow it to be GC'd
            }
        }

        private void CodeMarker(int id)
        {
            if (_buffer is null)
            {
                CodeMarkers.Instance.CodeMarker(id);
            }
            else
            {
                CodeMarkers.Instance.CodeMarkerEx(id, _buffer);
            }
        }
    }
}
