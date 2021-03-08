Public Class Form1

    Private Function IsIDInListList(ByRef listlist As List(Of RecordList), ByVal ID As String) As Integer
        Dim i As Integer

        For i = 0 To listlist.Count - 1
            If listlist(i).ID = ID Then
                Return i
            End If
        Next

        Return -1

    End Function

    Private Function parseText(ByVal text As String, ByVal newlineChar As String, ByVal FieldSeperationChar As String, ByVal fieldCount As Integer, ByVal timeCol As Integer, ByVal dateCol As Integer, ByVal HDOPCol As Integer) As String


        'structures needed:
        'OldRecord (DateTime, ID)
        'NewRecord (StartTime, StopTime, Duration, ID)
        'RecordList (ID, List of OldRecord, List of NewRecord)
        Dim lines() As String
        Dim fields() As String
        Dim restofLineStr As String
        Dim oldlist As New List(Of OldRecord)
        Dim listList As New List(Of RecordList)
        Dim newEntryflag As Boolean = True
        Dim maxGapBetweenLogs As TimeSpan = New TimeSpan(0, 0, 5 * 10)
        Dim retStr As String = ""

        Dim i, j, k As Integer

        FieldSeperationChar = ","

        'split into lines
        lines = text.Split(newlineChar)

        'generate one long list
        For i = 0 To lines.Length - 1
            Try
                fields = lines(i).Split(FieldSeperationChar)
                restofLineStr = ""
                For j = 3 To fields.Length - 1
                    restofLineStr &= FieldSeperationChar & fields(j)
                Next
                oldlist.Add(New OldRecord(DateTime.Parse(fields(0) & " " & fields(1)), fields(2), restofLineStr))
            Catch ex As Exception

            End Try

        Next

        'split list into sublists, one per tag
        For i = 0 To oldlist.Count - 1
            j = IsIDInListList(listList, oldlist(i).ID)
            If j = -1 Then
                'no list for current tag exists, create one.
                listList.Add(New RecordList(oldlist(i).ID))
                'also add this tag to that list
                listList.Last.OldRecList.Add(New OldRecord(oldlist(i).DateTime, oldlist(i).ID, oldlist(i).RestOfLine))
            Else
                listList(j).OldRecList.Add(New OldRecord(oldlist(i).DateTime, oldlist(i).ID, oldlist(i).RestOfLine))
            End If
        Next

        'order sublists by date/time
        For j = 0 To listList.Count - 1
            listList(j).OldRecList.Sort(New OldRecordComparer)
            'listList(j).OldRecList.Sort(Function(x As OldRecord, y As OldRecord) x.DateTime.CompareTo(y.DateTime))
        Next

        'for each sublist
        newEntryflag = True
        For j = 0 To listList.Count - 1
            listList(j).NewRecList.Add(New NewRecord(listList(j).OldRecList(0).DateTime, listList(j).OldRecList(0).DateTime, listList(j).OldRecList(0).ID, listList(j).OldRecList(0).RestOfLine))
            For i = 1 To listList(j).OldRecList.Count - 1


                If listList(j).OldRecList(i).DateTime <= listList(j).NewRecList.Last.StopTime + maxGapBetweenLogs Then
                    'if the next log is within (5*10) seconds of the first, modify the stopTime to this time
                    listList(j).NewRecList.Last.StopTime = listList(j).OldRecList(i).DateTime
                Else
                    'if not.. create a new newLog entry
                    listList(j).NewRecList.Add(New NewRecord(listList(j).OldRecList(i).DateTime, listList(j).OldRecList(i).DateTime, listList(j).OldRecList(i).ID, listList(j).OldRecList(i).RestOfLine))
                End If
            Next
        Next

        'go through, calculate all periods
        For j = 0 To listList.Count - 1
            For i = 0 To listList(j).NewRecList.Count - 1
                With listList(j).NewRecList(i)
                    .Duration = .StopTime - .StartTime
                End With
            Next
        Next

        'convert to text (csv format)
        retStr = "START DATE,START TIME,STOP DATE,STOP TIME,PICKUP DURATION,WID,OTHER FIELDS" & vbNewLine
        For j = 0 To listList.Count - 1
            For i = 0 To listList(j).NewRecList.Count - 1
                With listList(j).NewRecList(i)
                    retStr &= .StartTime.ToString("dd/MM/yyyy") & "," & .StartTime.ToString("HH:mm:ss") & "," _
                            & .StopTime.ToString("dd/MM/yyyy") & "," & .StopTime.ToString("HH:mm:ss") & "," _
                            & .Duration.Hours & ":" & .Duration.Minutes & ":" & .Duration.Seconds & "," _
                            & .ID _
                            & .RestOfLine & vbNewLine
                End With
            Next
        Next


        Return retStr

    End Function
    Private Sub btnParse_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)


    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        OFD1.Multiselect = False
        If (OFD1.ShowDialog = vbOK) Then
            Dim result As String = ""
            Dim i, j As Integer
            Dim sepChars As Integer = 2
            Dim NLChars As Integer = 3
            Dim newLineChar(NLChars) As String
            Dim sepChar(sepChars) As String
            newLineChar(0) = vbNewLine
            newLineChar(1) = vbLf
            newLineChar(2) = vbCr
            sepChar(0) = vbTab
            sepChar(1) = " "



            'For i = 0 To NLChars - 1
            'For j = 0 To sepChars - 1
            Try
                result = parseText(My.Computer.FileSystem.ReadAllText(OFD1.FileName), newLineChar(i), sepChar(j), 7, 1, 2, 6)
            Catch ex As Exception
                MsgBox("Error Parsing: " & vbNewLine & ex.InnerException.ToString, MsgBoxStyle.Critical)
            End Try
            'If result <> "" Then
            '    Exit For
            'End If
            'Next
            'Next

            If result <> "" Then
                txtOUT.Text = result
            End If

        End If
    End Sub

    Private Function ConvertTextWithInputsFromGUI(ByVal filename As String, ByVal newLineChar As String) As String

        Dim sepchar As String

        If txtSepChar.Text.ToLower = "[tab]" Then
            sepchar = vbTab
        Else
            sepchar = txtSepChar.Text
        End If

        Return parseText(My.Computer.FileSystem.ReadAllText(filename), newLineChar, sepchar, nudCols.Value, nudTimeCol.Value, nudDateCol.Value, nudHDOPNo.Value)
    End Function

    Private Sub btnBatchConvert_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnBatchConvert.Click
        OFD1.Multiselect = True
        If (OFD1.ShowDialog = vbOK) Then
            Dim result As String = ""
            Dim i, j As Integer
            Dim sepChars As Integer = 2
            Dim NLChars As Integer = 3
            Dim newLineChar(NLChars) As String
            Dim sepChar(sepChars) As String
            newLineChar(0) = vbNewLine
            newLineChar(1) = vbLf
            newLineChar(2) = vbCr
            sepChar(0) = vbTab
            sepChar(1) = " "

            txtOUT.Text = ""
            txtIN.Text = "Batch Operation..."

            Dim newFolderName As String = "Parsed"

            Dim fileName As String

            For Each fileName In OFD1.FileNames
                For i = 0 To NLChars - 1
                    '    For j = 0 To sepChars - 1
                    Try
                        result = ConvertTextWithInputsFromGUI(fileName, newLineChar(i))
                        '            result = parseText(My.Computer.FileSystem.ReadAllText(fileName), newLineChar(i), sepChar(j), 7, 1, 2, 6)
                    Catch ex As Exception

                    End Try
                    If result <> "" Then
                        Exit For
                    End If
                    '    Next
                Next
                result = ConvertTextWithInputsFromGUI(fileName, newLineChar(0))

                If result <> "" Then
                    Dim newDirName As String = My.Computer.FileSystem.GetFileInfo(fileName).Directory.FullName & "\" & newFolderName
                    Dim newFileName As String = newDirName & "\" & My.Computer.FileSystem.GetFileInfo(fileName).Name
                    If Not My.Computer.FileSystem.DirectoryExists(newDirName) Then
                        My.Computer.FileSystem.CreateDirectory(newDirName)
                    End If
                    If My.Computer.FileSystem.FileExists(newFileName) Then
                        Dim res As DialogResult = MsgBox("Do you want to overwrite this file?" & vbNewLine & newFileName, MsgBoxStyle.YesNoCancel, "Overwrite?")
                        If res = Windows.Forms.DialogResult.Yes Then
                            My.Computer.FileSystem.WriteAllText(newFileName, result, False)
                            txtOUT.Text &= "G File '" & My.Computer.FileSystem.GetFileInfo(fileName).Name & "' was converted!  New file in subfolder '" & newFolderName & "' overwrote old version." & vbNewLine
                        ElseIf res = Windows.Forms.DialogResult.No Then
                            txtOUT.Text &= "X File '" & My.Computer.FileSystem.GetFileInfo(fileName).Name & "' was chosen not to be overwritten." & vbNewLine
                        ElseIf res = Windows.Forms.DialogResult.Cancel Then
                            Exit Sub
                        End If
                    Else
                        My.Computer.FileSystem.WriteAllText(newFileName, result, False)
                        txtOUT.Text &= "G File '" & My.Computer.FileSystem.GetFileInfo(fileName).Name & "' was converted!  New file in subfolder '" & newFolderName & "'." & vbNewLine
                    End If
                Else
                    txtOUT.Text &= "X File '" & My.Computer.FileSystem.GetFileInfo(fileName).Name & "' could not be converted." & vbNewLine
                End If
            Next



        End If
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim msg As String = "To use the Batch feature:" & vbNewLine & _
                            "1. Click the 'Batch' button" & vbNewLine & _
                            "2. Select text files that you want converted" & vbNewLine & _
                            "3. Click open" & vbNewLine & _
                            "4. If any file already exists a dialog will ask you if you want to overwrite or not" & vbNewLine & _
                            "5. Results are displayed in the 'Output' box in the main screen." & vbNewLine & _
                            "      - Each line gives info on the conversion of a single file." & vbNewLine & _
                            "6. The new (converted) file can be found in 'oldfileDirectory/Parsed/oldfilename' where:" & vbNewLine & _
                            "      - 'oldfileDirectory' = the directory of the files to be parsed" & vbNewLine & _
                            "      - 'Parsed' = the name of the new folder where all the parsed files will be " & vbNewLine & _
                            "        (within the directory where the old files are stored)" & vbNewLine & _
                            "      - 'oldfilename' = the new file name within the new filder is the same as " & vbNewLine & _
                            "        the old file name in the old folder." & vbNewLine & _
                            "" & vbNewLine & _
                            "You can also do a single file conversion by clicking 'Open', selecting a file to parse" & vbNewLine & _
                            "and clicking open.  The converted text will appear in the 'Output' box in the" & vbNewLine & _
                            "program but will not be saved to a file"

        MsgBox(msg)


    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
End Class

Public Class OldRecord
    Implements IComparable(Of OldRecord)

    Public DateTime As DateTime
    Public ID As String
    Public RestOfLine As String

    Public Sub New(ByVal _DateTime As DateTime, ByVal _ID As String, Optional ByVal _RestOfLine As String = "")
        DateTime = _DateTime
        ID = _ID
        RestOfLine = _RestOfLine
    End Sub

    Public Sub New()

    End Sub

    Public Function CompareTo(ByVal other As OldRecord) As Integer Implements System.IComparable(Of OldRecord).CompareTo
        Return DateTime.CompareTo(other.DateTime)
    End Function
End Class

Public Class OldRecordComparer
    Implements IComparer(Of OldRecord)

    Public Function Compare(ByVal x As OldRecord, ByVal y As OldRecord) As Integer Implements System.Collections.Generic.IComparer(Of OldRecord).Compare
        Return x.DateTime.CompareTo(y.DateTime)
    End Function
End Class

Public Class NewRecord
    Public StartTime As DateTime
    Public StopTime As DateTime
    Public Duration As TimeSpan
    Public ID As String
    Public RestOfLine As String

    Public Sub New(ByVal _startT As DateTime, ByVal _stopT As DateTime, ByVal _ID As String, Optional ByVal _RestOfLine As String = "")
        StartTime = _startT
        StopTime = _stopT
        ID = _ID
        RestOfLine = _RestOfLine
    End Sub

    Public Sub New()

    End Sub
End Class

Public Class RecordList
    Public ID As String
    Public OldRecList As New List(Of OldRecord)
    Public NewRecList As New List(Of NewRecord)

    Public Sub New(ByVal _ID As String)
        ID = _ID
    End Sub

    Public Sub New()

    End Sub
End Class