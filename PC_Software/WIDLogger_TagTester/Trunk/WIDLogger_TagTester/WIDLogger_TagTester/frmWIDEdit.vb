Public Class frmWIDEdit



    Private Sub mtbRFFilter_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles mtbWIDID.KeyDown
        If e.KeyCode > Keys.F AndAlso _
           e.KeyCode <= Keys.Z AndAlso _
           Not e.Control AndAlso _
           Not e.Alt Then
            'The user has pressed a letter key greater than F, which would be allowed by the mask, so reject it.
            e.SuppressKeyPress = True

            If Me.mtbWIDID.BeepOnError Then
                My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
            End If
        End If
    End Sub

    Private Sub btnSave_Click(sender As System.Object, e As System.EventArgs) Handles btnSave.Click
        If WIDIDValidator(mtbWIDID.Text) = False Then
            MsgBox("Invalid WID ID!  Press cancel to escape form.", vbOKOnly & vbExclamation, "Invalid ID")
            Return
        End If

        Me.DialogResult = Windows.Forms.DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As System.Object, e As System.EventArgs) Handles btnCancel.Click
        Me.DialogResult = Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Function WIDIDValidator(WID_ID As String) As Boolean

        Dim i As Integer
        Dim c As String

        For i = 0 To 8 - 1
            c = WID_ID.Substring(i, 1)

            If (c.ToLower >= "a" And c.ToLower <= "f") Or (c <= "9" And c >= "0") Then
                'valid!

            Else
                Return False
            End If
        Next

        Return True
    End Function
End Class