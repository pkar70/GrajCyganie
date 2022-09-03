﻿Imports vb14 = Vblib.pkarlibmodule14

Public NotInheritable Class DbaseAndStorage
    Inherits Page

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        FillComboBaza()

        uiLocalPath.GetSettingsString()
        uiLocalODPath.GetSettingsString()
        ' FillComboPliki()
    End Sub

    Private Sub FillComboBaza()
        uiBaza.Items.Clear()

        Dim sSelected As String = App.inVb.GetCurrentDb.Nazwa
        Dim oSelect As ComboBoxItem = Nothing

        For Each oDbSrc In App.inVb.gaDbs
            Dim oNew As New ComboBoxItem
            oNew.Content = oDbSrc.Nazwa
            If oDbSrc.Nazwa = sSelected Then oSelect = oNew
            uiBaza.Items.Add(oDbSrc.Nazwa)
        Next

        If oSelect IsNot Nothing Then
            uiBaza.SelectedItem = oSelect
        End If

    End Sub

    'Private Sub FillComboPliki()
    '    uiPliki.Items.Clear()

    '    uiPliki.Items.Add("localstorage")
    '    'For Each oDbSrc In App.inVb.gaDbs
    '    '    uiPliki.Items.Add(oDbSrc.Nazwa)
    '    'Next

    '    Dim sSelected As String = vb14.GetSettingsString("usingFS")
    '    If sSelected <> "" Then
    '        For Each oItem As ComboBoxItem In uiPliki.Items
    '            If oItem.Content = sSelected Then oItem.IsSelected = True
    '        Next
    '    End If
    'End Sub

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
        If Not uiLocalPath.Text.EndsWith("\") Then uiLocalPath.Text &= "\"
        If Not uiLocalODPath.Text.EndsWith("\") Then uiLocalODPath.Text &= "\"

        uiLocalPath.SetSettingsString()
        uiLocalODPath.SetSettingsString()

        Me.GoBack()
        ' przełączenie następuje w ramach MainPage:Loaded
    End Sub

End Class
