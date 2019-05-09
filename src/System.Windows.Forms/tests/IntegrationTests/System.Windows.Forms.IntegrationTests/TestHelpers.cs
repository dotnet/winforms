// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Threading;
using System.IO;
using System.Reflection;

namespace System.Windows.Forms.Func.Tests
{
    public class TestHelpers
    {

        /// <summary>
        /// Calls StartProcess for the ProcessStartInfo containing the bin path of this directory plus the given byPathFromBinToExe; also ensures that repo\.dotnet\dotnet.exe exists
        /// </summary>
        /// <param name="byPathFromBinToExe">The string path to add onto the end of the bin path in order to reach the exe to run; trimed for tailing \'s</param>
        /// <seealso cref="StartProcess(ProcessStartInfo)"/>
        /// <seealso cref="BinPath()"/>
        /// <seealso cref="System.Diagnostics.Process"/>
        /// <seealso cref="System.IO.File.Exists(string)"/>
        /// <remarks>Throws ArgumentException if string byPathFromBin is null</remarks>
        /// <returns>The new Process</returns>
        public static Process StartProcess(string byPathFromBinToExe)
        {
            if(byPathFromBinToExe == null)
            {
                throw new ArgumentNullException(nameof(byPathFromBinToExe));
            }

            if (!byPathFromBinToExe.EndsWith(".exe", StringComparison.CurrentCultureIgnoreCase))
            {
                throw new ArgumentException(nameof(byPathFromBinToExe) + " must end in a .exe");
            }

            var dotnetPath = DotNetPath();
            if (!Directory.Exists(dotnetPath))
            {
                throw new DirectoryNotFoundException(dotnetPath + " directory cannot be found.");
            }

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = Path.Combine(BinPath(), byPathFromBinToExe.Trim('\\'));
            startInfo.EnvironmentVariables["DOTNET_ROOT"] = dotnetPath;	// required
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
        /// <remarks>Returns the entire path of this project if the bin is not part of it</remarks>
        /// <returns>The bin path as a string; example: example:\Project\bin\</returns>
        public static string BinPath()
        {
            return RelativePathForwardTo("bin");
        }


        public static string DotNetPath()
        {
            return RelativePathBackwardsUntilFind(".dotnet");
        }

        /// <summary>
        /// Returns the stop directory of this project on a given machine
        /// </summary>
        /// <param name="stop">The string to stop at in the path; compared all lower</param>
        /// <seealso cref="System.AppDomain.CurrentDomain.BaseDirectory"/>
        /// <seealso cref="System.IO.Path.DirectorySeparatorChar"/>
        /// <seealso cref="System.IO.Path.Combine(string, string)"/>
        /// <remarks>Returns the entire path of this project if the stop is not part of it</remarks>
        /// <returns>The path as a string; example: example:\Project\bin\ given "bin" if bin is present in the path</returns>
        public static string RelativePathForwardTo(string stop)
        {
            if(string.IsNullOrEmpty(stop))
            {
                throw new ArgumentException(nameof(stop) + " must not be null or empty.");
            }

            string ret = string.Empty;
            var path = AppDomain.CurrentDomain.BaseDirectory;
            var pathParts = path.Split(Path.DirectorySeparatorChar);
            uint i = 0;
            while (i < pathParts.Length)
            {
                ret = Path.Combine(ret, pathParts[i]);
                if (pathParts[i].Equals(stop, StringComparison.CurrentCultureIgnoreCase))
                {
                    break;
                }
                i++;
            }
            return ret;
        }

        /// <summary>
        /// Looks backwards form the current executing directory until it finds a sibling directory seek, then returns the full path of that sibling
        /// </summary>
        /// <param name="seek">The sibling directory to look for</param>
        /// <seealso cref="System.Reflection.Assembly.GetExecutingAssembly()"/>
        /// <seealso cref="System.IO.Path.GetDirectoryName(ReadOnlySpan{char})"/>
        /// <seealso cref="System.IO.Directory.GetDirectoryRoot(string)"/>
        /// <seealso cref="System.IO.Directory.GetDirectories(string, string, SearchOption)"/>
        /// <seealso cref="System.IO.Path.Combine(string, string)"/>
        /// <seealso cref="System.IO.Directory.GetParent(string)"/>
        /// <returns>The full path of the first sibling directory by the current executing directory, away from the root</returns>
        public static string RelativePathBackwardsUntilFind(string seek)
        {
            if (string.IsNullOrEmpty(seek))
            {
                throw new ArgumentNullException(nameof(seek));
            }

            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            var codeBasePath = Uri.UnescapeDataString(codeBaseUrl.AbsolutePath);
            var currentDirectory = Path.GetDirectoryName(codeBasePath);
            var root = Directory.GetDirectoryRoot(currentDirectory);
            while (!currentDirectory.Equals(root, StringComparison.CurrentCultureIgnoreCase))
            {
                if (Directory.GetDirectories(currentDirectory, seek, SearchOption.TopDirectoryOnly).Length == 1)
                {
                    var ret = Path.Combine(currentDirectory, seek);
                    return ret;
                }
                currentDirectory = Directory.GetParent(currentDirectory).FullName;
            }
            throw new DirectoryNotFoundException($"No {seek} folder was found among siblings of subfolders of {codeBasePath}.");
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
        /// Presses Right on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Right key to</param>
        /// <returns>Whether or not the Right key was pressed on the process</returns>
        /// <seealso cref="PressOnProcess(Process, string)"/>
        public static bool PressRightOnProcess(Process process)
        {
            return PressOnProcess(process, "{RIGHT}");
        }

        /// <summary>
        /// Presses Down on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Down key to</param>
        /// <returns>Whether or not the Down key was pressed on the process</returns>
        /// <seealso cref="PressOnProcess(Process, string)"/>
        public static bool PressDownOnProcess(Process process)
        {
            return PressOnProcess(process, "{DOWN}");
        }

        /// <summary>
        /// Presses Left on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Left key to</param>
        /// <returns>Whether or not the Left key was pressed on the process</returns>
        /// <seealso cref="PressOnProcess(Process, string)"/>
        public static bool PressLeftOnProcess(Process process)
        {
            return PressOnProcess(process, "{LEFT}");
        }

        /// <summary>
        /// Presses Up on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Up key to</param>
        /// <returns>Whether or not the Up key was pressed on the process</returns>
        /// <seealso cref="PressOnProcess(Process, string)"/>
        public static bool PressUpOnProcess(Process process)
        {
            return PressOnProcess(process, "{UP}");
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
