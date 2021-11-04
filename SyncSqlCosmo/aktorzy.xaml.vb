' odpowiednik strony aktorzy.asp
' funkcjonalności:
' 1) znajdź aktora wedle nazwiska <- DONE
' 2) jakie filmy danego aktora już znamy
' 3) dla filmu tt (przed jego ściągnięciem) - kogo już widziałem i gdzie (IMDB)
' 4) ? dodawanie do CosmoDB aktorów filmu
' 5) aktorzy dla filmu ttxxx <-- wejście z FILMY.xaml

Public NotInheritable Class aktorzy
    Inherits Page

    Private mOpenParam As String = ""

    Protected Overrides Sub onNavigatedTo(e As NavigationEventArgs)
        If e.Parameter Is Nothing Then Return

        mOpenParam = e.Parameter.ToString
    End Sub

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


#Region "wykaz filmów"

    Private Sub uiGoFile_Tapped(sender As Object, e As TappedRoutedEventArgs)
        Dim oTB As TextBlock = sender
        Dim oItem As oneAktorWfilmie = oTB.DataContext
        ' Me.Frame.Navigate(GetType(filmy),oItem.oFilm.filmId)
    End Sub

    Private Async Function IdAktora2IdFilmow(oItem As oneActorNames) As Task(Of List(Of oneActorFilm))
        ' wyszukuj oItem.id.Trim jako StartsWith oneActorFilm.actorId

        Dim oRet As New List(Of oneActorFilm)

        Dim sError As String = CosmosConnectActorFilm()
        If sError <> "" Then
            DialogBox(sError)
            Return oRet
        End If

        ' Cosmos cost: 3 RU
        Dim sQry As String = "SELECT * FROM c WHERE StartsWith(c.actorId, '" & oItem.id.Trim & "') OFFSET 0 LIMIT 50"
        Dim oContainer As Microsoft.Azure.Cosmos.Container = CosmosGetTableContainer("actorFilm")

        Dim oIterator As Microsoft.Azure.Cosmos.FeedIterator(Of oneActorFilm) =
                oContainer.GetItemQueryIterator(Of oneActorFilm)(sQry)

        While oIterator.HasMoreResults

            Dim currentResultSet As Microsoft.Azure.Cosmos.FeedResponse(Of oneActorFilm) = Await oIterator.ReadNextAsync()

            For Each oItemFilm As oneActorFilm In currentResultSet
                oRet.Add(oItemFilm)
            Next

        End While

        oIterator.Dispose()

        Return oRet

    End Function



    Private Async Function WyszukajFilmyAktora(oItem As oneActorNames) As Task
        uiListAktorzy.Visibility = Visibility.Collapsed
        uiAktorTitle.Visibility = Visibility.Visible
        uiAktorTitle.Text = oItem.name
        uiListFilmy.Visibility = Visibility.Visible

        Dim sError As String = CosmosConnectStoreFiles()
        If sError <> "" Then
            DialogBox(sError)
            Return
        End If


        ProgRingShow(True)

        ' 1) wyszukuj oItem.id.Trim jako StartsWith oneActorFilm.actorId
        Dim oListaIdFilmow As List(Of oneActorFilm) = Await IdAktora2IdFilmow(oItem)

        ' 2) dla każdego oneActorFilm.filmId , znajdz Contains w oneStoreFiles
        Dim oListaPlikow As New List(Of oneAktorWfilmie)

        For Each oFilmId As oneActorFilm In oListaIdFilmow

            Dim sQry As String = "SELECT * FROM c WHERE Contains(c.name, '" & oFilmId.filmId.Trim & "') OFFSET 0 LIMIT 50"
            Dim oContainer As Microsoft.Azure.Cosmos.Container = CosmosGetTableContainer("storeFiles")

            Dim oIterator As Microsoft.Azure.Cosmos.FeedIterator(Of oneStoreFiles) =
                oContainer.GetItemQueryIterator(Of oneStoreFiles)(sQry)

            While oIterator.HasMoreResults

                Dim currentResultSet As Microsoft.Azure.Cosmos.FeedResponse(Of oneStoreFiles) = Await oIterator.ReadNextAsync()

                For Each oItemFilm As oneStoreFiles In currentResultSet
                    Dim oNewActorWfilmie As New oneAktorWfilmie
                    oNewActorWfilmie.oActor = oItem
                    oNewActorWfilmie.oFilm = oFilmId
                    oNewActorWfilmie.oFile = oItemFilm
                    oListaPlikow.Add(oNewActorWfilmie)
                    uiListFilmy.ItemsSource = From c In oListaPlikow
                Next

            End While

            oIterator.Dispose()

        Next


        ProgRingShow(False)

        'uiListFilmy.ItemsSource = Nothing
        'uiListFilmy.ItemsSource = oListaPlikow

    End Function

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

Public Class oneAktorWfilmie
    ' Inherits oneStoreFiles

    Public Property oFile As oneStoreFiles
    Public Property oFilm As oneActorFilm
    Public Property oActor As oneActorNames

    ' Public Class oneActorFilm
    'Public Property actorId As String    '": "nm0713421   ", <- uwaga: są spacje!
    'Public Property postac As String    '": "Fernand Jérôme",

    '' Public Class oneActorNames
    'Public Property actorname As String    '": "Dan Hanlon",

    'Public Sub New()

    'End Sub

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

Public Class KonwersjaPostaci
    Implements IValueConverter

    Public Function Convert(ByVal value As Object,
            ByVal targetType As Type, ByVal parameter As Object,
            ByVal language As System.String) As Object _
            Implements IValueConverter.Convert

        ' value is the data from the source object.
        Dim sId As String = CType(value, String)

        Return "(as " & sId.Trim & ")"


    End Function

    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
            ByVal targetType As Type, ByVal parameter As Object,
            ByVal language As System.String) As Object _
            Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class

Public Class KonwersjaFilmIdNaUri
    Implements IValueConverter

    Public Function Convert(ByVal value As Object,
            ByVal targetType As Type, ByVal parameter As Object,
            ByVal language As System.String) As Object _
            Implements IValueConverter.Convert

        ' value is the data from the source object.
        Dim sId As String = CType(value, String)

        Return "https://www.imdb.com/title/" & sId.Trim & "/"


    End Function

    ' ConvertBack is not implemented for a OneWay binding.
    Public Function ConvertBack(ByVal value As Object,
            ByVal targetType As Type, ByVal parameter As Object,
            ByVal language As System.String) As Object _
            Implements IValueConverter.ConvertBack

        Throw New NotImplementedException

    End Function
End Class

