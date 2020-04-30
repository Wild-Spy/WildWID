Imports WidLoggerV2Comms.SerComms
Imports WidLoggerV2Comms.VarInterface
Imports System.Text.RegularExpressions
Imports System.Runtime.InteropServices
Imports System.IO.Ports
Imports System.Reflection

Public Class frmMain

    Public SerComLib As New SerComms(Me)
    Public DevPorts As New List(Of String)
    Public DevNames As New List(Of String)
    Public DTDiff As TimeSpan
    Private NoUpdate As Boolean = True
    Private DevNameInEdit As String
    Public mretVals() As Object
    Public Vars As New List(Of VarInterface)
    Private cancelPingLoop As Boolean = False
    Private skipDevNameUpdate As Boolean = False 'used for when the device name is changed on the device.  Stops the devName combo-box from updating all variables when the name devices name is updated in it's list
    Private CtrlListComms As List(Of Control)
    Private strScanPos As Integer = 0
    Private SerTextCmdActive As Integer = 0
    Private alltxt As String = ""
    Public connectedDevVersion As String = ""
    Dim TagStateVarID As Integer
    Dim chkPrintTags_UpdateValue As Boolean = True

    Private tagList As TagList
    Private tagListCache() As ListViewItem 'array to cache items for the virtual list
    Private tagListFirstItem As Integer 'stores the index of the first item in the cache
    Private sysEventList As TagList
    Private sysEventListCache() As ListViewItem 'array to cache items for the virtual list
    Private sysEventListFirstItem As Integer 'stores the index of the first item in the cache

    Private lviList As New List(Of ListViewItem)
    Private tags As New List(Of TagTestPickupStats)


    <StructLayout(LayoutKind.Sequential)> _
    Structure COMMPROP
        Public wPacketLength As Short
        Public wPacketVersion As Short
        Public dwServiceMask As Integer
        Public dwReserved1 As Integer
        Public dwMaxTxQueue As Integer
        Public dwMaxRxQueue As Integer
        Public dwMaxBaud As Integer
        Public dwProvSubType As Integer
        Public dwProvCapabilities As Integer
        Public dwSettableParams As Integer
        Public dwSettableBaud As Integer
        Public wSettableData As Short
        Public wSettableStopParity As Short
        Public dwCurrentTxQueue As Integer
        Public dwCurrentRxQueue As Integer
        Public dwProvSpec1 As Integer
        Public dwProvSpec2 As Integer
        Public wcProvChar As String
    End Structure

    <DllImport("kernel32.dll")> _
    Private Shared Function GetCommProperties(hFile As IntPtr, ByRef lpCommProp As COMMPROP) As Boolean
    End Function
    <DllImport("kernel32.dll", CharSet:=CharSet.Auto, SetLastError:=True)> _
    Private Shared Function CreateFile(lpFileName As String, dwDesiredAccess As Integer, dwShareMode As Integer, securityAttrs As IntPtr, dwCreationDisposition As Integer, dwFlagsAndAttributes As Integer, _
 hTemplateFile As IntPtr) As IntPtr
    End Function

    Public Enum VarNameList As Integer
        DevVersion
        LoggerName
        Records
        RTCTime

        RFFilter
        VerboseMode
        RFParams

        SendEmailPeriod
        SendEmailNext
        EmailToAddress
        GPRSSettings
    End Enum

    Public Enum VarCatPage As Integer
        General
        RFSettings
        GSMSettings
    End Enum

    Public Function FindNameInVarLst(ByVal ID As VarNameList) As VarInterface
        Dim n As VarInterface

        For Each n In Vars
            If n.ID = ID Then
                Return n
            End If
        Next

        Throw New Exception("No variable with name """ & Name & """exists!")
        Return Nothing

    End Function

    'Sets up controls which will be enabled/disabled based on whether the device is connected or not
    Private Sub setupControlList()
        CtrlListComms = New List(Of Control)
        CtrlListComms.Add(btnScan)
        CtrlListComms.Add(cbDevNames)

        CtrlListComms.Add(btnReadLog)
        CtrlListComms.Add(btnDevTimeSync)
        CtrlListComms.Add(btnEraseDataflash)

        CtrlListComms.Add(btnReadVals)
        CtrlListComms.Add(btnReadRFVals)
        CtrlListComms.Add(btnReadGSMVals)
        CtrlListComms.Add(btnWriteVals)
        CtrlListComms.Add(btnWriteRFVals)
        CtrlListComms.Add(btnWriteGSMVals)
        CtrlListComms.Add(chkGM862BridgeMode)
        CtrlListComms.Add(btnConfigureGM862)
    End Sub

    Private Sub ControlsEnabled(ByVal Enabled As Boolean)
        Dim c As Control
        For Each c In CtrlListComms

            c.Enabled = Enabled
        Next
    End Sub

    Private Sub btnCon_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnScan.Click
        Dim comPorts() As String = System.IO.Ports.SerialPort.GetPortNames()
        Dim PortStr, tmpName As String
        Dim tmpErr As MyErr_Enum
        Dim devID As UInt16
        Dim RetVals(0) As Object

        If lvLogs.Items.Count > 0 Or lvSysEvents.Items.Count > 0 Then
            If MsgBox("This will clear all existing data, are you sure you want to continue?", MsgBoxStyle.YesNo, "Warning!") = MsgBoxResult.Yes Then
                lvLogs.Items.Clear()
                lvSysEvents.Items.Clear()
            Else
                Exit Sub
            End If
        End If

        skipDevNameUpdate = True
        cbDevNames.Text = ""
        skipDevNameUpdate = False
        ControlsEnabled(False)
        NoUpdate = True

        'All vars to unknown
        For Each tmpVar As VarInterface In Vars
            tmpVar.State = VarState.VarState_Unknown
            Try
                tmpVar.Value = ""
            Catch ex As Exception
                Try
                    tmpVar.Value = 0
                Catch ex1 As Exception

                End Try
            End Try
        Next

        Application.DoEvents()

        Stop_Ping_Loop()
        If SerComLib.Ser.IsOpen Then
            SerComLib.Ser.Close()
        End If
        lblStatus.Text = "Scanning..."

        If SerComLib.Ser.IsOpen() Then
            SerComLib.Ser.Close()
        End If
        SerComLib.Ser_Timeout = 200 '00000
        SerComLib.Ser_Baud = SerDev_LoggerV2_ComFuncs.DevBaud

        'Clear lists
        DevPorts.Clear()
        DevNames.Clear()
        cbDevNames.Items.Clear()

        For Each PortStr In comPorts

            'PortStr = "COM28"
            SerComLib.Ser_PortName = PortStr

            Try
                tmpErr = SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_DevTypeID, Nothing, RetVals)
            Catch ex As Exception
                tmpErr = MyErr_Enum.MyErr_Unknown
            End Try

            If tmpErr = MyErr_Enum.MyErr_Success Then
                devID = CType(RetVals(0), UInt16)
                If devID = SerDev_LoggerV2_ComFuncs.DevID Then
                    'Read Device Name
                    tmpErr = SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_LoggerName, Nothing, RetVals)
                    If tmpErr = MyErr_Enum.MyErr_Success Then
                        tmpName = CType(RetVals(0), String)
                        DevPorts.Add(PortStr)
                        DevNames.Add(tmpName)
                        cbDevNames.Items.Add("[" & DevNames.Count & "] " & tmpName)
                    End If
                End If
            End If
        Next

        'If cbDevNames.Items.Count > 0 Then cbDevNames.Text = cbDevNames.Items(0)

        lblStatus.Text = "Not Connected"
        NoUpdate = False
        ControlsEnabled(True)

    End Sub

    Private Sub TimerRTC_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TimerRTC.Tick
        Dim AlreadyTrue As Boolean = False

        If NoUpdate = True Then AlreadyTrue = True

        NoUpdate = True
        If Vars(3).State = VarState.VarState_Current Then
            Vars(3).UpdateCurVal = True
        End If
        dtpDevDateTime.Value = Now() - DTDiff
        If AlreadyTrue = False Then NoUpdate = False
    End Sub

    Private Delegate Function FindSelectedPortNumDelegate() As String

    Public Function FindSelectedPortNum() As String
        If cbDevNames.InvokeRequired = True Then
            Dim del As New FindSelectedPortNumDelegate(AddressOf FindSelectedPortNum)
            Return CStr(cbDevNames.Invoke(del))
        Else
            Return DevPorts(cbDevNames.SelectedIndex)
        End If
    End Function

    Public Function VersionStringToInteger(verStr As String) As Integer
        Dim subVerStrs() As String
        Dim retVal As Integer = 0
        verStr = verStr.Trim
        If verStr.StartsWith("V") Then
            subVerStrs = verStr.Substring(1).Split(".")

            Try
                retVal += 10 ^ 6 * CInt(subVerStrs(0))
                retVal += 10 ^ 3 * CInt(subVerStrs(1))
                retVal += 10 ^ 0 * CInt(subVerStrs(2))
            Catch ex As Exception
                Throw New Exception("Invalid Version String")
            End Try

            Return retVal
        End If
        Throw New Exception("Invalid Version String")
    End Function

    Private Sub setLvLogsColumns(IncludeRSSI As Boolean)
        If IncludeRSSI Then
            If lvLogs.Columns.Count = 6 Then
                'do nothing
            ElseIf lvLogs.Columns.Count < 6 Then
                lvLogs.Columns.Add("RSSI")
            End If
        Else
            If lvLogs.Columns.Count = 6 Then
                'lvLogs.Columns.RemoveByKey("RSSI")
                lvLogs.Columns.RemoveAt(lvLogs.Columns.Count - 1)
            Else
                'do nothing
            End If
        End If
    End Sub

    Private Sub cbDevNames_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbDevNames.SelectedIndexChanged
        Dim tmpName As String
        Dim tmpErr As MyErr_Enum
        Dim devID As UInt16
        Dim RetVals(0) As Object
        Dim tmpVar As VarInterface
        Static curInd As Integer

        If skipDevNameUpdate = True Then
            curInd = cbDevNames.SelectedIndex
            Exit Sub
        End If

        If lvLogs.Items.Count > 0 Or lvSysEvents.Items.Count > 0 Then
            If MsgBox("This will clear all existing data, are you sure you want to continue?", MsgBoxStyle.YesNo, "Warning!") = MsgBoxResult.Yes Then
                lvLogs.Items.Clear()
                lvSysEvents.Items.Clear()
            Else
                skipDevNameUpdate = True
                cbDevNames.SelectedIndex = curInd
                skipDevNameUpdate = False
                Exit Sub
            End If
        End If

        curInd = cbDevNames.SelectedIndex

        ControlsEnabled(False)
        'All vars to unknown
        For Each tmpVar In Vars
            tmpVar.State = VarState.VarState_Unknown
            Try
                tmpVar.Value = ""
            Catch ex As Exception
                Try
                    tmpVar.Value = 0
                Catch ex1 As Exception

                End Try
            End Try
        Next

        Application.DoEvents()

        Stop_Ping_Loop()
        If SerComLib.Ser.IsOpen Then
            SerComLib.Ser.Close()
        End If
        lblStatus.Text = "Not Connected"

        'Make sure it's the right device still!
        SerComLib.Ser_PortName = FindSelectedPortNum()

        Try
            tmpErr = SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_DevTypeID, Nothing, RetVals)
        Catch ex As Exception
            tmpErr = MyErr_Enum.MyErr_Unknown
            DisconnectDevice()
        End Try
        If tmpErr = MyErr_Enum.MyErr_Success Then
            devID = CType(RetVals(0), UInt16)

            If devID = SerDev_LoggerV2_ComFuncs.DevID Then

                'Read Device Name
                Try
                    tmpErr = SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_LoggerName, Nothing, RetVals)
                Catch ex As Exception
                    tmpErr = MyErr_Enum.MyErr_Unknown
                    DisconnectDevice()
                End Try
                If tmpErr = MyErr_Enum.MyErr_Success Then
                    tmpName = CType(RetVals(0), String)

                    If tmpName = DevNames(cbDevNames.SelectedIndex) Then

                        Try
                            tmpErr = SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_Version, Nothing, RetVals)
                        Catch ex As Exception
                            tmpErr = MyErr_Enum.MyErr_Unknown
                            DisconnectDevice()
                        End Try
                        If tmpErr = MyErr_Enum.MyErr_Success Then
                            connectedDevVersion = CType(RetVals(0), String).Split("-").Last.Trim()

                            Try
                                If VersionStringToInteger(connectedDevVersion) >= VersionStringToInteger("V2.1.0") Then
                                    setLvLogsColumns(True)
                                Else
                                    setLvLogsColumns(False)
                                End If

                                'Open Serial Port
                                SerComLib.Ser_Baud = 115200
                                SerComLib.Ser.PortName = FindSelectedPortNum()
                                SerComLib.Ser.Open()
                                lblStatus.Text = "Reading Values..."
                                Application.DoEvents()

                                'Fill Data
                                For Each tmpVar In Vars
                                    If SerComLib.Ser.IsOpen = False Then
                                        Throw New Exception("Abort Connect")
                                    End If
                                    Try
                                        tmpVar.ReadFromDev()
                                    Catch ex As Exception

                                    End Try
                                Next

                                'FillOverviewInfo()
                                'FillRFSettingsInfo()
                                'FillGSMSettingsInfo()
                                lblStatus.Text = "Connected (" & FindSelectedPortNum() & ")"
                                Start_Ping_Loop()
                            Catch ex As Exception
                                If ex.Message = "Abort Connect" Then
                                    DisconnectDevice()
                                Else
                                    MsgBox("Invalid Version String", , "")
                                End If
                            End Try
                        End If

                    End If
                End If
            End If
        End If

        ControlsEnabled(True)
    End Sub

    Private Sub frmMain_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Stop_Ping_Loop()
        If SerComLib.Ser.IsOpen Then
            SerComLib.Ser.Close()
        End If
        lblStatus.Text = "Not Connected"
    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DTDiff = New TimeSpan()
        DTDiff = Now() - dtpDevDateTime.Value
        setupControlList()

        splitContainer2.Panel2Collapsed = True

        Me.ResizeRedraw = False
        Me.Show()
        'TabControlMain.SelectTab(5)

        Try
            Vars.Add(New VarInterface(VarNameList.DevVersion, VarCatPage.General, TLPOverview, 1, "Version", Nothing, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_Version, SerDev_LoggerV2_ComFuncs.CMD_ByteID.NONE))
            Vars.Add(New VarInterface(VarNameList.LoggerName, VarCatPage.General, TLPOverview, 2, "Logger Name", AddressOf LoggerNameValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_LoggerName, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_LoggerName))
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf LoggerNameSetHandler
            'Vars.Add(New VarInterface(VarNameList.Records, VarCatPage.General, TLPOverview, 3, "Records", Nothing, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RecordCount, SerDev_LoggerV2_ComFuncs.CMD_ByteID.NONE))
            Vars.Add(New VarInterface(VarNameList.Records, VarCatPage.General, pbRecordCount, txtRecordCount, Nothing, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RecordCount, SerDev_LoggerV2_ComFuncs.CMD_ByteID.NONE))
            Vars.Add(New VarInterface(VarNameList.RTCTime, VarCatPage.General, PBDevDateTime, dtpDevDateTime, AddressOf RTCTimeValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_DateTime, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_DateTime))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf RTCTimeGetHandler

            'Vars.Add(New VarInterface(VarNameList.RFChan, VarCatPage.RFSettings, TLPRFSet, 1, "RF Channel", AddressOf RFChanValidateHandler, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RFChan, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_RFChan))
            'Vars.Add(New VarInterface(VarNameList.RFID, VarCatPage.RFSettings, TLPRFSet, 2, "RF ID", AddressOf RFIDValidateHandler, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RFID, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_RFID))
            'Vars(Vars.Count - 1).GetHandlerFunc = AddressOf RFIDGetHandler
            'Vars(Vars.Count - 1).SetHandlerFunc = AddressOf RFIDSetHandler
            Vars.Add(New VarInterface(VarNameList.RFFilter, VarCatPage.RFSettings, TLPRFSet, 1, "RF Filter", AddressOf RFFilterValidateHandler, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RFFilter, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_RFFilter))
            Vars.Add(New VarInterface(VarNameList.VerboseMode, VarCatPage.RFSettings, pbVerboseMode, cbVerboseMode, AddressOf VerboseModeValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_VerboseMode, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_VerboseMode))
            TagStateVarID = Vars.Count - 1
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf VerboseModeGetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf VerboseModeSetHandler
            Vars.Add(New VarInterface(VarNameList.RFParams, VarCatPage.RFSettings, TLPRFSet, 3, "RF Params", AddressOf RFParamValidateHandler, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RFParams, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_RFParams))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf RFParamGetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf RFParamSetHandler
            Dim tmpTxt As TextBox = CType(Vars(Vars.Count - 1).Control, TextBox)
            TLPRFSet.AutoSize = True
            tmpTxt.Multiline = True
            tmpTxt.Height = 30
            TLPRFSet.AutoSize = False
            Vars.Add(New VarInterface(VarNameList.SendEmailPeriod, VarCatPage.GSMSettings, TLPGSMSet, 1, "Send Email Period", AddressOf SendEmailPeriodValidateHandler, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_SendDataGPRSPeriod, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_SendDataGPRSPeriod))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf SendEmailPeriodGetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf SendEmailPeriodSetHandler
            Vars.Add(New VarInterface(VarNameList.SendEmailNext, VarCatPage.GSMSettings, pbSendEmailNext, dtpSendEmailNext, AddressOf SendEmailNextValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_SendDataGPRSNext, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_SendDataGPRSNext))
            Vars.Add(New VarInterface(VarNameList.EmailToAddress, VarCatPage.GSMSettings, TLPGSMSet, 3, "Email To Address", AddressOf EmailToAddressValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_EmailToAddress, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_EmailToAddress))
            Vars.Add(New VarInterface(VarNameList.GPRSSettings, VarCatPage.GSMSettings, pbGPRSSettings, cbGPRSSettings, AddressOf GPRSSettingsValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_GPRSVals, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_GPRSVals))
            Vars(Vars.Count - 1).ComboBoxUseTextAsProperty = True
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf GPRSSettingsGetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf GPRSSettingsSetHandler
        Catch ex As Exception
            Stop
        End Try

        lvLogs.Columns(4).Width = 0
    End Sub

    Private Function GPRSSettingsGetHandler(ByVal sender As VarInterface, ByRef retVals() As Object) As Boolean
        Try
            sender.Value = "Dev Value - APN=""" & CStr(retVals(0)) & """, USERID=""" & CStr(retVals(1)) & """, PASSW=""" & CStr(retVals(2)) & """"
        Catch ex As Exception
            sender.Value = Nothing
            sender.State = VarState.VarState_Unknown
            Return False
        End Try
        Return True
    End Function

    Private Function GPRSSettingsSetHandler(ByVal sender As VarInterface) As Object()
        Dim ctrl As ComboBox = CType(sender.Control, ComboBox)
        Dim a, b, len As Integer
        Dim parts(2) As String
        Dim i As Integer
        Dim startMatch() As String = {" - APN=""", ", USERID=""", " PASSW="""}
        Dim Params(2) As Object
        Dim testTxt As String = ctrl.Text

        If testTxt Like "* - APN=""*"", USERID=""*"", PASSW=""*""" Then
            b = 0
            For i = 0 To 2
                'a = InStr(ctrl.Text, startMatch(i)) + startMatch(i).Length-1      'Start each search from start
                a = InStr(b + 1, testTxt, startMatch(i)) + startMatch(i).Length - 1    'Start each search from end of last match
                b = InStr(a + 1, testTxt, """")
                len = b - a - 1
                Params(i) = CStr(testTxt.Substring(a, len))
            Next
            Return Params
        End If
        'Should never get here.. should be validated before this function is ever entered!!
        Stop
        Return Nothing
    End Function

    Private Function GPRSSettingsValidateHandler(ByVal sender As VarInterface) As Boolean
        Dim ctrl As ComboBox = CType(sender.Control, ComboBox)
        Dim a, b, len As Integer
        Dim parts(2) As String
        Dim i As Integer
        Dim startMatch() As String = {" - APN=""", ", USERID=""", ", PASSW="""}
        Dim Params(2) As Object
        Dim testTxt As String = ctrl.Text


        If testTxt Like "* - APN=""*"", USERID=""*"", PASSW=""*""" And testTxt.Count(Function(c As Char) c = """") = 6 Then
            b = 0
            For i = 0 To 2
                'a = InStr(ctrl.Text, startMatch(i)) + startMatch(i).Length-1      'Start each search from start
                a = InStr(b + 1, testTxt, startMatch(i)) + startMatch(i).Length - 1    'Start each search from end of last match
                b = InStr(a + 1, testTxt, """")
                len = b - a - 1
                Params(i) = CStr(testTxt.Substring(a, len))
                If Params(i).ToString.Length > 29 Then
                    'if one of the variables is longer than 29 characters long, it wont fit in the variable so it's not valid!
                    Return False
                End If
            Next

            Return True
        End If

        Return False
    End Function

    Private Function EmailToAddressValidateHandler(ByVal sender As VarInterface) As Boolean
        Dim ctrl As TextBox = CType(sender.Control, TextBox)

        If ctrl.Text.Length > 59 Then
            Return False
        End If

        Return True
    End Function

    Private Function LoggerNameSetHandler(ByVal sender As VarInterface) As Object()
        Dim ctrl As TextBox = CType(sender.Control, TextBox)
        Dim Params(0) As Object
        Dim tmpIndex As Integer = cbDevNames.SelectedIndex

        skipDevNameUpdate = True
        cbDevNames.Items(tmpIndex) = "[" & cbDevNames.SelectedIndex + 1 & "] " & sender.Value
        cbDevNames.SelectedIndex = tmpIndex
        skipDevNameUpdate = False

        Params(0) = sender.Value
        Return Params
    End Function

    Private Function SendEmailPeriodSetHandler(ByVal sender As VarInterface) As Object()
        Dim i As Integer
        Dim ctrl As TextBox = CType(sender.Control, TextBox)

        'All characters are good, and we have the right length!
        'If sender.State = VarState.VarState_OutDated Then
        Dim Params(0) As Object
        Dim bAry(5) As Byte
        Dim n As String
        i = 0

        For Each n In ctrl.Text.Split(",")
            bAry(5 - i) = CInt(n.Trim.Substring(0, 2))
            i += 1
        Next

        bAry(0) = 0

        Params(0) = bAry
        Return Params
        'Else
        'Return Nothing
        'End If
    End Function

    Private Function SendEmailPeriodGetHandler(ByVal sender As VarInterface, ByRef retVals() As Object) As Boolean
        Try
            sender.Value = CInt(retVals(0)(5)).ToString("D2") & " Yrs, " & CInt(retVals(0)(4)).ToString("D2") & " Months, " & CInt(retVals(0)(3)).ToString("D2") & " Days, " & CInt(retVals(0)(2)).ToString("D2") & " Hrs, " & CInt(retVals(0)(1)).ToString("D2") & " Mins"
        Catch ex As Exception
            sender.Value = ""
            sender.State = VarState.VarState_Unknown
            Return False
        End Try
        Return True
    End Function

    Private Function RFParamSetHandler(ByVal sender As VarInterface) As Object()
        Dim i As Integer
        Dim ctrl As TextBox = CType(sender.Control, TextBox)

        'All characters are good, and we have the right length!
        'If sender.State = VarState.VarState_OutDated Then
        Dim Params(0) As Object
        Dim bAry(39) As Byte
        Dim n As String
        Dim tmpstr As String = ctrl.Text

        For i = 0 To 39
            bAry(i) = 0
        Next

        tmpstr = tmpstr.Replace("0x", "").Replace(vbCr, "").Replace(vbLf, "").Replace(vbTab, "").Replace(" ", "")

        For i = 0 To tmpstr.Length - 2 Step 2
            n = tmpstr.Substring(i, 2)
            bAry(i / 2) = Convert.ToByte(n, 16)
        Next

        Params(0) = bAry
        Return Params
        'Else
        'Return Nothing
        'End If
    End Function

    Private Function RFParamGetHandler(ByVal sender As VarInterface, ByRef retVals() As Object) As Boolean
        Try
            Dim tmpStr As String = ""
            Dim i, j As Integer
            Dim bytes() As Byte = retVals(0)

            j = 0
            For i = 0 To bytes.Length - 1
                tmpStr &= "0x" & CInt(retVals(0)(i)).ToString("X2") & " "
                j += 1
                If j >= 10 Then
                    tmpStr &= vbNewLine
                    j = 0
                End If
            Next

            sender.Value = tmpStr
        Catch ex As Exception
            'CType(sender.Control, TextBox).Text = ""
            sender.Value = ""
            sender.State = VarState.VarState_Unknown
            Return False
        End Try
        Return True

    End Function

    Private Function SendEmailPeriodValidateHandler(ByVal sender As VarInterface) As Boolean
        Dim ctrl As TextBox = CType(sender.Control, TextBox)

        If Not (ctrl.Text.Replace(" ", "") Like "##Yrs,##Months,##Days,##Hrs,##Mins") Then '"##Yrs,##Months,##Days##Hrs##Mins" Then
            Return False
        End If

        Return True
    End Function

    Private Function SendEmailNextValidateHandler(ByVal sender As VarInterface) As Boolean
        Dim ctrl As DateTimePicker = CType(sender.Control, DateTimePicker)

        If ctrl.Value.Year < 2000 Or ctrl.Value.Year > 2255 Then
            Return False
        End If

        Return True
        'DTDiff = Now() - dtpDevDateTime.Value
    End Function

    Private Function VerboseModeSetHandler(ByVal sender As VarInterface) As Object()
        Dim ctrl As ComboBox = CType(sender.Control, ComboBox)

        updatechkPrintTags(sender.Value)
        'If sender.State = VarState.VarState_OutDated Then
        Dim Params(0) As Object

        Params(0) = sender.Value + 1
        Return Params
        'Else
        'Return Nothing
        'End If
    End Function

    Private Function VerboseModeGetHandler(ByVal sender As VarInterface, ByRef retVals() As Object) As Boolean
        Try
            sender.Value = retVals(0) - 1
            If sender.Value < 0 Then
                Throw New Exception("No value")
            End If

            updatechkPrintTags(sender.Value)

        Catch ex As Exception
            CType(sender.Control, ComboBox).Text = "INVALID"
            sender.Value = -1
            sender.State = VarState.VarState_Unknown
            Return False
        End Try
        Return True
    End Function

    Private Function VerboseModeValidateHandler(ByVal sender As VarInterface) As Boolean
        Dim ctrl As ComboBox = CType(sender.Control, ComboBox)

        If ctrl.SelectedIndex < 0 Then
            Return False
        End If

        Return True
    End Function

    Private Function LoggerNameValidateHandler(ByVal sender As VarInterface) As Boolean
        Dim ctrl As TextBox = CType(sender.Control, TextBox)

        If ctrl.Text.Length > 19 Then
            'ctrl.Text = ctrl.Text.Substring(0, 19)
            Return False
        End If

        Return True
    End Function

    Private Function RFChanValidateHandler(ByVal sender As VarInterface) As Boolean
        Dim ctrl As TextBox = CType(sender.Control, TextBox)

        Try
            If CInt(ctrl.Text) > 255 Or CInt(ctrl.Text) < 0 Then
                Throw New ArgumentOutOfRangeException()
            End If
        Catch ex As Exception
            Return False
        End Try

        Return True
    End Function

    Private Function RFFilterValidateHandler(ByVal sender As VarInterface) As Boolean
        Dim ctrl As TextBox = CType(sender.Control, TextBox)

        Dim i As Integer
        Dim tmpchar As Char

        If ctrl.Text.Length = 8 Then
            For i = 0 To 7
                tmpchar = ctrl.Text.Substring(i, 1)
                If Not ((tmpchar >= "0" And tmpchar <= "9") Or (tmpchar >= "A" And tmpchar <= "F") _
                    Or (tmpchar >= "a" And tmpchar <= "f") Or (tmpchar = "*")) Then
                    'MsgBox("Enter only numbers [0-9,A-F] or wildcard tokens [*]." & vbNewLine & "There must be 8 characters exactly.", MsgBoxStyle.Exclamation, "Invalid format!")
                    Return False
                End If
            Next
            Return True
        Else
            'MsgBox("Enter only numbers [0-9,A-F] or wildcard tokens [*]." & vbNewLine & "There must be 8 characters exactly.", MsgBoxStyle.Exclamation, "Invalid format!")
            Return False
        End If
    End Function

    Private Function RFParamValidateHandler(ByVal sender As VarInterface) As Boolean
        Dim ctrl As TextBox = CType(sender.Control, TextBox)

        Dim i As Integer
        Dim tmpChar As Char
        Dim tmpStr As String = ctrl.Text

        tmpStr = tmpStr.Replace("0x", "").Replace(vbCr, "").Replace(vbLf, "").Replace(vbTab, "").Replace(" ", "")

        If tmpStr.Length >= (37 * 2) And tmpStr.Length <= (40 * 2) And (tmpStr.Length Mod 2 = 0) Then
            For i = 0 To tmpStr.Length - 1
                tmpChar = tmpStr.Substring(i, 1)
                If Not ((tmpChar >= "0" And tmpChar <= "9") Or (tmpChar >= "A" And tmpChar <= "F") _
                    Or (tmpChar >= "a" And tmpChar <= "f")) Then
                    'MsgBox("Enter only numbers [0-9,A-F] or wildcard tokens [*]." & vbNewLine & "There must be 8 characters exactly.", MsgBoxStyle.Exclamation, "Invalid format!")
                    Return False
                End If
            Next
            Return True
        Else
            'MsgBox("Enter only numbers [0-9,A-F] or wildcard tokens [*]." & vbNewLine & "There must be 8 characters exactly.", MsgBoxStyle.Exclamation, "Invalid format!")
            Return False
        End If
    End Function

    Private Function RTCTimeValidateHandler(ByVal sender As VarInterface) As Boolean
        Dim ctrl As DateTimePicker = CType(sender.Control, DateTimePicker)

        If ctrl.Value.Year < 2000 Or ctrl.Value.Year > 2255 Then
            Return False
        End If

        If NoUpdate = False Then 'sender.UpdateCurVal = False Then
            TimerRTC.Enabled = False
        End If

        Return True
        'DTDiff = Now() - dtpDevDateTime.Value
    End Function

    Private Function RTCTimeGetHandler(ByVal sender As VarInterface, ByRef retVals() As Object) As Boolean
        DTDiff = Now() - CType(retVals(0), DateTime)
        sender.Value = retVals(0)
        sender.State = VarState.VarState_Current
        TimerRTC.Enabled = True
        Return True
    End Function

    Private Function RFIDSetHandler(ByVal sender As VarInterface) As Object()
        Dim i As Integer
        Dim ctrl As TextBox = CType(sender.Control, TextBox)

        'All characters are good, and we have the right length!
        'If sender.State = VarState.VarState_OutDated Then
        Dim Params(0) As Object
        Dim bAry(4) As Byte
        Dim n As String
        i = 0

        For Each n In ctrl.Text.Split("-")
            bAry(i) = Convert.ToByte(n, 16)
            i += 1
        Next

        Params(0) = bAry
        Return Params
        'Else
        'Return Nothing
        'End If
    End Function

    Private Function RFIDGetHandler(ByVal sender As VarInterface, ByRef retVals() As Object) As Boolean
        Try
            'CType(sender.Control, TextBox).Text = CInt(retVals(0)(0)).ToString("X2") & "-" & CInt(retVals(0)(1)).ToString("X2") & "-" & CInt(retVals(0)(2)).ToString("X2") & "-" & CInt(retVals(0)(3)).ToString("X2") & "-" & CInt(retVals(0)(4)).ToString("X2")
            sender.Value = CInt(retVals(0)(0)).ToString("X2") & "-" & CInt(retVals(0)(1)).ToString("X2") & "-" & CInt(retVals(0)(2)).ToString("X2") & "-" & CInt(retVals(0)(3)).ToString("X2") & "-" & CInt(retVals(0)(4)).ToString("X2")
        Catch ex As Exception
            'CType(sender.Control, TextBox).Text = ""
            sender.Value = ""
            sender.State = VarState.VarState_Unknown
            Return False
        End Try
        Return True
    End Function

    Private Function RFIDValidateHandler(ByVal sender As VarInterface) As Boolean
        Dim i As Integer
        Dim tmpchar As Char
        Dim ctrl As TextBox = CType(sender.Control, TextBox)

        If ctrl.Text Like "??-??-??-??-??" Then
            For i = 0 To 13
                tmpchar = ctrl.Text.Substring(i, 1)
                If Not ((tmpchar >= "0" And tmpchar <= "9") Or (tmpchar >= "A" And tmpchar <= "F") _
                    Or (tmpchar >= "a" And tmpchar <= "f") Or (tmpchar = "-")) Then
                    'MsgBox("Enter only numbers [0-9, A-F]" & vbNewLine & "in the format XX-XX-XX-XX-XX.", MsgBoxStyle.Exclamation, "Invalid format!")
                    Return False
                End If
            Next
            'All good!
            Return True
        Else
            'MsgBox("Enter only numbers [0-9, A-F]" & vbNewLine & "in the format XX-XX-XX-XX-XX.", MsgBoxStyle.Exclamation, "Invalid format!")
            Return False
        End If
    End Function

    Private Sub dtpDevDateTime_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            dtpDevDateTime_ValueChanged(sender, e)
        End If
    End Sub

    Private Sub dtpDevDateTime_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If NoUpdate = True Then Exit Sub


        If dtpDevDateTime.Value.Year > 2000 And dtpDevDateTime.Value.Year < 2255 Then
            Dim tmpErr As MyErr_Enum
            Dim Params() As Object

            DTDiff = Now() - dtpDevDateTime.Value

            ReDim Params(0)
            Params(0) = dtpDevDateTime.Value
            tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_DateTime, Params, Nothing)
            If tmpErr < 0 Then Exit Sub
        Else
            MsgBox("Invalid date.. must be on or after 1/1/2000", , "")
        End If
    End Sub

    Private Sub btnDevTimeSync_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDevTimeSync.Click
        dtpDevDateTime.Value = Now()
        DTDiff = Now() - Now()
        TimerRTC.Enabled = True
    End Sub

    Private Sub btnReadLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReadLog.Click
        If cbDevNames.Text = "" Then
            MsgBox("Not connected!", MsgBoxStyle.OkOnly & vbExclamation, "Problem")
            Exit Sub
        End If

        Dim retVals() As Object = Nothing
        'Dim tmpStr As String
        Dim useRSSI As Boolean = False
        Dim lvLogItems As New List(Of ListViewItem)
        Dim lvSysEventItems As New List(Of ListViewItem)

        If Not lblStatus.Text.StartsWith("Connected") Then Exit Sub

        If VersionStringToInteger(connectedDevVersion) >= VersionStringToInteger("V2.1.0") Then
            useRSSI = True
        End If

        If btnReadLog.Text = "Cancel" Then
            btnReadLog.Text = "Canceling"
            SerComLib.CommFuncs.Tx_Cancel = True
            Exit Sub
        End If

        If lvLogs.Items.Count > 0 Or lvSysEvents.Items.Count > 0 Then
            If MsgBox("Ready to read?" & vbNewLine & "Existing data will be overwitten!", MsgBoxStyle.OkCancel, "Warning!") = MsgBoxResult.Cancel Then
                Exit Sub
            End If
        End If

        Stop_Ping_Loop()
        ControlsEnabled(False)

        pBTransfer.Value = 1
        SplitContainer3.Panel2MinSize = 65
        SplitContainer3.SplitterDistance = 1000
        btnReadLog.Text = "Cancel"

        bgwRxRecords.RunWorkerAsync()
        Do
            pBTransfer.Value = SerComLib.CommFuncs.Tx_PercDone
            lblRecCount.Text = SerComLib.CommFuncs.Tx_RecordsDone.ToString() & "/" & SerComLib.CommFuncs.Tx_RecordCount.ToString()
            lblTransfrRate.Text = SerComLib.CommFuncs.Tx_Rate & " kB/s"
            lblTimeRemain.Text = SerComLib.CommFuncs.Tx_TimeRemain & " s"
            Application.DoEvents()
        Loop While SerComLib.CommFuncs.Tx_InProgress = True

        'Put values into list view...
        Dim times As List(Of DateTime)
        Dim IDs As List(Of UInt32)
        Dim Flags As List(Of Byte)
        Dim RSSIs As List(Of Integer)
        Dim i As Integer
        Dim n As ListViewItem

        Try
            times = mretVals(0)
            IDs = mretVals(1)
            Flags = mretVals(2)
            RSSIs = mretVals(3)

            tagList = New TagList(useRSSI) 'times, IDs, Flags, RSSIs)
            sysEventList = New TagList(useRSSI)
        Catch ex As Exception
            GoTo FinishSub
        End Try


        'Erase all logs from list view control
        lvLogs.Items.Clear()
        lvSysEvents.Items.Clear()
        lvLogs.BeginUpdate()
        lvSysEvents.BeginUpdate()

        For i = 0 To times.Count - 1
            'n = New ListViewItem()
            'n.Text = i
            'n.SubItems.Add(times(i).ToString("dd/MM/yyyy"))
            'n.SubItems.Add(times(i).ToString("HH:mm:ss"))

            If Flags(i) = 2 Then
                'System Message
                sysEventList.AddItem(times(i), IDs(i), Flags(i), RSSIs(i))
            Else
                'Tag Pickup
                tagList.AddItem(times(i), IDs(i), Flags(i), RSSIs(i))
            End If
        Next

        lvSysEvents.VirtualListSize = sysEventList.Count
        lvLogs.VirtualListSize = tagList.Count

FinishSub:
        lvLogs.EndUpdate()
        lvSysEvents.EndUpdate()
        Start_Ping_Loop()
        ControlsEnabled(True)
        SplitContainer3.Panel2MinSize = 30
        SplitContainer3.SplitterDistance = 1000
        btnReadLog.Text = "Read Log"

    End Sub



    Private Sub bgwRxRecords_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bgwRxRecords.DoWork
        Dim tmpErr As MyErr_Enum
        Dim ParamLst(4) As Object
        Dim BaudConfig As BaudRateConfig

        SerComLib.CommFuncs.Tx_InProgress = True
        SerComLib.CommFuncs.Tx_PercDone = 1

        If VersionStringToInteger(connectedDevVersion) >= VersionStringToInteger("V2.1.1") Then

            Dim tmpD As New DownSpeed()

            tmpD = New DownSpeed()

            If tmpD.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                SerComLib.CommFuncs.Tx_InProgress = False
                Exit Sub
            End If

            Try
                BaudConfig = BaudRateConfig.BaudRateConfigs(tmpD.retVal)
            Catch ex As Exception
                SerComLib.CommFuncs.Tx_InProgress = False
                Exit Sub
            End Try

            'BaudConfig = BaudRateConfig.BaudRateConfigs(BaudRateConfig.BaudRateConfigIDs.Baud_115200)
            'BaudConfig = BaudRateConfig.BaudRateConfigs(2) 'CInt(InputBox("BaudConfig Rate...? (0-2)")))

            ParamLst(0) = 0
            ParamLst(1) = UInt32.MaxValue
            ParamLst(2) = BaudConfig.bsel
            ParamLst(3) = BaudConfig.bscale
            ParamLst(4) = BaudConfig.BaudRate
            tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_AllRecordsFast, ParamLst, mretVals)
        Else
            Dim a() As Object
            tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_AllRecordsFast, a, mretVals)
        End If

        If tmpErr < 0 Then
            SerComLib.CommFuncs.Tx_InProgress = False
            Exit Sub
        End If
        SerComLib.CommFuncs.Tx_InProgress = False

    End Sub

    Private Sub cbVerboseMode_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.KeyCode = Keys.Enter Then
            cbVerboseMode_LostFocus(sender, e)
        End If
    End Sub

    Private Sub cbVerboseMode_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs)
        If CInt(cbVerboseMode.Text.Substring(0, 1)) < 256 Then
            'All characters are good, and we have the right length!
            If pbVerboseMode.BackColor = Color.Red Then
                Dim tmpErr As MyErr_Enum
                Dim Params(0) As Object

                Params(0) = CType(cbVerboseMode.Text.Substring(0, 1), Byte)
                tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_VerboseMode, Params, Nothing)
                If tmpErr < 0 Then Exit Sub
                pbVerboseMode.BackColor = Color.Green
            End If
        Else
            MsgBox("Please choose a selection from the list!", MsgBoxStyle.Exclamation, "Invalid Value!")
        End If
    End Sub

    Private Sub btnClearLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClearLog.Click
        If lvLogs.Items.Count > 0 Then
            If MsgBox("This will clear all existing data, are you sure you want to continue?", MsgBoxStyle.YesNo, "Warning!") = MsgBoxResult.Yes Then
                lvLogs.Items.Clear()
                lvSysEvents.Items.Clear()
            End If
        End If
    End Sub

    Private Sub aaa()
        Dim i As Integer
        Dim cur_t As DateTime
        Dim last_t As DateTime
        Dim n As ListViewItem
        Dim ts As TimeSpan
        Dim TAG_TIME_SEP As Decimal = 9.2
        Dim misses As Integer
        'Dim 

        For i = 0 To lvLogs.Items.Count - 1
            n = lvLogs.Items(i)
            cur_t = DateTime.Parse(n.SubItems(1).Text & " " & n.SubItems(2).Text)

            If i > 0 Then
                ts = last_t - cur_t

                If ts.TotalSeconds > TAG_TIME_SEP Then
                    misses += 1
                End If

                last_t = cur_t

            End If
        Next
    End Sub

    Private Sub SaveSystemEventsLog()
        If cbDevNames.Text = "" Then
            If lvSysEvents.Items.Count > 0 And Vars(VarNameList.DevVersion).Value <> "" Then

            Else
                MsgBox("Not connected and no valid system logs.", MsgBoxStyle.OkOnly & vbExclamation, "Problem")
                Exit Sub
            End If
        End If

        Dim outFile As String = ""
        Dim tmpLine As String
        Dim i, j As Integer
        Dim cols As Integer = 4
        Dim useAll As Boolean = False

        If lvLogs.Items.Count > 0 Then
            Dim n As ListViewItem

            saveDialog.FileName = Now.ToString("dd-MM-yyyy") & "_" & Vars(VarNameList.LoggerName).OrigVal & "_SystemLogs"
            If saveDialog.ShowDialog() <> Windows.Forms.DialogResult.OK Then
                Exit Sub
            End If

            If lvSysEvents.SelectedIndices.Count <= 1 Then
                useAll = True
            End If

            ControlsEnabled(False)
            pBTransfer.Value = 1
            SplitContainer3.Panel2MinSize = 65
            SplitContainer3.SplitterDistance = 1000
            'btnReadLog.Text = "Cancel"

            'For Each n In lvLogs.Items
            Dim sw As System.IO.StreamWriter
            Try
                sw = New System.IO.StreamWriter(saveDialog.FileName)

                sw.WriteLine("RecordNumber,Date,Time,Command,Data")

                For i = 0 To lvSysEvents.Items.Count - 1
                    n = lvLogs.Items(i)
                    If useAll = True OrElse n.Selected = True Then
                        tmpLine = ""
                        For j = 0 To cols
                            tmpLine &= n.SubItems(j).Text & ","
                        Next
                        tmpLine = tmpLine.Substring(0, tmpLine.Length - 1) '& vbCrLf
                        sw.WriteLine(tmpLine)
                    End If
                    If i Mod 100 = 0 Then

                        pBTransfer.Value = (i / (lvLogs.Items.Count - 1)) * 100
                        lblRecCount.Text = (i + 1).ToString & "/" & lvLogs.Items.Count.ToString
                        lblTransfrRate.Text = ""
                        lblTimeRemain.Text = ""
                        Application.DoEvents()
                    End If
                Next
                sw.Flush()
                sw.Close()
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly + MsgBoxStyle.Critical)
                GoTo Abort
            End Try

Abort:
            lvLogs.EndUpdate()
            lvSysEvents.EndUpdate()
            ControlsEnabled(True)
            SplitContainer3.Panel2MinSize = 30
            SplitContainer3.SplitterDistance = 1000

        End If
    End Sub

    Private Sub SaveWIDLogs()
        If cbDevNames.Text = "" Then
            If lvLogs.Items.Count > 0 And Vars(VarNameList.DevVersion).Value <> "" Then

            Else
                MsgBox("Not connected and no valid logs.", MsgBoxStyle.OkOnly & vbExclamation, "Problem")
                Exit Sub
            End If
        End If

        Dim outFile As String = ""
        Dim tmpLine As String
        Dim i, j As Integer
        Dim cols As Integer = 4
        Dim useAll As Boolean = False

        Try
            If VersionStringToInteger(connectedDevVersion) >= VersionStringToInteger("V2.1.0") Then cols = 5
        Catch ex As Exception
            MsgBox("Invalid Version!", MsgBoxStyle.OkOnly & vbExclamation, "Problem")
        End Try


        If lvLogs.Items.Count > 0 Then
            Dim n As ListViewItem

            saveDialog.FileName = Now.ToString("dd-MM-yyyy") & "_" & Vars(VarNameList.LoggerName).OrigVal & "_WIDLogs"
            If saveDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                ' My.Computer.FileSystem.WriteAllText(saveDialog.FileName, outFile, False)
            Else
                Exit Sub
            End If

            If lvLogs.SelectedIndices.Count <= 1 Then
                useAll = True
            End If

            ControlsEnabled(False)
            pBTransfer.Value = 1
            SplitContainer3.Panel2MinSize = 65
            SplitContainer3.SplitterDistance = 1000
            'btnReadLog.Text = "Cancel"

            'For Each n In lvLogs.Items
            Dim sw As System.IO.StreamWriter
            Try
                sw = New System.IO.StreamWriter(saveDialog.FileName)

                sw.WriteLine("RecordNumber,Date,Time,TagID,Flags,RSSI")

                For i = 0 To lvLogs.Items.Count - 1
                    n = lvLogs.Items(i)
                    If useAll = True OrElse n.Selected = True Then
                        tmpLine = ""
                        For j = 0 To cols
                            tmpLine &= n.SubItems(j).Text & ","
                        Next
                        tmpLine = tmpLine.Substring(0, tmpLine.Length - 1) '& vbCrLf
                        sw.WriteLine(tmpLine)
                        'outFile &= tmpLine
                    End If
                    If i Mod 100 = 0 Then

                        pBTransfer.Value = (i / (lvLogs.Items.Count - 1)) * 100
                        lblRecCount.Text = (i + 1).ToString & "/" & lvLogs.Items.Count.ToString
                        lblTransfrRate.Text = ""
                        lblTimeRemain.Text = ""
                        Application.DoEvents()
                    End If
                Next
                sw.Flush()
                sw.Close()
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly + MsgBoxStyle.Critical)
                GoTo Abort
            End Try

Abort:
            lvLogs.EndUpdate()
            lvSysEvents.EndUpdate()
            ControlsEnabled(True)
            SplitContainer3.Panel2MinSize = 30
            SplitContainer3.SplitterDistance = 1000

        End If
    End Sub

    Private Sub btnSaveLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveLog.Click

        If TabControlRecords.SelectedTab.Name = tpTags.Name Then
            SaveWIDLogs()
        ElseIf TabControlRecords.SelectedTab.Name = tpSystemEvents.Name Then
            SaveSystemEventsLog()
        End If

    End Sub

    Private Sub updateSerialText()
        Dim tmpTxt As String

        'Don't ever interrupt an ongoing process
        If SerComLib.Port_InUse = False Then
            If SerComLib.Ser.BytesToRead > 0 Then
                'get text from buffer
                tmpTxt = SerComLib.Ser.ReadExisting().Replace(vbNullChar, "")
                alltxt = alltxt & tmpTxt
                If alltxt.Length = 0 Then Exit Sub
                'add to text box
                EditrtbSerOutText(tmpTxt)
                'scroll to end or text box
                rtbSerOutScroll2End()
                'process the data for messages!
                Dim a As Integer = InStrRev(alltxt, vbLf, alltxt.Length) - 1
                If a > 0 Then
                    If a > strScanPos Then
                        Dim unProcLines() As String = alltxt.Substring(strScanPos, a - strScanPos + 1).Split(vbLf)
                        Dim i As Integer
                        For i = 0 To unProcLines.Length - 1
                            If SerTextCmdActive <> 5 Then
                                If unProcLines(i).StartsWith("Turning GM862 On!") Then 'unProcLines(i).Contains("Turning GM862 On!") Then
                                    SerTextCmdActive = 1
                                    ControlsEnabledAsync(False)
                                    EditStatusLabelText("Sending Email...")
                                ElseIf unProcLines(i).StartsWith("Next GPRS Send Time:") Then 'Or unProcLines(i).Contains("Send Data Failed!") Or unProcLines(i).Contains("Send Data Succeeded!") Then
                                    SerTextCmdActive = 0
                                    EditStatusLabelText("Connected (" & FindSelectedPortNum() & ")")
                                    ControlsEnabledAsync(True)
                                ElseIf unProcLines(i).Contains("PWRDWN!") Then
                                    SerTextCmdActive = 2
                                    EditStatusLabelText("No Battery, Asleep...")
                                    ControlsEnabledAsync(True)
                                ElseIf unProcLines(i).StartsWith("Low Battery!") Then
                                    SerTextCmdActive = 2
                                    EditStatusLabelText("Low Battery, Asleep...")
                                    ControlsEnabledAsync(True)
                                ElseIf unProcLines(i).StartsWith("Awake...") Then
                                    SerTextCmdActive = 3
                                    EditStatusLabelText("Waking Up...")
                                    ControlsEnabledAsync(True)
                                ElseIf unProcLines(i).StartsWith("Wake from bat back mode.") Then
                                    SerTextCmdActive = 0
                                    EditStatusLabelText("Connected (" & FindSelectedPortNum() & ")")
                                    ControlsEnabledAsync(True)
                                ElseIf unProcLines(i).Contains("Next_GPRS_Real(RIY1):") Then
                                    SerTextCmdActive = 4
                                    EditStatusLabelText("Reeling in the years...")
                                    FindNameInVarLst(VarNameList.SendEmailNext).State = VarState.VarState_OutDated
                                ElseIf unProcLines(i).Contains("Next_GPRS_Real(RIY2):") Then
                                    SerTextCmdActive = 0
                                    EditStatusLabelText("Connected (" & FindSelectedPortNum() & ")")
                                    ControlsEnabledAsync(True)
                                ElseIf unProcLines(i).StartsWith("RX ID: 0x") Then
                                    FindNameInVarLst(VarNameList.Records).State = VarState.VarState_OutDated
                                ElseIf unProcLines(i).StartsWith("Configure GM862...") Then
                                    SerTextCmdActive = 6
                                    EditStatusLabelText("Configuring GM862...")
                                    ControlsEnabledAsync(False)
                                ElseIf unProcLines(i).StartsWith("GM862 Configure Done.") Then
                                    SerTextCmdActive = 0
                                    EditStatusLabelText("Connected (" & FindSelectedPortNum() & ")")
                                    ControlsEnabledAsync(True)
                                End If
                            End If
                        Next
                    End If
                    If a >= strScanPos Then
                        strScanPos = a + 1
                    End If
                End If
            End If
        End If
    End Sub

    Private Delegate Sub EditStatusLabelTextDelegate(ByVal text As String)

    Public Sub EditStatusLabelText(ByVal [text] As String)
        If lblStatus.InvokeRequired = True Then
            Dim del As New EditStatusLabelTextDelegate(AddressOf EditStatusLabelText)
            lblStatus.Invoke(del, New Object() {[text]})
        Else
            lblStatus.Text = text
        End If
    End Sub

    'Private Delegate Sub EditVarStateDelegate(ByRef var As VarInterface, ByVal state As VarState)

    'Public Sub EditVarState(ByRef var As VarInterface, ByVal state As VarState)
    '    If lblStatus.InvokeRequired = True Then
    '        Dim del As New EditVarStateDelegate(AddressOf EditVarState)
    '        var.Invoke(del, New Object() {var, state})
    '    Else

    '    End If
    'End Sub

    Private Delegate Sub EditrtbSerOutTextDelegate(ByVal text As String)

    Public Sub EditrtbSerOutText(ByVal text As String)
        If rtbSerOut.InvokeRequired = True Then
            Dim del As New EditrtbSerOutTextDelegate(AddressOf EditrtbSerOutText)
            rtbSerOut.Invoke(del, New Object() {text})
        Else
            rtbSerOut.AppendText(text)
            rtbSerOut.SelectionStart = rtbSerOut.Text.Length
            rtbSerOut.ScrollToCaret()
            'rtbSerOut.Select(rtbSerOut.TextLength, 0)
            'rtbSerOut.Text = text
            'rtbSerOut.Text = "abc"
        End If
    End Sub

    Private Delegate Sub EditcbDevNamesTextDelegate(ByVal text As String)

    Public Sub EditcbDevNamesText(ByVal [text] As String)
        If cbDevNames.InvokeRequired = True Then
            Dim del As New EditcbDevNamesTextDelegate(AddressOf EditcbDevNamesText)
            cbDevNames.Invoke(del, New Object() {[text]})
        Else
            cbDevNames.Text = text
        End If
    End Sub

    Private Delegate Function GetrtbSerOutTextDelegate() As String

    Public Function GetrtbSerOutText() As String
        If lblStatus.InvokeRequired = True Then
            Dim del As New GetrtbSerOutTextDelegate(AddressOf GetrtbSerOutText)
            Return CStr(rtbSerOut.Invoke(del))
        Else
            Return rtbSerOut.Text
        End If
    End Function

    Private Delegate Sub rtbSerOutScroll2EndDelegate()

    Public Sub rtbSerOutScroll2End()
        If lblStatus.InvokeRequired = True Then
            Dim del As New rtbSerOutScroll2EndDelegate(AddressOf rtbSerOutScroll2End)
            rtbSerOut.Invoke(del)
        Else
            rtbSerOut.Select(rtbSerOut.TextLength, 0)
            rtbSerOut.ScrollToCaret()
            rtbSerOut.Select(rtbSerOut.TextLength, 0)
        End If
    End Sub

    Public Sub ControlsEnabledAsync(ByVal enabled As Boolean)
        Dim c As Control
        For Each c In CtrlListComms
            SingleControlEnabledAsync(c, enabled)
        Next
    End Sub

    Public Delegate Sub SingleControlEnabledDelegate(ByRef ctrl As Control, ByVal enabled As Boolean)

    Public Sub SingleControlEnabledAsync(ByRef ctrl As Control, ByVal enabled As Boolean)
        If ctrl.InvokeRequired = True Then
            Dim del As New SingleControlEnabledDelegate(AddressOf SingleControlEnabledAsync)
            ctrl.Invoke(del, New Object() {ctrl, enabled})
        Else
            ctrl.Enabled = enabled
        End If
    End Sub

    'ControlsEnabled

    Private Sub bgwPing_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles bgwPing.DoWork
        Dim tmpErr As MyErr_Enum
        Dim RetVals(0) As Object
        Dim devID As UInt16
        Dim tmpVar As VarInterface
        Dim startTime As DateTime = Now()
        Dim StartTimetxtRead As DateTime
        Dim receivedSerial As Boolean
        Dim PingAttempts As Integer = 0

        Threading.Thread.CurrentThread.Priority = Threading.ThreadPriority.AboveNormal

        Do
            startTime = Now()

            'If VarInterface.waitforParam(SerComLib.Port_InUse, False, 5000) = False Then GoTo errexit

            If SerComLib.Port_InUse = False Then
                If SerComLib.Ser.IsOpen = True Then
                    StartTimetxtRead = Now() '- New TimeSpan(1, 0, 0)
                    receivedSerial = False
                    Try
                        While SerComLib.Ser.BytesToRead > 0 Or (Now() - StartTimetxtRead).TotalMilliseconds < 1000
                            If SerComLib.Ser.BytesToRead > 0 Then StartTimetxtRead = Now()
                            updateSerialText()
                            receivedSerial = True
                            PingAttempts = 0
                        End While
                    Catch ex As Exception
                        If SerComLib.Ser.IsOpen = False Then
                            GoTo errexit
                        End If
                    End Try

                    If receivedSerial = False Then
                        If SerTextCmdActive = 0 Then
                            'startTime = Now()
                            Try
                                tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_DevTypeID, Nothing, RetVals)
                            Catch ex As Exception
                                If SerComLib.Ser.IsOpen = False Then
                                    GoTo errexit
                                Else
                                    tmpErr = MyErr_Enum.MyErr_Unknown
                                End If
                            End Try

                            'Debug.Print((Now() - startTime).TotalMilliseconds)

                            If tmpErr = MyErr_Enum.MyErr_Success Then
                                devID = CType(RetVals(0), UInt16)
                                If devID = SerDev_LoggerV2_ComFuncs.DevID Then
                                    'Our device is still there!
                                    PingAttempts = 0
                                    GoTo allgood
                                End If
                            End If
                            'otherwise.. error!
                            PingAttempts += 1
                            If PingAttempts > 3 Then
                                GoTo errexit
                            End If
                        End If
                    End If
                Else
                    GoTo errexit
                End If

            End If
allgood:
            'Threading.Thread.Yield()
            'Wait for 2 seconds (minimum) between every loop
            Do
                Threading.Thread.Sleep(10)
                If SerComLib.Ser.IsOpen Then
                    If SerComLib.Ser.BytesToRead > 0 Then Exit Do
                End If
            Loop While (Now - startTime).TotalMilliseconds < 5000 And cancelPingLoop = False
        Loop While cancelPingLoop = False
        cancelPingLoop = False
        Exit Sub

errExit:
        'otherwise.. error!
        'Set all vars to unknown
        For Each tmpVar In Vars
            tmpVar.State = VarState.VarState_Unknown
        Next
        cancelPingLoop = False
        'TODO[ ]: Actually update the line below.. cross thread call needed
        EditStatusLabelText("Not Connected")
        EditcbDevNamesText("")
        SerComLib.Ser.Close()
        MsgBox("Disconnected!", , "")
    End Sub

    Public Sub DisconnectDevice()
        Stop_Ping_Loop()
        If SerComLib.Ser.IsOpen Then
            SerComLib.Ser.Close()
        End If

        For Each tmpVar In Vars
            tmpVar.State = VarState.VarState_Unknown
        Next

        cbDevNames.Text = ""

        lblStatus.Text = "Not Connected"
        NoUpdate = False
        ControlsEnabled(True)
    End Sub

    Private Sub ReadValsInCat(ByVal Cat As VarCatPage, ByVal force As Boolean)
        Dim tmpVar As VarInterface

        If cbDevNames.Text <> "" Then
            ControlsEnabled(False)
            Stop_Ping_Loop()
            For Each tmpVar In Vars
                If tmpVar.Category = Cat Then
                    If ((tmpVar.State <> VarState.VarState_Current) Or force) Then tmpVar.ReadFromDev()
                End If
            Next
            Start_Ping_Loop()
            ControlsEnabled(True)
        Else
            MsgBox("Not connected!", MsgBoxStyle.OkOnly & vbExclamation, "Problem")
        End If
    End Sub

    Private Sub WriteValsInCat(ByVal Cat As VarCatPage)
        Dim tmpVar As VarInterface

        If cbDevNames.Text <> "" Then
            ControlsEnabled(False)
            Stop_Ping_Loop()
            For Each tmpVar In Vars
                If tmpVar.Category = Cat Then
                    If tmpVar.State <> VarState.VarState_Current Then tmpVar.WriteToDev()
                End If
            Next
            Start_Ping_Loop()
            ControlsEnabled(True)
        Else
            MsgBox("Not connected!", MsgBoxStyle.OkOnly & vbExclamation, "Problem")
        End If
    End Sub

    Private Sub btnReadVals_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReadVals.Click
        ReadValsInCat(VarCatPage.General, True)
    End Sub

    Private Sub btnWriteVals_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWriteVals.Click
        WriteValsInCat(VarCatPage.General)
    End Sub

    Private Sub btnReadRFVals_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReadRFVals.Click
        ReadValsInCat(VarCatPage.RFSettings, False)
    End Sub

    Private Sub btnWriteRFVals_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWriteRFVals.Click
        WriteValsInCat(VarCatPage.RFSettings)
    End Sub

    Private Sub btnReadGSMVals_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReadGSMVals.Click
        ReadValsInCat(VarCatPage.GSMSettings, True)
    End Sub

    Private Sub btnWriteGSMVals_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnWriteGSMVals.Click
        WriteValsInCat(VarCatPage.GSMSettings)
    End Sub

    Public Sub Stop_Ping_Loop()
        cancelPingLoop = True
        Do
            Application.DoEvents()
        Loop While bgwPing.IsBusy = True
    End Sub

    Public Sub Start_Ping_Loop()
        cancelPingLoop = False
        bgwPing.RunWorkerAsync()
    End Sub

    Private Sub btnEraseDataflash_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEraseDataflash.Click
        Dim tmpErr As MyErr_Enum

        If MsgBox("Are you sure you want to erase ALL records on the device?" & vbNewLine & "This may take up to 2 minutes", MsgBoxStyle.YesNo, "Warning!") = MsgBoxResult.Yes Then
            btnEraseDataflash.Text = "..."
            Me.Enabled = False
            tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Cmd_EraseDataflash, Nothing, Nothing)
            If tmpErr = MyErr_Enum.MyErr_Success Then
                'MsgBox("Erase Succeeded")
                My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Asterisk)
            ElseIf tmpErr < 0 Then
                'MsgBox("Erase Failed")
                My.Computer.Audio.PlaySystemSound(Media.SystemSounds.Exclamation)
            End If
            Me.Enabled = True
            btnEraseDataflash.Text = "Erase"

            'Update Record Count
            FindNameInVarLst(VarNameList.Records).ReadFromDev()
        End If
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        DevPorts.Add("COM1")
        DevNames.Add("FAKELOGGER1")
        cbDevNames.Items.Add("[" & DevNames.Count & "] " & "FAKELOGGER1")
    End Sub

    Private Sub rtbSerOut_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles rtbSerOut.KeyPress
        If SerComLib.Port_InUse = False And SerTextCmdActive > 0 Then
            SerComLib.Ser.Write(e.KeyChar)
        End If
    End Sub

    Private Sub rtbSerOut_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.KeyCode = Keys.Escape Then
            If MsgBox("Are you sure you want to clear the data?", vbYesNo, "Warning!") = MsgBoxResult.Yes Then
                rtbSerOut.Text = ""
                alltxt = ""
                strScanPos = 0
            End If
        End If
    End Sub

    Private Sub chkGM862BridgeMode_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkGM862BridgeMode.CheckedChanged
        If chkGM862BridgeMode.Tag = "waiting" Then Exit Sub

        chkGM862BridgeMode.Tag = New String("waiting")

        If chkGM862BridgeMode.Checked = True Then

            Dim tmpErr As MyErr_Enum

            Stop_Ping_Loop()
            If waitforParam(SerComLib.Port_InUse, False, 1000) = False Then tmpErr = MyErr_Enum.MyErr_PortInUse
            tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Cmd_GM862BridgeMode, Nothing, Nothing)
            If tmpErr < 0 Then
                MsgBox("Could not enter bridge mode.", , "")
                chkGM862BridgeMode.Checked = False
            Else
                SerTextCmdActive = 5
                lblStatus.Text = "GM862 Bridge Mode"
            End If
            Start_Ping_Loop()
        Else
            If SerComLib.Port_InUse = False And SerTextCmdActive > 0 Then
                Stop_Ping_Loop()
                SerComLib.Ser.Write(New Byte() {&H3}, 0, 1)
                chkGM862BridgeMode.Text = "Exiting..."
                SerComLib.DoEventDelay(6000)
                SerTextCmdActive = 0
                chkGM862BridgeMode.Text = "GM862 Bridge Mode"
                EditStatusLabelText("Connected (" & FindSelectedPortNum() & ")")
                If SerComLib.Ser.BytesToRead > 0 Then
                    SerComLib.Ser.ReadExisting()
                End If
                Start_Ping_Loop()
            Else
                chkGM862BridgeMode.Checked = True
            End If

        End If

        chkGM862BridgeMode.Tag = Nothing

    End Sub

    Private Sub lblStatus_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles lblStatus.MouseDoubleClick
        If e.Button = Windows.Forms.MouseButtons.Right Then
            If chkGM862BridgeMode.Visible = True Then
                'go to simple mode
                TabPage7.Parent = tcHiddenControls
                TabPage4.Parent = tcHiddenControls
                tpWIDTest.Parent = tcHiddenControls
            Else
                'go to advanced mode
                TabPage7.Parent = TabControlMain
                TabPage4.Parent = TabControlMain
                tpWIDTest.Parent = TabControlMain
                'TabControlMain.TabPages(TabControlMain.TabPages.Count - 1)
            End If

            chkGM862BridgeMode.Visible = Not chkGM862BridgeMode.Visible
            btnConfigureGM862.Visible = Not btnConfigureGM862.Visible
            splitContainer2.Panel2Collapsed = Not splitContainer2.Panel2Collapsed

        End If
    End Sub

    Private Sub btnConfigureGM862_Click(sender As System.Object, e As System.EventArgs) Handles btnConfigureGM862.Click
        Dim tmpErr As MyErr_Enum
        If waitforParam(SerComLib.Port_InUse, False, 1000) = False Then Exit Sub
        tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Cmd_GM862_ConfigModule, Nothing, Nothing)

    End Sub

    Private Sub Button2_Click(sender As System.Object, e As System.EventArgs) Handles Button2.Click
        'Dim _commProp As New COMMPROP()
        Dim portName As String = InputBox("Port Name (eg. COM3)")
        'Dim hFile As IntPtr = CreateFile("\\.\" & portName, 0, 0, IntPtr.Zero, 3, &H80, _
        ' IntPtr.Zero)
        'GetCommProperties(hFile, _commProp)

        Dim _port As New SerialPort(portName)
        _port.Open()
        Dim p As Object = _port.BaseStream.[GetType]().GetField("commProp", BindingFlags.Instance Or BindingFlags.NonPublic).GetValue(_port.BaseStream)
        Dim bv As Int32 = DirectCast(p.[GetType]().GetField("dwSettableBaud", BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.[Public]).GetValue(p), Int32)
        Dim mb As Int32 = DirectCast(p.[GetType]().GetField("dwMaxBaud", BindingFlags.Instance Or BindingFlags.NonPublic Or BindingFlags.[Public]).GetValue(p), Int32)

        _port.Close()

        MsgBox("MaxBaud = 0x" & mb.ToString("X8") & vbNewLine & "SettableBaud = 0x" & bv.ToString("X8"))


    End Sub

    Private Sub frmMain_ResizeBegin(sender As Object, e As System.EventArgs) Handles Me.ResizeBegin
        Me.SuspendLayout()
    End Sub

    Private Sub frmMain_ResizeEnd(sender As Object, e As System.EventArgs) Handles Me.ResizeEnd
        Me.UpdateBounds()
        Me.Update()
        Me.ResumeLayout()
    End Sub

    Private Sub btnAbout_Click(sender As System.Object, e As System.EventArgs) Handles btnAbout.Click
        About.ShowDialog()
    End Sub


    Private Sub btnClear_Click(sender As Object, e As EventArgs) Handles btnClear.Click
        If MsgBox("Are you sure you want to clear the data?", vbYesNo, "Warning!") = MsgBoxResult.Yes Then
            rtbSerOut.Text = ""
            alltxt = ""
            strScanPos = 0
        End If
    End Sub

    Private Sub updatechkPrintTags(cbVerboseModeIndex As Integer)
        chkPrintTags_UpdateValue = False
        With chkPrintTags
            Select Case cbVerboseModeIndex
                Case 0
                    .Text = "Print Tags: Off"
                    .CheckState = CheckState.Unchecked
                Case 1
                    .Text = "Print Tags: Tag Only"
                    .CheckState = CheckState.Checked
                Case 2
                    .Text = "Print Tags: Tag and Time"
                    .CheckState = CheckState.Indeterminate
                Case Else
                    .Text = "Print Tags: Off" '"Print Tags: Not resolved"
                    .CheckState = CheckState.Unchecked
            End Select
        End With
        chkPrintTags_UpdateValue = True
    End Sub

    Private Sub chkPrintTags_CheckStateChanged(sender As Object, e As EventArgs) Handles chkPrintTags.CheckStateChanged
        Static runningChange As Boolean = False

        If runningChange Then Exit Sub

        '1 - Silent
        '2 - Print Tag ID
        '3 - Pint Tag ID and Time
        '4 - Print Tag ID, Time and Address

        runningChange = True
        If cbDevNames.Text <> "" Then
            With chkPrintTags
                Select Case .CheckState
                    Case CheckState.Checked
                        .Text = "Print Tags: Tag Only"
                        cbVerboseMode.SelectedIndex = 1
                    Case CheckState.Indeterminate
                        .Text = "Print Tags: Tag and Time"
                        cbVerboseMode.SelectedIndex = 2
                    Case CheckState.Unchecked
                        .Text = "Print Tags: Off"
                        cbVerboseMode.SelectedIndex = 0
                End Select

                If chkPrintTags_UpdateValue = True Then
                    ControlsEnabled(False)
                    Vars(TagStateVarID).WriteToDev()
                    ControlsEnabled(True)
                End If

                If Vars(TagStateVarID).State <> VarState.VarState_Current Then
                    'revert!
                    '.CheckState = (.CheckState - 1 + 3) Mod 2
                    updatechkPrintTags(Vars(TagStateVarID).OrigVal)
                End If


            End With
        Else
            MsgBox("Not connected!", MsgBoxStyle.OkOnly & vbExclamation, "Problem")
        End If
        runningChange = False
    End Sub

    Private Sub chkPrintTags_CheckedChanged(sender As Object, e As EventArgs) Handles chkPrintTags.CheckedChanged

    End Sub

    Private Sub chkPrintTags_Click(sender As Object, e As EventArgs) Handles chkPrintTags.Click

    End Sub

    Private Sub btnDisconnect_Click(sender As System.Object, e As System.EventArgs) Handles btnDisconnect.Click
        DisconnectDevice()
    End Sub

    Private abortWIDTest As Boolean = False

    Private Sub WidTestSave(lviList As List(Of ListViewItem))
        Dim outfile As String = ""
        Dim tmpline As String

        Dim n As ListViewItem

        '
        For Each n In lviList
            tmpline = ""
            For i = 0 To 6 - 1
                tmpline &= n.SubItems(i).Text & ","
            Next
            tmpline = tmpline.Substring(0, tmpline.Length - 1) & vbCrLf
            outfile &= tmpline
        Next

        If saveDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
            My.Computer.FileSystem.WriteAllText(saveDialog.FileName, outfile, False)
        Else
            Exit Sub
        End If

        Dim tmpParser As New tagParser
        tmpParser.ParseText(outfile, CDec(txtTagTestExpectedPeriod.Text))

        Dim tmpstr As String
        Dim lvListItems As New List(Of ListViewItem)

        lvParsedTags.Items.Clear()

        While lvParsedTags.Columns.Count > 3
            lvParsedTags.Columns.RemoveAt(lvParsedTags.Columns.Count - 1)
        End While

        For Each tmpstr In tmpParser.tagList
            lvParsedTags.Columns.Add(tmpstr)
            lvParsedTags.Columns(lvParsedTags.Columns.Count - 1).Width = 80
        Next

        For i = 0 To tmpParser.LogCollection.Count - 1
            n = New ListViewItem
            n.Text = i

            n.SubItems.Add(tmpParser.LogCollection(i).Time.ToString("dd/MM/yyyy"))
            n.SubItems.Add(tmpParser.LogCollection(i).Time.ToString("HH:mm:ss.f"))
            For j = 0 To tmpParser.tagList.Count - 1
                n.SubItems.Add(IIf(tmpParser.LogCollection(i).Tags(j) = -1, "", Math.Round(tmpParser.LogCollection(i).Tags(j), 2)))
            Next
            lvListItems.Add(n)
        Next

        lvParsedTags.Items.AddRange(lvListItems.ToArray)
        Dim ws As Integer
        Try
            ws = InputBox("Enter moving average window size")
        Catch ex As Exception
            ws = 50
        End Try
        tmpParser.chrtMovingAvg(chrtTagParse, True, ws)
        'save other files:
        outfile = "WID ID, Tx Period, Avg RSSI, RSSI Std Dev, Measurement Count" & vbNewLine
        For Each n In lvWidTest.Items
            tmpline = ""
            For i = 0 To 5 - 1
                tmpline &= n.SubItems(i).Text & ","
            Next
            tmpline = tmpline.Substring(0, tmpline.Length - 1) & vbCrLf
            outfile &= tmpline
        Next

        My.Computer.FileSystem.WriteAllText(System.IO.Path.GetDirectoryName(saveDialog.FileName) + "\" + System.IO.Path.GetFileNameWithoutExtension(saveDialog.FileName) & "_TestResults.csv", outfile, False)
        ChartBmpToFile(chrtTagParse, System.IO.Path.GetDirectoryName(saveDialog.FileName) + "\" + System.IO.Path.GetFileNameWithoutExtension(saveDialog.FileName) & "_RSSIChart.png")
        ' chrtTagParse.SaveImage(System.IO.Path.GetDirectoryName(saveDialog.FileName) + "\" + System.IO.Path.GetFileNameWithoutExtension(saveDialog.FileName) & "_RSSIChart.png", System.Drawing.Imaging.ImageFormat.Png)
    End Sub

    Private Sub ChartBmpToFile(chrt As Windows.Forms.DataVisualization.Charting.Chart, file As String)
        ''Dim c As chart
        'Dim bmp As New System.Drawing.Bitmap(control.Width, control.Height)
        'control.DrawToBitmap(bmp, control.DisplayRectangle)

        'bmp.Save(file, System.Drawing.Imaging.ImageFormat.Png)

        Dim oSize As Size = chrt.Size
        Dim oDock As DockStyle = chrt.Dock

        chrt.Dock = DockStyle.None
        chrt.Size = New Size(1024, 600)

        chrt.SaveImage(file, System.Drawing.Imaging.ImageFormat.Png)

        chrt.Size = oSize
        chrt.Dock = oDock
    End Sub

    Private Sub btnStartWIDTest_Click(sender As System.Object, e As System.EventArgs) Handles btnStartWIDTest.Click
        If btnStartWIDTest.Text = "Stop" Then
            abortWIDTest = True
            Exit Sub
        End If

        btnStartWIDTest.Text = "Stop"
        abortWIDTest = False

        Dim txtLen As Long = rtbSerOut.Text.Length
        Dim txtlenMarker As Long = 0
        Dim changedText As String = ""
        Dim lines() As String
        Dim alltagsDone As Boolean
        Dim nameoftag As String
        Dim readRSSI As Decimal
        Dim curT As TagTestPickupStats

        'Dim m As ListViewItem

        'add reference tag
        lviList = New List(Of ListViewItem)
        tags = New List(Of TagTestPickupStats)
        tags.Add(New TagTestPickupStats(txtWidTestRefTagID.Text))

        'start a loop  count out a certain number of pickups for the main tag and for the reference tag
        Do
            'check if text is updated
            If rtbSerOut.Text.Length > txtLen Then
                txtlenMarker = rtbSerOut.Text.Length
                'get segment of extra text
                changedText = rtbSerOut.Text.Substring(txtLen, txtlenMarker - txtLen)
                lines = changedText.Split(vbLf)

                'process lines
                For Each line As String In lines
                    txtLen += line.Length
                    line.Replace(vbCr, "")

                    Try
                        nameoftag = GetTagIDFromLineString(line)
                        readRSSI = GetRssiFromLineString(line)

                        curT = findTagInList(tags, nameoftag)

                        If IsNothing(curT) Then
                            tags.Add(New TagTestPickupStats(nameoftag))
                            tags.Last.AddPickup(readRSSI, DateTime.Now)
                        Else
                            curT.AddPickup(readRSSI, DateTime.Now)
                        End If
                        'add pickup to list
                        lviList.Add(New ListViewItem({0, Now.ToString("dd/MM/yyyy"), Now.ToString("HH:mm:ss"), nameoftag, 3, readRSSI})) '.ToString("X8")}))
                    Catch ex As Exception

                    End Try
                Next

                If tags.Count = 0 Then
                    alltagsDone = False
                Else
                    alltagsDone = True
                    For Each t As TagTestPickupStats In tags
                        If t.Measurements < CInt(txtWIDTestMeasurementCount.Text) Then
                            alltagsDone = False
                            Exit For
                        End If
                    Next
                End If

            End If

            updateWidStatsInTable(tags)

            Application.DoEvents()
        Loop Until abortWIDTest = True Or alltagsDone = True

        'calculate stats
        updateWidStatsInTable(tags)

ErrExit:
        updateWidStatsInTable(tags)
        WidTestSave(lviList)
        abortWIDTest = False
        btnStartWIDTest.Text = "Start"
    End Sub

    Private Function isTagNew(TagList As List(Of TagTestPickupStats), tagName As String) As Boolean
        For Each Tag As TagTestPickupStats In TagList
            If Tag.TagName = tagName Then
                Return False
            End If
        Next
        Return True
    End Function

    Private Function GetRssiFromLineString(line As String) As Decimal
        Dim pos1 As Integer = InStr(line, "RSSI: ")
        If pos1 Then
            Return CDec(line.Substring(pos1 + 6 - 1).TrimEnd.TrimStart)
        Else
            Throw New Exception("invalid string")
        End If
    End Function

    Private Function GetTagIDFromLineString(line As String) As Decimal
        Dim pos1 As Integer = InStr(line, "ID: 0x")
        If pos1 Then
            Return line.Substring(pos1 + 6 - 1, 8)
        Else
            Throw New Exception("invalid string")
        End If
    End Function

    Private Sub updateWidStatsInTable(TagList As List(Of TagTestPickupStats))
        Dim n As ListViewItem

        lvWidTest.Items.Clear()
        lvWidTest.BeginUpdate()
        For Each Tag As TagTestPickupStats In TagList
            If Tag.Measurements > 0 Then
                Try
                    n = New ListViewItem({Tag.TagName, Math.Round(Tag.AvgTimeBetweenPickups, 2), Math.Round(Tag.RSSI_Mean, 2), Math.Round(Tag.RSSI_SampleStdDev, 2), Tag.Measurements})
                    lvWidTest.Items.Add(n)
                Catch ex As Exception

                End Try
            End If
        Next
        lvWidTest.EndUpdate()
    End Sub

    Private Function findTagInList(TagList As List(Of TagTestPickupStats), tagName As String) As TagTestPickupStats
        For Each Tag As TagTestPickupStats In TagList
            If Tag.TagName = tagName Then
                Return Tag
            End If
        Next
        Return Nothing
    End Function

    Private Sub btnRFDefaults_Click(sender As System.Object, e As System.EventArgs) Handles btnRFDefaults.Click
        FindNameInVarLst(VarNameList.RFFilter).Value = "********"
        'setup for 50kbps as for Cheryl's rat project
        FindNameInVarLst(VarNameList.RFParams).Value = "0xD3 0x91 0x06 0x04 0x04 0x00 0x00 0x0A 0x00 0x5D " + _
                                                       "0x93 0xB1 0x6A 0xF8 0x13 0x22 0xF8 0x50 0x07 0x30 " + _
                                                       "0x14 0x1D 0x1C 0xC7 0x00 0xB2 0xB6 0x10 0xA9 0x0A " + _
                                                       "0x00 0x11 0x88 0x31 0x09 0xFF 0x2E 0x00 0x06 0x00 "
        FindNameInVarLst(VarNameList.RFFilter).State = VarState.VarState_OutDated
        FindNameInVarLst(VarNameList.RFParams).State = VarState.VarState_OutDated

    End Sub

    Private Sub btnTestSaveAgain_Click(sender As System.Object, e As System.EventArgs) Handles btnTestSaveAgain.Click
        updateWidStatsInTable(tags)
        WidTestSave(lviList)
    End Sub

    Private Sub lvLogs_CacheVirtualItems(sender As Object, e As System.Windows.Forms.CacheVirtualItemsEventArgs) Handles lvLogs.CacheVirtualItems
        'We've gotten a request to refresh the cache. 
        'First check if it's really neccesary. 
        If Not (tagListCache Is Nothing) AndAlso e.StartIndex >= tagListFirstItem AndAlso e.EndIndex <= tagListFirstItem + tagListCache.Length Then
            'If the newly requested cache is a subset of the old cache,  
            'no need to rebuild everything, so do nothing. 
            Return
        End If

        'Now we need to rebuild the cache.
        tagListFirstItem = e.StartIndex
        Dim length As Integer = e.EndIndex - e.StartIndex + 1 'indexes are inclusive
        tagListCache = New ListViewItem(length) {}

        'Fill the cache with the appropriate ListViewItems. 
        Dim i As Integer
        For i = 0 To length
            tagListCache(i) = tagList.GetLVI(tagListFirstItem + i)
        Next i
    End Sub

    Private Sub lvLogs_RetrieveVirtualItem(sender As Object, e As System.Windows.Forms.RetrieveVirtualItemEventArgs) Handles lvLogs.RetrieveVirtualItem
        'Caching is not required but improves performance on large sets. 
        'To leave out caching, don't connect the CacheVirtualItems event  
        'and make sure myCache is null. 
        'check to see if the requested item is currently in the cache 
        If Not (tagListCache Is Nothing) AndAlso e.ItemIndex >= tagListFirstItem AndAlso e.ItemIndex < tagListFirstItem + tagListCache.Length Then
            'A cache hit, so get the ListViewItem from the cache instead of making a new one.
            e.Item = tagListCache((e.ItemIndex - tagListFirstItem))
        Else
            'A cache miss, so create a new ListViewItem and pass it back. 
            e.Item = tagList.GetLVI(e.ItemIndex)
        End If
    End Sub

    Private Sub lvSysEvents_CacheVirtualItems(sender As Object, e As System.Windows.Forms.CacheVirtualItemsEventArgs) Handles lvSysEvents.CacheVirtualItems
        'We've gotten a request to refresh the cache. 
        'First check if it's really neccesary. 
        If Not (sysEventListCache Is Nothing) AndAlso e.StartIndex >= sysEventListFirstItem AndAlso e.EndIndex <= sysEventListFirstItem + sysEventListCache.Length Then
            'If the newly requested cache is a subset of the old cache,  
            'no need to rebuild everything, so do nothing. 
            Return
        End If

        'Now we need to rebuild the cache.
        sysEventListFirstItem = e.StartIndex
        Dim length As Integer = e.EndIndex - e.StartIndex + 1 'indexes are inclusive
        sysEventListCache = New ListViewItem(length) {}

        'Fill the cache with the appropriate ListViewItems. 
        Dim i As Integer
        For i = 0 To length
            sysEventListCache(i) = sysEventList.GetLVI(sysEventListFirstItem + i)
        Next i
    End Sub

    Private Sub lvSysEvents_RetrieveVirtualItem(sender As Object, e As System.Windows.Forms.RetrieveVirtualItemEventArgs) Handles lvSysEvents.RetrieveVirtualItem
        'Caching is not required but improves performance on large sets. 
        'To leave out caching, don't connect the CacheVirtualItems event  
        'and make sure myCache is null. 
        'check to see if the requested item is currently in the cache 
        If Not (sysEventListCache Is Nothing) AndAlso e.ItemIndex >= sysEventListFirstItem AndAlso e.ItemIndex < sysEventListFirstItem + sysEventListCache.Length Then
            'A cache hit, so get the ListViewItem from the cache instead of making a new one.
            e.Item = sysEventListCache((e.ItemIndex - sysEventListFirstItem))
        Else
            'A cache miss, so create a new ListViewItem and pass it back. 
            e.Item = sysEventList.GetLVI(e.ItemIndex)
        End If
    End Sub
End Class

Public Class TagTestPickupStats
    Public TagName As String
    Private RSSIVals As New List(Of Decimal)
    Private TimeVals As New List(Of DateTime)

    Public Sub New()

    End Sub

    Public ReadOnly Property Measurements As Integer
        Get
            Return RSSIVals.Count
        End Get
    End Property

    Public Sub New(_name As String)
        TagName = _name
    End Sub

    Public Sub AddPickup(Rssi As Decimal, time As DateTime)
        RSSIVals.Add(Rssi)
        TimeVals.Add(time)
    End Sub

    'in milliseconds
    Public ReadOnly Property AvgTimeBetweenPickups
        Get
            Dim t As TimeSpan
            t = TimeVals.Last - TimeVals.First

            Return t.TotalMilliseconds / TimeVals.Count

        End Get
    End Property

    Public ReadOnly Property RSSI_Mean As Decimal
        Get
            Dim sum As Decimal = 0

            For Each d As Decimal In RSSIVals
                sum += d
            Next

            Return sum / RSSIVals.Count

        End Get
    End Property

    Public ReadOnly Property RSSI_SampleStdDev As Decimal
        Get
            Dim sum As Decimal = 0
            Dim mean As Decimal = RSSI_Mean

            For Each d As Decimal In RSSIVals
                sum += (d - mean) ^ 2
            Next

            sum = sum / (RSSIVals.Count - 1)

            Return Math.Sqrt(sum)

        End Get
    End Property

End Class

Public Class VarInterface
    Public Delegate Function TestFunc_Type(ByVal sender As VarInterface) As Boolean
    Public Delegate Function GetHandlerFunc_Type(ByVal sender As VarInterface, ByRef retVals() As Object) As Boolean
    Public Delegate Function SetHandlerFunc_Type(ByVal sender As VarInterface) As Object()

    Public Enum VarState As Integer
        VarState_OutDated = 0   '(Yellow) Value shown has been modified and no longer reflects the value on the device
        VarState_Current = 1    '(Green)  Value shown matches the value on the device
        VarState_Unknown = 2    '(White)  Error reading device value
        VarState_WriteError = 3 '(Red)    Error writing device value
    End Enum

    Public PicB As PictureBox
    Public WithEvents Control As Control
    Public ID As Integer
    Public Category As Integer
    Private SerComLib As SerComms
    Private GetCMD As SerDev_LoggerV2_ComFuncs.CMD_ByteID
    Private SetCMD As SerDev_LoggerV2_ComFuncs.CMD_ByteID
    Public UpdateCurVal As Boolean = False
    Public ComboBoxUseTextAsProperty As Boolean = False

    Public FormatTestFunc As TestFunc_Type
    Public GetHandlerFunc As GetHandlerFunc_Type
    Public SetHandlerFunc As SetHandlerFunc_Type

    Public OrigVal As Object = Nothing
    Private _State As VarState
    Private _ReadOnly As Boolean = False

    Public Sub AddCtrlToHandle(ByVal ctrl As Object)
        Select Case ctrl.GetType.ToString
            Case "System.Windows.Forms.TextBox"
                AddHandler CType(ctrl, TextBox).TextChanged, AddressOf CtrlEditHandler
            Case "System.Windows.Forms.DateTimePicker"
                AddHandler CType(ctrl, DateTimePicker).ValueChanged, AddressOf CtrlEditHandler
            Case "System.Windows.Forms.NumericUpDown"
                AddHandler CType(ctrl, NumericUpDown).ValueChanged, AddressOf CtrlEditHandler
            Case "System.Windows.Forms.ComboBox"
                AddHandler CType(ctrl, ComboBox).SelectedIndexChanged, AddressOf CtrlEditHandler
                AddHandler CType(ctrl, ComboBox).TextChanged, AddressOf CtrlEditHandler
        End Select

        AddHandler CType(ctrl, Control).KeyUp, AddressOf CtrlKeyUpHandler
    End Sub

    Private Sub Init(ByVal mID As Integer, ByVal cat As Integer, ByVal fmtTstFunc As TestFunc_Type, ByRef SerComsLib As SerComms, ByVal Get_Cmd As SerDev_LoggerV2_ComFuncs.CMD_ByteID, ByVal Set_Cmd As SerDev_LoggerV2_ComFuncs.CMD_ByteID)
        ID = mID
        State = VarState.VarState_Unknown
        FormatTestFunc = fmtTstFunc
        SerComLib = SerComsLib
        GetCMD = Get_Cmd
        SetCMD = Set_Cmd
        Category = cat

        If Set_Cmd = SerDev_LoggerV2_ComFuncs.CMD_ByteID.NONE Then
            _ReadOnly = True
            SetControlReadOnly()
        End If

        AddCtrlToHandle(Control)

    End Sub

    Public Sub New(ByVal mID As Integer, ByVal cat As Integer, ByRef pb As PictureBox, ByRef ctrl As Control, ByVal fmtTstFunc As TestFunc_Type, ByRef SerComsLib As SerComms, ByVal Get_Cmd As SerDev_LoggerV2_ComFuncs.CMD_ByteID, ByVal Set_Cmd As SerDev_LoggerV2_ComFuncs.CMD_ByteID)
        PicB = pb
        Control = ctrl

        Init(mID, cat, fmtTstFunc, SerComsLib, Get_Cmd, Set_Cmd)

    End Sub

    Public Sub New(ByVal mID As Integer, ByVal cat As Integer, ByRef tlpParent As TableLayoutPanel, ByVal Row As Integer, ByVal labelText As String, ByVal fmtTstFunc As TestFunc_Type, ByRef SerComsLib As SerComms, ByVal Get_Cmd As SerDev_LoggerV2_ComFuncs.CMD_ByteID, ByVal Set_Cmd As SerDev_LoggerV2_ComFuncs.CMD_ByteID)

        Dim tmpLbl As New Label()
        tmpLbl.Text = labelText
        Dim TLPtmp As New TableLayoutPanel()
        TLPtmp.Padding = New Padding(0)
        PicB = New PictureBox()
        PicB.Margin = New Padding(0)
        PicB.BackgroundImageLayout = ImageLayout.Stretch
        Control = New TextBox()
        CType(Control, TextBox).Margin = New Padding(3, 0, 0, 0)

        TLPtmp.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 20))
        TLPtmp.Controls.Add(PicB, 0, 0)
        TLPtmp.Controls.Add(Control, 1, 0)
        CType(Control, TextBox).Dock = DockStyle.Fill
        'PicB.Dock = DockStyle.Fill
        PicB.Width = 20
        PicB.Height = 20
        'PicB.Margin = New Padding(TLPtmp.ColumnStyles(0).Width / 2 - (PicB.Width / 2), TLPtmp.Height / 2 - (PicB.Height), 0, 0)
        PicB.Dock = DockStyle.Fill
        PicB.BackgroundImageLayout = ImageLayout.Center


        If Row > tlpParent.RowCount - 1 Then
            tlpParent.RowStyles.Add(New RowStyle(SizeType.Absolute, 26))
        End If
        tlpParent.Controls.Add(tmpLbl, 0, Row)
        tlpParent.Controls.Add(TLPtmp, 1, Row)

        TLPtmp.Dock = DockStyle.Fill
        tmpLbl.Dock = DockStyle.Fill
        tmpLbl.TextAlign = ContentAlignment.MiddleLeft

        Init(mID, cat, fmtTstFunc, SerComsLib, Get_Cmd, Set_Cmd)

    End Sub

    Private Sub CtrlEditHandler(ByVal sender As System.Object, ByVal e As System.EventArgs)
        Dim FormatGood As Boolean = True

        If FormatTestFunc <> Nothing Then
            FormatGood = FormatTestFunc(Me)
        End If

        Try

            If FormatGood Then
                If _ReadOnly = True Then
                    CType(Control, Control).BackColor = Color.WhiteSmoke  'SystemColors.Control
                Else
                    CType(Control, Control).BackColor = Color.White
                End If
            Else
                CType(Control, Control).BackColor = Color.Red
            End If
        Catch ex As Exception
            Stop
        End Try

        'Skip the entire test??
        If UpdateCurVal = True Then
            OrigVal = Value
            UpdateCurVal = False
        End If

        If State = VarState.VarState_Current Then
            'value was current
            Try
                If Value <> OrigVal Then
                    'value has now changed
                    'set to outdated
                    State = VarState.VarState_OutDated
                End If
            Catch ex As Exception
                State = VarState.VarState_OutDated
            End Try
        ElseIf State = VarState.VarState_OutDated Then
            'value was outdated
            If Value = OrigVal Then
                'Match the actual device value
                'set back to Current value
                State = VarState.VarState_Current
            End If
        End If
    End Sub

    Private Sub CtrlKeyUpHandler(ByVal sender As System.Object, ByVal e As System.Windows.Forms.KeyEventArgs)
        If e.KeyCode = Keys.Escape Then
            'If State = VarState.VarState_OutDated Then
            ReadFromDev()
            'Value = OrigVal
            'State = VarState.VarState_Current
            'End If
            'if key = enter - update value now! ??
        End If
    End Sub

    Private Sub SetControlReadOnly()
        Select Case Control.GetType.ToString
            Case "System.Windows.Forms.TextBox"
                CType(Control, TextBox).ReadOnly = True
                'CType(Control, TextBox).BackColor = Color.LightGray
            Case "System.Windows.Forms.DateTimePicker"
                CType(Control, DateTimePicker).Enabled = False
            Case "System.Windows.Forms.NumericUpDown"
                CType(Control, NumericUpDown).ReadOnly = True
                'CType(Control, NumericUpDown).BackColor = Color.Gray
            Case "System.Windows.Forms.ComboBox"
                CType(Control, ComboBox).Enabled = False
            Case Else
                CType(Control, Control).Enabled = False
        End Select
    End Sub

    Public Property Value As Object
        Get
            Select Case Control.GetType.ToString
                Case "System.Windows.Forms.TextBox"
                    Return CType(Control, TextBox).Text
                Case "System.Windows.Forms.DateTimePicker"
                    Return CType(Control, DateTimePicker).Value
                Case "System.Windows.Forms.NumericUpDown"
                    Return CType(Control, NumericUpDown).Value
                Case "System.Windows.Forms.ComboBox"
                    If ComboBoxUseTextAsProperty = True Then
                        Return CType(Control, ComboBox).Text
                    Else
                        Return CType(Control, ComboBox).SelectedIndex
                    End If

            End Select
            Return Nothing
        End Get
        Set(ByVal value As Object)
            OrigVal = value
            Try
                If value Is Nothing Then
                    Select Case Control.GetType.ToString
                        Case "System.Windows.Forms.TextBox"
                            CType(Control, TextBox).Text = ""
                        Case "System.Windows.Forms.DateTimePicker"
                            CType(Control, DateTimePicker).Value = CType(Control, DateTimePicker).MinDate
                        Case "System.Windows.Forms.NumericUpDown"
                            CType(Control, NumericUpDown).Value = 0
                        Case "System.Windows.Forms.ComboBox"
                            CType(Control, ComboBox).Text = "INVALID"
                    End Select
                Else
                    Select Case Control.GetType.ToString
                        Case "System.Windows.Forms.TextBox"
                            CType(Control, TextBox).Text = CType(value, String)
                        Case "System.Windows.Forms.DateTimePicker"
                            CType(Control, DateTimePicker).Value = CType(value, DateTime)
                        Case "System.Windows.Forms.NumericUpDown"
                            CType(Control, NumericUpDown).Value = CType(value, Decimal)
                        Case "System.Windows.Forms.ComboBox"
                            If ComboBoxUseTextAsProperty = True Then
                                CType(Control, ComboBox).Text = CType(value, String)
                            Else
                                CType(Control, ComboBox).SelectedIndex = CType(value, Integer)
                            End If
                    End Select
                End If

                'If _ReadOnly = True Then SetControlReadOnly()

            Catch ex As Exception
                State = VarState.VarState_Unknown
            End Try
        End Set
    End Property

    Public Property State As VarState
        Get
            Return _State
        End Get
        Set(ByVal value As VarState)
            _State = value
            Select Case value
                Case VarState.VarState_Current
                    'PicB.BackColor = Color.Green
                    PicB.BackgroundImage = My.Resources.Green

                Case VarState.VarState_OutDated
                    'PicB.BackColor = Color.Red
                    PicB.BackgroundImage = My.Resources.Yellow

                Case VarState.VarState_Unknown
                    'PicB.BackColor = Color.White
                    PicB.BackgroundImage = My.Resources.White
                    'Me.Value = Nothing

                Case VarState.VarState_WriteError
                    'PicB.BackColor = Color.Black
                    PicB.BackgroundImage = My.Resources.Red

                Case Else
                    'PicB.BackColor = Color.White
                    PicB.BackgroundImage = My.Resources.White
                    _State = VarState.VarState_Unknown
                    'Me.Value = Nothing
            End Select
        End Set
    End Property

    Public Function ReadFromDev() As MyErr_Enum
        Dim tmpErr As MyErr_Enum = MyErr_Enum.MyErr_Success
        Dim RetVals(0) As Object

        'If State <> VarState.VarState_Current Then
        If SerComLib.Ser.IsOpen = False Then
            Return MyErr_Enum.MyErr_PortClosed
        End If
        Try
            If waitforParam(SerComLib.Port_InUse, False, 5000) = False Then Return MyErr_Enum.MyErr_PortInUse
        Catch ex As Exception
            Return MyErr_Enum.MyErr_Unknown
        End Try
        SerComLib.Port_InUse = True

        SerComLib.Ser_Timeout = 500
        Try
            tmpErr = SerComLib.Ser_SendCmd_NS(GetCMD, Nothing, RetVals)
        Catch ex As Exception
            Return MyErr_Enum.MyErr_Unknown
        End Try
        If tmpErr < 0 Then
            State = VarState.VarState_Unknown
        Else
            'UpdateCurVal = True
            If GetHandlerFunc <> Nothing Then
                If GetHandlerFunc(Me, RetVals) = True Then
                    State = VarState.VarState_Current
                End If
            Else
                Value = RetVals(0)
                State = VarState.VarState_Current
            End If
        End If
        'End If
        Return tmpErr
    End Function

    Public Function WriteToDev() As MyErr_Enum
        Dim tmpErr As MyErr_Enum = MyErr_Enum.MyErr_Success
        Dim Params(0) As Object

        If _ReadOnly = True Then Return MyErr_Enum.MyErr_BadCmd

        If FormatTestFunc <> Nothing Then
            If FormatTestFunc(Me) = False Then
                State = VarState.VarState_WriteError
                Return MyErr_Enum.MyErr_BadCmd
            End If
        End If

        'If State <> VarState.VarState_Current Then

        If waitforParam(SerComLib.Port_InUse, False, 1000) = False Then Return MyErr_Enum.MyErr_PortInUse
        'SerComLib.Port_InUse = True

        If SetHandlerFunc = Nothing Then
            Params(0) = Value
        Else
            Params = SetHandlerFunc(Me)
        End If
        SerComLib.Ser_Timeout = 500

        tmpErr = SerComLib.Ser_SendCmd_NS(SetCMD, Params, Nothing)
        If tmpErr < 0 Then
            State = VarState.VarState_WriteError
        Else
            OrigVal = Value
            State = VarState.VarState_Current
        End If
        'End If
        Return tmpErr
    End Function

    'the function will return once param = matchvalue (returns true) or the function times out (returns false)
    Public Shared Function waitforParam(ByRef Param As Boolean, ByVal Matchvalue As Boolean, ByVal msTimeout As Integer) As Boolean
        Dim starttime As DateTime = Now()

        Try
            Do
                'Threading.Thread.Sleep(1)
                Threading.Thread.Yield()
                Application.DoEvents()
            Loop While Param <> Matchvalue And (Now() - starttime).TotalMilliseconds < msTimeout
        Catch ex As Exception
            Throw ex
        End Try

        If Param <> Matchvalue Then '(Now() - starttime).TotalMilliseconds >= msTimeout Then
            Return False
        Else
            Return True
        End If

    End Function

End Class

Public Class TagList
    Public times As List(Of DateTime)
    Public IDs As List(Of UInt32)
    Public Flags As List(Of Byte)
    Public RSSIs As List(Of Integer)
    Public useRSSI As Boolean = True

    Public Sub New(Optional _useRssi As Boolean = False)
        times = New List(Of DateTime)()
        IDs = New List(Of UInt32)()
        Flags = New List(Of Byte)()
        RSSIs = New List(Of Integer)()
        useRSSI = _useRssi
    End Sub

    Public Sub New(_times As List(Of DateTime), _IDs As List(Of UInt32), _Flags As List(Of Byte), _RSSIs As List(Of Integer))
        times = _times
        IDs = _IDs
        Flags = _Flags
        RSSIs = _RSSIs
    End Sub

    Public ReadOnly Property Count()
        Get
            Return times.Count
        End Get
    End Property

    Public Sub RemoveItem(index As Integer)
        times.RemoveAt(index)
        IDs.RemoveAt(index)
        Flags.RemoveAt(index)
        RSSIs.RemoveAt(index)
    End Sub

    Public Sub AddItem(time As DateTime, id As UInt32, flag As Byte, RSSI As Integer) 'As Integer
        times.Add(time)
        IDs.Add(id)
        Flags.Add(flag)
        RSSIs.Add(RSSI)

        'Return RSSIs.Count - 1
    End Sub

    Public Function GetLVI(id As Integer) As ListViewItem
        Dim n As New ListViewItem()
        Dim tmpStr As String

        n.Text = id
        n.SubItems.Add(times(id).ToString("dd/MM/yyyy"))
        n.SubItems.Add(times(id).ToString("HH:mm:ss"))

        If Flags(id) = 2 Then
            'System Message
            Select Case (IDs(id) And &HFF000000) >> 24
                Case &H0
                    tmpStr = "Power On"
                Case &H1
                    tmpStr = "Wakeup from Bat Backup (No Time Update)"
                Case &H2
                    tmpStr = "Wakeup from Bat Backup (Time Updated from GSM)"
                Case &H3
                    tmpStr = "Send Email Failed (Attempts)"
                Case &H4
                    tmpStr = "Send Email Success (Attempts)"
                Case &H5
                    tmpStr = "Send Email Attempt"
                Case &H6
                    tmpStr = "Updated RTC Time (Source)"
                Case Else
                    tmpStr = "Unknown System Message"
            End Select
            n.SubItems.Add(tmpStr)
            n.SubItems.Add((IDs(id) And &HFFFFFF).ToString("X"))
        Else
            'Tag Pickup
            n.SubItems.Add(IDs(id).ToString("X8"))
            If Flags(id) = 3 Then
                n.SubItems.Add("Start") ' (03)")
            ElseIf Flags(id) = 1 Then
                n.SubItems.Add("Stop") ' (01)")
            ElseIf Flags(id) = 0 Then
                n.SubItems.Add("Unit Init") ' (00)")
            Else
                'This can't actually happen because flag is only a 2 bit value (4 possible options)
                n.SubItems.Add(CInt(Flags(id)).ToString("X2"))
            End If
            If useRSSI = True Then
                n.SubItems.Add(RSSIs(id))
            End If
        End If

        Return n
    End Function



    'Public times() As DateTime
    'Public IDs() As UInt32
    'Public Flags() As Byte
    'Public RSSIs() As Integer

    'Public Sub New(_times As List(Of DateTime), _IDs As List(Of UInt32), _Flags As List(Of Byte), _RSSIs As List(Of Integer))
    '    times = _times.ToArray
    '    IDs = _IDs.ToArray
    '    Flags = _Flags.ToArray
    '    RSSIs = _RSSIs.ToArray
    'End Sub
End Class