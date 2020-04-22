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

#Disable Warning IDE1006 ' Naming Styles, Justification:=<VBInputBox.resx depends on these names>
        Private ReadOnly components As Container
        Private TextBox As Windows.Forms.TextBox
        Private Label As Windows.Forms.Label
        Private OKButton As Windows.Forms.Button
        Private MyCancelButton As Windows.Forms.Button
#Enable Warning IDE1006 ' Naming Styles
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
                If Not (components Is Nothing) Then
                    components.Dispose()
                End If
            End If
            MyBase.Dispose(disposing)
        End Sub

        Private Sub InitializeComponent()
            Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(VBInputBox))
            OKButton = New Windows.Forms.Button
            MyCancelButton = New Windows.Forms.Button
            TextBox = New Windows.Forms.TextBox
            Label = New Windows.Forms.Label
            SuspendLayout()
            '
            'OKButton
            '
            resources.ApplyResources(OKButton, "OKButton", CultureInfo.CurrentUICulture)
            OKButton.Name = "OKButton"
            '
            'MyCancelButton
            '
            MyCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
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
            Label.Text = Prompt
            TextBox.Text = DefaultResponse
            AddHandler OKButton.Click, AddressOf OKButton_Click
            AddHandler MyCancelButton.Click, AddressOf MyCancelButton_Click

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

        Private Sub OKButton_Click(sender As Object, e As EventArgs)
            Output = TextBox.Text
            Close()
        End Sub

        Private Sub MyCancelButton_Click(sender As Object, e As EventArgs)
            Close()
        End Sub
    End Class

End Namespace
