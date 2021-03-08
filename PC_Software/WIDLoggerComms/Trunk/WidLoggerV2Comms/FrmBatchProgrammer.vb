Imports WidLoggerV2Comms.SerComms
Imports WidLoggerV2Comms.VarInterface
Imports WidLoggerV2Comms.RunProcess
Imports WidLoggerV2Comms.ProgTable
Imports System.Text.RegularExpressions
Imports System.Runtime.InteropServices
Imports System.IO.Ports
Imports System.Reflection

Public Class FrmBatchProgrammer

    Public SerComLib As New SerComms(Me)
    Public DevPorts As New List(Of String)
    Public DevNames As New List(Of String)
    Public devID As UInt16
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
    Public IMIEFound As String

    Public RFParamNames As List(Of String)
    Public RFParamValues As List(Of String)
    Public InterLoggerRFParamNames As List(Of String)
    Public InterLoggerRFParamValues As List(Of String)
    Private logFileName As String
    Private logTable As ProgTable
    Private logTableColNames As List(Of String)

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

    'Sets up controls which will be enabled/disabled based on whether the device is connected or not
    Private Sub setupControlList()
        CtrlListComms = New List(Of Control)
        'CtrlListComms.Add(btnScan)
        'CtrlListComms.Add(cbDevNames)

        'CtrlListComms.Add(btnReadLog)
        CtrlListComms.Add(btnDevTimeSync)
        CtrlListComms.Add(btnEraseDataflash)

        CtrlListComms.Add(btnReadVals)
        CtrlListComms.Add(btnReadRFVals)
        CtrlListComms.Add(btnReadGSMVals)
        CtrlListComms.Add(btnWriteVals)
        CtrlListComms.Add(btnWriteRFVals)
        CtrlListComms.Add(btnWriteGSMVals)
        'CtrlListComms.Add(chkGM862BridgeMode)
        'CtrlListComms.Add(btnConfigureGM862)
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

    Private Sub cbRFParams_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbRFParams.SelectedIndexChanged
        txtRFParams.Text = RFParamValues(cbRFParams.SelectedIndex)
    End Sub

    Private Sub cbInterloggerRFParams_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles cbInterloggerRFParams.SelectedIndexChanged
        txtInterloggerRFParams.Text = InterLoggerRFParamValues(cbInterloggerRFParams.SelectedIndex)
    End Sub

    Private Sub FrmBatchProgrammer_Load(sender As Object, e As EventArgs) Handles Me.Load
        DTDiff = New TimeSpan()
        DTDiff = Now() - dtpDevDateTime.Value
        setupControlList()

        setupRFParamsComboBoxes()
        txtLogDir.Text = My.Application.Info.DirectoryPath + "\Logs"

        Me.Show()
        'TabControlMain.SelectTab(5)

        Try
            Vars.Add(New VarInterface(VarNameList.DevVersion, VarCatPage.General, TLPOverview, 1, "Version", Nothing, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_Version, SerDev_LoggerV2_ComFuncs.CMD_ByteID.NONE))
            Dim tmpTxt1 As TextBox = CType(Vars(Vars.Count - 1).Control, TextBox)
            tmpTxt1.ReadOnly = False
            'Vars(Vars.Count - 1).Value = "WID Logger 3 - V2.3.1"
            Vars.Add(New VarInterface(VarNameList.LoggerName, VarCatPage.General, TLPOverview, 2, "Logger Name", AddressOf LoggerNameValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_LoggerName, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_LoggerName))
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf LoggerNameSetHandler
            Vars(Vars.Count - 1).PostSetHandlerFunc = AddressOf LoggerNamePostSetHandler
            Vars(Vars.Count - 1).Value = "Logger Name 1"
            Vars.Add(New VarInterface(VarNameList.LoggerId, VarCatPage.General, TLPOverview, 3, "Logger ID", Nothing, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_LoggerId, SerDev_LoggerV2_ComFuncs.CMD_ByteID.NONE))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf uint32AsHexGetHandler

            Vars.Add(New VarInterface(VarNameList.Records, VarCatPage.General, pbRecordCount, txtRecordCount, Nothing, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RecordCount, SerDev_LoggerV2_ComFuncs.CMD_ByteID.NONE))
            Vars.Add(New VarInterface(VarNameList.RTCTime, VarCatPage.General, PBDevDateTime, dtpDevDateTime, AddressOf RTCTimeValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_DateTime, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_DateTime))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf RTCTimeGetHandler
            Vars.Add(New VarInterface(VarNameList.LoggerGroupId, VarCatPage.General, TLPOverview, 6, "Group ID", AddressOf uint16ValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_LoggerGroupId, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_LoggerGroupId))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf uint16GetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf uint16SetHandler
            Vars(Vars.Count - 1).Value = "0"
            Vars.Add(New VarInterface(VarNameList.WarningSignalDuration, VarCatPage.General, TLPOverview, 7, "Warning Signal Duration", AddressOf uint16ValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_WarningSignalDuration, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_WarningSignalDuration))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf uint16GetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf uint16SetHandler
            Vars(Vars.Count - 1).Value = "60"


            Vars.Add(New VarInterface(VarNameList.RFFilter, VarCatPage.RFSettings, TLPRFSet, 1, "RF Filter", AddressOf RFFilterValidateHandler, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_RFFilter, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_RFFilter))
            Vars(Vars.Count - 1).Value = "********"
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
            cbRFParams.SelectedIndex = 1
            cbInterloggerRFParams.SelectedIndex = 0

            Vars.Add(New VarInterface(VarNameList.SendEmailPeriod, VarCatPage.GSMSettings, TLPGSMSet, 1, "Send Email Period", AddressOf SendEmailPeriodValidateHandler, SerComLib, WidLoggerV2Comms.SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_SendDataGPRSPeriod, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_SendDataGPRSPeriod))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf SendEmailPeriodGetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf SendEmailPeriodSetHandler
            Vars(Vars.Count - 1).Value = CInt(0).ToString("D2") & " Yrs, " & CInt(0).ToString("D2") & " Months, " & CInt(1).ToString("D2") & " Days, " & CInt(0).ToString("D2") & " Hrs, " & CInt(0).ToString("D2") & " Mins"
            Vars.Add(New VarInterface(VarNameList.SendEmailNext, VarCatPage.GSMSettings, pbSendEmailNext, dtpSendEmailNext, AddressOf SendEmailNextValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_SendDataGPRSNext, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_SendDataGPRSNext))
            Dim t As DateTime = Now
            t = t.AddDays(1)
            t = t.AddHours(-t.Hour + 1)
            t = t.AddMinutes(-t.Minute)
            t = t.AddSeconds(-t.Second)
            Vars(Vars.Count - 1).Value = t

            Vars.Add(New VarInterface(VarNameList.NextDataReset, VarCatPage.GSMSettings, pbNextDataReset, dtpNextDataReset, AddressOf NextDataResetValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_IridiumMonthNextReset, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_IridiumMonthNextReset))
            t = Now
            t = t.AddDays(-t.Day + 1)
            t = t.AddHours(-t.Hour)
            t = t.AddMinutes(-t.Minute)
            t = t.AddSeconds(-t.Second)
            t = t.AddMonths(1)
            Vars(Vars.Count - 1).Value = t

            Vars.Add(New VarInterface(VarNameList.MonthlyData, VarCatPage.GSMSettings, TLPGSMSet, 4, "Monthly Data Allowance (bytes)", AddressOf uint16ValidateHandler, SerComLib, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_MonthByteLimit, SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_MonthByteLimit))
            Vars(Vars.Count - 1).GetHandlerFunc = AddressOf uint16GetHandler
            Vars(Vars.Count - 1).SetHandlerFunc = AddressOf uint16SetHandler
            Vars(Vars.Count - 1).Value = (30 * 1024).ToString

        Catch ex As Exception
            Stop
        End Try
    End Sub

    Private Function LoggerNamePostSetHandler(ByVal sender As VarInterface)
        Vars(VarNameList.LoggerId).ReadFromDev()
    End Function
    Private Function LoggerNameSetHandler(ByVal sender As VarInterface) As Object()
        Dim ctrl As TextBox = CType(sender.Control, TextBox)
        Dim Params(0) As Object
        'Dim tmpIndex As Integer = cbDevNames.SelectedIndex

        skipDevNameUpdate = True
        'cbDevNames.Items(tmpIndex) = "[" & cbDevNames.SelectedIndex + 1 & "] " & sender.Value
        'cbDevNames.SelectedIndex = tmpIndex
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
            '    TimerRTC.Enabled = False
        End If

        Return True
        'DTDiff = Now() - dtpDevDateTime.Value
    End Function

    Private Function RTCTimeGetHandler(ByVal sender As VarInterface, ByRef retVals() As Object) As Boolean
        DTDiff = Now() - CType(retVals(0), DateTime)
        sender.Value = retVals(0)
        sender.State = VarState.VarState_Current
        'TimerRTC.Enabled = True
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

    Private Delegate Sub rtbProgConsoleScroll2EndDelegate()

    Public Sub rtbProgConsoleScroll2End()
        If rtbProgConsole.InvokeRequired = True Then
            Dim del As New rtbProgConsoleScroll2EndDelegate(AddressOf rtbProgConsoleScroll2End)
            rtbProgConsole.Invoke(del)
        Else
            rtbProgConsole.Select(rtbProgConsole.TextLength, 0)
            rtbProgConsole.ScrollToCaret()
            rtbProgConsole.Select(rtbProgConsole.TextLength, 0)
        End If
    End Sub

    Private Delegate Sub ConsoleLogDelegate(level As String, ByVal text As String)

    Public Sub ConsoleLog(level As String, ByVal text As String)
        If rtbProgConsole.InvokeRequired = True Then
            Dim del As New ConsoleLogDelegate(AddressOf ConsoleLog)
            rtbProgConsole.Invoke(del, New Object() {text})
        Else
            Dim txtToAppend As String = Now.ToString("dd/MM/yyyy HH:mm:ss:fff ") + level.ToUpper + ": "
            Dim lenstart As Integer = txtToAppend.Length
            text = text.Replace(vbCr, "")
            Dim parts() As String = text.Split(vbLf)

            txtToAppend += parts(0) + vbCrLf
            For i = 1 To parts.Length - 1
                txtToAppend += New String(" ", lenstart) + parts(i) + vbCrLf
            Next


            rtbProgConsole.SelectionStart = rtbProgConsole.Text.Length
            'Dim col = rtbProgConsole.SelectionColor
            Select Case level.ToUpper
                Case "ERROR"
                    rtbProgConsole.SelectionColor = Color.Red
                Case "WARNING"
                    rtbProgConsole.SelectionColor = Color.Yellow
                Case "INFO"
                    rtbProgConsole.SelectionColor = Color.White
                Case "SUCCESS"
                    rtbProgConsole.SelectionColor = Color.LimeGreen
                Case "CRITICAL"
                    rtbProgConsole.SelectionColor = Color.Pink
                Case Else
                    rtbProgConsole.SelectionColor = Color.White
            End Select
            rtbProgConsole.AppendText(txtToAppend)
            rtbProgConsole.SelectionColor = Color.White
            rtbProgConsole.SelectionStart = rtbProgConsole.Text.Length
            rtbProgConsole.ScrollToCaret()
            My.Computer.FileSystem.WriteAllText(logFileName, txtToAppend, True)
            'rtbProgConsole.Select(rtbProgConsole.TextLength, 0)
            'rtbProgConsole.Text = text
            'rtbProgConsole.Text = "abc"
        End If
    End Sub

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
                                'If unProcLines(i).StartsWith("Turning GM862 On!") Then 'unProcLines(i).Contains("Turning GM862 On!") Then
                                '    SerTextCmdActive = 1
                                '    ControlsEnabledAsync(False)
                                '    EditStatusLabelText("Sending Email...")
                                'ElseIf unProcLines(i).StartsWith("Next GPRS Send Time:") Then 'Or unProcLines(i).Contains("Send Data Failed!") Or unProcLines(i).Contains("Send Data Succeeded!") Then
                                '    SerTextCmdActive = 0
                                '    EditStatusLabelText("Connected (" & FindSelectedPortNum() & ")")
                                '    ControlsEnabledAsync(True)
                                'ElseIf unProcLines(i).Contains("PWRDWN!") Then
                                '    SerTextCmdActive = 2
                                '    EditStatusLabelText("No Battery, Asleep...")
                                '    ControlsEnabledAsync(True)
                                'ElseIf unProcLines(i).StartsWith("Low Battery!") Then
                                '    SerTextCmdActive = 2
                                '    EditStatusLabelText("Low Battery, Asleep...")
                                '    ControlsEnabledAsync(True)
                                'ElseIf unProcLines(i).StartsWith("Awake...") Then
                                '    SerTextCmdActive = 3
                                '    EditStatusLabelText("Waking Up...")
                                '    ControlsEnabledAsync(True)
                                'ElseIf unProcLines(i).StartsWith("Wake from bat back mode.") Then
                                '    SerTextCmdActive = 0
                                '    EditStatusLabelText("Connected (" & FindSelectedPortNum() & ")")
                                '    ControlsEnabledAsync(True)
                                'ElseIf unProcLines(i).Contains("Next_GPRS_Real(RIY1):") Then
                                '    SerTextCmdActive = 4
                                '    EditStatusLabelText("Reeling in the years...")
                                '    FindNameInVarLst(VarNameList.SendEmailNext).State = VarState.VarState_OutDated
                                'ElseIf unProcLines(i).Contains("Next_GPRS_Real(RIY2):") Then
                                '    SerTextCmdActive = 0
                                '    EditStatusLabelText("Connected (" & FindSelectedPortNum() & ")")
                                '    ControlsEnabledAsync(True)
                                'ElseIf unProcLines(i).StartsWith("RX ID: 0x") Then
                                '    FindNameInVarLst(VarNameList.Records).State = VarState.VarState_OutDated
                                'ElseIf unProcLines(i).StartsWith("Configure GM862...") Then
                                '    SerTextCmdActive = 6
                                '    EditStatusLabelText("Configuring GM862...")
                                '    ControlsEnabledAsync(False)
                                'ElseIf unProcLines(i).StartsWith("GM862 Configure Done.") Then
                                '    SerTextCmdActive = 0
                                '    EditStatusLabelText("Connected (" & FindSelectedPortNum() & ")")
                                '    ControlsEnabledAsync(True)
                                'End If
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

    Private Sub ControlsEnabled(ByVal Enabled As Boolean)
        Dim c As Control
        For Each c In CtrlListComms
            c.Enabled = Enabled
        Next
    End Sub

    Private Function programLogger(tool As String, device As String, file As String) As Boolean
        Dim args As String = "-v -t " + tool + " -i PDI -d " + device + " program -f """ + file + """"
        Dim output As String = RunProcess.Run_Process("D:\Programs\Atmel\Studio\7.0\atbackend\atprogram.exe", args, True, False)
        ConsoleLog("INFO", "atprogram " + args)
        ConsoleLog("INFO", output)
        If output.Contains("[DEBUG] Exit successfully.") Then
            'TODO[ ]: test to see if it programmed correctly
            logTable.updateInCurrentRow("Programmed", "PASS")
            ConsoleLog("SUCCESS", "Programmed logger")
            Return True
        Else
            'TODO[ ]: test to see if it programmed correctly
            logTable.updateInCurrentRow("Programmed", "FAIL")
            ConsoleLog("ERROR", "Error programming logger!")
            Return False
        End If



        Return True
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

    Private Function writeSettings() As Boolean
        Dim vnli As VarNameList
        Dim allPassing = True

        SerComLib.Ser_PortName = DevPorts(devID)
        SerComLib.Ser_Baud = 115200
        SerComLib.Ser.Open()

        'Write all values
        For Each tmpVar In Vars
            vnli = CType(tmpVar.ID, VarNameList)
            If tmpVar.isReadOnly Then Continue For
            If tmpVar.WriteToDev() <> MyErr_Enum.MyErr_Success Then
                ConsoleLog("ERROR", "Failed to write " + vnli.ToString + " Value was " + tmpVar.Value.ToString)
                allPassing = False
                logTable.updateInCurrentRow("Write " + vnli.ToString, "FAIL")
            Else
                ConsoleLog("SUCCESS", "Successfully wrote " + vnli.ToString + " Value was " + tmpVar.Value.ToString)
                logTable.updateInCurrentRow("Write " + vnli.ToString, tmpVar.Value.ToString)
            End If
            Application.DoEvents()
            Threading.Thread.Sleep(50)
        Next

        vnli = VarNameList.LoggerId
        If FindNameInVarLst(vnli).ReadFromDev() <> MyErr_Enum.MyErr_Success Then
            ConsoleLog("ERROR", "Failed to read " + vnli.ToString + " Value was " + FindNameInVarLst(vnli).Value.ToString)
            logTable.updateInCurrentRow("Logger Id", "ERROR!")
            Return False
        Else
            If FindNameInVarLst(vnli).Value.ToString = "ERROR, PLEASE REPROGRAM LOGGER NAME" Then
                ConsoleLog("ERROR", "Read " + vnli.ToString + " Value was " + FindNameInVarLst(vnli).Value.ToString)
                logTable.updateInCurrentRow("Logger Id", FindNameInVarLst(vnli).Value.ToString)
            Else
                ConsoleLog("SUCCESS", "Successfully read " + vnli.ToString + " Value was " + FindNameInVarLst(vnli).Value.ToString)
                logTable.updateInCurrentRow("Logger Id", FindNameInVarLst(vnli).Value.ToString)
            End If
        End If

        SerComLib.Ser.Close()

        If allPassing Then
            logTable.updateInCurrentRow("Write Settings", "PASS")
        Else
            logTable.updateInCurrentRow("Write Settings", "FAIL")
        End If
        Return allPassing

    End Function

    Private Function connectToDev() As Boolean
        'Read all connected devices
        Dim comPorts() As String = System.IO.Ports.SerialPort.GetPortNames()
        Dim PortStr, tmpName As String
        Dim tmpErr As MyErr_Enum
        Dim RetVals(0) As Object

        skipDevNameUpdate = True
        skipDevNameUpdate = False
        ControlsEnabled(False)
        NoUpdate = True
        Dim vnli As VarNameList

tryagain1:
        comPorts = System.IO.Ports.SerialPort.GetPortNames()
        'All vars to unknown
        For Each tmpVar In Vars
            tmpVar.State = VarState.VarState_Unknown
        Next
        ConsoleLog("INFO", "Scanning...")

        If SerComLib.Ser.IsOpen() Then
            EditrtbSerOutText("Close Serial Port")
            SerComLib.Ser.Close()
        End If
        SerComLib.Ser_Timeout = 200 '00000
        SerComLib.Ser_Baud = SerDev_LoggerV2_ComFuncs.DevBaud

        'Clear lists
        DevPorts.Clear()
        DevNames.Clear()
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
                    End If
                End If
            End If
        Next

        NoUpdate = False
        ControlsEnabled(True)

        If DevNames.Count = 0 Then
            If MsgBox("No logger could be detected.  Please connect a logger and press ok to try again", vbOKCancel) = vbCancel Then
                ConsoleLog("ERROR", "User Aborted - no device found")
                Return False
            Else
                GoTo tryagain1
            End If
        ElseIf DevNames.Count > 1 Then
            Dim lgrs As String = ""
            For i = 0 To DevNames.Count - 1
                lgrs += "(" + Str(i) + ") " + DevNames(i) + vbCrLf
            Next
            Dim res = InputBox(lgrs + "Multiple loggers found, please select which one you want to program." + vbCrLf + "Enter the number next to the logger's name")
            Try
                devID = Convert.ToInt16(res)
            Catch ex As Exception

            End Try
        Else
            devID = 0
        End If

        'Check that the name is "yyyyyyyyyy" - ie FF FF FF FF... (20? chars)
        If DevNames(devID) <> "ÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿÿ" Then
            If MsgBox("Device contains data, are you sure you want to re-program it?", vbOKCancel) = MsgBoxResult.Cancel Then
                ConsoleLog("ERROR", "User Aborted - device contained data")
                Return False
            End If
        End If

        SerComLib.Ser_PortName = DevPorts(devID)
        SerComLib.Ser_Baud = 115200
        SerComLib.Ser.Open()

        vnli = VarNameList.DevVersion
        If FindNameInVarLst(vnli).ReadFromDev() <> MyErr_Enum.MyErr_Success Then
            ConsoleLog("ERROR", "Failed to read " + vnli.ToString + " Value was " + FindNameInVarLst(vnli).Value.ToString)
            logTable.updateInCurrentRow("Firmware Version", "ERROR!")
            Return False
        Else
            logTable.updateInCurrentRow("Firmware Version", FindNameInVarLst(vnli).Value.ToString)
            If FindNameInVarLst(vnli).Value.ToString <> txtExpectedVersionString.Text Then
                ConsoleLog("ERROR", "Read Version String (""" + FindNameInVarLst(vnli).Value.ToString + """) but does not match expected value of """ + txtExpectedVersionString.Text + """")
                logTable.updateInCurrentRow("Firmware Version Correct", "FAIL")
                Return False
            Else
                ConsoleLog("SUCCESS", "Successfully read and verfied " + vnli.ToString + " Value was """ + FindNameInVarLst(vnli).Value.ToString + """")
                logTable.updateInCurrentRow("Firmware Version Correct", "PASS")
            End If
        End If

        vnli = VarNameList.LoggerId
        If FindNameInVarLst(vnli).ReadFromDev() <> MyErr_Enum.MyErr_Success Then
            ConsoleLog("ERROR", "Failed to read " + vnli.ToString + " Value was " + FindNameInVarLst(vnli).Value.ToString)
            logTable.updateInCurrentRow("Logger Id", "ERROR!")
            Return False
        Else
            ConsoleLog("SUCCESS", "Successfully read " + vnli.ToString + " Value was " + FindNameInVarLst(vnli).Value.ToString)
            logTable.updateInCurrentRow("Logger Id", FindNameInVarLst(vnli).Value.ToString)
        End If

        SerComLib.Ser.Close()

        Return True
    End Function

    Private Function Test24GHzPickup() As Boolean
        Dim vnli As VarNameList

        SerComLib.Ser_PortName = DevPorts(devID)
        SerComLib.Ser_Baud = 115200
        SerComLib.Ser.Open()

        'Enable verbose mode
        vnli = VarNameList.VerboseMode
        FindNameInVarLst(vnli).Value = 1
        If FindNameInVarLst(vnli).WriteToDev() <> MyErr_Enum.MyErr_Success Then
            ConsoleLog("ERROR", "Failed to wrote " + vnli.ToString + " Value was " + FindNameInVarLst(vnli).Value.ToString)
            SerComLib.Ser.Close()
            Return False
        Else
            ConsoleLog("INFO", "Successfully wrote " + vnli.ToString + " Value was " + FindNameInVarLst(vnli).Value.ToString)
        End If


        'Wait for detected pickup...
        'RX ID: 0x12000010, RSSI: -81, Activity: 0.00
        Dim endtime = Now.AddSeconds(30)
        Dim found As Boolean = False
        Dim rssi As Double
        alltxt = ""
        Do
            If SerComLib.Ser.BytesToRead > 0 Then
                updateSerialText()
                'get last line
                Dim parts() As String = alltxt.Replace(vbCr, "").Split(vbLf)
                For Each part In parts
                    Dim a As Integer
                    Try
                        a = part.IndexOf(",", 25)
                        rssi = Convert.ToDouble(part.Substring(25, a - 25))
                        If part.StartsWith("RX ID: 0x") Then
                            If part.Substring(9, 8) = txtTagId.Text Then
                                logTable.updateInCurrentRow("2.4GHz Pickup Tag", txtTagId.Text)
                                logTable.updateInCurrentRow("2.4GHz Pickup RSSI (dBm)", rssi)
                                If rssi >= Convert.ToDouble(txtTagWorstRSSI.Text) Then
                                    'Picked up successfully!
                                    found = True
                                    Exit Do
                                Else
                                    'bad
                                    ConsoleLog("WARNING", "Picked up test tag with ID " + txtTagId.Text + " but RSSI (" + CStr(CInt(rssi)) + "dBm) not satisfactory")
                                End If
                            End If
                        End If
                    Catch ex As Exception
                        Continue For
                    End Try
                Next
            End If
            Application.DoEvents()
        Loop While Now < endtime

        ConsoleLog("EXTRA", alltxt)

        If found = True Then
            ConsoleLog("SUCCESS", "Picked up test tag with ID " + txtTagId.Text + " and RSSI " + CStr(CInt(rssi)) + "dBm")
            logTable.updateInCurrentRow("2.4GHz Pickup", "PASS")
        Else
            ConsoleLog("ERROR", "Failed to pick up test tag with ID " + txtTagId.Text)
            logTable.updateInCurrentRow("2.4GHz Pickup", "FAIL")
        End If

        'disable verbose mode
        vnli = VarNameList.VerboseMode
        FindNameInVarLst(vnli).Value = 0
        If FindNameInVarLst(vnli).WriteToDev() <> MyErr_Enum.MyErr_Success Then
            ConsoleLog("ERROR", "Failed to wrote " + vnli.ToString + " Value was " + FindNameInVarLst(vnli).Value.ToString)
        Else
            ConsoleLog("INFO", "Successfully wrote " + vnli.ToString + " Value was " + FindNameInVarLst(vnli).Value.ToString)
        End If

        SerComLib.Ser.Close()

        Return found
    End Function

    Private Function config9602() As Boolean
        Dim tmpErr As MyErr_Enum
        If waitforParam(SerComLib.Port_InUse, False, 1000) = False Then
            logTable.updateInCurrentRow("Configure 9602", "FAIL")
            ConsoleLog("ERROR", "Port in use!")
            Return False
        End If

        SerComLib.Ser_PortName = DevPorts(devID)
        SerComLib.Ser_Baud = 115200
        SerComLib.Ser.Open()

        tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Cmd_GM862_ConfigModule, Nothing, Nothing)

        'Expected output
        'Configure Iridium...
        'O-AT&K0
        'I-OK
        'O-AT&W0
        'I-OK
        'O-AT&Y0
        'I-OK
        'O-AT*F
        'I-OK
        'Iridium Configure Done.

        Dim expected_text As String = "Configure Iridium..." + vbCrLf +
                                      "O-AT&K0" + vbCrLf +
                                      "I-OK" + vbCrLf +
                                      "O-AT&W0" + vbCrLf +
                                      "I-OK" + vbCrLf +
                                      "O-AT&Y0" + vbCrLf +
                                      "I-OK" + vbCrLf +
                                      "O-AT*F" + vbCrLf +
                                      "I-OK" + vbCrLf +
                                      "Iridium Configure Done." + vbCrLf

        Dim endtime = Now.AddSeconds(30)
        Dim found As Boolean = False
        alltxt = ""
        Do
            If SerComLib.Ser.BytesToRead > 0 Then
                updateSerialText()
                'get last line
                If alltxt.Contains(expected_text) Then
                    found = True
                    Exit Do
                End If
            End If
            Application.DoEvents()
        Loop While Now < endtime

        ConsoleLog("EXTRA", alltxt)

        If found = True Then
            ConsoleLog("SUCCESS", "Configuring 9602 module succeeded!")
        Else
            ConsoleLog("ERROR", "Configuring 9602 module failed!")
        End If

        SerComLib.Ser.Close()

        If tmpErr = MyErr_Enum.MyErr_Success Then
            logTable.updateInCurrentRow("Configure 9602", "PASS")
        Else
            logTable.updateInCurrentRow("Configure 9602", "FAIL")
        End If

        If tmpErr = MyErr_Enum.MyErr_Success Then Return True
        Return False
    End Function

    Public Function serial_expect(expected_text As String, Optional timeout_seconds As Double = 5.0) As Boolean
        Dim endtime = Now.AddSeconds(timeout_seconds)
        Dim found As Boolean = False
        Dim alltxt_start As Integer = alltxt.Length
        Dim search_text As String
        Do
            If SerComLib.Ser.BytesToRead > 0 Then
                updateSerialText()
                'get last line
                search_text = alltxt.Substring(alltxt_start)
                If search_text.Contains(expected_text) Then
                    Return True
                    Exit Do
                End If
            End If
            SerComLib.DoEventDelay(100)
        Loop While Now < endtime
        Return False
    End Function

    Public Function serial_expect(expected_text As List(Of String), Optional timeout_seconds As Double = 5.0) As String
        Dim endtime = Now.AddSeconds(timeout_seconds)
        Dim found As Boolean = False
        Dim alltxt_start As Integer = alltxt.Length
        Dim search_text As String
        Do
            If SerComLib.Ser.BytesToRead > 0 Then
                updateSerialText()
                'get last line
                search_text = alltxt.Substring(alltxt_start)
                For Each estr As String In expected_text
                    If search_text.Contains(estr) Then
                        Return estr
                        Exit Do
                    End If
                Next
            End If
            SerComLib.DoEventDelay(100)
        Loop While Now < endtime
        Return False
    End Function

    Public Function readIMEIFrom9602() As Boolean
        Dim tmpErr As MyErr_Enum
        If waitforParam(SerComLib.Port_InUse, False, 1000) = False Then
            ConsoleLog("ERROR", "Port in use!")
            logTable.updateInCurrentRow("Read IMEI from 9602", "FAIL")
            logTable.updateInCurrentRow("IMEI", "")
            Return False
        End If

        SerComLib.Ser_PortName = DevPorts(devID)
        SerComLib.Ser_Baud = 115200
        SerComLib.Ser.Open()

        tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Cmd_GM862BridgeMode, Nothing, Nothing)

        If tmpErr <> MyErr_Enum.MyErr_Success Then
            GoTo errret
        End If
        SerComLib.DoEventDelay(2000)

        alltxt = ""

        SerComLib.Ser.Write("AT" + vbCrLf)
        If serial_expect("OK") = False Then GoTo errret

        SerComLib.Ser.Write("AT+")
        SerComLib.Ser.Write("GSN")
        SerComLib.Ser.Write(vbCrLf)
        If serial_expect("OK") = False Then GoTo errret

        Dim loc As Integer = alltxt.IndexOf("AT+GSN" + vbCrLf) + ("AT+GSN" + vbCrLf).Length
        Dim loc2 As Integer = alltxt.IndexOf(vbLf, loc)
        Dim imei As String = alltxt.Substring(loc, loc2).Replace(" ", "").Replace(vbCr, "").Replace(vbLf, "")

        ConsoleLog("EXTRA", alltxt)

        SerComLib.Ser.Write(New Byte() {&H3}, 0, 1)
        ConsoleLog("INFO", "Exiting 9602 Bridge Mode")
        SerComLib.DoEventDelay(2000)

        'If found = True Then
        ConsoleLog("SUCCESS", "Successfully read IMEI from 9602.")
        ConsoleLog("SUCCESS", "IMEI = " + imei)
        logTable.updateInCurrentRow("Read IMEI from 9602", "PASS")
        logTable.updateInCurrentRow("IMEI", imei)
        'Else
        'ConsoleLog("ERROR", "Configuring 9602 module failed!")
        'End If

        SerComLib.Ser.Close()

        Return True


errret:
        SerComLib.Ser.Write(New Byte() {&H3}, 0, 1)
        ConsoleLog("EXTRA", alltxt)
        ConsoleLog("INFO", "Exiting 9602 Bridge Mode")
        SerComLib.DoEventDelay(2000)
        SerComLib.Ser.Close()
        ConsoleLog("ERROR", "Could not get IMEI from 9602!")
        logTable.updateInCurrentRow("Read IMEI from 9602", "FAIL")
        logTable.updateInCurrentRow("IMEI", "")
        Return False
    End Function

    Public Function testIridiumNetworkConnect() As Boolean
        Dim tmpErr As MyErr_Enum
tryAgain:
        If waitforParam(SerComLib.Port_InUse, False, 1000) = False Then
            ConsoleLog("ERROR", "Port in use!")
            Return False
        End If

        Try
            SerComLib.Ser_PortName = DevPorts(devID)
            SerComLib.Ser_Baud = 115200
            SerComLib.Ser.Open()
        Catch ex As Exception
            If MsgBox("Did you plug the serial port back in? We have an error here.  Try again?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                GoTo tryAgain
            Else
                Return False
            End If
        End Try

        tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Cmd_GM862BridgeMode, Nothing, Nothing)

        SerComLib.DoEventDelay(2000)

        alltxt = ""

        SerComLib.Ser.Write("AT" + vbCrLf)
        If serial_expect("OK") = False Then GoTo errret

        SerComLib.Ser.Write("AT*R1" + vbCrLf)
        If serial_expect("OK") = False Then GoTo errret

        SerComLib.DoEventDelay(100)

        Dim sbdreg_success As Boolean = False
        Dim i As Integer = 3
        Do
            SerComLib.Ser.Write("AT")
            SerComLib.DoEventDelay(50)
            SerComLib.Ser.Write("+SBD")
            SerComLib.DoEventDelay(50)
            SerComLib.Ser.Write("REG")
            SerComLib.DoEventDelay(50)
            SerComLib.Ser.Write(vbCrLf)
            SerComLib.DoEventDelay(100)
            Dim l As List(Of String) = New List(Of String)
            l.Add("2.0")
            l.Add("OK")
            If serial_expect(l, 30) = "2,0" Then
                sbdreg_success = True
            Else
                sbdreg_success = False
            End If
            i = i - 1
        Loop While sbdreg_success = False And i > 0

        If sbdreg_success = False Then GoTo errret

        SerComLib.Ser.Write("AT*R0" + vbCrLf)
        If serial_expect("OK") = False Then ConsoleLog("CRITICAL", "Error gracefully shutting down 9602.")

        ConsoleLog("EXTRA", alltxt)

        SerComLib.Ser.Write(New Byte() {&H3}, 0, 1)
        ConsoleLog("INFO", "Exiting 9602 Bridge Mode")
        SerComLib.DoEventDelay(2000)

        ConsoleLog("SUCCESS", "9602 successfully connected Iridium to network.")

        SerComLib.Ser.Close()

        If tmpErr = MyErr_Enum.MyErr_Success Then Return True
        Return False

errret:
        SerComLib.Ser.Write("AT*R0" + vbCrLf)
        If serial_expect("OK") = False Then ConsoleLog("CRITICAL", "Error gracefully shutting down 9602.")
        SerComLib.Ser.Write(New Byte() {&H3}, 0, 1)
        ConsoleLog("EXTRA", alltxt)
        ConsoleLog("INFO", "Exiting 9602 Bridge Mode")
        SerComLib.DoEventDelay(2000)
        SerComLib.Ser.Close()
        ConsoleLog("ERROR", "Error connecting to Iridium network.")
        Return False
    End Function

    Private Sub incrementsettings()
        'increment logger name
        If cbIncLgrName.Text = "Yes" Then
            nudLoggerNameStart.Value += 1
            FindNameInVarLst(VarNameList.LoggerName).Value = txtBaseLoggerName.Text + CStr(nudLoggerNameStart.Value)
        ElseIf cbIncLgrName.Text = "Ask For New Name" Then
            FindNameInVarLst(VarNameList.LoggerName).Value = InputBox("Please enter a new logger name:", , FindNameInVarLst(VarNameList.LoggerName).Value)
        Else
            Throw New Exception("No valid selection for cbIncLgrName")
        End If

        'increment logger group
        If cbIncLgrGrp.Text = "Yes" Then
            FindNameInVarLst(VarNameList.LoggerGroupId).Value += 1
        ElseIf cbIncLgrGrp.Text = "No" Then
            'do nothing
        ElseIf cbIncLgrGrp.Text = "Ask" Then
            If MsgBox("Current logger group is " + CStr(FindNameInVarLst(VarNameList.LoggerGroupId).Value) + ". Do you want to increment it for the next logger?", vbYesNo) = MsgBoxResult.Yes Then
                FindNameInVarLst(VarNameList.LoggerGroupId).Value += 1
            End If
        ElseIf cbIncLgrGrp.Text = "Ask For Value" Then
            Dim isvalid As Boolean = False
            Do While isvalid = False
                Dim new_group As String = InputBox("Please enter a new logger group:", , FindNameInVarLst(VarNameList.LoggerGroupId).Value)
                Try
                    FindNameInVarLst(VarNameList.LoggerGroupId).Value = Convert.ToInt16(new_group)
                Catch
                    MsgBox("Invalid valuentered, please try again.")
                End Try
            Loop
            FindNameInVarLst(VarNameList.LoggerGroupId).Value = InputBox("Please enter a new logger group:", , FindNameInVarLst(VarNameList.LoggerGroupId).Value)
        Else
                Throw New Exception("No valid selection for cbIncLgrGrp")
        End If

        logTable.updateInCurrentRow("Name", FindNameInVarLst(VarNameList.LoggerName).Value)
        logTable.updateInCurrentRow("Group Id", FindNameInVarLst(VarNameList.LoggerGroupId).Value)

    End Sub

    Private Function checkCurrentDraw() As Boolean

        logTable.updateInCurrentRow("Measure Current Draw", "FAIL")
        logTable.updateInCurrentRow("Current Avg", "")
        logTable.updateInCurrentRow("Current Max", "")
        logTable.updateInCurrentRow("Supply Voltage", "")

        'ask are you ready
        MsgBox("Get ready to do a current draw measurement.")
        MsgBox("Make sure your multimeter is connected in series with the logger and set to current measure mode (mA)." + vbNewLine +
               "Disconnect the avrdragon and FTDI connector." + vbNewLine +
               "Reset your multimeter's min mean average mode." + vbNewLine +
               "Also setup a second multimeter to measure the supply voltage.", vbOKOnly Or vbExclamation)
        MsgBox("Press ok to start the reading.  Then wait for the next message box to appear", vbOKOnly Or vbExclamation)
        SerComLib.DoEventDelay(30000)
        Dim avg_str As String
        Dim avg As Double
        Do
            avg_str = InputBox("Thanks!  Now please enter the average current measured on the multimeter." + vbNewLine + "Enter the value in mA.")
            Try
                avg = Convert.ToDouble(avg_str)
            Catch ex As Exception
                If MsgBox("That wasn't a valid number.  Want to try again?", MsgBoxStyle.OkCancel Or MsgBoxStyle.Question) = MsgBoxResult.Cancel Then
                    ConsoleLog("ERROR", "Current draw reading failed.")
                    Return False
                End If
                Continue Do
            End Try
            Exit Do
        Loop
        ConsoleLog("SUCCESS", "Average Current Draw = " + CStr(avg))
        logTable.updateInCurrentRow("Current Avg", CStr(avg))

        Dim max_str As String
        Dim max As Double
        Do
            max_str = InputBox("Great.  Now enter the maximum current measured on the multimeter." + vbNewLine + "Enter the value in mA.")
            Try
                max = Convert.ToDouble(max_str)
            Catch ex As Exception
                If MsgBox("That wasn't a valid number.  Want to try again?", MsgBoxStyle.OkCancel Or MsgBoxStyle.Question) = MsgBoxResult.Cancel Then
                    ConsoleLog("ERROR", "Current draw reading failed.")
                    Return False
                End If
                Continue Do
            End Try
            Exit Do
        Loop
        ConsoleLog("SUCCESS", "Max Current Draw = " + CStr(max))
        logTable.updateInCurrentRow("Current Max", CStr(max))


        Dim volt_str As String
        Dim volt As Double
        Do
            volt_str = InputBox("Great.  Now enter the supply voltage measured on the second multimeter." + vbNewLine + "Enter the value in mA.")
            Try
                volt = Convert.ToDouble(volt_str)
            Catch ex As Exception
                If MsgBox("That wasn't a valid number.  Want to try again?", MsgBoxStyle.OkCancel Or MsgBoxStyle.Question) = MsgBoxResult.Cancel Then
                    ConsoleLog("ERROR", "Current draw reading failed.")
                    Return False
                End If
                Continue Do
            End Try
            Exit Do
        Loop
        ConsoleLog("SUCCESS", "Supply Voltage = " + CStr(volt))
        logTable.updateInCurrentRow("Supply Voltage", CStr(volt))

        If volt * avg < 100 Then
            logTable.updateInCurrentRow("Measure Current Draw", "PASS")
            Return True
        Else
            logTable.updateInCurrentRow("Measure Current Draw", "FAIL")
            ConsoleLog("ERROR", "Current draw was higher than expected!")
            MsgBox("Current draw was higher than expected!")
            Return False
        End If

    End Function

    Private Function eraseFlash() As Boolean
        Dim tmpErr As MyErr_Enum

tryAgain:
        If waitforParam(SerComLib.Port_InUse, False, 1000) = False Then
            logTable.updateInCurrentRow("Erase Flash", "FAIL")
            Return False
        End If

        Try
            SerComLib.Ser_PortName = DevPorts(devID)
            SerComLib.Ser_Baud = 115200
            SerComLib.Ser.Open()
        Catch ex As Exception
            If MsgBox("Did you plug the serial port back in? We have an error here.  Try again?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
                GoTo tryAgain
            Else
                Return False
            End If
        End Try


        FindNameInVarLst(VarNameList.Records).ReadFromDev()
        If FindNameInVarLst(VarNameList.Records).Value < 100 OrElse MsgBox("Are you sure you want to erase ALL records on the device, " + CStr(FindNameInVarLst(VarNameList.Records).Value) + " records are present?" & vbNewLine & "This may take up to 2 minutes", MsgBoxStyle.YesNo, "Warning!") = MsgBoxResult.Yes Then
            'Me.Enabled = False
eraseDataflash_TryAgain:
            ConsoleLog("INFO", "Attempting to erase dataflash...")
            tmpErr = SerComLib.Ser_SendCmd_NS(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Cmd_EraseDataflash, Nothing, Nothing)
            If tmpErr = MyErr_Enum.MyErr_Success Then
                'MsgBox("Erase Succeeded")
                ConsoleLog("SUCCESS", "Dataflash erase suceeded.")
            ElseIf tmpErr < 0 Then
                'MsgBox("Erase Failed")
                If MsgBox("Erasing dataflash failed, try again?", vbYesNo Or vbExclamation) = MsgBoxResult.Yes Then
                    GoTo eraseDataflash_TryAgain
                End If
                ConsoleLog("ERROR", "Erase data flash failed.")
                SerComLib.Ser.Close()
                logTable.updateInCurrentRow("Erase Flash", "FAIL")
                Return False
            End If
            'Me.Enabled = True

            'Update Record Count
            FindNameInVarLst(VarNameList.Records).ReadFromDev()
            If FindNameInVarLst(VarNameList.Records).Value > 5 Then
                ConsoleLog("ERROR", "Weird..  Dataflash read worked but then the record count was > 5...")
                If MsgBox("Erasing dataflash failed - record count is > 5 " + CStr(FindNameInVarLst(VarNameList.Records).Value) + ", try again?", vbYesNo Or vbExclamation) = MsgBoxResult.Yes Then
                    GoTo eraseDataflash_TryAgain
                End If
                ConsoleLog("ERROR", "Giving up erasing data flash.")
                SerComLib.Ser.Close()
                logTable.updateInCurrentRow("Erase Flash", "FAIL")
                Return False
            End If
        Else
            ConsoleLog("ERROR", "User canceled udata flash erase.")
            SerComLib.Ser.Close()
            logTable.updateInCurrentRow("Erase Flash", "FAIL")
            Return False
        End If

        SerComLib.Ser.Close()
        logTable.updateInCurrentRow("Erase Flash", "PASS")
        Return True
    End Function

    Private Sub genColNames()
        Dim vnli As VarNameList

        logTableColNames = New List(Of String)
        logTableColNames.Add("Name")
        logTableColNames.Add("Logger Id")
        logTableColNames.Add("Group Id")
        logTableColNames.Add("Overall Status")

        If chkProgramLogger.Checked = True Then
            logTableColNames.Add("Programmed")
        End If

        logTableColNames.Add("Firmware Version")
        logTableColNames.Add("Firmware Version Correct")

        If chkSetSettings.Checked = True Then
            logTableColNames.Add("Write Settings")
            For Each tmpVar In Vars
                vnli = CType(tmpVar.ID, VarNameList)
                If tmpVar.isReadOnly Then Continue For
                logTableColNames.Add("Write " + vnli.ToString)
            Next
        End If

        If chk24GHzPickup.Checked = True Then
            logTableColNames.Add("2.4GHz Pickup")
            logTableColNames.Add("2.4GHz Pickup Tag")
            logTableColNames.Add("2.4GHz Pickup RSSI (dBm)")
        End If

        If chkTest433MHzInterLogger.Checked = True Then
            'TODO[ ]:....................
        End If

        If chkConfig9602.Checked = True Then
            logTableColNames.Add("Configure 9602")
        End If

        If chkRead9602IMEI.Checked = True Then
            logTableColNames.Add("Read IMEI from 9602")
            logTableColNames.Add("IMEI")
        End If

        If chkTestIridiumTx.Checked = True Then
            'logTableColNames.Add("Test Iridium Tx")
            'TODO[ ]................

        End If

        If chkMeasureCurrent.Checked = True Then
            logTableColNames.Add("Measure Current Draw")
            logTableColNames.Add("Current Avg")
            logTableColNames.Add("Current Max")
            logTableColNames.Add("Supply Voltage")
        End If

        If chkTestIridiumNetworkConnect.Checked = True Then
            'logTableColNames.Add("Test Iridium Network Connect")
            'TODO[ ]:.............
        End If

        If chkEraseFlash.Checked = True Then
            logTableColNames.Add("Erase Flash")
        End If
    End Sub

    Private Sub btnStartProgramming_Click(sender As Object, e As EventArgs) Handles btnStartProgramming.Click

        Dim i As Integer = 0

        'clear log text
        rtbProgConsole.Text = ""

        'create log file
        If My.Computer.FileSystem.DirectoryExists(txtLogDir.Text) = False Then
            My.Computer.FileSystem.CreateDirectory(txtLogDir.Text)
        End If

        logFileName = txtLogDir.Text + "\ProgramSessionLogs" + Now.ToString("ddMMyyyy HHmmss") + ".log"
        My.Computer.FileSystem.WriteAllText(logFileName, "PROGRAMMING SESSION", False)

        'create log table
        genColNames()
        logTable = New ProgTable(logTableColNames)
        'same filename as above but .csv instead of .log
        logTable.filename = logFileName.Substring(0, logFileName.Length - 3) + "csv"
        logTable.addNewRow()
        FindNameInVarLst(VarNameList.LoggerName).Value = txtBaseLoggerName.Text + CStr(nudLoggerNameStart.Value)
        logTable.updateInCurrentRow("Name", FindNameInVarLst(VarNameList.LoggerName).Value)
        logTable.updateInCurrentRow("Group Id", FindNameInVarLst(VarNameList.LoggerGroupId).Value)
        logTable.saveAsCsv(False) ' error if the file already exists!  But we will overwrite from now on.

        'Good work Matt.  Nice goto's there, super easy to understand...
        Do
            i = i + 1
            If i > 1 Then
                logTable.saveAsCsv(True)
                logTable.addNewRow()
                incrementsettings()
            End If

            ConsoleLog("INFO", "PROGRAMMING NEW DEVICE (NUMBER " + CStr(i) + ")")
            GoTo continueNormalProg

tryProgLgrAgain:
            ConsoleLog("INFO", "PROGRAMMING DEVICE (NUMBER " + CStr(i) + ") AGAIN")
            If MsgBox("Press Ok to keep the failed row in the csv." + vbNewLine + "Press Cancel to overwrite the failed row on this pass.", vbOKCancel) = MsgBoxResult.Ok Then
                logTable.saveAsCsv(True)
                logTable.addNewRow()
                i = i + 1
            End If

continueNormalProg:
            MsgBox("Setup the logger to program and press OK when ready.")
            If chkProgramLogger.Checked = True Then
                ConsoleLog("INFO", "Programming Logger with Firmware """ + txtProgramFile.Text + """")
                While programLogger(cbProgTool.Text, cbProgDevice.Text, txtProgramFile.Text) = False
                    If MsgBox("Programming logger failed, try again?", vbYesNo Or vbQuestion) = MsgBoxResult.No Then
                        GoTo progerr
                    End If
                End While
                ConsoleLog("INFO", "Waiting for device to start up.")
                SerComLib.DoEventDelay(8000)
            End If

            If connectToDev() = False Then GoTo progerr

            If chkSetSettings.Checked = True Then
                ConsoleLog("INFO", "Writing Settings to Logger")
                While writeSettings() = False
                    If MsgBox("Wite Setting failed, try again?", vbYesNo Or vbQuestion) = MsgBoxResult.No Then
                        GoTo progerr
                    End If
                End While
            End If

            If chk24GHzPickup.Checked = True Then
                ConsoleLog("INFO", "2.4GHz Test")
                While Test24GHzPickup() = False
                    If MsgBox("Test 2.4GHz Pickup failed, try again?", vbYesNo Or vbQuestion) = MsgBoxResult.No Then
                        GoTo progerr
                    End If
                End While
            End If

            If chkTest433MHzInterLogger.Checked = True Then
                ConsoleLog("INFO", "Interlogger 433MHz Test")
            End If

            If chkConfig9602.Checked = True Then
                ConsoleLog("INFO", "Configure 9602")
                While config9602() = False
                    If MsgBox("Config 9602 failed, try again?", vbYesNo Or vbQuestion) = MsgBoxResult.No Then
                        GoTo progerr
                    End If
                End While
            End If

            If chkRead9602IMEI.Checked = True Then
                ConsoleLog("INFO", "Reading IMEI from 9602")
                While readIMEIFrom9602() = False
                    If MsgBox("Read IMEI from 9602 failed, try again?", vbYesNo Or vbQuestion) = MsgBoxResult.No Then
                        GoTo progerr
                    End If
                End While
            End If

            If chkTestIridiumTx.Checked = True Then
                ConsoleLog("INFO", "Test Iridium Tx")

            End If

            If chkMeasureCurrent.Checked = True Then
                ConsoleLog("INFO", "Measure Current Draw")
                While checkCurrentDraw() = False
                    If MsgBox("Measure Current Draw failed, try again?", vbYesNo Or vbQuestion) = MsgBoxResult.No Then
                        GoTo progerr
                    End If
                End While
            End If

            If chkTestIridiumNetworkConnect.Checked = True Then
                ConsoleLog("INFO", "Test Iridium Network Connect")
                While testIridiumNetworkConnect() = False
                    If MsgBox("Test Iridium Network Connect failed, try again?", vbYesNo Or vbQuestion) = MsgBoxResult.No Then
                        GoTo progerr
                    End If
                End While
            End If

            If chkEraseFlash.Checked = True Then
                ConsoleLog("INFO", "Erase Flash")
                While eraseFlash() = False
                    If MsgBox("Erase Flash failed, try again?", vbYesNo Or vbQuestion) = MsgBoxResult.No Then
                        GoTo progerr
                    End If
                End While
            End If

            ConsoleLog("SUCCESS", "All Programming and tests for logger " + CStr(i) + " finished Successfully")
            logTable.updateInCurrentRow("Overall Status", "PASS")
            MsgBox("You just finished programming logger with name " + FindNameInVarLst(VarNameList.LoggerName).Value)
            Continue Do
progerr:
            logTable.updateInCurrentRow("Overall Status", "FAIL")
            Dim res = MsgBox("Programming logger " + CStr(i) + " failed. Try again?" + vbCrLf + " To try again press yes" + vbCrLf + "to skip to the next logger press no" + vbCrLf + "To abort all programming press cancel", vbYesNoCancel)
            If res = MsgBoxResult.Yes Then
                GoTo tryProgLgrAgain
            ElseIf res = MsgBoxResult.No Then
                'do nothing
            ElseIf res = MsgBoxResult.Cancel Then
                Exit Do
            End If



        Loop While MsgBox("Do you want to program another logger?", vbYesNo, "Another?") = MsgBoxResult.Yes

        logTable.saveAsCsv(True)
        Return

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If ofdProgFile.ShowDialog() = Windows.Forms.DialogResult.OK Then
            txtProgramFile.Text = ofdProgFile.FileName
        End If
    End Sub

    Private Sub ofdProgFile_FileOk(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles ofdProgFile.FileOk

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        fbdLogDir.SelectedPath = txtLogDir.Text
        If fbdLogDir.ShowDialog = Windows.Forms.DialogResult.OK Then
            txtLogDir.Text = fbdLogDir.SelectedPath
        End If
    End Sub
End Class