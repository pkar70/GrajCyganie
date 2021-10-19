Partial Module cosmoDB

    Private gmCosmosClient As Microsoft.Azure.Cosmos.CosmosClient = Nothing
    Private gmCosmosDatabase As Microsoft.Azure.Cosmos.Database = Nothing
    Private mCosmosLoginContainer As Microsoft.Azure.Cosmos.Container = Nothing
    Private mCosmosFilesContainer As Microsoft.Azure.Cosmos.Container = Nothing

    Public Async Function CosmosQueryFilesAsync(sWhere As String, Optional iTop As Integer = 100) As Threading.Tasks.Task(Of List(Of oneFile))

        Dim oRet As New List(Of oneFile)
        Dim oMsg As New oneFile

        Dim sErr As String = CosmosConnectStoreFiles()
        If sErr <> "" Then
            oMsg.len = 0
            oMsg.name = "ERROR"
            oMsg.path = sErr
            oRet.Add(oMsg)
            Return oRet
        End If

        Dim sQry As String = "SELECT * FROM c " & sWhere & " OFFSET 0 LIMIT " & iTop

        'oMsg.len = 0
        'oMsg.name = "QUERY"
        'oMsg.path = sQry
        'oRet.Add(oMsg)

        Dim oIterator As Microsoft.Azure.Cosmos.FeedIterator(Of oneFile) =
                mCosmosFilesContainer.GetItemQueryIterator(Of oneFile)(sQry)

        While oIterator.HasMoreResults

            Dim currentResultSet As Microsoft.Azure.Cosmos.FeedResponse(Of oneFile) = Await oIterator.ReadNextAsync()

            'Dim oMsg1 As New oneFile
            'oMsg1.len = 0
            'oMsg1.name = "HasMoreResults"
            'oMsg1.path = currentResultSet.Count
            'oRet.Add(oMsg1)

            For Each oItem As oneFile In currentResultSet
                oRet.Add(oItem)
            Next

        End While

        oIterator.Dispose()

        Return oRet

    End Function

    Public Function CosmosGetTableContainer(sTable As String) As Microsoft.Azure.Cosmos.Container
        Select Case sTable.ToLower
            Case "storefiles"
                Return mCosmosFilesContainer
            Case "loginsy"
                Return mCosmosLoginContainer
        End Select

        Return Nothing
    End Function


    Public Async Function CosmosGetCountAsync(sTable As String, Optional sWhere As String = "") As Threading.Tasks.Task(Of Long)
        Dim sQry As String = "SELECT VALUE COUNT(1) FROM c"
        If sWhere <> "" Then sQry = sQry & " WHERE " & sWhere

        Dim oCont As Microsoft.Azure.Cosmos.Container = CosmosGetTableContainer(sTable)
        If oCont Is Nothing Then Return -1

        Dim sqlQueryDef As New Microsoft.Azure.Cosmos.QueryDefinition(sQry)
        Dim oIterator As Microsoft.Azure.Cosmos.FeedIterator(Of Long) = oCont.GetItemQueryIterator(Of Long)(sqlQueryDef)
        While oIterator.HasMoreResults
            Dim currentResultSet As Microsoft.Azure.Cosmos.FeedResponse(Of Long) = Await oIterator.ReadNextAsync()
            For Each oItem As Integer In currentResultSet
                Return oItem
            Next
        End While

        Return -2    ' brak danych?

    End Function


    Public Async Function CosmosGetSumAsync(sTable As String, sField As String, Optional sWhere As String = "") As Threading.Tasks.Task(Of Long)
        Dim sQry As String = "SELECT VALUE SUM(c." & sField & ") FROM c"
        If sWhere <> "" Then sQry = sQry & " WHERE " & sWhere

        Dim oCont As Microsoft.Azure.Cosmos.Container = CosmosGetTableContainer(sTable)
        If oCont Is Nothing Then Return -1

        Dim sqlQueryDef As New Microsoft.Azure.Cosmos.QueryDefinition(sQry)
        Dim oIterator As Microsoft.Azure.Cosmos.FeedIterator(Of Long) = oCont.GetItemQueryIterator(Of Long)(sqlQueryDef)
        While oIterator.HasMoreResults
            Dim currentResultSet As Microsoft.Azure.Cosmos.FeedResponse(Of Long) = Await oIterator.ReadNextAsync()
            For Each oItem As Integer In currentResultSet
                Return oItem
            Next
        End While

        Return -2    ' brak danych?

    End Function


    Public Function CosmosConnectLoginsy() As String
        If gmCosmosDatabase Is Nothing Then Return "Cannot connect to dbase"

        If mCosmosLoginContainer Is Nothing Then
            mCosmosLoginContainer = gmCosmosDatabase.GetContainer("loginsy")
        End If

        If mCosmosLoginContainer Is Nothing Then Return "cannot get container"

        Return ""

    End Function
    Public Function CosmosConnectStoreFiles() As String
        If gmCosmosDatabase Is Nothing Then Return "Cannot connect to dbase"

        If mCosmosFilesContainer Is Nothing Then
            mCosmosFilesContainer = gmCosmosDatabase.GetContainer("StoreFiles")
        End If

        If mCosmosFilesContainer Is Nothing Then Return "cannot get container"

        Return ""

    End Function

    Public Function CosmosConnect() As String

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

    Public Function ConvertMask(sFieldName As String, sMask As String) As String

        If String.IsNullOrEmpty(sMask) Then Return ""

        ' 1) kontrola czy mamy jakieś maski
        Dim bMask As Boolean = 0
        If sMask.Contains("?") Then bMask = True
        If sMask.Contains("%") Then bMask = True
        If sMask.Contains("*") Then bMask = True
        If sMask.Contains("$") Then bMask = True
        If sMask.Contains("^") Then bMask = True

        ' 2) jesli nie mamy maski, to robimy zwykly contains (w SQL: like %xx%)
        If Not bMask Then
            sMask.Replace("'", "''")
            Return " CONTAINS(c." & sFieldName & ", '" & sMask & "',true) "
        End If

        ' 3) jest maska
        ' można byłoby sprawdzić ENDSWITH, STARTSWITH, ale idziemy regexpem
        sMask = sMask.Replace("%", "*")
        sMask = sMask.Replace(".", "\.")
        sMask = sMask.Replace("?", ".")

        Return " REGEXMATCH(c." & sFieldName & ", '" & sMask & "','i') "
    End Function

End Module


