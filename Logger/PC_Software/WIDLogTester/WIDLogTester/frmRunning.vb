Imports System.Windows.Forms

Public Class frmRunning

    Public parentFrm As FrmProg

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        'Form1.Text = "Run"
        'Form1.proc.Kill()
        Me.Close()
    End Sub

    Private Sub frmRunning_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        If parentFrm.proc.HasExited = False Then
            parentFrm.Text = "Run"
            parentFrm.proc.Kill()
        End If
    End Sub

    Private Sub frmRunning_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class
