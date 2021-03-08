Imports System.IO

Public Class FrmProg

    Public proc As Process
    Private myErrorList As AVRErrorList

    Public Function GenArgs() As String
        Dim lvi As ListViewItem
        Dim Args As String = ""

        Args = " -c " & cbProgrammer.Text & " "
        Args &= "-p " & cbChip.Text & " "
        Args &= "-P " & txtPort.Text & " "

        For Each lvi In lvOperations.Items
            If lvi.Text.StartsWith("-") Then
                Args &= lvi.Text & " " & lvi.SubItems(1).Text & " "
            Else
                Args &= genCmdLineParam(lvi)
            End If
        Next

        Return Args

    End Function

    Public Function genCmdLineParam(ByVal lvi As ListViewItem) As String
        Dim locStr As String
        locStr = lvi.SubItems(2).Text.Replace("&&APP&&", AppDomain.CurrentDomain.BaseDirectory())
        Return "-U " & lvi.Text & ":" & lvi.SubItems(1).Text & ":" & locStr & ":" & lvi.SubItems(3).Text & " "
    End Function

    Public Function RunProg(Optional ByVal x As Integer = -1, Optional ByVal y As Integer = -1) As Integer
        If btnRun.Text = "Cancel" Then
            proc.Kill()
            Return -2
        Else
            Dim retVal As Integer
            ' Set start information.
            Dim start_info As New ProcessStartInfo("avrdude")
            start_info.Arguments = GenArgs() '" -c dragon_pdi -p x32d4 -P usb -U fuse1:w:0xFF:m -U fuse2:w:0x00:m -U fuse4:w:0xFE:m -U fuse5:w:0xFF:m"
            start_info.UseShellExecute = False
            start_info.CreateNoWindow = True
            start_info.RedirectStandardOutput = True
            start_info.RedirectStandardError = True
            start_info.WorkingDirectory = txtWorkingDir.Text.Replace("&&APP&&", AppDomain.CurrentDomain.BaseDirectory())

            ' Make the process and set its start information.
            proc = New Process
            proc.StartInfo = start_info

            ' Start the process.
            proc.Start()

            ' Attach to stdout and stderr.
            Dim std_out As StreamReader = proc.StandardOutput()
            Dim std_err As StreamReader = proc.StandardError()

            ' Display the results.
            Dim startTime As DateTime = DateTime.Now()
            Dim endTime As DateTime = DateTime.Now()
            Dim tmpTime As DateTime
            Dim elapsedTime As TimeSpan
            Dim mFrm As New frmRunning

            mFrm.parentFrm = Me
            mFrm.Show(Me)
            If x = -1 Then x = Me.Left + Me.Width / 2 - mFrm.Width / 2
            If y = -1 Then y = Me.Top + Me.Height / 2 - mFrm.Height / 2
            mFrm.Top = y
            mFrm.Left = x

            endTime = endTime.AddSeconds(30)
            btnRun.Text = "Cancel"
            While proc.HasExited = False And DateTime.Now < endTime
                Application.DoEvents()
                elapsedTime = Now() - startTime
                tmpTime = New DateTime(elapsedTime.Ticks)
                mFrm.lblRunning.Text = tmpTime.ToString("mm:ss.ff")
            End While
            btnRun.Text = "Run"

            If DateTime.Now >= endTime Then
                proc.Kill()
            End If

            mFrm.Close()
            Application.DoEvents()
            elapsedTime = Now() - startTime

            txtStdout.Text = std_out.ReadToEnd()
            txtStderr.Text = std_err.ReadToEnd()

            ' Clean up.
            std_out.Close()
            std_err.Close()
            proc.Close()

            'Now parse everything...

            Dim Lines() As String
            Dim Line As String
            Dim i, j, k As Integer
            Dim Ops As New List(Of AVRDudeOperation)
            Dim CritError As Boolean = False
            Dim ErrorList As New List(Of String)

            myErrorList = New AVRErrorList

            Ops.Add(New AVRDudeOperation("init"))

            For i = 0 To lvOperations.Items.Count - 1
                Try
                    If Not (lvOperations.Items(i).SubItems(2).Text = "" Or lvOperations.Items(i).SubItems(3).Text = "" Or lvOperations.Items(i).SubItems(0).Text.StartsWith("-")) Then
                        Ops.Add(New AVRDudeOperation(lvOperations.Items(i)))
                    End If
                Catch ex As Exception

                End Try
            Next

            Line = txtStderr.Text
            Line = Line.Replace(vbCr, "")
            Lines = Line.Split(vbLf)

            i = 0
            j = 0
            While i < Lines.Count
                'find first line mode
                If j >= Ops.Count Then Exit While
                Do
                    Line = Lines(i)
                    Line = Line.Trim()
                    If Line.StartsWith("avrdude:") Then Line = Line.Replace("avrdude: ", "")

                    If Line Like Ops(j).SubOperations(0).Identifier Then Exit Do
                    If myErrorList.LikeInList(Line) = True Then
                        ErrorList.Add(Line)
                        CritError = True
                    End If
                    i += 1
                    If i >= Lines.Count Then Exit While
                Loop While i < Lines.Count
                'found the first line, now process it and all others
                k = 0
                While k < Ops(j).SubOperations.Count And i < Lines.Count
                    Line = Lines(i)
                    Line = Line.Trim()
                    If Line.StartsWith("avrdude:") Then Line = Line.Replace("avrdude: ", "")

                    If Line <> "" Then
                        If myErrorList.LikeInList(Line) = True Then
                            ErrorList.Add(Line)
                            CritError = True
                            Exit While
                        Else
                            If k >= Ops(j).SubOperations.Count Then Exit While
                            If Ops(j).SubOperations(k).TestSubOp(Line) = False Then
                                ErrorList.Add(Line)
                                CritError = True
                            End If
                        End If
                        k += 1
                    End If

                    i += 1
                End While
                j += 1
            End While

            For i = 0 To Ops.Count - 1
                Ops(i).genFinishedInfo()
                If Ops(i).Result = "FAILED" Then
                    CritError = True
                    Exit For
                End If
            Next

            txtStdout.Text = ""

            If CritError = True Then
                txtStdout.Text &= "ERROR!" & vbNewLine
                For i = 0 To ErrorList.Count - 1
                    txtStdout.Text &= "Error: " & ErrorList(i) & vbNewLine
                Next
                retVal = -1
            Else
                txtStdout.Text &= "SUCCESSFUL" & vbNewLine
                retVal = 0
            End If

            txtStdout.Text &= "Time taken: " & tmpTime.ToString("mm:ss.ff") & vbNewLine & vbNewLine

            For i = 0 To Ops.Count - 1
                txtStdout.Text &= Ops(i).genFinishedInfo & vbNewLine & vbNewLine
            Next

            Return retVal

        End If
    End Function

    Private Sub btnRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRun.Click
        RunProg()
    End Sub

    Private Sub Label4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label4.Click
        MsgBox(rtbFormatInfo.Text)
    End Sub

    Private Sub btnAddOperation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddOperation.Click
        Dim lvi As New ListViewItem()

        lvi.Text = cbMemory.Text
        lvi.SubItems.Add(cbOperation.Text)
        lvi.SubItems.Add(txtFileName.Text)
        lvi.SubItems.Add(cbFormat.Text)

        lvOperations.Items.Add(lvi)

        'Testing...
        'Dim n As New AVRDudeOperation(lvi)
        'n.SubOperations(0).ExtractFields("reading input file ""0xFF""")
        'n.SubOperations(1).ExtractFields("writing fuse1 (1 bytes):")
        'n.SubOperations(2).ExtractFields("Writing | ################################################## | 100% 0.06s")
        'n.SubOperations(3).ExtractFields("1 bytes of fuse1 written")
        'n.SubOperations(4).ExtractFields("verifying fuse1 memory against 0xFF:")


    End Sub

    Private Sub btnDeleteOperation_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDeleteOperation.Click
        If lvOperations.SelectedIndices.Count > 0 Then
            lvOperations.Items.RemoveAt(lvOperations.SelectedIndices(0))
        End If
    End Sub

    Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        Dim SAVESTR As String = ""
        Dim lvi As ListViewItem

        SAVESTR = "-c " & cbProgrammer.Text & vbCrLf
        SAVESTR &= "-p " & cbChip.Text & vbCrLf
        SAVESTR &= "-P " & txtPort.Text & vbCrLf
        SAVESTR &= "-w " & txtWorkingDir.Text & vbCrLf

        For Each lvi In lvOperations.Items
            SAVESTR &= lvi.SubItems(0).Text & vbTab & lvi.SubItems(1).Text & vbTab & lvi.SubItems(2).Text & vbTab & lvi.SubItems(3).Text & vbCrLf
        Next

        If SFD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Dim bAns As Boolean = False
            Dim objReader As StreamWriter

            Try
                objReader = New StreamWriter(SFD.FileName)
                objReader.Write(SAVESTR)
                objReader.Close()
            Catch Ex As Exception
                MsgBox("ERROR WRITING TO FILE:" & vbNewLine & Ex.Message, MsgBoxStyle.Critical, "ERROR!")
            End Try
        End If
    End Sub

    Public Sub LoadFile(ByVal FileName As String)
        Dim LOADSTR As String = ""
        Dim strLines() As String
        Dim strItems() As String
        Dim tmpLine As String
        Dim lvi As ListViewItem
        Dim objReader As StreamReader

        Try
            objReader = New StreamReader(FileName)
            LOADSTR = objReader.ReadToEnd()
            objReader.Close()
        Catch Ex As Exception
            MsgBox("ERROR READING FILE:" & vbNewLine & Ex.Message, MsgBoxStyle.Critical, "ERROR!")
        End Try

        'LOADSTR
        lvOperations.Items.Clear()
        LOADSTR = LOADSTR.Replace(vbCr, "")
        strLines = LOADSTR.Split(vbLf)

        For Each tmpLine In strLines
            tmpLine = tmpLine.Trim()
            If tmpLine = "" Then
                'do nothing!
            ElseIf tmpLine.StartsWith("-c") Then
                cbProgrammer.Text = tmpLine.Split(" ")(1)
            ElseIf tmpLine.StartsWith("-p") Then
                cbChip.Text = tmpLine.Split(" ")(1)
            ElseIf tmpLine.StartsWith("-P") Then
                txtPort.Text = tmpLine.Split(" ")(1)
            ElseIf tmpLine.StartsWith("-w") Then
                txtWorkingDir.Text = tmpLine.Substring(3)
            Else
                strItems = tmpLine.Split(vbTab)
                lvi = New ListViewItem(strItems, 0)
                lvOperations.Items.Add(lvi)
            End If
        Next
    End Sub

    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        Dim LOADSTR As String = ""
        Dim strLines() As String
        Dim strItems() As String
        Dim tmpLine As String
        Dim lvi As ListViewItem

        If OFD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            Dim objReader As StreamReader
            Try
                objReader = New StreamReader(OFD.FileName)
                LOADSTR = objReader.ReadToEnd()
                objReader.Close()
            Catch Ex As Exception
                MsgBox("ERROR READING FILE:" & vbNewLine & Ex.Message, MsgBoxStyle.Critical, "ERROR!")
            End Try
        End If

        'LOADSTR
        lvOperations.Items.Clear()
        LOADSTR = LOADSTR.Replace(vbCr, "")
        strLines = LOADSTR.Split(vbLf)

        For Each tmpLine In strLines
            tmpLine = tmpLine.Trim()
            If tmpLine = "" Then
                'do nothing!
            ElseIf tmpLine.StartsWith("-c") Then
                cbProgrammer.Text = tmpLine.Split(" ")(1)
            ElseIf tmpLine.StartsWith("-p") Then
                cbChip.Text = tmpLine.Split(" ")(1)
            ElseIf tmpLine.StartsWith("-P") Then
                txtPort.Text = tmpLine.Split(" ")(1)
            ElseIf tmpLine.StartsWith("-w") Then
                txtWorkingDir.Text = tmpLine.Substring(3)
            Else
                strItems = tmpLine.Split(vbTab)
                lvi = New ListViewItem(strItems, 0)
                lvOperations.Items.Add(lvi)
            End If
        Next
    End Sub

    Private Sub FrmProg_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Me.Hide()
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        myErrorList = New AVRErrorList()
    End Sub
End Class

Class AVRDudeOperation

    Public CMDLnParam As String
    Public IgnoreMe As Boolean

    Dim Name As String

    Public Memory As String
    Public Operation As String
    Public FileName As String
    Public FileFormat As String

    Public SubOperations As New List(Of AVRDudeSubOperation)

    Public Result As String
    Public ErrorDesc As String

    Public Sub New(ByVal lvi As ListViewItem)
        CMDLnParam = FrmProg.genCmdLineParam(lvi)
        If lvi.Text.StartsWith("-") Then
            IgnoreMe = True
        Else
            IgnoreMe = False
        End If
        Memory = lvi.Text
        Operation = lvi.SubItems(1).Text
        FileName = lvi.SubItems(2).Text
        FileFormat = lvi.SubItems(3).Text

        Select Case Operation
            Case "r"

            Case "w"
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.readfile, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.writingchip, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.writingchipprog, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.writtenchip, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.verifyingstart, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.loaddata, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.filebytes, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.readingchip, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.readingchipprog, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.verifyingmid, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.verifiedchip, Me))
            Case "v"
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.verifyingstart, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.loaddata, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.filebytes, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.readingchip, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.readingchipprog, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.verifyingmid, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.verifiedchip, Me))
            Case "init"
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.AVRDudeinit, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.readingchipprog, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.AVRDevSig, Me))
            Case "-e"
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.erasechip, Me))
        End Select
    End Sub

    Public Sub New(ByVal type As String)
        Name = type
        Operation = type

        Select Case Name
            Case "init"
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.AVRDudeinit, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.readingchipprog, Me))
                SubOperations.Add(New AVRDudeSubOperation(SubOpName.AVRDevSig, Me))
        End Select

    End Sub

    Public Function genFinishedInfo(Optional ByVal extraInfo As Boolean = True) As String
        Dim retStr As String = ""
        'Dim failedAt As Integer

        Dim tmpSubOp As AVRDudeSubOperation

        Me.Result = "SUCCEEDED"
        Me.ErrorDesc = ""

        For Each tmpSubOp In SubOperations
            If tmpSubOp.WasError = True Then
                Me.Result = "FAILED"
                Me.ErrorDesc = tmpSubOp.ErrorInfo
                Exit For
            ElseIf tmpSubOp.WasShown = False Then
                Me.Result = "FAILED"
                Me.ErrorDesc = "Failed to execute instruction: " & [Enum].GetName(GetType(SubOpName), tmpSubOp.Name)
                Exit For
            End If
        Next

        Select Case Operation
            Case "r", "w", "v"
                retStr = "Operation: " & Me.CMDLnParam & vbCrLf
                retStr &= vbTab & "Memory Type: " & Me.Memory & vbCrLf
                retStr &= vbTab & "Operation Type: " & Me.op2str(Operation) & vbCrLf
                retStr &= vbTab & "File Name: " & Me.FileName & vbCrLf
                retStr &= vbTab & "File Format: " & Me.FF2str(Me.FileFormat) & vbCrLf
                retStr &= "Result: " & Me.Result & vbCrLf
            Case "init"
                retStr = "Operation: " & "Initialise" & vbCrLf
                retStr &= "Result: " & Me.Result & vbCrLf
            Case "-e"
                retStr = "Operation: " & "Erase Chip" & vbCrLf
                retStr &= "Result: " & Me.Result & vbCrLf
            Case Else

        End Select


        If Me.Result = "FAILED" Then
            retStr &= "Reason: " & Me.ErrorDesc & vbCrLf
            If extraInfo = True Then
                Select Case Operation
                    Case "r"

                    Case "w"
                        retStr &= vbTab & "Bytes in file: " & Me.SubOperations(1).Fields(1).ActualData & vbCrLf
                        retStr &= vbTab & "Bytes written: " & Me.SubOperations(3).Fields(0).ActualData & vbCrLf
                        retStr &= vbTab & "Percent written: " & Me.SubOperations(2).Fields(1).ActualData & "%" & vbCrLf
                        retStr &= vbTab & "Write time: " & Me.SubOperations(2).Fields(2).ActualData & "s" & vbCrLf
                        retStr &= vbTab & "Percent read: " & Me.SubOperations(8).Fields(1).ActualData & "%" & vbCrLf
                        retStr &= vbTab & "Read time: " & Me.SubOperations(8).Fields(2).ActualData & "s" & vbCrLf
                        retStr &= vbTab & "Bytes verified: " & Me.SubOperations(10).Fields(0).ActualData & vbCrLf
                    Case "v"

                    Case "init"
                        'nothing...
                    Case "-e"

                    Case Else

                End Select
            End If
        Else
            If extraInfo = True Then
                Select Case Operation
                    Case "r"

                    Case "w"
                        retStr &= vbTab & "Bytes in file: " & Me.SubOperations(1).Fields(1).ActualData & vbCrLf
                        retStr &= vbTab & "Bytes written: " & Me.SubOperations(3).Fields(0).ActualData & vbCrLf
                        retStr &= vbTab & "Percent written: " & Me.SubOperations(2).Fields(1).ActualData & "%" & vbCrLf
                        retStr &= vbTab & "Write time: " & Me.SubOperations(2).Fields(2).ActualData & "s" & vbCrLf
                        retStr &= vbTab & "Percent read: " & Me.SubOperations(8).Fields(1).ActualData & "%" & vbCrLf
                        retStr &= vbTab & "Read time: " & Me.SubOperations(8).Fields(2).ActualData & "s" & vbCrLf
                        retStr &= vbTab & "Bytes verified: " & Me.SubOperations(10).Fields(0).ActualData & vbCrLf
                    Case "v"
                    Case "init"
                        retStr &= vbTab & "Read time: " & Me.SubOperations(1).Fields(2).ActualData & "s" & vbCrLf
                        retStr &= vbTab & "Device signature: " & Me.SubOperations(2).Fields(0).ActualData & vbCrLf
                    Case "-e"

                    Case Else

                End Select
            End If
        End If


        Return retStr

    End Function

    Private Function op2str(ByVal op As String) As String
        Select Case op
            Case "r"
                Return "read"
            Case "w"
                Return "write"
            Case "v"
                Return "verify"
            Case "init"
                Return "initialise"
            Case "-e"
                Return "erase"
            Case Else
                Return op
        End Select
    End Function

    Private Function FF2str(ByVal op As String) As String
        Select Case op
            Case "i"
                Return "Intel hex"
            Case "s"
                Return "Motorola S-record"
            Case "r"
                Return "raw binary"
            Case "m"
                Return "immediate mode"
            Case "a"
                Return "auto detect"
            Case "d"
                Return "decimal"
            Case "h"
                Return "hexadecimal"
            Case "o"
                Return "octal"
            Case "b"
                Return "binary"
            Case Else
                Return op
        End Select
    End Function

End Class

Class AVRDudeSubOperation
    Public Name As Integer          'enumerated value of type SubOpName
    Public Identifier As String     'eg. reading input file
    Public WasError As Boolean
    Public ErrorInfo As String
    Public WasShown As Boolean = False
    Public Fields As New List(Of AVRDudeFields)    'eg. WilLogger.hex or 14374
    Public parent As AVRDudeOperation

    Public Sub New(ByVal type As String, ByRef pp As AVRDudeOperation)
        Name = type
        parent = pp
        Select Case Name
            Case SubOpName.readfile
                Identifier = "reading input file ""*"""
                Fields.Add(New AVRDudeFields("FileName", parent.FileName))
            Case SubOpName.writingchip
                Identifier = "writing * (* bytes):"
                Fields.Add(New AVRDudeFields("Memory", parent.Memory))
                Fields.Add(New AVRDudeFields("Bytes"))
            Case SubOpName.writingchipprog
                Identifier = "Writing | * | *% *s"
                Fields.Add(New AVRDudeFields("ProgBar", "##################################################"))
                Fields.Add(New AVRDudeFields("Percent", "100"))
                Fields.Add(New AVRDudeFields("Time"))
            Case SubOpName.writtenchip
                Identifier = "* bytes of * written"
                Fields.Add(New AVRDudeFields("Bytes"))
                Fields.Add(New AVRDudeFields("Memory", parent.Memory))
            Case SubOpName.verifyingstart
                Identifier = "verifying * memory against *:"
                Fields.Add(New AVRDudeFields("Memory", parent.Memory))
                Fields.Add(New AVRDudeFields("FileName", parent.FileName))
            Case SubOpName.loaddata
                Identifier = "load data * data from input file *:"
                Fields.Add(New AVRDudeFields("Memory", parent.Memory))
                Fields.Add(New AVRDudeFields("FileName", parent.FileName))
            Case SubOpName.filebytes
                Identifier = "input file * contains * bytes"
                Fields.Add(New AVRDudeFields("FileName", parent.FileName))
                Fields.Add(New AVRDudeFields("Bytes"))
            Case SubOpName.readingchip
                Identifier = "reading on-chip * data:"
                Fields.Add(New AVRDudeFields("Memory", parent.Memory))
            Case SubOpName.readingchipprog
                Identifier = "Reading | * | *% *s"
                Fields.Add(New AVRDudeFields("ProgBar", "##################################################"))
                Fields.Add(New AVRDudeFields("Percent", "100"))
                Fields.Add(New AVRDudeFields("Time"))
            Case SubOpName.verifyingmid
                Identifier = "verifying ..."
            Case SubOpName.verifiedchip
                Identifier = "* bytes of * verified"
                Fields.Add(New AVRDudeFields("Bytes"))
                Fields.Add(New AVRDudeFields("Memory", parent.Memory))
            Case SubOpName.AVRDudeinit
                Identifier = "AVR device initialized and ready to accept instructions"
            Case SubOpName.AVRDevSig
                Identifier = "Device signature = 0x*"
                Fields.Add(New AVRDudeFields("DevSig"))
            Case SubOpName.erasechip
                Identifier = "erasing chip"
        End Select
    End Sub

    Public Function TestSubOp(ByVal input As String) As Boolean
        Dim i As Integer
        WasError = False

        If Not input Like Identifier Then
            WasError = True
            ErrorInfo = "Incorrect Format"
            Return False
        End If

        Try
            ExtractFields(input)
        Catch ex As Exception
            WasError = True
            ErrorInfo = "Parsing Error"
            Return False
        End Try

        For i = 0 To Fields.Count - 1
            If Not Fields(i).ExpectedData = "**N/A**" Then
                If Fields(i).Name = "FileName" Then
                    If Fields(i).ExpectedData.Replace("""", "") <> Fields(i).ActualData Then
                        WasError = True
                        ErrorInfo = "Field " & Fields(i).Name & " data not as expected"
                        Return False
                    End If
                Else
                    If Fields(i).ExpectedData <> Fields(i).ActualData Then
                        WasError = True
                        ErrorInfo = "Field " & Fields(i).Name & " data not as expected"
                        Return False
                    End If
                End If

            End If
        Next

        WasShown = True
        Return True

    End Function

    Public Sub ExtractFields(ByVal input As String)
        Dim i As Integer
        Dim pos1, pos2, pos3 As Integer
        Dim tmpStr1, tmpstr2 As String
        Dim lastField As Boolean = False

        tmpStr1 = Identifier
        pos1 = InStr(Identifier, "*")
        If pos1 = 0 Then Exit Sub
        input = input.Substring(pos1 - 1)
        tmpStr1 = tmpStr1.Substring(pos1)

        For i = 0 To Fields.Count - 1
            pos2 = InStr(tmpStr1, "*")
            If pos2 = 0 Then
                pos2 = tmpStr1.Length + 1
                lastField = True
            End If
            tmpstr2 = Mid(tmpStr1, 1, pos2 - 1)
            pos3 = input.IndexOf(tmpstr2)
            If tmpStr1 = "" Then
                Fields(i).ActualData = Mid(input, 1, Len(input))
            Else
                Fields(i).ActualData = Mid(input, 1, pos3)
            End If

            If lastField = True Then
                Exit For
            Else
                input = input.Substring(pos3 + pos2 - 1)
                tmpStr1 = tmpStr1.Substring(pos2)
            End If
        Next
    End Sub
End Class

Class AVRDudeFields
    Public Name As String
    Public ExpectedData As String
    Public ActualData As String

    Public Sub New(ByVal aName As String, Optional ByVal eData As String = "**N/A**")
        Name = aName
        ExpectedData = eData
    End Sub
End Class

Enum SubOpName As Integer
    readfile = 0
    writingchip = 1
    writingchipprog = 2
    writtenchip = 3
    verifyingstart = 4
    loaddata = 5
    filebytes = 6
    readingchip = 7
    readingchipprog = 8
    verifyingmid = 9
    verifiedchip = 10
    AVRDudeinit = 11
    AVRDevSig = 12
    erasechip = 13
End Enum

Class AVRErrorList
    Public Errors As New List(Of String)

    Public Sub New()
        Errors.Add("usbdev_open(): did not find any USB device ""usb""")
        Errors.Add("jtagmkII_setparm(): bad response to set parameter command: *")
        Errors.Add("jtagmkII_close(): bad response to * command: *")
        Errors.Add("ERROR: address * out of range at line * of *")
        Errors.Add("write to file '*' failed")
        Errors.Add("ERROR: *")
        Errors.Add("jtagmkII_recv_frame(): timeout")
        Errors.Add("jtagmkII_getsync(): sign-on command: status -1")
        Errors.Add("jtagmkII_initialize(): part * has no * interface")
        Errors.Add("initialization failed, rc=-1")
        Errors.Add("Expected signature for * is *")
        Errors.Add("jtagmkII_setparm(): bad response to set parameter command: *")
        Errors.Add("jtagmkII_close(): timeout/error communicating with programmer (status *)")
        'Errors.Add("")
        'Errors.Add("")
        'Errors.Add("")
        'Errors.Add("")
        'Errors.Add("")
        'Errors.Add("")
        'Errors.Add("")
        'Errors.Add("")
    End Sub

    Public Function LikeInList(ByVal input As String) As Boolean
        Dim tmpStr As String

        For Each tmpStr In Errors
            If input Like tmpStr Then
                Return True
            End If
        Next

        Return False

    End Function
End Class