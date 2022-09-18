

Public NotInheritable Class SearchPlyty
    Inherits Page

    Private Shared mLista As List(Of Vblib.oneAlbumForArtist)

    Private Async Sub uiSzukaj_Click(sender As Object, e As RoutedEventArgs)
        Dim sMask As String = uiArtist.Text
        If sMask.Length < 3 Then Return

        Me.ProgRingShow(True)
        mLista = Await App.inVb.GetCurrentDb.GetMusicAlbums(sMask)
        Me.ProgRingShow(False)

        uiLista.ItemsSource = mLista

    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Me.ProgRingInit(True, False)

        If App.mtGranyUtwor IsNot Nothing Then
            uiArtist.Text = App.mtGranyUtwor.oAudioParam.artist
        End If

        If mLista IsNot Nothing Then
            uiLista.ItemsSource = mLista
        End If

    End Sub
End Class
