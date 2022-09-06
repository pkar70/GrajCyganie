Imports vb14 = Vblib.pkarlibmodule14

Public Module MediaGrajek
    Private moMediaPlayer As New Windows.Media.Playback.MediaPlayer

    Public Delegate Function _GoNextSongUI() As Task
    Private GoNextSongUI As _GoNextSongUI

    Private mbAfterInit As Boolean = False
    Private moLastNextDate As Date

    Public Sub Grajek_Init(oMPE As Windows.UI.Xaml.Controls.MediaPlayerElement, oGoNextSong As _GoNextSongUI)
        If oMPE Is Nothing Then Throw New ArgumentNullException("Grajek_Init MediaPlayerElement NULL")
        If oGoNextSong Is Nothing Then Throw New ArgumentNullException("Grajek_Init _GoNextSongUI NULL")

        oMPE.SetMediaPlayer(moMediaPlayer)
        GoNextSongUI = oGoNextSong

        If mbAfterInit Then Return

        AddHandler moMediaPlayer.MediaEnded, AddressOf MediaPlayer_MediaEnded
        AddHandler moMediaPlayer.MediaFailed, AddressOf MediaPlayer_MediaFailed

        mbAfterInit = True
    End Sub

    Public Function Grajek_GetCurrentDeviceName() As String
        Try
            Return moMediaPlayer.AudioDevice.Name
        Catch ex As Exception
            ' podczas grania nie da się?
        End Try
        Return "<nie teraz>"
    End Function

    Public Sub Grajek_SetDevice(newDevice As Windows.Devices.Enumeration.DeviceInformation)
        If moMediaPlayer Is Nothing Then Return
        If newDevice Is Nothing Then Return

        moMediaPlayer.AudioDevice = newDevice
    End Sub

    Public Function Grajek_GetMediaPlayer() As Windows.Media.Playback.MediaPlayer
        Return moMediaPlayer
    End Function

    Public Sub Grajek_SetSource(oMediaSource As Windows.Media.Core.MediaSource)
        If oMediaSource Is Nothing Then Return
        moMediaPlayer.Source = oMediaSource
    End Sub

    Public Sub Grajek_Play()
        moMediaPlayer.Play()
    End Sub
    Public Sub Grajek_Pause()
        moMediaPlayer.Pause()
    End Sub

    Public Sub Grajek_PauseResume()
        vb14.DumpCurrMethod()
        ' dla MediaPlayer
        Dim bPaused As Boolean = False

        If WinVer() > 14392 Then
            If moMediaPlayer.PlaybackSession.PlaybackState = Windows.Media.Playback.MediaPlaybackState.Paused Then bPaused = True
        Else
#Disable Warning BC40019 ' Property accessor is obsolete
#Disable Warning BC40000 ' Type or member is obsolete
            If moMediaPlayer.CurrentState = Windows.Media.Playback.MediaPlayerState.Paused Then bPaused = True
#Enable Warning BC40000 ' Type or member is obsolete
#Enable Warning BC40019 ' Property accessor is obsolete
        End If

        If bPaused Then
            Grajek_Play()
        Else
            Grajek_Pause()
        End If

    End Sub

    Public Sub Grajek_UstawKontrolki()
        With moMediaPlayer.SystemMediaTransportControls
            .IsNextEnabled = True
            '.IsPreviousEnabled = True
            .IsPauseEnabled = True
            .IsFastForwardEnabled = True
            '.IsPlayEnabled = True
            .IsEnabled = True
            With .DisplayUpdater
                .Type = Windows.Media.MediaPlaybackType.Music
                .MusicProperties.Artist = App.mtGranyUtwor.oAudioParam.artist
                .MusicProperties.Title = App.mtGranyUtwor.oAudioParam.title
            End With
        End With
        AddHandler moMediaPlayer.SystemMediaTransportControls.ButtonPressed, AddressOf SystemMediaControls_Button
    End Sub
    Private Async Sub SystemMediaControls_Button(sender As Windows.Media.SystemMediaTransportControls, args As Windows.Media.SystemMediaTransportControlsButtonPressedEventArgs)
        vb14.DumpCurrMethod()

        Select Case args.Button
            Case Windows.Media.SystemMediaTransportControlsButton.Next
                If moLastNextDate.AddSeconds(3) > Date.Now Then Exit Sub
                moLastNextDate = Date.Now
                Await GoNextSongUI()
            Case Windows.Media.SystemMediaTransportControlsButton.Pause
                Grajek_Pause()
        End Select

    End Sub

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

    Public Function Grajek_CzyGra() As Boolean
        If WinVer() > 14392 Then
            If moMediaPlayer.PlaybackSession.PlaybackState = Windows.Media.Playback.MediaPlaybackState.Playing OrElse
                    moMediaPlayer.PlaybackSession.PlaybackState = Windows.Media.Playback.MediaPlaybackState.Opening OrElse
                    moMediaPlayer.PlaybackSession.PlaybackState = Windows.Media.Playback.MediaPlaybackState.Buffering Then
                Return True
            End If
        Else
#Disable Warning BC40019 ' Property accessor is obsolete
#Disable Warning BC40000 ' Type or member is obsolete
            If moMediaPlayer.CurrentState = Windows.Media.Playback.MediaPlayerState.Playing OrElse
                    moMediaPlayer.CurrentState = Windows.Media.Playback.MediaPlayerState.Opening OrElse
                    moMediaPlayer.CurrentState = Windows.Media.Playback.MediaPlayerState.Buffering Then
#Enable Warning BC40000
#Enable Warning BC40019
                Return True
            End If
        End If

        Return False
    End Function
End Module
