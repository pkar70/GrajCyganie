
' start: 2021.10.17

Partial Public NotInheritable Class MainPage
    Inherits Page

    Private Sub uiGoLogin_Click(sender As Object, e As RoutedEventArgs)
        Me.Frame.Navigate(GetType(Loginy))
    End Sub

    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        ProgRingInit(True, False)

        ProgRingShow(True)
        Dim sMsg As String = CosmosLogin()
        ProgRingShow(False)
        If Not String.IsNullOrEmpty(sMsg) Then Await DialogBoxAsync(sMsg)

    End Sub

    Private Function CosmosLogin() As String

        If App.gmCosmosClient Is Nothing Then
            App.gmCosmosClient = New Microsoft.Azure.Cosmos.CosmosClient(cosmosEndpointUri, cosmosPrimaryKeyRW)
        End If
        If App.gmCosmosClient Is Nothing Then Return "Cannot create dbase client"

        If App.gmCosmosDatabase Is Nothing Then
            App.gmCosmosDatabase = App.gmCosmosClient.GetDatabase("PKARweb")
        End If

        If App.gmCosmosDatabase Is Nothing Then Return "Cannot connect to dbase"

        Return ""
    End Function
End Class
