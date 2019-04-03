Public Class Form2

    ' field that will contain the messege
    Private PromtMsg As String
    Sub New(ByVal promtmsg As String)

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        ' set global field with the argument that was passed to the constructor
        Me.PromtMsg = promtmsg
    End Sub

    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' set the msg label
        Me.lblPromtMsg.Text = Me.PromtMsg
    End Sub

    Private Sub btnCustomYes_Click(sender As Object, e As EventArgs) Handles btnCustomYes.Click
        ' user choosed yes - return DialogResult.Yes
        Me.DialogResult = DialogResult.Yes
        Me.Close()
    End Sub

    Private Sub btnCustomNo_Click(sender As Object, e As EventArgs) Handles btnCustomNo.Click
        ' user choosed no - DialogResult.no
        Me.DialogResult = DialogResult.No
        Me.Close()
    End Sub

End Class