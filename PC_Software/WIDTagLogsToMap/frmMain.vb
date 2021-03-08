Imports System.IO
Imports System.Xml
Imports System.Text
Imports SharpKml.Engine
Imports WIDTagLogsToMap.Mapping

Public Class frmMain

    Dim t1, t2 As DateTime

    Public loggerList As New List(Of GPSFix)
    Public Mapper As Mapping
    Public tagPickupList As New List(Of TagPickupEvent)

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        'Dim doc As XmlDocument = New XmlDocument()
        'doc.LoadXml("<EventTracker><StandardItem><Header1>Header1 Text</Header1> <Header2>Header2 Text</Header2></StandardItem><Item> <Events> <EventSub EventId='73' EventName='Orchestra' Description='0'> <Person PersonId='189323156' PersonName='Chandra' Address='Arunachal'/><Person PersonId='189323172' PersonName='Sekhar' Address='Himachal'/></EventSub> </Events> </Item> </EventTracker>")
        'Dim header1 As String = doc.SelectSingleNode("EventTracker/StandardItem/Header1").InnerText
        'Dim header2 As String = doc.SelectSingleNode("EventTracker/StandardItem/Header2").InnerText
        'For Each eventSubNode As XmlNode In doc.SelectNodes("EventTracker/Item/Events/EventSub")
        '    Dim eventId As String = eventSubNode.Attributes("EventId").InnerText
        '    Dim eventName As String = eventSubNode.Attributes("EventName").InnerText
        '    Dim eventDescription As String = eventSubNode.Attributes("Description").InnerText
        '    For Each personNode As XmlNode In eventSubNode.SelectNodes("Person")
        '        Dim personId As String = personNode.Attributes("PersonId").InnerText
        '        Dim personName As String = personNode.Attributes("PersonName").InnerText
        '        Dim personAddress As String = personNode.Attributes("Address").InnerText
        '    Next
        'Next

        'Dim xmlString As String = My.Computer.FileSystem.ReadAllText("E:\Wild Spy\Projects_Online\WS005 - WID Logger\4 - Dev Testing\Cheryl Test 1\WID test 3 Waypoints_2.kml")
        Dim xmlString As String = My.Computer.FileSystem.ReadAllText(IO.Path.GetDirectoryName(Application.ExecutablePath) & "\WID test 3 Waypoints_2.kml")


        Dim doc As XmlDocument = New XmlDocument
        doc.LoadXml(xmlString)

        'Create an XmlNamespaceManager for resolving namespaces.
        Dim xmlnsManager As New XmlNamespaceManager(doc.NameTable)
        xmlnsManager.AddNamespace("aa", "http://www.opengis.net/kml/2.2")
        xmlnsManager.AddNamespace("kml", "http://www.opengis.net/kml/2.2")
        xmlnsManager.AddNamespace("gx", "http://www.google.com/kml/ext/2.2")
        xmlnsManager.AddNamespace("atom", "http://www.w3.org/2005/Atom")

        Dim LoggersNode As XmlNode
        For Each node As XmlNode In doc.SelectNodes("/kml:kml/kml:Document/kml:Folder/kml:Folder", xmlnsManager)
            If node.SelectSingleNode("kml:name", xmlnsManager).InnerText = "Loggers" Then
                'This is the folder We want
                LoggersNode = node
                Exit For
            End If
        Next

        If IsNothing(LoggersNode) Then
            Exit Sub
        End If

        loggerList.Clear()
        For Each node As XmlNode In LoggersNode.SelectNodes("kml:Placemark", xmlnsManager)
            loggerList.Add(ReadKMLPlacemark(node, xmlnsManager))
        Next

    End Sub

    Public Function ReadKMLPlacemark(node As XmlNode, xmlnsman As XmlNamespaceManager) As GPSFix
        If node.Name <> "Placemark" Then
            Return Nothing
        End If

        Dim retVal As New GPSFix

        retVal.name = node.SelectSingleNode("kml:name", xmlnsman).InnerText
        retVal.LoggerID = retVal.name.Substring(1)

        Dim timeStr As String
        timeStr = node.SelectSingleNode("kml:TimeStamp/kml:when", xmlnsman).InnerText
        timeStr = timeStr.Replace("T", " ")
        timeStr = timeStr.Replace("Z", "")
        retVal.time = DateTime.ParseExact(timeStr, "yyyy-MM-dd HH:mm:ss", Nothing)

        Dim tmpStrs() As String
        tmpStrs = node.SelectSingleNode("kml:Point/kml:coordinates", xmlnsman).InnerText.Split(",")
        retVal.longitude = tmpStrs(0)
        retVal.latitude = tmpStrs(1)
        retVal.altitude = tmpStrs(2)

        Return retVal

    End Function

    Private Function NavigateElementWithSubElement(reader As XmlReader, ElementName As String, SubElementMatchName As String, SubElementMatchValue As String) As Boolean

        Do While reader.ReadToFollowing(ElementName) = True
            If reader.ReadToFollowing(SubElementMatchName) = True Then
                reader.Read()
                If reader.NodeType = XmlNodeType.Text Then
                    If reader.Value = SubElementMatchValue Then
                        Return True
                    End If
                End If
            End If
        Loop

        Return False


    End Function

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click

        If OpenFileDialog1.ShowDialog() <> Windows.Forms.DialogResult.OK Then
            Exit Sub
        End If

        Dim txt As String = My.Computer.FileSystem.ReadAllText(OpenFileDialog1.FileName) '"E:\Wild Spy\Projects_Online\WS005 - WID Logger\4 - Dev Testing\Cheryl Test 1\WID test 3 data\Tag File Outputs\Tag_30000016.csv")

        tagPickupList.Clear()
        For Each n As String In txt.Replace(vbCr, "").Split(vbLf)
            Try
                tagPickupList.Add(New TagPickupEvent(n))
            Catch ex As Exception

            End Try
        Next
        tbFixTime.Maximum = tagPickupList.Count - 1
        tbFixTime.Minimum = 0
        tbFixTime.Value = 0

    End Sub

    Public MapMouseStartPoint As Point = New Point(0, 0)

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If loggerList.Count > 0 Then
            'tbFixTime.Minimum = 1
            'tbFixTime.Maximum = loggerList.Count - 1
            'tbFixTime.SmallChange = 1
            'Dim ll As New List(Of GPSFix)
            'For i As Integer = 0 To loggerList.Count - 1
            '    ll.Add(loggerList(i))

            '    Mapper = New Mapping(pbMap, ll)
            '    'startVP = Mapper.ZoomAll(FixList)

            '    Mapper.StopTime = New DateTime(2100, 1, 1, 1, 1, 1)
            '    Mapper.StartTime = New DateTime(1900, 1, 1, 1, 1, 1)
            '    UpdateMap()
            '    DoEventDelay(250)
            'Next

            Mapper = New Mapping(pbMap, loggerList)
            'startVP = Mapper.ZoomAll(FixList)

            Mapper.StopTime = New DateTime(2100, 1, 1, 1, 1, 1)
            Mapper.StartTime = New DateTime(1900, 1, 1, 1, 1, 1)
            UpdateMap()
        End If
    End Sub


    Private Sub UpdateMap(Optional ByVal vp As WorldViewPort = Nothing)
        Mapper.DrawMapToControl(vp)
    End Sub

    Private Sub tbFixTime_Scroll(sender As Object, e As EventArgs) Handles tbFixTime.Scroll
        'Dim hoursBefore As Integer = 24 * 7
        ''alglib.exponentialintegralei(

        'If Not IsNothing(Mapper) Then
        '    'Mapper.StopTime = FixList.FixList(tbFixTime.Minimum).GetFixTime.AddHours((tbFixTime.Value - tbFixTime.Minimum) / 2)
        '    'Mapper.StartTime = Mapper.StopTime.Subtract(New TimeSpan(5, 0, 0)) 'FixList.FixList(tbFixTime.Value - a).GetFixTime

        '    Mapper.StopTime = FixList(tbFixTime.Value).Time()
        '    Mapper.StartTime = Mapper.StopTime.Subtract(New TimeSpan(hoursBefore, 0, 0)) 'FixList.FixList(tbFixTime.Value - a).GetFixTime

        '    UpdateMap()
        'End If
    End Sub

    Private Sub pbMap_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pbMap.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Left Then
            MapMouseStartPoint = e.Location
            pbMap.Focus()
        End If
    End Sub

    Private Sub pbMap_MouseMove(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pbMap.MouseMove

        If e.Button = Windows.Forms.MouseButtons.Left And Not IsNothing(Mapper) Then
            'Mapper = New Mapping(MapB,)

            Dim diff As Point = MapMouseStartPoint - e.Location

            Dim vp As WorldViewPort = Mapper.MapViewPort.Clone()
            Dim scale As PointF = Mapper.Screen.CalculateScale(vp)

            vp.ChangeWorldViewPortPosition(vp.World_OriginX + diff.X / scale.X, vp.World_OriginY - diff.Y / scale.Y)

            UpdateMap(vp)
        End If


    End Sub

    Private Sub pbMap_MouseUp(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pbMap.MouseUp
        If Not IsNothing(Mapper) Then
            Dim diff As Point = MapMouseStartPoint - e.Location
            Dim vp As WorldViewPort = Mapper.MapViewPort.Clone()
            Dim scale As PointF = Mapper.Screen.CalculateScale(vp)

            vp.ChangeWorldViewPortPosition(vp.World_OriginX + diff.X / scale.X, vp.World_OriginY - diff.Y / scale.Y)
            Mapper.SetViewPort(vp)
            UpdateMap()
        End If
    End Sub

    Private Sub pbMap_MouseWheel(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles pbMap.MouseWheel
        If Not IsNothing(Mapper) Then
            'Dim diff As Point = MapMouseStartPoint - e.Location
            'Dim vp As WorldViewPort = Mapper.MapViewPort.Clone()
            'Dim scale As PointF = Mapper.Screen.CalculateScale(vp)

            'vp.ChangeWorldViewPortPosition(vp.World_OriginX + diff.X / scale.X, vp.World_OriginY + diff.Y / scale.Y)
            'Mapper.SetViewPort(vp)

            If e.Delta < 0 Then
                Mapper.MapViewPort.Zoom(1.25)
            ElseIf e.Delta > 0 Then
                Mapper.MapViewPort.Zoom(0.75)
            End If

            UpdateMap()
        End If
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click

    End Sub

    'Private Sub btnPlayMap_Click(sender As Object, e As EventArgs) Handles btnPlayMap.Click
    '    Static IsRunning As Boolean
    '    Static abort As Boolean
    '    Dim stp As Integer = 1
    '    Dim delayms As Integer = 100
    '    Dim nextVal As Integer

    '    If IsRunning = True Then
    '        abort = True
    '        Exit Sub
    '    End If

    '    IsRunning = True
    '    abort = False
    '    btnPlayMap.Text = "| |"
    '    Do
    '        'get the next successful of partial fix
    '        nextVal = tbFixTime.Value + 1
    '        'Do
    '        '    nextVal += 1
    '        'Loop While FixList(nextVal).Type = GPSFixType.Fix_Failed


    '        'If nextVal >= tbFixTime.Maximum Then
    '        'tbFixTime.Value = tbFixTime.Maximum
    '        Application.DoEvents()
    '        'Dim t1, t2 As DateTime
    '        't1 = tagPickupList(tbFixTime.Value).timestamp
    '        't2 = tagPickupList(tbFixTime.Value).timestamp.Subtract(New TimeSpan(0, 0, 1))
    '        'If t1 > t2 Then
    '        '    Mapper.StopTime = t1
    '        '    Mapper.StartTime = t2
    '        'Else
    '        '    Mapper.StopTime = t2
    '        '    Mapper.StartTime = t1
    '        'End If

    '        For Each logger As GPSFix In loggerList
    '            If tagPickupList(tbFixTime.Value).LoggerID = logger.LoggerID Then
    '                logger.size = (tagPickupList(tbFixTime.Value).RSSI + 100) * 2
    '                'If logger.size > 50 Then logger.size = 50
    '            Else
    '                logger.size = 1
    '                If logger.size < 0 Then logger.size = 0
    '            End If

    '        Next

    '        UpdateMap()

    '        'Else
    '        tbFixTime.Value = nextVal
    '        tbFixTime_Scroll(sender, e)
    '        'End If

    '        DoEventDelay(delayms)
    '    Loop While tbFixTime.Value < tbFixTime.Maximum And abort = False

    '    IsRunning = False
    '    btnPlayMap.Text = ">"
    'End Sub

    Private Sub btnPlayMap_Click(sender As Object, e As EventArgs) Handles btnPlayMap.Click
        Const WID_TX_PER As Integer = 2
        Static IsRunning As Boolean
        Static abort As Boolean
        Dim stp As Integer = 1
        Dim delayms As Integer = 100
        'Dim nextVal As Integer
        Dim tStart, tEnd As DateTime
        'Dim t1, t2 As DateTime
        Dim totSecs As Integer

        If IsRunning = True Then
            abort = True
            Exit Sub
        End If

        If tbFixTime.Value = 0 Then
            tStart = tagPickupList.First.timestamp
            tEnd = tagPickupList.Last.timestamp

            totSecs = CType((tEnd - tStart), TimeSpan).TotalSeconds / WID_TX_PER

            'subtaract because we add first thing in the main loop
            t1 = tStart.AddSeconds(-WID_TX_PER)
            t2 = tStart '.AddSeconds(WID_TX_PER)
        End If

        IsRunning = True
        abort = False
        btnPlayMap.Text = "| |"
        Button6.Enabled = False
        Do
            'get the next successful of partial fix
            'nextVal = tbFixTime.Value + 1

            t1 = t1.AddSeconds(WID_TX_PER)
            t2 = t2.AddSeconds(WID_TX_PER)

            'Do
            '    nextVal += 1
            'Loop While FixList(nextVal).Type = GPSFixType.Fix_Failed


            'If nextVal >= tbFixTime.Maximum Then
            'tbFixTime.Value = tbFixTime.Maximum
            Application.DoEvents()

            For Each logger As GPSFix In loggerList
                logger.size = 0
            Next

            For i = tbFixTime.Value To tbFixTime.Maximum
                If tagPickupList(i).timestamp >= t1 And tagPickupList(i).timestamp < t2 Then
                    For Each logger As GPSFix In loggerList
                        If tagPickupList(i).LoggerID = logger.LoggerID Then
                            logger.size = (tagPickupList(i).RSSI + 100) * 2
                            Exit For
                        End If
                    Next
                Else
                    tbFixTime.Value = i
                    Exit For
                End If
            Next

            Mapper.StopTime = t2

            UpdateMap()

            'tbFixTime.Value = nextVal
            tbFixTime_Scroll(sender, e)

            DoEventDelay(delayms)
        Loop While tbFixTime.Value < tbFixTime.Maximum And abort = False

        IsRunning = False
        btnPlayMap.Text = ">"
        Button6.Enabled = True
    End Sub

    Public Sub DoEventDelay(ByVal msDelay As Integer)
        Dim t1 As DateTime

        t1 = Now()
        t1 = t1.AddMilliseconds(msDelay)
        Do
            Application.DoEvents()
        Loop While Now() < t1

    End Sub

    Private Sub Label1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles Label1.MouseDoubleClick
        If e.Button = Windows.Forms.MouseButtons.Right Then
            Button1.Visible = Not Button1.Visible
            Button2.Visible = Not Button2.Visible
            Button3.Visible = Not Button3.Visible
            Button4.Visible = Not Button4.Visible
        End If
    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Button1_Click(sender, e)
        Button2_Click(sender, e)
        Button3_Click(sender, e)
        Button4_Click(sender, e)
    End Sub

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If btnPlayMap.Text <> ">" Then
            btnPlayMap_Click(sender, e)
            DoEventDelay(500)
        End If
        Application.Exit()
    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub frmMain_MaximizedBoundsChanged(sender As Object, e As EventArgs) Handles Me.MaximizedBoundsChanged
        Button3_Click(sender, e)
    End Sub

    Private Sub frmMain_ResizeEnd(sender As Object, e As EventArgs) Handles Me.ResizeEnd
        Button3_Click(sender, e)
    End Sub

    Private Sub pbMap_Click(sender As Object, e As EventArgs) Handles pbMap.Click

    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click

        'If btnPlayMap.Text <> ">" Then
        '    btnPlayMap_Click(sender, e)
        '    tbFixTime.Value = 0
        '    btnPlayMap_Click(sender, e)
        'Else
        tbFixTime.Value = 0
        'End If

    End Sub
End Class

Public Class TagPickupEvent
    Public LoggerID As Integer
    Public timestamp As DateTime
    Public RSSI As Integer
    Public recordnumber As Integer

    Public Sub New(intext As String)
        Dim cols() As String = intext.Split(",")
        LoggerID = cols(0)
        timestamp = getTimeFromFileLine(intext)
        RSSI = cols(5)
        recordnumber = cols(1)
    End Sub

    Private Function getTimeFromFileLine(line As String) As DateTime
        Dim parts() As String = line.Split(",")

        Dim TimeStr As String = parts(2).Trim & " " & parts(3).Trim

        Dim dt As DateTime = DateTime.ParseExact(TimeStr, "dd/MM/yyyy HH:mm:ss", Nothing)

        Return dt

    End Function
End Class