Imports vb14 = Vblib.pkarlibmodule14
Imports Vblib.Extensions


' 2021.10.16: nowszy pkarmodule (z RemoteSystem)
' 2021.10.16: początek przerabiania na CosmoDB

' 2020.01.03: przejście na shared pkarmodule

' 2019.05.24: 
' log - ostatnie wpisy, typu 'szukam nastepnego'
' poprawka guzik myszy Do szukania wiki przestal dzialac po zmianie TextBlock na TextBox
' MainPage: część komend do wspólnego Button settings
' przygotowanie do RemoteSystem (jako pilot/grajek)
' miesiąc - automatyczne zerowanie przy zmianie Date.Now.Month

' Imports Vblib.

Public NotInheritable Class MainPage
    Inherits Page

    '    ---- MainPage ----
    '1. panel procentowy - wedle grup dekadowych (z cache pliku dekad)
    '2. informacja o granym pliku (z audioParams)
    '3. wybór grania (next, pause, stop...)
    '4. z cmdbar: czytacz (albo i na ekranie - ale tam chyba nie byłoby na to miejsca)

    ' SetSettingsInt("maxSoundId", sRes)

    Private miNextMode As Vblib.eNextMode = Vblib.eNextMode.random
    Private miCoGram As eCoGram = eCoGram.nic

    ' Private moMSource As Windows.Media.Core.MediaSource
    'Private moMSource As Windows.Media.Playback.MediaPlaybackItem

#Region "Gadaczka"


    Private Function CreateWinTitle(oGranyUtwor As Vblib.tGranyUtwor, iNextMode As Vblib.eNextMode) As String
        vb14.DumpCurrMethod()
        Dim sTxt As String = ""
        If iNextMode <> Vblib.eNextMode.sameArtist Then sTxt = oGranyUtwor.oAudioParam.artist
        If iNextMode <> Vblib.eNextMode.sameTitle Then
            If sTxt <> "" Then sTxt &= ": "
            sTxt &= oGranyUtwor.oAudioParam.title
        End If

        Return sTxt
    End Function

    Private Async Function SpeakOdczytajAsync() As Task
        vb14.DumpCurrMethod()
        If miNextMode = Vblib.eNextMode.loopSong Then Exit Function

        Dim sLang As String = RozpoznajJezykUtworu(App.mtGranyUtwor)

        ' artis: title
        ' ale gdy loop/lock, wtedy niekoniecznie
        Dim sTxt As String = CreateWinTitle(App.mtGranyUtwor, miNextMode)

        If sTxt <> "" Then Await SpeakOdczytajStringAsync(sTxt, sLang, vb14.GetLangString("uiSexZapowiedzi"))

    End Function

#End Region

#Region "informacja na ekranie"

    Private Sub WypelnPoleZapetlacza(oRadio As RadioButton, iCount As Integer)
        If iCount > 999 Then
            oRadio.Content = "*"
        Else
            oRadio.Content = iCount
        End If
        oRadio.IsEnabled = (iCount > 1)
    End Sub

    Private Sub WypelnPolaOpisu(oGranyUtwor As Vblib.tGranyUtwor)
        If oGranyUtwor Is Nothing Then
            vb14.DumpMessage("Empty App.mtGranyUtwor")
            Return
        End If

        uiArtist.Text = oGranyUtwor.oAudioParam.artist
        WypelnPoleZapetlacza(uiArtist_Radio, oGranyUtwor.countArtist)

        uiTitle.Text = oGranyUtwor.oAudioParam.title
        WypelnPoleZapetlacza(uiTitle_Radio, oGranyUtwor.countTitle)

        If oGranyUtwor.oAudioParam.track <> "" Then
            uiAlbum.Text = oGranyUtwor.oAudioParam.track & " z: " & oGranyUtwor.oAudioParam.album
        Else
            uiAlbum.Text = oGranyUtwor.oAudioParam.album
        End If
        WypelnPoleZapetlacza(uiAlbum_Radio, oGranyUtwor.countAlbum)

        uiRok.Text = oGranyUtwor.oAudioParam.year
        WypelnPoleZapetlacza(uiRok_Radio, oGranyUtwor.countYear)

        uiDekada.Text = oGranyUtwor.oAudioParam.dekada
        WypelnPoleZapetlacza(uiDekada_Radio, oGranyUtwor.countDekada)

        uiComment.Text = oGranyUtwor.oAudioParam.comment

    End Sub

    Private Sub WypelnSliderDekady(sDekada As String)
        If Vblib.App._dekady.GetList Is Nothing Then
            uiSlider.Visibility = Visibility.Collapsed
            uiSliderInfo.Visibility = Visibility.Collapsed
        Else
            uiSlider.Visibility = Visibility.Visible
            uiSliderInfo.Visibility = Visibility.Visible
            For Each oItem As Vblib.tDekada In Vblib.App._dekady.GetList
                If oItem.sNazwa = sDekada Then
                    uiSlider.Value = oItem.iFreq
                    uiSliderInfo.Text = oItem.GetFreqString
                    Exit For ' to samo co Return, ale tak jest ładniej :)
                End If
            Next
        End If
    End Sub



    Private Sub WypelnijPola(oGranyUtwor As Vblib.tGranyUtwor)
        vb14.DumpCurrMethod()

        WypelnPolaOpisu(oGranyUtwor)
        WypelnSliderDekady(oGranyUtwor.oAudioParam.dekada)

        ' AktualizujLiczniki(oGranyUtwor.oStoreFile.len)

        Try
            ApplicationView.GetForCurrentView.Title = CreateWinTitle(oGranyUtwor, miNextMode)
        Catch ex As Exception

        End Try
    End Sub

#End Region

    Private Sub AktualizujLiczniki(lFileLen As Long)
        Dim iMiB As Integer = lFileLen
        iMiB = iMiB / 1024 / 1024
        iMiB = iMiB + 1

        vb14.SetSettingsInt("miTotalFiles", vb14.GetSettingsInt("miTotalFiles") + 1)
        vb14.SetSettingsInt("miTotalMiB", vb14.GetSettingsInt("miTotalMiB") + iMiB)

        If vb14.GetSettingsInt("miMonth") <> Date.Now.Month Then
            vb14.SetSettingsInt("miMonth", Date.Now.Month)
            vb14.SetSettingsInt("miMonthFiles", 0)
            vb14.SetSettingsInt("miMonthMiB", 0)
        End If
        vb14.SetSettingsInt("miMonthFiles", vb14.GetSettingsInt("miMonthFiles") + 1)
        vb14.SetSettingsInt("miMonthMiB", vb14.GetSettingsInt("miMonthMiB") + iMiB)
    End Sub

    'Private Async Sub SNM_CloseRequested(sender As Object, e As Windows.UI.Core.Preview.SystemNavigationCloseRequestedPreviewEventArgs)
    '    vb14.DumpCurrMethod()
    '    Dim oDef As Deferral = e.GetDeferral
    '    ' tylko do systray
    '    'If (Metadata.ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0)) Then
    '    '    Await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync()
    '    'End If

    '    e.Handled = False
    '    oDef.Complete()

    'End Sub


    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        'moLastNextDate = Date.Now
        Me.ProgRingInit(True, False)

        ' ConnectServices()

        If Not IsFamilyMobile() Then
            ApplicationView.GetForCurrentView().SetPreferredMinSize(New Size(310, 400))
        End If

        ' AddHandler Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView().CloseRequested, AddressOf SNM_CloseRequested

        Grajek_Init(uiGrajek, AddressOf GoNextSongUI)

        uiReadAfter.GetSettingsBool()
        uiReadBefore.GetSettingsBool()

        If Not Await App.inVb.GetCurrentDb.LoginAsync(True) Then
            uiGoLogin.Visibility = Visibility.Visible
            uiGoSetting.Visibility = Visibility.Collapsed
            uiGoSearch.Visibility = Visibility.Collapsed
            Exit Sub
        End If

        uiGoLogin.Visibility = Visibility.Collapsed
        uiGoSetting.Visibility = Visibility.Visible
        uiGoSearch.Visibility = Visibility.Visible

        Await App.inVb.GetCurrentDb.ReloadDekadyAsync(False)

        uiUseMicro.GetSettingsBool()
        If Not Await SpeechRecoInit(AddressOf SpeechCommand) Then
            uiUseMicro.IsChecked = False
            uiUseMicro.IsEnabled = False
        End If


        If App.mtGranyUtwor Is Nothing Then Exit Sub

        ' If Not Await App.BeskidGetDekady(False) Then Exit Sub - do ustawiania slider dekady jest potrzebne

        WypelnijPola(App.mtGranyUtwor)
        AktualizujLiczniki(App.mtGranyUtwor.oStoreFile.len)
        ZaznaczRadio(0)

        Dim bGramy As Boolean = Grajek_CzyGra()

        If bGramy Then
            uiStart.Visibility = Visibility.Collapsed
            uiGrajek.Visibility = Visibility.Visible
            uiLoop_Radio.Visibility = Visibility.Visible
            uiNext.Visibility = Visibility.Visible
            ' uiGrajek.SetMediaPlayer(App.moMediaPlayer)

        End If

        AddHandler App.PilotChce, AddressOf PilotChce

        'If App.WinVer() > 14390 Then    ' 
        '    With uiGrajek.TransportControls
        '        .IsCompact = False   ' True: jedna linijka, False: dwie linijki (osobno progress, osobno guziki)
        '        .IsFullWindowButtonVisible = False
        '        '.IsPreviousTrackButtonVisible = True
        '        '.IsSkipBackwardButtonVisible = True
        '        '.IsSkipForwardButtonVisible = True
        '        .IsNextTrackButtonVisible = True
        '        '1) samo NextTrack nie pokazuje, musi byc rownie PrevTrack?
        '        '2) Event odpala na pause/resume, ale na nexttrack nie
        '        'AddHandler App.moMediaPlayer.SystemMediaTransportControls.ButtonPressed, AddressOf EventGuzikaMediaPlayer
        '        ' AddHandler App.moMediaPlayer.
        '    End With
        '    SystemMediaTransportControls.GetForCurrentView.IsNextEnabled = True
        'End If

        ' AddHandler uiGrajek.TransportControls.tr
        'AddHandler uiGrajek.TransportControls.
        ' Play/Pause/Continue, Loop utwór, Stop, Next



    End Sub

    'Private Sub EventGuzikaMediaPlayer(sender As Windows.Media.SystemMediaTransportControls, args As Windows.Media.SystemMediaTransportControlsButtonPressedEventArgs)
    '    vb14.DumpCurrMethod()
    '    Debug.WriteLine("guzik media player")
    '    'Select Case args.Button
    '    '    Case SystemMediaTransportControlsButton.Next
    '    '        Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, AddressOf GoNextSongSub)
    '    'End Select
    'End Sub
#Region "voice commands"

    Private Shared msVoiceCmd As String

    Private Async Sub SpeechCommand(sVoiceCommand As String)
        msVoiceCmd = sVoiceCommand
        If msVoiceCmd = "" Then Exit Sub
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, AddressOf recoDoCommand)
    End Sub

    Private Sub recoDoCommand()
        vb14.DumpCurrMethod("msVoiceCmd=" & msVoiceCmd)
        Select Case msVoiceCmd
            Case "play"
                uiStart_Click(Nothing, Nothing)
            Case "pause"
                Grajek_Pause()
            Case "cont"
                Grajek_Play()
            Case "next"
                GoNextSongUI()
            Case "loopArt"
                ZaznaczRadio(1)
            Case "loopTitle"
                ZaznaczRadio(2)
            Case "loopAlbum"
                ZaznaczRadio(3)
            Case "loopYear"
                ZaznaczRadio(4)
            Case "loopDecade"
                ZaznaczRadio(5)
            Case "loopSong"
                ZaznaczRadio(6)
            Case "loopNone"
                ZaznaczRadio(0)
            Case "stat"
                uiGoStat_Click(Nothing, Nothing)
            Case "before"
                uiReadBefore.IsChecked = Not uiReadBefore.IsChecked
            Case "after"
                uiReadAfter.IsChecked = Not uiReadAfter.IsChecked
            Case "prev"
            Case "info"
            Case "stop"
        End Select
    End Sub



#End Region


#Region "Zapetlenia - UI"

    Private Sub ZaznaczRadio(iMode As Vblib.eNextMode)
        vb14.DumpCurrMethod()
        If miNextMode = iMode Then iMode = Vblib.eNextMode.random ' wyłączenie Radio buttonów

        uiArtist_Radio.IsChecked = (iMode = Vblib.eNextMode.sameArtist)
        uiTitle_Radio.IsChecked = (iMode = Vblib.eNextMode.sameTitle)
        uiAlbum_Radio.IsChecked = (iMode = Vblib.eNextMode.sameAlbum)
        uiRok_Radio.IsChecked = (iMode = Vblib.eNextMode.sameRok)
        uiDekada_Radio.IsChecked = (iMode = Vblib.eNextMode.sameDekada)
        uiLoop_Radio.IsChecked = (iMode = Vblib.eNextMode.loopSong)

        miNextMode = iMode
    End Sub

    Private Sub uiArtist_Radio_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiArtist_Radio.Tapped
        ZaznaczRadio(Vblib.eNextMode.sameArtist)
    End Sub

    Private Sub uiTitle_Radio_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiTitle_Radio.Tapped
        ZaznaczRadio(Vblib.eNextMode.sameTitle)
    End Sub

    Private Sub uiAlbum_Radio_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiAlbum_Radio.Tapped
        ZaznaczRadio(Vblib.eNextMode.sameAlbum)
    End Sub

    Private Sub uiRok_Radio_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiRok_Radio.Tapped
        ZaznaczRadio(Vblib.eNextMode.sameRok)
    End Sub

    Private Sub uiDekada_Radio_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiDekada_Radio.Tapped
        ZaznaczRadio(Vblib.eNextMode.sameDekada)
    End Sub

    Private Sub uiLoop_Radio_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiLoop_Radio.Tapped
        ZaznaczRadio(Vblib.eNextMode.loopSong)
    End Sub
#End Region

    Private Enum eCoGram
        nic = 0
        zapowiedzPre = 1
        zapowiedzPost = 2
        song = 3
    End Enum

    Private Async Function LosujDoSkutkuAsync(oGrany As Vblib.tGranyUtwor, iNextMode As Vblib.eNextMode) As Task(Of Vblib.tGranyUtwor)

        Dim oNextSong As Vblib.tGranyUtwor = Nothing

        For iGuard As Integer = 0 To 50

            oNextSong = Await App.inVb.GetCurrentDb.GetNextSongAsync(iNextMode, oGrany)
            If oNextSong Is Nothing Then Return Nothing

            App.gsLog = App.gsLog & "GoNextSong, wylosowany " & oNextSong.oAudioParam.id & vbCrLf

            ' skoro nie losowo, to slidery dekadowe są ignorowane
            If iNextMode <> Vblib.eNextMode.random Then Return oNextSong

            If Vblib.App._dekady.CanPlay(oNextSong.oAudioParam.dekada) Then Return oNextSong

            ' jeszcze nie
            App.gsLog = App.gsLog & "GoNextSong, ale nie każdy z dekady " & oNextSong.oAudioParam.dekada & vbCrLf

        Next

        ' no to ostatni który był analizowany sobie zagramy
        vb14.DialogBox("Przewidziane niemożliwe nr 1 - max kolejnych nietrafionych")
        Return oNextSong
    End Function


    Private Async Function GoNextSong(iNextMode As Vblib.eNextMode) As Task(Of Boolean)
        Dim sDOut As String = ""
        If App.mtGranyUtwor IsNot Nothing Then
            sDOut = $"miNextMode={iNextMode}, miCoGram={miCoGram}, artist={App.mtGranyUtwor.oAudioParam.artist}"
        Else
            sDOut = $"miNextMode={iNextMode}, miCoGram={miCoGram}, mtGranyUtwor=NULL"
        End If
        App.gsLog = App.gsLog & sDOut & vbCrLf
        vb14.DumpCurrMethod(sDOut)

        If iNextMode <> Vblib.eNextMode.loopSong Then

            ' zakonczone granie utworu
            If miCoGram = eCoGram.song Then
                miCoGram = eCoGram.zapowiedzPost
                If uiReadAfter.IsChecked Then
                    Await SpeakOdczytajAsync() ' ale Speak zakończy się kolejnym wywołaniem GoNextSong
                    Return True
                End If
            End If

            ' po zakonczeniu czytania informacji o poprzednim - wylosuj nastepny utwor
            If miCoGram = eCoGram.zapowiedzPost Or miCoGram = eCoGram.nic Then

                Me.ProgRingShow(True)
                Dim oNextSong As Vblib.tGranyUtwor = Await LosujDoSkutkuAsync(App.mtGranyUtwor, iNextMode)
                If oNextSong Is Nothing Then
                    ' nie wiadomo co tak naprawdę, bo nie powinno tak się zdarzyć
                    Await vb14.DialogBoxAsync("nie wiem co się podziało - nie powinno być NULL z LosujDoSkutkuAsync")
                    Throw New Exception("serio nie wiem co teraz począć")
                End If

                ' jak juz wiadomo jaki plik - to wczytaj liczniki
                Await App.inVb.GetCurrentDb.RetrieveCountsyAsync(oNextSong)

                WypelnijPola(oNextSong)
                AktualizujLiczniki(oNextSong.oStoreFile.len)
                Me.ProgRingShow(False)

                App.mtGranyUtwor = oNextSong

                miCoGram = eCoGram.zapowiedzPre
                If uiReadBefore.IsChecked Then
                    Await SpeakOdczytajAsync()
                    Return True
                End If

            End If    ' wylosowano juz nastepny utwor, ewentualnie zapowiedziano

        End If ' if NOT LoopUtworu

        If miCoGram = eCoGram.zapowiedzPre Or iNextMode = Vblib.eNextMode.loopSong Then
            'Dim sTxt As String = App.mtGranyUtwor.uri

            'sTxt = sTxt.Replace("#", "%23")  ' to jest tylko proba - teraz moze podwojnie escapeowa?
            'Dim oUri As Uri = New Uri(App.BaseUri & "/p" & sTxt)   ' sTxt = "/store/.... "
            Dim moMSource As Windows.Media.Core.MediaSource = Await GetMediaSourceFrom(App.mtGranyUtwor.oStoreFile)
            If moMSource Is Nothing Then
                App.gsLog = App.gsLog & "GoNextSong, ale nie ma pliku w " & App.mtGranyUtwor.oStoreFile.path & vbCrLf
                miCoGram = eCoGram.zapowiedzPost ' znaczy na pewno będzie losował następny
                Return Await GoNextSong(iNextMode)
            End If

            Grajek_SetSource(moMSource)
            Grajek_Play()

            miCoGram = eCoGram.song
            'uiGrajek.SetMediaPlayer(App.moMediaPlayer)

            Await Task.Delay(1000)

            Grajek_UstawKontrolki()
        End If

        Return True
    End Function


    Private Async Sub GoNextSongSub()
        vb14.DumpCurrMethod()
        Await GoNextSong(miNextMode)
    End Sub

    Private Async Function GoNextSongUI() As Task
        vb14.DumpCurrMethod()
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, AddressOf GoNextSongSub)
    End Function


    Private Async Sub uiStart_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        uiStart.Visibility = Visibility.Collapsed
        uiGrajek.Visibility = Visibility.Visible
        uiLoop_Radio.Visibility = Visibility.Visible
        uiNext.Visibility = Visibility.Visible
        Await GoNextSongUI()
    End Sub

    Private Async Sub uiNextSong_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        Await GoNextSongUI()
    End Sub

    Private Sub uiSlider_ValueChanged(sender As Object, e As RangeBaseValueChangedEventArgs) Handles uiSlider.ValueChanged
        vb14.DumpCurrMethod()
        'Dim oDekada As tDekada = TryCast(oSlider.DataContext, tDekada)
        'oDekada.sFreq = App.FreqSlider2Text(oSlider.Value)

        For Each oDekada As Vblib.tDekada In Vblib.App._dekady.GetList
            If oDekada.sNazwa = uiDekada.Text Then
                oDekada.iFreq = uiSlider.Value

                uiSliderInfo.Text = oDekada.GetFreqString
            End If
        Next

    End Sub

    Private Sub uiReadAfterBefore_Changed(sender As Object, e As RoutedEventArgs) Handles uiReadBefore.Unchecked, uiReadAfter.Unchecked, uiReadBefore.Checked, uiReadAfter.Checked
        vb14.DumpCurrMethod()
        Dim oItem As ToggleButton = TryCast(sender, ToggleButton)
        vb14.SetSettingsBool(oItem.Name, oItem.IsChecked)
    End Sub

    'Private Sub uiComment_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiComment.Tapped
    '    Dim oMPI As Windows.Media.Playback.MediaPlaybackItem = Windows.Media.Playback.MediaPlaybackItem.FindFromMediaSource(moMSource)
    'End Sub

    Private Sub uiRadio_AccessKeyInvoked(sender As UIElement, args As AccessKeyInvokedEventArgs) Handles uiLoop_Radio.AccessKeyInvoked, uiAlbum_Radio.AccessKeyInvoked, uiArtist_Radio.AccessKeyInvoked, uiTitle_Radio.AccessKeyInvoked, uiDekada_Radio.AccessKeyInvoked, uiRok_Radio.AccessKeyInvoked
        vb14.DumpCurrMethod()
        Dim oRadio As RadioButton = TryCast(sender, RadioButton)
        If oRadio.IsChecked Then
            oRadio.IsChecked = False
            args.Handled = True
        End If
    End Sub


    Private Sub uiGrajek_AccessKeyInvoked(sender As UIElement, args As AccessKeyInvokedEventArgs) Handles uiGrajek.AccessKeyInvoked
        vb14.DumpCurrMethod()
        Grajek_PauseResume()
    End Sub

    Private Sub uiMenu_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        ' ui(Artist|Title|Album)(Wiki|Search|Copy)
        Dim sTxt As String = ""
        Dim iInd As Integer
        Dim sSenderName As String = TryCast(sender, MenuFlyoutItem).Name.ToLower

        Select Case sSenderName.Substring(2, 4)
            Case "arti"
                sTxt = App.mtGranyUtwor.oAudioParam.artist
                iInd = sTxt.IndexOfAny(",&")
                If iInd > -1 Then sTxt = sTxt.Substring(0, iInd)
            Case "titl"
                sTxt = App.mtGranyUtwor.oAudioParam.title
            Case "albu"
                sTxt = App.mtGranyUtwor.oAudioParam.album
        End Select

        If sTxt = "" Then Return

        If sSenderName.EndsWith("copy") Then
            vb14.ClipPut(sTxt)
        ElseIf sSenderName.EndsWith("search") Then
        ElseIf sSenderName.EndsWith("wiki") Then
            Dim sUrl As String = "https://en.wikipedia.org/wiki"
            sUrl = sUrl & "/" & sTxt
            Dim oUri = New Uri(sUrl)
            oUri.OpenBrowser()
        End If

    End Sub

#Region "BottomMenu"
#Region "BottomMenu-Settings"

    Private Sub uiLogShow_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()

        If App.gsLog.Length > 10000 Then
            App.gsLog = App.gsLog.Substring(App.gsLog.Length - 10000)
        End If
        vb14.DialogBox(App.gsLog)
    End Sub

    Private Async Sub uiLogClear_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()

        If Await vb14.DialogBoxYNAsync("Skasować log?") Then
            App.gsLog = ""
        End If
    End Sub

    Private Sub uiGoStat_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        Me.Navigate(GetType(OpenM3u))
    End Sub

    Private Sub uiGoNetStat_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        Me.Navigate(GetType(Siec))
    End Sub

    Private Sub uiGoDbaseFiles_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        Me.Navigate(GetType(DbaseAndStorage))
    End Sub
#End Region

    Private Sub uiGoSearch_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        Me.Navigate(GetType(Search))
    End Sub

    Private Sub uiGoLogin_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        Me.Navigate(GetType(Login))
    End Sub

    Private Sub uiGoNet_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()

        If Not NetIsIPavailable(False) Then
            vb14.DialogBox("Ale najpierw włącz Internety...")
            Return
        End If
        Me.Navigate(GetType(JamPilot))
    End Sub

    Private Sub uiGoGadaczka_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        Me.Navigate(GetType(SetupAudio))
    End Sub

#End Region

    Private Sub Page_Unloaded(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        RemoveHandler App.PilotChce, AddressOf PilotChce
    End Sub

    Private Sub PilotChce(sCmd As String)
        vb14.DumpCurrMethod()

        Select Case sCmd
            Case "next"
                GoNextSongUI()
            Case "pause"
                Grajek_PauseResume()
        End Select
    End Sub

    Private Async Sub uiUseMicro_Tapped(sender As Object, e As TappedRoutedEventArgs)
        uiUseMicro.SetSettingsBool()
        Await SpeechOnOffAsync(uiUseMicro.IsChecked)
    End Sub


End Class
