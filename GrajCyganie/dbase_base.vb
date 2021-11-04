' 2021.10.27
' wszystko związane z bazą danych
' - wydzielone, by było łatwo zmieniać między Azure a SQL (i może jeszcze czymś)

' beskid
' local SQL
' Azure

Public MustInherit Class dbase_base

    Public MustOverride ReadOnly Property Nazwa As String

    ''' <summary>
    ''' bForce: wymuszenie przelogowania
    ''' </summary>
    Public Async Function Login(bForce As Boolean) As Task(Of Boolean)
        If Not bForce AndAlso mbGranted Then Return True
        Await GetPermission(GetSettingsString("userName"))
        If mbGranted Then Return True
        Return False
    End Function

    Public MustOverride Async Function GetPermission(sUser As String) As Task(Of String)

    Protected MustOverride Async Function DekadyDownload() As Task(Of Boolean)
    Public MustOverride Async Function GetMaxId() As Task(Of Boolean)

    Protected mbGranted As Boolean = False

    Public mlDekady As ObservableCollection(Of tDekada) = Nothing

    Public MustOverride Async Function GetCountsy(oGrany As tGranyUtwor) As Task(Of Boolean)
    Public MustOverride Async Function GetNextSong(iNextMode As Integer, oGrany As tGranyUtwor) As Task(Of Boolean)
    ' Dim sPage As String = Await App.HttpPageAsync("/cygan-info.asp?" & sParams, "file data")

    Private Async Function DekadyLoad() As Task(Of Boolean)
        Dim oFold As Windows.Storage.StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder
        Dim oFile As Windows.Storage.StorageFile = Await oFold.TryGetItemAsync("dekady.xml")
        If oFile Is Nothing Then Return False

        Dim oSer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(ObservableCollection(Of tDekada)))
        Dim oStream As Stream = Await oFile.OpenStreamForReadAsync
        Dim bError As Boolean = False
        Try
            mlDekady = TryCast(oSer.Deserialize(oStream), ObservableCollection(Of tDekada))
        Catch ex As Exception
            bError = True
        End Try
        oStream.Dispose()   ' == fclose
        If bError Then Return False

        If mlDekady.Count > 0 Then Return True
        Return False
    End Function

    Public Async Function DekadySave() As Task(Of Boolean)
        Dim oFold As Windows.Storage.StorageFolder = Windows.Storage.ApplicationData.Current.LocalFolder
        Dim oFile As Windows.Storage.StorageFile = Await oFold.CreateFileAsync("dekady.xml", Windows.Storage.CreationCollisionOption.ReplaceExisting)
        If oFile Is Nothing Then Return False

        Dim oSer As Xml.Serialization.XmlSerializer = New Xml.Serialization.XmlSerializer(GetType(ObservableCollection(Of tDekada)))
        Dim oStream As Stream = Await oFile.OpenStreamForWriteAsync
        oSer.Serialize(oStream, mlDekady)
        oStream.Dispose()   ' == fclose

        Return True
    End Function

    Public Async Function GetDekady(bForce As Boolean) As Task(Of Boolean)
        If Not Await Login(False) Then
            Await DialogBoxAsync("nie jesteś zalogowany")
            Return False
        End If

        If Not bForce AndAlso mlDekady IsNot Nothing Then Return True

        mlDekady = New ObservableCollection(Of tDekada)

        ' jesli ponad 30 dni minelo, to wymuś wczytanie dekad z sieci 
        If Math.Abs(GetSettingsInt("lastDekadyStat") - Date.Now.DayOfYear) > 30 Then bForce = True

        If Not bForce Then
            If Await DekadyLoad() Then Return True
        End If

        ' jesli nie z cache, to z serwera
        If Not Await DekadyDownload() Then Return False

        ' przetworz int na procenty
        For Each oItem As tDekada In mlDekady
            oItem.sFreq = App.FreqSlider2Text(oItem.iFreq)
        Next

        Await DekadySave()
        SetSettingsInt("lastDekadyStat", Date.Now.DayOfYear)

        Return True

    End Function

End Class
