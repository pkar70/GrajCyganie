'' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

'''' <summary>
'''' An empty page that can be used on its own or navigated to within a Frame.
'''' </summary>
'Public NotInheritable Class Login
'    Inherits Page

'    Private Async Sub uiOk_Click(sender As Object, e As RoutedEventArgs)
'        Dim sTmp As String = uiUserName.Text

'        If Await TestPermission(sTmp) Then
'            SetSettingsString("userName", sTmp)
'            SetSettingsInt("loginTryCount", 0)
'        Else
'            Dim iCnt As Integer = GetSettingsInt("loginTryCount")
'            SetSettingsInt("loginTryCount", iCnt + 1)
'        End If

'    End Sub

'    Private Async Function TestPermission(sUser As String) As Task(Of Boolean)
'        If sUser = "" Then
'            uiLoginRes.Text = "Zaloguj się"
'            Return False
'        End If

'        Dim sRes As String = Await App.goDbase.GetPermission(sUser)
'        uiLoginRes.Text = sRes
'        If sRes.IndexOf("Masz prawo") < 0 Then Return False
'        Return True
'    End Function

'    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
'        uiUserName.Text = GetSettingsString("userName")
'        If GetSettingsInt("loginTryCount") > 5 Then
'            uiUserName.IsReadOnly = True
'            uiLoginRes.Text = "Brak uprawnień"
'            ' App.mbGranted = False
'        Else
'            uiUserName.IsReadOnly = False
'            TestPermission(uiUserName.Text)
'        End If
'    End Sub
'End Class
