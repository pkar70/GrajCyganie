' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class Siec
    Inherits Page
    '    ---- Network ---
    '1. ping
    '2. statystyka transferu

    Private Function BigNumFormat(iValue As Integer) As String
        Dim sTxt As String = iValue.ToString

        If sTxt.Length > 4 Then sTxt = sTxt.Substring(0, sTxt.Length - 3) & " " & sTxt.Substring(sTxt.Length - 3)

        Return sTxt
    End Function

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        uiSessionFiles.Text = App.miSessionFiles
        uiSessionMiB.Text = App.miSessionMiB

        uiTotalFiles.Text = BigNumFormat(GetSettingsInt("miTotalFiles"))
        uiTotalMiB.Text = BigNumFormat(GetSettingsInt("miTotalMiB"))

        uiMonthFiles.Text = BigNumFormat(GetSettingsInt("miMonthFiles"))
        uiMonthMiB.Text = BigNumFormat(GetSettingsInt("miMonthMiB"))

    End Sub

    Private Async Sub uiResetSession_Click(sender As Object, e As RoutedEventArgs)
        If Await DialogBoxYNAsync("Wyzerować liczniki?") Then
            App.miSessionFiles = 0
            App.miSessionMiB = 0
        End If
    End Sub

    Private Async Sub uiResetTotal_Click(sender As Object, e As RoutedEventArgs)
        If Await DialogBoxYNAsync("Wyzerować liczniki?") Then
            SetSettingsInt("miTotalFiles", 0)
            SetSettingsInt("miTotalMiB", 0)
        End If
    End Sub

End Class
