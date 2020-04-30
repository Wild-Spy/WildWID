Module RunProcess
#Region " Run Process Function "

    ' [ Run Process Function ]
    '
    ' // By Elektro H@cker
    '
    ' Examples :
    '
    ' MsgBox(Run_Process("Process.exe")) 
    ' MsgBox(Run_Process("Process.exe", "Arguments"))
    ' MsgBox(Run_Process("CMD.exe", "/C Dir /B", True))
    ' MsgBox(Run_Process("CMD.exe", "/C @Echo OFF & For /L %X in (0,1,50000) Do (Echo %X)", False, False))
    ' MsgBox(Run_Process("CMD.exe", "/C Dir /B /S %SYSTEMDRIVE%\*", , False, 500))
    ' If Run_Process("CMD.exe", "/C Dir /B", True).Contains("File.txt") Then MsgBox("File found")

    Public Function Run_Process(ByVal Process_Name As String, Optional Process_Arguments As String = Nothing, Optional Read_Output As Boolean = False, Optional Process_Hide As Boolean = False, Optional Process_TimeOut As Integer = 999999999)

        ' Returns True if "Read_Output" argument is False and Process was finished OK
        ' Returns False if ExitCode is not "0"
        ' Returns Nothing if process can't be found or can't be started
        ' Returns "ErrorOutput" or "StandardOutput" (In that priority) if Read_Output argument is set to True.

        Try

            Dim My_Process As New Process()
            Dim My_Process_Info As New ProcessStartInfo()
            Dim stdout As String = ""
            Dim stderr As String = ""

            My_Process_Info.FileName = Process_Name ' Process filename
            My_Process_Info.Arguments = Process_Arguments ' Process arguments
            My_Process_Info.CreateNoWindow = Process_Hide ' Show or hide the process Window
            My_Process_Info.UseShellExecute = False ' Don't use system shell to execute the process
            My_Process_Info.RedirectStandardOutput = Read_Output '  Redirect (1) Output
            My_Process_Info.RedirectStandardError = Read_Output ' Redirect non (1) Output
            My_Process.EnableRaisingEvents = True ' Raise events
            My_Process.StartInfo = My_Process_Info
            My_Process.Start() ' Run the process NOW

            'Dim output As String

            'Do While (My_Process.StandardOutput.Peek() > -1)
            '    stdout += My_Process.StandardOutput.ReadLine() + vbCrLf
            'Loop

            'Do While (My_Process.StandardError.Peek() > -1)
            '    stderr += My_Process.StandardError.ReadLine() + vbCrLf
            'Loop

            'If Read_Output Then
            '    Dim line As String
            '    Dim stdout = My_Process.StandardOutput
            '    Dim stderr = My_Process.StandardError
            '    Do
            '        Application.DoEvents()
            '        If stderr.EndOfStream = False Then
            '            line = stderr.ReadLine
            '            output += line + vbCrLf
            '        End If
            '        If stdout.EndOfStream = False Then
            '            line = stdout.ReadLine()
            '            output += line + vbCrLf
            '        End If

            '    Loop While My_Process.HasExited = False
            'End If

            Dim Process_ErrorOutput As String = My_Process.StandardError.ReadToEnd() ' Stores the Error Output (If any)
            Dim Process_StandardOutput As String = My_Process.StandardOutput.ReadToEnd() ' Stores the Standard Output (If any)

            My_Process.WaitForExit(Process_TimeOut) ' Wait X ms to kill the process (Default value is 999999999 ms which is 277 Hours)

            Dim ERRORLEVEL = My_Process.ExitCode ' Stores the ExitCode of the process
            'If Not ERRORLEVEL = 0 Then Return False ' Returns the Exitcode if is not 0

            If Read_Output = True Then
                'Return output
                'Dim Process_ErrorOutput As String = My_Process.StandardError.ReadToEnd() ' Stores the Error Output (If any)
                'Dim Process_StandardOutput As String = My_Process.StandardOutput.ReadToEnd() ' Stores the Standard Output (If any)
                ' Return output by priority
                If Process_ErrorOutput IsNot Nothing Then Return Process_ErrorOutput ' Returns the ErrorOutput (if any)
                If Process_StandardOutput IsNot Nothing Then Return Process_StandardOutput ' Returns the StandardOutput (if any)
            End If

        Catch ex As Exception
            'MsgBox(ex.Message)
            Return Nothing ' Returns nothing if the process can't be found or started.
        End Try

        Return True ' Returns True if Read_Output argument is set to False and the process finished without errors.

    End Function

#End Region
End Module
