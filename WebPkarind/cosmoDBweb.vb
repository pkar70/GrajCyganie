Partial Module cosmoDB
    Public Async Function CheckLoginAsync(oPage As Page) As Threading.Tasks.Task(Of String)

        If oPage.Session("loginLock") > 3 Then Return "ERROR: session lockout"

        Dim sLoginName As String = oPage.Request("user")
        If sLoginName <> "" Then
            ' user override
            oPage.Session("loggedUser") = ""
            oPage.Session("sLimitPath") = "NAPEWNONICTAKIEGONIEMA"
            oPage.Session("bAlsoPriv") = False
            oPage.Session("bAlsoTajne") = False
            oPage.Session("bNoLinks") = False
        End If

        If oPage.Session("loggedUser") <> "" Then
            oPage.Session("loginLock") = 0
            Return ""
        End If

        If sLoginName = "" Then
            Dim oReqCookie As HttpCookie = oPage.Request?.Cookies?.Get("cyganUserName")
            If oReqCookie IsNot Nothing Then sLoginName = oReqCookie.Value
        End If

        If sLoginName = "" Then sLoginName = oPage.Request("user")

        If sLoginName = "" Then Return "ERROR: no login provided"

        Dim lastLoginTry As String = oPage.Application("lastLoginTry")
        Dim currLoginTry As String = DateTime.Now.ToString("MMddHHmmss")
        If lastLoginTry = currLoginTry Then Return "ERROR: too many login requests"

        oPage.Application("lastLoginTry") = currLoginTry

        Dim sCosmosError As String = CosmosConnect()
        If sCosmosError <> "" Then Return sCosmosError

        sCosmosError = CosmosConnectLoginsy()
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
                oPage.Session("bNoLinks") = oItem.bNoLinks
                Return True
            Next
        End While

        Return False
    End Function
End Module
