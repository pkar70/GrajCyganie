
Public NotInheritable Class Filmy
    Inherits Page

    Private Async Sub Page_Loaded(sender As Object, e As RoutedEventArgs)
        'Total 10484 files, total playing time: 303d 16:18:44
        ProgRingInit(True, False)

        ProgRingShow(True)
        Dim sError As String = CosmosConnectVideoParam()
        ProgRingShow(False)
        If sError <> "" Then
            DialogBox(sError)
            Return
        End If

        ProgRingShow(True)
        If gCountFiles = 0 Then
            gCountFilmy = Await CosmosGetCountAsync("videoParam")
            gSumFilmy = Await CosmosGetSumAsync("videoParam", "duration")
        End If
        uiStats.Text = "Total: " & gCountFilmy & " files, total playing time: " & gSumFiles.ToStringDHMS
        ProgRingShow(False)


    End Sub

    Private Async Sub uiSearch_Click(sender As Object, e As RoutedEventArgs)
        Dim sName As String = uiName.Text
        If sName.Length < 3 Then
            DialogBox("Cos jednak musisz wyszukiwać...")
            Return
        End If

        Dim sWhere As String = ConvertMask("name", uiName.Text) & Combo2Where()

        sWhere = "WHERE " & sWhere & " AND c.del=false"

        ProgRingShow(True)
        Dim oLista As List(Of oneStoreFiles) = Await CosmosQueryFilesAsync(sWhere)
        ProgRingShow(False)

        ' ale to jest uproszczenie, tylko nazwa, katalog i bytes-len; bez linku do tt (z nazwy pliku bądź katalogu), bez info, bez odwołania do IMDB, i bez prznosin do aktorów
        uiList.ItemsSource = oLista

    End Sub

    Private Function Combo2Where() As String
        Dim oCBI As ComboBoxItem = uiPath.SelectedItem
        Dim sTxt As String = oCBI.Content.ToString
        If sTxt = "wszystko" Then Return ""

        Return " AND CONTAINS(c.path, '" & sTxt.Trim & ", true) "
    End Function
End Class
