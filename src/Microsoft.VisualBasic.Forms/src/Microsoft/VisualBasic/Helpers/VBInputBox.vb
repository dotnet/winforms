' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.ComponentModel
Imports System.Drawing
Imports System.Globalization
Imports System.Windows.Forms

Namespace Microsoft.VisualBasic.CompilerServices

    <DesignerCategory("Default")>
    Friend NotInheritable Class VBInputBox
        Inherits Form

#Disable Warning IDE1006 ' Naming Styles, Justification:=<VBInputBox.resx depends on these names>
        Private ReadOnly components As Container
        Private Label As Label
        Private MyCancelButton As Button
        Private OKButton As Button
        Private TextBox As TextBox
#Enable Warning IDE1006 ' Naming Styles
        Public Output As String = String.Empty

        'This constructor needed to be able to show the designer at design-time.
        Friend Sub New()
            MyBase.New()
            InitializeComponent()
        End Sub

        Friend Sub New(prompt As String, title As String, defaultResponse As String, xPos As Integer, yPos As Integer)
            MyBase.New()
            InitializeComponent()
            InitializeInputBox(prompt, title, defaultResponse, xPos, yPos)
        End Sub

        Private Sub InitializeComponent()
            Dim resources As New ComponentResourceManager(GetType(VBInputBox))
            OKButton = New Button
            MyCancelButton = New Button
            TextBox = New TextBox
            Label = New Label
            SuspendLayout()
            '
            'OKButton
            '
            resources.ApplyResources(OKButton, "OKButton", CultureInfo.CurrentUICulture)
            OKButton.Name = "OKButton"
            '
            'MyCancelButton
            '
            MyCancelButton.DialogResult = DialogResult.Cancel
            resources.ApplyResources(MyCancelButton, "MyCancelButton", CultureInfo.CurrentUICulture)
            MyCancelButton.Name = "MyCancelButton"
            '
            'TextBox
            '
            resources.ApplyResources(TextBox, "TextBox", CultureInfo.CurrentUICulture)
            TextBox.Name = "TextBox"
            '
            'Label
            '
            resources.ApplyResources(Label, "Label", CultureInfo.CurrentUICulture)
            Label.Name = "Label"
            '
            'VBInputBox
            '
            AcceptButton = OKButton
            resources.ApplyResources(Me, "$this", CultureInfo.CurrentUICulture)
            CancelButton = MyCancelButton
            Controls.Add(TextBox)
            Controls.Add(Label)
            Controls.Add(OKButton)
            Controls.Add(MyCancelButton)
            FormBorderStyle = FormBorderStyle.FixedDialog
            MaximizeBox = False
            MinimizeBox = False
            Name = "VBInputBox"
            ResumeLayout(False)
            PerformLayout()

        End Sub

        'Initialize labels etc from the args passed in to InputBox()
        Private Sub InitializeInputBox(prompt As String, title As String, defaultResponse As String, xPos As Integer, yPos As Integer)
            Text = title
            Label.Text = prompt
            TextBox.Text = defaultResponse
            AddHandler OKButton.Click, AddressOf OKButton_Click
            AddHandler MyCancelButton.Click, AddressOf MyCancelButton_Click

            'Re-size the dialog if the prompt is too large
            Dim labelGraphics As Graphics = Label.CreateGraphics
            Dim labelSizeNeeded As SizeF = labelGraphics.MeasureString(prompt, Label.Font, Label.Width)
            labelGraphics.Dispose()
            If labelSizeNeeded.Height > Label.Height Then
                'The current label size is not large enough to accommodate the prompt. We need
                '  to expand the label and the dialog, and move the textbox to make room.
                Dim dialogHeightChange As Integer = CInt(labelSizeNeeded.Height - Label.Height)
                Label.Height += dialogHeightChange
                TextBox.Top += dialogHeightChange
                Height += dialogHeightChange
            End If

            'Position the form
            If (xPos = -1) AndAlso (yPos = -1) Then
                StartPosition = FormStartPosition.CenterScreen
            Else
                If xPos = -1 Then xPos = 600
                If yPos = -1 Then yPos = 350
                StartPosition = FormStartPosition.Manual
                DesktopLocation = New Point(xPos, yPos)
            End If
        End Sub

        Private Sub MyCancelButton_Click(sender As Object, e As EventArgs)
            Close()
        End Sub

        Private Sub OKButton_Click(sender As Object, e As EventArgs)
            Output = TextBox.Text
            Close()
        End Sub

        Protected Overloads Overrides Sub Dispose(disposing As Boolean)
            If disposing Then
                components?.Dispose()
            End If
            MyBase.Dispose(disposing)
        End Sub

    End Class
End Namespace
