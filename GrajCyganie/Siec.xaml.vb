Imports vb14 = Vblib.pkarlibmodule14
Imports Vblib.Extensions


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

        uiTotalFiles.Text = vb14.GetSettingsInt("miTotalFiles").BigNumFormat
        uiTotalMiB.Text = vb14.GetSettingsInt("miTotalMiB").BigNumFormat

        uiMonthFiles.Text = vb14.GetSettingsInt("miMonthFiles").BigNumFormat
        uiMonthMiB.Text = vb14.GetSettingsInt("miMonthMiB").BigNumFormat

    End Sub

    Private Async Sub uiResetSession_Click(sender As Object, e As RoutedEventArgs)
        If Await vb14.DialogBoxYNAsync("Wyzerować liczniki?") Then
            App.miSessionFiles = 0
            App.miSessionMiB = 0
        End If
    End Sub

    Private Async Sub uiResetTotal_Click(sender As Object, e As RoutedEventArgs)
        If Await vb14.DialogBoxYNAsync("Wyzerować liczniki?") Then
            vb14.SetSettingsInt("miTotalFiles", 0)
            vb14.SetSettingsInt("miTotalMiB", 0)
        End If
    End Sub

End Class
