Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Net.Mail

Public Class frmMain
    Public SerComLib As New SerComms(Me)
    Dim logTester1 As New DevPickupList(Me)
    Dim Loggers As New List(Of DevPickupList)
    Public StartNewTest As Boolean = True
    Public testStatus As String = "NOT STARTED"
    Private graphedLogger As Integer = -1
    Private graph_noUpdate As Boolean = True

    Private Sub btnScanDevices_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnScanDevices.Click
        Dim Dev As Ser_Device
        Dim DevList As List(Of Ser_Device)
        Dim m As ListViewItem

        If btnScanDevices.Text = "..." Then
            Exit Sub
        End If

        btnScanDevices.BackgroundImage = My.Resources.search
        btnScanDevices.Text = "..."
        Application.DoEvents()

        DevList = SerComLib.FindDevices
        lvAvailableDevices.Items.Clear()

        For Each Dev In DevList
            m = New ListViewItem
            m.Text = Dev.DevPort
            m.SubItems.Add(Dev.DevName)
            lvAvailableDevices.Items.Add(m)
        Next

        btnScanDevices.Text = ""
        btnScanDevices.BackgroundImage = My.Resources.search
        Application.DoEvents()
    End Sub

    Private Sub btnTestRun_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTestRun.Click

        logTester1.AddWID(New WID_Info("DEAD0001", 10, 3))
        logTester1.AddWID(New WID_Info("DEAD0002", 10, 3))
        logTester1.AddWID(New WID_Info("DEAD0003", 10, 3))
        logTester1.AddWID(New WID_Info("DEAD0004", 10, 3))

        logTester1.SetLogValues(3, 10, "DEAD****", lvAvailableDevices.SelectedItems(0).Text, lvAvailableDevices.SelectedItems(0).SubItems(1).Text)
        logTester1.SetupForTest()
        logTester1.StartTest()


    End Sub

    Public Function RFIDStringToBytes(ByVal str As String) As Byte()
        Dim i As Integer
        Dim b(4) As Byte

        Try
            For i = 0 To 5 - 1
                b(i) = System.Convert.ToInt32(str.Substring(i * 3, 2), 16)
            Next
        Catch ex As Exception
            Throw New Exception("Invalid RFID String")
        End Try

        Return b
    End Function

    Public Function BytesToRFIDString(ByVal b() As Byte) As String
        Dim i As Integer
        Dim str As String = ""

        Try
            For i = 0 To 5 - 1
                'b(i) = System.Convert.ToInt32(str.Substring(i * 3, 2), 16)
                str &= b(i).ToString("X2")
                If (i < 4) Then
                    str &= "-"
                End If
            Next
        Catch ex As Exception
            Throw New Exception("Invalid RFID bytes")
        End Try

        Return str
    End Function

    Public Function PreTestValidator() As Boolean
        If mtbRFFilter.MaskCompleted = False Then
            MsgBox("Please Fill out RF Filter to start test!")
            Return False
        ElseIf mtbRFID.MaskCompleted = False Then
            MsgBox("Please Fill out RFID to start test!")
            Return False
        ElseIf lvDevicesInTest.Items.Count < 1 Then
            MsgBox("There needs to be at least one logger selected to start a test...")
            Return False
        ElseIf lvWIDS.Items.Count < 1 Then
            MsgBox("There needs to be at least one WID (tag) entered to start a test...")
            Return False
        End If

        Return True

    End Function

    Public Sub GenerateLoggersFromData()
        Dim i, j As Integer
        Dim b As Byte()

        Try
            b = RFIDStringToBytes(mtbRFID.Text)
        Catch ex As Exception
            MsgBox("Invalid RFID!")
            Exit Sub
        End Try

        Loggers.Clear()
        For i = 0 To lvDevicesInTest.Items.Count - 1
            Loggers.Add(New DevPickupList(Me))
            For j = 0 To lvWIDS.Items.Count - 1
                Loggers(i).AddWID(New WID_Info(lvWIDS.Items(j).Text, nudTXPer.Value, nudRFChan.Value, b, lvWIDS.Items(j).SubItems(1).Text, lvWIDS.Items(j).SubItems(2).Text))
            Next
            Loggers(i).SetLogValues(nudRFChan.Value, nudTXPer.Value, mtbRFFilter.Text, lvDevicesInTest.Items(i).Text, lvDevicesInTest.Items(i).SubItems(1).Text, b)
        Next

    End Sub

    Public Sub StartRunningTest()
        Dim i As Integer

        If PreTestValidator() = False Then Exit Sub

        If StartNewTest = True Then GenerateLoggersFromData()

        For i = 0 To lvDevicesInTest.Items.Count - 1
            Loggers(i).SetupForTest()
        Next

        For i = 0 To lvDevicesInTest.Items.Count - 1
            Loggers(i).StartTest()
        Next

        tmrUpdateOVData.Enabled = True
        UpdateAllOVData()

    End Sub

    Public Sub StopRunningTest()

        For i = 0 To lvDevicesInTest.Items.Count - 1
            Loggers(i).StopTest()
        Next

        tmrUpdateOVData.Enabled = False
        tmrUpdateGraph.Enabled = False
        UpdateAllOVData()
    End Sub

    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.Click
        Dim sngs() As Single = logTester1.GetWIDAvgs
        Label1.Text = sngs(0) & ", " & sngs(1) & ", " & sngs(2) & ", " & sngs(3)
    End Sub

    Private Sub btnTestSave_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTestSave.Click
        'logTester1.SaveToFile("E:\TestFile.WidLogTest")
        'Loggers(GraphedLogger).SaveToFile("E:\TestFile.WidLogTest")
        SaveTestFile("E:\TestFile.WidLogTest")
    End Sub

    Private Sub btnTestLoad_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnTestLoad.Click
        'logTester1.LoadFromFile("E:\TestFile.WidLogTest")
        LoadTestFile("E:\TestFile.WidLogTest")
    End Sub

    Public Sub SaveTestFile(ByVal FileName As String)

        Dim j As Integer
        Dim l As Long
        Dim i As Integer

        'open file
        Dim fs As IO.FileStream
        Dim sw As IO.StreamWriter
        Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()

        fs = New IO.FileStream(FileName, IO.FileMode.OpenOrCreate)
        sw = New IO.StreamWriter(fs)


        Dim a As New SavedFileData
        a.Channel = nudRFChan.Value
        a.Log_RFFilter = mtbRFFilter.Text
        a.RFID = RFIDStringToBytes(mtbRFID.Text)
        a.TX_RATE = nudTXPer.Value

        ReDim a.WIDS(lvWIDS.Items.Count - 1)
        For i = 0 To lvWIDS.Items.Count - 1
            a.WIDS(i).WID_ID = lvWIDS.Items(i).SubItems(0).Text
            a.WIDS(i).WID_Battery = lvWIDS.Items(i).SubItems(1).Text
            a.WIDS(i).WID_Info = lvWIDS.Items(i).SubItems(2).Text
        Next

        ReDim a.Loggers(Loggers.Count - 1)
        For i = 0 To Loggers.Count - 1
            a.Loggers(i).Log_Name = Loggers(i).Log_Name
            a.Loggers(i).Log_Port = Loggers(i).Log_Port
            ReDim a.Loggers(i).Data(Loggers(i).WIDS.Count - 1, Loggers(i).SegmentsDone - 1)
            For j = 0 To Loggers(i).WIDS.Count - 1
                For l = 0 To Loggers(i).SegmentsDone - 1
                    a.Loggers(i).Data(j, l) = Loggers(i).Data(j, l)
                Next
            Next
        Next

        bf.Serialize(fs, a)

        sw.Close()
        fs.Close()

        'Dim i As Integer
        'Dim s As String
        'Dim PartialFileName As String

        ''sync wids etc. to loggers
        'Dim mainFS As New System.IO.FileStream(FileName, IO.FileMode.Create, IO.FileAccess.ReadWrite)
        'Dim mainSW As New System.IO.StreamWriter(mainFS)

        'mainSW.Write("Parts: " & Loggers.Count & vbCrLf)

        ''combine files...
        'For i = 0 To Loggers.Count - 1
        '    PartialFileName = FileName & "_" & i
        '    Loggers(i).SaveToFile(PartialFileName)
        '    s = My.Computer.FileSystem.ReadAllText(PartialFileName)
        '    mainSW.Write("Length: " & s.Length & vbCrLf)
        '    mainSW.Write(s)

        '    'delete temp files..
        '    My.Computer.FileSystem.DeleteFile(PartialFileName)
        'Next

        'mainSW.Close()

    End Sub

    Public Sub LoadTestFile(ByVal FileName As String)


        Dim i As Integer
        Dim j As Integer
        Dim l As Long

        Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()
        Dim a As New SavedFileData
        Dim bytes As Byte() = My.Computer.FileSystem.ReadAllBytes(FileName)
        a = DirectCast(bf.Deserialize(New System.IO.MemoryStream(bytes)), SavedFileData)

        nudRFChan.Value = a.Channel
        mtbRFFilter.Text = a.Log_RFFilter
        mtbRFID.Text = BytesToRFIDString(a.RFID)
        nudTXPer.Value = a.TX_RATE

        Dim n As ListViewItem
        lvWIDS.Items.Clear()
        For i = 0 To a.WIDS.Length - 1
            n = New ListViewItem
            n.Text = a.WIDS(i).WID_ID
            n.SubItems.Add(a.WIDS(i).WID_Battery)
            n.SubItems.Add(a.WIDS(i).WID_Info)
            lvWIDS.Items.Add(n)
        Next

        Loggers.Clear()
        lvDevicesInTest.Items.Clear()
        For i = 0 To a.Loggers.Count - 1
            n = New ListViewItem
            Loggers.Add(New DevPickupList(Me))
            Loggers(i).Log_Channel = a.Channel
            Loggers(i).Log_RFFilter = a.Log_RFFilter
            Loggers(i).Log_RFID = a.RFID
            Loggers(i).Logger_TX_RATE = a.TX_RATE
            Loggers(i).Log_Name = a.Loggers(i).Log_Name
            Loggers(i).Log_Port = a.Loggers(i).Log_Port

            For j = 0 To a.WIDS.Length - 1
                Loggers(i).WIDS.Add(New WID_Info(a.WIDS(j).WID_ID, a.TX_RATE, a.Channel, a.RFID, a.WIDS(j).WID_Battery, a.WIDS(j).WID_Info))
            Next


            ReDim Loggers(i).Data(a.WIDS.Length - 1, (a.Loggers(i).Data.Length / a.WIDS.Length) - 1)
            For j = 0 To a.WIDS.Count - 1
                For l = 0 To (a.Loggers(i).Data.Length / a.WIDS.Length) - 1
                    Loggers(i).Data(j, l) = a.Loggers(i).Data(j, l)
                Next
            Next

            n.Text = a.Loggers(i).Log_Port
            n.SubItems.Add(a.Loggers(i).Log_Name)
            lvDevicesInTest.Items.Add(n)

            Loggers(i).SegmentsDone = (a.Loggers(i).Data.Length / a.WIDS.Length)
            Loggers(i).Log_Name = a.Loggers(i).Log_Name
            Loggers(i).Log_Port = a.Loggers(i).Log_Port

        Next

        lvOVStats.Items.Clear()
        lvOVStats.Columns.Clear()

        Dim avgs() As Single
        lvOVStats.Columns.Add("Logger")
        For j = 0 To lvWIDS.Items.Count - 1
            lvOVStats.Columns.Add(lvWIDS.Items(j).Text)
        Next
        For i = 0 To Loggers.Count - 1
            avgs = Loggers(i).GetWIDAvgs()
            lvOVStats.Items.Add(New ListViewItem(Loggers(i).Log_Name))
            lvOVStats.Items(i).UseItemStyleForSubItems = False
            For j = 1 To lvWIDS.Items.Count
                lvOVStats.Items(i).SubItems.Add(0)
            Next
        Next

        lvOVStats.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent)
        For j = 1 To lvOVStats.Columns.Count - 1
            lvOVStats.AutoResizeColumn(j, ColumnHeaderAutoResizeStyle.HeaderSize)
        Next

        graphedLogger = 0
        graph_noUpdate = True
        tbStartTime.Maximum = Loggers(graphedLogger).SegmentsDone
        tbEndTime.Maximum = Loggers(graphedLogger).SegmentsDone
        If lblEndTime.Text = "END" Then
            tbEndTime.Value = tbEndTime.Maximum
        End If
        UpdateTransientOVData()
        FillOVStatsWithData()
        graph_noUpdate = False

        SetupGraph(Loggers(graphedLogger), chrtOvf)
        updateGraph()

        Exit Sub

        'Dim i As Integer
        'Dim s As String
        'Dim PartialFileName As String
        'Loggers = New List(Of DevPickupList)

        ''unpack file..
        'Dim c() As Char
        'Dim parts As Integer
        'Dim len As Long
        'Dim subFS As System.IO.FileStream
        'Dim subSW As System.IO.StreamWriter
        ''s = My.Computer.FileSystem.ReadAllText(FileName)
        'Dim mainFS As New System.IO.FileStream(FileName, IO.FileMode.Open, IO.FileAccess.Read)
        'Dim mainSR As New System.IO.StreamReader(mainFS)

        's = mainSR.ReadLine()
        'parts = CInt(s.Split(" ")(1))

        'lvDevicesInTest.Items.Clear()
        'For i = 0 To parts - 1
        '    PartialFileName = FileName & "_" & i

        '    'read length
        '    s = mainSR.ReadLine
        '    len = CLng(s.Split(" ")(1))
        '    'read i'th file
        '    ReDim c(len - 1)
        '    If mainSR.Read(c, 0, len) <> len Then
        '        Stop
        '    End If

        '    'write to file
        '    subFS = New System.IO.FileStream(PartialFileName, IO.FileMode.Create, IO.FileAccess.ReadWrite)
        '    subSW = New System.IO.StreamWriter(subFS)
        '    subSW.Write(c) ', 0 len)

        '    subSW.Close()

        '    Loggers.Add(New DevPickupList(Me))
        '    Loggers(i).LoadFromFile(PartialFileName)
        '    lvDevicesInTest.Items.Add(New ListViewItem({Loggers(i).Log_Port, Loggers(i).Log_Name}))
        '    My.Computer.FileSystem.DeleteFile(PartialFileName)
        'Next

        ''fill in details..
        'nudRFChan.Value = Loggers(0).Log_Channel
        'mtbRFID.Text = Hex(Loggers(0).Log_RFID(0)) & "-" & Hex(Loggers(0).Log_RFID(1)) & "-" & Hex(Loggers(0).Log_RFID(2)) & "-" & Hex(Loggers(0).Log_RFID(3)) & "-" & Hex(Loggers(0).Log_RFID(4))
        'mtbRFFilter.Text = Loggers(0).Log_RFFilter
        'nudTXPer.Value = Loggers(0).Logger_TX_RATE

        'lvWIDS.Items.Clear()
        'For i = 0 To Loggers(0).WIDS.Count - 1
        '    lvWIDS.Items.Add(New ListViewItem({Loggers(0).WIDS(i).WID_ID, Loggers(0).WIDS(i).WID_Battery, Loggers(GraphedLogger).WIDS(i).WID_Info}))
        'Next

        ''GenerateLoggersFromData()

    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'logTester1.TestSaveLoad("E:\TestSaveLoad.txt")
        TabControl1.SelectedIndex = 1

        graphedLogger = 0
        btnTestLoad_Click(sender, e)
        TabControl1.SelectedIndex = 2
        'SetupGraph(Loggers(graphedLogger), chrtOvf)
        'UpdateCumulativeGraph(Loggers(graphedLogger), chrtOvf, 10)
    End Sub

    Private Sub mtbRFID_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles mtbRFID.KeyDown

        If e.KeyCode > Keys.F AndAlso _
           e.KeyCode <= Keys.Z AndAlso _
           Not e.Control AndAlso _
           Not e.Alt Then
            'The user has pressed a letter key greater than F, which would be allowed by the mask, so reject it.
            e.SuppressKeyPress = True

            If Me.mtbRFID.BeepOnError Then
                My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Beep)
            End If
        End If

    End Sub

    Private Sub mtbRFFilter_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles mtbRFFilter.KeyDown


        If e.KeyCode = Keys.Multiply Then
            Return
        End If
        e.SuppressKeyPress = True

        If e.KeyCode = Keys.Multiply Then
            e.SuppressKeyPress = False
        ElseIf e.KeyCode = Keys.D8 AndAlso e.Shift = True Then
            e.SuppressKeyPress = False
        ElseIf e.KeyCode >= Keys.D0 And e.KeyCode <= Keys.D9 AndAlso e.Shift = False Then
            e.SuppressKeyPress = False
        ElseIf e.KeyCode >= Keys.NumPad0 And e.KeyCode <= Keys.NumPad9 Then
            e.SuppressKeyPress = False
        ElseIf e.KeyCode >= Keys.A And e.KeyCode <= Keys.F Then
            e.SuppressKeyPress = False
        ElseIf e.KeyCode >= Keys.Left And e.KeyCode <= Keys.Down Then
            e.SuppressKeyPress = False
        ElseIf e.KeyCode = Keys.Home Or e.KeyCode = Keys.End Then
            e.SuppressKeyPress = False
        Else
            e.SuppressKeyPress = True
        End If

    End Sub
    Private Sub mtbRFFilter_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles mtbRFFilter.KeyPress, mtbRFID.KeyPress
        e.KeyChar = Char.ToUpper(e.KeyChar)
    End Sub

    Private Sub btnAddDeviceToTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddDeviceToTest.Click
        Dim i As Integer

        If lvAvailableDevices.SelectedItems.Count > 0 Then
            For i = 0 To lvDevicesInTest.Items.Count - 1
                If lvDevicesInTest.Items(i).Text = lvAvailableDevices.SelectedItems(0).Text Or _
                    lvDevicesInTest.Items(i).SubItems(1).Text = lvAvailableDevices.SelectedItems(0).SubItems(1).Text Then
                    MsgBox("Cannot have duplicate entries!", MsgBoxStyle.OkOnly & MsgBoxStyle.Exclamation, "Cannot Add!")
                    Return
                End If
            Next
            lvDevicesInTest.Items.Add(New ListViewItem({lvAvailableDevices.SelectedItems(0).Text, lvAvailableDevices.SelectedItems(0).SubItems(1).Text}))
        End If

    End Sub

    Private Sub btnRemoveDevFromTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveDevFromTest.Click
        If lvDevicesInTest.SelectedItems.Count > 0 Then
            Loggers.RemoveAt(lvDevicesInTest.SelectedIndices(0))
            lvDevicesInTest.Items.RemoveAt(lvDevicesInTest.SelectedIndices(0))
        End If
    End Sub

    Private Sub btnAddLoggerManually_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddLoggerManually.Click
        Dim inPort, inName As String
        inPort = InputBox("Please enter the port the new logger can be found on:", "Enter Port", "COM1")
        If inPort <> "" Then
            inName = InputBox("Please enter the name of the new logger:", "Enter Logger Name", "Unnamed Logger")
            If inName <> "" Then
                lvDevicesInTest.Items.Add(New ListViewItem({inPort, inName}))
            End If
        End If
    End Sub

    Private Sub lvDevicesInTest_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lvDevicesInTest.DoubleClick
        If lvDevicesInTest.SelectedItems.Count > 0 Then
            Dim inPort, inName As String
            inPort = InputBox("Please enter the new port the logger can be found on:", "Enter Port", lvDevicesInTest.SelectedItems(0).Text)
            If inPort <> "" Then
                lvDevicesInTest.SelectedItems(0).Text = inPort
                inName = InputBox("Please enter the new name of the logger:", "Enter Logger Name", lvDevicesInTest.SelectedItems(0).SubItems(1).Text)
                If inName <> "" Then
                    lvDevicesInTest.SelectedItems(0).SubItems(1).Text = inName
                End If
            End If
        End If
    End Sub

    Private Sub btnAddWID_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnAddWID.Click
        Dim inID As String
        Dim i As Integer

        inID = InputBox("Please enter the WID ID:", "Enter WID ID", "ABCD1234")
        If inID <> "" Then
            If WIDIDValidator(inID) = True Then
                For i = 0 To lvWIDS.Items.Count - 1
                    If lvWIDS.Items(i).Text = inID Then
                        MsgBox("That WID is already in the test!", vbExclamation & vbOKOnly, "Cannot Add WID!")
                        Exit Sub
                    End If
                Next

                lvWIDS.Items.Add(New ListViewItem({inID, "", ""}))
            Else
                MsgBox("That was not a valid WID ID!" & vbNewLine & "ID should be of the form 'XXXXXXXX' where X is a hexadecimal digit.", vbExclamation & vbOKOnly, "Cannot Add WID!")
            End If

        End If
    End Sub

    Private Sub btnRemoveWID_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRemoveWID.Click
        If lvWIDS.SelectedItems.Count > 0 Then
            lvWIDS.Items.RemoveAt(lvWIDS.SelectedIndices(0))
        End If
    End Sub

    Private Function WIDIDValidator(ByVal WID_ID As String) As Boolean

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

    Private Sub btnEditWID_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEditWID.Click
        Dim EditDialog As New frmWIDEdit

        If lvWIDS.SelectedItems.Count > 0 Then
            EditDialog.mtbWIDID.Text = lvWIDS.SelectedItems(0).Text
            EditDialog.cbBattery.Text = lvWIDS.SelectedItems(0).SubItems(1).Text
            EditDialog.txtInfo.Text = lvWIDS.SelectedItems(0).SubItems(2).Text

            EditDialog.StartPosition = FormStartPosition.CenterParent

            If EditDialog.ShowDialog(Me) = Windows.Forms.DialogResult.OK Then
                lvWIDS.Items(lvWIDS.SelectedIndices(0)) = New ListViewItem({EditDialog.mtbWIDID.Text, EditDialog.cbBattery.Text, EditDialog.txtInfo.Text})
            End If
        End If
    End Sub

    Private Sub ToTrayToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToTrayToolStripMenuItem.Click
        NotifyIcon.Visible = True
        Me.Visible = False
    End Sub

    Private Sub NotifyIcon_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon.MouseDoubleClick
        Me.Visible = True
        NotifyIcon.Visible = False
    End Sub

    Private Sub ExitToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitToolStripMenuItem.Click
        Application.Exit()
    End Sub

    Public Sub UpdateAllOVData()
        lblOVChan.Text = Loggers(graphedLogger).Log_Channel
        lblOVRFFilter.Text = Loggers(graphedLogger).Log_RFFilter
        lblOVRFID.Text = Hex(Loggers(graphedLogger).Log_RFID(0)) & "-" & Hex(Loggers(graphedLogger).Log_RFID(1)) & "-" & Hex(Loggers(graphedLogger).Log_RFID(2)) & "-" & Hex(Loggers(graphedLogger).Log_RFID(3)) & "-" & Hex(Loggers(graphedLogger).Log_RFID(4))
        lblOVTxPer.Text = Loggers(graphedLogger).Logger_TX_RATE
        lblOVTestStatus.Text = testStatus
        lblOVDataRate.Text = (Loggers.Count) * (lvWIDS.Items.Count) * 4 * 3600 / Loggers(graphedLogger).Logger_TX_RATE / 1024 & " KB/hour"

        UpdateTransientOVData()
    End Sub

    Public Sub UpdateTransientOVData()
        lblOVStartTime.Text = Loggers(graphedLogger).TestStartTime

        lblOVRunningTime.Text = Loggers(graphedLogger).GetTestRunningTime().ToString("d\:hh\:mm\:ss")

        lblOVTestData.Text = Loggers(graphedLogger).SegmentsDone * (Loggers.Count) * (lvWIDS.Items.Count) * 4 & " bytes" '4 bytes to a single
        lblOVReservedMemory.Text = Math.Round(Loggers(graphedLogger).Data.Length * (Loggers.Count) / 1024, 2) & " kbytes"

        lblOVTestStatus.Text = testStatus
        If lblOVTestStatus.Text = "RUNNING" Then
            lblOVTestStatus.ForeColor = Color.DarkGreen
        Else
            lblOVTestStatus.ForeColor = Color.Red
        End If

    End Sub

    Private Sub tmrUpdateOVData_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrUpdateOVData.Tick
        UpdateTransientOVData()
        FillOVStatsWithData()
    End Sub

    Public Function getColorFromAvg(ByVal val As Single) As Color
        If val < 0.5 Then
            Return Color.Red
        ElseIf val < 0.85 Then
            Return Color.Orange
        ElseIf val < 1 Then
            Return Color.DarkGreen
        Else
            Return Color.Blue
        End If
    End Function

    Private Sub FillOVStatsWithData()
        Dim i, j As Integer
        Dim avgs() As Single

        For i = 0 To Loggers.Count - 1
            avgs = Loggers(i).GetWIDAvgs()
            For j = 1 To lvWIDS.Items.Count
                lvOVStats.Items(i).SubItems(j).Text = Math.Round(avgs(j - 1) * 100, 2) & " %"
                lvOVStats.Items(i).SubItems(j).ForeColor = getColorFromAvg(avgs(j - 1))
            Next
        Next
    End Sub

    Private Sub btnRunTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnRunTest.Click
        StartRunningTest()
        testStatus = "RUNNING"

        lvOVStats.Items.Clear()
        lvOVStats.Columns.Clear()

        Dim i, j As Integer
        Dim avgs() As Single
        lvOVStats.Columns.Add("Logger")
        For j = 0 To lvWIDS.Items.Count - 1
            lvOVStats.Columns.Add(lvWIDS.Items(j).Text)
        Next
        For i = 0 To Loggers.Count - 1
            avgs = Loggers(i).GetWIDAvgs()
            lvOVStats.Items.Add(New ListViewItem(Loggers(i).Log_Name))
            lvOVStats.Items(i).UseItemStyleForSubItems = False
            For j = 1 To lvWIDS.Items.Count
                lvOVStats.Items(i).SubItems.Add(0)
            Next
        Next

        lvOVStats.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent)
        For j = 1 To lvOVStats.Columns.Count - 1
            lvOVStats.AutoResizeColumn(j, ColumnHeaderAutoResizeStyle.HeaderSize)
        Next

        'update twice per interval
        tmrUpdateOVData.Interval = nudTXPer.Value * 1000 / 2
        tmrUpdateGraph.Interval = 5000
        tmrUpdateGraph.Enabled = True

        FillOVStatsWithData()
        SetupGraph(Loggers(graphedLogger), chrtOvf)
        UpdateCumulativeGraph(Loggers(graphedLogger), chrtOvf)

    End Sub

    Private Sub btnStopTest_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStopTest.Click
        testStatus = "Stopped"
        StopRunningTest()
    End Sub

    Private Sub lblOVTestData_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lblOVTestData.Click

    End Sub

    Private Sub lblOVTestData_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles lblOVTestData.DoubleClick
        Dim lbl As Label = CType(sender, Label)
        My.Computer.Clipboard.SetText(lbl.Text)
    End Sub

    Private Sub ToolStripStatusLabel1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ToolStripStatusLabel1.Click

    End Sub

    Private Sub tmrUpdateGraph_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tmrUpdateGraph.Tick
        'UpdateInstantaneousGraph(Loggers(graphedLogger), chrtOvf,tb)
        graph_noUpdate = True
        tbStartTime.Maximum = Loggers(graphedLogger).SegmentsDone
        tbEndTime.Maximum = Loggers(graphedLogger).SegmentsDone
        If lblEndTime.Text = "END" Then
            tbEndTime.Value = tbEndTime.Maximum
        End If
        graph_noUpdate = False
        updateGraph()
    End Sub

    Public Sub SetupGraph(ByVal dev As DevPickupList, ByVal chrt As Chart)
        chrt.ChartAreas.Clear()
        chrt.Titles.Clear()
        chrt.Series.Clear()
        chrt.Legends.Clear()

        chrt.Titles.Add("'" & dev.Log_Name & "' Pickups")
        chrt.ChartAreas.Add("Main")
        'chrt.ChartAreas("Main")

        Dim i As Integer
        For i = 0 To chrt.ChartAreas(0).Axes.Length - 1
            With chrt.ChartAreas(0).Axes(i)
                .TitleForeColor = Color.White
                .LineColor = Color.White
                .MajorGrid.LineColor = Color.White
                .MinorGrid.LineColor = Color.White
                .InterlacedColor = Color.Gray
                .LineColor = Color.White
                .LineColor = Color.White
                .LabelStyle.ForeColor = Color.White
            End With

            'chrt.ChartAreas(0).AxisX.StripLines(0).col

        Next

        With chrt.ChartAreas(0)
            .BackColor = Color.Black
            .BorderColor = Color.White
        End With

        For i = 0 To lvWIDS.Items.Count - 1
            chrt.Series.Add(lvWIDS.Items(i).Text)
            chrt.Series(i).ChartType = SeriesChartType.Line
            chrt.Series(i).LabelBackColor = Color.Black
            chrt.Series(i).LabelForeColor = Color.White
            chrt.Series(i).BorderWidth = 2

        Next

        chrt.Legends.Add("Legend")
        With chrt.Legends(0)
            .ForeColor = Color.White
            .TitleForeColor = Color.White
            .BackColor = Color.Black
            .TitleBackColor = Color.Black
        End With

        'chrt.Legends("Legend").Enabled = True

        'UpdateInstantaneousGraph(dev, chrt)
    End Sub

    Private Function GetAxisIntervalFromPoints(ByVal pts As Long) As Integer
        Dim i As Long = 100

        Do
            If pts < i Then
                Return i / 10
            Else
                i *= 10
            End If
        Loop

        Return 100

        'If pts < 100 Then
        '    Return 100 / 10
        'ElseIf pts < 1000 Then
        '    Return 1000 / 10
        'ElseIf pts < 10000 Then

        'End If
    End Function

    Public Sub UpdateInstantaneousGraph(ByVal dev As DevPickupList, ByVal chrt As Chart, Optional ByVal points As Integer = 100)

        Dim i, j As Integer
        Dim pntCol As List(Of PointF)

        'For i = 0 To lvWIDS.Items.Count - 1
        '    chrt.Series(i).Points.Clear()
        'Next

        SetupGraph(dev, chrt)
        'Exit Sub

        For i = 0 To lvWIDS.Items.Count - 1
            'chrt.Series(i).Points.Clear()
            pntCol = dev.GetInstGraphData(DevPickupList.GraphType.Graph_Instantaneous, points, i, tbStartTime.Value)
            If Not IsNothing(pntCol) Then
                For j = 1 To pntCol.Count - 1
                    chrt.Series(i).Points.AddXY(pntCol(j).X, pntCol(j).Y)
                Next
            End If
        Next

        'Dim interval As Integer = GetAxisIntervalFromPoints(Loggers(0).SegmentsDone) ' Math.Log10(Loggers(0).SegmentsDone)

        'chrt.ChartAreas(0).AxisX.Interval = interval
        ''chrt.ChartAreas(0).AxisX.Interval = nudTXPer.Value
        'Try
        '    chrt.ChartAreas(0).AxisX.Minimum = pntCol(1).X - (pntCol(1).X Mod interval)
        '    chrt.ChartAreas(0).AxisX.Maximum = pntCol(pntCol.Length - 1).X - (pntCol(pntCol.Length - 1).X Mod interval) + interval

        '    'chrt.ChartAreas(0).AxisX.Minimum = pntCol(1).X - (pntCol(1).X Mod nudTXPer.Value)
        '    'chrt.ChartAreas(0).AxisX.Maximum = pntCol(pntCol.Length - 1).X - (pntCol(pntCol.Length - 1).X Mod nudTXPer.Value) + nudTXPer.Value


        '    'chrt.ChartAreas(0).AxisX.Interval = (chrt.ChartAreas(0).AxisX.Maximum - chrt.ChartAreas(0).AxisX.Minimum) / 20
        'Catch ex As Exception

        'End Try


        Try
            chrt.ChartAreas(0).AxisX.Interval = dev.Logger_TX_RATE  'Math.Round(((pntCol(pntCol.Count - 1).X - pntCol(0).X) / points) / 1000, 1) * 1000
            chrt.ChartAreas(0).AxisX.Minimum = pntCol(0).X - (pntCol(0).X Mod dev.Logger_TX_RATE)
            chrt.ChartAreas(0).AxisX.Maximum = pntCol(pntCol.Count - 1).X
        Catch ex As Exception

        End Try

    End Sub

    Private Function getPointTime(ByRef dev As DevPickupList, ByVal pnt As Long) As DateTime
        Return dev.TestStartTime.AddSeconds(dev.Logger_TX_RATE * pnt)
    End Function


    Public Sub UpdateCumulativeGraph(ByVal dev As DevPickupList, ByVal chrt As Chart, Optional ByVal points As Integer = 10, Optional ByVal pointsToAvg As Integer = 100, Optional ByVal startTime As DateTime = Nothing, Optional ByVal stopTime As DateTime = Nothing)

        Dim i, j As Integer
        Dim pntCol() As PointF

        dev.TestStartTime = Now

        For i = 0 To lvWIDS.Items.Count - 1
            chrt.Series(i).Points.Clear()
            pntCol = dev.GetCumulativeGraphData(DevPickupList.GraphType.Graph_Cumulative, points, i, pointsToAvg, startTime, stopTime)
            If Not IsNothing(pntCol) Then
                For j = 0 To pntCol.Length - 1
                    'chrt.Series(i).Points.AddXY(pntCol(j).X, pntCol(j).Y)
                    chrt.Series(i).Points.AddXY(getPointTime(dev, pntCol(j).X).ToOADate, pntCol(j).Y)
                Next
            End If
        Next



        Dim interval As Integer = (pntCol.Last.X - pntCol.First.X) / 20 'GetAxisIntervalFromPoints(Loggers(0).SegmentsDone) ' Math.Log10(Loggers(0).SegmentsDone)

        chrt.Series(0).XValueType = ChartValueType.DateTime
        chrt.ChartAreas(0).AxisX.LabelStyle.Format = "HH:mm:ss"
        chrt.ChartAreas(0).AxisX.IntervalType = DateTimeIntervalType.Seconds
        chrt.ChartAreas(0).AxisX.Interval = interval * dev.Logger_TX_RATE
        chrt.ChartAreas(0).AxisX.LabelAutoFitStyle = LabelAutoFitStyles.LabelsAngleStep90
        Try
            chrt.ChartAreas(0).AxisX.Minimum = getPointTime(dev, pntCol(1).X - (pntCol(1).X Mod interval)).ToOADate
            chrt.ChartAreas(0).AxisX.Maximum = getPointTime(dev, pntCol(pntCol.Length - 1).X - (pntCol(pntCol.Length - 1).X Mod interval) + interval).ToOADate
            'chrt.ChartAreas(0).AxisX.CustomLabels.Add(chrt.ChartAreas(0).AxisX.Minimum, chrt.ChartAreas(0).AxisX.Maximum / 10, "hi there", 10, LabelMarkStyle.LineSideMark, GridTickTypes.Gridline)

            'Offset values
            'chrt.ChartAreas(0).AxisX.Minimum = pntCol(1).X - (pntCol(1).X Mod interval)
            'chrt.ChartAreas(0).AxisX.Maximum = pntCol(pntCol.Length - 1).X - (pntCol(pntCol.Length - 1).X Mod interval) + interval


        Catch ex As Exception

        End Try

    End Sub



    Private Sub OpenToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OpenToolStripMenuItem.Click
        If OFD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            If OFD.FileName <> "" Then
                LoadTestFile(OFD.FileName)
            End If
        End If
    End Sub

    Private Sub SaveToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveToolStripMenuItem.Click
        If SFD.ShowDialog() = Windows.Forms.DialogResult.OK Then
            If SFD.FileName <> "" Then
                SaveTestFile(SFD.FileName)
            End If
        End If
    End Sub

    Private Sub chrtOvf_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chrtOvf.Click

    End Sub

    Private Sub chrtOvf_MouseClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles chrtOvf.MouseClick
        If e.Button = Windows.Forms.MouseButtons.Right Then
            cmsChartOptions.Show(chrtOvf, e.Location)
        End If
    End Sub

    Private Sub RefreshToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RefreshToolStripMenuItem.Click
        updateGraph()
    End Sub

    Private Sub updateGraph()

        If graph_noUpdate = True Then Exit Sub

        Dim startDate As DateTime
        Dim endDate As DateTime

        If lblStartTime.Text = "START" Then
            startDate = Nothing
        Else
            startDate = DateTime.Parse(lblStartTime.Text)
        End If

        If lblEndTime.Text = "END" Then
            endDate = Nothing
        Else
            endDate = DateTime.Parse(lblEndTime.Text)
        End If


        Select Case cbChartType.Text
            Case "Instantaneous"
                UpdateInstantaneousGraph(Loggers(graphedLogger), chrtOvf, tbPoints.Value)

            Case "Moving Average"
                UpdateCumulativeGraph(Loggers(graphedLogger), chrtOvf, tbPoints.Value, lblAvgPoints.Text, startDate, endDate)
            Case Else

        End Select

    End Sub

    Private Sub tbPoints_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbPoints.Scroll
        lblGraphPoints.Text = tbPoints.Value
        lblGraphPoints.Update()
        updateGraph()
    End Sub

    Private Sub tbEndTime_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbEndTime.Scroll
        If tbEndTime.Value = tbEndTime.Maximum Then
            lblEndTime.Text = "END"
        ElseIf tbEndTime.Value < tbStartTime.Value Then
            'tbEndTime.Value = tbStartTime.Value
            graph_noUpdate = True
            tbStartTime.Value = tbEndTime.Value
            graph_noUpdate = False
        Else
            lblEndTime.Text = getPointTime(Loggers(graphedLogger), tbEndTime.Value)
        End If

        lblStartTime.Update()
        updateGraph()
    End Sub

    Private Sub tbStartTime_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles tbStartTime.MouseDown
        If My.Computer.Keyboard.ShiftKeyDown Then
            chkLockGraphTimeDif.Checked = True
        End If
    End Sub

    Private Sub tbStartTime_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles tbStartTime.MouseUp
        If My.Computer.Keyboard.ShiftKeyDown Then
            chkLockGraphTimeDif.Checked = False
        End If
    End Sub

    Private Sub tbStartTime_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbStartTime.Scroll

        If chkLockGraphTimeDif.Checked = True Then
            graph_noUpdate = True
            Dim endVal As Integer = tbStartTime.Value + chkLockGraphTimeDif.Tag
            If endVal >= tbEndTime.Maximum Then
                tbEndTime.Value = tbEndTime.Maximum
            Else
                tbEndTime.Value = tbStartTime.Value + chkLockGraphTimeDif.Tag
            End If
            tbEndTime_Scroll(sender, e)
            graph_noUpdate = False
        End If

        If tbStartTime.Value = 0 Then
            lblStartTime.Text = "START"
        Else
            If tbStartTime.Value > tbEndTime.Value Then
                'tbStartTime.Value = tbEndTime.Value
                graph_noUpdate = True
                tbEndTime.Value = tbStartTime.Value
                graph_noUpdate = False
            Else
                lblStartTime.Text = getPointTime(Loggers(graphedLogger), tbStartTime.Value)
            End If

        End If


        lblStartTime.Update()
        updateGraph()
    End Sub

    Private Sub lvOVStats_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles lvOVStats.SelectedIndexChanged
        If lvOVStats.SelectedIndices.Count > 0 Then
            graphedLogger = lvOVStats.SelectedIndices(0)
            updateGraph()
        End If
    End Sub

    Private Sub chkLockGraphTimeDif_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkLockGraphTimeDif.CheckedChanged
        tbEndTime.Enabled = Not chkLockGraphTimeDif.Checked
        chkLockGraphTimeDif.Tag = tbEndTime.Value - tbStartTime.Value
    End Sub

    Private Sub tbAvgPoints_Scroll(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbAvgPoints.Scroll
        lblAvgPoints.Text = tbAvgPoints.Value * 2
        lblAvgPoints.Update()
        updateGraph()
    End Sub

    Private Sub cbChartType_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbChartType.SelectedIndexChanged
        Select Case cbChartType.Text
            Case "Instantaneous"
                tbAvgPoints.Visible = False
                lbltbAvgPoints.Visible = False
                lblAvgPoints.Visible = False

                tbEndTime.Visible = False
                lbltbEndTime.Visible = False
                lblEndTime.Visible = False

                tbPoints.Maximum = 100

                chkLockGraphTimeDif.Visible = False

                chrtOvf.ChartAreas(0).AxisY.Maximum = 1.2

                updateGraph()

            Case "Moving Average"
                tbAvgPoints.Visible = True
                lbltbAvgPoints.Visible = True
                lblAvgPoints.Visible = True

                tbEndTime.Visible = True
                lbltbEndTime.Visible = True
                lblAvgPoints.Visible = True

                tbPoints.Maximum = 1000

                chkLockGraphTimeDif.Visible = True

                chrtOvf.ChartAreas(0).AxisY.MaximumAutoSize = 1.2

                updateGraph()
            Case Else

        End Select
    End Sub
End Class
