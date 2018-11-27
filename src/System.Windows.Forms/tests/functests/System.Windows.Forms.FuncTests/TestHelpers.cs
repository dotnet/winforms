using System.Diagnostics;
using System.Threading;

namespace System.Windows.Forms.FuncTests
{
    public class TestHelpers
    {
        /// <summary>
        /// Calls StartProcess for the ProcessStartInfo containing the bin path of this directory plsu the given byPathFromBin
        /// </summary>
        /// <param name="byPathFromBin">The string path to add onto the end of the bin path; trimed for tailing \'s</param>
        /// <seealso cref="StartProcess(ProcessStartInfo)"/>
        /// <seealso cref="BinPath()"/>
        /// <seealso cref="System.Diagnostics.Process"/>
        /// <remarks>Throws ArgumentException if string byPathFromBin is null</remarks>
        /// <returns>The new Process</returns>
        public static Process StartProcess(string byPathFromBin)
        {
            if(null == byPathFromBin)
            {
                throw new ArgumentException(nameof(byPathFromBin) + " must not be null.");
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = BinPath() + byPathFromBin.Trim('\\');
            // ...

            return StartProcess(startInfo);
        }

        /// <summary>
        /// Calls Process.Start() on the given ProcessStartInfo; waits 500 ms
        /// </summary>
        /// <param name="info">The info with which to start the process</param>
        /// <seealso cref="System.Diagnostics.Process.Start()"/>
        /// <seealso cref="System.Threading.Thread.Sleep(int)"/>
        /// <remarks>Throws ArgumentException if ProcessStartInfo info is null</remarks>
        /// <returns>The new Process</returns>
        public static Process StartProcess(ProcessStartInfo info)
        {
            Process process = new Process();

            process.StartInfo = info ?? throw new ArgumentException(nameof(info) + " must not be null.");
            process.Start();

            Thread.Sleep(500);

            return process;
        }

        /// <summary>
        /// Returns the bin directory of this project on a given machine
        /// </summary>
        /// <seealso cref="System.AppDomain.CurrentDomain.BaseDirectory"/>
        /// <seealso cref="System.IO.Path.DirectorySeparatorChar"/>
        /// <remarks>Returns the entire path of this project if the bin is not part of it</remarks>
        /// <returns>The bin path as a string; example: example:\Project\bin\</returns>
        public static string BinPath()
        {
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var pathParts = path.Split('\\');
            string ret = "";
            uint i = 0;
            while (i < pathParts.Length)
            {
                ret += pathParts[i] + IO.Path.DirectorySeparatorChar;
                if (pathParts[i].ToLower().Equals("bin".ToLower()))
                {
                    break;
                }
                i++;
            }
            return ret;
        }

        /// <summary>
        /// Presses Enter on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Enter key to</param>
        /// <returns>Whether or not the Enter key was pressed on the process</returns>
        /// <seealso cref="PressOnProcess(Process, string)"/>
        public static bool PressEnterOnProcess(Process process)
        {
            return PressOnProcess(process, "~");
        }

        /// <summary>
        /// Presses Tab on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Tab key to</param>
        /// <returns>Whether or not the Tab key was pressed on the process</returns>
        /// <seealso cref="PressOnProcess(Process, string)"/>
        public static bool PressTabOnProcess(Process process)
        {
            return PressOnProcess(process, "{TAB}");
        }

        /// <summary>
        /// Presses Tab any number of times on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Tab key(s) to</param>
        /// <param name="times">The number of times to press tab in a row</param>
        /// <remarks>Throws an ArgumentException if number of times is zero; this is unlikely to be intended.</remarks>
        /// <returns>Whether or not the Tab key(s) were pressed on the process</returns>
        /// <seealso cref="PressOnProcess(Process, string)"/>
        public static bool PressTabsOnProcess(Process process, uint times)
        {
            if (times == 0)
            {
                throw new ArgumentException(nameof(times) + " may not be zero.");
            }

            string keys = "";
            for (uint i = 0; i < times; i++)
            {
                keys += "{TAB}";
            }
            return PressOnProcess(process, keys);
        }

        /// <summary>
        /// Presses the given keys on the given process, then waits 200ms
        /// </summary>
        /// <param name="process">The process to send the key(s) to</param>
        /// <param name="keys">The key(s) to send to the process</param>
        /// <remarks>Throws an ArgumentException if the process is null or has exited.</remarks>
        /// <remarks>Throws an ArgumentException if the given key(s) is null or the empty string.</remarks>
        /// <returns>Whether or not the key(s) were pressed on the process</returns>
        /// <seealso cref="System.Diagnostics.Process.MainWindowHandle"/>
        /// <seealso cref="ExternalTestHelpers.TrySetForegroundWindow(IntPtr)"/>
        /// <seealso cref="ExternalTestHelpers.TryGetForegroundWindow()"/>
        /// <seealso cref="System.Windows.Forms.SendKeys.SendWait(string)"/>
        /// <seealso cref="System.Threading.Thread.Sleep(int)"/>
        internal static bool PressOnProcess(Process process, string keys)
        {
            if (null == process)
            {
                throw new ArgumentException(nameof(process) + " must not be null.");
            }

            if (process.HasExited)
            {
                throw new ArgumentException(nameof(process) + " must not have exited.");
            }

            if (string.IsNullOrEmpty(keys))
            {
                throw new ArgumentException(nameof(keys) + " must not be null or empty.");
            }

            var handle = process.MainWindowHandle;
            ExternalTestHelpers.TrySetForegroundWindow(handle);

            if (handle.Equals(ExternalTestHelpers.TryGetForegroundWindow()))
            {
                SendKeys.SendWait(keys);             

                Thread.Sleep(200);

                return true;
            }
            else
            {
                Debug.Assert(true, "Given process could not be made to be the ForegroundWindow");

                return false;
            }
        }

    }
}
