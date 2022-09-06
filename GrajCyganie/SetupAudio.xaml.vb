

Public NotInheritable Class SetupAudio
    Inherits Page

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        uiSexZapowiedzi.GetSettingsBool()

        Try
            uiDefaultVoice.Text = OpiszVoice(Windows.Media.SpeechSynthesis.SpeechSynthesizer.DefaultVoice)
        Catch ex As Exception
            uiDefaultVoice.Text = "FAIL: cannot get voices - try to install some"
            uiDefaultVoice.Foreground = New SolidColorBrush(Windows.UI.Colors.Red)
            uiGadaj.IsEnabled = False
            App.gbNoSpeak = True
        End Try

        ' wypełnianie combo języków
        uiVoices.Items.Clear()
        For Each oVoice As Windows.Media.SpeechSynthesis.VoiceInformation In Windows.Media.SpeechSynthesis.SpeechSynthesizer.AllVoices
            Dim oNew As New ComboBoxItem
            oNew.Content = OpiszVoice(oVoice)
            oNew.DataContext = oVoice
            uiVoices.Items.Add(oNew)
        Next
        If uiVoices.Items.Count > 0 Then uiGadaj.IsEnabled = True

        FillComboDevices()

    End Sub

    Private Async Sub FillComboDevices()
        Dim sSelected As String = Grajek_GetCurrentDeviceName()

        Dim audioSelector As String = Windows.Media.Devices.MediaDevice.GetAudioRenderSelector()
        Dim outputDevices As Windows.Devices.Enumeration.DeviceInformationCollection = Await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(audioSelector)

        For Each oDev As Windows.Devices.Enumeration.DeviceInformation In outputDevices
            Dim oNew As ComboBoxItem = New ComboBoxItem
            oNew.Content = oDev.Name
            If oDev.Name = sSelected Then oNew.IsSelected = True
            oNew.DataContext = oDev
            uiComboDevices.Items.Add(oNew)
        Next
    End Sub


    Private Sub uiGadaj_Click(sender As Object, e As RoutedEventArgs)
        Dim oVoice As Windows.Media.SpeechSynthesis.VoiceInformation
        oVoice = TryCast(uiVoices.SelectedItem, ComboBoxItem)?.DataContext
        If oVoice Is Nothing Then Return

        Dim sTxt As String = uiTextDoPrzeczytania.Text
        If sTxt.Length < 5 Then Return

        SpeakOdczytajStringAsync(sTxt, oVoice.Language, If(oVoice.Gender = Windows.Media.SpeechSynthesis.VoiceGender.Male, True, False))

    End Sub

    Private Sub uiSave_Click(sender As Object, e As RoutedEventArgs)
        uiSexZapowiedzi.SetSettingsBool()

        Dim selectedDevice As Windows.Devices.Enumeration.DeviceInformation = TryCast(uiComboDevices.SelectedItem, ComboBoxItem).DataContext
        Grajek_SetDevice(selectedDevice)
    End Sub

    ' uiMamLanguagi - ale tylko pokazanie
    ' button 'cos powiedz', może być pole do podania tekstu oraz combo do wyboru jezyka (tabelka)
End Class
