Public Class ProgTable
    Private mColNames As List(Of String)
    Private mTable As List(Of List(Of String))
    Private mCurrentRow As List(Of String)
    Private mFilename As String
    Private mColSeperator As String = ","
    Private mRowSeperator As String = vbCrLf


    Public Sub New(rowNames As List(Of String))
        mColNames = New List(Of String)(rowNames)
        mTable = New List(Of List(Of String))
    End Sub

    'Adds a new row, sets the current row to this one
    Public Sub addNewRow()
        mCurrentRow = New List(Of String)
        mTable.Add(mCurrentRow)
        For Each col As String In mColNames
            mCurrentRow.Add("")
        Next
    End Sub

    Public Sub updateInCurrentRow(colName As String, value As String)
        Dim index As Integer = mColNames.IndexOf(colName)
        If index = -1 Then
            Throw New Exception("Column not found")
        End If
        mCurrentRow(index) = value.Replace(vbCr, "").Replace(vbLf, "").Replace(colSeperator, "$COLSEP$").Replace(rowSeperator, "$ROWSEP$")
    End Sub

    Public Sub setCurrentRow(index As Integer)
        If index >= Me.count Then
            Throw New Exception("Row out of range")
        End If
        mCurrentRow = mTable(index)
    End Sub

    Public ReadOnly Property count
        Get
            Return mTable.Count
        End Get
    End Property

    Public Property filename
        Get
            Return mFilename
        End Get
        Set(value)
            mFilename = value
        End Set
    End Property
    Public Property colSeperator
        Get
            Return mColSeperator
        End Get
        Set(value)
            mColSeperator = value
        End Set
    End Property

    Public Property rowSeperator
        Get
            Return mRowSeperator
        End Get
        Set(value)
            mRowSeperator = value
        End Set
    End Property

    'Public Overrides Function toString() As String

    'End Function

    Private Function getCsvString() As String
        Dim retStr As String = "index" + colSeperator
        For Each col In mColNames
            retStr += col + colSeperator
        Next
        retStr = retStr.Substring(0, retStr.Length - colSeperator.Length) 'remove last col seperator
        retStr += rowSeperator

        Dim i As Integer = 0
        For Each row In mTable
            i += 1
            retStr += CStr(i) + colSeperator
            For Each col In row
                retStr += col + colSeperator
            Next
            retStr = retStr.Substring(0, retStr.Length - colSeperator.Length) 'remove last col seperator
            retStr += rowSeperator
        Next
        retStr = retStr.Substring(0, retStr.Length - rowSeperator.Length) 'remove last row seperator

        Return retStr
    End Function

    Public Sub saveAsCsv(Optional overwrite As Boolean = False)
        saveAsCsv(mFilename, overwrite)
    End Sub

    Public Sub saveAsCsv(filename As String, Optional overwrite As Boolean = False)
        If My.Computer.FileSystem.FileExists(filename) And overwrite = False Then
            Throw New Exception("File already exists")
        End If
        My.Computer.FileSystem.WriteAllText(filename, getCsvString, False)
    End Sub
End Class
