// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Text;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace System.Windows.Forms.UITests;

internal static class DataCollectionService
{
    private static readonly ConditionalWeakTable<Exception, StrongBox<bool>> s_loggedExceptions = [];
    private static ImmutableList<CustomLoggerData> s_customInProcessLoggers = [];
    private static bool s_firstChanceExceptionHandlerInstalled;

    [ThreadStatic]
#pragma warning disable IDE1006 // Naming Styles
    private static bool t_inHandler;
#pragma warning restore IDE1006

    internal static ITest? CurrentTest { get; set; }

    static DataCollectionService()
    {
        // Register the default custom logger to take screenshots on failure
        RegisterCustomLogger(
            ScreenshotService.TakeScreenshot,
            logId: string.Empty,
            extension: "png");
    }

    private static string CurrentTestName
    {
        get
        {
            if (CurrentTest is null)
            {
                return "Unknown";
            }

            return GetTestName(CurrentTest.TestCase);
        }
    }

    /// <summary>
    ///  Register a custom logger to collect data in the event of a test failure.
    /// </summary>
    /// <remarks>
    /// <para>
    ///  The <paramref name="logId"/> and <paramref name="extension"/> should be chosen to avoid conflicts with
    ///  other loggers. Otherwise, it is possible for logs to be overwritten during data collection. Built-in logs
    ///  include:
    /// </para>
    ///
    /// <list type="table">
    ///   <listheader>
    ///     <description><strong>Log ID</strong></description>
    ///     <description><strong>Extension</strong></description>
    ///     <description><strong>Purpose</strong></description>
    ///   </listheader>
    ///   <item>
    ///     <description>None</description>
    ///     <description><c>log</c></description>
    ///     <description>Exception details</description>
    ///   </item>
    ///   <item>
    ///     <description>None</description>
    ///     <description><c>png</c></description>
    ///     <description>Screenshot</description>
    ///   </item>
    /// </list>
    /// </remarks>
    /// <param name="callback">The callback to invoke to collect log information. The argument to the callback is
    ///  the fully-qualified file path where the log data should be written.
    /// </param>
    /// <param name="logId">An optional log identifier to include in the resulting file name.</param>
    /// <param name="extension">The extension to give the resulting file.</param>
    public static void RegisterCustomLogger(Action<string> callback, string logId, string extension)
    {
        ImmutableInterlocked.Update(
            ref s_customInProcessLoggers,
            (loggers, newLogger) => loggers.Add(newLogger),
            new CustomLoggerData(callback, logId, extension));
    }

    internal static string GetTestName(ITestCase testCase)
    {
        var testMethod = testCase.TestMethod.Method;
        string testClass = testCase.TestMethod.TestClass.Class.Name;
        int lastDot = testClass.LastIndexOf('.');
        testClass = testClass[(lastDot + 1)..];
        return $"{testClass}.{testMethod.Name}";
    }

    internal static void InstallFirstChanceExceptionHandler()
    {
        if (!s_firstChanceExceptionHandlerInstalled)
        {
            AppDomain.CurrentDomain.FirstChanceException += OnFirstChanceException;
            s_firstChanceExceptionHandlerInstalled = true;
        }
    }

    internal static bool LogAndCatch(Exception ex)
    {
        try
        {
            TryLog(ex);
        }
        catch
        {
            // Make sure exceptions do not escape the exception filter
        }

        return true;
    }

    internal static bool LogAndPropagate(Exception ex)
    {
        try
        {
            TryLog(ex);
        }
        catch
        {
            // Make sure exceptions do not escape the exception filter
        }

        return false;
    }

    internal static bool TryLog(Exception ex)
    {
        if (ex is null)
        {
            return false;
        }

        var logged = s_loggedExceptions.GetOrCreateValue(ex);
        if (logged.Value)
        {
            // Only log the first time an exception is thrown
            return false;
        }

        logged.Value = true;
        CaptureFailureState(CurrentTestName, ex);
        return true;
    }

    internal static void CaptureFailureState(string testName, Exception ex)
    {
        if (t_inHandler)
        {
            // Avoid stack overflow which could occur by recursively trying to capture failure states
            return;
        }

        try
        {
            t_inHandler = true;

            string logDir = GetLogDirectory();
            var timestamp = DateTimeOffset.UtcNow;
            testName ??= "Unknown";
            string errorId = ex.GetType().Name;

            Directory.CreateDirectory(logDir);

            StringBuilder exceptionDetails = new();
            exceptionDetails.AppendLine(ex.ToString());
            exceptionDetails.AppendLine("---------------------------------");
            exceptionDetails.AppendLine("Stack Trace at Log Time:");
            exceptionDetails.AppendLine(new StackTrace(true).ToString());
            File.WriteAllText(CreateLogFileName(logDir, timestamp, testName, errorId, logId: string.Empty, "log"), exceptionDetails.ToString());
            foreach (var (callback, logId, extension) in s_customInProcessLoggers)
            {
                callback(CreateLogFileName(logDir, timestamp, testName, errorId, logId, extension));
            }
        }
        finally
        {
            t_inHandler = false;
        }
    }

    private static void OnFirstChanceException(object? sender, FirstChanceExceptionEventArgs e)
    {
        if (e.Exception is not XunitException)
        {
            // Only xunit exceptions are logged in this handler
            return;
        }

        TryLog(e.Exception);
    }

    /// <summary>
    ///  Computes a full log file name.
    /// </summary>
    /// <param name="logDirectory">The location where logs are saved.</param>
    /// <param name="timestamp">The timestamp of the failure.</param>
    /// <param name="testName">The current test name, or <c>Unknown</c> if the test is not known.</param>
    /// <param name="errorId">The error ID, e.g. the name of the exception instance.</param>
    /// <param name="logId">
    ///  The log ID (e.g. <c>DotNet</c> or <c>Watson</c>). This may be an empty string for one log output of a
    ///  particular <paramref name="extension"/>.
    /// </param>
    /// <param name="extension">The log file extension, without a dot (e.g. <c>log</c>).</param>
    /// <returns>The fully qualified log file name.</returns>
    private static string CreateLogFileName(string logDirectory, DateTimeOffset timestamp, string testName, string errorId, string logId, string extension)
    {
        const int MaxPath = 260;

        string path = CombineElements(logDirectory, timestamp, testName, errorId, logId, extension);
        if (path.Length > MaxPath)
        {
            testName = testName[..Math.Max(0, testName.Length - (path.Length - MaxPath))];
            path = CombineElements(logDirectory, timestamp, testName, errorId, logId, extension);
        }

        return path;

        static string CombineElements(string logDirectory, DateTimeOffset timestamp, string testName, string errorId, string logId, string extension)
        {
            if (!string.IsNullOrEmpty(logId))
            {
                logId = $".{logId}";
            }

            string sanitizedTestName = new(testName.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray());
            string sanitizedErrorId = new(errorId.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray());

            return Path.Combine(Path.GetFullPath(logDirectory), $"{timestamp:HH.mm.ss}-{testName}-{errorId}{logId}.{extension}");
        }
    }

    internal static string GetLogDirectory()
    {
        return Path.Combine(GetBaseLogDirectory(), "Screenshots");
    }

    private static string GetBaseLogDirectory()
    {
        if (Environment.GetEnvironmentVariable("XUNIT_LOGS") is { Length: > 0 } baseLogDirectory)
        {
            return Path.GetFullPath(baseLogDirectory);
        }

        // Output assembly is located in a directory similar to:
        //   C:\dev\winforms\artifacts\bin\System.Windows.Forms.UI.IntegrationTests\Debug\net8.0
        string assemblyDirectory = GetAssemblyDirectory();

        int binPathSeparator = assemblyDirectory.IndexOf(@"\bin\", StringComparison.Ordinal);
        if (binPathSeparator > 0)
        {
            string configuration = Path.GetFileName(Path.GetDirectoryName(assemblyDirectory))!;
            return Path.Combine(assemblyDirectory[..binPathSeparator], "log", configuration);
        }

        return Path.Combine(assemblyDirectory, "xUnitResults");
    }

    private static string GetAssemblyDirectory()
    {
        string assemblyPath = typeof(DataCollectionService).Assembly.Location;
        return Path.GetDirectoryName(assemblyPath)!;
    }

    internal record struct CustomLoggerData(Action<string> Callback, string LogId, string Extension);
}
