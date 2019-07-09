// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using ReflectTools;
using WFCTestLib.Log;
using System.Reflection;
using System.Windows.Forms.IntegrationTests.Common;
using System.Threading;

/**
 * Priority 1 Tests for BackgroundWorker
 * author: ddrozd
 */

namespace System.Windows.Forms.IntegrationTests.MauiTests
{

    public class MauiBackgroundWorkerTests : ReflectBase
{

    private BackgroundWorker backgroundWorker1;
    private BackgroundWorker backgroundWorker2;
    private Boolean workerCompleted;
    private Boolean workerThrewWhenAccessed;
    private object workerResult;
    private object failedResult;
    private int workerProgressPercentage;

    const int numberToCompute = 14;
    const int resultExpected = 610;

    /**
	 * Calls static method LaunchTest to start the tests
	 */
    public static void Main(String[] args)
    {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiBackgroundWorkerTests(args));
    }

    public MauiBackgroundWorkerTests(String[] args) : base(args)
    {
            this.BringToForeground();
            this.Text = "Priority 1 Tests for BackgroundWorker";
    }

    protected override void InitTest(TParams p)
    {
        base.InitTest(p);

        // Create and Initialize component
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();

        // backgroundWorker1
        this.backgroundWorker1.WorkerReportsProgress = true;
        this.backgroundWorker1.WorkerSupportsCancellation = true;
        this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
        this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
        this.backgroundWorker1.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
    }

    private void backgroundWorker1_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
        // This method should not manipulate any WinForms controls created on the UI thread
        e.Result = ComputeFibonacci(numberToCompute, this.backgroundWorker1, e);
    }

    private void backgroundWorker1_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
        workerCompleted = true;
        if (e.Cancelled == false)
            workerResult = e.Result;
    }

    private void backgroundWorker1_ProgressChanged(object sender, System.ComponentModel.ProgressChangedEventArgs e)
    {
        workerProgressPercentage = e.ProgressPercentage;
    }

    long ComputeFibonacci(int n, BackgroundWorker worker, DoWorkEventArgs e)
    {
        // The parameter n must be >= 0 and <= 91. 
        // Fib(n), with n > 91, overflows a long. 
        if ((n < 0) || (n > 91))
        {
            throw new ArgumentOutOfRangeException("value must be >= 0 and <= 91", "n");
        }

        long result = 0;
        int highestPercentageReached = 0;

        // Abort the operation if the user has cancelled. 
        if (worker.CancellationPending)
        {
            e.Cancel = true;
        }
        else
        {
            if (n < 2)
            {
                result = 1;
            }
            else
            {
                result = ComputeFibonacci(n - 1, worker, e) + ComputeFibonacci(n - 2, worker, e);
            }

            // Report progress as a percentage of the total task. 
            // Note: this percentage is going to be wrong: the percent will keep drifting upwards and back down because of our
            // funky dual recursion.
            int percentComplete = (int)((float)(numberToCompute - n) / (float)numberToCompute * 100);

            if (percentComplete > highestPercentageReached)
            {
                highestPercentageReached = percentComplete;
                worker.ReportProgress(percentComplete);
            }
        }

        return result;
    }

    private void InitializeFailureComponent()
    {
        this.backgroundWorker2 = new System.ComponentModel.BackgroundWorker();

        // backgroundWorker1
        this.backgroundWorker2.WorkerReportsProgress = true;
        this.backgroundWorker2.WorkerSupportsCancellation = true;
        this.backgroundWorker2.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker2_DoWork);
        this.backgroundWorker2.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker2_RunWorkerCompleted);
        this.backgroundWorker2.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker1_ProgressChanged);
    }
    private void backgroundWorker2_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
    {
        throw (new MyException());
    }

    private void backgroundWorker2_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
    {
        /*
                try
                {
                    failedResult = e.Result;
                }
                catch (MyException ex)
                {
                    // expect to throw because e.Result is being accessed
                    workerThrewWhenAccessed = true;
                }
                workerCompleted = true;
        */
        try
        {
            failedResult = e.Result;
        }
        catch (TargetInvocationException ex)
        {
            if (ex.InnerException is MyException)
            {
                // expect to throw because e.Result is being accessed
                workerThrewWhenAccessed = true;
            }
            else
            {
                throw;
            }
        }
        workerCompleted = true;





    }

    protected override bool BeforeScenario(TParams p, MethodInfo scenario)
    {
        workerCompleted = false;
        return base.BeforeScenario(p, scenario);
    }

        //======================================================
        // ReportProgress()
        //======================================================
        //
        // ReportProgress() when WorkerReportsProgress is false
        // Expected Result: InvalidOperationException thrown
        //

        [Scenario(true)]
        protected ScenarioResult ReportProgress(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        InitializeComponent();
        backgroundWorker1.WorkerReportsProgress = false;

            try
            {
            backgroundWorker1.ReportProgress(10);
            sr.IncCounters(false, "didn't throw invalid operation exception", p.log);
        }
        catch (InvalidOperationException)
            {
            sr.IncCounters(true, "", p.log);
        }

        return sr;
    }

        //======================================================
        // NotRunningCancel()
        //======================================================
        //
        // Cancel when WorkerSupportsCancellation = false; 
        // Expected Result: InvalidOperationException thrown
        //

        [Scenario(true)]
        protected ScenarioResult NotRunningCancel(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        InitializeComponent();
        backgroundWorker1.WorkerSupportsCancellation = false;


            // try cancelling when not running
            try
            {
            backgroundWorker1.CancelAsync();
            sr.IncCounters(false, "didn't throw exception when not running", p.log);
        }
        catch (InvalidOperationException)
            {
            sr.IncCounters(true, "", p.log);
        }

        return sr;
    }

        //======================================================
        // RunningCancel()
        //======================================================
        //
        // Cancel when WorkerSupportsCancellation = false; 
        // Expected Result: InvalidOperationException thrown
        //

        [Scenario(true)]
        protected ScenarioResult RunningCancel(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        InitializeComponent();
        backgroundWorker1.WorkerSupportsCancellation = false;


            // try cancelling when running
            try
            {
            backgroundWorker1.RunWorkerAsync();
            backgroundWorker1.CancelAsync();
            sr.IncCounters(false, "didn't throw exception when running", p.log);
        }
        catch (InvalidOperationException)
            {
            sr.IncCounters(true, "", p.log);
        }
        return sr;
    }

        //======================================================
        // AfterCompletedCancel()
        //======================================================
        //
        // Cancel after completed event 
        // Expected Result: InvalidOperationException thrown
        //

        [Scenario(true)]
        protected ScenarioResult AfterCompletedCancel(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        InitializeComponent();
        backgroundWorker1.WorkerSupportsCancellation = false;


            // try cancelling when running
            try
            {
            backgroundWorker1.RunWorkerAsync();
            while (!workerCompleted)
            {
                System.Threading.Thread.Sleep(1000);
                Application.DoEvents();
            }
            backgroundWorker1.CancelAsync();
            sr.IncCounters(false, "didn't throw exception when completed", p.log);
        }
        catch (InvalidOperationException)
            {
            sr.IncCounters(true, "", p.log);
        }
        return sr;
    }
        //======================================================
        // RunTwice()
        //======================================================
        //
        // After RunWorkerAsync has completed, verify runs again
        // Expected Result: Runs twice, with valid result
        //

        [Scenario(true)]
        protected ScenarioResult RunTwice(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        InitializeComponent();

        backgroundWorker1.RunWorkerAsync();

        while (!workerCompleted)
        {
            System.Threading.Thread.Sleep(1000);
            Application.DoEvents();
        }

        workerCompleted = false;
        Wait(30);
        backgroundWorker1.RunWorkerAsync();
        while (!workerCompleted)
        {
            System.Threading.Thread.Sleep(1000);
            Application.DoEvents();
        }

        sr.IncCounters(((long)workerResult) == resultExpected, this.workerResult.ToString(), p.log);
        return sr;
    }

    private static void Wait(int time)
    {
        for (int i = 0; i < time; i++)
        {
            System.Threading.Thread.Sleep(100);
            Application.DoEvents();
        }

    }

        //======================================================
        // RunWhenAlreadyRunning
        //======================================================
        //
        // Call RunWorkerAsync on a backgroundWorker which is already running; should throw invalid operation exception
        // Expected Result: InvalidOperationException
        //

        [Scenario(true)]
        protected ScenarioResult RunWhenAlreadyRunning(TParams p)
    {
        InitializeComponent();

        backgroundWorker1.RunWorkerAsync();
        try
        {
            backgroundWorker1.RunWorkerAsync();
            return new ScenarioResult(false, "Didn't throw expected exception", p.log);
        }
        catch (InvalidOperationException)
        {
            return new ScenarioResult(true, "Threw expected exception", p.log);
        }
    }

        //======================================================
        // RunWithoutDoWorkEventHandler
        //======================================================
        //
        // Call RunWorkerAsync on a backgroundWorker when DoWork eventhandler is not attached; 
        // Expected Result: InvalidOperationException
        //

        [Scenario(true)]
        protected ScenarioResult RunWithoutDoWorkEventHandler(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        BackgroundWorker backgroundWorker3 = new BackgroundWorker();

            try
            {
            backgroundWorker3.RunWorkerAsync();
            sr.IncCounters(true, "running without DoWorkEventHandler attached doesn't throw", p.log);
        }
        catch (InvalidOperationException)
            {
            sr.IncCounters(false, "InvalidOperation appropriately thrown", p.log);
        }
        return sr;
    }

        //======================================================
        // FailedWorkerNonNullException
        //======================================================
        //
        // Listen for Completed; 
        // verify that a task which throws an exception has a non-null exception value for CompletedEventArgs
        //

        [Scenario(true)]
        protected ScenarioResult FailedWorkerNonNullException(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();
        InitializeFailureComponent();
        workerCompleted = false;
        workerThrewWhenAccessed = false;

        backgroundWorker2.RunWorkerAsync();
        while (!workerCompleted)
        {
            System.Threading.Thread.Sleep(1000);
            Application.DoEvents();
        }

        sr.IncCounters(workerThrewWhenAccessed, "worker did not throw when result was accessed", p.log);
        try
        {
            MyException ex = (MyException)failedResult;
        }
        catch (Exception e)
        {
            sr.IncCounters(e.GetType() == typeof(MyException), e.GetType().ToString(), p.log);
        }


        return sr;
    }

        //======================================================
        // FailedWorkerThrowsException
        //======================================================
        //
        // Listen for Completed; 
        // verify that when CompletedEventArgs have a non-null exception value, the exception is thrown when result is accessed
        //

        [Scenario(true)]
        protected ScenarioResult FailedWorkerThrowsException(TParams p)
    {
        ScenarioResult sr = new ScenarioResult();

        InitializeFailureComponent();
        workerCompleted = false;
        workerThrewWhenAccessed = false;

        backgroundWorker2.RunWorkerAsync();
        while (!workerCompleted)
        {
            System.Threading.Thread.Sleep(1000);
            Application.DoEvents();
        }

        sr.IncCounters(workerThrewWhenAccessed, "worker did not throw when result was accessed", p.log);

        return sr;
    }
}

public class MyException : Exception
{
    public override string Message
    {
        get
        {
            return "Failure message";
        }
    }

}
}

// [Scenarios]
//@ ReportProgress()
//@ NotRunningCancel()
//@ RunningCancel()
//@ AfterCompletedCancel()
//@ RunTwice()
//@ RunWhenAlreadyRunning()
//@ RunWithoutDoWorkEventHandler()
//@ FailedWorkerNonNullException()
//@ FailedWorkerThrowsException()
