Imports vb14 = Vblib.pkarlibmodule14

Public NotInheritable Class DbaseAndStorage
    Inherits Page

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        FillComboBaza()

        uiLocalPath.GetSettingsString()
        uiLocalODPath.GetSettingsString()
        ' drugi OneDrive - 2022.09.08
        uiLocalODPath2.GetSettingsString()
        ' FillComboPliki()
    End Sub

    Private Sub FillComboBaza()
        uiBaza.Items.Clear()

        Dim sSelected As String = App.inVb.GetCurrentDb.Nazwa
        Dim oSelect As ComboBoxItem = Nothing

        For Each oDbSrc In Vblib.App.gaDbs
            Dim oNew As New ComboBoxItem
            oNew.Content = oDbSrc.Nazwa
            If oDbSrc.Nazwa = sSelected Then oSelect = oNew
            uiBaza.Items.Add(oDbSrc.Nazwa)
        Next

        If oSelect IsNot Nothing Then
            uiBaza.SelectedItem = oSelect
        End If

    End Sub

    Private Sub SaveCombo(uiCombo As ComboBox)
        Dim iSelItem As Integer = uiCombo.SelectedIndex
        If iSelItem < 0 Then Return
        Dim sValue As String = uiCombo.SelectedItem().ToString
        Dim sSettName As String = "using"
        If uiCombo.Name.Contains("Pliki") Then
            sSettName &= "FS"
        Else
            sSettName &= "DB"
        End If
        vb14.SetSettingsString(sSettName, sValue)
    End Sub
    Private Sub uiSave_Click(sender As Object, e As RoutedEventArgs)
        SaveCombo(uiBaza)
        'SaveCombo(uiPliki)

        ' żeby potem łatwiej sklejać
        If uiLocalPath.Text <> "" AndAlso Not uiLocalPath.Text.EndsWith("\") Then uiLocalPath.Text &= "\"
        If uiLocalODPath.Text <> "" AndAlso Not uiLocalODPath.Text.EndsWith("\") Then uiLocalODPath.Text &= "\"

        uiLocalPath.SetSettingsString()
        uiLocalODPath.SetSettingsString()
        uiLocalODPath2.SetSettingsString()

        Me.GoBack()
        ' przełączenie następuje w ramach MainPage:Loaded
    End Sub

End Class
