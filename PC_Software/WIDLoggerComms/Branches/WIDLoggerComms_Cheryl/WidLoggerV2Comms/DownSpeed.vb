Imports System.Windows.Forms

Public Class DownSpeed
    Public retVal As Integer = -1

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.OK
        retVal = cbBaudSelect.SelectedIndex
        Me.Close()
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub DownSpeed_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        cbBaudSelect.SelectedIndex = 0
    End Sub
End Class
