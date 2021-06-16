Public Class Form1
    Private Sub TextBox1_IdleTextChanged(sender As Object, e As EventArgs) Handles TextBox1.IdleTextChanged
        Label1.Text = TextBox1.IdleText
    End Sub
End Class
