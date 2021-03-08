<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmMain
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
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.Button3 = New System.Windows.Forms.Button()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.pbMap = New System.Windows.Forms.PictureBox()
        Me.tbFixTime = New System.Windows.Forms.TrackBar()
        Me.Button4 = New System.Windows.Forms.Button()
        Me.btnPlayMap = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.Button5 = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Button6 = New System.Windows.Forms.Button()
        Me.GroupBox1.SuspendLayout()
        CType(Me.pbMap, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.tbFixTime, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Button1
        '
        Me.Button1.Location = New System.Drawing.Point(40, 22)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(89, 23)
        Me.Button1.TabIndex = 0
        Me.Button1.Text = "Load KML"
        Me.Button1.UseVisualStyleBackColor = True
        Me.Button1.Visible = False
        '
        'Button2
        '
        Me.Button2.Location = New System.Drawing.Point(40, 51)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(89, 23)
        Me.Button2.TabIndex = 1
        Me.Button2.Text = "Load Tag CSV"
        Me.Button2.UseVisualStyleBackColor = True
        Me.Button2.Visible = False
        '
        'Button3
        '
        Me.Button3.Location = New System.Drawing.Point(135, 22)
        Me.Button3.Name = "Button3"
        Me.Button3.Size = New System.Drawing.Size(89, 23)
        Me.Button3.TabIndex = 2
        Me.Button3.Text = "Plot Loggers"
        Me.Button3.UseVisualStyleBackColor = True
        Me.Button3.Visible = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.pbMap)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 80)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(596, 382)
        Me.GroupBox1.TabIndex = 3
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Map View - Click and drag to move, scroll to zoom."
        '
        'pbMap
        '
        Me.pbMap.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pbMap.Location = New System.Drawing.Point(3, 16)
        Me.pbMap.Name = "pbMap"
        Me.pbMap.Size = New System.Drawing.Size(590, 363)
        Me.pbMap.TabIndex = 1
        Me.pbMap.TabStop = False
        '
        'tbFixTime
        '
        Me.tbFixTime.AutoSize = False
        Me.tbFixTime.BackColor = System.Drawing.SystemColors.Window
        Me.tbFixTime.Enabled = False
        Me.tbFixTime.Location = New System.Drawing.Point(253, 44)
        Me.tbFixTime.Name = "tbFixTime"
        Me.tbFixTime.Size = New System.Drawing.Size(167, 30)
        Me.tbFixTime.TabIndex = 10
        '
        'Button4
        '
        Me.Button4.Location = New System.Drawing.Point(135, 51)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(89, 23)
        Me.Button4.TabIndex = 11
        Me.Button4.Text = "Pickup Tags"
        Me.Button4.UseVisualStyleBackColor = True
        Me.Button4.Visible = False
        '
        'btnPlayMap
        '
        Me.btnPlayMap.Location = New System.Drawing.Point(457, 44)
        Me.btnPlayMap.Name = "btnPlayMap"
        Me.btnPlayMap.Size = New System.Drawing.Size(25, 23)
        Me.btnPlayMap.TabIndex = 13
        Me.btnPlayMap.Text = ">"
        Me.btnPlayMap.UseVisualStyleBackColor = True
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OFD"
        '
        'Button5
        '
        Me.Button5.Location = New System.Drawing.Point(253, 15)
        Me.Button5.Name = "Button5"
        Me.Button5.Size = New System.Drawing.Size(167, 23)
        Me.Button5.TabIndex = 14
        Me.Button5.Text = "Load Tag csv"
        Me.Button5.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(597, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(11, 13)
        Me.Label1.TabIndex = 15
        Me.Label1.Text = "*"
        '
        'Button6
        '
        Me.Button6.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Button6.Location = New System.Drawing.Point(426, 44)
        Me.Button6.Name = "Button6"
        Me.Button6.Size = New System.Drawing.Size(25, 23)
        Me.Button6.TabIndex = 16
        Me.Button6.Text = "<<"
        Me.Button6.UseVisualStyleBackColor = True
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(620, 474)
        Me.Controls.Add(Me.Button6)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.Button5)
        Me.Controls.Add(Me.btnPlayMap)
        Me.Controls.Add(Me.Button4)
        Me.Controls.Add(Me.tbFixTime)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.Button3)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.Button1)
        Me.MaximizeBox = False
        Me.Name = "frmMain"
        Me.Text = "WID Tag Logs To Map"
        Me.GroupBox1.ResumeLayout(False)
        CType(Me.pbMap, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.tbFixTime, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Button1 As System.Windows.Forms.Button
    Friend WithEvents Button2 As System.Windows.Forms.Button
    Friend WithEvents Button3 As System.Windows.Forms.Button
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents pbMap As System.Windows.Forms.PictureBox
    Friend WithEvents tbFixTime As System.Windows.Forms.TrackBar
    Friend WithEvents Button4 As System.Windows.Forms.Button
    Friend WithEvents btnPlayMap As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents Button5 As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Button6 As System.Windows.Forms.Button

End Class
