Public Module ColorPicker  ' source Internet : https://stackoverflow.com/questions/47190131/simple-dialog-like-msgbox-with-custom-buttons-vb, but changed to serve my needs: custom btn, names...
    Private result As String
    Public Function Show(options As IEnumerable(Of String), Optional message As String = "", Optional title As String = "") As String
        result = "Cancel"
        Dim ColorPickerForm As New Form With {.Text = title}
        Dim tlp As New TableLayoutPanel With {.ColumnCount = 1, .RowCount = 2}
        Dim flp As New FlowLayoutPanel()
        Dim l As New Label With {.Text = message}
        ColorPickerForm.Controls.Add(tlp)
        tlp.Dock = DockStyle.Fill
        tlp.Controls.Add(l)
        l.Dock = DockStyle.Fill
        tlp.Controls.Add(flp)
        flp.Dock = DockStyle.Fill
        For Each o In options
            Dim b As New Button With {.Text = o}
            flp.Controls.Add(b)
            AddHandler b.Click,
                Sub(sender As Object, e As EventArgs)
                    result = DirectCast(sender, Button).TabIndex
                    ColorPickerForm.Close()
                End Sub
        Next
        ColorPickerForm.FormBorderStyle = FormBorderStyle.FixedDialog
        ColorPickerForm.Height = 100
        ColorPickerForm.Width = 350
        ColorPickerForm.ShowDialog()
        Return result
    End Function
End Module
