Imports System
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.IO
'// http://www.daniweb.com/techtalkforums/thread35078.html
Namespace TGREER
    <Serializable()> _
    Public Class myStreamReader
        Inherits TextReader
        Public Shared Shadows ReadOnly Null As myStreamReader = New NullmyStreamReader()
        'Public Shared Null As myStreamReader = New NullmyStreamReader()
        Friend Const DefaultBufferSize As Integer = 1024 ' Byte buffer size
        Private Const DefaultFileStreamBufferSize As Integer = 4096
        Private Const MinBufferSize As Integer = 128
        Private stream As Stream
        Private encoding As Encoding
        Private decoder As Decoder
        Private byteBuffer() As Byte
        Private charBuffer() As Char
        Private _preamble() As Byte
        Private charPos As Integer
        Private charLen As Integer
        Private byteLen As Integer
        Private _maxCharsPerBuffer As Integer
        Private _detectEncoding As Boolean
        Private _checkPreamble As Boolean
        Private _isBlocked As Boolean
        Private _lineLength As Integer
        Public ReadOnly Property LineLength() As Integer
            Get
                Return _lineLength
            End Get
        End Property
        Private _bytesRead As Integer
        Public ReadOnly Property BytesRead() As Integer
            Get
                Return _bytesRead
            End Get
        End Property
        Friend Sub New()
        End Sub 'New

        Public Sub New(ByVal stream As Stream)
            MyClass.New(stream, Encoding.UTF8, True, DefaultBufferSize)
        End Sub 'New

        Public Sub New(ByVal stream As Stream, ByVal detectEncodingFromByteOrderMarks As Boolean)
            MyClass.New(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks, DefaultBufferSize)
        End Sub 'New

        Public Sub New(ByVal stream As Stream, ByVal encoding As Encoding)
            MyClass.New(stream, encoding, True, DefaultBufferSize)
        End Sub 'New

        Public Sub New(ByVal stream As Stream, ByVal encoding As Encoding, ByVal detectEncodingFromByteOrderMarks As Boolean)
            MyClass.New(stream, encoding, detectEncodingFromByteOrderMarks, DefaultBufferSize)
        End Sub 'New

        Public Sub New(ByVal stream As Stream, ByVal encoding As Encoding, ByVal detectEncodingFromByteOrderMarks As Boolean, ByVal bufferSize As Integer)
            If stream Is Nothing OrElse encoding Is Nothing Then
                Throw New ArgumentNullException(IIf(stream Is Nothing, "stream", "encoding"))
            End If
            If Not stream.CanRead Then
                Throw New ArgumentException(Environment.GetEnvironmentVariable("Argument_StreamNotReadable"))
            End If
            If bufferSize <= 0 Then
                Throw New ArgumentOutOfRangeException("bufferSize", Environment.GetEnvironmentVariable("ArgumentOutOfRange_NeedPosNum"))
            End If
            Init(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        End Sub 'New

        '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.myStreamReader4"]/*' />
        Public Sub New(ByVal path As String)
            MyClass.New(path, Encoding.UTF8, True, DefaultBufferSize)
        End Sub 'New

        '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.myStreamReader9"]/*' />
        Public Sub New(ByVal path As String, ByVal detectEncodingFromByteOrderMarks As Boolean)
            MyClass.New(path, Encoding.UTF8, detectEncodingFromByteOrderMarks, DefaultBufferSize)
        End Sub 'New

        '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.myStreamReader5"]/*' />
        Public Sub New(ByVal path As String, ByVal encoding As Encoding)
            MyClass.New(path, encoding, True, DefaultBufferSize)
        End Sub 'New

        '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.myStreamReader6"]/*' />
        Public Sub New(ByVal path As String, ByVal encoding As Encoding, ByVal detectEncodingFromByteOrderMarks As Boolean)
            MyClass.New(path, encoding, detectEncodingFromByteOrderMarks, DefaultBufferSize)
        End Sub 'New

        '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.myStreamReader7"]/*' />
        Public Sub New(ByVal path As String, ByVal encoding As Encoding, ByVal detectEncodingFromByteOrderMarks As Boolean, ByVal bufferSize As Integer)
            ' Don't open a Stream before checking for invalid arguments,
            ' or we'll create a FileStream on disk and we won't close it until
            ' the finalizer runs, causing problems for applications.
            If path Is Nothing OrElse encoding Is Nothing Then
                Throw New ArgumentNullException(IIf(path Is Nothing, "path", "encoding"))
            End If
            If path.Length = 0 Then
                Throw New ArgumentException(Environment.GetEnvironmentVariable("Argument_EmptyPath"))
            End If
            If bufferSize <= 0 Then
                Throw New ArgumentOutOfRangeException("bufferSize", Environment.GetEnvironmentVariable("ArgumentOutOfRange_NeedPosNum"))
            End If
            Dim stream As FileStream = New FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, DefaultFileStreamBufferSize)
            Init(stream, encoding, detectEncodingFromByteOrderMarks, bufferSize)
        End Sub 'New

        Private Sub Init(ByVal stream As Stream, ByVal encoding As Encoding, ByVal detectEncodingFromByteOrderMarks As Boolean, ByVal bufferSize As Integer)
            Me.stream = stream
            Me.encoding = encoding
            decoder = encoding.GetDecoder()
            If bufferSize < MinBufferSize Then
                bufferSize = MinBufferSize
            End If
            byteBuffer = New Byte(bufferSize) {}
            _maxCharsPerBuffer = encoding.GetMaxCharCount(bufferSize)
            charBuffer = New Char(_maxCharsPerBuffer) {}
            byteLen = 0
            _detectEncoding = detectEncodingFromByteOrderMarks
            _preamble = encoding.GetPreamble()
            _checkPreamble = _preamble.Length > 0
            _isBlocked = False
        End Sub 'Init

        '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.Close"]/*' />
        Public Overrides Sub Close()
            Dispose(True)
        End Sub 'Close

        '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.Dispose"]/*' />
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            If disposing Then
                If Not (stream Is Nothing) Then
                    stream.Close()
                End If
            End If
            If Not (stream Is Nothing) Then
                stream = Nothing
                encoding = Nothing
                decoder = Nothing
                byteBuffer = Nothing
                charBuffer = Nothing
                charPos = 0
                charLen = 0
            End If
            MyBase.Dispose(disposing)
        End Sub 'Dispose
        '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.CurrentEncoding"]/*' />
        Public Overridable ReadOnly Property CurrentEncoding() As Encoding
            Get
                Return encoding
            End Get
        End Property
        '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.BaseStream"]/*' />
        Public Overridable ReadOnly Property BaseStream() As Stream
            Get
                Return stream
            End Get
        End Property
        ' DiscardBufferedData tells myStreamReader to throw away its internal
        ' buffer contents. This is useful if the user needs to seek on the
        ' underlying stream to a known location then wants the myStreamReader
        ' to start reading from this new point. This method should be called
        ' very sparingly, if ever, since it can lead to very poor performance.
        ' However, it may be the only way of handling some scenarios where 
        ' users need to re-read the contents of a myStreamReader a second time.
        '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.DiscardBufferedData"]/*' />
        Public Sub DiscardBufferedData()
            byteLen = 0
            charLen = 0
            charPos = 0
            decoder = encoding.GetDecoder()
            _isBlocked = False
        End Sub 'DiscardBufferedData

        '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.Peek"]/*' />
        Public Overrides Function Peek() As Integer
            'if (stream == null)
            '__Error.ReaderClosed();
            If charPos = charLen Then
                If _isBlocked OrElse ReadBuffer() = 0 Then
                    Return -1
                End If
            End If
            'Return charBuffer(charPos)
            Return AscW(charBuffer(charPos))
        End Function 'Peek

        Public Overloads Overrides Function Read() As Integer
            'if (stream == null)
            '__Error.ReaderClosed();
            If charPos = charLen Then
                If ReadBuffer() = 0 Then
                    Return -1
                End If
            End If

            Dim retV As Integer = AscW(charBuffer(charPos)) 'AscW(charBuffer(charPos + 1))

            _bytesRead += 1
            charPos += 1

            Return retV
            'Return charBuffer(charPos + 1) 
        End Function 'Read


        'Public override Integer Read((In, Out) Char() buffer, Integer index, Integer count)
        Public Overloads Overrides Function Read(<[In](), Out()> ByVal buffer() As Char, ByVal index As Integer, ByVal count As Integer) As Integer
            'if (stream == null)
            '__Error.ReaderClosed();
            If buffer Is Nothing Then
                Throw New ArgumentNullException("buffer", Environment.GetEnvironmentVariable("ArgumentNull_Buffer"))
            End If
            If index < 0 OrElse count < 0 Then
                Throw New ArgumentOutOfRangeException(IIf(index < 0, "index", "count"), Environment.GetEnvironmentVariable("ArgumentOutOfRange_NeedNonNegNum"))
            End If
            If buffer.Length - index < count Then
                Throw New ArgumentException(Environment.GetEnvironmentVariable("Argument_InvalidOffLen"))
            End If
            Dim charsRead As Integer = 0
            ' As a perf optimization, if we had exactly one buffer's worth of 
            ' data read in, let's try writing directly to the user's buffer.
            Dim readToUserBuffer As Boolean = False
            While count > 0
                Dim n As Integer = charLen - charPos
                If n = 0 Then
                    n = ReadBuffer(buffer, index + charsRead, count, readToUserBuffer)
                End If
                If n = 0 Then
                    Exit While ' We're at EOF
                End If
                If n > count Then
                    n = count
                End If
                If Not readToUserBuffer Then
                    System.Buffer.BlockCopy(charBuffer, charPos * 2, buffer, (index + charsRead) * 2, n * 2)
                    'buffer.BlockCopy(charBuffer, charPos * 2, buffer, (index + charsRead) * 2, n * 2)
                    charPos += n
                End If
                charsRead += n
                count -= n
                ' This function shouldn't block for an indefinite amount of time,
                ' or reading from a network stream won't work right. If we got
                ' fewer bytes than we requested, then we want to break right here.
                If _isBlocked Then
                    Exit While
                End If
            End While
            Return charsRead
        End Function 'Read

        '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.ReadToEnd"]/*' />
        Public Overrides Function ReadToEnd() As String
            'if (stream == null)
            '__Error.ReaderClosed();
            ' For performance, call Read(char[], int, int) with a buffer
            ' as big as the myStreamReader's internal buffer, to get the 
            ' readToUserBuffer optimization.
            Dim chars(charBuffer.Length) As Char
            Dim len As Integer
            Dim sb As New StringBuilder(charBuffer.Length)
            While (len = Read(chars, 0, chars.Length)) <> 0
                sb.Append(chars, 0, len)
            End While
            Return sb.ToString()
        End Function 'ReadToEnd

        ' Trims n bytes from the front of the buffer.
        Private Sub CompressBuffer(ByVal n As Integer)
            Buffer.BlockCopy(byteBuffer, n, byteBuffer, 0, byteLen - n)
            byteLen -= n
        End Sub 'CompressBuffer

        ' returns whether the first array starts with the second array.
        Private Shared Function BytesMatch(ByVal buffer() As Byte, ByVal compareTo() As Byte) As Boolean
            Dim i As Integer
            For i = 0 To compareTo.Length - 1
                If buffer(i) <> compareTo(i) Then
                    Return False
                End If
            Next i
            Return True
        End Function 'BytesMatch

        Private Sub DetectEncoding()
            If byteLen < 2 Then
                Return
            End If
            _detectEncoding = False
            Dim changedEncoding As Boolean = False
            If byteBuffer(0) = &HFE AndAlso byteBuffer(1) = &HFF Then
                ' Big Endian Unicode
                encoding = New UnicodeEncoding(True, True)
                decoder = encoding.GetDecoder()
                CompressBuffer(2)
                changedEncoding = True
            ElseIf byteBuffer(0) = &HFF AndAlso byteBuffer(1) = &HFE Then
                ' Little Endian Unicode
                encoding = New UnicodeEncoding(False, True)
                decoder = encoding.GetDecoder()
                CompressBuffer(2)
                changedEncoding = True
            ElseIf byteLen >= 3 AndAlso byteBuffer(0) = &HEF AndAlso byteBuffer(1) = &HBB AndAlso byteBuffer(2) = &HBF Then
                ' UTF-8
                encoding = Text.Encoding.UTF8
                decoder = encoding.GetDecoder()
                CompressBuffer(3)
                changedEncoding = True
            ElseIf byteLen = 2 Then
                _detectEncoding = True
            End If ' Note: in the future, if we change this algorithm significantly,
            ' we can support checking for the preamble of the given encoding.
            If changedEncoding Then
                _maxCharsPerBuffer = encoding.GetMaxCharCount(byteBuffer.Length)
                charBuffer = New Char(_maxCharsPerBuffer) {}
            End If
        End Sub 'DetectEncoding


        Private Overloads Function ReadBuffer() As Integer
            charLen = 0
            byteLen = 0
            charPos = 0
            Do
                byteLen = stream.Read(byteBuffer, 0, byteBuffer.Length)
                If byteLen = 0 Then ' We're at EOF
                    Return charLen
                End If
                ' _isBlocked == whether we read fewer bytes than we asked for.
                ' Note we must check it here because CompressBuffer or 
                ' DetectEncoding will screw with byteLen.
                _isBlocked = byteLen < byteBuffer.Length
                If _checkPreamble AndAlso byteLen >= _preamble.Length Then
                    _checkPreamble = False
                    If BytesMatch(byteBuffer, _preamble) Then
                        _detectEncoding = False
                        CompressBuffer(_preamble.Length)
                    End If
                End If
                ' If we're supposed to detect the encoding and haven't done so yet,
                ' do it. Note this may need to be called more than once.
                If _detectEncoding AndAlso byteLen >= 2 Then
                    DetectEncoding()
                End If
                charLen += decoder.GetChars(byteBuffer, 0, byteLen, charBuffer, charLen)
            Loop While charLen = 0
            'Console.WriteLine("ReadBuffer called. chars: "+charLen);
            Return charLen
        End Function 'ReadBuffer


        ' This version has a perf optimization to decode data DIRECTLY into the 
        ' user's buffer, bypassing StreamWriter's own buffer.
        ' This gives a > 20% perf improvement for our encodings across the board,
        ' but only when asking for at least the number of characters that one
        ' buffer's worth of bytes could produce.
        ' This optimization, if run, will break SwitchEncoding, so we must not do 
        ' this on the first call to ReadBuffer. 
        Private Overloads Function ReadBuffer(ByVal userBuffer() As Char, ByVal userOffset As Integer, ByVal desiredChars As Integer, ByRef readToUserBuffer As Boolean) As Integer
            charLen = 0
            byteLen = 0
            charPos = 0
            Dim charsRead As Integer = 0
            ' As a perf optimization, we can decode characters DIRECTLY into a
            ' user's char[]. We absolutely must not write more characters 
            ' into the user's buffer than they asked for. Calculating 
            ' encoding.GetMaxCharCount(byteLen) each time is potentially very 
            ' expensive - instead, cache the number of chars a full buffer's 
            ' worth of data may produce. Yes, this makes the perf optimization 
            ' less aggressive, in that all reads that asked for fewer than AND 
            ' returned fewer than _maxCharsPerBuffer chars won't get the user 
            ' buffer optimization. This affects reads where the end of the
            ' Stream comes in the middle somewhere, and when you ask for 
            ' fewer chars than than your buffer could produce.
            readToUserBuffer = desiredChars >= _maxCharsPerBuffer
            Do
                byteLen = stream.Read(byteBuffer, 0, byteBuffer.Length)
                If byteLen = 0 Then ' EOF
                    Return charsRead
                End If
                ' _isBlocked == whether we read fewer bytes than we asked for.
                ' Note we must check it here because CompressBuffer or 
                ' DetectEncoding will screw with byteLen.
                _isBlocked = byteLen < byteBuffer.Length
                ' On the first call to ReadBuffer, if we're supposed to detect the encoding, do it.
                If _detectEncoding AndAlso byteLen >= 2 Then
                    DetectEncoding()
                    ' DetectEncoding changes some buffer state. Recompute this.
                    readToUserBuffer = desiredChars >= _maxCharsPerBuffer
                End If
                If _checkPreamble AndAlso byteLen >= _preamble.Length Then
                    _checkPreamble = False
                    If BytesMatch(byteBuffer, _preamble) Then
                        _detectEncoding = False
                        CompressBuffer(_preamble.Length)
                        ' CompressBuffer changes some buffer state. Recompute this.
                        readToUserBuffer = desiredChars >= _maxCharsPerBuffer
                    End If
                End If
                '
                ' if (readToUserBuffer)
                ' Console.Write('.');
                ' else {
                ' Console.WriteLine("Desired chars is wrong. byteBuffer.length: "+byteBuffer.Length+" max chars is: "+encoding.GetMaxCharCount(byteLen)+" desired: "+desiredChars);
                ' }
                ' 
                charPos = 0
                If readToUserBuffer Then
                    charsRead += decoder.GetChars(byteBuffer, 0, byteLen, userBuffer, userOffset + charsRead)
                    charLen = 0 ' myStreamReader's buffer is empty.
                Else
                    charsRead = decoder.GetChars(byteBuffer, 0, byteLen, charBuffer, charsRead)
                    charLen += charsRead ' Number of chars in myStreamReader's buffer.
                End If
            Loop While charsRead = 0
            'Console.WriteLine("ReadBuffer: charsRead: "+charsRead+" readToUserBuffer: "+readToUserBuffer);
            Return charsRead
        End Function 'ReadBuffer


        ' Reads a line. A line is defined as a sequence of characters followed by
        ' a carriage return ('\r'), a line feed ('\n'), or a carriage return
        ' immediately followed by a line feed. The resulting string does not
        ' contain the terminating carriage return and/or line feed. The returned
        ' value is null if the end of the input stream has been reached.
        '
        '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.ReadLine"]/*' />
        Public Overrides Function ReadLine() As String
            _lineLength = 0
            'if (stream == null)
            ' __Error.ReaderClosed();
            If charPos = charLen Then
                If ReadBuffer() = 0 Then
                    Return Nothing
                End If
            End If
            Dim sb As StringBuilder = Nothing
            Do
                Dim i As Integer = charPos
                Do
                    Dim ch As Char = charBuffer(i)
                    Dim EolChars As Integer = 0
                    If ch = ControlChars.Cr OrElse ch = ControlChars.Lf Then
                        EolChars = 1
                        Dim s As String
                        If Not (sb Is Nothing) Then
                            sb.Append(charBuffer, charPos, i - charPos)
                            s = sb.ToString()
                        Else
                            s = New String(charBuffer, charPos, i - charPos)
                        End If
                        charPos = i + 1
                        If ch = ControlChars.Cr AndAlso (charPos < charLen OrElse ReadBuffer() > 0) Then
                            If charBuffer(charPos) = ControlChars.Lf Then
                                charPos += 1
                                EolChars = 2
                            End If
                        End If
                        _lineLength = s.Length + EolChars
                        _bytesRead = _bytesRead + _lineLength
                        Return s
                    End If
                    i += 1
                Loop While i < charLen
                i = charLen - charPos
                If sb Is Nothing Then
                    sb = New StringBuilder(i + 80)
                End If
                sb.Append(charBuffer, charPos, i)
            Loop While ReadBuffer() > 0
            Dim ss As String = sb.ToString()
            _lineLength = ss.Length
            _bytesRead = _bytesRead + _lineLength
            Return ss
        End Function 'ReadLine
        ' No data, class doesn't need to be serializable.
        ' Note this class is threadsafe.
        Private Class NullmyStreamReader
            Inherits myStreamReader
            Public Overrides ReadOnly Property BaseStream() As Stream
                Get
                    Return stream.Null
                End Get
            End Property
            Public Overrides ReadOnly Property CurrentEncoding() As Encoding
                Get
                    Return Text.Encoding.Unicode
                End Get
            End Property
            Public Overrides Function Peek() As Integer
                Return -1
            End Function 'Peek

            Public Overloads Overrides Function Read() As Integer
                Return -1
            End Function 'Read

            '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.NullmyStreamReader.Read"]/*' />
            Public Overloads Overrides Function Read(ByVal buffer() As Char, ByVal index As Integer, ByVal count As Integer) As Integer
                Return 0
            End Function 'Read

            '/ <include file='doc\myStreamReader.uex' path='docs/doc[@for="myStreamReader.NullmyStreamReader.ReadLine"]/*' />
            Public Overrides Function ReadLine() As String
                Return Nothing
            End Function 'ReadLine

            Public Overrides Function ReadToEnd() As String
                Return String.Empty
            End Function 'ReadToEnd
        End Class 'NullmyStreamReader
    End Class 'myStreamReader
End Namespace 'TGREER