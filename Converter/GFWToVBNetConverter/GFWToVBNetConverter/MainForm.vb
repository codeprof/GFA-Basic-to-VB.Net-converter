'
' Created by SharpDevelop.
' User: benutzer
' Date: 17.01.2016
' Time: 14:14
' 
' To change this template use Tools | Options | Coding | Edit Standard Headers.
'
Public Partial Class MainForm
	Public Sub New()
		' The Me.InitializeComponent call is required for Windows Forms designer support.
		Me.InitializeComponent()
	End Sub
	
	Sub BtnSelectInputLSTFileClick(sender As Object, e As EventArgs)
		If openFileDialogLST.ShowDialog() = DialogResult.OK Then
			textBoxInputLSTFile.Text = openFileDialogLST.FileName
			textBoxOutputVBFile.Text = openFileDialogLST.FileName+".vb"
		End If		
	End Sub
	
	Sub BtnSelectOutputVBFileClick(sender As Object, e As EventArgs)
		If openFileDialogVB.ShowDialog() = DialogResult.OK Then
			textBoxOutputVBFile.Text = openFileDialogVB.FileName
		End If
	End Sub
	
	Sub BtnConvertClick(sender As Object, e As EventArgs)	
		Dim conv As GFWConverter.Converter  = New GFWConverter.Converter()
		conv.DoConvertFile(textBoxInputLSTFile.Text, textBoxOutputVBFile.Text)
		MsgBox("File " + Chr(34) + textBoxInputLSTFile.Text + Chr(34) + " converted to:" + Chr(13) + Chr(34) + textBoxOutputVBFile.Text + Chr(34))
	End Sub
	
	Sub TextBoxInputLSTFileTextChanged(sender As Object, e As EventArgs)
		If ((textBoxOutputVBFile.Text <> "") And (textBoxInputLSTFile.Text <> "")) Then
			BtnConvert.Enabled = True
		Else
			BtnConvert.Enabled = False
		End If
	End Sub
	
	Sub TextBoxOutputVBFileTextChanged(sender As Object, e As EventArgs)
		If ((textBoxOutputVBFile.Text <> "") And (textBoxInputLSTFile.Text <> "")) Then
			BtnConvert.Enabled = True
		Else
			BtnConvert.Enabled = False
		End If		
	End Sub
End Class
