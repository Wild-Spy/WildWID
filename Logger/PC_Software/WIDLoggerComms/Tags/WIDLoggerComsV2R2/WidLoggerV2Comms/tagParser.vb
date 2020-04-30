Imports System.Windows.Forms.DataVisualization.Charting

Public Class tagParser
    Public LogCollection As List(Of LogRow)
    Public tagList As List(Of String)

    Public Sub chrtMovingAvg(chrt As Chart, Optional clearData As Boolean = True)

        Dim i, j As Integer
        Dim tmpListData As List(Of PointF)

        If clearData = True Then chrt.Series.Clear()

        For i = 0 To tagList.Count - 1
            tmpListData = genDataForMovingAvg(i)
            chrt.Series.Add(tagList(i))
            chrt.Series(tagList(i)).ChartType = SeriesChartType.Line
            chrt.Series(tagList(i)).YAxisType = AxisType.Primary
            If Not IsNothing(tmpListData) Then
                For j = 0 To tmpListData.Count - 1
                    chrt.Series(tagList(i)).Points.AddXY(tmpListData(j).X, tmpListData(j).Y)
                Next
            End If

        Next

        For i = 0 To tagList.Count - 1
            tmpListData = genRSSIDataForMovingAvg(i)
            chrt.Series.Add(tagList(i) & " RSSI")
            chrt.Series(tagList(i) & " RSSI").ChartType = SeriesChartType.Line
            chrt.Series(tagList(i) & " RSSI").YAxisType = AxisType.Secondary
            If Not IsNothing(tmpListData) Then
                For j = 0 To tmpListData.Count - 1
                    chrt.Series(tagList(i) & " RSSI").Points.AddXY(tmpListData(j).X, tmpListData(j).Y)
                Next
            End If
        Next

        'chrt.ChartAreas(0).AxisX.Interval = (LogCollection.Count - 1) / 100
        chrt.ChartAreas(0).AxisX.IsStartedFromZero = True
        'chrt.ChartAreas(0).AxisX.MajorGrid.Interval = (LogCollection.Count - 1) / 50

        'chrt.ChartAreas(0).AxisY.Interval = Math.Round(maxBin / 10 / 5) * 5
        chrt.ChartAreas(0).RecalculateAxesScale()
    End Sub

    Private Function genDataForMovingAvg(tagId As Integer, Optional windowSize As Integer = 50) As List(Of PointF)
        Dim retVal As New List(Of PointF)
        Dim sum, i As Integer

        If windowSize >= LogCollection.Count Then Return Nothing

        sum = 0
        For i = 0 To windowSize - 1
            sum += IIf(LogCollection(i).Tags(tagId) = -1, 0, 1)
        Next

        For i = windowSize To LogCollection.Count - 1
            retVal.Add(New PointF(i - windowSize / 2, (sum / windowSize)))
            'retVal.Add(New PointF(i - windowSize / 2, IIf(LogCollection(i - windowSize / 2).Tags(tagId) = -1, 0, 1)))
            'retVal.Add(New PointF(i - windowSize / 2, IIf(LogCollection(i - windowSize / 2).Tags(tagId) = -1, -500, LogCollection(i - windowSize / 2).TagsRSSI(tagId))))
            sum -= IIf(LogCollection(i - windowSize).Tags(tagId) = -1, 0, 1)
            sum += IIf(LogCollection(i).Tags(tagId) = -1, 0, 1)
        Next

        retVal.Add(New PointF(i - windowSize / 2, (sum / windowSize)))

        Return retVal

    End Function

    Private Function genRSSIDataForMovingAvg(tagId As Integer, Optional windowSize As Integer = 50) As List(Of PointF)
        Dim retVal As New List(Of PointF)
        Dim sum, i As Integer

        If windowSize >= LogCollection.Count Then Return Nothing

        sum = 0
        For i = 0 To windowSize - 1
            sum += LogCollection(i).TagsRSSI(tagId)
        Next

        For i = windowSize To LogCollection.Count - 1
            retVal.Add(New PointF(i - windowSize / 2, (sum / windowSize)))
            'retVal.Add(New PointF(i - windowSize / 2, IIf(LogCollection(i - windowSize / 2).Tags(tagId) = -1, 0, 1)))
            'retVal.Add(New PointF(i - windowSize / 2, IIf(LogCollection(i - windowSize / 2).Tags(tagId) = -1, -500, LogCollection(i - windowSize / 2).TagsRSSI(tagId))))
            sum -= LogCollection(i - windowSize).TagsRSSI(tagId)
            sum += LogCollection(i).TagsRSSI(tagId)
        Next

        retVal.Add(New PointF(i - windowSize / 2, (sum / windowSize)))

        Return retVal

    End Function

    Public Sub ParseText(ByVal parseStr As String) ', ByRef lvAdd As ListView)
        Dim lines() As String = parseStr.Split(vbLf)
        Dim line As String
        Dim parts() As String
        Dim n, m As LogRow
        Dim skipLine As Boolean
        Dim lineTime, lineGroupStartTime, lastStartTime As DateTime
        Dim i As Integer = 0
        Dim j As Integer
        Const MAXTAGS As Integer = 20
        Const TAGPERIOD As Decimal = 10 'in seconds

        LogCollection = New List(Of LogRow)
        tagList = New List(Of String)

        For Each line In lines
            skipLine = False
            n = Nothing
            m = Nothing

            Try
                line = line.Trim()
                parts = line.Split(",")
                'parts(0) = parts(0).Replace("""", "")
                'parts(1) = parts(1).Split(":")(1).Substring(1)
                lineTime = DateTime.Parse(parts(1) & " " & parts(2))
                If i = 0 Then
                    With lineTime
                        lastStartTime = New DateTime(.Year, .Month, .Day, .Hour, .Minute, .Second)
                    End With
                End If
                With lineTime
                    lineGroupStartTime = lastStartTime.AddSeconds(Math.Floor((lineTime - lastStartTime).TotalSeconds / TAGPERIOD) * TAGPERIOD)
                    'tmpTimeStart = New DateTime(.Year, .Month, .Day, .Hour, .Minute, .Second)
                    'tmpTimeStart.AddSeconds(TAGPERIOD * LogCollection.)
                End With
                If i = 0 Then lastStartTime = lineGroupStartTime
                n = findTimeInList(lineGroupStartTime) '.ToString("G"))
            Catch ex As Exception
                skipLine = True
            End Try

            If skipLine = False Then

                'If tmpTimeStart > New DateTime(2011, 11, 14, 1, 0, 30) Then Stop

                While (lineGroupStartTime - lastStartTime).TotalSeconds > TAGPERIOD
                    lastStartTime = lastStartTime.AddSeconds(TAGPERIOD)
                    m = New LogRow(MAXTAGS)
                    m.Time = lastStartTime '.ToString("G")
                    LogCollection.Add(m)
                    'lastAddedIndex += 1
                End While

                If n Is Nothing Then
                    'first entry..??
                    n = New LogRow(MAXTAGS)
                    n.Time = lineGroupStartTime '.ToString("G")
                    LogCollection.Add(n)
                    lastStartTime = lastStartTime.AddSeconds(TAGPERIOD)
                    'lastAddedTime.AddSeconds(TAGPERIOD)
                    'lastAddedIndex += 1
                End If

                j = FindTagInList(parts(3))
                If j > -1 Then
                    'total number of seconds since the start time listed
                    n.Tags(j) = (lineTime - lineGroupStartTime).TotalSeconds
                    n.TagsRSSI(j) = parts(5)
                Else
                    If tagList.Count < MAXTAGS Then
                        tagList.Add(parts(3))
                        n.Tags(tagList.Count - 1) = (lineTime - lineGroupStartTime).TotalSeconds
                        n.TagsRSSI(tagList.Count - 1) = parts(5)
                    End If

                End If

                'Select Case parts(1)
                '    Case "0xABCDEFAB" '0xDEAD0001" '"0xFFFF1116", "FFFF1116"
                '        'OO
                '        n.Tags(0) = tmpTime.Second
                '    Case "0xEEFF1122" '"0xDEAD0002" '"0xFFFF7770", "FFFF7770"
                '        'ON
                '        n.Tags(1) = tmpTime.Second
                '    Case "0xAABBB00B" '"0xDEAD0003" '"0xFFFF9999", "FFFF9999"
                '        'NN
                '        n.Tags(2) = tmpTime.Second
                '    Case "0xDEAD0004" '"0xFFFF3336", "FFFF3336"
                '        'PSU
                '        n.Tags(3) = tmpTime.Second
                'End Select

                'lastStartTime = tmpTimeStart
                i += 1

            End If

        Next
    End Sub

    Private Function roundNearestTen(ByVal value As Integer) As Integer
        Return Math.Floor(CDec(value) / 10) * 10
    End Function

    Private Function findTimeInList(ByVal fTime As DateTime) As LogRow
        'Dim n As ListViewItem

        Dim i As Integer

        Try
            For i = LogCollection.Count - 1 To LogCollection.Count - 10 Step -1 '0 Step -1
                'If LogCollection(i).Time.ToString("G") = fTime.ToString("G") Then
                If LogCollection(i).Time = fTime Then
                    Return LogCollection(i)
                End If
            Next
        Catch ex As Exception
            Return Nothing
        End Try


        'For Each n In lvAdd.Items()
        '    If n.SubItems(0).Text = fTime Then
        '        Return n
        '    End If
        'Next

        Return Nothing

    End Function

    Private Function FindTagInList(ByVal tagID As String) As Integer
        'Dim n As ListViewItem

        Dim i As Integer

        Try
            For i = 0 To tagList.Count - 1
                If tagList(i) = tagID Then
                    Return i
                End If
            Next
        Catch ex As Exception
            'Return False
        End Try

        Return -1

    End Function


End Class


Public Class LogRow
    Public Time As DateTime
    Public Tags() As Decimal
    Public TagsRSSI() As Decimal

    Public Sub New()
        Time = ""
        ReDim Tags(0)
        Tags(0) = -1
    End Sub

    Public Sub New(numTags As Integer)
        Dim i As Integer

        Time = Nothing '""

        ReDim Tags(numTags - 1)
        ReDim TagsRSSI(numTags - 1)

        For i = 0 To Tags.Count - 1
            Tags(i) = -1
            TagsRSSI(i) = -110
        Next
    End Sub
End Class