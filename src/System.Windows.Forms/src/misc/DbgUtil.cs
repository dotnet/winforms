// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using static Interop;

namespace System.Windows.Forms.Internal
{
    /// <summary>
    ///  Debug help utility.
    /// </summary>
    internal sealed class DbgUtil
    {
        public static int gdipInitMaxFrameCount = 8;
        // disable csharp compiler warning #0414: field assigned unused value
#pragma warning disable 0414
        public static int gdiUseMaxFrameCount = 8;
        public static int finalizeMaxFrameCount = 5;
#pragma warning restore 0414

        // Methods

        [Conditional("DEBUG")]
        public static void AssertWin32(bool expression, string message)
        {
#if DEBUG
            if (!expression)
            {
                Debug.Fail(message + "\r\nError: " + DbgUtil.GetLastErrorStr());
            }
#endif
        }

        [Conditional("DEBUG")]
        public static void AssertWin32(bool expression, string format, object arg1)
        {
#if DEBUG
            if (!expression)
            {
                object[] args = new object[] { arg1 };
                AssertWin32Impl(expression, format, args);
            }
#endif
        }

        [Conditional("DEBUG")]
        public static void AssertWin32(bool expression, string format, object arg1, object arg2)
        {
#if DEBUG
            if (!expression)
            {
                object[] args = new object[] { arg1, arg2 };
                AssertWin32Impl(expression, format, args);
            }
#endif
        }

        [Conditional("DEBUG")]
        public static void AssertWin32(bool expression, string format, object arg1, object arg2, object arg3)
        {
#if DEBUG
            if (!expression)
            {
                object[] args = new object[] { arg1, arg2, arg3 };
                AssertWin32Impl(expression, format, args);
            }
#endif
        }

        [Conditional("DEBUG")]
        public static void AssertWin32(bool expression, string format, object arg1, object arg2, object arg3, object arg4)
        {
#if DEBUG
            if (!expression)
            {
                object[] args = new object[] { arg1, arg2, arg3, arg4 };
                AssertWin32Impl(expression, format, args);
            }
#endif
        }

        [Conditional("DEBUG")]
        public static void AssertWin32(bool expression, string format, object arg1, object arg2, object arg3, object arg4, object arg5)
        {
#if DEBUG
            if (!expression)
            {
                object[] args = new object[] { arg1, arg2, arg3, arg4, arg5 };
                AssertWin32Impl(expression, format, args);
            }
#endif
        }

        [Conditional("DEBUG")] // This code will be compiled into the assembly anyways, it is up to the compiler to ignore the call.
        private static void AssertWin32Impl(bool expression, string format, object[] args)
        {
#if DEBUG
            if (!expression)
            {
                string message = string.Format(CultureInfo.CurrentCulture, format, args);
                Debug.Fail(message + "\r\nError: " + DbgUtil.GetLastErrorStr());
            }
#endif
        }

        //
        // WARNING: Your PInvoke function needs to have the DllImport.SetLastError=true for this method
        // to work properly.  From the MSDN:
        // GetLastWin32Error exposes the Win32 GetLastError API method from Kernel32.DLL. This method exists
        // because it is not safe to make a direct platform invoke call to GetLastError to obtain this information.
        // If you want to access this error code, you must call GetLastWin32Error rather than writing your own
        // platform invoke definition for GetLastError and calling it. The common language runtime can make
        // internal calls to APIs that overwrite the operating system maintained GetLastError.
        //
        // You can only use this method to obtain error codes if you apply the System.Runtime.InteropServices.DllImportAttribute
        // to the method signature and set the SetLastError field to true.
        //
        public static string GetLastErrorStr()
        {
            int MAX_SIZE = 255;
            StringBuilder buffer = new StringBuilder(MAX_SIZE);
            string message;
            int err = 0;

            try
            {
                err = Marshal.GetLastWin32Error();
                uint retVal = Kernel32.FormatMessageW(
                    Kernel32.FormatMessageOptions.IGNORE_INSERTS | Kernel32.FormatMessageOptions.FROM_SYSTEM,
                    IntPtr.Zero,
                    (uint)err,
                    Kernel32.GetUserDefaultLCID(),
                    buffer,
                    MAX_SIZE,
                    IntPtr.Zero);
                message = retVal != 0 ? buffer.ToString() : "<error returned>";
            }
            catch (Exception ex)
            {
                if (DbgUtil.IsCriticalException(ex))
                {
                    throw;  //rethrow critical exception.
                }
                message = ex.ToString();
            }

            return string.Format(CultureInfo.CurrentCulture, "0x{0:x8} - {1}", err, message);
        }

        /// <summary>
        ///  Duplicated here from ClientUtils not to depend on that code because this class is to be
        ///  compiled into System.Drawing and System.Windows.Forms.
        /// </summary>
        private static bool IsCriticalException(Exception ex)
        {
            return
                //ex is NullReferenceException ||
                ex is StackOverflowException ||
                ex is OutOfMemoryException ||
                ex is Threading.ThreadAbortException;
        }

        /// <summary>
        ///  Returns information about the top stack frames in a string format.  The input param determines the number of
        ///  frames to include.
        /// </summary>
        public static string StackFramesToStr(int maxFrameCount)
        {
            string trace = string.Empty;

            try
            {
                StackTrace st = new StackTrace(true);
                int dbgUtilFrameCount = 0;

                //
                // Ignore frames for methods on this library.
                // Note: The stack frame holds the latest frame at index 0.
                //
                while (dbgUtilFrameCount < st.FrameCount)
                {
                    StackFrame sf = st.GetFrame(dbgUtilFrameCount);

                    if (sf is null || sf.GetMethod().DeclaringType != typeof(DbgUtil))
                    {
                        break;
                    }

                    dbgUtilFrameCount++;
                }

                maxFrameCount += dbgUtilFrameCount; // add ignored frames.

                if (maxFrameCount > st.FrameCount)
                {
                    maxFrameCount = st.FrameCount;
                }

                for (int i = dbgUtilFrameCount; i < maxFrameCount; i++)
                {
                    StackFrame sf = st.GetFrame(i);

                    if (sf is null)
                    {
                        continue;
                    }

                    MethodBase mi = sf.GetMethod();

                    if (mi is null)
                    {
                        continue;
                    }

                    string args = string.Empty;
                    string fileName = sf.GetFileName();

                    int backSlashIndex = fileName is null ? -1 : fileName.LastIndexOf('\\');

                    if (backSlashIndex != -1)
                    {
                        fileName = fileName.Substring(backSlashIndex + 1, fileName.Length - backSlashIndex - 1);
                    }

                    foreach (ParameterInfo pi in mi.GetParameters())
                    {
                        args += pi.ParameterType.Name + ", ";
                    }

                    if (args.Length > 0)   // remove last comma.
                    {
                        args = args.Substring(0, args.Length - 2);
                    }

                    trace += string.Format(CultureInfo.CurrentCulture, "at {0} {1}.{2}({3})\r\n", fileName, mi.DeclaringType, mi.Name, args);
                }
            }
            catch (Exception ex)
            {
                if (DbgUtil.IsCriticalException(ex))
                {
                    throw;  //rethrow critical exception.
                }
                trace += ex.ToString();
            }

            return trace.ToString();
        }

        /// <summary>
        ///  Returns information about the top stack frames in a string format.  The input param determines the number of
        ///  frames to include.  The 'message' parameter is used as the header of the returned string.
        /// </summary>
        public static string StackTraceToStr(string message, int frameCount)
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}\r\nTop Stack Trace:\r\n{1}", message, DbgUtil.StackFramesToStr(frameCount));
        }

        /// <summary>
        ///  Returns information about the top stack frames in a string format. The 'message' parameter is used as the header of the returned string.
        /// </summary>
        public static string StackTraceToStr(string message)
        {
            return StackTraceToStr(message, DbgUtil.gdipInitMaxFrameCount);
        }
    }
}
