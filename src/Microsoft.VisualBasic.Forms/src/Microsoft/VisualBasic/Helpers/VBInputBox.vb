' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.
' See the LICENSE file in the project root for more information.

Imports System.Globalization

Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms

Namespace Microsoft.VisualBasic.CompilerServices

    Friend NotInheritable Class VBInputBox
        Inherits Windows.Forms.Form

        Private ReadOnly _components As Container
        Private _textBox As Windows.Forms.TextBox
        Private _label As Windows.Forms.Label
        Private _oKButton As Windows.Forms.Button
        Private _myCancelButton As Windows.Forms.Button
        Public Output As String = ""

        'This constructor needed to be able to show the designer at design-time.
        Friend Sub New()
            MyBase.New()
            InitializeComponent()
        End Sub

        Friend Sub New(Prompt As String, Title As String, DefaultResponse As String, XPos As Integer, YPos As Integer)
            MyBase.New()
            InitializeComponent()
            InitializeInputBox(Prompt, Title, DefaultResponse, XPos, YPos)
        End Sub

        Protected Overloads Overrides Sub Dispose(disposing As Boolean)
            If disposing Then
                If Not (_components Is Nothing) Then
                    _components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(VBInputBox))
            _oKButton = New Windows.Forms.Button
            _myCancelButton = New Windows.Forms.Button
            _textBox = New Windows.Forms.TextBox
            _label = New Windows.Forms.Label
            SuspendLayout()
            '
            'OKButton
            '
            resources.ApplyResources(_oKButton, "OKButton", CultureInfo.CurrentUICulture)
            _oKButton.Name = "OKButton"
            '
            'MyCancelButton
            '
            _myCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
            resources.ApplyResources(_myCancelButton, "MyCancelButton", CultureInfo.CurrentUICulture)
            _myCancelButton.Name = "MyCancelButton"
            '
            'TextBox
            '
            resources.ApplyResources(_textBox, "TextBox", CultureInfo.CurrentUICulture)
            _textBox.Name = "TextBox"
            '
            'Label
            '
            resources.ApplyResources(_label, "Label", CultureInfo.CurrentUICulture)
            _label.Name = "Label"
            '
            'VBInputBox
            '
            AcceptButton = _oKButton
            resources.ApplyResources(Me, "$this", CultureInfo.CurrentUICulture)
            CancelButton = _myCancelButton
            Controls.Add(_textBox)
            Controls.Add(_label)
            Controls.Add(_oKButton)
            Controls.Add(_myCancelButton)
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
            MaximizeBox = False
            MinimizeBox = False
            Name = "VBInputBox"
            ResumeLayout(False)
            PerformLayout()

        End Sub

        'Initialize labels etc from the args passed in to InputBox()
        Private Sub InitializeInputBox(Prompt As String, Title As String, DefaultResponse As String, XPos As Integer, YPos As Integer)
            Text = Title
            _label.Text = Prompt
            _textBox.Text = DefaultResponse
            AddHandler _oKButton.Click, AddressOf OKButton_Click
            AddHandler _myCancelButton.Click, AddressOf MyCancelButton_Click

            'Re-size the dialog if the prompt is too large
            Dim LabelGraphics As Graphics = _label.CreateGraphics
            Dim LabelSizeNeeded As SizeF = LabelGraphics.MeasureString(Prompt, _label.Font, _label.Width)
            LabelGraphics.Dispose()
            If LabelSizeNeeded.Height > _label.Height Then
                'The current label size is not large enough to accommodate the prompt.  We need
                '  to expand the label and the dialog, and move the textbox to make room.
                Dim DialogHeightChange As Integer = CInt(LabelSizeNeeded.Height) - _label.Height
                _label.Height += DialogHeightChange
                _textBox.Top += DialogHeightChange
                Height += DialogHeightChange
            End If

            'Position the form
            If (XPos = -1) AndAlso (YPos = -1) Then
                StartPosition = FormStartPosition.CenterScreen
            Else
                If (XPos = -1) Then XPos = 600
                If (YPos = -1) Then YPos = 350
                StartPosition = FormStartPosition.Manual
                DesktopLocation = New Point(XPos, YPos)
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
