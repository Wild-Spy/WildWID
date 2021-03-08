Imports WIDLogger_TagTester.SerComms
Imports System.Threading

<Serializable()> Public Structure SavedWid
    Public WID_ID As String
    'Public WID_TX_Rate As Integer   'Measured in seconds, ie tx one blip every XXX seconds.
    Public WID_Info As String       'Info about the WID.. what 
    Public WID_Battery As String    'What power source is the WID hooked up to?
    'Public WID_Channel As Integer   'The channel number (usually 2)
    'Public WID_RFID() As Byte      '5 bytes, usually E6-E6-E6-E6-E6
End Structure

<Serializable()> Public Structure SavedLogger
    Public Log_Port As String
    Public Log_Name As String
    Public Data(,) As Single
End Structure

<Serializable()> Public Structure SavedFileData
    Public Channel As Integer
    Public RFID() As Byte
    Public TX_RATE As Integer 'in seconds
    Public Log_RFFilter As String
    Public WIDS() As SavedWid
    Public Loggers() As SavedLogger
End Structure


Public Class DevPickupList
    Const NO_PICKUP As Single = -1.0
    Const TEST_TIME_BREAK As Single = -2.0
    Public SegmentsDone As Long
    Public Data(,) As Single   'WID, time
    Dim ShortPickups() As Single
    Dim SegmentStart As DateTime
    Public WIDS As New List(Of WID_Info)
    Dim Parent As Form
    Dim SerComLib As SerComms
    Dim Running As Boolean
    Dim StopThread As Boolean = False
    Dim DevThread As Thread
    Public TestStartTime As DateTime
    Private tagPickups() As Long
    Private DataSemaphore As Boolean = False

    'Logger related stuff
    Public Log_Channel As Integer
    Public Log_RFID() As Byte = {&HE6, &HE6, &HE6, &HE6, &HE6}
    Public Log_Port As String
    Public Log_Name As String
    Public Logger_TX_RATE As Integer 'in seconds
    Public Log_RFFilter As String

    Public Enum Log_VerboseMode As Byte
        Silent = 1
        TagOnly = 2
        TagTime = 3
        TagTimeAddress = 4
    End Enum

    '*****************************************************************************************************
    '***************************************** NOT STARTED ***********************************************
    '*****************************************************************************************************


    '*****************************************************************************************************
    '***************************************** IN PROGRESS ***********************************************
    '*****************************************************************************************************

    Public Function GetTestRunningTime() As TimeSpan
        Return Now - TestStartTime
    End Function

    Public Sub SaveToFile(ByVal FileName As String)

        Dim fs As IO.FileStream
        Dim sw As IO.StreamWriter
        Dim bf As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter()

        fs = New IO.FileStream(FileName, IO.FileMode.OpenOrCreate)
        sw = New IO.StreamWriter(fs)


        Dim a As New SavedFileData
        a.Channel = 2
        a.Log_RFFilter = "********"
        ReDim a.WIDS(0)
        a.WIDS(0).WID_ID = "ABCD0001"
        a.WIDS(0).WID_Battery = "battery 1"
        a.WIDS(0).WID_Info = "WID 1 Info"

        bf.Serialize(fs, a)
        ''save misc. bits in header
        ''segments done
        ''SegmentStart
        'sw.WriteLine(Log_Channel)
        'sw.WriteLine(Log_RFID(0) & "-" & Log_RFID(1) & "-" & Log_RFID(2) & "-" & Log_RFID(3) & "-" & Log_RFID(4))
        'sw.WriteLine(Log_Port)
        'sw.WriteLine(Log_Name)
        'sw.WriteLine(Logger_TX_RATE)
        'sw.WriteLine(Log_RFFilter)
        'sw.WriteLine(TestStartTime)
        'sw.WriteLine(SegmentsDone)
        'sw.WriteLine(WIDS.Count())
        'For j = 0 To WIDS.Count() - 1
        '    sw.WriteLine(WIDS(j).Export)
        'Next

        'sw.WriteLine("Data Start")
        'sw.Flush()
        'fs.Seek(sw.BaseStream.Position, IO.SeekOrigin.Begin)

        'For i = 0 To SegmentsDone - 1
        '    For j = 0 To WIDS.Count - 1
        '        fs.Write(BitConverter.GetBytes(Data(j, i)), 0, 4)
        '    Next
        'Next

        'sw.WriteLine("Data End")

        sw.Close()
        fs.Close()
        'save data bits
    End Sub

    'Private Function ReadLineAndAddLengthToCounter(ByRef sr As IO.StreamReader, ByRef counter As Integer) As String
    '    Dim s As String
    '    s = sr.ReadLine()
    '    counter += s.Length + sr.BaseStream.fl

    'End Function

    Public Sub LoadFromFile(ByVal FileName As String)
        Dim j As Integer
        Dim i As Long
        Dim tmpStr As String
        Dim RFID() As String
        Dim b(3) As Byte
        Dim WidsCount As Integer
        Dim cCount As Integer = 0
        'Dim s As String

        'open file
        Dim fs As IO.FileStream
        Dim sr As TGREER.myStreamReader

        fs = New IO.FileStream(FileName, IO.FileMode.OpenOrCreate)
        sr = New TGREER.myStreamReader(fs)

        'save misc. bits in header
        Log_Channel = sr.ReadLine()

        tmpStr = sr.ReadLine()
        cCount += tmpStr.Length + 2
        RFID = tmpStr.Split("-")
        For i = 0 To 4
            Log_RFID(i) = RFID(i)
        Next

        Log_Port = sr.ReadLine()
        Log_Name = sr.ReadLine()
        Logger_TX_RATE = sr.ReadLine()
        Log_RFFilter = sr.ReadLine()
        TestStartTime = sr.ReadLine()
        SegmentsDone = sr.ReadLine()
        WidsCount = sr.ReadLine()

        For j = 0 To WidsCount - 1
            tmpStr = ""
            Do
                i = sr.Read()
                tmpStr &= ChrW(i)
            Loop While CStr(ChrW(i)) <> "¶"
            AddWID(New WID_Info(tmpStr))
        Next

        sr.ReadLine() 'end line after last wid

        tmpStr = sr.ReadLine
        If tmpStr <> "Data Start" Then
            'we have a problem
            Stop
        End If

        fs.Flush()
        'fs.Position

        'fs.Position = 
        fs.Position = fs.Length - (Len("Data End" & vbCrLf)) - (SegmentsDone * 4 * WidsCount)
        fs.Seek(sr.BytesRead, IO.SeekOrigin.Begin)

        ReDim Data(WidsCount, SegmentsDone - (SegmentsDone Mod 1000) + 999)
        For i = 0 To SegmentsDone - 1
            For j = 0 To WIDS.Count - 1
                'fs.Write(BitConverter.GetBytes(Data(j, i)), 0, 4)
                fs.Read(b, 0, 4)
                Data(j, i) = BitConverter.ToSingle(b, 0)
            Next
        Next

        'sr.BaseStream.Position = fs.Position

        'tmpStr = sr.ReadToEnd
        'If tmpStr <> "Data Stop" Then
        '    'we have a problem
        '    Stop
        'End If

        sr.Close()
        fs.Close()
        'save data bits
    End Sub

    Public Function GetWIDCounts() As Long()
        Dim i As Long
        Dim j As Integer

        If IsNothing(tagPickups) Then
            ReDim tagPickups(WIDS.Count - 1)
        Else
            ReDim tagPickups(WIDS.Count - 1)

            For j = 0 To WIDS.Count - 1
                tagPickups(j) = 0
            Next

            For i = 1 To SegmentsDone - 1
                For j = 0 To WIDS.Count - 1
                    If Data(j, i) <> NO_PICKUP Then
                        tagPickups(j) += 1
                    End If
                Next
            Next

        End If

        Return tagPickups

    End Function

    Public Function GetWIDAvgs() As Single()
        Dim j As Integer
        Dim avg(WIDS.Count - 1) As Single

        GetWIDCounts()

        For j = 0 To WIDS.Count - 1
            avg(j) = tagPickups(j) / (SegmentsDone - 1)
        Next

        Return avg

    End Function

    Public Enum GraphType
        Graph_Cumulative
        Graph_MovingAvg
        Graph_Instantaneous
        Graph_TXTimeOffset
    End Enum

    Public Function GetCumulativeGraphData(ByVal gType As GraphType, ByVal points As Integer, ByVal TagID As Integer, Optional ByVal pointsToAvg As Integer = 100, Optional ByRef startTime As DateTime = Nothing, Optional ByRef stopTime As DateTime = Nothing) As PointF()
        Dim retData() As PointF
        Dim i As Long
        Dim segsAvailable As Long
        Dim segsPerPoint As Integer
        Dim startIndex As Long = 0
        Dim stopIndex As Long = 0
        Dim LastIndex As Long   'also do this point so that the graph size is constant!
        'Dim pointsToAvg As Long = 100 '5 * 2 'MUST BE EVEN!!!

        If pointsToAvg Mod 2 <> 0 Then
            pointsToAvg -= 1
        End If

        'Assume GraphType is 'cumulative'...
        'Want to take every nth point where n = PointsAvailable/PointsToPrint

        If points < 2 Then
            Return Nothing
        End If

        'shouldn't use 0'th segment!
        segsAvailable = SegmentsDone - 1

        If segsAvailable > 0 And DataSemaphore = False Then
            'calculate first segment
            If startTime.Year = 1 Then
                startIndex = 1 + pointsToAvg / 2
            Else
                startIndex = Math.Floor((CType(startTime - TestStartTime, TimeSpan).TotalSeconds) / Logger_TX_RATE) + pointsToAvg / 2
            End If
            'calculate last segment
            If stopTime.Year = 1 Then
                stopIndex = segsAvailable - (pointsToAvg / 2 - 1)
            Else
                stopIndex = Math.Floor((CType(stopTime - TestStartTime, TimeSpan).TotalSeconds) / Logger_TX_RATE) - (pointsToAvg / 2 - 1)
            End If

            If startIndex >= stopIndex Then
                Return Nothing
            End If
            'calculate number of segments
            segsAvailable = stopIndex - startIndex
            'calculate points per segment or segments per point
            segsPerPoint = Math.Floor(segsAvailable / points)
            'Assign the 'set' last index
            LastIndex = stopIndex

            stopIndex = startIndex + segsPerPoint * points

            If segsPerPoint < 1 Then
                Return Nothing
            End If

            'reserve memory for points
            ReDim retData(points)
            'assign data to points

            Try
                Dim pointIndex As Long
                For i = 0 To points - 1
                    pointIndex = startIndex + i * segsPerPoint
                    retData(i) = New PointF(pointIndex, GetMovingAvgAtPoint(pointIndex, pointsToAvg, TagID))
                Next
            Catch ex As Exception
                'Stop
            End Try

            Try
                retData(points) = New PointF(LastIndex, GetMovingAvgAtPoint(LastIndex, pointsToAvg, TagID))
            Catch ex As Exception

            End Try

            Return retData

        End If

        Return Nothing
    End Function

    Private Function GetMovingAvgAtPoint(ByVal start As Integer, ByVal samples As Integer, ByVal tagID As Integer)
        Dim Sum As Decimal = 0
        Dim i As Integer

        For i = -(samples / 2) To (samples / 2) - 1
            If Not (Data(tagID, start + i) = NO_PICKUP) Then
                Sum += 1
            End If
        Next

        'If Sum > 0 Then
        '    Stop
        'End If

        Return Sum / samples

    End Function

    Public Function GetInstGraphData(ByVal gType As GraphType, ByVal points As Integer, ByVal TagID As Integer, Optional ByVal startIndex As Integer = -1) As List(Of PointF)
        Dim retData As New List(Of PointF)
        Dim i As Long
        Dim segsAvailable As Long
        Dim SegStep As Integer
        'Dim StartIndex As Long
        Dim EndIndex As Long

        'Assume GraphType is 'cumulative'...
        'Want to take every nth point where n = PointsAvailable/PointsToPrint

        'shouldn't use 0'th segment!
        segsAvailable = SegmentsDone - 1

        If segsAvailable > 0 Then

            If startIndex = -1 Then
                If points > segsAvailable Then
                    startIndex = 1
                    EndIndex = segsAvailable
                    points = segsAvailable
                    SegStep = 1
                Else
                    'SegStep = segsAvailable / points
                    SegStep = 1
                    startIndex = segsAvailable - points
                    EndIndex = segsAvailable - 1
                End If
            Else
                If startIndex + points > segsAvailable Then
                    EndIndex = segsAvailable
                Else
                    'SegStep = segsAvailable / points
                    SegStep = 1
                    'startIndex = segsAvailable - points
                    EndIndex = startIndex + points
                End If
            End If



            'ReDim retData(0)

            retData.Add(New PointF(Logger_TX_RATE * (startIndex) * SegStep, 0))

            For i = startIndex To EndIndex - 1
                If Data(TagID, i * SegStep) = NO_PICKUP Then
                    'ReDim Preserve retData(retData.Length + 1)
                    'ReDim Preserve retData(retData.Length + 1)
                    'retData(retData.Length) = New PointF(Logger_TX_RATE * (i) * SegStep, 0)
                    retData.Add(New PointF(Logger_TX_RATE * (i) * SegStep, 0))
                Else
                    'ReDim Preserve retData(retData.Length + 4)
                    'ReDim Preserve retData(retData.Length + 4)
                    'retData(retData.Length - 4) = New PointF(Logger_TX_RATE * (i) * SegStep + (Data(TagID, (i)) / 1000) - 0.2, 0)
                    'retData(retData.Length - 3) = New PointF(Logger_TX_RATE * (i) * SegStep + (Data(TagID, (i)) / 1000) - 0.2, 1)
                    'retData(retData.Length - 2) = New PointF(Logger_TX_RATE * (i) * SegStep + (Data(TagID, (i)) / 1000) + 0.0, 1)
                    'retData(retData.Length - 1) = New PointF(Logger_TX_RATE * (i) * SegStep + (Data(TagID, (i)) / 1000) + 0.0, 0)
                    retData.Add(New PointF(Logger_TX_RATE * (i) * SegStep + (Data(TagID, (i)) / 1000) - 0.2, 0))
                    retData.Add(New PointF(Logger_TX_RATE * (i) * SegStep + (Data(TagID, (i)) / 1000) - 0.2, 1))
                    retData.Add(New PointF(Logger_TX_RATE * (i) * SegStep + (Data(TagID, (i)) / 1000) + 0.0, 1))
                    retData.Add(New PointF(Logger_TX_RATE * (i) * SegStep + (Data(TagID, (i)) / 1000) + 0.0, 0))
                End If

            Next

            retData.Add(New PointF(Logger_TX_RATE * (EndIndex) * SegStep, 0))

            Return retData
        End If

        Return Nothing
    End Function

    Public Sub SetLogValues(ByVal ch As Integer, ByVal tx_Rate As Integer, ByVal RFFilter As String, ByVal Port As String, ByVal Name As String, Optional ByVal rfid() As Byte = Nothing)

        Log_Channel = ch
        Logger_TX_RATE = tx_Rate
        Log_RFFilter = RFFilter
        Log_Port = Port
        Log_Name = Name

        If IsNothing(rfid) Then
            Log_RFID = {&HE6, &HE6, &HE6, &HE6, &HE6}
        Else
            Dim i As Integer
            Try
                For i = 0 To 4
                    Log_RFID(i) = rfid(i)
                Next
            Catch ex As Exception
                Throw New Exception("Invalid RFID passed to logger")
            End Try
        End If

    End Sub

    Public Sub StartTest()
        Dim t As DateTime = Now

        'round to nearest 10 seconds
        t = t.AddMilliseconds(-t.Millisecond)
        t = t.AddSeconds(-(t.Second Mod 10))

        If Log_Set_VerboseMode(Log_VerboseMode.TagOnly) <> MyErr_Enum.MyErr_Success Then Throw New Exception("Could Not Set Logger To Tag Only Verbose Mode " & Log_Name)

        'Align segment start
        SegmentStart = t
        TestStartTime = t.AddSeconds(Logger_TX_RATE)

        'start new thread
        StopThread = False
        DevThread = New Threading.Thread(AddressOf ThreadCore)
        DevThread.IsBackground = True
        DevThread.Start()

        '... more
    End Sub

    Private Sub ThreadCore()
        SerComLib.Ser.ReadTimeout = 10

        Running = True
        SerComLib.Ser.Open()

        Do
            Ser_ReadLine()
            If Now > SegmentStart.AddSeconds(Logger_TX_RATE) Then
                FinishedSegment()
            End If
            'Thread.Sleep(10) '??
        Loop While StopThread = False

        SerComLib.Ser.Close()
        SerComLib.Ser_Timeout = 500
        Running = False

        If Log_Set_VerboseMode(Log_VerboseMode.Silent) <> MyErr_Enum.MyErr_Success Then Throw New Exception("Could Not Set Logger To Silent Mode " & Log_Name)
    End Sub

    Public Sub SetupForTest()


        'locks in data
        ReDim ShortPickups(WIDS.Count - 1)
        'ReDim Data(999, WIDS.Count - 1)
        ReDim Data(WIDS.Count - 1, 10)
        'setup sercomlib?
        SerComLib.Ser_Baud = 115200
        SerComLib.Ser_PortName = Log_Port
        SerComLib.Ser_Timeout = 500
        'setup the logger to correct mode?
        CheckLogger()
        SetupLogger()

    End Sub

    Public Sub New(ByVal parentForm As Form)
        Parent = parentForm
        SerComLib = New SerComms(Parent)
    End Sub

    '*****************************************************************************************************
    '*************************************** JUST ABOUT DONE *********************************************
    '*****************************************************************************************************

    Public Sub StopTest()
        StopThread = True
        Do
            Application.DoEvents()
        Loop While Running = True
    End Sub

    Private Sub SetupLogger()
        'write some info out to console??
        'say which logger in the exceptions?
        If Log_Set_VerboseMode(Log_VerboseMode.Silent) <> MyErr_Enum.MyErr_Success Then Throw New Exception("Could Not Set Logger To Silent Mode " & Log_Name)
        If Log_Set_EmailTime(Now.AddYears(20)) <> MyErr_Enum.MyErr_Success Then Throw New Exception("Could Not Set Next Email Time " & Log_Name)
        If Log_Set_Time(Now) <> MyErr_Enum.MyErr_Success Then Throw New Exception("Could Not Set Logger Time " & Log_Name)
        If Log_Set_Channel(Log_Channel) <> MyErr_Enum.MyErr_Success Then Throw New Exception("Could Not Set Logger RF Channel " & Log_Name)
        If Log_Set_RFID(Log_RFID) <> MyErr_Enum.MyErr_Success Then Throw New Exception("Could Not Set Logger RFID " & Log_Name)
        If Log_Set_RFFilter(Log_RFFilter) <> MyErr_Enum.MyErr_Success Then Throw New Exception("Could Not Set Logger RF Filter " & Log_Name)
        'If Log_Set_VerboseMode(Log_VerboseMode.TagOnly) <> MyErr_Enum.MyErr_Success Then Throw New Exception("Could Not Set Logger To Tag Only Verbose Mode " & Log_Name)
    End Sub

    Private Sub FinishedSegment()
        'Move short term buffer into long term buffer
        Dim i As Integer

        DataSemaphore = True
        If (SegmentsDone > 0) Then
            For i = 0 To WIDS.Count - 1
                If (ShortPickups(i) > NO_PICKUP) Then tagPickups(i) += 1
            Next
        End If

        For i = 0 To WIDS.Count - 1
            Data(i, SegmentsDone) = ShortPickups(i)
            ShortPickups(i) = NO_PICKUP
        Next

        If SegmentsDone + 10 > Data.GetUpperBound(1) Then
            Thread.BeginCriticalRegion()

            'ReDim Preserve Data(WIDS.Count - 1, Data.GetUpperBound(0) + 1000)
            ReDim Preserve Data(WIDS.Count - 1, Data.GetUpperBound(1) + 1000)

            Thread.EndCriticalRegion()
        End If

        SegmentsDone += 1
        'segmentstart = now (exact segment boundary though) = segmentstart + SEGMENTTIME
        SegmentStart = SegmentStart.AddSeconds(Logger_TX_RATE)
        'shortpickups all == NO_PICKUP.. done above!
        DataSemaphore = False
    End Sub

    Private Function GetWIDIndexFromID(ByVal ID As String) As Integer
        Dim i As Integer

        For i = 0 To WIDS.Count - 1
            If ID = WIDS(i).WID_ID Then
                Return i
            End If
        Next

        Return -1

    End Function

    Public Sub AddWID(ByVal wid As WID_Info)
        If Running = False Then
            WIDS.Add(wid)
        Else
            Throw New Exception("Can't add WID once started..")
        End If

    End Sub

    Private Sub Ser_ReadLine()
        Dim line As String = ""

        Try
            line = SerComLib.Ser.ReadLine()
        Catch ex As Exception
            Return
        End Try

        ParseLineAndSavePickupShortTerm(line)

    End Sub

    Private Sub ParseLineAndSavePickupShortTerm(ByVal txt As String)
        'Add to short term buffer
        Dim PickupTime As DateTime = Now

        If txt Like "RX ID: 0x*" & vbCr Then
            Dim index As Integer
            Dim span As TimeSpan

            index = GetWIDIndexFromID(txt.Substring(9, 8))

            If index >= 0 Then
                span = PickupTime - SegmentStart

                'todo[ ]: if it already was picked up, ... DO WHAT???
                ShortPickups(index) = span.TotalMilliseconds
            End If

        End If
    End Sub


    Private Sub CheckLogger()
        Dim tmpErr As MyErr_Enum
        Dim devID As UInt16
        Dim RetVals(0) As Object
        Dim tmpName As String

        SerComLib.Ser_Baud = 115200
        SerComLib.Ser_PortName = Log_Port

        tmpErr = SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_DevTypeID, Nothing, RetVals, 3)

        If tmpErr = MyErr_Enum.MyErr_Success Then
            devID = CType(RetVals(0), UInt16)
            If devID = SerDev_LoggerV2_ComFuncs.DevID Then
                'Read Device Name
                tmpErr = SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Get_LoggerName, Nothing, RetVals, 3)
                If tmpErr = MyErr_Enum.MyErr_Success Then
                    tmpName = CType(RetVals(0), String)
                    If tmpName = Log_Name Then
                        Return
                    Else
                        Throw New Exception("Incorrect Logger Name! We've got the wrong logger here!")
                    End If
                End If
            Else
                Throw New Exception("Incorrect Device ID! Must not be a logger on this port..")
            End If
        Else
            Throw New Exception("Error connecting.  Could not read DevID!")
        End If

        'check that the logger is there...
        'check that it's responsive
        'check that it is a logger (DevTypeID == 1)
        'check that it is the right logger.. Correct name.
    End Sub

    Private Function Log_Set_Channel(ByVal ch As Byte) As MyErr_Enum
        Dim Params(0) As Object

        Params(0) = CByte(Log_Channel)
        Return SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_RFChan, Params, Nothing, 3)

    End Function

    Private Function Log_Set_RFID(ByVal RFID() As Byte) As MyErr_Enum
        Dim Params(0) As Object

        Params(0) = RFID
        Return SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_RFID, Params, Nothing, 3)

    End Function

    Private Function Log_Set_RFFilter(ByVal RFFilter As String) As MyErr_Enum
        Dim Params(0) As Object

        Params(0) = RFFilter
        Return SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_RFFilter, Params, Nothing, 3)

    End Function

    Private Function Log_Set_VerboseMode(ByVal mode As Log_VerboseMode) As MyErr_Enum
        Dim Params(0) As Object

        Params(0) = mode
        Return SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_VerboseMode, Params, Nothing, 3)

    End Function

    Private Function Log_Set_Time(ByVal time As DateTime) As MyErr_Enum
        Dim Params(0) As Object

        Params(0) = time
        Return SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_DateTime, Params, Nothing, 3)

    End Function

    Private Function Log_Set_EmailTime(ByVal time As DateTime) As MyErr_Enum
        Dim Params(0) As Object

        Params(0) = time
        Return SerComLib.Ser_SendCmd(SerDev_LoggerV2_ComFuncs.CMD_ByteID.Set_SendDataGPRSNext, Params, Nothing, 3)

    End Function

    '*****************************************************************************************************
    '************************************ FINISHED *******************************************************
    '*****************************************************************************************************
    Public Sub New()

    End Sub


    '*****************************************************************************************************
    '************************************ TESTING/DUMMY/JUNK *********************************************
    '*****************************************************************************************************

    Public Sub TestSaveLoad(ByVal FileName As String)
        'open file
        Dim s As Single
        Dim b(3) As Byte
        Dim fs As IO.FileStream
        Dim sw As IO.StreamWriter
        Dim sr As IO.StreamReader
        Dim tr As IO.TextReader
        Dim br As IO.BinaryReader
        Dim tmpStr As String

        s = 1234.5678

        fs = New IO.FileStream(FileName, IO.FileMode.OpenOrCreate)
        sw = New IO.StreamWriter(fs)
        sw.WriteLine("Data Start")
        sw.WriteLine("a")
        sw.Flush()
        fs.Seek(sw.BaseStream.Position, IO.SeekOrigin.Begin)
        fs.Write(BitConverter.GetBytes(s), 0, 4)
        sw.Close()
        fs.Close()

        fs = New IO.FileStream(FileName, IO.FileMode.OpenOrCreate)
        sr = New IO.StreamReader(fs)
        br = New IO.BinaryReader(fs)
        tr = IO.TextReader.Synchronized(sr)
        'tmpStr = sr.ReadLine
        tmpStr = tr.ReadLine()
        If tmpStr <> "Data Start" Then
            'we have a problem
            Stop
        End If

        fs.Position = sr.BaseStream.Position

        fs.Read(b, 0, 4)
        s = BitConverter.ToSingle(b, 0)
        sr.Close()
        fs.Close()

    End Sub
End Class

Public Class PickupSegment
    Public TimeSegment As DateTime
    Public WIDPickupTime() As Single '# of seconds from the start of the segment or -1 if the wid wasn't picked up

    Public Sub New()

    End Sub

    Public Sub New(ByVal wids As Integer)
        ReDim WIDPickupTime(wids)
    End Sub
End Class

Public Class WID_Info
    'Public WID_TEST_INDEX As Integer 'For the 'PickupSegment.. could leave this, have a list of WID_Info's and just use the index in that as the index in the PickupSegment...!
    Public WID_ID As String
    Public WID_TX_Rate As Integer   'Measured in seconds, ie tx one blip every XXX seconds.
    Public WID_Info As String       'Info about the WID.. what 
    Public WID_Battery As String    'What power source is the WID hooked up to?
    Public WID_Channel As Integer   'The channel number (usually 2)
    Public WID_RFID(4) As Byte      '5 bytes, usually E6-E6-E6-E6-E6

    Public Sub New()

    End Sub

    Public Sub New(ByVal ID As String, ByVal TxRate As Integer, ByVal Channel As Integer, Optional ByVal RFID() As Byte = Nothing, Optional ByVal Battery As String = "Undefined", Optional ByVal Info As String = "")
        WID_ID = ID
        WID_TX_Rate = TxRate
        WID_Channel = Channel
        WID_Battery = Battery
        WID_Info = Info

        If IsNothing(RFID) Then
            WID_RFID = {&HE6, &HE6, &HE6, &HE6, &HE6}
        Else
            Dim i As Integer
            Try
                For i = 0 To 4
                    WID_RFID(i) = RFID(i)
                Next
            Catch ex As Exception
                Throw New Exception("Invalid RFID passed")
            End Try
        End If

    End Sub

    Public Sub New(ByVal instr As String)
        Dim s() As String = instr.Split("♫")
        Dim rfid() As String = s(6).Split("-")
        Dim i As Integer

        WID_ID = s(1)
        WID_TX_Rate = s(2)
        WID_Info = s(3)
        WID_Battery = s(4)
        WID_Channel = s(5)

        For i = 0 To 3
            WID_RFID(i) = CByte(rfid(i))
        Next
        WID_RFID(4) = CByte(rfid(4).Substring(0, rfid(4).Length - 1))
    End Sub

    Public Function Export() As String
        Return "♫" & WID_ID & "♫" & WID_TX_Rate & "♫" & WID_Info & "♫" & WID_Battery & "♫" & WID_Channel & "♫" & WID_RFID(0) & "-" & WID_RFID(1) & "-" & WID_RFID(2) & "-" & WID_RFID(3) & "-" & WID_RFID(4) & "¶"
    End Function
End Class

'Public Sub LoadFromFile(ByVal FileName As String)
'    Dim j As Integer
'    Dim i As Long
'    Dim tmpStr As String
'    Dim RFID() As String
'    Dim b(3) As Byte
'    Dim WidsCount As Integer
'    Dim cCount As Integer = 0
'    'Dim s As String

'    'open file
'    Dim fs As IO.FileStream
'    Dim sr As TGREER.myStreamReader

'    fs = New IO.FileStream(FileName, IO.FileMode.OpenOrCreate)
'    sr = New TGREER.myStreamReader(fs)

'    'save misc. bits in header
'    Log_Channel = sr.ReadLine()

'    tmpStr = sr.ReadLine()
'    cCount += tmpStr.Length + 2
'    RFID = tmpStr.Split("-")
'    For i = 0 To 4
'        Log_RFID(i) = RFID(i)
'    Next

'    Log_Port = sr.ReadLine()
'    Log_Name = sr.ReadLine()
'    Logger_TX_RATE = sr.ReadLine()
'    Log_RFFilter = sr.ReadLine()
'    TestStartTime = sr.ReadLine()
'    SegmentsDone = sr.ReadLine()
'    WidsCount = sr.ReadLine()

'    For j = 0 To WidsCount - 1
'        tmpStr = ""
'        Do
'            i = sr.Read()
'            tmpStr &= ChrW(i)
'        Loop While CStr(ChrW(i)) <> "¶"
'        AddWID(New WID_Info(tmpStr))
'    Next

'    sr.ReadLine() 'end line after last wid

'    tmpStr = sr.ReadLine
'    If tmpStr <> "Data Start" Then
'        'we have a problem
'        Stop
'    End If

'    fs.Flush()
'    'fs.Position

'    'fs.Position = 
'    fs.Position = fs.Length - (Len("Data End" & vbCrLf)) - (SegmentsDone * 4 * WidsCount)
'    fs.Seek(sr.BytesRead, IO.SeekOrigin.Begin)

'    ReDim Data(WidsCount, SegmentsDone - (SegmentsDone Mod 1000) + 999)
'    For i = 0 To SegmentsDone - 1
'        For j = 0 To WIDS.Count - 1
'            'fs.Write(BitConverter.GetBytes(Data(j, i)), 0, 4)
'            fs.Read(b, 0, 4)
'            Data(j, i) = BitConverter.ToSingle(b, 0)
'        Next
'    Next

'    'sr.BaseStream.Position = fs.Position

'    'tmpStr = sr.ReadToEnd
'    'If tmpStr <> "Data Stop" Then
'    '    'we have a problem
'    '    Stop
'    'End If

'    sr.Close()
'    fs.Close()
'    'save data bits
'End Sub

'Public Function GetCumulativeGraphData(ByVal gType As GraphType, ByVal points As Integer, ByVal TagID As Integer, Optional ByRef startTime As DateTime = Nothing, Optional ByRef stopTime As DateTime = Nothing) As PointF()
'    Dim retData() As PointF
'    Dim i As Long
'    Dim segsAvailable As Long
'    Dim segsPerPoint As Integer
'    Dim startIndex As Long = 0
'    Dim stopIndex As Long = 0
'    Dim LastIndex As Long   'also do this point so that the graph size is constant!
'    Dim pointsToAvg As Long = 100 '5 * 2 'MUST BE EVEN!!!

'    'Assume GraphType is 'cumulative'...
'    'Want to take every nth point where n = PointsAvailable/PointsToPrint

'    If points < 2 Then
'        Return Nothing
'    End If

'    'shouldn't use 0'th segment!
'    segsAvailable = SegmentsDone - 1

'    If segsAvailable > 0 And DataSemaphore = False Then
'        'calculate first segment
'        If startTime.Year = 1 Then
'            startIndex = 1 + pointsToAvg / 2
'        Else
'            startIndex = Math.Floor((CType(startTime - TestStartTime, TimeSpan).TotalSeconds) / Logger_TX_RATE) + pointsToAvg / 2
'        End If
'        'calculate last segment
'        If stopTime.Year = 1 Then
'            stopIndex = segsAvailable - pointsToAvg / 2
'        Else
'            stopIndex = Math.Floor((CType(stopTime - TestStartTime, TimeSpan).TotalSeconds) / Logger_TX_RATE) - pointsToAvg / 2
'        End If

'        If startIndex >= stopIndex Then
'            Return Nothing
'        End If
'        'calculate number of segments
'        segsAvailable = stopIndex - startIndex
'        'calculate points per segment or segments per point
'        segsPerPoint = segsAvailable / (points - 1) 'CInt(Math.Floor(CDec(segsAvailable) / CDec(points - 1)))
'        'Assign the 'set' last index
'        LastIndex = stopIndex

'        stopIndex = startIndex + segsPerPoint * (points - 1) - segsPerPoint / 2

'        If segsPerPoint < 1 Then
'            Return Nothing
'        End If

'        'reserve memory for points
'        ReDim retData(points)
'        'assign data to points

'        Try
'            For i = startIndex To stopIndex Step segsPerPoint
'                If DataSemaphore = True Then Return Nothing
'                'retData(((i - startIndex) / segsPerPoint)) = New PointF(startIndex + i, GetMovingAvgAtPoint(startIndex + i - (pointsToAvg / 2), pointsToAvg, TagID))
'                retData(((i - startIndex) / segsPerPoint)) = New PointF(i, GetMovingAvgAtPoint(i - (pointsToAvg / 2), pointsToAvg, TagID))
'                If retData(((i - startIndex) / segsPerPoint)).Y > 0 Then
'                    'Stop
'                End If
'            Next
'        Catch ex As Exception
'            'Stop
'        End Try


'        retData(points) = New PointF(LastIndex, GetMovingAvgAtPoint(LastIndex - (pointsToAvg), pointsToAvg, TagID))

'        Return retData

'    End If

'    Return Nothing
'End Function