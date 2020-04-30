<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtIN = New System.Windows.Forms.RichTextBox()
        Me.txtOUT = New System.Windows.Forms.RichTextBox()
        Me.OFD1 = New System.Windows.Forms.OpenFileDialog()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnBatchConvert = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.nudTimeCol = New System.Windows.Forms.NumericUpDown()
        Me.nudHDOPNo = New System.Windows.Forms.NumericUpDown()
        Me.nudDateCol = New System.Windows.Forms.NumericUpDown()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtSepChar = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.nudCols = New System.Windows.Forms.NumericUpDown()
        CType(Me.nudTimeCol, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudHDOPNo, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudDateCol, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudCols, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(212, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(317, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Picks fix with best HDOP out of several grouped fixes in a text file."
        Me.Label1.Visible = False
        '
        'txtIN
        '
        Me.txtIN.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtIN.Location = New System.Drawing.Point(12, 80)
        Me.txtIN.Name = "txtIN"
        Me.txtIN.Size = New System.Drawing.Size(863, 136)
        Me.txtIN.TabIndex = 1
        Me.txtIN.Text = ""
        '
        'txtOUT
        '
        Me.txtOUT.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtOUT.Location = New System.Drawing.Point(12, 233)
        Me.txtOUT.Name = "txtOUT"
        Me.txtOUT.Size = New System.Drawing.Size(863, 237)
        Me.txtOUT.TabIndex = 3
        Me.txtOUT.Text = "" & Global.Microsoft.VisualBasic.ChrW(10)
        '
        'OFD1
        '
        Me.OFD1.Filter = "Comma Seperated List Files (*.csv)|*.csv|All Files (*.*)|*.*"
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(12, 33)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 4
        Me.Button1.Text = "Open File"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(12, 64)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(34, 13)
        Me.Label2.TabIndex = 5
        Me.Label2.Text = "Input:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 217)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(42, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Output:"
        '
        'btnBatchConvert
        '
        Me.btnBatchConvert.Location = New System.Drawing.Point(93, 33)
        Me.btnBatchConvert.Name = "btnBatchConvert"
        Me.btnBatchConvert.Size = New System.Drawing.Size(75, 23)
        Me.btnBatchConvert.TabIndex = 7
        Me.btnBatchConvert.Text = "Batch Convert"
        Me.btnBatchConvert.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(212, 30)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(207, 26)
        Me.Label4.TabIndex = 8
        Me.Label4.Text = "Input line should be of the form:" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "DATE,TIME,WID_ID,OTHER_COLUMNS"
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(174, 33)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(16, 23)
        Me.Button2.TabIndex = 9
        Me.Button2.Text = "?"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'nudTimeCol
        '
        Me.nudTimeCol.Location = New System.Drawing.Point(694, 36)
        Me.nudTimeCol.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.nudTimeCol.Name = "nudTimeCol"
        Me.nudTimeCol.Size = New System.Drawing.Size(46, 20)
        Me.nudTimeCol.TabIndex = 10
        Me.nudTimeCol.Value = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudTimeCol.Visible = False
        '
        'nudHDOPNo
        '
        Me.nudHDOPNo.Location = New System.Drawing.Point(798, 36)
        Me.nudHDOPNo.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.nudHDOPNo.Name = "nudHDOPNo"
        Me.nudHDOPNo.Size = New System.Drawing.Size(46, 20)
        Me.nudHDOPNo.TabIndex = 11
        Me.nudHDOPNo.Value = New Decimal(New Integer() {6, 0, 0, 0})
        Me.nudHDOPNo.Visible = False
        '
        'nudDateCol
        '
        Me.nudDateCol.Location = New System.Drawing.Point(746, 36)
        Me.nudDateCol.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.nudDateCol.Name = "nudDateCol"
        Me.nudDateCol.Size = New System.Drawing.Size(46, 20)
        Me.nudDateCol.TabIndex = 12
        Me.nudDateCol.Value = New Decimal(New Integer() {2, 0, 0, 0})
        Me.nudDateCol.Visible = False
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(795, 20)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(38, 13)
        Me.Label5.TabIndex = 13
        Me.Label5.Text = "HDOP"
        Me.Label5.Visible = False
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(743, 20)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(30, 13)
        Me.Label6.TabIndex = 14
        Me.Label6.Text = "Date"
        Me.Label6.Visible = False
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(691, 20)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(30, 13)
        Me.Label7.TabIndex = 15
        Me.Label7.Text = "Time"
        Me.Label7.Visible = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(719, 5)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(87, 13)
        Me.Label8.TabIndex = 16
        Me.Label8.Text = "Column Numbers"
        Me.Label8.Visible = False
        '
        'txtSepChar
        '
        Me.txtSepChar.Location = New System.Drawing.Point(574, 20)
        Me.txtSepChar.Name = "txtSepChar"
        Me.txtSepChar.Size = New System.Drawing.Size(55, 20)
        Me.txtSepChar.TabIndex = 17
        Me.txtSepChar.Text = "[tab]"
        Me.txtSepChar.Visible = False
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(561, 4)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(83, 13)
        Me.Label9.TabIndex = 18
        Me.Label9.Text = "Seperation Char"
        Me.Label9.Visible = False
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(561, 38)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(79, 13)
        Me.Label10.TabIndex = 20
        Me.Label10.Text = "No. of Columns"
        Me.Label10.Visible = False
        '
        'nudCols
        '
        Me.nudCols.Location = New System.Drawing.Point(574, 54)
        Me.nudCols.Maximum = New Decimal(New Integer() {10000, 0, 0, 0})
        Me.nudCols.Name = "nudCols"
        Me.nudCols.Size = New System.Drawing.Size(55, 20)
        Me.nudCols.TabIndex = 19
        Me.nudCols.Value = New Decimal(New Integer() {7, 0, 0, 0})
        Me.nudCols.Visible = False
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(887, 482)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.nudCols)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.txtSepChar)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.nudDateCol)
        Me.Controls.Add(Me.nudHDOPNo)
        Me.Controls.Add(Me.nudTimeCol)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.btnBatchConvert)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.txtOUT)
        Me.Controls.Add(Me.txtIN)
        Me.Controls.Add(Me.Label1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form1"
        Me.Text = "Fix Parser"
        CType(Me.nudTimeCol, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudHDOPNo, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudDateCol, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudCols, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents txtIN As System.Windows.Forms.RichTextBox
    Friend WithEvents txtOUT As System.Windows.Forms.RichTextBox
    Friend WithEvents OFD1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents btnBatchConvert As System.Windows.Forms.Button
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents nudTimeCol As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudHDOPNo As System.Windows.Forms.NumericUpDown
    Friend WithEvents nudDateCol As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents txtSepChar As System.Windows.Forms.TextBox
    Friend WithEvents Label9 As System.Windows.Forms.Label
    Friend WithEvents Label10 As System.Windows.Forms.Label
    Friend WithEvents nudCols As System.Windows.Forms.NumericUpDown

End Class
