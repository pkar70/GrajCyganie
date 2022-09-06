Imports vb14 = Vblib.pkarlibmodule14

Public NotInheritable Class Login
    Inherits Page

    Private Async Sub uiOk_Click(sender As Object, e As RoutedEventArgs)
        Dim sTmp As String = uiUserName.Text

        If Await TestPermission(sTmp) Then
            vb14.SetSettingsString("userName", sTmp)
            vb14.SetSettingsInt("loginTryCount", 0)
        Else
            Dim iCnt As Integer = vb14.GetSettingsInt("loginTryCount")
            vb14.SetSettingsInt("loginTryCount", iCnt + 1)
        End If

    End Sub

    Private Async Function TestPermission(sUser As String) As Task(Of Boolean)
        If sUser = "" Then
            uiLoginRes.Text = "Zaloguj się"
            Return False
        End If

        Dim sRes As String = Await App.inVb.GetCurrentDb.GetPermissionAsync(sUser)
        uiLoginRes.Text = sRes
        If sRes.IndexOf("Masz prawo") < 0 Then Return False
        Return True
    End Function

    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        uiUserName.Text = vb14.GetSettingsString("userName")
        If vb14.GetSettingsInt("loginTryCount") > 5 Then
            uiUserName.IsReadOnly = True
            uiLoginRes.Text = "Brak uprawnień"
            ' App.mbGranted = False
        Else
            uiUserName.IsReadOnly = False
            Await TestPermission(uiUserName.Text)
        End If
    End Sub
End Class
