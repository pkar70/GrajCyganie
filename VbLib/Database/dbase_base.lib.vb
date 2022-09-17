' 2021.10.27
' wszystko związane z bazą danych
' - wydzielone, by było łatwo zmieniać między Azure a SQL (i może jeszcze czymś)

' beskid
' local SQL
' Azure

Imports System.ComponentModel
Imports System.Linq.Expressions

Public MustInherit Class dbase_base

    Public MustOverride ReadOnly Property Nazwa As String

    Protected mbGranted As Boolean = False

    ''' <summary>
    ''' bForce: wymuszenie przelogowania
    ''' </summary>
    Public Async Function LoginAsync(bForce As Boolean) As Task(Of Boolean)
        DumpCurrMethod()

        If Not bForce AndAlso mbGranted Then Return True
        Await GetPermissionAsync(GetSettingsString("userName"))
        If mbGranted Then Return True
        Return False
    End Function

    Public MustOverride Async Function GetPermissionAsync(sUser As String) As Task(Of String)

    Protected MustOverride Async Function DekadyDownloadAsync() As Task(Of List(Of tDekada))

    Protected MustOverride Async Function RetrieveMaxIdAsync() As Task(Of Integer)

    Public MustOverride Async Function RetrieveCountsyAsync(oGrany As tGranyUtwor) As Task(Of Boolean)

    Public MustOverride Async Function GetNextSongAsync(iNextMode As eNextMode, oGrany As tGranyUtwor) As Task(Of tGranyUtwor)

    Public MustOverride Async Function SearchAsync(sArtist As String, sTitle As String, sAlbum As String, sRok As String) As Task(Of List(Of oneAudioParam))

    Public MustOverride Async Function GetStoreFileAsync(id As Integer) As Task(Of oneStoreFile)
    Public MustOverride Async Function GetStorageItemsAsync(sPath As String) As Task(Of List(Of oneStoreFile))

    Public MustOverride Async Function GetDirSizeAsync(id As Integer) As Task(Of Long)

    Protected MustOverride Async Function GetModelsSummaryMainAsync(sModel As String) As Task(Of List(Of oneModelSummmary))

    ' Dim sPage As String = Await App.HttpPageAsync("/cygan-info.asp?" & sParams, "file data")

    ''' <summary>
    ''' ustawia SettingsInt("maxSoundId") na liczbę utworów w bazie
    ''' </summary>
    ''' <returns></returns>
    Public Async Function GetMaxIdAsync() As Task(Of Boolean)
        Dim iMaxId As Integer = Await RetrieveMaxIdAsync()
        If iMaxId < 1 Then Return False
        SetSettingsInt("maxSoundId", iMaxId)
        Return True
    End Function


    Public Async Function ReloadDekadyAsync(bForce As Boolean) As Task(Of Boolean)

        If Not Await LoginAsync(False) Then
            Await DialogBoxAsync("nie jesteś zalogowany")
            Return False
        End If

        If App._dekady.IsObsolete(30) Then bForce = True

        If Not bForce AndAlso App._dekady.Count > 0 Then Return True


        Dim oLista As List(Of tDekada) = Await DekadyDownloadAsync()
        If oLista.Count < 2 Then Return False

        App._dekady.ImportNew(oLista)

        If Not Await GetMaxIdAsync() Then Return False

        Return True

    End Function

    Public Async Function GetModelsSummaryAsync(sModelName As String) As Task(Of List(Of oneModelSummmary))
        sModelName = sModelName.Replace(" ", "")

        'Dim iLen As Integer = sModelName.Length
        'If sModelName.StartsWith("^") Then iLen -= 1
        'If sModelName.EndsWith("$") Then iLen -= 1
        'If iLen < 3 Then Return Nothing


        'sModelName = Vblib.dbase_baseASP.ConvertQueryParam(sModelName)
        'sModelName = sModelName.Replace("%", "*")
        Dim lista As List(Of oneModelSummmary) = Await GetModelsSummaryMainAsync(sModelName)


        Dim listaSorted As New List(Of oneModelSummmary)
        '         ' AnnaAberg	2	367
        ' AnnaAberg\Models.001	2	78764
        ' AnnaJagodzinska	9	979
        ' AnnaJagodzinska\Models.002	10	3655712
        ' AnnaJagodzinska\Models.004	104	15626125
        ' AnnaJagodzinska\Models.004\0bw	15	2502598

        Dim sLastName As String = ""
        Dim oNew As oneModelSummmary = Nothing
        For Each oItem As oneModelSummmary In lista
            sModelName = oItem.modelDir
            Dim iInd As Integer = sModelName.IndexOf("\")
            If iInd > 1 Then sModelName = sModelName.Substring(0, iInd)
            If sModelName <> sLastName Then
                If oNew IsNot Nothing Then listaSorted.Add(oNew)
                oNew = New oneModelSummmary
                oNew.modelDir = sModelName
                sLastName = sModelName
            End If

            oNew.items += oItem.items
            oNew.total += oItem.total
        Next

        Return listaSorted
    End Function
End Class
