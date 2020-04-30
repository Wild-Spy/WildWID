Public Class frmMain

    Private lvTimelineColSort As ListViewColumnSorter
    Private lvLogsColSort As ListViewColumnSorter
    Private lvSysEventsColSort As ListViewColumnSorter
    Private LogList As New List(Of LogEntry)

    Public Sub SortLogList(ByRef List As List(Of LogEntry))
        Dim n As LogEntry
        Dim newList As New List(Of LogEntry)
        Dim cnt As Integer = 0
        Dim lastId As Integer = 0

        Do
            For Each n In List
                If n.Address = lastId Then
                    newList.Add(n)
                    cnt += 1
                    Exit For
                End If
            Next

            lastId += 1
        Loop While cnt < List.Count

        List = newList

    End Sub

    Private Sub btnInterpret_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInterpret.Click
        InterpretTextAsList(rtbInterpretText.Text, False)
    End Sub

    Private Function ReplaceSpacesWithTab(ByVal txt As String) As String
        Dim i As Integer

        For i = 0 To txt.Length - 1
            If txt(i) = " " Then
                txt = txt.Substring(0, i) & vbTab & txt.Substring(i + 1)

                i += 1

                If i < txt.Length Then
                    While txt(i) = " "
                        i += 1
                        If i >= txt.Length Then
                            Exit While
                        End If
                    End While
                End If
            End If
        Next

        txt = txt.Replace(" ", "")

        Return txt
    End Function

    Private Sub InterpretTextAsList(ByVal iText As String, ByVal append As Boolean)
        Dim lines() As String
        Dim tmpLine As String
        Dim parts() As String
        Dim Address As New List(Of UInt32)
        Dim Times As New List(Of DateTime)
        Dim IDs As New List(Of UInt32)
        Dim Flags As New List(Of Byte)

        'Like...
        '00000000        03      10000053        28-10-11 19:25:44
        '00000001        03      10000041        28-10-11 19:25:44
        lines = iText.Split(vbLf)

        For Each tmpLine In lines
            'Try

            If Not tmpLine Like "[0-9 A-F a-f][0-9 A-F a-f][0-9 A-F a-f][0-9 A-F a-f][0-9 A-F a-f][0-9 A-F a-f][0-9 A-F a-f][0-9 A-F a-f]        [0-9 A-F a-f][0-9 A-F a-f]      [0-9 A-F a-f][0-9 A-F a-f][0-9 A-F a-f][0-9 A-F a-f][0-9 A-F a-f][0-9 A-F a-f][0-9 A-F a-f][0-9 A-F a-f]        ##-##-## ##:##:##*" Then '"########        ##      ########        ##-##-## ##:##:##*" Then
                tmpLine &= ""
            Else
                parts = ReplaceSpacesWithTab(tmpLine).Split(vbTab)
                Address.Add(Int32.Parse(parts(0), Globalization.NumberStyles.HexNumber))
                Flags.Add(parts(1))
                IDs.Add(UInt32.Parse(parts(2), Globalization.NumberStyles.HexNumber))
                Try
                    Times.Add(New DateTime(parts(3).Substring(6, 2) + 2000, parts(3).Substring(3, 2), parts(3).Substring(0, 2), _
                                                           parts(4).Substring(0, 2), parts(4).Substring(3, 2), parts(4).Substring(6, 2)))
                Catch ex As Exception
                    Times.Add(New DateTime())
                End Try

                'If address

            End If


            'Catch ex As Exception

            'End Try
        Next

        AddToList(Address, Times, IDs, Flags, append)
        SortLogList(LogList)
        CalcTagTimelines()

    End Sub

    Private Function AddressInLogList(ByVal Address As UInt32) As Boolean
        Dim LE As LogEntry

        For Each LE In LogList
            If LE.Address = Address Then
                Return True
            End If
        Next

        Return False

    End Function

    Private Function FindAddressInLogList(ByVal Address As UInt32) As Integer
        Dim i As Integer

        For i = 0 To LogList.Count - 1 'lvLogs.Items.Count - 1
            If LogList(i).Address = Address Then 'UInt32.Parse(lvLogs.Items(i).SubItems(0).Text) = Address Then
                Return i
            End If
        Next

        Return -1

    End Function

    Private Sub AddToList(ByRef Address As List(Of UInt32), ByRef Times As List(Of DateTime), ByRef IDs As List(Of UInt32), ByRef Flags As List(Of Byte), ByVal append As Boolean)
        Dim i As Integer
        Dim n As ListViewItem
        Dim tmpStr As String
        Dim LE As LogEntry

        If append = False Then
            'Erase all logs from list view control
            lvLogs.Items.Clear()
            lvSysEvents.Items.Clear()
            LogList = New List(Of LogEntry)
            lvLogs.BeginUpdate()
            lvSysEvents.BeginUpdate()
        End If

        For i = 0 To Address.Count - 1
            'Only add unique addresses!
            If AddressInLogList(Address(i)) = False Then
                'Add new Log Entry
                LE = New LogEntry
                LE.Address = Address(i)
                LE.Flags = Flags(i)
                LE.ID = IDs(i)
                LE.Time = Times(i)
                LogList.Add(LE)

                n = New ListViewItem()
                n.Text = Address(i)
                n.SubItems.Add(Times(i).ToString("dd/MM/yyyy"))
                n.SubItems.Add(Times(i).ToString("HH:mm:ss"))

                If Flags(i) = 2 Then
                    'System Message
                    Select Case (IDs(i) And &HFF000000) >> 24
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
                    n.SubItems.Add((IDs(i) And &HFFFFFF).ToString("X"))
                    lvSysEvents.Items.Add(n)
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
                    lvLogs.Items.Add(n)
                End If
            Else
                Dim tmpI As Integer
                tmpI = FindAddressInLogList(Address(i))
                With LogList(tmpI)
                    If .Address <> Address(i) Or .Flags <> Flags(i) Or .ID <> IDs(i) Or .Time <> Times(i) Then
                        'missmatch!!
                        Dim rslt As MsgBoxResult = MsgBox("Address " & Address(i) & " (0x" & Address(i).ToString("X8") & ") is not identical to duplicate which is already in list!  " & vbNewLine & "Continue?", vbYesNo)
                        If rslt <> MsgBoxResult.Yes Then
                            Exit For
                        End If
                    End If
                End With
            End If
        Next

        lvLogs.EndUpdate()
        lvSysEvents.EndUpdate()
    End Sub

    Private Function CalcTagTimelines()
        Dim n As LogEntry
        Dim ActList As New List(Of UInt32)
        Dim AddList As New List(Of UInt32)
        Dim listPos As Integer
        Dim logListPos As Integer
        Dim i As Integer

        lvTimeline.Items.Clear()

        For Each n In LogList
            If n.Flags = 3 Then 'START
                If AddToActList(ActList, AddList, n.ID, n.Address) = False Then
                    'The item was already in the list!!
                    'TODO[ ]: Fix this...

                    'ID is in the list
                    listPos = ActList.IndexOf(n.ID)
                    logListPos = FindAddressInLogList(AddList(listPos))

                    AddToTimelineList(logListPos, n, True)

                    'Remove the old one
                    ActList.RemoveAt(listPos)
                    AddList.RemoveAt(listPos)

                    'Add the new one
                    AddToActList(ActList, AddList, n.ID, n.Address)

                    'Stop
                End If
            ElseIf n.Flags = 1 Then 'STOP
                If ActList.Contains(n.ID) Then
                    'ID is in the list
                    listPos = ActList.IndexOf(n.ID)
                    logListPos = FindAddressInLogList(AddList(listPos))

                    AddToTimelineList(logListPos, n)

                    ActList.RemoveAt(listPos)
                    AddList.RemoveAt(listPos)

                Else
                    'ID not in list!!
                    'TODO [ ]: Fix this...
                    AddToTimelineList(-1, n, False, True)
                    'Stop
                End If
            ElseIf n.Flags = 2 Then
                Dim id As Integer = (n.ID And &HFF000000) >> 24

                If id = 0 OrElse id = 1 OrElse id = 2 Then 'power on, wakeup from bat backup (no time update),  wakeup from bat backup (time updated)
                    'clear list
                    For i = 0 To ActList.Count - 1
                        AddToTimelineList(AddList(i), n, True, False)
                    Next

                    AddList.Clear()
                    ActList.Clear()
                End If

            Else

            End If
        Next

        While ActList.Count > 0
            logListPos = AddList(0)
            AddToTimelineList(listPos, Nothing, True, False)
            'Remove the old one
            ActList.RemoveAt(0)
            AddList.RemoveAt(0)
        End While

        Return True

    End Function

    'Private Function CalcTagTimelines()
    '    Dim n As ListViewItem
    '    Dim ActList As New List(Of UInt32)
    '    Dim AddList As New List(Of UInt32)
    '    Dim listPos As Integer
    '    Dim listCtrlPos As Integer

    '    lvTimeline.Items.Clear()

    '    For Each n In lvLogs.Items
    '        If n.SubItems(4).Text = "Start" Then
    '            If AddToActList(ActList, AddList, UInt32.Parse(n.SubItems(3).Text, Globalization.NumberStyles.HexNumber), UInt32.Parse(n.SubItems(0).Text)) = False Then
    '                'The item was already in the list!!
    '                'TODO[ ]: Fix this...

    '                'ID is in the list
    '                listPos = ActList.IndexOf(UInt32.Parse(n.SubItems(3).Text, Globalization.NumberStyles.HexNumber))
    '                listCtrlPos = FindAddressInListView(AddList(listPos))

    '                AddToTimelineList(listCtrlPos, n, True)

    '                'Remove the old one
    '                ActList.RemoveAt(listPos)
    '                AddList.RemoveAt(listPos)

    '                'Add the new one
    '                AddToActList(ActList, AddList, UInt32.Parse(n.SubItems(3).Text, Globalization.NumberStyles.HexNumber), UInt32.Parse(n.SubItems(0).Text))

    '                'Stop
    '            End If
    '        ElseIf n.SubItems(4).Text = "Stop" Then
    '            If ActList.Contains(UInt32.Parse(n.SubItems(3).Text, Globalization.NumberStyles.HexNumber)) Then
    '                'ID is in the list
    '                listPos = ActList.IndexOf(UInt32.Parse(n.SubItems(3).Text, Globalization.NumberStyles.HexNumber))
    '                listCtrlPos = FindAddressInListView(AddList(listPos))

    '                AddToTimelineList(listCtrlPos, n)

    '                ActList.RemoveAt(listPos)
    '                AddList.RemoveAt(listPos)

    '            Else
    '                'ID not in list!!
    '                'TOO[ ]: Fix this...
    '                AddToTimelineList(-1, n, False, True)
    '                'Stop
    '            End If
    '        Else

    '        End If
    '    Next

    '    While ActList.Count > 0
    '        listCtrlPos = AddList(0)
    '        AddToTimelineList(listPos, Nothing, True, False)
    '        'Remove the old one
    '        ActList.RemoveAt(0)
    '        AddList.RemoveAt(0)
    '    End While

    '    Return True

    'End Function

    Private Sub AddToTimelineList(ByVal startLogListPos As Integer, ByRef n As LogEntry, Optional ByVal noStop As Boolean = False, Optional ByVal noStart As Boolean = False)
        Dim m As New ListViewItem
        Dim startTim, StopTim As DateTime
        Dim TagPer As TimeSpan
        Dim t As LogEntry

        If noStart = True And noStop = True Then
            Exit Sub
        End If

        'Start Time
        If noStart = True Then
            m.Text = "NO START"
        Else
            t = LogList(startLogListPos)
            m.Text = t.Time.ToString("dd/MM/yyyy HH:mm:ss")
        End If

        'Stop Time
        If noStop = True Then
            m.SubItems.Add("NO STOP")
        Else
            m.SubItems.Add(n.Time.ToString("dd/MM/yyyy HH:mm:ss"))
        End If

        'Period
        If noStart = True Or noStop = True Then
            m.SubItems.Add("??:??:??")
        Else
            startTim = DateTime.Parse(m.SubItems(0).Text)
            StopTim = DateTime.Parse(m.SubItems(1).Text)
            TagPer = StopTim - startTim
            m.SubItems.Add(TagPer.ToString())
        End If

        'ID
        If noStop = True Then
            m.SubItems.Add(t.ID.ToString("X8"))
        Else 'If noStart = True Then
            m.SubItems.Add(n.ID.ToString("X8"))
        End If

        'StartAdd
        If noStart = True Then
            m.SubItems.Add("N/A")
        Else
            m.SubItems.Add(t.Address)
        End If

        'StopAdd
        If noStop = True Then
            m.SubItems.Add("N/A")
        Else
            m.SubItems.Add(n.Address)
        End If


        lvTimeline.Items.Add(m)

    End Sub

    Private Function AddToActList(ByRef ActList As List(Of UInt32), ByRef AddList As List(Of UInt32), ByVal ID As UInt32, ByVal Add As UInt32) As Boolean

        If ActList.Contains(ID) Then
            Return False
        Else
            ActList.Add(ID)
            AddList.Add(Add)
            Return True
        End If

    End Function

    Private Sub btnInterpretAppend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnInterpretAppend.Click
        InterpretTextAsList(rtbInterpretText.Text, True)
    End Sub

    Private Sub SetupColumnSorter(ByRef ColSorter As ListViewColumnSorter, ByRef lvCrtl As ListView, Optional ByVal sortCol As Integer = 0, Optional ByVal Order As SortOrder = SortOrder.Ascending)
        ColSorter = New ListViewColumnSorter()
        lvCrtl.ListViewItemSorter = ColSorter
        ColSorter.SortColumn = sortCol
        ColSorter.Order = Order
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        SetupColumnSorter(lvTimelineColSort, lvTimeline)
        SetupColumnSorter(lvLogsColSort, lvLogs)
        SetupColumnSorter(lvSysEventsColSort, lvSysEvents)
    End Sub

    Private Sub lvTimeline_ColumnClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles lvTimeline.ColumnClick
        ColumnSort(lvTimelineColSort, lvTimeline, e)
    End Sub

    Private Sub lvSysEvents_ColumnClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles lvSysEvents.ColumnClick
        ColumnSort(lvSysEventsColSort, lvSysEvents, e)
    End Sub

    Private Sub lvLogs_ColumnClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.ColumnClickEventArgs) Handles lvLogs.ColumnClick
        ColumnSort(lvLogsColSort, lvLogs, e)
    End Sub

    Private Sub ColumnSort(ByRef ColSorter As ListViewColumnSorter, ByRef lvCrtl As ListView, ByVal e As System.Windows.Forms.ColumnClickEventArgs)
        ' Determine if the clicked column is already the column that is 
        ' being sorted.
        If (e.Column = ColSorter.SortColumn) Then
            ' Reverse the current sort direction for this column.
            If (ColSorter.Order = SortOrder.Ascending) Then
                ColSorter.Order = SortOrder.Descending
            Else
                ColSorter.Order = SortOrder.Ascending
            End If
        Else
            ' Set the column number that is to be sorted; default to ascending.
            ColSorter.SortColumn = e.Column
            ColSorter.Order = SortOrder.Ascending
        End If

        ' Perform the sort with these new sort options.
        lvCrtl.Sort()
    End Sub

    Private Sub btnSaveLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSaveLog.Click
        If lvTimeline.Items.Count > 0 Then
            saveDialog.InitialDirectory = System.IO.Path.GetDirectoryName(My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\WildSpyApps\" & Application.ProductName, "SaveDir", My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\aaa.zip"))

            If saveDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then

                My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\WildSpyApps\" & Application.ProductName, "SaveDir", saveDialog.FileName, Microsoft.Win32.RegistryValueKind.String)

                SaveLogFile(saveDialog.FileName)
            End If
        End If
    End Sub

    Public Sub SaveLogFile(fileName As String)
        Dim outFile As String = ""
        Dim tmpLine As String

        If lvTimeline.Items.Count > 0 Then

            For Each n In LogList
                tmpLine = CType(n.Address, UInt32).ToString("X8") & "        "
                tmpLine &= CType(n.Flags, UInt16).ToString("X2") & "      "
                tmpLine &= CType(n.ID, UInt32).ToString("X8") & "        "
                tmpLine &= n.Time.ToString("dd-MM-yy HH:mm:ss") & vbCrLf

                outFile &= tmpLine
            Next

            My.Computer.FileSystem.WriteAllText(fileName, outFile, False)
        End If
    End Sub

    Private Sub btnLoadLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLoadLog.Click
        OpenDialog.InitialDirectory = System.IO.Path.GetDirectoryName(My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\WildSpyApps\" & Application.ProductName, "LoadDir", My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\aaa.zip"))
        If OpenDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
            My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\WildSpyApps\" & Application.ProductName, "LoadDir", OpenDialog.FileName, Microsoft.Win32.RegistryValueKind.String)
            If MsgBox("Are you sure you want to load this file?  You will loose all unsaved data!!!", vbOKCancel + vbCritical, "Warning!") = vbOK Then
                Dim tmpStr As String
                tmpStr = My.Computer.FileSystem.ReadAllText(OpenDialog.FileName)
                InterpretTextAsList(tmpStr, False)
            End If
        End If
    End Sub

    Private Sub btnClearLog_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnClearLog.Click
        If MsgBox("Are you sure you want to clear all data?", vbOKCancel + vbCritical, "Warning!") = vbOK Then
            lvLogs.Items.Clear()
            lvSysEvents.Items.Clear()
            lvTimeline.Items.Clear()
            LogList.Clear()
        End If

    End Sub

    Private Sub btnSaveTimelines_Click(sender As System.Object, e As System.EventArgs) Handles btnSaveTimelines.Click
        If lvLogs.Items.Count > 0 Then
            SaveDialog1.InitialDirectory = System.IO.Path.GetDirectoryName(My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\WildSpyApps\" & Application.ProductName, "SaveDir", My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\aaa.zip"))

            If SaveDialog1.ShowDialog() = Windows.Forms.DialogResult.OK Then

                My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\WildSpyApps\" & Application.ProductName, "SaveDir", SaveDialog1.FileName, Microsoft.Win32.RegistryValueKind.String)

                SaveTimelines(SaveDialog1.FileName)
            End If
        End If
    End Sub

    Public Sub SaveTimelines(fileName As String)
        Dim outFile As String = "Start Time, Stop Time, Period, ID, Start Address, Stop Address" & vbNewLine
        Dim tmpLine As String

        If lvLogs.Items.Count > 0 Then
            Dim n As ListViewItem
            For Each n In lvTimeline.Items
                tmpLine = n.SubItems(0).Text & "," & n.SubItems(1).Text & "," & n.SubItems(2).Text & "," & n.SubItems(3).Text & "," & n.SubItems(4).Text & "," & n.SubItems(5).Text & vbNewLine

                outFile &= tmpLine
            Next

            My.Computer.FileSystem.WriteAllText(filename, outFile, False)
        End If
    End Sub

    Private Sub btnSaveAll_Click(sender As System.Object, e As System.EventArgs) Handles btnSaveAll.Click
        If lvLogs.Items.Count > 0 Then
            SaveDialog2.InitialDirectory = System.IO.Path.GetDirectoryName(My.Computer.Registry.GetValue("HKEY_CURRENT_USER\Software\WildSpyApps\" & Application.ProductName, "SaveDir", My.Computer.FileSystem.SpecialDirectories.MyDocuments & "\aaa.zip"))

            If SaveDialog2.ShowDialog() = Windows.Forms.DialogResult.OK Then

                My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\WildSpyApps\" & Application.ProductName, "SaveDir", SaveDialog2.FileName, Microsoft.Win32.RegistryValueKind.String)

                SaveTimelines(System.IO.Path.GetDirectoryName(SaveDialog2.FileName) & "\" & System.IO.Path.GetFileNameWithoutExtension(SaveDialog2.FileName) & ".csv")
                SaveLogFile(System.IO.Path.GetDirectoryName(SaveDialog2.FileName) & "\" & System.IO.Path.GetFileNameWithoutExtension(SaveDialog2.FileName) & ".wid")
            End If
        End If
    End Sub
End Class

Public Class LogEntry
    Public Address As UInt32
    Public Flags As Byte
    Public ID As UInt32
    Public Time As DateTime
End Class
