' 2021.10.27
' wszystko związane z bazą danych
' - wydzielone, by było łatwo zmieniać między Azure a SQL (i może jeszcze czymś)

' beskid
' local SQL
' Azure

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

End Class
