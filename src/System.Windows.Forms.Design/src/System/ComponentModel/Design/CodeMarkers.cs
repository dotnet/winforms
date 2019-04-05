﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;

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

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            [DllImport(TestDllName, EntryPoint = "PerfCodeMarker")]
            public static extern void TestDllPerfCodeMarkerString(int nTimerID, [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 2)] string aUserParams, int cbParams);

            [DllImport(ProductDllName, EntryPoint = "PerfCodeMarker")]
            public static extern void ProductDllPerfCodeMarker(int nTimerID, [MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 2)] byte[] aUserParams, int cbParams);

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            [DllImport(ProductDllName, EntryPoint = "PerfCodeMarker")]
            public static extern void ProductDllPerfCodeMarkerString(int nTimerID, [MarshalAs(UnmanagedType.LPWStr, SizeParamIndex = 2)] string aUserParams, int cbParams);

            /// global native method imports
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern ushort FindAtom([MarshalAs(UnmanagedType.LPWStr)] string lpString);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            public static extern IntPtr GetModuleHandle([MarshalAs(UnmanagedType.LPWStr)] string lpModuleName);
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
            /// The atom is present. CodeMarkers are enabled.
            /// </summary>
            Enabled,

            /// <summary>
            /// The atom is not present, but InitPerformanceDll has not yet been called.
            /// </summary>
            Disabled,

            /// <summary>
            /// Disabled because the CodeMarkers transport DLL could not be found or an import failed to resolve.
            /// </summary>
            DisabledDueToDllImportException
        }

        private State _state;

        /// <summary>
        /// Are CodeMarkers enabled? Note that even if IsEnabled returns false, CodeMarkers may still be enabled later in another component.
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
        private readonly string _regroot = null;
        private bool? _shouldUseTestDll;

        // This guid should match vscommon\testtools\PerfWatson2\Responsiveness\Listener\Microsoft.Performance.ResponseTime\ContextProviders\ScenarioContextProvider\ScenarioContextProvider.cs
        // And also match toolsrc\Telescope\Batch\PerfWatsonService.Reducer\SessionProcessors\ScenarioProcessor.cs
        private static readonly byte[] s_correlationMarkBytes = new Guid("AA10EEA0-F6AD-4E21-8865-C427DAE8EDB9").ToByteArray();

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
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
                        if (_regroot == null)
                        {
                            _shouldUseTestDll = NativeMethods.GetModuleHandle(ProductDllName) == IntPtr.Zero;
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
        /// Sends a code marker event
        /// </summary>
        /// <param name="nTimerID">The code marker event ID</param>
        /// <returns>true if the code marker was successfully sent, false if code markers are not enabled or an error occurred.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public bool CodeMarker(int nTimerID)
        {
            if (!IsEnabled)
                return false;

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
        /// Sends a code marker event with additional user data
        /// </summary>
        /// <param name="nTimerID">The code marker event ID</param>
        /// <param name="aBuff">User data buffer. May not be null.</param>
        /// <returns>true if the code marker was successfully sent, false if code markers are not enabled or an error occurred.</returns>
        /// <exception cref="ArgumentNullException">aBuff was null</exception>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public bool CodeMarkerEx(int nTimerID, byte[] aBuff)
        {
            if (!IsEnabled)
                return false;

            // Check the arguments only after checking whether code markers are enabled
            // This allows the calling code to pass null value and avoid calculation of data if nothing is to be logged
            if (aBuff == null)
                throw new ArgumentNullException(nameof(aBuff));

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
        /// Used by ManagedPerfTrack.cs to report errors accessing the DLL.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public void SetStateDLLException()
        {
            _state = State.DisabledDueToDllImportException;
        }


        /// <summary>
        /// Sends a code marker event with additional Guid user data
        /// </summary>
        /// <param name="nTimerID">The code marker event ID</param>
        /// <param name="guidData">The additional Guid to include with the event</param>
        /// <returns>true if the code marker was successfully sent, false if code markers are not enabled or an error occurred.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public bool CodeMarkerEx(int nTimerID, Guid guidData)
        {
            return CodeMarkerEx(nTimerID, guidData.ToByteArray());
        }

        /// <summary>
        /// Sends a code marker event with additional String user data
        /// </summary>
        /// <param name="nTimerID">The code marker event ID</param>
        /// <param name="stringData">The additional String to include with the event</param>
        /// <returns>true if the code marker was successfully sent, false if code markers are not enabled or an error occurred.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public bool CodeMarkerEx(int nTimerID, string stringData)
        {
            if (!IsEnabled)
                return false;

            // Check the arguments only after checking whether code markers are enabled
            // This allows the calling code to pass null value and avoid calculation of data if nothing is to be logged
            if (stringData == null)
                throw new ArgumentNullException(nameof(stringData));

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
        /// Converts a string into a byte buffer including a zero terminator (needed for proper ETW message formatting)
        /// </summary>
        /// <param name="stringData">String to be converted to bytes</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal static byte[] StringToBytesZeroTerminated(string stringData)
        {
            Text.Encoding encoding = Text.Encoding.Unicode;
            int stringByteLength = encoding.GetByteCount(stringData);
            byte[] data = new byte[stringByteLength + sizeof(char)]; /* string + null termination */
            encoding.GetBytes(stringData, 0, stringData.Length, data, 0); // null terminator is already there, just write string over it
            return data;
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
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
        /// Sends a code marker event with additional DWORD user data
        /// </summary>
        /// <param name="nTimerID">The code marker event ID</param>
        /// <param name="uintData">The additional DWORD to include with the event</param>
        /// <returns>true if the code marker was successfully sent, false if code markers are not enabled or an error occurred.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public bool CodeMarkerEx(int nTimerID, uint uintData)
        {
            return CodeMarkerEx(nTimerID, BitConverter.GetBytes(uintData));
        }

        /// <summary>
        /// Sends a code marker event with additional QWORD user data
        /// </summary>
        /// <param name="nTimerID">The code marker event ID</param>
        /// <param name="ulongData">The additional QWORD to include with the event</param>
        /// <returns>true if the code marker was successfully sent, false if code markers are not enabled or an error occurred.</returns>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public bool CodeMarkerEx(int nTimerID, ulong ulongData)
        {
            return CodeMarkerEx(nTimerID, BitConverter.GetBytes(ulongData));
        }

        /// <summary>
        /// Checks the registry to see if code markers are enabled
        /// </summary>
        /// <param name="regRoot">The registry root</param>
        /// <param name="registryView">The registry view.</param>
        /// <returns>Whether CodeMarkers are enabled in the registry</returns>
        private static bool UsePrivateCodeMarkers(string regRoot, RegistryView registryView)
        {
            if (regRoot == null)
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

#if Codemarkers_IncludeAppEnum
        /// <summary>
        /// Check the registry and, if appropriate, loads and initializes the code markers dll.
        /// InitPerformanceDll may be called more than once, but only the first successful call will do anything.
        /// Subsequent calls will be ignored.
        /// For 32-bit processes on a 64-bit machine, the 32-bit (Wow6432Node) registry will be used.
        /// For 64-bit processes, the 64-bit registry will be used. If you need to use the Wow6432Node in this case
        /// then use the overload of InitPerformanceDll that takes a RegistryView parameter.
        /// </summary>
        /// <param name="iApp">The application ID value that distinguishes these code marker events from other applications.</param>
        /// <param name="strRegRoot">The registry root of the application. The default value of the "Performance" subkey under this
        /// root will be checked to determine if CodeMarkers should be enabled.</param>
        /// <returns>true if CodeMarkers were initialized successfully, or if InitPerformanceDll has already been called
        /// successfully once.
        /// false indicates that either CodeMarkers are not enabled in the registry, or that the CodeMarkers transport
        /// DLL failed to load.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public bool InitPerformanceDll(int iApp, string strRegRoot)
        {            
            return InitPerformanceDll(iApp, strRegRoot, RegistryView.Default);
        }

        /// <summary>
        /// Check the registry and, if appropriate, loads and initializes the code markers dll.
        /// InitPerformanceDll may be called more than once, but only the first successful call will do anything.
        /// Subsequent calls will be ignored.
        /// </summary>
        /// <param name="iApp">The application ID value that distinguishes these code marker events from other applications.</param>
        /// <param name="strRegRoot">The registry root of the application. The default value of the "Performance" subkey under this
        /// root will be checked to determine if CodeMarkers should be enabled.</param>
        /// <param name="registryView">Specify RegistryView.Registry32 to use the 32-bit registry even if the calling application
        /// is 64-bit</param>
        /// <returns>true if CodeMarkers were initialized successfully, or if InitPerformanceDll has already been called
        /// successfully once.
        /// false indicates that either CodeMarkers are not enabled in the registry, or that the CodeMarkers transport
        /// DLL failed to load.</returns>
        public bool InitPerformanceDll(int iApp, string strRegRoot, RegistryView registryView)
        {           
            // Prevent multiple initializations.
            if (IsEnabled)
            {
                return true;
            }

            if (strRegRoot == null)
            {
                throw new ArgumentNullException(nameof(strRegRoot));
            }
            
            this.regroot = strRegRoot;
            this.registryView = registryView;

            try
            {
                if (this.ShouldUseTestDll)
                {
                    NativeMethods.TestDllInitPerf(iApp);
                }
                else
                {
                    NativeMethods.ProductDllInitPerf(iApp);
                }
                
                this.state = State.Enabled;
                
                // Add an ATOM so that other CodeMarker enabled code in this process
                // knows that CodeMarkers are enabled 
                NativeMethods.AddAtom(AtomName);
            }
            // catch BadImageFormatException to handle 64-bit process loading 32-bit CodeMarker DLL (e.g., msbuild.exe)
            catch (BadImageFormatException)
            {
                this.state = State.DisabledDueToDllImportException;
            }
            catch (DllNotFoundException)
            {
                this.state = State.DisabledDueToDllImportException;
                return false;
            }

            return true;
        }

        
        // Opposite of InitPerformanceDLL. Call it when your app does not need the code markers dll.
        public void UninitializePerformanceDLL(int iApp)
        {
            bool? usingTestDL = this.shouldUseTestDll; // remember this or we can end up uninitializing the wrong dll.
            this.shouldUseTestDll = null; // reset which DLL we should use (needed for unit testing)
            this.regroot = null;

            if (!IsEnabled)
            {
                return;
            }

            this.state = State.Disabled;

            // Delete the atom created during the initialization if it exists
            System.UInt16 atom = NativeMethods.FindAtom(AtomName);
            if (atom != 0)
            {
                NativeMethods.DeleteAtom(atom);
            }

            try
            {
                if (usingTestDL.HasValue)  // If we don't have a value, then we never initialized the DLL.
                {
                    if (usingTestDL.Value)
                    {
                        NativeMethods.TestDllUnInitPerf(iApp);
                    }
                    else
                    {
                        NativeMethods.ProductDllUnInitPerf(iApp);
                    }
                }
            }
            catch (DllNotFoundException)
            {
                // Swallow exception
            }
        }        
#endif //Codemarkers_IncludeAppEnum
    }

#if !Codemarkers_NoCodeMarkerStartEnd
    /// <summary>
    /// Use CodeMarkerStartEnd in a using clause when you need to bracket an operation with a start/end CodeMarker event pair.  If you are using correlated codemarkers and providing your own event manifest, include two GUIDs (the correlation "marker" and the correlation ID itself) as the very first fields.
    /// </summary>
    internal struct CodeMarkerStartEnd : IDisposable
    {
        private int _end;
        private byte[] _buffer;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
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
            if (_buffer == null)
            {
                CodeMarkers.Instance.CodeMarker(id);
            }
            else
            {
                CodeMarkers.Instance.CodeMarkerEx(id, _buffer);
            }
        }
    }

    /// <summary>
    /// Use CodeMarkerExStartEnd in a using clause when you need to bracket an operation with a start/end CodeMarker event pair.  If you are using correlated codemarkers and providing your own event manifest, include two GUIDs (the correlation "marker" and the correlation ID itself) as the very first fields.
    /// </summary>
    internal struct CodeMarkerExStartEnd : IDisposable
    {
        private int _end;
        private byte[] _aBuff;

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CodeMarkerExStartEnd(int begin, int end, byte[] aBuff, bool correlated = false)
        {
            Debug.Assert(end != default);
            _aBuff =
                correlated
                ? CodeMarkers.AttachCorrelationId(aBuff, Guid.NewGuid())
                : aBuff;
            _end = end;
            CodeMarkers.Instance.CodeMarkerEx(begin, _aBuff);
        }

        // Specialization to use Guids for the code marker data
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CodeMarkerExStartEnd(int begin, int end, Guid guidData, bool correlated = false)
            : this(begin, end, guidData.ToByteArray(), correlated)
        {
        }

        // Specialization for string
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CodeMarkerExStartEnd(int begin, int end, string stringData, bool correlated = false)
            : this(begin, end, CodeMarkers.StringToBytesZeroTerminated(stringData), correlated)
        {
        }

        // Specialization for uint
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CodeMarkerExStartEnd(int begin, int end, uint uintData, bool correlated = false)
            : this(begin, end, BitConverter.GetBytes(uintData), correlated)
        {
        }

        // Specialization for ulong
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CodeMarkerExStartEnd(int begin, int end, ulong ulongData, bool correlated = false)
            : this(begin, end, BitConverter.GetBytes(ulongData), correlated)
        {
        }

        public void Dispose()
        {
            if (_end != default) // Protect against multiple Dispose calls
            {
                CodeMarkers.Instance.CodeMarkerEx(_end, _aBuff);
                _end = default;
                _aBuff = null;
            }
        }
    }

#endif
}
