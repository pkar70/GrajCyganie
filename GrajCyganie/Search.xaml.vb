Imports vb14 = Vblib.pkarlibmodule14
Imports Vblib.Extensions

Public NotInheritable Class Search
    Inherits Page

    ' Public Shared mPageWidth As Integer = -1
    Private Shared mLista As New List(Of Vblib.oneAudioParam)   ' shared: ponowne wejście daje że mamy, ale wtedy nie ma szukań kolejnych
    Private mSearch As String = ""

    Protected Overrides Sub onNavigatedTo(e As NavigationEventArgs)
        mSearch = e.Parameter?.ToString
        If mSearch Is Nothing Then mSearch = ""
    End Sub


    Private Sub uiCurrent_Click(sender As Object, e As RoutedEventArgs)
        If App.mtGranyUtwor Is Nothing Then Return
        uiArtist.Text = App.mtGranyUtwor.oAudioParam.artist
        uiTitle.Text = App.mtGranyUtwor.oAudioParam.title
        uiAlbum.Text = App.mtGranyUtwor.oAudioParam.album
        uiRok.Text = App.mtGranyUtwor.oAudioParam.year
    End Sub

    Private Async Sub uiSearch_Click(sender As Object, e As RoutedEventArgs)
        If (uiArtist.Text & uiTitle.Text & uiAlbum.Text & uiRok.Text).Length < 3 Then
            vb14.DialogBox("Ale że jak? czego szukasz?")
            Return
        End If

        Me.ProgRingShow(True)
        mLista = Await App.inVb.GetCurrentDb.SearchAsync(uiArtist.Text, uiTitle.Text, uiAlbum.Text, uiRok.Text)
        Me.ProgRingShow(False)

        PokazListe(mLista)
    End Sub

    Private Sub PokazListe(oLista As List(Of Vblib.oneAudioParam))
        uiFoundSummary.Text = ""

        If oLista Is Nothing Then Return

        Select Case meSortMode
            Case eSortMode.NoOrder
                uiListaSzeroka.ItemsSource = oLista
                uiListaWaska.ItemsSource = oLista
            Case eSortMode.ByArtist
                uiListaSzeroka.ItemsSource = From c In oLista Order By c.artist
                uiListaWaska.ItemsSource = From c In oLista Order By c.artist
            Case eSortMode.ByTitle
                uiListaSzeroka.ItemsSource = From c In oLista Order By c.title
                uiListaWaska.ItemsSource = From c In oLista Order By c.title
        End Select

        uiFoundSummary.Text = CreateSummary(oLista)
    End Sub

    Private Function CreateSummary(oLista As List(Of Vblib.oneAudioParam)) As String
        Dim iTotalSecs As Long = 0
        For Each oItem As Vblib.oneAudioParam In oLista
            iTotalSecs += oItem.duration
        Next

        Return $"Found: " & oLista.Count & If(oLista.Count = 200, "+", "") & " files, total playing time " & iTotalSecs.ToStringDHMS
    End Function

    Private Sub uiPage_SizeChanged(sender As Object, e As SizeChangedEventArgs)
        'mPageWidth = Me.ActualWidth
        uiListaSzeroka.Visibility = If(Me.ActualWidth > 800, Visibility.Visible, Visibility.Collapsed)
        uiListaWaska.Visibility = If(Me.ActualWidth <= 800, Visibility.Visible, Visibility.Collapsed)
    End Sub

    Private Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        Me.ProgRingInit(True, False)
        PokazListe(mLista)

        If Not String.IsNullOrWhiteSpace(mSearch) Then
            Dim aArr As String() = mSearch.Split("|")
            Select Case aArr(0).ToLowerInvariant
                Case "artist"
                    uiArtist.Text = aArr(1)
                Case "title"
                    uiTitle.Text = aArr(1)
                Case "album"
                    uiAlbum.Text = aArr(1)
            End Select
        End If
    End Sub

    Private Sub uiInfo_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Dim oItem As Vblib.oneAudioParam = TryCast(sender, FrameworkElement).DataContext

        Dim sTxt As String = $" 
Artist:{vbTab}{oItem.artist}
Title:{vbTab}{oItem.title}
Album:{vbTab}{oItem.album}
Track:{vbTab}{oItem.track}
Year:{vbTab}{oItem.year}
Dekada:{vbTab}{oItem.dekada}
Time:{vbTab}{oItem.duration.ToStringDHMS}
Bitrate:{vbTab}{oItem.bitrate}
Channels:{vbTab}{oItem.channels}
Sample:{vbTab}{oItem.sample}
VBR:{vbTab}{oItem.vbr}
ID:{vbTab}{oItem.id}
FileID:{vbTab}{oItem.fileID}
Comment: {oItem.comment}
"
        vb14.DialogBox(sTxt)
    End Sub

    Private Async Sub uiDoPlay_Tapped(sender As Object, e As TappedRoutedEventArgs)

        Dim oAudioParam As Vblib.oneAudioParam = TryCast(sender, FrameworkElement).DataContext
        If oAudioParam Is Nothing Then Return

        If Grajek_CzyGra() Then
            If Not Await vb14.DialogBoxYNAsync("Ale gram, na pewno przerwać?") Then Return
        End If

        Grajek_Pause()
        Dim oGranyUtwor As New Vblib.tGranyUtwor
        oGranyUtwor.oAudioParam = oAudioParam
        Await App.inVb.GetCurrentDb.RetrieveCountsyAsync(oGranyUtwor)
        oGranyUtwor.oStoreFile = Await App.inVb.GetCurrentDb.GetStoreFileAsync(oGranyUtwor.oAudioParam.fileID)
        oGranyUtwor.oStoreFile.path = IO.Path.Combine(oGranyUtwor.oStoreFile.path, oGranyUtwor.oStoreFile.name)

        App.mtGranyUtwor = oGranyUtwor

        Dim moMSource As Windows.Media.Core.MediaSource = Await MainPage.ZagrajPlik(App.mtGranyUtwor)
        If moMSource Is Nothing Then
            vb14.DialogBox("Ale tego pliku niestety nie mam dostępnego")
            Return
        End If

        Grajek_SetSource(moMSource)

        Grajek_Play()
        Me.GoBack ' zakładam że back to do mainpage

    End Sub

    Private Function GetTextBoxFromMFI(sender As Object) As TextBox
        Dim oFE As FrameworkElement = TryCast(sender, FrameworkElement)
        Select Case oFE.Name.ToLowerInvariant.Substring(2, 4)
            Case "arti"
                Return uiArtist
            Case "titl"
                Return uiTitle
            Case "albu"
                Return uiAlbum
        End Select
        Return Nothing
    End Function

    Private Function GetStringAttribFromMFI(sender As Object) As String
        Dim oFE As FrameworkElement = sender
        Dim oItem As Vblib.oneAudioParam = oFE?.DataContext

        Select Case oFE?.Name.ToLowerInvariant.Substring(2, 4)
            Case "arti"
                Return oItem.artist
            Case "titl"
                Return oItem.title
            Case "albu"
                Return oItem.album
        End Select
        Return ""
    End Function

    Private Sub uiMenuCopy_Click(sender As Object, e As RoutedEventArgs)
        vb14.ClipPut(GetStringAttribFromMFI(sender))
    End Sub

    Private Sub uiMenuWiki_Click(sender As Object, e As RoutedEventArgs)
        Dim sUrl As String = "https://en.wikipedia.org/wiki"
        sUrl = sUrl & "/" & GetStringAttribFromMFI(sender)
        Dim oUri = New Uri(sUrl)
        oUri.OpenBrowser()
    End Sub

    Private Sub uiMenuSearch_Click(sender As Object, e As RoutedEventArgs)
        vb14.DumpCurrMethod()

        uiArtist.Text = ""
        uiTitle.Text = ""
        uiAlbum.Text = ""
        uiRok.Text = ""

        GetTextBoxFromMFI(sender).Text = GetStringAttribFromMFI(sender)

    End Sub

    Private Enum eSortMode
        NoOrder
        ByArtist
        ByTitle
    End Enum

    Private meSortMode As eSortMode = eSortMode.NoOrder

    Private Sub uiSortArtist_Tap(sender As Object, e As TappedRoutedEventArgs)
        If mLista Is Nothing Then Return
        meSortMode = eSortMode.ByArtist
        PokazListe(mLista)
    End Sub

    Private Sub uiSortTitle_Tap(sender As Object, e As TappedRoutedEventArgs)
        If mLista Is Nothing Then Return
        If mLista Is Nothing Then Return
        meSortMode = eSortMode.ByTitle
        PokazListe(mLista)
    End Sub

    Private Sub uiFotosy_Click(sender As Object, e As RoutedEventArgs)
        ' *TODO* zrobienie fotosów
    End Sub

    Private Sub uiMask_Changed(sender As Object, e As TextChangedEventArgs)
        If Not uiSearchInside.IsChecked Then Return

        Dim oTBox As TextBox = sender
        Dim sQuery As String = oTBox.Text.ToUpperInvariant
        Select Case oTBox.Name.Substring(2, 3).ToLowerInvariant
            Case "art"
                PokazListe((From c In mLista Where c.artist.ToUpperInvariant.Contains(sQuery)).ToList)
            Case "tit"
                PokazListe((From c In mLista Where c.title.ToUpperInvariant.Contains(sQuery)).ToList)
            Case "alb"
                PokazListe((From c In mLista Where c.album.ToUpperInvariant.Contains(sQuery)).ToList)
            Case "rok"
                PokazListe((From c In mLista Where c.year.ToUpperInvariant.Contains(sQuery)).ToList)
        End Select
    End Sub
End Class

'Public Class KonwersjaZnikaniePol
'    Implements IValueConverter

'    Public Function Convert(ByVal value As Object,
'    ByVal targetType As Type, ByVal parameter As Object,
'    ByVal language As System.String) As Object _
'    Implements IValueConverter.Convert

'        ' ignorujemy binding jako taki, bierzemy tylko jego parametr
'        If Search.mPageWidth > 0 AndAlso Search.mPageWidth < 500 Then Return 0

'        If parameter Is Nothing Then Return 0
'        Dim sParam As String = CType(parameter, String)
'        Return Integer.Parse(sParam)

'    End Function


'    ' ConvertBack is not implemented for a OneWay binding.
'    Public Function ConvertBack(ByVal value As Object,
'    ByVal targetType As Type, ByVal parameter As Object,
'    ByVal language As System.String) As Object _
'    Implements IValueConverter.ConvertBack

'        Throw New NotImplementedException

'    End Function
'End Class

Public Class KonwersjaCzasu
    Implements IValueConverter

    Public Function Convert(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.Convert

        Dim seconds As Integer = CType(value, Integer)
        Return seconds.ToStringDHMS

    End Function


    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class

Public Class KonwersjaIntOrEmpty
    Implements IValueConverter

    Public Function Convert(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.Convert

        Dim temp As Integer = CType(value, Integer)
        If temp = 0 Then Return ""

        Return temp

    End Function


    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
    ByVal targetType As Type, ByVal parameter As Object,
    ByVal language As System.String) As Object _
    Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class