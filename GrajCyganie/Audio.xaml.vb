' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

Imports Windows.Devices.Enumeration
''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class Audio
    Inherits Page

    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)

        Dim sSelected As String = ""
        Try
            If App.moMediaPlayer IsNot Nothing Then
                sSelected = App.moMediaPlayer.AudioDevice.Name
            End If
        Catch ex As Exception
            ' podczas grania nie da się?
        End Try

        uiSex.IsOn = GetSettingsBool("sexZapowiedzi")

        Dim audioSelector As String = Windows.Media.Devices.MediaDevice.GetAudioRenderSelector()
        Dim outputDevices As DeviceInformationCollection = Await DeviceInformation.FindAllAsync(audioSelector)

        For Each oDev As DeviceInformation In outputDevices
            Dim oNew As ComboBoxItem = New ComboBoxItem
            oNew.Content = oDev.Name
            If oDev.Name = sSelected Then oNew.IsSelected = True
            oNew.Tag = oDev
            uiComboDevices.Items.Add(oNew)
        Next


    End Sub

    Private Sub uiSetAudio_Click(sender As Object, e As RoutedEventArgs)
        If App.moMediaPlayer Is Nothing Then
            DialogBox("MediaPlayer nie zainicjalizowany?")
            Exit Sub
        End If

        Dim selectedDevice As DeviceInformation = TryCast(uiComboDevices.SelectedItem, ComboBoxItem).Tag
        If selectedDevice IsNot Nothing Then App.moMediaPlayer.AudioDevice = selectedDevice

        SetSettingsBool("sexZapowiedzi", uiSex.IsOn)

    End Sub

    '    ---- Audio ---
    '1. wybór audiorenderer (ze w telefonie: BT, glosnik, sluchawki?
    '2. ewentualnie korektor

End Class
