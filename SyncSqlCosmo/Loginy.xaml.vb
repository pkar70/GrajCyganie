﻿
' https://docs.microsoft.com/en-us/azure/cosmos-db/sql/sql-api-get-started

Public NotInheritable Class Loginy
    Inherits Page

    Private mLoginy As New List(Of oneLogin)

    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        ProgRingInit(True, False)

        uiNewItem.Visibility = Visibility.Collapsed

        ProgRingShow(True)
        Dim sError As String = CosmosConnectLoginsy()
        If sError <> "" Then
            DialogBox(sError)
            Return
        End If

        Dim sMsg As String = Await LoadFromCosmos()
        ProgRingShow(False)
        If sMsg <> "" Then
            DialogBox("Cannot read data from dbase")
            Return
        End If

        DialogBox("Read " & mLoginy.Count & " logins")
        uiList.ItemsSource = mLoginy

    End Sub

    Private Async Sub uiAdd_Click(sender As Object, e As RoutedEventArgs)
        ' dodawanie danych

        If uiUserName.Text.Length < 5 Or uiUserName.Text.Length > 90 Then
            DialogBox("Za krotka badz za dluga nazwa usera")
            Return
        End If

        For Each oItem As oneLogin In mLoginy
            If oItem.sUserName = uiUserName.Text Then
                DialogBox("Ale taki user juz jest!")
                Return
            End If
        Next

        Dim mCosmosLoginContainer As Microsoft.Azure.Cosmos.Container = CosmosGetTableContainer("loginsy")
        If mCosmosLoginContainer Is Nothing Then
            DialogBox("container is null")
            Return
        End If

        Dim oNew As New oneLogin
        oNew.sUserName = uiUserName.Text
        oNew.sLimitPath = uiLimitPath.Text
        oNew.bAlsoPriv = uiAlsoPriv.IsOn
        oNew.bAlsoTajne = uiAlsoTajne.IsOn
        oNew.bNoLinks = uiNoLinks.IsOn
        oNew.id = oNew.sUserName

        ' dodaj do bazy Cosmos
        ' założenie: OnLoaded połączyło i w ogóle
        ProgRingShow(True)
        Await mCosmosLoginContainer.CreateItemAsync(Of oneLogin)(oNew)
        ProgRingShow(False)

        ' skoro udane, to mozesz dodac do listy ekranowej
        mLoginy.Add(oNew)
        uiList.ItemsSource = mLoginy

        ' i ukryj formatke (moze tego jednak nie robić, nie jestem pewien...)
        uiShowAdd.IsChecked = False
        uiNewItem.Visibility = Visibility.Collapsed

    End Sub

    Private Sub uiDelItem_Click(sender As Object, e As RoutedEventArgs)
        ' odejmowanie danych
        DialogBox("nie umiem - i może nigdy nie będę umieć")
    End Sub

    Private Sub uiShowAdd_Click(sender As Object, e As RoutedEventArgs)
        If uiShowAdd.IsChecked Then
            uiNewItem.Visibility = Visibility.Visible
        Else
            uiNewItem.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Async Function LoadFromCosmos() As Task(Of String)
        ' zakładam, że MainPage zrobiła klienta

        Dim mCosmosLoginContainer As Microsoft.Azure.Cosmos.Container = CosmosGetTableContainer("loginsy")
        If mCosmosLoginContainer Is Nothing Then Return "mCosmosLoginContainer is null"

        Dim sqlQueryText As String = "SELECT * FROM c"
        ' Dim sqlQueryDef As New Microsoft.Azure.Cosmos.QueryDefinition(sqlQueryText)

        Dim oIterator As Microsoft.Azure.Cosmos.FeedIterator(Of oneLogin) =
            mCosmosLoginContainer.GetItemQueryIterator(Of oneLogin)(sqlQueryText)

        While oIterator.HasMoreResults
            Dim currentResultSet As Microsoft.Azure.Cosmos.FeedResponse(Of oneLogin) = Await oIterator.ReadNextAsync()

            For Each oItem As oneLogin In currentResultSet
                mLoginy.Add(oItem)
            Next
        End While

        oIterator.Dispose()

        'Dim sError As String = CosmosConnectLoginsy()
        'If sError <> "" Then Return sError


        'Dim mCosmosLoginContainer As Microsoft.Azure.Cosmos.Container = Nothing

        'Dim sqlQueryText As String = "SELECT * FROM c"
        'Dim sqlQueryDef As New Microsoft.Azure.Cosmos.QueryDefinition(sqlQueryText)

        'Dim oIterator As Microsoft.Azure.Cosmos.FeedIterator(Of Object) = mCosmosLoginContainer.GetItemQueryIterator(Of Object)(sqlQueryText)

        'While oIterator.HasMoreResults
        '    Dim currentResultSet As Microsoft.Azure.Cosmos.FeedResponse(Of oneLogin) = Await oIterator.ReadNextAsync()

        '    For Each oItem As oneLogin In currentResultSet
        '        mLoginy.Add(oItem)
        '    Next
        'End While

        Return ""
    End Function

End Class



