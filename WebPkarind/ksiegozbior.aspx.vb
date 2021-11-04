Public Class ksiegozbior
    Inherits System.Web.UI.Page

    Protected Async Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        loggedOK.Visible = False
        loggedError.Visible = False
        askForLogin.Visible = False

        Dim sErrorMsg As String = Await CheckLoginAsync(Me)

        If Me.Session("loggedUser") <> "" Then
            loggedOK.Visible = True
            Await QueryCosmosDatabase
        Else
            If sErrorMsg.ToLower.Contains("no login provided") Then
                askForLogin.Visible = True
            Else
                loggedError.Visible = True
                loginMessage.InnerText = sErrorMsg
            End If
        End If

    End Sub

    Private Function GetWhereClause() As String
        Dim sQry As String = ""
        Dim sTmp As String = ""

        If Len(Request("name")) > 3 Then
            sQry = ConvertMask("name", Request("name"))
        End If

        If Len(Request("path")) > 1 Then
            If sQry <> "" Then sQry = sQry & " AND "
            sQry = ConvertMask("path", Request("path"))
        End If

        If sQry = "" Then Return "" ' nie ma Query

        If Me.Session("sLimitPath") <> "" Then
            sQry = sQry & " AND CONTAINS(c.path,'" & Me.Session("sLimitPath") & "',true)"
        End If

        ' contains("\text") dawało także w Public\Public.xxx\Texts, ale było wolniejsze - bo starts jest szybsze.
        Return " WHERE " & sQry & " AND STARTSWITH(c.path,'u:\Public\MyProduction\Texts',true) "

    End Function

    Private Async Function QueryCosmosDatabase() As Threading.Tasks.Task
        If Me.Session("loggedUser") = "" Then Return

        ' no nie, bo limit moze byc \_3bib-rel , przeciez nie moge podac pełnej sciezki (z Texts) do tego, bo po drodze jest numerek
        'If Me.Session("sLimitPath") <> "" AndAlso (Not Me.Session("sLimitPath").ToString.ToLower.Contains("\texts.")) Then Return

        Dim sWhere As String = GetWhereClause()
        If sWhere = "" Then Return  ' nie ma pytania, więc tylko formatkę pokazuje
        sWhere = sWhere & " AND c.del=false "

        sWhere = sWhere & " ORDER BY c." & Request("ordering")

        ' usun istniejące, zostawiając 3 sztuki
        For i = uiResults.Rows.Count To 4 Step -1
            uiResults.Rows.Remove(uiResults.Rows.Item(i))
        Next

        Dim oList As List(Of oneStoreFiles) = Await CosmosQueryFilesAsync(sWhere, 100)
        ' Response.Write "<tr><td colspan=4><small>Running query: " & sQry & "</small></tr>"

        'Dim oMsg As New oneFile
        'oMsg.len = 0
        'oMsg.name = "COUNT"
        'oMsg.path = oList.Count.ToString
        'oList.Add(oMsg)

        For Each oItem As oneStoreFiles In oList
            Dim oNewRow As HtmlTableRow =
                StorageFileToHtmlRow(oItem, Me.Session("bNoLinks"), "\Public\MyProduction\Texts\")
            uiResults.Rows.Add(oNewRow)
        Next

        If oList.Count > 99 Then
            Dim oNewRow As New HtmlTableRow
            Dim oNewCell = New HtmlTableCell()
            oNewCell.InnerHtml = "<tr><td><font color='red'>(i tak dalej)</font><td><td><td></tr>"
            oNewRow.Cells.Add(oNewCell)
        End If

        ' to za duzo czasu zajmuje
        ' 6175.79 RUs , co oznacza prawie 10 sekund; jednak StoreFiles jest olbrzymie, >1.5 mln rekordów
        ' uiTotal.InnerText = Int2BigString(Await CosmosGetCountAsync("StoreFiles", "CONTAINS(c.path,'\Texts',true)"))
        '  Set oRs = objConn.Execute("SELECT SUM(len) FROM StoreFiles WHERE del=0" & sLimit)

    End Function

End Class

