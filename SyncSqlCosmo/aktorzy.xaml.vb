' odpowiednik strony aktorzy.asp
' funkcjonalności:
' 1) znajdź aktora wedle nazwiska <- DONE
' 2) jakie filmy danego aktora już znamy
' 3) dla filmu tt (przed jego ściągnięciem) - kogo już widziałem i gdzie
' 4) ? dodawanie do CosmoDB aktorów filmu

Public NotInheritable Class aktorzy
    Inherits Page

    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        ProgRingInit(True, False)

        ProgRingShow(True)
        Dim sError As String = CosmosConnectActorNamesFiles()
        ProgRingShow(False)
        If sError <> "" Then
            DialogBox(sError)
            Return
        End If

        If gCountActors < 1 Then
            ProgRingShow(True)
            gCountActors = Await CosmosGetCountAsync("actorNames")
            ProgRingShow(False)
        End If

        uiStats.Text = "Znamy " & gCountActors.ToStringWithSpaces & " aktorów"

    End Sub

    Private Async Sub uiSearchName_Click(sender As Object, e As RoutedEventArgs)
        If uiName.Text.Length < 5 Then
            DialogBox("Wpisz dłuższy tekst!")
            Return
        End If

        Dim oLista As List(Of oneActorNames) = Await CosmosQueryActorNamesAsync(uiName.Text)
        uiListAktorzy.ItemsSource = oLista
        uiListAktorzy.Visibility = Visibility.Visible

        uiAktorTitle.Visibility = Visibility.Collapsed

        uiListFilmy.Visibility = Visibility.Collapsed
    End Sub


#Region "wykaz aktorów"

    Private Function Sender2ItemName(sender As Object) As oneActorNames
        Dim oMFI As MenuFlyoutItem = sender
        Return oMFI.DataContext
    End Function

    Private Sub uiShowFilmy_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As oneActorNames = Sender2ItemName(sender)
        WyszukajFilmyAktora(oItem)
    End Sub

    Private Sub uiOpenImdbName_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As oneActorNames = Sender2ItemName(sender)
        OpenBrowser("https://www.imdb.com/name/" & oItem.id.Trim & "/")
    End Sub

    Private Sub uiShowFilmy_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Dim oTB As TextBlock = sender
        Dim oItem As oneActorNames = oTB.DataContext
        WyszukajFilmyAktora(oItem)
    End Sub

#End Region

    Private Async Function WyszukajFilmyAktora(oItem As oneActorNames) As Task
        uiListAktorzy.Visibility = Visibility.Collapsed

        uiAktorTitle.Visibility = Visibility.Visible
        uiAktorTitle.Text = oItem.name

        ' 1) wyszukuj oItem.id.Trim jako StartsWith oneActorFilm.actorId
        ' 2) dla każdego oneActorFilm.filmId , znajdz Contains w oneStoreFiles
        ' 3) uzyj jeszcze oneActorFilm.postac

        Dim oLista As List(Of oneActorNames) = Await CosmosQueryActorNamesAsync(uiName.Text)

        uiListFilmy.ItemsSource = oLista
        uiListFilmy.Visibility = Visibility.Visible
    End Function

#Region "wykaz filmów"

#End Region


    Private Sub uiOpenImdbName_Click(sender As Object, e As TappedRoutedEventArgs)
        Dim oTB As TextBlock = sender
        Dim oItem As oneActorNames = oTB.DataContext
        OpenBrowser("https://www.imdb.com/name/" & oItem.id.Trim & "/")
    End Sub


    Public Async Function CosmosQueryActorNamesAsync(sWhere As String) As Threading.Tasks.Task(Of List(Of oneActorNames))

        Dim oRet As New List(Of oneActorNames)

        Dim sErr As String = CosmosConnectStoreFiles()
        If sErr <> "" Then
            Dim oMsg As New oneActorNames
            oMsg.name = "ERROR: " & sErr
            oRet.Add(oMsg)
            Return oRet
        End If

        Dim sQry As String = "SELECT * FROM c WHERE " & ConvertMask("name", sWhere) & " OFFSET 0 LIMIT 50"
        Dim oContainer As Microsoft.Azure.Cosmos.Container = CosmosGetTableContainer("actorNames")

        Dim oIterator As Microsoft.Azure.Cosmos.FeedIterator(Of oneActorNames) =
                oContainer.GetItemQueryIterator(Of oneActorNames)(sQry)

        While oIterator.HasMoreResults

            Dim currentResultSet As Microsoft.Azure.Cosmos.FeedResponse(Of oneActorNames) = Await oIterator.ReadNextAsync()

            For Each oItem As oneActorNames In currentResultSet
                oRet.Add(oItem)
            Next

        End While

        oIterator.Dispose()

        Return oRet

    End Function

End Class

Public Class KonwersjaActorIdNaUri
    Implements IValueConverter

    Public Function Convert(ByVal value As Object,
            ByVal targetType As Type, ByVal parameter As Object,
            ByVal language As System.String) As Object _
            Implements IValueConverter.Convert

        ' value is the data from the source object.
        Dim sId As String = CType(value, String)

        Return "https://www.imdb.com/name/" & sId.Trim & "/"


    End Function

    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
            ByVal targetType As Type, ByVal parameter As Object,
            ByVal language As System.String) As Object _
            Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class