'
' Created by SharpDevelop.
' User: benutzer
' Date: 17.01.2016
' Time: 14:14
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Partial Class MainForm
	Inherits System.Windows.Forms.Form
	
	''' <summary>
	''' Designer variable used to keep track of non-visual components.
	''' </summary>
	Private components As System.ComponentModel.IContainer
	
	''' <summary>
	''' Disposes resources used by the form.
	''' </summary>
	''' <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		If disposing Then
			If components IsNot Nothing Then
				components.Dispose()
			End If
		End If
		MyBase.Dispose(disposing)
	End Sub
	
	''' <summary>
	''' This method is required for Windows Forms designer support.
	''' Do not change the method contents inside the source code editor. The Forms designer might
	''' not be able to load this method if it was changed manually.
	''' </summary>
	Private Sub InitializeComponent()
		Me.textBoxInputLSTFile = New System.Windows.Forms.TextBox()
		Me.textBoxOutputVBFile = New System.Windows.Forms.TextBox()
		Me.btnConvert = New System.Windows.Forms.Button()
		Me.btnSelectInputLSTFile = New System.Windows.Forms.Button()
		Me.btnSelectOutputVBFile = New System.Windows.Forms.Button()
		Me.label1 = New System.Windows.Forms.Label()
		Me.label2 = New System.Windows.Forms.Label()
		Me.openFileDialogLST = New System.Windows.Forms.OpenFileDialog()
		Me.openFileDialogVB = New System.Windows.Forms.OpenFileDialog()
		Me.SuspendLayout
		'
		'textBoxInputLSTFile
		'
		Me.textBoxInputLSTFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.textBoxInputLSTFile.Location = New System.Drawing.Point(12, 28)
		Me.textBoxInputLSTFile.Name = "textBoxInputLSTFile"
		Me.textBoxInputLSTFile.Size = New System.Drawing.Size(240, 20)
		Me.textBoxInputLSTFile.TabIndex = 0
		AddHandler Me.textBoxInputLSTFile.TextChanged, AddressOf Me.TextBoxInputLSTFileTextChanged
		'
		'textBoxOutputVBFile
		'
		Me.textBoxOutputVBFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
						Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.textBoxOutputVBFile.Location = New System.Drawing.Point(12, 70)
		Me.textBoxOutputVBFile.Name = "textBoxOutputVBFile"
		Me.textBoxOutputVBFile.Size = New System.Drawing.Size(240, 20)
		Me.textBoxOutputVBFile.TabIndex = 1
		AddHandler Me.textBoxOutputVBFile.TextChanged, AddressOf Me.TextBoxOutputVBFileTextChanged
		'
		'btnConvert
		'
		Me.btnConvert.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.btnConvert.Enabled = false
		Me.btnConvert.Location = New System.Drawing.Point(208, 98)
		Me.btnConvert.Name = "btnConvert"
		Me.btnConvert.Size = New System.Drawing.Size(75, 23)
		Me.btnConvert.TabIndex = 2
		Me.btnConvert.Text = "Convert!"
		Me.btnConvert.UseVisualStyleBackColor = true
		AddHandler Me.btnConvert.Click, AddressOf Me.BtnConvertClick
		'
		'btnSelectInputLSTFile
		'
		Me.btnSelectInputLSTFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.btnSelectInputLSTFile.Location = New System.Drawing.Point(253, 25)
		Me.btnSelectInputLSTFile.Name = "btnSelectInputLSTFile"
		Me.btnSelectInputLSTFile.Size = New System.Drawing.Size(30, 23)
		Me.btnSelectInputLSTFile.TabIndex = 3
		Me.btnSelectInputLSTFile.Text = "..."
		Me.btnSelectInputLSTFile.UseVisualStyleBackColor = true
		AddHandler Me.btnSelectInputLSTFile.Click, AddressOf Me.BtnSelectInputLSTFileClick
		'
		'btnSelectOutputVBFile
		'
		Me.btnSelectOutputVBFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.btnSelectOutputVBFile.Location = New System.Drawing.Point(253, 67)
		Me.btnSelectOutputVBFile.Name = "btnSelectOutputVBFile"
		Me.btnSelectOutputVBFile.Size = New System.Drawing.Size(30, 23)
		Me.btnSelectOutputVBFile.TabIndex = 4
		Me.btnSelectOutputVBFile.Text = "..."
		Me.btnSelectOutputVBFile.UseVisualStyleBackColor = true
		AddHandler Me.btnSelectOutputVBFile.Click, AddressOf Me.BtnSelectOutputVBFileClick
		'
		'label1
		'
		Me.label1.Location = New System.Drawing.Point(12, 9)
		Me.label1.Name = "label1"
		Me.label1.Size = New System.Drawing.Size(160, 16)
		Me.label1.TabIndex = 5
		Me.label1.Text = "Select source file (*.LST)"
		'
		'label2
		'
		Me.label2.Location = New System.Drawing.Point(12, 51)
		Me.label2.Name = "label2"
		Me.label2.Size = New System.Drawing.Size(160, 16)
		Me.label2.TabIndex = 6
		Me.label2.Text = "Select destination file (*.VB)"
		'
		'openFileDialogLST
		'
		Me.openFileDialogLST.DefaultExt = "*.LST"
		Me.openFileDialogLST.FileName = "noname.lst"
		Me.openFileDialogLST.Filter = "Lst-Files (*.LST)|*.LST|All Files (*.*)|*.*"
		Me.openFileDialogLST.Title = "Select a .lst file to convert"
		'
		'openFileDialogVB
		'
		Me.openFileDialogVB.CheckFileExists = false
		Me.openFileDialogVB.FileName = "output.vb"
		Me.openFileDialogVB.Filter = "VB.Net (*.VB)|*.VB|All Files (*.*)|*.*"
		Me.openFileDialogVB.Title = "Declare a filename for the vb-file to be created"
		'
		'MainForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.BackColor = System.Drawing.Color.PowderBlue
		Me.ClientSize = New System.Drawing.Size(284, 122)
		Me.Controls.Add(Me.label2)
		Me.Controls.Add(Me.label1)
		Me.Controls.Add(Me.btnSelectOutputVBFile)
		Me.Controls.Add(Me.btnSelectInputLSTFile)
		Me.Controls.Add(Me.btnConvert)
		Me.Controls.Add(Me.textBoxOutputVBFile)
		Me.Controls.Add(Me.textBoxInputLSTFile)
		Me.MaximizeBox = false
		Me.MaximumSize = New System.Drawing.Size(300, 160)
		Me.MinimumSize = New System.Drawing.Size(300, 160)
		Me.Name = "MainForm"
		Me.ShowIcon = false
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "GFAWin 16-Bit to VB.Net Converter"
		Me.ResumeLayout(false)
		Me.PerformLayout
	End Sub
	Private openFileDialogVB As System.Windows.Forms.OpenFileDialog
	Private openFileDialogLST As System.Windows.Forms.OpenFileDialog
	Private label2 As System.Windows.Forms.Label
	Private label1 As System.Windows.Forms.Label
	Private btnSelectOutputVBFile As System.Windows.Forms.Button
	Private btnSelectInputLSTFile As System.Windows.Forms.Button
	Private btnConvert As System.Windows.Forms.Button
	Private textBoxOutputVBFile As System.Windows.Forms.TextBox
	Private textBoxInputLSTFile As System.Windows.Forms.TextBox
End Class
