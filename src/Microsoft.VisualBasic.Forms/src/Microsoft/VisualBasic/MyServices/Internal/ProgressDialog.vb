' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Option Explicit On
Option Strict On

Imports System.Drawing
Imports System.Globalization
Imports System.Threading
Imports System.Windows.Forms

Namespace Microsoft.VisualBasic.MyServices.Internal

    ''' <summary>
    '''  A dialog that shows progress used for Network.Download and Network.Upload
    ''' </summary>
    Friend Class ProgressDialog
        Inherits Windows.Forms.Form

        ''' <summary>
        ''' Event raised when user cancels the dialog or closes it before the operation is completed
        ''' </summary>
        Public Event UserHitCancel()

        ''' <summary>
        '''  Constructor
        ''' </summary>
        Friend Sub New()
            MyBase.New()
            InitializeComponent()
        End Sub

        ''' <summary>
        '''  Increments the progress bar by the passed in amount
        ''' </summary>
        ''' <param name="incrementAmount">The amount to increment the bar</param>
        ''' <remarks>
        '''  This method should never be called directly. It should be called with
        '''  an InvokeBegin by a secondary thread.
        '''</remarks>
        Public Sub Increment(incrementAmount As Integer)
            ProgressBarWork.Increment(incrementAmount)
        End Sub

        ''' <summary>
        '''  Closes the Progress Dialog
        ''' </summary>
        ''' <remarks>
        '''  This method should never be called directly. It should be called with
        '''  an InvokeBegin by a secondary thread.
        '''</remarks>
        Public Sub CloseDialog()
            _closeDialogInvoked = True
            Close()
        End Sub

        ''' <summary>
        '''  Displays the progress dialog modally
        ''' </summary>
        ''' <remarks>This method should be called on the main thread after the worker thread has been started</remarks>
        Public Sub ShowProgressDialog()
            Try
                If Not _closing Then
                    ShowDialog()
                End If
            Finally
                FormClosableSemaphore.Set()
            End Try
        End Sub

        ''' <summary>
        '''  Sets the text of the label (usually something like Copying x to y)
        ''' </summary>
        ''' <value>The value to set the label to</value>
        ''' <remarks>This should only be called on the main thread before showing the dialog</remarks>
        Public Property LabelText() As String
            Get
                Return LabelInfo.Text
            End Get
            Set(Value As String)
                LabelInfo.Text = Value
            End Set
        End Property

        ''' <summary>
        ''' Used to set or get the semaphore which signals when the dialog
        ''' is in a closable state.
        ''' </summary>
        ''' <value>The ManualResetEvent</value>
        Public ReadOnly Property FormClosableSemaphore() As ManualResetEvent
            Get
                Return _formClosableSemaphore
            End Get
        End Property

        ''' <summary>
        '''  Inform the dialog that CloseDialog will soon be called
        ''' </summary>
        ''' <remarks>
        '''  This method should be called directly from the secondary thread. We want
        '''  to indicate we're closing as soon as we can so w don't show the dialog when we
        '''  don't need to (when the work is finished before we can show the dialog)
        '''</remarks>
        Public Sub IndicateClosing()
            _closing = True
        End Sub

        ''' <summary>
        '''  Indicated if the user has clicked the cancel button
        ''' </summary>
        ''' <value>True if the user has canceled, otherwise False</value>
        ''' <remarks>
        '''  The secondary thread checks this property directly. If it's True, the thread
        '''  breaks out of its loop.
        '''</remarks>
        Public ReadOnly Property UserCanceledTheDialog() As Boolean
            Get
                Return _canceled
            End Get
        End Property

        ''' <summary>
        ''' This enables a dialog with a close button, sizable borders, and no icon
        ''' </summary>
        Protected Overrides ReadOnly Property CreateParams() As CreateParams
            Get
                Dim cp As CreateParams = MyBase.CreateParams
                cp.Style = cp.Style Or WS_THICKFRAME
                Return cp
            End Get
        End Property

        ''' <summary>
        '''  Handles user clicking Cancel. Sets a flag read by secondary thread.
        ''' </summary>
        ''' <param name="sender">The cancel button</param>
        ''' <param name="e">Arguments</param>
        Private Sub ButtonCloseDialog_Click(sender As Object, e As EventArgs) Handles ButtonCloseDialog.Click
            ButtonCloseDialog.Enabled = False
            _canceled = True
            RaiseEvent UserHitCancel()
        End Sub

        ''' <summary>
        ''' Indicates the form is closing
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' We listen for this event since we want to make closing the dialog before it's
        ''' finished behave the same as a cancel
        '''</remarks>
        Private Sub ProgressDialog_FormClosing(sender As Object, e As Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
            If e.CloseReason = CloseReason.UserClosing And Not _closeDialogInvoked Then
                ' If the progress bar isn't finished and the user hasn't already canceled
                If ProgressBarWork.Value < 100 And Not _canceled Then
                    ' Cancel the Close since we want the dialog to be closed from a call from the
                    ' secondary thread
                    e.Cancel = True

                    ' Indicate the user has canceled. We'll actually close the dialog from WebClientCopy
                    _canceled = True
                    RaiseEvent UserHitCancel()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Ensure the label resizes with the dialog
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks>
        ''' Since the label has AutoSize set to True we have to set the maximum size so the label
        ''' will grow down rather than off the dialog. As the size of the dialog changes, the maximum
        ''' size needs to be adjusted.
        ''' </remarks>
        Private Sub ProgressDialog_Resize(sender As Object, e As EventArgs) Handles Me.Resize
            LabelInfo.MaximumSize = New Size(ClientSize.Width - BORDER_SIZE, 0)
        End Sub

        ''' <summary>
        '''  Exits the monitor when we're activated
        ''' </summary>
        ''' <param name="sender">Dialog</param>
        ''' <param name="e">Arguments</param>
        Private Sub ProgressDialog_Activated(sender As Object, e As EventArgs) Handles Me.Shown
            _formClosableSemaphore.Set()
        End Sub

        ' Indicates whether or not the dialog is closing
        Private _closing As Boolean

        ' Indicates whether or not the user has canceled the copy
        Private _canceled As Boolean

        ' Used to signal when the dialog is in a closable state. The dialog is in a closable
        ' state when it has been activated or when it has been flagged to be closed before
        ' ShowDialog has been called
        Private _formClosableSemaphore As ManualResetEvent = New ManualResetEvent(False)

        ' Indicates CloseDialog has been called
        Private _closeDialogInvoked As Boolean

        ' Constant used to get re-sizable dialog with border style set to fixed dialog.
        Private Const WS_THICKFRAME As Integer = &H40000

        ' Border area for label (10 pixels on each side)
        Private Const BORDER_SIZE As Integer = 20

#Region " Windows Form Designer generated code "

        'Form overrides dispose to clean up the component list.
        Protected Overloads Overrides Sub Dispose(disposing As Boolean)
            If disposing Then
                If Not (_components Is Nothing) Then
                    _components.Dispose()
                End If
                If _formClosableSemaphore IsNot Nothing Then
                    _formClosableSemaphore.Dispose()
                    _formClosableSemaphore = Nothing
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub
        Friend WithEvents LabelInfo As Windows.Forms.Label
        Friend WithEvents ProgressBarWork As Windows.Forms.ProgressBar
        Friend WithEvents ButtonCloseDialog As Windows.Forms.Button

        'Required by the Windows Form Designer
        Private ReadOnly _components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.
        'Do not modify it using the code editor.
        <DebuggerStepThrough()>
        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ProgressDialog))
            LabelInfo = New Windows.Forms.Label
            ProgressBarWork = New Windows.Forms.ProgressBar
            ButtonCloseDialog = New Windows.Forms.Button
            SuspendLayout()
            '
            'LabelInfo
            '
            resources.ApplyResources(LabelInfo, "LabelInfo", CultureInfo.CurrentUICulture)
            LabelInfo.MaximumSize = New Size(300, 0)
            LabelInfo.Name = "LabelInfo"
            '
            'ProgressBarWork
            '
            resources.ApplyResources(ProgressBarWork, "ProgressBarWork", CultureInfo.CurrentUICulture)
            ProgressBarWork.Name = "ProgressBarWork"
            '
            'ButtonCloseDialog
            '
            resources.ApplyResources(ButtonCloseDialog, "ButtonCloseDialog", CultureInfo.CurrentUICulture)
            ButtonCloseDialog.Name = "ButtonCloseDialog"
            '
            'ProgressDialog
            '
            resources.ApplyResources(Me, "$this", CultureInfo.CurrentUICulture)
            Controls.Add(ButtonCloseDialog)
            Controls.Add(ProgressBarWork)
            Controls.Add(LabelInfo)
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            MaximizeBox = False
            MinimizeBox = False
            Name = "ProgressDialog"
            ShowInTaskbar = False
            ResumeLayout(False)
            PerformLayout()

        End Sub

#End Region

    End Class
End Namespace
