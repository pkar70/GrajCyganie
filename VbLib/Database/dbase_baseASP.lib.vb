﻿

Partial Public MustInherit Class dbase_baseASP
    Inherits dbase_base

    Public MustOverride Overrides ReadOnly Property Nazwa As String

    Protected MustOverride ReadOnly Property BaseUri As String

    Public Overrides Async Function GetPermissionAsync(sUser As String) As Task(Of String)
        DumpCurrMethod()

        mbGranted = False
        If sUser = "" Then Return "Zaloguj się"

        Dim sRes As String = Await ThisHttpPageAsync("/cygan-login.asp?user=" & sUser, "login page")
        If sRes.IndexOf("Masz prawo") > -1 Then mbGranted = True

        Return sRes
    End Function

    Protected Overrides Async Function DekadyDownloadAsync() As Task(Of List(Of tDekada))
        DumpCurrMethod()

        Dim sRes As String = Await ThisHttpPageAsync("/cygan-dekady.asp", "statystyka dekad")

        Dim mlDekady As New List(Of tDekada)

        Dim aDekady As String() = sRes.Split(vbCrLf)
        Dim oNew As tDekada

        For Each sDekada In aDekady
            Dim aFields As String() = sDekada.Split("|")
            If aFields.GetUpperBound(0) = 2 Then
                oNew = New tDekada
                oNew.sNazwa = aFields(0).Trim
                oNew.iCount = aFields(1)
                oNew.iPlayTimeSecs = aFields(2)
                oNew.iFreq = 3
                mlDekady.Add(oNew)
            End If
        Next

        If mlDekady.Count < 1 Then Return mlDekady

        Return mlDekady
    End Function

    Protected Overrides Async Function RetrieveMaxIdAsync() As Task(Of Integer)
        DumpCurrMethod()

        Dim sRes As String = Await ThisHttpPageAsync("/api-maxId.asp", "maxno")
        If sRes = "" Then Return -1
        Return sRes.Trim
    End Function

    Public Overrides Async Function RetrieveCountsyAsync(oGrany As tGranyUtwor) As Task(Of Boolean)
        DumpCurrMethod()

        Dim sPage As String = Await ThisHttpPageAsync("/cygan-counts.asp?id=" & oGrany.oAudioParam.id, "song stats")
        If String.IsNullOrEmpty(sPage) Then Return False

        ExtractCyganCounts(sPage, oGrany) ' uzupełnia count* w oGrany
        Return True
    End Function

    Private Sub ExtractCyganCounts(sPage As String, oGrany As tGranyUtwor)
        DumpCurrMethod()

        Dim aArr As String() = sPage.Split(vbCrLf)
        If aArr.GetUpperBound(0) < 2 Then Return    ' jakiś błąd, ale niby jaki?

        For i As Integer = 0 To aArr.GetUpperBound(0)
            If Not aArr(i).Trim.StartsWith("COUNT") Then Continue For

            Dim aFld As String() = aArr(i).Split("|")
            If aFld.GetUpperBound(0) = 2 Then
                Select Case aFld(1)
                    Case "artist"
                        oGrany.countArtist = aFld(2).Trim
                    Case "title"
                        oGrany.countTitle = aFld(2).Trim
                    Case "album"
                        oGrany.countAlbum = aFld(2).Trim
                    Case "year"
                        oGrany.countYear = aFld(2).Trim
                    Case "dekada"
                        oGrany.countDekada = aFld(2).Trim
                End Select
            End If
        Next

    End Sub

#If True Then
    Public Overrides Async Function GetNextSongAsync(iNextMode As eNextMode, oGrany As tGranyUtwor) As Task(Of tGranyUtwor)
        DumpCurrMethod("miNextMode=" & iNextMode)

        Try
            Dim iCurrId As Integer = 0
            Dim sParams As String = ""

            If oGrany IsNot Nothing Then iCurrId = oGrany.oAudioParam.id
            Select Case iNextMode
                Case eNextMode.random
                    iCurrId = GetSettingsInt("maxSoundId")
                    If iCurrId = 0 Then Return Nothing
                    iCurrId = App.MakeRandom(iCurrId)
                    ' random
                    sParams = "id=" & iCurrId & "&mode=random"
                Case eNextMode.sameArtist
                    sParams = "id=" & iCurrId & "&mode=artist"
                Case eNextMode.sameTitle
                    sParams = "id=" & iCurrId & "&mode=title"
                Case eNextMode.sameAlbum
                    sParams = "id=" & iCurrId & "&mode=album"
                Case eNextMode.sameRok
                    sParams = "id=" & iCurrId & "&mode=rok"
                Case eNextMode.sameDekada
                    sParams = "id=" & iCurrId & "&mode=dekada"
            End Select

            ' dane o nastepnym HttpPage -> App.mtGranyUtwor
            Dim sPage As String = Await ThisHttpPageAsync("/api-info.asp?" & sParams, "file data")
            If sPage = "" Then
                Await DialogBoxAsync("ERROR get cygan-info empty, " & sParams)
                Return Nothing
            End If

            If sPage.StartsWith("[") Then sPage = sPage.Substring(1)
            If sPage.EndsWith("]") Then sPage = sPage.Substring(0, sPage.Length - 1)
            Dim oRet As New tGranyUtwor
            oRet.oAudioParam = Newtonsoft.Json.JsonConvert.DeserializeObject(sPage, GetType(oneAudioParam))
            oRet.oStoreFile = Newtonsoft.Json.JsonConvert.DeserializeObject(sPage, GetType(oneStoreFile))

            Return oRet

        Catch ex As Exception
            CrashMessageAdd("@GetNextSong", ex)
            DialogBox("FAIL: " & ex.Message)
        End Try

        Await DialogBoxAsync("exception caught")
        Return Nothing
    End Function
#Else
    Public Overrides Async Function GetNextSongAsync(iNextMode As eNextMode, oGrany As tGranyUtwor) As Task(Of tGranyUtwor)
        DumpCurrMethod("miNextMode=" & iNextMode)

        Try
            Dim iCurrId As Integer = 0
            Dim sParams As String = ""

            If oGrany IsNot Nothing Then iCurrId = oGrany.oAudioParam.id
            Select Case iNextMode
                Case eNextMode.random
                    iCurrId = GetSettingsInt("maxSoundId")
                    If iCurrId = 0 Then Return Nothing
                    iCurrId = App.MakeRandom(iCurrId)
                    ' random
                    sParams = "id=" & iCurrId & "&mode=random"
                Case eNextMode.sameArtist
                    sParams = "id=" & iCurrId & "&mode=artist"
                Case eNextMode.sameTitle
                    sParams = "id=" & iCurrId & "&mode=title"
                Case eNextMode.sameAlbum
                    sParams = "id=" & iCurrId & "&mode=album"
                Case eNextMode.sameRok
                    sParams = "id=" & iCurrId & "&mode=rok"
                Case eNextMode.sameDekada
                    sParams = "id=" & iCurrId & "&mode=dekada"
            End Select

            ' dane o nastepnym HttpPage -> App.mtGranyUtwor
            Dim sPage As String = Await ThisHttpPageAsync("/cygan-info.asp?" & sParams, "file data")
            If sPage = "" Then
                Await DialogBoxAsync("ERROR get cygan-info empty, " & sParams)
                Return Nothing
            End If
            Dim aArr As String() = sPage.Split(vbCrLf)
            If aArr.GetUpperBound(0) < 2 Then
                Await DialogBoxAsync("ERROR get cygan-info too short, " & vbCrLf &
                    "Request: " & "/*-info.asp?" & sParams & vbCrLf &
                    "Returned: " & sPage)
                Return Nothing
            End If
            If aArr(0).Trim <> "OK" Then
                ' moze wygasła sesja, to powtarzamy
                Await LoginAsync(True)
                sPage = Await ThisHttpPageAsync("/cygan-info.asp?" & sParams, "file data")
                If sPage = "" Then
                    Await DialogBoxAsync("ERROR retry get cygan-info empty")
                    Return Nothing
                End If

                aArr = sPage.Split(vbCrLf)
                If aArr.GetUpperBound(0) < 2 Then
                    Await DialogBoxAsync("ERROR retry get cygan-info too short, " & vbCrLf &
                    "Request: " & "/*-info.asp?" & sParams & vbCrLf &
                    "Returned: " & sPage)
                    Return Nothing
                End If

                If aArr(0).Trim <> "OK" Then
                    Await DialogBoxAsync("ERROR get cygan-info not OK, aArr(0): " & aArr(0))
                    Return Nothing
                End If
            End If

            Return Await ExtractCyganInfo(aArr)

        Catch ex As Exception
            CrashMessageAdd("@GetNextSong", ex)
            DialogBox("FAIL: " & ex.Message)
        End Try

        Await DialogBoxAsync("exception caught")
        Return Nothing
    End Function
#End If

    Private Async Function ExtractCyganInfo(aArr As String()) As Task(Of tGranyUtwor)
        DumpCurrMethod()

        Dim oNew As New tGranyUtwor

        ' base64 z linku
        Dim sTxt As String = aArr(1).Trim
        If sTxt.Length Mod 4 > 0 Then sTxt = sTxt & "=" ' dodaj padding
        If sTxt.Length Mod 4 > 0 Then sTxt = sTxt & "="
        If sTxt.Length Mod 4 > 0 Then sTxt = sTxt & "="

        Dim oBuff As Byte() = Nothing
        Dim bError As Boolean = False
        Try
            oBuff = System.Convert.FromBase64String(sTxt)
        Catch ex As Exception
            bError = True
        End Try

        If bError Then
            Await DialogBoxAsync("dodaje minus")
            sTxt = sTxt & "="
            bError = False
            Try
                oBuff = System.Convert.FromBase64String(sTxt)
            Catch ex As Exception
                bError = True
            End Try
        End If

        If bError Then
            Await DialogBoxAsync("dodaje minus2")
            sTxt = sTxt & "="
            bError = False
            Try
                oBuff = System.Convert.FromBase64String(sTxt)
            Catch ex As Exception
                bError = True
            End Try
        End If

        If bError Then Return Nothing

        sTxt = System.Text.Encoding.ASCII.GetString(oBuff)
        ' "U:\Public\MyProduction\OldMusic\DVD\OldMusic.001\OldMusic\70\AlRobertsJr"
        ' "/store/Public/MyProduction/OldMusic/DVD/OldMusic.014/BeeGees/AKickInTheHeadIsWorhtEightInThePants/BeeGees-AKITHIWEITP-03-HomeAgainRivers.Mp3"
        oNew.oStoreFile.path = sTxt.Substring(7)
        oNew.oStoreFile.name = ""

        For i As Integer = 2 To aArr.GetUpperBound(0)
            Dim aFld As String() = aArr(i).Split("|")
            If aFld.GetUpperBound(0) = 2 Then
                Select Case aFld(1)
                    Case "ID"
                        oNew.oAudioParam.id = aFld(2).Trim
                    Case "Artist"
                        oNew.oAudioParam.artist = aFld(2).Trim
                    Case "Title"
                        oNew.oAudioParam.title = aFld(2).Trim
                    Case "Album"
                        oNew.oAudioParam.album = aFld(2).Trim
                    Case "Rok"
                        oNew.oAudioParam.year = aFld(2).Trim
                    Case "Dekada"
                        oNew.oAudioParam.dekada = aFld(2).Trim
                    Case "Track"
                        oNew.oAudioParam.track = aFld(2).Trim
                    Case "Bitrate"
                        oNew.oAudioParam.bitrate = aFld(2).Trim
                    Case "Duration"
                        oNew.oAudioParam.duration = aFld(2).Trim
                    Case "Channels"
                        oNew.oAudioParam.channels = aFld(2).Trim
                    Case "Sample"
                        oNew.oAudioParam.sample = aFld(2).Trim
                    Case "fSize"
                        oNew.oStoreFile.len = aFld(2).Trim
                    Case Else   ' Comment
                        oNew.oAudioParam.comment &= aFld(2).Trim
                End Select

            End If
            '|Artist|The Beatles |Title|Till There Was You |Album|Anthology 1 (disc 2) |Rok|1995 
            '|Dekada|199x |Track| |Bitrate|192 |Duration|174 |channels|Stereo |sample|44100 |fsize|4183064 |Comment|RiBor 
        Next

        Return oNew

    End Function

    Public Async Function ThisHttpPageAsync(sUrl As String, sErrMsg As String, Optional sData As String = "") As Task(Of String)
        DumpCurrMethod()

        HttpPageSetAgent("GrajCyganie")
        If sUrl.Substring(0, 4) <> "http" Then sUrl = BaseUri() & "/dysk" & sUrl

        Dim sPage As String = Await HttpPageAsync(sUrl, sData)
        If sPage = "" Then
            Await DialogBoxAsync(sErrMsg)
        End If
        If sPage.Contains("Brak uprawnien, co tu robisz") Then
            ' czyżby na tym polegał minus?
            Await DialogBoxAsync(sPage)
        End If

        Return sPage

    End Function

    Public Shared Function ConvertQueryParam(sMask As String) As String
        If String.IsNullOrWhiteSpace(sMask) Then Return ""
        sMask = sMask.Replace("'", "''")
        sMask = sMask.Replace("*", "%")
        sMask = sMask.Replace("?", "_")
        If sMask.Contains("%") OrElse sMask.Contains("_") Then Return sMask

        If Not sMask.StartsWith("^") Then
            sMask = "%" & sMask
        Else
            sMask = sMask.Substring(1)
        End If

        If Not sMask.EndsWith("$") Then
            sMask = sMask & "%"
        Else
            sMask = sMask.Substring(0, sMask.Length - 1)
        End If

        Return sMask
    End Function

    Public Overrides Async Function SearchMusicAsync(sArtist As String, sTitle As String, sAlbum As String, sRok As String) As Task(Of List(Of oneAudioParam))
        If (sArtist & sTitle & sAlbum & sRok).Length < 3 Then Return Nothing

        sArtist = ConvertQueryParam(sArtist).Replace("&", "%26")
        sTitle = ConvertQueryParam(sTitle).Replace("&", "%26")
        sAlbum = ConvertQueryParam(sAlbum).Replace("&", "%26")
        sRok = ConvertQueryParam(sRok)

        Dim sLinkQuery As String = $"artist={sArtist}&title={sTitle}&album={sAlbum}&rok={sRok}"
        sLinkQuery = sLinkQuery.Replace("%", "%25")

        Dim sPage As String = Await ThisHttpPageAsync("/api-search.asp?" & sLinkQuery, "file data")
        If sPage = "" Then Return Nothing

        Dim oLista As New List(Of oneAudioParam)
        oLista = Newtonsoft.Json.JsonConvert.DeserializeObject(sPage, GetType(List(Of oneAudioParam)))
        Return oLista

    End Function

    Public Overrides Async Function GetStoreFileAsync(id As Integer) As Task(Of oneStoreFile)
        DumpCurrMethod()

        Dim sPage As String = Await ThisHttpPageAsync("/api-getfile.asp?id=" & id, "file data")
        If sPage = "" Then Return Nothing

        Dim oItem As oneStoreFile
        oItem = Newtonsoft.Json.JsonConvert.DeserializeObject(sPage, GetType(oneStoreFile))
        Return oItem

    End Function

    Public Overrides Async Function GetStorageItemsAsync(sPath As String) As Task(Of List(Of oneStoreFile))
        DumpCurrMethod()

        sPath = sPath.Replace(" ", "%25") ' pewnie można lepiej to zrobić
        Dim sPage As String = Await ThisHttpPageAsync("/api-getdir.asp?path=" & sPath, "file data")
        If sPage = "" Then Return Nothing

        Dim oLista As List(Of oneStoreFile)
        oLista = Newtonsoft.Json.JsonConvert.DeserializeObject(sPage, GetType(List(Of oneStoreFile)))
        Return oLista

    End Function

    Public Overrides Async Function GetDirSizeAsync(id As Integer) As Task(Of Long)
        DumpCurrMethod(id)
        Dim sPage As String = Await ThisHttpPageAsync("/api-getdirsize.asp?id=" & id, "dirsize")
        If sPage = "" Then Return -1

        Dim retval As Long = -1
        If Not Long.TryParse(sPage, retval) Then Return -1

        Return retval
    End Function

    Protected Overrides Async Function GetModelsSummaryMainAsync(sModel As String) As Task(Of List(Of oneModelSummmary))
        DumpCurrMethod(sModel)
        Dim sPage As String = Await ThisHttpPageAsync("/api-srchmodel.asp?model=" & sModel, "modelgroup")
        If sPage = "" Then Return Nothing

        Dim oLista As List(Of oneModelSummmary)
        oLista = Newtonsoft.Json.JsonConvert.DeserializeObject(sPage, GetType(List(Of oneModelSummmary)))
        Return oLista

    End Function

    Public Overrides Async Function GetMusicAlbums(sArtist As String) As Task(Of List(Of oneAlbumForArtist))
        DumpCurrMethod(sArtist)
        Dim sPage As String = Await ThisHttpPageAsync("/api-musicalbums.asp?artist=" & sArtist, "artistalbum")
        If sPage = "" Then Return Nothing

        Dim oLista As List(Of oneAlbumForArtist)
        oLista = Newtonsoft.Json.JsonConvert.DeserializeObject(sPage, GetType(List(Of oneAlbumForArtist)))
        Return oLista

    End Function

    Public Overrides Async Function SearchFilesAsync(sPathMask As String, sFileMask As String) As Task(Of List(Of oneStoreFile))
        DumpCurrMethod(sPathMask & ", " & sFileMask)
        Dim sPage As String = Await ThisHttpPageAsync("/api-searchfiles.asp?path=" & sPathMask & "&name=" & sFileMask, "searchfiles")
        If sPage = "" Then Return Nothing

        Dim oLista As List(Of oneStoreFile)
        oLista = Newtonsoft.Json.JsonConvert.DeserializeObject(sPage, GetType(List(Of oneStoreFile)))
        Return oLista

    End Function

End Class
