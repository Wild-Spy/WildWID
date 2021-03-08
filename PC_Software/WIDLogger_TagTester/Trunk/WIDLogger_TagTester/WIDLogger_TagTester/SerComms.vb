Imports WIDLogger_TagTester.SerDev_LoggerV2_ComFuncs

Public Class SerComms

    Public Ser As New System.IO.Ports.SerialPort() '("COM28", 115200, IO.Ports.Parity.None, 8, IO.Ports.StopBits.One)

    Public CancelOperation As Boolean = False
    Public Ser_Timeout As Integer = 1000000
    Public Ser_Baud As Integer = 115200
    Public Ser_PortName As String = "COM28"

    Private _Port_InUse As Boolean
    Private RetVal As MyErr_Enum
    Private RxBuf(1023) As Byte
    Friend ParentFrm As Form
    Friend CommFuncs As New SerDev_LoggerV2_ComFuncs(Me) 'THIS IS THE ONLY LINE THAT NEEDS TO BE EDITED!

    Public Enum MyErr_Enum As Integer
        MyErr_Success = 0
        MyErr_Timeout = -1
        MyErr_PortClosed = -2
        MyErr_UserCancel = -3
        MyErr_DeviceAbort = -4
        MyErr_BadCmd = -5
        MyErr_Unknown = -6
        MyErr_UnknownCmd = -7
        MyErr_NoPortExists = -8
        MyErr_PortInUse = -9
    End Enum

    Private Enum SegID As Byte
        SegID_RX = 1    'PC Sends
        SegID_TX = 2    'PC Receives
        SegID_Fin = 3   'Finish Segment
    End Enum

    Private Enum SerResp As Byte
        SerResp_OK = 1
        SerResp_Resend = 2
        SerRest_Abort = 3
    End Enum

    Private Enum SerAckVal As Byte
        SerAckVal_ACK = 1
        SerAckVal_NAK = 2
    End Enum

    Public Sub New(ByRef Creator As Form)
        ParentFrm = Creator
        Ser.PortName = Ser_PortName
        Ser.BaudRate = Ser_Baud
        Ser.Close()

    End Sub

    Public Property Port_InUse As Boolean
        Get
            Return _Port_InUse
        End Get
        Set(ByVal value As Boolean)
            _Port_InUse = value
        End Set
    End Property

    Public Function FindDevices() As List(Of Ser_Device)
        Dim comPorts() As String = System.IO.Ports.SerialPort.GetPortNames()
        Dim PortStr, tmpName As String
        Dim tmpErr As MyErr_Enum
        Dim devID As UInt16
        Dim RetVals(0) As Object
        Dim DevList As New List(Of Ser_Device)

        If Me.Ser.IsOpen Then
            Me.Ser.Close()
        End If

        Me.Ser_Timeout = 200 '00000
        Me.Ser_Baud = SerDev_LoggerV2_ComFuncs.DevBaud


        For Each PortStr In comPorts
            Me.Ser_PortName = PortStr

            tmpErr = Me.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_DevTypeID, Nothing, RetVals)

            If tmpErr = MyErr_Enum.MyErr_Success Then
                devID = CType(RetVals(0), UInt16)
                If devID = SerDev_LoggerV2_ComFuncs.DevID Then
                    'Read Device Name
                    tmpErr = Me.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_LoggerName, Nothing, RetVals)
                    If tmpErr = MyErr_Enum.MyErr_Success Then
                        tmpName = CType(RetVals(0), String)
                        DevList.Add(New Ser_Device(PortStr, tmpName))
                    End If
                End If
            End If
        Next

        Return DevList

    End Function


    Public Function Ser_SendCmd(ByVal CMD_Byte As CMD_ByteID, ByRef ParamList() As Object, ByRef RetVals() As Object, Optional ByVal retries As Integer = 0) As MyErr_Enum
        Dim tryNo As Integer = 0
        Dim tmpErr As MyErr_Enum
        Do
            tmpErr = Ser_SendCmd_guts(CMD_Byte, ParamList, RetVals)
            Threading.Thread.Sleep(50)
            tryNo += 1
        Loop While tmpErr < 0 And tryNo < retries
        Return tmpErr

        'Return Ser_SendCmd_guts(CMD_Byte, ParamList, RetVals)

    End Function

    Public Function Ser_SendCmd_guts(ByVal CMD_Byte As CMD_ByteID, ByRef ParamList() As Object, ByRef RetVals() As Object) As MyErr_Enum
        _Port_InUse = True
        'Dim j As Integer
        Dim CMDByte(0) As Byte
        Dim tmpErr As MyErr_Enum
        Dim HandlerDelegate As Ser_CMD.Func_Type = Nothing
        Dim tmpTimeout As Integer = Ser_Timeout

        CMDByte(0) = CByte(CMD_Byte)

        'Lookup Command, make sure it exists
        tmpErr = CommFuncs.GetDelegate(CInt(CMDByte(0)), HandlerDelegate)
        If tmpErr < 0 Then GoTo WasErr

        'Setup serial port
        Try
            Ser.ReadTimeout = 1000
            Ser.WriteTimeout = 1000
            Ser.PortName = Ser_PortName
            Ser.BaudRate = Ser_Baud
            Ser.Open()
        Catch ex As Exception
            If ex.Message = "'PortName' cannot be set while the port is open." Then
                MsgBox("Port already open")
            ElseIf ex.Message Like "The port '*' does not exist." Then
                _Port_InUse = False
                Return MyErr_Enum.MyErr_NoPortExists
            End If

        End Try

        Try
            'Flush serial Rx buffer
            If Ser.BytesToRead > 0 Then
                Ser.ReadExisting()
            End If
            Ser.Write(vbNullChar)
        Catch ex As System.InvalidOperationException
            If ex.Message = "The port is closed." Then
                Ser.Close()
                _Port_InUse = False
                Return MyErr_Enum.MyErr_PortClosed
            End If
        Catch ex As Exception
            _Port_InUse = False
            Return MyErr_Enum.MyErr_Unknown
        End Try

        'Tx CMD Byte
        tmpErr = Ser_RxSeg(Ser_Timeout, CMDByte, 1)
        If tmpErr < 0 Then GoTo WasErr

        Ser_Timeout = 1000
        'Call handling function (already know CMD index)
        tmpErr = HandlerDelegate(ParamList, RetVals)
        If tmpErr < 0 Then GoTo WasErr

        'Finnish transfer
        tmpErr = Ser_FinishSeg(Ser_Timeout)
        If tmpErr < 0 Then GoTo WasErr

WasErr:
        Ser_Timeout = tmpTimeout
        Ser.Close()
        _Port_InUse = False
        Return tmpErr
    End Function

    'No serial setup
    Public Function Ser_SendCmd_NS(ByVal CMD_Byte As CMD_ByteID, ByRef ParamList() As Object, ByRef RetVals() As Object) As MyErr_Enum
        _Port_InUse = True
        'Dim j As Integer
        Dim CMDByte(0) As Byte
        Dim tmpErr As MyErr_Enum
        Dim HandlerDelegate As Ser_CMD.Func_Type = Nothing
        Dim tmpTimeout As Integer = Ser_Timeout

        CMDByte(0) = CByte(CMD_Byte)

        'Lookup Command, make sure it exists
        tmpErr = CommFuncs.GetDelegate(CInt(CMDByte(0)), HandlerDelegate)
        If tmpErr < 0 Then GoTo WasErr

        Try
            'Flush serial Rx buffer
            If Ser.BytesToRead > 0 Then
                Ser.ReadExisting()
            End If
            Ser.Write(vbNullChar)
        Catch ex As System.InvalidOperationException
            If ex.Message = "The port is closed." Then
                'Ser.Close()
                _Port_InUse = False
                Return MyErr_Enum.MyErr_PortClosed
            End If
        Catch ex As Exception
            _Port_InUse = False
            Return MyErr_Enum.MyErr_Unknown
        End Try

        'Tx CMD Byte
        tmpErr = Ser_RxSeg(Ser_Timeout, CMDByte, 1)
        If tmpErr < 0 Then GoTo WasErr

        Ser_Timeout = 1000
        'Call handling function (already know CMD index)
        tmpErr = HandlerDelegate(ParamList, RetVals)
        If tmpErr < 0 Then GoTo WasErr

        'Finish transfer
        tmpErr = Ser_FinishSeg(Ser_Timeout)
        If tmpErr < 0 Then GoTo WasErr

WasErr:
        Ser_Timeout = tmpTimeout
        _Port_InUse = False
        Return tmpErr
    End Function

    Public Function Ser_RxSeg(ByVal Timeout_ms As Integer, ByRef TxData As Byte(), ByVal DataLen As UInt16) As MyErr_Enum
        If Ser.IsOpen = True Then
            Dim tmpByte(1) As Byte
            Dim RxDataLen, ChkSum, i As UInt16
            Dim tmpErr As MyErr_Enum

            '(1) Receive Segment Identifier
            tmpErr = Ser_Rx_Byte_BG(Timeout_ms, tmpByte(0))
            If (tmpErr < MyErr_Enum.MyErr_Success) Then
                Return tmpErr
            End If

            '(2) Send Response
            If (tmpByte(0) = SegID.SegID_Fin) Then
                Ser_WriteByte(SerAckVal.SerAckVal_ACK)
                Return MyErr_Enum.MyErr_Unknown
            ElseIf (tmpByte(0) <> SegID.SegID_RX) Then
                Ser_WriteByte(SerAckVal.SerAckVal_NAK)
                Return MyErr_Enum.MyErr_BadCmd
            End If
            Ser_WriteByte(SerAckVal.SerAckVal_ACK)

            '(3) Receive Data Length
            tmpErr = Ser_Rx_UInt16_BG(Timeout_ms, RxDataLen)
            If (tmpErr < MyErr_Enum.MyErr_Success) Then
                Return tmpErr
            End If

            '(4) Send Response
            If (RxDataLen <> DataLen) Then
                Ser_WriteByte(SerAckVal.SerAckVal_NAK)
                Return MyErr_Enum.MyErr_BadCmd
            End If
            Ser_WriteByte(SerAckVal.SerAckVal_ACK)

            Do
                'delay 1 ms ??
                DoEventDelay(1)

                '(5) Send Data
                ChkSum = 0
                For i = 0 To DataLen - 1
                    'Send one at a time and calculate checksum to make sure device can keep up
                    Ser.Write(TxData, i, 1)

                    'ChkSum = ChkSum + TxData(i)
                    ChkSum = CRC_Lib.CRC.update_crc_16(ChkSum, CChar(ChrW(TxData(i))))

                Next

                '(6) Send Checksum
                Ser.Write(BitConverter.GetBytes(ChkSum), 0, 2)

                '(7) Receive Ser Response
                'Dim tmpTime As DateTime = Now()
                tmpErr = Ser_Rx_Byte_BG(Timeout_ms, tmpByte(0))
                'Debug.Print((Now() - tmpTime).TotalMilliseconds)
                If (tmpErr < MyErr_Enum.MyErr_Success) Then
                    Return tmpErr
                End If

                '(9) Send Response
                If (tmpByte(0) = SerResp.SerResp_OK) Then
                    Ser_WriteByte(SerAckVal.SerAckVal_ACK)
                ElseIf (tmpByte(0) = SerResp.SerResp_Resend) Then
                    Ser_WriteByte(SerAckVal.SerAckVal_ACK)
                ElseIf (tmpByte(0) = SerResp.SerRest_Abort) Then
                    Ser_WriteByte(SerAckVal.SerAckVal_ACK)
                    Return MyErr_Enum.MyErr_DeviceAbort
                Else
                    Ser_WriteByte(SerAckVal.SerAckVal_NAK)
                    Return MyErr_Enum.MyErr_BadCmd
                End If
            Loop While tmpByte(0) = SerResp.SerResp_Resend

            'Finished successfully!
            Return MyErr_Enum.MyErr_Success

        Else
            RetVal = MyErr_Enum.MyErr_PortClosed
            Return RetVal   'port not open
        End If
    End Function

    'Receive from Device
    Public Function Ser_TxSeg(ByVal Timeout_ms As Integer, ByRef RxData As Byte(), ByVal DataLen As UInt16) As MyErr_Enum
        If Ser.IsOpen = True Then
            Dim tmpBuf(DataLen) As Byte
            Dim tmpByte(1) As Byte
            Dim RxDataLen, ChkSum, i As UInt16
            Dim tmpErr As MyErr_Enum

            '(1) Receive Segment Identifier
            tmpErr = Ser_Rx_Byte_BG(Timeout_ms, tmpByte(0))
            If (tmpErr < MyErr_Enum.MyErr_Success) Then
                Return tmpErr
            End If

            '(2) Send Response
            If (tmpByte(0) = SegID.SegID_Fin) Then
                Ser_WriteByte(SerAckVal.SerAckVal_ACK)
                Return MyErr_Enum.MyErr_Unknown
            ElseIf (tmpByte(0) <> SegID.SegID_TX) Then
                Ser_WriteByte(SerAckVal.SerAckVal_NAK)
                Return MyErr_Enum.MyErr_BadCmd
            End If
            Ser_WriteByte(SerAckVal.SerAckVal_ACK)

            '(3) Receive Segment Identifier
            tmpErr = Ser_Rx_UInt16_BG(Timeout_ms, RxDataLen)
            If (tmpErr < MyErr_Enum.MyErr_Success) Then
                Return tmpErr
            End If

            '(4) Send Response
            If (RxDataLen <> DataLen) Then
                Ser_WriteByte(SerAckVal.SerAckVal_NAK)
                Return MyErr_Enum.MyErr_BadCmd
            End If
            Ser_WriteByte(SerAckVal.SerAckVal_ACK)

            Do

                '(5) Receive Data
                ChkSum = 0
                For i = 0 To DataLen - 1

                    'Put received data into temp buffer
                    tmpErr = Ser_Rx_Byte_BG(Timeout_ms, tmpBuf(i))
                    If (tmpErr < MyErr_Enum.MyErr_Success) Then
                        Return tmpErr
                    End If

                    'ChkSum += tmpBuf(i)
                    ChkSum = CRC_Lib.CRC.update_crc_16(ChkSum, CChar(ChrW(tmpBuf(i))))

                Next

                'If there are more bytes to read still?!?!, abort!
                If Ser.BytesToRead > 0 Then
                    Ser.ReadExisting()
                    Return MyErr_Enum.MyErr_BadCmd
                End If

                '(6) Send Checksum
                Ser.Write(BitConverter.GetBytes(ChkSum), 0, 2)

                '(7) Receive Ser Response
                tmpErr = Ser_Rx_Byte_BG(Timeout_ms, tmpByte(0))
                If (tmpErr < MyErr_Enum.MyErr_Success) Then
                    Return tmpErr
                End If

                '(9) Send Response
                If (tmpByte(0) = SerResp.SerResp_OK) Then
                    Ser_WriteByte(SerAckVal.SerAckVal_ACK)
                ElseIf (tmpByte(0) = SerResp.SerResp_Resend) Then
                    Ser_WriteByte(SerAckVal.SerAckVal_ACK)
                ElseIf (tmpByte(0) = SerResp.SerRest_Abort) Then
                    Ser_WriteByte(SerAckVal.SerAckVal_ACK)
                    Return MyErr_Enum.MyErr_DeviceAbort
                Else
                    Ser_WriteByte(SerAckVal.SerAckVal_NAK)
                    Return MyErr_Enum.MyErr_BadCmd
                End If
            Loop While tmpByte(0) = SerResp.SerResp_Resend

            'copy tmp buffer to real location
            For i = 0 To DataLen - 1
                RxData(i) = tmpBuf(i)
            Next

            'Finished successfully!
            Return MyErr_Enum.MyErr_Success

        Else
            RetVal = MyErr_Enum.MyErr_PortClosed
            Return RetVal   'port not open
        End If
    End Function

    'Finish device segment
    Public Function Ser_FinishSeg(ByVal Timeout_ms As Integer) As MyErr_Enum
        If Ser.IsOpen = True Then
            Dim tmpByte(1) As Byte
            Dim tmpErr As MyErr_Enum

            '(1) Receive Segment Identifier
            tmpErr = Ser_Rx_Byte_BG(Timeout_ms, tmpByte(0))
            If (tmpErr < MyErr_Enum.MyErr_Success) Then
                Return tmpErr
            End If

            '(2) Send Response
            If (tmpByte(0) <> SegID.SegID_Fin) Then
                Ser_WriteByte(SerAckVal.SerAckVal_NAK)
                Return MyErr_Enum.MyErr_BadCmd
            End If
            Ser_WriteByte(SerAckVal.SerAckVal_ACK)
            Return MyErr_Enum.MyErr_Success
        End If
        Return MyErr_Enum.MyErr_PortClosed
    End Function


    Public Sub DoEventDelay(ByVal msDelay As Integer)
        Dim t1 As DateTime

        t1 = Now()
        t1 = t1.AddMilliseconds(msDelay)
        Do
            Application.DoEvents()
        Loop While Now() < t1

    End Sub

    Public Function Ser_Rx_UInt16_BG(ByVal Timeout_ms As Integer, ByRef RxInt As UInt16) As MyErr_Enum
        Dim tmpByte(1) As Byte
        Dim tmpErr As MyErr_Enum
        Dim i As Integer

        For i = 0 To 1
            tmpErr = Ser_Rx_Byte_BG(Timeout_ms, tmpByte(i))
            If (tmpErr < MyErr_Enum.MyErr_Success) Then
                Return tmpErr
            End If
        Next

        RxInt = BitConverter.ToUInt16(tmpByte, 0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Public Function Ser_Rx_Int16_BG(ByVal Timeout_ms As Integer, ByVal RxInt As Int16) As MyErr_Enum
        Dim tmpByte(1) As Byte
        Dim tmpErr As MyErr_Enum
        Dim i As Integer

        For i = 0 To 1
            tmpErr = Ser_Rx_Byte_BG(Timeout_ms, tmpByte(i))
            If (tmpErr < MyErr_Enum.MyErr_Success) Then
                Return tmpErr
            End If
        Next

        RxInt = BitConverter.ToInt16(tmpByte, 0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Public Function Ser_Rx_UInt32_BG(ByVal Timeout_ms As Integer, ByRef RxInt As UInt32) As MyErr_Enum
        Dim tmpByte(3) As Byte
        Dim tmpErr As MyErr_Enum
        Dim i As Integer

        For i = 0 To 3
            tmpErr = Ser_Rx_Byte_BG(Timeout_ms, tmpByte(i))
            If (tmpErr < MyErr_Enum.MyErr_Success) Then
                Return tmpErr
            End If
        Next

        RxInt = BitConverter.ToUInt32(tmpByte, 0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Public Function Ser_Rx_Int32_BG(ByVal Timeout_ms As Integer, ByRef RxInt As Int32) As MyErr_Enum
        Dim tmpByte(3) As Byte
        Dim tmpErr As MyErr_Enum
        Dim i As Integer

        For i = 0 To 3
            tmpErr = Ser_Rx_Byte_BG(Timeout_ms, tmpByte(i))
            If (tmpErr < MyErr_Enum.MyErr_Success) Then
                Return tmpErr
            End If
        Next

        RxInt = BitConverter.ToInt32(tmpByte, 0)

        Return MyErr_Enum.MyErr_Success

    End Function

    Public Function Ser_Rx_ByteAry_BG(ByVal Timeout_ms As Integer, ByRef RxAry As Byte(), ByVal len As Integer) As MyErr_Enum
        Dim tmpErr As MyErr_Enum
        Dim i As Integer

        For i = 0 To len - 1
            tmpErr = Ser_Rx_Byte_BG(Timeout_ms, RxAry(i))
            If (tmpErr < MyErr_Enum.MyErr_Success) Then
                Return tmpErr
            End If
        Next

        Return MyErr_Enum.MyErr_Success

    End Function

    Public Function Ser_Rx_Byte_BG(ByVal Timeout_ms As Integer, ByRef RxByte As Byte) As MyErr_Enum
        If Ser.IsOpen = True Then
            Dim t1 As DateTime = Now()
            t1 = t1.AddMilliseconds(Timeout_ms)
            Try
                Do
                    If Ser.BytesToRead > 0 Then
                        RxByte = CByte(Ser.ReadByte())
                        RetVal = MyErr_Enum.MyErr_Success
                        Return RetVal
                    End If
                    Application.DoEvents()
                Loop While Now < t1 And CancelOperation = False

                If CancelOperation = True Then
                    Return MyErr_Enum.MyErr_UserCancel  'canceled
                End If
            Catch ex As System.InvalidOperationException
                If ex.Message = "The port is closed." Then
                    RetVal = MyErr_Enum.MyErr_PortClosed
                    Return RetVal 'port not open
                Else
                    RetVal = MyErr_Enum.MyErr_Unknown
                    Return RetVal
                End If
            End Try

            RetVal = MyErr_Enum.MyErr_Timeout
            Return RetVal   'timeout
        Else
            RetVal = MyErr_Enum.MyErr_PortClosed
            Return RetVal   'port not open
        End If
    End Function

    'Public Function Ser_Rx_Byte_BG(ByVal Timeout_ms As Integer, ByRef RxByte As Byte) As MyErr_Enum
    '    Dim bgThread As Threading.Thread

    '    bgThread = New Threading.Thread(AddressOf Ser_Rx_Byte)
    '    bgThread.IsBackground = True
    '    bgThread.Start(Timeout_ms)

    '    Do
    '        Application.DoEvents()
    '    Loop While bgThread.IsAlive() = True And CancelOperation = False

    '    If CancelOperation = True Then
    '        Return MyErr_Enum.MyErr_UserCancel  'canceled
    '    End If

    '    'The Funcions return value is stored in RetVal
    '    If RetVal = MyErr_Enum.MyErr_Success Then
    '        'The received byte is stored in RxBuf(0)
    '        RxByte = RxBuf(0)
    '    End If

    '    Return RetVal
    'End Function

    'blocking.. run in another thread!
    Public Function Ser_Rx_Byte(ByVal Timeout_ms As Integer) As MyErr_Enum
        If Ser.IsOpen = True Then
            Dim t1 As DateTime = Now
            t1 = t1.AddMilliseconds(Timeout_ms)

            'Use RetVal to pass the return value back out
            Try
                Do
                    If Ser.BytesToRead > 0 Then
                        RxBuf(0) = CByte(Ser.ReadByte())
                        RetVal = MyErr_Enum.MyErr_Success
                        Return RetVal
                    End If
                Loop While Now < t1

            Catch ex As System.InvalidOperationException
                If ex.Message = "The port is closed." Then
                    RetVal = MyErr_Enum.MyErr_PortClosed
                    Return RetVal 'port not open
                Else
                    RetVal = MyErr_Enum.MyErr_Unknown
                    Return RetVal
                End If
            End Try


            RetVal = MyErr_Enum.MyErr_Timeout
            Return RetVal   'timeout

        Else
            RetVal = MyErr_Enum.MyErr_PortClosed
            Return RetVal   'port not open
        End If
    End Function

    Private Sub Ser_WriteByte(ByVal data As Byte)
        Dim tmpByte(0) As Byte
        tmpByte(0) = data

        Ser.Write(tmpByte, 0, 1)

    End Sub

End Class

Public Class Ser_CMD
    Public CMDByte As CMD_ByteID
    'Public Func As Func(Of SerComms.MyErr_Enum)
    'Public Func As Func(Of Object(), Object(), SerComms.MyErr_Enum)
    Public Delegate Function Func_Type(ByRef ParamLst() As Object, ByRef RetVals() As Object) As SerComms.MyErr_Enum
    Public Func As Func_Type

    Public Sub New()
        CMDByte = Nothing
        Func = Nothing
    End Sub

    Public Sub New(ByVal CMD_Byte As CMD_ByteID, ByVal mFunc As Func_Type) 'ByVal mFunc As Func(Of Object(), Object(), SerComms.MyErr_Enum))
        CMDByte = CMD_Byte
        Func = mFunc
    End Sub

End Class

Public Class SerDev_ComFuncs

    Public CmdAry As New List(Of Ser_CMD)
    Friend SerComParent As SerComms

    Public Sub New()
        SerComParent = Nothing
    End Sub

    Public Sub New(ByRef Parent As SerComms)
        SerComParent = Parent
    End Sub

    Public Function GetDelegate(ByVal CMDByte As Integer, ByRef RetFunc As Ser_CMD.Func_Type) As SerComms.MyErr_Enum
        Dim j As Integer

        For j = 0 To CmdAry.Count - 1
            If CMDByte = CmdAry(j).CMDByte Then
                RetFunc = CmdAry(j).Func
                Return SerComms.MyErr_Enum.MyErr_Success
            End If
        Next
        Return SerComms.MyErr_Enum.MyErr_UnknownCmd
    End Function

End Class

Public Class Ser_Device
    Public ScanTime As DateTime
    Public DevPort As String
    Public DevName As String

    Public Sub New()

    End Sub

    Public Sub New(port As String, name As String)
        DevPort = port
        DevName = name
        ScanTime = Now
    End Sub
End Class