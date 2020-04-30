Imports WidLoggerV2Comms.SerComms

Public Class SerDev_LoggerV2_ComFuncs
    Inherits WidLoggerV2Comms.SerDev_ComFuncs

    Public Const DevID As UInt16 = &H1
    Public Const DevBaud As Integer = 115200

    Public Tx_Rate As Decimal
    Public Tx_RecordCount As Long
    Public Tx_TimeRemain As Integer
    Public Tx_RecordsDone As Long
    Public Tx_InProgress As Boolean
    Public Tx_Cancel As Boolean
    Public Tx_PercDone As Decimal

    Enum CMD_ByteID As Byte
        Get_DevTypeID = &H0
        Get_AllRecordsFast = &H1
        Cmd_SaveDummy_Record = &H2
        Get_EntryAtAddress = &H3
        Cmd_EraseDataflash = &H4
        Cmd_PollDataFlashErased = &H5
        Get_Version = &H6
        Get_RecordCount = &H7
        Set_RecordCount = &H8
        Get_DateTime = &H9
        Set_DateTime = &HA
        Get_RFChan = &HB
        Set_RFChan = &HC
        Get_RFID = &HD
        Set_RFID = &HE
        Get_RFFilter = &HF
        Set_RFFilter = &H10
        Get_SendDataGPRSPeriod = &H11
        Set_SendDataGPRSPeriod = &H12
        Get_SendDataGPRSNext = &H13
        Set_SendDataGPRSNext = &H14
        Get_LoggerName = &H15
        Set_LoggerName = &H16
        Get_VerboseMode = &H17
        Set_VerboseMode = &H18
        Cmd_GM862 = &H19
        Cmd_GM862BridgeMode = &H1A
        Get_GPRSVals = &H1B
        Set_GPRSVals = &H1C
        Get_EmailToAddress = &H1D
        Set_EmailToAddress = &H1E
        Cmd_UpdateTimeFromGSM = &H1F
        Get_SendEmailAttempts = &H20
        Set_SendEmailAttempts = &H21
        Cmd_GM862_ConfigModule = &H22
        Get_RFParams = &H23
        Set_RFParams = &H24

        NONE = &HFF
    End Enum

    Public Sub New(ByRef Parent As SerComms)
        InitCmdTable()
        Me.SerComParent = Parent
    End Sub

    Private Sub InitCmdTable()
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_DevTypeID, AddressOf Ser_CMD_GetDevTypeID))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_AllRecordsFast, AddressOf Ser_CMD_Get_AllRecordsFast))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Cmd_SaveDummy_Record, AddressOf Ser_CMD_Cmd_SaveDummy_Record))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_EntryAtAddress, AddressOf Ser_CMD_Get_EntryAtAddress))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Cmd_EraseDataflash, AddressOf Ser_CMD_Cmd_EraseDataflash))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Cmd_PollDataFlashErased, AddressOf Ser_CMD_Cmd_PollDataFlashErased))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_Version, AddressOf Ser_CMD_Get_Version))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_RecordCount, AddressOf Ser_CMD_Get_RecordCount))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Set_RecordCount, AddressOf Ser_CMD_Set_RecordCount))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_DateTime, AddressOf Ser_CMD_Get_DateTime))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Set_DateTime, AddressOf Ser_CMD_Set_DateTime))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_RFChan, AddressOf Ser_CMD_Get_RFChan))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Set_RFChan, AddressOf Ser_CMD_Set_RFChan))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_RFID, AddressOf Ser_CMD_Get_RFID))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Set_RFID, AddressOf Ser_CMD_Set_RFID))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_RFFilter, AddressOf Ser_CMD_Get_RFFilter))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Set_RFFilter, AddressOf Ser_CMD_Set_RFFilter))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_SendDataGPRSPeriod, AddressOf Ser_CMD_Get_SendDataGPRSPeriod))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Set_SendDataGPRSPeriod, AddressOf Ser_CMD_Set_SendDataGPRSPeriod))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_SendDataGPRSNext, AddressOf Ser_CMD_Get_SendDataGPRSNext))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Set_SendDataGPRSNext, AddressOf Ser_CMD_Set_SendDataGPRSNext))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_LoggerName, AddressOf Ser_CMD_Get_LoggerName))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Set_LoggerName, AddressOf Ser_CMD_Set_LoggerName))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_VerboseMode, AddressOf Ser_CMD_Get_VerboseMode))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Set_VerboseMode, AddressOf Ser_CMD_Set_VerboseMode))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Cmd_GM862, AddressOf Ser_CMD_Cmd_GM862))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Cmd_GM862BridgeMode, AddressOf Ser_CMD_Cmd_GM862BridgeMode))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_GPRSVals, AddressOf Ser_CMD_Get_GPRSVals))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Set_GPRSVals, AddressOf Ser_CMD_Set_GPRSVals))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_EmailToAddress, AddressOf Ser_CMD_Get_EmailToAddress))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Set_EmailToAddress, AddressOf Ser_CMD_Set_EmailToAddress))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Cmd_UpdateTimeFromGSM, AddressOf Ser_CMD_Cmd_UpdateTimeFromGSM))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_SendEmailAttempts, AddressOf Ser_CMD_Get_SendEmailAttempts))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Set_SendEmailAttempts, AddressOf Ser_CMD_Set_SendEmailAttempts))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Cmd_GM862_ConfigModule, AddressOf Ser_CMD_Cmd_GM862_ConfigModule))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Get_RFParams, AddressOf Ser_CMD_Get_RFParams))
        CmdAry.Add(New Ser_CMD(CMD_ByteID.Set_RFParams, AddressOf Ser_CMD_Set_RFParams))
    End Sub

    Private Function bytes2Time(ByVal Bytes() As Byte) As DateTime
        Try
            Return New DateTime(Bytes(5) + 2000, Bytes(4), Bytes(3), Bytes(2), Bytes(1), Bytes(0))
        Catch ex As Exception
            Return New DateTime(100, 1, 1)
        End Try

    End Function

    Private Function time2Bytes(ByVal Date_Time As DateTime) As Byte()
        Dim retBytes(5) As Byte
        retBytes(0) = Date_Time.Second
        retBytes(1) = Date_Time.Minute
        retBytes(2) = Date_Time.Hour
        retBytes(3) = Date_Time.Day
        retBytes(4) = Date_Time.Month
        retBytes(5) = Date_Time.Year - 2000

        Return retBytes
    End Function

    Private Function Ser_CMD_GetDevTypeID(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(1) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 2)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = BitConverter.ToUInt16(tmpBuf, 0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_AllRecordsFast(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        'inputs: 
        '   ParamLst(0) = Starting Record (uint32_t)
        '   ParamLst(1) = Ending Record (uint32_t)
        '   ParamLst(2) = BaudRate bsel (int16)
        '   ParamLst(3) = BaudRate bscale (int8 or sbyte)
        '   ParamLst(4) = baudRate as integer in bps (uint32)

        Dim tmpBytes(20) As Byte
        Dim tmpErr As MyErr_Enum
        Dim recordCount As UInt32
        Dim recPerGrp As UInt16
        Dim startTime As DateTime
        Dim checkSUM As UInt16
        Dim recCount As UInt32
        Dim recChkSum As UInt16
        Dim wasErr As Boolean
        Dim i, j As Integer
        Dim tmpDateTime As DateTime
        Dim tmpID As UInt32
        Dim tmpRSSI As SByte
        Dim tmpBufDateTime As New List(Of DateTime)
        Dim tmpBufID As New List(Of UInt32)
        Dim tmpBufFlags As New List(Of Byte)
        Dim tmpBufRSSI As New List(Of Integer)
        Dim lastPDone As Decimal
        Dim recSinceUpdate As Integer
        Dim rxPeriod As TimeSpan
        Dim tmpFlags As Byte
        Dim includeRSSI As Boolean = False
        Dim SortPreceedingData As Boolean = False
        Dim tmpBuf(3) As Byte
        Dim RxRecCount As UInt32

        Dim RECORD_LEN As Integer = 10 'compressed len is 9 but flags are tx'd as a seperate byte so len here is 10

        Try
            If SerComParent.VersionStringToInteger(SerComParent.DevVersion) >= SerComParent.VersionStringToInteger("V2.1.1") Then
                includeRSSI = True
                RECORD_LEN = 11 'compressed len is 10 but flags are tx'd as a seperate byte so len here is 11
                SortPreceedingData = True
            ElseIf SerComParent.VersionStringToInteger(SerComParent.DevVersion) >= SerComParent.VersionStringToInteger("V2.1.0") Then
                includeRSSI = True
                RECORD_LEN = 11 'compressed len is 10 but flags are tx'd as a seperate byte so len here is 11
                SortPreceedingData = False
            ElseIf SerComParent.VersionStringToInteger(SerComParent.DevVersion) < SerComParent.VersionStringToInteger("V2.1.0") Then
                RECORD_LEN = 10
                includeRSSI = False
                SortPreceedingData = False
            Else
                '?????
            End If
        Catch ex As Exception
            RECORD_LEN = 10
            includeRSSI = False
        End Try

        If SortPreceedingData = True Then
            'Receive Record Count
            tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 4)
            If tmpErr < 0 Then Return tmpErr
            RxRecCount = BitConverter.ToUInt32(tmpBuf, 0)

            'Tx Starting Record
            If ParamLst.Count >= 1 Then
                tmpBuf = BitConverter.GetBytes(CType(ParamLst(0), UInt32))
            Else
                tmpBuf = BitConverter.GetBytes(CType(0, UInt32))
            End If
            tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 4)
            If tmpErr < 0 Then Return tmpErr

            'Tx Ending Record
            If ParamLst.Count >= 2 Then
                tmpBuf = BitConverter.GetBytes(CType(ParamLst(1), UInt32))
            Else
                tmpBuf = BitConverter.GetBytes(CType(RxRecCount, UInt32))
            End If
            tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 4)
            If tmpErr < 0 Then Return tmpErr

            'Tx BaudRate bsel
            If ParamLst.Count >= 5 Then
                tmpBuf = BitConverter.GetBytes(CType(ParamLst(2), Int16))
            Else
                tmpBuf = BitConverter.GetBytes(CType(75, Int16))
            End If
            tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 2)
            If tmpErr < 0 Then Return tmpErr

            'Tx BaudRate bscale
            If ParamLst.Count >= 5 Then
                tmpBuf = BitConverter.GetBytes(CType(ParamLst(3), SByte))
            Else
                tmpBuf = BitConverter.GetBytes(CType(-6, SByte))
            End If
            tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 1)
            If tmpErr < 0 Then Return tmpErr
        End If

        SerComParent.Ser.Close()
        Tx_Cancel = False
        SerComParent.Ser.ReadBufferSize = 1024 * 1024 * 32 / 8
        If SortPreceedingData = True AndAlso ParamLst.Count >= 5 Then
            SerComParent.Ser.BaudRate = ParamLst(4)
        Else
            SerComParent.Ser.BaudRate = 921600
        End If
        SerComParent.Ser.Open()

        SerComParent.Ser.Write("g")

        'tmpErr = SerComParent.Ser_Rx_Byte_BG(SerComParent.Ser_Timeout, tmpByte(0))
        'If tmpErr < 0 Then GoTo WasErr 'TODO[ ]: This will freeze up the MCU!! fix this!

        'Rx total records
        tmpErr = SerComParent.Ser_Rx_UInt32_BG(SerComParent.Ser_Timeout, recordCount)
        If tmpErr < 0 Then GoTo WasErrLbl 'TODO[ ]: Exiting now will freeze up the MCU!! fix this!
        Tx_RecordCount = recordCount

        'If SortPreceedingData = True Then
        'start setting up data
        'If ParamLst.Count >= 2 Then
        'tmpBufDateTime.Capacity = ParamLst(1) - ParamLst(0)
        'tmpBufID.Capacity = ParamLst(1) - ParamLst(0)
        'End If
        'Else
        tmpBufDateTime.Capacity = recordCount
        tmpBufID.Capacity = recordCount
        'End If


        startTime = Now()

        While (recCount < recordCount)
            'Rx Records per group
            tmpErr = SerComParent.Ser_Rx_UInt16_BG(SerComParent.Ser_Timeout, recPerGrp)
            If tmpErr < 0 Then
                GoTo WasErrLbl 'TODO[ ]: Exiting now will freeze up the MCU!! fix this!
            End If

            'Reset CheckSUM
            checkSUM = &HFFFF
            recChkSum = 0
            wasErr = False
            i = 0

            For i = 0 To recPerGrp - 1

                If Tx_Cancel = True Then
                    SerComParent.Ser.Write("c")
                    GoTo WasErrLbl
                End If

                'Rx 10 bytes (1 record)
                While SerComParent.Ser.BytesToRead < (RECORD_LEN + 1)
                    'Threading.
                End While
                SerComParent.Ser.Read(tmpBytes, 0, RECORD_LEN + 1)

                For j = 0 To RECORD_LEN
                    checkSUM = CRC_Lib.CRC.update_crc_16(checkSUM, CChar(ChrW(tmpBytes(j))))
                Next

                tmpDateTime = bytes2Time(tmpBytes)
                tmpID = BitConverter.ToUInt32(tmpBytes, 6)
                tmpFlags = tmpBytes(10)

                tmpBufDateTime.Add(tmpDateTime)
                tmpBufID.Add(tmpID)
                tmpBufFlags.Add(tmpFlags)
                If includeRSSI Then
                    tmpRSSI = IIf(tmpBytes(11) < 128, tmpBytes(11), tmpBytes(11) - 256)
                    'tmpRSSI = Convert.ToSByte()
                    tmpBufRSSI.Add(tmpRSSI)
                End If

                recSinceUpdate += 1

                Tx_PercDone = Math.Round(((CDec((recCount + i)) / CDec(recordCount)) * 100), 2)
                If ((Tx_PercDone Mod 2 = 0) And (Tx_PercDone > lastPDone)) Or ((recSinceUpdate > 50) And Tx_PercDone > 0) Then
                    lastPDone = Tx_PercDone
                    recSinceUpdate = 0
                    rxPeriod = Now - startTime

                    Tx_RecordsDone = recCount + i
                    If rxPeriod.TotalMilliseconds > 0 Then
                        Tx_Rate = Math.Floor((CDec(((recCount + i) * 10)) / CDec((rxPeriod.TotalMilliseconds / 1000))) / 1000)
                        Tx_TimeRemain = Math.Ceiling(((CDec(rxPeriod.TotalSeconds) / Tx_PercDone) * (100 - Tx_PercDone)))
                    End If
                End If
            Next

            If wasErr = False Then
                'Rx CheckSUM
                tmpErr = SerComParent.Ser_Rx_UInt16_BG(SerComParent.Ser_Timeout, recChkSum)
                If tmpErr < 0 Then
                    GoTo WasErrLbl 'TODO[ ]: Exiting now will freeze up the MCU!! fix this!
                End If

            End If
            If Tx_Cancel = True Then
                'TODO[ ]: implement this code!
                SerComParent.Ser.Write("c")
                GoTo WasErrLbl
            End If

            If recChkSum <> checkSUM Then
                If recPerGrp > 4 Then
                    SerComParent.Ser.Write("r")
                    lastPDone = 0
                Else
                    SerComParent.Ser.Write("c")
                    GoTo WasErrLbl
                End If
            Else
                'update record count
                recCount += recPerGrp
                SerComParent.Ser.Write("g")
            End If
        End While

        ReDim RetVals(3)
        RetVals(0) = tmpBufDateTime
        RetVals(1) = tmpBufID
        RetVals(2) = tmpBufFlags
        RetVals(3) = tmpBufRSSI


        SerComParent.Ser.Close()
        SerComParent.Ser.BaudRate = 115200
        SerComParent.Ser.Open()

        Return MyErr_Enum.MyErr_Success

WasErrLbl:
        Tx_InProgress = False
        SerComParent.Ser.Close()
        SerComParent.Ser.BaudRate = 115200
        SerComParent.Ser.Open()
        Return tmpErr
    End Function

    Private Function Ser_CMD_Cmd_SaveDummy_Record(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(1) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 2)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = BitConverter.ToInt16(tmpBuf, 0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_EntryAtAddress(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(1) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 2)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = BitConverter.ToInt16(tmpBuf, 0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Cmd_EraseDataflash(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum

        'Can alter this and it will be returned to the true value when the send_cmd function returns
        SerComParent.Ser_Timeout = 80000

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Cmd_PollDataFlashErased(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(1) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 2)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = BitConverter.ToInt16(tmpBuf, 0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_Version(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(100) As Byte
        Dim tmpErr As MyErr_Enum
        Dim i As Integer

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 100)
        If tmpErr < 0 Then Return tmpErr

        'Must do this for variable length strings!
        For i = 0 To 20
            If tmpBuf(i) = 0 Then Exit For
        Next

        RetVals(0) = System.Text.Encoding.Default.GetString(tmpBuf).Substring(0, i)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_RecordCount(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(3) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 4)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = BitConverter.ToUInt32(tmpBuf, 0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Set_RecordCount(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(3) As Byte
        Dim tmpErr As MyErr_Enum

        tmpBuf = BitConverter.GetBytes(CType(ParamLst(0), UInt32))

        tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 4)
        If tmpErr < 0 Then Return tmpErr

        Return MyErr_Enum.MyErr_Success
    End Function

    Private Function Ser_CMD_Get_DateTime(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(5) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 6)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = bytes2Time(tmpBuf)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Set_DateTime(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(5) As Byte
        Dim tmpErr As MyErr_Enum

        tmpBuf = time2Bytes(CType(ParamLst(0), DateTime))

        tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 6)
        If tmpErr < 0 Then Return tmpErr

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_RFChan(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(0) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 1)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = tmpBuf(0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Set_RFChan(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(0) As Byte
        Dim tmpErr As MyErr_Enum

        tmpBuf(0) = ParamLst(0)

        tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 1)
        If tmpErr < 0 Then Return tmpErr

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_RFID(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(5) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 5)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = tmpBuf

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Set_RFID(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(5) As Byte
        Dim tmpErr As MyErr_Enum

        tmpBuf = ParamLst(0)

        tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 5)
        If tmpErr < 0 Then Return tmpErr

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_RFFilter(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(7) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 8)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = System.Text.Encoding.Default.GetString(tmpBuf) 'BitConverter.ToString(tmpBuf, 0, 8)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Set_RFFilter(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(7) As Byte
        Dim tmpErr As MyErr_Enum

        tmpBuf = System.Text.Encoding.Default.GetBytes(CStr(ParamLst(0)))

        tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 8)
        If tmpErr < 0 Then Return tmpErr

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_SendDataGPRSPeriod(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(5) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 6)
        If tmpErr < 0 Then Return tmpErr

        'just return the bytes!.. a timespan object dosen't accomodate us (no months or years element)
        RetVals(0) = tmpBuf

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Set_SendDataGPRSPeriod(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(5) As Byte
        Dim tmpErr As MyErr_Enum

        'raw mytes (see get function above)
        tmpBuf = ParamLst(0)

        tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 6)
        If tmpErr < 0 Then Return tmpErr

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_SendDataGPRSNext(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(5) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 6)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = bytes2Time(tmpBuf)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Set_SendDataGPRSNext(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(5) As Byte
        Dim tmpErr As MyErr_Enum

        tmpBuf = time2Bytes(CType(ParamLst(0), DateTime))

        tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 6)
        If tmpErr < 0 Then Return tmpErr

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_LoggerName(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(20) As Byte
        Dim tmpErr As MyErr_Enum
        Dim i As Integer
        'Dim tmpStr As String

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 20)
        If tmpErr < 0 Then Return tmpErr

        For i = 0 To 19
            If tmpBuf(i) = 0 Then Exit For
        Next

        'tmpStr = System.Text.Encoding.Default.GetString(tmpBuf)
        RetVals(0) = System.Text.Encoding.Default.GetString(tmpBuf).Substring(0, i)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Set_LoggerName(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(20) As Byte
        Dim tmpBuf1() As Byte
        Dim tmpErr As MyErr_Enum
        Dim i As Integer

        tmpBuf1 = System.Text.Encoding.Default.GetBytes(CStr(ParamLst(0)))

        For i = 0 To 19
            tmpBuf(i) = 0
        Next

        For i = 0 To tmpBuf1.Length - 1
            tmpBuf(i) = tmpBuf1(i)
        Next

        tmpBuf(19) = 0

        tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 20)
        If tmpErr < 0 Then Return tmpErr

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_VerboseMode(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(0) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 1)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = tmpBuf(0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Set_VerboseMode(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(0) As Byte
        Dim tmpErr As MyErr_Enum

        tmpBuf(0) = ParamLst(0)

        tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 1)
        If tmpErr < 0 Then Return tmpErr

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Cmd_GM862(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(1) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 2)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = BitConverter.ToInt16(tmpBuf, 0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Public Function Ser_CMD_GetDevTypeID(ByRef DevID As UInt16) As MyErr_Enum
        Dim RetVals(0) As Object
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_DevTypeID, Nothing, RetVals)

        DevID = CType(RetVals(0), UInt16)

        Return tmpErr

    End Function


    Private Function Ser_CMD_Cmd_GM862BridgeMode(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_GPRSVals(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(2)() As Byte
        Dim tmpErr As MyErr_Enum
        Dim i, j As Integer
        'Dim tmpStr As String

        ReDim RetVals(2)

        For j = 0 To 2
            ReDim Preserve tmpBuf(j)(30)
            tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf(j), 30)
            If tmpErr < 0 Then Return tmpErr

            For i = 0 To 29
                If tmpBuf(j)(i) = 0 Then Exit For
            Next

            'tmpStr = System.Text.Encoding.Default.GetString(tmpBuf)
            RetVals(j) = System.Text.Encoding.Default.GetString(tmpBuf(j)).Substring(0, i)
        Next

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Set_GPRSVals(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(2)() As Byte
        Dim tmpBuf1() As Byte
        Dim tmpErr As MyErr_Enum
        Dim i, j As Integer

        For j = 0 To 2
            ReDim Preserve tmpBuf(j)(30)
            tmpBuf1 = System.Text.Encoding.Default.GetBytes(CStr(ParamLst(j)))

            'clear the final buffer
            For i = 0 To 29
                tmpBuf(j)(i) = 0
            Next

            'copy the real data into the final buffer
            For i = 0 To tmpBuf1.Length - 1
                tmpBuf(j)(i) = tmpBuf1(i)
            Next

            tmpBuf(j)(29) = 0

            tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf(j), 30)
            If tmpErr < 0 Then Return tmpErr
        Next

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_EmailToAddress(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(60) As Byte
        Dim tmpErr As MyErr_Enum
        Dim i As Integer
        'Dim tmpStr As String

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 60)
        If tmpErr < 0 Then Return tmpErr

        For i = 0 To 59
            If tmpBuf(i) = 0 Then Exit For
        Next

        'tmpStr = System.Text.Encoding.Default.GetString(tmpBuf)
        RetVals(0) = System.Text.Encoding.Default.GetString(tmpBuf).Substring(0, i)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Set_EmailToAddress(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(60) As Byte
        Dim tmpBuf1() As Byte
        Dim tmpErr As MyErr_Enum
        Dim i As Integer

        tmpBuf1 = System.Text.Encoding.Default.GetBytes(CStr(ParamLst(0)))

        For i = 0 To 59
            tmpBuf(i) = 0
        Next

        For i = 0 To tmpBuf1.Length - 1
            tmpBuf(i) = tmpBuf1(i)
        Next

        tmpBuf(59) = 0

        tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 60)
        If tmpErr < 0 Then Return tmpErr

        Return MyErr_Enum.MyErr_Success
    End Function

    Private Function Ser_CMD_Cmd_UpdateTimeFromGSM(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(1) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 2)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = BitConverter.ToInt16(tmpBuf, 0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Get_SendEmailAttempts(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(1) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 2)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = BitConverter.ToInt16(tmpBuf, 0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Set_SendEmailAttempts(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(1) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 2)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = BitConverter.ToInt16(tmpBuf, 0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Cmd_GM862_ConfigModule(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Return MyErr_Enum.MyErr_Success
    End Function


    Private Function Ser_CMD_Get_RFParams(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(39) As Byte
        Dim tmpErr As MyErr_Enum

        tmpErr = SerComParent.Ser_TxSeg(SerComParent.Ser_Timeout, tmpBuf, 40)
        If tmpErr < 0 Then Return tmpErr

        RetVals(0) = tmpBuf

        Return MyErr_Enum.MyErr_Success

    End Function

    Private Function Ser_CMD_Set_RFParams(ByRef ParamLst() As Object, ByRef RetVals() As Object) As MyErr_Enum
        Dim tmpBuf(39) As Byte
        Dim tmpErr As MyErr_Enum

        tmpBuf = ParamLst(0)

        tmpErr = SerComParent.Ser_RxSeg(SerComParent.Ser_Timeout, tmpBuf, 40)
        If tmpErr < 0 Then Return tmpErr

        Return MyErr_Enum.MyErr_Success

    End Function

End Class

