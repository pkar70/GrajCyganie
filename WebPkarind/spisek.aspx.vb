Public Class spisek
    Inherits System.Web.UI.Page

    Protected Async Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        loggedOK.Visible = False
        loggedError.Visible = False
        askForLogin.Visible = False

        Dim sErrorMsg As String = Await CheckLoginAsync(Me)

        If Me.Session("loggedUser") <> "" Then
            loggedOK.Visible = True
        Else
            If sErrorMsg.ToLower.Contains("no login provided") Then
                askForLogin.Visible = True
            Else
                loggedError.Visible = True
                loginMessage.InnerText = sErrorMsg
            End If
        End If

    End Sub

End Class