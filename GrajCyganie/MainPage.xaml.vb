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


Imports Windows.Media

Public NotInheritable Class MainPage
    Inherits Page

    '    ---- MainPage ----
    '1. panel procentowy - wedle grup dekadowych (z cache pliku dekad)
    '2. informacja o granym pliku (z audioParams)
    '3. wybór grania (next, pause, stop...)
    '4. z cmdbar: czytacz (albo i na ekranie - ale tam chyba nie byłoby na to miejsca)

    ' SetSettingsInt("maxSoundId", sRes)

    Private miNextMode As Integer = 0
    Private miCoCzytam As Integer = 0
    Private moMSource As Windows.Media.Core.MediaSource
    'Private moMSource As Windows.Media.Playback.MediaPlaybackItem



#Region "Gadaczka"

    Private Function SpeakRozpoznajJezykStringu(sString As String) As String

        Dim sT As String = sString.ToLower
        If sT.IndexOf("ą") > -1 Then Return "pl-PL"
        If sT.IndexOf("ć") > -1 Then Return "pl-PL"
        If sT.IndexOf("ę") > -1 Then Return "pl-PL"
        If sT.IndexOf("ł") > -1 Then Return "pl-PL"
        If sT.IndexOf("ń") > -1 Then Return "pl-PL"
        If sT.IndexOf("ó") > -1 Then Return "pl-PL"
        If sT.IndexOf("ś") > -1 Then Return "pl-PL"
        If sT.IndexOf("ż") > -1 Then Return "pl-PL"
        If sT.IndexOf("ź") > -1 Then Return "pl-PL"

        Return ""
    End Function

    Private Async Function SpeakOdczytajString(sString As String, sLang As String) As Task
        vb14.DumpCurrMethod(sString & ", " & sLang)
        Dim oSynth As Windows.Media.SpeechSynthesis.SpeechSynthesizer = New Windows.Media.SpeechSynthesis.SpeechSynthesizer()
        ' 15063 ma oSynth.Options którymi warto byłoby sie pobawić

        Dim bFound As Boolean = False
        Dim cGender As Windows.Media.SpeechSynthesis.VoiceGender = Windows.Media.SpeechSynthesis.VoiceGender.Female
        If vb14.GetSettingsBool("sexZapowiedzi") Then cGender = Windows.Media.SpeechSynthesis.VoiceGender.Male

        For Each oVoice As Windows.Media.SpeechSynthesis.VoiceInformation In oSynth.AllVoices
            If oVoice.Language = sLang AndAlso oVoice.Gender = cGender Then
                oSynth.Voice = oVoice
                bFound = True
                Exit For
            End If
        Next

        ' nie ma tej płci, ale moze innej płci jest?
        If Not bFound Then
            For Each oVoice As Windows.Media.SpeechSynthesis.VoiceInformation In oSynth.AllVoices
                If oVoice.Language = sLang Then
                    oSynth.Voice = oVoice
                    bFound = True
                    Exit For
                End If
            Next
        End If

        ' a jeśli nie znalazł, to idzie wedle default voice

        Dim oStream As Windows.Media.SpeechSynthesis.SpeechSynthesisStream = Await oSynth.SynthesizeTextToStreamAsync(sString)

        Dim oMSource As Windows.Media.Core.MediaSource
        oMSource = Windows.Media.Core.MediaSource.CreateFromStream(oStream, oStream.ContentType)
        App.moMediaPlayer.Source = oMSource

        App.moMediaPlayer.Play()

    End Function

    Private Function CreateWinTitle() As String
        vb14.DumpCurrMethod()
        Dim sTxt As String = ""
        If miNextMode <> 1 Then sTxt = App.mtGranyUtwor.oAudioParam.artist
        If miNextMode <> 2 Then
            If sTxt <> "" Then sTxt = sTxt & ": "
            sTxt = sTxt & App.mtGranyUtwor.oAudioParam.title
        End If

        Return sTxt
    End Function

    Private Async Function SpeakOdczytaj() As Task
        vb14.DumpCurrMethod()
        If miNextMode = 6 Then Exit Function

        Dim sLang As String
        sLang = SpeakRozpoznajJezykStringu(App.mtGranyUtwor.oAudioParam.artist)
        If sLang = "" Then sLang = SpeakRozpoznajJezykStringu(App.mtGranyUtwor.oAudioParam.title)
        If sLang = "" Then
            Dim sT As String = App.mtGranyUtwor.oStoreFile.path.ToLower
            If sT.IndexOf("/pol/") > -1 Then sLang = "pl-PL"
            If sT.IndexOf("/polcd/") > -1 Then sLang = "pl-PL"
            If sT.IndexOf("/winylownia/") > -1 Then sLang = "pl-PL" ' choć nie tylko polskie tam są
            If sT.IndexOf("/polskie/") > -1 Then sLang = "pl-PL"
        End If

        If sLang = "" Then sLang = "en-US" ' default

        ' artis: title
        ' ale gdy loop/lock, wtedy niekoniecznie
        Dim sTxt As String = CreateWinTitle()

        If sTxt <> "" Then Await SpeakOdczytajString(sTxt, sLang)

    End Function
#End Region

    Private Sub WypelnPoleZapetlacza(oRadio As RadioButton, iCount As Integer)
        If iCount > 999 Then
            oRadio.Content = "*"
        Else
            oRadio.Content = iCount
        End If
        oRadio.IsEnabled = (iCount > 1)
    End Sub

    Private Sub WypelnijPola()
        vb14.DumpCurrMethod()

        Try
            If App.mtGranyUtwor Is Nothing Then
                vb14.CrashMessageAdd("@WypelnijPola", "Empty App.mtGranyUtwor")
                Return
            End If

            uiArtist.Text = App.mtGranyUtwor.oAudioParam.artist
            WypelnPoleZapetlacza(uiArtist_Radio, App.mtGranyUtwor.countArtist)

            uiTitle.Text = App.mtGranyUtwor.oAudioParam.title
            WypelnPoleZapetlacza(uiTitle_Radio, App.mtGranyUtwor.countTitle)

            If App.mtGranyUtwor.oAudioParam.track <> "" Then
                uiAlbum.Text = App.mtGranyUtwor.oAudioParam.track & " z: " & App.mtGranyUtwor.oAudioParam.album
            Else
                uiAlbum.Text = App.mtGranyUtwor.oAudioParam.album
            End If
            WypelnPoleZapetlacza(uiAlbum_Radio, App.mtGranyUtwor.countAlbum)

            uiRok.Text = App.mtGranyUtwor.oAudioParam.year
            WypelnPoleZapetlacza(uiRok_Radio, App.mtGranyUtwor.countYear)

            uiDekada.Text = App.mtGranyUtwor.oAudioParam.dekada
            WypelnPoleZapetlacza(uiDekada_Radio, App.mtGranyUtwor.countDekada)

            uiComment.Text = App.mtGranyUtwor.oAudioParam.comment

            If App.inVb._dekady.GetList Is Nothing Then
                uiSlider.Visibility = Visibility.Collapsed
                uiSliderInfo.Visibility = Visibility.Collapsed
            Else
                uiSlider.Visibility = Visibility.Visible
                uiSliderInfo.Visibility = Visibility.Visible
                For Each oItem As Vblib.tDekada In App.inVb._dekady.GetList
                    If oItem.sNazwa = App.mtGranyUtwor.oAudioParam.dekada Then
                        uiSlider.Value = oItem.iFreq
                        uiSliderInfo.Text = oItem.GetFreqString
                        Exit For
                    End If
                Next
            End If

            Dim iMiB As Integer = App.mtGranyUtwor.oStoreFile.len
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

            Try
                ApplicationView.GetForCurrentView.Title = CreateWinTitle()
            Catch ex As Exception

            End Try


            'Public Property ID As Integer
            'Public Property bitrate As Integer
            'Public Property duration As Integer
            'Public Property channels As String
            'Public Property sample As Integer
            'Public Property uri As String
            'Public Property flen As Integer
        Catch ex As Exception
            vb14.CrashMessageAdd("@WypelnijPola", ex)
        End Try

    End Sub

    Private Async Sub SNM_CloseRequested(sender As Object, e As Windows.UI.Core.Preview.SystemNavigationCloseRequestedPreviewEventArgs)
        vb14.DumpCurrMethod()
        Dim oDef As Deferral = e.GetDeferral
        ' tylko do systray
        'If (Metadata.ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0)) Then
        '    Await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync()
        'End If

        e.Handled = False
        oDef.Complete()

    End Sub

    'Private Sub ConnectDbase()
    '    Dim sSelected As String = vb14.GetSettingsString("usingDB")

    '    If (New dbase_beskidAsp).Nazwa = sSelected Then
    '        App.goDbase = New dbase_beskidAsp
    '        Return
    '    End If

    '    If (New dbase_localASP).Nazwa = sSelected Then
    '        App.goDbase = New dbase_localASP
    '        Return
    '    End If

    '    If App.goDbase Is Nothing Then App.goDbase = New dbase_localASP

    'End Sub

    Private Sub ConnectFiles()
        Dim sSelected As String = vb14.GetSettingsString("usingFS")

        If (New Storage_Local).Nazwa = sSelected Then
            App.goStorage = New Storage_Local
            Return
        End If

        If App.goStorage Is Nothing Then App.goStorage = New Storage_Local

    End Sub



    Private Sub ConnectServices()
        'ConnectDbase()
        ConnectFiles()


    End Sub

    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        moLastNextDate = Date.Now

        ConnectServices()

        If Not IsFamilyMobile() Then
            ApplicationView.GetForCurrentView().SetPreferredMinSize(New Size(310, 400))
        End If

        AddHandler Windows.UI.Core.Preview.SystemNavigationManagerPreview.GetForCurrentView().CloseRequested, AddressOf SNM_CloseRequested

        If App.moMediaPlayer Is Nothing Then
            App.moMediaPlayer = New Windows.Media.Playback.MediaPlayer
            AddHandler App.moMediaPlayer.MediaEnded, AddressOf MediaPlayer_MediaEnded
            AddHandler App.moMediaPlayer.MediaFailed, AddressOf MediaPlayer_MediaFailed
            uiGrajek.SetMediaPlayer(App.moMediaPlayer)
        End If

        uiReadAfter.IsChecked = vb14.GetSettingsBool("uiReadAfter")
        uiReadBefore.IsChecked = vb14.GetSettingsBool("uiReadBefore")

        If Not Await App.inVb.GetCurrentDb.LoginAsync(True) Then
            uiGoLogin.Visibility = Visibility.Visible
            uiGoSetting.Visibility = Visibility.Collapsed
            uiGoAudio.Visibility = Visibility.Collapsed
            uiGoSearch.Visibility = Visibility.Collapsed
            Exit Sub
        End If

        uiGoLogin.Visibility = Visibility.Collapsed
        uiGoSetting.Visibility = Visibility.Visible
        uiGoAudio.Visibility = Visibility.Visible
        uiGoSearch.Visibility = Visibility.Visible

        Await App.inVb.GetCurrentDb.ReloadDekadyAsync(False)

        If App.moReco Is Nothing Then
            App.moReco = New Windows.Media.SpeechRecognition.SpeechRecognizer()
            App.SpeechCommandCreateRules()
            Await App.moReco.CompileConstraintsAsync()
            App.SpeechCommandSetTimeouts()

            AddHandler App.moReco.HypothesisGenerated, AddressOf recoNewHypo
            AddHandler App.moReco.ContinuousRecognitionSession.ResultGenerated, AddressOf recoGetText

        End If

        If App.moReco.State = Windows.Media.SpeechRecognition.SpeechRecognizerState.Idle Then
            Try
                Await App.moReco.ContinuousRecognitionSession.StartAsync()
            Catch ex As Exception
            End Try
        End If

        GetSettingsBool(uiUseMicro) ' to przy okazji włączy (bo będzie ToggleButton.Tapped?)


        If App.mtGranyUtwor Is Nothing Then Exit Sub

        ' If Not Await App.BeskidGetDekady(False) Then Exit Sub - do ustawiania slider dekady jest potrzebne

        WypelnijPola()
        ZaznaczRadio(0)

        Dim bGramy As Boolean = False
        If WinVer() > 14392 Then
            If App.moMediaPlayer.PlaybackSession.PlaybackState = Windows.Media.Playback.MediaPlaybackState.Playing OrElse
                    App.moMediaPlayer.PlaybackSession.PlaybackState = Windows.Media.Playback.MediaPlaybackState.Opening OrElse
                    App.moMediaPlayer.PlaybackSession.PlaybackState = Windows.Media.Playback.MediaPlaybackState.Buffering Then
                bGramy = True
            End If
        Else
#Disable Warning BC40019 ' Property accessor is obsolete
#Disable Warning BC40000 ' Type or member is obsolete
            If App.moMediaPlayer.CurrentState = Windows.Media.Playback.MediaPlayerState.Playing OrElse
                    App.moMediaPlayer.CurrentState = Windows.Media.Playback.MediaPlayerState.Opening OrElse
                    App.moMediaPlayer.CurrentState = Windows.Media.Playback.MediaPlayerState.Buffering Then
#Enable Warning BC40000
#Enable Warning BC40019
                bGramy = True
            End If
        End If

        If bGramy Then
            uiStart.Visibility = Visibility.Collapsed
            uiGrajek.Visibility = Visibility.Visible
            uiLoop_Radio.Visibility = Visibility.Visible
            uiNext.Visibility = Visibility.Visible
            uiGrajek.SetMediaPlayer(App.moMediaPlayer)

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

    Private Sub EventGuzikaMediaPlayer(sender As SystemMediaTransportControls, args As SystemMediaTransportControlsButtonPressedEventArgs)
        vb14.DumpCurrMethod()
        Debug.WriteLine("guzik media player")
        'Select Case args.Button
        '    Case SystemMediaTransportControlsButton.Next
        '        Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, AddressOf GoNextSongSub)
        'End Select
    End Sub
#Region "voice commands"

    Private Shared msVoiceCmd As String

    Private Async Sub SpeechCommand(sTag As String, sTxt As String)
        vb14.DumpCurrMethod(sTag & ", " & sTxt)
        If sTag <> "" Then
            msVoiceCmd = sTag
        Else
            msVoiceCmd = App.SpeechCommandText2Tag(sTxt)
        End If

        If msVoiceCmd = "" Then Exit Sub
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, AddressOf recoDoCommand)
    End Sub

    Private Sub recoDoCommand()
        vb14.DumpCurrMethod("msVoiceCmd=" & msVoiceCmd)
        Select Case msVoiceCmd
            Case "play"
                uiStart_Click(Nothing, Nothing)
            Case "pause"
                App.moMediaPlayer.Pause()
            Case "cont"
                App.moMediaPlayer.Play()
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

    Private Sub recoNewHypo(sender As Windows.Media.SpeechRecognition.SpeechRecognizer, args As Windows.Media.SpeechRecognition.SpeechRecognitionHypothesisGeneratedEventArgs)
        vb14.DumpCurrMethod()
        SpeechCommand("", args.Hypothesis.Text)
    End Sub

    Private Sub recoGetText(sender As Windows.Media.SpeechRecognition.SpeechContinuousRecognitionSession, args As Windows.Media.SpeechRecognition.SpeechContinuousRecognitionResultGeneratedEventArgs)
        vb14.DumpCurrMethod()
        Dim sTxt As String
        If args.Result.Constraint IsNot Nothing Then
            sTxt = args.Result.Constraint.Tag
        Else
            sTxt = ""
        End If
        SpeechCommand(sTxt, args.Result.Text)

    End Sub

#End Region





#Region "Zapetlenia"


    Private Sub ZaznaczRadio(iMode As Integer)
        vb14.DumpCurrMethod()
        If miNextMode = iMode Then iMode = 0

        uiArtist_Radio.IsChecked = If(iMode = 1, True, False)
        uiTitle_Radio.IsChecked = If(iMode = 2, True, False)
        uiAlbum_Radio.IsChecked = If(iMode = 3, True, False)
        uiRok_Radio.IsChecked = If(iMode = 4, True, False)
        uiDekada_Radio.IsChecked = If(iMode = 5, True, False)
        uiLoop_Radio.IsChecked = If(iMode = 6, True, False)

        miNextMode = iMode
    End Sub

    Private Sub uiArtist_Radio_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiArtist_Radio.Tapped
        ZaznaczRadio(1)
    End Sub

    Private Sub uiTitle_Radio_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiTitle_Radio.Tapped
        ZaznaczRadio(2)
    End Sub

    Private Sub uiAlbum_Radio_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiAlbum_Radio.Tapped
        ZaznaczRadio(3)
    End Sub

    Private Sub uiRok_Radio_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiRok_Radio.Tapped
        ZaznaczRadio(4)
    End Sub

    Private Sub uiDekada_Radio_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiDekada_Radio.Tapped
        ZaznaczRadio(5)
    End Sub

    Private Sub uiLoop_Radio_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiLoop_Radio.Tapped
        ZaznaczRadio(6)
    End Sub
#End Region

    Private Async Function GoNextSong() As Task(Of Boolean)
        Dim sDOut As String = ""
        If App.mtGranyUtwor IsNot Nothing Then
            sDOut = "GoNextSong, miNextMode=" & miNextMode & ", miCoCzytam=" & miCoCzytam & ", artist=" & App.mtGranyUtwor.oAudioParam.artist & vbCrLf
        Else
            sDOut = "GoNextSong, miNextMode=" & miNextMode & ", miCoCzytam=" & miCoCzytam & ", artist=NULL" & vbCrLf
        End If
        App.gsLog = App.gsLog & sDOut & vbCrLf
        vb14.DebugOut("GoNextSong: " & sDOut)

        If miNextMode <> 6 Then

            ' = 3, czyli: zakonczone granie utworu
            If miCoCzytam = 3 Then
                miCoCzytam = 2
                If uiReadAfter.IsChecked Then
                    Await SpeakOdczytaj()
                    Return True
                End If
            End If

            ' =2, czyli po zakonczeniu czytania informacji o poprzednim - wylosuj nastepny utwor
            If miCoCzytam = 2 Or miCoCzytam = 0 Then

                Dim bFound As Boolean = False
                Dim iGuard As Integer = 50

                While Not bFound AndAlso iGuard > 0
                    iGuard = iGuard - 1
                    If iGuard < 1 Then
                        vb14.DialogBox("Przewidziane niemożliwe nr 1")
                        Return False
                    End If

                    Dim oNextSong As Vblib.tGranyUtwor = Await App.inVb.GetCurrentDb.GetNextSongAsync(miNextMode, App.mtGranyUtwor)
                    If oNextSong Is Nothing Then Return False
                    App.mtGranyUtwor = oNextSong

                    App.gsLog = App.gsLog & "GoNextSong, wylosowany " & App.mtGranyUtwor.oAudioParam.id & vbCrLf

                    If miNextMode = 0 Then
                        Dim iCnt As Integer = vb14.GetSettingsInt("dekada" & App.mtGranyUtwor.oAudioParam.dekada)
                        iCnt = iCnt - 1
                        If iCnt > 1 Then
                            ' jeszcze nie
                            vb14.SetSettingsInt("dekada" & App.mtGranyUtwor.oAudioParam.dekada, iCnt)
                            App.gsLog = App.gsLog & "GoNextSong, ale nie każdy z tej dekady! " & vbCrLf
                            bFound = False
                        Else
                            ' juz zagraj
                            iCnt = 1
                            For Each oItem As Vblib.tDekada In App.inVb._dekady.GetList
                                If oItem.sNazwa = App.mtGranyUtwor.oAudioParam.dekada Then
                                    iCnt = oItem.GetCounterValue
                                    Exit For
                                End If
                            Next
                            vb14.SetSettingsInt("dekada" & App.mtGranyUtwor.oAudioParam.dekada, iCnt)
                            bFound = True
                        End If
                    Else
                        bFound = True
                    End If
                End While


                ' App.mtGranyUtwor.ID ustawiony wczesniej (w ExtractCyganInfo)
                ' to robimy tylko raz, zas cygan-info moze isc kilka razy (bo nie kazdy z tej dekady jest do pokazania)
                Await App.inVb.GetCurrentDb.RetrieveCountsyAsync(App.mtGranyUtwor)

                'Dim sPage As String = Await App.HttpPageAsync("/cygan-counts.asp?id=" & App.mtGranyUtwor.oAudioParam.id, "song stats")
                'ExtractCyganCounts(sPage) ' uzupełnia count* w App.mtGranyUtwor

                WypelnijPola()

                miCoCzytam = 1
                If uiReadBefore.IsChecked Then
                    SpeakOdczytaj()
                    Return True
                End If

            End If    ' wylosowano juz nastepny utwor, ewentualnie zapowiedziano

        End If ' if LoopUtworu

        If miCoCzytam = 1 Or miNextMode = 6 Then
            'Dim sTxt As String = App.mtGranyUtwor.uri

            'sTxt = sTxt.Replace("#", "%23")  ' to jest tylko proba - teraz moze podwojnie escapeowa?
            'Dim oUri As Uri = New Uri(App.BaseUri & "/p" & sTxt)   ' sTxt = "/store/.... "
            '' Dim oMSource As Windows.Media.Core.MediaSource
            moMSource = Await App.goStorage.GetMediaSourceFrom(App.mtGranyUtwor.oStoreFile)
            If moMSource Is Nothing Then Return False

            App.moMediaPlayer.Source = moMSource
            Try
                App.moMediaPlayer.Play()
            Catch ex As Exception
                ' chyba sie nie powinno zdarzyc...
            End Try
            miCoCzytam = 3
            'uiGrajek.SetMediaPlayer(App.moMediaPlayer)

            Await Task.Delay(1000)

            With App.moMediaPlayer.SystemMediaTransportControls
                .IsNextEnabled = True
                '.IsPreviousEnabled = True
                .IsPauseEnabled = True
                .IsFastForwardEnabled = True
                '.IsPlayEnabled = True
                .IsEnabled = True
                With .DisplayUpdater
                    .Type = MediaPlaybackType.Music
                    .MusicProperties.Artist = App.mtGranyUtwor.oAudioParam.artist
                    .MusicProperties.Title = App.mtGranyUtwor.oAudioParam.title
                End With
            End With
            AddHandler App.moMediaPlayer.SystemMediaTransportControls.ButtonPressed, AddressOf SystemMediaControls_Button
        End If


        'AddHandler App.moMediaPlayer.MediaEnded, AddressOf MediaPlayer_MediaEnded
        'AddHandler App.moMediaPlayer.MediaFailed, AddressOf MediaPlayer_MediaFailed

        Return True
    End Function

    Private moLastNextDate As Date

    Private Async Sub SystemMediaControls_Button(sender As SystemMediaTransportControls, args As SystemMediaTransportControlsButtonPressedEventArgs)
        vb14.DumpCurrMethod()

        Select Case args.Button
            Case SystemMediaTransportControlsButton.Next
                If moLastNextDate.AddSeconds(3) > Date.Now Then Exit Sub
                moLastNextDate = Date.Now
                Await GoNextSongUI()
            Case SystemMediaTransportControlsButton.Pause
                App.moMediaPlayer.Pause()
        End Select

    End Sub

    Private Async Sub GoNextSongSub()
        vb14.DumpCurrMethod()
        Await GoNextSong()
    End Sub

    Private Async Function GoNextSongUI() As Task
        vb14.DumpCurrMethod()
        Await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, AddressOf GoNextSongSub)
    End Function

    Private Async Sub MediaPlayer_MediaEnded(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        Await GoNextSongUI()
    End Sub
    Private Async Sub MediaPlayer_MediaFailed(sender As Object, e As Windows.Media.Playback.MediaPlayerFailedEventArgs)
        vb14.DumpCurrMethod()

        Try
            Await vb14.DialogBoxAsync("failed")
        Catch ex As Exception
            ' moze wtedy wylatuje z errorem?? moze nie moze byc dialogbox?
        End Try
        Await GoNextSongUI()
    End Sub
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

        For Each oDekada As Vblib.tDekada In App.inVb._dekady.GetList
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

    'Private Sub uiPole_Tapped(sender As Object, e As RightTappedRoutedEventArgs) Handles uiTitle.RightTapped, uiAlbum.RightTapped, uiArtist.RightTapped
    '    Dim sUrl As String = "https://en.wikipedia.org/wiki"
    '    Dim sTxt As String = ""

    '    Dim oTBox As TextBlock = TryCast(sender, TextBlock)
    '    Dim iInd As Integer

    '    Select Case oTBox.Name
    '        Case "uiArtist"
    '            sTxt = App.mtGranyUtwor.artist
    '            iInd = sTxt.IndexOfAny(",&")
    '            If iInd > -1 Then sTxt = sTxt.Substring(0, iInd)
    '        Case "uiTitle"
    '            sTxt = App.mtGranyUtwor.title
    '        Case "uiAlbum"
    '            sTxt = App.mtGranyUtwor.album
    '    End Select

    '    If sTxt = "" Then Exit Sub

    '    sUrl = sUrl & "/" & sTxt

    '    App.OpenBrowser(sUrl, False)

    'End Sub

    Private Sub uiComment_Tapped(sender As Object, e As TappedRoutedEventArgs) Handles uiComment.Tapped
        Dim oMPI As Windows.Media.Playback.MediaPlaybackItem = Windows.Media.Playback.MediaPlaybackItem.FindFromMediaSource(moMSource)



    End Sub

    Private Sub uiRadio_AccessKeyInvoked(sender As UIElement, args As AccessKeyInvokedEventArgs) Handles uiLoop_Radio.AccessKeyInvoked, uiAlbum_Radio.AccessKeyInvoked, uiArtist_Radio.AccessKeyInvoked, uiTitle_Radio.AccessKeyInvoked, uiDekada_Radio.AccessKeyInvoked, uiRok_Radio.AccessKeyInvoked
        vb14.DumpCurrMethod()
        Dim oRadio As RadioButton = TryCast(sender, RadioButton)
        If oRadio.IsChecked Then
            oRadio.IsChecked = False
            args.Handled = True
        End If
    End Sub

    Private Sub GrajekPauseResume()
        vb14.DumpCurrMethod()
        ' dla MediaPlayer
        Dim bPaused As Boolean = False

        If WinVer() > 14392 Then
            If App.moMediaPlayer.PlaybackSession.PlaybackState = Windows.Media.Playback.MediaPlaybackState.Paused Then bPaused = True
        Else
#Disable Warning BC40019 ' Property accessor is obsolete
#Disable Warning BC40000 ' Type or member is obsolete
            If App.moMediaPlayer.CurrentState = Windows.Media.Playback.MediaPlayerState.Paused Then bPaused = True
#Enable Warning BC40000 ' Type or member is obsolete
#Enable Warning BC40019 ' Property accessor is obsolete
        End If

        If bPaused Then
            App.moMediaPlayer.Play()
        Else
            App.moMediaPlayer.Pause()
        End If

    End Sub

    Private Sub uiGrajek_AccessKeyInvoked(sender As UIElement, args As AccessKeyInvokedEventArgs) Handles uiGrajek.AccessKeyInvoked
        vb14.DumpCurrMethod()
        GrajekPauseResume()
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
        Me.Frame.Navigate(GetType(JamPilot))
    End Sub

    Private Sub uiGoAudio_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()
        Me.Frame.Navigate(GetType(Audio))
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
                GrajekPauseResume()
        End Select
    End Sub

    Private Async Sub uiUseMicro_Tapped(sender As Object, e As TappedRoutedEventArgs)
        SetSettingsBool(uiUseMicro)

        Try

            If uiUseMicro.IsChecked Then
                Await App.moReco.ContinuousRecognitionSession.StartAsync()
            Else
                Await App.moReco.ContinuousRecognitionSession.StopAsync()
            End If

        Catch ex As Exception
        End Try

    End Sub

End Class
