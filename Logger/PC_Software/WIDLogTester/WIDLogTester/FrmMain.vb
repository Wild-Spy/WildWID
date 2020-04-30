Public Class FrmMain

    Dim CustomerName As List(Of String)
    Dim CustomerID As List(Of String)
    Dim RetVal As Integer
    Dim progFrm As FrmProg
    Dim cancelRun As Boolean = False

    Private Sub FrmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Dim ports() As String = System.IO.Ports.SerialPort.GetPortNames()

        Dim i As Integer

        For i = 0 To ports.Length - 1
            cbLogPort.Items.Add(ports(i))
        Next
        If ports.Count > 0 Then
            cbLogPort.SelectedIndex = 0
            If cbLogPort.Text <> "" Then serLog.PortName = cbLogPort.Text
        End If

        loadCodeVersionsIntoList()

        RTBSerLog.Top = RTBLog.Top
        RTBSerLog.Left = RTBLog.Left
        RTBProgLog.Top = RTBLog.Top
        RTBProgLog.Left = RTBLog.Left

        progFrm = New FrmProg

    End Sub

    Private Sub loadCodeVersionsIntoList()
        Dim files() As String
        Dim i As Integer

        files = System.IO.Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory(), "WIDLog_*.wspp")
        If files.Count > 0 Then
            cbCodeVersion.Items.Clear()
            For i = 0 To files.Count - 1
                files(i) = files(i).Split("\").Last
                files(i) = files(i).Substring(7, files(i).Length - 12)
                cbCodeVersion.Items.Add(files(i))
            Next
            cbCodeVersion.Text = files(0)
        End If

    End Sub

    Private Function isvalidhex(ByRef str As String) As Boolean
        For i = 1 To str.Length
            If Not (IsNumeric(str.Substring(i - 1, 1)) _
                Or str.Substring(i - 1, 1).ToLower() = "a" _
                Or str.Substring(i - 1, 1).ToLower() = "b" _
                Or str.Substring(i - 1, 1).ToLower() = "c" _
                Or str.Substring(i - 1, 1).ToLower() = "d" _
                Or str.Substring(i - 1, 1).ToLower() = "e" _
                Or str.Substring(i - 1, 1).ToLower() = "f") Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Sub btnGetPorts_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnGetPorts.Click

        Dim ports() As String = System.IO.Ports.SerialPort.GetPortNames()

        Dim i As Integer

        cbLogPort.Items.Clear()

        For i = 0 To ports.Length - 1
            cbLogPort.Items.Add(ports(i))
        Next
        If ports.Count > 0 Then
            cbLogPort.SelectedIndex = 0
            If cbLogPort.Text <> "" Then serLog.PortName = cbLogPort.Text
        End If
    End Sub

    Private Sub btnRUN_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRUN.Click

        If btnRUN.BackColor = Color.Red Then
            cancelRun = True
        Else
            Dim CritError As Boolean = False

            cancelRun = False
            RTBLog.Text = ""
            RTBSerLog.Text = ""
            RTBProgLog.Text = ""
            btnRUN.BackColor = Color.Red
            btnRUN.Text = "STOP"
            gbDevID.Enabled = False
            gbRFVars.Enabled = False
            chkProgram.Enabled = False
            chkSetVal.Enabled = False
            chkTest.Enabled = False
            Me.BackColor = btnShowSER.BackColor
            lblWarning.ForeColor = Color.Black

            If txtTestTagID.Text.Length <> 8 Or txtRFChan.Text.Length <> 3 Or _
                    txtRFID.Text.Length <> 10 Or txtTestRFID.Text.Length <> 10 Then
                txtAddtoLog("Error: Invalid configuration parameters entered" & vbCrLf)
                CritError = True
                GoTo Cleanup
            End If

            If chkTest.Checked = True Or chkSetVal.Checked = True Then
                Try
                    If serLog.IsOpen = False Then
                        serLog.BaudRate = 119200
                        serLog.PortName = cbLogPort.Text
                        serLog.Open()
                        cbLogPort.Enabled = False
                        lblLogStatus.Text = "Connected"
                        txtAddtoLog("Connected to Logger on '" & serLog.PortName & "' at " & serLog.BaudRate & " baud" & vbCrLf)
                    End If
                Catch ex As Exception
                    cbLogPort.Enabled = True
                    lblLogStatus.Text = "Not Connected"
                    txtAddtoLog("Error: Could not connect to Logger on '" & serLog.PortName & "'" & vbCrLf)
                    txtAddtoLog("Error: " & ex.Message & vbCrLf)
                    CritError = True
                    'MsgBox(ex.Message)
                    GoTo Cleanup
                End Try
                Application.DoEvents()
            End If

            'Program Code To MCU
            If chkProgram.Checked = True Then
                progFrm = New FrmProg
                progFrm.Hide()
                progFrm.LoadFile(AppDomain.CurrentDomain.BaseDirectory() & "WIDLog_" & cbCodeVersion.Text & ".wspp") '"\WIDLog.wspp")
                If progFrm.RunProg(Me.Left + Me.Width / 2 - frmRunning.Width / 2, Me.Top + Me.Height / 2 - frmRunning.Height / 2) = 0 Then
                    txtAddtoLog("SUCCESS: Logger Programmed Successfully!" & vbCrLf)
                Else
                    txtAddtoLog("Error: Error Programming Logger." & vbCrLf)
                    CritError = True
                    GoTo Cleanup
                End If

                RTBProgLog.Text = progFrm.txtStdout.Text
                RTBProgLog.SelectionStart = RTBProgLog.TextLength
                RTBProgLog.ScrollToCaret()

                progFrm.Hide()
            End If

            'Testing
            If chkTest.Checked = True Then
                Dim savedAddy As Integer

                'If chkSetVal.Checked = False And chkPreserveFlash.Checked = False Or _
                'chkSetVal.Checked = True And chkPreserveFlash.Checked = False And chkProgram.Checked = True Then
                If chkPreserveFlash.Checked = False Then
                    txtAddtoLog("TEST1: Flash Erase" & vbCrLf)
                    If Logger_EraseDataFlash() <> 0 Then
                        CritError = True
                        GoTo Cleanup
                    Else
                        txtAddtoLog("SUCCESS: TEST1 - Flash Erase PASSED." & vbCrLf)
                    End If
                ElseIf chkPreserveFlash.Checked = True Then
                    txtAddtoLog("TEST1: Flash Erase - SKIPPING, in preserve dataflash mode" & vbCrLf)
                Else
                    txtAddtoLog("TEST1: Flash Erase - SKIPPING, will be performed in 'Set Values'" & vbCrLf)
                End If


                txtAddtoLog("TEST2: RF Tag Pickup" & vbCrLf)
                If Logger_SetVerboseMode(3) <> 0 Then
                    CritError = True
                    GoTo Cleanup
                End If

                If Logger_SetRFVals(txtTestRFChan.Text, txtTestRFID.Text) <> 0 Then
                    CritError = True
                    GoTo Cleanup
                End If

                If Logger_ListenForTag(txtTestTagID.Text, txtTestTxPer.Text, savedAddy) <> 0 Then
                    CritError = True
                    GoTo Cleanup
                Else
                    txtAddtoLog("SUCCESS: TEST2 - RF Tag Pickup PASSED." & vbCrLf)
                End If

                txtAddtoLog("TEST3A: Flash Memory (valid entry)" & vbCrLf)
                If Logger_CompareFlashEntry(savedAddy, "*", txtTestTagID.Text) <> 0 Then
                    CritError = True
                    GoTo Cleanup
                Else
                    txtAddtoLog("SUCCESS: TEST3A - Flash Memory (valid entry) PASSED." & vbCrLf)
                End If

                txtAddtoLog("TEST3B: Flash Memory (erased entry)" & vbCrLf)
                If Logger_CompareFlashEntry(1234, "31-15-47 31:63:63", "FFFFFFFF") <> 0 Then
                    CritError = True
                    GoTo Cleanup
                Else
                    txtAddtoLog("SUCCESS: TEST3B - Flash Memory (erased entry) PASSED." & vbCrLf)
                End If

                txtAddtoLog("TEST4: RTC Test" & vbCrLf)
                If Logger_SyncTime(5) <> 0 Then
                    CritError = True
                    GoTo Cleanup
                Else
                    txtAddtoLog("SUCCESS: TEST4 - RTC Test PASSED." & vbCrLf)
                End If

            End If

            'Set Values
            If chkSetVal.Checked = True Then
                txtAddtoLog("CONFIGURING LOGGER:" & vbCrLf)
                txtAddtoLog("Setting RF Values... " & vbCrLf)
                If Logger_SetRFVals(txtRFChan.Text, txtRFID.Text) <> 0 Then
                    CritError = True
                    GoTo Cleanup
                End If
                txtAddtoLog("Setting RF Values... SUCCESSFUL" & vbCrLf)
                txtAddtoLog("Syncing time with computer..." & vbCrLf)
                If Logger_SyncTime(0) <> 0 Then
                    CritError = True
                    GoTo Cleanup
                End If
                txtAddtoLog("Syncing time with computer... SUCCESSFUL" & vbCrLf)
                If chkPreserveFlash.Checked = False Then
                    txtAddtoLog("Erasing Dataflash..." & vbCrLf)
                    If Logger_EraseDataFlash() <> 0 Then
                        CritError = True
                        GoTo Cleanup
                    End If
                    txtAddtoLog("Erasing Dataflash... SUCCESSFUL" & vbCrLf)
                End If
                txtAddtoLog("CONFIGURING LOGGER: ALL SUCCESSFUL!" & vbCrLf)
            End If

            If CritError = False Then
                txtAddtoLog("ALL ACTIONS COMPLETED SUCCESSFULLY!" & vbCrLf)
                Me.BackColor = Color.Green
                lblWarning.ForeColor = Color.Red
            End If

            'Yes, lables, you fricken bastards
Cleanup:
            If CritError = True Then
                Me.BackColor = Color.Red
                lblWarning.ForeColor = Color.Yellow
            End If

            'Close Serial Ports
            Try
                If serLog.IsOpen Then
                    serLog.Close()
                End If
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
            cbLogPort.Enabled = True
            lblLogStatus.Text = "Not Connected"
            Application.DoEvents()
            btnRUN.BackColor = Color.Green
            btnRUN.Text = "RUN"
            gbDevID.Enabled = True
            gbRFVars.Enabled = True
            chkProgram.Enabled = True
            chkSetVal.Enabled = True
            chkTest.Enabled = True
            If CritError = True And chkSaveLogs.Checked = True Then
                'save important information
                'saveLogs(CritError)
            End If
            saveLogs(CritError)
        End If
    End Sub

    Public Sub saveLogs(ByVal Failed As Boolean)
        Dim sw As System.IO.StreamWriter
        Dim p, n As String
        Dim i As Integer = 0
        Dim t1 As DateTime = Now

        p = AppDomain.CurrentDomain.BaseDirectory() & "\Logs"

        If Not IO.Directory.Exists(p) Then
            IO.Directory.CreateDirectory(p)
        End If

        n = IIf(Failed, "FAIL ", "PASS ") & " " & t1.ToString("dd-MM-yyyy hh.mm.ss") & ".log"

        If IO.File.Exists(p & "\" & n) Then
            Do
                n = IIf(Failed, "FAIL ", "PASS ") & " " & t1.ToString("DD-MM-YY hh.mm.ss") & "(" & i & ").log"
                i += 1
            Loop While IO.File.Exists(p & "\" & n)
        End If

        sw = New System.IO.StreamWriter(p & "\" & n)

        sw.WriteLine("Test Log:")
        sw.Write(RTBLog.Text.Replace(vbLf, vbCrLf))
        sw.WriteLine("")
        sw.WriteLine("")
        sw.WriteLine("Prog Log:")
        sw.Write(RTBProgLog.Text.Replace(vbLf, vbCrLf))
        sw.WriteLine("")
        sw.WriteLine("")
        sw.WriteLine("AVRDUDE Output:")
        sw.Write(progFrm.txtStderr.Text)
        sw.WriteLine("")
        sw.WriteLine("")
        sw.WriteLine("Logger Serial Log:")
        sw.Write(RTBSerLog.Text.Replace(vbLf, vbCrLf))

        sw.Close()

    End Sub

    Public Sub DoEventDelay(ByVal msDelay As Integer)
        Dim t1 As DateTime

        t1 = Now()
        t1 = t1.AddMilliseconds(msDelay)
        Do
            Application.DoEvents()
        Loop While Now() < t1

    End Sub

    Public Function Logger_SyncTime(ByVal waitSecs As Integer) As Integer
        Dim tmpInt, i As Integer
        Dim a As String
        Dim t1 As DateTime

        If Logger_AllignSer() = -1 Then
            txtAddtoLog("Error: Couldnt sync with logger" & vbCrLf)
            Return -2
        End If

        If tmpInt = Logger_SetTime(Now.ToString("ddMMyyHHmmss")) <> 0 Then
            Return tmpInt
        End If

        Logger_SendCommandFromSleep("t")

        If tmpInt = Logger_WaitForSleep() <> 0 Then
            Return tmpInt
        End If

        t1 = Now
        t1 = t1.AddSeconds(2)
        i = 0

        Do
            Try
                If serLog.BytesToRead > 0 Then
                    a = serLog.ReadLine()
                Else
                    a = ""
                End If
            Catch ex As TimeoutException
                'Not connected
                txtAddtoLog("Error: Serial comms timed out while waiting for time value." & vbCrLf)
                Return -6
            Catch ex As Exception
                txtAddtoLog("Error: Serial comms error while waiting for time value." & vbCrLf)
                txtAddtoLog("Error: " & ex.Message & vbCrLf)
                Return -7
            End Try
            txtAddLineLog(a)

            If a.StartsWith("Time:") Then
                If i < waitSecs Then
                    i += 1
                    t1 = Now
                    t1 = t1.AddSeconds(2)
                Else
                    Exit Do
                End If
            End If

            If cancelRun Then
                txtAddtoLog("Error: Cancelled." & vbCrLf)
                Return -1
            End If

            Application.DoEvents()

        Loop While Now < t1

        If Now >= t1 Then
            txtAddtoLog("Error: Serial comms Never printed out time value." & vbCrLf)
            Return -9
        Else
            t1 = Date.ParseExact(a.Substring(6, 17), "dd-MM-yy HH:mm:ss", Nothing)
            If t1.ToString("ddMMyyHHmmss") <> Now.ToString("ddMMyyHHmmss") Then '.Substring(0, 11)
                txtAddtoLog("Error: Set time value not correct." & vbCrLf)
                Logger_SendCommandFromSleep("t")
                Return -10
            End If
        End If

        Logger_SendCommandFromSleep("t")

        txtAddtoLog("SUCCESS: Time synced with computer." & vbCrLf)

        Return 0

    End Function

    Public Function Logger_WaitForSleep() As Integer
        Dim t1 As DateTime
        Dim a As String

        t1 = Now
        t1 = t1.AddSeconds(5)

        'wait for response
        Do
            Try
                If serLog.BytesToRead > 0 Then
                    a = serLog.ReadLine()
                Else
                    a = ""
                End If
            Catch ex As TimeoutException
                'Not connected
                txtAddtoLog("Error: Serial comms timed out while waiting for sleep." & vbCrLf)
                Return -6
            Catch ex As Exception
                txtAddtoLog("Error: Serial comms error while waiting for sleep." & vbCrLf)
                txtAddtoLog("Error: " & ex.Message & vbCrLf)
                Return -7
            End Try
            txtAddLineLog(a)

            If cancelRun Then
                txtAddtoLog("Error: Cancelled." & vbCrLf)
                Return -1
            End If

            Application.DoEvents()

        Loop While Not a.EndsWith("Going to sleep.  Press any key to wake." & vbCr) And Now < t1

        If Now >= t1 Then
            txtAddtoLog("Error: Serial comms timed out while waiting for sleep." & vbCrLf)
            Return -9
        End If

        Return 0

    End Function

    Public Function Logger_SetTime(ByVal timeStr As String) As Integer
        'DOES NOT ALLIGN!!!
        Dim tmpInt As Integer
        Dim t1 As DateTime
        Dim isOK As Boolean

        Logger_SendCommandFromSleep("T")
        DoEventDelay(20)                'IS THIS TOO SHORT???
        serLog.Write(timeStr)

        t1 = Now
        t1 = t1.AddSeconds(5)
        isOK = True

        If tmpInt = Logger_WaitForSleep() <> 0 Then
            Return tmpInt
        End If

        'txtAddtoLog("SUCCESS: Set time successfully!" & vbCrLf)
        Return 0

    End Function

    Public Function Logger_CompareFlashEntry(ByVal index As Integer, ByVal timeStr As String, ByVal IDStr As String) As Integer

        Dim InTimeStr, InIDStr As String
        Dim tmpInt As Integer

        InTimeStr = ""
        InIDStr = ""

        tmpInt = Logger_ReadEntryFromFlash(index, InTimeStr, InIDStr)

        If tmpInt <> 0 Then
            Return tmpInt
        End If

        If timeStr = "*" Then
            'do nothing...
        ElseIf UCase(InTimeStr) <> UCase(timeStr) Then
            txtAddtoLog("Error: Value Time: " & InTimeStr & " did not match expected value of " & timeStr & vbCrLf)
            Return -20
        End If

        If UCase(InIDStr) <> UCase(IDStr) Then
            txtAddtoLog("Error: Value Time: " & InIDStr & " did not match expected value of " & IDStr & vbCrLf)
            Return -21
        End If

        txtAddtoLog("SUCCESS: Flash entry comoparison successful!" & vbCrLf)

        Return 0

    End Function

    Public Function Logger_ReadEntryFromFlash(ByVal index As Integer, ByRef timeStr As String, ByRef IDStr As String) As Integer
        Dim a As String
        Dim t1 As DateTime
        Dim isOK As Boolean
        Dim i As Integer

        If Logger_AllignSer() = -1 Then
            txtAddtoLog("Error: Couldnt sync with logger" & vbCrLf)
            Return -2
        End If

        Logger_SendCommandFromSleep("r")
        DoEventDelay(10)                'IS THIS TOO SHORT???
        a = index.ToString("0000")
        For i = 0 To 3
            serLog.Write(a.Substring(i, 1))
            DoEventDelay(1)
        Next

        t1 = Now
        t1 = t1.AddSeconds(5)
        isOK = True

        'wait for response
        Do
            Try
                If serLog.BytesToRead > 0 Then
                    a = serLog.ReadLine()
                Else
                    a = ""
                End If
            Catch ex As TimeoutException
                'Not connected
                txtAddtoLog("Error: Serial comms timed out while reading values." & vbCrLf)
                Return -6
            Catch ex As Exception
                txtAddtoLog("Error: Serial comms error while reading values." & vbCrLf)
                txtAddtoLog("Error: " & ex.Message & vbCrLf)
                Return -7
            End Try
            txtAddLineLog(a)

            If a.StartsWith("Address: ") Then
                Try
                    If CInt(a.Split(":")(1).Trim()) / 9 <> index Then
                        txtAddtoLog("Error: Invalid index." & vbCrLf)
                        Return -8
                    End If
                Catch ex As Exception
                    txtAddtoLog("Error: Invalid index." & vbCrLf)
                    Return -8
                End Try
            ElseIf a.StartsWith("Read Time: ") Then
                timeStr = a.Substring(11, 17)
            ElseIf a.StartsWith("RX Data:") Then
                IDStr = UCase(a.Split(":")(1).Substring(2, 8))
            End If

            If cancelRun Then
                txtAddtoLog("Error: Cancelled." & vbCrLf)
                Return -1
            End If

            Application.DoEvents()

        Loop While Not a.EndsWith("Going to sleep.  Press any key to wake." & vbCr) And Now < t1

        If Now >= t1 Then
            txtAddtoLog("Error: Serial comms timed out while reading values." & vbCrLf)
            Return -9
        End If

        'txtAddtoLog("SUCCESS: Read flash entry successfully!" & vbCrLf)
        Return 0

    End Function

    Public Function Logger_SetVerboseMode(ByVal mode As Integer) As Integer
        Dim a As String
        Dim t1 As DateTime
        Dim EraseTimeOut As Integer = 50

        If Logger_AllignSer() = -1 Then
            txtAddtoLog("Error: Couldnt sync with logger" & vbCrLf)
            Return -2
        End If

        If mode > 9 Then mode = 9
        If mode < 0 Then mode = 0

        Logger_SendCommandFromSleep("f")
        DoEventDelay(50)
        serLog.Write(mode)

        t1 = Now
        t1 = t1.AddSeconds(EraseTimeOut)

        Do
            Try
                If serLog.BytesToRead > 0 Then
                    a = serLog.ReadLine()
                    If a Like "Verbose mode: *" Then
                        If a.Substring(14, 1) = mode Then
                            'success
                            txtAddtoLog("Verbose mode set to " & mode & vbCrLf)
                        Else
                            'fail! invalid response
                            txtAddtoLog("Invalid response when setting verbose mode!" & vbCrLf)
                            Return -5
                        End If
                    End If
                Else
                    a = ""
                End If
            Catch ex As TimeoutException
                'Not connected
                txtAddtoLog("Serial comms timed out while reading verbose mode value." & vbCrLf)
                Return -3
            Catch ex As Exception
                txtAddtoLog("Error: Serial comms error while reading verbose mode value." & vbCrLf)
                txtAddtoLog("Error: " & ex.Message & vbCrLf)
                Return -4
            End Try
            txtAddLineLog(a)

            Application.DoEvents()
            If cancelRun Then
                txtAddtoLog("Error: Cancelled." & vbCrLf)
                Return -1
            End If
        Loop While Not a.EndsWith("Going to sleep.  Press any key to wake." & vbCr) And Now < t1

        If Now >= t1 Then
            txtAddtoLog("Error: Erase operation timed out after " & EraseTimeOut & " seconds!" & vbCrLf)
            Return -6
        End If

        Return 0

    End Function

    Public Function Logger_ListenForTag(ByVal tagID As String, ByVal period As String, ByRef savedIndex As Integer) As Integer
        Dim a, b, c As String
        Dim t1 As DateTime
        Dim isOK As Boolean

        If Logger_AllignSer() = -1 Then
            txtAddtoLog("Error: Couldnt sync with logger" & vbCrLf)
            Return -2
        End If

        t1 = Now
        t1 = t1.AddSeconds(CInt(period.Substring(0, 1)) * 60 + CInt(period.Substring(1, 2)) + 5)
        isOK = False

        Do
            Application.DoEvents()
            'number of bytes in "RX ID: 12345678\r\n"
            If serLog.BytesToRead > 15 Then
                Do
                    Try
                        If serLog.BytesToRead > 0 Then
                            a = serLog.ReadLine()
                        Else
                            a = ""
                        End If
                    Catch ex As TimeoutException
                        'Not connected
                        txtAddtoLog("Serial comms timed out while reading values." & vbCrLf)
                        Return -3
                    Catch ex As Exception
                        txtAddtoLog("Error: Serial comms error while reading values." & vbCrLf)
                        txtAddtoLog("Error: " & ex.Message & vbCrLf)
                        Return -4
                    End Try
                    txtAddLineLog(a)

                    If a.StartsWith("Address:") Then
                        'this is the address in flash where the record was stored...
                        c = a.Split(":")(1).Trim
                        savedIndex = CInt(c) / 9
                        'If c <> txtDevFullID.Text Then isOK = False
                    ElseIf a.StartsWith("Time:") Then
                        'c = a.Split(":")(1).Substring(0, 14)
                        'If c <> txtRFID.Text Then isOK = False
                    ElseIf a.StartsWith("RX ID: ") Then
                        c = UCase(a.Split(":")(1).Substring(3, 8))
                        b = UCase(tagID)
                        If c <> b Then
                            txtAddtoLog("Error: Value " & a.Split(":")(0) & ":" & c & " did not match expected value of " & b & vbCrLf)
                        Else
                            txtAddtoLog("SUCCESS: Tag picked up on RF." & vbCrLf)
                            isOK = True
                        End If
                    End If
                    Application.DoEvents()
                    If cancelRun Then
                        txtAddtoLog("Error: Cancelled." & vbCrLf)
                        Return -1
                    End If
                Loop While Not a.EndsWith("." & vbCr)
            End If
            If cancelRun Then
                txtAddtoLog("Error: Cancelled." & vbCrLf)
                Return -1
            End If
        Loop While Now < t1 And isOK = False

        If isOK = False Then
            txtAddtoLog("Error: Timed out waiting for ping from tag." & vbCrLf)
            Return -5
        End If

        Return 0

    End Function

    Public Function Logger_EraseDataFlash() As Integer
        Dim a As String
        Dim t1, t2 As DateTime
        Dim EraseTimeOut As Integer = 50

        If Logger_AllignSer() = -1 Then
            txtAddtoLog("Error: Couldnt sync with logger" & vbCrLf)
            Return -2
        End If

        Logger_SendCommandFromSleep("E")
        DoEventDelay(50)
        serLog.Write("y")

        t1 = Now
        t2 = t1.AddSeconds(3)
        t1 = t1.AddSeconds(EraseTimeOut)

        'number of bytes in "nRF24L01+ Woke me.CrLf"

        Do
            Try
                If serLog.BytesToRead > 0 Then
                    a = serLog.ReadLine()
                Else
                    a = ""
                End If
            Catch ex As TimeoutException
                'Not connected
                txtAddtoLog("Serial comms timed out while reading values." & vbCrLf)
                Return -3
            Catch ex As Exception
                txtAddtoLog("Error: Serial comms error while reading values." & vbCrLf)
                txtAddtoLog("Error: " & ex.Message & vbCrLf)
                Return -4
            End Try
            txtAddLineLog(a)

            Application.DoEvents()
            If cancelRun Then
                txtAddtoLog("Error: Cancelled." & vbCrLf)
                Return -1
            End If
        Loop While Not a.EndsWith("Going to sleep.  Press any key to wake." & vbCr) And Now < t1

        If Now < t2 Then
            'too short of an elapsed time
            txtAddtoLog("Error: Erase too short.. problem with erasing!" & vbCrLf)
            Return -5
        ElseIf Now >= t1 Then
            txtAddtoLog("Error: Erase operation timed out after " & EraseTimeOut & " seconds!" & vbCrLf)
            Return -6
        End If

        Return 0

    End Function

    Public Function Logger_ReadIn(ByVal testStr As String, Optional ByVal TimeOut As Integer = 500, Optional ByVal useLikeOp As Boolean = True, Optional ByVal Len As Integer = 0) As Integer
        Dim t1 As DateTime
        Dim CritError As Boolean
        Dim doneFunc As Boolean = False
        Dim tmpStr As String = ""

        If useLikeOp = False Then Len = testStr.Length
        If testStr = "" Then Return 0
        Dim charAry(Len - 1) As Char

        Application.DoEvents()

        t1 = Now
        t1 = t1.AddMilliseconds(TimeOut)

        'wait for response
        Do
            Try
                If serLog.BytesToRead >= Len Then
                    If Len > 0 Then
                        serLog.Read(charAry, 0, Len)
                        tmpStr = CStr(charAry)
                        'tmpStr &= serLog.ReadExisting()
                        'read some data.. it's all the data!
                        If tmpStr.Substring(0, Len) = testStr Then
                            doneFunc = True
                        Else
                            'bad data - too long!
                            Return -6
                        End If
                    ElseIf Len = 0 Then
                        tmpStr &= serLog.ReadExisting()
                        'unlimited string length
                        If tmpStr Like testStr Then
                            doneFunc = True
                        End If
                    End If
                End If
            Catch ex As TimeoutException
                CritError = True
                txtAddtoLog("Error: Serial comms timed out while setting values." & vbCrLf)
                Return -3
            Catch ex As Exception
                txtAddtoLog("Error: Serial comms error while setting values." & vbCrLf)
                txtAddtoLog("Error: " & ex.Message & vbCrLf)
                CritError = True
                Return -4
            End Try

            Application.DoEvents()

            If cancelRun Then
                txtAddtoLog("Error: Cancelled." & vbCrLf)
                CritError = True
                Return -1
            End If

        Loop While Not doneFunc And Now < t1

        If tmpStr = "" Then
            serLog.ReadExisting()
        Else
            txtAddLineLog(tmpStr)
        End If

        If Now >= t1 Then
            CritError = True
            txtAddtoLog("Error: Serial comms timed out while setting values." & vbCrLf)
            Return -5
        End If

        Return 0

    End Function

    Public Function Logger_WriteRFID(ByVal RFID As String)
        Dim i, tmpInt As Integer
        Dim a As String

        For i = 0 To 4
            a = RFID.Substring(i * 2, 2)
            'write 2 characters to the USART
            serLog.Write(a.Substring(0, 1))
            serLog.Write(a.Substring(1, 1))

            'check that the two chars are echoed
            tmpInt = Logger_ReadIn(IIf(i < 4, a & "-", a), 500, False)
            'if there's an error, exit!
            If tmpInt < 0 Then
                Return tmpInt
            End If
        Next

        Return 0

    End Function

    Public Function Logger_SetRFVals(ByVal RFChan As String, ByVal RFID As String) As Integer

        Dim a, b, c As String
        Dim t1 As DateTime
        Dim CritError As Boolean
        Dim isOK As Boolean
        Dim tmpInt, i As Integer

        If Logger_AllignSer() = -1 Then
            CritError = True
            txtAddtoLog("Error: Couldnt sync with Logger" & vbCrLf)
            Return -2   'Couldn't sync with WID
        End If

        Application.DoEvents()

        Logger_SendCommandFromSleep("*")

        t1 = Now
        t1 = t1.AddSeconds(5)

        'wait for response
        Do
            Try
                If serLog.BytesToRead > 0 Then
                    a = serLog.ReadLine()
                Else
                    a = ""
                End If
            Catch ex As TimeoutException
                CritError = True
                txtAddtoLog("Error: Serial comms timed out while setting values." & vbCrLf)
                Return -3
            Catch ex As Exception
                txtAddtoLog("Error: Serial comms error while setting values." & vbCrLf)
                txtAddtoLog("Error: " & ex.Message & vbCrLf)
                CritError = True
                Return -4
            End Try

            txtAddLineLog(a)

            Select Case a
                Case "RFID (XX-XX-XX-XX-XX):" & vbCr
                    tmpInt = Logger_WriteRFID(RFID)
                    If tmpInt < 0 Then
                        txtAddtoLog("Error: Error writing RFID. Bad response." & vbCrLf)
                        Return tmpInt
                    End If
                Case "RFCHAN (###):" & vbCr
                    For i = 0 To 2
                        serLog.Write(RFChan.Substring(i, 1))
                        DoEventDelay(1)
                    Next
            End Select

            Application.DoEvents()

            If cancelRun Then
                txtAddtoLog("Error: Cancelled." & vbCrLf)
                CritError = True
                Return -1
            End If

        Loop While Not a.EndsWith("Going to sleep.  Press any key to wake." & vbCr) And Now < t1

        If Now >= t1 Then
            CritError = True
            txtAddtoLog("Error: Serial comms timed out while setting values." & vbCrLf)
            Return -5
        End If

        isOK = True

        DoEventDelay(1000)

        Logger_SendCommandFromSleep("?")

        t1 = Now
        t1 = t1.AddSeconds(5)

        'wait for response
        Do
            Try
                If serLog.BytesToRead > 0 Then
                    a = serLog.ReadLine()
                Else
                    a = ""
                End If
            Catch ex As TimeoutException
                'Not connected
                txtAddtoLog("Error: Serial comms timed out while reading values." & vbCrLf)
                CritError = True
                Return -6
            Catch ex As Exception
                txtAddtoLog("Error: Serial comms error while reading values." & vbCrLf)
                txtAddtoLog("Error: " & ex.Message & vbCrLf)
                CritError = True
                Return -7
            End Try
            txtAddLineLog(a)

            If a.StartsWith("RFID:") Then
                c = UCase(a.Split(":")(1).Substring(0, 14).Replace("-", ""))
                b = UCase(RFID)
                If c <> b Then isOK = False
            ElseIf a.StartsWith("RFCHAN:") Then
                c = a.Split(":")(1).Replace(vbCrLf, "")
                b = RFChan
                If CInt(c) <> CInt(b) Then isOK = False
            Else
                c = ""
                b = ""
            End If

            If isOK = False Then
                txtAddtoLog("Error: Value " & a.Split(":")(0) & ":" & c & " did not match expected value of " & b & vbCrLf)
                CritError = True
                Return -8
            End If

            If cancelRun Then
                txtAddtoLog("Error: Cancelled." & vbCrLf)
                CritError = True
                Return -1
            End If

            Application.DoEvents()

        Loop While Not a.EndsWith("Going to sleep.  Press any key to wake." & vbCr) And Now < t1

        If Now >= t1 Then
            CritError = True
            txtAddtoLog("Error: Serial comms timed out while reading values." & vbCrLf)
            Return -9
        End If

        If CritError = False Then
            txtAddtoLog("SUCCESS: Logger configured successfully!" & vbCrLf)
            Return 0
        Else
            Return -10  'unknown error
        End If

    End Function

    Public Function Logger_AllignSer() As Integer
        Dim bgThread As Threading.Thread

        bgThread = New Threading.Thread(AddressOf AllignSerLog_BG)
        bgThread.IsBackground = True
        bgThread.Start()
        Do
            Application.DoEvents()
        Loop While bgThread.IsAlive() = True
        Return RetVal
    End Function

    Public Function Logger_SendCommandFromSleep(ByVal cmdStr As String) As Integer
        Dim bgThread As Threading.Thread

        bgThread = New Threading.Thread(AddressOf SendCommandFromSleepLog_BG)
        bgThread.IsBackground = True
        bgThread.Start(cmdStr)
        Do
            Application.DoEvents()
        Loop While bgThread.IsAlive() = True
        Return RetVal
    End Function

    'blocking.. run in another thread!
    Public Function AllignSerLog_BG() As Integer
        If serLog.IsOpen = True Then
            Dim a As String
            Dim c(0) As Byte
            Dim i As Integer
            Dim hasFound As Boolean = False

            serLog.Write("/")
            serLog.ReadTimeout = 500

            Do
                Try
                    'wait for response
                    Do
                        c(0) = CByte(serLog.ReadChar())
                        a = System.Text.Encoding.ASCII.GetString(c)
                        txtAddLineLog(a)
                    Loop While Not a = ">"
                    hasFound = True
                Catch ex As Exception
                    serLog.Write("/")
                End Try
                i += 1
            Loop While i < 10 And hasFound = False

            serLog.ReadTimeout = 1500

            If hasFound = False Then
                RetVal = -1
                Return -1   'couldn't find ">" char
            End If

            System.Threading.Thread.Sleep(500)

            serLog.Write("/")

            Dim t1 As DateTime = Now
            t1 = t1.AddSeconds(3)

            Do
                Try
                    If serLog.BytesToRead > 0 Then
                        a = serLog.ReadLine()
                    Else
                        a = ""
                    End If
                Catch ex As TimeoutException
                    RetVal = -3
                    Return -3   'Timeout
                Catch ex As Exception
                    RetVal = -4
                    Return -4   'Other exception
                End Try
                txtAddLineLog(a)
            Loop While Not a.EndsWith("Going to sleep.  Press any key to wake." & vbCr) And Now < t1

            If Now >= t1 Then
                RetVal = -5
                Return -5
            Else
                System.Threading.Thread.Sleep(500)
                RetVal = 0
                Return 0
            End If

        Else
            RetVal = -2
            Return -2   'port not open
        End If
    End Function

    'blocking.. run in another thread!
    Public Function SendCommandFromSleepLog_BG(ByVal cmdStr As String) As Integer
        If serLog.IsOpen = True Then
            Dim a As String
            Dim c(0) As Byte

            serLog.Write("/")

            Try
                'wait for response
                Do
                    c(0) = CByte(serLog.ReadChar())
                    a = System.Text.Encoding.ASCII.GetString(c)
                    txtAddLineLog(a)
                Loop While Not a = ">"

            Catch ex As Exception
                RetVal = -1
                Return -1   'did not wake up
            End Try

            System.Threading.Thread.Sleep(500)

            serLog.Write(cmdStr)

            RetVal = 0
            Return 0

        Else
            RetVal = -2
            Return -2   'port not open
        End If
    End Function

    Private Delegate Sub txtAddLineLogDelegate(ByVal addText As String) ', Optional ByVal tx As Boolean = False)

    Public Sub txtAddLineLog(ByVal addText As String) ', Optional ByVal tx As Boolean = False)

        With RTBSerLog
            If .InvokeRequired Then
                Dim del As New txtAddLineLogDelegate(AddressOf txtAddLineLog)
                Me.Invoke(del, addText)
            Else
                .Text &= addText
                .SelectionStart = .TextLength
                .ScrollToCaret()
            End If
        End With

    End Sub


    Private Delegate Sub txtAddtoLogDelegate(ByVal addText As String)

    Public Sub txtAddtoLog(ByVal addText As String)

        With RTBLog
            If .InvokeRequired Then
                Dim del As New txtAddtoLogDelegate(AddressOf txtAddtoLog)
                Me.Invoke(del, addText)
            Else
                .Text &= addText
                .SelectionStart = .TextLength
                .ScrollToCaret()
            End If
        End With

    End Sub

    Private Sub btnShowSER_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnShowSER.Click
        Select Case lblLog.Text
            Case "Test Log:"
                RTBLog.Visible = False
                RTBSerLog.Visible = True
                RTBProgLog.Visible = False
                lblLog.Text = "Logger Serial:"
            Case "Logger Serial:"
                RTBLog.Visible = False
                RTBSerLog.Visible = False
                RTBProgLog.Visible = True
                lblLog.Text = "Programming Log:"
            Case "Programming Log:"
                RTBLog.Visible = True
                RTBSerLog.Visible = False
                RTBProgLog.Visible = False
                lblLog.Text = "Test Log:"
        End Select
    End Sub

    Private Sub FrmMain_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles Me.MouseDoubleClick
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If btnShowSER.Visible = True Then
                btnShowSER.Visible = False
                chkSaveLogs.Visible = False
                btnProg.Visible = False
                cbCodeVersion.Enabled = False
            Else
                btnShowSER.Visible = True
                chkSaveLogs.Visible = True
                btnProg.Visible = True
                cbCodeVersion.Enabled = True
            End If

        End If
    End Sub

    Private Sub btnProg_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnProg.Click
        progFrm.Show()
    End Sub

    Private Sub chkPreserveFlash_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkPreserveFlash.CheckedChanged
        If chkPreserveFlash.Checked = True Then
            lblWarning.Visible = False
        Else
            lblWarning.Visible = True
        End If
    End Sub
End Class