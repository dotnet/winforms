﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using Newtonsoft.Json.Linq;
using static Interop.User32;

namespace System.Windows.Forms.IntegrationTests.Common
{
    public static class TestHelpers
    {
        /// <summary>
        ///  Gets the current config
        /// </summary>
        private static string Config
        {
            get
            {
#if DEBUG
                return "Debug";
#else
                return "Release";
#endif
            }
        }

        /// <summary>
        ///  Should always match the TargetFramework in the .csproj
        /// </summary>
        private static string TargetFramework
            => "netcoreapp5.0";

        /// <summary>
        ///  Get the output exe path for a specified project.
        ///  Throws an exception if the path does not exist
        /// </summary>
        /// <param name="projectName">The name of the project</param>
        /// <returns>The exe path</returns>
        public static string GetExePath(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
                throw new ArgumentNullException(nameof(projectName));

            var repoRoot = GetRepoRoot();
            var exePath = Path.Combine(repoRoot, $"artifacts\\bin\\{projectName}\\{Config}\\{TargetFramework}\\{projectName}.exe");

            if (!File.Exists(exePath))
                throw new FileNotFoundException("File does not exist", exePath);

            return exePath;
        }

        /// <summary>
        ///  Start a process with the specified path.
        ///  Also searches for a local .dotnet folder and adds it to the path.
        ///  If a local folder is not found, searches for a machine-wide install that matches
        ///  the version specified in the global.json.
        /// </summary>
        /// <param name="path">The path to the file to execute</param>
        /// <param name="setCwd">Set the current working directory to the process path</param>
        /// <returns>The new Process</returns>
        public static Process StartProcess(string path, bool setCwd = false)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("File does not exist", path);

            var startInfo = new ProcessStartInfo
            {
                FileName = path
            };
            if (setCwd)
            {
                startInfo.WorkingDirectory = Path.GetDirectoryName(path);
            }

            return StartProcess(startInfo);
        }

        /// <summary>
        ///  Start a process with the specified start info.
        ///  Also searches for a local .dotnet folder and adds it to the path.
        ///  If a local folder is not found, searches for a machine-wide install that matches
        ///  the version specified in the global.json.
        /// </summary>
        /// <param name="startInfo">The start info</param>
        /// <returns>The new Process</returns>
        public static Process StartProcess(ProcessStartInfo startInfo)
        {
            var dotnetPath = GetDotNetPath();
            if (!Directory.Exists(dotnetPath))
                throw new DirectoryNotFoundException(dotnetPath + " directory cannot be found.");

            var process = new Process();

            // Set the dotnet_root for the exe being launched
            // This allows the exe to look for runtime dependencies (like the shared framework (NetCore.App))
            // outside of the normal machine-wide install location (program files\dotnet)
            startInfo.EnvironmentVariables["DOTNET_ROOT"] = dotnetPath;
            process.StartInfo = startInfo;

            process.Start();
            process.WaitForExit(500);
            return process;
        }

        /// <summary>
        /// End the process.
        /// </summary>
        /// <returns>The process ExitCode</returns>
        public static int EndProcess(Process process, int timeout = Timeout.Infinite)
        {
            try
            {
                process.Kill();
            }
            catch (Exception)
            {
            }
            process.WaitForExit(timeout);
            return process.ExitCode;
        }

        /// <summary>
        ///  Get the path where dotnet is installed
        /// </summary>
        /// <returns>The full path of the folder containing the dotnet.exe</returns>
        private static string GetDotNetPath()
        {
            string dotNetPath;

            // walk the dir tree up, looking for a .dotnet folder
            try
            {
                dotNetPath = RelativePathBackwardsUntilFind(".dotnet");
            }
            catch (DirectoryNotFoundException)
            {
                // If there is no private install, check for a machine-wide install
                dotNetPath = GetGlobalDotNetPath();
            }

            return dotNetPath;
        }

        /// <summary>
        ///  Get the path of the globally installed dotnet that matches the version specified in the global.json
        ///
        ///  The file looks something like this:
        ///
        ///  {
        ///  "tools": {
        ///  "dotnet": "3.0.100-preview5-011568"
        ///  },
        ///
        ///  All we care about is the dotnet entry under tools
        /// </summary>
        /// <returns>The path to the globally installed dotnet that matches the version specified in the global.json.</returns>
        private static string GetGlobalDotNetPath()
        {
            // find the repo root
            var repoRoot = GetRepoRoot();

            // make sure there's a global.json
            var jsonFile = Path.Combine(repoRoot, "global.json");
            if (!File.Exists(jsonFile))
                throw new FileNotFoundException("global.json does not exist");

            // parse the file into a json object
            var jsonContents = File.ReadAllText(jsonFile);
            var jsonObject = JObject.Parse(jsonContents);

            string dotnetVersion;
            try
            {
                // check if tools:dotnet is specified
                dotnetVersion = jsonObject["tools"]["dotnet"].ToString();
            }
            catch
            {
                // no version was found, so we're done
                throw new Exception("global.json does not contain a tools:dotnet version");
            }

            // Check to see if the matching version is installed
            // The default install location is C:\Program Files\dotnet\sdk
            var defaultSdkRoot = @"C:\Program Files\dotnet\sdk";
            var sdkPath = Path.Combine(defaultSdkRoot, dotnetVersion);
            if (!Directory.Exists(sdkPath))
                throw new DirectoryNotFoundException($"dotnet sdk {dotnetVersion} is not installed globally");

            // if we get here, there was a match
            return sdkPath;
        }

        /// <summary>
        ///  Get the full path to the root of the repository
        /// </summary>
        /// <returns>The repo root</returns>
        private static string GetRepoRoot()
        {
            var gitPath = RelativePathBackwardsUntilFind(".git");
            var repoRoot = Directory.GetParent(gitPath).FullName;
            return repoRoot;
        }

        /// <summary>
        ///  Looks backwards form the current executing directory until it finds a sibling directory seek, then returns the full path of that sibling
        /// </summary>
        /// <param name="seek">The sibling directory to look for</param>
        /// <returns>The full path of the first sibling directory by the current executing directory, away from the root</returns>
        private static string RelativePathBackwardsUntilFind(string seek)
        {
            if (string.IsNullOrEmpty(seek))
            {
                throw new ArgumentNullException(nameof(seek));
            }

            var codeBaseUrl = new Uri(Assembly.GetExecutingAssembly().Location);
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
        ///  Presses Enter on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Enter key to</param>
        /// <returns>Whether or not the Enter key was pressed on the process</returns>
        /// <seealso cref="SendKeysToProcess(Process, string, bool)"/>
        public static bool SendEnterKeyToProcess(Process process, bool switchToMainWindow = true)
        {
            return SendKeysToProcess(process, "~", switchToMainWindow);
        }

        /// <summary>
        ///  Presses Tab on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Tab key to</param>
        /// <returns>Whether or not the Tab key was pressed on the process</returns>
        /// <seealso cref="SendKeysToProcess(Process, string, bool)"/>
        public static bool SendTabKeyToProcess(Process process, bool switchToMainWindow = true)
        {
            return SendKeysToProcess(process, "{TAB}", switchToMainWindow);
        }

        /// <summary>
        ///  Presses Right on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Right key to</param>
        /// <returns>Whether or not the Right key was pressed on the process</returns>
        /// <seealso cref="SendKeysToProcess(Process, string, bool)"/>
        public static bool SendRightArrowKeyToProcess(Process process, bool switchToMainWindow = true)
        {
            return SendKeysToProcess(process, "{RIGHT}", switchToMainWindow);
        }

        /// <summary>
        ///  Presses Down on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Down key to</param>
        /// <returns>Whether or not the Down key was pressed on the process</returns>
        /// <seealso cref="SendKeysToProcess(Process, string, bool)"/>
        public static bool SendDownArrowKeyToProcess(Process process, bool switchToMainWindow = true)
        {
            return SendKeysToProcess(process, "{DOWN}", switchToMainWindow);
        }

        /// <summary>
        ///  Presses Left on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Left key to</param>
        /// <returns>Whether or not the Left key was pressed on the process</returns>
        /// <seealso cref="SendKeysToProcess(Process, string, bool)"/>
        public static bool SendLeftArrowKeyToProcess(Process process, bool switchToMainWindow = true)
        {
            return SendKeysToProcess(process, "{LEFT}", switchToMainWindow);
        }

        /// <summary>
        ///  Presses Up on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Up key to</param>
        /// <returns>Whether or not the Up key was pressed on the process</returns>
        /// <seealso cref="SendKeysToProcess(Process, string, bool)"/>
        public static bool SendUpArrowKeyToProcess(Process process, bool switchToMainWindow = true)
        {
            return SendKeysToProcess(process, "{UP}");
        }

        /// <summary>
        ///  Presses Tab any number of times on the given process if it can be made the foreground process
        /// </summary>
        /// <param name="process">The process to send the Tab key(s) to</param>
        /// <param name="times">The number of times to press tab in a row</param>
        /// <remarks>Throws an ArgumentException if number of times is zero; this is unlikely to be intended.</remarks>
        /// <returns>Whether or not the Tab key(s) were pressed on the process</returns>
        /// <seealso cref="SendKeysToProcess(Process, string, bool)"/>
        public static bool SendTabKeysToProcess(Process process, MainFormControlsTabOrder times, bool switchToMainWindow = true)
        {
            return SendTabKeysToProcess(process, (int)times, switchToMainWindow);
        }

        public static bool SendTabKeysToProcess(Process process, int times, bool switchToMainWindow = true)
        {
            if (times == 0)
            {
                return true;
            }

            string keys = string.Empty;
            for (uint i = 0; i < times; i++)
            {
                keys += "{TAB}";
            }

            return SendKeysToProcess(process, keys, switchToMainWindow);
        }

        /// <summary>
        ///  Bring the specified form to the foreground
        /// </summary>
        /// <param name="form">The form</param>
        public static void BringToForeground(this Form form)
        {
            if (form is null)
                throw new ArgumentNullException(nameof(form));

            form.WindowState = FormWindowState.Minimized;
            form.Show();
            form.WindowState = FormWindowState.Normal;
        }

        /// <summary>
        ///  Set the culture for the current thread
        /// </summary>
        /// <param name="culture">The culture</param>
        public static void SetCulture(this Thread thread, string culture)
        {
            if (string.IsNullOrEmpty(culture))
                throw new ArgumentNullException(nameof(culture));

            var cultureInfo = new CultureInfo(culture);
            Thread.CurrentThread.CurrentCulture = cultureInfo;
            Thread.CurrentThread.CurrentUICulture = cultureInfo;
        }

        /// <summary>
        ///  Presses the given keys on the given process, then waits 200ms
        /// </summary>
        /// <param name="process">The process to send the key(s) to</param>
        /// <param name="keys">The key(s) to send to the process</param>
        /// <remarks>Throws an ArgumentException if the process is null or has exited.</remarks>
        /// <remarks>Throws an ArgumentException if the given key(s) is null or the empty string.</remarks>
        /// <returns>Whether or not the key(s) were pressed on the process</returns>
        /// <seealso cref="System.Diagnostics.Process.MainWindowHandle"/>
        /// <seealso cref="SetForegroundWindow(IntPtr)"/>
        /// <seealso cref="GetForegroundWindow()"/>
        /// <seealso cref="System.Windows.Forms.SendKeys.SendWait(string)"/>
        /// <seealso cref="System.Threading.Thread.Sleep(int)"/>
        internal static bool SendKeysToProcess(Process process, string keys, bool switchToMainWindow = true)
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

            IntPtr mainWindowHandle = process.MainWindowHandle;

            if (switchToMainWindow)
            {
                SetForegroundWindow(mainWindowHandle);
            }

            IntPtr foregroundWindow = GetForegroundWindow();

            string windowTitle = GetWindowText(foregroundWindow);

            if (GetWindowThreadProcessId(foregroundWindow, out uint processId) == 0 ||
                processId != process.Id)
            {
                Debug.WriteLine($"ForegroundWindow doesn't belong the test process! The current window is {windowTitle}.");
            }

            if ((switchToMainWindow && mainWindowHandle.Equals(foregroundWindow)) ||
                (!switchToMainWindow && foregroundWindow != IntPtr.Zero))
            {
                SendKeys.SendWait(keys);

                Thread.Sleep(200);

                return true;
            }
            else
            {
                Debug.Assert(true, $"Given process could not be made to be the ForegroundWindow. The current window is {windowTitle}.");

                return false;
            }
        }
    }
}
