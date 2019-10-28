' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports Microsoft.VisualBasic
Imports System
Imports System.Globalization
Imports System.Text
Imports System.Collections
Imports System.Threading
Imports System.Runtime.InteropServices
Imports System.Diagnostics

Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Imports Microsoft.Win32
Imports Microsoft.VisualBasic.CompilerServices

Namespace Microsoft.VisualBasic.CompilerServices

    Friend NotInheritable Class VBInputBox
        Inherits System.Windows.Forms.Form

        Private components As System.ComponentModel.Container
        Private TextBox As System.Windows.Forms.TextBox
        Private Label As System.Windows.Forms.Label
        Private OKButton As System.Windows.Forms.Button
        Private MyCancelButton As System.Windows.Forms.Button
        Public Output As String = ""

        'This constructor needed to be able to show the designer at designtime.
        Friend Sub New()
            MyBase.New()
            InitializeComponent()
        End Sub

        Friend Sub New(ByVal Prompt As String, ByVal Title As String, ByVal DefaultResponse As String, ByVal XPos As Integer, ByVal YPos As Integer)
            MyBase.New()
            InitializeComponent()
            InitializeInputBox(Prompt, Title, DefaultResponse, XPos, YPos)
        End Sub

        Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(VBInputBox))
            Me.OKButton = New System.Windows.Forms.Button
            Me.MyCancelButton = New System.Windows.Forms.Button
            Me.TextBox = New System.Windows.Forms.TextBox
            Me.Label = New System.Windows.Forms.Label
            Me.SuspendLayout()
            '
            'OKButton
            '
            resources.ApplyResources(Me.OKButton, "OKButton", CultureInfo.CurrentUICulture)
            Me.OKButton.Name = "OKButton"
            '
            'MyCancelButton
            '
            Me.MyCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
            resources.ApplyResources(Me.MyCancelButton, "MyCancelButton", CultureInfo.CurrentUICulture)
            Me.MyCancelButton.Name = "MyCancelButton"
            '
            'TextBox
            '
            resources.ApplyResources(Me.TextBox, "TextBox", CultureInfo.CurrentUICulture)
            Me.TextBox.Name = "TextBox"
            '
            'Label
            '
            resources.ApplyResources(Me.Label, "Label", CultureInfo.CurrentUICulture)
            Me.Label.Name = "Label"
            '
            'VBInputBox
            '
            Me.AcceptButton = Me.OKButton
            resources.ApplyResources(Me, "$this", CultureInfo.CurrentUICulture)
            Me.CancelButton = Me.MyCancelButton
            Me.Controls.Add(Me.TextBox)
            Me.Controls.Add(Me.Label)
            Me.Controls.Add(Me.OKButton)
            Me.Controls.Add(Me.MyCancelButton)
            Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            Me.MaximizeBox = False
            Me.MinimizeBox = False
            Me.Name = "VBInputBox"
            Me.ResumeLayout(False)
            Me.PerformLayout()

        End Sub

        'Initialize labels etc from the args passed in to InputBox()
        Private Sub InitializeInputBox(ByVal Prompt As String, ByVal Title As String, ByVal DefaultResponse As String, ByVal XPos As Integer, ByVal YPos As Integer)
            Me.Text = Title
            Label.Text = Prompt
            TextBox.Text = DefaultResponse
            AddHandler OKButton.Click, AddressOf Me.OKButton_Click
            AddHandler MyCancelButton.Click, AddressOf Me.MyCancelButton_Click

            'Re-size the dialog if the prompt is too large
            Dim LabelGraphics As Graphics = Label.CreateGraphics
            Dim LabelSizeNeeded As SizeF = LabelGraphics.MeasureString(Prompt, Label.Font, Label.Width)
            LabelGraphics.Dispose()
            If LabelSizeNeeded.Height > Label.Height Then
                'The current label size is not large enough to accommodate the prompt.  We need
                '  to expand the label and the dialog, and move the textbox to make room.
                Dim DialogHeightChange As Integer = CInt(LabelSizeNeeded.Height) - Label.Height
                Label.Height += DialogHeightChange
                TextBox.Top += DialogHeightChange
                Me.Height += DialogHeightChange
            End If

            'Position the form
            If (XPos = -1) AndAlso (YPos = -1) Then
                Me.StartPosition = FormStartPosition.CenterScreen
            Else
                If (XPos = -1) Then XPos = 600
                If (YPos = -1) Then YPos = 350
                Me.StartPosition = FormStartPosition.Manual
                Me.DesktopLocation = New Point(XPos, YPos)
            End If
        End Sub

        Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Output = TextBox.Text
            Me.Close()
        End Sub

        Private Sub MyCancelButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
            Me.Close()
        End Sub
    End Class

End Namespace
