Partial Module modulek

    Private gmCosmosClient As Microsoft.Azure.Cosmos.CosmosClient = Nothing
    Private gmCosmosDatabase As Microsoft.Azure.Cosmos.Database = Nothing
    Private mCosmosLoginContainer As Microsoft.Azure.Cosmos.Container = Nothing

    Public Async Function CheckLoginAsync(oPage As Page) As Threading.Tasks.Task(Of String)

        If oPage.Session("loginLock") > 3 Then Return "ERROR: session lockout"

        Dim sLoginName As String = oPage.Request("user")
        If sLoginName <> "" Then
            ' user override
            oPage.Session("loggedUser") = ""
            oPage.Session("sLimitPath") = "NAPEWNONICTAKIEGONIEMA"
            oPage.Session("bAlsoPriv") = False
            oPage.Session("bAlsoTajne") = False
        End If

        If oPage.Session("loggedUser") <> "" Then
            oPage.Session("loginLock") = 0
            Return ""
        End If

        If sLoginName = "" Then sLoginName = oPage.Request.Cookies.Get("cyganUserName").Value

        If sLoginName = "" Then sLoginName = oPage.Request("user")

        If sLoginName = "" Then Return "ERROR: no login provided"

        Dim lastLoginTry As String = oPage.Application("lastLoginTry")
        Dim currLoginTry As String = DateTime.Now.ToString("MMddHHmmss")
        If lastLoginTry = currLoginTry Then Return "ERROR: too many login requests"

        oPage.Application("lastLoginTry") = currLoginTry

        Dim sCosmosError As String = CosmosConnect()
        If sCosmosError <> "" Then Return sCosmosError

        sCosmosError = CosmosGetkLoginsy()
        If sCosmosError <> "" Then Return sCosmosError

        oPage.Session("loginLock") = oPage.Session("loginLock") + 1
        If Not Await CosmosLoginPermissionToSessionAsync(oPage, sLoginName) Then Return "ERROR: brak permissionów"

        Dim oCookie As New HttpCookie("cyganUserName", sLoginName)
        oCookie.Expires = Date.Now.AddDays(90)
        oPage.Response.Cookies.Set(oCookie)

        Return ""   ' OK
    End Function

    Private Async Function CosmosLoginPermissionToSessionAsync(oPage As Page, sLogin As String) As Threading.Tasks.Task(Of Boolean)
        ' Page potrzebny dla Session

        Dim sqlQueryText As String = "SELECT * FROM c WHERE c.id = '" & sLogin & "'"
        Dim sqlQueryDef As New Microsoft.Azure.Cosmos.QueryDefinition(sqlQueryText)

        Dim oIterator As Microsoft.Azure.Cosmos.FeedIterator(Of oneLogin) = mCosmosLoginContainer.GetItemQueryIterator(Of oneLogin)(sqlQueryDef)

        While oIterator.HasMoreResults
            Dim currentResultSet As Microsoft.Azure.Cosmos.FeedResponse(Of oneLogin) = Await oIterator.ReadNextAsync()

            For Each oItem As oneLogin In currentResultSet
                ' ale wystarczy nam tylko jeden
                oPage.Session("loggedUser") = oItem.id
                oPage.Session("sLimitPath") = oItem.sLimitPath
                oPage.Session("bAlsoPriv") = oItem.bAlsoPriv
                oPage.Session("bAlsoTajne") = oItem.bAlsoTajne
                Return True
            Next
        End While

        Return False
    End Function

    Private Function CosmosGetkLoginsy() As String
        If gmCosmosDatabase Is Nothing Then Return "Cannot connect to dbase"

        If mCosmosLoginContainer Is Nothing Then
            mCosmosLoginContainer = gmCosmosDatabase.GetContainer("loginsy")
        End If

        If mCosmosLoginContainer Is Nothing Then Return "cannot get container"

        Return ""

    End Function

    Private Function CosmosConnect() As String

        If gmCosmosClient Is Nothing Then
            gmCosmosClient = New Microsoft.Azure.Cosmos.CosmosClient(cosmosEndpointUri, cosmosPrimaryKeyRO)
        End If
        If gmCosmosClient Is Nothing Then Return "Cannot create dbase client"

        If gmCosmosDatabase Is Nothing Then
            gmCosmosDatabase = gmCosmosClient.GetDatabase("PKARweb")
        End If

        If gmCosmosDatabase Is Nothing Then Return "Cannot connect to dbase"

        Return ""
    End Function


End Module


Public Class oneLogin
    Public Property id As String
    Public Property sUserName As String
    Public Property sLimitPath As String
    Public Property bAlsoPriv As Boolean
    Public Property bAlsoTajne As Boolean
End Class