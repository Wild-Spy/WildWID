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
    Public mretVals_loggerTable() As Object
    Public mretVals() As Object
    Public Vars As New List(Of VarInterface)
    Private cancelPingLoop As Boolean = False
    Private skipDevNameUpdate As Boolean = False 'used for when the device name is changed on the device.  Stops the devName combo-box from updating all variables when the name devices name is updated in it's list
    Private CtrlListComms As List(Of Control)
    Private strScanPos As Integer = 0
    Private SerTextCmdActive As Integer = 0
    Private alltxt As String = ""
    Public connectedDevVersion As String = ""

    Public RFParamNames As List(Of String)
    Public RFParamValues As List(Of String)
    Public InterLoggerRFParamNames As List(Of String)
    Public InterLoggerRFParamValues As List(Of String)

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
        For Each tmpVar In Vars
            tmpVar.State = VarState.VarState_Unknown
        Next
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
        'TODO: I noticed that if you have a bluetooth port that isn't connected then this loop below can
        '      freeze.  Need a way around that.  But there is already a timeout I think so that isn't working.
        For Each PortStr In comPorts

            'PortStr = "COM28"
            SerComLib.Ser_PortName = PortStr

            tmpErr = SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_DevTypeID, Nothing, RetVals)

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

        If FindNameInVarLst(VarNameList.RTCTime).State = VarState.VarState_Current Then
            FindNameInVarLst(VarNameList.RTCTime).UpdateCurVal = True
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
        Next
        Stop_Ping_Loop()
        If SerComLib.Ser.IsOpen Then
            SerComLib.Ser.Close()
        End If
        lblStatus.Text = "Not Connected"

        'Make sure it's the right device still!
        SerComLib.Ser_PortName = FindSelectedPortNum()

        tmpErr = SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_DevTypeID, Nothing, RetVals)
        If tmpErr = MyErr_Enum.MyErr_Success Then
            devID = CType(RetVals(0), UInt16)

            If devID = SerDev_LoggerV2_ComFuncs.DevID Then

                'Read Device Name
                tmpErr = SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_LoggerName, Nothing, RetVals)
                If tmpErr = MyErr_Enum.MyErr_Success Then
                    tmpName = CType(RetVals(0), String)

                    If tmpName = DevNames(cbDevNames.SelectedIndex) Then

                        tmpErr = SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_Version, Nothing, RetVals)
                        If tmpErr = MyErr_Enum.MyErr_Success Then
                            connectedDevVersion = CType(RetVals(0), String).Split("-").Last.Split(":").First.Trim()

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
                                    tmpVar.ReadFromDev()
                                    Threading.Thread.Sleep(50)
                                Next

                                'FillOverviewInfo()
                                'FillRFSettingsInfo()
                                'FillGSMSettingsInfo()
                                lblStatus.Text = "Connected (" & FindSelectedPortNum() & ")"
                                Start_Ping_Loop()
                            Catch ex As Exception
                                MsgBox("Invalid Version String")
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



    Private Sub setupRFParamsComboBoxes()
        RFParamNames = New List(Of String)
        InterLoggerRFParamNames = New List(Of String)
        RFParamValues = New List(Of String)
        InterLoggerRFParamValues = New List(Of String)

        RFParamNames.Add("2.4GHz Short Range WIDs (250kbps)")
        RFParamValues.Add("0xD3 0x91 0x06 0x04 0x04 0x00 0x00 0x0A 0x00 0x5D " + vbCrLf +
                          "0x93 0xB1 0x2D 0x3B 0x73 0x42 0xF8 0x00 0x07 0x30 " + vbCrLf +
                          "0x14 0x1D 0x1C 0xC7 0x00 0xB2 0xB6 0x10 0xEA 0x0A " + vbCrLf +
                          "0x00 0x11 0x88 0x31 0x09 0xFF 0x2E 0x00 0x06 0x81 " + vbCrLf)
        RFParamNames.Add("2.4GHz Medium Range WIDs (50kbps)")
        RFParamValues.Add("0xD3 0x91 0x06 0x04 0x04 0x00 0x00 0x0A 0x00 0x5D " + vbCrLf +
                          "0x93 0xB1 0x8A 0xF8 0x13 0x22 0xF8 0x45 0x07 0x30 " + vbCrLf +
                          "0x14 0x1D 0x1C 0xC7 0x00 0xB2 0xB6 0x10 0xA9 0x0A " + vbCrLf +
                          "0x00 0x11 0x88 0x31 0x09 0xFF 0x2E 0x00 0x06 0x00 " + vbCrLf)
        RFParamNames.Add("2.4GHz Long Range WIDs (2.4kbps)")
        RFParamValues.Add("0xD3 0x91 0x06 0x04 0x04 0x00 0x00 0x0A 0x00 0x5D " + vbCrLf +
                          "0x93 0xB1 0x86 0x83 0x03 0x22 0xF8 0x44 0x07 0x30 " + vbCrLf +
                          "0x14 0x16 0x6C 0x03 0x40 0x91 0x56 0x10 0xA9 0x0A " + vbCrLf +
                          "0x00 0x11 0x88 0x31 0x09 0xFF 0x2E 0x00 0x06 0x81 " + vbCrLf)


        InterLoggerRFParamNames.Add("Inter Logger Comms (38kbps)")
        InterLoggerRFParamValues.Add("0xD3 0x91 0xFF 0x04 0x05 0x00 0x02 0x06 0x00 0x10 " + vbCrLf +
                                     "0xA7 0x62 0xCA 0x83 0x13 0x22 0xF8 0x35 0x07 0x30 " + vbCrLf +
                                     "0x18 0x16 0x6C 0x43 0x40 0x91 0x56 0x10 0xE9 0x2A " + vbCrLf +
                                     "0x00 0x1F 0x81 0x35 0x09 0x00 0x00 0x00 0x00 0x00 " + vbCrLf)
        InterLoggerRFParamNames.Add("433MHz WID (2.4kbps 2FSK)")
        InterLoggerRFParamValues.Add("0xD3 0x91 0x06 0x04 0x04 0x00 0x04 0x06 0x00 0x10 " + vbCrLf +
                                     "0xA7 0x62 0xF6 0x83 0x03 0x22 0xF8 0x15 0x07 0x30 " + vbCrLf +
                                     "0x18 0x17 0x6C 0x03 0x40 0x91 0x56 0x10 0xE9 0x2A " + vbCrLf +
                                     "0x00 0x1F 0x88 0x31 0x09 0xC0 0x00 0x00 0x00 0x00 " + vbCrLf)
        InterLoggerRFParamNames.Add("433MHz WID (5kbps GFSK)")
        InterLoggerRFParamValues.Add("0xD3 0x91 0x06 0x04 0x04 0x00 0x04 0x06 0x04 0x10 " + vbCrLf +
                                     "0xA7 0x62 0xF7 0x93 0x13 0x22 0xF8 0x24 0x07 0x30 " + vbCrLf +
                                     "0x18 0x17 0x6C 0x03 0x40 0x91 0x56 0x10 0xE9 0x2A " + vbCrLf +
                                     "0x00 0x1F 0x88 0x31 0x09 0xC0 0x00 0x00 0x00 0x00 " + vbCrLf)
        InterLoggerRFParamNames.Add("433MHz WID (7.5kbps 2FSK)")
        InterLoggerRFParamValues.Add("0xD3 0x91 0x06 0x04 0x04 0x00 0x04 0x06 0x00 0x10 " + vbCrLf +
                                     "0xA7 0x62 0xF8 0x2E 0x03 0x22 0xF8 0x31 0x07 0x30 " + vbCrLf +
                                     "0x18 0x17 0x6C 0x03 0x40 0x91 0x56 0x10 0xE9 0x2A " + vbCrLf +
                                     "0x00 0x1F 0x88 0x31 0x09 0xC0 0x00 0x00 0x00 0x00 " + vbCrLf)

        cbRFParams.Items.AddRange(RFParamNames.ToArray)
        cbInterloggerRFParams.Items.AddRange(InterLoggerRFParamNames.ToArray)
    End Sub

    Private Sub frmMain_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        DTDiff = New TimeSpan()
        DTDiff = Now() - dtpDevDateTime.Value
        setupControlList()

        setupRFParamsComboBoxes()

        Me.Show()
        'TabControlMain.SelectTab(5)

        Try
            Vars.Add(New VarInterface(VarNameList.DevVersion, VarCatPage.General, TLPOverview, 1, "Version", Nothing, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_Version, SerDev_LoggerV2_ComFuncs.CMD_ByteID.NONE))
            Vars.Add(New VarInterface(VarNameList.LoggerName, VarCatPage.General, TLPOverview, 2, "Logger Name", AddressOf LoggerNameValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_LoggerName, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_LoggerName))
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf LoggerNameSetHandler
            Vars(Vars.Count - 1).PostSetHandlerFunc = AddressOf LoggerNamePostSetHandler
            Vars.Add(New VarInterface(VarNameList.LoggerId, VarCatPage.General, TLPOverview, 3, "Logger ID", Nothing, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_LoggerId, SerDev_LoggerV2_ComFuncs.CMD_ByteID.NONE))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf uint32AsHexGetHandler
            'Vars.Add(New VarInterface(VarNameList.Records, VarCatPage.General, TLPOverview, 3, "Records", Nothing, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RecordCount, SerDev_LoggerV2_ComFuncs.CMD_ByteID.NONE))
            Vars.Add(New VarInterface(VarNameList.Records, VarCatPage.General, pbRecordCount, txtRecordCount, Nothing, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RecordCount, SerDev_LoggerV2_ComFuncs.CMD_ByteID.NONE))
            Vars.Add(New VarInterface(VarNameList.RTCTime, VarCatPage.General, PBDevDateTime, dtpDevDateTime, AddressOf RTCTimeValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_DateTime, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_DateTime))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf RTCTimeGetHandler
            Vars.Add(New VarInterface(VarNameList.LoggerGroupId, VarCatPage.General, TLPOverview, 6, "Group ID", AddressOf uint16ValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_LoggerGroupId, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_LoggerGroupId))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf uint16GetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf uint16SetHandler
            Vars.Add(New VarInterface(VarNameList.WarningSignalDuration, VarCatPage.General, TLPOverview, 7, "Warning Signal Duration", AddressOf uint16ValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_WarningSignalDuration, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_WarningSignalDuration))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf uint16GetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf uint16SetHandler

            'Vars.Add(New VarInterface(VarNameList.RFChan, VarCatPage.RFSettings, TLPRFSet, 1, "RF Channel", AddressOf RFChanValidateHandler, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RFChan, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_RFChan))
            'Vars.Add(New VarInterface(VarNameList.RFID, VarCatPage.RFSettings, TLPRFSet, 2, "RF ID", AddressOf RFIDValidateHandler, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RFID, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_RFID))
            'Vars(Vars.Count - 1).GetHandlerFunc = AddressOf RFIDGetHandler
            'Vars(Vars.Count - 1).SetHandlerFunc = AddressOf RFIDSetHandler
            Vars.Add(New VarInterface(VarNameList.RFFilter, VarCatPage.RFSettings, TLPRFSet, 1, "RF Filter", AddressOf RFFilterValidateHandler, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RFFilter, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_RFFilter))
            Vars.Add(New VarInterface(VarNameList.VerboseMode, VarCatPage.RFSettings, pbVerboseMode, cbVerboseMode, AddressOf VerboseModeValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_VerboseMode, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_VerboseMode))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf VerboseModeGetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf VerboseModeSetHandler
            Vars.Add(New VarInterface(VarNameList.RFParams, VarCatPage.RFSettings, pbRFParams, txtRFParams, AddressOf RFParamValidateHandler, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RFParams, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_RFParams))
            txtRFParams.Tag = cbRFParams
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf RFParamGetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf RFParamSetHandler
            Dim tmpTxt As TextBox = CType(Vars(Vars.Count - 1).Control, TextBox)
            TLPRFSet.AutoSize = True
            tmpTxt.Multiline = True
            tmpTxt.Height = 30
            TLPRFSet.AutoSize = False
            Vars.Add(New VarInterface(VarNameList.InterLogRFParams, VarCatPage.RFSettings, pbInterloggerRFParams, txtInterloggerRFParams, AddressOf RFParamValidateHandler, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_InterLogRFParams, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_InterLogRFParams))
            txtInterloggerRFParams.Tag = cbInterloggerRFParams
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf RFParamGetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf RFParamSetHandler
            tmpTxt = CType(Vars(Vars.Count - 1).Control, TextBox)
            TLPRFSet.AutoSize = True
            tmpTxt.Multiline = True
            tmpTxt.Height = 30
            TLPRFSet.AutoSize = False

            Vars.Add(New VarInterface(VarNameList.SendEmailPeriod, VarCatPage.GSMSettings, TLPGSMSet, 1, "Send Email Period", AddressOf SendEmailPeriodValidateHandler, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_SendDataGPRSPeriod, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_SendDataGPRSPeriod))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf SendEmailPeriodGetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf SendEmailPeriodSetHandler
            Vars.Add(New VarInterface(VarNameList.SendEmailNext, VarCatPage.GSMSettings, pbSendEmailNext, dtpSendEmailNext, AddressOf SendEmailNextValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_SendDataGPRSNext, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_SendDataGPRSNext))
            'Vars.Add(New VarInterface(VarNameList.EmailToAddress, VarCatPage.GSMSettings, TLPGSMSet, 3, "Email To Address", AddressOf EmailToAddressValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_EmailToAddress, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_EmailToAddress))
            'Vars.Add(New VarInterface(VarNameList.GPRSSettings, VarCatPage.GSMSettings, pbGPRSSettings, cbGPRSSettings, AddressOf GPRSSettingsValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_GPRSVals, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_GPRSVals))
            'Vars(Vars.Count - 1).ComboBoxUseTextAsProperty = True
            'Vars(Vars.Count - 1).GetHandlerFunc = AddressOf GPRSSettingsGetHandler
            'Vars(Vars.Count - 1).SetHandlerFunc = AddressOf GPRSSettingsSetHandler
            Vars.Add(New VarInterface(VarNameList.NextDataReset, VarCatPage.GSMSettings, pbNextDataReset, dtpNextDataReset, AddressOf NextDataResetValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_IridiumMonthNextReset, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_IridiumMonthNextReset))
            Vars.Add(New VarInterface(VarNameList.MonthlyData, VarCatPage.GSMSettings, TLPGSMSet, 4, "Monthly Data Allowance (bytes)", AddressOf uint16ValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_MonthByteLimit, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_MonthByteLimit))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf uint16GetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf uint16SetHandler

            TabControlRecords.TabPages.Remove(TabPageParsedTags)

        Catch ex As Exception
            Stop
        End Try
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

    Private Function LoggerNamePostSetHandler(ByVal sender As VarInterface)
        Vars(VarNameList.LoggerId).ReadFromDev()
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

    Private Sub checkRfParamsTextAndUpdateCombo(ByRef sender As VarInterface, ByRef tmpStr As String)
        Dim found As Boolean = False
        Dim cb As ComboBox = sender.Control.Tag

        If cb.Name = "cbRFParams" Then
            For i = 0 To RFParamValues.Count - 1
                If tmpStr = RFParamValues(i) Then
                    cb.SelectedIndex = i
                    found = True
                    Exit For
                End If
            Next
        ElseIf cb.Name = "cbInterloggerRFParams" Then
            For i = 0 To InterLoggerRFParamValues.Count - 1
                If tmpStr = InterLoggerRFParamValues(i) Then
                    cb.SelectedIndex = i
                    found = True
                    Exit For
                End If
            Next
        End If

        If found = False Then
            cb.Text = "Custom Configuration"
        End If
    End Sub

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

            checkRfParamsTextAndUpdateCombo(sender, tmpStr)

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

    Private Function NextDataResetValidateHandler(ByVal sender As VarInterface) As Boolean
        Dim ctrl As DateTimePicker = CType(sender.Control, DateTimePicker)

        If ctrl.Value.Year < 2000 Or ctrl.Value.Year > 2255 Then
            Return False
        End If

        If (dtpSendEmailNext.Value - ctrl.Value).TotalDays > 31 Then
            MsgBox(vbCritical, "WARNING!!! next data reset is set to a long time after next email send.  This may result in a failure to reset the monthly data allowance and therefore a failure to transmit data once it has been reached!")
        End If

        Return True
        'DTDiff = Now() - dtpDevDateTime.Value
    End Function

    Private Function VerboseModeSetHandler(ByVal sender As VarInterface) As Object()
        Dim ctrl As ComboBox = CType(sender.Control, ComboBox)

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

    Private Function uint16ValidateHandler(ByVal sender As VarInterface) As Boolean
        Dim ctrl As TextBox = CType(sender.Control, TextBox)

        Try
            If CInt(ctrl.Text) > 65535 Or CInt(ctrl.Text) < 0 Then
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

        checkRfParamsTextAndUpdateCombo(sender, tmpStr)

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


    Private Function uint16SetHandler(ByVal sender As VarInterface) As Object()
        'The field has already been validated we just need to convert it
        Dim ctrl As TextBox = CType(sender.Control, TextBox)
        Dim Params(0) As Object

        Params(0) = BitConverter.GetBytes(Convert.ToInt16(ctrl.Text))
        Return Params
    End Function

    Private Function uint32AsHexGetHandler(ByVal sender As VarInterface, ByRef retVals() As Object) As Boolean
        Try
            sender.Value = Convert.ToString(retVals(0), 16).ToUpper()
            If sender.Value = "FFFFFFFF" Then
                sender.Value = "ERROR, PLEASE REPROGRAM LOGGER NAME"
                sender.State = VarState.VarState_Unknown
            End If
        Catch ex As Exception
            sender.Value = ""
            sender.State = VarState.VarState_Unknown
            Return False
        End Try
        Return True
    End Function

    Private Function uint16GetHandler(ByVal sender As VarInterface, ByRef retVals() As Object) As Boolean
        Try
            sender.Value = CStr(BitConverter.ToInt16(retVals(0), 0))
        Catch ex As Exception
            sender.Value = ""
            sender.State = VarState.VarState_Unknown
            Return False
        End Try
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
            MsgBox("Invalid date.. must be on or after 1/1/2000")
        End If
    End Sub

    Private Sub btnDevTimeSync_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnDevTimeSync.Click
        dtpDevDateTime.Value = Now()
        DTDiff = Now() - Now()
        TimerRTC.Enabled = True
    End Sub

    Private Sub btnReadLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnReadLog.Click
        Dim retVals() As Object = Nothing
        Dim tmpStr As String
        Dim tmpStr2 As String
        Dim useRSSI As Boolean = False
        Dim useActivity As Boolean = False
        Dim useSoundId As Boolean = False
        Dim lvLogItems As New List(Of ListViewItem)
        Dim lvSysEventItems As New List(Of ListViewItem)

        If Not lblStatus.Text.StartsWith("Connected") Then Exit Sub

        If VersionStringToInteger(connectedDevVersion) >= VersionStringToInteger("V2.1.0") Then
            useRSSI = True
        End If

        If VersionStringToInteger(connectedDevVersion) >= VersionStringToInteger("V2.3.0") Then
            useActivity = True
        End If

        If VersionStringToInteger(connectedDevVersion) >= VersionStringToInteger("V2.3.1") Then
            useSoundId = True
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

        'Erase all logs from list view control
        lvLogs.Items.Clear()
        lvSysEvents.Items.Clear()

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
        Dim Activities As List(Of Single)
        Dim SoundIds As List(Of Byte)
        Dim LoggerLongIds As List(Of String) = New List(Of String)
        Dim LoggerTable As HashSet(Of String)
        Dim i As Integer
        Dim n As ListViewItem

        Try
            times = mretVals(0)
            IDs = mretVals(1)
            Flags = mretVals(2)
            If useRSSI Then RSSIs = mretVals(3)
            If useActivity Then Activities = mretVals(4)

            If useSoundId Then
                SoundIds = mretVals(5)
                LoggerTable = mretVals_loggerTable(0)

                ''Sort out each real ID
                'For Each shortid In LoggerShortIds

                '    Try
                '        LoggerLongIds.Add(LoggerTable(shortid))
                '    Catch
                '        LoggerLongIds.Add("UNKNOWN")
                '    End Try
                'Next

            End If

        Catch ex As Exception
            GoTo FinishSub
        End Try

        lvLogs.BeginUpdate()
        lvSysEvents.BeginUpdate()

        For i = 0 To times.Count - 1
            If LoggerLongIds.Count Mod 100 = 0 Then
                pBTransfer.Value = i / times.Count * 100
                lblRecCount.Text = i.ToString() & "/" & times.Count.ToString()
                lblTransfrRate.Text = "..."
                lblTimeRemain.Text = "..."
                Application.DoEvents()
            End If

            n = New ListViewItem()
            n.Text = i
            n.SubItems.Add(times(i).ToString("dd/MM/yyyy"))
            n.SubItems.Add(times(i).ToString("HH:mm:ss"))

            If Flags(i) = 2 Then
                tmpStr2 = ""
                'System Message
                Select Case (IDs(i) And &HFF000000) >> 24
                    Case &H0
                        tmpStr = "Power On"
                        Dim p As UInt32 = IDs(i) And &HFFFFFF
                        If (p And &H20) > 0 Then
                            tmpStr2 += "Software Reset | "
                        End If
                        If (p And &H10) > 0 Then
                            tmpStr2 += "PDI Reset | "
                        End If
                        If (p And &H8) > 0 Then
                            tmpStr2 += "Watchdog Reset | "
                        End If
                        If (p And &H4) > 0 Then
                            tmpStr2 += "Brown Out Reset | "
                        End If
                        If (p And &H2) > 0 Then
                            tmpStr2 += "External Reset | "
                        End If
                        If (p And &H1) > 0 Then
                            tmpStr2 += "Power On | "
                        End If
                        If tmpStr2.Length > 0 Then
                            tmpStr2 = tmpStr2.Substring(0, tmpStr2.Length - 3)
                        End If
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
                        Select Case (IDs(i) And &HFFFFFF)
                            Case 1
                                tmpStr2 = "Iridium"
                            Case 2
                                tmpStr2 = "PC/USB"
                            Case 3
                                tmpStr2 = "InterLogger"
                            Case Else
                                tmpStr2 = "Unknown"
                        End Select
                    Case &H7
                        tmpStr = "Ping"
                    Case Else
                        tmpStr = "Unknown System Message"
                End Select
                n.SubItems.Add(tmpStr)
                If tmpStr2 = "" Then
                    n.SubItems.Add((IDs(i) And &HFFFFFF).ToString("X"))
                Else
                    n.SubItems.Add(tmpStr2)
                End If

                lvSysEventItems.Add(n)
            Else
                n.SubItems.Add(IDs(i).ToString("X8"))
                If Flags(i) = 3 Then
                    n.SubItems.Add("Start") ' (03)")
                ElseIf Flags(i) = 1 Then
                    n.SubItems.Add("Stop") ' (01)")
                ElseIf Flags(i) = 0 Then
                    n.SubItems.Add("Unit Init") ' (00)")
                Else
                    'This can't actually happen because flag is only a 2 bit value (4 possible options)
                    n.SubItems.Add(CInt(Flags(i)).ToString("X2"))
                End If
                If useRSSI = True Then
                    n.SubItems.Add(RSSIs(i))
                End If
                If useActivity = True Then
                    n.SubItems.Add(Activities(i))
                End If
                If useSoundId = True Then
                    Try
                        If SoundIds(i) = 0 Then
                            n.SubItems.Add("NONE")
                        Else
                            n.SubItems.Add(SoundIds(i))
                        End If
                    Catch
                        n.SubItems.Add("UNKNOWN")
                    End Try
                    'n.SubItems.Add(LoggerLongIds(i))
                End If
                lvLogItems.Add(n)
            End If
        Next

        lvLogs.Items.AddRange(lvLogItems.ToArray)
        lvSysEvents.Items.AddRange(lvSysEventItems.ToArray)

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

        ReDim mretVals(0)

        If VersionStringToInteger(connectedDevVersion) >= VersionStringToInteger("V2.3.1") Then
            tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_LoggerTable, ParamLst, mretVals_loggerTable)
            Threading.Thread.Sleep(100)
        End If

        If VersionStringToInteger(connectedDevVersion) >= VersionStringToInteger("V2.1.1") Then

            BaudConfig = BaudRateConfig.BaudRateConfigs(BaudRateConfig.BaudRateConfigIDs.Baud_115200)
            'BaudConfig = BaudRateConfig.BaudRateConfigs(CInt(InputBox("BaudConfig Rate...? (0-2)")))

            ParamLst(0) = 0
            ParamLst(1) = UInt32.MaxValue
            ParamLst(2) = BaudConfig.bsel
            ParamLst(3) = BaudConfig.bscale
            ParamLst(4) = BaudConfig.BaudRate
            tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_AllRecordsFast, ParamLst, mretVals)
        Else
            tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_AllRecordsFast, New Object(), mretVals)
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

    Private Sub btnSaveLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveLog.Click
        Dim outFile As String = ""
        Dim tmpLine As String
        Dim i, j As Integer
        Dim cols As Integer = 4
        Dim useAll As Boolean = False

        If VersionStringToInteger(connectedDevVersion) >= VersionStringToInteger("V2.1.0") Then cols = 5
        If VersionStringToInteger(connectedDevVersion) >= VersionStringToInteger("V2.3.1") Then cols = 7

        If lvLogs.Items.Count > 0 Then
            Dim n As ListViewItem

            If lvLogs.SelectedIndices.Count <= 1 Then
                useAll = True
            End If

            '
            For Each n In lvLogs.Items
                If n.Selected = True Or useAll = True Then
                    tmpLine = ""
                    For i = 0 To cols
                        tmpLine &= n.SubItems(i).Text & ","
                    Next
                    tmpLine = tmpLine.Substring(0, tmpLine.Length - 1) & vbCrLf
                    outFile &= tmpLine
                End If
            Next

            If saveDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
                My.Computer.FileSystem.WriteAllText(saveDialog.FileName, outFile, False)
            End If

            Dim tmpParser As New tagParser
            tmpParser.ParseText(outFile)

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
            tmpParser.chrtMovingAvg(chrtTagParse)
            'End If
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
        If rtbSerOut.InvokeRequired = True Then
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
                        GoTo errExit
                    End Try


                    If receivedSerial = False Then
                        If SerTextCmdActive = 0 Then
                            'startTime = Now()
                            tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_DevTypeID, Nothing, RetVals)

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
                Threading.Thread.Sleep(500)
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
        MsgBox("Disconnected!")
    End Sub

    Private Sub ReadValsInCat(ByVal Cat As VarCatPage, ByVal force As Boolean)
        Dim tmpVar As VarInterface

        ControlsEnabled(False)
        Stop_Ping_Loop()
        For Each tmpVar In Vars
            If tmpVar.Category = Cat Then
                If ((tmpVar.State <> VarState.VarState_Current) Or force) Then tmpVar.ReadFromDev()
            End If
        Next
        Start_Ping_Loop()
        ControlsEnabled(True)
    End Sub

    Private Sub WriteValsInCat(ByVal Cat As VarCatPage)
        Dim tmpVar As VarInterface

        ControlsEnabled(False)
        Stop_Ping_Loop()
        For Each tmpVar In Vars
            If tmpVar.Category = Cat Then
                If tmpVar.State <> VarState.VarState_Current Then tmpVar.WriteToDev()
            End If
        Next
        Start_Ping_Loop()
        ControlsEnabled(True)
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

    Private Sub btnDisconnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

    End Sub

    Private Sub rtbSerOut_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles rtbSerOut.KeyPress
        If SerComLib.Port_InUse = False And SerTextCmdActive > 0 Then
            SerComLib.Ser.Write(e.KeyChar)
        End If
    End Sub

    Private Sub rtbSerOut_KeyUp(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles rtbSerOut.KeyUp
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
                MsgBox("Could not enter bridge mode.")
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
            chkGM862BridgeMode.Visible = Not chkGM862BridgeMode.Visible
            btnConfigureGM862.Visible = Not btnConfigureGM862.Visible
            btnBatchProgram.Visible = Not btnBatchProgram.Visible
            Button2.Visible = Not Button2.Visible
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

    Private Sub cbRFParams_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbRFParams.SelectedIndexChanged
        txtRFParams.Text = RFParamValues(cbRFParams.SelectedIndex)
    End Sub

    Private Sub cbInterloggerRFParams_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbInterloggerRFParams.SelectedIndexChanged
        txtInterloggerRFParams.Text = InterLoggerRFParamValues(cbInterloggerRFParams.SelectedIndex)
    End Sub

    Private Sub btnBatchProgram_Click(sender As Object, e As EventArgs) Handles btnBatchProgram.Click
        'Show programmer...
        FrmBatchProgrammer.Show()
    End Sub

    Private Sub lblStatus_Click(sender As Object, e As EventArgs) Handles lblStatus.Click

    End Sub
End Class