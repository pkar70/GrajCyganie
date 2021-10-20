' The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

''' <summary>
''' An empty page that can be used on its own or navigated to within a Frame.
''' </summary>
Public NotInheritable Class SzukajPliku
    Inherits Page

    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        ProgRingInit(True, False)

        ProgRingShow(True)
        Dim sError As String = CosmosConnectStoreFiles()
        ProgRingShow(False)
        If sError <> "" Then
            DialogBox(sError)
            Return
        End If

        ProgRingShow(True)
        Dim iCount As Long = Await CosmosGetCountAsync("StoreFiles")
        Dim iTotalBytes As Long = Await CosmosGetSumAsync("StoreFiles", "len")
        uiStats.Text = "Global stat: " & iCount.ToStringWithSpaces & ", " & iTotalBytes.ToStringWithSpaces
        ProgRingShow(False)
    End Sub

    Private Async Sub uiSearch_Click(sender As Object, e As RoutedEventArgs)
        ' na razie bez SORT z pola uiSort

        Dim sName As String = uiName.Text
        Dim sPath As String = uiPath.Text

        If sName.Length < 3 Then sName = ""
        If sPath.Length < 3 Then sPath = ""

        If sName.Length + sPath.Length < 3 Then
            DialogBox("Cos jednak musisz wyszukiwać...")
            Return
        End If

        Dim sWhere1 As String = ConvertMask("name", uiName.Text)
        Dim sWhere2 As String = ConvertMask("path", uiPath.Text)
        Dim sWhere As String = sWhere1
        If sWhere2 <> "" Then
            If sWhere <> "" Then sWhere &= " AND "
            sWhere &= sWhere2
        End If

        Dim oCBI As ComboBoxItem = uiOrder.SelectedItem

        sWhere = "WHERE " & sWhere & " AND c.del=false ORDER BY c." & oCBI.Content.ToString

        ProgRingShow(True)
        Dim oLista As List(Of oneStoreFiles) = Await CosmosQueryFilesAsync(sWhere)
        ProgRingShow(False)

        uiList.ItemsSource = oLista
    End Sub

    Private Function GetGisWebPath(oItem As oneStoreFiles)

        Dim sPath As String = gmStorageUri
        sPath = sPath & oItem.path.Substring(3).Replace("\", "/")
        sPath = sPath & "/" & oItem.name
        sPath.Replace(" ", "%20")

        Return sPath
    End Function
    Private Sub uiCopyGisExpPath_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As oneStoreFiles = Sender2Item(sender)

        Dim sPath = oItem.path.Substring(2) & "\" & oItem.name
        sPath = GetMatrixPath(sPath) ' ma być od \
        ClipPut(sPath)
    End Sub

    Private Function Sender2Item(sender As Object) As oneStoreFiles
        Dim oMFI As MenuFlyoutItem = sender
        Return oMFI.DataContext
    End Function

    Private Sub uiCopyGisWebPath_Click(sender As Object, e As RoutedEventArgs)
        ClipPut(GetGisWebPath(Sender2Item(sender)))
    End Sub

    Private Sub uiOpenGisWeb_Click(sender As Object, e As RoutedEventArgs)
        OpenBrowser(GetGisWebPath(Sender2Item(sender)))
    End Sub

    Private Async Sub uiOpenGisExp_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As oneStoreFiles = Sender2Item(sender)

        If Not CheckRAIDavail() Then
            DialogBox("Sorry, bez Matrixa nie da rady!")
            Return
        End If

        Dim sPath = oItem.path.Substring(2) & "\"

        sPath = GetMatrixPath(sPath) ' ma być od \

        Dim oFold As Windows.Storage.StorageFolder
        Try
            oFold = Await Windows.Storage.StorageFolder.GetFolderFromPathAsync(sPath)
        Catch ex As Exception
            DialogBox("Cannot GetFolderFromPathAsync:" & vbCrLf & ex.Message)
            Return
        End Try

        OpenExplorer(oFold)
    End Sub

    Private Async Sub uiOpenGisFile_Click(sender As Object, e As RoutedEventArgs)
        Dim oItem As oneStoreFiles = Sender2Item(sender)

        If oItem.isDir Then
            DialogBox("to DIR")
            Return
        End If

        If Not CheckRAIDavail() Then
            DialogBox("Sorry, bez Matrixa nie da rady!")
            Return
        End If

        Dim sPath = oItem.path.Substring(2) & "\" & oItem.name
        sPath = GetMatrixPath(sPath) ' ma być od \
        Dim oFile As Windows.Storage.StorageFile
        Try
            oFile = Await Windows.Storage.StorageFile.GetFileFromPathAsync(sPath)
        Catch ex As Exception
            DialogBox("Cannot GetFileFromPathAsync:" & vbCrLf & ex.Message)
            Return
        End Try

        Windows.System.Launcher.LaunchFileAsync(oFile)
    End Sub
    Private Sub uiOpenLocalFolder_Click(sender As Object, e As RoutedEventArgs)
        ' czy jest plik... tylko jak to zrobić? d:\ ? wyszukiwanie po wszystkich dyskach?
        DialogBox("jeszcze nie umiem")
    End Sub

End Class
