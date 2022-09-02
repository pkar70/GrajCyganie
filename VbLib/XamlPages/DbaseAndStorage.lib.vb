'Public NotInheritable Class DbaseAndStorage
'    Inherits Page

'    Private mbLoading As Boolean = True

'    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
'        mbLoading = True
'        FillComboBaza()
'        FillComboPliki()

'        mbLoading = False
'    End Sub

'    Private Sub FillComboBaza()
'        uiBaza.Items.Clear()

'        uiBaza.Items.Add((New dbase_beskidAsp).Nazwa)
'        uiBaza.Items.Add((New dbase_localASP).Nazwa)

'        Dim sSelected As String = GetSettingsString("using" & uiBaza.Name)
'        If sSelected <> "" Then
'            For Each oItem As ComboBoxItem In uiBaza.Items
'                If oItem.Content = sSelected Then oItem.IsSelected = True
'            Next
'        End If
'    End Sub

'    Private Sub FillComboPliki()
'        uiPliki.Items.Clear()

'        uiPliki.Items.Add((New storage_beskid).Nazwa)
'        uiPliki.Items.Add((New Storage_Local).Nazwa)

'        Dim sSelected As String = GetSettingsString("using" & uiPliki.Name)
'        If sSelected <> "" Then
'            For Each oItem As ComboBoxItem In uiPliki.Items
'                If oItem.Content = sSelected Then oItem.IsSelected = True
'            Next
'        End If
'    End Sub


'    Private Sub SaveCombo(uiCombo As ComboBox)
'        Dim iSelItem As Integer = uiCombo.SelectedIndex
'        If iSelItem < 0 Then Return
'        Dim sValue As String = uiCombo.SelectedItem().ToString
'        SetSettingsString("using" & uiCombo.Name, sValue)
'    End Sub
'    Private Sub uiSave_Click(sender As Object, e As RoutedEventArgs)
'        SaveCombo(uiBaza)
'        SaveCombo(uiPliki)
'        Me.Frame.GoBack()
'        ' przełączenie następuje w ramach MainPage:Loaded
'    End Sub

'End Class
