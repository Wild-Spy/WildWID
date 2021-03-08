Public Class frmMain
    Const NUMBER_OF_COLUMNS As Integer = 6
    Const TAG_ID_COLUMN_INDEX As Integer = 3 'first colum's index is zero

    Dim FullFilePaths() As String
    Dim FullFileLoggerID() As String

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnLoadFolder.Click
        'If dlgFolderBrowse.ShowDialog <> Windows.Forms.DialogResult.OK Then
        'Exit Sub
        'End If

        dlgFolderBrowse.SelectedPath = "E:\Wild Spy\Projects_Online\WS005 - WID Logger\4 - Dev Testing\Cheryl Test 1\WID test 3 data"

        FullFilePaths = System.IO.Directory.GetFiles(dlgFolderBrowse.SelectedPath, "*.csv")

        ReDim FullFileLoggerID(FullFilePaths.Length - 1)

        lstCSVFiles.Items.Clear()
        lstTags.Items.Clear()

        Dim i As Integer = 0
        For Each s As String In FullFilePaths
            Dim fileName As String = System.IO.Path.GetFileName(s)
            Dim loggerNumber As String = getLoggerIDFromFileName(fileName)

            FullFileLoggerID(i) = loggerNumber

            lstCSVFiles.Items.Add("Logger #" & loggerNumber & " (" & fileName & ")")

            i += 1
        Next

        If FullFilePaths.Length > 0 Then
            GroupBox2.Enabled = True
            GroupBox3.Enabled = False
        End If

    End Sub

    Private Function getLoggerIDFromFileName(fN As String) As Integer
        Const SEARCH_STR As String = "WSLog"
        Const SEARCH_STR_END As String = "_"
        Dim a As Integer = InStr(fN, SEARCH_STR) + SEARCH_STR.Length - 1
        If a = -1 Then Return ""
        Dim b As Integer = fN.IndexOf(SEARCH_STR_END, a)
        If b = -1 Then Return ""

        Dim retstr As String = fN.Substring(a, b - a)

        Try
            Return retstr
        Catch ex As Exception
            Return ""
        End Try

    End Function

    Private Sub btnFindUniqueTags_Click(sender As Object, e As EventArgs) Handles btnFindUniqueTags.Click

        'open each file, search for tags add to list if tag not exist
        Dim fullTagList As New List(Of String)

        'File Example...
        'RecordNumber,Date,Time,TagID,Flags,RSSI
        '7047,04/03/2014,14:51:13,30000016,Start,-98
        '7048,04/03/2014,14:52:03,30000016,Start,-97
        '7049,04/03/2014,14:53:03,30000013,Start,-98

        lstTags.Items.Clear()
        lstTags.BeginUpdate()

        For Each s As String In FullFilePaths
            Dim fileLines() As String = My.Computer.FileSystem.ReadAllText(s).Replace(vbCr, "").Split(vbLf)

            For Each l As String In fileLines
                Dim cols() As String = l.Split(",")
                If cols.Length = NUMBER_OF_COLUMNS Then
                    Try
                        Dim tmpint As Integer = CInt(cols(0)) 'abort if first column 'RecordNumber' is not an integer

                        If IsValueInList(cols(TAG_ID_COLUMN_INDEX), fullTagList) = False Then
                            fullTagList.Add(cols(TAG_ID_COLUMN_INDEX))

                            Dim sss As String = cols(TAG_ID_COLUMN_INDEX)
                            If sss Like "300000##" Then
                                lstTags.Items.Add(cols(TAG_ID_COLUMN_INDEX), True)
                            Else
                                lstTags.Items.Add(cols(TAG_ID_COLUMN_INDEX), False)
                            End If
                        End If

                    Catch ex As Exception

                    End Try
                End If
            Next


        Next

        lstTags.EndUpdate()

        If lstTags.Items.Count > 0 Then
            GroupBox3.Enabled = True
        End If

    End Sub

    Public Function IsValueInList(value As String, list As List(Of String)) As Boolean
        Dim tmp As String

        For Each tmp In list
            If value = tmp Then
                Return True
            End If
        Next
        Return False
    End Function


    Private Sub btnOutputTagFiles_Click(sender As Object, e As EventArgs) Handles btnOutputTagFiles.Click
        'want to go through every output file one at a time.
        'for each output file we go through each source file and create an entry in a list with all of the 
        'info from the source file plus the logger number
        'then we sort this file
        'then we save this file

        Dim checkedCount As Integer = 0
        pb1.Enabled = True
        pb1.Visible = True
        pb1.Value = 0
        pb1.Maximum = lstTags.CheckedItems.Count

        If My.Computer.FileSystem.DirectoryExists(dlgFolderBrowse.SelectedPath & "\Tag File Outputs") = False Then
            My.Computer.FileSystem.CreateDirectory(dlgFolderBrowse.SelectedPath & "\Tag File Outputs")
        End If

        For i = 0 To lstTags.Items.Count - 1
            If lstTags.GetItemChecked(i) = True Then
                'this is a file we want to output

                Dim OutputFileLineList As New List(Of String)
                'OutputFileLineList.Add("LoggerNumber,RecordNumber,Date,Time,Flags,RSSI")
                For j = 0 To FullFilePaths.Length - 1
                    Dim inFile As String = FullFilePaths(j)

                    Dim fileLines() As String = My.Computer.FileSystem.ReadAllText(inFile).Replace(vbCr, "").Split(vbLf)

                    For Each l As String In fileLines
                        Dim cols() As String = l.Split(",")
                        If cols.Length = NUMBER_OF_COLUMNS Then

                            If cols(TAG_ID_COLUMN_INDEX) = lstTags.Items(i).ToString Then
                                Dim tmpLine As String = FullFileLoggerID(j) & "," & cols(0) & "," & cols(1) & "," & cols(2) & "," & cols(4) & "," & cols(5)
                                OutputFileLineList.Add(tmpLine)
                                'getTimeFromOutputFileLine(tmpLine)
                                'getTimeFromOutputFileLine("6,19580,04/03/2014,17:08:50,Start,-99")

                            End If

                        End If
                        Application.DoEvents()
                    Next
                Next

                'Now Sort The File By Date/Time
                OutputFileLineList.Sort(New sortOutputFileDateTimeHelper)

                Dim objWriter As New System.IO.StreamWriter(dlgFolderBrowse.SelectedPath & "\Tag File Outputs\Tag_" & lstTags.Items(i).ToString & ".csv", False)

                objWriter.WriteLine("LoggerNumber,RecordNumber,Date,Time,Flags,RSSI")

                For Each tmps As String In OutputFileLineList
                    objWriter.WriteLine(tmps)
                Next

                objWriter.Close()

                'Dim outputfile As String = "LoggerNumber,RecordNumber,Date,Time,Flags,RSSI" & vbNewLine
                'For Each tmps As String In OutputFileLineList
                '    outputfile &= tmps & vbNewLine
                'Next

                'My.Computer.FileSystem.WriteAllText(dlgFolderBrowse.SelectedPath & "\Tag File Outputs\Tag_" & lstTags.Items(i).ToString & ".csv", outputfile, False)

                checkedCount += 1
                pb1.Value = checkedCount
                Application.DoEvents()
            End If
        Next

    End Sub

    Public Shared Function sortOutputFileDateTime() As IComparer(Of String)
        Return CType(New sortOutputFileDateTimeHelper(), IComparer)
    End Function

End Class

Public Class sortOutputFileDateTimeHelper
    Implements IComparer(Of String)

    Function Compare(ByVal x As String, ByVal y As String) As Integer Implements IComparer(Of String).Compare

        Dim t1 As DateTime = getTimeFromOutputFileLine(x)
        Dim t2 As DateTime = getTimeFromOutputFileLine(y)

        If t1 < t2 Then
            Return -1
        End If

        If t1 > t2 Then
            Return 1
        Else
            Return 0
        End If

    End Function

    Private Function getTimeFromOutputFileLine(line As String) As DateTime
        Dim parts() As String = line.Split(",")

        Dim TimeStr As String = parts(2).Trim & " " & parts(3).Trim

        Dim dt As DateTime = DateTime.ParseExact(TimeStr, "dd/MM/yyyy HH:mm:ss", Nothing)

        Return dt

    End Function

End Class